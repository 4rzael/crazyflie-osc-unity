using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryHover : Trajectory {

    private bool firstTimeCall = true;

	void Update () {
        if (this.started && this.hasVariables("position") && firstTimeCall)
        {
            firstTimeCall = false;
            transform.position = getVariable<Vector3>("position");
        }
	}
}
