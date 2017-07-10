using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DronesManager))]
class DronesManagerEditor : Editor {
	public override void OnInspectorGUI() {

		DronesManager dronesManager = (DronesManager)target;
		MonitorArray.monitor(target, "arrayone", dronesManager.dronesConfigs, (int i) => {
			dronesManager.dronesConfigs[i] = new DronesManager.DroneConfig();
			dronesManager.dronesConfigs[i].color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
			dronesManager.dronesConfigs[i].viz_id = i;
		});

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

		if (GUILayout.Button ("OSC - Start Position Sync")) {
			dronesManager.startPositionSync ();
		}
		if (GUILayout.Button ("OSC - Stop Position Sync")) {
			dronesManager.stopPositionSync ();
		}

		if (GUILayout.Button ("OSC - Reset Kalman Filter - all")) {
			dronesManager.resetKalmanFilters();
		}

	}
}