using UnityEngine;
using System.Collections;

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

	public override void Shoot() {
		if (!canShoot)
			return;
		
		owner.KnockBack(knockback);
		RaycastShoot(transform.root.position, transform.root.forward);
		volume.SetFrame(1);
		anim.Play();
		canShoot = false;
		bulletsFired++;
		Invoke("ResetShoot", shootSpeed + (bulletsFired == clipSize ? reloadSpeed : 0));
		if (bulletsFired == clipSize) {
			transform.Translate(Vector3.down * .1f);
		}

		byte[] bytes = new byte[6];
		bytes[0] = (byte)PicaVoxel.VoxelState.Active;
		PicaVoxel.Voxel vox = new PicaVoxel.Voxel(bytes);
		PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(transform.root.position + transform.root.forward * .45f,
			vox, .05f, (transform.up - transform.right) * 2.5f + Random.insideUnitSphere * .5f);
		
		PlayerEffects(shakePower, shakeLength);

		if (!silenced) {
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
		Invoke("ResetShoot", shootSpeed + reloadSpeed);
	}

	private void ResetShoot() {
		canShoot = true;
		if (bulletsFired % clipSize == 0) {  // just finished reloading
			bulletsFired = 0;
			transform.Translate(Vector3.up * .1f);
			if (isPlayer) {
				UpdateUI();
			}
		}
	}

	public override void UpdateUI() {
		GameUI.instance.UpdateAmmo(clipSize - bulletsFired, clipSize);
	}

	public override void Release() {}
}
