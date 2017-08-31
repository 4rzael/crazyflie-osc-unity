using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityOSC;

/// <summary>
/// A simple tool to tune the pid position controller.
/// Should be put in the Drone class later.
/// Gives an interface to tune the values of the Position-Control PID in realtime.
/// </summary>
public class PidPositionController : MonoBehaviour {

    Drone droneScript;

    public struct PidVariable
    {
        public string name;
        public float value;
        public float min;
        public float max;

        public PidVariable(string _name, float _value, float _min=0, float _max=float.PositiveInfinity)
        {
            name = _name;
            value = _value;
            min = _min;
            max = _max;
        }
    }


    public List<PidVariable> variables = new List<PidVariable>()
    {
        new PidVariable("xKp", 1, 0, 5),
        new PidVariable("yKp", 1, 0, 5),
        new PidVariable("zKp", 1, 0, 5),

        new PidVariable("xKi", float.NaN, 0, 5),
        new PidVariable("yKi", float.NaN, 0, 5),
        new PidVariable("zKi", 0.5f, 0, 2),

        new PidVariable("xKd", float.NaN, 0, 5),
        new PidVariable("yKd", float.NaN, 0, 5),
        new PidVariable("zKd", float.NaN, 0, 5),
    };

    private OscManager oscManager;
    private OSCClient _oscClient;

	// Use this for initialization
	void Start () {
        oscManager = GameObject.Find("OscManager").GetComponent<OscManager>();
        droneScript = gameObject.GetComponent<Drone>();

        _oscClient = oscManager.createClient("drones");

        // On connection : send params
        gameObject.GetComponent<Drone>().ConnectionEvent += (Drone drone) =>
        {
            foreach (PidVariable variable in variables)
            {
                SetVariable(variable.name, variable.value);
            }
        };
    }

    public void SetVariable(string key, float value)
    {
        if (!float.IsNaN(value))
        {
            int varIndex = variables.FindIndex((PidVariable v) => v.name == key);
            if (varIndex >= 0)
            {
                PidVariable variable = variables[varIndex];
                float boundedValue = Mathf.Min(Mathf.Max(value, variable.min), variable.max);
                variable.value = boundedValue;
                variables[varIndex] = variable;
                droneScript.SetParam("posCtlPid", key, boundedValue);
            }
        }
    }
}
