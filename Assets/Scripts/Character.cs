using UnityEngine;
using System.Collections;

public abstract class Character : PossibleObjective, Damageable {

	public TextObject speech;
	
	protected Rigidbody rb;
	public WalkCycle walk;

	public float health;
	public Inventory inventory;

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

		bool returnVal = !isAlive;  // save it because it could change right after

		health -= damage;
		exploder.transform.position = location + angle * Random.Range(-.1f, .15f) + new Vector3(0, Random.Range(-.1f, .1f), 0);
		if (health <= 0) {
			Die(angle);
		}
		rb.AddForce(400 * angle.normalized, ForceMode.Impulse);

		return returnVal;
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
		rb.AddForce(400 * angle.normalized, ForceMode.Impulse);
		HideWeapon();
		exploder.Explode(angle * 3);

		if (isObjective && !isCompleted)
			MarkCompleted();
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
