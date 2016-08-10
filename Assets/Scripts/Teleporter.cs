using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public Vector3 translation;
	private bool hasTeleported;

	void OnTriggerEnter(Collider other) {
		if (hasTeleported)
			return;

		hasTeleported = true;

		other.transform.root.position += translation;

		Door d = GetComponentInParent<Door>();
		if (d != null) {
			d.Mirror();
		}

		Invoke("BoolSwitch", .3f);
	}

	private void BoolSwitch() {
		hasTeleported = false;
	}
}
