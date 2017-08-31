using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Represent the configuration of a drone, in order for the DronesManager to create them at game startup.
/// </summary>
[Serializable]
public class DroneConfig
{
    public bool is_active;
    public string radio_uri;
	public Color color;

    public int viz_id; // ONLY USED BY EDITOR

    /// <summary>
    /// Initializes a new instance of the <see cref="DroneConfig"/> class.
    /// </summary>
    /// <param name="_viz_id">The viz identifier.</param>
    public DroneConfig(int _viz_id=0)
	{
        viz_id = _viz_id;
        is_active = true;
        radio_uri = string.Format("radio://0/80/2M/E7{0}{0}{0}{0}", (viz_id + 1).ToString("00"));
		// color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f); // cannot be done in constructor
	}
}
