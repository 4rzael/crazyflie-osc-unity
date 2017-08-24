using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(PidPositionController))]
class PidPositionControllerEditor : Editor
{
    Dictionary<string, float> variables = new Dictionary<string, float>();

    public override void OnInspectorGUI()
    {

        PidPositionController positionController = (PidPositionController)target;

        DrawDefaultInspector();

        IEnumerable<string> keys = positionController.variables.Keys.OrderBy(s => new string(s.ToCharArray().Reverse().ToArray()));

        foreach (string key in keys)
        {
            variables[key] = EditorGUILayout.Slider(key,
                                   positionController.variables[key],
                                   0,
                                   Math.Max(positionController.variables[key], 100.0f));

        }
        foreach (string key in variables.Keys)
        {
            if (variables[key] != positionController.variables[key])
            {
                positionController.SetVariable(key, variables[key]);
            }
        }
    }
}