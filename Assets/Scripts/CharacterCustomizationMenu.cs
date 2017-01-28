using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class CharacterCustomizationMenu : MonoBehaviour {

	public static CharacterCustomizationMenu instance;

	public string nextScene;
	public PlayerControls[] players;
	public Camera[] cams;
	private List<int> playingPlayers;
	private bool[] readyPlayers;
	public float rotationSpeed;
	public Accessory[] accessories;
	public GameObject[] weapons;
	public GameObject[] sidearms;

	void Awake() {
		instance = this;
		playingPlayers = new List<int>(new int[] { 1 });
		readyPlayers = new bool[4];
	}

	void Start() {
		float dist = 500f;
		cams = new Camera[] { Camera.main, null, null, null };
		for (int i = 1; i < players.Length; i++) {
			players[i] = (Instantiate(players[0].gameObject, 
								      players[0].transform.position + Vector3.right * dist * i, 
									  players[0].transform.rotation) as GameObject).GetComponent<PlayerControls>();
			players[i].id = i + 1;
			cams[i] = (Instantiate(cams[0].gameObject, cams[0].transform.position + Vector3.right * dist * i, 
								   cams[0].transform.rotation) as GameObject).GetComponent<Camera>();
			cams[i].gameObject.SetActive(false);
			cams[i].GetComponentInChildren<WeaponSelection>().playerId = players[i].id;
		}
		foreach (PlayerControls pc in players) {
			ColorizeFromPrefs(pc);
		}
	}
	
	void Update() {
		foreach (int playerId in playingPlayers) {
			if (playerId == 1 && Input.GetMouseButton(0)) {
				float dir = Input.GetAxis("Mouse X");
				players[0].transform.RotateAround(players[0].transform.position, Vector3.up, dir * -rotationSpeed);
			} else {
				float dir = Input.GetAxis("Horizontal" + playerId) / 2f;
				players[playerId - 1].transform.RotateAround(players[playerId - 1].transform.position, Vector3.up, dir * -rotationSpeed);
			}
		}
		for (int id = 2; id <= 4; id++) {
			if (!playingPlayers.Contains(id) && Input.GetKeyDown("joystick " + id + " button 1")) {
				LobbyJoin(id);
			} else if (playingPlayers.Contains(id) && Input.GetKeyDown("joystick " + id + " button 2")) {
				LobbyLeave(id);
			}
		}
		if (!readyPlayers[0] && Input.GetKeyDown("joystick 1 button 2")) {
			// TODO: close lobby
		}
	}

	private void LobbyJoin(int id) {
		playingPlayers.Add(id);
		UpdateCameras();
	}

	private void LobbyLeave(int id) {
		playingPlayers.Remove(id);
		UpdateCameras();
	}

	// Returns: new ready state
	public bool ToggleReady(int id) {
		readyPlayers[id - 1] = !readyPlayers[id - 1];
		bool allReady = true;
		foreach (int pid in playingPlayers)
			if (!readyPlayers[pid-1])
				allReady = false;
		if (allReady)
			StartGame();
		return readyPlayers[id - 1];
	}

	private void StartGame() {
		GameManager.playersToSpawn = playingPlayers.ToArray();
		SceneManager.LoadScene(nextScene);
	}

	private void UpdateCameras() {
		foreach (Camera c in cams)
			c.gameObject.SetActive(false);
		if (playingPlayers.Count == 1) {
			cams[playingPlayers[0]-1].rect = new Rect(0, 0, 1, 1);
		} else if (playingPlayers.Count == 2) {
			cams[playingPlayers[0]-1].rect = new Rect(0, .5f, 1, .5f);
			cams[playingPlayers[1]-1].rect = new Rect(0, 0, 1, .5f);
		} else if (playingPlayers.Count == 3) {
			cams[playingPlayers[0]-1].rect = new Rect(0, .5f, 1, .5f);
			cams[playingPlayers[1]-1].rect = new Rect(0, 0, .5f, .5f);
			cams[playingPlayers[2]-1].rect = new Rect(.5f, 0, .5f, .5f);
		} else if (playingPlayers.Count == 4) {
			cams[playingPlayers[0]-1].rect = new Rect(0, .5f, .5f, .5f);
			cams[playingPlayers[1]-1].rect = new Rect(.5f, .5f, .5f, .5f);
			cams[playingPlayers[2]-1].rect = new Rect(0, 0, .5f, .5f);
			cams[playingPlayers[3]-1].rect = new Rect(.5f, 0f, .5f, .5f);
		}
		foreach (int id in playingPlayers)
			cams[id - 1].gameObject.SetActive(true);
	}

	// Instance functions for loading saved config

	public void LoadWeaponsFromPrefs(PlayerControls p) {
		p.guns = new GameObject[] {
			sidearms[CurrentSidearmId(p.id)],
			weapons[CurrentWeaponId(p.id)]
		};
	}
	public int CurrentWeaponId(int playerId) {
		return PlayerPrefs.GetInt("p" + playerId + "_weapon", 0);
	}
	public int CurrentSidearmId(int playerId) {
		return PlayerPrefs.GetInt("p" + playerId + "_sidearm", 0);
	}

	public void ColorizeFromPrefs(PlayerControls p) {
		CharacterCustomization cc = p.GetComponent<CharacterCustomization>();
		cc.skinColor = LoadColor(p.id, "skinColor", cc.skinColor);
		cc.hairColor = LoadColor(p.id, "hairColor", cc.hairColor);
		cc.eyeColor = LoadColor(p.id, "eyeColor", cc.eyeColor);
		cc.ColorCharacter(Outfits.fits[LoadOutfitName(p.id)], accessories:LoadAccessories(p.id));
	}
	
	public string LoadOutfitName(int id) {
		return PlayerPrefs.GetString("p" + id + "_outfit", "default");
	}

	public void SetOutfit(int id, string name) {
		PlayerPrefs.SetString("p" + id + "_outfit", name);
		ColorizeFromPrefs(players[id - 1]);
	}

	private static Accessory[] LoadAccessories(int id) {
		return PlayerPrefs.GetString("p" + id + "_accessories")
						  .Split('$')
						  .Where(x => x.Length > 0)
						  .Select(x => instance.accessories[int.Parse(x)])
						  .ToArray();
	}

	private static Color32 LoadColor(int id, string name, Color32 defaultColor) {
		if (!PlayerPrefs.HasKey("p" + id + "_" + name + "_r"))
			return defaultColor;
		int r = PlayerPrefs.GetInt("p" + id + "_" + name + "_r", -1);			
		int g = PlayerPrefs.GetInt("p" + id + "_" + name + "_g");
		int b = PlayerPrefs.GetInt("p" + id + "_" + name + "_b");
		int a = PlayerPrefs.GetInt("p" + id + "_" + name + "_a");
		return new Color32((byte) r, (byte) g, (byte) b, (byte) a);
	}
}
