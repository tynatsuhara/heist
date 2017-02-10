using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutfitModel : MonoBehaviour {

	public string name;
	public Color32 skinPlaceholder;
	public Color32 hairPlaceholder;
	public Accessory[] accessories;

	public PicaVoxel.Volume head;
	public PicaVoxel.Volume body;
	public PicaVoxel.Volume arms;
	public PicaVoxel.Volume legs;

	void Awake() {
		Outfits.fits.Add(name, new Outfits.Outfit(new string[] {
			Encode(body),
			Encode(head),
			Encode(legs),
			Encode(arms)
		}, accessories: accessories));
	}

	private string Encode(PicaVoxel.Volume v) {
		Dictionary<Color32, List<byte>> map = new Dictionary<Color32, List<byte>>();
		for (int x = 0; x < v.XSize; x++) {
			for (int y = 0; y < v.YSize; y++) {
				for (int z = 0; z < v.ZSize; z++) {
					PicaVoxel.Voxel? vox = v.Frames[1].GetVoxelAtArrayPosition(x, y, z);
					if (!vox.HasValue || !vox.Value.Active)
						continue;
					Color32 c = v.Frames[0].GetVoxelAtArrayPosition(x, y, z).Value.Color;
					if (!map.ContainsKey(c))
						map[c] = new List<byte>();
					map[c].Add(vox.Value.Value);
				}
			}
		}
		string res = "";
		foreach (Color32 c in map.Keys) {
			if (c.Equals(skinPlaceholder)) {
				res += "6 ";
			} else if (c.Equals(hairPlaceholder)) {
				res += "7 ";
			} else {
				res += GetHex(c) + " ";
			}
			foreach (byte b in map[c]) {
				res += b + " ";
			}
			res += ";";
		}
		return res;
	}

	private string GetHex(Color32 c) {
		return c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2");
	}
}
