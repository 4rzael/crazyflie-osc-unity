using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModeJuggleHorizontal : ControlMode {

    private TrajectoryManager trajectoryManager;

    public ControlModeJuggleHorizontal(Controller c) : base(c) { }

    public override void start()
    {
        base.start();
        Debug.Log("Juggle 3D start");

        trajectoryManager = _controller.getCurrentDroneTrajectoryManager();
        trajectoryManager.setTrajectory(TrajectoryManager.TrajectoryType.JUGGLE_HORIZONTAL);
    }


    public override void end()
    {
        Vector3 velocity = _controller.velocity;
        trajectoryManager.getTrajectory().setVariable("hand_vector", velocity);
        trajectoryManager.getTrajectory().startTrajectory();

        base.end();
        Debug.Log("Juggle 3D end");
    }

    override public void update(Dictionary<string, GloveInput> gloveInputs)
    {
    }
}
