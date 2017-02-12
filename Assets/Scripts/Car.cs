using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Car : MonoBehaviour, Damageable, Interactable {

	public bool locked;
	public int spots = 2;
	public Character[] characters;
	private List<Character> charactersByDoors;
	public PicaVoxel.Exploder exploder;
	private Collider[] doorSpots;	
	public Transform destination;

	private int spotsFilled;
	public bool isEmpty {
		get { return spotsFilled == 0; }
	}
	public bool isGetaway {
		get { return this == GameManager.instance.getaway; }
	}

	void Awake() {
		characters = new Character[spots];
		charactersByDoors = new List<Character>();
		doorSpots = Object.FindObjectsOfType<Collider>().Where((x) => x.isTrigger).ToArray();
	}

	void Update() {
		if (destination != null) {
			GetComponent<NavMeshAgent>().SetDestination(destination.position);
		}
	}

	public bool GetIn(Character c) {
		if (locked)
			return false;

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
	}

	public bool GetOut(Character c) {
		for (int i = characters.Length - 1; i >= 0; i++) {
			if (characters[i] == c) {
				spotsFilled--;
				characters[i] = null;

				// do stuff with the character, make them appear at the door
				int whichDoor = i % doorSpots.Length;
				c.transform.position = doorSpots[whichDoor].transform.position;
				c.gameObject.SetActive(true);

				return true;
			}
		}
		return false;
	}

	public bool Damage(Vector3 location, Vector3 angle, float damage, bool playerAttacker = false, DamageType type = DamageType.BULLET) {
		if (type == DamageType.MELEE || type == DamageType.NONLETHAL || type == DamageType.SLICE)
			return false;
		
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

	public void Interact(Character c) {
		if (!isGetaway)
			return;
		if (charactersByDoors.Contains(c)) {
			if (GetIn(c)) {
				c.gameObject.SetActive(false);
				// if the player gets in the getaway with a bag, we need to save the money in it
				if (c.hasBag && this == GameManager.instance.getaway) {
					c.bag.SaveLoot();
					c.DropBag();
				}
			}
		}
	}

	public bool ContainsAllLivingPlayers() {
		foreach (PlayerControls pc in GameManager.players) {
			if (pc.isAlive && !ContainsCharacter(pc)) {
				return false;
			}
		}
		return true;
	}

	public bool ContainsCharacter(Character c) {
		foreach (Character ch in characters) {
			if (ch == c) {
				return true;
			}
		}
		return false;
	}

	public void Uninteract(Character character) {}

	void OnTriggerEnter(Collider other) {
		if (!isGetaway)
			return;
		Character c = other.GetComponentInParent<Character>();
		if (c != null)
			charactersByDoors.Add(c);
	}

	void OnTriggerExit(Collider other) {
		if (!isGetaway)
			return;
		Character c = other.GetComponentInParent<Character>();
		if (c != null)
			charactersByDoors.Remove(c);
	}
}
