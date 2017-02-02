using UnityEngine;

public class Painting : MonoBehaviour, Damageable {

	public float health;

	public bool Damage(Vector3 location, Vector3 angle, float damage, bool playerAttacker = false, DamageType type = DamageType.BULLET) {
		if (health == 0 || type == DamageType.EXPLOSIVE) {
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
