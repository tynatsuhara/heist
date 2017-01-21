using UnityEngine;
using System.Linq;

public class CharacterCustomizationMenu : MonoBehaviour {

	public static CharacterCustomizationMenu instance;

	public PlayerControls player;
	public int playerId;
	public float rotationSpeed;
	public Accessory[] accessories;
	public GameObject[] weapons;
	public GameObject[] sidearms;

	void Start () {
		instance = this;
		CustomizeFromPrefs(player);
	}
	
	void Update () {
		if (Input.GetMouseButton(0)) {
			float dir = Input.GetAxis("Mouse X");
			player.transform.RotateAround(player.transform.position, Vector3.up, dir * -rotationSpeed);
		}
	}


	// Static functions for loading data from prefs

	public static void CustomizeFromPrefs(PlayerControls p) {
		CharacterCustomization cc = p.GetComponent<CharacterCustomization>();
		cc.shirtColor1 = LoadColor(p.id, "shirtColor1", cc.shirtColor1);
		cc.shirtColor2 = LoadColor(p.id, "shirtColor2", cc.shirtColor2);
		cc.shirtColor3 = LoadColor(p.id, "shirtColor3", cc.shirtColor3);
		cc.pantsColor1 = LoadColor(p.id, "pantsColor1", cc.pantsColor1);
		cc.pantsColor2 = LoadColor(p.id, "pantsColor2", cc.pantsColor2);
		cc.shoesColor = LoadColor(p.id, "shoesColor", cc.shoesColor);
		cc.skinColor = LoadColor(p.id, "skinColor", cc.skinColor);
		cc.hairColor = LoadColor(p.id, "hairColor", cc.hairColor);
		cc.eyeColor = LoadColor(p.id, "eyeColor", cc.eyeColor);
		cc.ColorCharacter(LoadOutfit(p.id), accessories:LoadAccessories(p.id));
	}

	private static string[] LoadOutfit(int id) {
		string[] defaultOutfit = {
			"3 0-13 70-73; 0 14-69; 1 58-59 44-45 30-31 16-17",
			"8 37 40; 7 26-33 44-51 60 62-69 71 78-89 96-119 91-94",
			"3 1; 5 0",
			"0 1-3"
		};
		string res = PlayerPrefs.GetString("p" + id + "_outfit");
		return res.Length > 0 ? res.Split('$') : defaultOutfit;
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
