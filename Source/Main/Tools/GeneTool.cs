// Decompiled with JetBrains decompiler
// Type: CharacterEditor.GeneTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class GeneTool
{
    private static bool bAllCategories;
    private static StatCategoryDef selected_StatFactor_CatDef;
    private static StatCategoryDef selected_StatOffset_CatDef;
    internal static HashSet<StatCategoryDef> lCategoryDef_Factors;
    internal static HashSet<StatCategoryDef> lCategoryDef_Offsets;
    internal static HashSet<ConditionalStatAffecter> lFreeConditionalStats;
    internal static HashSet<StatDef> lFreeStatDefFactors;
    internal static HashSet<StatDef> lFreeStatDefOffsets;
    internal static HashSet<SkillDef> lFreeAptitudes;
    internal static HashSet<PawnCapacityDef> lFreeCapacities;
    internal static HashSet<AbilityDef> lFreeAbilities;
    internal static HashSet<NeedDef> lFreeNeeds;
    internal static HashSet<HeadTypeDef> lFreeForcedHeadTypes;
    internal static HashSet<HediffDef> lFreeImmunities;
    internal static HashSet<HediffDef> lFreeProtections;
    internal static HashSet<DamageDef> lFreeDamageFactors;
    internal static HashSet<GeneticTraitData> lFreeForcedTraits;
    internal static HashSet<GeneticTraitData> lFreeSuppressedTraits;
    internal static HashSet<WorkTags> lFreeWorkTags;
    internal static List<StatModifier> lCopyStatFactors = new();
    internal static List<StatModifier> lCopyStatOffsets = new();
    internal static List<Aptitude> lCopyAptitude = new();
    internal static List<DamageFactor> lCopyDamageFactors = new();
    internal static List<PawnCapacityModifier> lCopyCapacities = new();
    internal static List<AbilityDef> lCopyAbilities = new();
    internal static List<GeneticTraitData> lCopyForcedTraits = new();
    internal static List<GeneticTraitData> lCopySuppressedTraits = new();
    internal static List<HediffDef> lCopyImmunities = new();
    internal static List<HediffDef> lCopyProtections = new();
    internal static List<NeedDef> lCopyDisabledNeeds = new();
    internal static List<HeadTypeDef> lCopyForcedHeadTypes = new();
    internal static List<string> lCopyCustomEffectDescriptions = new();
    internal static List<string> lCopyExclusionTags = new();
    internal static List<string> lCopyHairTags = new();
    internal static List<string> lCopyBeardTags = new();
    internal static List<float> lCopyGizmoThres = new();
    internal static WorkTags lCopyDisabledWorkTags = 0;
    internal static List<GeneDef> cachedGenes = new();

    internal static StatCategoryDef StatFactorCategory
    {
        get => selected_StatFactor_CatDef;
        set
        {
            selected_StatFactor_CatDef = value;
            SelectedGene.UpdateFreeLists(FreeList.CategoryFactors);
            SelectedGene.UpdateFreeLists(FreeList.StatFactors);
        }
    }

    internal static StatCategoryDef StatOffsetCategory
    {
        get => selected_StatOffset_CatDef;
        set
        {
            selected_StatOffset_CatDef = value;
            SelectedGene.UpdateFreeLists(FreeList.CategoryOffsets);
            SelectedGene.UpdateFreeLists(FreeList.StatOffsets);
        }
    }

    internal static bool UseAllCategories
    {
        get => bAllCategories;
        set
        {
            bAllCategories = value;
            SelectedGene.UpdateFreeLists(FreeList.All);
        }
    }

    internal static GeneDef SelectedGene { get; set; }

    internal static Func<DamageDef, string> DamageLabel => (DamageDef def) => "DamageType".Translate(def.label).CapitalizeFirst();

    internal static GeneCategoryDef BodySizeCategory => DefTool.GetDef<GeneCategoryDef>("SZSpecial");

    internal static HashSet<GeneDef> ListBodySizeGenes => DefTool.ListBy((Func<GeneDef, bool>)(x => !x.labelShortAdj.NullOrEmpty() && x.labelShortAdj.StartsWith("bodysize")));

    internal static void UpdateFreeLists(this GeneDef g, GeneTool.FreeList f)
    {
        bool flag = f == GeneTool.FreeList.All || f == GeneTool.FreeList.CategoryFactors;
        if (flag)
        {
            GeneTool.lCategoryDef_Factors = GeneTool.ListOfStatCategoryDef_ForStatFactor(CEditor.API.Pawn.RaceProps.IsMechanoid, GeneTool.UseAllCategories);
        }

        bool flag2 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.CategoryOffsets;
        if (flag2)
        {
            GeneTool.lCategoryDef_Offsets = GeneTool.ListOfStatCategoryDef_ForStatOffset(CEditor.API.Pawn.RaceProps.IsMechanoid, GeneTool.UseAllCategories);
        }

        bool flag3 = g == null;
        if (!flag3)
        {
            g.ResolveReferences();
            bool flag4 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.ConditionalStats;
            if (flag4)
            {
                GeneTool.lFreeConditionalStats = GeneTool.ListConditionalAffectors();
            }

            bool flag5 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.StatFactors;
            if (flag5)
            {
                ICollection<StatModifier> statFactors = g.statFactors;
                HashSet<StatCategoryDef> lselectedCat;
                if (GeneTool.selected_StatFactor_CatDef != null)
                {
                    (lselectedCat = new HashSet<StatCategoryDef>()).Add(GeneTool.selected_StatFactor_CatDef);
                }
                else
                {
                    lselectedCat = GeneTool.lCategoryDef_Factors;
                }

                GeneTool.lFreeStatDefFactors = statFactors.ListOfFreeCustom(DefTool.StatDefs_Selection(lselectedCat), DefTool.CompareStatModifier, DefTool.CompareStatCategoryNot, GeneTool.selected_StatFactor_CatDef);
            }

            bool flag6 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.StatOffsets;
            if (flag6)
            {
                ICollection<StatModifier> statOffsets = g.statOffsets;
                HashSet<StatCategoryDef> lselectedCat2;
                if (GeneTool.selected_StatOffset_CatDef != null)
                {
                    (lselectedCat2 = new HashSet<StatCategoryDef>()).Add(GeneTool.selected_StatOffset_CatDef);
                }
                else
                {
                    lselectedCat2 = GeneTool.lCategoryDef_Offsets;
                }

                GeneTool.lFreeStatDefOffsets = statOffsets.ListOfFreeCustom(DefTool.StatDefs_Selection(lselectedCat2), DefTool.CompareStatModifier, DefTool.CompareStatCategoryNot, GeneTool.selected_StatOffset_CatDef);
            }

            bool flag7 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.Aptitudes;
            if (flag7)
            {
                GeneTool.lFreeAptitudes = g.aptitudes.ListOfFree(DefTool.CompareAptitude);
            }

            bool flag8 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.DamageFactors;
            if (flag8)
            {
                GeneTool.lFreeDamageFactors = g.damageFactors.ListOfFree(DefTool.CompareDamageFactor);
            }

            bool flag9 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.Capacities;
            if (flag9)
            {
                GeneTool.lFreeCapacities = g.capMods.ListOfFree(DefTool.ComparePawnCapacityModifier);
            }

            bool flag10 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.Abilities;
            if (flag10)
            {
                GeneTool.lFreeAbilities = g.abilities.ListOfFreeDef<AbilityDef>();
            }

            bool flag11 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.Immunities;
            if (flag11)
            {
                GeneTool.lFreeImmunities = g.makeImmuneTo.ListOfFreeDef<HediffDef>();
            }

            bool flag12 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.Protections;
            if (flag12)
            {
                GeneTool.lFreeProtections = g.hediffGiversCannotGive.ListOfFreeDef<HediffDef>();
            }

            bool flag13 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.DamageFactors;
            if (flag13)
            {
                GeneTool.lFreeDamageFactors = g.damageFactors.ListOfFree(DefTool.CompareDamageFactor);
            }

            bool flag14 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.ForcedTraits;
            if (flag14)
            {
                GeneTool.lFreeForcedTraits = g.forcedTraits.ListOfFreeNonDef(TraitTool.ListAllAsGenticTraitData(), DefTool.CompareGeneticTraitData);
            }

            bool flag15 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.SuppressedTraits;
            if (flag15)
            {
                GeneTool.lFreeSuppressedTraits = g.suppressedTraits.ListOfFreeNonDef(TraitTool.ListAllAsGenticTraitData(), DefTool.CompareGeneticTraitData);
            }

            bool flag16 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.DisabledNeeds;
            if (flag16)
            {
                GeneTool.lFreeNeeds = g.disablesNeeds.ListOfFreeDef<NeedDef>();
            }

            bool flag17 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.ForcedHeadTypes;
            if (flag17)
            {
                GeneTool.lFreeForcedHeadTypes = g.forcedHeadTypes.ListOfFreeDef<HeadTypeDef>();
            }

            bool flag18 = f == GeneTool.FreeList.All || f == GeneTool.FreeList.DisabledWorkTags;
            if (flag18)
            {
                GeneTool.lFreeWorkTags = GeneTool.ListOfFreeDisabledWorkTags(g);
            }
        }
    }

    internal static void CopyList<T>(this List<T> lsource, ref List<T> ltarget)
    {
        ltarget = lsource == null ? null : lsource.ToList().ListFullCopy();
    }

    internal static void CopyHashSet<T>(this HashSet<T> lsource, ref HashSet<T> ltarget)
    {
        ltarget = lsource == null ? null : lsource.ToList().ListFullCopy().ToHashSet();
    }

    internal static void CopyStatOffsets(this GeneDef g)
    {
        g.statOffsets.CopyList(ref lCopyStatOffsets);
    }

    internal static void CopyStatFactors(this GeneDef g)
    {
        g.statFactors.CopyList(ref lCopyStatFactors);
    }

    internal static void CopyAptitudes(this GeneDef g)
    {
        g.aptitudes.CopyList(ref lCopyAptitude);
    }

    internal static void CopyDamageFactors(this GeneDef g)
    {
        g.damageFactors.CopyList(ref lCopyDamageFactors);
    }

    internal static void CopyCapacities(this GeneDef g)
    {
        g.capMods.CopyList(ref lCopyCapacities);
    }

    internal static void CopyAbilities(this GeneDef g)
    {
        g.abilities.CopyList(ref lCopyAbilities);
    }

    internal static void CopyForcedTraits(this GeneDef g)
    {
        g.forcedTraits.CopyList(ref lCopyForcedTraits);
    }

    internal static void CopySuppressedTraits(this GeneDef g)
    {
        g.suppressedTraits.CopyList(ref lCopySuppressedTraits);
    }

    internal static void CopyProtections(this GeneDef g)
    {
        g.hediffGiversCannotGive.CopyList(ref lCopyProtections);
    }

    internal static void CopyImmunities(this GeneDef g)
    {
        g.makeImmuneTo.CopyList(ref lCopyImmunities);
    }

    internal static void CopyDisabledNeeds(this GeneDef g)
    {
        g.disablesNeeds.CopyList(ref lCopyDisabledNeeds);
    }

    internal static void CopyForcedHeadTypes(this GeneDef g)
    {
        g.forcedHeadTypes.CopyList(ref lCopyForcedHeadTypes);
    }

    internal static void CopyCustomEffectDescriptions(this GeneDef g)
    {
        g.customEffectDescriptions.CopyList(ref lCopyCustomEffectDescriptions);
    }

    internal static void CopyExclusionTags(this GeneDef g)
    {
        g.exclusionTags.CopyList(ref lCopyExclusionTags);
    }

    internal static void CopyHairTags(this GeneDef g)
    {
        if (g.hairTagFilter == null)
            return;
        g.hairTagFilter.tags.CopyList(ref lCopyHairTags);
    }

    internal static void CopyBeardTags(this GeneDef g)
    {
        if (g.beardTagFilter == null)
            return;
        g.beardTagFilter.tags.CopyList(ref lCopyBeardTags);
    }

    internal static void CopyGizmoThres(this GeneDef g)
    {
        g.resourceGizmoThresholds.CopyList(ref lCopyGizmoThres);
    }

    internal static void CopyDisabledWorkTags(this GeneDef g)
    {
        lCopyDisabledWorkTags = g == null ? 0 : (WorkTags)(int)g.disabledWorkTags;
    }

    internal static void PasteListNonDefMulti<T1, T, T2, T3>(
        this List<T1> ltarget,
        List<T1> lsource,
        Func<T1, T, bool> comparator,
        Action<List<T1>, T1, T, T2, T3> valueSetter,
        Func<T1, T> defGetter,
        Func<T1, T2> valGetter1,
        Func<T1, T3> valGetter2)
        where T : Def
    {
        if (lsource.NullOrEmpty())
            return;
        foreach (var obj in lsource)
            ltarget.SetMulti(comparator, valueSetter, defGetter(obj), valGetter1(obj), valGetter2(obj));
    }

    internal static void PasteListNonDef<T1, T, T2>(
        this List<T1> ltarget,
        List<T1> lsource,
        Func<T1, T, bool> comparator,
        Action<List<T1>, T1, T, T2> valueSetter,
        Func<T1, T> defGetter,
        Func<T1, T2> valGetter)
        where T : Def
    {
        if (lsource.NullOrEmpty())
            return;
        foreach (var obj in lsource)
            ltarget.Set(comparator, valueSetter, defGetter(obj), valGetter(obj));
    }

    internal static void PasteListNonDef2<T1, T, T2>(
        this List<T1> ltarget,
        List<T1> lsource,
        Func<T1, T1, bool> comparator,
        Action<List<T1>, T1, T, T2> valueSetter,
        Func<T1, T> defGetter,
        Func<T1, T2> valGetter)
        where T : Def
    {
        if (lsource.NullOrEmpty())
            return;
        foreach (var obj in lsource)
            ltarget.Set(comparator, valueSetter, obj, defGetter(obj), valGetter(obj));
    }

    internal static void PasteList<T>(this List<T> ltarget, List<T> lsource) where T : Def
    {
        if (lsource.NullOrEmpty<T>())
            return;
        foreach (T def in lsource)
            ltarget.AddDef(def);
    }

    internal static void PasteList(this List<string> ltarget, List<string> lsource)
    {
        if (lsource.NullOrEmpty())
            return;
        foreach (var str in lsource)
            ltarget.Add(str);
    }

    internal static void PasteList(this List<float> ltarget, List<float> lsource)
    {
        if (lsource.NullOrEmpty())
            return;
        foreach (var num in lsource)
            ltarget.Add(num);
    }

    internal static void PasteDisabledWorkTags(this GeneDef g)
    {
        g.disabledWorkTags = lCopyDisabledWorkTags;
        g.UpdateFreeLists(FreeList.DisabledWorkTags);
    }

    internal static void PasteDisabledNeeds(this GeneDef g)
    {
        if (g.disablesNeeds == null)
            g.disablesNeeds = new List<NeedDef>();
        g.disablesNeeds.PasteList(lCopyDisabledNeeds);
        g.UpdateFreeLists(FreeList.DisabledNeeds);
    }

    internal static void PasteForcedHeadTypes(this GeneDef g)
    {
        if (g.forcedHeadTypes == null)
            g.forcedHeadTypes = new List<HeadTypeDef>();
        g.forcedHeadTypes.PasteList(lCopyForcedHeadTypes);
        g.UpdateFreeLists(FreeList.ForcedHeadTypes);
    }

    internal static void PasteCustomEffectDescriptions(this GeneDef g)
    {
        if (g.customEffectDescriptions == null)
            g.customEffectDescriptions = new List<string>();
        g.customEffectDescriptions.PasteList(lCopyCustomEffectDescriptions);
    }

    internal static void PasteExclusionTags(this GeneDef g)
    {
        if (g.exclusionTags == null)
            g.exclusionTags = new List<string>();
        g.exclusionTags.PasteList(lCopyExclusionTags);
    }

    internal static void PasteGizmoThres(this GeneDef g)
    {
        if (g.resourceGizmoThresholds == null)
            g.resourceGizmoThresholds = new List<float>();
        g.resourceGizmoThresholds.PasteList(lCopyGizmoThres);
    }

    internal static void PasteHairTags(this GeneDef g)
    {
        if (g.hairTagFilter == null)
            g.hairTagFilter = new TagFilter();
        g.hairTagFilter.tags.PasteList(lCopyHairTags);
    }

    internal static void PasteBeardTags(this GeneDef g)
    {
        if (g.beardTagFilter == null)
            g.beardTagFilter = new TagFilter();
        g.beardTagFilter.tags.PasteList(lCopyBeardTags);
    }

    internal static void PasteProtections(this GeneDef g)
    {
        if (g.hediffGiversCannotGive == null)
            g.hediffGiversCannotGive = new List<HediffDef>();
        g.hediffGiversCannotGive.PasteList(lCopyProtections);
        g.UpdateFreeLists(FreeList.Protections);
    }

    internal static void PasteImmunities(this GeneDef g)
    {
        if (g.makeImmuneTo == null)
            g.makeImmuneTo = new List<HediffDef>();
        g.makeImmuneTo.PasteList(lCopyImmunities);
        g.UpdateFreeLists(FreeList.Immunities);
    }

    internal static void PasteAbilities(this GeneDef g)
    {
        if (g.abilities == null)
            g.abilities = new List<AbilityDef>();
        g.abilities.PasteList(lCopyAbilities);
        g.UpdateFreeLists(FreeList.Abilities);
    }

    internal static void PasteForcedTraits(this GeneDef g)
    {
        if (g.forcedTraits == null)
            g.forcedTraits = new List<GeneticTraitData>();
        g.forcedTraits.PasteListNonDef2(lCopyForcedTraits, DefTool.CompareGeneticTraitData, DefTool.SetGeneticTraitData, DefTool.DefGetterGeneticTraitData, DefTool.ValGetterGeneticTraitData);
        g.UpdateFreeLists(FreeList.ForcedTraits);
    }

    internal static void PasteSuppressedTraits(this GeneDef g)
    {
        if (g.suppressedTraits == null)
            g.suppressedTraits = new List<GeneticTraitData>();
        g.suppressedTraits.PasteListNonDef2(lCopySuppressedTraits, DefTool.CompareGeneticTraitData, DefTool.SetGeneticTraitData, DefTool.DefGetterGeneticTraitData, DefTool.ValGetterGeneticTraitData);
        g.UpdateFreeLists(FreeList.SuppressedTraits);
    }

    internal static void PasteAptitude(this GeneDef g)
    {
        if (g.aptitudes == null)
            g.aptitudes = new List<Aptitude>();
        g.aptitudes.PasteListNonDef(lCopyAptitude, DefTool.CompareAptitude, DefTool.SetAptitude, DefTool.DefGetterAptitude, DefTool.ValGetterAptitude);
        g.UpdateFreeLists(FreeList.Aptitudes);
    }

    internal static void PasteDamageFactors(this GeneDef g)
    {
        if (g.damageFactors == null)
            g.damageFactors = new List<DamageFactor>();
        g.damageFactors.PasteListNonDef(lCopyDamageFactors, DefTool.CompareDamageFactor, DefTool.SetDamageFactor, DefTool.DefGetterDamageFactor, DefTool.ValGetterDamageFactor);
        g.UpdateFreeLists(FreeList.DamageFactors);
    }

    internal static void PasteStatFactors(this GeneDef g)
    {
        if (g.statFactors == null)
            g.statFactors = new List<StatModifier>();
        g.statFactors.PasteListNonDef(lCopyStatFactors, DefTool.CompareStatModifier, DefTool.SetStatModifier, DefTool.DefGetterStatModifier, DefTool.ValGetterStatModifier);
        g.UpdateFreeLists(FreeList.StatFactors);
    }

    internal static void PasteStatOffsets(this GeneDef g)
    {
        if (g.statOffsets == null)
            g.statOffsets = new List<StatModifier>();
        g.statOffsets.PasteListNonDef(lCopyStatOffsets, DefTool.CompareStatModifier, DefTool.SetStatModifier, DefTool.DefGetterStatModifier, DefTool.ValGetterStatModifier);
        g.UpdateFreeLists(FreeList.StatOffsets);
    }

    internal static void PasteCapacities(this GeneDef g)
    {
        if (g.capMods == null)
            g.capMods = new List<PawnCapacityModifier>();
        g.capMods.PasteListNonDefMulti(lCopyCapacities, DefTool.ComparePawnCapacityModifier, DefTool.SetPawnCapacityModifier, DefTool.DefGetterPawnCapacityModifier, DefTool.ValGetterPCMoffset, DefTool.ValGetterPCMfactor);
        g.UpdateFreeLists(FreeList.Capacities);
    }

    internal static HashSet<ConditionalStatAffecter> ListConditionalAffectors()
    {
        var conditionalStatAffecterSet = new HashSet<ConditionalStatAffecter>();
        conditionalStatAffecterSet.Add(new ConditionalStatAffecter_Child());
        conditionalStatAffecterSet.Add(new ConditionalStatAffecter_Clothed());
        conditionalStatAffecterSet.Add(new ConditionalStatAffecter_Unclothed());
        conditionalStatAffecterSet.Add(new ConditionalStatAffecter_InSunlight());
        return conditionalStatAffecterSet;
    }

    internal static HashSet<T> ListOfFreeCustom<T1, T, T2>(this ICollection<T1> otherList, ICollection<T> startList, Func<T1, T, bool> comparator, Func<T, T2, bool> comparator2, T2 otherDef) where T : Def
    {
        bool flag = !otherList.EnumerableNullOrEmpty<T1>();
        if (flag)
        {
            bool flag2 = otherDef != null;
            for (int i = 0; i < startList.Count; i++)
            {
                T t = startList.ElementAt(i);
                bool flag3 = otherList.FindBy(comparator, t) != null;
                if (flag3)
                {
                    startList.Remove(t);
                    i--;
                }
                else
                {
                    bool flag4 = flag2 && comparator2(t, otherDef);
                    if (flag4)
                    {
                        startList.Remove(t);
                        i--;
                    }
                }
            }
        }

        return startList.ToHashSet<T>();
    }

    internal static HashSet<T> ListOfFree<T1, T>(this List<T1> otherList, Func<T1, T, bool> comparator) where T : Def
    {
        HashSet<T> hashSet = DefTool.ListBy<T>((T x) => !x.defName.NullOrEmpty());
        bool flag = !otherList.NullOrEmpty<T1>();
        if (flag)
        {
            for (int i = 0; i < hashSet.Count; i++)
            {
                T t = hashSet.At(i);
                bool flag2 = otherList.FindBy(comparator, t) != null;
                if (flag2)
                {
                    hashSet.Remove(t);
                    i--;
                }
            }
        }

        return hashSet;
    }


    internal static HashSet<T> ListOfFreeNonDef<T>(
        this ICollection<T> otherList,
        ICollection<T> startList,
        Func<T, T, bool> comparator)
    {
        if (!otherList.EnumerableNullOrEmpty())
            for (var index = 0; index < startList.Count; ++index)
            {
                var obj = startList.ElementAt(index);
                if (otherList.FindBy(comparator, obj) != null)
                {
                    startList.Remove(obj);
                    --index;
                }
            }

        return startList.ToHashSet();
    }

    internal static HashSet<T> ListOfFreeDef<T>(this List<T> otherList) where T : Def
    {
        var l = DefTool.ListBy((Func<T, bool>)(x => !x.defName.NullOrEmpty()));
        if (!otherList.NullOrEmpty())
            for (var index = 0; index < l.Count; ++index)
            {
                var obj = l.At(index);
                if (otherList.Contains(obj))
                {
                    l.Remove(obj);
                    --index;
                }
            }

        return l;
    }

    internal static HashSet<WorkTags> ListOfFreeDisabledWorkTags(GeneDef g)
    {
        if (g == null)
            return new HashSet<WorkTags>();
        var values = Enum.GetValues(typeof(WorkTags));
        var workTagsSet = new HashSet<WorkTags>();
        foreach (WorkTags workTags in values)
            if (workTags != (g.disabledWorkTags & workTags))
                workTagsSet.Add(workTags);
        return workTagsSet;
    }

    internal static HashSet<StatCategoryDef> ListOfStatCategoryDef_ForStatOffset(
        bool isMechanoid,
        bool all)
    {
        var lcat = DefTool.StatCategoryDefs_Selection(List_StatCategories_ToSkip(!isMechanoid, all));
        lcat.RemoveCategoriesWithNoStatDef();
        return lcat;
    }

    internal static HashSet<StatCategoryDef> ListOfStatCategoryDef_ForStatFactor(
        bool isMechanoid,
        bool all)
    {
        var lcat = DefTool.StatCategoryDefs_Selection(List_StatCategories_ToSkip(!isMechanoid, all));
        lcat.RemoveCategoriesWithNoStatDef();
        return lcat;
    }

    internal static HashSet<StatCategoryDef> List_StatCategories_ToSkip(
        bool skipMechnoid,
        bool allowAll)
    {
        var statCategoryDefSet1 = new HashSet<StatCategoryDef>();
        HashSet<StatCategoryDef> statCategoryDefSet2;
        if (allowAll)
        {
            statCategoryDefSet2 = statCategoryDefSet1;
        }
        else
        {
            statCategoryDefSet1.Add(StatCategoryDefOf.StuffStatOffsets);
            statCategoryDefSet1.Add(StatCategoryDefOf.EquippedStatOffsets);
            statCategoryDefSet1.Add(StatCategoryDefOf.BasicsNonPawn);
            statCategoryDefSet1.Add(StatCategoryDefOf.Building);
            statCategoryDefSet1.Add(StatCategoryDefOf.Terrain);
            statCategoryDefSet1.Add(StatCategoryDefOf.Apparel);
            statCategoryDefSet1.Add(StatCategoryDefOf.StuffStatFactors);
            statCategoryDefSet1.Add(StatCategoryDefOf.Weapon_Ranged);
            statCategoryDefSet1.Add(StatCategoryDefOf.Weapon_Melee);
            statCategoryDefSet1.Add(StatCategoryDefOf.Ability);
            statCategoryDefSet1.Add(StatCategoryDefOf.BasicsNonPawnImportant);
            statCategoryDefSet1.Add(StatCategoryDefOf.Meditation);
            statCategoryDefSet1.Add(DefTool.GetDef<StatCategoryDef>("Mechanitor"));
            if (skipMechnoid)
                statCategoryDefSet1.Add(StatCategoryDefOf.Mechanoid);
            statCategoryDefSet2 = statCategoryDefSet1;
        }

        return statCategoryDefSet2;
    }

    internal static List<string> GetAllExclusionTags()
    {
        var dic = new SortedDictionary<string, int>();
        foreach (var allDef in DefDatabase<GeneDef>.AllDefs)
            dic.AddFromList(allDef.exclusionTags);
        return dic.Keys.ToList();
    }

    internal static List<string> GetAllHairTags()
    {
        var dic = new SortedDictionary<string, int>();
        foreach (var allDef in DefDatabase<GeneDef>.AllDefs)
            if (allDef.hairTagFilter != null)
                dic.AddFromList(allDef.hairTagFilter.tags);
        return dic.Keys.ToList();
    }

    internal static List<string> GetAllBeardTags()
    {
        var dic = new SortedDictionary<string, int>();
        foreach (var allDef in DefDatabase<GeneDef>.AllDefs)
            if (allDef.beardTagFilter != null)
                dic.AddFromList(allDef.beardTagFilter.tags);
        return dic.Keys.ToList();
    }

    internal static void SetSuppressedTrait(this GeneDef g, GeneticTraitData gtd, TraitDef t, int degree)
    {
        DefTool.Set<GeneticTraitData, TraitDef, int>(ref g.suppressedTraits, DefTool.CompareGeneticTraitData, new Action<List<GeneticTraitData>, GeneticTraitData, TraitDef, int>(DefTool.SetGeneticTraitData), gtd, t, degree);
        g.UpdateFreeLists(GeneTool.FreeList.SuppressedTraits);
    }

    internal static void SetForcedTrait(this GeneDef g, GeneticTraitData gtd, TraitDef t, int degree)
    {
        DefTool.Set<GeneticTraitData, TraitDef, int>(ref g.forcedTraits, DefTool.CompareGeneticTraitData, new Action<List<GeneticTraitData>, GeneticTraitData, TraitDef, int>(DefTool.SetGeneticTraitData), gtd, t, degree);
        g.UpdateFreeLists(GeneTool.FreeList.ForcedTraits);
    }

    internal static void SetDisabledNeed(this GeneDef g, NeedDef n)
    {
        if (g.disablesNeeds == null)
            g.disablesNeeds = new List<NeedDef>();
        g.disablesNeeds.AddDef(n);
        g.UpdateFreeLists(FreeList.DisabledNeeds);
        g.DoGeneActionForAllPawns(RecalcNeeds);
    }

    internal static void SetForcedHeadType(this GeneDef g, HeadTypeDef h)
    {
        if (g.forcedHeadTypes == null)
            g.forcedHeadTypes = new List<HeadTypeDef>();
        g.forcedHeadTypes.AddDef(h);
        g.UpdateFreeLists(FreeList.ForcedHeadTypes);
    }

    internal static void SetProtection(this GeneDef g, HediffDef h)
    {
        if (g.hediffGiversCannotGive == null)
            g.hediffGiversCannotGive = new List<HediffDef>();
        g.hediffGiversCannotGive.AddDef(h);
        g.UpdateFreeLists(FreeList.Protections);
    }

    internal static void SetImmunity(this GeneDef g, HediffDef h)
    {
        if (g.makeImmuneTo == null)
            g.makeImmuneTo = new List<HediffDef>();
        g.makeImmuneTo.AddDef(h);
        g.UpdateFreeLists(FreeList.Immunities);
    }

    internal static void SetAbility(this GeneDef g, AbilityDef a)
    {
        if (g.abilities == null)
            g.abilities = new List<AbilityDef>();
        g.abilities.AddDef(a);
        g.UpdateFreeLists(FreeList.Abilities);
    }


    internal static void SetCapacity(this GeneDef g, PawnCapacityDef c, float offset, float factor)
    {
        DefTool.SetMulti<PawnCapacityModifier, PawnCapacityDef, float, float>(ref g.capMods, DefTool.ComparePawnCapacityModifier, new Action<List<PawnCapacityModifier>, PawnCapacityModifier, PawnCapacityDef, float, float>(DefTool.SetPawnCapacityModifier), c, offset, factor);
        g.UpdateFreeLists(GeneTool.FreeList.Capacities);
    }

    
    internal static void SetAptitude(this GeneDef g, SkillDef s, int level)
    {
        DefTool.Set<Aptitude, SkillDef, int>(ref g.aptitudes, DefTool.CompareAptitude, new Action<List<Aptitude>, Aptitude, SkillDef, int>(DefTool.SetAptitude), s, level);
        g.UpdateFreeLists(GeneTool.FreeList.Aptitudes);
    }

    
    internal static void SetDamageFactor(this GeneDef g, DamageDef d, float factor)
    {
        DefTool.Set<DamageFactor, DamageDef, float>(ref g.damageFactors, DefTool.CompareDamageFactor, new Action<List<DamageFactor>, DamageFactor, DamageDef, float>(DefTool.SetDamageFactor), d, factor);
        g.UpdateFreeLists(GeneTool.FreeList.DamageFactors);
    }

    
    internal static void SetStatOffset(this GeneDef g, StatDef s, float offset)
    {
        DefTool.Set<StatModifier, StatDef, float>(ref g.statOffsets, DefTool.CompareStatModifier, new Action<List<StatModifier>, StatModifier, StatDef, float>(DefTool.SetStatModifier), s, offset);
        g.UpdateFreeLists(GeneTool.FreeList.StatOffsets);
    }

    
    internal static void SetStatFactor(this GeneDef g, StatDef s, float factor)
    {
        DefTool.Set<StatModifier, StatDef, float>(ref g.statFactors, DefTool.CompareStatModifier, new Action<List<StatModifier>, StatModifier, StatDef, float>(DefTool.SetStatModifier), s, factor);
        g.UpdateFreeLists(GeneTool.FreeList.StatFactors);
    }


    internal static void SetPassionMod(
        this GeneDef g,
        SkillDef skill,
        PassionMod.PassionModType type)
    {
        if (g == null)
            return;
        if (g.passionMod == null)
            g.passionMod = new PassionMod();
        g.passionMod.skill = skill;
        g.passionMod.modType = type;
        if (skill == null)
            g.passionMod = null;
        g.ResolveReferences();
        if (skill == null)
            return;
        g.DoGeneActionForAllPawns(RecalcPassion);
    }

    internal static void SetCausesNeed(this GeneDef g, NeedDef need)
    {
        if (g == null)
            return;
        g.causesNeed = need;
        g.ResolveReferences();
        g.DoGeneActionForAllPawns(RecalcNeeds);
    }

    internal static void SetChemicalDef(this GeneDef g, ChemicalDef chemical)
    {
        if (g == null)
            return;
        g.chemical = chemical;
        g.ResolveReferences();
        g.DoGeneActionForAllPawns(RecalcNeeds);
    }

    internal static void SetForcedHairDef(this GeneDef g, HairDef hair)
    {
        if (g == null)
            return;
        g.forcedHair = hair;
        g.ResolveReferences();
        g.DoGeneActionForAllPawns(RecalcForcedHair);
    }

    internal static void SetDisabledWorkTags(this GeneDef g, WorkTags workTag)
    {
        if (g == null || workTag == 0)
            return;
        var list = g.disabledWorkTags.GetAllSelectedItems<WorkTags>().ToList();
        if (!list.Contains(workTag))
            list.Add(workTag);
        list.Remove(0);
        var num = 0;
        foreach (var workTags in list)
            num += (int)workTags;
        g.disabledWorkTags = (WorkTags)Enum.Parse(typeof(WorkTags), num.ToString());
        g.UpdateFreeLists(FreeList.DisabledWorkTags);
        g.DoGeneActionForAllPawns(RecalcWork);
    }

    internal static void RemoveStatOffset(this GeneDef g, StatDef def)
    {
        var by = g.statOffsets.FindBy(DefTool.CompareStatModifier, def);
        if (by == null)
            return;
        g.statOffsets.Remove(by);
        g.UpdateFreeLists(FreeList.StatOffsets);
    }

    internal static void RemoveStatFactor(this GeneDef g, StatDef def)
    {
        var by = g.statFactors.FindBy(DefTool.CompareStatModifier, def);
        if (by == null)
            return;
        g.statFactors.Remove(by);
        g.UpdateFreeLists(FreeList.StatFactors);
    }

    internal static void RemoveAptitude(this GeneDef g, SkillDef def)
    {
        var by = g.aptitudes.FindBy(DefTool.CompareAptitude, def);
        if (by == null)
            return;
        g.aptitudes.Remove(by);
        g.UpdateFreeLists(FreeList.Aptitudes);
    }

    internal static void RemoveDamageFactor(this GeneDef g, DamageDef def)
    {
        var by = g.damageFactors.FindBy(DefTool.CompareDamageFactor, def);
        if (by == null)
            return;
        g.damageFactors.Remove(by);
        g.UpdateFreeLists(FreeList.DamageFactors);
    }

    internal static void RemoveCapacity(this GeneDef g, PawnCapacityDef def)
    {
        var by = g.capMods.FindBy(DefTool.ComparePawnCapacityModifier, def);
        if (by == null)
            return;
        g.capMods.Remove(by);
        g.UpdateFreeLists(FreeList.Capacities);
    }

    internal static void RemoveAbility(this GeneDef g, AbilityDef def)
    {
        AbilityDef abilityDef = g.abilities.FindByDef(def);
        bool flag = abilityDef != null;
        if (flag)
        {
            g.abilities.Remove(abilityDef);
            g.UpdateFreeLists(GeneTool.FreeList.Abilities);
        }
    }

    internal static void RemoveForcedTrait(this GeneDef g, GeneticTraitData gtd)
    {
        var by = g.forcedTraits.FindBy(DefTool.CompareGeneticTraitData, gtd);
        if (by == null)
            return;
        g.forcedTraits.Remove(by);
        g.UpdateFreeLists(FreeList.ForcedTraits);
    }

    internal static void RemoveSuppressedTrait(this GeneDef g, GeneticTraitData gtd)
    {
        var by = g.suppressedTraits.FindBy(DefTool.CompareGeneticTraitData, gtd);
        if (by == null)
            return;
        g.suppressedTraits.Remove(by);
        g.UpdateFreeLists(FreeList.SuppressedTraits);
    }

    internal static void RemoveProtection(this GeneDef g, HediffDef def)
    {
        HediffDef hediffDef = g.hediffGiversCannotGive.FindByDef(def);
        bool flag = hediffDef != null;
        if (flag)
        {
            g.hediffGiversCannotGive.Remove(hediffDef);
            g.UpdateFreeLists(GeneTool.FreeList.Protections);
        }
    }

    
    internal static void RemoveImmunity(this GeneDef g, HediffDef def)
    {
        HediffDef hediffDef = g.makeImmuneTo.FindByDef(def);
        bool flag = hediffDef != null;
        if (flag)
        {
            g.makeImmuneTo.Remove(hediffDef);
            g.UpdateFreeLists(GeneTool.FreeList.Immunities);
        }
    }

    
    internal static void RemoveDisabledNeed(this GeneDef g, NeedDef def)
    {
        NeedDef needDef = g.disablesNeeds.FindByDef(def);
        bool flag = needDef != null;
        if (flag)
        {
            g.disablesNeeds.Remove(needDef);
            g.UpdateFreeLists(GeneTool.FreeList.DisabledNeeds);
            g.DoGeneActionForAllPawns(new Action<Pawn, GeneDef>(GeneTool.RecalcNeeds));
        }
    }

    
    internal static void RemoveForcedHeadType(this GeneDef g, HeadTypeDef def)
    {
        HeadTypeDef headTypeDef = g.forcedHeadTypes.FindByDef(def);
        bool flag = headTypeDef != null;
        if (flag)
        {
            g.forcedHeadTypes.Remove(headTypeDef);
            g.UpdateFreeLists(GeneTool.FreeList.ForcedHeadTypes);
        }
    }

    internal static void RemoveDisabledWorkTags(this GeneDef g, WorkTags workTag)
    {
        if (g == null || workTag == 0)
            return;
        var list = g.disabledWorkTags.GetAllSelectedItems<WorkTags>().ToList();
        if (list.Contains(workTag))
            list.Remove(workTag);
        var num = 0;
        foreach (var workTags in list)
            num += (int)workTags;
        g.disabledWorkTags = (WorkTags)Enum.Parse(typeof(WorkTags), num.ToString());
        g.UpdateFreeLists(FreeList.DisabledWorkTags);
        g.DoGeneActionForAllPawns(RecalcWork);
    }

    internal static void DoAllGeneActions(this GeneDef g)
    {
        g.DoGeneActionForAllPawns(RecalcNeeds);
        g.DoGeneActionForAllPawns(RecalcWork);
        g.DoGeneActionForAllPawns(RecalcForcedHair);
    }

    internal static void RecalcForcedHair(Pawn p, GeneDef g)
    {
        p.SetHair(g.forcedHair);
    }

    internal static void RecalcWork(Pawn p, GeneDef g)
    {
        p.Recalculate_WorkTypes();
    }

    internal static void RecalcNeeds(Pawn p, GeneDef g)
    {
        p.needs?.AddOrRemoveNeedsAsAppropriate();
    }

    internal static void RecalcPassion(Pawn p, GeneDef g)
    {
        var skill = p.skills.GetSkill(g.passionMod.skill);
        skill.passion = g.passionMod.NewPassionFor(skill);
    }

    internal static HashSet<GeneticBodyType?> ListOfGeneticBodyType()
    {
        var hashSet = EnumTool.GetAllEnumsOfType<GeneticBodyType>().ToHashSet();
        var nullableSet = new HashSet<GeneticBodyType?>();
        nullableSet.Add(new GeneticBodyType?());
        foreach (var geneticBodyType in hashSet)
            nullableSet.Add(geneticBodyType);
        return nullableSet;
    }

    internal static bool IsBodySizeGene(this GeneDef g)
    {
        return g != null && !g.defName.NullOrEmpty() && g.defName.StartsWith("SZBodySize_");
    }

    internal static LifeStageDef GetLifeStageDef(this GeneDef g)
    {
        return !g.IsBodySizeGene() ? null : DefTool.GetDef<LifeStageDef>("SZHumanSize_" + g.defName.SubstringFrom("SZBodySize_"));
    }

    internal static XenotypeDef GetXenoTypeDef(this Pawn pawn)
    {
        return !pawn.HasGeneTracker() ? null : pawn.genes.Xenotype;
    }

    internal static string GetXenoTypeDefName(this Pawn pawn)
    {
        var xenoTypeDef = pawn.GetXenoTypeDef();
        return xenoTypeDef == null ? "" : xenoTypeDef.defName;
    }

    internal static string GetXenoCustomName(this Pawn pawn)
    {
        return !pawn.HasGeneTracker() ? "" : pawn.genes.xenotypeName ?? "";
    }

    internal static string GetXenogeneAsSeparatedString(this Pawn p)
    {
        if (!p.HasGeneTracker())
            return "";
        var text = "";
        foreach (var xenogene in p.genes.Xenogenes)
            text = text + xenogene.def.defName + ":";
        return text.SubstringRemoveLast();
    }

    internal static string GetEndogeneAsSeparatedString(this Pawn p)
    {
        if (!p.HasGeneTracker())
            return "";
        var text = "";
        foreach (var endogene in p.genes.Endogenes)
            text = text + endogene.def.defName + ":";
        return text.SubstringRemoveLast();
    }

    internal static void SetXenogeneFromSeparatedString(this Pawn p, string s)
    {
        if (!p.HasGeneTracker())
            return;
        var str = s;
        var separator = new string[1] { ":" };
        foreach (var defName in str.Split(separator, StringSplitOptions.None))
        {
            var geneDef = DefTool.GeneDef(defName);
            if (geneDef != null)
                p.genes.AddGene(geneDef, true);
        }
    }

    internal static void SetEndogeneFromSeparatedString(this Pawn p, string s)
    {
        if (!p.HasGeneTracker())
            return;
        var str = s;
        var separator = new string[1] { ":" };
        foreach (var defName in str.Split(separator, StringSplitOptions.None))
        {
            var geneDef = DefTool.GeneDef(defName);
            if (geneDef != null)
                p.genes.AddGene(geneDef, false);
        }
    }

    internal static void ClearXenogenes(this Pawn p)
    {
        if (!p.HasGeneTracker())
            return;
        for (var index = p.genes.Xenogenes.Count - 1; index >= 0; --index)
            p.genes.RemoveGene(p.genes.Xenogenes[index]);
    }

    internal static void ClearEndogenes(this Pawn p)
    {
        if (!p.HasGeneTracker())
            return;
        for (var index = p.genes.Endogenes.Count - 1; index >= 0; --index)
            p.genes.RemoveGene(p.genes.Endogenes[index]);
    }

    internal static List<Gene> GetHairGenes(this Pawn p)
    {
        return p.HasGeneTracker() ? p.genes.GenesListForReading.Where(td => td.IsHairGene()).OrderByDescending(td => !td.Overridden && td.Active).ToList() : new List<Gene>();
    }

    internal static List<Gene> GetSkinGenes(this Pawn p)
    {
        return p.HasGeneTracker() ? p.genes.GenesListForReading.Where(td => td.IsSkinGene()).OrderByDescending(td => !td.Overridden && td.Active).ToList() : new List<Gene>();
    }

    internal static List<GeneDef> GetAllSkinGenes()
    {
        return DefTool.ListBy((Func<GeneDef, bool>)(x => x.IsSkinGene())).ToList();
    }

    internal static List<GeneDef> GetAllHairGenes()
    {
        return DefTool.ListBy((Func<GeneDef, bool>)(x => x.IsHairGene())).ToList();
    }

    internal static bool IsSkinGene(this GeneDef g)
    {
        if (g == null)
            return false;
        if (g.endogeneCategory == EndogeneCategory.Melanin)
            return true;
        return !g.defName.NullOrEmpty() && g.defName.StartsWith("Skin_");
    }

    internal static bool IsHairGene(this GeneDef g)
    {
        return g != null && g.endogeneCategory == EndogeneCategory.HairColor;
    }

    internal static bool IsSkinGene(this Gene g)
    {
        return g.def.IsSkinGene();
    }

    internal static bool IsHairGene(this Gene g)
    {
        return g.def.IsHairGene();
    }

    internal static void ClearGenes(this Pawn p, bool xeno, bool keepHairAndSkin)
    {
        if (keepHairAndSkin)
        {
            var geneList = xeno ? p.genes.Xenogenes : p.genes.Endogenes;
            var flag1 = false;
            var flag2 = false;
            for (var index = geneList.Count - 1; index >= 0; --index)
            {
                var g = geneList[index];
                var flag3 = g.IsHairGene();
                var flag4 = g.IsSkinGene();
                if (flag4 && !flag1)
                    flag1 = true;
                else if (flag3 && !flag2)
                    flag2 = true;
                else if ((!flag3 && !flag4) || flag1 & flag4 || flag2 & flag3)
                    p.genes.RemoveGene(g);
            }
        }
        else if (xeno)
        {
            p.ClearXenogenes();
        }
        else
        {
            p.ClearEndogenes();
        }
    }

    internal static void PreGeneChange_HeadAndBodyTest(
        this Pawn p,
        out HeadTypeDef oldHead,
        out BodyTypeDef oldBody)
    {
        oldHead = p.story?.headType;
        oldBody = p.story?.bodyType;
    }

    internal static void PostGeneChange_HeadAndBodyTest(
        this Pawn p,
        HeadTypeDef oldHead,
        BodyTypeDef oldBody)
    {
        var headType = p.story?.headType;
        var bodyType = p.story?.bodyType;
        var headDefList = p.GetHeadDefList();
        if (!headDefList.Contains(headType))
        {
            if (headDefList.Contains(oldHead))
                p.SetHeadTypeDef(oldHead);
            else
                p.SetHeadTypeDef(headDefList.RandomElement());
        }

        var bodyDefList = p.GetBodyDefList();
        if (bodyDefList.Contains(bodyType))
            return;
        if (bodyDefList.Contains(oldBody))
            p.SetBody(oldBody);
        else
            p.SetBody(bodyDefList.RandomElement());
    }

    internal static void PresetXenoType(
        this Pawn pawn,
        string defName,
        string name,
        bool clearXeno = true,
        bool clearEndo = true)
    {
        if (!pawn.HasGeneTracker())
            return;
        if (clearXeno)
            pawn.ClearXenogenes();
        if (clearEndo)
            pawn.ClearEndogenes();
        pawn.genes.Reset();
        var xenotypeDef = defName.NullOrEmpty() ? null : DefTool.XenotypeDef(defName);
        name = name.NullOrEmpty() ? null : name;
        pawn.genes.SetXenotypeDirect(xenotypeDef);
        pawn.genes.xenotypeName = name;
        pawn.genes.iconDef = null;
        if (xenotypeDef != null && xenotypeDef != XenotypeDefOf.Baseliner)
            return;
        if (Prefs.DevMode)
            Log.Message("loading icon for custom xenotype " + name);
        foreach (var allCustomXenotype in GetAllCustomXenotypes())
            if (allCustomXenotype.name == name)
            {
                pawn.genes.iconDef = allCustomXenotype.iconDef;
                break;
            }
    }

    internal static List<CustomXenotype> GetAllCustomXenotypes()
    {
        var lcustomxeontypes = new List<CustomXenotype>();
        try
        {
            foreach (var fileInfo in GenFilePaths.AllCustomXenotypeFiles.OrderBy(f => f.LastWriteTime))
            {
                var filePath = GenFilePaths.AbsFilePathForXenotype(Path.GetFileNameWithoutExtension(fileInfo.Name));
                PreLoadUtility.CheckVersionAndLoad(filePath, (ScribeMetaHeaderUtility.ScribeHeaderMode)5, () =>
                {
                    if (!GameDataSaveLoader.TryLoadXenotype(filePath, out var customXenotype))
                        return;
                    lcustomxeontypes.Add(customXenotype);
                }, true);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message + "\n" + ex.StackTrace);
        }

        return lcustomxeontypes;
    }

    internal static void SetGenesFromList(this Pawn p, List<GeneDef> l, bool asXeno, bool keepOld = false)
    {
        if (!keepOld)
        {
            if (asXeno)
                p.ClearXenogenes();
            else
                p.ClearEndogenes();
        }

        foreach (var geneDef in l)
            if (geneDef != null)
                p.genes.AddGene(geneDef, asXeno);
    }

    internal static void OverrideAllConflictingGenes(this Pawn_GeneTracker genes, Gene gene)
    {
        if (!ModLister.BiotechInstalled)
            return;
        gene.OverrideBy(null);
        foreach (var gene1 in genes.GenesListForReading)
            if (gene1 != gene && gene1.def.ConflictsWith(gene.def))
                gene1.OverrideBy(gene);
    }

    internal static void DoGeneActionForAllPawns(this GeneDef geneDef, Action<Pawn, GeneDef> action)
    {
        var allPawns = Find.CurrentMap?.mapPawns?.AllPawns;
        if (allPawns.NullOrEmpty())
            return;
        foreach (var pawn in allPawns)
            if (pawn.HasGeneTracker() && pawn.genes.GenesListForReading.FirstOrFallback(g => g.def == geneDef) != null)
                action(pawn, geneDef);
    }

    internal static Gene RemoveGeneKeepFirst(this Pawn p, Gene gene)
    {
        var l = gene.IsHairGene() ? p.GetHairGenes() : gene.IsSkinGene() ? p.GetSkinGenes() : p.genes.GenesListForReading.Where(x => x.def.displayCategory == gene.def.displayCategory).ToList();
        if (l.NullOrEmpty())
            return null;
        var gene1 = l.First();
        var g = (gene1.def == gene.def ? l.At(l.NextOrPrevIndex(0, true, false)) : null) ?? gene1;
        p.genes.RemoveGene(gene);
        p.genes.CallMethod("Notify_GenesChanged", new object[1]
        {
            gene.def
        });
        p.MakeGeneFirst(g);
        return g;
    }

    internal static void AddGeneAsFirst(this Pawn p, GeneDef geneDef, bool xeno)
    {
        foreach (var endogene in p.genes.Endogenes)
            if (endogene.def == geneDef)
            {
                MessageTool.Show("", MessageTypeDefOf.RejectInput);
                return;
            }

        var g = p.genes.AddGene(geneDef, xeno);
        p.MakeGeneFirst(g);
    }

    internal static void MakeGeneFirst(this Pawn p, Gene g)
    {
        p.genes.GenesListForReading.Remove(g);
        p.genes.GenesListForReading.Insert(0, g);
        p.genes.OverrideAllConflictingGenes(g);
        if (g.IsHairGene())
            p.SetHairColor(true, g.def.hairColorOverride ?? g.def.IconColor);
        else if (g.IsSkinGene())
            p.SetSkinColor(true, g.def.skinColorOverride ?? g.def.skinColorBase ?? g.def.IconColor);
        if (g.def.forcedHair == null)
            return;
        p.SetHair(g.def.forcedHair);
    }

    internal static GeneDef ClosestColorGene(Color color, bool hair)
    {
        var geneDefList = hair ? GetAllHairGenes() : GetAllSkinGenes();
        var dictionary = new Dictionary<GeneDef, Color>();
        foreach (var key in geneDefList)
            dictionary.Add(key, key.IconColor);
        GeneDef geneDef1 = null;
        GeneDef geneDef2 = null;
        GeneDef geneDef3 = null;
        GeneDef geneDef4 = null;
        GeneDef geneDef5 = null;
        GeneDef geneDef6 = null;
        GeneDef geneDef7 = null;
        var num1 = 0.100000001490116;
        var num2 = 0.200000002980232;
        var num3 = 0.300000011920929;
        var num4 = 0.400000005960464;
        var num5 = 0.5;
        var num6 = 0.600000023841858;
        var num7 = 0.699999988079071;
        foreach (var key in dictionary.Keys)
        {
            var color1 = dictionary[key];
            var num8 = Math.Pow(Convert.ToDouble(color1.r) - color.r, 2.0);
            var num9 = Math.Pow(Convert.ToDouble(color1.g) - color.g, 2.0);
            var num10 = Math.Sqrt(Math.Pow(Convert.ToDouble(color1.b) - color.b, 2.0) + num9 + num8);
            Color color2;
            if (num10 == 0.0)
            {
                color2 = color1;
                geneDef1 = key;
                break;
            }

            if (num10 < num1)
            {
                num1 = num10;
                color2 = color1;
                geneDef1 = key;
            }
            else if (num10 < num2)
            {
                num2 = num10;
                geneDef2 = key;
            }
            else if (num10 < num3)
            {
                num3 = num10;
                geneDef3 = key;
            }
            else if (num10 < num4)
            {
                num4 = num10;
                geneDef4 = key;
            }
            else if (num10 < num5)
            {
                num5 = num10;
                geneDef5 = key;
            }
            else if (num10 < num6)
            {
                num6 = num10;
                geneDef6 = key;
            }
            else if (num10 < num7)
            {
                num7 = num10;
                geneDef7 = key;
            }
        }

        return geneDef1 ?? geneDef2 ?? geneDef3 ?? geneDef4 ?? geneDef5 ?? geneDef6 ?? geneDef7 ?? geneDefList.First();
    }

    internal static void SetPawnXenotype(this Pawn p, XenotypeDef def, bool toXenogene)
    {
        bool flag = !p.HasGeneTracker() || def == null;
        if (!flag)
        {
            p.genes.SetXenotypeDirect(def);
            p.genes.iconDef = null;
            bool inStartingScreen = CEditor.InStartingScreen;
            if (inStartingScreen)
            {
                int index = StartingPawnUtility.PawnIndex(p);
                PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(index);
                generationRequest.ForcedXenotype = def;
                generationRequest.ForcedCustomXenotype = null;
                StartingPawnUtility.SetGenerationRequest(index, generationRequest);
            }
            p.SetGenesFromList(def.genes, toXenogene, Event.current.control);
            CEditor.API.UpdateGraphics();
        }
    }

    internal static void SetPawnXenotype(this Pawn p, CustomXenotype c, bool toXenogene)
    {
        bool flag = !p.HasGeneTracker() || c == null;
        if (!flag)
        {
            p.genes.SetXenotypeDirect(XenotypeDefOf.Baseliner);
            p.genes.xenotypeName = c.name;
            p.genes.iconDef = c.iconDef;
            bool flag2 = !Current.Game.customXenotypeDatabase.customXenotypes.Contains(c);
            if (flag2)
            {
                Current.Game.customXenotypeDatabase.customXenotypes.Add(c);
            }
            bool inStartingScreen = CEditor.InStartingScreen;
            if (inStartingScreen)
            {
                int index = StartingPawnUtility.PawnIndex(p);
                PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(index);
                generationRequest.ForcedXenotype = null;
                generationRequest.ForcedCustomXenotype = c;
                StartingPawnUtility.SetGenerationRequest(index, generationRequest);
            }
            p.SetGenesFromList(c.genes, toXenogene, Event.current.control);
            CEditor.API.UpdateGraphics();
        }
    }

    internal static string PrintIfXenotypeIsPrefered(Pawn p)
    {
        return ModsConfig.IdeologyActive && p.HasIdeoTracker() ? Label.XENOTYPEISPREFERED + " " + (p.Ideo.IsPreferredXenotype(p) ? "Yes".Translate() : "No".Translate()) : "";
    }

    private static void Remove(GeneDef def)
    {
        typeof(DefDatabase<GeneDef>).CallMethod(nameof(Remove), new object[1]
        {
            def
        });
    }

    internal static void UpdateGeneCache()
    {
        var atype = Reflect.GetAType("RimWorld", "GeneUtility");
        if (atype != null)
            atype.SetMemberValue("cachedGeneDefsInOrder", null);
        var genesInOrder = GeneUtility.GenesInOrder;
    }

    internal static void EnDisableBodySizeGenes()
    {
        var isBodysizeActive = CEditor.IsBodysizeActive;
        var listBodySizeGenes = ListBodySizeGenes;
        var bodySizeCategory = BodySizeCategory;
        if (cachedGenes.NullOrEmpty() && !listBodySizeGenes.NullOrEmpty())
            foreach (var geneDef in listBodySizeGenes)
                cachedGenes.Add(geneDef);
        if (isBodysizeActive)
        {
            if (listBodySizeGenes.NullOrEmpty())
            {
                foreach (var cachedGene in cachedGenes)
                {
                    DefDatabase<GeneDef>.Add(cachedGene);
                    cachedGene.displayCategory = bodySizeCategory;
                    cachedGene.ResolveReferences();
                    cachedGene.PostLoad();
                }

                bodySizeCategory?.ResolveReferences();
                UpdateGeneCache();
                Log.Message("restoring bodysize genes done");
            }
            else
            {
                Log.Message("bodysizes genes are active");
            }
        }
        else if (!listBodySizeGenes.NullOrEmpty())
        {
            foreach (var def in listBodySizeGenes)
                GeneTool.Remove(def);
            bodySizeCategory?.ResolveReferences();
            UpdateGeneCache();
            Log.Message("removing bodysize genes done");
        }
        else
        {
            Log.Message("bodysize genes are inactive");
        }
    }

    internal enum FreeList
    {
        CategoryFactors,
        CategoryOffsets,
        StatFactors,
        StatOffsets,
        Aptitudes,
        Capacities,
        Abilities,
        Immunities,
        Protections,
        DamageFactors,
        ForcedTraits,
        SuppressedTraits,
        DisabledNeeds,
        ForcedHeadTypes,
        DisabledWorkTags,
        ConditionalStats,
        All
    }
}
