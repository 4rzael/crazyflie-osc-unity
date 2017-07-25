using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityOSC;

public class Controller : MonoBehaviour {

    [SerializeField] private string controllerOscTopic;

    private OscManager _oscManager;
    private DronesManager _dronesManager;
    [SerializeField] private int _droneID;
    private GameObject _drone;

    private Vector3 _position = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _acceleration = Vector3.zero;

    private LinkedList<MathUtils.Vec3Stamped> _lastPositions = new LinkedList<MathUtils.Vec3Stamped>();
    private LinkedList<MathUtils.Vec3Stamped> _lastVelocities = new LinkedList<MathUtils.Vec3Stamped>();

    // Use this for initialization
    void Start () {
        _oscManager = GameObject.Find("OscManager").GetComponent<OscManager>();
        _dronesManager = GameObject.Find("DronesManager").GetComponent<DronesManager>();
        _drone = _dronesManager.getDroneById(_droneID);

        // soft "emergency" : do not need to reboot the drone
        _drone.GetComponent<Drone>().disableSendingCommands = true;

        _oscManager.OscSubscribe(string.Format("{0}/buttons", controllerOscTopic), this.onButtons);
    }

    void onButtons(string topic, OSCPacket packet, GroupCollection path_args)
    {
        IEnumerable<int> buttons = (IEnumerable<int>)packet.Data;
        Debug.Log(buttons.Select(b => b.ToString()).Aggregate((a, b) => a + " " + b));
    }

    private void Update()
    {
        Debug.DrawLine(_drone.transform.position, _drone.transform.position + _velocity, Color.red);
        Debug.DrawLine(_drone.transform.position, _drone.transform.position + _acceleration, Color.blue);

        Vector3 newPosition = _drone.GetComponent<Drone>()._realPosition;

        if (newPosition != Vector3.zero)
        {
            // STORE POSITION, DERIVE VELOCITY AND ACCELERATION
            _position = newPosition;

            _lastPositions.AddLast(_position);
            deleteAllFromLinkedList(_lastPositions, MathUtils.Vec3Stamped.tmsIsBefore(0.1f));

            _velocity = MathUtils.Vec3Stamped.average(_lastPositions);
            _lastVelocities.AddLast(_velocity);
            deleteAllFromLinkedList(_lastVelocities, MathUtils.Vec3Stamped.tmsIsBefore(0.1f));

            _acceleration = MathUtils.Vec3Stamped.average(_lastVelocities);
        }
    }

    public void deleteAllFromLinkedList<T>(LinkedList<T> list, Predicate<T> predicate)
    {
        var node = list.First;
        while (node != null)
        {
            var nextNode = node.Next;
            if (predicate(node.Value))
            {
                list.Remove(node);
            }
            node = nextNode;
        }
    }

}
