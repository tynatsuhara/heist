using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public abstract class Character : PossibleObjective, Damageable {

	// Ranked in order of ascending priority
	public enum Reaction : int {
		MILDLY_SUSPICIOUS,
		SUSPICIOUS,
		AGGRO
	};
	public LayerMask sightLayers;
	public TextObject speech;
	public Accessory[] accessories;
	
	protected Rigidbody rb;
	public WalkCycle walk;

	// each inheriting class should define walking
	public bool walking;

	public float healthMax;
	public float health;
	public float armorMax;
	public float armor;

	public Inventory inventory;
	public Inventory weaponInv;
	public Bag bag;
	public bool hasBag {
		get { return bag != null; }
	}
	public bool isHacking {
		get { return currentInteractScript is Computer; }
	}

	public PicaVoxel.Volume head;
	public PicaVoxel.Volume body;
	public PicaVoxel.Volume arms;

	public float moveSpeed;
	public float rotationSpeed;
	public Vector3 lastMoveDirection;

	public GameObject[] guns;
	protected Gun currentGun;
	private int gunIndex = 0;
	public PicaVoxel.Exploder exploder;
	public Explosive explosive;
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
	public bool beingDragged;

	public abstract void Alert(Character.Reaction importance, Vector3 position);
	public void Alert() {
		PlayerControls pc = ClosestPlayerInSight();
		if (pc != null)
			Alert(Reaction.AGGRO, pc.transform.position);
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
		
	public virtual bool Damage(Vector3 location, Vector3 angle, float damage, bool melee = false, bool playerAttacker = false, bool explosive = false) {
		bool isPlayer = tag.Equals("Player");

		if (!weaponDrawn)
			damage *= 2f;

		if (isPlayer && playerAttacker && !GameManager.instance.friendlyFireEnabled)
			damage = 0f;

		if (armor > 0) {
			armor -= damage;
			if (armor >= 0) {
				if (!isPlayer)
					rb.AddForce(300 * angle.normalized, ForceMode.Impulse);
				return false;
			}
			damage = -armor;  // for applying leftover damage later
		}

		if (isAlive && !isPlayer)
			Invoke("Alert", .7f);

		if (isAlive)
			Bleed(Random.Range(0, 10), location, angle);

		bool wasAlive = isAlive;  // save it beforehand

		health = Mathf.Max(0, health - damage);
		exploder.transform.position = location + angle * Random.Range(-.1f, .15f) + new Vector3(0, Random.Range(-.7f, .3f), 0);
		if (!isAlive && wasAlive) {
			Die(angle, !melee);
		} else if (!isAlive && explosive) {
			exploder.Explode(angle * 3);			
		}

		// regular knockback
		if (!isPlayer || !isAlive) {
			float forceVal = Random.Range(400, 500);
			if (wasAlive && !isAlive) {
				forceVal *= 1.5f;
			} else if (!isAlive) {
				forceVal *= .5f;
			}
			if (melee) {
				forceVal *= 2f;
			}
			rb.AddForceAtPosition(forceVal * angle.normalized, 
								  melee ? transform.position + Vector3.up * Random.Range(-.4f, .3f) : exploder.transform.position,
								  ForceMode.Impulse);
		}

		return !wasAlive;
	}

	public void Die() {		
		Die(Vector3.one);
	}

	public virtual void Die(Vector3 angle, bool explode = false) {
		InteractCancel();
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		if (agent != null)
			agent.enabled = false;
		NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
		if (obstacle != null)
			obstacle.enabled = true;

		walk.StopWalk();
		rb.constraints = RigidbodyConstraints.None;

		if (weaponDrawn)
			DropWeapon(angle * Random.Range(5, 10) + Vector3.up * Random.Range(2, 6));

		if (explode)
			exploder.Explode(angle * 3);

		if (isObjective && !isCompleted)
			MarkCompleted();

		BleedEverywhere();

		if (Random.Range(0, 2) == 0) {
			speech.SayRandom(Speech.DEATH_QUOTES, showFlash: true);
		}

//		Invoke("RemoveBody", 60f);
	}

	public void RemoveBody() {
		Destroy(gameObject);
	}

	private void BleedEverywhere() {
		int bloodSpurtAmount = Random.Range(3, 15);
		for (int i = 0; i < bloodSpurtAmount; i++) {
			Invoke("SpurtBlood", Random.Range(.3f, 1.5f) * i);
		}
		InvokeRepeating("PuddleBlood", .5f, .2f);
		Invoke("CancelPuddling", Random.Range(10f, 30f));
	}

	private void PuddleBlood() {
		int times = Random.Range(1, 5);
		for (int i = 0; i < times; i++)
			WorldBlood.instance.BleedFrom(gameObject, transform.position);
	}

	private void CancelPuddling() {
		CancelInvoke("PuddleBlood");
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
			voxel.Color = WorldBlood.instance.BloodColor();
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

	public void DropWeapon(Vector3 force) {
		if (guns == null || currentGun == null)
			return;
		
		SetWeaponDrawn(false, true);
		currentGun.Drop(force);
		currentGun = null;
	}

	protected void SetWeaponDrawn(bool drawn, bool weaponDropped = false) {
		if (drawn == weaponDrawn_)
			return;

		weaponDrawn_ = drawn;
		arms.gameObject.SetActive(!drawn);
		currentGun = guns[gunIndex].GetComponent<Gun>();

		if (!weaponDropped)
			guns[gunIndex].SetActive(drawn);
	}

	public virtual void Shoot() {
		if (weaponDrawn_ && currentGun != null && !isDragging && !isHacking) {
			currentGun.Shoot();
		} 
	}

	public void Reload() {
		if (weaponDrawn_ && currentGun != null && !isDragging) {
			currentGun.Reload();
		} 
	}

	public void Melee() {
		if (weaponDrawn_ && currentGun != null && !isDragging) {
			currentGun.Melee();
		}
	}

	public void Explosive() {
		if (weaponDrawn_ && currentGun != null && !isDragging && !isHacking && explosive != null) {		
			explosive.Trigger();
		}
	}

	protected Interactable currentInteractScript;
	public void Interact() {
		if (currentInteractScript != null) {
			currentInteractScript.Interact(this);
			return;
		}

		float interactDist = 1.8f;
		float interactStep = .1f;
		// look straight forward, then downwards if you don't see anything
		for (float i = 0; i < interactDist - interactStep * 5; i += interactStep) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, 
								(transform.forward * (interactDist - i) - transform.up * i), 
								out hit, (1 + i * .7f))) {
				currentInteractScript = hit.collider.GetComponentInParent<Interactable>();
				if (currentInteractScript != null) {
					currentInteractScript.Interact(this);
					return;
				}
			}
		}
	}
	public void InteractCancel() {
		if (currentInteractScript != null) {
			currentInteractScript.Uninteract(this);
			currentInteractScript = null;
		}
	}

	public void SpawnGun() {
		if (guns == null || guns.Length == 0)
			return;
		
		for (int i = guns.Length - 1; i >= 0; i--) {
			if (guns[i] == null)
				continue;
			guns[i].SetActive(false);
			GameObject gun = guns[i] = Instantiate(guns[i]) as GameObject;
			gun.name = gun.name.Replace("(Clone)", "");
			List<PicaVoxel.Volume> gunVolumes = new List<PicaVoxel.Volume>();
			foreach (GameObject g in guns)
				gunVolumes.AddRange(g.GetComponentsInChildren<PicaVoxel.Volume>());
			GetComponent<CharacterCustomization>().gunz = gunVolumes.ToArray();
			currentGun = gun.GetComponent<Gun>();
			currentGun.isPlayer = this is PlayerControls;			
			gun.transform.parent = transform;
			gun.transform.localPosition = currentGun.inPlayerPos;
			gun.transform.localRotation = Quaternion.Euler(currentGun.inPlayerRot);
			if (currentGun.isPlayer)
				currentGun.player = (PlayerControls) this;
		}
		currentGun = null;
		SelectGun(0);
	}

	private bool swapping;
	public void SelectGun(int index) {
		index = Mathf.Clamp(index, 0, guns.Length);
		if (this is PlayerControls)
			guns[index].GetComponent<Gun>().UpdateUI();
		if (!weaponDrawn || gunIndex == index) {
			gunIndex = index;
			return;
		}
		float totalSwapTime = .3f;
		currentGun.CancelReload();
		currentGun.DelayAttack(totalSwapTime / 2f);
		gunIndex = index;
		Invoke("SelectGunSwap", totalSwapTime / 2f);
	}
	private void SelectGunSwap() {
		foreach (GameObject g in guns)
			g.SetActive(false);
		guns[gunIndex].SetActive(true);
		currentGun = guns[gunIndex].GetComponent<Gun>();
		currentGun.DelayAttack(.25f);
	}

	// Basically, they're not a civilian. Has a weapon/mask/whatever. Cops should attack!
	public bool IsEquipped() {
		return weaponDrawn || hasBag || isHacking;
	}

	public bool seesEvidence;
	public void CheckForEvidence() {
		seesEvidence = CanSeeEvidence();
	}

	public Computer cameraScreen;
	private bool CanSeeEvidence() {
		bool canSeeOnCameras = cameraScreen != null && cameraScreen.PlayerInSight();
		List<PlayerControls> seenPlayers = GameManager.players.Where(x => CanSee(x.gameObject) && x.IsEquipped()).ToList();
		if (seenPlayers.Count > 0 || canSeeOnCameras)
			return true;
		
		foreach (Character c in GameManager.characters) {
			bool isEvidence = !c.isAlive;
			if (c is Civilian) {
				Civilian civ = (Civilian) c;
				isEvidence |= civ.currentState == Civilian.CivilianState.HELD_HOSTAGE_CALLING;
				isEvidence |= civ.currentState == Civilian.CivilianState.HELD_HOSTAGE_TIED;
				isEvidence |= civ.currentState == Civilian.CivilianState.HELD_HOSTAGE_UNTIED;
			}
			if ((isEvidence && CanSee(c.gameObject)) || 
			    (isEvidence && cameraScreen != null && cameraScreen.InSight(c.gameObject))) {
				return true;
			}
		}

		return false;
	}

	public bool CanSee(GameObject target, float fov = 130f, float viewDist = 20f) {
		Vector3 diff = transform.position - target.transform.position;
		if (diff.magnitude > viewDist)
			return false;
		
		float angle = Vector3.Dot(Vector3.Normalize(transform.position - target.transform.position), transform.forward);
		float angleDegrees = 90 + Mathf.Asin(angle) * Mathf.Rad2Deg;
		if (angleDegrees > fov / 2f) {
			return false;
		}

		RaycastHit hit;		
		if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, viewDist, sightLayers))
			return hit.collider.transform.root.gameObject == target;

		return false;
	}

	public PlayerControls ClosestPlayerInSight() {
		PlayerControls playerScript = null;
		foreach (PlayerControls pc in GameManager.players) {
			if (!CanSee(pc.gameObject))
				continue;

			if (playerScript == null || ((transform.position - pc.transform.position).magnitude < 
									     (transform.position - playerScript.transform.position).magnitude))
				playerScript = pc;
		}
		return playerScript;
	}

	public bool ClearShot(GameObject target, float dist = 20f) {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, dist))
			return hit.collider.transform.root.gameObject == target;

		return false;
	}

	private List<Character> CharactersInFront() {
		Vector3 inFrontPos = transform.position + transform.forward * .75f;
		return GameManager.instance.CharactersWithinDistance(inFrontPos, 1.2f);
	}

	public void DragBody() {
		if (draggedBody != null)
			return;
		
		List<Character> draggableChars = CharactersInFront().Where(x => {
			if (x is Civilian) {
				Civilian z = (Civilian) x;
				return !x.isAlive || z.currentState == Civilian.CivilianState.HELD_HOSTAGE_TIED;
			}
			return !x.isAlive;
		}).ToList();

		// must have line of sight
		foreach (Character c in draggableChars) {
			Vector3 dir = c.transform.position - transform.position;
			RaycastHit hit;
			if (!Physics.Raycast(transform.position, dir, out hit)) {
				continue;
			}
			if (hit.collider.GetComponentInParent<Character>() != c) {
				continue;
			}
			draggedBody = c.gameObject;
			Character bodyChar = draggedBody.GetComponent<Character>();
			bodyChar.beingDragged = true;
			break;
		}
	}

	protected void Drag() {
		if (draggedBody != null) {
			Vector3 dragPos = transform.position + transform.forward.normalized * 1.2f;
			// add a bit of a buffer between the floor and character to avoid friction
			dragPos.y = Mathf.Max(draggedBody.transform.position.y, .4f);
			Vector3 force = (dragPos - draggedBody.transform.position).normalized;
			draggedBody.GetComponent<Rigidbody>().AddForce(force * 10000f, ForceMode.Force);
		}
	}

	public void ReleaseBody() {
		if (draggedBody == null)
			return;
		draggedBody.GetComponent<Character>().beingDragged = false;
		draggedBody = null;
	}

	public void AddBag(Bag bag) {
		if (hasBag) return;

		this.bag = bag;
		bag.transform.parent = transform;
		bag.transform.localPosition = new Vector3(.5f, -.8f, -.1f);
		bag.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
	}

	public void DropBag(bool launch = true) {
		if (!hasBag) return;
		GameObject facingObject = FacingObstruction();
		if (facingObject != null && facingObject.GetComponentInParent<Car>() != GameManager.instance.getaway) {
			return;
		}

		bag.DropBag();
		bag.transform.parent = null;
		if (launch) {
			bag.transform.position = transform.position + transform.forward * 1f - transform.up * .6f;
			bag.GetComponent<Rigidbody>().AddForce(transform.forward * (walking ? 800f : 400f), ForceMode.Impulse);
		}
		bag = null;
	}

	// Returns the gameObject the player is facing, or null if there isn't one
	public GameObject FacingObstruction(out RaycastHit hit, float distance = 1f) {
		if (Physics.Raycast(transform.position, transform.forward, out hit, distance)) {
			return hit.collider.gameObject;
		}
		return null;
	}
	public GameObject FacingObstruction(float distance = 1f) {
		RaycastHit hit;
		return FacingObstruction(out hit, distance);
	}
}
