using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextObject : MonoBehaviour {

	private Text text;

	private float flashSpeed = .1f;
	private float togglingStartTime;
	private float togglesLeft;
	private bool shouldClear;
	private float timeToClear;

	public bool pausable = true;
	public bool currentlyDisplaying = false;

	void Awake() {
		text = GetComponent<Text>();
	}

	void Update() {
		if (GameManager.paused && pausable) {
			togglingStartTime += Time.unscaledDeltaTime;
			timeToClear += Time.unscaledDeltaTime;
			return;
		}
			
		CheckToggleTime();
		CheckClearTime();
	}

	private void CheckClearTime() {
		if (shouldClear && Time.realtimeSinceStartup >= timeToClear) {
			shouldClear = false;
			Clear();
			currentlyDisplaying = false;
		}
	}

	private void CheckToggleTime() {
		if (togglesLeft <= 0)
			return;

		float elapsed = Time.realtimeSinceStartup - togglingStartTime;
		if (elapsed > flashSpeed) {
			togglesLeft--;
			togglingStartTime += flashSpeed;
			text.enabled = !text.enabled;
		}
	}

	// Dispaly a message in the text box
	public void Say(string message, bool showFlash = false, float duration = 2f, string color = "white", bool permanent = false) {
		currentlyDisplaying = true;
		SetColor(color);
		CancelInvoke();
		text.text = message.ToUpper();
		text.enabled = true;
		int flashTimes = 6;
		if (showFlash) {
			togglingStartTime = Time.realtimeSinceStartup;
			togglesLeft = flashTimes;  // make it an even number
		}

		if (!permanent) {
			shouldClear = true;
			timeToClear = Time.realtimeSinceStartup + duration +  (showFlash ? flashTimes * flashSpeed : 0);
		}
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
		currentlyDisplaying = false;
		text.text = "";
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
