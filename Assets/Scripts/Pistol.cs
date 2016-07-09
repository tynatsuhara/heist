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
		RaycastShoot(transform.root.position, transform.root.forward, 1f);
		volume.SetFrame(1);
		anim.Play();
		canShoot = false;
		Invoke("ResetShoot", shootSpeed);

		ScreenShake(.3f, .3f);
	}

	private void ResetShoot() {
		canShoot = true;
	}

	public override void Release() {}
}
