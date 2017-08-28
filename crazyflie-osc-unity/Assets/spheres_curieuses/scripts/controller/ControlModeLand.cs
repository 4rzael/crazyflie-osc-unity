using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModeLand : ControlMode
{
    public ControlModeLand(Controller c) : base(c) { }

    public override void start()
    {
        base.start();
        trajectoryManager.setTrajectory(TrajectoryManager.TrajectoryType.LAND);
    }


    public override void end()
    {
        base.end();

        trajectoryManager.getTrajectory().startTrajectory();
    }

    override public void update(Dictionary<string, GloveInput> gloveInputs)
    {
    }
}
