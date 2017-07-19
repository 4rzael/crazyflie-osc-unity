using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(SceneManager))]
class SceneManagerEditor : Editor {
    void OnEnable()
    {
        EditorUtility.SetDirty(this);
    }

    public override void OnInspectorGUI() {
        SceneManager manager = (SceneManager)target;

		if (GUILayout.Button ("Start all")) {
			manager.Start ();
		}

		if (GUILayout.Button ("Stop all")) {
			manager.Stop ();
		}

		if (GUILayout.Button ("EMERGENCY")) {
			manager.EMERGENCY ();
		}

        manager.shouldInitialize = EditorGUILayout.Toggle("should initialize ?", manager.shouldInitialize);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
	}
}