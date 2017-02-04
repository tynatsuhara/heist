using UnityEngine;
using System.Collections;

public class MeleeWeapon : Gun {

	public float swingSpeed;
	private bool canSwing;
	public int swingDirection;

	public override bool Shoot() {
		bool couldSwing = canSwing;
		Melee();
		return couldSwing;
	}
	private void ResetShoot() {
		canSwing = true;
		volume.SetFrame(GUN_BASE_FRAME);
	}

	public override void Melee(int dir = 0) {
		if (!canSwing)
			return;
		canSwing = false;
		volume.SetFrame(ANIM_START_FRAME);
		base.Melee(swingDirection);
		Invoke("ResetShoot", swingSpeed);		
	}

	public override void Release() {}
	public override void Reload() {}
	public override void CancelReload() {}
	public override bool NeedsToReload() { return false; }
	
	public override void UpdateUI() {
		player.playerUI.UpdateAmmo(name);
	}
}
