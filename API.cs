using Quintessential;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Brimstone;

using BondType = enum_126;
using PartRenderer = class_195;
using PartType = class_139;
using Texture = class_256;
using VanillaPermissions = enum_149;

/// <summary>
/// The API for Brimstone
/// </summary>
public static class API
{
    #region helper classes & enum

    /// <summary>
    /// A three state enum, to communicate what happened.
    /// </summary>
    public enum SuccessInfo
    {
        /// <summary>
        /// Command couldn't run to completion, due to invalid arguments, or conditions
        /// </summary>
        failure = 0,
        /// <summary>
        /// Command could execute, but it didn't change anything
        /// </summary>
        idempotent = 1,
        /// <summary>
        /// Command fully executed, and caused a change.
        /// </summary>
        success = 2
    }

    #endregion
    #region AtomType utils

    /// <summary>
    /// A class of the vanilla atoms, excluding the repeat atom.
    /// </summary>
    public static class VanillaAtoms
    {
        /// <summary>
        /// The salt atom type 🜔
        /// </summary>
        public static readonly AtomType salt = class_175.field_1675;
        /// <summary>
        /// The air atomtype 🜁
        /// </summary>
        public static readonly AtomType air = class_175.field_1676;
        /// <summary>
        /// The earth atomtype 🜃
        /// </summary>
        public static readonly AtomType earth = class_175.field_1677;
        /// <summary>
        /// The fire atomtype 🜂
        /// </summary>
        public static readonly AtomType fire = class_175.field_1678;
        /// <summary>
        /// The water atomtype 🜄
        /// </summary>
        public static readonly AtomType water = class_175.field_1679;
        /// <summary>
        /// The quicksilver atomtype ☿
        /// </summary>
        public static readonly AtomType quicksilver = class_175.field_1680;
        /// <summary>
        /// The lead atomtype ♄
        /// </summary>
        public static readonly AtomType lead = class_175.field_1681;
        /// <summary>
        /// The tin atomtype ♃
        /// </summary>
        public static readonly AtomType tin = class_175.field_1683;
        /// <summary>
        /// The iron atomtype ♂
        /// </summary>
        public static readonly AtomType iron = class_175.field_1684;
        /// <summary>
        /// The copper atomtype ♀
        /// </summary>
        public static readonly AtomType copper = class_175.field_1682;
        /// <summary>
        /// The silver atomtype ☽
        /// </summary>
        public static readonly AtomType silver = class_175.field_1685;
        /// <summary>
        /// The gold atomtype ☉
        /// </summary>
        public static readonly AtomType gold = class_175.field_1686;
        /// <summary>
        /// The vitae atom type 🜍
        /// </summary>
        public static readonly AtomType vitae = class_175.field_1687;
        /// <summary>
        /// The mors atomtype (no unicode character)
        /// </summary>
        public static readonly AtomType mors = class_175.field_1688;
        /// <summary>
        /// The quintessence atomtype 🜀
        /// </summary>
        public static readonly AtomType quintessence = class_175.field_1690;
    };

    /// <summary>
    /// Creates a cardinal atomtype, I recommend you use named parameters with this function.
    /// </summary>
    /// <param name="ID">A positive integer from 0 to 255, the values from 1 to 16 are already used by the game and shouldn't be used.
    /// Please consult other mods that add atoms to avoid ID collisions.</param>
    /// <param name="modName">The name of your mod.</param>
    /// <param name="name">The name of the atom in uppercase.</param>
    /// <param name="pathToBase">A file that leads to a texture of a proxy's bottom.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="pathToFog">A file that leads to a texture of a proxy's center.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="pathToRim">A file that leads to a texture of a proxy's rimlight.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="pathToSymbol">A file that leads to a texture of a proxy's symbol
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="pathToShadow">A file that leads to a texture of a proxy's shadow.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <returns>An AtomType that can be passed to Quintessential.</returns>
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
            QuintAtomType = modName + ":" + name.ToLower().Replace(' ', '_'),
        };
        return atom;
    }

    /// <summary>
    /// Creates a metal atom, that has the option to be promoted, I recommend you to use named parameters with this function.
    /// </summary>
    /// <param name="ID">A positive integer from 0 to 255, the values from 1 to 16 are already used by the game and shouldn't be used.
    /// Please consult other mods that add atoms to avoid ID collisions.</param>
    /// <param name="modName">The name of your mod.</param>
    /// <param name="name">The name of the atom in title case</param>
    /// <param name="pathToSymbol">A file path that leads to a texture of a proxy's symbol.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="pathToLightramp">A file path that leads to a texture of a proxy's lightramp; which is a 256 x 8 gradient
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="pathToShadow">A file path that leads to a texture of a proxy's shadow, defaults to a black shadow.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="pathToRimlight">A file path that leads to a texture of a proxy's rimlight, defaults to the specular highlight.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="promotesTo">The metal above it in the promotion chain, used by the glyph of projection and purification.</param>
    /// <returns>An AtomType that can be passed to Quintessential.</returns>
    public static AtomType CreateMetalAtom(byte ID, string modName, string name, string pathToSymbol, string pathToLightramp, string pathToShadow = "", string pathToRimlight = "", AtomType promotesTo = null)
    {
        return CreateMetalAtom(ID, modName, name, GetTexture(pathToSymbol), GetTexture(pathToLightramp), pathToShadow == "" ? class_238.field_1989.field_81.field_599 : GetTexture(pathToShadow), pathToRimlight == "" ? class_238.field_1989.field_81.field_613.field_634 : GetTexture(pathToRimlight), promotesTo);
    }

    /// <summary>
    /// Creates a metal atom, that has the option to be promoted, I recommend you to use named parameters with this function.
    /// </summary>
    /// <param name="ID">A positive integer from 0 to 255, the values from 1 to 16 are already used by the game and shouldn't be used.
    /// Please consult other mods that add atoms to avoid ID collisions.</param>
    /// <param name="modName">The name of your mod.</param>
    /// <param name="name">The name of the atom in title case.</param>
    /// <param name="symbol">The texture of a proxy's symbol.</param>
    /// <param name="lightramp">The texture of a proxy's lightramp; which is a 256 x 8 gradient.</param>
    /// <param name="shadow">The texture of a proxy's shadow.</param>
    /// <param name="rimlight">The texture of a proxy's rimlight; which rotates towards the engine's lamp.</param>
    /// <param name="promotesTo">The metal above it in the promotion chain, used by the glyph of projection and purification.</param>
    /// <returns>An AtomType that can be passed to Quintessential.</returns>
    public static AtomType CreateMetalAtom(byte ID, string modName, string name, Texture symbol, Texture lightramp, Texture shadow, Texture rimlight, AtomType promotesTo = null)
    {
        AtomType atom = new()
        {
            field_2283 = ID,
            field_2284 = class_134.method_254(name), // Non local name
            field_2285 = class_134.method_253("Elemental " + name, string.Empty), // Atomic name
            field_2286 = class_134.method_253(name, string.Empty), // Local name
            field_2287 = symbol, // Symbol
            field_2288 = shadow, // Shadow
            field_2291 = new()
            {
                field_13 = class_238.field_1989.field_81.field_577, // Diffuse
                field_14 = lightramp,
                field_15 = rimlight
            },
            field_2294 = true, // Metal
            QuintAtomType = modName + ":" + name.ToLower().Replace(' ', '_')
        };
        if (promotesTo is not null)
        {
            atom.field_2297 = Maybe<AtomType>.method_1089(promotesTo);
        }
        return atom;
    }

    /// <summary>
    /// Creates a normal atom, I recommend you to use named parameters with this function.
    /// </summary>
    /// <param name="ID">A positive integer from 0 to 255, the values from 1 to 16 are already used by the game and shouldn't be used.
    /// Please consult other mods that add atoms to avoid ID collisions.</param>
    /// <param name="modName">The name of your mod.</param>
    /// <param name="name">The name of the atom in uppercase.</param>
    /// <param name="pathToSymbol">A file path that leads to a texture of a proxy's symbol.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="pathToDiffuse">A file path that leads to a texture of a proxy' surface.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="pathToShadow">A file path that leads to a texture of a proxy's shadow.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <param name="pathToShade">A file path that leads to a texture of a proxy's shade.
    /// The root is your mod's content folder, don't include the file extension.</param>
    /// <returns>An AtomType that can be passed to Quintessential.</returns>
    public static AtomType CreateNormalAtom(byte ID, string modName, string name, string pathToSymbol, string pathToDiffuse, string pathToShadow = "", string pathToShade = "")
    {
        return CreateNormalAtom(ID, modName, name, GetTexture(pathToSymbol), GetTexture(pathToDiffuse), pathToShadow == "" ? class_238.field_1989.field_81.field_599 : GetTexture(pathToShadow), pathToShade == "" ? class_238.field_1989.field_81.field_597 : GetTexture(pathToShade));
    }

    /// <summary>
    /// Creates a normal atom, I recommend you to use named parameters with this function.
    /// </summary>
    /// <param name="ID">A positive integer from 0 to 255, the values from 1 to 16 are already used by the game and shouldn't be used.
    /// Please consult other mods that add atoms to avoid ID collisions.</param>
    /// <param name="modName">The name of your mod.</param>
    /// <param name="name">The name of the atom in title case.</param>
    /// <param name="symbol">The texture of a proxy's symbol.</param>
    /// <param name="diffuse">The texture of a proxy' surface.</param>
    /// <param name="shadow">The texture of a proxy's shadow.</param>
    /// <param name="shade">The texture of a proxy's shade.</param>
    /// <returns>An AtomType that can be passed to Quintessential.</returns>
    public static AtomType CreateNormalAtom(byte ID, string modName, string name, Texture symbol, Texture diffuse, Texture shadow, Texture shade)
    {
        AtomType atom = new()
        {
            field_2283 = ID,
            field_2284 = class_134.method_254(name), // Non local name
            field_2285 = class_134.method_253("Elemental " + name, String.Empty), // Atomic name
            field_2286 = class_134.method_253(name, String.Empty), // Local name
            field_2287 = symbol,
            field_2288 = shadow,
            field_2290 = new()
            {
                field_994 = diffuse,
                field_995 = shade,
            },
            QuintAtomType = modName + ":" + name.ToLower()
        };
        return atom;
    }

    #endregion
    #region PartType Utils
    /// <summary>
    /// Creates a new part type, pass it into various QApi methods to make it do more interesting thing other than take up space in the heap, I recommend you to use named parameters with this function.
    /// </summary>
    /// <param name="ID">The part's ID</param>
    /// <param name="name">The name displayed in the solution editor</param>
    /// <param name="description">The description displayed in the solution editor</param>
    /// <param name="cost">The amount of guilder the part costs (usually a multiple of 5)</param>
    /// <param name="glow">The glow texture, drawn under the glyph</param>
    /// <param name="stroke">The stroke texture, drawn above the glyph</param>
    /// <param name="icon">The icon shown in the parts tray</param>
    /// <param name="hoveredIcon">The icon shown in the parts tray when hovering with your mouse</param>
    /// <param name="usedHexes">The area taken up by this glyph</param>
    /// <param name="vanillaPerms">The vanilla permissions that need to active. (defaults to none)</param>
    /// <param name="customPermission">The custom permission name that needs to be active. (defaults to none)</param>
    /// <returns>The part with the following configuration</returns>
    public static PartType CreateSimpleGlyph(String ID, String name, String description, int cost, Texture glow, Texture stroke, Texture icon, Texture hoveredIcon, HexIndex[] usedHexes, VanillaPermissions vanillaPerms = VanillaPermissions.None, String customPermission = null)
    {
        PartType p = new()
        {
            field_1528 = ID,
            field_1529 = class_134.method_253(name, string.Empty),
            field_1530 = class_134.method_253(description, string.Empty),
            field_1531 = cost,
            field_1539 = true, // is it a glyph?
            field_1540 = usedHexes,
            field_1547 = icon,
            field_1548 = hoveredIcon,
            field_1549 = glow,
            field_1550 = stroke,
            field_1551 = vanillaPerms,
        };
        if (customPermission is not null)
        {
            p.CustomPermissionCheck = perms => perms.Contains(customPermission);
        }
        return p;
    }


    #endregion
    #region File Utils

    /// <summary>
    /// Packages a set of frames from a folder into an array, to then be played using another function.
    /// </summary>
    /// <param name="containingFolder">The folder that contains the frames, usually has a name ending with .array.
    /// The root is your mod's content folder.</param>
    /// <param name="frameBaseName">The name shared by each frame.
    /// e.g. if your frames were "hello_0000.png", "hello_0001.png", "hello_0002.png" ... this would be "hello".</param>
    /// <param name="frameCount">The number of frames present in this animation.</param>
    /// <param name="padding">The number of digits each frame possesses, this so the frames are in a sensible order when sorted by name</param>
    /// <returns>An array of textures representing your animation, missing frames are replaced with "oh no" texture.</returns>
    public static Texture[] GetAnimation(string containingFolder, string frameBaseName, int frameCount, int padding = 4)
    {
        Texture[] anim = new Texture[frameCount];
        for (int i = 0; i < frameCount; i++)
        {
            anim[i] = GetTexture(Path.Combine(containingFolder, frameBaseName) + "_" + (i + 1).ToString().PadLeft(padding, '0'));
        }
        return anim;
    }

    /// <summary>
    /// Packages a set of lighting textures to be accurately lit by the engine's lamp.<br/>
    /// See also: <seealso cref="class_195.method_527(class_126, HexIndex, Vector2, Vector2, float)"/>
    /// </summary>
    /// <param name="containingFolder">The folder containing the lighting info, usually ending in .lighting</param>
    /// <returns>A wrapper that represents a light map.</returns>
    public static class_126 GetLighting(string containingFolder) => new(GetTexture(Path.Combine(containingFolder, "left")),
                                                                        GetTexture(Path.Combine(containingFolder, "right")),
                                                                        GetTexture(Path.Combine(containingFolder, "bottom")),
                                                                        GetTexture(Path.Combine(containingFolder, "top")));

    /// <summary>
    /// searches the loaded mods for one whose name matches the argument, and gives the path to the content directory.
    /// </summary>
    /// <param name="modName">The name of mod, as given in its' quintessential.yaml file</param>
    /// <returns>An absolute path to the requested mod's content folder.</returns>
    public static Maybe<string> GetContentPath(string modName)
    {
        IEnumerable<ModMeta> mods = QuintessentialLoader.Mods.Where((m) => m.Name == modName);
        return mods.Any() ? (Maybe<string>)Path.Combine(mods.First().PathDirectory, "Content/") : (Maybe<string>)struct_18.field_1431;
    }

    /// <summary>
    /// Searchs the loaded mods for one whose name matches the argument.
    /// </summary>
    /// <param name="modName">The name set in the mod's quintessential.yaml file to find.</param>
    /// <returns>The mod of the desired name.</returns>
    public static Maybe<QuintessentialMod> GetMod(string modName)
    {
        foreach (QuintessentialMod mod in QuintessentialLoader.CodeMods)
        {
            if (mod.Meta.Name == modName)
            {
                return mod;
            }
        }
        return struct_18.field_1431;
    }

    /// <summary>
    /// Retrieves a .wav file for the game to play. Some extra post-processing is required to ensure the game won't crash when playing it.
    /// </summary>
    /// <param name="contentDir">The absolute path of your mod's content directory, use the return value of GetContentPath here.</param>
    /// <param name="path">The path to the .wav file, do not include the file extension.</param>
    /// <returns>A sound object that represents sound file.</returns>
    public static Maybe<Sound> GetSound(string contentDir, string path)
    {
        string soundPath = Path.Combine(contentDir, path + ".wav");
        if (!File.Exists(soundPath))
        {
            return struct_18.field_1431;
        }
        Sound sound = new()
        {
            field_4060 = Path.GetFileNameWithoutExtension(soundPath),
            field_4061 = class_158.method_375(soundPath)
        };
        return sound;
    }

    /// <summary>
    /// Retrieves a texture from a file.
    /// </summary>
    /// <param name="path">The path to the texture file, do not include the file extension.
    /// The root is your mod's content folder.</param>
    /// <returns>A texture object representing the file</returns>
    public static Texture GetTexture(string path = "Quintessential/missing") => class_235.method_615(path);

    #endregion
    #region Misc.

    /// <summary>
    /// Retrieves a method from <typeparamref name="T"/> that is not public.
    /// </summary>
    /// <typeparam name="T">The class of interest.</typeparam>
    /// <param name="method">The method's name.</param>
    /// <returns>An object that represents a method of the class, use the Invoke method to call it.</returns>
    public static MethodInfo PrivateMethod<T>(string method) => typeof(T).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

    /// <summary>
    /// Retrieves a method from <paramref name="T"/> that is not public.
    /// </summary>
    /// <param name="T">The class of interest.</param>
    /// <param name="method">The method's name.</param>
    /// <returns>An object that represents a method of the class, use the Invoke method to call it.</returns>
    public static MethodInfo PrivateMethod(Type T, string method) => T.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

    /// <summary>
    /// Retrieves a field from <typeparamref name="T"/> that is not public
    /// </summary>
    /// <typeparam name="T">The class of interest.</typeparam>
    /// <param name="field">The field's name</param>
    /// <returns>An object that represent a field, use GetField or SetField to... get and set it.</returns>
    public static FieldInfo PrivateField<T>(string field) => typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

    /// <summary>
    /// Retrieves a field from <paramref name="T"/> that is not public
    /// </summary>
    /// <param name="T">The class of interest.</param>
    /// <param name="field">The field's name</param>
    /// <returns>An object that represent a field, use GetField or SetField to... get and set it.</returns>
    public static FieldInfo PrivateField(Type T, string field) => T.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

    /// <summary>
    /// Determines if a mod is loaded, useful for optional dependencies.
    /// Borrowed from Reductive Metallurgy.
    /// </summary>
    /// <param name="modName">The name field in the mod's quintessential.yaml file.</param>
    /// <returns>True if the mod with said name is loaded, and false otherwise.</returns>
    public static bool IsModLoaded(string modName) => QuintessentialLoader.Mods.Any(mod => mod.Name == modName);

    #endregion
    #region Drawing Utils

    /// <summary>
    /// Converts a hex offset into a Vector2
    /// </summary>
    /// <param name="hex">The hex to convert</param>
    /// <returns>The converted Vector</returns>
    public static Vector2 HexIndexToVector2(HexIndex hex) => class_187.field_1742.method_492(hex);


    /// <summary>
    /// Todo: documentation<br />
    /// Stol- I mean borrowed from Reductive Metallurgy, thanks Isaac!
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="tex"></param>
    /// <param name="graphicPivot"></param>
    /// <param name="graphicAngle"></param>
    /// <param name="graphicTranslation"></param>
    /// <param name="screenTranslation"></param>
    public static void DrawPartGraphic(PartRenderer renderer, Texture tex, Vector2 graphicPivot, float graphicAngle, Vector2 graphicTranslation, Vector2 screenTranslation) => DrawPartGraphicScaled(renderer, tex, graphicPivot, graphicAngle, graphicTranslation, screenTranslation, new Vector2(1f, 1f));


    /// <summary>
    /// Todo: documentation<br />
    /// Stol- I mean borrowed from Reductive Metallurgy, thanks Isaac!
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="tex"></param>
    /// <param name="graphicPivot"></param>
    /// <param name="graphicAngle"></param>
    /// <param name="graphicTranslation"></param>
    /// <param name="screenTranslation"></param>
    /// <param name="scaling"></param>
    /// <param name="tint">the color to tint the texture towards</param>
    public static void DrawPartGraphicScaled(PartRenderer renderer, Texture tex, Vector2 graphicPivot, float graphicAngle, Vector2 graphicTranslation, Vector2 screenTranslation, Vector2 scaling, Color tint)
    {
        //for graphicPivot and graphicTranslation, rightwards is the positive-x direction and upwards is the positive-y direction
        //graphicPivot is an absolute position, with (0,0) denoting the bottom-left corner of the texture
        //graphicTranslation is a translation, so (5,-3) means "translate 5 pixels right and 3 pixels down"
        //graphicAngle is measured in radians, and counterclockwise is the positive-angle direction
        //screenTranslation is the final translation applied, so it is not affected by rotations
        Matrix4 matrixScreenPosition = Matrix4.method_1070(renderer.field_1797.ToVector3(0f));
        Matrix4 matrixTranslateOnScreen = Matrix4.method_1070(screenTranslation.ToVector3(0f));
        Matrix4 matrixRotatePart = Matrix4.method_1073(renderer.field_1798);
        Matrix4 matrixTranslateGraphic = Matrix4.method_1070(graphicTranslation.ToVector3(0f));
        Matrix4 matrixRotateGraphic = Matrix4.method_1073(graphicAngle);
        Matrix4 matrixPivotOffset = Matrix4.method_1070(-graphicPivot.ToVector3(0f));
        Matrix4 matrixScaling = Matrix4.method_1074(scaling.ToVector3(0f));
        Matrix4 matrixTextureSize = Matrix4.method_1074(tex.field_2056.ToVector3(0f));

        Matrix4 matrix4 = matrixScreenPosition * matrixTranslateOnScreen * matrixRotatePart * matrixTranslateGraphic * matrixRotateGraphic * matrixPivotOffset * matrixScaling * matrixTextureSize;
        class_135.method_262(tex, tint, matrix4);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="tex"></param>
    /// <param name="graphicPivot"></param>
    /// <param name="graphicAngle"></param>
    /// <param name="graphicTranslation"></param>
    /// <param name="screenTranslation"></param>
    /// <param name="scaling"></param>
    public static void DrawPartGraphicScaled(PartRenderer renderer, Texture tex, Vector2 graphicPivot, float graphicAngle, Vector2 graphicTranslation, Vector2 screenTranslation, Vector2 scaling) => DrawPartGraphicScaled(renderer, tex, graphicPivot, graphicAngle, graphicTranslation, screenTranslation, scaling, Color.White);

    /// <summary>
    /// Draws a texture using mask, mainly used by gloss.<br />
    /// Stol- I mean borrowed from Reductive Metallurgy, thanks Isaac!
    /// </summary>
    /// <param name="renderer">the part context renderer, this is the first parameter of the Quintessental PartRender delegate.</param>
    /// <param name="gloss">The base texture to draw</param>
    /// <param name="mask">The mask to occult the gloss texture, only the alpha channel is used</param>
    /// <param name="offset"></param>
    /// <param name="hex"></param>
    /// <param name="angle"></param>
    public static void DrawGloss(PartRenderer renderer, Texture gloss, Texture mask, Vector2 offset, HexIndex hex, float angle)
    {
        class_135.method_257().field_1692 = class_238.field_1995.field_1757;
        class_135.method_257().field_1693[1] = gloss;
        // this is yoinked from the game, no clue why it does this.
        class_135.method_257().field_1695 = 0.0001f * (renderer.field_1797 + HexIndexToVector2(new HexIndex(0, 0)).Rotated(renderer.field_1798) - (0.5f * class_115.field_1433));
        DrawPartGraphic(renderer, mask, offset, angle, HexIndexToVector2(hex), Vector2.Zero);
        // reset
        class_135.method_257().field_1692 = class_135.method_257().field_1696;
        class_135.method_257().field_1693[1] = class_238.field_1989.field_71;
        class_135.method_257().field_1695 = Vector2.Zero;
    }

    #endregion
    #region Simulation Utils

    /// <summary>
    /// Spawns a new molecule consisting of a single atom.<br />
    /// See also: <see cref="AddSmallCollider(Sim, Part, HexIndex)"/>
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="part">The part that <paramref name="offset"/> is relative to.</param>
    /// <param name="offset">The spawn position.</param>
    /// <param name="atomType">The atom to spawn.</param>
    public static void AddAtom(Sim sim, Part part, HexIndex offset, AtomType atomType)
    {
        Molecule molecule = new();
        molecule.method_1105(new Atom(atomType), part.method_1184(offset));
        sim.field_3823.Add(molecule);
    }

    /// <summary>
    /// Adds a bond in a molecule given 2 offsets.<br />
    /// See also:
    /// <see cref="AddBond(Sim, Molecule, HexIndex, HexIndex, BondType, bool, bool)"/>
    /// <see cref="JoinMoleculesAtHexes(Sim, Part, HexIndex, HexIndex)"/>,
    /// <see cref="RemoveBondsRelative(Sim, Part, HexIndex, HexIndex, bool, bool)"/>
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="part">The part <paramref name="offset1"/> and <paramref name="offset2"/> are relative to</param>
    /// <param name="offset1">Where the bond's "left" end is.
    /// Ensure that this is adjancent to <paramref name="offset2"/>, or an exception will be thrown.</param>
    /// <param name="offset2">Where the bond's "right" end is.
    /// Ensure that this is adjancent to <paramref name="offset1"/>, or an exception will be thrown.</param>
    /// <param name="bt">The bond to create. If this is "None", behaves like <see cref="RemoveBondsRelative(Sim, Part, HexIndex, HexIndex, bool, bool)"/></param>
    /// <param name="playAnimation">Whether or not to play the default animation.</param>
    /// <param name="playSound">Whether or not to play the default sound.</param>
    /// <returns>Failure: The hexes didn't have atoms, or if they were different molecules.
    /// Idempotent: The bond already existed, or the existing bond can't coexist with the new one.
    /// Success: The bond was successfully added.</returns>
    public static SuccessInfo AddBond(Sim sim, Part part, HexIndex offset1, HexIndex offset2, BondType bt, bool playAnimation = true, bool playSound = true)
    {
        HexIndex h1 = part.method_1184(offset1);
        HexIndex h2 = part.method_1184(offset2);
        if (!sim.FindAtom(h1).method_99(out AtomReference atom1) || !sim.FindAtom(h2).method_99(out AtomReference atom2))
        {
            return SuccessInfo.failure;
        }
        Molecule subject = atom1.field_2277;
        return subject == atom2.field_2277 ? bt == BondType.None ? RemoveBonds(sim, subject, h1, h2, playAnimation, playSound) : AddBond(sim, subject, h1, h2, bt, playAnimation, playSound) : SuccessInfo.failure;
    }

    /// <summary>
    /// Adds a bond to a molecule givenn the molecule and absolute positons.
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="subject">The molecule which this bond will be applied</param>
    /// <param name="h1"></param>
    /// <param name="h2"></param>
    /// <param name="bt"></param>
    /// <param name="playAnimation"></param>
    /// <param name="playSound"></param>
    /// <returns></returns>
    public static SuccessInfo AddBond(Sim sim, Molecule subject, HexIndex h1, HexIndex h2, BondType bt, bool playAnimation = true, bool playSound = true)
    {
        class_200 btInfo = bt.method_779();
        BondEffect bondEffect = new(sim.field_3818, (enum_7)1, btInfo.field_1817, 60f, btInfo.field_1818);
        if (!subject.method_1112(btInfo.field_1814, h1, h2, bondEffect))
        {
            return SuccessInfo.idempotent;
        }
        if (playAnimation)
        {
            Vector2 center = class_162.method_413(class_187.field_1742.method_492(h1), class_187.field_1742.method_492(h2), 0.5f);
            sim.field_3818.field_3935.Add(new class_228(sim.field_3818, (enum_7)1, center, btInfo.field_1819, 30f, Vector2.Zero, class_187.field_1742.method_492(h2 - h1).Angle()));
        }
        if (playSound)
        {
            PlaySound(sim, btInfo.field_1820);
        }
        return SuccessInfo.success;
    }

    /// <summary>
    /// Spawns a small atom collider, to represent the top of a proxy emerging from an iris.<br />
    /// See also: <see cref="AddAtom(Sim, Part, HexIndex, AtomType)"/>
    /// Borrowed from TrueAn.
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="part">The part <paramref name="offset"/> is relative to.</param>
    /// <param name="offset">The offset relative to the part's center.</param>
    public static void AddSmallCollider(Sim sim, Part part, HexIndex offset) => sim.field_3826.Add(new()
    {
        field_3850 = (Sim.enum_190)0,
        field_3851 = class_187.field_1742.method_492(part.method_1184(offset)),
        field_3852 = 15f
    });

    /// <summary>
    /// Transmutes an atom into another.
    /// </summary>
    /// <param name="atom">The atom to change.</param>
    /// <param name="newType">What type it will turn into.</param>
    public static void ChangeAtom(AtomReference atom, AtomType newType) => atom.field_2277.method_1106(newType, atom.field_2278);

    /// <summary>
    /// Draws an atom falling into a hole. 
    /// </summary>
    /// <param name="seb">The solution editor base to draw in to.</param>
    /// <param name="hex">The hex the atom appears in</param>
    /// <param name="type">The atom type</param>
    public static void DrawFallingAtom(SolutionEditorBase seb, HexIndex hex, AtomType type) => seb.field_3937.Add(new(seb, hex, type));

    /// <summary>
    /// Draws an atom falling into a hole. 
    /// </summary>
    /// <param name="seb">The solution editor base</param>
    /// <param name="atom">The atom to assume falling (will not be removed)</param>
    public static void DrawFallingAtom(SolutionEditorBase seb, AtomReference atom) => DrawFallingAtom(seb, atom.field_2278, atom.field_2280);

    /// <summary>
    /// Finds the bond(s) present between two hexes.
    /// </summary>
    /// <param name="molecule">The molecule to query.</param>
    /// <param name="h1">The position of the "left" side of a bond.</param>
    /// <param name="h2">The position of the "right" side of a bond.</param>
    /// <returns>The bond(s) that spans these two hexes in the given molecule.</returns>
    public static BondType FindBondType(Molecule molecule, HexIndex h1, HexIndex h2) => molecule.method_1113(h1, h2);

    /// <summary>
    /// Finds the bond(s) present between two hexes
    /// </summary>
    /// <param name="sim">The simulation object</param>
    /// <param name="part">The part <paramref name="offset1"/> and <paramref name="offset2"/> are relative to</param>
    /// <param name="offset1">The position of the "left" side of a bond</param>
    /// <param name="offset2">The position of the "right" side of a bond</param>
    /// <returns>The bond(s) that span these two hexes</returns>
    public static BondType FindBondTypeRelative(Sim sim, Part part, HexIndex offset1, HexIndex offset2)
    {
        HexIndex h1 = part.method_1184(offset1);
        HexIndex h2 = part.method_1184(offset2);
        return !API.FindMolecule(sim, h1).method_99(out Molecule molecule) ? BondType.None : API.FindBondType(molecule, h1, h2);
    }

    /// <summary>
    /// Like <see cref="Sim.FindAtom(HexIndex)"/>, but finds molecules instead.
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="hex">The position to search.</param>
    /// <returns>A molecule with an atom present in the given location.</returns>
    public static Maybe<Molecule> FindMolecule(Sim sim, HexIndex hex)
    {
        return !sim.FindAtom(hex).method_99(out AtomReference atom) ? (Maybe<Molecule>)struct_18.field_1431 : (Maybe<Molecule>)atom.field_2277;
    }

    /// <summary>
    /// Like <see cref="Sim.FindAtomRelative(Part, HexIndex)"/>, but finds molecules instead.
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="part">The part <paramref name="offset"/> is relative to.</param>
    /// <param name="offset">The position to search.</param>
    /// <returns>A molecule with an atom present in the given location.</returns>
    public static Maybe<Molecule> FindMoleculeRelative(Sim sim, Part part, HexIndex offset)
    {
        return API.FindMolecule(sim, part.method_1184(offset));
    }

    /// <summary>
    /// Tells the game to recalcute the given molecule's bond network, and separated the molecule if necessary.
    /// </summary>
    /// <param name="molecule">The molecule to divide.</param>
    public static void ForceRecomputeBonds(Molecule molecule)
    {
        molecule.field_2638 = true;
    }

    /// <summary>
    /// Determines if a hex on a grid axis, think of a rook from chess.
    /// </summary>
    /// <param name="hex">The hex to check, the difference of two hexes is also sensible here.</param>
    /// <returns>Whether a hex is axially aligned.</returns>
    public static bool IsHexAligned(HexIndex hex) => hex.Q == 0 || hex.R == 0 || hex.Q == -hex.R;

    /// <summary>
    /// Causes two separate molecules to become one, and move together as a group.
    /// This method is obsolete, please use <see cref="JoinMoleculesAtHexes(Sim, Part, HexIndex, HexIndex, out Maybe{Molecule})"/>
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="part">The part <paramref name="offset1"/> and <paramref name="offset2"/> are relative to.</param>
    /// <param name="offset1">The position of an atom in one molecule.</param>
    /// <param name="offset2">The position of an atom in a different molecule.</param>
    /// <returns>Failure: A molecule wasn't present in one of the locations.
    /// Idempotent: Both positions where occupied by the same molecule.
    /// Success: The molecules at those two locations are now combined.</returns>
    [Obsolete("The molecules referenced are removed from the board, then a new combined molecule is spawned in, any molecule edits after joining should happen to the new molecule.")]
    public static SuccessInfo JoinMoleculesAtHexes(Sim sim, Part part, HexIndex offset1, HexIndex offset2) => JoinMoleculesAtHexes(sim, part, offset1, offset2, out Maybe<Molecule> _);

    /// <summary>
    /// Causes two separate molecules to become one, and move together as a group.
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="part">The part <paramref name="offset1"/> and <paramref name="offset2"/> are relative to.</param>
    /// <param name="offset1">The position of an atom in one molecule.</param>
    /// <param name="offset2">The position of an atom in a different molecule.</param>
    /// <param name="joined">The joined molecule</param>
    /// <returns>Failure: A molecule wasn't present in one of the locations.
    /// Idempotent: Both positions where occupied by the same molecule.
    /// Success: The molecules at those two locations are now combined.</returns>
    public static SuccessInfo JoinMoleculesAtHexes(Sim sim, Part part, HexIndex offset1, HexIndex offset2, out Maybe<Molecule> joined)
    {
        joined = struct_18.field_1431;
        if (!FindMoleculeRelative(sim, part, offset1).method_99(out Molecule molecule1) || !FindMoleculeRelative(sim, part, offset2).method_99(out Molecule molecule2))
        {
            return SuccessInfo.failure;
        }
        else
        {
            SuccessInfo r = JoinMolecules(sim, molecule1, molecule2, out Molecule joinedO);
            joined = joinedO;
            return r;
        }
    }

    /// <summary>
    /// Causes two separate molecules to become one, and move together as a group.<br/>
    /// This method is obsolete, please use <see cref="JoinMolecules(Sim, Molecule, Molecule, out Molecule)"/>
    /// </summary>
    /// <param name="sim">The simulation object</param>
    /// <param name="molecule1">A molecule to be joined</param>
    /// <param name="molecule2">A molecule to be joined</param>
    /// <returns>Idempotent: If the two molecules are the same.
    /// Success: The molecules are now combined.</returns>
    [Obsolete("The molecules referenced are removed from the board, then a new combined molecule is spawned in, any molecule edits after joining should happen to the new molecule.")]
    public static SuccessInfo JoinMolecules(Sim sim, Molecule molecule1, Molecule molecule2) => JoinMolecules(sim, molecule1, molecule2, out Molecule _);

    /// <summary>
    /// Causes two separate molecules to become one, and move together as a group.
    /// </summary>
    /// <param name="sim">The simulation object</param>
    /// <param name="molecule1">A molecule to be joined</param>
    /// <param name="molecule2">A molecule to be joined</param>
    /// <param name="joined">The joined molecule</param>
    /// <returns>Idempotent: If the two molecules are the same.
    /// Success: The molecules are now combined.</returns>
    public static SuccessInfo JoinMolecules(Sim sim, Molecule molecule1, Molecule molecule2, out Molecule joined)
    {
        if (molecule1 == molecule2)
        {
            joined = molecule1;
            return SuccessInfo.idempotent;
        }
        sim.field_3823.Remove(molecule1);
        sim.field_3823.Remove(molecule2);
        joined = molecule1.method_1119(molecule2);
        sim.field_3823.Add(joined);
        return SuccessInfo.success;
    }

    /// <summary>
    /// Play a loaded sound effect.
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="sound">The sound to play.</param>
    public static void PlaySound(Sim sim, Sound sound) => PrivateMethod<Sim>("method_1856").Invoke(sim, new object[] { sound });

    /// <summary>
    /// Remove an atom from the engine's surface.
    /// </summary>
    /// <param name="atom">The atom to remove.</param>
    public static void RemoveAtom(AtomReference atom) => atom.field_2277.method_1107(atom.field_2278);

    /// <summary>
    /// Removes a bond in a molecule between two hexes.
    /// This can create a "disjoint molecule", call <see cref="API.ForceRecomputeBonds(Molecule)"/> to tell the game to separate it.
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="molecule">The molecule of interest.</param>
    /// <param name="h1">The position of the "left" side of a bond.</param>
    /// <param name="h2">The position of the "right" side of a bond.</param>
    /// <param name="playAnimation">Whether to play the default animation.</param>
    /// <param name="playSound">Whether to play the default sound.</param>
    /// <returns>Idempotent: There was no bond present.
    /// Success: A bond was removed.</returns>
    public static SuccessInfo RemoveBonds(Sim sim, Molecule molecule, HexIndex h1, HexIndex h2, bool playAnimation = true, bool playSound = true)
    {
        List<class_277> moleculeBonds = (List<class_277>)API.PrivateField(molecule.GetType(), "field_2643").GetValue(molecule);

        Predicate<class_277> bondTest = bond => (bond.field_2187 == h1 && bond.field_2188 == h2) || (bond.field_2188 == h1 && bond.field_2187 == h2);

        int count = (int)moleculeBonds.GetType().GetMethod("RemoveAll").Invoke(moleculeBonds, new object[] { bondTest });
        if (count == 0)
        {
            return SuccessInfo.idempotent;
        }
        if (playAnimation)
        {
            BondType bt = API.FindBondType(molecule, h1, h2);
            Vector2 center = class_162.method_413(class_187.field_1742.method_492(h1), class_187.field_1742.method_492(h2), 0.5f);
            class_256[] debondAnimation = ((bt & enum_126.Standard) != enum_126.Standard) ? class_238.field_1989.field_83.field_156 : class_238.field_1989.field_83.field_154;
            sim.field_3818.field_3935.Add(new class_228(sim.field_3818, (enum_7)1, center, debondAnimation, 75f, new Vector2(1.5f, -5f), class_187.field_1742.method_492(h2 - h1).Angle()));
        }
        if (playSound)
        {
            API.PlaySound(sim, class_238.field_1991.field_1849);
        }
        return SuccessInfo.success;

    }

    /// <summary>
    /// Removes a bond in a molecule between two hexes.<br />
    /// This can create a "disjoint molecule", call <see cref="API.ForceRecomputeBonds(Molecule)"/> to tell the game to separate it.
    /// </summary>
    /// <param name="sim">The simulation object.</param>
    /// <param name="part">The part <paramref name="offset1"/> and <paramref name="offset2"/> are relative to.</param>
    /// <param name="offset1">The position of the "left" side of a bond.</param>
    /// <param name="offset2">The position of the "right" side of a bond.</param>
    /// <param name="playAnimation">Whether to play the default animation.</param>
    /// <param name="playSound">Whether to play the default sound.</param>
    /// <returns>Failure: One of the hexes didn't have an atom.
    /// Idempotent: No bond was present to remove.
    /// Success: A bond was removed.</returns>
    public static SuccessInfo RemoveBondsRelative(Sim sim, Part part, HexIndex offset1, HexIndex offset2, bool playAnimation = true, bool playSound = true)
    {
        offset1 = part.method_1184(offset1);
        offset2 = part.method_1184(offset2);
        return !sim.FindAtom(offset1).method_99(out AtomReference atom) || !sim.FindAtom(offset2).method_1085()
            ? SuccessInfo.failure
            : API.RemoveBonds(sim, atom.field_2277, offset1, offset2, playAnimation, playSound);
    }

    /// <summary>
    /// Removes an atom from a molecule, and any bonds attached to it.
    /// This can create a "disjoint molecule", call <see cref="API.ForceRecomputeBonds(Molecule)"/> to tell the game to separate it.
    /// See also: <see cref="RemoveAtom(AtomReference)"/>
    /// </summary>
    /// <param name="molecule">The molecule to cut.</param>
    /// <param name="hex">The position to remove.</param>
    /// <returns>Whether an atom was present.</returns>
    public static bool RemoveHexFromMolecule(Molecule molecule, HexIndex hex)
    {
        // *Zprivate and internal qualifier related anger noises*
        Dictionary<HexIndex, Atom> moleculeAtoms = (Dictionary<HexIndex, Atom>)API.PrivateField(molecule.GetType(), "field_2642").GetValue(molecule);
        if (!(bool)moleculeAtoms.GetType().GetMethod("Remove").Invoke(moleculeAtoms, new object[] { hex }))
        {
            return false;
        }
        List<class_277> moleculeBonds = (List<class_277>)API.PrivateField(molecule.GetType(), "field_2643").GetValue(molecule);
        Predicate<class_277> bondTest = bond => bond.field_2187 == hex || bond.field_2188 == hex;
        moleculeBonds.GetType().GetMethod("RemoveAll").Invoke(moleculeBonds, new object[] { bondTest });
        return true;
    }

    /// <summary>
    /// Removes an atom from a molecule, and any bonds attached to it.
    /// This can create a "disjoint molecule", call <see cref="API.ForceRecomputeBonds(Molecule)"/> to tell the game to separate it.
    /// </summary>
    /// <param name="sim">The simulation object</param>
    /// <param name="part">The part <paramref name="offset"/> is relative to.</param>
    /// <param name="offset">the atom to remove.</param>
    /// <returns>Whether an atom was present.</returns>
    public static bool RemoveHexFromMoleculeRelative(Sim sim, Part part, HexIndex offset)
    {
        HexIndex hex = part.method_1184(offset);
        return !API.FindMolecule(sim, hex).method_99(out Molecule molecule) ? false : API.RemoveHexFromMolecule(molecule, hex);
    }
    #endregion
}