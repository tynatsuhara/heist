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
		GameManager.instance.MarkObjectiveComplete(this);
	}
}

