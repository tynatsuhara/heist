using UnityEngine;

public class Computer : PossibleObjective, Interactable {

	public GameObject[] itemsToPower;
	private bool invoked;
	private Character hacker;
	public TextObject speech;
	public float hackTime;
	private float hackProgressTime;
	private bool hacked;
	public SecurityCamera[] hookedUpCameras;

	public void Update() {
		if (invoked && !hacker.CanSee(gameObject, 140f, 2)) {
			Uninteract(null);
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
		if (!hacked) {
			speech.Clear();
		}
		hacker = null;
	}

	public bool PlayerInSight() {
		foreach (SecurityCamera c in hookedUpCameras) {
			if (c.PlayerInSight()) {
				return true;
			}
		}
		return false;
	}
}
