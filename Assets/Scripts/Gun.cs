using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class Gun : MonoBehaviour {

	public Character owner;
	public PicaVoxel.Volume volume;
	public GunAnimation anim;
	public float damage;
	public float range;
	public int maxEnemiesMelee = 1;
	public Vector3 inPlayerPos;
	public Collider droppedCollider;

	protected bool enqueuedReload = false;

	// Gun frames
	public const int GUN_BASE_FRAME = 0;
	public const int DROPPED_GUN_FRAME = 1;	 
	public const int ANIM_START_FRAME = 2;

	public bool isPlayer;
	public PlayerControls player;

	public virtual void Start() {
		owner = transform.root.GetComponent<Character>();
		volume = GetComponent<PicaVoxel.Volume>();
		anim = GetComponent<GunAnimation>();
	}

	// returns true if the attack executed that frame (ie gun fired)
	abstract public bool Shoot();
	abstract public void Release();
	abstract public void Reload();
	abstract public void CancelReload();
	abstract public bool NeedsToReload();
	abstract public void UpdateUI();

	public void RaycastShoot(Vector3 source, Vector3 direction) {
		RaycastHit[] hits = Physics.RaycastAll(source, direction, range)
			.Where(h => h.transform.root != transform.root)
			.OrderBy(h => h.distance)
			.ToArray();
		bool keepGoing = true;
		bool hitEnemy = false;
		for (int i = 0; i < hits.Length && keepGoing; i++) {
			Damageable damageScript = hits[i].transform.GetComponentInParent<Damageable>();
			if (damageScript == null)
				break;
			if (!hitEnemy) {
				Character c = hits[i].transform.GetComponentInParent<Character>();
				hitEnemy = c != null && c.isAlive && !(c is PlayerControls);
			}
			keepGoing = damageScript.Damage(hits[i].point, direction.normalized, damage, isPlayer);
		}
		if (hitEnemy && isPlayer) {
			player.playerUI.HitMarker();
		}
	}

	public virtual void Drop(Vector3 force) {
		CancelInvoke();
		volume.SetFrame(DROPPED_GUN_FRAME);
		droppedCollider.enabled = true;
		transform.parent = null;
		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
		GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * Random.Range(10f, 100f), ForceMode.Force);
		owner = null;
	}

	public bool meleeing;
	private float meleeDirection;
	public virtual void Melee(DamageType type = DamageType.MELEE, int dir = 0) {
		if (meleeing)
			return;
		meleeDirection = dir != 0 ? Mathf.Clamp(dir, -1, 1) : Random.Range(0, 2) * 2 - 1;
		meleeing = true;
		CancelReload();  // interrupt reloading to melee, if necessary
		StartCoroutine("MeleeAnimation");
		int range = 8;
		List<Damageable> hits = new List<Damageable>();
		for (int i = -range/2; i <= range/2; i++) {
			// Debug.DrawLine(owner.transform.position, 
			// 		owner.transform.position + owner.transform.forward * 1.3f + meleeDirection * i * owner.transform.right / 5f, 
			// 		Color.red, 5f);
			RaycastHit[] rhits = Physics.RaycastAll(owner.transform.position, owner.transform.forward * 1.3f + meleeDirection * i * owner.transform.right / 5f, 1.5f)
					.Where(h => h.transform.root != transform.root && h.collider.GetComponentInParent<Damageable>() != null && !hits.Contains(h.collider.GetComponentInParent<Damageable>()))
					.OrderBy(h => h.distance)
					.ToArray();
			if (rhits.Length > 0) {
				Damageable d = rhits[0].collider.GetComponentInParent<Damageable>();
				d.Damage(rhits[0].collider.transform.root.position, owner.transform.forward, 1f, isPlayer, type);
				if (rhits[0].collider.GetComponentInParent<Character>() != null)
					MeleeHitPlayerCallback();
				hits.Add(d);
			}
			if (hits.Count == maxEnemiesMelee)
				break;
		}
	}
	private IEnumerator MeleeAnimation() {
		int angle = 40;
		Quaternion initialRotation = transform.localRotation;
		Vector3 initialPosition = transform.localPosition;
		transform.RotateAround(transform.root.position, Vector3.up, angle * meleeDirection);
		Quaternion end = transform.localRotation;
		transform.RotateAround(transform.root.position, Vector3.up, -2.4f * angle * meleeDirection);
		float diff = 100f;			
		while (diff > .03f) {
			Vector3 nextRot = Quaternion.Lerp(transform.localRotation, end, .3f).eulerAngles;
			diff = Mathf.Abs(nextRot.y - transform.localRotation.eulerAngles.y);
			transform.RotateAround(transform.root.position, Vector3.up, diff * meleeDirection);
			yield return new WaitForSeconds(.01f);
		}
		diff = 100f;
		end = initialRotation;
		while (diff > .05f) {
			Vector3 nextRot = Quaternion.Lerp(transform.localRotation, end, .4f).eulerAngles;
			diff = Mathf.Abs(transform.localRotation.eulerAngles.y - nextRot.y);
			transform.RotateAround(transform.root.position, Vector3.up, -diff * meleeDirection);
			yield return new WaitForSeconds(.01f);
		}
		transform.localRotation = initialRotation;
		transform.localPosition = initialPosition;

		meleeing = false;
		if (enqueuedReload) {
			Reload();
		}
	}
	protected virtual void MeleeHitPlayerCallback() {}

	public void SetLoweredPosition(bool lowered) {
		transform.localPosition = inPlayerPos + (Vector3.down + Vector3.back) * (lowered ? .1f : 0f);
	}

	protected bool delayed = false;
	public void DelayAttack(float delay) {
		CancelInvoke("UnDelay");
		delayed = true;
		SetLoweredPosition(true);
		Invoke("UnDelay", delay);
	}
	
	private void UnDelay() {
		SetLoweredPosition(false);		
		delayed = false;
	}

	public void PlayerEffects(float power, float duration) {
		if (isPlayer) {
			transform.root.GetComponent<PlayerControls>().playerCamera.Shake(power, duration);
		}
	}
}
