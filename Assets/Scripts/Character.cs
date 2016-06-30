using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	public PicaVoxel.Volume arms;

	public float moveSpeed;
	public float rotationSpeed;
	public int weapon;

	public Transform lookTarget;
	public Vector3 lookPosition;

	void Start () {
	
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

	public void DrawWeapon() {
		if (arms.CurrentFrame == weapon) {
			return;
		}

		this.weapon = weapon;
		arms.SetFrame(weapon);
	}
}
