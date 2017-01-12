using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public static bool paused = false;
	public int playerAmount;

	public GameObject[] enemyPrefabs;
	public GameObject playerPrefab;
	public GameObject playerCamPrefab;
	public GameObject playerUIPrefab;
	
	public Car getaway;
	public bool objectivesComplete;

	private List<PossibleObjective> objectives;
	public static List<Character> characters;
	public static List<PlayerControls> players;

	public bool alarmsRaised = false;
	public bool gameOver = false;
	public bool friendlyFireEnabled;

	void Awake() {
		instance = this;
	}

	void Start () {

		// 1. generate level
		GetComponent<LevelBuilder>().BuildLevel();

		// 2. spawn characters?
		characters = Object.FindObjectsOfType<Character>().Where(x => !(x is PlayerControls)).ToList();
		players = SpawnPlayers(playerAmount);

		// 3. get objectives
		objectives = Object.FindObjectsOfType<PossibleObjective>().Where(x => x.isObjective && !x.isCompleted).ToList();
		objectivesComplete = CheckObjectivesComplete();
		GameUI.instance.UpdateObjectives(objectives.ToArray());
	}
	
	void Update () {
		getaway.locked = !objectivesComplete;

		// WIN!
		if (players.All(x => !x.isAlive)) {
			GameOver(false);
		} else if (getaway.ContainsAllLivingPlayers()) {
			GameOver(true);
		}

		CheckPause();
		CheckSceneReload();
	}

	public void GameOver(bool success) {
		if (gameOver)
			return;
		
		gameOver = true;
		CancelInvoke("SpawnCop");
		Debug.Log("game over! you " + (success ? "win!" : "lose!"));
		GameUI.instance.objectivesText.gameObject.SetActive(false);
		foreach (PlayerControls pc in players)
			pc.playerUI.gameObject.SetActive(false);

		if (success) {
			getaway.destination = GameObject.Find("EscapePoint").transform;

		} else {

		}
	}

	private void CheckPause() {
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 9")) {
			SetPaused(!paused);
		}
	}

	private float currentTimeScale;
	private void SetPaused(bool paused) {
		if (GameManager.paused == paused)
			return;

		if (paused && !GameManager.paused)
			currentTimeScale = Time.timeScale;
		GameManager.paused = paused;
		Time.timeScale = paused ? 0 : currentTimeScale;
	}

	private void CheckSceneReload() {
		if ((gameOver || paused) && (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown("joystick button 8"))) {
			SetPaused(false);
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	// Alerts all characters to the given range to an event with the given
	// severity and range. If visual is nonnull, the character must have line
	// of sight to the visual to be alerted.
	public void AlertInRange(Character.Reaction importance, Vector3 location, float range, GameObject visual = null) {
		foreach (Character c in characters) {
			if ((c.transform.position - location).magnitude < range) {
				if (visual != null && !c.CanSee(visual))
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
		return ret.OrderBy(c => (c.transform.position - from).magnitude).ToList();
	}

	// Call this to indicate it is no longer a stealth-possible mission,
	// ALARMS HAVE BEEN RAISED -- START SPAWNING ENEMIES
	public void WereGoingLoudBoys() {
		if (alarmsRaised)
			return;

		alarmsRaised = true;
		foreach (PlayerControls pc in players)
			pc.DrawWeapon();
		InvokeRepeating("SpawnCop", 1f, 5f);
	}

	private List<PlayerControls> SpawnPlayers(int amount) {
		List<PlayerControls> result = new List<PlayerControls>();
		List<Camera> cams = new List<Camera>();
		for (int i = 0; i < amount; i++) {
			GameObject p = Instantiate(playerPrefab, new Vector3(i, 1f, 1f), Quaternion.identity) as GameObject;
			PlayerControls pc = p.GetComponent<PlayerControls>();
			pc.id = i + 1;
			result.Add(pc);

			pc.playerCamera = (Instantiate(playerCamPrefab) as GameObject).GetComponent<PlayerCamera>();
			pc.playerCamera.player = pc;
			cams.Add(pc.playerCamera.cam);

			pc.playerUI = (Instantiate(playerUIPrefab) as GameObject).GetComponent<PlayerUI>();
			pc.playerUI.GetComponent<Canvas>().worldCamera = pc.playerCamera.cam;
			pc.playerUI.player = pc;
			pc.playerUI.transform.SetParent(pc.playerCamera.transform, false);
		}

		// split screen dimensions
		if (cams.Count == 2) {
			cams[0].rect = new Rect(0, .5f, 1, .5f);
			cams[1].rect = new Rect(0, 0, 1, .5f);
		} else if (cams.Count == 3) {
			cams[0].rect = new Rect(0, .5f, 1, .5f);
			cams[1].rect = new Rect(0, 0, .5f, .5f);
			cams[2].rect = new Rect(.5f, 0, .5f, .5f);
		} else if (cams.Count == 4) {
			cams[0].rect = new Rect(0, .5f, .5f, .5f);
			cams[1].rect = new Rect(.5f, .5f, .5f, .5f);
			cams[2].rect = new Rect(0, 0, .5f, .5f);
			cams[3].rect = new Rect(.5f, 0f, .5f, .5f);
		}
		return result;
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
