using UnityEngine;
using System.Collections;

public class Pistol : Gun {

	public float knockback;
	public float shootSpeed;
	private bool canShoot = true;

	public override void Shoot() {
		if (!canShoot)
			return;
		
		owner.KnockBack(knockback);
		RaycastShoot(transform.root.position, transform.root.forward);
		volume.SetFrame(1);
		anim.Play();
		canShoot = false;
		Invoke("ResetShoot", shootSpeed);

		byte[] bytes = new byte[6];
		bytes[0] = (byte)PicaVoxel.VoxelState.Active;
		PicaVoxel.Voxel vox = new PicaVoxel.Voxel(bytes);
		PicaVoxel.VoxelParticleSystem.Instance.SpawnSingle(transform.root.position + transform.root.forward * .45f,
			vox, .05f, (transform.up - transform.right) * 2.5f);
				
		ScreenShake(.3f, .3f);
	}

	private void ResetShoot() {
		canShoot = true;
	}

	public override void Release() {}
}
