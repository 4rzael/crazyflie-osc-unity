using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followHand : MonoBehaviour {

    public int torsoId;
    public int handId;
    public int droneId;

    private GameObject torso;
    private GameObject hand;
    private GameObject drone;

    public float difference = 1f;

    private DronesManager dronesManager;

    // Use this for initialization
	void Start () {
        dronesManager = GameObject.Find("DronesManager").GetComponent<DronesManager>();

        torso = dronesManager.GetDroneGameObjectById(torsoId);
        hand = dronesManager.GetDroneGameObjectById(handId);
        drone = dronesManager.GetDroneGameObjectById(droneId);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 handPos = hand.GetComponent<Drone>().GetRealPositionMarker().transform.position;
        Vector3 torsoPos = torso.GetComponent<Drone>().GetRealPositionMarker().transform.position - new Vector3(0, -0.2f, 0); // -0.2f is because I put the drone on my head
        Vector3 handVector = handPos - torsoPos;
        Debug.DrawLine(torsoPos, handPos, Color.red);
        drone.transform.position = handPos + handVector * difference;
	}
}
