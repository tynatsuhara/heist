using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public RectTransform tint;
	public Transform cursor;
	public bool perPerson = false;
	public int playerId;

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
		if(GetUp())
			Debug.Log("up");
		if(GetDown())
			Debug.Log("down");
		if(GetLeft())
			Debug.Log("left");
		if(GetRight())
			Debug.Log("right");
		SaveDPad();
	}

	private void GetInput() {
		if (selectedNode == null)
			return;

		selectedNode.Deselect();

		if (GetUp() && selectedNode.up != null) {
			selectedNode = selectedNode.up;
		} else if (GetDown() && selectedNode.down != null) {
			selectedNode = selectedNode.down;
		} else if (GetLeft()) {
			if (selectedNode.carousel) {
				Carousel(selectedNode, -1);
			} else if (selectedNode.left != null) {
				selectedNode = selectedNode.left;
			}
		} else if (GetRight()) {
			if (selectedNode.carousel) {
				Carousel(selectedNode, 1);
			} else if (selectedNode.right != null) {
				selectedNode = selectedNode.right;
			}
		}

		selectedNode.Select();
	}

	private Vector2 lastDPad;
	private void SaveDPad() {
		if (perPerson) {
			lastDPad = new Vector2(Input.GetAxis("DPX" + playerId), Input.GetAxis("DPY" + playerId));
		} else {
			lastDPad = new Vector2(Input.GetAxis("DPX"), Input.GetAxis("DPY"));
		}
	}
	private bool GetUp() {
		return ((Input.GetKeyDown(KeyCode.W) && (!perPerson || playerId == 1)) || 
				(lastDPad.y == 0 && perPerson && Input.GetAxis("DPY" + playerId) > 0) || 
				(lastDPad.y == 0 && !perPerson && Input.GetAxis("DPY") > 0));
	}
	private bool GetDown() {
		return ((Input.GetKeyDown(KeyCode.S) && (!perPerson || playerId == 1)) || 
				(lastDPad.y == 0 && perPerson && Input.GetAxis("DPY" + playerId) < 0) || 
				(lastDPad.y == 0 && !perPerson && Input.GetAxis("DPY") < 0));
	}
	private bool GetLeft() {
		return ((Input.GetKeyDown(KeyCode.A) && (!perPerson || playerId == 1)) || 
				(lastDPad.x == 0 && perPerson && Input.GetAxis("DPX" + playerId) < 0) || 
				(lastDPad.x == 0 && !perPerson && Input.GetAxis("DPX") < 0));
	}
	private bool GetRight() {
		return ((Input.GetKeyDown(KeyCode.D) && (!perPerson || playerId == 1)) || 
				(lastDPad.x == 0 && perPerson && Input.GetAxis("DPX" + playerId) > 0) || 
				(lastDPad.x == 0 && !perPerson && Input.GetAxis("DPX") > 0));
	}

	public virtual void Carousel(MenuNode node, int dir) {}
	public virtual void Enter(MenuNode node) {}
}
