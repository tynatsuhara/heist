using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : Menu {

	public override void Enter(MenuNode node) {
		SceneManager.LoadScene("customization");
	}
}
