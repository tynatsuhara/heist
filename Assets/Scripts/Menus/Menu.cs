using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public RectTransform tint;
	public Transform cursor;

	public MenuNode selectedNode;
	private MenuNode[] all;

	void Start() {
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

		if (Input.GetAxisRaw("Vertical") != 0) {
			int dir = (int) Mathf.Sign(Input.GetAxisRaw("Vertical"));
			if (dir == 1 && selectedNode.up != null) {
				selectedNode = selectedNode.up;
			} else if (dir == -1 && selectedNode.down != null) {
				selectedNode = selectedNode.down;
			}
		} else if (Input.GetAxisRaw("Horizontal") != 0) {
			int dir = (int) Mathf.Sign(Input.GetAxisRaw("Horizontal"));
			if (selectedNode.carousel) {
				Carousel(selectedNode.name, dir);
				// this has options, cycle and do something based on that
			} else {
				// go to the left or right node
				if (dir == 1 && selectedNode.right != null) {
					selectedNode = selectedNode.right;
				} else if (dir == -1 && selectedNode.left != null) {
					selectedNode = selectedNode.left;
				}
			}
		}

		selectedNode.Select();
	}

	public virtual void Carousel(string key, int dir) {}
	public virtual void Enter(string key) {}
}
