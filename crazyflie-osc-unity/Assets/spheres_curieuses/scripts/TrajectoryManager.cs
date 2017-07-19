using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour {

    [Serializable]
    public enum TrajectoryType
    {
        HOVER = 0,
        MULTI_LINES = 2,
        JUGGLE_3D = 3,
    }

    private Dictionary<TrajectoryType, Type> _possibleTrajectories = new Dictionary<TrajectoryType, Type>
        {
            {TrajectoryType.HOVER, typeof(TrajectoryHover)},
            { TrajectoryType.MULTI_LINES, typeof(TrajectoryMultiLines)},
            { TrajectoryType.JUGGLE_3D, typeof(TrajectoryJuggle3D)}
        };

    private Trajectory currentTrajectory;

    public void removeTrajectories()
    {
        foreach (Trajectory traj in gameObject.GetComponents<Trajectory>())
        {
            Destroy(traj);
        }
    }

    public void setTrajectory(TrajectoryType type)
    {
        this.removeTrajectories();
        if (_possibleTrajectories.ContainsKey(type))
        {
            gameObject.AddComponent(_possibleTrajectories[type]);
        }
    }

    public Trajectory getTrajectory()
    {
        return gameObject.GetComponent<Trajectory>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
