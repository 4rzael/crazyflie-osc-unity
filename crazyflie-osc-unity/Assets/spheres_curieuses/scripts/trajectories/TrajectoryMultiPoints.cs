using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrajectoryMultiPoints : TrajectoryFollowPoint
{

    public struct TimedPoint
    {
        public float timestamp;
        public Vector3 point;

        public TimedPoint(Vector3 p) { timestamp = Time.time; point = p; }
    }

    private GameObject _renderer;
    private List<TimedPoint> _points;

    private float sequenceBeginTimestamp;

    public override void render()
    {
        if (this._points == null) return;
        if (_renderer == null)
        {
            _renderer = new GameObject("line_renderer", typeof(LineRenderer));
        }
        LineRenderer line = _renderer.GetComponent<LineRenderer>();
        line.positionCount = this._points.Count();
        line.SetPositions(this._points.Select(pt => pt.point).ToArray());
        line.widthCurve = AnimationCurve.Linear(0, 0.1f, 1, 0.1f);
        line.material = gameObject.GetComponentInChildren<Renderer>().material;
        line.loop = this.getVariable<bool>("loop", false);
    }

    public override void unrender()
    {
        if (_renderer != null)
        {
            GameObject.Destroy(_renderer);
            _renderer = null;
        }
    }

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void onStart()
    {
        base.onStart();

        if (hasVariables("timed_points"))
        {
            _points = new List<TimedPoint>(getVariable<IEnumerable<TimedPoint>>("timed_points"));
            _points = new List<TimedPoint>(_points.Select((pt) =>
            {
                pt.timestamp -= _points[0].timestamp;
                return pt;
            }));

            bool loop = getVariable("loop", false);
            const float speed = 1.0f;
            if (loop)
            {
                TimedPoint loopPoint;
                loopPoint.point = _points[0].point;
                loopPoint.timestamp = _points[_points.Count() - 1].timestamp + speed * (_points[0].point - _points[_points.Count() - 1].point).magnitude;
                _points.Add(loopPoint);
            }
            sequenceBeginTimestamp = Time.time;
        }
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (this.started && this.hasVariables("timed_points"))
        {
            bool loop = getVariable("loop", false);

            TimedPoint nextPoint;
            try
            {
                nextPoint = _points.Where(pt => pt.timestamp >= Time.time - sequenceBeginTimestamp).First();
                setVariable("point", nextPoint.point);
            }
            catch(System.Exception) {
                if (loop)
                    sequenceBeginTimestamp = Time.time;
                else
                    stopTrajectory();
            }
        }
        base.FixedUpdate();
    }

    protected override void onStop()
    {
        base.onStop();
    }
}
