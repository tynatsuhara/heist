using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour, Damageable {
	
	private Rigidbody rb;

	public PicaVoxel.Volume arms;

	public float moveSpeed;
	public float rotationSpeed;

	public int weapon;
	public GameObject gun_;
	public PicaVoxel.Exploder exploder;
	private Gun gun;
	private bool weaponDrawn;

	public Transform lookTarget;
	public Vector3 lookPosition;

	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
		gun = gun_.GetComponent<Gun>();
	}
	
	void FixedUpdate () {
		Rotate();
	}

	public void Move(float x, float y) {
		Vector3 pos = transform.position;
		pos.x += moveSpeed * x;
		pos.z += moveSpeed * y;
		transform.position = pos;
	}

	public void KnockBack(float force) {
		rb.AddForce(force * -transform.forward);
	}

	public void LookAt(Transform target) {
		lookTarget = target;
	}

	public void LookAt(Vector3 target) {
		lookTarget = null;
		lookPosition = target;
	}

	void Rotate() {
		if (lookTarget != null) {
			lookPosition = lookTarget.position;
		}
		if (lookPosition != null) {
			lookPosition.y = transform.position.y;
			Quaternion targetRotation = Quaternion.LookRotation(lookPosition - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
		}
	}

	public void Damage(Vector3 location, Vector3 angle, float damage) {
		exploder.transform.position = location + angle * Random.Range(-.1f, .2f) + new Vector3(0, Random.Range(-.1f, .1f), 0);
		exploder.Explode(angle * 3);
		exploder.ExplosionRadius += .2f;
		rb.constraints = RigidbodyConstraints.None;
		KnockBack(100000);
	}

	public void DrawWeapon() {
		if (weaponDrawn) {
			return;
		}
		weaponDrawn = true;
		arms.SetFrame(weapon);
	}

	public void HideWeapon() {
		if (!weaponDrawn) {
			return;
		}

		weaponDrawn = false;
	}

	public void Shoot() {
		if (weaponDrawn && gun != null) {
			gun.Shoot();
		}
	}
}
