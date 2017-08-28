using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple utilitary class used to make a GameObject face the camera.
/// </summary>
public class FaceCamera : MonoBehaviour {
	void Update () {
		transform.LookAt (Camera.main.transform.position);
	}
}
