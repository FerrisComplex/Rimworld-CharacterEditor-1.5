// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogAddStat
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogAddStat : Window
{
    private bool doOnce;
    private Func<StatCategoryDef, string> FGetStatCategoryLabel;
    private Func<StatDef, string> FGetStatLabel;
    private readonly bool isEquip;
    private readonly bool isWeapon;
    private List<StatDef> lOfStat;
    private List<StatCategoryDef> lOfStatCat;
    private StatCategoryDef selectedStatCategoryDef;
    private StatDef selectedStatDef;
    private readonly ThingDef thingDef;

    internal DialogAddStat(ThingDef _thingDef, bool _isWeapon, bool _isEquip)
    {
        thingDef = _thingDef;
        isWeapon = _isWeapon;
        isEquip = _isEquip;
        doOnce = true;
        SearchTool.Update(SearchTool.SIndex.AddStat);
        lOfStatCat = !isWeapon ? !isEquip ? CEditor.API.ListOf<StatCategoryDef>(EType.StatCategoryApparel) : CEditor.API.ListOf<StatCategoryDef>(EType.StatCategoryOnEquip) : CEditor.API.ListOf<StatCategoryDef>(EType.StatCategoryWeapon);
        lOfStat = new List<StatDef>();
        FGetStatLabel = GetStatLabel;
        FGetStatCategoryLabel = GetStatCategoryLabel;
        this.doCloseX = true;
        this.absorbInputAroundWindow = true;
        this.closeOnCancel = true;
        this.closeOnClickedOutside = true;
        this.draggable = true;
        this.layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => new(600f, 100f);

    private string GetStatLabel(StatDef s)
    {
        return s == null ? Label.NONE : s.label;
    }

    private string GetStatCategoryLabel(StatCategoryDef s)
    {
        return s == null ? Label.ALL : s.label;
    }

    public override void DoWindowContents(Rect inRect)
    {
        float w = this.InitialSize.x - 40f;
        float num = this.InitialSize.y - 104f;
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.AddStat, ref this.windowRect, ref this.doOnce, 0);
        }
        Text.Font = GameFont.Medium;
        SZWidgets.Label(0f, 0f, w, 30f, Label.ADDSTAT, null);
        Text.Font = GameFont.Small;
        SZWidgets.Label(0f, 35f, 100f, 30f, Label.STATTYPE, null);
    }

    private void ASetStatCategory(StatCategoryDef statCategoryDef)
    {
        selectedStatCategoryDef = statCategoryDef;
        selectedStatDef = null;
        lOfStat = CEditor.API.ListOfStatDef(statCategoryDef, isWeapon, isEquip);
    }

    private void ASetStat(StatDef stat)
    {
        selectedStatDef = stat;
    }

    private void AAddStat()
    {
        if (thingDef == null)
            return;
        if (isEquip)
            thingDef.AddEquipStat(selectedStatDef, 0.0f);
        else
            thingDef.AddStat(selectedStatDef, 0.0f);
        if (selectedStatDef == StatDefOf.EnergyShieldEnergyMax || selectedStatDef == StatDefOf.EnergyShieldRechargeRate)
        {
            var mt = thingDef.apparel.layers.Contains(ApparelLayerDefOf.Belt) ? MessageTypeDefOf.SilentInput : MessageTypeDefOf.RejectInput;
            MessageTool.Show(Label.ONLYFORSHIELD, mt);
            thingDef.ResolveReferences();
            thingDef.PostLoad();
        }

        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.AddStat, this.windowRect.position);
        base.Close(doCloseSound);
    }
}
