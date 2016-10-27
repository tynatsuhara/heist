using UnityEngine;
using System.Collections;

public class CameraComputer : MonoBehaviour, Interactable {

	public Character cameraGuy;
	public SecurityCamera[] cams;

	public void AlarmWatcher() {
		if (cameraGuy == null)
			return;
		
		cameraGuy.Alert(Character.Reaction.AGGRO, GameManager.instance.player.transform.position);
	}

	public bool PlayerInSight() {
		foreach (SecurityCamera c in cams) {
			if (c.PlayerInSight()) {
				return true;
			}
		}
		return false;
	}

	public void Interact(Character character) {
		if (cameraGuy != null && cameraGuy.isAlive)
			return;

		cameraGuy = character;
	}
	public void Uninteract(Character character){
		cameraGuy = null;
	}
}
