using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityOSC;

public class PidPositionController : MonoBehaviour {

    public Dictionary<string, float> variables = new Dictionary<string, float>();
    private OscManager oscManager;
    private OSCClient _oscClient;

    private Coroutine _askForMoreParams = null;
    private bool shouldStopCoroutine = false;

	// Use this for initialization
	void Start () {
        oscManager = GameObject.Find("OscManager").GetComponent<OscManager>();

        string toSubscribe = "/param/" + gameObject.GetComponent<Drone>().id.ToString() + "/posCtlPid/{variable_name}";

        Debug.Log(toSubscribe);
        oscManager.OscSubscribe(toSubscribe, onVariable);
        _oscClient = oscManager.createClient("drones");

        _askForMoreParams = StartCoroutine(askForParams());
	}

    private void onVariable(string topic, OSCPacket packet, GroupCollection path_args)
    {
        float value = float.Parse(packet.Data[0].ToString());

        variables[path_args["variable_name"].Value] = value;
        shouldStopCoroutine = true;
    }

    public void setVariable(string key, float value)
    {
        variables[key] = value;
        oscManager.sendOscMessage(_oscClient, string.Format("/param/{0}/posCtlPid/{1}/set", gameObject.GetComponent<Drone>().id, key), value);
    }

    IEnumerator askForParams()
    {
        yield return new WaitForSeconds(10);
        Debug.Log("ASKING FOR MORE INFORMATION ON PARAMS");
        oscManager.sendOscMessage(_oscClient, string.Format("/param/{0}/get_all_values", gameObject.GetComponent<Drone>().id));
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
