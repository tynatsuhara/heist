using System;
using UnityEngine;

public class Wall : MonoBehaviour, Damageable {

	public float damangeThreshold;
	public bool canBeShotThrough;
	public PicaVoxel.Exploder exploder;

	// The return value is used for projectile damage. If the bullet should go
	// through the object and continue, return true. Otherwise return false.
	public bool Damage(Vector3 location, Vector3 angle, float damage, bool melee = false, bool playerAttacker = false) {
		if (damage >= damangeThreshold && canBeShotThrough) {
			if (exploder != null) {
				exploder.ExplosionRadius = UnityEngine.Random.Range(.1f, .25f);
				exploder.transform.position = location + new Vector3(UnityEngine.Random.Range(-.1f, .1f), 
																	 UnityEngine.Random.Range(-.5f, 1f), 
																	 UnityEngine.Random.Range(-.1f, .1f));
				float explosionScale = 3f;
				exploder.Explode(angle * explosionScale);
			}
		} else {
			// discoloration of Wall?
			
		}

		return canBeShotThrough;
	}
}

