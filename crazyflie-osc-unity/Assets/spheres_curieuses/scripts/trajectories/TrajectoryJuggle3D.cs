using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryJuggle3D : Trajectory {

    private Vector3 x_z_plan_normal;
    private Vector3 x_y_plan_normal;

    private Matrix4x4 matrix_unrotate;
    private Matrix4x4 matrix_rotate;

    // Use this for initialization
    protected override void Awake () {
        base.Awake();

        this.setVariable("gravity", -10f);
    }

    protected override void onStart()
    {
        if (this.hasVariables("base_x_vector", "hand_vector"))
        {
            Vector3 hand_vector = this.getVariable<Vector3>("hand_vector");
            Vector3 base_x_vector = this.getVariable<Vector3>("base_x_vector");
            x_z_plan_normal = Vector3.Cross(hand_vector, base_x_vector).normalized;
            x_y_plan_normal = Vector3.Cross(base_x_vector, x_z_plan_normal).normalized;
            // TODO : continue
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if (this.started && this.hasVariables("base_x_vector", "hand_vector"))
        {
            // TODO : continue
        }
	}
}
