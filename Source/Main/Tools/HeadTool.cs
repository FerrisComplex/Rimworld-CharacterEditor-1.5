// Decompiled with JetBrains decompiler
// Type: CharacterEditor.HeadTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class HeadTool
{
    internal static string GetAllHeadAddonsAsSeparatedString(this Pawn p)
    {
        string str;
        if (!p.IsAlienRace())
        {
            str = "";
        }
        else
        {
            var addonVariants = p.AlienRaceComp_GetAddonVariants();
            var text = "";
            if (!addonVariants.NullOrEmpty())
            {
                foreach (var num in addonVariants)
                {
                    text += num.ToString();
                    text += ":";
                }

                text = text.SubstringRemoveLast();
            }

            str = text;
        }

        return str;
    }

    internal static void SetHeadAddonsFromSeparatedString(this Pawn p, string s)
    {
        if ((s.NullOrEmpty() ? 1 : !p.IsAlienRace() ? 1 : 0) != 0)
            return;
        var strArray = s.SplitNo(":");
        var l = new List<int>();
        if ((uint)strArray.Length > 0U)
            foreach (var input in strArray)
                l.Add(input.AsInt32());
        p.AlienRaceComp_SetAddonVariants(l);
    }

    internal static void GetHeadPathFolderFor(
        this Pawn pawn,
        out string pathGenderized,
        out string pathNonGenderized)
    {
        pathGenderized = "";
        pathNonGenderized = "";
        if ((!pawn.HasStoryTracker() ? 1 : pawn.story.headType == null ? 1 : 0) != 0)
            return;
        var text1 = pawn.story.headType.graphicPath ?? "";
        if (Prefs.DevMode)
            Log.Message("head graphic path=" + text1);
        var text2 = text1.SubstringBackwardTo("/");
        if (Prefs.DevMode)
            Log.Message("heads path=" + text2);
        var gender = pawn.gender;
        if (text2.StartsWith("Things/Pawn/Humanlike/Heads"))
        {
            pathNonGenderized = "Things/Pawn/Humanlike/Heads/";
            if (gender == Gender.Female)
                pathGenderized = text2.Replace("Male", "Female");
            else if (gender == Gender.Male)
                pathGenderized = text2.Replace("Female", "Male");
        }
        else if ((text2.Contains("/Male") ? 1 : text2.Contains("/Female") ? 1 : 0) != 0)
        {
            pathNonGenderized = text2.Contains("/Male") ? text2.SubstringBackwardTo("/Male") : text2.SubstringBackwardTo("/Female");
            if (gender == Gender.Female)
                pathGenderized = text2.Replace("/Male", "/Female");
            else if (gender == Gender.Male)
                pathGenderized = text2.Replace("/Female", "/Male");
        }

        if (!Prefs.DevMode)
            return;
        Log.Message("genderized  subpath=" + pathGenderized);
        Log.Message("non-genderized path=" + pathNonGenderized);
    }

    internal static HashSet<HeadTypeDef> ReduceListByGender(
        Pawn pawn,
        HashSet<HeadTypeDef> l)
    {
        HashSet<HeadTypeDef> headTypeDefSet1;
        if ((!pawn.HasStoryTracker() ? 1 : l.NullOrEmpty() ? 1 : 0) != 0)
        {
            headTypeDefSet1 = new HashSet<HeadTypeDef>();
        }
        else
        {
            if (Prefs.DevMode)
                Log.Message("trying alternative method");
            var headTypeDefSet2 = new HashSet<HeadTypeDef>();
            var str1 = pawn.gender == Gender.Female ? "Female" : "Male";
            foreach (var headTypeDef in l)
                if (headTypeDef.graphicPath.Contains(str1))
                    headTypeDefSet2.Add(headTypeDef);
            if (Prefs.DevMode)
            {
                var num = l.Count();
                var str2 = num.ToString();
                num = headTypeDefSet2.Count();
                var str3 = num.ToString();
                Log.Message("heads in list=" + str2 + " matching heads=" + str3);
            }

            headTypeDefSet1 = headTypeDefSet2;
        }

        return headTypeDefSet1;
    }

    internal static HashSet<HeadTypeDef> GetTestedHeadList(
        Pawn pawn,
        HashSet<HeadTypeDef> l,
        bool skipSkull = false)
    {
        HashSet<HeadTypeDef> headTypeDefSet1;
        if ((!pawn.HasStoryTracker() ? 1 : l.NullOrEmpty() ? 1 : 0) != 0)
        {
            headTypeDefSet1 = new HashSet<HeadTypeDef>();
        }
        else
        {
            var headTypeDefSet2 = new HashSet<HeadTypeDef>();
            foreach (var headTypeDef in l)
                if (pawn.TestHead(headTypeDef.graphicPath) && (!skipSkull ? 0 : headTypeDef.graphicPath.Contains("Skull") ? 1 : headTypeDef.graphicPath.Contains("Stump") ? 1 : 0) == 0)
                    headTypeDefSet2.Add(headTypeDef);
            if (Prefs.DevMode)
            {
                var num = l.Count();
                var str1 = num.ToString();
                num = headTypeDefSet2.Count();
                var str2 = num.ToString();
                Log.Message("heads in list=" + str1 + " matching heads=" + str2);
            }

            headTypeDefSet1 = headTypeDefSet2;
        }

        return headTypeDefSet1;
    }

    internal static bool IsCoreHeadPath(string s)
    {
        return s == "Things/Pawn/Humanlike/Heads/" || s == "Things/Pawn/Humanlike/Heads";
    }

    internal static HashSet<HeadTypeDef> GetHeadDefList(
        this Pawn pawn,
        bool genderized = false)
    {
        HashSet<HeadTypeDef> headTypeDefSet1;
        if (!pawn.HasStoryTracker())
        {
            headTypeDefSet1 = null;
        }
        else
        {
            if ((pawn.kindDef.IsFromCoreMod() ? 0 : pawn.IsAlienRace() ? 1 : 0) != 0)
            {
                var headTypes = pawn.AlienPartGenerator_GetHeadTypes();
                var testedHeadList = GetTestedHeadList(pawn, headTypes.ToHashSet());
                if (testedHeadList.Count > 0)
                    return testedHeadList;
                if (headTypes.Count > 0)
                {
                    var headTypeDefSet2 = ReduceListByGender(pawn, headTypes.ToHashSet());
                    return headTypeDefSet2.Count > 0 ? headTypeDefSet2 : headTypes.ToHashSet();
                }
            }

            string pathG;
            string pathNG;
            pawn.GetHeadPathFolderFor(out pathG, out pathNG);
            var skipSkull = IsCoreHeadPath(pathNG) && CEditor.IsAlienRaceActive;
            var l1 = DefTool.ListBy((Func<HeadTypeDef, bool>)(x => x.graphicPath.Contains(pathG)));
            var l2 = DefTool.ListBy((Func<HeadTypeDef, bool>)(x => x.graphicPath.Contains(pathNG)));
            var testedHeadList1 = GetTestedHeadList(pawn, l1, skipSkull);
            var testedHeadList2 = GetTestedHeadList(pawn, l2, skipSkull);
            headTypeDefSet1 = testedHeadList2.Count <= 0 ? testedHeadList1.Count <= 0 ? l1.Count <= 0 ? l2.Count <= 0 ? DefTool.ListBy((Func<HeadTypeDef, bool>)(x => x.modContentPack == pawn.kindDef.modContentPack)) : l2 : l1 : testedHeadList1 : testedHeadList2;
        }

        return headTypeDefSet1;
    }

    internal static string GetHeadName(this Pawn pawn, string path = null)
    {
        var str1 = "";
        string str2;
        if (pawn == null)
        {
            str2 = "";
        }
        else
        {
            string str3;
            if ((str3 = path) == null)
                str3 = pawn != null ? (pawn.story?.headType).graphicPath : null;
            path = str3;
            path = path ?? "";
            if (path.Contains("Female"))
                str1 += "♀ ";
            else if (path.Contains("Male"))
                str1 += "♂ ";
            if (path.Contains("Average"))
                str1 += "Average";
            else if (path.Contains("Narrow"))
                str1 += "Narrow";
            var text = !path.Contains("Wide") ? !path.Contains("Pointy") ? !path.Contains("Normal") ? str1 + (path.EndsWith("Average") || path.EndsWith("Narrow") ? "" : path.SubstringBackwardFrom("_", false)) : str1 + " Normal" : str1 + " Pointy" : str1 + " Wide";
            if (text.Contains("/"))
                text = text.SubstringBackwardFrom("/");
            str2 = text;
        }

        return str2;
    }

    internal static string GetHeadTypeDefName(this Pawn p)
    {
        return p == null || p.story == null || p.story.headType == null ? "" : p.story.headType.defName;
    }

    internal static void SetHeadTypeDef(this Pawn p, string defName)
    {
        if ((p == null || p.story == null ? 0 : !defName.NullOrEmpty() ? 1 : 0) == 0)
            return;
        p.SetHeadTypeDef(DefTool.GetDef<HeadTypeDef>(defName));
    }

    internal static void SetHeadTypeDef(this Pawn p, HeadTypeDef def)
    {
        if ((!p.HasStoryTracker() ? 1 : def == null ? 1 : 0) != 0)
            return;
        p.story.headType = def;
        p?.Drawer?.renderer?.SetAllGraphicsDirty();
    }

    internal static bool SetHead(this Pawn pawn, bool next, bool random)
    {
        bool result;
        if (pawn == null)
        {
            result = false;
        }
        else
        {
            HashSet<HeadTypeDef> headDefList = pawn.GetHeadDefList(false);
            HeadTypeDef headType = pawn.story.headType;
            int index = headDefList.IndexOf(headType);
            index = headDefList.NextOrPrevIndex(index, next, random);
            HeadTypeDef headTypeDef = headDefList.ElementAt(index);
            if (Prefs.DevMode)
            {
                MessageTool.Show(headTypeDef.defName + " " + headTypeDef.modContentPack.Name, null);
            }
            pawn.SetHeadTypeDef(headTypeDef);
            result = true;
        }
        return result;
    }

    internal static bool TestHead(this Pawn pawn, string headPath)
    {
        bool result;
        if (pawn == null || headPath.NullOrEmpty())
        {
            result = false;
        }
        else
        {
            string text = headPath.Replace("\\", "/");
            if (Prefs.DevMode)
            {
                Log.Message("testing head path=" + text);
            }
            bool flag = TextureTool.TestTexturePath(text + "_south", false);
            if (!flag)
            {
                result = false;
            }
            else
            {
                if (pawn.IsAlienRace())
                {
                    bool flag2 = text.ToLower().Contains("/female");
                    bool flag3 = text.ToLower().Contains("/male");
                    if (flag2 || flag3)
                    {
                        flag = ((pawn.gender == Gender.Female && flag2) || (pawn.gender == Gender.Male && flag3));
                    }
                    if (!flag2 && !flag3)
                    {
                        flag = true;
                    }
                }
                result = flag;
            }
        }
        return result;
    }

    internal static Color GetEyeColor(this Pawn p)
    {
        var controllerComp = p.FA_GetControllerComp(FacialTool.EYE);
        return controllerComp == null ? Color.white : controllerComp.GetMemberValue("color", Color.white);
    }

    internal static void SetEyeColor(this Pawn p, Color col)
    {
        var controllerComp = p.FA_GetControllerComp(FacialTool.EYE);
        if (controllerComp == null)
            return;
        controllerComp.SetMemberValue("color", col);
        p?.Drawer?.renderer?.SetAllGraphicsDirty();
        CEditor.API.UpdateGraphics();
    }
}
