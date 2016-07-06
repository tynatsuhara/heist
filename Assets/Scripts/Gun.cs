﻿using UnityEngine;

public abstract class Gun : MonoBehaviour {

	public Character owner;
	public PicaVoxel.Volume volume;
	public PicaVoxel.BasicAnimator anim;

	void Start() {
		owner = transform.root.GetComponent<Character>();
		volume = GetComponent<PicaVoxel.Volume>();
		anim = GetComponent<PicaVoxel.BasicAnimator>();
	}

	abstract public void Shoot();
	abstract public void Release();

	public void RaycastShoot(Vector3 source, Vector3 direction, float damageVal, float range = 30f) {
		RaycastHit hit;
		// Debug.DrawRay(source, direction * range, Color.red, 3f);
		if (Physics.Raycast(source, direction, out hit, range)) {
			Damageable damageScript = hit.transform.root.GetComponent<Damageable>();
			if (damageScript != null) {
				damageScript.Damage(hit.point, direction.normalized, damageVal);
			}
		}
	}
}
