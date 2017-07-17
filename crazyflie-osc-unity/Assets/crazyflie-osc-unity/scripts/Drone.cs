using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
using UnityEngine.UI;

public class Drone : MonoBehaviour {

	public int id;
	public string radio_uri { get; set; }
	public string baseTopic { get; set; }
	public GameObject realPositionMarkerPrefab;

	private GameObject _realPositionMarker;
	private Slider _batterySlider;
	private OSCClient _oscClient;
	private OscManager _oscManager;

	private bool _syncPosition = false;
	private Color _color;

	private Vector3 _realPosition = Vector3.zero;
	private float _battery = 0.0f;

	public void connect()
	{
		string topic = string.Format ("{0}/{1}/add", this.baseTopic, this.id.ToString());
		string uri = this.radio_uri;

		this._oscManager.sendOscMessage (this._oscClient, topic, uri);
	}

	public void startPositionSync() {
		if (this._realPosition != Vector3.zero)
			transform.position = this._realPosition;
		this._syncPosition = true;
	}
	public void stopPositionSync() {
		this._syncPosition = false;
	}

	public void resetKalmanFilter() {
		StartCoroutine (resetKalmanFilterCoroutine());
	}

	private IEnumerator resetKalmanFilterCoroutine() {
		this._oscManager.sendOscMessage(this._oscClient, string.Format ("/param/{0}/resetEstimation/set", this.id), 1);
		yield return new WaitForSeconds (0.1f);
		this._oscManager.sendOscMessage(this._oscClient, string.Format ("/param/{0}/resetEstimation/set", this.id), 0);	
	}

	public static int compareDrones(GameObject drone1, GameObject drone2)
	{
		return drone1.GetComponent<Drone> ().id - drone2.GetComponent<Drone> ().id;
	}

	void setRealPositionMarkerColor() {
		if (this._realPositionMarker)
			this._realPositionMarker.GetComponent<Renderer> ().material.color = this._color;
	}

	void setRealPositionMarkerPosition(Vector3 position) {
		if (this._realPositionMarker)
			this._realPositionMarker.transform.position = position;
	}


	public void setColor(Color c) {
		this._color = c;
		gameObject.GetComponentInChildren<Renderer> ().material.color = c;
		setRealPositionMarkerColor ();
	}
		
	public void init(OSCClient client, OscManager manager) {
		this._oscClient = client;
		this._oscManager = manager;

		this._oscManager.OscSubscribe(string.Format("/log/{0}/position", this.id),
			delegate(string topic, OSCPacket packet, System.Text.RegularExpressions.GroupCollection path_args) {
				this._realPosition = new Vector3(
					(float)packet.Data[0],
					(float)packet.Data[2], // As always, y and z are inverted
					(float)packet.Data[1]);
		});

		this._oscManager.OscSubscribe (string.Format ("/log/{0}/battery", this.id),
			delegate(string topic, OSCPacket packet, System.Text.RegularExpressions.GroupCollection path_args) {
				float battery = (float)packet.Data [0];
				this._battery = ((battery - 3.0f) / 1.1f) * 100.0f;
			});
	}

	void Start() {
		if (this.realPositionMarkerPrefab) {
			this._realPositionMarker = GameObject.Instantiate (this.realPositionMarkerPrefab,
				gameObject.GetComponent<Transform> ().position,
				Quaternion.identity);
            this._realPositionMarker.name = string.Format("RealPositionMarker_{0}", this.id);
			setRealPositionMarkerColor ();
		}
		this._batterySlider = gameObject.GetComponentInChildren<Slider>();
	}

	void Update() {
		this.setRealPositionMarkerPosition(this._realPosition);
		this._batterySlider.value = this._battery;

		if (this._oscManager != null) {
			if (this._syncPosition) {
				string topic = string.Format ("{0}/{1}/goal", this.baseTopic, this.id.ToString());
				this._oscManager.sendOscMessage (this._oscClient, topic, transform.position.x, transform.position.z, transform.position.y, 0);
			}
		}
	}

	void OnDestroy() {
		if (this._realPositionMarker)
			GameObject.DestroyImmediate (this._realPositionMarker.gameObject);
	}
}
