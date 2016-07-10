using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour, Damageable {

	public int spots = 2;
	public Character[] characters;
	public PicaVoxel.Exploder exploder;

	private int spotsFilled;
	public bool isEmpty {
		get { return spotsFilled == 0; }
	}

	void Start() {
		characters = new Character[spots];
	}

	public bool GetIn(Character c) {
		int firstNullSpot = -1;
		for (int i = 0; i < characters.Length; i++) {
			if (characters[i] == c) {
				return false;
			} else if (characters[i] == null) {
				firstNullSpot = i;
			}
		}

		if (firstNullSpot == -1)
			return false;
		
		characters[firstNullSpot] = c;
		spotsFilled++;
		return true;

		// do stuff with the character, probably. make them invisible?
	}

	public bool GetOut(Character c) {
		for (int i = characters.Length - 1; i >= 0; i++) {
			if (characters[i] == c) {
				spotsFilled--;
				characters[i] = null;

				// do stuff with the character, make them appear at the door

				return true;
			}
		}
		return false;
	}

	public bool Damage(Vector3 location, Vector3 angle, float damage) {
		Vector3 pos = location + angle * Random.Range(-.1f, .2f) + new Vector3(0, Random.Range(-.3f, 0), 0);;
		exploder.transform.position = pos;
		pos = exploder.transform.localPosition;
		Random.Range(-0.025f, 0.4f);
		pos.y = Random.Range(-0.02f, 0.4f);
		exploder.transform.localPosition = pos;
		exploder.Explode(angle * 2);

		// Damage people inside?

		return false;
	}
}
