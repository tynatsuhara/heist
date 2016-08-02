using UnityEngine;
using System.Collections;

public class GameUI : MonoBehaviour {

	public static GameUI instance;
	public Material textWhite;
	public Material textRed;
	public Material textGreen;
	public Material textBlue;

	void Awake () {
		instance = this;
	}
}
