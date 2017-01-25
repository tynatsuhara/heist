using UnityEngine;
using System.Collections;

public class LightPulse : MonoBehaviour {

	private float multiplier = 1.5f;
	private float scale = 7.5f;
	public bool sin;

	void Update () {
		if (sin)
			GetComponent<Light>().intensity = multiplier * Mathf.Abs(Mathf.Sin(Time.time * scale));
		else
			GetComponent<Light>().intensity = multiplier * Mathf.Abs(Mathf.Cos(Time.time * scale));
	}
}
