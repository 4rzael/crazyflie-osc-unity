using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

	private OscManager _oscManager;
	private LpsManager _lpsManager;
	private DronesManager _dronesManager;

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
		print ("Waiting for LPS and Drones managers init");
		yield return new WaitForSeconds (0.1f);
		print ("Connecting drones");
		_dronesManager.configDrones ();
		_dronesManager.connectDronesOsc ();

		yield return new WaitForSeconds (5);
		print ("Sending LPS configs");
		_lpsManager.sendConfigOsc ();
		yield return new WaitForSeconds (1);
		print ("Reseting kalman filters");
		_dronesManager.resetKalmanFilters ();
		print ("Waiting for kalman filters to converge");
		yield return new WaitForSeconds (10);
		print ("SYSTEM OK");
		this.system_initialized = true;
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void Stop() {
		this._lpsManager.removeNodes ();
		this._dronesManager.removeDrones ();
		this._oscManager.OnApplicationQuit ();
	}
}
