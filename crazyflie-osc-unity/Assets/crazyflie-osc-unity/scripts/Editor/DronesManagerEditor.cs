using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DronesManager))]
class DronesManagerEditor : Editor {
	public override void OnInspectorGUI() {

		DronesManager dronesManager = (DronesManager)target;
		MonitorArray.monitor(target, "arrayone", dronesManager.dronesConfigs, (int i) => {
			dronesManager.dronesConfigs[i] = new DroneConfig(i);
			dronesManager.dronesConfigs[i].color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
		});

		DrawDefaultInspector();

		if (GUILayout.Button ("Recreate drones")) {
			dronesManager.RecreateDrones ();
		}

		if (GUILayout.Button ("Remove drones")) {
			dronesManager.DestroyDrones ();
		}

		// OSC actions

		if (GUILayout.Button ("OSC - Connect - all")) {
			dronesManager.ConnectDrones ();
		}

		if (GUILayout.Button ("OSC - Start Position Sync")) {
			dronesManager.StartPositionSync ();
		}
		if (GUILayout.Button ("OSC - Stop Position Sync")) {
			dronesManager.StopPositionSync ();
		}

		if (GUILayout.Button ("OSC - Reset Kalman Filter - all")) {
			dronesManager.ResetKalmanFilters();
		}

        if (GUILayout.Button("OSC - EMERGENCY - all"))
        {
            dronesManager.SendEmergencySignals();
        }
    }
}