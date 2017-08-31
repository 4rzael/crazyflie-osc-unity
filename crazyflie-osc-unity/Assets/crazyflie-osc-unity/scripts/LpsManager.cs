using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityOSC;

/// <summary>
/// Manages the LPS system.
/// Creates GameObjects to represent anchors in Unity, and sends their position to the OSC server.
/// </summary>
[ExecuteInEditMode]
public class LpsManager : MonoBehaviour {

	public GameObject lpsNodePrefab;
	public Vector3[] lpsNodesPositions;
	public string lpsOscTopic = "/lps";

	private List<GameObject> _lpsNodes;
	private OscManager oscManager;
	private UnityOSC.OSCClient oscClient;

    /// <summary>
    /// Cleans the node list by removing null references.
    /// </summary>
    private void cleanNodeList () {
		this._lpsNodes.RemoveAll (n => n == null);
	}

	private void printNodes () {
		this.cleanNodeList ();
		foreach(GameObject node in _lpsNodes)
		{
			Debug.Log(node.GetComponent<Transform>().position);
		}
	}


    /// <summary>
    /// Finds the nodes GameObjects in the current scene.
    /// </summary>
    private void findNodes () {
		this._lpsNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("LpsNode"));
		this._lpsNodes.Sort (LpsNode.compareNodes);
	}

    /* PUBLIC METHODS */

    /// <summary>
    /// Gets the node by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public GameObject GetNodeById(int id)
    {
        return _lpsNodes[id];
    }

    /// <summary>
    /// Removes the LPS nodes GameObjects.
    /// </summary>
    public void RemoveNodes () {
		foreach (GameObject node in this._lpsNodes)
		{
			if (node != null) {
				GameObject.DestroyImmediate (node);
			}
		}
		this._lpsNodes.Clear ();
	}


    /// <summary>
    /// Recreates the LPS nodes from this manager configuration.
    /// </summary>
    public void RecreateNodes () {
		this.RemoveNodes ();
		int id = 0;
		foreach (Vector3 nodePosition in this.lpsNodesPositions)
		{
			GameObject go = GameObject.Instantiate (lpsNodePrefab, nodePosition, Quaternion.identity);
			go.name = string.Format("LpsNode_{0}", id.ToString ());
			go.GetComponent<LpsNode> ().id = id;
			this._lpsNodes.Add (go);
			++id;
		}
	}

    /// <summary>
    /// Sends the LPS configuration through OSC.
    /// </summary>
    public void SendConfigOsc () {
		this.printNodes ();
		int id = 0;
		this.oscManager.SendOscMessage (this.oscClient, string.Format("{0}/get_node_number", this.lpsOscTopic), this.lpsNodesPositions.Length);
		foreach (Vector3 position in this.lpsNodesPositions) {
			string topic = string.Format ("{0}/{1}/set_position", this.lpsOscTopic, id.ToString ());
			this.oscManager.SendOscMessage (this.oscClient, topic, position.x, position.z, position.y);
			++id;
		}
	}

    /// <summary>
    /// Get the GameObjects current positions and save them in this manager.
    /// </summary>
    public void SavePositions () {
		int i = 0;
		this.cleanNodeList ();
		foreach (GameObject node in this._lpsNodes) {
			this.lpsNodesPositions[i] = node.GetComponent<Transform> ().position;
			++i;
		}
	}

	// Use this for initialization
	public void Awake () {
		this.findNodes ();

		this.oscManager = GameObject.Find ("OscManager").GetComponent<OscManager> ();
		this.oscClient = this.oscManager.CreateClient ("drones");

		if (Application.isPlaying) {
			if (this._lpsNodes.Count < this.lpsNodesPositions.Length)
				this.RecreateNodes ();
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
