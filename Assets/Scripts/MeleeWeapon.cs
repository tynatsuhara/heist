using UnityEngine;
using System.Collections;

public class MeleeWeapon : Gun {

	public float swingSpeed;
	private bool canSwing;

	public override bool Shoot() {
		bool couldSwing = canSwing;
		Melee();
		return couldSwing;
	}
	private void ResetShoot() {
		canSwing = true;
	}

	public override void Melee() {
		if (!canSwing)
			return;
		canSwing = false;
		volume.SetFrame(ANIM_START_FRAME);
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
