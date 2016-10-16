using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class for the intro splash screen
public class ColorShift : MonoBehaviour {

	public RawImage layer1;
	public RawImage layer2;
	public float shiftSpeed;
	public float finalTime;
	public float fadeAfter;
	public float fadeRate;
	private float timeElapsed;
	private bool invoked;
	private float layer1Speed;
	private float layer2Speed;
	public float growRate;
	public CanvasScaler scaler;

	void Awake() {
		layer1Speed = Random.Range(.3f, 1.2f) * Random.Range(0, 2) == 0 ? 1f : -1f;
		layer2Speed = Random.Range(.3f, 1.2f) * Random.Range(0, 2) == 0 ? 1f : -1f;
		layer1.color = ShiftHue(layer1.color, Random.Range(0f, 1f));
		layer2.color = ShiftHue(layer2.color, Random.Range(0f, 1f));
	}

	void Update() {
		scaler.scaleFactor += growRate * Time.deltaTime;
		Cycle(layer1, layer1Speed);
		Cycle(layer2, layer2Speed);
	}

	private void Cycle(RawImage img, float hueShiftMultiplier) {
		Color c = img.color;
		if (c.a <= 0f)
			return;
		
		Color d = ShiftHue(c, hueShiftMultiplier);
		if (timeElapsed > fadeAfter)
			d.a = c.a - fadeRate * Time.deltaTime;
		if (d.a <= 0 && !invoked) {
			Invoke("Transition", finalTime);
			invoked = true;
		}
		img.color = d;
		timeElapsed += Time.deltaTime;
	}

	private Color ShiftHue(Color c, float shiftAmount) {
		float h, s, v;
		Color.RGBToHSV(c, out h, out s, out v);
		h = (h + shiftAmount * shiftSpeed * Time.deltaTime) % 1f;
		return Color.HSVToRGB(h, s, v);
	}

	private void Transition() {
		SceneManager.LoadScene("game");
	}
}
