using UnityEngine;
using System.Linq;

public abstract class Gun : MonoBehaviour {

	public Character owner;
	public PicaVoxel.Volume volume;
	public PicaVoxel.BasicAnimator anim;
	public float damage;
	public float range;

	void Awake() {
		owner = transform.root.GetComponent<Character>();
		volume = GetComponent<PicaVoxel.Volume>();
		anim = GetComponent<PicaVoxel.BasicAnimator>();
	}

	abstract public void Shoot();
	abstract public void Release();
	abstract public void Reload();

	public void RaycastShoot(Vector3 source, Vector3 direction) {
		RaycastHit[] hits = Physics.RaycastAll(source, direction, range)
			.OrderBy(h => h.distance)
			.ToArray();
		bool keepGoing = true;
		for (int i = 0; i < hits.Length && keepGoing; i++) {
			Damageable damageScript = hits[i].transform.root.GetComponent<Damageable>();
			if (damageScript == null)
				return;
			
			keepGoing = damageScript.Damage(hits[i].point, direction.normalized, damage);
		}
	}

	public void ScreenShake(float power, float duration) {
		if (transform.root.name == "Player")
			CameraMovement.instance.Shake(power, duration);
	}
}
