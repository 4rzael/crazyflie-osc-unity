using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModeJuggle : ControlMode {

    public ControlModeJuggle(Controller c) : base(c) { }

    public override void start()
    {
        base.start();
        Debug.Log("Juggle 3D start");
    }

    public override void end()
    {
        base.end();
        Debug.Log("Juggle 3D end");
    }

    override public void update(Dictionary<string, GloveInput> gloveInputs)
    {
        Debug.Log("Juggle 3D update");
    }
}
