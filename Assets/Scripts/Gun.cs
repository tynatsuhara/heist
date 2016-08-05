using UnityEngine;
using System.Linq;

public abstract class Gun : MonoBehaviour {

	public Character owner;
	public PicaVoxel.Volume volume;
	public PicaVoxel.BasicAnimator anim;
	public float damage;
	public float range;

	private bool isPlayer;

	void Awake() {
		owner = transform.root.GetComponent<Character>();
		volume = GetComponent<PicaVoxel.Volume>();
		anim = GetComponent<PicaVoxel.BasicAnimator>();
		isPlayer = owner.name == "Player";
	}

	abstract public void Shoot();
	abstract public void Release();
	abstract public void Reload();

	public void RaycastShoot(Vector3 source, Vector3 direction) {
		RaycastHit[] hits = Physics.RaycastAll(source, direction, range)
			.OrderBy(h => h.distance)
			.Where(h => h.transform.root.name != "Player")
			.ToArray();
		bool keepGoing = true;
		bool hitEnemy = false;
		for (int i = 0; i < hits.Length && keepGoing; i++) {
			Damageable damageScript = hits[i].transform.root.GetComponent<Damageable>();
			if (damageScript == null)
				break;
			if (!hitEnemy) {
				Character c = hits[i].transform.root.GetComponent<Character>();
				hitEnemy = c != null && c.isAlive;
			}
			keepGoing = damageScript.Damage(hits[i].point, direction.normalized, damage);
		}
		if (hitEnemy && isPlayer) {
			GameUI.instance.HitMarker();
		}
	}

	public void PlayerEffects(float power, float duration) {
		if (isPlayer) {
			CameraMovement.instance.Shake(power, duration);
		}
	}
}
