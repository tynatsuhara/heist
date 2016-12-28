using UnityEngine;
using System.Collections;

public class Window : MonoBehaviour, Damageable {

	public PicaVoxel.Exploder exploder;

	public bool Damage(Vector3 location, Vector3 angle, float damage, bool melee = false, bool playerAttacker = false) {
		exploder.transform.position = location + new Vector3(0f, Random.Range(-.3f, .6f) , 0f);
		exploder.ExplosionRadius = Random.Range(.3f, .6f);
		exploder.Explode();
		return true;
	}
}
