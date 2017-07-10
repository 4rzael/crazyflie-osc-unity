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
		if (GUILayout.Button ("OSC - Start Position Sync")) {
			drone.startPositionSync ();
		}
		if (GUILayout.Button ("OSC - Stop Position Sync")) {
			drone.stopPositionSync ();
		}
		if (GUILayout.Button ("OSC - Reset Kalman Filter")) {
			drone.resetKalmanFilter();
		}
	}
}