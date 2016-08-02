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

		byte[] bytes = new byte[6];
		bytes[0] = (byte)PicaVoxel.VoxelState.Active;
		PicaVoxel.Voxel vox = new PicaVoxel.Voxel(bytes);
		PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(transform.root.position + transform.root.forward * .45f,
			vox, .05f, (transform.up - transform.right) * 2.5f + Random.insideUnitSphere * .5f);
		
		ScreenShake(.3f, .3f);

		if (!silenced)
			GameManager.instance.AlertInRange(Character.Reaction.AGGRO, 
				transform.position, 15f, visual: (silenced ? transform.root.gameObject : null));
	}

	public override void Reload() {
		if (bulletsFired == clipSize)
			return;
		
		CancelInvoke("ResetShoot");
		bulletsFired = clipSize;
		canShoot = false;
		Invoke("ResetShoot", shootSpeed + reloadSpeed);

	}

	private void ResetShoot() {
		canShoot = true;
		if (bulletsFired % clipSize == 0) {
			bulletsFired = 0;
		}
	}

	public override void Release() {}
}
