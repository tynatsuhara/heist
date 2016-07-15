using UnityEngine;
using System.Collections;

public class Computer : MonoBehaviour, Interactable {

	public GameObject[] itemsToPower;

	public void Interact(Character character) {
		if (itemsToPower == null)
			return;

		foreach (GameObject item in itemsToPower) {
			Powerable p = item.GetComponent<Powerable>();
			if (p != null)
				p.Power();
		}
	}

	public void Uninteract(Character character) {}
}
