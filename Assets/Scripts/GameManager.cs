﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public static bool paused = false;

	public GameObject[] enemyPrefabs;
	public Car getaway;
	public bool objectivesComplete;

	private List<PossibleObjective> objectives;
	public static List<Character> characters;
	public PlayerControls player;

	public bool alarmsRaised = false;
	public bool gameOver = false;

	void Awake() {
		instance = this;
	}

	void Start () {

		// 1. generate level
		GetComponent<LevelBuilder>().BuildLevel();

		// 2. spawn characters?
		characters = Object.FindObjectsOfType<Character>().Where(x => !(x is PlayerControls)).ToList();
		player = GameObject.FindWithTag("Player").GetComponent<PlayerControls>();

		// 3. get objectives
		objectives = Object.FindObjectsOfType<PossibleObjective>().Where(x => x.isObjective && !x.isCompleted).ToList();
		objectivesComplete = CheckObjectivesComplete();
		GameUI.instance.UpdateObjectives(objectives.ToArray());
	}
	
	void Update () {
		getaway.locked = !objectivesComplete;

		// WIN!
		if (!getaway.isEmpty) {
			GameOver(true);
			getaway.destination = GameObject.Find("EscapePoint").transform;
		}

		CheckPause();
		CheckSceneReload();
	}

	private void CheckPause() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SetPaused(!paused);
		}
	}

	private void SetPaused(bool paused) {
		GameManager.paused = paused;
		Time.timeScale = paused ? 0 : 1;
	}

	private void CheckSceneReload() {
		if (paused && Input.GetKeyDown(KeyCode.N)) {
			SetPaused(false);			
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	// Alerts all characters to the given range to an event with the given
	// severity and range. If visual is nonnull, the character must have line
	// of sight to the visual to be alerted.
	public void AlertInRange(Character.Reaction importance, Vector3 location, float range, GameObject visual = null) {
		foreach (Character c in characters) {
			if ((c.transform.position - player.transform.position).magnitude < range) {
				if (visual != null && !c.CanSee(player.gameObject))
					continue;
				c.Alert(importance, location);
			}
		}
	}

	// Return all characters in the given range from the given point, ordered by increasing distance
	public List<Character> CharactersWithinDistance(Vector3 from, float range) {
		List<Character> ret = new List<Character>();
		foreach (Character c in characters) {
			if ((c.transform.position - from).magnitude < range) {
				ret.Add(c);
			}
		}
		return ret.OrderBy(c => (c.transform.position - player.transform.position).magnitude).ToList();
	}

	// Call this to indicate it is no longer a stealth-possible mission,
	// ALARMS HAVE BEEN RAISED -- START SPAWNING ENEMIES
	public void WereGoingLoudBoys() {
		if (alarmsRaised)
			return;

		alarmsRaised = true;
		InvokeRepeating("SpawnCop", 1f, 5f);
	}

	public void GameOver(bool success) {
		if (gameOver)
			return;
		
		gameOver = true;
		CancelInvoke("SpawnCop");
		Debug.Log("game over! you " + (success ? "win!" : "lose!"));
	}

	public void SpawnCop() {
		GameObject enemy = (GameObject) Instantiate(enemyPrefabs[0], 
			new Vector3(6f, 1f, 6f) + 5f * Random.insideUnitSphere, Quaternion.identity);
		characters.Add(enemy.GetComponent<Character>());
	}

	public void MarkObjectiveComplete(PossibleObjective po) {
		po.isCompleted = true;
		objectivesComplete = CheckObjectivesComplete();
		GameUI.instance.UpdateObjectives(objectives.ToArray());
	}

	private bool CheckObjectivesComplete() {
		return objectives.All(x => !x.isRequired || x.isCompleted);
	}

	private Dictionary<string, List<int>> lootAmounts;
	public void AddLoot(string category, int dollarAmount) {
		if (category == null || category.Length == 0)
			return;
		
		category = category.ToUpper();
		if (lootAmounts == null) {
			lootAmounts = new Dictionary<string, List<int>>();
		}

		List<int> loots = lootAmounts.ContainsKey(category) ? lootAmounts[category] : new List<int>();
		loots.Add(dollarAmount);
		lootAmounts[category] = loots;

		Debug.Log(category + " + $" + dollarAmount);
	}
}
