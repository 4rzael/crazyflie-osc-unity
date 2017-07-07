using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

[ExecuteInEditMode]
public class LpsManager : MonoBehaviour {

	public GameObject lpsNodePrefab;
	public Vector3[] lpsNodesPositions;
	public string lpsOscTopic = "/lps";

	private List<GameObject> _lpsNodes;
	private OscManager oscManager;
	private UnityOSC.OSCClient oscClient;


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


	private void findNodes () {
		this._lpsNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("LpsNode"));
		this._lpsNodes.Sort (LpsNode.compareNodes);
	}

	/* PUBLIC METHODS */

	public void removeNodes () {
		foreach (GameObject node in this._lpsNodes)
		{
			if (node != null) {
				GameObject.DestroyImmediate (node);
			}
		}
		this._lpsNodes.Clear ();
	}
		

	public void recreateNodes () {
		this.removeNodes ();
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

	public void sendConfigOsc () {
		this.printNodes ();
		int id = 0;
		this.oscManager.sendOscMessage (this.oscClient, string.Format("{0}/get_node_number", this.lpsOscTopic), this.lpsNodesPositions.Length);
		foreach (Vector3 position in this.lpsNodesPositions) {
			string topic = string.Format ("{0}/{1}/set_position", this.lpsOscTopic, id.ToString ());
			this.oscManager.sendOscMessage (this.oscClient, topic, position.x, position.z, position.y);
			++id;
		}
	}

	public void savePositions () {
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
		this.oscClient = this.oscManager.createClient ("drones");

		if (Application.isPlaying) {
			if (this._lpsNodes.Count < this.lpsNodesPositions.Length)
				this.recreateNodes ();
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
