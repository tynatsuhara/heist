using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextObject : MonoBehaviour {

	private Text text;

	void Awake() {
		text = GetComponent<Text>();
	}

	// Dispaly a message in the text box
	public void Say(string message, bool showFlash = false, float duration = 2f, string color = "white", bool permanent = false) {
		SetColor(color);
		CancelInvoke();
		text.text = message.ToUpper();
		text.enabled = true;
		float flashSpeed = .1f;
		int flashTimes = 6;  // make sure it's an even number
		if (showFlash) {
			for (int i = 0; i < flashTimes; i++) {
				Invoke("ToggleEnabled", flashSpeed * i);
			}
		}

		if (!permanent)
			Invoke("Clear", duration + (showFlash ? flashTimes * flashSpeed : 0));
	}

	// Display a series of messages in the text box
	public void Say(string[] messages, bool showFlash = false, float interval = 2f, string color = "white") {
		Debug.Log("TODO: implement Say with array param");
	}

	// Say a random message from the array
	public void SayRandom(string[] messages, bool showFlash = false, float duration = 2f, string color = "white") {
		int index = Random.Range(0, messages.Length);
		Say(messages[index], showFlash, duration, color);
	}

	// Remove any displayed text
	public void Clear() {
		text.text = "";
	}

	private void ToggleEnabled() {
		text.enabled = !text.enabled;
	}

	private void SetColor(string color) {
		if (color == "red") {
			text.material = GameUI.instance.textRed;
		} else if (color == "green") {
			text.material = GameUI.instance.textGreen;
		} else if (color == "blue") {
			text.material = GameUI.instance.textBlue;
		} else if (color == "orange") {
			text.material = GameUI.instance.textOrange;
		} else if (color == "yellow") {
			text.material = GameUI.instance.textYellow;
		} else {
			text.material = GameUI.instance.textWhite;
		}
	}
}
