// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogChoosePart
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogChoosePart : Window
{
    private readonly bool addWholeBody;
    private readonly IPartable callback;
    private readonly int countParts;
    private bool doOnce;
    private readonly HediffDef hediff;
    private readonly List<BodyPartRecord> lOfParts;
    private Vector2 scrollPos;
    private BodyPartRecord selectedPart;

    internal DialogChoosePart(IPartable _callback, HediffDef _hediff)
    {
        hediff = _hediff;
        scrollPos = new Vector2();
        selectedPart = null;
        addWholeBody = false;
        doOnce = true;
        SearchTool.Update(SearchTool.SIndex.ChoosePart);
        callback = _callback;
        lOfParts = CEditor.API.Pawn.GetListOfAllowedBodyPartRecords(hediff);
        countParts = lOfParts.CountAllowNull();
        if (countParts == 1)
            selectedPart = lOfParts[0];
        doCloseX = true;
        absorbInputAroundWindow = true;
        draggable = true;
        layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    public override void DoWindowContents(Rect inRect)
    {
        float frameW = this.InitialSize.x - 40f;
        float frameH = this.InitialSize.y - 115f;
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.ChoosePart, ref this.windowRect, ref this.doOnce, 0);
        }
        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(0f, 0f, 300f, 30f), Label.SELECT_BODYPART);
        this.DrawPartList(frameW, frameH);
        WindowTool.SimpleAcceptButton(this, new Action(this.DoAndClose));
    }

    private void DrawPartList(float frameW, float frameH)
    {
        Text.Font = GameFont.Small;
        Rect outRect = new Rect(0f, 30f, frameW, frameH);
        Rect rect = new Rect(0f, 30f, outRect.width - 16f, (float)this.countParts * 27f - 25f);
        Widgets.BeginScrollView(outRect, ref this.scrollPos, rect, true);
        Rect rect2 = rect.ContractedBy(4f);
        rect2.height = (float)this.countParts * 27f + (float)(this.addWholeBody ? 27 : 0);
        Listing_Standard listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect2);
        bool flag = this.addWholeBody && listing_Standard.RadioButton("WholeBody".Translate(), this.selectedPart == null, 0f, null, null);
        if (flag)
        {
            this.selectedPart = null;
        }
        listing_Standard.Gap(2f);
        foreach (BodyPartRecord bodyPartRecord in this.lOfParts)
        {
            Listing_Standard listing_Standard2 = listing_Standard;
            string label = (bodyPartRecord == null) ? "WholeBody".Translate().ToString() : bodyPartRecord.Label.CapitalizeFirst();
            bool active = this.selectedPart == bodyPartRecord;
            float tabIn = 0f;
            string tooltip;
            if (bodyPartRecord == null)
            {
                tooltip = null;
            }
            else
            {
                BodyPartDef def = bodyPartRecord.def;
                tooltip = ((def != null) ? def.description : null);
            }
            bool flag2 = listing_Standard2.RadioButton(label, active, tabIn, tooltip, null);
            if (flag2)
            {
                this.selectedPart = bodyPartRecord;
            }
            listing_Standard.Gap(2f);
        }
        listing_Standard.End();
        Widgets.EndScrollView();
    }

    private void DoAndClose()
    {
        callback.SelectedPart = selectedPart;
        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.ChoosePart, this.windowRect.position);
        base.Close(doCloseSound);
    }
}
