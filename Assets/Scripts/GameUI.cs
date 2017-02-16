using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public GameObject winScreen;

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

	public void ShowWinScreen(Dictionary<string, List<int>> loot) {
		winScreen.SetActive(true);
		string left = "";
		string right = "";
		int bigSum = 0;
		foreach (string s in loot.Keys) {
			if (loot[s].Count > 1)
				left += loot[s].Count + " x ";
			left += s.ToUpper();
			int sum = 0;
			foreach (int x in loot[s])
				sum += x;
			left += "\n";
			right += "$" + sum.ToString("#,##0") + "\n";
			bigSum += sum;
		}
		left += "TOTAL";
		right += "$" + bigSum.ToString("#,##0");
		Text textL = winScreen.GetComponentsInChildren<Text>().Where(x => x.name == "Loot left").First();
		Text textR = winScreen.GetComponentsInChildren<Text>().Where(x => x.name == "Loot right").First();
		textL.text = left;
		textR.text = right;
		textL.GetComponent<RectTransform>().position 
				= textR.GetComponent<RectTransform>().position 
				= new Vector3(Screen.width, Screen.height, 0) * .5f + new Vector3(0, 14 + 20 * loot.Keys.Count, 0);

		PlayerPrefs.SetFloat("totalEarned", PlayerPrefs.GetFloat("totalEarned", 0) + bigSum);				
	}

	public void ShowLoseScreen() {

	}
}
