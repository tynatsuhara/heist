using UnityEngine;

public class Painting : MonoBehaviour, Damageable {

	public bool Damage(Vector3 location, Vector3 angle, float damage, bool melee = false, bool playerAttacker = false) {
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.isKinematic = false;
		enabled = false;
		return true;
	}
}
