﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Maestro : MonoBehaviour {

    public bool runChoregraphy = false;

    private bool firstTimeSinceSceneInit = true;

    private DronesManager _dronesManager;
    private SceneManager _sceneManager;

	// Use this for initialization
	void Start () {
        _dronesManager = GameObject.Find("DronesManager").GetComponent<DronesManager>();
        _sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
    }


    // Update is called once per frame
	void Update () {
        if (firstTimeSinceSceneInit && _sceneManager.isInitialized())
        {
            firstTimeSinceSceneInit = false;
            if (runChoregraphy)
                this.startChoregraphy();
        }
	}

    public void startChoregraphy()
    {
        if (_sceneManager.isInitialized() == false)
            return;

        IEnumerable<TrajectoryManager> drones = _dronesManager.GetDrones().Select(d => d.GetComponent<TrajectoryManager>());
        foreach (TrajectoryManager drone in drones)
        {
            //drone.setTrajectory(TrajectoryManager.TrajectoryType.HOVER);
            //drone.getTrajectory().setVariable("position", new Vector3(10f, 10f, 10f));

            //drone.setTrajectory(TrajectoryManager.TrajectoryType.MULTI_LINES);
            //drone.getTrajectory().setVariable("positions", new List<Vector3> {
            //    new Vector3 (1f, 1f, 1f),
            //    new Vector3 (3f, 1f, 1f),
            //    new Vector3 (2f, 3f, 1f),
            //});
            //drone.getTrajectory().setVariable("time", 5.0f);
            //drone.getTrajectory().setVariable("loop", true);
            ////drone.getTrajectory().setVariable<Trajectory.SpeedFunction>("speed_function", x => 3 * Mathf.Pow(x, 2));
            //drone.getTrajectory().startTrajectory();

            //drone.setTrajectory(TrajectoryManager.TrajectoryType.JUGGLE_3D);
            //drone.getTrajectory().setVariable("base_x_vector", new Vector3(1f, 0f, 0f));
            //drone.getTrajectory().setVariable("hand_vector", new Vector3(1f, 5f, 10f));
            //drone.getTrajectory().setVariable("time_multiplier", 0.5f);
            //drone.getTrajectory().startTrajectory();

            //            drone.setTrajectory(TrajectoryManager.TrajectoryType.CIRCLE);
            //            drone.getTrajectory().setVariable("center", new Vector3(2f, 2f, 2f));
            //            drone.getTrajectory().setVariable("normal_vector", new Vector3(0f, 1f, 0f));
            //            drone.getTrajectory().setVariable("radius", 0.75f);
            //            drone.getTrajectory().setVariable("time", 5.0f);
            ////            drone.getTrajectory().setVariable<Trajectory.SpeedFunction>("speed_function", x => 3 * Mathf.Pow(x, 2));
            //            drone.getTrajectory().startTrajectory();

            drone.setTrajectory(TrajectoryManager.TrajectoryType.TAKEOFF);
            drone.getTrajectory().startTrajectory();

            //drone.setTrajectory(TrajectoryManager.TrajectoryType.JUGGLE_HORIZONTAL);
            //drone.getTrajectory().setVariable("hand_vector", new Vector3(1f, 5f, 10f));
            //drone.getTrajectory().setVariable("time_multiplier", 0.5f);
            //drone.getTrajectory().startTrajectory();
        }
    }
}
