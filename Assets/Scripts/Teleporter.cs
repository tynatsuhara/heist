using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public Vector3 translation;
	public bool relative = true;
	private bool hasTeleported;

	void OnTriggerEnter(Collider other) {
		if (hasTeleported)
			return;

		hasTeleported = true;

		if (relative) {
			other.transform.root.position += translation;
		} else {
			other.transform.root.position = translation;
		}

		Invoke("BoolSwitch", .3f);
	}

	private void BoolSwitch() {
		hasTeleported = false;
	}
}
