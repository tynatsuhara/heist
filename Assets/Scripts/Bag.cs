using UnityEngine;
using System.Collections;

public class Bag : PossibleObjective, Interactable {

	private bool onGround;
	public float speedMultiplier = .75f;
	public Collider collider;

	void Start() {
		SetOnGround(true);
		GetComponent<Rigidbody>().mass = (1 - speedMultiplier) * 180 + 5;
	}

	public void Interact(Character character) {
		if (!onGround || character.hasBag)
			return;

		SetOnGround(false);
		MarkCompleted();
		character.AddBag(this);
	}

	public void Uninteract(Character character) {}

	public void DropBag() {
		SetOnGround(true);
	}

	private void SetOnGround(bool onGround) {
		GetComponent<Rigidbody>().isKinematic = !onGround;
		collider.enabled = onGround;
		this.onGround = onGround;
	}
}
