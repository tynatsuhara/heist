using UnityEngine;
using System.Collections;

public class GunDrop : BasicDrop {

	void OnTriggerEnter(Collider other) {
		if (item == Inventory.Item.NONE)
			return;

		Character c = other.GetComponentInParent<Character>();
		if (c == null)
			return;

		if (c == null 
			|| (onlyForPlayer && c.tag != "Player")
			|| (mustBeEquipped && !c.IsEquipped())
			|| c.weaponInv)
			return;

		c.inventory.Add(item, amount);

		if (isObjective && !isCompleted)
			MarkCompleted();

		gameObject.SetActive(false);
	}
}
