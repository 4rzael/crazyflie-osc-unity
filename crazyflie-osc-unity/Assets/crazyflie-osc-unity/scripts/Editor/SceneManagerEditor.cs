using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneManager))]
class SceneManagerEditor : Editor {
	public override void OnInspectorGUI() {

		SceneManager manager = (SceneManager)target;

		DrawDefaultInspector();

		if (GUILayout.Button ("Start all")) {
			manager.Start ();
		}

		if (GUILayout.Button ("Stop all")) {
			manager.Stop ();
		}

	}
}