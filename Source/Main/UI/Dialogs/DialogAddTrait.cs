// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogAddTrait
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

internal class DialogAddTrait : Window
{
    private bool doOnce;
    private readonly Func<StatModifier, string> FStatLabel = t => t != null ? t.stat.LabelCap.ToString() : Label.ALL;
    private readonly List<string> lOfFilters;
    private readonly HashSet<StatModifier> lOfSM;
    private List<KeyValuePair<TraitDef, TraitDegreeData>> lOfTraits;
    private KeyValuePair<TraitDef, TraitDegreeData> oldSelectedTrait;
    private readonly Trait oldTrait;
    private Vector2 scrollPos;
    private readonly SearchTool search;
    private KeyValuePair<TraitDef, TraitDegreeData> selectedTrait;

    internal DialogAddTrait(Trait _trait = null)
    {
        oldTrait = _trait;
        scrollPos = new Vector2();
        doOnce = true;
        search = SearchTool.Update(SearchTool.SIndex.TraitDef);
        lOfSM = TraitTool.ListOfTraitStatModifier(search.modName, true);
        lOfFilters = new List<string>();
        lOfFilters.Add(null);
        lOfFilters.Add(Label.STAT);
        lOfFilters.Add(Label.MENTAL);
        lOfFilters.Add(Label.THOUGHTS);
        lOfFilters.Add(Label.INSPIRATIONS);
        lOfFilters.Add(Label.FOCUS);
        lOfFilters.Add(Label.SKILLGAINS);
        lOfFilters.Add(Label.ABILITIES);
        lOfFilters.Add(Label.NEEDS);
        lOfFilters.Add(Label.INGESTIBLEMOD);
        lOfTraits = TraitTool.ListOfTraitsKeyValuePair(search.modName, (StatModifier)search.ofilter1, search.filter1);
        TraitTool.UpdateDicTooltip(lOfTraits);
        selectedTrait = lOfTraits.FirstOrDefault();
        oldSelectedTrait = selectedTrait;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnCancel = true;
        closeOnClickedOutside = true;
        draggable = true;
        layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    public override void DoWindowContents(Rect inRect)
    {
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.TraitDef, ref this.windowRect, ref this.doOnce, 105);
        }
        int h = (int)this.InitialSize.y - 115;
        int num = (int)this.InitialSize.x - 40;
        int x = 0;
        int y = 0;
        SZWidgets.ButtonImage((float)(num - 25), 0f, 25f, 25f, "brandom", new Action(this.ARandomTrait), "", default(Color));
        y = this.DrawTitle(x, y, num, 30);
        y = this.DrawDropdown(x, y, num);
        y = this.DrawList(x, y, num, h);
        WindowTool.SimpleAcceptButton(this, new Action(this.DoAndClose));
    }

    private int DrawTitle(int x, int y, int w, int h)
    {
        Text.Font = (GameFont)2;
        Widgets.Label(new Rect(x, y, w, h), Label.ADD_TRAIT);
        return h;
    }

    private int DrawDropdown(int x, int y, int w)
    {
        Text.Font = GameFont.Small;
        Rect rect = new Rect((float)x, (float)y, (float)w, 30f);
        SZWidgets.FloatMenuOnButtonText<string>(rect, this.search.SelectedModName, CEditor.API.Get<HashSet<string>>(EType.ModsTraitDef), (string s) => s ?? Label.ALL, new Action<string>(this.AChangedModName), "");
        Rect rect2 = new Rect(rect)
        {
            y = rect.y + 30f
        };
        SZWidgets.FloatMenuOnButtonText<StatModifier>(rect2, this.FStatLabel((StatModifier)this.search.ofilter1), this.lOfSM, this.FStatLabel, new Action<StatModifier>(this.AChangedSM), "");
        SZWidgets.FloatMenuOnButtonText<string>(new Rect(rect2)
        {
            y = rect2.y + 30f
        }, this.search.SelectedFilter1, this.lOfFilters, (string s) => (s == null) ? Label.ALL : s, new Action<string>(this.AChangedCategory), "");
        return y + 90;
    }

    private int DrawList(int x, int y, int w, int h)
    {
        Text.Font = (GameFont)1;
        SZWidgets.ListView(x, y, w, h - y + 30, (ICollection<KeyValuePair<TraitDef, TraitDegreeData>>)lOfTraits, (Func<KeyValuePair<TraitDef, TraitDegreeData>, string>)TraitTool.FTraitLabel, (Func<KeyValuePair<TraitDef, TraitDegreeData>, string>)TraitTool.FTraitTooltip, (Func<KeyValuePair<TraitDef, TraitDegreeData>, KeyValuePair<TraitDef, TraitDegreeData>, bool>)TraitTool.FTraitComparator, ref selectedTrait, ref scrollPos);
        if (!TraitTool.FTraitComparator(oldSelectedTrait, selectedTrait))
        {
            oldSelectedTrait = selectedTrait;
            if (Prefs.DevMode)
                MessageTool.Show(selectedTrait.Key.defName);
        }

        return h - y;
    }

    private void AChangedModName(string val)
    {
        search.modName = val;
        lOfTraits = TraitTool.ListOfTraitsKeyValuePair(search.modName, (StatModifier)search.ofilter1, search.filter1);
        TraitTool.UpdateDicTooltip(lOfTraits);
    }

    private void AChangedSM(StatModifier val)
    {
        search.ofilter1 = val;
        lOfTraits = TraitTool.ListOfTraitsKeyValuePair(search.modName, (StatModifier)search.ofilter1, search.filter1);
        TraitTool.UpdateDicTooltip(lOfTraits);
    }

    private void AChangedCategory(string val)
    {
        search.filter1 = val;
        lOfTraits = TraitTool.ListOfTraitsKeyValuePair(search.modName, (StatModifier)search.ofilter1, search.filter1);
        TraitTool.UpdateDicTooltip(lOfTraits);
    }

    private void ARandomTrait()
    {
        selectedTrait = lOfTraits.RandomElement();
        SZWidgets.sFind = TraitTool.FTraitLabel(selectedTrait);
    }

    private void DoAndClose()
    {
        if (selectedTrait.Key != null)
            CEditor.API.Pawn.AddTrait(selectedTrait.Key, selectedTrait.Value, doChangeSkillValue: true, oldTraitToReplace: oldTrait);
        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.TraitDef, this.windowRect.position);
        base.Close(doCloseSound);
    }

    public override void OnAcceptKeyPressed()
    {
        base.OnAcceptKeyPressed();
        DoAndClose();
    }
}
