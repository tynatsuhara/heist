using UnityEngine;
using System.Linq;

public abstract class Gun : MonoBehaviour {

	public Character owner;
	public PicaVoxel.Volume volume;
	public GunAnimation anim;
	public float damage;
	public float range;
	public Vector3 inPlayerPos;
	public Vector3 inPlayerRot;

	public bool isPlayer;

	void Awake() {
		owner = transform.root.GetComponent<Character>();
		volume = GetComponent<PicaVoxel.Volume>();
		anim = GetComponent<GunAnimation>();
		isPlayer = owner.name == "Player";
	}

	abstract public void Drop(Vector3 force);
	abstract public void Melee();
	abstract public void Shoot();
	abstract public void Release();
	abstract public void Reload();
	abstract public void UpdateUI();

	public void RaycastShoot(Vector3 source, Vector3 direction) {
		RaycastHit[] hits = Physics.RaycastAll(source, direction, range)
			.Where(h => h.transform.root != transform.root)
			.OrderBy(h => h.distance)
			.ToArray();
		bool keepGoing = true;
		bool hitEnemy = false;
		for (int i = 0; i < hits.Length && keepGoing; i++) {
			Damageable damageScript = hits[i].transform.GetComponentInParent<Damageable>();
			if (damageScript == null)
				break;
			if (!hitEnemy) {
				Character c = hits[i].transform.GetComponentInParent<Character>();
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
