using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryFollowPoint : Trajectory
{
    void Update()
    {
        if (this.started && this.hasVariables("point"))
        {
            Vector3 currentPosition = transform.position;
            Vector3 direction = getVariable<Vector3>("point", currentPosition) - currentPosition;
            float maxSpeed = getVariable<float>("speed", 2f);
            transform.position += direction * Time.deltaTime * maxSpeed;
        }
    }
}
