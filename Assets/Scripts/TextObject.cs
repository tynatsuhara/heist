using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TextObject : MonoBehaviour {

	private Text text;
	private List<string> wordQueue;

	private float flashSpeed = .1f;
	private float togglingStartTime;
	private float togglesLeft;
	private bool shouldClear;
	private float timeToClear;
	private float lastSayTime;
	private float cycleInterval;
	private bool looping;
	private bool cycling;

	public bool pausable = true;
	public bool currentlyDisplaying = false;

	void Awake() {
		text = GetComponent<Text>();
		wordQueue = new List<string>();
		wordQueue.Add(text.text);
	}

	void Update() {
		if (GameManager.paused && pausable) {
			// push the start times further into the future
			togglingStartTime += Time.unscaledDeltaTime;
			timeToClear += Time.unscaledDeltaTime;
			lastSayTime += Time.unscaledDeltaTime;
			return;
		}
		text.text = wordQueue.Count > 0 ? wordQueue[0] : "";
		CheckLoopTime();	
		CheckToggleTime();
		CheckClearTime();
	}

	private void CheckLoopTime() {
		if (!cycling || Time.realtimeSinceStartup - lastSayTime < cycleInterval || wordQueue.Count == 0)
			return;

		string str = wordQueue[0];
		wordQueue.RemoveAt(0);
		if (looping) {
			wordQueue.Add(str);
			lastSayTime = Time.realtimeSinceStartup;
		}
	}

	private void CheckClearTime() {
		if (shouldClear && Time.realtimeSinceStartup >= timeToClear) {
			shouldClear = false;
			Clear();
			currentlyDisplaying = false;
			wordQueue.Clear();		
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
		looping = false;
		currentlyDisplaying = true;
		SetColor(color);
		wordQueue.Clear();
		wordQueue.Add(message);
		text.enabled = true;
		int flashTimes = 6;
		lastSayTime = Time.realtimeSinceStartup;
		cycling = false;
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
	public void Say(string[] messages, float interval = 1f, string color = "white", bool loop = false) {
		looping = loop;
		currentlyDisplaying = true;
		SetColor(color);
		wordQueue.Clear();
		wordQueue.AddRange(messages);
		text.enabled = true;
		shouldClear = false;
		cycling = true;
		cycleInterval = interval;
	}

	// Say a random message from the array
	public void SayRandom(string[] messages, bool showFlash = false, float duration = 2f, string color = "white") {
		int index = Random.Range(0, messages.Length);
		Say(messages[index], showFlash, duration, color);
	}

	// Remove any displayed text
	public void Clear() {
		currentlyDisplaying = false;
		wordQueue.Clear();
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
