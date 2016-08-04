using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameUI : MonoBehaviour {

	public static GameUI instance;
	public Material textWhite;
	public Material textRed;
	public Material textGreen;
	public Material textBlue;

	public TextObject invText;

	private List<Dictionary<string, int>> displayedInventories;

	void Awake () {
		instance = this;
		displayedInventories = new List<Dictionary<string, int>>();
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
}
