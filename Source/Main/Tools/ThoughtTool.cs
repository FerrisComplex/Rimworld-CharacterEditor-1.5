// Decompiled with JetBrains decompiler
// Type: CharacterEditor.ThoughtTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class ThoughtTool
{
    internal static bool IsTrulySocial(this ThoughtDef t)
    {
        if (t == null)
            return false;
        if (t.IsSocial)
            return true;
        return t.IsTypeOf<Thought_Memory>() && t.HasLabelPlaceholder();
    }

    internal static bool IsClassOf<T>(this ThoughtDef t)
    {
        return t.thoughtClass == typeof(T);
    }

    internal static bool IsTypeOf<T>(this ThoughtDef t)
    {
        if (t == null)
            return false;
        if (t.thoughtClass == typeof(T) || t.ThoughtClass == typeof(T))
            return true;
        if (!(t.thoughtClass != null))
            return false;
        if (t.thoughtClass.BaseType == typeof(T))
            return true;
        if (!(t.thoughtClass.BaseType != null))
            return false;
        if (t.thoughtClass.BaseType.BaseType == typeof(T))
            return true;
        return t.thoughtClass.BaseType.BaseType != null && t.thoughtClass.BaseType.BaseType.BaseType == typeof(T);
    }

    internal static bool IsForWeapon(this ThoughtDef t)
    {
        return t.HasLabelPlaceholder("{WEAPON}");
    }

    internal static bool IsForTitle(this ThoughtDef t)
    {
        return t.IsTypeOf<Thought_MemoryRoyalTitle>();
    }

    internal static bool HasLabelPlaceholder(this ThoughtDef t, string ph = "{0}")
    {
        return t != null && !t.stages.NullOrEmpty() && t.stages[0].SLabel().Contains(ph);
    }

    internal static bool HasMemories(this Pawn pawn)
    {
        return pawn != null && pawn.needs != null && pawn.needs.mood != null && pawn.needs.mood.thoughts != null && pawn.needs.mood.thoughts.memories != null && pawn.needs.mood.thoughts.memories.Memories != null;
    }

    internal static Gender GetOppositeGender(this Pawn p)
    {
        if (p == null)
            return (Gender)2;
        return p.gender != Gender.Male ? (Gender)1 : (Gender)2;
    }

    internal static List<Thought_Situational> GetAllThoughtSituationals(
        this Pawn p)
    {
        return p.needs.mood.thoughts.situational.GetMemberValue("cachedThoughts", (List<Thought_Situational>)null);
    }

    internal static int FixStageValue(ThoughtDef t, int stage)
    {
        if (stage < 0 || t.stages.NullOrEmpty())
            return 0;
        return t.stages.CountAllowNull() > stage ? stage : t.stages.IndexOf(t.stages.Last());
    }

    internal static string GetAsSeparatedString(this Thought t)
    {
        bool flag = t == null || t.def.IsNullOrEmpty();
        string result;
        if (flag)
        {
            result = "";
        }
        else
        {
            string text = "";
            text = text + t.def.defName + "|";
            string text2;
            Pawn otherPawn = t.GetOtherPawn(out text2);
            bool flag2 = otherPawn == null && !text2.NullOrEmpty();
            if (flag2)
            {
                text = text + text2 + "|";
            }
            else
            {
                text = text + otherPawn.GetPawnNameAsSeparatedString() + "|";
            }
            text = text + t.CurStageIndex.ToString() + "|";
            bool flag3 = t.def.IsTypeOf<Thought_MemoryRoyalTitle>();
            if (flag3)
            {
                text = text + ((Thought_MemoryRoyalTitle)t).titleDef.defName + "|";
            }
            else
            {
                text += "|";
            }
            bool flag4 = t.def.IsTypeOf<Thought_Memory>();
            if (flag4)
            {
                text = text + ((Thought_Memory)t).moodPowerFactor.ToString() + "|";
            }
            else
            {
                text += "1|";
            }
            float opinionOffset = t.GetOpinionOffset();
            text = text + ((opinionOffset == float.MinValue) ? 0f : opinionOffset).ToString() + "|";
            bool flag5 = t.def.IsTypeOf<Thought_Memory>();
            if (flag5)
            {
                text += ((Thought_Memory)t).age.ToString();
            }
            else
            {
                text += "0";
            }
            result = text;
        }
        return result;
    }
    internal static string GetAllMemoriesAsSeparatedString(this Pawn p)
    {
        if (!p.HasMemoryTracker() || p.needs.mood.thoughts.memories.Memories.NullOrEmpty())
            return "";
        var text = "";
        foreach (var memory in p.needs.mood.thoughts.memories.Memories)
        {
            text += memory.GetAsSeparatedString();
            text += ":";
        }

        return text.SubstringRemoveLast();
    }

    internal static string GetAllSituationalMemoriesAsSeparatedString(this Pawn p)
    {
        if (!p.HasSituationalTracker() || p.GetAllThoughtSituationals().NullOrEmpty())
            return "";
        var text = "";
        foreach (var thoughtSituational in p.GetAllThoughtSituationals())
        {
            text += thoughtSituational.GetAsSeparatedString();
            text += ":";
        }

        return text.SubstringRemoveLast();
    }

    internal static void SetMemories(this Pawn p, string s)
    {
        if (!p.HasMemoryTracker())
            return;
        p.needs.mood.thoughts.memories.Memories.Clear();
        if (s.NullOrEmpty())
            return;
        foreach (var s1 in s.SplitNo(":"))
        {
            var strArray = s1.SplitNo("|");
            if (strArray.Length == 7)
            {
                var t = DefTool.ThoughtDef(strArray[0]);
                if (t != null)
                {
                    var fromSeparatedString = PawnxTool.GetOtherPawnFromSeparatedString(strArray[1]);
                    p.AddThought(t, fromSeparatedString, strArray[2].AsInt32(), strArray[3], strArray[4].AsFloat(), strArray[5].AsFloat(), strArray[6].AsInt32());
                }
            }
        }
    }

    internal static void SetSituationalMemories(this Pawn p, string s)
    {
        if (!p.HasSituationalTracker())
            return;
        p.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
        p.needs.mood.thoughts.situational.SetMemberValue("lastMoodThoughtsRecalculationTick", Find.TickManager.TicksGame);
        if (s.NullOrEmpty())
            return;
        foreach (var s1 in s.SplitNo(":"))
        {
            var strArray = s1.SplitNo("|");
            if (strArray.Length == 7)
            {
                var t = DefTool.ThoughtDef(strArray[0]);
                if (t != null)
                {
                    var fromSeparatedString = PawnxTool.GetOtherPawnFromSeparatedString(strArray[1]);
                    p.AddThought(t, fromSeparatedString, strArray[2].AsInt32(), strArray[3], strArray[4].AsFloat(), strArray[5].AsFloat(), strArray[6].AsInt32());
                }
            }
        }
    }

    internal static bool HasOtherPawnMember(this ThoughtDef t)
    {
        return t != null && (t.IsSocial || t.stackLimitForSameOtherPawn > 0 || t.IsTypeOf<Thought_MemorySocialCumulative>() || t.IsTypeOf<Thought_MemorySocial>() || t.IsTypeOf<Thought_SituationalSocial>() || t.IsTypeOf<Thought_PsychicHarmonizer>() || (t.IsTypeOf<Thought_Memory>() && t.GetThoughtLabel().Contains("{0}")) || t.IsTypeOf<Thought_BondedAnimalMaster>() || t.IsTypeOf<Thought_NotBondedAnimalMaster>() || t.SDefname().Equals("Jealous"));
    }

    internal static Pawn GetOtherPawn(this Thought t, out string reason)
    {
        Pawn pawn = null;
        reason = "";
        if (t == null)
            return pawn;
        if (t.def.IsTypeOf<Thought_Memory>())
            return ((Thought_Memory)t).otherPawn;
        if (t.def.IsTypeOf<Thought_SituationalSocial>())
            return ((Thought_SituationalSocial)t).otherPawn;
        if (t.def.IsTypeOf<Thought_BondedAnimalMaster>() || t.def.IsTypeOf<Thought_NotBondedAnimalMaster>())
        {
            reason = ((Thought_Situational)t).GetMemberValue(nameof(reason), "");
            var directRelations = t.pawn.relations.DirectRelations;
            for (var index = 0; index < directRelations.Count; ++index)
            {
                var directPawnRelation = directRelations[index];
                var otherPawn = directPawnRelation.otherPawn;
                if (directPawnRelation.def == PawnRelationDefOf.Bond && !otherPawn.Dead && otherPawn.Spawned && otherPawn.Faction == Faction.OfPlayer && otherPawn.GetPawnName() == reason)
                    return otherPawn;
            }

            return null;
        }

        if (t.def.SDefname().Equals("Jealous"))
        {
            reason = ((Thought_Situational)t).GetMemberValue(nameof(reason), "");
            return null;
        }

        if (t.def.IsTypeOf<ThoughtWorker_WantToSleepWithSpouseOrLover>() || t.def.IsTypeOf<Thought_OpinionOfMyLover>())
        {
            var directPawnRelation = LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(t.pawn, false);
            if (directPawnRelation != null)
            {
                reason = directPawnRelation.def.GetGenderSpecificLabel(directPawnRelation.otherPawn);
                return directPawnRelation.otherPawn;
            }
        }

        return pawn;
    }

    internal static float GetOpinionOffset(this Thought t)
    {
        var minValue = float.MinValue;
        return t == null ? minValue : !t.def.IsTypeOf<Thought_MemorySocial>() ? !t.def.IsTypeOf<Thought_SituationalSocial>() ? float.MinValue : t.CurStageIndex < 0 ? 0.0f : ((Thought_SituationalSocial)t).OpinionOffset() : ((Thought_MemorySocial)t).opinionOffset;
    }

    internal static float TryGetMoodOffset(this Thought t)
    {
        var num = 0.0f;
        if (t != null)
            try
            {
                num = t.MoodOffset();
            }
            catch
            {
            }

        return num;
    }

    internal static string GetThoughtLabel(this ThoughtDef t, int stage = 0, Pawn p = null)
    {
        if (t == null)
            return "";
        string str = null;
        if (!t.stages.NullOrEmpty())
        {
            try
            {
                str = str ?? t.stages[stage].labelSocial;
            }
            catch
            {
            }

            try
            {
                str = str ?? t.stages[stage].label;
            }
            catch
            {
            }

            try
            {
                str = str ?? t.stages[stage].untranslatedLabelSocial;
            }
            catch
            {
            }

            try
            {
                str = str ?? t.stages[stage].untranslatedLabel;
            }
            catch
            {
            }
        }

        var format = ((str ?? t.label) ?? t.defName).CapitalizeFirst();
        if (format.StartsWith("{"))
        {
            if (t.defName.StartsWith("HumanLeatherApparel"))
                format = string.Format(format, "Apparel");
            else if (t.defName == "JealousRage" || t.defName == "OnKill_GoodThought" || t.defName == "RelicAtRitual" || t.defName == "IdeoBuildingMissing" || t.defName == "IdeoBuildingDisrespected" || t.defName == "RitualDelayed" || t.defName == "OnKill_BadThought" || t.defName == "ObservedTerror" || t.defName == "KillThirst" || t.defName.StartsWith("BondedThought"))
                format = t.defName;
            else if (p != null && t.Worker != null)
                format = t.Worker.PostProcessLabel(p, format);
        }

        return format;
    }

    internal static string GetThoughtDescription(this ThoughtDef t, int stage = 0, Thought thought = null)
    {
        if (t == null)
            return "";
        string str1 = null;
        if (!t.stages.NullOrEmpty())
            try
            {
                str1 = t.stages[stage].description;
            }
            catch
            {
            }

        var str2 = (str1 ?? t.description) ?? "";
        try
        {
            if (thought != null)
            {
                if (t.IsClassOf<Thought_IdeoRoleEmpty>())
                {
                    var thoughtIdeoRoleEmpty = (Thought_IdeoRoleEmpty)thought;
                    str2 = str2.Replace("{0}", thoughtIdeoRoleEmpty.Role.ideo.memberName).Replace("{ROLE_labelIndef}", thoughtIdeoRoleEmpty.Role.LabelCap);
                }
                else if (t.IsClassOf<Thought_MemoryRoyalTitle>())
                {
                    var memoryRoyalTitle = (Thought_MemoryRoyalTitle)thought;
                    var newValue = CEditor.API.Pawn.gender == Gender.Female ? memoryRoyalTitle.titleDef.labelFemale : memoryRoyalTitle.titleDef.label;
                    str2 = str2.Replace("{TITLE_label}", newValue);
                }
                else if (t.IsClassOf<Thought_IdeoRoleLost>())
                {
                    var thoughtIdeoRoleLost = (Thought_IdeoRoleLost)thought;
                    str2 = str2.Replace("{0}", thoughtIdeoRoleLost.Role.ideo.memberName).Replace("{ROLE_labelIndef}", thoughtIdeoRoleLost.Role.LabelCap);
                }
                else if (t.IsClassOf<Thought_IdeoRoleApparelRequirementNotMet>())
                {
                    var requirementNotMet = (Thought_IdeoRoleApparelRequirementNotMet)thought;
                    str2 = str2.Replace("{0}", requirementNotMet.Role.ideo.memberName).Replace("{ROLE_labelIndef}", requirementNotMet.Role.LabelCap);
                }
                else if (t.IsClassOf<Thought_RelicAtRitual>())
                {
                    var thoughtRelicAtRitual = (Thought_RelicAtRitual)thought;
                    str2 = str2.Replace("{RELICNAME_labelIndef}", thoughtRelicAtRitual.relicName);
                }
                else if (t.IsClassOf<Thought_Situational_WearingDesiredApparel>())
                {
                    var sourcePrecept = (Precept_Apparel)thought.sourcePrecept;
                    str2 = str2.Replace("{APPAREL_indefinite}", new TaggedString(sourcePrecept.apparelDef.LabelCap));
                }
            }
        }
        catch
        {
        }

        return (str2 ?? "").CapitalizeFirst();
    }

    internal static int CountOfDefs<T>(List<Thought> l, out Thought example, string startsWdefname = null)
    {
        var num = 0;
        example = null;
        foreach (var thought in l)
            if (thought.def.IsClassOf<T>() && (startsWdefname == null || thought.def.defName.StartsWith(startsWdefname)))
            {
                example = thought;
                ++num;
            }

        return num;
    }

    internal static string GetThoughtLabel(this Thought t)
    {
        if (t == null)
            return "";
        var stage = FixStageValue(t.def, t.CurStageIndex);
        return t.def.GetThoughtLabel(stage);
    }

    internal static string GetThoughtDescription(this Thought t)
    {
        if (t == null)
            return "";
        var stage = FixStageValue(t.def, t.CurStageIndex);
        return t.def.GetThoughtDescription(stage, t);
    }

    internal static List<Thought> GetThoughtsSorted(this Pawn p)
    {
        var thoughtList1 = new List<Thought>();
        if (p.HasMemoryTracker() && p.HasSituationalTracker())
        {
            var memories = p.needs.mood.thoughts.memories.Memories;
            var thoughtSituationals = p.GetAllThoughtSituationals();
            var thoughtList2 = new List<Thought>();
            thoughtList2.AddRange(memories);
            thoughtList2.AddRange(thoughtSituationals);
            var list1 = thoughtList2.Where(td => td.GetOpinionOffset() == -3.40282346638529E+38).OrderBy(td => td.TryGetMoodOffset()).ToList();
            var list2 = thoughtList2.Where(td => td.GetOpinionOffset() != -3.40282346638529E+38).OrderBy(td => td.GetOpinionOffset()).ToList();
            thoughtList1.AddRange(list1);
            thoughtList1.Reverse();
            thoughtList1.AddRange(list2);
        }

        return thoughtList1;
    }

    internal static void AddThought(
        this Pawn pawn,
        ThoughtDef t,
        Pawn otherPawn = null,
        int stage = 0,
        string optDefName = null,
        float moodPowerFactor = 1f,
        float opinionOffset = 1f,
        int age = 0)
    {
        if (pawn == null || t == null)
            return;
        if (t.IsTypeOf<Thought_Memory>())
        {
            pawn.AddThought_Memory(t, otherPawn, stage, optDefName, moodPowerFactor, opinionOffset, age);
        }
        else
        {
            if (!t.IsTypeOf<Thought_Situational>())
                return;
            AddThoughtSituational(pawn, t, otherPawn, stage, optDefName, moodPowerFactor, opinionOffset);
        }
    }

   private static void AddThought_Memory(this Pawn p, ThoughtDef t, Pawn otherPawn, int stage, string optDefName, float moodPowerFactor, float opinionOffset, int age)
		{
			IndividualThoughtToAdd individualThoughtToAdd = new IndividualThoughtToAdd(t, p, otherPawn, moodPowerFactor, opinionOffset);
			individualThoughtToAdd.thought.SetForcedStage(stage);
			bool flag = t.IsTypeOf<Thought_Memory>();
			if (flag)
			{
				individualThoughtToAdd.thought.age = age;
				individualThoughtToAdd.thought.moodPowerFactor = moodPowerFactor;
			}
			bool flag2 = t.IsTypeOf<Thought_MemorySocial>();
			if (flag2)
			{
				((Thought_MemorySocial)individualThoughtToAdd.thought).opinionOffset = opinionOffset;
			}
			bool flag3 = t.IsTypeOf<Thought_MemoryRoyalTitle>() && !optDefName.NullOrEmpty();
			if (flag3)
			{
				((Thought_MemoryRoyalTitle)individualThoughtToAdd.thought).titleDef = DefTool.GetDef<RoyalTitleDef>(optDefName);
			}
			bool flag4 = t.IsTypeOf<Thought_PsychicHarmonizer>() && p.HasHealthTracker();
			if (flag4)
			{
				List<BodyPartRecord> listOfAllowedBodyPartRecords = p.GetListOfAllowedBodyPartRecords(HediffDefOf.PsychicHarmonizer);
				p.AddHediff2(false, HediffDefOf.PsychicHarmonizer, 1f, listOfAllowedBodyPartRecords.FirstOrDefault<BodyPartRecord>(), false, -1, -1, -1, null);
				foreach (Hediff hediff in p.health.hediffSet.hediffs)
				{
					bool flag5 = hediff.def == HediffDefOf.PsychicHarmonizer;
					if (flag5)
					{
						((Thought_PsychicHarmonizer)individualThoughtToAdd.thought).harmonizer = hediff;
						break;
					}
				}
			}
			bool flag6 = t.IsTypeOf<Thought_WeaponTrait>() && p.HasEquipmentTracker();
			if (flag6)
			{
				List<ThingWithComps> allEquipmentListForReading = p.equipment.AllEquipmentListForReading;
				((Thought_WeaponTrait)individualThoughtToAdd.thought).weapon = allEquipmentListForReading.FirstOrDefault<ThingWithComps>();
			}
			bool flag7 = t.IsTypeOf<Thought_MemorySocial>() && otherPawn == null;
			if (!flag7)
			{
				individualThoughtToAdd.Add();
			}
		}

    private static void AddThoughtSituational(
        Pawn p,
        ThoughtDef t,
        Pawn otherPawn,
        int stage,
        string optDefName,
        float moodPowerFactor,
        float opinionOffset)
    {
        var t1 = ThoughtMaker.MakeThought(t);
        t1.pawn = p;
        var num = FixStageValue(t, stage);
        t1.SetMemberValue("curStageIndex", num);
        if (otherPawn != null)
        {
            if (t.IsTypeOf<Thought_SituationalSocial>())
                ((Thought_SituationalSocial)t1).otherPawn = otherPawn;
            if (t.IsTypeOf<Thought_BondedAnimalMaster>() || t.IsTypeOf<Thought_NotBondedAnimalMaster>() || t.SDefname().Equals("Jealous"))
                ((Thought_Situational)t1).SetMemberValue("reason", otherPawn.LabelShort);
        }

        try
        {
            var memberValue = p.needs.mood.thoughts.situational.GetMemberValue("cachedThoughts", (List<Thought_Situational>)null);
            memberValue.CallMethod("Add", new object[1]
            {
                t1
            });
            p.needs.mood.thoughts.situational.SetMemberValue("cachedThoughts", memberValue);
        }
        catch (Exception ex)
        {
            RemoveThoughtSituational(p, t1);
            Log.Error(ex.Message + "\n" + ex.StackTrace);
            MessageTool.Show("failed to add thought. reason = this thought or thought stage cause an exception. action was rolled back.");
        }
    }

    internal static void ClearAllThoughts(this Pawn p)
    {
        if (p.HasMemoryTracker())
            p.needs.mood.thoughts.memories.Memories.Clear();
        if (!p.HasSituationalTracker())
            return;
        p.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
        p.needs.mood.thoughts.situational.SetMemberValue("lastMoodThoughtsRecalculationTick", Find.TickManager.TicksGame);
    }

    internal static void RemoveThought(this Pawn p, Thought t)
    {
        if (p == null || t == null)
            return;
        if (t.def.IsTypeOf<Thought_Memory>())
        {
            RemoveThoughtMemory(p, t);
        }
        else
        {
            if (!t.def.IsTypeOf<Thought_Situational>())
                return;
            RemoveThoughtSituational(p, t);
        }
    }

    private static void RemoveThoughtMemory(Pawn p, Thought t)
    {
        if (!p.HasMemoryTracker() || t == null)
            return;
        try
        {
            var thoughtMemory = t as Thought_Memory;
            p.needs.mood.thoughts.memories.RemoveMemory(thoughtMemory);
        }
        catch
        {
        }
    }

    private static void RemoveThoughtSituational(Pawn p, Thought t)
    {
        if (!p.HasSituationalTracker() || t == null)
            return;
        try
        {
            var thoughtSituational = t as Thought_Situational;
            var memberValue = p.needs.mood.thoughts.situational.GetMemberValue("cachedThoughts", (List<Thought_Situational>)null);
            if (memberValue == null || !memberValue.Contains(thoughtSituational))
                return;
            memberValue.Remove(thoughtSituational);
            p.needs.mood.thoughts.situational.SetMemberValue("cachedThoughts", memberValue);
        }
        catch
        {
        }
    }
}
