using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModeJuggleHorizontal : ControlMode {

    public ControlModeJuggleHorizontal(Controller c) : base(c) { }

    public override void start()
    {
        base.start();

        trajectoryManager.setTrajectory(TrajectoryManager.TrajectoryType.JUGGLE_HORIZONTAL);
    }


    public override void end()
    {
        base.end();

        Vector3 velocity = _controller.velocity;
        trajectoryManager.getTrajectory().setVariable("hand_vector", velocity);
        trajectoryManager.getTrajectory().startTrajectory();
    }

    override public void update(Dictionary<string, GloveInput> gloveInputs)
    {
    }
}
