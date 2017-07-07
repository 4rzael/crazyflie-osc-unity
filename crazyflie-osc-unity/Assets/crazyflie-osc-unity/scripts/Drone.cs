﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

[ExecuteInEditMode]
public class Drone : MonoBehaviour {

	public int id;
	public string radio_uri { get; set; }

	public OSCClient oscClient { get; set; }
	public OscManager oscManager { get; set; }
	public string baseTopic { get; set; }

	private bool syncSend = false;
	private bool syncReceive = false;

	public void connect()
	{
		string topic = string.Format ("{0}/{1}/add", this.baseTopic, this.id.ToString());
		string uri = this.radio_uri;

		this.oscManager.sendOscMessage (this.oscClient, topic, uri);
	}

	public void startSyncSend() {
		this.syncSend = true;
	}
	public void stopSyncSend() {
		this.syncSend = false;
	}

	public void startSyncReceive() {
		this.syncReceive = true;
	}
	public void stopSyncReceive() {
		this.syncReceive = false;
	}

	public void resetKalmanFilter() {
		StartCoroutine (resetKalmanFilterCoroutine());
	}

	private IEnumerator resetKalmanFilterCoroutine() {
		this.oscManager.sendOscMessage(this.oscClient, string.Format ("/param/{0}/resetEstimation/set", this.id), 1);
		yield return new WaitForSeconds (0.1f);
		this.oscManager.sendOscMessage(this.oscClient, string.Format ("/param/{0}/resetEstimation/set", this.id), 0);	
	}

	public static int compareDrones(GameObject drone1, GameObject drone2)
	{
		return drone1.GetComponent<Drone> ().id - drone2.GetComponent<Drone> ().id;
	}

	void Update() {
		if (this.oscManager) {
			if (this.syncSend) {
				string topic = string.Format ("{0}/{1}/goal", this.baseTopic, this.id.ToString());
				this.oscManager.sendOscMessage (this.oscClient, topic, transform.position.x, transform.position.z, transform.position.y, 0);
			}
		}
	}
}