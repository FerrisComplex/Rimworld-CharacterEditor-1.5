// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogAddThought
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

internal class DialogAddThought : Window, IPawnable
{
    private bool bAllOk;
    private float baseMoodOffset;
    private bool bNeedOtherPawn;
    private bool bNeedStage;
    private bool bNeedTitle;
    private readonly Dictionary<int, string> dicStages;
    private bool doOnce;
    private HashSet<ThoughtDef> lListThoughts;
    private readonly HashSet<string> lListType;
    private readonly HashSet<string> lModnames;
    private readonly HashSet<RoyalTitleDef> lRoyalTitles;
    private HashSet<ThoughtDef> lTAll;
    private HashSet<ThoughtDef> lTMemory;
    private HashSet<ThoughtDef> lTMemorySocial;
    private HashSet<ThoughtDef> lTSituational;
    private HashSet<ThoughtDef> lTSituationalSocial;
    private HashSet<ThoughtDef> lTUnsupported;
    private Vector2 scrollPos;
    private string selectedModName;
    private float selectedMoodOffset;
    private float selectedOpinionOffset;
    private string selectedStage;
    private ThoughtDef selectedThought;
    private RoyalTitleDef selectedTitle;
    private string selectedType;

    internal DialogAddThought(string _selectedType = null)
    {
        scrollPos = new Vector2();
        SearchTool.Update(SearchTool.SIndex.ThoughtDef);
        baseMoodOffset = 0.0f;
        selectedMoodOffset = 1f;
        selectedOpinionOffset = 1f;
        selectedModName = Label.ALL;
        selectedType = _selectedType ?? Label.ALL;
        SelectedPawn = null;
        selectedStage = null;
        selectedThought = null;
        selectedTitle = null;
        lModnames = DefTool.ListModnamesWithNull((Func<ThoughtDef, bool>)(x => true));
        lListType = new HashSet<string>
        {
            Label.ALL,
            Label.TH_MEMORY,
            Label.TH_SOCIAL,
            Label.TH_SITUATIONAL,
            Label.TH_SITUATIONAL + Label.TH_SOCIAL,
            Label.TH_UNSUPPORTED
        };
        lTMemory = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtMemory);
        lTMemorySocial = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtMemorySocial);
        lTSituational = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtSituational);
        lTSituationalSocial = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtSituationalSocial);
        lTUnsupported = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtUnsupported);
        lTAll = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtsAll);
        lListThoughts = lTAll;
        lRoyalTitles = DefTool.ListBy((Func<RoyalTitleDef, bool>)(x => true));
        dicStages = new Dictionary<int, string>();
        AThoughtTypeSelected(selectedType);
        this.doOnce = true;
        this.doCloseX = true;
        this.absorbInputAroundWindow = true;
        this.closeOnCancel = true;
        this.closeOnClickedOutside = true;
        this.draggable = true;
        layer = CEditor.Layer;
    }

    private Func<ThoughtDef, string> FGetTooltip => t => GetTooltipForThought(t);

    private Func<ThoughtDef, string> FGetThoughtLabel => t => GetLabelForThought(t);

    private Func<RoyalTitleDef, string> FGetRoyalTitleLabel => r => r != null ? r.LabelCap.ToString() : Label.NONE;

    private Func<Pawn, string> FGetPawnName => p => p != null ? p.LabelShort : Label.NONE;

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    public Pawn SelectedPawn { get; set; }

    public Pawn SelectedPawn2 { get; set; }

    public Pawn SelectedPawn3 { get; set; }

    public Pawn SelectedPawn4 { get; set; }

    private string GetLabelForThought(ThoughtDef t)
    {
        if (t == null)
            return Label.NONE;
        return t.defName == "DeadMansApparel" ? t.defName : t.GetThoughtLabel(p: CEditor.API.Pawn);
    }

    private string GetTooltipForThought(ThoughtDef t)
    {
        var str1 = t.SDefname();
        if (t != null)
        {
            var str2 = str1 + "\n\n" + t.GetThoughtDescription() + "\n";
            if (!t.stages.NullOrEmpty())
                for (var index = 0; index < t.stages.Count; ++index)
                    try
                    {
                        var baseMoodEffect = (int)t.stages[index].baseMoodEffect;
                        var baseOpinionOffset = (int)t.stages[index].baseOpinionOffset;
                        str2 = str2 + "\nStage " + index + " BaseMood: " + baseMoodEffect.ToString().Colorize(baseMoodEffect == 0 ? ColorLibrary.Grey : baseMoodEffect > 0 ? ColorLibrary.Green : ColorLibrary.Red);
                        str2 = str2 + " BaseOpinion: " + baseOpinionOffset.ToString().Colorize(baseOpinionOffset == 0 ? ColorLibrary.Grey : baseOpinionOffset > 0 ? ColorLibrary.Green : ColorLibrary.Red);
                    }
                    catch
                    {
                    }

            str1 = str2 + "\n" + t.ThoughtClass;
            if (t.IsTypeOf<Thought_PsychicHarmonizer>())
                str1 += ("\nrequires hediff: " + HediffDefOf.PsychicHarmonizer.LabelCap.ToString()).Colorize(ColorLibrary.Purple);
            if (t.IsTypeOf<Thought_WeaponTrait>())
                str1 += "\nrequires weapon".Colorize(ColorLibrary.Purple);
            if (t.IsTypeOf<ThoughtWorker_WantToSleepWithSpouseOrLover>() || t.IsTypeOf<Thought_OpinionOfMyLover>())
                str1 += "\nrequires love relation".Colorize(ColorLibrary.Purple);
            if (!t.requiredTraits.NullOrEmpty())
                str1 += ("\nrequires trait: " + t.requiredTraits.First().SDefname()).Colorize(ColorLibrary.Purple);
        }

        return str1;
    }

    public override void DoWindowContents(Rect inRect)
    {
        int w = (int)this.InitialSize.x - 40;
        int num = 0;
        int x = 0;
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.ThoughtDef, ref this.windowRect, ref this.doOnce, 105);
        }
        this.DrawTitle(x, ref num, w, 30);
        this.DrawUpperDropdowns(x, ref num, w, 30);
        this.DrawThoughtList(x, ref num, w, 500);
        this.DrawLowerDropdowns(x, ref num, w, 30);
        this.DrawSlider(x, ref num, w, 30);
        this.DrawAccept(x, ref num, w, 30);
    }

    private void DrawTitle(int x, ref int y, int w, int h)
    {
        Text.Font = (GameFont)2;
        SZWidgets.Label(new Rect(x, y, w, h), Label.ADD_THOUGHT);
        GUI.DrawTexture(new Rect(x + w - 32, y, 30f, 30f), ContentFinder<Texture2D>.Get("bmemory"));
        y += 30;
    }

    private void DrawUpperDropdowns(int x, ref int y, int w, int h)
    {
        Text.Font = (GameFont)1;
        SZWidgets.FloatMenuOnButtonText(new Rect(x, y, w, h), selectedModName ?? Label.ALL, lModnames, s => s ?? Label.ALL, AModnameSelected);
        y += 30;
        SZWidgets.FloatMenuOnButtonText(new Rect(x, y, w, h), selectedType ?? Label.ALL, lListType, s => s ?? Label.ALL, AThoughtTypeSelected);
        y += 32;
    }

    private void AModnameSelected(string val)
    {
        selectedModName = val;
        if (selectedModName == null || val == Label.ALL)
        {
            lTMemory = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtMemory);
            lTMemorySocial = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtMemorySocial);
            lTSituational = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtSituational);
            lTSituationalSocial = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtSituationalSocial);
            lTUnsupported = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtUnsupported);
            lTAll = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtsAll);
        }
        else
        {
            lTMemory = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtMemory).Where(t => t.IsFromMod(selectedModName)).ToHashSet();
            lTMemorySocial = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtMemorySocial).Where(t => t.IsFromMod(selectedModName)).ToHashSet();
            lTSituational = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtSituational).Where(t => t.IsFromMod(selectedModName)).ToHashSet();
            lTSituationalSocial = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtSituationalSocial).Where(t => t.IsFromMod(selectedModName)).ToHashSet();
            lTUnsupported = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtUnsupported).Where(t => t.IsFromMod(selectedModName)).ToHashSet();
            lTAll = CEditor.API.Get<HashSet<ThoughtDef>>(EType.ThoughtsAll).Where(t => t.IsFromMod(selectedModName)).ToHashSet();
        }

        AThoughtTypeSelected(selectedType);
    }

    private void AThoughtTypeSelected(string val)
    {
        selectedType = val;
        SelectedPawn = null;
        selectedStage = null;
        selectedThought = null;
        selectedTitle = null;
        dicStages.Clear();
        if (val == Label.TH_MEMORY)
            lListThoughts = lTMemory;
        else if (val == Label.TH_SOCIAL)
            lListThoughts = lTMemorySocial;
        else if (val == Label.TH_SITUATIONAL)
            lListThoughts = lTSituational;
        else if (val == Label.TH_SITUATIONAL + Label.TH_SOCIAL)
            lListThoughts = lTSituationalSocial;
        else if (val == Label.TH_UNSUPPORTED)
            lListThoughts = lTUnsupported;
        else
            lListThoughts = lTAll;
    }

    private void DrawThoughtList(int x, ref int y, int w, int h)
    {
        SZWidgets.ListView(new Rect(x, y, w, h), lListThoughts, FGetThoughtLabel, FGetTooltip, (t1, t2) => t1 == t2, ref selectedThought, ref scrollPos, action: AThoughtSelected);
        y += 510;
    }

    private void AThoughtSelected(ThoughtDef t)
    {
        selectedThought = t;
        SelectedPawn = null;
        selectedStage = null;
        selectedTitle = null;
        dicStages.Clear();
        if (t == null)
            return;
        selectedMoodOffset = 1f;
        var num = selectedThought.stages.CountAllowNull();
        if (!selectedThought.stages.NullOrEmpty())
        {
            for (var index = 0; index < num; ++index)
            {
                var thoughtLabel = t.GetThoughtLabel(index);
                if (thoughtLabel == t.defName)
                    dicStages.Add(index, "");
                else
                    dicStages.Add(index, thoughtLabel);
            }

            selectedStage = dicStages.First().Value;
            baseMoodOffset = (int)t.stages[dicStages.KeyByValue(selectedStage)].baseMoodEffect;
            selectedOpinionOffset = t.stages[dicStages.KeyByValue(selectedStage)].baseOpinionOffset;
        }
        else
        {
            selectedStage = null;
        }
    }

    private void DrawLowerDropdowns(int x, ref int y, int w, int h)
    {
        Text.Font = (GameFont)1;
        if (selectedThought.HasOtherPawnMember())
        {
            bNeedOtherPawn = true;
            SZWidgets.ButtonText(new Rect(x, y, w, h), FGetPawnName(SelectedPawn), ASelectPawn);
            y += 30;
        }
        else
        {
            bNeedOtherPawn = false;
            SelectedPawn = null;
        }

        if (selectedThought != null && selectedThought.stages.CountAllowNull() > 1)
        {
            bNeedStage = true;
            SZWidgets.FloatMenuOnButtonText(new Rect(x, y, w, h), selectedStage, dicStages.Values.ToList(), s => s, AStageSelected);
            y += 30;
        }
        else
        {
            bNeedStage = false;
            selectedStage = null;
        }

        if (selectedThought != null && selectedThought.IsForTitle())
        {
            bNeedTitle = true;
            SZWidgets.FloatMenuOnButtonText(new Rect(x, y, w, h), FGetRoyalTitleLabel(selectedTitle), lRoyalTitles, FGetRoyalTitleLabel, ARoyalTitleSelected);
            y += 30;
        }
        else
        {
            bNeedTitle = false;
            selectedTitle = null;
        }
    }

    private void ASelectPawn()
    {
        WindowTool.Open(new DialogChoosePawn(this, gender: 0));
    }

    private void AStageSelected(string val)
    {
        selectedStage = val;
        if (selectedThought.stages.NullOrEmpty())
            return;
        baseMoodOffset = (int)selectedThought.stages[dicStages.KeyByValue(selectedStage)].baseMoodEffect;
        selectedOpinionOffset = selectedThought.stages[dicStages.KeyByValue(selectedStage)].baseOpinionOffset;
    }

    private void ARoyalTitleSelected(RoyalTitleDef val)
    {
        selectedTitle = val;
    }

    private void DrawSlider(int x, ref int y, int w, int h)
    {
        if (selectedThought == null)
            return;
        var listingX = new Listing_X();
        if (baseMoodOffset != 0.0)
        {
            if (selectedThought.IsTypeOf<Thought_Memory>())
            {
                SZWidgets.SimpleMultiplierSlider(new Rect(x, y, w, 45f), "mood", "int", false, baseMoodOffset, ref selectedMoodOffset, -4f, 5f);
                y += 45;
            }
            else
            {
                var num = (int)(selectedMoodOffset * (double)baseMoodOffset);
                var str = "mood [" + num + "]";
                SZWidgets.Label(new Rect(x, y, w, 30f), str.Colorize(num > 0 ? Color.green : num == 0 ? Color.grey : Color.red));
                y += 30;
            }
        }

        if (selectedThought.IsTypeOf<Thought_MemorySocial>())
        {
            SZWidgets.SimpleMultiplierSlider(new Rect(x, y, w, 45f), "opinion", "int", false, 1f, ref selectedOpinionOffset, -100f, 100f);
            y += 45;
        }
        else
        {
            if (selectedOpinionOffset == 0.0 && SelectedPawn == null)
                return;
            SZWidgets.Label(new Rect(x, y, w, 30f), "opinion [" + (int)selectedOpinionOffset + "]");
            y += 30;
        }
    }

    private void DrawAccept(int x, ref int y, int w, int h)
    {
        bAllOk = selectedThought != null && (!bNeedOtherPawn || SelectedPawn != null) && (!bNeedTitle || selectedTitle != null) && (!bNeedStage || selectedStage != null);
        GUI.color = !bAllOk || (bNeedStage && selectedStage.NullOrEmpty()) ? Color.red : Color.green;
        if (!bAllOk)
            return;
        WindowTool.SimpleAcceptButton(this, DoAndClose);
    }

    private void DoAndClose()
    {
        var stage = 0;
        if (selectedStage != null)
            stage = dicStages.KeyByValue(selectedStage);
        CEditor.API.Pawn.AddThought(selectedThought, SelectedPawn, stage, selectedTitle.SDefname(), selectedMoodOffset, selectedOpinionOffset);
        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.ThoughtDef, this.windowRect.position);
        base.Close(doCloseSound);
    }
}
