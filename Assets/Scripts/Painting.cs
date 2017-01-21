using UnityEngine;

public class Painting : MonoBehaviour, Damageable {

	public float health;

	public bool Damage(Vector3 location, Vector3 angle, float damage, bool melee = false, bool playerAttacker = false) {
		if (health == 0) {
			Rigidbody rb = GetComponent<Rigidbody>();
			rb.isKinematic = false;
			enabled = false;
		} else {
			transform.RotateAround(transform.position, transform.right, Random.Range(-10, 11));
			health--;
		}
		return true;
	}
}
