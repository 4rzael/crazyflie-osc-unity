using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityOSC;
using System.Reflection;
using System.Net;

public class OscManager : MonoBehaviour {
	[Serializable]
	public struct DistantOsc {
		public string name;
		public string ip;
		public short port;
	}

	public DistantOsc[] servers;

	public DistantOsc getOscDistantServer(string name) {
		return Array.Find<DistantOsc> (this.servers, s => s.name == name);
	}

	public void sendOscMessage(OSCClient client, string topic, params object[] values) {
		Debug.Log ("sending message on " + topic);
		OSCMessage msg = new OSCMessage (topic);
		foreach (object val in values) {
			Type val_type = val.GetType ();
			MethodInfo method = typeof(OSCMessage).GetMethod ("Append");
			MethodInfo generic = method.MakeGenericMethod (val_type);

			object[] valWrapped = new object[1];
			valWrapped [0] = val;
			generic.Invoke (msg, valWrapped);
		}
		client.Send (msg);
	}

	public OSCClient createClient(string server) {
		OscManager.DistantOsc oscDestination = this.getOscDistantServer (server);

		IPAddress ip = IPAddress.Parse(oscDestination.ip);
		return new UnityOSC.OSCClient (ip, (int)(oscDestination.port));
	}

}
