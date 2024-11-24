using System.Collections.Generic;
using Quintessential;

namespace Brimstone;

public static class BrimstoneAPI
{

	private static readonly Dictionary<string, ModMeta> modMetaDictionary = new();
	private static readonly Dictionary<string, Campaign> modCampaignDictionary = new();

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
		if (promotesTo != null) {
			atom.field_2297 = Maybe<AtomType>.method_1089(promotesTo);
		}
		return atom;
	}
 
	//Get the metadata of a mod via it's name
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
}