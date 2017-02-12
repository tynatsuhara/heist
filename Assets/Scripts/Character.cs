using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : PossibleObjective, Damageable {

	// Ranked in order of ascending priority
	public enum Reaction : int {
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
	public int zipties;
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

	public Rigidbody draggedBody;
	public bool isDragging {
		get { return draggedBody != null; }
	}

	public virtual void Start() {
		rb = GetComponent<Rigidbody>();
		separateBodyParts.Add(rb);
		SpawnGun();
		speech = GetComponentInChildren<TextObject>();
	}

	public abstract void Alert(Reaction importance, Vector3 position);
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

	protected List<Rigidbody> separateBodyParts = new List<Rigidbody>();
	private bool lastDamageNonlethal;
	private bool killedByPlayer;
	public virtual bool Damage(Vector3 location, Vector3 angle, float damage, bool playerAttacker = false, DamageType type = DamageType.BULLET) {
		bool isPlayer = tag.Equals("Player");
		lastDamageNonlethal = type == DamageType.NONLETHAL;

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
			killedByPlayer = isPlayer;
			Die(location, angle, type);
		}

		// regular knockback
		if (!isPlayer || !isAlive) {
			float forceVal = Random.Range(500, 900);
			if ((wasAlive && !isAlive) || type == DamageType.MELEE || type == DamageType.SLICE) {
				forceVal *= 1.5f;
			}
			foreach (Rigidbody body in separateBodyParts) {
				body.AddForceAtPosition(forceVal * angle.normalized, type == DamageType.MELEE 
									? transform.position + Vector3.up * Random.Range(-.4f, .3f) 
									: exploder.transform.position, ForceMode.Impulse);
			}
		}

		return !wasAlive;
	}

	public void Die() {		
		Die(transform.position, Vector3.one);
	}

	public virtual void Die(Vector3 location, Vector3 angle, DamageType type = DamageType.MELEE) {
		GameManager.instance.AlertInRange(Reaction.AGGRO, transform.position, 2f);
		InteractCancel();
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		if (agent != null)
			agent.enabled = false;
		NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
		if (obstacle != null)
			obstacle.enabled = true;

		walk.StopWalk();
		rb.constraints = RigidbodyConstraints.None;

		if (weaponDrawn) {
			DropWeapon(angle * Random.Range(5, 10) + Vector3.up * Random.Range(2, 6));
		}

		if (type == DamageType.SLICE) {
			if (Random.Range(0, 2) == 0)
				Decapitate();
		} else if (type != DamageType.MELEE && type != DamageType.NONLETHAL) {
			exploder.Explode(angle * 3);
			// BloodSplatter(location);
		}
		if (type == DamageType.NONLETHAL) {
			SpurtBlood();
			PuddleBlood();
		} else {
			BleedEverywhere();
		}

		if (isObjective && !isCompleted) {
			MarkCompleted();
		}

		if (Random.Range(0, 2) == 0) {
			speech.SayRandom(Speech.DEATH_QUOTES, showFlash: true);
		}

		StartCoroutine("FallOver");
		// Invoke("RemoveBody", 60f);
	}

	private IEnumerator FallOver() {
		yield return new WaitForSeconds(Random.Range(.3f, 1f));
		for (int i = 0; i < 10; i++) {
			int decidingAngle = 8;
			if (Mathf.Abs(transform.eulerAngles.z) < decidingAngle && Mathf.Abs(transform.eulerAngles.x) < decidingAngle) {	
				Vector3 fallDir = Random.insideUnitCircle;
				fallDir.z = fallDir.y;
				fallDir.y = 0;
				for (int j = 0; j < 3; j++) {
					GetComponent<Rigidbody>().AddForce(fallDir * (400f + i * 150f), ForceMode.Impulse);
					yield return new WaitForSeconds(.08f);
				}
			}
			if (Mathf.Abs(transform.eulerAngles.z) >= decidingAngle || Mathf.Abs(transform.eulerAngles.x) >= decidingAngle) {
				yield return new WaitForSeconds(.5f);
			}
		}
	}

	private void Decapitate() {
		head.transform.parent = null;
		Rigidbody b = head.gameObject.AddComponent<Rigidbody>() as Rigidbody;
		b.mass = rb.mass;
		separateBodyParts.Add(b);
	}

	public void RemoveBody() {
		Destroy(gameObject);
	}


	// GORE GORE GORE

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
		for (int i = 0; i < times; i++) {
			Vector3 pos = separateBodyParts[Random.Range(0, separateBodyParts.Count)].transform.position;		
			WorldBlood.instance.BleedFrom(gameObject, pos);
		}
	}

	private void CancelPuddling() {
		CancelInvoke("PuddleBlood");
	}

	private void SpurtBlood() {
		Vector3 pos = separateBodyParts[Random.Range(0, separateBodyParts.Count)].transform.position;
		Bleed(Random.Range(5, 10), pos + Vector3.up * .3f, Vector3.up);
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

	// TODO: Revisit this?
	public void BloodSplatter(Vector3 pos, float radius = 2f, int rayAmount = 30) {
		for (int k = 0; k < rayAmount; k++) {
			float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
			var off = 2f / rayAmount;
			var y = k * off - 1 + (off / 2);
			var r = Mathf.Sqrt(1 - y * y);
			var phi = k * inc;
			var x = (float)(Mathf.Cos(phi) * r);
			var z = (float)(Mathf.Sin(phi) * r);
			Debug.DrawRay(pos, new Vector3(x, y, z) * radius, Color.red, 5f);
			
			RaycastHit[] hits = Physics.RaycastAll(pos, new Vector3(x, y, z), radius)
				.Where(h => h.transform.root != transform.root && 
						h.transform.GetComponentInParent<PicaVoxel.Volume>() != null &&
						h.transform.GetComponentInParent<Damageable>() != null &&
						h.transform.GetComponentInParent<Wall>() == null)
				.OrderBy(h => h.distance)
				.ToArray();
			if (hits.Length > 0) {
				int splatSize = Random.Range(2, 70);
				for (int i = 0; i < splatSize; i++) {
					Vector3 splatRange = Random.insideUnitSphere * .3f;
					Vector3 point = hits[0].point + new Vector3(x, y, z).normalized * .1f + splatRange;
					PicaVoxel.Volume vol = hits[0].transform.GetComponentInParent<PicaVoxel.Volume>();
					PicaVoxel.Voxel? voxq = vol.GetVoxelAtWorldPosition(point);
					if (voxq == null || !voxq.Value.Active) {
						// Debug.Log("Couldn't paint!");
					} else {
						PicaVoxel.Voxel vox = (PicaVoxel.Voxel)voxq;
						vox.Color = WorldBlood.instance.BloodColor();
						vol.SetVoxelAtWorldPosition(point, vox);
					}
				}
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
			gun.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
			if (currentGun.isPlayer)
				currentGun.player = (PlayerControls) this;
		}
		currentGun = null;
		SelectGun(guns.Length > 1 ? 1 : 0);
	}

	private bool swapping;
	public void SelectGun(int index) {
		index = Mathf.Clamp(index, 0, guns.Length);
		if (this is PlayerControls)
			guns[index].GetComponent<Gun>().UpdateUI();
		if (gunIndex == index || guns[gunIndex].GetComponent<Gun>().meleeing) {
			return;
		} else if (!weaponDrawn) {
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
		Floor f = LevelBuilder.instance.FloorAt(transform.position);
		bool inRestrictedArea = f != null && f.restricted;
		return weaponDrawn || hasBag || isHacking || inRestrictedArea;
	}

	public bool seesEvidence;
	public void CheckForEvidence() {
		seesEvidence = CanSeeEvidence();
	}

	private bool CanSeeEvidence() {
		Computer cameraScreen = CheckForCameraComputer();
		bool canSeeOnCameras = cameraScreen != null && cameraScreen.PlayerInSight();
		List<PlayerControls> seenPlayers = GameManager.players.Where(x => CanSee(x.gameObject) && x.IsEquipped()).ToList();
		if (seenPlayers.Count > 0 || canSeeOnCameras)
			return true;
		
		foreach (NPC c in GameManager.characters) {
			bool isEvidence = !c.isAlive;
			isEvidence |= c.currentState == Civilian.NPCState.ALERTING;
			isEvidence |= c.currentState == Civilian.NPCState.HELD_HOSTAGE_TIED;
			isEvidence |= c.currentState == Civilian.NPCState.HELD_HOSTAGE_UNTIED;
			if ((isEvidence && CanSee(c.gameObject)) || 
			    (isEvidence && cameraScreen != null && cameraScreen.InSight(c.gameObject))) {
				return true;
			}
		}

		return false;
	}
	private Computer CheckForCameraComputer() {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, 2f)) {
			return hit.collider.GetComponentInParent<Computer>();
		}
		return null;
	}

	public bool CanSee(GameObject target, float fov = 130f, float viewDist = 20f) {
		Vector3 targetPos = target.transform.position;
		targetPos.y = transform.position.y;
		float angle = Vector3.Dot(Vector3.Normalize(transform.position - targetPos), transform.forward);
		float angleDegrees = 90 + Mathf.Asin(angle) * Mathf.Rad2Deg;
		if (angleDegrees > fov / 2f) {
			// Debug.Log(target.name + " " + angle);
			return false;
		}

		RaycastHit[] hits;		
		hits = Physics.RaycastAll(transform.position, target.transform.position - transform.position, viewDist, sightLayers)
					  .Where(x => x.transform.root != transform.root)
					  .OrderBy(x => (x.point - transform.position).magnitude).ToArray();
		if (hits.Length > 0) {
			// return target.GetComponentsInChildren<Collider>().Contains(hit.collider);
			// if (hits[0].collider.transform.root != target.transform.root)
			// 	Debug.Log("hit " + hits[0].collider.transform.root.name + " instead");
			// else
			// 	Debug.Log("hit " + hits[0].collider.transform.root.name + " as intended");			
			return hits[0].collider.transform.root == target.transform.root;
		}

		// Debug.Log(target.name + " not hit");
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
		
		List<Character> draggableChars = GameManager.allCharacters.Where(x => {
			if (x is NPC) {
				NPC z = (NPC) x;
				return !x.isAlive || z.currentState == NPC.NPCState.HELD_HOSTAGE_TIED;
			}
			return !x.isAlive;
		}).ToList();

		List<Rigidbody> rbs = new List<Rigidbody>();
		foreach (Character c in draggableChars) {
			rbs.AddRange(c.separateBodyParts);
		}

		float grabRange = 1.5f;
		rbs = rbs.Where(x => (x.transform.position - transform.position).magnitude < grabRange)
				 .OrderBy(x => (x.transform.position - transform.position).magnitude).ToList();
		foreach (Rigidbody limb in rbs) {
			if (CanSee(limb.gameObject, 100f, grabRange)) {
				draggedBody = limb;
				break;
			} else {
				// Debug.Log("couldn't see " + limb.name);
			}
		}
	}

	protected void Drag() {
		if (draggedBody != null) {
			Vector3 dragPos = transform.position + transform.forward.normalized * 1.2f;
			// add a bit of a buffer between the floor and character to avoid friction
			dragPos.y = Mathf.Max(draggedBody.transform.position.y, .4f);
			Vector3 force = (dragPos - draggedBody.transform.position).normalized;
			draggedBody.AddForce(force * 10000f, ForceMode.Force);
		}
	}

	public void ReleaseBody() {
		if (draggedBody == null)
			return;
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
