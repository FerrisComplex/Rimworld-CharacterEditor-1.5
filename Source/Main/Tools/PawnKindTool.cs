// Decompiled with JetBrains decompiler
// Type: CharacterEditor.PawnKindTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class PawnKindTool
{
    internal static string GetPawnKindDefname(this Pawn p)
    {
        return p == null || p.kindDef == null ? "" : p.kindDef.defName;
    }

    internal static bool IsFromMod(this PawnKindDef pkd, string modname)
    {
        if (modname.NullOrEmpty())
            return true;
        return pkd.modContentPack != null && pkd.modContentPack.Name == modname;
    }

    internal static bool IsTribalOrBoss(this PawnKindDef pkd)
    {
        if (pkd == null || pkd.defName == null)
            return false;
        return pkd.defName.Contains("Tribal_") || pkd.defName.Contains("Empire_Fighter_Champion") || pkd.defName.Contains("ZhthyhlMid");
    }

    internal static bool IsDroidColonist(this PawnKindDef pkd)
    {
        return pkd != null && pkd.defName != null && pkd.defName.EndsWith("DroidColonist");
    }

    internal static bool IsDorfbewohner(this PawnKindDef pkd)
    {
        return pkd != null && pkd.defName != null && pkd.defName.StartsWith("Villager");
    }

    internal static bool IsXeno(this PawnKindDef pkd)
    {
        return pkd.race != null && pkd.modContentPack != null && pkd.modContentPack.Name == "Alien Vs Predator" && !pkd.RaceProps.Humanlike && pkd.race.defName != null && !pkd.race.defName.ToLower().Contains("yautja");
    }

    internal static bool IsAbomination(this PawnKindDef pkd)
    {
        return pkd != null && pkd.defName != null && pkd.defName.StartsWith("Abomination");
    }

    internal static bool IsInsektoid(this PawnKindDef pkd)
    {
        return pkd.RaceProps.FleshType == FleshTypeDefOf.Insectoid;
    }

    internal static bool IsMechanoid(this PawnKindDef pkd)
    {
        return pkd.RaceProps.FleshType == FleshTypeDefOf.Mechanoid;
    }

    internal static bool IsZombie(this PawnKindDef pkd)
    {
        return pkd.defName == "Zombie";
    }

    internal static bool IsAnimal(this PawnKindDef pkd)
    {
        return pkd != null && pkd.RaceProps != null && pkd.RaceProps.Animal;
    }

    internal static PawnKindDef ThisOrFromList(
        this PawnKindDef pkd,
        List<PawnKindDef> lpkd)
    {
        if (pkd != null)
            return pkd;
        var pawnKindDefList = new List<PawnKindDef>();
        pawnKindDefList.AddRange(lpkd);
        pawnKindDefList.Remove(null);
        return pawnKindDefList.RandomElementWithFallback();
    }

    internal static PawnKindDef ThisOrRandom(this PawnKindDef pkd, string modname)
    {
        return pkd ?? GetRandomPawnKindDef(modname);
    }

    internal static PawnKindDef GetRandomPawnKindDef(string modname)
    {
        return DefTool.ListByMod<PawnKindDef>(modname).RandomElement();
    }

    internal static List<PawnKindDef> ThisOrDefault(this List<PawnKindDef> l)
    {
        return !l.NullOrEmpty() && (l.Count != 1 || l.First() != null) ? l : CEditor.API.ListOf<PawnKindDef>(EType.PawnKindListed).Where(pkd => pkd != null).ToList();
    }

    internal static List<PawnKindDef> GetHumanlikes()
    {
        return DefDatabase<PawnKindDef>.AllDefs.Where(td => td.race != null && td.race.label != null && td.RaceProps.Humanlike).OrderBy(td => td.race.label).ToList();
    }

    internal static List<PawnKindDef> GetAnimals()
    {
        return DefDatabase<PawnKindDef>.AllDefs.Where(td => td.race != null && td.race.label != null && td.RaceProps.Animal).OrderBy(td => td.race.label).ToList();
    }

    internal static List<PawnKindDef> GetOther()
    {
        return DefDatabase<PawnKindDef>.AllDefs.Where(td => td.race != null && td.race.label != null && !td.RaceProps.Animal && !td.RaceProps.Humanlike).OrderBy(td => td.race.label).ToList();
    }

    internal static List<PawnKindDef> GetPawnKindListxx(Faction f, string key = null)
    {
        key = key ?? CEditor.ListName;
        bool humanoid;
        bool animal;
        bool other;
        bool xeno;
        PawnxTool.SetPawnKindFlags(key, f, out var _, out humanoid, out animal, out other, out xeno, out var _);
        var pawnKindDefList1 = humanoid ? CEditor.API.ListOf<PawnKindDef>(EType.PawnKindHuman) : null;
        var pawnKindDefList2 = animal || f == Faction.OfInsects ? CEditor.API.ListOf<PawnKindDef>(EType.PawnKindAnimal) : null;
        var pawnKindDefList3 = other ? CEditor.API.ListOf<PawnKindDef>(EType.PawnKindOther) : null;
        var pawnKindDefList4 = new List<PawnKindDef>();
        if (!pawnKindDefList1.NullOrEmpty())
            pawnKindDefList4.AddRange(pawnKindDefList1);
        if (!pawnKindDefList2.NullOrEmpty())
            pawnKindDefList4.AddRange(pawnKindDefList2);
        if (!pawnKindDefList3.NullOrEmpty())
            pawnKindDefList4.AddRange(pawnKindDefList3);
        var pawnKindDefList5 = !f.IsInsektoid() ? !f.IsMechanoid() ? pawnKindDefList4.Where(td => td.defName != "Zombie").OrderBy(td => td.label).ToList() : pawnKindDefList4.Where(td => td.RaceProps.FleshType == FleshTypeDefOf.Mechanoid).ToList() : pawnKindDefList4.Where(td => td.RaceProps.FleshType == FleshTypeDefOf.Insectoid).ToList();
        if (xeno)
            pawnKindDefList5 = pawnKindDefList5.Where(td => td.race != null && td.IsFromMod("Alien Vs Predator") && !td.RaceProps.Humanlike && td.race.defName != null && !td.race.defName.ToLower().Contains("yautja")).ToList();
        return pawnKindDefList5;
    }

    internal static HashSet<ThingDef> ListOfRaces(bool humanlike, bool nonhumanlike)
    {
        var thingDefSet1 = new HashSet<ThingDef>();
        var thingDefSet2 = new HashSet<ThingDef>();
        foreach (var allDef in DefDatabase<PawnKindDef>.AllDefs)
            if (allDef != null && allDef.race != null && allDef.race.label != null)
            {
                if (allDef.RaceProps.Humanlike)
                {
                    if (!thingDefSet1.Contains(allDef.race))
                        thingDefSet1.Add(allDef.race);
                }
                else if (!thingDefSet2.Contains(allDef.race))
                {
                    thingDefSet2.Add(allDef.race);
                }
            }

        var flag = humanlike & nonhumanlike || (!humanlike && !nonhumanlike);
        var thingDefSet3 = new HashSet<ThingDef>();
        if (flag | humanlike)
            thingDefSet3.AddRange(thingDefSet1);
        if (flag | nonhumanlike)
            thingDefSet3.AddRange(thingDefSet2);
        return thingDefSet3;
    }

    internal static HashSet<PawnKindDef> ListOfPawnKindDefByRace(
        ThingDef raceDef,
        bool humanlike,
        bool nonhumanlike)
    {
        var allDefs = DefDatabase<PawnKindDef>.AllDefs;
        var all = humanlike & nonhumanlike || (!humanlike && !nonhumanlike);
        return raceDef == null
            ? allDefs.Where(td =>
            {
                if (td == null || td.race == null)
                    return false;
                return all || td.RaceProps.Humanlike == humanlike;
            }).OrderBy(td => td.label).ToHashSet()
            : allDefs.Where(td =>
            {
                if (td == null || td.race == null || td.race != raceDef)
                    return false;
                return all || td.RaceProps.Humanlike == humanlike;
            }).OrderBy(td => td.label).ToHashSet();
    }

    internal static List<PawnKindDef> ListOfPawnKindDef(
        Faction f,
        string key,
        string modname)
    {
        key = key ?? CEditor.ListName;
        bool humanoid;
        bool animal;
        bool other;
        bool xeno;
        PawnxTool.SetPawnKindFlags(key, f, out var _, out humanoid, out animal, out other, out xeno, out var _);
        var pawnKindDefList1 = humanoid ? CEditor.API.ListOf<PawnKindDef>(EType.PawnKindHuman) : null;
        var pawnKindDefList2 = animal || f == Faction.OfInsects ? CEditor.API.ListOf<PawnKindDef>(EType.PawnKindAnimal) : null;
        var pawnKindDefList3 = other ? CEditor.API.ListOf<PawnKindDef>(EType.PawnKindOther) : null;
        var pawnKindDefList4 = new List<PawnKindDef>();
        if (!pawnKindDefList1.NullOrEmpty())
            pawnKindDefList4.AddRange(pawnKindDefList1);
        if (!pawnKindDefList2.NullOrEmpty())
            pawnKindDefList4.AddRange(pawnKindDefList2);
        if (!pawnKindDefList3.NullOrEmpty())
            pawnKindDefList4.AddRange(pawnKindDefList3);
        return !f.IsInsektoid() ? !f.IsMechanoid() ? !xeno ? pawnKindDefList4.Where(td => td.IsFromMod(modname) && !td.IsZombie()).OrderBy(td => td.label).ToList() : pawnKindDefList4.Where(td => td.IsFromMod(modname) && td.IsXeno()).ToList() : pawnKindDefList4.Where(td => td.IsFromMod(modname) && td.IsMechanoid()).ToList() : pawnKindDefList4.Where(td => td.IsFromMod(modname) && td.IsInsektoid()).ToList();
    }
}
