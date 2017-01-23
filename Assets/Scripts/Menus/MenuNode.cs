using UnityEngine;
using UnityEngine.UI;

public class MenuNode : MonoBehaviour {

	public string key;

	public MenuNode up;
	public MenuNode down;
	public MenuNode left;
	public MenuNode right;

	public bool carousel;
	public Material defaultMaterial;
	public Material selectedMaterial;

	private bool selected;

	public void Select() {
		SetSelected(true);
	}

	public void Deselect() {
		SetSelected(false);
	}

	private void SetSelected(bool newSelectedState) {
		if (selected == newSelectedState)
			return;
		selected = newSelectedState;
		Text[] t = GetComponentsInChildren<Text>();
		foreach (Text txt in t)
			txt.material = selected ? selectedMaterial : defaultMaterial;
	}
}
