using UnityEngine;
using System.Collections;

public class Bag : PossibleObjective, Interactable {

	private bool onGround;
	public float speedMultiplier = .75f;
	public Collider collider;

	void Start() {
		onGround = true;
	}

	public void Interact(Character character) {
		if (!onGround || character.hasBag)
			return;

		GetComponent<Rigidbody>().isKinematic = true;
		collider.enabled = false;
		character.AddBag(this);		
	}

	public void Uninteract(Character character) {

	}

	public void DropBag() {
		GetComponent<Rigidbody>().isKinematic = false;
		collider.enabled = true;
	}
}
