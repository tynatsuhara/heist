using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cheats : MonoBehaviour {

	private List<KeyCode> prevKeys;

	// Aliases
	private static KeyCode up = KeyCode.UpArrow;
	private static KeyCode down = KeyCode.DownArrow;
	private static KeyCode left = KeyCode.LeftArrow;
	private static KeyCode right = KeyCode.RightArrow;

	private static KeyCode[] trackedKeys = {
		up, down, left, right
	};

	void Start() {
		prevKeys = new List<KeyCode>();
	}

	void Update() {
		foreach (KeyCode k in trackedKeys) {
			if (Input.GetKeyDown(k)) {
				prevKeys.Add(k);
				Debug.Log(k);
			}
		}
	}
}
