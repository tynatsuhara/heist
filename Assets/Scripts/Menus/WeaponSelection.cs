using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
	The menu for weapon selection and character customization.
*/

public class WeaponSelection : Menu {

	public MenuNode sidearm;
	public MenuNode weapon;
	private CharacterCustomizationMenu ccm;
	private List<string> outfitNames;

	void Start() {
		ccm = CharacterCustomizationMenu.instance;
		sidearm.SetText(ccm.sidearms[PlayerPrefs.GetInt("p" + playerId + "_sidearm", 0)].name.ToUpper());		
		weapon.SetText(ccm.weapons[PlayerPrefs.GetInt("p" + playerId + "_weapon", 0)].name.ToUpper());	
		outfitNames = Outfits.fits.Keys.OrderBy(x => x.ToString()).ToList();
	}

	public override void Carousel(MenuNode node, int dir) {
		if (node == sidearm) {
			node.SetText(LoadWeapon(ccm.sidearms, ccm.CurrentSidearmId(playerId), dir, "_sidearm"));
		} else if (node == weapon) {
			node.SetText(LoadWeapon(ccm.weapons, ccm.CurrentWeaponId(playerId), dir, "_weapon"));
		} else if (node.name == "Outfit") {
			int outfitIndex = (outfitNames.Count + outfitNames.IndexOf(ccm.LoadOutfitName(playerId)) + dir) % outfitNames.Count;
			ccm.SetOutfit(playerId, outfitNames[outfitIndex]);
		}
	}

	private string LoadWeapon(GameObject[] arr, int currentIndex, int dir, string prefString) {
		int index = (arr.Length + currentIndex + dir) % arr.Length;
		GameObject gun = arr[index];
		PlayerPrefs.SetInt("p" + playerId + prefString, index);
		return gun.name.ToUpper();
	}

	public override void Enter(MenuNode node) {
		if (node.name == "Ready") {
			bool nowReady = ccm.ToggleReady(playerId);
			if (nowReady) {
				node.SetText("READY");
			} else {
				node.SetText("READY?");
			}
		}
	}
}
