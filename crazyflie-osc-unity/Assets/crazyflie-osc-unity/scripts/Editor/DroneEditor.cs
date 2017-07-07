using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Drone))]
class DroneEditor : Editor {
	public override void OnInspectorGUI() {

		Drone drone = (Drone)target;

		DrawDefaultInspector();

		if (GUILayout.Button ("OSC - Connect")) {
			drone.connect ();
		}
		if (GUILayout.Button ("OSC - Start Sync - Send")) {
			drone.startSyncSend ();
		}
		if (GUILayout.Button ("OSC - Stop Sync - Send")) {
			drone.stopSyncSend ();
		}
		if (GUILayout.Button ("OSC - Start Sync - Receive")) {
			drone.startSyncReceive ();
		}
		if (GUILayout.Button ("OSC - Stop Sync - Receive")) {
			drone.stopSyncReceive ();
		}
		if (GUILayout.Button ("OSC - Reset Kalman Filter")) {
			drone.resetKalmanFilter();
		}
	}
}