using UnityEngine;
using System.Linq;

public class CharacterCustomizationMenu : MonoBehaviour {

	public static CharacterCustomizationMenu instance;

	public GameObject player;
	public int playerId;
	public Accessory[] accessories;

	void Start () {
		instance = this;
	}
	
	void Update () {
	
	}

	public static void CustomizeFromPrefs(PlayerControls p) {
		p.GetComponent<CharacterCustomization>().ColorCharacter(LoadOutfit(p.id), accessories:LoadAccessories(p.id));
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
						  .Select(x => instance.accessories[int.Parse(x)])
						  .ToArray();
	}
}
