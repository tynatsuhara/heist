using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public GameObject[] enemyPrefabs;
	public Car getaway;
	public bool objectivesComplete;

	private List<PossibleObjective> objectives;

	void Start () {
		instance = this;

		// generate level


		objectives = Object.FindObjectsOfType<PossibleObjective>().Where(x => x.isObjective && !x.isCompleted).ToList();
		objectivesComplete = CheckObjectivesComplete();
	}
	
	void Update () {
		getaway.locked = !objectivesComplete;

		// WIN!
		if (!getaway.isEmpty) {
			Debug.Log("U WIN");
		}
	}

	public void MarkObjectiveComplete(PossibleObjective po) {
		po.isCompleted = true;
		objectivesComplete = CheckObjectivesComplete();
	}

	bool CheckObjectivesComplete() {
		return objectives.All(x => !x.isRequired || x.isCompleted);
	}
}
