// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogChangeRace
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogChangeRace : Window
{
    private bool doOnce;
    private readonly bool humanlike;
    private string key;
    private HashSet<PawnKindDef> lpkd;
    private HashSet<ThingDef> lraces;
    private readonly Pawn pawn;
    private ThingDef raceDef;
    private Vector2 scrollPos;
    private readonly SearchTool search;
    private PawnKindDef selectedPKD;

    internal DialogChangeRace(Pawn _pawn)
    {
        pawn = _pawn;
        humanlike = pawn.RaceProps.Humanlike;
        scrollPos = new Vector2();
        search = SearchTool.Update(SearchTool.SIndex.Race);
        doOnce = true;
        selectedPKD = null;
        raceDef = pawn.def;
        this.search.ofilter1 = (this.search.ofilter1 == null || (bool)this.search.ofilter1);
        selectedPKD = pawn?.kindDef;
        key = pawn == null || pawn.RaceProps.Humanlike ? Label.COLONISTS : Label.WILDANIMALS;
        UpdateRacesList();
        UpdatePKDList();
        this.doCloseX = true;
        this.absorbInputAroundWindow = true;
        this.closeOnCancel = true;
        this.closeOnClickedOutside = true;
        this.draggable = true;
        layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    private void UpdateRacesList()
    {
        var nonhumanlike = CEditor.ListName == Label.COLONYANIMALS || CEditor.ListName == Label.WILDANIMALS;
        lraces = PawnKindTool.ListOfRaces(CEditor.ListName == Label.HUMANOID || CEditor.ListName == Label.COLONISTS, nonhumanlike);
    }

    private void UpdatePKDList()
    {
        lpkd = PawnKindTool.ListOfPawnKindDefByRace(raceDef, humanlike, !humanlike);
    }

    public override void DoWindowContents(Rect inRect)
    {
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.Race, ref this.windowRect, ref this.doOnce, 105);
        }
        int h = (int)this.InitialSize.y - 115;
        int num = (int)this.InitialSize.x - 40;
        int x = 0;
        int y = 0;
        y = this.DrawTitle(x, y, num, 30);
        y = this.DrawDropdown(x, y, num);
        y = this.DrawList(x, y, num, h);
        SZWidgets.CheckBoxOnChange(new Rect(0f, this.InitialSize.y - 105f, (float)num, 25f), Label.RACESPECIFICDRESS, (bool)this.search.ofilter1, new Action<bool>(this.ARedress));
        WindowTool.SimpleAcceptButton(this, new Action(this.DoAndClose));
    }

    private void DoAndClose()
    {
        RaceTool.ChangeRace(pawn, pawn.RaceProps.Humanlike ? selectedPKD : raceDef.race.AnyPawnKind, (bool)search.ofilter1);
        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.Race, this.windowRect.position);
        base.Close(doCloseSound);
    }

    private void ARedress(bool val)
    {
        search.ofilter1 = val;
    }

    private int DrawTitle(int x, int y, int w, int h)
    {
        Text.Font = (GameFont)2;
        Widgets.Label(new Rect(x, y, w - 40, h), Label.CHANGERACE);
        return h;
    }

    private int DrawDropdown(int x, int y, int w)
    {
        Text.Font = GameFont.Small;
        Rect rect = new Rect((float)x, (float)y, (float)w, 30f);
        SZWidgets.FloatMenuOnButtonText<ThingDef>(rect, CEditor.RACENAME(this.raceDef), this.lraces, CEditor.RACENAME, new Action<ThingDef>(this.AChangeRace), "");
        return y + 30;
    }

    private int DrawList(int x, int y, int w, int h)
    {
        SZWidgets.ListView(new Rect(x, y, w, h - y), lpkd, CEditor.PKDNAME, CEditor.PKDTOOLTIP, (pkd1, pkd2) => pkd1 == pkd2, ref selectedPKD, ref scrollPos);
        return h - y;
    }

    private void AChangeRace(ThingDef race)
    {
        raceDef = race;
        UpdatePKDList();
    }
}
