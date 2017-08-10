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
		public ushort port;
	}

	public delegate void OscSubscribeCallback(string topic, OSCPacket packet, GroupCollection path_args);
	private struct OscSubscriber {
		public Regex topicRegex;
		public OscSubscribeCallback callback;
	}

	public string localIP = "127.0.0.1";
	public short localPort = 6006;
	public DistantOsc[] servers;

    public bool shouldPrintDebugMessages = true;

	private OSCServer _localServer;
	private List<OscSubscriber> _serverSubscriber = new List<OscSubscriber>();
	private List<OSCClient> _localClients = new List<OSCClient>();

	private DistantOsc GetOscDistantServer(string name) {
		return Array.Find<DistantOsc> (this.servers, s => s.name == name);
	}

    /// <summary>
    /// Sends an OSC message.
    /// </summary>
    /// <param name="client">The OSC client to send from.</param>
    /// <param name="topic">The OSC topic.</param>
    /// <param name="values">The values to send.</param>
    public void SendOscMessage(OSCClient client, string topic, params object[] values) {
        if (shouldPrintDebugMessages)
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

    /// <summary>
    /// Creates an OSC client.
    /// </summary>
    /// <param name="server">The server to connect the client to.</param>
    /// <returns></returns>
    public OSCClient createClient(string server) {
		OscManager.DistantOsc oscDestination = this.GetOscDistantServer (server);

		IPAddress ip = IPAddress.Parse(oscDestination.ip);
		OSCClient c = new UnityOSC.OSCClient (ip, (int)(oscDestination.port));
		this._localClients.Add (c);
		return c;
	}

    /// <summary>
    /// Called when [packet received].
    /// </summary>
    /// <param name="server">The distant server that sent the packet.</param>
    /// <param name="packet">The packet received.</param>
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
        if (shouldPrintDebugMessages)
    		Debug.LogFormat ("Server Listening on {0}", this.localPort.ToString ());
        this._localServer.SleepMilliseconds = 0; // If not set to very low value, we get a VERY HIGH input latency (May go up to 5-10 seconds)
		this._localServer.PacketReceivedEvent += this.OnPacketReceived;
	}

    /// <summary>
    /// Subscribes to an OSC topic.
    /// Regex notation means :
    /// `{variable_name}` to accept anything and store it as `variable_name`
    /// `*` to accept anything
    /// </summary>
    /// <param name="topicRegexLike">The topic ("regex"-like notation).</param>
    /// <param name="callback">The callback to call on packet reception.</param>
    public void OscSubscribe(string topicRegexLike, OscSubscribeCallback callback) {
        if (shouldPrintDebugMessages)
    		Debug.LogFormat ("OSC SUBSCRIBE ON {0}", topicRegexLike);
        topicRegexLike = Regex.Escape(topicRegexLike);
		topicRegexLike = "^" + topicRegexLike + "$";
		OscSubscriber sub = new OscSubscriber ();
        string regexified = Regex.Replace(topicRegexLike, Regex.Escape(Regex.Escape("*")), ".*");
        regexified = Regex.Replace(regexified, "\\\\{(.+?)}", "(?<$1>.+)");
		sub.topicRegex = new Regex(regexified);
		sub.callback = callback;
		this._serverSubscriber.Add (sub);
	}

    /// <summary>
    /// Stops this instance.
    /// </summary>
    public void Stop() 
	{
        if (this._localServer != null)
        {
            this._localServer.Close();
            this._localServer = null;
        }

		foreach(OSCClient c in this._localClients)
		{
			c.Close ();
		}
        this._localClients.Clear();
	}

    private void OnDestroy()
    {
        Stop();
    }

}
