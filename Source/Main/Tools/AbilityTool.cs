// Decompiled with JetBrains decompiler
// Type: CharacterEditor.AbilityTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class AbilityTool
{
    internal static List<Ability> GetCopyOfAbilites(this Pawn p)
    {
        if (!p.HasAbilityTracker() || p.abilities.abilities.NullOrEmpty())
            return new List<Ability>();
        var array = new Ability[p.abilities.abilities.Count];
        p.abilities.abilities.CopyTo(array);
        return array.ToList();
    }

    internal static void RemoveTemporaryAbilities(this Pawn p, List<Ability> copyOriginalAbilities)
    {
        if (!p.HasAbilityTracker())
            return;
        for (var index = p.abilities.abilities.Count - 1; index >= 0; --index)
            if (copyOriginalAbilities.NullOrEmpty() || !copyOriginalAbilities.Contains(p.abilities.abilities[index]))
                p.abilities.abilities.Remove(p.abilities.abilities[index]);
    }

    internal static void CheckAddPsylink(this Pawn p, int level = -1)
    {
        if (!ModsConfig.RoyaltyActive || !p.HasPsyTracker())
            return;
        var mainPsylinkSource1 = p.GetMainPsylinkSource();
        var flag = mainPsylinkSource1 == null;
        if (mainPsylinkSource1 == null)
        {
            var copyOfAbilites = p.GetCopyOfAbilites();
            p.ChangePsylinkLevel(level > 0 ? level : 1);
            p.RemoveTemporaryAbilities(copyOfAbilites);
            p.psychicEntropy.Notify_GainedPsylink();
            p.psychicEntropy.PsychicEntropyTrackerTick();
        }
        else if (level >= 0)
        {
            p.ChangePsylinkLevel(level - mainPsylinkSource1.level);
        }

        var mainPsylinkSource2 = p.GetMainPsylinkSource();
        if (mainPsylinkSource2 == null)
            return;
        if (level > 0 && mainPsylinkSource2.level != level)
        {
            mainPsylinkSource2.level = level;
            mainPsylinkSource2.Severity = level;
        }

        p.health.Notify_HediffChanged(mainPsylinkSource2);
    }

    internal static float GetPsyfocus(this Pawn p)
    {
        return !p.HasPsylink ? 0.0f : p.psychicEntropy.CurrentPsyfocus;
    }

    internal static float GetEntropy(this Pawn p)
    {
        return !p.HasPsylink ? 0.0f : p.psychicEntropy.EntropyValue;
    }

    internal static void SetEntropy(this Pawn p, float val)
    {
        if (!p.HasPsylink)
            return;
        p.psychicEntropy.SetMemberValue("currentEntropy", val);
    }

    internal static void SetPsyfocus(this Pawn p, float val)
    {
        if (!p.HasPsylink)
            return;
        p.psychicEntropy.SetMemberValue("currentPsyfocus", val);
    }

    internal static string GetPsyAbilitiesAsSeparatedString(this Pawn p)
    {
        if (!p.HasAbilityTracker())
            return "";
        var text = "";
        foreach (var ability in p.abilities.abilities)
            text = text + ability.def.defName + ":";
        return text.SubstringRemoveLast();
    }

    internal static void SetPsyAbilitiesFromSeparatedString(this Pawn p, string s)
    {
        if (!p.HasAbilityTracker())
            return;
        var str = s;
        var separator = new string[1] { ":" };
        foreach (var defName in str.Split(separator, StringSplitOptions.None))
        {
            var abilityDef = DefTool.AbilityDef(defName);
            if (abilityDef != null)
                p.abilities.GainAbility(abilityDef);
        }

        p.abilities.Notify_TemporaryAbilitiesChanged();
    }
}
