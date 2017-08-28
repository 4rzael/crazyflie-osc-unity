using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModeTakeoff : ControlMode
{
    public ControlModeTakeoff(Controller c) : base(c) { }

    public override void start()
    {
        base.start();
        trajectoryManager.setTrajectory(TrajectoryManager.TrajectoryType.TAKEOFF);
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
