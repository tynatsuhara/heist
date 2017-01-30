using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Pistol : Gun {

	public float knockback;
	public float shootSpeed;
	private bool shooting;
	private bool reloading;
	private bool meleeing;
	public bool silenced;
	public int clipSize = 15;
	private int bulletsFired = 0;
	public float reloadSpeed = 1f;
	public float shakePower = .3f;
	public float shakeLength = .3f;
	public Collider droppedCollider;
	public bool shellDropOnFire;
	public bool shellDropOnReload;

	// Gun frames
	private const int DROPPED_GUN_FRAME = 1;	 
	private const int ANIM_START_FRAME = 2;

	public override void Shoot() {
		if (delayed || shooting || reloading || bulletsFired == clipSize)
			return;
		
		owner.KnockBack(knockback);
		RaycastShoot(transform.root.position, -transform.forward);
		// SetFrame before Play to avoid delay
		volume.SetFrame(ANIM_START_FRAME);
		anim.Shoot();
		shooting = true;
		bulletsFired++;
		Invoke("ResetShoot", shootSpeed);
		
		if (shellDropOnFire) {
			byte[] bytes = new byte[6];
			bytes[0] = (byte)PicaVoxel.VoxelState.Active;
			PicaVoxel.Voxel vox = new PicaVoxel.Voxel(bytes);
			PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(transform.root.position + transform.root.forward * .45f,
				vox, .05f, (transform.up - transform.right) * 2.5f + Random.insideUnitSphere * .5f);
		}
		
		PlayerEffects(shakePower, shakeLength);

		if (!silenced && isPlayer) {
			GameManager.instance.AlertInRange(Character.Reaction.AGGRO, 
				transform.position, 15f, visual: (silenced ? transform.root.gameObject : null));
		}
	}

	public override void Reload() {
		if (shooting || reloading || meleeing || bulletsFired == 0)  // already reloading or no need to
			return;
		CancelReload();
		SetReloadPosition(true);
		reloading = true;			
		Invoke("ResetShoot", shootSpeed + reloadSpeed);
	}

	public override bool NeedsToReload() {
		return bulletsFired == clipSize;
	}

	private void ResetShoot() {
		shooting = false;
		if (reloading) {  // just finished reloading
			if (shellDropOnReload) {
				for (int i = 0; i < bulletsFired; i++) {
					byte[] bytes = new byte[6];
					bytes[0] = (byte)PicaVoxel.VoxelState.Active;
					PicaVoxel.Voxel vox = new PicaVoxel.Voxel(bytes);
					PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(transform.root.position + 
							transform.root.forward * .45f + Vector3.down * .2f,
							vox, .05f, Random.insideUnitSphere * .5f);
				}
			}

			reloading = false;
			bulletsFired = 0;			
			SetReloadPosition(false);
		}
	}

	public override void CancelReload() {
		if (!reloading)
			return;
		SetReloadPosition(false);
		CancelInvoke("ResetShoot");
		reloading = false;
	}

	private void SetReloadPosition(bool reloading) {
		transform.localPosition = inPlayerPos + (Vector3.down + Vector3.back) * (reloading ? .1f : 0f);
	}

	public override void Drop(Vector3 force) {
		CancelInvoke();
		volume.SetFrame(DROPPED_GUN_FRAME);
		droppedCollider.enabled = true;
		transform.parent = null;
		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
		GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * Random.Range(10f, 100f), ForceMode.Force);
		owner = null;
	}

	private bool delayed = false;
	public override void DelayAttack(float delay) {
		CancelInvoke("UnDelay");
		delayed = true;
		SetReloadPosition(true);
		Invoke("UnDelay", delay);
	}
	private void UnDelay() {
		SetReloadPosition(false);		
		delayed = false;
	}

	public override void UpdateUI() {
		if (reloading)
			player.playerUI.ShowReloading(name);
		else
			player.playerUI.UpdateAmmo(name, clipSize - bulletsFired, clipSize);
	}

	public override void Melee() {
		if (shooting || meleeing)
			return;
		meleeing = true;
		CancelReload();  // interrupt reloading to melee, if necessary
		StartCoroutine("MeleeAnimation");
		List<Character> chars = GameManager.instance.CharactersWithinDistance(owner.transform.position + 
																			  owner.transform.forward * 1.1f, .6f);
		foreach (Character c in chars) {
			if (owner.CanSee(c.gameObject, 90)) {
				c.Damage(c.transform.position, owner.transform.forward, 1f, melee: true);
				player.playerUI.HitMarker();
				break;
			}
		}
	}
	private IEnumerator MeleeAnimation() {
		int angle = 40;
		Quaternion initialRotation = transform.localRotation;
		Vector3 initialPosition = transform.localPosition;
		transform.RotateAround(transform.root.position, Vector3.up, angle);
		Quaternion end = transform.localRotation;
		transform.RotateAround(transform.root.position, Vector3.up, -2.4f * angle);
		float diff = 100f;			
		while (diff > .03f) {
			Vector3 nextRot = Quaternion.Lerp(transform.localRotation, end, .3f).eulerAngles;
			diff = (nextRot.y - transform.localRotation.eulerAngles.y);
			transform.RotateAround(transform.root.position, Vector3.up, diff);
			yield return new WaitForSeconds(.01f);
		}
		diff = 100f;
		end = initialRotation;
		while (diff > .05f) {
			Vector3 nextRot = Quaternion.Lerp(transform.localRotation, end, .4f).eulerAngles;
			diff = (transform.localRotation.eulerAngles.y - nextRot.y);
			transform.RotateAround(transform.root.position, Vector3.up, -diff);
			yield return new WaitForSeconds(.01f);
		}
		transform.localRotation = initialRotation;
		transform.localPosition = initialPosition;

		meleeing = false;
	}

	public override void Release() {}
}
