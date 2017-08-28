using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityOSC;

/// <summary>
/// The class handling one controller (one hand)
/// It start and stop control modes, which communicate with the drone TrajectoryManager
/// </summary>
public class Controller : MonoBehaviour {

    [SerializeField] private Text debugText;
    [SerializeField] private string specktrOscTopic = "/specktr/{channel}/{message_type}";

    private OscManager _oscManager;
    private DronesManager _dronesManager;

    #region head drone attributes
    [SerializeField] private int _headDroneID = -1;
    private GameObject _headDrone;
    #endregion

    #region hand drone attributes
    [SerializeField] private int _handDroneID = -1;
    private GameObject _handDrone;
    #endregion

    #region computed inputs attributes
    public Vector3 position = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;

    public Vector3 pointingVector; // the vector between the shoulders and the hand

    private LinkedList<MathUtils.Vec3Stamped> _lastPositions = new LinkedList<MathUtils.Vec3Stamped>();
    private LinkedList<MathUtils.Vec3Stamped> _lastVelocities = new LinkedList<MathUtils.Vec3Stamped>();
    #endregion

    #region drone selection attributes
    private List<int> _dronesIds;
    private int _currentDroneIdx = 0;
    private string[] _droneSelectionInputs = new string[] { "palm+2", "palm+3", "palm+4", "palm+5" };
    #endregion

    #region input handling attributes
    private Dictionary<int, string> _buttons_osc_map = new Dictionary<int, string>
    {
        {1, "thumb+5"}, // with keyboard controller : 67
        {2, "thumb+4"}, // with keyboard controller : 65
        {3, "thumb+3"}, // with keyboard controller : 64
        {4, "thumb+2"}, // with keyboard controller : 62
        {5, "palm+5"},  // with keyboard controller : 55
        {6, "palm+4"},  // with keyboard controller : 53
        {7, "palm+3"},  // with keyboard controller : 52
        {8, "palm+2"},  // with keyboard controller : 50
        {9, "palm+1"},  // with keyboard controller : 48
    };

    private Dictionary<string, GloveInput> _buttons = new Dictionary<string, GloveInput>
    {
        {"thumb+5", new GloveInput()},
        {"thumb+4", new GloveInput()},
        {"thumb+3", new GloveInput()},
        {"thumb+2", new GloveInput()},
        {"palm+5", new GloveInput()},
        {"palm+4", new GloveInput()},
        {"palm+3", new GloveInput()},
        {"palm+2", new GloveInput()},
        {"palm+1", new GloveInput()},
    };
    #endregion

    #region control mode selection attributes
    private Dictionary<string, Type> _controlModeInputs = new Dictionary<string, Type>
    {
        {"thumb+2", typeof(ControlModeSampler)},
        {"thumb+3", typeof(ControlModeSlide)},
        {"thumb+4", typeof(ControlModeTakeoff)},
        {"thumb+5", typeof(ControlModeLand)},
    };

    private ControlMode _currentControlMode = null;
    #endregion

    private void Start () {
        // retrieve objects from the scene
        _oscManager = GameObject.Find("OscManager").GetComponent<OscManager>();
        _dronesManager = GameObject.Find("DronesManager").GetComponent<DronesManager>();

        Debug.Assert(_handDroneID >= 0, "hand drone ID not set");
        _handDrone = _dronesManager.GetDroneGameObjectById(_handDroneID);
        Debug.Assert(_headDroneID >= 0, "head drone ID not set");
        _headDrone = _dronesManager.GetDroneGameObjectById(_headDroneID);

        _dronesIds = new List<int>(_dronesManager
                           .GetDrones()
                           .Select(d_go => d_go.Id)
                           .Where(id => id != _handDroneID && id != _headDroneID));

        // soft "emergency" : do not need to reboot the drone nor the server
        _handDrone.GetComponent<Drone>().UseAsInput = true;
        _headDrone.GetComponent<Drone>().UseAsInput = true;

        // Subscribe to the specktr OSC topic
        _oscManager.OscSubscribe(specktrOscTopic, this.onButtons);
    }

    private void Update()
    {
        Debug.DrawLine(_handDrone.transform.position, _handDrone.transform.position + velocity, Color.red);
        //Debug.DrawLine(_drone.transform.position, _drone.transform.position + acceleration, Color.blue);

        Vector3 newPosition = _handDrone.GetComponent<Drone>().GetRealPosition();

        if (newPosition != Vector3.zero)
        {
            // STORE POSITION, DERIVE VELOCITY AND ACCELERATION
            position = newPosition;

            _lastPositions.AddLast(position);
            DeleteAllFromLinkedList(_lastPositions, MathUtils.Vec3Stamped.tmsIsBefore(0.5f));

            velocity = MathUtils.Vec3Stamped.average(_lastPositions);
            _lastVelocities.AddLast(velocity);
            DeleteAllFromLinkedList(_lastVelocities, MathUtils.Vec3Stamped.tmsIsBefore(0.5f));
            if (velocity.magnitude < 1)
                velocity = Vector3.zero;

            acceleration = MathUtils.Vec3Stamped.average(_lastVelocities);
        }

        // compute the pointing vector
        if (_handDrone != null && _headDrone != null)
        {
            Vector3 handPosition = _handDrone.GetComponent<Drone>().GetRealPosition();
            Vector3 shoulderPosition = _headDrone.GetComponent<Drone>().GetRealPosition() - new Vector3(0, 0.3f, 0); // 30cm between the head and the shoulders
            pointingVector = handPosition - shoulderPosition;

            Debug.DrawLine(handPosition, handPosition + pointingVector, Color.green);
        }

        this.HandleInputs();
        this.ClearInputs();
    }

    public void DeleteAllFromLinkedList<T>(LinkedList<T> list, Predicate<T> predicate)
    {
        var node = list.First;
        while (node != null)
        {
            var nextNode = node.Next;
            if (predicate(node.Value))
            {
                list.Remove(node);
            }
            node = nextNode;
        }
    }

    int GetButtonById(int key) { return (_buttons_osc_map.ContainsKey(key)) ? _buttons[_buttons_osc_map[key]].currentValue : 0; }
    void SetButtonById(int key, int value) { if (_buttons_osc_map.ContainsKey(key)) _buttons[_buttons_osc_map[key]].setValue(value); }


    /// <summary>
    /// Called on note_on osc packet.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    void onNoteOn(int key, int value)
    {
        if (_buttons_osc_map.ContainsKey(key))
            SetButtonById(key, value);
    }

    /// <summary>
    /// Called on note_off osc packet.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    void onNoteOff(int key, int value)
    {
        if (_buttons_osc_map.ContainsKey(key))
            SetButtonById(key, 0);
    }

    /// <summary>
    /// Called when an OSC packet is received from the specktr server.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="packet">The packet.</param>
    /// <param name="path_args">The path arguments.</param>
    void onButtons(string topic, OSCPacket packet, GroupCollection path_args)
    {
        string messageType = path_args["message_type"].Value;
 
        switch (messageType)
        {
            case "note_on": this.onNoteOn((int)packet.Data[0], (int)packet.Data[1]);
                Debug.LogFormat("NOTE ON", (int)packet.Data[0]);
                break;
            case "note_off": this.onNoteOff((int)packet.Data[0], (int)packet.Data[1]);
                break;
            case "control_change": // rotations. Not used yet
                break;
        }
    }


    // Input handling stuff
    void HandleInputs()
    {
        if (debugText != null)
        {
            string toPrint = "Buttons :\n";
            foreach (string key in _buttons.Keys)
            {
                toPrint += string.Format("{0}\t=>\t{1}\n", key, _buttons[key]);
            }
            debugText.text = toPrint;
        }

        HandleDroneSelection();
        HandleControlModeSelection();
        if (_currentControlMode != null)
            _currentControlMode.update(_buttons);
    }

    void ClearInputs()
    {
        foreach (string key in _buttons.Keys)
        {
            _buttons[key].edgeValue = 0;
        }
    }


    void HandleDroneSelection()
    {
        string pressedDroneSelectionKey = _droneSelectionInputs
            .Select(key => new KeyValuePair<string, GloveInput>(key, _buttons[key]))
            .Where(pair => pair.Value.edgeValue == 1)
            .Select(pair => pair.Key)
            .FirstOrDefault();

        if (pressedDroneSelectionKey != null)
        {
            SelectDroneByInput(pressedDroneSelectionKey);
        }
    }

    public void SelectDroneByInput(string input)
    {
        if (_droneSelectionInputs.Contains(input))
        {
            int selectedIndex = Array.IndexOf(_droneSelectionInputs, input);
            if (selectedIndex >= 0)
            {
                _currentDroneIdx = (selectedIndex >= _dronesIds.Count) ? _dronesIds.Count - 1 : selectedIndex;
                Debug.LogFormat("Drone selected : {0}", _dronesIds[_currentDroneIdx]);
            }
        }
    }

    // controlMode selection stuff
    public void ClearControlMode()
    {
        if (_currentControlMode != null)
        {
            _currentControlMode.end();
        }
        _currentControlMode = null;
    }

    public void SetControlMode(string s)
    {
        ClearControlMode();
        if (_controlModeInputs.ContainsKey(s) && _controlModeInputs[s] != null)
        {
            _currentControlMode = (ControlMode)Activator.CreateInstance(_controlModeInputs[s], new object[] { this });
            _currentControlMode.start();
        }
    }

    private void HandleControlModeSelection()
    {
        List<string> pressedControlModeSelectionKeys = new List<string>(
            _controlModeInputs.Keys
            .Select(key => new KeyValuePair<string, GloveInput>(key, _buttons[key])) // get buttons values
            .Where(pair => pair.Value.edgeValue == 1) // take only those with a rising edge signal
            .Select(pair => pair.Key)); // only get their name

        if (pressedControlModeSelectionKeys.Count > 0)
        {
            SetControlMode(pressedControlModeSelectionKeys[0]);
        }

        if (_currentControlMode != null) {
            bool controlModeKeyReleased =
                _controlModeInputs.Keys
                    .Select(key => new KeyValuePair<string, GloveInput>(key, _buttons[key])) // get buttons values
                    .Where(pair => pair.Value.edgeValue == -1) // take only those with a rising edge signal
                    .Select(pair => pair.Key) // only get their name
                    .FirstOrDefault(key => _controlModeInputs[key] == _currentControlMode.GetType()) // get only the key corresponding to the current controlMode
                != null;

            if (controlModeKeyReleased)
                ClearControlMode();
        }
    }

    // utils
    public GameObject GetCurrentDrone()
    {
        return _dronesManager.GetDroneGameObjectById(_dronesIds[_currentDroneIdx]);
    }

    public TrajectoryManager GetCurrentDroneTrajectoryManager()
    {
        return GetCurrentDrone().GetComponent<TrajectoryManager>();
    }

    public GameObject GetHandDrone()
    {
        return _handDrone;
    }

    public GameObject GetHeadDrone()
    {
        return _headDrone;
    }

}
