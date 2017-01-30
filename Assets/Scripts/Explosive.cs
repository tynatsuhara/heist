using UnityEngine;
using System.Collections;
using System.Linq;

public class Explosive : MonoBehaviour {

	private bool isPlayer;
	void Awake() {
		isPlayer = GetComponent<PlayerControls>() != null;
	}

	// Called when the user presses a button
	public virtual void Trigger() {}

	public void Explode(GameObject bomb, float radius, int rayAmount, float damagePerRay) {
		for (int k = 0; k < rayAmount; k++) {
			float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
			var off = 2f / rayAmount;
			var y = k * off - 1 + (off / 2);
			var r = Mathf.Sqrt(1 - y * y);
			var phi = k * inc;
			var x = (float)(Mathf.Cos(phi) * r);
			var z = (float)(Mathf.Sin(phi) * r);
			Debug.DrawRay(bomb.transform.position, new Vector3(x, y, z) * radius, Color.red, 5f);
			
			RaycastHit[] hits = Physics.RaycastAll(bomb.transform.position, new Vector3(x, y, z), radius)
				.OrderBy(h => h.distance)
				.ToArray();
			bool keepGoing = true;
			for (int i = 0; i < hits.Length && keepGoing; i++) {
				Damageable damageScript = hits[i].transform.GetComponentInParent<Damageable>();
				if (damageScript == null)
					break;
				Debug.Log("damaged " + hits[i].collider.transform.root.name);
				keepGoing = damageScript.Damage(hits[i].point, new Vector3(x, y, z), damagePerRay, false, isPlayer, true);
			}
		}
	}
}
