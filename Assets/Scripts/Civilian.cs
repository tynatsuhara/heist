using UnityEngine;
using System.Collections;

public class Civilian : Character {

	private string[] copUniform = {
		"0 0-73; 1 57 60 44 45 31; 2 46; 6 58 59; 4 14-27; 3 17",
		"8 37 40; 7 26-33 44-51 60 62-69 71 78-89 96-119 91-94",
		"0 1; 5 0",
		"0 1-3"
	};

	void Awake() {
		rb = GetComponent<Rigidbody>();
	}

	void Start () {
		GetComponent<CharacterCustomization>().ColorCharacter(copUniform, true);
	}
	
	void Update () {
	
	}

	public override void Alert(Character.Reaction importance, Vector3 position) {

	}
}
