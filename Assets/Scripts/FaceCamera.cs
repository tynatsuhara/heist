using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {

	// Update is called once per frame
	void LateUpdate () {
		transform.rotation = CameraMovement.instance.cam.transform.rotation;
	}
}
