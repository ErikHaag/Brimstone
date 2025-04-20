using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Quintessential;

namespace Brimstone;
public static class BrimstoneAPI
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

    // Create a nonmetal atom type
    [Flags]
    public enum AtomBehavior
    {
        None = 0,
        Cardinal = 1,
        Quicksilver = 2,
        Quintessense = 4
    }
    public static AtomType CreateAtom(byte ID, string modName, string name, string pathToSymbol, string pathToDiffuse, string pathToShade = "textures/atoms/salt_shade", AtomBehavior atomBehavior = AtomBehavior.None)
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
            field_2293 = (atomBehavior & AtomBehavior.Cardinal) == AtomBehavior.Cardinal,
            field_2295 = (atomBehavior & AtomBehavior.Quicksilver) == AtomBehavior.Quicksilver,
            field_2296 = (atomBehavior & AtomBehavior.Quintessense) == AtomBehavior.Quintessense,
            QuintAtomType = modName + ":" + name.ToLower()
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
        if (promotesTo is not null)
        {
            atom.field_2297 = Maybe<AtomType>.method_1089(promotesTo);
        }
        return atom;
    }

    /* File Utils */

    public static class_256 GetTexture(string path = "Quintessential/missing") => class_235.method_615(path);

    public static string GetContentPath(string modName)
    {
        foreach (string dir in QuintessentialLoader.ModContentDirectories)
        {
            Quintessential.Logger.Log(dir);
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

    public static void PlaySound(Sim sim, Sound sound) => BrimstoneAPI.PrivateMethod<Sim>("method_1856").Invoke(sim, new object[] { sound });
}