using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LpsNode : MonoBehaviour {

	public int id;

	public static int compareNodes(GameObject node1, GameObject node2)
	{
		return node1.GetComponent<LpsNode> ().id - node2.GetComponent<LpsNode> ().id;
	}
}