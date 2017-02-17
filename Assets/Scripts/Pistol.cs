using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Pistol : Gun {

	public float knockback;
	public float shootSpeed;
	private bool shooting;
	private bool reloading;
	public bool silenced;
	public int clipSize = 15;
	private int bulletsFired = 0;
	public float reloadSpeed = 1f;
	public float shakePower = .3f;
	public float shakeLength = .3f;
	public bool shellDropOnFire;
	public bool shellDropOnReload;

	protected Vector3 shellDropLocation;

	public override void Awake() {
		base.Awake();
		shellDropLocation = transform.root.position + transform.root.forward * .45f;
	}

	public override bool Shoot() {
		if (delayed || shooting || reloading || bulletsFired == clipSize || owner == null)
			return false;
		
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
			PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(shellDropLocation,
				vox, .05f, (transform.up - transform.right) * 2.5f + Random.insideUnitSphere * .5f);
		}
		
		PlayerEffects(shakePower, shakeLength);

		if (!silenced && isPlayer) {
			GameManager.instance.AlertInRange(Character.Reaction.AGGRO, 
				transform.position, 15f, visual: (silenced ? transform.root.gameObject : null));
		}
		return true;
	}

	public override void Reload() {
		if (reloading || bulletsFired == 0)  // already reloading or no need to
			return;
		if (meleeing || shooting) {  // reload ASAP
			enqueuedReload = true;
			return;
		}
		CancelReload();
		SetLoweredPosition(true);
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
			SetLoweredPosition(false);
		} else if (enqueuedReload) {
			enqueuedReload = false;
			Reload();
		}
	}

	public override void CancelReload() {
		if (!reloading)
			return;
		SetLoweredPosition(false);
		CancelInvoke("ResetShoot");
		reloading = false;
	}

	public override void UpdateUI() {
		if (player.playerUI == null)
			return;
		else if (reloading)
			player.playerUI.ShowReloading(name);
		else
			player.playerUI.UpdateAmmo(name, clipSize - bulletsFired, clipSize);
	}

	public override void Melee(DamageType type = DamageType.MELEE, int dir = 0) {
		if (shooting)
			return;
		base.Melee(type, dir);
	}

	public override void Release() {}
}
