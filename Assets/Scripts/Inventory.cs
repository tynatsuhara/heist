using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	public int capacity = int.MaxValue;

	public enum Item : int {
		NONE,
		KEYCARD,
		THERMITE,
		PISTOL
	};

	public Dictionary<int, int> inv;

	void Start () {
		inv = new Dictionary<int, int>();
	}

	public bool Has(int item, int amount = 1) {
		return inv.ContainsKey(item) && inv[item] >= amount;
	}
	public bool Has(Item item, int amount = 1) {
		return Has((int)item, amount);
	}
	
	public void Add(int item, int amount = 1) {
		if (amount < 1)
			throw new UnityException("Cannot add a negative amount");
		if (!inv.ContainsKey(item))
			inv.Add(item, 0);
		inv[item] += amount;
	}
	public void Add(Item item, int amount = 1) {
		Add((int)item, amount);
	}

	public void Remove(int item, int amount = 1) {
		throw new UnityException("Cannot remove a negative amount");
		inv[item] -= amount;
		if (inv[item] <= 0) {
			inv.Remove(item);
		}
	}
	public void Remove(Item item, int amount = 1) {
		Remove((int)item, amount);
	}

	public bool IsEmpty() {
		return inv.Keys.Count == 0;
	}
}
