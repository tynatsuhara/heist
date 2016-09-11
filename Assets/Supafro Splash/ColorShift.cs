using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ColorShift : MonoBehaviour {

	public RawImage layer1;
	public RawImage layer2;
	public float shiftSpeed;
	public float finalTime;
	public float fadeAfter;
	public float fadeRate;
	private float timeElapsed;
	private bool invoked;

	void Update () {
		Cycle(layer1, 1f);
		Cycle(layer2, -.5f);
	}

	private void Cycle(RawImage img, float hueShiftMultiplier) {
		Color c = img.color;
		if (c.a <= 0f)
			return;
		float h, s, v;
		Color.RGBToHSV(c, out h, out s, out v);
		h = (h + hueShiftMultiplier * shiftSpeed * Time.deltaTime) % 1f;
		Color d = Color.HSVToRGB(h, s, v);
		if (timeElapsed > fadeAfter)
			d.a = c.a - fadeRate * Time.deltaTime;
		if (d.a <= 0 && !invoked) {
			Invoke("Transition", finalTime);
			invoked = true;
		}
		img.color = d;
		timeElapsed += Time.deltaTime;
	}

	private void Transition() {
		SceneManager.LoadScene("game");
	}
}
