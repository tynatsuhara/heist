using UnityEngine;
using System.Collections;

public class Bag : PossibleObjective, Interactable {

	private bool onGround;
	public float speedMultiplier = .75f;
	public Collider collider;
	public string lootCategory;
	public int dollarAmount;
	private bool putInGetaway;

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
		GetComponent<PicaVoxel.Volume>().SetFrame(onGround ? 0 : 1);
		collider.enabled = onGround;
		this.onGround = onGround;
	}

	void OnCollisionEnter(Collision collision) {
		if (!onGround)
			return;

		if (collision.collider.GetComponentInParent<Car>() == GameManager.instance.getaway) {
			SaveLoot();
		}
	}

	public void SaveLoot() {
		if (putInGetaway)
			return;
		
		putInGetaway = true;
		GameManager.instance.AddLoot(lootCategory, dollarAmount);
		gameObject.SetActive(false);
	}
}
