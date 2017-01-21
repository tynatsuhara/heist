using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public RectTransform tint;
	public Transform cursor;

	void Update () {
		if (tint != null)
			tint.sizeDelta = new Vector2(Screen.width, Screen.height);
	}
}
