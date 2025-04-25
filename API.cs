using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Quintessential;

namespace Brimstone;

using Texture = class_256;
public static class API
{
    /* AtomType utils */

    public static readonly Dictionary<string, AtomType> VanillaAtoms = new() {
        { "salt", class_175.field_1675 },
        { "air",  class_175.field_1676 },
        { "fire", class_175.field_1678 },
        { "quicksilver", class_175.field_1680 },
        { "water", class_175.field_1679 },
        { "earth", class_175.field_1677 },
        { "lead", class_175.field_1681 },
        { "tin", class_175.field_1683 },
        { "iron", class_175.field_1684 },
        { "copper", class_175.field_1682 },
        { "silver", class_175.field_1685 },
        { "gold", class_175.field_1686 },
        { "vitae", class_175.field_1687 },
        { "mors", class_175.field_1688 },
        { "quintessence", class_175.field_1690 }
    };

    public static AtomType CreateCardinalAtom(byte ID, string modName, string name, string pathToBase, string pathToFog, string pathToRim, string pathToSymbol, string pathToShadow)
    {
        AtomType atom = new()
        {
            field_2283 = ID,
            field_2284 = class_134.method_254(name), // Non local name
            field_2285 = class_134.method_253("Elemental " + name, string.Empty), // Atomic name
            field_2286 = class_134.method_253(name, string.Empty), // Local name
            field_2287 = class_235.method_615(pathToSymbol), // Symbol
            field_2288 = class_235.method_615(pathToShadow), // Shadow
            field_2289 = new()
            {
                field_8 = class_235.method_615(pathToBase),
                field_9 = class_235.method_615(pathToFog),
                field_10 = class_235.method_615(pathToRim)
            },
            field_2293 = true, // Cardinal
            QuintAtomType = modName + ":" + name.ToLower(),
        };
        return atom;
    }

    // Create a promotable metal element
    public static AtomType CreateMetalAtom(byte ID, string modName, string name, string pathToSymbol, string pathToLightramp, string pathToShadow = "textures/atoms/shadow", class_126 lighting = null, AtomType promotesTo = null)
    {
        AtomType atom = new()
        {
            field_2283 = ID,
            field_2284 = class_134.method_254(name), // Non local name
            field_2285 = class_134.method_253("Elemental " + name, string.Empty), // Atomic name
            field_2286 = class_134.method_253(name, string.Empty), // Local name
            field_2287 = class_235.method_615(pathToSymbol), // Symbol
            field_2288 = class_235.method_615(pathToShadow), // Shadow
            field_2291 = new()
            {
                field_13 = class_238.field_1989.field_81.field_577, // Diffuse
                field_14 = class_235.method_615(pathToLightramp), 
                field_15 = class_238.field_1989.field_81.field_613.field_634,
                atomLighting = lighting
            },
            field_2294 = true, // Metal
            QuintAtomType = modName + ":" + name.ToLower()
        };
        if (promotesTo is not null)
        {
            atom.field_2297 = Maybe<AtomType>.method_1089(promotesTo);
        }
        return atom;
    }

    // Create a nonmetal atom type
    public static AtomType CreateNormalAtom(byte ID, string modName, string name, string pathToSymbol, string pathToDiffuse, string pathToShadow = "texture/atoms/shadow", string pathToShade = "textures/atoms/salt_shade")
    {
        AtomType atom = new()
        {
            field_2283 = ID,
            field_2284 = class_134.method_254(name), // Non local name
            field_2285 = class_134.method_253("Elemental " + name, String.Empty), // Atomic name
            field_2286 = class_134.method_253(name, String.Empty), // Local name
            field_2287 = class_235.method_615(pathToSymbol),
            field_2288 = class_235.method_615(pathToShadow),
            field_2290 = new()
            {
                field_994 = class_235.method_615(pathToDiffuse),
                field_995 = class_235.method_615(pathToShade),
            },
            QuintAtomType = modName + ":" + name.ToLower()
        };
        return atom;
    }

    /* File Utils */

    public static Texture GetTexture(string path = "Quintessential/missing") => class_235.method_615(path);

    public static readonly string[] directionNames = { "left", "right", "bottom", "top" };

    public static class_126 GetLighting(string path = "textures/atoms/atom.lighting") => new class_126(GetTexture(Path.Combine(path, directionNames[0] + ".png")), GetTexture(Path.Combine(path, directionNames[1] + ".png")), GetTexture(Path.Combine(path, directionNames[2] + ".png")), GetTexture(Path.Combine(path, directionNames[3] + ".png")));

    public static QuintessentialMod GetMod(string modName)
    {
        foreach (QuintessentialMod mod in QuintessentialLoader.CodeMods)
        {
            if (mod.Meta.Name == modName)
            {
                return mod;
            }
        }
        return null;
    }

    public static string GetContentPath(string modName)
    {
        foreach (string dir in QuintessentialLoader.ModContentDirectories)
        {
            int lastCharIndex = dir.Length - 1;
            // Assumes you don't use letters in version string
            while ("_1234567890.".Contains(dir[lastCharIndex]))
            {
                lastCharIndex--;
            }
            if (!dir.Substring(0, lastCharIndex + 1).EndsWith(modName))
            {
                continue;
            }
            return Path.Combine(dir, "Content/");
        }
        return null;
    }
    public static Sound GetSound(string contentDir, string path)
    {
        string soundPath = Path.Combine(contentDir, path + ".wav");
        if (!File.Exists(soundPath))
        {
            return null;
        }
        Sound sound = new()
        {
            field_4060 = Path.GetFileNameWithoutExtension(soundPath),
            field_4061 = class_158.method_375(soundPath)
        };
        return sound;
    }

    /* Misc. */

    public static MethodInfo PrivateMethod<T>(string method) => typeof(T).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

    // Borrowed from Reductive Metallurgy
    public static bool IsModLoaded(string modName) => QuintessentialLoader.Mods.Any(mod => mod.Name == modName);

    /* Simulation Utils */

    public static void ChangeAtom(AtomReference atom, AtomType newType) => atom.field_2277.method_1106(newType, atom.field_2278);

    public static void RemoveAtom(AtomReference atom) => atom.field_2277.method_1107(atom.field_2278);

    public static void PlaySound(Sim sim, Sound sound) => API.PrivateMethod<Sim>("method_1856").Invoke(sim, new object[] { sound });
}