﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public GameObject[] enemyPrefabs;
	public Car getaway;
	public bool objectivesComplete;

	private List<PossibleObjective> objectives;
	private List<Character> characters;
	private PlayerControls player;

	public bool alarmsRaised = false;

	void Awake() {
		instance = this;
	}

	void Start () {

		// 1. generate level

		// 2. spawn characters?
		characters = Object.FindObjectsOfType<Character>().Where(x => !(x is PlayerControls)).ToList();
		player = GameObject.FindWithTag("Player").GetComponent<PlayerControls>();

		// 3. get objectives
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

	public void AlertInRange(Vector3 location, float range) {
		foreach (Character c in characters) {
			if ((c.transform.position - player.transform.position).magnitude < range) {
				c.Alert();
			}
		}
	}

	// Call this to indicate it is no longer a stealth-possible mission,
	// ALARMS HAVE BEEN RAISED -- START SPAWNING ENEMIES
	public void WereGoingLoudBoys() {
		if (alarmsRaised)
			return;

		alarmsRaised = true;
		InvokeRepeating("SpawnCop", 1f, 10f);
		InvokeRepeating("SpawnCop", 1f, 10f);
		InvokeRepeating("SpawnCop", 1f, 10f);
		InvokeRepeating("SpawnCop", 1f, 10f);
	}

	public void GameOver(bool success) {
		CancelInvoke("SpawnCop");
		Debug.Log("game over");
	}

	public void SpawnCop() {
		GameObject enemy = (GameObject) Instantiate(enemyPrefabs[0], 
			new Vector3(-5f, 1, -8) + 5f * Random.insideUnitSphere, Quaternion.identity);
	}

	public void MarkObjectiveComplete(PossibleObjective po) {
		po.isCompleted = true;
		objectivesComplete = CheckObjectivesComplete();
	}

	bool CheckObjectivesComplete() {
		return objectives.All(x => !x.isRequired || x.isCompleted);
	}
}
