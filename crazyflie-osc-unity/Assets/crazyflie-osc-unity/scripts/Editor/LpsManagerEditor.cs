using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LpsManager))]
class LpsManagerEditor : Editor {
	public override void OnInspectorGUI() {

		DrawDefaultInspector();
		LpsManager lpsManager = (LpsManager)target;

		if (GUILayout.Button ("START")) {
			lpsManager.Awake ();
		}

		if (GUILayout.Button ("Recreate nodes")) {
			lpsManager.RecreateNodes ();
		}

		if (GUILayout.Button ("Send config through OSC")) {
			lpsManager.SendConfigOsc ();
		}

		if (GUILayout.Button ("Save positions")) {
			lpsManager.SavePositions ();
		}

		if (GUILayout.Button ("Remove nodes")) {
			lpsManager.RemoveNodes ();
		}

	}
}