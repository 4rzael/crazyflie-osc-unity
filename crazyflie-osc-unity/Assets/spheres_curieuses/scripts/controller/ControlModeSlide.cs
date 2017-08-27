using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModeSlide : ControlMode
{

    public ControlModeSlide(Controller c) : base(c) { }

    private float speedFunction(float t)
    {
        float speed = 3 * Mathf.Pow(1 - t, 2);
        return speed;
    }

    public override void start()
    {
        base.start();
        Debug.Log("Slide start");

        trajectoryManager.setTrajectory(TrajectoryManager.TrajectoryType.MULTI_LINES);
        trajectoryManager.getTrajectory().setVariable("loop", false);
        trajectoryManager.getTrajectory().setVariable<Trajectory.SpeedFunction>("speed_function", speedFunction); // slow down trajectory
    }


    public override void end()
    {
        base.end();
        Debug.Log("Slide end");

        Vector3 velocity = _controller.velocity;
        trajectoryManager.getTrajectory().setVariable("positions", new List<Vector3> {
           _controller.GetCurrentDrone().transform.position,
           _controller.GetCurrentDrone().transform.position + velocity,
        });
        trajectoryManager.getTrajectory().setVariable<float>("time", velocity.magnitude);
        trajectoryManager.getTrajectory().setVariable<Trajectory.SpeedFunction>("speed_function", speedFunction); // slow down trajectory
        trajectoryManager.getTrajectory().startTrajectory();
    }

    override public void update(Dictionary<string, GloveInput> gloveInputs)
    {
    }
}
