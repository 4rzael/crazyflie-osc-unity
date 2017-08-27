using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControlModeSampler : ControlModeFollowHand
{
    private List<TrajectoryMultiPoints.TimedPoint> points = new List<TrajectoryMultiPoints.TimedPoint>();
    private float lastRecordTimestamp;

    public ControlModeSampler(Controller c) : base(c) { }

    public override void start()
    {
        base.start();
        Record();
    }

    public override void end()
    {
        base.end();

        Debug.LogFormat("SAMPLING STOPPED, STARTING LOOPING {0}", points.Count());

        Trajectory t = trajectoryManager.setTrajectory(TrajectoryManager.TrajectoryType.MULTI_POINTS);
        t.setVariable("loop", true);
        t.setVariable("timed_points", points);

        t.startTrajectory();
    }

    override public void update(Dictionary<string, GloveInput> gloveInputs)
    {
        base.update(gloveInputs);

        if (Time.time - lastRecordTimestamp >= 0.1f)
            Record();
    }

    protected void Record()
    {
        Vector3 pt = trajectoryManager.getTrajectory().getVariable<Vector3>("point", new Vector3(-1, -1, -1));
        if (pt != new Vector3(-1, -1, -1))
        {
            points.Add(new TrajectoryMultiPoints.TimedPoint(pt));
            lastRecordTimestamp = Time.time;
        }
    }
}
