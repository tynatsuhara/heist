using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Interactable {

	public bool locked;
	private int key;
	private bool open;

	public void Interact(Character character) {
		if (open)
			return;

		if (locked && character.inventory.Has(key))
			character.inventory.Remove(key);
		else if (locked)
			return;

		open = true;
		Debug.Log("open door");
	}

	public void Cancel(Character character) {}

	public void SetKey(Inventory.Item key) {
		SetKey((int)key);
	}
	public void SetKey(int key) {
		this.key = key;
	}
}
