using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
using UnityEngine.UI;

public class Drone : MonoBehaviour
{
    #region Event handlers
    public delegate void ConnectionEventCallback(Drone drone);
    public delegate void ConnectionFailedEventCallback(Drone drone);
    public delegate void DisconnectionEventCallback(Drone drone);
    public delegate void LinkQualityEventCallback(Drone drone, float qualityPercentage);
    public delegate void PositionEventCallback(Drone drone, Vector3 position);
    public delegate void RollPitchYawEventCallback(Drone drone, Vector3 RollPitchYaw);
    public delegate void BatteryEventCallback(Drone drone, float battery);

    /// UnityEvents for drone OSC events
    public event ConnectionEventCallback ConnectionEvent;
    public event ConnectionFailedEventCallback ConnectionFailedEvent;
    public event DisconnectionEventCallback DisconnectionEvent;
    public event LinkQualityEventCallback LinkQualityEvent;
    public event PositionEventCallback PositionEvent;
    public event RollPitchYawEventCallback RollPitchYawEvent;
    public event BatteryEventCallback BatteryEvent;
    #endregion


    #region Public properties
    public GameObject realPositionMarkerPrefab;
    public string baseTopic = "/crazyflie";

    public Vector3 RealPosition { get { return _realPosition; } }

    [SerializeField] private int id = -1;
    public int Id { get { return id; } }
    [SerializeField] private string radioUri = null;
    public string RadioUri { get { return radioUri; } }

    public bool disableSendingCommands = false;
    #endregion


    #region Private properties
    // OSC
    private OscManager _oscManager;
    private OSCClient _oscClient;

    // "States"
    private bool _initialized = false;
    private bool _connected = false;
    private bool _syncPosition = false;

    // Real Position marker
    private Vector3 _rollPitchYaw;
    private Vector3 _realPosition = Vector3.zero;
    private GameObject _realPositionMarker;
    private Color _color;

    // Battery GUI
    private float _battery = 0.0f;
    private Slider _batterySlider;
    #endregion


    #region Public methods
    /// <summary>
    /// Initializes the specified identifier.
    /// </summary>
    /// <param name="id">The drone ID.</param>
    /// <param name="radio_uri">The radio URI.</param>
    /// <param name="baseTopic">optional : the topic for the Crazyflie module.</param>
    /// <exception cref="System.Exception">Drone initialized with bad ID or radio URI. Make sure they are set in the inspector</exception>
    public void Initialize(int id, string radio_uri, string baseTopic=null)
    {
        if (!_initialized)
        {
            if (id == -1 || radio_uri == null)
                throw new System.Exception("Drone initialized with bad ID or radio URI. Make sure they are set in the inspector");

            this.id = id;
            this.radioUri = radio_uri;
            if (baseTopic != null)
                this.baseTopic = baseTopic;

            _oscManager = GameObject.Find("OscManager").GetComponent<OscManager>();
            _oscClient = _oscManager.createClient("drones");

            #region Link OSC packets to events
            _oscManager.OscSubscribe(string.Format("{0}/{1}/connection", this.baseTopic, this.id),
            delegate (string topic, OSCPacket packet, System.Text.RegularExpressions.GroupCollection path_args)
            {
                bool success = (int)packet.Data[0] > 0;
                if (success)
                    OnConnection();
                else
                    OnConnectionFailed();
            });

            _oscManager.OscSubscribe(string.Format("{0}/{1}/disconnection", this.baseTopic, this.id),
            delegate (string topic, OSCPacket packet, System.Text.RegularExpressions.GroupCollection path_args)
            {
                OnDisconnection();
            });

            _oscManager.OscSubscribe(string.Format("{0}/{1}/link_quality", this.baseTopic, this.id),
            delegate (string topic, OSCPacket packet, System.Text.RegularExpressions.GroupCollection path_args)
            {
                float quality = (float)packet.Data[0];
                OnLinkQuality(quality);
            });

            _oscManager.OscSubscribe(string.Format("/log/{0}/position", this.id),
            delegate (string topic, OSCPacket packet, System.Text.RegularExpressions.GroupCollection path_args)
            {
                Vector3 pos = new Vector3(
                    (float)packet.Data[0],
                    (float)packet.Data[2], // As always, y and z are inverted
                    (float)packet.Data[1]);

                OnPosition(pos);
            });

            _oscManager.OscSubscribe(string.Format("/log/{0}/rpy", this.id),
            delegate (string topic, OSCPacket packet, System.Text.RegularExpressions.GroupCollection path_args)
            {
                Vector3 rpy = new Vector3(
                    (float)packet.Data[0],
                    (float)packet.Data[1],
                    (float)packet.Data[2]);

                OnRollPitchYaw(rpy);
            });

            _oscManager.OscSubscribe(string.Format("/log/{0}/battery", this.id),
            delegate (string topic, OSCPacket packet, System.Text.RegularExpressions.GroupCollection path_args)
            {
                float voltage = (float)packet.Data[0];

                OnBattery(voltage);
            });
            #endregion

            _initialized = true;
        }
    }

    /// <summary>
    /// Sends an OSC message asking to connect the drone through radio
    /// </summary>
    public void Connect() {
        Debug.Assert(_initialized, "Not Initialized");
        if (_initialized)
        {
            string topic = string.Format("{0}/{1}/add", this.baseTopic, this.id.ToString());
            string uri = this.radioUri;

            this._oscManager.SendOscMessage(this._oscClient, topic, uri);
        }
    }

    /// <summary>
    /// Sends an OSC message asking to disconnect the drone through radio
    /// </summary>
    public void Disconnect() {
        Debug.Assert(_initialized, "Not Initialized");
        if (_initialized)
        {
            string topic = string.Format("{0}/{1}/remove", this.baseTopic, this.id.ToString());
            this._oscManager.SendOscMessage(this._oscClient, topic);
        }
    }

    /// <summary>
    /// Starts sending goals packets from the position of this gameobject
    /// </summary>
    public void StartPositionSync() {
        // If the drone gameobject did not move, set it to go a the current real position
        if (transform.position == Vector3.zero)
        {
            transform.position = new Vector3(this._realPosition.x, 0, this._realPosition.z);
        }
        this._syncPosition = true;
    }

    /// <summary>
    /// Stops sending goals packets from the position of this gameobject
    /// </summary>
    public void StopPositionSync() {
        this._syncPosition = false;
    }

    /// <summary>
    /// Resets the kalman filter.
    /// </summary>
    public void ResetKalmanFilter()
    {
        Debug.Assert(_initialized, "Not Initialized");
        if (_initialized)
        {
            StartCoroutine(ResetKalmanFilterCoroutine());
        }
    }

    /// <summary>
    /// Sends an emergency packet to the drone. Cuts engines and prevent them from firing again.
    /// </summary>
    public void SendEmergencySignal()
    {
        if (_initialized)
        {
            this._oscManager.SendOscMessage(this._oscClient, string.Format("/crazyflie/{0}/emergency", id), 1);
        }
    }

    /// <summary>
    /// Determines whether this drone is connected.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this drone is connected; otherwise, <c>false</c>.
    /// </returns>
    public bool IsConnected()
    {
        return _connected;
    }

    /// <summary>
    /// Sets the color.
    /// </summary>
    /// <param name="color">The color.</param>
    public void SetColor(Color color)
    {
        this._color = color;
        gameObject.GetComponentInChildren<Renderer>().material.color = color;
        SetRealPositionMarkerColor();
    }

    /// <summary>
    /// Gets the real position marker GameObject.
    /// </summary>
    /// <returns></returns>
    public GameObject GetRealPositionMarker()
    {
        return _realPositionMarker;
    }

    public Vector3 GetRealPosition()
    {
        return _realPositionMarker.transform.position;
    }
    #endregion


    #region Log / Param access methods (public)
    public struct LogVariable
    {
        public string name;
        public string type;
    }
    /// <summary>
    /// Adds a log configuration to the drone.
    /// (See more informations on logging here : https://wiki.bitcraze.io/doc:crazyflie:api:python:index)
    /// </summary>
    /// <param name="logName">Name of the log group to create.</param>
    /// <param name="logVariables">The log variables. (full names. For example : kalman.stateX)</param>
    /// <param name="periodMillis">The logging period in milliseconds (multiples of 10).</param>
    /// <param name="callback">Optional: An OSC callback to be called each time this log is received.</param>
    public void AddLog(string logName, IEnumerable<LogVariable> logVariables, int periodMillis, OscManager.OscSubscribeCallback callback = null)
    {
        Debug.Assert(_initialized, "Not Initialized");
        if (_initialized)
        {
            _oscManager.SendOscMessage(_oscClient, string.Format("/log/{0}/add", id), logName, periodMillis);
            foreach (LogVariable variable in logVariables)
            {
                _oscManager.SendOscMessage(_oscClient, string.Format("/log/{0}/{1}/add_variable", id, logName), variable.name, variable.type);
            }
            _oscManager.SendOscMessage(_oscClient, string.Format("/log/{0}/{1}/start", id, logName));

            if (callback != null)
            {
                _oscManager.OscSubscribe(string.Format("/log/{0}/{1}", id, logName), callback);
                // Subscribe a no-op to sub-packets (each variable is also sent separatly from the log group)
                _oscManager.OscSubscribe(string.Format("/log/{0}/{1}/*", id, logName), (topic, packet, path_args) => { });
            }
        }
    }

    /// <summary>
    /// Sets the parameter {paramGroup}.{paramName} to the value {value}
    /// (See more informations on logging here : https://wiki.bitcraze.io/doc:crazyflie:api:python:index)
    /// </summary>
    /// <typeparam name="T">The type of the value to set (allows basic types only)</typeparam>
    /// <param name="paramGroup">Group of the parameter variable.</param>
    /// <param name="paramName">Name of the parameter variable.</param>
    /// <param name="value">Value to set to the parameter.</param>
    public void SetParam<T>(string paramGroup, string paramName, T value)
    {
        Debug.Assert(_initialized, "Not Initialized");
        if (_initialized)
        {
            this._oscManager.SendOscMessage(this._oscClient, string.Format("/param/{0}/{1}/{2}/set", this.id, paramGroup, paramName), value);
        }
    }
    #endregion


    #region OSC events methods (protected)
    /// <summary>
    /// Called on drone connection. Also dispatch a ConnectionEvent
    /// Don't forget to call the base method if you override this
    /// </summary>
    protected void OnConnection()
    {
        _connected = true;
        if (ConnectionEvent != null) ConnectionEvent(this);
    }


    /// <summary>
    /// Called on drone connection failure. Also dispatch a ConnectionFailedEvent
    /// Don't forget to call the base method if you override this
    /// </summary>
    protected void OnConnectionFailed()
    {
        if (ConnectionFailedEvent != null) ConnectionFailedEvent(this);
    }


    /// <summary>
    /// Called on drone disconnection. Also dispatch a DisconnectionEvent
    /// Don't forget to call the base method if you override this
    /// </summary>
    protected void OnDisconnection()
    {
        _connected = false;
        if (DisconnectionEvent != null) DisconnectionEvent(this);
    }

    /// <summary>
    /// Called on link quality reception. Also dispatch a LinkQualityEvent
    /// Don't forget to call the base method if you override this
    /// </summary>
    /// <param name="qualityPercentage">The percentage of successfully sent and received packets, from 0 to 100.</param>
    protected void OnLinkQuality(float qualityPercentage)
    {
        if (LinkQualityEvent != null) LinkQualityEvent(this, qualityPercentage);
    }


    /// <summary>
    /// Called on drone position reception. Also dispatch a PositionEvent
    /// Don't forget to call the base method if you override this
    /// </summary>
    /// <param name="position">The position given by the Kalman Filter (In the Unity3d base).</param>
    protected void OnPosition(Vector3 position)
    {
        _realPosition = position;
        if (PositionEvent != null) PositionEvent(this, position);
    }


    /// <summary>
    /// Called when [roll pitch yaw].
    /// Don't forget to call the base method if you override this
    /// </summary>
    /// <param name="rpy">The Roll/Pitch/Yaw Vector3 (can be converted to a quaternion).</param>
    protected void OnRollPitchYaw(Vector3 rpy)
    {
        _rollPitchYaw = rpy;
        if (RollPitchYawEvent != null) RollPitchYawEvent(this, rpy);
    }


    /// <summary>
    /// Called on drone battery reception. Also dispatch a BatteryEvent
    /// Don't forget to call the base method if you override this
    /// </summary>
    /// <param name="battery">The battery voltage in volts.</param>
    protected void OnBattery(float battery) {
        // 100% arbitrary :D (should take into account the current thrust to really work)
        _battery = ((battery - 3.0f) / 1.1f) * 100.0f;

        if (BatteryEvent != null) BatteryEvent(this, battery);
    }
    #endregion


    #region Static methods
    public static int CompareDrones(GameObject drone1, GameObject drone2)
    {
        return drone1.GetComponent<Drone>().id - drone2.GetComponent<Drone>().id;
    }
    #endregion


    #region Real position marker private methods
    private void SetRealPositionMarkerColor()
    {
        if (this._realPositionMarker)
            this._realPositionMarker.GetComponent<Renderer>().material.color = this._color;
    }

    private void SetRealPositionMarkerPosition(Vector3 position)
    {
        if (this._realPositionMarker)
            this._realPositionMarker.transform.position = position;
    }

    private void SetRealPositionMarkerRotation(Quaternion rotation)
    {
        if (this._realPositionMarker)
            this._realPositionMarker.transform.rotation = rotation;
    }
    #endregion


    #region Unity specific methods (start, update, etc..)
    void Start()
    {
        Initialize(this.id, this.radioUri); // id and radio_uri must be set

        // initialize UI stuff
        if (this.realPositionMarkerPrefab)
        {
            this._realPositionMarker = GameObject.Instantiate(this.realPositionMarkerPrefab,
                gameObject.GetComponent<Transform>().position,
                Quaternion.identity);
            this._realPositionMarker.name = string.Format("RealPositionMarker_{0}", this.id);
            SetRealPositionMarkerColor();
        }
        this._batterySlider = gameObject.GetComponentInChildren<Slider>();
    }

    void Update()
    {
        // update UI stuff
        this.SetRealPositionMarkerPosition(this._realPosition);
        this.SetRealPositionMarkerRotation(Quaternion.Euler(-_rollPitchYaw[0], -_rollPitchYaw[2], _rollPitchYaw[1]));
        this._batterySlider.value = this._battery;

        // update goal
        if (this._oscManager != null)
        {
            if (this.disableSendingCommands == false && this._syncPosition)
            {
                SendGoal(transform.position);
            }
        }

        // Debug UI stuff
        Debug.DrawLine(_realPositionMarker.transform.position, _realPositionMarker.transform.position - _realPositionMarker.transform.up);
    }

    void OnDestroy()
    {
        if (this._realPositionMarker)
            GameObject.DestroyImmediate(this._realPositionMarker.gameObject);
    }
    #endregion


    #region Other methods
    private IEnumerator ResetKalmanFilterCoroutine()
    {
        SetParam("kalman", "resetEstimation", 1);
        yield return new WaitForSeconds(0.1f);
        SetParam("kalman", "resetEstimation", 0);
    }

    /// <summary>
    /// Sends the position goal to the drone.
    /// </summary>
    /// <param name="position">The position.</param>
    protected void SendGoal(Vector3 position)
    {
        string topic = string.Format("{0}/{1}/goal", this.baseTopic, this.id.ToString());
        this._oscManager.SendOscMessage(this._oscClient, topic, position.x, position.z, position.y, 0); // Y and Z are inverted
    }
    #endregion
}
