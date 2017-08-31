using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryFollowPoint : Trajectory
{
    [SerializeField] private float maxSpeed;

    protected override void onStart()
    {
        maxSpeed = getVariable<float>("speed", 3f);
    }

    protected virtual void FixedUpdate()
    {
        if (this.started && this.hasVariables("point"))
        {
            Vector3 currentPosition = transform.position;
            Vector3 direction = getVariable<Vector3>("point", currentPosition) - currentPosition;
            transform.position += direction * Time.fixedDeltaTime * maxSpeed;
        }
    }
}
