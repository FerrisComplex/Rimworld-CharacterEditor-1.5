// Decompiled with JetBrains decompiler
// Type: CharacterEditor.AlienRaceTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class AlienRaceTool
{
    internal static string BodyAddon_GetPath(object bodyAddon)
    {
        return bodyAddon == null ? "" : bodyAddon.GetMemberValue("path", "");
    }

    internal static int BodyAddon_GetVariantCountMax(object bodyAddon)
    {
        return bodyAddon == null ? 0 : bodyAddon.GetMemberValue("variantCountMax", 0);
    }

    internal static bool BodyAddon_GetDrawForFemale(object bodyAddon)
    {
        return bodyAddon != null && bodyAddon.GetMemberValue("drawForFemale", false);
    }

    internal static bool BodyAddon_GetDrawForMale(object bodyAddon)
    {
        return bodyAddon != null && bodyAddon.GetMemberValue("drawForMale", false);
    }

    internal static Vector2 BodyAddon_GetDrawSize(object bodyAddon)
    {
        return bodyAddon == null ? Vector2.one : bodyAddon.GetMemberValue("drawSize", Vector2.one);
    }

    internal static float BodyAddon_GetRotation(object bodyAddon)
    {
        return bodyAddon == null ? 0.0f : bodyAddon.GetMemberValue("angle", 0.0f);
    }

    internal static object BodyAddon_GetOffsets(object bodyAddon)
    {
        return bodyAddon == null ? null : bodyAddon.GetMemberValue<object>("offsets", null);
    }

    internal static object BodyAddon_GetRoationOffsetSouth(object bodyAddon)
    {
        return BodyAddon_GetOffsets(bodyAddon).GetMemberValue<object>("south", null);
    }

    internal static object BodyAddon_GetRoationOffsetNorth(object bodyAddon)
    {
        return BodyAddon_GetOffsets(bodyAddon).GetMemberValue<object>("north", null);
    }

    internal static object BodyAddon_GetRoationOffsetEast(object bodyAddon)
    {
        return BodyAddon_GetOffsets(bodyAddon).GetMemberValue<object>("east", null);
    }

    internal static object BodyAddon_GetRoationOffsetWest(object bodyAddon)
    {
        return BodyAddon_GetOffsets(bodyAddon).GetMemberValue<object>("west", null);
    }

    internal static float BodyAddon_GetLayerOffset(object bodyAddonRotationOffset)
    {
        return bodyAddonRotationOffset == null ? 0.0f : bodyAddonRotationOffset.GetMemberValue("layerOffset", 0.0f);
    }

    internal static void AlienPartGenerator_BodyAddon_Toggle_DrawFor(
        this Pawn p,
        int index,
        bool female)
    {
        var bodyAddonAtIndex = p.AlienPartGenerator_GetBodyAddonAtIndex(index);
        var flag = !bodyAddonAtIndex.GetMemberValue(female ? "drawForFemale" : "drawForMale", false);
        bodyAddonAtIndex.SetMemberValue(female ? "drawForFemale" : "drawForMale", flag);
    }

    internal static void BodyAddon_SetDrawForFemale(object bodyAddon, bool val)
    {
        bodyAddon.SetMemberValue("drawForFemale", val);
    }

    internal static void BodyAddon_SetDrawForMale(object bodyAddon, bool val)
    {
        bodyAddon.SetMemberValue("drawForMale", val);
    }

    internal static void BodyAddon_SetDrawSize(object bodyAddon, float val)
    {
        bodyAddon.SetMemberValue("drawSize", new Vector2(val, val));
    }

    internal static void BodyAddon_SetRotation(object bodyAddon, float val)
    {
        bodyAddon.SetMemberValue("angle", val);
    }

    internal static void BodyAddon_SetLayerOffset(object bodyAddonRotationOffset, float val)
    {
        bodyAddonRotationOffset.SetMemberValue("layerOffset", val);
    }

    internal static bool IsAlienRace(this Pawn pawn)
    {
        return CEditor.IsAlienRaceActive && pawn != null && pawn.def != null && pawn.def.GetType().ToString().StartsWith("AlienRace");
    }

    internal static object AlienRace(this Pawn pawn)
    {
        return !pawn.IsAlienRace() ? null : pawn.def.GetMemberValue<object>("alienRace", null);
    }

    internal static object AlienRace_GetGeneralSettings(this Pawn pawn)
    {
        return pawn.AlienRace().GetMemberValue<object>("generalSettings", null);
    }

    internal static object AlienPartGenerator_GetBodyAddons(this Pawn p)
    {
        return p.AlienPartGenerator().GetMemberValue<object>("bodyAddons", null);
    }

    internal static object[] AlienPartGenerator_GetBodyAddonsAsArray(this Pawn p)
    {
        return (object[])p.AlienPartGenerator_GetBodyAddons().CallMethod("ToArray", null);
    }

    internal static object AlienPartGenerator_GetBodyAddonAtIndex(this Pawn p, int index)
    {
        var bodyAddonsAsArray = p.AlienPartGenerator_GetBodyAddonsAsArray();
        return bodyAddonsAsArray == null || bodyAddonsAsArray.Length < index ? null : bodyAddonsAsArray[index];
    }

    internal static object AlienPartGenerator(this Pawn pawn)
    {
        return pawn.AlienRace_GetGeneralSettings().GetMemberValue<object>("alienPartGenerator", null);
    }

    internal static Vector2 AlienPartGenerator_GetCustomDrawSize(this Pawn p)
    {
        return p.AlienPartGenerator().GetMemberValue("customDrawSize", Vector2.one);
    }

    internal static List<HeadTypeDef> AlienPartGenerator_GetHeadTypes(this Pawn p)
    {
        return p.AlienPartGenerator().GetMemberValue("headTypes", (List<HeadTypeDef>)null);
    }

    internal static List<BodyTypeDef> AlienPartGenerator_GetBodyTypes(this Pawn p)
    {
        return p.AlienPartGenerator().GetMemberValue("bodyTypes", (List<BodyTypeDef>)null);
    }

    internal static void AlienPartGenerator_SetCustomDrawSize(this Pawn p, Vector2 val)
    {
        p.AlienPartGenerator().SetMemberValue("customDrawSize", val);
    }

    internal static void AlienPartGenerator_DeleteAllAddons(this Pawn p)
    {
        var bodyAddons = p.AlienPartGenerator_GetBodyAddons();
        bodyAddons.CallMethod("Clear", null);
        p.AlienPartGenerator().SetMemberValue("bodyAddons", bodyAddons);
    }

    internal static ThingComp AlienRace_GetAlienRaceComp(this Pawn pawn)
    {
        if (pawn.AlienRace() == null)
            return null;
        foreach (var comp in pawn.GetComps<ThingComp>())
            if (comp.GetType().Namespace.EqualsIgnoreCase("AlienRace") && comp.GetType().Name.EndsWith("AlienComp"))
                return comp;
        return null;
    }

    internal static Vector2 AlienRaceComp_GetCustomDrawSize(this Pawn p)
    {
        return p.AlienRace_GetAlienRaceComp().GetMemberValue("customDrawSize", Vector2.one);
    }

    internal static object AlienRaceComp_GetChannel(this Pawn p, string channelName)
    {
        return p.AlienRace_GetAlienRaceComp().CallMethod("GetChannel", new object[1]
        {
            channelName
        });
    }

    internal static Color AlienRaceComp_GetChannelColor(
        this Pawn p,
        string channelName,
        bool primary)
    {
        return p.AlienRaceComp_GetChannel(channelName).GetMemberValue(primary ? "first" : "second", Color.white);
    }

    internal static Color AlienRaceComp_GetSkinColor(this Pawn p, bool primary)
    {
        return p.AlienRaceComp_GetChannelColor("skin", primary);
    }

    internal static List<Graphic> AlienRaceComp_GetAddonGraphics(this Pawn p)
    {
        return p.AlienRace_GetAlienRaceComp().GetMemberValue("addonGraphics", (List<Graphic>)null);
    }

    internal static List<int> AlienRaceComp_GetAddonVariants(this Pawn p)
    {
        return p.AlienRace_GetAlienRaceComp().GetMemberValue("addonVariants", (List<int>)null);
    }

    internal static void AlienRaceComp_SetCustomDrawSize(this Pawn p, Vector2 val)
    {
        p.AlienRace_GetAlienRaceComp().SetMemberValue("customDrawSize", val);
    }

    internal static void AlienRaceComp_SetChannelColor(
        this Pawn p,
        string channelName,
        bool primary,
        Color val)
    {
        p.AlienRaceComp_GetChannel(channelName).SetMemberValue(primary ? "first" : "second", val);
    }

    internal static void AlienRaceComp_SetSkinColor(this Pawn p, bool primary, Color val)
    {
        p.AlienRaceComp_SetChannelColor("skin", primary, val);
    }

    internal static void AlienRaceComp_SetHairColor(this Pawn p, bool primary, Color val)
    {
        p.AlienRaceComp_SetChannelColor("hair", primary, val);
    }

    internal static void AlienRaceComp_SetAddonVariants(this Pawn p, List<int> l)
    {
        p.AlienRace_GetAlienRaceComp().SetMemberValue("addonVariants", l);
    }

    internal static void AlienRaceComp_ClearAllAddons(this Pawn p)
    {
        p.AlienRace_GetAlienRaceComp().SetMemberValue("addonVariants", new List<int>());
        p.AlienRace_GetAlienRaceComp().SetMemberValue("addonGraphics", new List<Graphic>());
    }
}