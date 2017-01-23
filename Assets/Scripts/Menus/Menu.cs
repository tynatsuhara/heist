using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public RectTransform tint;
	public Transform cursor;

	public MenuNode selectedNode;
	private MenuNode[] all;

	void Awake() {
		all = GetComponentsInChildren<MenuNode>();
	}

	void Update () {
		if (tint != null)
			tint.sizeDelta = new Vector2(Screen.width, Screen.height);
		GetInput();
		selectedNode.Select();
	}

	private void GetInput() {
		if (selectedNode == null)
			return;

		selectedNode.Deselect();

		if (Input.GetKeyDown(KeyCode.W) && selectedNode.up != null) {
			selectedNode = selectedNode.up;
		} else if (Input.GetKeyDown(KeyCode.S) && selectedNode.down != null) {
			selectedNode = selectedNode.down;
		} else if (Input.GetKeyDown(KeyCode.A)) {
			if (selectedNode.carousel) {
				Carousel(selectedNode, -1);
			} else if (selectedNode.left != null) {
				selectedNode = selectedNode.left;
			}
		} else if (Input.GetKeyDown(KeyCode.D)) {
			if (selectedNode.carousel) {
				Carousel(selectedNode, 1);
			} else if (selectedNode.right != null) {
				selectedNode = selectedNode.right;
			}
		}

		selectedNode.Select();
	}

	public virtual void Carousel(MenuNode node, int dir) {}
	public virtual void Enter(MenuNode node) {}
}
