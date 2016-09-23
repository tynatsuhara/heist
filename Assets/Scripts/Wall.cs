using System;
using UnityEngine;

public class Wall : MonoBehaviour, Damageable {

	public float damangeThreshold;
	public bool canBeShotThrough;
	public PicaVoxel.Exploder exploder;

	// The return value is used for projectile damage. If the bullet should go
	// through the object and continue, return true. Otherwise return false.
	public bool Damage(Vector3 location, Vector3 angle, float damage) {
		if (damage >= damangeThreshold) {
			if (exploder != null) {
				float explosionScale = 3f;
				exploder.Explode(angle * explosionScale);
			}
		}

		return canBeShotThrough;
	}
}

