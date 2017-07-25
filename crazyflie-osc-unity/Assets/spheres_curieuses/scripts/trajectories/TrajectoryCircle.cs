using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryCircle : TrajectoryMultiLines {

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
    }

    protected override void onStart()
    {
        if (this.hasVariables("center", "normal_vector", "radius", "time"))
        {
            Vector3 center = this.getVariable<Vector3>("center");
            Vector3 y_axis = this.getVariable<Vector3>("normal_vector").normalized;
            float radius = this.getVariable<float>("radius");

            // TODO : Add a way to choose the beginning, instead of random (closest point from the drone ?)
            Vector3 x_axis = MathUtils.Cross(y_axis, new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100))).normalized;
            Vector3 z_axis = MathUtils.Cross(x_axis, y_axis).normalized;

            int iterNumber = (int)((Mathf.PI * 2.0f * radius) * 10.0f);
            List<Vector3> points = new List<Vector3>(iterNumber);

            for (int i = 0; i < iterNumber; ++i)
            {
                points.Add(
                    radius * (
                        Mathf.Cos((float)i * (Mathf.PI * 2) / iterNumber) * x_axis
                        + Mathf.Sin((float)i * (Mathf.PI * 2) / iterNumber) * z_axis)
                    + center);
            }

            this.setVariable("loop", true);
            this.setVariable("positions", points);
            print(points);
        }

        // call MultiLine onStart
        base.onStart();
    }
}
