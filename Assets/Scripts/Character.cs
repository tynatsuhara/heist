﻿using UnityEngine;
using System.Collections;

public abstract class Character : PossibleObjective, Damageable {

	public TextObject speech;
	
	protected Rigidbody rb;
	public WalkCycle walk;

	public float health;
	public float armor;
	public Inventory inventory;

	public PicaVoxel.Volume head;
	public PicaVoxel.Volume body;
	public PicaVoxel.Volume arms;

	public float moveSpeed;
	public float rotationSpeed;
	public Vector3 lastMoveDirection;

	public GameObject gun;
	public PicaVoxel.Exploder exploder;
	protected Gun gunScript;
	protected bool weaponDrawn_;
	public bool weaponDrawn {
		get { return weaponDrawn_; }
	}

	protected bool hasLookTarget = false;
	public Transform lookTarget;
	public Vector3 lookPosition;

	public bool isAlive {
		get { return health > 0; }
	}

	public GameObject draggedBody;
	public bool isDragging {
		get { return draggedBody != null; }
	}

	public abstract void Alert();

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

	protected void Rotate() {
		if (lookTarget != null) {
			lookPosition = lookTarget.position;
			hasLookTarget = true;
		}
		if (hasLookTarget) {
			lookPosition.y = transform.position.y;
			Vector3 vec = lookPosition - transform.position;
			if (vec == Vector3.zero)
				return;
			Quaternion targetRotation = Quaternion.LookRotation(vec);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
		}
	}
		
	public bool Damage(Vector3 location, Vector3 angle, float damage) {
		if (!weaponDrawn)
			damage *= 2f;
		Invoke("Alert", .7f);

		if (armor > 0) {
			armor -= damage;
			if (armor >= 0) {
				rb.AddForce(300 * angle.normalized, ForceMode.Impulse);
				return false;
			}
			damage = -armor;  // for applying leftover damage
		}

		Bleed(Random.Range(0, 10), location, angle);

		bool wasAlive = isAlive;  // save it beforehand

		health -= damage;
		exploder.transform.position = location + angle * Random.Range(-.1f, .15f) + new Vector3(0, Random.Range(-.7f, .3f), 0);
		if (health <= 0 && wasAlive) {
			Die(angle);
		}

		rb.AddForce(300 * angle.normalized, ForceMode.Impulse);

		return wasAlive;
	}

	public void Die() {		
		Die(Vector3.one);
	}

	public void Die(Vector3 angle) {
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		if (agent != null)
			agent.enabled = false;

		walk.StopWalk();
		rb.constraints = RigidbodyConstraints.None;
		HideWeapon();
		exploder.Explode(angle * 3);

		if (isObjective && !isCompleted)
			MarkCompleted();

		int bloodSpurtAmount = Random.Range(3, 15);
		for (int i = 0; i < bloodSpurtAmount; i++) {
			Invoke("SpurtBlood", Random.Range(.3f, 1.5f) * i);
		}

		speech.SayRandom(new string[] {
			"aaahhhh",
			"tell my wife i loved her",
			"i'm coming for you, harambe",
			"he got me",
			"just a flesh wound",
			"my spline!",
			"blarglefargle",
			"you can't kill me",
			"avenge me",
			"aack",
			"i knew this would happen",
			"i can see the light",
			"my life is flashing before my eyes",
			"why do the good die young?"
			}, showFlash: true);
	}

	private void SpurtBlood() {
		Bleed(Random.Range(5, 10), transform.position + Vector3.up * .3f, Vector3.up);
	}

	public void Bleed(int amount, Vector3 position, Vector3 velocity) {
		PicaVoxel.Volume volume = Random.Range(0, 3) == 1 ? head : body;
		if (volume == body)
			position.y -= .5f;
		for (int i = 0; i < amount; i++) {
			PicaVoxel.Voxel voxel = new PicaVoxel.Voxel();
			byte gb = (byte)Random.Range(0, 30);
			voxel.Color = new Color32((byte)(120 + Random.Range(0, 60)), gb, gb, 0);
			voxel.State = PicaVoxel.VoxelState.Active;
			Vector3 spawnPos = position + Random.insideUnitSphere * .2f;
			PicaVoxel.PicaVoxelPoint pos = volume.GetVoxelArrayPosition(spawnPos);
			PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(spawnPos, 
				voxel, .1f, 4 * velocity + 3 * Random.insideUnitSphere + Vector3.up * 0f);
			PicaVoxel.Voxel? hit = volume.GetVoxelAtArrayPosition(pos.X, pos.Y, pos.Z);
			if (hit != null) {
				PicaVoxel.Voxel nonnullHit = (PicaVoxel.Voxel)hit;
				voxel.Value = nonnullHit.Value;

				if (nonnullHit.Active)
					volume.SetVoxelAtArrayPosition(pos, voxel);
			}
		}
	}

	public void DrawWeapon() {
		SetWeaponDrawn(true);
	}

	public void HideWeapon() {
		SetWeaponDrawn(false);
	}

	protected void SetWeaponDrawn(bool drawn) {
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

	protected Interactable currentInteractScript;
	public void Interact() {
		if (currentInteractScript != null) {
			currentInteractScript.Interact(this);
			return;
		}

		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, 1.8f)) {
			currentInteractScript = hit.collider.transform.root.GetComponent<Interactable>();
			if (currentInteractScript != null) {
				currentInteractScript.Interact(this);
			}
		}
	}
	public void InteractCancel() {
		if (currentInteractScript != null) {
			currentInteractScript.Uninteract(this);
			currentInteractScript = null;
		}
	}

	// Basically, they're not a civilian. Has a weapon/mask/whatever.
	public bool IsEquipped() {
		return weaponDrawn;
	}

	public bool CanSeeCharacter(GameObject target) {
		float viewDist = 20f;

		Vector3 diff = transform.position - target.transform.position;
		if (diff.magnitude > viewDist)
			return false;
		
		float angle = Vector3.Dot(Vector3.Normalize(transform.position - target.transform.position), transform.forward);
		float viewingAngle = -.3f;
		if (angle >= viewingAngle)
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

	protected void Drag() {
		if (draggedBody != null) {
			Vector3 dragPos = transform.position + transform.forward.normalized * 1.2f;
			dragPos.y = draggedBody.transform.position.y;
			draggedBody.transform.position = Vector3.Lerp(draggedBody.transform.position, dragPos, .3f);
		}
	}

	public void ReleaseBody() {
		draggedBody = null;
	}
}
