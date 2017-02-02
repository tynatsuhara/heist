using UnityEngine;
using System.Collections;

public class DualPistol : Pistol {

	public PicaVoxel.Volume volume2;
	public GunAnimation anim2;
	public Collider droppedCollider2;

	void Start() {
		owner = transform.root.GetComponent<Character>();
		if (Random.Range(0, 2) == 0)
			SwapArms();
	}

	public override bool Shoot() {
		shellDropLocation = volume.transform.position + transform.forward * .45f;		
		if (base.Shoot()) {
			SwapArms();
			return true;
		}
		return false;
	}

	private void SwapArms() {
		GunAnimation temp = anim;
		anim = anim2;
		anim2 = temp;
		PicaVoxel.Volume tempV = volume;
		volume = volume2;
		volume2 = tempV;
	}

	public override void Drop(Vector3 force) {
		CancelInvoke();
		volume.SetFrame(DROPPED_GUN_FRAME);
		volume2.SetFrame(DROPPED_GUN_FRAME);
		droppedCollider2.enabled = droppedCollider.enabled = true;
		transform.parent = null;
		foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
			rb.isKinematic = false;
			rb.AddForce(force, ForceMode.Impulse);
			rb.AddTorque(Random.insideUnitSphere * Random.Range(10f, 100f), ForceMode.Force);
		}
		owner = null;
	}	
}
