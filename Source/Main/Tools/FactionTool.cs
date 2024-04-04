// Decompiled with JetBrains decompiler
// Type: CharacterEditor.FactionTool
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

internal static class FactionTool
{
    internal static Dictionary<string, Faction> DicFactions => CEditor.API.Get<Dictionary<string, Faction>>(EType.Factions);

    internal static List<Faction> FactionEnemies(this Pawn pawn)
    {
        return pawn == null || Find.FactionManager == null ? new List<Faction>() : Find.FactionManager.AllFactionsInViewOrder.Where(f => f != pawn.Faction && pawn.Faction.RelationWith(f) != null && !pawn.IsFriendlyFaction(f)).ToList();
    }

    internal static Dictionary<string, Faction> GetDicOfFactions(
        bool addPseudoFaction = true,
        bool addCreatures = true,
        bool addHumanoids = true)
    {
        var dic = new Dictionary<string, Faction>();
        var list = Current.Game.World.factionManager.AllFactions.ToList();
        list.SortBy(s => s.Name);
        if (addPseudoFaction)
        {
            dic.Add(Label.HUMANOID, null);
            dic.Add(Label.COLONISTS, Faction.OfPlayer);
            dic.Add(Label.COLONYANIMALS, Faction.OfPlayer);
            if (!CEditor.InStartingScreen)
                dic.Add(Label.WILDANIMALS, null);
        }

        foreach (var f in list)
            if ((addCreatures || !f.IsCreature()) && (addHumanoids || !f.IsHumanlike()))
                dic.AddLabeled((f.GetCallLabel() ?? f.Name).CapitalizeFirst(), f);
        return dic;
    }

    internal static string GetFactionFullDesc(this Pawn pawn, Faction f = null)
    {
        if (pawn == null || pawn.Faction.IsNullOrEmpty())
            return "";
        var str1 = (f == null ? pawn.Faction.Name.CapitalizeFirst() : f.Name.CapitalizeFirst()) + "\n";
        if (f == null && pawn.Faction != Faction.OfPlayer)
            f = Faction.OfPlayer;
        if (f != null)
        {
            var factionRelation = pawn.Faction.RelationWith(f, true);
            if (factionRelation != null)
            {
                var num = 0;
                try
                {
                    num = pawn.Faction.GoodwillWith(f);
                }
                catch
                {
                }

                var str2 = num + " ";
                var kind = factionRelation.kind;
                str1 += (str2 + kind.GetLabel() + "\n").Colorize(kind.GetColor());
            }
        }

        var str3 = str1 + "\n";
        return f != null ? str3 + f.def?.description : str3 + pawn.Faction.def?.description;
    }

    internal static Color GetFacionColor(this Faction f)
    {
        return !f.HasFactionColor() ? Color.white : ColorsFromSpectrum.Get(f.def.colorSpectrum, f.colorFromSpectrum);
    }

    internal static Color GetFacionColor(this Pawn pawn)
    {
        return pawn != null ? pawn.Faction.GetFacionColor() : Color.white;
    }

    internal static bool HasFactionColor(this Faction f)
    {
        return !f.IsNullOrEmpty() && !f.def.colorSpectrum.NullOrEmpty();
    }

    internal static void ChangeFaction(this Pawn p, Faction f)
    {
        if (p == null || f == null || p.Faction == f)
            return;
        p.SetFactionDirect(f);
    }

    internal static bool CanCreateRelations(this Faction f, PawnKindDef pkd)
    {
        if (!f.IsNotInsectXenoMechZombie() || pkd.modContentPack == null)
            return false;
        return pkd.modContentPack.IsCoreMod || pkd.modContentPack.Name == "Royalty";
    }

    internal static bool IsAbomination(this Faction f)
    {
        return !f.IsNullOrEmpty() && f.def.defName == "Abomination";
    }

    internal static bool IsAnimal(this Faction f, string key)
    {
        if (!f.IsNotInsectXenoMechZombie() || !(key != Label.COLONISTS))
            return false;
        return key == Label.COLONYANIMALS || key == Label.WILDANIMALS;
    }

    internal static bool IsCreature(this Faction f)
    {
        return f.IsInsektoid() || f.IsXeno() || f.IsMechanoid() || f.IsAbomination() || f.IsZombie();
    }

    internal static bool IsFriendlyFaction(this Pawn pawn, Faction f)
    {
        if (pawn == null)
            return false;
        return f == pawn.Faction || !f.HostileTo(pawn.Faction);
    }

    internal static bool IsHumanlike(this Faction f)
    {
        return !f.IsNullOrEmpty() && f.def.humanlikeFaction;
    }

    internal static bool IsHumanoid(this Faction f, string key)
    {
        return f.IsNotInsectXenoMechZombie() && key != Label.COLONYANIMALS && key != Label.WILDANIMALS;
    }

    internal static bool IsInsektoid(this Faction f)
    {
        return f == Faction.OfInsects;
    }

    internal static bool IsMechanoid(this Faction f)
    {
        return f == Faction.OfMechanoids;
    }

    internal static bool IsNotInsectXenoMechZombie(this Faction f)
    {
        return !f.IsInsektoid() && !f.IsXeno() && !f.IsMechanoid() && !f.IsAbomination() && !f.IsZombie();
    }

    internal static bool IsNullOrEmpty(this Faction f)
    {
        return f == null || f.def == null;
    }

    internal static bool IsOther(this Faction f, string key)
    {
        return !f.IsNullOrEmpty() && key != Label.COLONISTS && key != Label.COLONYANIMALS;
    }

    internal static bool IsXeno(this Faction f)
    {
        return !f.IsNullOrEmpty() && f.Name.ToLower().Contains("xenomorph");
    }

    internal static bool IsZombie(this Faction f)
    {
        return !f.IsNullOrEmpty() && f.def.defName == "Zombies";
    }

    internal static Faction ThisOrDefault(this Faction f)
    {
        return f ?? GetDicOfFactions(false, false).Values.RandomElement();
    }

    internal static string GetPawnFactionAsSeparatedString(this Pawn p)
    {
        return p == null || p.Faction == null ? Faction.OfPlayer.Name + "|" + Faction.OfPlayer.def.defName : p.Faction.Name + "|" + p.Faction.def.defName;
    }

    internal static Faction GetBySeparatedString(string s, Faction factionFallback)
    {
        if (s.NullOrEmpty())
            return factionFallback;
        Faction faction1 = null;
        var strArray = s.SplitNo("|");
        if (strArray.Length == 2)
        {
            var key = strArray[0];
            var str = strArray[1];
            faction1 = DicFactions.GetValue(key);
            if (faction1 == null)
                foreach (var faction2 in DicFactions.Values)
                    if (faction2 != null && faction2.def.defName == str)
                    {
                        faction1 = faction2;
                        break;
                    }
        }

        return faction1 ?? factionFallback;
    }
}
