using System.Collections.Generic;
using UnityEngine;

public class ControlMode {

    protected Controller _controller;
    bool _ended = false;

    protected TrajectoryManager trajectoryManager;


    public ControlMode(Controller controller)
    {
        _controller = controller;
    }

    public virtual void start() {
        trajectoryManager = _controller.getCurrentDroneTrajectoryManager();
    }

    public virtual void end() { _ended = true; }
    public virtual bool hasEnded() { return _ended; }

    public virtual void update(Dictionary<string, GloveInput> inputs) { }
}
