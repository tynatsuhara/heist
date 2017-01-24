using UnityEngine;
using System.Collections;

public class WeaponSelection : Menu {

	public MenuNode sidearm;
	public MenuNode weapon;
	private CharacterCustomizationMenu ccm;	

	void Start() {
		ccm = CharacterCustomizationMenu.instance;
		sidearm.SetText(ccm.sidearms[PlayerPrefs.GetInt("p" + playerId + "_sidearm", 0)].name.ToUpper());		
		weapon.SetText(ccm.weapons[PlayerPrefs.GetInt("p" + playerId + "_weapon", 0)].name.ToUpper());		
	}

	public override void Carousel(MenuNode node, int dir) {
		if (node.name == "Sidearm") {
			node.SetText(LoadWeapon(ccm.sidearms, ccm.CurrentSidearmId(playerId), dir, "_sidearm"));
		} else if (node.name == "Weapon") {
			node.SetText(LoadWeapon(ccm.weapons, ccm.CurrentWeaponId(playerId), dir, "_weapon"));
		}
	}

	private string LoadWeapon(GameObject[] arr, int currentIndex, int dir, string prefString) {
		int index = (arr.Length + currentIndex + dir) % arr.Length;
		GameObject gun = arr[index];
		PlayerPrefs.SetInt("p" + playerId + prefString, index);
		return gun.name.ToUpper();
	}

	public override void Enter(MenuNode node) {
		switch (node.name) {
			case "Ready":
				break;
		}
	}
}
