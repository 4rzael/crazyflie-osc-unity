using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Maestro : MonoBehaviour {

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
            this.startChoregraphy();
        }
	}

    public void startChoregraphy()
    {
        if (_sceneManager.isInitialized() == false)
            return;

        IEnumerable<TrajectoryManager> drones = _dronesManager.getDrones().Select(d => d.GetComponent<TrajectoryManager>());
        foreach (TrajectoryManager drone in drones)
        {
            drone.setTrajectory(TrajectoryManager.TrajectoryType.HOVER);
            drone.getTrajectory().setVariable("position", new Vector3(10f, 10f, 10f));
        }
    }
}
