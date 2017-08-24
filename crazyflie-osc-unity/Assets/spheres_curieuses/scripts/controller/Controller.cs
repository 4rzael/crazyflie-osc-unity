using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityOSC;

public class Controller : MonoBehaviour {

    [SerializeField] private Text debugText;
    [SerializeField] private string specktrOscTopic = "/specktr/{channel}/{message_type}";

    private OscManager _oscManager;
    private DronesManager _dronesManager;
    [SerializeField] private int _controllerDroneID;
    private GameObject _drone;

    public Vector3 position = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;

    private LinkedList<MathUtils.Vec3Stamped> _lastPositions = new LinkedList<MathUtils.Vec3Stamped>();
    private LinkedList<MathUtils.Vec3Stamped> _lastVelocities = new LinkedList<MathUtils.Vec3Stamped>();


    void Start () {
        // retrieve obects from the scene
        _oscManager = GameObject.Find("OscManager").GetComponent<OscManager>();
        _dronesManager = GameObject.Find("DronesManager").GetComponent<DronesManager>();
        _drone = _dronesManager.GetDroneGameObjectById(_controllerDroneID);

        _dronesIds = new List<int>(_dronesManager
                           .GetDrones()
                           .Select(d_go => d_go.Id)
                           .Where(id => id != _controllerDroneID));

        // soft "emergency" : do not need to reboot the drone
        _drone.GetComponent<Drone>().UseAsInput = true;

        // Subscribe to the specktr OSC topic
        _oscManager.OscSubscribe(specktrOscTopic, this.onButtons);
    }

    private void Update()
    {
        Debug.DrawLine(_drone.transform.position, _drone.transform.position + velocity, Color.red);
        //Debug.DrawLine(_drone.transform.position, _drone.transform.position + acceleration, Color.blue);

        Vector3 newPosition = _drone.GetComponent<Drone>().GetRealPosition();

        if (newPosition != Vector3.zero)
        {
            // STORE POSITION, DERIVE VELOCITY AND ACCELERATION
            position = newPosition;

            _lastPositions.AddLast(position);
            deleteAllFromLinkedList(_lastPositions, MathUtils.Vec3Stamped.tmsIsBefore(0.5f));

            velocity = MathUtils.Vec3Stamped.average(_lastPositions);
            _lastVelocities.AddLast(velocity);
            deleteAllFromLinkedList(_lastVelocities, MathUtils.Vec3Stamped.tmsIsBefore(0.5f));
            if (velocity.magnitude < 1)
                velocity = Vector3.zero;

            acceleration = MathUtils.Vec3Stamped.average(_lastVelocities);
        }

        this.handleInputs();
        this.clearInputs();
    }

    public void deleteAllFromLinkedList<T>(LinkedList<T> list, Predicate<T> predicate)
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

    // Input handling stuff (todo : Inspector UI ?)

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

    private Dictionary<string, ControlMode> _controlModes = new Dictionary<string, ControlMode>
    {
        {"thumb+5", null},
        {"thumb+4", null},
        {"thumb+3", null},
        {"thumb+2", null},
    };

    int getButtonById(int key) { return (_buttons_osc_map.ContainsKey(key)) ? _buttons[_buttons_osc_map[key]].currentValue : 0; }
    void setButtonById(int key, int value) { if (_buttons_osc_map.ContainsKey(key)) _buttons[_buttons_osc_map[key]].setValue(value); }


    void onNoteOn(int key, int value)
    {
        if (_buttons_osc_map.ContainsKey(key))
            setButtonById(key, value);
    }

    void onNoteOff(int key, int value)
    {
        if (_buttons_osc_map.ContainsKey(key))
            setButtonById(key, 0);
    }

    void onButtons(string topic, OSCPacket packet, GroupCollection path_args)
    {
        string messageType = path_args["message_type"].Value;
        //Debug.LogFormat("spectkr command of type {0} received : '{1}'",
        //    messageType,
        //    packet.Data.Select(b => b.ToString()).Aggregate((a, b) => a + ", " + b));

        switch (messageType)
        {
            case "note_on": this.onNoteOn((int)packet.Data[0], (int)packet.Data[1]);
                Debug.LogFormat("NOTE ON", (int)packet.Data[0]);
                break;
            case "note_off": this.onNoteOff((int)packet.Data[0], (int)packet.Data[1]);
                break;
            case "control_change":
                break;
        }
    }



    // Input handling stuff
    void handleInputs()
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

        handleDroneSelection();
        handleControlModeSelection();
        if (_currentControlMode != null)
            _currentControlMode.update(_buttons);
    }

    void clearInputs()
    {
        foreach (string key in _buttons.Keys)
        {
            _buttons[key].edgeValue = 0;
        }
    }



    // drone selection stuff
    private List<int> _dronesIds;
    private int _currentDroneIdx = 0;
    private string[] _droneSelectionInputs = new string[] {"palm+2", "palm+3", "palm+4", "palm+5" };

    void handleDroneSelection()
    {
        string pressedDroneSelectionKey = _droneSelectionInputs
            .Select(key => new KeyValuePair<string, GloveInput>(key, _buttons[key]))
            .Where(pair => pair.Value.edgeValue == 1)
            .Select(pair => pair.Key)
            .FirstOrDefault();

        if (pressedDroneSelectionKey != null)
        {
            selectDroneByInput(pressedDroneSelectionKey);
        }

        //List<int> pressedDroneSelectionKeys = new List<int>(
        //    _droneSelectionInputs
        //    .Select(key => new KeyValuePair<int, GloveInput>(Array.IndexOf(_droneSelectionInputs, key), _buttons[key])) // get buttons values
        //    .Where(pair => pair.Key >= 0)
        //    .Where(pair => pair.Value.edgeValue == 1) // take those with a rising edge signal
        //    .Select(pair => pair.Key)); // only get their position in the drone id array

        //if (pressedDroneSelectionKeys.Count > 0) // drone selected
        //{
        //    int selectedIndex = pressedDroneSelectionKeys[0];
        //    _currentDroneIdx = (selectedIndex >= _dronesIds.Count) ? _dronesIds.Count - 1 : selectedIndex;

        //    Debug.LogFormat("Drone selected : {0}", _dronesIds[_currentDroneIdx]);
        //}
    }

    public void selectDroneByInput(string input)
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
    private Dictionary<string, Type> _controlModeInputs = new Dictionary<string, Type>
    {
        {"thumb+2", typeof(ControlModeJuggleHorizontal)},
        {"thumb+3", typeof(ControlModeSlide)},
        {"thumb+4", typeof(ControlModeTakeoff)},
        {"thumb+5", null},
    };

    private ControlMode _currentControlMode = null;

    private void clearControlMode()
    {
        if (_currentControlMode != null)
            _currentControlMode.end();
        _currentControlMode = null;
    }

    public void setControlMode(string s)
    {
        if (_controlModeInputs.ContainsKey(s) && _controlModeInputs[s] != null)
        {
            clearControlMode();
            _currentControlMode = (ControlMode)Activator.CreateInstance(_controlModeInputs[s], new object[] { this });
            _currentControlMode.start();
        }
    }

    private void handleControlModeSelection()
    {
        List<string> pressedControlModeSelectionKeys = new List<string>(
            _controlModeInputs.Keys
            .Select(key => new KeyValuePair<string, GloveInput>(key, _buttons[key])) // get buttons values
            .Where(pair => pair.Value.edgeValue == 1) // take only those with a rising edge signal
            .Select(pair => pair.Key)); // only get their name

        if (pressedControlModeSelectionKeys.Count > 0)
        {
            setControlMode(pressedControlModeSelectionKeys[0]);
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
                clearControlMode();
        }
    }

    // utils
    public GameObject getCurrentDrone()
    {
        return _dronesManager.GetDroneGameObjectById(_dronesIds[_currentDroneIdx]);
    }

    public TrajectoryManager getCurrentDroneTrajectoryManager()
    {
        return getCurrentDrone().GetComponent<TrajectoryManager>();
    }

}
