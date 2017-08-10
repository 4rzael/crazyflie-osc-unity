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
			drone.Connect ();
		}
		if (GUILayout.Button ("OSC - Start Position Sync")) {
			drone.StartPositionSync ();
		}
		if (GUILayout.Button ("OSC - Stop Position Sync")) {
			drone.StopPositionSync ();
		}
		if (GUILayout.Button ("OSC - Reset Kalman Filter")) {
			drone.ResetKalmanFilter();
		}
        if (GUILayout.Button("OSC - EMERGENCY")) {
            drone.SendEmergencySignal();
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // Separator
        EditorGUILayout.LabelField("DEBUG ONLY");
        // Nothing :D

    }
}