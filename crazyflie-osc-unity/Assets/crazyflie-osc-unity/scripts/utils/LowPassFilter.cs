using System;
using System.Collections.Generic;
using UnityEngine;

public class LowPassFilter {

    private uint _windowSize;
    private List<Vector3> _windowValues;

    /// <summary>
    /// Initializes a new instance of the <see cref="LowPassFilter{T}"/> class.
    /// </summary>
    /// <param name="windowSize">Size of the moving-average window. Higher values gives a more filtered value but with a higher reaction delay.</param>
    public LowPassFilter(uint windowSize=1) {
        _windowSize = windowSize;
        _windowValues = new List<Vector3>((int)(_windowSize));
    }

    /// <summary>
    /// Resets the filter.
    /// </summary>
    public void ResetFilter()
    {
        _windowValues.Clear();
    }

    /// <summary>
    /// Adds an input to this filter and returns the new output.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public Vector3 AddValue(Vector3 value) {
        if (_windowValues.Count >= _windowSize)
        {
            _windowValues.RemoveRange(0, (int)(_windowValues.Count - _windowSize) + 1);
        }
        _windowValues.Add(value);
        return GetValue();
    }

    /// <summary>
    /// Returns the current computed output of this filter.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetValue()
    {
        Vector3 res = new Vector3();

        foreach (Vector3 v in _windowValues)
        {
            res += v;
        }
        return res / _windowValues.Count;
    }

    public void SetWindowSize(uint windowSize)
    {
        _windowSize = windowSize;
    }
}
