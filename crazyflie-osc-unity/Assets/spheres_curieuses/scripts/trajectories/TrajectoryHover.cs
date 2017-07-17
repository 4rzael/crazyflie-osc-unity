using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryHover : Trajectory {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (this.hasVariables("position"))
        {
            transform.position = (Vector3)(_variables["position"]);
        }
	}
}
