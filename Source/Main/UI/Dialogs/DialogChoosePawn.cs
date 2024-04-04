// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogChoosePawn
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

internal class DialogChoosePawn : Window
{
    private readonly IPawnable callback;
    private readonly Gender choosenGender;
    private readonly string customText;
    private bool doOnce;
    private readonly int id;
    private readonly List<Pawn> lOfPawns;
    private Vector2 scrollPos;
    private string selectedListname;
    private Pawn selectedPawn;

    internal DialogChoosePawn(IPawnable _callback, int _id = 1, Gender gender = 0, string _customText = "")
    {
        scrollPos = new Vector2();
        id = _id;
        doOnce = true;
        SearchTool.Update(SearchTool.SIndex.OtherPawn);
        customText = _customText;
        choosenGender = gender;
        lOfPawns = new List<Pawn>();
        DicFactions.Clear();
        DicFactions.Merge(FactionTool.GetDicOfFactions());
        callback = _callback;
        selectedPawn = null;
        AFactionSelected(DicFactions.First().Key);
        doCloseX = true;
        absorbInputAroundWindow = true;
        draggable = true;
        layer = CEditor.Layer;
    }

    private Dictionary<string, Faction> DicFactions => CEditor.API.DicFactions;

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    private Func<Pawn, string> FPawnLabel => p => p.GetPawnName(true);

    private Func<Pawn, string> FPawnTooltip => p => p.GetPawnDescription();

    private Func<Pawn, Pawn, bool> FPawnComparator => (p1, p2) => p1 == p2;

    public override void DoWindowContents(Rect inRect)
    {
        int w = (int)this.InitialSize.x - 40;
        int num = 0;
        int x = 0;
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.OtherPawn, ref this.windowRect, ref this.doOnce, 0);
        }
        this.DrawTitle(x, ref num, w, 30);
        this.DrawDropdowns(x, ref num, w, 30);
        this.DrawPawnList(x, ref num, w, (int)this.InitialSize.y - num - 74);
        WindowTool.SimpleAcceptButton(this, new Action(this.DoAndClose));
    }

    private void DrawTitle(int x, ref int y, int w, int h)
    {
        Text.Font = (GameFont)2;
        Widgets.Label(new Rect(x, y, w, h), Label.CHOOSE_PAWN + " " + customText);
        if (choosenGender > 0)
            SZWidgets.Image(new Rect(x + w - 20, y + 8, 20f, 20f), choosenGender == Gender.Male ? "bmale" : "bfemale");
        y += 30;
    }

    internal void DrawDropdowns(int x, ref int y, int w, int h)
    {
        Text.Font = (GameFont)1;
        SZWidgets.FloatMenuOnButtonText(new Rect(x, y, w, h), selectedListname, DicFactions.Keys.ToList(), s => s, AFactionSelected);
        y += 32;
    }

    private void AFactionSelected(string listname)
    {
        this.selectedListname = listname;
        this.lOfPawns.Clear();
        List<Pawn> pawnList = PawnxTool.GetPawnList(this.selectedListname, false, this.DicFactions.GetValue(this.selectedListname));
        bool flag = this.choosenGender > Gender.None;
        if (flag)
        {
            foreach (Pawn pawn in pawnList)
            {
                bool flag2 = pawn.gender == this.choosenGender;
                if (flag2)
                {
                    this.lOfPawns.Add(pawn);
                }
            }
        }
        else
        {
            this.lOfPawns.AddRange(pawnList);
        }
        this.selectedPawn = this.lOfPawns.FirstOrFallback(null);
    }

    private void DrawPawnList(int x, ref int y, int w, int h)
    {
        SZWidgets.ListView(x, y, w, h, lOfPawns, FPawnLabel, FPawnTooltip, FPawnComparator, ref selectedPawn, ref scrollPos);
    }

    private void DoAndClose()
    {
        if (id <= 1)
            callback.SelectedPawn = selectedPawn;
        else if (id == 2)
            callback.SelectedPawn2 = selectedPawn;
        else if (id == 3)
            callback.SelectedPawn3 = selectedPawn;
        else if (id == 4)
            callback.SelectedPawn4 = selectedPawn;
        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.OtherPawn, this.windowRect.position);
        base.Close(doCloseSound);
    }
}
