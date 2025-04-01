using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Quintessential;

namespace Brimstone;
public static class BrimstoneAPI
{
	// Create a nonmetal atom type
    public static AtomType CreateAtom(byte ID, string modName, string name, string pathToSymbol, string pathToDiffuse, string pathToShade = "textures/atoms/salt_shade")
	{
		AtomType atom = new()
		{
			field_2283 = ID,
			field_2284 = class_134.method_254(name), // Non local name
			field_2285 = class_134.method_253("Elemental " + name, String.Empty), // Atomic name
			field_2286 = class_134.method_253(name, String.Empty), // Local name
			field_2287 = class_235.method_615(pathToSymbol), // Symbol
			field_2288 = class_235.method_615("textures/atoms/shadow"),
			field_2290 = new()
			{
				field_994 = class_235.method_615(pathToDiffuse),
				field_995 = class_235.method_615(pathToShade),
            },
			QuintAtomType = modName + ":" +  name.ToLower()
		};
		return atom;
	}
	
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
	
	/* Simulation Utils */

    public static void ChangeAtom(AtomReference atom, AtomType newType)
    {
        // A sudden change of type, that's what animation is meant to cover up.
        atom.field_2277.method_1106(newType, atom.field_2278);
    }

    public static void RemoveAtom(AtomReference atom)
	{
		// Poof!
		atom.field_2277.method_1107(atom.field_2278);
	}

}