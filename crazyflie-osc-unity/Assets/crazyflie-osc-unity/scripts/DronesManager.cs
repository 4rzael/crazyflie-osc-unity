using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
using System;

[ExecuteInEditMode]
public class DronesManager : MonoBehaviour {

	[Serializable]
	public class DroneConfig
	{
		public string radio_uri;

		public DroneConfig()
		{
			radio_uri = "radio://0/80/2M/E7E7E7E7E7";
		}
	}

	public GameObject dronePrefab;
	public string droneOscTopic = "/crazyflie";
	public DroneConfig[] dronesConfigs;

	private List<GameObject> _drones;
	private OscManager oscManager;
	private UnityOSC.OSCClient oscClient;


	private void cleanDronesList () {
		this._drones.RemoveAll (n => n == null);
	}

	private void initDrones () {
		foreach (GameObject drone in this._drones) {
			drone.GetComponent<Drone> ().oscClient = this.oscClient;
			drone.GetComponent<Drone> ().oscManager = this.oscManager;
		}
	}

	private void findDrones () {
		this._drones = new List<GameObject>(GameObject.FindGameObjectsWithTag("Drone"));
		this._drones.Sort (Drone.compareDrones);
	}

	/* PUBLIC METHODS */

	public void removeDrones () {
		foreach (GameObject drone in this._drones)
		{
			if (drone != null) {
				GameObject.DestroyImmediate (drone);
			}
		}
		this._drones.Clear ();
	}

	public void configDrones () {
		int id = 0;
		this.initDrones ();
		foreach (DroneConfig droneConfig in this.dronesConfigs) {
			Drone drone = this._drones.Find(d => d.GetComponent<Drone>().id == id).GetComponent<Drone>();
			drone.radio_uri = droneConfig.radio_uri;
			drone.baseTopic = this.droneOscTopic;
			++id;
		}
	}

	public void recreateDrones () {
		this.removeDrones ();
		int id = 0;
		foreach (DroneConfig droneConfig in this.dronesConfigs) {
			GameObject go = GameObject.Instantiate (dronePrefab, Vector3.zero, Quaternion.identity);
			go.name = "Drone_" + id.ToString ();
			Drone drone = go.GetComponent<Drone> ();
			drone.id = id;
			this._drones.Add (go);
			++id;
		}

		this.configDrones ();
	}

	public void connectDronesOsc () {
		this.cleanDronesList ();
		foreach (GameObject drone in this._drones) {
			drone.GetComponent<Drone> ().connect ();
		}
	}

	public void resetKalmanFilters () {
		this.cleanDronesList ();
		foreach (GameObject drone in this._drones) {
			drone.GetComponent<Drone> ().resetKalmanFilter ();
		}
	}

	// Use this for initialization
	public void Awake () {
		this.findDrones ();

		this.oscManager = GameObject.Find ("OscManager").GetComponent<OscManager> ();
		this.oscClient = this.oscManager.createClient ("drones");

		this.initDrones ();

		if (Application.isPlaying) {
			if (this._drones.Count < this.dronesConfigs.Length)
				this.recreateDrones ();
		}

	}
	
	// Update is called once per frame
	void Update () {
	}
}
