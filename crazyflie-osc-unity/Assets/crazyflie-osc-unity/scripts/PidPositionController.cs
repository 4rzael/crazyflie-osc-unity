using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityOSC;

/// <summary>
/// A simple tool for debugging.
/// Should be put in the Drone class later.
/// Gives an interface to tune the values of the Position-Control PID in realtime.
/// </summary>
public class PidPositionController : MonoBehaviour {

    public Dictionary<string, float> variables = new Dictionary<string, float>();
    private OscManager oscManager;
    private OSCClient _oscClient;

    private Coroutine _askForMoreParams = null;
    private bool shouldStopCoroutine = false;

	// Use this for initialization
	void Start () {
        oscManager = GameObject.Find("OscManager").GetComponent<OscManager>();

        string toSubscribe = "/param/" + gameObject.GetComponent<Drone>().Id.ToString() + "/posCtlPid/{variable_name}";

        oscManager.OscSubscribe(toSubscribe, OnVariable);
        _oscClient = oscManager.createClient("drones");

        _askForMoreParams = StartCoroutine(AskForParams());
	}

    private void OnVariable(string topic, OSCPacket packet, GroupCollection path_args)
    {
        float value = float.Parse(packet.Data[0].ToString());

        variables[path_args["variable_name"].Value] = value;
        shouldStopCoroutine = true;
    }

    public void SetVariable(string key, float value)
    {
        variables[key] = value;
        gameObject.GetComponent<Drone>().SetParam("posCtlPid", key, value);
    }

    IEnumerator AskForParams()
    {
        yield return new WaitForSeconds(10);
        Debug.Log("ASKING FOR MORE INFORMATION ON PARAMS");
        oscManager.SendOscMessage(_oscClient, string.Format("/param/{0}/get_all_values", gameObject.GetComponent<Drone>().Id));
    }

    public void Update()
    {
        if (shouldStopCoroutine && _askForMoreParams != null)
        {
            StopCoroutine(_askForMoreParams);
            _askForMoreParams = null;
        }
    }
}
