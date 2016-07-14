using UnityEngine;
using System.Collections;

public class BasicDrop : PossibleObjective {

	public Inventory.Item item;
	private int itemID;

	public int amount = 1;
	public bool onlyForPlayer = true;
	public bool mustBeEquipped = true;

	public bool floaty;
	public float floatHeight;
	public float rotationAngle;
	Vector3 initialPos;

	void Start() {
		itemID = (int)item;
		initialPos = transform.position;
	}

	void Update() {
		float height = (Mathf.Sin(Time.time) + 1) / 2;
		Vector3 newPos = initialPos + new Vector3(0, floatHeight * height, 0);
		transform.RotateAround(transform.position, Vector3.up, rotationAngle * Time.deltaTime);
		transform.position = newPos;
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
