using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor
{
    internal static class TraitTool
    {
        internal static void UpdateDicTooltip(List<KeyValuePair<TraitDef, TraitDegreeData>> l)
        {
            TraitTool.dicDesc.Clear();
            TraitTool.pid = CEditor.API.Pawn.thingIDNumber;
            if (!l.NullOrEmpty<KeyValuePair<TraitDef, TraitDegreeData>>())
            {
                foreach (KeyValuePair<TraitDef, TraitDegreeData> key in l)
                {
                    if (key.Key != null)
                    {
                        string text = "";
                        string text2 = (key.Value != null) ? key.Value.description : key.Key.description;
                        if (!text2.NullOrEmpty())
                        {
                            try
                            {
                                text2 = new Trait(key.Key, (key.Value != null) ? key.Value.degree : 0, false).TipString(CEditor.API.Pawn);
                            }
                            catch
                            {
                            }
                        }

                        text += text2;
                        text += "\n\n";
                        text += key.Key.GetModName().Colorize(Color.gray);
                        TraitTool.dicDesc.Add(key, text);
                    }
                }
            }
        }


        internal static Func<List<KeyValuePair<TraitDef, TraitDegreeData>>, GeneticTraitData, string> FTraitLabel2
        {
            get
            {
                return (List<KeyValuePair<TraitDef, TraitDegreeData>> l, GeneticTraitData gtd) => TraitTool.FTraitLabel(l.Where(delegate(KeyValuePair<TraitDef, TraitDegreeData> pair)
                {
                    KeyValuePair<TraitDef, TraitDegreeData> keyValuePair = pair;
                    bool result;
                    if (keyValuePair.Key == gtd.def)
                    {
                        keyValuePair = pair;
                        result = (keyValuePair.Value.degree == gtd.degree);
                    }
                    else
                    {
                        result = false;
                    }

                    return result;
                }).FirstOrFallback(default(KeyValuePair<TraitDef, TraitDegreeData>)));
            }
        }


        internal static string GetGeneticTraitTooltip(GeneticTraitData gtd)
        {
            if (TraitTool.dicDesc.NullOrEmpty<KeyValuePair<TraitDef, TraitDegreeData>, string>() || TraitTool.pid != CEditor.API.Pawn.thingIDNumber)
            {
                TraitTool.UpdateDicTooltip(TraitTool.ListOfTraitsKeyValuePair(null, null, null));
            }

            TraitDegreeData tdd = TraitTool.GetTraitDegreeDataFromGenetic(gtd);
            return TraitTool.FTraitTooltip(TraitTool.dicDesc.Keys.First((KeyValuePair<TraitDef, TraitDegreeData> x) => x.Key == gtd.def && x.Value == tdd));
        }


        internal static TraitDegreeData GetTraitDegreeDataFromGenetic(GeneticTraitData gtd)
        {
            if (!gtd.def.degreeDatas.NullOrEmpty<TraitDegreeData>())
            {
                foreach (TraitDegreeData traitDegreeData in gtd.def.degreeDatas)
                {
                    if (traitDegreeData.degree == gtd.degree)
                    {
                        return traitDegreeData;
                    }
                }

                return null;
            }

            return null;
        }


        internal static string GetGeneticTraitLabel(GeneticTraitData gtd)
        {
            string result;
            if (gtd == null || gtd.def == null)
            {
                result = "";
            }
            else
            {
                if (!gtd.def.degreeDatas.NullOrEmpty<TraitDegreeData>())
                {
                    foreach (TraitDegreeData traitDegreeData in gtd.def.degreeDatas)
                    {
                        if (traitDegreeData.degree == gtd.degree)
                        {
                            return traitDegreeData.LabelCap;
                        }
                    }

                    return gtd.def.label;
                }

                result = gtd.def.LabelCap;
            }

            return result;
        }


        internal static HashSet<GeneticTraitData> ListAllAsGenticTraitData()
        {
            List<TraitDef> list = TraitTool.ListOfTraitDef(null);
            HashSet<GeneticTraitData> hashSet = new HashSet<GeneticTraitData>();
            foreach (TraitDef traitDef in list)
            {
                if (traitDef.degreeDatas.NullOrEmpty<TraitDegreeData>())
                {
                    hashSet.Add(new GeneticTraitData
                    {
                        def = traitDef,
                        degree = 0
                    });
                }
                else
                {
                    foreach (TraitDegreeData traitDegreeData in traitDef.degreeDatas)
                    {
                        hashSet.Add(new GeneticTraitData
                        {
                            def = traitDef,
                            degree = traitDegreeData.degree
                        });
                    }
                }
            }

            return hashSet;
        }


        internal static string GetTraitAsSeparatedString(this Trait t)
        {
            string result;
            if (t == null || t.def.IsNullOrEmpty())
            {
                result = "";
            }
            else
            {
                result = "" + t.def.defName + "|" + t.Degree.ToString();
            }

            return result;
        }


        internal static string GetAllTraitsAsSeparatedString(this Pawn p)
        {
            if (p == null || !p.HasTraitTracker() || p.story.traits.allTraits.NullOrEmpty<Trait>())
                return "";
            string text = "";
            foreach (Trait t in p.story.traits.allTraits)
            {
                if (t == null) continue;
                text += t.GetTraitAsSeparatedString();
                text += ":";
            }

            text = text.SubstringRemoveLast();
            return text;
        }


        internal static void SetTraitsFromSeparatedString(this Pawn p, string s)
        {
            if (p.HasTraitTracker() && !s.NullOrEmpty())
            {
                Dictionary<TraitDef, int> geneBasedTraitDefsToSkip = p.GetGeneBasedTraitDefsToSkip();
                string[] array = s.Split(new string[]
                {
                    ":"
                }, StringSplitOptions.None);
                for (int i = 0; i < array.Length; i++)
                {
                    string[] array2 = array[i].Split(new string[]
                    {
                        "|"
                    }, StringSplitOptions.None);
                    if (array2.Length == 2)
                    {
                        TraitDef traitDef = DefTool.TraitDef(array2[0]);
                        int num = array2[1].AsInt32();
                        bool flag = false;
                        foreach (TraitDef traitDef2 in geneBasedTraitDefsToSkip.Keys)
                        {
                            if (traitDef.defName == traitDef2.defName && num == geneBasedTraitDefsToSkip[traitDef2])
                            {
                                geneBasedTraitDefsToSkip[traitDef2] = -100;
                                flag = true;
                                break;
                            }
                        }

                        if (traitDef != null && !flag)
                        {
                            p.AddTrait(traitDef, traitDef.GetDegreeDataOrFirst(num), false, false, null);
                        }
                    }
                }

                MeditationFocusTypeAvailabilityCache.ClearFor(p);
                StatsReportUtility.Reset();
            }
        }


        internal static Dictionary<TraitDef, int> GetGeneBasedTraitDefsToSkip(this Pawn p)
        {
            Dictionary<TraitDef, int> dictionary = new Dictionary<TraitDef, int>();
            Dictionary<TraitDef, int> result;
            if (!p.HasGeneTracker())
            {
                result = dictionary;
            }
            else
            {
                List<Gene> genesListForReading = p.genes.GenesListForReading;
                for (int i = 0; i < genesListForReading.Count; i++)
                {
                    Gene gene = genesListForReading[i];
                    if (!gene.def.forcedTraits.NullOrEmpty<GeneticTraitData>())
                    {
                        foreach (GeneticTraitData geneticTraitData in gene.def.forcedTraits)
                        {
                            if (!dictionary.ContainsKey(geneticTraitData.def))
                            {
                                dictionary.Add(geneticTraitData.def, geneticTraitData.degree);
                            }
                        }
                    }
                }

                result = dictionary;
            }

            return result;
        }


        internal static string GetTooltipForSkillpoints(this Pawn pawn, SkillRecord skillrecord)
        {
            string result;
            if (pawn == null || skillrecord == null || skillrecord.def == null || !pawn.HasStoryTracker())
            {
                result = "";
            }
            else
            {
                SkillDef def = skillrecord.def;
                string text = "Base lvl " + skillrecord.levelInt.ToString() + "\n";
                try
                {
                    foreach (Trait trait in pawn.story.traits.allTraits)
                    {
                        if (trait.CurrentData != null && !trait.CurrentData.skillGains.Empty<SkillGain>())
                        {
                            foreach (SkillGain skillGain in trait.CurrentData.skillGains)
                            {
                                if (skillGain.skill.defName == def.defName)
                                {
                                    int amount = skillGain.amount;
                                    if (amount != 0)
                                    {
                                        if (amount > 0)
                                        {
                                            text += "+";
                                        }
                                        else
                                        {
                                            text = (text ?? "");
                                        }

                                        text = text + amount.ToString() + " ";
                                        string str = text;
                                        string str2;
                                        if ((str2 = trait.LabelCap) == null)
                                        {
                                            str2 = (trait.Label ?? trait.def.label);
                                        }

                                        text = str + str2;
                                        text += "\n";
                                    }
                                }
                            }
                        }
                    }

                    BackstoryDef backstory = pawn.story.GetBackstory(BackstorySlot.Adulthood);
                    BackstoryDef backstory2 = pawn.story.GetBackstory(BackstorySlot.Childhood);
                    if (backstory != null && !backstory.skillGains.Empty<SkillGain>())
                    {
                        foreach (SkillGain skillGain2 in backstory.skillGains)
                        {
                            if (skillGain2.skill.defName == def.defName)
                            {
                                int amount2 = skillGain2.amount;
                                if (amount2 != 0)
                                {
                                    if (amount2 > 0)
                                    {
                                        text += "+";
                                    }
                                    else
                                    {
                                        text = (text ?? "");
                                    }

                                    text = text + amount2.ToString() + " ";
                                    text += backstory.TitleCapFor(pawn.gender);
                                    text += "\n";
                                }
                            }
                        }
                    }

                    if (backstory2 != null && !backstory2.skillGains.Empty<SkillGain>())
                    {
                        foreach (SkillGain skillGain3 in backstory2.skillGains)
                        {
                            if (skillGain3.skill.defName == def.defName)
                            {
                                int amount3 = skillGain3.amount;
                                if (amount3 != 0)
                                {
                                    if (amount3 > 0)
                                    {
                                        text += "+";
                                    }
                                    else
                                    {
                                        text = (text ?? "");
                                    }

                                    text = text + amount3.ToString() + " ";
                                    text += backstory2.TitleCapFor(pawn.gender);
                                    text += "\n";
                                }
                            }
                        }
                    }
                }
                catch
                {
                }

                if (skillrecord.Aptitude > 0)
                {
                    text = text + "+" + skillrecord.Aptitude.ToString() + " Genetic";
                }
                else if (skillrecord.Aptitude < 0)
                {
                    text = text + skillrecord.Aptitude.ToString() + " Genetic";
                }

                result = text;
            }

            return result;
        }


        internal static int GetTraitOffsetForSkill(this Pawn pawn, SkillDef skill)
        {
            int result;
            if (pawn == null || skill == null || !pawn.HasStoryTracker())
            {
                result = 0;
            }
            else
            {
                int num = 0;
                foreach (Trait trait in pawn.story.traits.allTraits)
                {
                    if (trait.CurrentData != null && !trait.CurrentData.skillGains.Empty<SkillGain>())
                    {
                        foreach (SkillGain skillGain in trait.CurrentData.skillGains)
                        {
                            if (skillGain.skill.defName == skill.defName)
                            {
                                num += skillGain.amount;
                            }
                        }
                    }
                }

                result = num;
            }

            return result;
        }


        internal static void AddTrait(this Pawn pawn, TraitDef traitDef, TraitDegreeData degreeData, bool random = false, bool doChangeSkillValue = false, Trait oldTraitToReplace = null)
        {
            if (pawn != null && pawn.story != null && (random || traitDef != null))
            {
                if (random)
                {
                    traitDef = TraitTool.GetRandomTrait(out degreeData);
                    if (pawn.HasTrait(traitDef, degreeData))
                    {
                        traitDef = TraitTool.GetRandomTrait(out degreeData);
                    }
                }

                if (pawn.story.traits == null)
                {
                    pawn.story.traits = new TraitSet(pawn);
                }

                if (pawn.story.traits.allTraits == null)
                {
                    pawn.story.traits.allTraits = new List<Trait>();
                }

                Trait trait = new Trait(traitDef, (degreeData == null) ? traitDef.degreeDatas.FirstOrDefault<TraitDegreeData>().degree : degreeData.degree, false);
                if (oldTraitToReplace != null)
                {
                    if (pawn.skills != null && doChangeSkillValue)
                    {
                        TraitDegreeData currentData = oldTraitToReplace.CurrentData;
                        if (currentData != null && !currentData.skillGains.Empty<SkillGain>())
                        {
                            foreach (SkillRecord skillRecord in pawn.skills.skills)
                            {
                                foreach (SkillGain skillGain in currentData.skillGains)
                                {
                                    if (skillRecord.def.defName == skillGain.skill.defName)
                                    {
                                        skillRecord.Level -= skillGain.amount;
                                    }
                                }
                            }
                        }
                    }

                    pawn.story.traits.allTraits.Replace(oldTraitToReplace, trait);
                }
                else
                {
                    pawn.story.traits.allTraits.Add(trait);
                }

                trait.pawn = pawn;
                pawn.Notify_DisabledWorkTypesChanged();
                if (pawn.skills != null)
                {
                    pawn.skills.Notify_SkillDisablesChanged();
                    if (doChangeSkillValue && (degreeData != null && !degreeData.skillGains.Empty<SkillGain>()))
                    {
                        foreach (SkillRecord skillRecord2 in pawn.skills.skills)
                        {
                            foreach (SkillGain skillGain2 in degreeData.skillGains)
                            {
                                if (skillRecord2.def.defName == skillGain2.skill.defName)
                                {
                                    skillRecord2.Level += skillGain2.amount;
                                }
                            }
                        }
                    }
                }

                if (!pawn.Dead && pawn.RaceProps.Humanlike && pawn.needs.mood != null)
                {
                    pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
                }

                MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
                StatsReportUtility.Reset();
            }
        }


        internal static void RemoveTrait(this Pawn pawn, Trait t)
        {
            if (t != null)
            {
                if (t.CurrentData != null && !t.CurrentData.skillGains.Empty<SkillGain>())
                {
                    foreach (SkillRecord skillRecord in pawn.skills.skills)
                    {
                        foreach (SkillGain skillGain in t.CurrentData.skillGains)
                        {
                            if (skillRecord.def.defName == skillGain.skill.defName)
                            {
                                skillRecord.Level -= skillGain.amount;
                            }
                        }
                    }
                }

                CEditor.API.Pawn.story.traits.allTraits.Remove(t);
                MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
                StatsReportUtility.Reset();
            }
        }


        internal static TraitDegreeData GetDegreeDataOrFirst(this TraitDef t, int degree)
        {
            if (t != null && !t.degreeDatas.NullOrEmpty<TraitDegreeData>())
            {
                foreach (TraitDegreeData traitDegreeData in t.degreeDatas)
                {
                    if (traitDegreeData.degree == degree)
                    {
                        return traitDegreeData;
                    }
                }

                return t.degreeDatas.FirstOrDefault<TraitDegreeData>();
            }

            return null;
        }


        internal static TraitDef GetRandomTrait(out TraitDegreeData degreeData)
        {
            TraitDef traitDef = DefDatabase<TraitDef>.AllDefs.RandomElement<TraitDef>();
            degreeData = (traitDef.degreeDatas.NullOrEmpty<TraitDegreeData>() ? null : traitDef.degreeDatas.RandomElement<TraitDegreeData>());
            return traitDef;
        }


        internal static bool HasTrait(this Pawn pawn, TraitDef traitDef, TraitDegreeData degreeData)
        {
            if (pawn != null && pawn.story != null && traitDef != null && pawn.story.traits != null && !pawn.story.traits.allTraits.NullOrEmpty<Trait>())
            {
                int num = (degreeData == null) ? 0 : degreeData.degree;
                foreach (Trait trait in pawn.story.traits.allTraits)
                {
                    if (trait.def.defName == traitDef.defName && trait.Degree == num)
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }


        internal static List<TraitDef> ListOfTraitDef(string modname)
        {
            bool bAll = modname.NullOrEmpty();
            return (from td in DefDatabase<TraitDef>.AllDefs
                where td != null && !td.defName.NullOrEmpty() && (bAll || td.IsFromMod(modname))
                orderby td.defName
                select td).ToList<TraitDef>();
        }


        internal static HashSet<StatModifier> ListOfTraitStatModifier(string modname, bool withNull)
        {
            List<TraitDef> list = TraitTool.ListOfTraitDef(modname);
            HashSet<StatModifier> hashSet = new HashSet<StatModifier>();
            List<string> list2 = new List<string>();
            if (withNull)
            {
                hashSet.Add(null);
            }

            if (!list.NullOrEmpty<TraitDef>())
            {
                foreach (TraitDef traitDef in list)
                {
                    if (traitDef != null && !traitDef.degreeDatas.NullOrEmpty<TraitDegreeData>())
                    {
                        foreach (TraitDegreeData traitDegreeData in traitDef.degreeDatas)
                        {
                            if (traitDegreeData != null)
                            {
                                if (!traitDegreeData.statOffsets.NullOrEmpty<StatModifier>())
                                {
                                    foreach (StatModifier statModifier in traitDegreeData.statOffsets)
                                    {
                                        if (statModifier.stat != null && statModifier.stat.defName != null && !list2.Contains(statModifier.stat.defName))
                                        {
                                            list2.Add(statModifier.stat.defName);
                                            hashSet.Add(statModifier);
                                        }
                                    }
                                }

                                if (!traitDegreeData.statFactors.NullOrEmpty<StatModifier>())
                                {
                                    foreach (StatModifier statModifier2 in traitDegreeData.statFactors)
                                    {
                                        if (statModifier2.stat != null && statModifier2.stat.defName != null && !list2.Contains(statModifier2.stat.defName))
                                        {
                                            list2.Add(statModifier2.stat.defName);
                                            hashSet.Add(statModifier2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return hashSet;
        }


        internal static List<KeyValuePair<TraitDef, TraitDegreeData>> ListOfTraitsKeyValuePair(string modname, StatModifier sm = null, string category = null)
        {
            List<TraitDef> list = TraitTool.ListOfTraitDef(modname);
            List<KeyValuePair<TraitDef, TraitDegreeData>> list2 = new List<KeyValuePair<TraitDef, TraitDegreeData>>();
            foreach (TraitDef traitDef in list)
            {
                if (traitDef.degreeDatas.NullOrEmpty<TraitDegreeData>())
                {
                    list2.Add(new KeyValuePair<TraitDef, TraitDegreeData>(traitDef, null));
                }
                else
                {
                    foreach (TraitDegreeData value in traitDef.degreeDatas)
                    {
                        list2.Add(new KeyValuePair<TraitDef, TraitDegreeData>(traitDef, value));
                    }
                }
            }

            List<KeyValuePair<TraitDef, TraitDegreeData>> list3 = list2.OrderBy(delegate(KeyValuePair<TraitDef, TraitDegreeData> td)
            {
                KeyValuePair<TraitDef, TraitDegreeData> keyValuePair = td;
                string result;
                if (keyValuePair.Value == null)
                {
                    keyValuePair = td;
                    result = keyValuePair.Key.LabelCap.RawText;
                }
                else
                {
                    keyValuePair = td;
                    if ((result = keyValuePair.Value.LabelCap) == null)
                    {
                        keyValuePair = td;
                        if ((result = keyValuePair.Value.label) == null)
                        {
                            keyValuePair = td;
                            result = keyValuePair.Key.label;
                        }
                    }
                }

                return result;
            }).ToList<KeyValuePair<TraitDef, TraitDegreeData>>();
            if (sm != null)
            {
                List<KeyValuePair<TraitDef, TraitDegreeData>> list4 = new List<KeyValuePair<TraitDef, TraitDegreeData>>();
                foreach (KeyValuePair<TraitDef, TraitDegreeData> item in list3)
                {
                    if (!item.Key.degreeDatas.NullOrEmpty<TraitDegreeData>())
                    {
                        foreach (TraitDegreeData traitDegreeData in item.Key.degreeDatas)
                        {
                            if (traitDegreeData != null)
                            {
                                if (!traitDegreeData.statOffsets.NullOrEmpty<StatModifier>() && traitDegreeData.statOffsets.Contains(sm))
                                {
                                    list4.Add(item);
                                    break;
                                }

                                if (!traitDegreeData.statFactors.NullOrEmpty<StatModifier>() && traitDegreeData.statFactors.Contains(sm))
                                {
                                    list4.Add(item);
                                    break;
                                }
                            }
                        }
                    }
                }

                list3 = list4;
            }

            if (category != null)
            {
                List<KeyValuePair<TraitDef, TraitDegreeData>> list5 = new List<KeyValuePair<TraitDef, TraitDegreeData>>();
                foreach (KeyValuePair<TraitDef, TraitDegreeData> item2 in list3)
                {
                    if (!item2.Key.degreeDatas.NullOrEmpty<TraitDegreeData>())
                    {
                        foreach (TraitDegreeData traitDegreeData2 in item2.Key.degreeDatas)
                        {
                            if (traitDegreeData2 != null)
                            {
                                if (category == Label.STAT)
                                {
                                    if (!traitDegreeData2.statFactors.NullOrEmpty<StatModifier>() || !traitDegreeData2.statOffsets.NullOrEmpty<StatModifier>())
                                    {
                                        list5.Add(item2);
                                        break;
                                    }
                                }
                                else if (category == Label.MENTAL)
                                {
                                    if (traitDegreeData2.mentalBreakInspirationGainChance != 0f || traitDegreeData2.forcedMentalStateMtbDays != -1f || traitDegreeData2.forcedMentalState != null || traitDegreeData2.randomMentalState != null || !traitDegreeData2.disallowedMentalStates.NullOrEmpty<MentalStateDef>() || !traitDegreeData2.theOnlyAllowedMentalBreaks.NullOrEmpty<MentalBreakDef>())
                                    {
                                        list5.Add(item2);
                                        break;
                                    }
                                }
                                else if (category == Label.THOUGHTS)
                                {
                                    if (!traitDegreeData2.disallowedThoughts.NullOrEmpty<ThoughtDef>() || !traitDegreeData2.disallowedThoughtsFromIngestion.NullOrEmpty<TraitIngestionThoughtsOverride>() || !traitDegreeData2.extraThoughtsFromIngestion.NullOrEmpty<TraitIngestionThoughtsOverride>())
                                    {
                                        list5.Add(item2);
                                        break;
                                    }
                                }
                                else if (category == Label.INSPIRATIONS)
                                {
                                    if (traitDegreeData2.mentalBreakInspirationGainChance != 0f || !traitDegreeData2.disallowedInspirations.NullOrEmpty<InspirationDef>() || !traitDegreeData2.mentalBreakInspirationGainSet.NullOrEmpty<InspirationDef>())
                                    {
                                        list5.Add(item2);
                                        break;
                                    }
                                }
                                else if (category == Label.FOCUS)
                                {
                                    if (!traitDegreeData2.allowedMeditationFocusTypes.NullOrEmpty<MeditationFocusDef>() || !traitDegreeData2.disallowedMeditationFocusTypes.NullOrEmpty<MeditationFocusDef>())
                                    {
                                        list5.Add(item2);
                                        break;
                                    }
                                }
                                else if (category == Label.SKILLGAINS)
                                {
                                    if (!traitDegreeData2.skillGains.Empty<SkillGain>())
                                    {
                                        list5.Add(item2);
                                        break;
                                    }
                                }
                                else if (category == Label.ABILITIES)
                                {
                                    if (!traitDegreeData2.abilities.NullOrEmpty<AbilityDef>())
                                    {
                                        list5.Add(item2);
                                        break;
                                    }
                                }
                                else if (category == Label.NEEDS)
                                {
                                    if (!traitDegreeData2.needs.NullOrEmpty<NeedDef>())
                                    {
                                        list5.Add(item2);
                                        break;
                                    }
                                }
                                else if (category == Label.INGESTIBLEMOD && !traitDegreeData2.ingestibleModifiers.NullOrEmpty<IngestibleModifiers>())
                                {
                                    list5.Add(item2);
                                    break;
                                }
                            }
                        }
                    }
                }

                list3 = list5;
            }

            return list3;
        }


        static TraitTool()
        {
        }


        private static int pid = 0;


        private static Dictionary<KeyValuePair<TraitDef, TraitDegreeData>, string> dicDesc = new Dictionary<KeyValuePair<TraitDef, TraitDegreeData>, string>();


        internal static Func<KeyValuePair<TraitDef, TraitDegreeData>, string> FTraitLabel = delegate(KeyValuePair<TraitDef, TraitDegreeData> t)
        {
            string result;
            if (t.Value == null)
            {
                result = t.Key.LabelCap.RawText;
            }
            else if ((result = t.Value.LabelCap) == null)
            {
                result = (t.Value.label ?? t.Key.label);
            }

            return result;
        };


        internal static Func<KeyValuePair<TraitDef, TraitDegreeData>, string> FTraitTooltip = (KeyValuePair<TraitDef, TraitDegreeData> t) => TraitTool.dicDesc.GetValue(t);


        internal static Func<KeyValuePair<TraitDef, TraitDegreeData>, KeyValuePair<TraitDef, TraitDegreeData>, bool> FTraitComparator = (KeyValuePair<TraitDef, TraitDegreeData> t1, KeyValuePair<TraitDef, TraitDegreeData> t2) => t1.Key == t2.Key && t1.Value == t2.Value;


        internal static Func<GeneticTraitData, string> FGeneticTraitLabel = (GeneticTraitData gtd) => TraitTool.GetGeneticTraitLabel(gtd);
    }
}
