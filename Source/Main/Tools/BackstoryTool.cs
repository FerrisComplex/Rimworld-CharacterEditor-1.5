// Decompiled with JetBrains decompiler
// Type: CharacterEditor.BackstoryTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class BackstoryTool
{
    internal static List<BackstoryDef> ListOfBackstories(
        bool isChildhood,
        bool notDisabling)
    {
        return DefDatabase<BackstoryDef>.AllDefs.Where(td =>
        {
            if (td == null || (int)td.slot != (isChildhood ? 0 : 1) || string.IsNullOrEmpty(td.title))
                return false;
            return !notDisabling || !td.DisabledWorkTypes.Any();
        }).OrderBy(td => td.title).ToList();
    }

    internal static BackstoryDef GetBackstory(string s)
    {
        if (s.NullOrEmpty())
            return null;
        var list = DefDatabase<BackstoryDef>.AllDefs.Where(x => !x.IsNullOrEmpty() && !x.defName.NullOrEmpty()).OrderBy(x => x.defName).ToList();
        foreach (var backstoryDef in list)
            if (backstoryDef.defName == s)
                return backstoryDef;
        var backstoryDef1 = list.RandomElement();
        MessageTool.Show("could not find backstory " + s + " loaded " + backstoryDef1.defName + " instead");
        return backstoryDef1;
    }

    internal static string GetBackstrory(this Pawn pawn, bool isChildhood)
    {
        if (!pawn.HasStoryTracker())
            return "";
        return !isChildhood ? pawn.story.Adulthood != null ? pawn.story.Adulthood.defName : "" : pawn.story.Childhood != null ? pawn.story.Childhood.defName : "";
    }

    internal static void SetBackstory(
        this Pawn pawn,
        bool next,
        bool random,
        bool isChildhood,
        bool notDisabled)
    {
        if (pawn == null || pawn.story == null)
            return;
        var list = DefDatabase<BackstoryDef>.AllDefs.Where(td =>
        {
            if (td == null || (int)td.slot != (isChildhood ? 0 : 1) || string.IsNullOrEmpty(td.title))
                return false;
            return !notDisabled || !td.DisabledWorkTypes.Any();
        }).OrderBy(td => td.title).ToList();
        var index1 = list.IndexOf(isChildhood ? pawn.story.Childhood : pawn.story.Adulthood);
        var index2 = list.NextOrPrevIndex(index1, next, random);
        var childhood = isChildhood ? list[index2] : pawn.story?.Childhood;
        var adulthood = !isChildhood ? list[index2] : pawn.story?.Adulthood;
        pawn.SetBackstory(childhood, adulthood);
    }

    internal static void SetBackstory(this Pawn p, BackstoryDef childhood, BackstoryDef adulthood)
    {
        if (p == null || p.story == null)
            return;
        p.story.Childhood = childhood;
        p.story.Adulthood = adulthood;
        var childhood1 = p.story.Childhood;
        var adulthood1 = p.story.Adulthood;
        if (p.skills != null && p.skills.skills != null)
        {
            p.EnableAllSkills();
            foreach (var skill in p.skills.skills)
            {
                if (p.story.Childhood != null && skill.def.IsDisabled(p.story.Childhood.workDisables, p.story.Childhood.DisabledWorkTypes))
                    p.SkillDisable(skill.def, 0);
                if (p.story.Adulthood != null && skill.def.IsDisabled(p.story.Adulthood.workDisables, p.story.Adulthood.DisabledWorkTypes))
                    p.SkillDisable(skill.def, 0);
            }
        }

        p.skills?.Notify_SkillDisablesChanged();
        p.Recalculate_WorkTypes();
        MeditationFocusTypeAvailabilityCache.ClearFor(p);
        StatsReportUtility.Reset();
    }
}
