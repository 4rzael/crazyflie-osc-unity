using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DroneConfig
{
	public string radio_uri;
	public Color color;

	public DroneConfig()
	{
		radio_uri = "radio://0/80/2M/E7E7E7E7E7";
		color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
	}
}
