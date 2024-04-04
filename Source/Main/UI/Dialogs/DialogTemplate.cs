// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogTemplate`1
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

internal abstract class DialogTemplate<T> : Window where T : Def
{
    internal const int WIDTH_EXTENDED = 1000;
    internal const int WIDTH_DEFAULT = 500;
    internal const float WTITLE = 500f;
    private bool bDoOnce;
    internal bool bRemoveOnClick;
    internal string customAcceptLabel;
    private Func<string, string> FGetTagLabel = s => s ?? Label.NONE;
    internal int frameH;
    internal int frameW;
    internal int hScrollParam;
    internal int id;
    private readonly SearchTool.SIndex idxList;
    internal int iTick_AllowRemoveStat;
    internal HashSet<T> lDefs;
    internal HashSet<string> lMods;
    internal bool mInPlacingMode = false;
    internal T oldSelectedDef;
    internal Vector2 scrollPosParam;
    internal SearchTool search;
    internal StatDef selected_StatFactor;
    internal StatDef selected_StatOffset;
    internal T selectedDef;
    private readonly string title;
    internal Listing_X view;
    internal int x;
    internal int xPosOffset;
    internal int y;

    internal DialogTemplate(SearchTool.SIndex listIdx, string _title, int _xPosOffset = 0)
    {
        title = _title;
        xPosOffset = _xPosOffset;
        scrollPosParam = new Vector2();
        view = new Listing_X();
        selected_StatFactor = null;
        selected_StatOffset = null;
        iTick_AllowRemoveStat = 0;
        bRemoveOnClick = false;
        bDoOnce = true;
        idxList = listIdx;
        search = SearchTool.Update(idxList);
        x = 0;
        y = 0;
        lMods = ListModnames();
        Preselection();
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnClickedOutside = true;
        closeOnCancel = true;
        draggable = true;
        layer = CEditor.Layer;
    }

    internal float ELEMENTH => SZWidgets.GetGraphicH<T>();

    internal bool IsGeneDef => selectedDef.GetType() == typeof(GeneDef);

    internal Color RemoveColor => iTick_AllowRemoveStat <= 0 ? Color.white : Color.red;

    internal int WPARAM => 1000 - (int)WindowTool.DefaultToolWindow.x;

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    internal void ToggleRemove()
    {
        iTick_AllowRemoveStat = iTick_AllowRemoveStat > 0 ? 0 : 2000;
    }

    internal virtual HashSet<string> ListModnames()
    {
        return DefTool.ListModnamesWithNull<T>().ToHashSet();
    }

    internal virtual void Preselection()
    {
        ASelectedModName(search.modName);
        selectedDef = lDefs.FirstOrDefault();
    }

    public override void DoWindowContents(Rect inRect)
    {
        SizeAndPosition();
        DrawTitle(x, y, frameW, 30);
        y += 30;
        if (!mInPlacingMode)
            SZWidgets.ButtonImage(frameW - 25, 0.0f, 25f, 25f, "brandom", ARandomDef, col: new Color());
        DrawDropdownModname(x, y, frameW, 30);
        y += 30;
        y += DrawCustomFilter(x, y, frameW);
        y += 2;
        Text.Font = (GameFont)1;
        SZWidgets.ListView(x, y, frameW, frameH + 28 - y, (ICollection<T>)lDefs, (Func<T, string>)(def => def.SLabel()), (Func<T, string>)(def => def.STooltip<T>()), new Func<T, T, bool>(DefTool.DefNameComparator<T>), ref selectedDef, ref search.scrollPos);
        if (!DefTool.DefNameComparator(oldSelectedDef, selectedDef))
        {
            oldSelectedDef = selectedDef;
            if (Prefs.DevMode && selectedDef != null)
                MessageTool.Show(selectedDef.defName);
            OnSelectionChanged();
        }

        if (CEditor.IsExtendedUI)
            DrawParameterBase();
        DrawLowerButtons();
    }

    internal void DrawParameterBase()
    {
        bool flag = this.selectedDef == null;
        if (!flag)
        {
            this.CalcHSCROLL();
            this.id = 1;
            bool flag2 = this.iTick_AllowRemoveStat > 0;
            if (flag2)
            {
                this.iTick_AllowRemoveStat--;
            }
            this.bRemoveOnClick = (this.iTick_AllowRemoveStat > 0);
            Rect outRect = new Rect(WindowTool.DefaultToolWindow.x - 20f, 0f, (float)(this.WPARAM - 12), (float)(this.frameH + 20));
            Rect rect = new Rect(0f, 0f, outRect.width - 16f, (float)this.hScrollParam);
            Widgets.BeginScrollView(outRect, ref this.scrollPosParam, rect, true);
            Rect rect2 = rect.ContractedBy(4f);
            rect2.y -= 4f;
            rect2.height = (float)this.hScrollParam;
            this.view.Begin(rect2);
            this.view.verticalSpacing = 30f;
            this.DrawParameter();
            this.view.End();
            Widgets.EndScrollView();
        }
    }

    internal abstract void CalcHSCROLL();

    internal abstract void DrawParameter();

    internal abstract void AReset();

    internal abstract void AResetAll();

    internal abstract void ASave();

    internal abstract HashSet<T> TList();

    internal virtual void DrawLowerButtons()
    {
        WindowTool.SimpleAcceptAndExtend(this, DoAndClose, AReset, AResetAll, ASave, 1000, customAcceptLabel);
    }

    private void SizeAndPosition()
    {
        if (bDoOnce)
            // ISSUE: cast to a reference type
            SearchTool.SetPosition(idxList, ref windowRect, ref bDoOnce, xPosOffset);
        frameW = (int)base.InitialSize.x - 40;
        frameH = (int)base.InitialSize.y - 115;
        y = 0;
        x = 0;
    }

    internal virtual void DrawTitle(int x, int y, int w, int h)
    {
        Text.Font = (GameFont)2;
        Widgets.Label(new Rect(x, y, w, h), title);
    }

    private void DrawDropdownModname(int x, int y, int w, int h)
    {
        Text.Font = GameFont.Small;
        Rect rect = new Rect((float)x, (float)y, (float)w, (float)h);
        SZWidgets.FloatMenuOnButtonText<string>(rect, FLabel.TString(this.search.modName), this.lMods, new Func<string, string>(FLabel.TString), new Action<string>(this.ASelectedModName), "");
    }


    internal virtual void ASelectedModName(string val)
    {
        search.modName = val;
        lDefs = TList();
    }

    internal abstract int DrawCustomFilter(int x, int y, int w);

    internal abstract void OnAccept();

    internal abstract void OnSelectionChanged();

    private void DoAndClose()
    {
        if (selectedDef != null)
            OnAccept();
        if (mInPlacingMode)
            return;
        base.Close();
    }

    public override void OnAcceptKeyPressed()
    {
        base.OnAcceptKeyPressed();
        DoAndClose();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(this.idxList, this.windowRect.position);
        base.Close(doCloseSound);
    }

    private void ARandomDef()
    {
        DefTool.RandomSearchedDef(lDefs, ref selectedDef);
    }
}
