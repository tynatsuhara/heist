using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cheats : MonoBehaviour {

	// Aliases
	private static KeyCode up = KeyCode.UpArrow;
	private static KeyCode down = KeyCode.DownArrow;
	private static KeyCode left = KeyCode.LeftArrow;
	private static KeyCode right = KeyCode.RightArrow;
	private static KeyCode b = KeyCode.B;
	private static KeyCode a = KeyCode.A;

	private static KeyCode[] trackedKeys = { up, down, left, right, b, a };
	public static Cheats instance;

	private List<KeyCode> prevKeys;
	private Dictionary<string, KeyCode[]> cheats;
	private Dictionary<string, bool> enabledCheats;

	void Start() {
		GenerateCheats();

		prevKeys = new List<KeyCode>();
		instance = this;
	}

	private void GenerateCheats() {
		cheats = new Dictionary<string, KeyCode[]>();
		enabledCheats = new Dictionary<string, bool>();

		// doubles speed of the player
		cheats.Add("konami", new KeyCode[]{ up, up, down, down, left, right, left, right, b, a });

		foreach (string key in cheats.Keys) {
			enabledCheats[key] = false;
		}
	}

	void Update() {
		if (!GameManager.paused)
			return;

		foreach (KeyCode k in trackedKeys) {
			if (Input.GetKeyDown(k)) {
				prevKeys.Add(k);
				CheckAll();
			}
		}
	}

	private void CheckAll() {
		foreach (string key in cheats.Keys) {
			KeyCode[] cheatArr = cheats[key];
			if (cheatArr.Length > prevKeys.Count) {
				continue;
			}

			bool match = false;
			for (int i = 1; i <= cheatArr.Length; i++) {
				KeyCode cheatKey = cheatArr[cheatArr.Length - i];
				KeyCode pressedKey = prevKeys[prevKeys.Count - i];
				if (cheatKey != pressedKey) {
					break;
				} else if (i == cheatArr.Length) {
					match = true;
				}
			}

			if (match && !enabledCheats[key]) {
				enabledCheats[key] = true;
				GameUI.instance.topCenterText.Say("Cheat Enabled: " + key.ToUpper());
				Debug.Log(key + " cheat enabled");
			}
		}
	}

	public bool IsCheatEnabled(string key) {
		return enabledCheats.ContainsKey(key) && enabledCheats[key];
	}
}
