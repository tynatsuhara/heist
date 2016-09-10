using UnityEngine;
using UnityEngine.UI;

public class ColorShift : MonoBehaviour {

	public float shiftSpeed;
	public float fadeAfter;
	public float fadeRate;
	private RawImage img;
	private float timeElapsed;
	private bool invoked;

	void Start () {
		img = GetComponent<RawImage>();
	}
	
	void Update () {
		timeElapsed += Time.deltaTime;
		Color c = img.color;
		if (c.a <= 0f)
			return;
		float h, s, v;
		Color.RGBToHSV(c, out h, out s, out v);
		h = (h + shiftSpeed * Time.deltaTime) % 1f;
		Color d = Color.HSVToRGB(h, s, v);
		if (timeElapsed > fadeAfter)
			d.a = c.a - fadeRate * Time.deltaTime;
		if (d.a <= 0 && !invoked) {
			Invoke("Transition", .2f);
			invoked = true;
		}
		img.color = d;
	}

	private void Transition() {
		Debug.Log("Transition!");
	}
}
