// Decompiled with JetBrains decompiler
// Type: CharacterEditor.HairTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class HairTool
{
    internal static string selectedHairModName;
    internal static HashSet<HairDef> lOfHairDefs;
    internal static bool isHairConfigOpen = true;
    internal static bool onMouseover = false;

    internal static Color GetHairColor(this Pawn p, bool primary)
    {
        if (!p.HasStoryTracker())
            return Color.white;
        return !primary ? p.GetGradientColor() : p.story.HairColor;
    }

    internal static void SetGradientMask(this Pawn p, string mask)
    {
        if ((p == null ? 1 : mask.NullOrEmpty() ? 1 : 0) != 0)
            return;
        var gradientComp = p.GetGradientComp();
        if (gradientComp == null)
            return;
        var memberValue = gradientComp.GetMemberValue<object>("settings", null);
        if (memberValue != null)
        {
            memberValue.SetMemberValue(nameof(mask), mask);
            memberValue.SetMemberValue("enabled", true);
        }

        p?.Drawer?.renderer?.SetAllGraphicsDirty();
        CEditor.API.UpdateGraphics();
    }

    internal static ThingComp GetGradientComp(this Pawn p)
    {
        ThingComp thingComp;
        if ((p == null || p.AllComps.NullOrEmpty() ? 1 : !CEditor.IsGradientHairActive ? 1 : 0) != 0)
        {
            thingComp = null;
        }
        else
        {
            foreach (var allComp in p.AllComps)
                if (allComp.GetType().ToString().EndsWith("CompGradientHair"))
                    return allComp;
            thingComp = null;
        }

        return thingComp;
    }

    internal static List<string> GetAllGradientHairs()
    {
        var list = ContentFinder<Texture2D>.GetAllInFolder("GradientHair").ToList();
        var texture2DSet = new HashSet<Texture2D>();
        var stringSet = new HashSet<string>();
        foreach (var texture2D in list)
            if (!stringSet.Contains(texture2D.name))
            {
                texture2DSet.Add(texture2D);
                stringSet.Add(texture2D.name);
            }

        return texture2DSet.OrderBy(t => t.name).Select(t => "GradientHair/" + t.name).ToList();
    }

    internal static void RandomizeGradientMask(this Pawn p)
    {
        if (p == null)
            return;
        var mask = GetAllGradientHairs().RandomElement();
        p.SetGradientMask(mask);
    }

    internal static string GetGradientMask(this Pawn p)
    {
        string str;
        if (p == null)
        {
            str = "";
        }
        else
        {
            var gradientComp = p.GetGradientComp();
            if (gradientComp != null)
            {
                var memberValue = gradientComp.GetMemberValue<object>("settings", null);
                str = memberValue == null ? "" : memberValue.GetMemberValue("mask", "");
            }
            else
            {
                str = "";
            }
        }

        return str;
    }

    private static Color GetGradientColor(this Pawn p)
    {
        Color white;
        if (CEditor.IsGradientHairActive)
        {
            var objArray = new object[3]
            {
                p,
                null,
                null
            };
            Reflect.GetAType("GradientHair", "PublicApi").CallMethod("GetGradientHair", objArray);
            white = (Color)objArray[2];
        }
        else
        {
            white = Color.white;
        }

        return white;
    }

    internal static string GetHairDefName(this Pawn p)
    {
        return !p.HasStoryTracker() || p.story.hairDef == null ? "" : p.story.hairDef.defName;
    }

    internal static string GetHairName(this Pawn p)
    {
        return !p.HasStoryTracker() || p.story.hairDef == null ? "" : p.story.hairDef.LabelCap.ToString();
    }

    internal static HashSet<HairDef> GetHairList(string modname)
    {
        return DefTool.ListByMod<HairDef>(modname).OrderBy(hr => !hr.noGraphic).ToHashSet();
    }

    internal static int GetHairDefCount(string modname)
    {
        return DefTool.ListByMod<HairDef>(modname).ToList().CountAllowNull();
    }

    internal static HairDef GetRandomHairDef(string modname)
    {
        return DefTool.ListByMod<HairDef>(modname).ToList().RandomElement();
    }

    internal static void SetHair(this Pawn p, HairDef h)
    {
        if ((!p.HasStoryTracker() ? 1 : h == null ? 1 : 0) != 0)
            return;
        p.story.hairDef = h;
        p?.Drawer?.renderer?.SetAllGraphicsDirty();
    }

    internal static void SetHair(this Pawn pawn, bool next, bool random, string modname = null)
    {
        if ((pawn == null ? 1 : pawn.story == null ? 1 : 0) != 0)
            return;
        var list = DefTool.ListByMod<HairDef>(modname).ToList();
        var index1 = list.IndexOf(pawn.story.hairDef);
        var index2 = list.NextOrPrevIndex(index1, next, random);
        pawn.SetHair(list[index2]);
    }

    internal static void SetHairColor(this Pawn p, bool primary, Color col)
    {
        if (!p.HasStoryTracker())
            return;
        if (primary)
            p.story.SetMemberValue("hairColor", col);
        else
            p.SetGradientHairColor(col);
        if (p.IsAlienRace())
            p.AlienRaceComp_SetHairColor(primary, col);
        p?.Drawer?.renderer?.SetAllGraphicsDirty();
    }

    private static void SetGradientHairColor(this Pawn pawn, Color colorB)
    {
        if (!CEditor.IsGradientHairActive)
            return;
        var objArray = new object[3]
        {
            pawn,
            true,
            colorB
        };
        Reflect.GetAType("GradientHair", "PublicApi").CallMethod("SetGradientHair", objArray);
    }

    internal static void AChooseHairCustom()
    {
        SZWidgets.FloatMenuOnRect(GetHairList(null), s => new TaggedString(s.LabelCap), ASetHairCustom);
    }

    internal static void ASetHairCustom(HairDef hairDef)
    {
        CEditor.API.Pawn.SetHair(hairDef);
        CEditor.API.UpdateGraphics();
    }

    internal static void ARandomHair()
    {
        CEditor.API.Pawn.SetHair(true, true, selectedHairModName);
        if (Event.current.alt)
        {
            CEditor.API.Pawn.SetHairColor(true, ColorTool.RandomColor);
            CEditor.API.Pawn.SetHairColor(false, ColorTool.RandomColor);
        }
        else if (Event.current.control)
        {
            CEditor.API.Pawn.SetHairColor(true, ColorTool.RandomAlphaColor);
            CEditor.API.Pawn.SetHairColor(false, ColorTool.RandomAlphaColor);
        }

        CEditor.API.UpdateGraphics();
    }

    internal static void ASelectedHairModName(string val)
    {
        selectedHairModName = val;
        lOfHairDefs = GetHairList(selectedHairModName);
    }

    internal static void AConfigHair()
    {
        isHairConfigOpen = !isHairConfigOpen;
    }
}
