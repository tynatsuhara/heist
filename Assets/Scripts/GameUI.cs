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

	public TextObject objectivesText;
	public TextObject topCenterText;
	public GameObject pauseMenu;

	void Awake () {
		instance = this;
	}

	void Update() {
		pauseMenu.SetActive(GameManager.paused);
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
