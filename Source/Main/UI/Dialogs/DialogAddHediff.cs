// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogAddHediff
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

internal class DialogAddHediff : Window, IPawnable, IPartable
{
    private bool bNeedsPart;
    private bool bNeedsPawn;
    private readonly Dictionary<string, BodyPartRecord> DicOfFilter2;
    private bool doOnce;
    private Hediff example;
    private string extraTipString;
    private readonly Func<HediffDef, HediffDef, bool> FHediffComparator = (h1, h2) => h1 == h2;
    private readonly Func<HediffDef, string> FHediffLabel = h => new TaggedString(h.LabelCap);
    private readonly Func<HediffDef, string> FHediffTooltip = h => h.modContentPack?.Name + "\n" + h.description;
    private HediffComp_Disappears hDisappears;
    private HediffComp_GetsPermanent hPermanent;
    private bool inEditMode;
    private bool isAdjustable;
    private bool isPartSelectionDone;
    private bool isPawnSelectionDone;
    private bool isPermanent;
    private readonly List<string> lOfFilter1;
    private List<HediffDef> lOfHediffs;
    private HediffDef oldSelectedHediff;
    private float oldSeverity;
    private string paramName;
    private Vector2 scrollPos;
    private readonly SearchTool search;
    private int selectedDuration;
    private HediffDef selectedHediff;
    private int selectedLevel;
    private int selectedPain;
    private float selectedSeverity;
    private BodyPartRecord selPart;
    private Pawn selPawn;
    private int yCalc;

    internal DialogAddHediff(Hediff toSelect = null)
    {
        this.scrollPos = default(Vector2);
        this.selectedHediff = null;
        this.oldSelectedHediff = null;
        this.isAdjustable = false;
        this.isPermanent = false;
        this.selectedSeverity = 0.01f;
        this.selectedDuration = -1;
        this.selectedLevel = -1;
        this.selectedPain = -1;
        this.paramName = "";
        this.selPawn = null;
        this.selPart = null;
        this.isPartSelectionDone = false;
        this.isPawnSelectionDone = false;
        this.bNeedsPart = true;
        this.bNeedsPawn = false;
        this.extraTipString = "";
        this.doOnce = true;
        this.search = SearchTool.Update(SearchTool.SIndex.HediffDef);
        this.lOfFilter1 = new List<string>();
        this.lOfFilter1.Add(Label.ALL);
        this.lOfFilter1.Add(Label.HB_ALLIMPLANTS);
        this.lOfFilter1.Add(Label.HB_ALLADDICTIONS);
        this.lOfFilter1.Add(Label.HB_ALLDISEASES);
        this.lOfFilter1.Add(Label.HB_ALLINJURIES);
        this.lOfFilter1.Add(Label.HB_ALLTIME);
        this.lOfFilter1.Add(Label.HB_WITHLEVEL);
        this.DicOfFilter2 = new Dictionary<string, BodyPartRecord>();
        this.DicOfFilter2.Add(Label.ALL, null);
        this.DicOfFilter2.Add(Label.HB_WholeBody, null);
        foreach (BodyPartRecord bodyPartRecord in CEditor.API.Pawn.RaceProps.body.AllParts)
        {
            this.DicOfFilter2.AddLabeled(bodyPartRecord.Label.CapitalizeFirst(), bodyPartRecord);
        }

        this.UpdateHediffList();
        this.selectedHediff = ((toSelect != null) ? toSelect.def : null);
        this.CheckSelectionChanged(toSelect);
        this.doCloseX = true;
        this.absorbInputAroundWindow = true;
        this.closeOnCancel = true;
        this.closeOnClickedOutside = true;
        this.draggable = true;
        this.layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    public BodyPartRecord SelectedPart
    {
        get => selPart;
        set
        {
            selPart = value;
            isPartSelectionDone = true;
            CheckIsReady();
        }
    }

    public Pawn SelectedPawn
    {
        get => selPawn;
        set
        {
            selPawn = value;
            isPawnSelectionDone = true;
            CheckIsReady();
        }
    }

    public Pawn SelectedPawn2 { get; set; }

    public Pawn SelectedPawn3 { get; set; }

    public Pawn SelectedPawn4 { get; set; }

    public override void DoWindowContents(Rect inRect)
    {
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.HediffDef, ref this.windowRect, ref this.doOnce, 105);
        }

        int num = (int)this.InitialSize.x - 40;
        int num2 = (int)this.InitialSize.y - 115;
        int num3 = 0;
        int num4 = 0;
        this.DrawTitle(num4, num3, num, 30);
        num3 += 30;
        this.GetExtraTipString();
        Text.Font = GameFont.Small;
        this.DrawDropdown(num4, num3, num, 30);
        num3 += 92;
        SZWidgets.ListView<HediffDef>((float)num4, (float)num3, (float)num, (float)(num2 - 110 - this.yCalc), this.lOfHediffs, this.FHediffLabel, this.FHediffTooltip, this.FHediffComparator, ref this.selectedHediff, ref this.scrollPos, false, null, true, false, false, false);
        num3 += num2 - 110 - this.yCalc;
        num3 += 10;
        this.CheckSelectionChanged(null);
        this.yCalc = 0;
        this.yCalc += this.DrawAdjustableSeverity(num4, ref num3, num, 45);
        this.yCalc += this.DrawAdjustableLevel(num4, ref num3, num, 45);
        this.yCalc += this.DrawAdjustablePain(num4, ref num3, num, 45);
        this.yCalc += this.DrawAdjustableTime(num4, ref num3, num, 45);
        this.yCalc += this.DrawPermanent(num4, ref num3, num, 25);
        Rect rect = new Rect(this.InitialSize.x - 140f, this.InitialSize.y - 70f, 100f, 30f);
        bool flag2 = !this.inEditMode;
        if (flag2)
        {
            WindowTool.SimpleAcceptButton(this, new Action(this.CheckAndDo));
        }

        SZWidgets.ButtonImageCol(new Rect((float)(num - 30), 0f, 30f, 30f), "bresurrect", new Action(this.AToggleOverride), HealthTool.bIsOverridden ? Color.green : Color.white, "toggle value overriding. white=inactive, green=active\nclick on severity value to input overrides");
    }

    private void CheckAndDo()
    {
        bool flag = this.selectedHediff != null;
        if (flag)
        {
            this.CheckIsReady();
        }
        else
        {
            SearchTool.Save(SearchTool.SIndex.HediffDef, this.windowRect.position);
            this.Close(true);
        }
    }

    private void CheckIsReady()
    {
        if (selectedHediff == null)
            return;
        var flag1 = true;
        var flag2 = false;
        var flag3 = false;
        var _customText = selectedHediff == HediffDefOf.PregnantHuman ? "(" + "Father".Translate().ToString() + ")" : "";
        if (bNeedsPart && !isPartSelectionDone)
        {
            flag1 = false;
            flag2 = true;
        }

        if (bNeedsPawn && !isPawnSelectionDone)
        {
            flag1 = false;
            flag3 = true;
        }

        if (flag1)
        {
            DoAndClose();
        }
        else if (flag2)
        {
            var allowedBodyPartRecords = CEditor.API.Pawn.GetListOfAllowedBodyPartRecords(selectedHediff);
            if (allowedBodyPartRecords.CountAllowNull() <= 1)
                SelectedPart = allowedBodyPartRecords.FirstOrDefault();
            else
                WindowTool.Open(new DialogChoosePart(this, selectedHediff));
        }
        else if (flag3)
        {
            WindowTool.Open(new DialogChoosePawn(this, gender: 0, _customText: _customText));
        }
    }

    private void DoAndClose()
    {
        if (selectedHediff != null)
        {
            CEditor.API.Pawn.AddHediff2(false, selectedHediff, selectedSeverity, SelectedPart, isPermanent, selectedLevel, selectedPain < 0 ? -1 : (int)HealthTool.ConvertSliderToPainCategory(selectedPain), selectedDuration, SelectedPawn);
            StatsReportUtility.Reset();
            CEditor.API.UpdateGraphics();
        }

        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.HediffDef, this.windowRect.position);
        base.Close(doCloseSound);
    }

    public override void OnAcceptKeyPressed()
    {
        base.OnAcceptKeyPressed();
        DoAndClose();
    }

    private void GetExtraTipString()
    {
        try
        {
            extraTipString = example != null ? example.TipStringExtra : "";
        }
        catch
        {
            extraTipString = "";
        }
    }

    private void CheckSelectionChanged(Hediff toSelect = null)
    {
        if (FHediffComparator(oldSelectedHediff, selectedHediff))
            return;
        oldSelectedHediff = selectedHediff;
        selPawn = null;
        isPartSelectionDone = search.filter2 != null && !(search.filter2 == Label.ALL);
        selPart = isPartSelectionDone ? DicOfFilter2.GetValue(search.filter2) : null;
        isPawnSelectionDone = false;
        hDisappears = null;
        hPermanent = null;
        isAdjustable = HealthTool.IsAdjustableSeverity(selectedHediff);
        var allowedBodyPartRecords = CEditor.API.Pawn.GetListOfAllowedBodyPartRecords(selectedHediff);
        bNeedsPart = allowedBodyPartRecords.CountAllowNull() > 0;
        bNeedsPawn = selectedHediff.IsHediffWithOtherPawn();
        inEditMode = toSelect != null;
        example = toSelect != null ? toSelect : HediffMaker.MakeHediff(selectedHediff, CEditor.API.Pawn, allowedBodyPartRecords.FirstOrDefault());
        selectedSeverity = example.Severity;
        selectedLevel = example.GetLevel();
        selectedPain = HealthTool.ConvertPainCategoryToSliderVal((PainCategory)example.GetPainValue());
        selectedDuration = example.GetDuration();
        hDisappears = example.TryGetComp<HediffComp_Disappears>();
        hPermanent = example.TryGetComp<HediffComp_GetsPermanent>();
        if (hDisappears != null)
            hDisappears.Props.showRemainingTime = true;
        if (hPermanent != null)
            isPermanent = hPermanent.IsPermanent;
        example.ShowDebugInfo();
    }

    private void AToggleOverride()
    {
        HealthTool.bIsOverridden = !HealthTool.bIsOverridden;
    }

    private void DrawTitle(int x, int y, int w, int h)
    {
        Text.Font = (GameFont)2;
        Widgets.Label(new Rect(x, y, w - 40, h), inEditMode ? example.LabelCap : Label.ADD_HEDIFF);
    }

    private int DrawPermanent(int x, ref int y, int w, int h)
    {
        if (selectedHediff == null || (!isAdjustable && !HealthTool.bIsOverridden) || (selectedHediff.injuryProps == null && !HealthTool.bIsOverridden))
            return 0;
        SZWidgets.CheckBoxOnChange(new Rect(x, y + 10, w, 24f), new TaggedString("Permanent".Translate()), isPermanent, AChangePermanent);
        y += h;
        return h;
    }

    private void AChangePermanent(bool perma)
    {
        isPermanent = perma;
        if (hPermanent == null)
            return;
        hPermanent.IsPermanent = perma;
    }

    private int DrawAdjustableLevel(int x, ref int y, int w, int h)
    {
        if (selectedHediff == null || example == null || !selectedHediff.IsHediffWithLevel())
            return 0;
        Text.Font = (GameFont)1;
        var listingX = new Listing_X();
        ((Listing)listingX).Begin(new Rect(x, y, w, h));
        var minSeverity = (int)selectedHediff.minSeverity;
        var maxSeverity = (int)HealthTool.GetMaxSeverity(selectedHediff);
        var max = maxSeverity < minSeverity ? 20 : maxSeverity;
        listingX.AddIntSection(new TaggedString("Level".Translate()), "", ref paramName, ref selectedLevel, minSeverity, max, true);
        ((Listing)listingX).End();
        example.SetLevel(selectedLevel);
        example.Severity = selectedLevel;
        selectedSeverity = selectedLevel;
        if (selectedHediff.IsHediffPsycastAbilities())
            try
            {
                example.CallMethod("Reset", null);
            }
            catch
            {
            }

        y += h;
        return h;
    }

    private int DrawAdjustablePain(int x, ref int y, int w, int h)
    {
        if (hPermanent == null || selectedHediff == null)
            return 0;
        Text.Font = (GameFont)1;
        var listingX = new Listing_X();
        ((Listing)listingX).Begin(new Rect(x, y, w, h));
        listingX.AddIntSection(" ", "pain", ref paramName, ref selectedPain, 0, 3, true);
        ((Listing)listingX).End();
        hPermanent.SetPainCategory(HealthTool.ConvertSliderToPainCategory(selectedPain));
        y += h;
        return h;
    }

    private int DrawAdjustableTime(int x, ref int y, int w, int h)
    {
        if (hDisappears == null || selectedHediff == null)
            return 0;
        Text.Font = (GameFont)1;
        var listingX = new Listing_X();
        ((Listing)listingX).Begin(new Rect(x, y, w, h));
        var format = "dauer" + hDisappears.CompLabelInBracketsExtra;
        listingX.AddIntSection(Label.DAUER, format, ref paramName, ref selectedDuration, 0, 220000, true, extraTipString);
        ((Listing)listingX).End();
        hDisappears.ticksToDisappear = selectedDuration;
        y += h;
        return h;
    }

    private int DrawAdjustableSeverity(int x, ref int y, int w, int h)
    {
        if (selectedHediff == null || (!isAdjustable && !HealthTool.bIsOverridden))
            return 0;
        Text.Font = (GameFont)1;
        var listingX = new Listing_X();
        ((Listing)listingX).Begin(new Rect(x, y, w, h));
        var maxSeverity = HealthTool.GetMaxSeverity(selectedHediff);
        selectedSeverity = selectedSeverity <= (double)maxSeverity || HealthTool.bIsOverridden ? selectedSeverity : maxSeverity;
        oldSeverity = selectedSeverity;
        GUI.color = selectedHediff.lethalSeverity < 0.0 || (double)selectedSeverity < selectedHediff.lethalSeverity ? Color.white : Color.red;
        example.Severity = selectedSeverity;
        var labelInBrackets = example.LabelInBrackets;
        var format = !labelInBrackets.NullOrEmpty() ? "comp" + labelInBrackets : selectedHediff.IsHediffWithLevel() ? "int" : selectedHediff.IsAddiction ? "addict" : "";
        listingX.AddSection("ConfigurableSeverity".Translate().ToString().SubstringTo(":"), format, ref paramName, ref selectedSeverity, selectedHediff.minSeverity, maxSeverity, true, extraTipString);
        ((Listing)listingX).End();
        if (oldSeverity != (double)selectedSeverity)
            CEditor.API.UpdateGraphics();
        y += h;
        return h;
    }

    private void DrawDropdown(int x, int y, int w, int h)
    {
        Rect rect = new Rect((float)x, (float)y, (float)w, (float)h);
        SZWidgets.FloatMenuOnButtonText<string>(rect, this.search.SelectedModName, CEditor.API.Get<HashSet<string>>(EType.ModsHediffDef), (string s) => s ?? Label.ALL, new Action<string>(this.ASelectedModName), "");
        Rect rect2 = new Rect(rect);
        rect2.y += 30f;
        SZWidgets.FloatMenuOnButtonText<string>(rect2, this.search.SelectedFilter1, this.lOfFilter1, (string s) => s, new Action<string>(this.ASelectFilter1), "");
        Rect rect3 = new Rect(rect2);
        rect3.y += 30f;
        SZWidgets.FloatMenuOnButtonText<string>(rect3, this.search.SelectedFilter2, this.DicOfFilter2.Keys.ToList<string>(), (string s) => s, new Action<string>(this.ASelectBodyPartRecord), "");
    }

    private void ASelectedModName(string val)
    {
        search.modName = val;
        UpdateHediffList();
    }

    private void ASelectFilter1(string val)
    {
        search.filter1 = val;
        UpdateHediffList();
    }

    private void ASelectBodyPartRecord(string val)
    {
        search.filter2 = val;
        selPart = DicOfFilter2.GetValue(val);
        isPartSelectionDone = val != Label.ALL;
        UpdateHediffList();
    }

    private void UpdateHediffList()
    {
        lOfHediffs = HealthTool.ListOfHediffDef(search.modName, CEditor.API.Pawn, DicOfFilter2.GetValue(search.SelectedFilter2), search.SelectedFilter1, search.SelectedFilter2 == Label.HB_WholeBody);
    }
}
