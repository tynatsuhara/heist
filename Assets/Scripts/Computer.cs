using UnityEngine;

public class Computer : PossibleObjective, Interactable {

	public GameObject[] itemsToPower;
	private bool invoked;
	private Character hacker;
	public TextObject speech;
	public float hackTime;
	private float hackProgressTime;
	private bool hacked;

	public void Update() {
		if (invoked && !hacker.CanSee(gameObject, 140f, 2)) {
			CancelInvoke("Hack");
			invoked = false;
		} else if (invoked && !GameManager.paused) {
			hackProgressTime += Time.unscaledDeltaTime;
		}

		if (hacker != null && !hacked) {
			speech.Say("HACKING (" + (int)(100*hackProgressTime/hackTime) + "%)");			
		}
	}

	public void Interact(Character character) {
		if (itemsToPower == null)
			return;
		// speech.Say(new string[]{ "HACKING   ", "HACKING.  ", "HACKING.. ", "HACKING..." }, interval: .2f, loop: true);
		if (!invoked) {
			Invoke("Hack", hackTime);
			invoked = true;
			hacker = character;
		}
	}

	private void Hack() {
		foreach (GameObject item in itemsToPower) {
			if (item != null) {
				Powerable p = item.GetComponent<Powerable>();
				if (p != null) {
					p.Power();
				}
			}
		}
		MarkCompleted();
		hacked = true;
		speech.Say("HACKED", showFlash:true, color:"green");
	}

	public void Uninteract(Character character) {
		if (invoked) {
			CancelInvoke("Hack");
			invoked = false;
		}
		hackProgressTime = 0;
		speech.Clear();
		hacker = null;
	}
}
