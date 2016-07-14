using UnityEngine;
using System.Collections;

public class Computer : MonoBehaviour, Interactable {

	public Powerable[] connectedItems;

	public void Interact(Character character) {
		if (connectedItems == null)
			return;

		foreach (Powerable item in connectedItems) {
			item.Power();
		}
	}

	public void Uninteract(Character character) {}
}
