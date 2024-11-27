using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Quintessential;

namespace Brimstone;

using Song = class_186;
public static partial class BrimstoneAPI
{

	// Create a promotable metal element
	public static AtomType CreateMetal(byte ID, string modName, string name, string pathToSymbol, string pathToLightramp, AtomType promotesTo = null)
	{
		AtomType atom = new()
		{
			field_2283 = ID,
			field_2284 = class_134.method_254(name), // Non local name
			field_2285 = class_134.method_253("Elemental " + name, string.Empty), // Atomic name
			field_2286 = class_134.method_253(name, string.Empty), // Local name
			field_2287 = class_235.method_615(pathToSymbol), // Symbol
			field_2288 = class_235.method_615("textures/atoms/shadow"), // Shadow
			field_2291 = new()
			{
				field_13 = class_238.field_1989.field_81.field_577, // Diffuse
				field_14 = class_235.method_615(pathToLightramp),
				field_15 = class_238.field_1989.field_81.field_613.field_633 // Shiny
			},
			field_2294 = true,
			QuintAtomType = modName + ":" + name.ToLower()
		};
		if (promotesTo is not null) {
			atom.field_2297 = Maybe<AtomType>.method_1089(promotesTo);
		}
		return atom;
	}

    public static float FloatFromString(string str, float defaulF = 0f)
    {
        if (!string.IsNullOrEmpty(str))
        {
            return float.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat);
        }
        else
        {
            return defaulF;
        }
    }

    public static Vector2 Vector2FromString(string pos, float defaultX = 0f, float defaultY = 0f)
    {
        float x = FloatFromString(pos?.Split(',')[0], defaultX);
        float y = FloatFromString(pos?.Split(',')[1], defaultY);
        return new Vector2(x, y);
    }

    /* Data related stuff */

    //Get the metadata of a mod via it's name
    private static readonly Dictionary<string, ModMeta> modMetaDictionary = new();
    public static ModMeta GetModMeta(string name)
	{
		bool found = false;
		// Check if modmeta is stored
		if (modMetaDictionary.TryGetValue(name, out ModMeta modMeta))
		{
			found = true;
		}
		else
		{
			// Add each mod's folder path to dictionary
			foreach (ModMeta m in QuintessentialLoader.Mods)
			{
				modMetaDictionary.Add(m.Name, m);
				if (!found && m.Name == name)
				{
					modMeta = m;
					found = true;
				}
			}

		}
		if (found)
		{
			return modMeta;
		}
		else
		{
			Quintessential.Logger.Log("Brimstone: Unable to find mod \"" + name + "\"");
			return null;
		}
	}

	public static void RemoveModMeta(string name)
	{
		modMetaDictionary.Remove(name);
	}

    // Get campaign from it's title
    private static readonly Dictionary<string, Campaign> modCampaignDictionary = new();
    public static Campaign GetCampaign(string title)
	{
		bool found = false;
		// Check if campaign is stored
		if (modCampaignDictionary.TryGetValue(title, out Campaign campaign))
		{
			found = true;
		}
		else
		{
			// Add each campaign to dictionary
			foreach (Campaign c in QuintessentialLoader.AllCampaigns)
			{
				modCampaignDictionary.Add(c.QuintTitle, c);
				if (!found && c.QuintTitle == title)
				{
					campaign = c;
					found = true;
				}	
			}

		}
		if (found)
		{
			return campaign;
		}
		else
		{
			Quintessential.Logger.Log("Brimstone: Unable to find campaign \"" + title + "\"");
			return null;
		}
	}

	public static void RemoveCampaign(string title) {
		modCampaignDictionary.Remove(title);
	}

	// Getting sweet grooves
	private static Dictionary<string, Tuple<Song, Sound>> songAndFanfareDictionary = new();
	internal static void LoadSongAndFanfareDictionary()
	{
		Quintessential.Logger.Log("Brimstone: Loading sweet grooves");
		class_102 song = class_238.field_1992;
		class_201 fanfare = class_238.field_1991;
		songAndFanfareDictionary.Add("Map", Tuple.Create(song.field_968, fanfare.field_1832));
		songAndFanfareDictionary.Add("Solitaire", Tuple.Create(song.field_969, fanfare.field_1832));
		songAndFanfareDictionary.Add("Solving1", Tuple.Create(song.field_970, fanfare.field_1830));
		songAndFanfareDictionary.Add("Solving2", Tuple.Create(song.field_971, fanfare.field_1831));
		songAndFanfareDictionary.Add("Solving3", Tuple.Create(song.field_972, fanfare.field_1832));
		songAndFanfareDictionary.Add("Solving4", Tuple.Create(song.field_973, fanfare.field_1833));
		songAndFanfareDictionary.Add("Solving5", Tuple.Create(song.field_974, fanfare.field_1834));
		songAndFanfareDictionary.Add("Solving6", Tuple.Create(song.field_975, fanfare.field_1835));
		songAndFanfareDictionary.Add("Story1", Tuple.Create(song.field_976, fanfare.field_1832));
		songAndFanfareDictionary.Add("Story2", Tuple.Create(song.field_977, fanfare.field_1832));
		songAndFanfareDictionary.Add("Title", Tuple.Create(song.field_978, fanfare.field_1832));
		Quintessential.Logger.Log("Brimstone: Tracks sorted");
	}



	public static Tuple<Song, Sound> GetSongAndFanfare(string title)
	{
		if (!songAndFanfareDictionary.TryGetValue(title, out Tuple<Song, Sound> SAF))
		{
			Quintessential.Logger.Log("Brimstone: Unable to find \"" + title + "\", Defaulting to \"Solving1\"");
			return songAndFanfareDictionary["Solving1"];
		}
		return SAF;
	}

	private static Dictionary<string, AdvancedContentModel> AdvancedContentDictionary = new();

	// A D V A N C E D content
	public static AdvancedContentModel GetAdvancedContentModel(ModMeta modMeta)
	{
		if (AdvancedContentDictionary.TryGetValue(modMeta.Name, out	AdvancedContentModel acm))
		{
			return acm;
		}
        acm = YamlHelper.Deserializer.Deserialize<AdvancedContentModel>(new StreamReader(modMeta.PathDirectory + "/Puzzles/" + modMeta.Name + ".advanced.yaml"));
		AdvancedContentDictionary.Add(modMeta.Name, acm);
		return acm;
    }

	public static void RemoveAdvancedContentModel(ModMeta modMeta)
	{
		AdvancedContentDictionary.Remove(modMeta.Name);
	}
}