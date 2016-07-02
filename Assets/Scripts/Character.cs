using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour, Damageable {
	
	private Rigidbody rb;
	private WalkCycle walk;

	public PicaVoxel.Volume body;
	public PicaVoxel.Volume arms;

	public float moveSpeed;
	public float rotationSpeed;

	public GameObject gun;
	public PicaVoxel.Exploder exploder;
	private Gun gunScript;
	private bool weaponDrawn_;
	public bool weaponDrawn {
		get { return weaponDrawn_; }
	}

	public Transform lookTarget;
	public Vector3 lookPosition;

	void Start () {
		rb = GetComponent<Rigidbody>();
		walk = body.GetComponent<WalkCycle>();
		gunScript = gun.GetComponent<Gun>();
	}
	
	void FixedUpdate () {
		Rotate();
	}

	public void Move(float x, float y) {
		Vector3 pos = transform.position;
		pos.x += moveSpeed * x;
		pos.z += moveSpeed * y;
		transform.position = pos;
		if ((x != 0 || y != 0) && !walk.isWalking) {
			walk.StartWalk();
		} else if (x == 0 && y == 0 && walk.isWalking) {
			walk.StopWalk();
		}
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
			Vector3 vec = lookPosition - transform.position;
			if (vec == Vector3.zero)
				return;
			Quaternion targetRotation = Quaternion.LookRotation(vec);
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
		SetWeaponDrawn(true);
	}

	public void HideWeapon() {
		SetWeaponDrawn(false);
	}

	private void SetWeaponDrawn(bool drawn) {
		if (drawn == weaponDrawn_)
			return;

		weaponDrawn_ = drawn;
		arms.gameObject.SetActive(!drawn);
		gun.SetActive(drawn);
	}

	public void Shoot() {
		if (weaponDrawn_ && gunScript != null) {
			gunScript.Shoot();
		}
	}
}
