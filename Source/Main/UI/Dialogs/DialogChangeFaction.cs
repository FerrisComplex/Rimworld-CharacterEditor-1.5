// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogChangeFaction
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

internal class DialogChangeFaction : Window
{
    private bool doOnce;
    private int iChangeTick;
    private readonly List<Faction> lOfFactions;
    private readonly Pawn pawn;
    private Vector2 scrollPos;
    private Faction selectedFaction;

    internal DialogChangeFaction()
    {
        pawn = CEditor.API.Pawn;
        lOfFactions = Find.World.factionManager.AllFactions.OrderByDescending(td => td.def.defName).ToList();
        lOfFactions.Insert(0, null);
        selectedFaction = pawn.Faction;
        scrollPos = new Vector2();
        iChangeTick = 0;
        doOnce = true;
        SearchTool.Update(SearchTool.SIndex.ChangeFaction);
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnCancel = true;
        closeOnClickedOutside = true;
        draggable = true;
        layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    internal string Factionlabel(Faction f)
    {
        if (f == null)
            return Label.NONE;
        return !string.IsNullOrEmpty(f.GetCallLabel()) ? f.GetCallLabel() : f.def.label;
    }

    internal string Factiondescr(Faction f)
    {
        if (f == null)
            return "";
        return !f.IsPlayer ? f.GetInfoText() : f.def.description;
    }

    public override void DoWindowContents(Rect inRect)
    {
        float width = this.InitialSize.x - 50f;
        float height = this.InitialSize.y - 220f;
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.ChangeFaction, ref this.windowRect, ref this.doOnce, 105);
        }
        this.DrawEditLabel(0f, 0f, 340f, 30f);
        Rect rect = new Rect(10f, 30f, 100f, 100f);
        bool flag2 = this.selectedFaction != null;
        if (flag2)
        {
            Widgets.ButtonImage(rect, this.selectedFaction.def.FactionIcon, this.selectedFaction.Color, true);
        }
        Text.Font = GameFont.Small;
        Rect outRect = new Rect(0f, 130f, width, height);
        Rect rect2 = new Rect(0f, 30f, outRect.width - 16f, (float)this.lOfFactions.Count * 35f);
        Widgets.BeginScrollView(outRect, ref this.scrollPos, rect2, true);
        Rect rect3 = rect2.ContractedBy(4f);
        rect3.height = (float)this.lOfFactions.Count * 35f;
        Listing_Standard listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect3);
        GUI.color = Color.white;
        Text.Font = GameFont.Small;
        foreach (Faction faction in this.lOfFactions)
        {
            string label = this.Factionlabel(faction);
            string tooltip = this.Factiondescr(faction);
            bool flag3 = listing_Standard.RadioButton(label, faction == this.selectedFaction, 0f, tooltip, null);
            if (flag3)
            {
                this.selectedFaction = faction;
            }
            listing_Standard.Gap(10f);
        }
        listing_Standard.End();
        Widgets.EndScrollView();
        WindowTool.SimpleAcceptButton(this, new Action(this.DoAndClose));
    }

    private void DoAndClose()
    {
        if (pawn.Faction != selectedFaction)
        {
            if (pawn.Faction == Faction.OfPlayer)
                PawnBanishUtility.Banish(pawn);
            pawn.SetFaction(selectedFaction);
        }

        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.ChangeFaction, this.windowRect.position);
        base.Close(doCloseSound);
    }

    private void DrawEditLabel(float x, float y, float w, float h)
    {
        Text.Font = GameFont.Medium;
        Rect rect = new Rect(x, y, w, h);
        bool flag = this.iChangeTick <= 0;
        if (flag)
        {
            SZWidgets.Label(rect, this.Factionlabel(CEditor.API.Pawn.Faction), delegate()
            {
                this.iChangeTick = 400;
            }, "");
        }
        else
        {
            string text = this.Factionlabel(CEditor.API.Pawn.Faction);
            string text2 = Widgets.TextField(rect, text);
            bool flag2 = !text.Equals(text2);
            if (flag2)
            {
                CEditor.API.Pawn.Faction.Name = text2;
                this.iChangeTick = 400;
            }
            this.iChangeTick--;
        }
    }
}
