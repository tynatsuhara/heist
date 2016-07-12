using UnityEngine;
using System.Collections;

public class BasicDrop : PossibleObjective {

	public Inventory.Item item;
	private int itemID;

	public int amount = 1;
	public bool onlyForPlayer = true;
	public bool mustBeEquipped = true;

	void Start() {
		itemID = (int)item;
	}

	void OnTriggerEnter(Collider other) {
		if (itemID == (int)Inventory.Item.NONE)
			return;
		
		Character c = other.transform.root.GetComponent<Character>();
		if (c == null)
			return;

		if (c == null || 
			(onlyForPlayer && c.tag != "Player") ||
			(mustBeEquipped && !c.IsEquipped()))
			return;

		c.inventory.Add(item, amount);

		if (isObjective && !isCompleted)
			MarkCompleted();

		gameObject.SetActive(false);
	}

	void SetItem(int itemID) {
		this.itemID = itemID;
	}
}
