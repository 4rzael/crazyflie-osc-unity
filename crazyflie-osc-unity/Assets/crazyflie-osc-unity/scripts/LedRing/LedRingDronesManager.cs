using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LedRingDronesManager : MonoBehaviour {

    private DronesManager dronesManager;

    [SerializeField] private LedRingController.LedEffect effect = LedRingController.LedEffect.SOLID_COLOR_EFFECT;
    [SerializeField] private Color32 color = Color.white;
    [SerializeField] private bool headlight = false;

    private LedRingController.LedEffect lastEffect;
    private Color32 lastColor;
    private bool lastHeadlight;

    // Use this for initialization
    void Start()
    {
        dronesManager = GameObject.Find("DronesManager").GetComponent<DronesManager>();
        lastEffect = effect;
        lastColor = color;
        lastHeadlight = headlight;
    }

    // Update is called once per frame
    void Update () {
        IEnumerable<LedRingController> ledRings = dronesManager.GetDronesGameObjects()
            .Select((GameObject d) => d.GetComponent<LedRingController>())
            .Where((LedRingController lrc) => lrc != null);

        if (effect != lastEffect)
        {
            lastEffect = effect;
            foreach (LedRingController ledRing in ledRings)
                ledRing.effect = effect;
        }

        if ((Color)color != (Color)lastColor)
        {
            lastColor = color;
            foreach (LedRingController ledRing in ledRings)
                ledRing.color = color;
        }

        if (headlight != lastHeadlight)
        {
            lastHeadlight = headlight;
            foreach (LedRingController ledRing in ledRings)
                ledRing.headlight = headlight;
        }


    }
}
