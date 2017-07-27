using System.Collections.Generic;

public abstract class ControlMode {

    Controller _controller;
    bool _ended = false;

    public ControlMode(Controller controller)
    {
        _controller = controller;
    }

    public virtual void start() { }

    public virtual void end() { _ended = true; }
    public virtual bool hasEnded() { return _ended; }

    public abstract void update(Dictionary<string, GloveInput> inputs);
}
