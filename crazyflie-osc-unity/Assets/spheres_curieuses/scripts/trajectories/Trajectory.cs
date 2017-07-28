using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour {

    protected Dictionary<string, object> _variables = new Dictionary<string, object>();
    protected bool started = false;

    protected delegate bool VariableSetter<T>(T variable);
    private Dictionary<string, object> _setters = new Dictionary<string, object>();


    protected void addSetter<T>(string variableName, VariableSetter<T> setter)
    {
        _setters[variableName] = setter;
    }

    protected void removeSetter(string variableName)
    {
        _setters.Remove(variableName);
    }

    public void setVariable<T>(string name, T value)
    {
        bool shouldSetVariable = true;
        if (_setters.ContainsKey(name)) // call the setter first
        {
            VariableSetter<T> setter = (VariableSetter<T>)_setters[name];
            shouldSetVariable = setter(value);
        }
        if (shouldSetVariable)
        {
            _variables[name] = value;
        }
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
    public T getVariable<T>(string name, T defaultValue)
    {
        return _variables.ContainsKey(name) ? getVariable<T>(name) : defaultValue;
    }



    public bool hasVariables(params string[] args)
    {
        foreach (string key in args)
            if (!_variables.ContainsKey(key))
                return false;
        return true;
    }

    public void startTrajectory()
    {
        this.started = true;
        this.onStart();
        this.render();
    }
    protected virtual void onStart() { }

    protected void stopTrajectory()
    {
        this.started = false;
        this.onStop();
        this.unrender();
    }
    protected virtual void onStop() { }

    // RENDERING STUFF
    public virtual void render() { }
    public virtual void unrender() { }

    // SPEED FUNCTION STUFF 
    public delegate float SpeedFunction(float x);
    /*
     * Speed distribution over time.
     * Must be a function that integrates to 1 in the [0;1] space
     * TODO : computationaly approximate the integral in order to remove this limitation ?
     */
    private SpeedFunction _speedFunction = x => 1.0f;

    private bool setSpeedFunction(SpeedFunction func)
    {
        this._speedFunction = func;
        return true; // Does not override default setter
    }

    protected float getSpeedAtTime(float t)
    {
        float speed = this._speedFunction(t);
        return speed;
    }

    // Use this for initialization
    protected virtual void Awake () {
        this.addSetter<SpeedFunction>("speed_function", this.setSpeedFunction);
	}

    private void OnDestroy()
    {
        stopTrajectory();
    }

    private void OnApplicationQuit()
    {
        stopTrajectory();
    }
}
