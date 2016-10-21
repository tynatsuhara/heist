using UnityEngine;
using System;

public class PossibleObjective : MonoBehaviour {

	public bool isObjective;
	public bool isRequired;
	public bool isCompleted;
	public bool isLocked;
	public string message = "mysterious objective";

	public PossibleObjective[] nextObjectives;

	public void MarkCompleted() {
		isCompleted = true;
		foreach (PossibleObjective po in nextObjectives) {
			po.Unlock();
		}
		GameManager.instance.MarkObjectiveComplete(this);
	}

	public void Unlock() {
		isLocked = false;
	}
}
