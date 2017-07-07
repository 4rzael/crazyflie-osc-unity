using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DronesManager))]
class DronesManagerEditor : Editor {
	public override void OnInspectorGUI() {

		DronesManager dronesManager = (DronesManager)target;
		MonitorArray.monitor(target, "arrayone", dronesManager.dronesConfigs, (int i) => { dronesManager.dronesConfigs[i] = new DronesManager.DroneConfig(); });

		DrawDefaultInspector();

		if (GUILayout.Button ("START")) {
			dronesManager.Awake ();
		}


		if (GUILayout.Button ("Recreate drones")) {
			dronesManager.recreateDrones ();
		}

		if (GUILayout.Button ("Config drones")) {
			dronesManager.configDrones ();
		}

		if (GUILayout.Button ("Remove drones")) {
			dronesManager.removeDrones ();
		}

		// OSC actions

		if (GUILayout.Button ("OSC - Connect - all")) {
			dronesManager.connectDronesOsc ();
		}

		if (GUILayout.Button ("OSC - Reset Kalman Filter - all")) {
			dronesManager.resetKalmanFilters();
		}
	}
}