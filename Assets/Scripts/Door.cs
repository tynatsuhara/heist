using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Interactable {

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
			doorStates[0].SetActive(true);
			doorStates[1].SetActive(false);
			doorStates[2].SetActive(false);
		} else {
			doorStates[0].SetActive(false);
			// open away from the character
			float distFrom1 = (doorStates[1].transform.position - character.transform.position).magnitude;
			float distFrom2 = (doorStates[2].transform.position - character.transform.position).magnitude;
			if (distFrom1 > distFrom2) {
				doorStates[1].SetActive(true);
				doorStates[2].SetActive(false);
			} else {
				doorStates[2].SetActive(true);
				doorStates[1].SetActive(false);
			}
		}
		open = !open;
	}

	public void Cancel(Character character) {}

	public void SetKey(Inventory.Item key) {
		SetKey((int)key);
	}
	public void SetKey(int key) {
		this.key_ = key;
	}
}
