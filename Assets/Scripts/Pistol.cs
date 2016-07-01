using UnityEngine;
using System.Collections;

public class Pistol : Gun {

	public float knockback;
	
	public override int Value() { return 1; }

	public override void Shoot() {
		//owner.KnockBack(knockback);
		RaycastForward(transform.root.position, transform.root.forward, 1f);
	}

	public override void Release() {}
}
