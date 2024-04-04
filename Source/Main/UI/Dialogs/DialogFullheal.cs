// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogFullheal
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogFullheal : Window
{
    private readonly Dictionary<Hediff, bool> dicToRemove;
    private bool doOnce;
    private bool isChecked;
    private readonly List<Hediff> lOfHediff;
    private Vector2 scrollPos;

    internal DialogFullheal()
    {
        scrollPos = new Vector2();
        doOnce = true;
        SearchTool.Update(SearchTool.SIndex.FullHeal);
        lOfHediff = CEditor.API.Pawn.health.hediffSet.hediffs;
        dicToRemove = new Dictionary<Hediff, bool>();
        foreach (var key in lOfHediff)
            if (key.Part != null && key.def.hediffClass == typeof(Hediff_AddedPart))
                dicToRemove.Add(key, false);
            else
                dicToRemove.Add(key, true);
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnCancel = true;
        closeOnClickedOutside = true;
        draggable = true;
        layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => new(400f, 500f);

    public override void DoWindowContents(Rect inRect)
    {
        float num = this.InitialSize.x - 40f;
        float height = this.InitialSize.y - 115f;
        if (this.doOnce)
        {
            SearchTool.SetPosition(SearchTool.SIndex.FullHeal, ref this.windowRect, ref this.doOnce, 0);
        }
        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(0f, 0f, 350f, 30f), Label.HEAL);
        Text.Font = GameFont.Small;
        Rect outRect = new Rect(0f, 30f, num, height);
        Rect rect = new Rect(0f, 30f, num - 16f, (float)this.lOfHediff.Count * 29f - 5f);
        Widgets.BeginScrollView(outRect, ref this.scrollPos, rect, true);
        Rect rect2 = rect.ContractedBy(4f);
        rect2.height = (float)this.lOfHediff.Count * 29f;
        Listing_Standard listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect2);
        foreach (Hediff hediff in this.lOfHediff)
        {
            this.isChecked = this.dicToRemove[hediff];
            listing_Standard.CheckboxLabeled(hediff.GetFullLabel(), ref this.isChecked, hediff.TipStringExtra, 0f, 1f);
            this.dicToRemove[hediff] = this.isChecked;
            listing_Standard.Gap(5f);
        }
        listing_Standard.End();
        Widgets.EndScrollView();
        WindowTool.SimpleAcceptButton(this, new Action(this.DoAndClose));
    }


    private void DoAndClose()
    {
        foreach (var key in dicToRemove.Keys)
            try
            {
                if (dicToRemove[key])
                    CEditor.API.Pawn.RemoveHediff(key);
            }
            catch
            {
                if (CEditor.API.Pawn.Dead)
                    ResurrectionUtility.TryResurrect(CEditor.API.Pawn);
            }

        CEditor.API.UpdateGraphics();
        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.FullHeal, this.windowRect.position);
        base.Close(doCloseSound);
    }
}
