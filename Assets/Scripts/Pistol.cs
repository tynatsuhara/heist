using UnityEngine;
using System.Collections.Generic;

public class Pistol : Gun {

	public float knockback;
	public float shootSpeed;
	private bool canShoot = true;
	public bool silenced;
	public int clipSize = 15;
	private int bulletsFired = 0;
	public float reloadSpeed = 1f;
	public float shakePower = .3f;
	public float shakeLength = .3f;
	public Collider droppedCollider;

	// Gun frames
	private const int DROPPED_GUN_FRAME = 1;	 
	private const int ANIM_START_FRAME = 2;

	public override void Shoot() {
		if (!canShoot)
			return;
		
		owner.KnockBack(knockback);
		RaycastShoot(transform.root.position, transform.root.forward);
		// SetFrame before Play to avoid delay
		volume.SetFrame(ANIM_START_FRAME);
		anim.Shoot();
		canShoot = false;
		bulletsFired++;
		Invoke("ResetShoot", shootSpeed + (bulletsFired == clipSize ? reloadSpeed : 0));
		if (bulletsFired == clipSize) {
			transform.Translate(Vector3.down * .1f);
			transform.Translate(Vector3.forward * .1f);
		}

		byte[] bytes = new byte[6];
		bytes[0] = (byte)PicaVoxel.VoxelState.Active;
		PicaVoxel.Voxel vox = new PicaVoxel.Voxel(bytes);
		PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(transform.root.position + transform.root.forward * .45f,
			vox, .05f, (transform.up - transform.right) * 2.5f + Random.insideUnitSphere * .5f);
		
		PlayerEffects(shakePower, shakeLength);

		if (!silenced && isPlayer) {
			GameManager.instance.AlertInRange(Character.Reaction.AGGRO, 
				transform.position, 15f, visual: (silenced ? transform.root.gameObject : null));
		}

		if (isPlayer) {
			UpdateUI();
		}
	}

	public override void Reload() {
		if (bulletsFired == clipSize || bulletsFired == 0)
			return;
		
		CancelInvoke("ResetShoot");
		bulletsFired = clipSize;
		canShoot = false;
		transform.Translate(Vector3.down * .1f);
		transform.Translate(Vector3.forward * .1f);
		Invoke("ResetShoot", shootSpeed + reloadSpeed);
	}

	private void ResetShoot() {
		canShoot = true;
		if (bulletsFired % clipSize == 0) {  // just finished reloading
			bulletsFired = 0;
			transform.Translate(Vector3.up * .1f);
			transform.Translate(Vector3.back * .1f);
			if (isPlayer) {
				UpdateUI();
			}
		}
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

	public override void UpdateUI() {
		GameUI.instance.UpdateAmmo(clipSize - bulletsFired, clipSize);
	}

	public override void Melee() {
		List<Character> chars = GameManager.instance.CharactersWithinDistance(owner.transform.position + owner.transform.forward * 1f, .5f);
		foreach (Character c in chars) {
			if (owner.CanSee(c.gameObject, 90)) {
				c.Damage(c.transform.position, owner.transform.forward, 1f, melee: true);
				break;
			}
		}
	}

	public override void Release() {}
}
