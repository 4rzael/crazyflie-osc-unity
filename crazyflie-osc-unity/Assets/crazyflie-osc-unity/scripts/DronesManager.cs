using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class DronesManager : MonoBehaviour {
    #region Public properties
    public GameObject dronePrefab;
	public string droneOscTopic = "/crazyflie";
	public DroneConfig[] dronesConfigs;
    #endregion


    #region Private properties
    private List<GameObject> _dronesGameObjects;
    private bool _createdDrones = false;
    #endregion


    #region Basic drone management methods
    /// <summary>
    /// Destroys the drones GameObjects.
    /// </summary>
    public void DestroyDrones() {
        foreach (GameObject drone in this._dronesGameObjects)
        {
            if (drone != null)
            {
                GameObject.DestroyImmediate(drone);
            }
        }
        this._dronesGameObjects.Clear();
    }

    /// <summary>
    /// Destroys the drones if created by this manager.
    /// </summary>
    public void DestroyDronesIfCreated()
    {
        if (_createdDrones)
            DestroyDrones();
    }

    /// <summary>
    /// Recreates the drones.
    /// </summary>
    public void RecreateDrones() {
        this.DestroyDrones();
        int id = 0;
        foreach (DroneConfig droneConfig in dronesConfigs)
        {
            if (droneConfig.is_active)
            {
                GameObject go = GameObject.Instantiate(dronePrefab, Vector3.zero, Quaternion.identity);
                go.name = string.Format("Drone_{0}", id.ToString());
                Drone drone = go.GetComponent<Drone>();
                drone.Initialize(id, droneConfig.radio_uri, this.droneOscTopic);
                drone.SetColor(droneConfig.color);
                this._dronesGameObjects.Add(go);
            }
            ++id;
        }
        _createdDrones = true;
    }
    #endregion


    #region Drones getters
    /// <summary>
    /// Gets the drones game objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<GameObject> GetDronesGameObjects() {
        CleanDronesList();
        return _dronesGameObjects;
    }
    /// <summary>
    /// Gets the drones.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Drone> GetDrones() {
        return GetDronesGameObjects().Select(go => go.GetComponent<Drone>());
    }
    /// <summary>
    /// Gets the drone game object by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public GameObject GetDroneGameObjectById(int id) {
        return GetDronesGameObjects().FirstOrDefault(go => go.GetComponent<Drone>().Id == id);
    }
    /// <summary>
    /// Gets the drone by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public Drone GetDroneById(int id) {
        return GetDroneGameObjectById(id).GetComponent<Drone>();
    }
    #endregion


    # region Private utils
    /// <summary>
    /// Finds the drones in the current scene.
    /// </summary>
    private void FindDrones() {
        this._dronesGameObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Drone"));
        this._dronesGameObjects.Sort(Drone.CompareDrones);
    }

    /// <summary>
    /// Cleans the drones list by removing null references.
    /// </summary>
    private void CleanDronesList () {
		this._dronesGameObjects.RemoveAll (n => n == null);
	}
    /// <summary>
    /// Gets the active drone configs.
    /// </summary>
    /// <returns>The list of active configs</returns>
    private IEnumerable<DroneConfig> GetActiveDroneConfigs()
    {
        return dronesConfigs.Where(dc => dc.is_active == true);
    }
    #endregion


    #region Proxies to Drone methods
    /// <summary>
    /// PROXY : Asks every drone to connect.
    /// </summary>
    public void ConnectDrones () {
		this.CleanDronesList ();
        Debug.Log(GetDrones().Count());
		foreach (Drone drone in GetDrones()) {
			drone.Connect ();
		}
	}

    /// <summary>
    /// PROXY : Asks every drone to reset their kalman filter.
    /// </summary>
    public void ResetKalmanFilters () {
		this.CleanDronesList ();
        foreach (Drone drone in GetDrones())
        {
            drone.ResetKalmanFilter();
        }
    }

    /// <summary>
    /// PROXY : Asks every drone to start the position sync.
    /// </summary>
    public void StartPositionSync() {
		this.CleanDronesList ();
        foreach (Drone drone in GetDrones())
        {
            drone.StartPositionSync();
        }
    }

    /// <summary>
    /// PROXY : Asks every drone to stop the position sync.
    /// </summary>
    public void StopPositionSync() {
		this.CleanDronesList ();
        foreach (Drone drone in GetDrones())
        {
            drone.StopPositionSync();
        }
    }

    /// <summary>
    /// PROXY : Asks every drone to send emergency signals.
    /// </summary>
    public void SendEmergencySignals()
    {
        this.CleanDronesList();
        foreach (Drone drone in GetDrones())
        {
            drone.SendEmergencySignal();
        }
    }
    #endregion


    #region Unity specific methods
    public void Awake () {
		this.FindDrones ();

		if (Application.isPlaying) {
			if (this._dronesGameObjects.Count < GetActiveDroneConfigs().Count())
				this.RecreateDrones ();
		}

	}
    #endregion
}
