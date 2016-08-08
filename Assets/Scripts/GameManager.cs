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
	private List<Character> characters;
	private List<Character> deadCharacters;
	public PlayerControls player;

	public bool alarmsRaised = false;

	void Awake() {
		instance = this;
	}

	void Start () {

		// 1. generate level
		GetComponent<LevelBuilder>().Build();

		// 2. spawn characters?
		characters = Object.FindObjectsOfType<Character>().Where(x => !(x is PlayerControls)).ToList();
		deadCharacters = new List<Character>();
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
		}
	}

	// Alerts all characters to the given range to an event with the given
	// severity and range. If visual is nonnull, the character must have line
	// of sight to the visual to be alerted.
	//    severity: [1 - 5], with 1 being very mild and 10 being full aggro
	public void AlertInRange(Character.Reaction importance, Vector3 location, float range, GameObject visual = null) {
		foreach (Character c in characters) {
			if ((c.transform.position - player.transform.position).magnitude < range) {
				c.Alert(importance, location);
			}
		}
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
		CancelInvoke("SpawnCop");
		Debug.Log("game over! you " + (success ? "win!" : "lose!"));
		this.enabled = false;
	}

	public void SpawnCop() {
		GameObject enemy = (GameObject) Instantiate(enemyPrefabs[0], 
			new Vector3(-5f, 1, -8) + 5f * Random.insideUnitSphere, Quaternion.identity);
	}

	public void MarkObjectiveComplete(PossibleObjective po) {
		po.isCompleted = true;
		objectivesComplete = CheckObjectivesComplete();
		GameUI.instance.UpdateObjectives(objectives.ToArray());
	}

	bool CheckObjectivesComplete() {
		return objectives.All(x => !x.isRequired || x.isCompleted);
	}

	public void MarkDead(Character character) {
		characters.Remove(character);
		deadCharacters.Add(character);
	}

	public Character[] DeadCharacters() {
		return deadCharacters.ToArray();
	}
}
