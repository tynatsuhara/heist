using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Interactable, Powerable {

	public Door mirror;
	public bool locked;
	public Inventory.Item key;
	private int key_;
	private bool open;
	public GameObject[] doorStates;
	private TextObject text;
	private int state;

	public Teleporter openRightTeleporter;
	public Teleporter openLeftTeleporter;


	public void Start() {
		text = GetComponentInChildren<TextObject>();
	}

	public void Interact(Character character) {		
		if (key != Inventory.Item.NONE)
			key_ = (int)key;
		
		if (locked && character.inventory.Has(key_)) {
			character.inventory.Remove(key_);
			locked = false;
		} else if (locked) {
			text.Say("locked");
			return;
		}

		if (open) {
			SetState(0);
		} else {
			// open away from the character
			float distFrom1 = (doorStates[1].transform.position - character.transform.position).magnitude;
			float distFrom2 = (doorStates[2].transform.position - character.transform.position).magnitude;
			SetState(distFrom1 > distFrom2 ? 1 : 2);
		}
		open = !open;
	}

	private void SetState(int state) {
		this.state = state;
		for (int i = 0; i < doorStates.Length; i++) {
			doorStates[i].SetActive(i == state);
		}
	}

	public void Mirror(Door other) {
		mirror = other;
		SetState(other.state);
		openRightTeleporter.translation = -transform.position + other.transform.position;
		other.openLeftTeleporter.translation = openRightTeleporter.translation * -1f;
		openLeftTeleporter.translation = -transform.position + other.transform.position;
		other.openRightTeleporter.translation = openLeftTeleporter.translation * -1f;
		this.locked = other.locked;
		this.key = other.key;
		this.key_ = other.key_;
		this.open = other.open;
	}

	public void Mirror() {
		mirror.Mirror(this);
	}

	public void Uninteract(Character character) {}

	public void Power() {
		if (locked)
			text.Say("unlocked", showFlash:true, color:"green");
		locked = false;
	}

	public void Unpower() {}

	public void SetKey(Inventory.Item key) {
		SetKey((int)key);
	}
	public void SetKey(int key) {
		this.key_ = key;
	}
}
