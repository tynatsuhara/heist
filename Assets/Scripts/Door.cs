using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Interactable {

	public bool locked;
	private int key;
	private bool open;

	public void InteractStart(Character character) {
		if (open)
			return;

		Debug.Log("open door");
	}

	public void InteractStop(Character character) {}

	public void SetKey(Inventory.Item key) {
		SetKey((int)key);
	}
	public void SetKey(int key) {
		this.key = key;
	}
}
