﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour {

    [Serializable]
    public enum TrajectoryType
    {
        HOVER = 0,
    }

    private Dictionary<TrajectoryType, Type> _possibleTrajectories = new Dictionary<TrajectoryType, Type>
        {
            {TrajectoryType.HOVER, typeof(TrajectoryHover)},
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