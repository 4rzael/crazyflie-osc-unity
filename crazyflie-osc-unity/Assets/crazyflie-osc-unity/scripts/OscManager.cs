using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityOSC;
using System.Reflection;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;

public class OscManager : MonoBehaviour {
	[Serializable]
	public struct DistantOsc {
		public string name;
		public string ip;
		public short port;
	}

	public delegate void OscSubscribeCallback(string topic, OSCPacket packet, GroupCollection path_args);
	private struct OscSubscriber {
		public Regex topicRegex;
		public OscSubscribeCallback callback;
	}

	public string localIP = "127.0.0.1";
	public short localPort = 6006;
	public DistantOsc[] servers;

	private OSCServer _localServer;
	private List<OscSubscriber> _serverSubscriber = new List<OscSubscriber>();
	private List<OSCClient> _localClients = new List<OSCClient>();

	public DistantOsc getOscDistantServer(string name) {
		return Array.Find<DistantOsc> (this.servers, s => s.name == name);
	}

	public void sendOscMessage(OSCClient client, string topic, params object[] values) {
		Debug.LogFormat ("sending message on {0}", topic);
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
		OSCClient c = new UnityOSC.OSCClient (ip, (int)(oscDestination.port));
		this._localClients.Add (c);
		return c;
	}

	private void OnPacketReceived(OSCServer server, OSCPacket packet) {
		bool handled = false;

		foreach (OscSubscriber sub in this._serverSubscriber) {
			Match m = sub.topicRegex.Match (packet.Address);
			if (m.Success) {
				handled = true;
				sub.callback(packet.Address, packet, m.Groups);
			}
		}

        if (!handled)
        { // If not handled => print it
            Debug.LogFormat("OSC Message received on {0} : {1}", packet.Address, packet.Data.Select(d => d.ToString()).Aggregate((a, b) => a + " " + b));
        }

    }

	void Awake() {
		this._localServer = new OSCServer (this.localPort);
		Debug.LogFormat ("Server Listening on {0}", this.localPort.ToString ());
        this._localServer.SleepMilliseconds = 1; // If not set to very low value, we get a VERY HIGH input latency (May go up to 5-10 seconds)
		this._localServer.PacketReceivedEvent += this.OnPacketReceived;
	}

	public void OscSubscribe(string topicRegexLike, OscSubscribeCallback callback) {
		Debug.LogFormat ("OSC SUBSCRIBE ON {0}", topicRegexLike);
        topicRegexLike = Regex.Escape(topicRegexLike);
		topicRegexLike = "^" + topicRegexLike + "$";
		OscSubscriber sub = new OscSubscriber ();
        Debug.LogFormat("Regexified0 : {0}", topicRegexLike);
        string regexified = Regex.Replace(topicRegexLike, Regex.Escape(Regex.Escape("*")), ".*");
        Debug.LogFormat("Regexified1 : {0}", regexified);
        regexified = Regex.Replace(regexified, "\\\\{(.+?)}", "(?<$1>.+)");
        Debug.LogFormat("Regexified2 : {0}", regexified);
		sub.topicRegex = new Regex(regexified);
		sub.callback = callback;
		this._serverSubscriber.Add (sub);
	}

	public void Stop() 
	{
		this._localServer.Close ();

		foreach(OSCClient c in this._localClients)
		{
			c.Close ();
		}
	}

}
