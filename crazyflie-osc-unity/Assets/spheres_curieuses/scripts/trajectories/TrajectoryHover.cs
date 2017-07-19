using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryHover : Trajectory {

	// Update is called once per frame
	void Update () {
        if (this.started && this.hasVariables("position"))
        {
            transform.position = (Vector3)(_variables["position"]);
        }
	}
}
