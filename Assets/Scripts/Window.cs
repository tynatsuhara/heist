using UnityEngine;
using System.Collections;

public class Window : MonoBehaviour, Damageable {

	public PicaVoxel.Exploder exploder;

	public bool Damage(Vector3 location, Vector3 angle, float damage) {
		exploder.transform.position = location;
		exploder.ExplosionRadius = Random.Range(.4f, .9f);
		exploder.Explode();
		Debug.Log("hit window");
		return true;
	}
}
