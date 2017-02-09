using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Outfits {

	public static Dictionary<string, Outfit> fits = new Dictionary<string, Outfit>() {
		{
			// outfit format: [body, head, legs, arms]
			// string format:
			//     "<color number> <number or inclusive range>(s); <etc>"
			"default", 
			new Outfit(new string[] {
				"3 0-13 70-73; 0 14-69; 1 58-59 44-45 30-31 16-17",
				"8 37 40",
				"3 1; 5 0",
				"0 1-3"
			},  shirtColor1: "#3D5B8500", shirtColor2: "#2E415A00", shirtColor3: "#F9F79600",
			    pantsColor1: "#ABABAB00", pantsColor2: "#22222200", shoesColor: "#17171700")
		},
		{
			"cop1", 
			new Outfit(new string[] {
				"0 0-73; 1 57 60 44 45 31; 2 46; 6 58 59; 4 14-27; 3 17",
				"8 37 40",
				"0 1; 5 0",
				"0 1-3"
			}, shirtColor1: "#3D5B8500", shirtColor2: "#2E415A00", shirtColor3: "#F9F79600",
			   pantsColor1: "#ABABAB00", pantsColor2: "#22222200", shoesColor: "#17171700")
		}
	};

	public class Outfit {
		public string[] pattern;
		public Color32[] colors;
		public Accessory[] accessories;
		
		public Outfit(string[] pattern, string shirtColor1="#FFFFFF", string shirtColor2="#FFFFFF", 
										string shirtColor3="#FFFFFF", string pantsColor1="#FFFFFF", 
										string pantsColor2="#FFFFFF", string shoesColor="#FFFFFF", 
										Accessory[] accessories=null) {
			this.pattern = pattern;
			this.accessories = accessories == null ? new Accessory[0] : accessories;
			colors = new Color32[] {
				HexParse(shirtColor1),
				HexParse(shirtColor2),
				HexParse(shirtColor3),
				HexParse(pantsColor1),
				HexParse(pantsColor2),
				HexParse(shoesColor)
			};
		}

		private static Color32 HexParse(string hex) {
			hex = hex.Replace("0x", "").Replace("#", "");
			return new Color32((byte) int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
							   (byte) int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
							   (byte) int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
							   (byte) 255);
		}
	}
}
