using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.TagSearch
{
    public class TagDict
    {
		public static IDictionary<string, HashSet<string>> clothesDict = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

		public static void AddToClothesDict()
		{
			if (!clothesDict.ContainsKey("Colors"))
			{
				clothesDict.Add("Colors", new HashSet<string> { "beige", "black", "blue", "bourdeaux", "brown",
					"gold", "gray", "green", "light blue", "lime", "orange", "peach", "pink", "purple",
					"red", "silver","turquoise", "violet", "white", "yellow", "colorful"});
			}

			if (!clothesDict.ContainsKey("Type"))
			{ 
				clothesDict.Add("Type", new HashSet<string> { "bag", "bikini", "blazer", "blouse", 
				"board shorts", "boilersuit", "boots", "boxer", "brief","cardigan", "chinos",
				"coat", "co-ords", "crop top", "dress", "dressing gown", "fleece", "flip flop", 
				"gilets", "hat", "hoodies", "jacket", "jeans", "jogger", "jumper", "jumpsuit",
				"kimonos", "leggings", "lingerie", "lingerie sets", "loungewear set",
				"loungewear sets", "onesies", "overall", "pants", "polo shirt", "pyjamas", "sandals",
				"scarf", "shapewear", "shell top", "shirt", "shoes", "shorts", "skirt", "sliders",
				"slippers", "socks", "sports bras", "suit", "suit trousers", "sweatshirt", 
				"swimsuit", "t-shirt", "tank top", "tankinis", "tartan", "tights", "top", "trainers",
				"trousers", "trunk", "tunic", "underwear set" ,"vest", "waistcoat"});
			}

			if (!clothesDict.ContainsKey("Gender"))
            {
				clothesDict.Add("Gender", new HashSet<string> { "women", "men", "baby", "kids", "teenager"});
			}

			if (!clothesDict.ContainsKey("Pattern"))
			{
				clothesDict.Add("Pattern", new HashSet<string> { "animal print", "argyle", "batik",
				"camouflage", "check", "checkered", "chevron", "colorblock", "diamond", "fair isle",
				"floral", "geometric", "gingham" ,"herringbone", "houndstooth", "paisley",
				"patchwork", "plaid", "polka dot", "solid","spotted", "striped", "tie dye"});
			}

			if (!clothesDict.ContainsKey("Length"))
			{
				clothesDict.Add("Length", new HashSet<string> { "boat neck", "collared", "cowl neck",
				"crew neck", "half sleeve", "halter", "high neck", "knee", "knee length", "long",
				"long sleeve", "low", "maxi", "midi", "mini", "mock neck", "neckline",
				"off the shoulder", "one shoulder", "round neck", "scoop neck", "short", "sleeve", 
				"sleeve length", "sleeveless", "square neck", "turtleneck", "v-neck", "3/4 sleeve"});
			}

			if (!clothesDict.ContainsKey("Occasion"))
			{
				clothesDict.Add("Occasion", new HashSet<string> { "business", "casual", "formal", "party",
				"travel", "wedding", "workwear"});
			}

			if (!clothesDict.ContainsKey("Season"))
			{
				clothesDict.Add("Season", new HashSet<string> { "autumn", "spring", "summer", "winter"});
			}

			if (!clothesDict.ContainsKey("Brand"))
            {
				clothesDict.Add("Brand", new HashSet<string> { "Adidas", "Armani", "Burberry", 
				"Calvin Klein", "Chanel", "Columbia", "Dior", "Fila", "Guess", "Havaianas",
				"Hollister", "Hugo Boss", "Kappa", "Lacoste", "Lee Cooper", "Louis Vuitton",  
				"Michael Kors", "Microsoft", "Nike", "The North Face", "Prada", "Puma", 
				"Quicksilver", "Ray Ban", "Reebok", "Skechers", "Timberland", "Tommy Hilfiger",
				"Under Armour", "Vans"});
			}
		}


	}
}


