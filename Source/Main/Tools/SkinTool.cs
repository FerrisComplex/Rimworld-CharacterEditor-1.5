// Decompiled with JetBrains decompiler
// Type: CharacterEditor.SkinTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class SkinTool
{
    internal static void SetFavColor(this Pawn pawn, Color col)
    {
        if (!pawn.HasStoryTracker())
            return;
        pawn.story.favoriteColor = col;
    }

    internal static Color GetFavColor(this Pawn pawn)
    {
        return !pawn.HasStoryTracker() ? Color.white : pawn.story.favoriteColor.GetValueOrDefault();
    }

    internal static void SetMelanin(this Pawn pawn, float f)
    {
        if ((pawn == null || pawn.story == null ? 1 : f <= 0.0 ? 1 : 0) != 0)
            return;
        if (Prefs.DevMode)
            Log.Message("initialising genes from old save by melanin " + f);
        pawn.genes.InitializeGenesFromOldSave(f);
    }

    internal static Color GetSkinColor(this Pawn p, bool primary)
    {
        return !p.IsAlienRace() ? !p.HasStoryTracker() ? Color.white : p.story.SkinColor : p.AlienRaceComp_GetSkinColor(primary);
    }

    internal static void SetSkinColor(this Pawn p, bool primary, Color color)
    {
        if (p.IsAlienRace())
            p.AlienRaceComp_SetSkinColor(primary, color);
        else if (p.HasStoryTracker())
            p.story.skinColorOverride = color;
        p?.Drawer?.renderer?.SetAllGraphicsDirty();
    }
}