// Decompiled with JetBrains decompiler
// Type: CharacterEditor.MindTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class MindTool
{
    internal static string GetInspirationTooltip(this Pawn p)
    {
        return (p == null || p.Inspiration == null || p.Inspiration.def.beginLetter.NullOrEmpty()) ? "" : p.Inspiration.def.beginLetter.Formatted(p.NameShortColored, p.Named("PAWN")).AdjustedFor(p, "PAWN", true).ToString();
    }

    internal static string GetMentalStateTooltip(this Pawn p)
    {
        return p != null && p.MentalState != null ? p.MentalState.GetBeginLetterText().ToString() : "";
    }

    internal static Dictionary<EType, HashSet<ThoughtDef>> GetAllThoughtLists()
    {
        var thoughtDefSet1 = new HashSet<ThoughtDef>();
        var thoughtDefSet2 = new HashSet<ThoughtDef>();
        var thoughtDefSet3 = new HashSet<ThoughtDef>();
        var thoughtDefSet4 = new HashSet<ThoughtDef>();
        var thoughtDefSet5 = new HashSet<ThoughtDef>();
        var thoughtDefSet6 = new HashSet<ThoughtDef>();
        var thoughtDefSet7 = new HashSet<ThoughtDef>();
        foreach (var t in DefTool.ListAll<ThoughtDef>())
            try
            {
                if (!t.IsNullOrEmpty() && !thoughtDefSet7.Contains(t))
                    if (!t.GetThoughtLabel().NullOrEmpty())
                    {
                        thoughtDefSet7.Add(t);
                        if (t.IsTypeOf<Thought_Memory>())
                        {
                            if (t.IsTrulySocial())
                                thoughtDefSet4.Add(t);
                            else
                                thoughtDefSet1.Add(t);
                        }
                        else if (t.IsTypeOf<Thought_Situational>())
                        {
                            if (t.IsTrulySocial())
                                thoughtDefSet5.Add(t);
                            else
                                thoughtDefSet2.Add(t);
                        }
                        else
                        {
                            thoughtDefSet6.Add(t);
                        }
                    }
            }
            catch
            {
            }

        if (Prefs.DevMode)
        {
            var strArray = new string[12];
            strArray[0] = "total=";
            strArray[1] = thoughtDefSet7.Count.ToString();
            strArray[2] = ", mem=";
            var count = thoughtDefSet1.Count;
            strArray[3] = count.ToString();
            strArray[4] = ", memsoz=";
            count = thoughtDefSet4.Count;
            strArray[5] = count.ToString();
            strArray[6] = ", situ=";
            count = thoughtDefSet2.Count;
            strArray[7] = count.ToString();
            strArray[8] = ", situsoz=";
            count = thoughtDefSet5.Count;
            strArray[9] = count.ToString();
            strArray[10] = ", not=";
            count = thoughtDefSet6.Count;
            strArray[11] = count.ToString();
            MessageTool.Show(string.Concat(strArray));
        }

        var hashSet1 = thoughtDefSet1.OrderBy(t => t.GetThoughtLabel()).ToHashSet();
        var hashSet2 = thoughtDefSet4.OrderBy(t => t.GetThoughtLabel()).ToHashSet();
        var hashSet3 = thoughtDefSet2.OrderBy(t => t.GetThoughtLabel()).ToHashSet();
        var hashSet4 = thoughtDefSet5.OrderBy(t => t.GetThoughtLabel()).ToHashSet();
        var hashSet5 = thoughtDefSet6.OrderBy(t => t.GetThoughtLabel()).ToHashSet();
        var hashSet6 = thoughtDefSet7.OrderBy(t => t.GetThoughtLabel()).ToHashSet();
        var dictionary = new Dictionary<EType, HashSet<ThoughtDef>>();
        dictionary.Add(EType.ThoughtMemory, hashSet1);
        dictionary.Add(EType.ThoughtMemorySocial, hashSet2);
        dictionary.Add(EType.ThoughtSituational, hashSet3);
        dictionary.Add(EType.ThoughtSituationalSocial, hashSet4);
        dictionary.Add(EType.ThoughtUnsupported, hashSet5);
        dictionary.Add(EType.ThoughtsAll, hashSet6);
        return dictionary;
    }

    internal static List<MentalStateDef> GetAllMentalStates()
    {
        var list = DefDatabase<MentalStateDef>.AllDefs.Where(td => td != null && !string.IsNullOrEmpty(td.label)).OrderBy(td => td.label).ToList();
        list.Insert(0, null);
        return list;
    }

    internal static List<InspirationDef> GetAllInspirations()
    {
        var list = DefDatabase<InspirationDef>.AllDefs.Where(td => td != null && !string.IsNullOrEmpty(td.label)).OrderBy(td => td.label).ToList();
        list.Insert(0, null);
        return list;
    }

    internal static string GetAllNeedsAsSeparatedString(this Pawn p)
    {
        if (!p.HasNeedsTracker() && !p.needs.AllNeeds.NullOrEmpty())
            return "";
        var text = "";
        foreach (var allNeed in p.needs.AllNeeds)
        {
            text = text + allNeed.def.defName + "|";
            text += allNeed.CurLevelPercentage.ToString();
            text += ":";
        }

        return text.SubstringRemoveLast();
    }

    internal static Need GetNeedForThis(this Pawn p, NeedDef n)
    {
        return !p.HasNeedsTracker() ? null : p.needs.TryGetNeed(n);
    }

    internal static void SetNeeds(this Pawn p, string s)
    {
        if (s.NullOrEmpty() || !p.HasNeedsTracker())
            return;
        foreach (var s1 in s.SplitNo(":"))
        {
            var strArray = s1.SplitNo("|");
            if (strArray.Length == 2)
            {
                var n = DefTool.NeedDef(strArray[0]);
                if (n != null)
                {
                    var needForThis = p.GetNeedForThis(n);
                    if (needForThis != null)
                        needForThis.CurLevelPercentage = strArray[1].AsFloat();
                }
            }
        }
    }
}
