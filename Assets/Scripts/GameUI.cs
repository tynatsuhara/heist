﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

	public static GameUI instance;
	public Material textWhite;
	public Material textRed;
	public Material textGreen;
	public Material textBlue;

	public TextObject invText;
	public TextObject ammoText;
	public Transform cursor;

	private List<Dictionary<string, int>> displayedInventories;

	void Awake () {
		instance = this;
		displayedInventories = new List<Dictionary<string, int>>();
	}

	void Update() {
		cursor.transform.position = Input.mousePosition;
		Cursor.visible = false;
	}

	public void UpdateInventory(Dictionary<string, int> dict) {
		if (!displayedInventories.Contains(dict))
			displayedInventories.Add(dict);

		Dictionary<string, int> mergedInventories = new Dictionary<string, int>();
		foreach (Dictionary<string, int> d in displayedInventories) {
			foreach (string s in d.Keys) {
				if (!mergedInventories.ContainsKey(s))
					mergedInventories.Add(s, 0);
				mergedInventories[s] += d[s];
			}
		}

		string result = "";
		foreach (string s in mergedInventories.Keys) {
			result += s + (mergedInventories[s] > 1 ? " × " + mergedInventories[s] + "\n" : "\n");
		}

		invText.Say(result, permanent: true);
	}

	public void UpdateAmmo(int ammo, int clipSize) {
		ammoText.Say(ammo + " / " + clipSize, permanent: true);
	}

	public void HitMarker() {
		CancelInvoke("UnHitMarker");
		cursor.GetComponent<RawImage>().material = textRed;
		Invoke("UnHitMarker", .15f);
	}

	private void UnHitMarker() {
		cursor.GetComponent<RawImage>().material = textWhite;
	}
}
