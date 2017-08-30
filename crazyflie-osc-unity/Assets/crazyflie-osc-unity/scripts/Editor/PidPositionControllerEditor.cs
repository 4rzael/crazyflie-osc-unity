using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(PidPositionController))]
class PidPositionControllerEditor : Editor
{
        private struct VarToChange
        {
            public string name;
            public float value;
            public VarToChange(string _name, float _value)
            {
                name = _name;
                value = _value;
            }
        }

    Dictionary<string, float> variables = new Dictionary<string, float>();

    public override void OnInspectorGUI()
    {

        PidPositionController positionController = (PidPositionController)target;

        DrawDefaultInspector();

        IEnumerable<PidPositionController.PidVariable> variables = positionController.variables;

        List<VarToChange> toChange = new List<VarToChange>();

        foreach (PidPositionController.PidVariable variable in variables)
        {
            float res = EditorGUILayout.Slider(variable.name,
                                   variable.value,
                                   variable.min,
                                   variable.max);
            if (res != variable.value)
                toChange.Add(new VarToChange(variable.name, variable.value));
        }
        foreach (VarToChange v in toChange)
        {
            positionController.SetVariable(v.name, v.value);
        }
    }
}