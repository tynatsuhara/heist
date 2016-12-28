using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public RectTransform tint;

	void Update () {
		tint.sizeDelta = new Vector2(Screen.width, Screen.height);
	}
}
