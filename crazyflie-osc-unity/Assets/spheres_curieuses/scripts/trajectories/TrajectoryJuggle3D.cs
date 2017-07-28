using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class TrajectoryJuggle3D : Trajectory {

    private Vector3 x_z_plan_normal;
    private Vector3 x_y_plan_normal;

    private Matrix<float> matrix_unrotate;
    // private Matrix<float> matrix_rotate;

    private Vector3 gravityVector;

    private Vector3 speed;

    private Vector3 initialPosition;

    // Use this for initialization
    protected override void Awake ()
    {
        base.Awake();

        this.addSetter<float>("gravity", this.GravitySetter);
    }

    protected override void onStart()
    {
        if (this.hasVariables("base_x_vector", "hand_vector"))
        {
            Vector3 hand_vector = this.getVariable<Vector3>("hand_vector");
            Vector3 base_x_vector = this.getVariable<Vector3>("base_x_vector").normalized;
            float gravity = this.getVariable("gravity", -10f);

            // compute working space
            x_z_plan_normal = Vector3.Cross(base_x_vector, hand_vector).normalized;
            x_y_plan_normal = Vector3.Cross(x_z_plan_normal, base_x_vector).normalized;

            // compute rotation matrices
            matrix_unrotate = MathUtils.MatrixFromVectors(base_x_vector, x_y_plan_normal, x_z_plan_normal);
            // // matrix_rotate = matrix_unrotate.Inverse();

            // compute gravity vector
            computeGravityVector(gravity);

            // set common values
            this.initialPosition = transform.position;
            this.speed = hand_vector;
        }
    }
	
    private void computeGravityVector(float g)
    {
        this.gravityVector = MathUtils.MathNetVectorsToUnity(matrix_unrotate.Row(1) * g);
        Debug.LogFormat("Gravity computed : {0}", this.gravityVector);
    }

    private bool GravitySetter(float g) {
        if (this.started) {
            this.computeGravityVector(g);
        }
        return true;
    }

    private bool shouldStop()
    {
        var currentHeight = -MathUtils.UnityVectorsToMathNet(transform.position).DotProduct(MathUtils.UnityVectorsToMathNet(gravityVector));
        var initialHeight = -MathUtils.UnityVectorsToMathNet(initialPosition).DotProduct(MathUtils.UnityVectorsToMathNet(gravityVector));

        return currentHeight < initialHeight;
    }

    // Update is called once per frame
    void FixedUpdate () {
	    if (this.started && this.hasVariables("base_x_vector", "hand_vector"))
        {
            float timeMultiplier = this.getVariable("time_multiplier", 1.0f);
            this.speed += this.gravityVector * Time.fixedDeltaTime * timeMultiplier;
            transform.position += this.speed * Time.fixedDeltaTime * timeMultiplier;

            if (this.shouldStop())
            {
                this.stopTrajectory();
            }
        }
    }

    public override void render()
    {
        base.render();

    }
}
