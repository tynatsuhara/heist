using UnityEngine;
using System;

public abstract class PossibleObjective : MonoBehaviour {

	public bool isObjective;
	public bool isRequired;
	public bool isCompleted;
	public bool isLocked;

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
