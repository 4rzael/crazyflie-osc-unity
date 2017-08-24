using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryTakeoff : TrajectoryHover
{
    // TODO: Ramp
    protected override void onStart()
    {
        base.onStart();

        Vector3 currentPosition = gameObject.GetComponent<Drone>().GetRealPosition();

        currentPosition.y = 1.5f;
        setVariable("position", currentPosition);
    }
}
