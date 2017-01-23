using UnityEngine;
using System.Collections;

public class WeaponSelection : Menu {

	public int playerId;

	void Start() {

	}

	public override void Carousel(MenuNode node, int dir) {
		CharacterCustomizationMenu ccm = CharacterCustomizationMenu.instance;
		if (node.name == "Sidearm") {
				int index = (ccm.sidearms.Length + ccm.CurrentSidearmId(playerId) + dir) % ccm.sidearms.Length;
				GameObject gun = ccm.sidearms[index];
				node.SetText(gun.name.ToUpper());
				PlayerPrefs.SetInt("p" + playerId + "_sidearm", index);
		} else if (node.name == "Weapon") {
				int index = (ccm.weapons.Length + ccm.CurrentWeaponId(playerId) + dir) % ccm.weapons.Length;
				Debug.Log(index);
				GameObject gun = ccm.weapons[index];
				node.SetText(gun.name.ToUpper());
				PlayerPrefs.SetInt("p" + playerId + "_weapon", index);
		}
	}

	public override void Enter(MenuNode node) {
		switch (node.name) {
			case "Ready":
				break;
		}
	}
}
