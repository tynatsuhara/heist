using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Cheats : MonoBehaviour {

	// Aliases
	private static KeyCode up = KeyCode.UpArrow;
	private static KeyCode down = KeyCode.DownArrow;
	private static KeyCode left = KeyCode.LeftArrow;
	private static KeyCode right = KeyCode.RightArrow;

	private static KeyCode[] trackedKeys = { 
		up, down, left, right,
		KeyCode.A, KeyCode.B,
		KeyCode.C, KeyCode.D,
		KeyCode.E, KeyCode.F,
		KeyCode.G, KeyCode.H,
		KeyCode.I, KeyCode.J,
		KeyCode.K, KeyCode.L,
		KeyCode.M, KeyCode.N,
		KeyCode.O, KeyCode.P,
		KeyCode.Q, KeyCode.R,
		KeyCode.S, KeyCode.T,
		KeyCode.U, KeyCode.V,
		KeyCode.W, KeyCode.X,
		KeyCode.Y, KeyCode.Z
	};
	public static Cheats instance;

	private List<KeyCode> prevKeys;
	private Dictionary<string, KeyCode[]> cheats;
	private Dictionary<string, bool> enabledCheats;

	void Awake() {
		GenerateCheats();

		prevKeys = new List<KeyCode>();
		instance = this;
	}

	private void GenerateCheats() {
		cheats = new Dictionary<string, KeyCode[]>();
		enabledCheats = new Dictionary<string, bool>();

		// multiplies speed of the player
		cheats.Add("konami", new KeyCode[]{ up, up, down, down, left, right, left, right, KeyCode.A, KeyCode.B });
		cheats.Add("pride", new KeyCode[]{ KeyCode.P, KeyCode.R, KeyCode.I, KeyCode.D, KeyCode.E });

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

		// no need to store hundreds of keycodes
		if (prevKeys.Count > 100) {
			prevKeys = prevKeys.Skip(80).ToList();
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
				GameUI.instance.topCenterText.Say("Cheat Enabled: " + key.ToUpper(), showFlash:true);
				Debug.Log(key + " cheat enabled");
			}
		}
	}

	public bool IsCheatEnabled(string key) {
		return enabledCheats.ContainsKey(key) && enabledCheats[key];
	}
}
