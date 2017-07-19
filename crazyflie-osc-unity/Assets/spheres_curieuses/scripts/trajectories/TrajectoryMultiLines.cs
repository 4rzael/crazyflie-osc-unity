using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrajectoryMultiLines : Trajectory
{
    private float _loopStartTimestamp;
    private Vector3 _lastPosition;

    private float _currentDistance;
    private float _totalDistance;
    private List<Point> _points;

    private int _prevPointIdx;
    private int _nextPointIdx;

    private GameObject _renderer;

    private struct Point
    {
        public Vector3 position;
        public float distance;

        public Point(Vector3 p, float d) { position = p; distance = d; }
        public Point(Point pt) { position = pt.position; distance = pt.distance; }
        public override string ToString() { return string.Format("Point<{0} {1} {2}, \t {3}>", position.x, position.y, position.z, distance); }
    }

    public override void render()
    {
        if (this._points == null) return;
        if (_renderer == null)
        {
            _renderer = new GameObject("line_renderer", typeof(LineRenderer));
        }
        print(this._points.Select(pt => pt.ToString()).Aggregate((pt1, pt2) => pt1 + " " + pt2));
        print(this._points.Select(pt => pt.position).ToArray().Count());
        LineRenderer line = _renderer.GetComponent<LineRenderer>();
        line.positionCount = this._points.Count();
        line.SetPositions(this._points.Select(pt => pt.position).ToArray());
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

        this.setVariable("loop", false);
    }

    protected override void onStart()
    {
        base.onStart();

        this._points = new List<Point>();
        if (this.hasVariables("positions", "loop"))
        {
            IEnumerable<Vector3> positions = this.getVariable<IEnumerable<Vector3>>("positions");
            Vector3 lastPos = new Vector3(0,0,0);
            bool firstPoint = true;
            _totalDistance = 0;
            foreach (Vector3 newPos in positions)
            {
                if (firstPoint)
                {
                    firstPoint = false;
                    lastPos = newPos;
                }
                _totalDistance += (newPos - lastPos).magnitude;
                _points.Add(new Point(newPos, _totalDistance));
                lastPos = newPos;
            }
            if (this.getVariable<bool>("loop"))
            {
                _totalDistance += (_points[0].position - lastPos).magnitude;
                _points.Add(new Point(_points[0].position, _totalDistance));
            }
        }
        this._lastPosition = _points[0].position;
        this._prevPointIdx = 0;
        this._nextPointIdx = 1;
        this._loopStartTimestamp = Time.time;
    }

    private Point getLastPoint(float dist)
    {
        return _points.DefaultIfEmpty(_points[0]).Last(pt => pt.distance <= dist);
    }

    private Point getNextPoint(float dist)
    {
        return _points.DefaultIfEmpty(_points[0]).First(pt => pt.distance > dist);
    }

    private void setPosition(Vector3 pos)
    {
        this._lastPosition = pos;
        transform.position = pos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.started && this.hasVariables("time", "loop"))
        {
            float timeNeeded = this.getVariable<float>("time");
            bool loop = this.getVariable<bool>("loop");

            Point lastPoint = _points[_prevPointIdx];
            Point nextPoint = _points[_nextPointIdx];

            float speed = _totalDistance * this.getSpeedAtTime((Time.fixedTime  - this._loopStartTimestamp) / timeNeeded) / timeNeeded;
            Vector3 deltaPos = (nextPoint.position - lastPoint.position).normalized * speed * Time.fixedDeltaTime;
            Vector3 newPosition = this._lastPosition + deltaPos;

            this.setPosition(newPosition);

            _currentDistance = _currentDistance + deltaPos.magnitude * (Time.fixedDeltaTime > 0 ? 1 : -1);
            // if any trajectory boundary is crossed => end of trajectory
            if (_currentDistance >= _totalDistance)
            {
                this.setPosition(_points.Last().position);
                if (!loop) this.stopTrajectory();
            }
            else if (_currentDistance < 0)
            {
                this.setPosition(_points.Last().position);
                if (!loop) this.stopTrajectory();
            }

            // go to next point
            if (_currentDistance >= nextPoint.distance)
            {
                _prevPointIdx = _nextPointIdx;
                _nextPointIdx = (_nextPointIdx + 1 +_points.Count) % _points.Count;
            }
            // go to previous point
            else if (_currentDistance < lastPoint.distance)
            {
                _nextPointIdx = _prevPointIdx;
                _prevPointIdx = (_prevPointIdx != 0) ? _prevPointIdx - 1 : 0;
            }
            if (loop && (Time.fixedTime - this._loopStartTimestamp) / timeNeeded > 1.0f)
            {
                this._loopStartTimestamp += timeNeeded;
            }

            _currentDistance = (_currentDistance + _totalDistance) % _totalDistance;
        }
    }

    protected override void onStop()
    {
        base.onStop();
    }
}
