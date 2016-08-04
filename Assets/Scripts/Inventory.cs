using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	public int capacity = int.MaxValue;
	public bool showOnUI = false;

	public enum Item : int {
		NONE,
		KEYCARD,
		THERMITE,
		PISTOL
	};

	public Dictionary<int, int> inv;
	private Dictionary<int, string> messages;
	private Dictionary<string, int> amounts;

	void Start () {
		inv = new Dictionary<int, int>();
		messages = new Dictionary<int, string>();
		amounts = new Dictionary<string, int>();
	}

	public bool Has(int item, int amount = 1) {
		return inv.ContainsKey(item) && inv[item] >= amount;
	}
	public bool Has(Item item, int amount = 1) {
		return Has((int)item, amount);
	}
	
	public void Add(int item, int amount = 1, string label = "mysterious item") {
		if (amount < 1)
			throw new UnityException("Cannot add a negative amount");
		label = label.ToUpper();
		if (!inv.ContainsKey(item))
			inv.Add(item, 0);
		inv[item] += amount;
		if (!messages.ContainsKey(item))
			messages.Add(item, label);
		if (!amounts.ContainsKey(label))
			amounts.Add(label, 0);
		amounts[label] += amount;
		GameUI.instance.UpdateInventory(amounts);
	}
	public void Add(Item item, int amount = 1, string label = "mysterious item") {
		Add((int)item, amount, label);
	}

	public void Remove(int item, int amount = 1) {
		if (amount < 1)
			throw new UnityException("Cannot remove a negative amount");
		inv[item] -= amount;
		if (inv[item] <= 0) {
			inv.Remove(item);
		}

		string label = messages[item];
		amounts[label] -= amount;
		if (amounts[label] <= 0)
			amounts.Remove(label);
		GameUI.instance.UpdateInventory(amounts);
	}
	public void Remove(Item item, int amount = 1) {
		Remove((int)item, amount);
	}

	public bool IsEmpty() {
		return inv.Keys.Count == 0;
	}
}
