using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class SceneManager : MonoBehaviour {

	private OscManager _oscManager;
	private LpsManager _lpsManager;
	private DronesManager _dronesManager;

	private OSCClient _drones_osc_client;

	private bool system_initialized;

	public bool isInitialized() {
		return this.system_initialized;
	}

	// Use this for initialization
	public void Start () {
		this.system_initialized = false;
		this._oscManager = GameObject.Find ("OscManager").GetComponent<OscManager> ();		
		this._lpsManager = GameObject.Find ("LpsManager").GetComponent<LpsManager> ();
		this._dronesManager = GameObject.Find ("DronesManager").GetComponent<DronesManager> ();

		StartCoroutine (StartingCoroutine ());
	}

	IEnumerator StartingCoroutine()
	{
		yield return new WaitForFixedUpdate ();

		this._drones_osc_client = this._oscManager.createClient ("drones");
		this._oscManager.sendOscMessage (this._drones_osc_client,
			"/client/add",
			this._oscManager.localIP,
			(int)(this._oscManager.localPort));

		print ("Waiting for LPS and Drones managers init");
		yield return new WaitForSeconds (0.1f);
		print ("Connecting drones");
		_dronesManager.configDrones ();
		_dronesManager.connectDronesOsc ();

        this.system_initialized = true;
		//yield return new WaitForSeconds (5);
		//print ("Sending LPS configs");
		//_lpsManager.sendConfigOsc ();
		//yield return new WaitForSeconds (1);
		//print ("Reseting kalman filters");
		//_dronesManager.resetKalmanFilters ();
		//print ("Waiting for kalman filters to converge");
		//yield return new WaitForSeconds (10);
		//print ("SYSTEM OK");
		//this.system_initialized = true;
	}

	public void EMERGENCY() {
		this._oscManager.sendOscMessage (this._drones_osc_client,
			"/crazyflie/*/emergency", 1);
	}

	// Update is called once per frame
	void Update () {
	}

	public void Stop() {
		this._lpsManager.removeNodes ();
		this._dronesManager.removeDrones ();
		this._oscManager.Stop (); // REALLY IMPORTANT => CRASHES ON SECOND LAUNCH IF REMOVED
	}

	public void OnApplicationQuit() {
		this.Stop ();
	}
}
