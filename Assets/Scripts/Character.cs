﻿using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour, Damageable {
	
	private Rigidbody rb;
	private WalkCycle walk;

	public float health;

	public PicaVoxel.Volume body;
	public PicaVoxel.Volume arms;

	public float moveSpeed;
	public float rotationSpeed;
	public Vector3 lastMoveDirection;

	public GameObject gun;
	public PicaVoxel.Exploder exploder;
	private Gun gunScript;
	private bool weaponDrawn_;
	public bool weaponDrawn {
		get { return weaponDrawn_; }
	}

	private bool hasLookTarget = false;
	public Transform lookTarget;
	public Vector3 lookPosition;

	public bool isAlive {
		get { return health > 0; }
	}

	public GameObject draggedBody;
	public bool isDragging {
		get { return draggedBody != null; }
	}

	void Start () {
		rb = GetComponent<Rigidbody>();
		walk = body.GetComponent<WalkCycle>();
		gunScript = gun.GetComponent<Gun>();
	}
	
	void FixedUpdate () {
		if (!isAlive)
			return;
		
		Rotate();
		Drag();
	}

	public void Move(float x, float z) {
		Vector3 pos = transform.position;
		pos.x += moveSpeed * x;
		pos.z += moveSpeed * z;
		transform.position = pos;
		if ((x != 0 || z != 0) && !walk.isWalking) {
			walk.StartWalk();
		} else if (x == 0 && z == 0 && walk.isWalking) {
			walk.StopWalk();
		}
		if (x != 0 || z != 0) {
			lastMoveDirection = new Vector3(x, 0, z).normalized;
		}
	}

	public void KnockBack(float force) {
		rb.AddForce(force * -transform.forward, ForceMode.Impulse);
	}

	public void LookAt(Transform target) {
		hasLookTarget = true;
		lookTarget = target;
	}

	public void LookAt(Vector3 target) {
		hasLookTarget = true;
		lookTarget = null;
		lookPosition = target;
	}

	public void LoseLookTarget() {
		hasLookTarget = false;
		lookTarget = null;
	}

	void Rotate() {
		if (lookTarget != null) {
			lookPosition = lookTarget.position;
			hasLookTarget = true;
		}
		if (hasLookTarget) {
			// TODO: I don't think this actually works at all

			lookPosition.y = transform.position.y;
			Vector3 vec = lookPosition - transform.position;
			if (vec == Vector3.zero)
				return;
			Quaternion targetRotation = Quaternion.LookRotation(vec);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
		}
	}

	public void Damage(Vector3 location, Vector3 angle, float damage) {
		rb.AddForce(400 * angle.normalized, ForceMode.Impulse);
		exploder.transform.position = location + angle * Random.Range(-.1f, .2f) + new Vector3(0, Random.Range(-.1f, .1f), 0);
		health -= damage;
		if (health <= 0)
			Die(angle);
	}

	public void Die() {
		Die(Vector3.one);
	}

	public void Die(Vector3 angle) {
		exploder.Explode(angle * 3);
		rb.constraints = RigidbodyConstraints.None;
		rb.AddForce(400 * angle.normalized, ForceMode.Impulse);
		HideWeapon();
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
		if (weaponDrawn_ && gunScript != null && !isDragging) {
			gunScript.Shoot();
		} 
	}

	public void Interact() {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward * 2f, out hit)) {
			// interact with whatever you hit, if possible
		}
	}

	public bool PoliceShouldAttack() {
		return weaponDrawn;
	}

	public bool CanSeeCharacter(GameObject target) {
		float angle = Vector3.Dot(Vector3.Normalize(transform.position - target.transform.position), transform.forward);
		if (angle >= -.2f)
			return false;

		RaycastHit hit;
		if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit))
			return hit.collider.transform.root.gameObject == target;

		return false;
	}

	public void DragBody() {
		if (draggedBody != null)
			return;
		
		Vector3 dir = transform.forward * 2f;
		dir.y = -2;
		
		RaycastHit hit;
		if (!Physics.Raycast(transform.position, dir, out hit))
			return;
		draggedBody = hit.collider.transform.root.gameObject;
		Character bodyChar = draggedBody.GetComponent<Character>();
		if (bodyChar == null || bodyChar.isAlive) {
			draggedBody = null;
		}
	}

	private void Drag() {
		if (draggedBody != null) {
			Vector3 dragPos = transform.position + transform.forward.normalized * 1.2f;
			draggedBody.transform.position = Vector3.Lerp(draggedBody.transform.position, dragPos, .1f);
		}
	}

	public void ReleaseBody() {
		draggedBody = null;
	}
}
