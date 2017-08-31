using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModeFollowHand : ControlMode
{

    public ControlModeFollowHand(Controller c) : base(c) { }

    public override void start()
    {
        base.start();

        trajectoryManager.setTrajectory(TrajectoryManager.TrajectoryType.FOLLOW_POINT);
        trajectoryManager.getTrajectory().startTrajectory();
    }


    public override void end()
    {
        base.end();
        trajectoryManager.getTrajectory().stopTrajectory();
    }

    override public void update(Dictionary<string, GloveInput> gloveInputs)
    {
        trajectoryManager.getTrajectory().setVariable("point",
            _controller.GetHandDrone().GetComponent<Drone>().GetRealPosition() + _controller.pointingVector);
    }
}
