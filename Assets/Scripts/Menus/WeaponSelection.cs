using UnityEngine;
using System.Collections;

public class WeaponSelection : Menu {

	public override void Carousel(string key, int dir) {
		switch (key) {
			case "Sidearm":
				break;
			case "Weapon":
				break;
		}
	}

	public override void Enter(string key) {
		switch (key) {
			case "Ready":
				break;
		}
	}
}
