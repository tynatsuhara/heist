using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Interactable, Powerable {

	public bool locked;
	public Inventory.Item key;
	private int key_;
	private bool open;
	public GameObject[] doorStates;

	public void Interact(Character character) {		
		if (key != Inventory.Item.NONE)
			key_ = (int)key;
		
		if (locked && character.inventory.Has(key_)) {
			character.inventory.Remove(key_);
			locked = false;
		} else if (locked) {
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
		for (int i = 0; i < doorStates.Length; i++) {
			doorStates[i].SetActive(i == state);
		}
	}

	public void Uninteract(Character character) {}

	public void Power() {
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
