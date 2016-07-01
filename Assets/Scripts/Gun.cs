using UnityEngine;

public abstract class Gun : MonoBehaviour {

	public Character owner;

	void Start() {
		owner = transform.root.GetComponent<Character>();
	}

	abstract public int Value();
	abstract public void Shoot();
	abstract public void Release();

	public void RaycastForward(Vector3 source, Vector3 direction, float damageVal) {
		RaycastHit hit;
		Debug.DrawRay(source, direction * 30, Color.red, 3f);
		if (Physics.Raycast(source, direction, out hit)) {
			print("Found an object (" + hit.transform.name + ") - distance: " + hit.distance);
			Damageable damageScript = hit.transform.root.GetComponent<Damageable>();
			if (damageScript != null) {
				damageScript.Damage(source, direction.normalized, damageVal);
			}
		}
	}
}
