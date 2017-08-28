using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityOSC;

public class LpsNodeVerifier : MonoBehaviour {
    private bool firstTimeSinceSceneInit = true;
    private SceneManager _sceneManager;
    private LpsManager _lpsManager;

    private OscManager _oscManager;
    private OSCClient _oscClient;

    // Use this for initialization
    void Start () {
        _oscManager = GameObject.Find("OscManager").GetComponent<OscManager>();

        _sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        _lpsManager = GameObject.Find("LpsManager").GetComponent<LpsManager>();

        _oscClient = _oscManager.createClient("drones");

    }

    float[] distances =  {
        float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity,
        float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity };

	// Update is called once per frame
	void Update () {
        if (firstTimeSinceSceneInit && _sceneManager.isInitialized())
        {
            firstTimeSinceSceneInit = false;
            _oscManager.SendOscMessage(_oscClient, "/log/0/add", "tdoa1", 1000);
            _oscManager.SendOscMessage(_oscClient, "/log/0/add", "tdoa2", 1000);
            _oscManager.SendOscMessage(_oscClient, "/log/0/tdoa1/add_variable", "tdoa.d0", "float");
            _oscManager.SendOscMessage(_oscClient, "/log/0/tdoa1/add_variable", "tdoa.d1", "float");
            _oscManager.SendOscMessage(_oscClient, "/log/0/tdoa1/add_variable", "tdoa.d2", "float");
            _oscManager.SendOscMessage(_oscClient, "/log/0/tdoa1/add_variable", "tdoa.d3", "float");
            _oscManager.SendOscMessage(_oscClient, "/log/0/tdoa2/add_variable", "tdoa.d4", "float");
            _oscManager.SendOscMessage(_oscClient, "/log/0/tdoa2/add_variable", "tdoa.d5", "float");
            _oscManager.SendOscMessage(_oscClient, "/log/0/tdoa2/add_variable", "tdoa.d6", "float");
            _oscManager.SendOscMessage(_oscClient, "/log/0/tdoa2/add_variable", "tdoa.d7", "float");
            _oscManager.SendOscMessage(_oscClient, "/log/0/tdoa1/start");
            _oscManager.SendOscMessage(_oscClient, "/log/0/tdoa2/start");

            _oscManager.OscSubscribe("/log/0/tdoa*/tdoa.d{var_id}",
                delegate (string topic, OSCPacket packet, System.Text.RegularExpressions.GroupCollection path_args)
                {
                    int varId = int.Parse(path_args["var_id"].Value);
                    float distance = (float)packet.Data[0];

                    Debug.LogFormat("DISTANCE TO NODE {0} : {1}", varId, distance);
                    distances[varId] = distance;
                });
        }

        int minIndex = Enumerable.Range(0, distances.Length)
    .Aggregate((a, b) => (distances[a] < distances[b]) ? a : b); // returns 2

        Debug.Log(minIndex);

        GameObject node = _lpsManager.getNodeById(minIndex);

        foreach (GameObject g in Enumerable.Range(0, distances.Length).Where(i => i != minIndex).Select(i => _lpsManager.getNodeById(i)))
        {
            g.GetComponent<Renderer>().material.color = Color.blue;
        }
        node.GetComponent<Renderer>().material.color = Color.red;

    }
}
