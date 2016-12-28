using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

	public static GameUI instance;
	public Material textWhite;
	public Material textRed;
	public Material textGreen;
	public Material textBlue;
	public Material textYellow;
	public Material textOrange;

	public TextObject invText;
	public TextObject ammoText;
	public TextObject objectivesText;
	public TextObject healthText;
	public TextObject topCenterText;
	public GameObject pauseMenu;
	public Transform cursor;

	private Vector3 lastMousePos;
	public Vector3 mousePos;

	private List<Dictionary<string, int>> displayedInventories;

	void Awake () {
		lastMousePos = mousePos = Input.mousePosition;

		instance = this;
		displayedInventories = new List<Dictionary<string, int>>();
	}

	void Update() {
		pauseMenu.SetActive(GameManager.paused);

		mousePos += Input.mousePosition - lastMousePos;
		lastMousePos = Input.mousePosition;
		cursor.transform.position = mousePos;
		Cursor.visible = false;
	}

	public void JoystickCursorMove(Transform player, float dx, float dy) {
		if (dx == 0 && dy == 0)
			return;
		// TODO: make this distance configurable in settings
		float mouseDist = 180f;
		// Vector3 playerPos = PlayerCamera.instance.cam.WorldToScreenPoint(player.position);
		// mousePos = Vector3.Lerp(mousePos, playerPos + new Vector3(dx, dy, 0).normalized * mouseDist, .1f);
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
		ammoText.Say(ammo + "/" + clipSize, permanent: true);
	}

	public void UpdateHealth(float health, float healthMax, float armor, float armorMax) {
		if (armor <= 0) {
			healthText.Say(health + " HP", permanent: true);
		} else {
			healthText.Say(health + " HP + " + armor + " ARMOR", permanent: true);
		}
	}

	public void HitMarker() {
		CancelInvoke("UnHitMarker");
		cursor.GetComponent<RawImage>().material = textRed;
		Invoke("UnHitMarker", .15f);
	}

	private void UnHitMarker() {
		cursor.GetComponent<RawImage>().material = textWhite;
	}

	public void UpdateObjectives(PossibleObjective[] array) {
		string res = "";
		bool hasObjectivesLeft = false;
		foreach (PossibleObjective po in array) {
			if (po.isObjective && !po.isCompleted && !po.isLocked) {
				if (!hasObjectivesLeft) {
					hasObjectivesLeft = true;
					res += "objectives:\n";
				}
				res += po.message + (!po.isRequired ? "*" : "") + "\n";
			}
		}
		objectivesText.Say(res, permanent: true);
	}
}
