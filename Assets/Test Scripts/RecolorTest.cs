using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecolorTest : MonoBehaviour {

	public bool runTest = false;
	private Recolor recolor;

	private float startTime;
	private float timeElapsed;
	private Dictionary<Vector3, Color> dict;

	void Start() {
		recolor = GetComponent<Recolor>();
	}

	// Update is called once per frame
	void Update () {
		if (runTest) {
			runTest = false;
			Test();
		}
	}

	private void Test() {
		startTime = Time.realtimeSinceStartup;
		dict = recolor.ColorAll(Color.white);
		timeElapsed = Time.realtimeSinceStartup - startTime;
		Debug.Log("time elapsed = " + timeElapsed);
		Invoke("Untest", .05f);
	}

	private void Untest() {
		recolor.Colorize(dict);
	}
}
