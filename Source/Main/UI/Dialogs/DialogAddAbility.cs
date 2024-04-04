// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogAddAbility
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

internal class DialogAddAbility : Window
{
    private readonly string alllevels;
    private bool doOnce;
    private readonly Func<AbilityDef, AbilityDef, bool> FAbilityComparator = (a1, a2) => a1.defName == a2.defName;

    private readonly Func<AbilityDef, string> FAbilityLabel = a =>
    {
        var labelCap = a.LabelCap;
        if (!labelCap.NullOrEmpty())
            return a.LabelCap.ToString();
        return !a.label.NullOrEmpty() ? a.label : "";
    };

    private readonly Func<AbilityDef, string> FAbilityTooltip = a => a.GetTooltip();
    private List<AbilityDef> lOfAbilities;
    private readonly List<string> lOfStufen;
    private AbilityDef oldSelectedAbility;
    private readonly string prepend;
    private Vector2 scrollPos;
    private AbilityDef selectedAbility;
    private string selectedModName;
    private string selectedStufe;

    internal DialogAddAbility()
    {
        this.scrollPos = default(Vector2);
        this.doOnce = true;
        SearchTool.Update(SearchTool.SIndex.AbilityDef);
        this.selectedModName = null;
        this.selectedStufe = null;
        this.prepend = "Level".Translate() + " ";
        this.alllevels = Label.ALL;
        int num = 6;
        this.lOfStufen = new List<string>();
        this.lOfStufen.Add(this.alllevels);
        for (int i = 0; i <= num; i++)
        {
            this.lOfStufen.Add(this.prepend + i.ToString());
        }
        this.lOfAbilities = DefTool.ListByMod<AbilityDef>(null).ToList<AbilityDef>();
        this.selectedAbility = this.lOfAbilities.FirstOrDefault<AbilityDef>();
        this.oldSelectedAbility = this.selectedAbility;
        this.doCloseX = true;
        this.absorbInputAroundWindow = true;
        this.closeOnCancel = true;
        this.closeOnClickedOutside = true;
        this.draggable = true;
        this.layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    public override void DoWindowContents(Rect inRect)
    {
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.AbilityDef, ref this.windowRect, ref this.doOnce, 105);
        }
        int num = (int)this.InitialSize.x - 40;
        int num2 = (int)this.InitialSize.y - 115;
        int num3 = 0;
        int num4 = 0;
        this.DrawTitle(num4, num3, num, 30);
        num3 += 30;
        SZWidgets.ButtonImage((float)(num - 25), 0f, 25f, 25f, "brandom", new Action(this.ARandomAbility), "", default(Color));
        this.DrawDropdown(num4, num3, num, 30);
        num3 += 30;
        this.DrawFilter(num4, num3, num, 30);
        num3 += 30;
        Text.Font = GameFont.Small;
        SZWidgets.ListView<AbilityDef>((float)num4, (float)num3, (float)num, (float)(num2 - 64), this.lOfAbilities, this.FAbilityLabel, this.FAbilityTooltip, this.FAbilityComparator, ref this.selectedAbility, ref this.scrollPos, false, null, true, false, false, false);
        bool flag2 = !this.FAbilityComparator(this.oldSelectedAbility, this.selectedAbility);
        if (flag2)
        {
            this.oldSelectedAbility = this.selectedAbility;
            bool devMode = Prefs.DevMode;
            if (devMode)
            {
                MessageTool.Show(this.selectedAbility.defName, null);
            }
        }
        WindowTool.SimpleAcceptButton(this, new Action(this.DoAndClose));
    }

    private void DrawTitle(int x, int y, int w, int h)
    {
        Text.Font = (GameFont)2;
        Widgets.Label(new Rect(x, y, w, h), Label.PSYTALENTE);
    }

    private void DrawDropdown(int x, int y, int w, int h)
    {
        Text.Font = GameFont.Small;
        Rect rect = new Rect((float)x, (float)y, (float)w, (float)h);
        SZWidgets.FloatMenuOnButtonText<string>(rect, this.selectedModName ?? Label.ALL, CEditor.API.Get<HashSet<string>>(EType.ModsAbilityDef), (string s) => s ?? Label.ALL, new Action<string>(this.ASelectedModName), "");
    }

    
    private void DrawFilter(int x, int y, int w, int h)
    {
        Text.Font = GameFont.Small;
        Rect rect = new Rect((float)x, (float)y, (float)w, (float)h);
        SZWidgets.FloatMenuOnButtonText<string>(rect, this.selectedStufe ?? Label.ALL, this.lOfStufen, (string s) => s ?? Label.ALL, new Action<string>(this.ASelectedStufe), "");
    }

    private void ASelectedModName(string val)
    {
        selectedModName = val;
        lOfAbilities = DefTool.ListByMod<AbilityDef>(selectedModName).ToList();
    }

    private void ASelectedStufe(string val)
    {
        var level = val.Replace(alllevels, "").Replace(prepend, "").AsInt32();
        selectedStufe = val;
        var bAll = val == null || val == Label.ALL;
        lOfAbilities = DefTool.ListByMod<AbilityDef>(selectedModName).ToList().Where(td => bAll || td.level == level).OrderBy(td => td.label).ToList();
    }

    private void DoAndClose()
    {
        if (selectedAbility != null)
        {
            CEditor.API.Pawn.CheckAddPsylink();
            CEditor.API.Pawn.abilities.GainAbility(selectedAbility);
        }

        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.AbilityDef, this.windowRect.position);
        base.Close(doCloseSound);
    }

    public override void OnAcceptKeyPressed()
    {
        base.OnAcceptKeyPressed();
        DoAndClose();
    }

    private void ARandomAbility()
    {
        selectedAbility = lOfAbilities.RandomElement();
        SZWidgets.sFind = FAbilityLabel(selectedAbility);
    }
}
