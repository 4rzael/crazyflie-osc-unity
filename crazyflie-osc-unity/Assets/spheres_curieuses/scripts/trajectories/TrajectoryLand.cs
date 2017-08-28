using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLand : TrajectoryMultiLines
{
    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        Vector3 currentPosition = gameObject.GetComponent<Drone>().transform.position;
        Vector3 expectedPosition = new Vector3(currentPosition.x, 0, currentPosition.z);

        setVariable("positions", new List<Vector3>() { currentPosition, expectedPosition });
        setVariable("loop", false);
        setVariable("time", currentPosition.y);
    }
}
