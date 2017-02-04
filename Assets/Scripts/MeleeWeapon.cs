using UnityEngine;
using System.Collections;

public class MeleeWeapon : Gun {

	public float swingSpeed;
	private bool canSwing = true;
	public int swingDirection;
	public DamageType damageType;

	public override bool Shoot() {
		bool couldSwing = canSwing;
		Melee(DamageType.SLICE, swingDirection);
		return couldSwing;
	}
	private void ResetShoot() {
		canSwing = true;
		volume.SetFrame(GUN_BASE_FRAME);
	}

	public override void Melee(DamageType type = DamageType.MELEE, int dir = 0) {
		if (!canSwing)
			return;
		canSwing = false;
		volume.SetFrame(ANIM_START_FRAME);
		base.Melee(type, swingDirection);
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
