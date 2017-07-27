using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryJuggleHorizontal : TrajectoryJuggle3D {

    protected override void Awake()
    {
        base.Awake();

        this.addSetter<Vector3>("hand_vector", this.HandVectorSetter);
        this.setVariable("base_x_vector", Vector3.right);
    }

    bool HandVectorSetter(Vector3 vec)
    {
        Vector3 horizontal_projection = new Vector3(vec.x, 0, vec.z);
        this._variables["hand_vector"] = horizontal_projection;

        return false;
    }
}
