using UnityEngine;
using System.Collections;

public class CharacterCustomization : MonoBehaviour {

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
}
