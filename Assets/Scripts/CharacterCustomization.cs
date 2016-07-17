using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterCustomization : MonoBehaviour {

	private byte[] EYES = {37, 40};
	private byte[] GORE = {255};
	
	// Each character component
	public Recolor head;
	public Recolor body;
	public Recolor legs;
	public Recolor arms;
	public Recolor gunz;

	// Use this for initialization
	void Start () {
		byte[] a = Range(0, 7);
		byte[] b = Range(5, 12);
		byte[] c = Range(20, 30);
		DebugBytes(a);
		DebugBytes(b);
		DebugBytes(c);
		DebugBytes(Merge(a, b, c));
	}

	// Takes a dict from colors to byte[] and returns a dict
	// mapping bytes to colors
	private Dictionary<byte, Color32> BytesToColors(Dictionary<Color32, byte[]> dict) {
		Dictionary<byte, Color32> res = new Dictionary<byte, Color32>();
		foreach (Color32 color in dict.Keys) {
			foreach (byte b in dict[color]) {
				res.Add(b, color);
			}
		}
		return res;
	}

	// Returns a byte array from start to end (both inclusive)
	private byte[] Range(int start, int end) {
		byte startByte = (byte)start;
		byte endByte = (byte)end;
		byte[] res = new byte[end - start + 1];
		for (byte i = startByte; i <= endByte; i++) {
			res[i - start] = i;
		}
		return res;
	}

	private byte[] Merge(params byte[][] lists) {
		int len = 0;
		foreach (byte[] b in lists) {
			len += b.Length;
		}
		byte[] res = new byte[len];
		int index = 0;
		foreach (byte[] bytes in lists) {
			foreach (byte b in bytes) {
				res[index++] = b;
			}
		}
		return res;
	}

	// Private helper method for testing
	private void DebugBytes(byte[] bytes) {
		if (bytes.Length == 0) {
			Debug.Log("[]");
		} else if (bytes.Length == 1) {
			Debug.Log("[" + (int)bytes[0] + "]");
		} else {
			string s = "[" + (int)bytes[0];
			for (int i = 1; i < bytes.Length; i++) {
				s += "," + (int)bytes[i];
			}
			Debug.Log(s + "]");
		}
	}
}
