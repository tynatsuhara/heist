using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	public enum Item : int {
		NONE,
		KEYCARD,
		C4
	};

	public Dictionary<int, int> inv;

	void Start () {
		inv = new Dictionary<int, int>();
	}

	public bool Has(int item) {
		return inv.ContainsKey(item) && inv[item] > 0;
	}
	public bool Has(Item item) {
		return Has((int)item);
	}
	
	public void Add(int item, int amount = 1) {
		if (!inv.ContainsKey(item))
			inv.Add(item, 0);
		inv[item] += amount;
	}
	public void Add(Item item, int amount = 1) {
		Add((int)item, amount);
	}

	public void Remove(int item, int amount = 1) {
		inv[item] -= amount;
	}
	public void Remove(Item item, int amount = 1) {
		Remove((int)item, amount);
	}
}
