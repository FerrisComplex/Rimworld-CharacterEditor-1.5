// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogGenery
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

internal class DialogGenery : DialogTemplate<GeneDef>
{
    private readonly bool bIsXeno;
    private readonly Func<GeneCategoryDef, string> FCatLabel = a => !a.IsNullOrEmpty() ? a.LabelCap.ToString() : Label.ALL;
    private int iTick120 = 120;
    private readonly HashSet<GeneticBodyType?> lAllBodyTypes;
    private readonly HashSet<ChemicalDef> lAllChems;
    private readonly HashSet<EndogeneCategory> lAllEndogeneCategories;
    private readonly HashSet<GeneCategoryDef> lAllGeneCategories;
    private readonly HashSet<GeneDef> lAllGenes;
    private readonly HashSet<HairDef> lAllHairs;
    private readonly HashSet<HistoryEventDef> lAllHistoryEvents;
    private readonly HashSet<NeedDef> lAllNeeds;
    private readonly HashSet<SkillDef> lAllSkills;
    private readonly HashSet<SoundDef> lAllSounds;
    private readonly HashSet<GeneCategoryDef> lCat;
    private int mHscroll;
    private PawnCapacityDef selected_CapacityDef;
    private DamageDef selected_DamageDef;
    private HeadTypeDef selected_ForcedHeadType;
    private GeneticTraitData selected_ForcedTrait;
    private HediffDef selected_HediffDef;
    private NeedDef selected_NeedDef;
    private SkillDef selected_PassionSkillDef;
    private SkillDef selected_SkillDef;
    private GeneticTraitData selected_SuppressedTrait;
    private GeneCategoryDef selGeneCategoryDef;

    internal DialogGenery(bool xeno) : base(SearchTool.SIndex.GeneDef, Label.ADD_GENE, 105)
    {
        customAcceptLabel = new TaggedString("OK".Translate());
        bIsXeno = xeno;
        lCat = new HashSet<GeneCategoryDef>();
        lCat.Add(null);
        this.lCat.AddRange(DefTool.ListBy((GeneCategoryDef x) => !x.defName.NullOrEmpty()));
        mHscroll = 0;
        view = new Listing_X();
        lAllSounds = DefTool.AllDefsWithNameWithNull((Func<SoundDef, bool>)(s => s.defName.StartsWith("Pawn_")));
        lAllGenes = DefTool.AllDefsWithLabelWithNull<GeneDef>();
        lAllHistoryEvents = DefTool.AllDefsWithLabelWithNull<HistoryEventDef>();
        lAllNeeds = DefTool.AllDefsWithLabelWithNull<NeedDef>();
        lAllChems = DefTool.AllDefsWithLabelWithNull<ChemicalDef>();
        lAllHairs = DefTool.AllDefsWithLabelWithNull<HairDef>();
        lAllGeneCategories = DefTool.AllDefsWithLabelWithNull<GeneCategoryDef>();
        lAllEndogeneCategories = EnumTool.GetAllEnumsOfType<EndogeneCategory>().ToHashSet();
        lAllSkills = DefTool.AllDefsWithLabelWithNull<SkillDef>();
        lAllBodyTypes = GeneTool.ListOfGeneticBodyType();
    }

    internal override HashSet<GeneDef> TList()
    {
        return (from x in DefTool.ListByMod<GeneDef>(this.search.modName)
            orderby x.index
            select x).ToHashSet<GeneDef>();
    }

    internal override void AReset()
    {
        PresetGene.ResetToDefault(selectedDef?.defName);
        if (!GeneTool.SelectedGene.IsBodySizeGene())
            return;
        PresetLifeStage.ResetToDefault(GeneTool.SelectedGene.GetLifeStageDef().defName);
    }

    internal override void AResetAll()
    {
        PresetGene.ResetAllToDefaults();
        PresetLifeStage.ResetAllToDefaults();
    }

    internal override void ASave()
    {
        PresetGene.SaveModification(selectedDef);
        if (!GeneTool.SelectedGene.IsBodySizeGene())
            return;
        PresetLifeStage.SaveModification(GeneTool.SelectedGene.GetLifeStageDef());
    }

    internal override void CalcHSCROLL()
    {
        hScrollParam = 4000;
        if (mHscroll <= 800)
            return;
        hScrollParam = mHscroll;
    }

    internal override void Preselection()
    {
        base.ASelectedModName(search.modName);
        selectedDef = GeneTool.SelectedGene ?? lDefs.First();
        oldSelectedDef = null;
        GeneTool.UseAllCategories = false;
        selected_StatFactor = null;
        selected_StatOffset = null;
        selected_CapacityDef = null;
        selected_SkillDef = null;
        selected_HediffDef = null;
        selected_NeedDef = null;
        selected_PassionSkillDef = null;
        selected_ForcedHeadType = null;
        selected_DamageDef = null;
        selected_ForcedTrait = null;
        selected_SuppressedTrait = null;
        if (search.ofilter1 == null)
            return;
        ASelectedCategoryDef(search.ofilter1 as GeneCategoryDef);
    }

    internal override int DrawCustomFilter(int x, int y, int w)
    {
        Text.Font = GameFont.Small;
        Rect rect = new Rect((float)x, (float)y, (float)w, 30f);
        SZWidgets.FloatMenuOnButtonText<GeneCategoryDef>(rect, this.FCatLabel(this.selGeneCategoryDef), this.lCat, this.FCatLabel, new Action<GeneCategoryDef>(this.ASelectedCategoryDef), "");
        return 30;
    }

    private void ASelectedCategoryDef(GeneCategoryDef def)
    {
        search.ofilter1 = def;
        selGeneCategoryDef = def;
        lDefs = DefTool.ListByMod<GeneDef>(search.modName).ToHashSet().Where(td => def == null || td.displayCategory == def).OrderBy(td => td.label).ToHashSet();
    }

    internal override void ASelectedModName(string val)
    {
        base.ASelectedModName(val);
        selGeneCategoryDef = null;
    }

    internal override void OnAccept()
    {
        CEditor.API.Pawn.RememberBackstory();
        CEditor.API.Pawn.AddGeneAsFirst(selectedDef, bIsXeno);
        GeneTool.PrintIfXenotypeIsPrefered(CEditor.API.Pawn);
        CEditor.API.UpdateGraphics();
    }

    internal override void OnSelectionChanged()
    {
        mHscroll = 0;
        GeneTool.SelectedGene = selectedDef;
        GeneTool.SelectedGene.UpdateFreeLists(GeneTool.FreeList.All);
    }

    internal override void DrawParameter()
    {
        DrawLabel();
        DrawBioStats(400);
        var w = WPARAM - 12;
        DrawLifeStages(400);
        DrawStatFactors(w);
        DrawStatOffsets(w);
        DrawAptitudes(w);
        DrawCapacities(w);
        DrawAbilities(w);
        DrawForcedTraits(w);
        DrawSuppressedTraits(w);
        DrawImmunities(w);
        DrawProtections(w);
        DrawDamageFactors(w);
        DrawDisabledNeeds(w);
        DrawDisabledWorkTags(w);
        DrawForcedHeadTypes(w);
        DrawStringLists(w);
        DrawSounds(500);
        DrawFloats(400);
        DrawBools(400);
        DrawTypes(400);
        mHscroll = (int)view.CurY + 50;
        UpdateCachedDescription();
    }

    private void UpdateCachedDescription()
    {
        if (iTick120 <= 0)
        {
            GeneTool.SelectedGene.SetMemberValue("cachedDescription", null);
            GeneTool.SelectedGene.SetMemberValue("cachedLabelCap", null);
            if (GeneTool.SelectedGene.IsBodySizeGene())
                CEditor.API.UpdateGraphics();
            iTick120 = 120;
        }
        else
        {
            --iTick120;
        }
    }

    private void DrawLabel()
    {
        GUI.color = ColorTool.colBeige;
        SZWidgets.LabelEdit(this.view.GetRect(30f, 1f), 99, "", ref GeneTool.SelectedGene.label, GameFont.Medium, true);
        GUI.color = Color.white;
        this.view.Gap(8f);
    }

    private void DrawTypes(int w)
    {
        view.Gap(16f);
        GUI.color = Color.gray;
        SZWidgets.Label(view.GetRect(22f), GeneTool.SelectedGene.resourceGizmoType?.ToString());
        SZWidgets.Label(view.GetRect(22f), GeneTool.SelectedGene.geneClass?.ToString());
        SZWidgets.Label(view.GetRect(22f), GeneTool.SelectedGene.SDefname());
        GUI.color = Color.white;
        view.Gap(16f);
    }

    private void DrawCategories(int w)
    {
        SZWidgets.NonDefSelectorSimple<GeneticBodyType?>(this.view.GetRect(22f, 1f), w, this.lAllBodyTypes, ref GeneTool.SelectedGene.bodyType, Label.GENETICBODYTYPE + " ", FLabel.GeneticBodytype, delegate(GeneticBodyType? b) { GeneTool.SelectedGene.bodyType = b; }, false, null, null);
        this.view.Gap(4f);
        SZWidgets.NonDefSelectorSimple<EndogeneCategory>(this.view.GetRect(22f, 1f), w, this.lAllEndogeneCategories, ref GeneTool.SelectedGene.endogeneCategory, Label.ENDOGENECATEGORY + " ", FLabel.EndogeneCat, delegate(EndogeneCategory s) { GeneTool.SelectedGene.endogeneCategory = s; }, false, null, null);
        this.view.Gap(4f);
        SZWidgets.DefSelectorSimple<GeneCategoryDef>(this.view.GetRect(22f, 1f), w, this.lAllGeneCategories, ref GeneTool.SelectedGene.displayCategory, Label.GENECATEGORY + " ", new Func<GeneCategoryDef, string>(FLabel.DefLabel<GeneCategoryDef>), delegate(GeneCategoryDef c) { GeneTool.SelectedGene.displayCategory = c; }, false, null, null, true, "");
        this.view.Gap(4f);
    }

    private void DrawSounds(int w)
    {
        SZWidgets.DefSelectorSimple<SoundDef>(this.view.GetRect(22f, 1f), w, this.lAllSounds, ref GeneTool.SelectedGene.soundCall, Label.SOUNDCALL + " ", FLabel.Sound, delegate(SoundDef s) { GeneTool.SelectedGene.soundCall = SoundTool.GetAndPlayPawnSoundCur(s); }, true, "bsound", new Action<SoundDef>(SoundTool.PlayPawnSoundCur), true, "");
        this.view.Gap(4f);
        SZWidgets.DefSelectorSimple<SoundDef>(this.view.GetRect(22f, 1f), w, this.lAllSounds, ref GeneTool.SelectedGene.soundDeath, Label.SOUNDDEATH + " ", FLabel.Sound, delegate(SoundDef s) { GeneTool.SelectedGene.soundDeath = SoundTool.GetAndPlayPawnSoundCur(s); }, true, "bsound", new Action<SoundDef>(SoundTool.PlayPawnSoundCur), true, "");
        this.view.Gap(4f);
        SZWidgets.DefSelectorSimple<SoundDef>(this.view.GetRect(22f, 1f), w, this.lAllSounds, ref GeneTool.SelectedGene.soundWounded, Label.SOUNDWOUNDED + " ", FLabel.Sound, delegate(SoundDef s) { GeneTool.SelectedGene.soundWounded = SoundTool.GetAndPlayPawnSoundCur(s); }, true, "bsound", new Action<SoundDef>(SoundTool.PlayPawnSoundCur), true, "");
        this.view.Gap(4f);
    }

    private void DrawBioStats(int w)
    {
        SZWidgets.LabelIntFieldSlider(this.view, w, 30, FLabel.Complexity, ref GeneTool.SelectedGene.biostatCpx, 0, 9);
        SZWidgets.LabelIntFieldSlider(this.view, w, 40, FLabel.Metabolism, ref GeneTool.SelectedGene.biostatMet, -9, 9);
        SZWidgets.LabelIntFieldSlider(this.view, w, 50, FLabel.ArchitesRequired, ref GeneTool.SelectedGene.biostatArc, 0, 5);
    }

    private void DrawFloats(int w)
    {
        this.view.Gap(8f);
        this.DrawColors(420);
        this.DrawForcedHair(w);
        Listing_X view = this.view;
        int id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view, w, id, FLabel.RandomBrightnessFactor, ref GeneTool.SelectedGene.randomBrightnessFactor, 0f, 2f, 2);
        this.view.Gap(8f);
        this.DrawPassion(w);
        this.DrawCausedNeed(w);
        this.DrawChemicalDef(w);
        Listing_X view2 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view2, w, id, FLabel.AddictionChance, ref GeneTool.SelectedGene.addictionChanceFactor, 0f, 50f, 2);
        Listing_X view3 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view3, w, id, FLabel.OverdoseChance, ref GeneTool.SelectedGene.overdoseChanceFactor, 0f, 50f, 2);
        Listing_X view4 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view4, w, id, FLabel.ToleranceFactor, ref GeneTool.SelectedGene.toleranceBuildupFactor, 0f, 50f, 2);
        this.view.Gap(8f);
        Listing_X view5 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view5, w, id, FLabel.FoodPoisonChance, ref GeneTool.SelectedGene.foodPoisoningChanceFactor, -1f, 10f, 2);
        Listing_X view6 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view6, w, id, FLabel.PainFactor, ref GeneTool.SelectedGene.painFactor, 0f, 100f, 2);
        Listing_X view7 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view7, w, id, FLabel.PainOffset, ref GeneTool.SelectedGene.painOffset, 0f, 100f, 2);
        this.view.Gap(8f);
        Listing_X view8 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view8, w, id, FLabel.MentalBreakMTB, ref GeneTool.SelectedGene.mentalBreakMtbDays, 0f, 100f, 2);
        Listing_X view9 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view9, w, id, FLabel.MentalBreakChance, ref GeneTool.SelectedGene.aggroMentalBreakSelectionChanceFactor, 0f, 100f, 2);
        Listing_X view10 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view10, w, id, FLabel.SocialFightChance, ref GeneTool.SelectedGene.socialFightChanceFactor, 0f, 100f, 2);
        Listing_X view11 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view11, w, id, FLabel.PrisonBreak, ref GeneTool.SelectedGene.prisonBreakMTBFactor, 0f, 100f, 2);
        this.view.Gap(8f);
        Listing_X view12 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view12, w, id, FLabel.MarketValue, ref GeneTool.SelectedGene.marketValueFactor, 0f, 100f, 2);
        Listing_X view13 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view13, w, id, FLabel.MinAgeActive, ref GeneTool.SelectedGene.minAgeActive, 0f, 500f, 0);
        this.DrawHistoryEvent(w);
        this.view.Gap(8f);
        Listing_X view14 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view14, w, id, FLabel.SelectionWeight, ref GeneTool.SelectedGene.selectionWeight, 0f, 2f, 2);
        Listing_X view15 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view15, w, id, FLabel.SelectionWeightDark, ref GeneTool.SelectedGene.selectionWeightFactorDarkSkin, 0f, 2f, 2);
        Listing_X view16 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view16, w, id, FLabel.DisplayOrderInCat, ref GeneTool.SelectedGene.displayOrderInCategory, -100f, 10000f, 0);
        this.view.Gap(8f);
        Listing_X view17 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view17, w, id, FLabel.LovinMTBFactor, ref GeneTool.SelectedGene.lovinMTBFactor, 0f, 10f, 2);
        Listing_X view18 = this.view;
        int w2 = base.WPARAM - 40;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view18, w2, id, FLabel.MissingRomanceChance, ref GeneTool.SelectedGene.missingGeneRomanceChanceFactor, 0f, 100f, 2);
        this.view.Gap(8f);
        this.DrawCategories(w);
        this.DrawPrerequisite(w);
        Listing_X view19 = this.view;
        id = this.id;
        this.id = id + 1;
        SZWidgets.LabelFloatFieldSlider(view19, w, id, FLabel.ResourceLoss, ref GeneTool.SelectedGene.resourceLossPerDay, 0f, 100f, 2);
        Listing_X view20 = this.view;
        id = this.id;
        this.id = id + 1;
        view20.LabelEdit(id, Label.RESOURCELABEL, ref GeneTool.SelectedGene.resourceLabel, GameFont.Small);
        Listing_X view21 = this.view;
        id = this.id;
        this.id = id + 1;
        view21.LabelEdit(id, Label.RESOURCEDESC, ref GeneTool.SelectedGene.resourceDescription, GameFont.Small);
        this.view.Gap(8f);
        Listing_X view22 = this.view;
        id = this.id;
        this.id = id + 1;
        view22.LabelEdit(id, Label.LABELADJ, ref GeneTool.SelectedGene.labelShortAdj, GameFont.Small);
        Listing_X view23 = this.view;
        id = this.id;
        this.id = id + 1;
        view23.LabelEdit(id, Label.ICONPATH, ref GeneTool.SelectedGene.iconPath, GameFont.Small);
        this.view.Gap(16f);
    }

    private void DrawBools(int w)
    {
        this.view.CheckboxLabeled(Label.UNAFFECTEDBYDARK, 0f, (float)w, ref GeneTool.SelectedGene.ignoreDarkness, null, 2);
        this.view.CheckboxLabeled(Label.CANGENERATEINGENESET, 0f, (float)w, ref GeneTool.SelectedGene.canGenerateInGeneSet, null, 2);
        this.view.CheckboxLabeled(Label.REMOVEONREDRESS, 0f, (float)w, ref GeneTool.SelectedGene.removeOnRedress, null, 2);
        this.view.CheckboxLabeled(Label.PASSONDIRECTLY, 0f, (float)w, ref GeneTool.SelectedGene.passOnDirectly, null, 2);
        this.view.CheckboxLabeled(Label.RANDOMCHOSEN, 0f, (float)w, ref GeneTool.SelectedGene.randomChosen, null, 2);
        this.view.CheckboxLabeled(Label.STERILIZE, 0f, (float)w, ref GeneTool.SelectedGene.sterilize, null, 2);
        this.view.CheckboxLabeled(Label.DISLIKESSUNLIGHT, 0f, (float)w, ref GeneTool.SelectedGene.dislikesSunlight, null, 2);
        this.view.CheckboxLabeled(Label.DONTMINDRAWFOOD, 0f, (float)w, ref GeneTool.SelectedGene.dontMindRawFood, null, 2);
        this.view.CheckboxLabeled(Label.IMMUNETOTOXGASEXPOSURE, 0f, (float)w, ref GeneTool.SelectedGene.immuneToToxGasExposure, null, 2);
        this.view.CheckboxLabeled(Label.NEVERGRAYHAIR, 0f, (float)w, ref GeneTool.SelectedGene.neverGrayHair, null, 2);
        this.view.CheckboxLabeled(Label.WOMENCANHAVEBEARDS, 0f, (float)w, ref GeneTool.SelectedGene.womenCanHaveBeards, null, 2);
        this.view.CheckboxLabeled(Label.PREVENTPERMANENTWOUNDS, 0f, (float)w, ref GeneTool.SelectedGene.preventPermanentWounds, null, 2);
        this.view.CheckboxLabeled("showGizmoOnWorldView", 0f, (float)w, ref GeneTool.SelectedGene.showGizmoOnWorldView, null, 2);
        this.view.CheckboxLabeled("showGizmoWhenDrafted", 0f, (float)w, ref GeneTool.SelectedGene.showGizmoWhenDrafted, null, 2);
        this.view.CheckboxLabeled("showGizmoOnMultiSelect", 0f, (float)w, ref GeneTool.SelectedGene.showGizmoOnMultiSelect, null, 2);
    }


    private void DrawColors(int w)
    {
        this.view.NavSelectorColor(w, Label.HAIRCOLOROVERRIDE, "", GeneTool.SelectedGene.hairColorOverride, delegate { WindowTool.Open(new DialogColorPicker(ColorType.GeneColorHair, true, null, null, GeneTool.SelectedGene)); });
        this.view.Gap(4f);
        this.view.NavSelectorColor(w, Label.SKINCOLORBASE, "", GeneTool.SelectedGene.skinColorBase, delegate { WindowTool.Open(new DialogColorPicker(ColorType.GeneColorSkinBase, true, null, null, GeneTool.SelectedGene)); });
        this.view.Gap(4f);
        this.view.NavSelectorColor(w, Label.SKINCOLOROVERRIDE, "", GeneTool.SelectedGene.skinColorOverride, delegate { WindowTool.Open(new DialogColorPicker(ColorType.GeneColorSkinOverride, true, null, null, GeneTool.SelectedGene)); });
        this.view.Gap(4f);
    }

    private void DrawPrerequisite(int w)
    {
        SZWidgets.DefSelectorSimple<GeneDef>(this.view.GetRect(22f, 1f), w, this.lAllGenes, ref GeneTool.SelectedGene.prerequisite, Label.PREREQUISITE + " ", new Func<GeneDef, string>(FLabel.DefLabel<GeneDef>), delegate(GeneDef g) { GeneTool.SelectedGene.prerequisite = g; }, false, null, null, true, "");
        this.view.Gap(4f);
    }


    private void DrawHistoryEvent(int w)
    {
        SZWidgets.DefSelectorSimple<HistoryEventDef>(this.view.GetRect(22f, 1f), w, this.lAllHistoryEvents, ref GeneTool.SelectedGene.deathHistoryEvent, Label.HISTORYEVENTONDEATH + " ", new Func<HistoryEventDef, string>(FLabel.DefLabel<HistoryEventDef>), delegate(HistoryEventDef e) { GeneTool.SelectedGene.deathHistoryEvent = e; }, false, null, null, true, "");
        this.view.Gap(4f);
    }


    private void DrawPassion(int w)
    {
        bool flag = GeneTool.SelectedGene.passionMod == null || (GeneTool.SelectedGene.passionMod != null && GeneTool.SelectedGene.passionMod.modType != PassionMod.PassionModType.AddOneLevel);
        if (flag)
        {
            SZWidgets.DefSelectorSimple<SkillDef>(this.view.GetRect(22f, 1f), w, this.lAllSkills, ref this.selected_PassionSkillDef, Label.PASSIONMODADD + " ", FLabel.PassionModAdd, delegate(SkillDef s) { GeneTool.SelectedGene.SetPassionMod(s, PassionMod.PassionModType.AddOneLevel); }, false, null, null, true, "");
        }
        else
        {
            bool flag2 = GeneTool.SelectedGene.passionMod != null;
            if (flag2)
            {
                SZWidgets.DefSelectorSimple<SkillDef>(this.view.GetRect(22f, 1f), w, this.lAllSkills, ref GeneTool.SelectedGene.passionMod.skill, Label.PASSIONMODADD + " ", FLabel.PassionModAdd, delegate(SkillDef s) { GeneTool.SelectedGene.SetPassionMod(s, PassionMod.PassionModType.AddOneLevel); }, false, null, null, true, "");
            }
        }

        this.view.Gap(4f);
        bool flag3 = GeneTool.SelectedGene.passionMod == null || (GeneTool.SelectedGene.passionMod != null && GeneTool.SelectedGene.passionMod.modType != PassionMod.PassionModType.DropAll);
        if (flag3)
        {
            SZWidgets.DefSelectorSimple<SkillDef>(this.view.GetRect(22f, 1f), w, this.lAllSkills, ref this.selected_PassionSkillDef, Label.PASSIONMODSUB + " ", FLabel.PassionModDrop, delegate(SkillDef s) { GeneTool.SelectedGene.SetPassionMod(s, PassionMod.PassionModType.DropAll); }, false, null, null, true, "");
        }
        else
        {
            bool flag4 = GeneTool.SelectedGene.passionMod != null;
            if (flag4)
            {
                SZWidgets.DefSelectorSimple<SkillDef>(this.view.GetRect(22f, 1f), w, this.lAllSkills, ref GeneTool.SelectedGene.passionMod.skill, Label.PASSIONMODSUB + " ", FLabel.PassionModDrop, delegate(SkillDef s) { GeneTool.SelectedGene.SetPassionMod(s, PassionMod.PassionModType.DropAll); }, false, null, null, true, "");
            }
        }

        this.view.Gap(4f);
    }

    private void DrawCausedNeed(int w)
    {
        SZWidgets.DefSelectorSimple<NeedDef>(this.view.GetRect(22f, 1f), w, this.lAllNeeds, ref GeneTool.SelectedGene.causesNeed, Label.CAUSESNEED + " ", new Func<NeedDef, string>(FLabel.DefLabel<NeedDef>), delegate(NeedDef n) { GeneTool.SelectedGene.SetCausesNeed(n); }, false, null, null, true, "");
        this.view.Gap(4f);
    }

    private void DrawChemicalDef(int w)
    {
        SZWidgets.DefSelectorSimple<ChemicalDef>(this.view.GetRect(22f, 1f), w, this.lAllChems, ref GeneTool.SelectedGene.chemical, "Chemical".Translate() + " ", new Func<ChemicalDef, string>(FLabel.DefLabel<ChemicalDef>), delegate(ChemicalDef c) { GeneTool.SelectedGene.SetChemicalDef(c); }, false, null, null, true, "");
        this.view.Gap(4f);
    }

    private void DrawForcedHair(int w)
    {
        SZWidgets.DefSelectorSimple<HairDef>(this.view.GetRect(22f, 1f), w, this.lAllHairs, ref GeneTool.SelectedGene.forcedHair, "Hair".Translate().CapitalizeFirst() + " ", new Func<HairDef, string>(FLabel.DefLabel<HairDef>), delegate(HairDef h) { GeneTool.SelectedGene.SetForcedHairDef(h); }, false, null, null, true, "");
        this.view.Gap(4f);
    }

    private void DrawStatFactors(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.STAT_FACTORS, (GameFont)2);
        view.ButtonImage(w - 380, 5f, 24f, 24f, "bnone", GeneTool.UseAllCategories ? Color.white : Color.gray, b => GeneTool.UseAllCategories = !GeneTool.UseAllCategories, GeneTool.UseAllCategories, Label.TIP_TOGGLECATEGORIES);
        Text.Font = 0;
        view.FloatMenuButtonWithLabelDef("", w - 350, 200f, DefTool.CategoryLabel(GeneTool.StatFactorCategory), GeneTool.lCategoryDef_Factors, DefTool.CategoryLabel, cat => GeneTool.StatFactorCategory = cat, 0.0f);
        Text.Font = (GameFont)1;
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeStatDefFactors, s => s.SLabel(), stat => GeneTool.SelectedGene.SetStatFactor(stat, 0.0f)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyStatFactors());
        if (!GeneTool.lCopyStatFactors.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteStatFactors());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam(GeneTool.SelectedGene.statFactors, ref selected_StatFactor, s => s.stat, s => s.value, null, s => s.stat.minValue, s => s.stat.maxValue, false, bRemoveOnClick, (s, val) => s.value = val, null, stat => GeneTool.SelectedGene.RemoveStatFactor(stat));
        view.GapLine(25f);
    }

    private void DrawStatOffsets(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.STAT_OFFSETS, (GameFont)2);
        Text.Font = 0;
        view.FloatMenuButtonWithLabelDef("", w - 350, 200f, DefTool.CategoryLabel(GeneTool.StatOffsetCategory), GeneTool.lCategoryDef_Offsets, DefTool.CategoryLabel, cat => GeneTool.StatOffsetCategory = cat, 0.0f);
        Text.Font = (GameFont)1;
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeStatDefOffsets, s => s.SLabel(), stat => GeneTool.SelectedGene.SetStatOffset(stat, 0.0f)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyStatOffsets());
        if (!GeneTool.lCopyStatOffsets.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteStatOffsets());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam(GeneTool.SelectedGene.statOffsets, ref selected_StatOffset, s => s.stat, s => s.value, null, s => s.stat.minValue, s => s.stat.maxValue, false, bRemoveOnClick, (s, val) => s.value = val, null, stat => GeneTool.SelectedGene.RemoveStatOffset(stat));
        view.GapLine(25f);
    }

    private void DrawAptitudes(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.APTITUDE, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeAptitudes, skill => skill.SLabel(), skill => GeneTool.SelectedGene.SetAptitude(skill, 0)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyAptitudes());
        if (!GeneTool.lCopyAptitude.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteAptitude());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam(GeneTool.SelectedGene.aptitudes, ref selected_SkillDef, a => a.skill, a => a.level, null, a => -999f, a => 999f, true, bRemoveOnClick, (a, val) => a.level = (int)val, null, skill => GeneTool.SelectedGene.RemoveAptitude(skill));
        view.GapLine(25f);
    }

    private void DrawCapacities(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.CAPACITIES, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeCapacities, cap => cap.SLabel(), cap => GeneTool.SelectedGene.SetCapacity(cap, 0.0f, 0.0f)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyCapacities());
        if (!GeneTool.lCopyCapacities.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteCapacities());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam(GeneTool.SelectedGene.capMods, ref selected_CapacityDef, c => c.capacity, c => c.offset, c => c.postFactor, c => 0.0f, c => c.setMax, false, bRemoveOnClick, (c, val) => c.offset = val, (c, val) => c.postFactor = val, capacity => GeneTool.SelectedGene.RemoveCapacity(capacity));
        view.GapLine(25f);
    }

    private void DrawAbilities(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.ABILITIES, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeAbilities, ability => ability.SLabel(), ability => GeneTool.SelectedGene.SetAbility(ability)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyAbilities());
        if (!GeneTool.lCopyAbilities.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteAbilities());
        view.Gap(30f);
        if (!IsGeneDef)
            return;
        if (SZContainers.DrawElementStack(new Rect(view.CurX, view.CurY, WPARAM - 32, 50f), GeneTool.SelectedGene.abilities, bRemoveOnClick, a => GeneTool.SelectedGene.RemoveAbility(a)))
            view.GapLineCustom(5f + 70f * (int)Math.Ceiling(GeneTool.SelectedGene.abilities.Count / (float)(int)Math.Floor((WPARAM - 32.0) / 38.0)), 10f);
        else
            view.GapLine(25f);
    }

    private void DrawForcedTraits(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.FORCEDTRAITS, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeForcedTraits, TraitTool.FGeneticTraitLabel, gtd => GeneTool.SelectedGene.SetForcedTrait(gtd, gtd.def, gtd.degree)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyForcedTraits());
        if (!GeneTool.lCopyForcedTraits.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteForcedTraits());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam2(GeneTool.SelectedGene.forcedTraits, ref selected_ForcedTrait, DefTool.DefGetterGeneticTraitData, TraitTool.FGeneticTraitLabel, bRemoveOnClick, gtd => GeneTool.SelectedGene.RemoveForcedTrait(gtd));
        view.GapLine(25f);
    }

    private void DrawSuppressedTraits(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.SUPPRESSEDTRAITS, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeSuppressedTraits, TraitTool.FGeneticTraitLabel, gtd => GeneTool.SelectedGene.SetSuppressedTrait(gtd, gtd.def, gtd.degree)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopySuppressedTraits());
        if (!GeneTool.lCopySuppressedTraits.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteSuppressedTraits());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam2(GeneTool.SelectedGene.suppressedTraits, ref selected_SuppressedTrait, DefTool.DefGetterGeneticTraitData, TraitTool.FGeneticTraitLabel, bRemoveOnClick, gtd => GeneTool.SelectedGene.RemoveSuppressedTrait(gtd));
        view.GapLine(25f);
    }

    private void DrawImmunities(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.IMMUNETO, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeImmunities, h => h.SLabel(), h => GeneTool.SelectedGene.SetImmunity(h)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyImmunities());
        if (!GeneTool.lCopyImmunities.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteImmunities());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam1(GeneTool.SelectedGene.makeImmuneTo, ref selected_HediffDef, bRemoveOnClick, h => GeneTool.SelectedGene.RemoveImmunity(h));
        view.GapLine(25f);
    }

    private void DrawProtections(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.FULLYPROTECTEDFROM, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeProtections, h => h.SLabel(), h => GeneTool.SelectedGene.SetProtection(h)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyProtections());
        if (!GeneTool.lCopyProtections.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteProtections());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam1(GeneTool.SelectedGene.hediffGiversCannotGive, ref selected_HediffDef, bRemoveOnClick, h => GeneTool.SelectedGene.RemoveProtection(h));
        view.GapLine(25f);
    }

    private void DrawDamageFactors(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.DAMAGEFACTOR, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeDamageFactors, GeneTool.DamageLabel, d => GeneTool.SelectedGene.SetDamageFactor(d, 0.0f)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyDamageFactors());
        if (!GeneTool.lCopyDamageFactors.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteDamageFactors());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam(GeneTool.SelectedGene.damageFactors, ref selected_DamageDef, d => d.damageDef, d => d.factor, null, d => -999f, d => 999f, false, bRemoveOnClick, (d, val) => d.factor = val, null, d => GeneTool.SelectedGene.RemoveDamageFactor(d));
        view.GapLine(25f);
    }

    private void DrawDisabledNeeds(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.DISABLEDNEEDS, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeNeeds, need => need.SLabel(), need => GeneTool.SelectedGene.SetDisabledNeed(need)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyDisabledNeeds());
        if (!GeneTool.lCopyDisabledNeeds.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteDisabledNeeds());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam1(GeneTool.SelectedGene.disablesNeeds, ref selected_NeedDef, bRemoveOnClick, need => GeneTool.SelectedGene.RemoveDisabledNeed(need));
        view.GapLine(25f);
    }

    private void DrawForcedHeadTypes(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.FORCEDHEADTYPES, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeForcedHeadTypes, h => h.SDefname(), h => GeneTool.SelectedGene.SetForcedHeadType(h)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyForcedHeadTypes());
        if (!GeneTool.lCopyForcedHeadTypes.NullOrEmpty())
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteForcedHeadTypes());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewParam1(GeneTool.SelectedGene.forcedHeadTypes, ref selected_ForcedHeadType, bRemoveOnClick, h => GeneTool.SelectedGene.RemoveForcedHeadType(h));
        view.GapLine(25f);
    }

    private void DrawDisabledWorkTags(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        view.Label(0.0f, 0.0f, 500f, 30f, Label.DISABLEDWORKTAGS, (GameFont)2);
        view.ButtonImage(w - 60, 5f, 24f, 24f, "UI/Buttons/Dev/Add", () => SZWidgets.FloatMenuOnRect(GeneTool.lFreeWorkTags, work => work.LabelTranslated(), work => GeneTool.SelectedGene.SetDisabledWorkTags(work)));
        view.ButtonImage(w - 85, 5f, 24f, 24f, "bminus", ToggleRemove, RemoveColor);
        view.ButtonImage(w - 110, 5f, 18f, 24f, "UI/Buttons/Copy", () => GeneTool.SelectedGene.CopyDisabledWorkTags());
        if (GeneTool.lCopyDisabledWorkTags > 0)
            view.ButtonImage(w - 130, 5f, 18f, 24f, "UI/Buttons/Paste", () => GeneTool.SelectedGene.PasteDisabledWorkTags());
        view.Gap(30f);
        if (IsGeneDef)
            view.FullListViewWorkTags(GeneTool.SelectedGene.disabledWorkTags, bRemoveOnClick, work => GeneTool.SelectedGene.RemoveDisabledWorkTags(work));
        view.GapLine(25f);
    }

    private void DrawStringLists(int w)
    {
        if (GeneTool.SelectedGene == null)
            return;
        DrawCustomEffectDescriptors(w);
        DrawExclusionTags(w);
        DrawHairTags(w);
        DrawBeardTags(w);
        DrawGizmoThres(w);
    }

    private void DrawCustomEffectDescriptors(int w)
    {
        SZWidgets.DrawStringListCustom(ref GeneTool.SelectedGene.customEffectDescriptions, 1, w, this.view, Label.CUSTOMEFFECTDESCRIPTIONS, ref GeneTool.lCopyCustomEffectDescriptions, delegate(string s) { GeneTool.SelectedGene.customEffectDescriptions.Remove(s); }, null);
    }

    private void DrawExclusionTags(int w)
    {
        SZWidgets.DrawStringList(ref GeneTool.SelectedGene.exclusionTags, CEditor.API.ListOf<string>(EType.ExclusionTags), w, this.view, Label.EXCLUSIONTAGS, ref GeneTool.lCopyExclusionTags, delegate(string s)
        {
            List<string> exclusionTags = GeneTool.SelectedGene.exclusionTags;
            if (exclusionTags != null)
            {
                exclusionTags.Remove(s);
            }
        }, delegate(string s) { Extension.AddElem<string>(ref GeneTool.SelectedGene.exclusionTags, s); });
    }

    private void DrawHairTags(int w)
    {
        SZWidgets.DrawTagFilter(ref GeneTool.SelectedGene.hairTagFilter, CEditor.API.ListOf<string>(EType.HairTags), w, this.view, Label.HAIRTAGS, ref GeneTool.lCopyHairTags, delegate(string s)
        {
            TagFilter hairTagFilter = GeneTool.SelectedGene.hairTagFilter;
            if (hairTagFilter != null)
            {
                hairTagFilter.tags.Remove(s);
            }
        }, delegate(string s) { Extension.AddTag(ref GeneTool.SelectedGene.hairTagFilter, s); });
    }

    private void DrawBeardTags(int w)
    {
        SZWidgets.DrawTagFilter(ref GeneTool.SelectedGene.beardTagFilter, CEditor.API.ListOf<string>(EType.BeardTags), w, this.view, Label.BEARDTAGS, ref GeneTool.lCopyBeardTags, delegate(string s)
        {
            TagFilter beardTagFilter = GeneTool.SelectedGene.beardTagFilter;
            if (beardTagFilter != null)
            {
                beardTagFilter.tags.Remove(s);
            }
        }, delegate(string s) { Extension.AddTag(ref GeneTool.SelectedGene.beardTagFilter, s); });
    }

    private void DrawGizmoThres(int w)
    {
        bool flag = GeneTool.SelectedGene == null;
        if (!flag)
        {
            this.view.Label(0f, 0f, 500f, 30f, Label.RESOURCEGIZMOTHRESHOLD, GameFont.Medium, "");
            this.view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate() { SZWidgets.ActivateLabelEdit(5); }, null);
            this.view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(base.ToggleRemove), new Color?(base.RemoveColor));
            this.view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", delegate() { GeneTool.SelectedGene.CopyGizmoThres(); }, null);
            bool flag2 = !GeneTool.lCopyGizmoThres.NullOrEmpty<float>();
            if (flag2)
            {
                this.view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", delegate() { GeneTool.SelectedGene.PasteGizmoThres(); }, null);
            }

            this.view.Gap(30f);
            SZWidgets.AddLabelEditToList(this.view, 5, ref GeneTool.SelectedGene.resourceGizmoThresholds, null);
            bool isGeneDef = base.IsGeneDef;
            if (isGeneDef)
            {
                this.view.FullListViewFloat(base.WPARAM - 40, GeneTool.SelectedGene.resourceGizmoThresholds, this.bRemoveOnClick, delegate(float s) { GeneTool.SelectedGene.resourceGizmoThresholds.Remove(s); });
            }

            this.view.GapLine(25f);
        }
    }

    private void DrawLifeStages(int w)
    {
        bool flag = !this.selectedDef.IsBodySizeGene();
        if (!flag)
        {
            LifeStageDef lifeStageDef = GeneTool.SelectedGene.GetLifeStageDef();
            bool flag2 = lifeStageDef == null;
            if (!flag2)
            {
                this.view.GapLine(12f);
                Listing_X view = this.view;
                int id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatFieldSlider(view, w, id, FLabel.BodySizeFactor, ref lifeStageDef.bodySizeFactor, 0.1f, 5f, 2);
                Listing_X view2 = this.view;
                id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatZeroFieldSlider(view2, w, id, FLabel.BodyWidth, ref lifeStageDef.bodyWidth, 0.1f, 5f, 2);
                Listing_X view3 = this.view;
                id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatZeroFieldSlider(view3, w, id, FLabel.HeadSizeFactor, ref lifeStageDef.headSizeFactor, 0.1f, 5f, 2);
                Listing_X view4 = this.view;
                id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatFieldSlider(view4, w, id, FLabel.HealthScaleFactor, ref lifeStageDef.healthScaleFactor, 0.1f, 5f, 2);
                Listing_X view5 = this.view;
                id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatFieldSlider(view5, w, id, FLabel.HungerRateFactor, ref lifeStageDef.hungerRateFactor, 0f, 5f, 2);
                Listing_X view6 = this.view;
                id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatFieldSlider(view6, w, id, FLabel.FoodMaxFactor, ref lifeStageDef.foodMaxFactor, 0f, 5f, 2);
                Listing_X view7 = this.view;
                id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatFieldSlider(view7, w, id, FLabel.MeleeDamageFactor, ref lifeStageDef.meleeDamageFactor, 0f, 5f, 2);
                Listing_X view8 = this.view;
                id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatZeroFieldSlider(view8, w, id, FLabel.EyeSizeFactor, ref lifeStageDef.eyeSizeFactor, 0f, 5f, 2);
                Listing_X view9 = this.view;
                id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatFieldSlider(view9, w, id, FLabel.MarketValueFactor, ref lifeStageDef.marketValueFactor, 0f, 5f, 2);
                Listing_X view10 = this.view;
                id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatFieldSlider(view10, w, id, FLabel.VoicePitch, ref lifeStageDef.voxPitch, 0.1f, 5f, 2);
                Listing_X view11 = this.view;
                id = this.id;
                this.id = id + 1;
                SZWidgets.LabelFloatFieldSlider(view11, w, id, FLabel.VoiceVolume, ref lifeStageDef.voxVolume, 0.1f, 5f, 2);
                this.view.CheckboxLabeled(Label.REPRODUCTIVE, 0f, (float)w, ref lifeStageDef.reproductive, null, 2);
                this.view.CheckboxLabeled(Label.CARAVANRIDEABLE, 0f, (float)w, ref lifeStageDef.caravanRideable, null, 2);
                this.view.GapLine(12f);
            }
        }
    }
}
