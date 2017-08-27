using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Controller))]
class ControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {

        Controller controller = (Controller)target;

        DrawDefaultInspector();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // Separator

        string[] droneCommands = { "palm+1", "palm+2", "palm+3", "palm+4", "palm+5", };

        foreach (string c in droneCommands)
        {
            if (GUILayout.Button(c))
            {
                controller.SelectDroneByInput(c);
            }
        }


        string[] controlModeCommands = {"thumb+2", "thumb+3", "thumb+4", "thumb+5", "nothing"};

        foreach(string c in controlModeCommands)
        {
            if (GUILayout.Button(c))
            {
                controller.SetControlMode(c);
            }
        }
    }
}