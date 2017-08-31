using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedRingController : MonoBehaviour {

    private Drone droneScript;
    private Light droneLight;

    public Color32 color = Color.white;
    public LedEffect effect = LedEffect.SOLID_COLOR_EFFECT;
    public bool headlight = false;

    private bool isConnected = false;

    public enum LedEffect
    {
        OFF=0,
        WHITE_SPINNER=1,
        COLOR_SPINNER=2,
        TILT_EFFECT=3,
        BRIGHTNESS_EFFECT=4,
        COLOR_SPINNER_2=5,
        DOUBLE_SPINNER=6,
        SOLID_COLOR_EFFECT=7,
        FACTORY_TEST=8,
        BATTERY_STATUS=9,
        BOAT_LIGHT=10,
        ALERT=11,
        GRAVITY=12,
        N_A=13
    };

    private LedRingController.LedEffect lastEffect;
    private Color32 lastColor;
    private bool lastHeadlight;

    // Use this for initialization
    void Start () {
        droneScript = gameObject.GetComponent<Drone>();
        droneLight = gameObject.GetComponentInChildren<Light>();

        lastEffect = effect;
        lastColor = color;
        lastHeadlight = headlight;

        droneScript.ConnectionEvent += (Drone drone) =>
        {
            SetParams(true);
        };

        cooldown = Time.time;
	}

    void SetParams(bool force=false)
    {
        if (effect != lastEffect || force)
        {
            lastEffect = effect;
            droneScript.SetParam("ring", "effect", (int)effect);
        }
        if ((Color)color != (Color)lastColor || force)
        {
            lastColor = color;
            droneScript.SetParam("ring", "solidBlue", (int)color.b);
            droneScript.SetParam("ring", "solidGreen", (int)color.g);
            droneScript.SetParam("ring", "solidRed", (int)color.r);
        }
        if (headlight != lastHeadlight || force)
        {
            lastHeadlight = headlight;
            droneScript.SetParam("ring", "headlightEnable", headlight ? 1 : 0);
        }
    }

    private float cooldown;

	// Update is called once per frame
	void Update () {
        if (Time.time >= cooldown + 0.1f && droneScript.IsConnected())
        {
            cooldown = Time.time;
            SetParams();
        }

        if (droneLight != null)
            droneLight.color = color;
    }
}
