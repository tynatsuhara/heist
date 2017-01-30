using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour, Damageable {

		public bool Damage(Vector3 location, Vector3 angle, float damage, bool melee = false, 
				bool playerAttacker = false, bool explosive = false) {
			return true;
		}
}
