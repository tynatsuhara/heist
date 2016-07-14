using UnityEngine;
using System.Collections;

public class VaultDoor : PossibleObjective, Interactable, Powerable {

	public Transform axis;
	public Transform door;
	public float openSpeed = 1.5f;
	public float openAngle = 90f;
	public float thermiteTime = 10f;
	public PicaVoxel.Exploder exploder;

	private bool burning;
	private bool opening;

	void Update() {
		if (burning)
			Burn();

		if (opening)
			ContinueOpen();
	}

	private void Burn() {
		thermiteTime -= Time.deltaTime;
		if (thermiteTime <= 0) {
			exploder.Explode();
			Open();
		}
	}
	
	public void Open() {
		if (isObjective)
			MarkCompleted();
		
		opening = true;
	}
	private void ContinueOpen() {
		float dist = Time.deltaTime * openSpeed;
		door.RotateAround(axis.position, Vector3.up, dist);
		openAngle -= dist;
		if (openAngle <= 0) {
			this.enabled = false;
		}
	}

	public void Interact(Character character) {
		if (!character.inventory.Has(Inventory.Item.THERMITE))
			return;

		character.inventory.Remove(Inventory.Item.THERMITE);
		burning = true;
	}

	public void Uninteract(Character character) {}

	public void Power() {
		Open();
	}
	public void Unpower() {}
}
