using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour {

    protected Dictionary<string, object> _variables = new Dictionary<string, object>();

    public void setVariable<T>(string name, T value)
    {
        _variables[name] = value;
    }

    public T getVariable<T>(string name)
    {
        if (_variables.ContainsKey(name))
        {
            return (T)(_variables[name]);
        }
        else
        {
            throw new System.Exception(string.Format("Variable {0} does not exist", name));
        }
    }


    public bool hasVariables(params string[] args)
    {
        foreach (string key in args)
            if (!_variables.ContainsKey(key))
                return false;
        return true;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {		
	}
}
