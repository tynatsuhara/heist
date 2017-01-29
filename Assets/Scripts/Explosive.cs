using UnityEngine;
using System.Collections;

public class Explosive : MonoBehaviour {

	// Called when the user presses a button
	public virtual void Trigger() {}

	public void Explode(GameObject bomb, float radius) {
		int rayAmount = 50;
		for (int i = 0; i < rayAmount; i++) {
			float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
			var off = 2f / rayAmount;
			var y = i * off - 1 + (off / 2);
			var r = Mathf.Sqrt(1 - y * y);
			var phi = i * inc;
			var x = (float)(Mathf.Cos(phi) * r);
			var z = (float)(Mathf.Sin(phi) * r);
			Debug.DrawRay(bomb.transform.position, new Vector3(x, y, z), Color.red, 5f);
		}
	}
}
