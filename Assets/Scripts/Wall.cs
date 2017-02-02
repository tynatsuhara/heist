using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour, Damageable {

	public float damangeThreshold;
	public bool canBeShotThrough;
	public PicaVoxel.Exploder exploder;
	public bool paintingNegativeWall;
	public bool paintingPositiveWall;
	public GameObject[] paintingPrefabs;

	void Start() {
		exploder.ValueFilter = canBeShotThrough ? 100 : 0;  // inner wall has val 100, outer has val 0

		if (paintingPositiveWall)
			SpawnPainting(1);
		if (paintingNegativeWall)
			SpawnPainting(-1);
	}

	private void SpawnPainting(int sign) {
		GameObject painting = Instantiate(paintingPrefabs[Random.Range(0, paintingPrefabs.Length)]) as GameObject;
		painting.transform.parent = transform;
		painting.transform.localPosition = new Vector3(sign * .25f, -.1f, sign * painting.GetComponent<PicaVoxel.Volume>().ZSize/20f);
		if (sign > 0)
			painting.transform.eulerAngles = new Vector3(0, 180f, 0);
	}

	// The return value is used for projectile damage. If the bullet should go
	// through the object and continue, return true. Otherwise return false.
	public bool Damage(Vector3 location, Vector3 angle, float damage, bool playerAttacker = false, DamageType type = DamageType.BULLET) {
		if (damage >= damangeThreshold && exploder != null) {
			for (int i = 0; i < (type == DamageType.EXPLOSIVE ? 10 : 1); i++) {
				exploder.ExplosionRadius = Random.Range(.05f, .25f);
				exploder.transform.position = location + new Vector3(Random.Range(-.1f, .1f), 
																	Random.Range(-.5f, 1f), 
																	Random.Range(-.1f, .1f));

				// If the wall is penetrable, destroy more of the middle/other side
				if (canBeShotThrough && Random.Range(0, 3) == 0) {
					exploder.transform.localPosition = new Vector3(Random.Range(-.1f, .1f), 
																exploder.transform.localPosition.y, 
																exploder.transform.localPosition.z);
					exploder.ExplosionRadius = Random.Range(0f, .1f);	
				}
				float explosionScale = 3f;
				exploder.Explode(angle * explosionScale);
			}
		}

		return damage >= damangeThreshold && canBeShotThrough;
	}
}

