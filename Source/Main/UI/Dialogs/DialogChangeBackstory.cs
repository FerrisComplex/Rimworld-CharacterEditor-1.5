// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogChangeBackstory
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

internal class DialogChangeBackstory : Window
{
    private Dictionary<BackstoryDef, string> dicBackstory;
    private readonly Dictionary<string, SkillDef> dicOfFilters;
    private bool doOnce;
    private readonly Func<BackstoryDef, BackstoryDef, bool> FBackstoryComparator;
    private readonly Func<BackstoryDef, string> FBackstoryLabel;
    private readonly Func<BackstoryDef, string> FBackstoryTooltip;
    private readonly Gender gender;
    private readonly bool isChildhood;
    private bool isFiltered;
    private bool isFilteredOld;
    private List<BackstoryDef> lOfBackstories;
    private readonly HashSet<string> lOfCategories;
    private readonly List<string> lOfFilter2;
    private BackstoryDef oBackAdult;
    private BackstoryDef oBackChild;
    private Vector2 scrollPos;
    private BackstoryDef selectedBackstory;
    private string selectedCategory;
    private KeyValuePair<string, SkillDef> selectedFilter;
    private string selectedFilter2;

    internal DialogChangeBackstory(bool _isChildhood)
    {
        RememberOldBackstory();
        scrollPos = new Vector2();
        isChildhood = _isChildhood;
        selectedBackstory = isChildhood ? CEditor.API.Pawn.story.Childhood : CEditor.API.Pawn.story.Adulthood;
        selectedCategory = Label.ALL;
        selectedFilter = new KeyValuePair<string, SkillDef>(Label.ALL, null);
        selectedFilter2 = Label.ALL;
        isFiltered = false;
        isFilteredOld = false;
        doOnce = true;
        gender = CEditor.API.Pawn.gender;
        MessageTool.Show("backstory=" + selectedBackstory.SDefname());
        UpdateDictionary();
        SearchTool.Update(_isChildhood ? SearchTool.SIndex.BackstoryDefChild : SearchTool.SIndex.BackstroryDefAdult);
        var list = dicBackstory.Keys.Select(td => td.spawnCategories).ToList();
        lOfCategories = new HashSet<string>();
        lOfCategories.Add(Label.ALL);
        foreach (var stringList in list)
        foreach (var str in stringList)
            lOfCategories.Add(str);
        dicOfFilters = new Dictionary<string, SkillDef>();
        dicOfFilters.Add(Label.ALL, null);
        foreach (var skill in CEditor.API.Pawn.skills.skills)
        {
            dicOfFilters.Add(skill.def.LabelCap.ToString() + " +", skill.def);
            dicOfFilters.Add(skill.def.LabelCap.ToString() + " ++", skill.def);
            dicOfFilters.Add(skill.def.LabelCap.ToString() + " +++", skill.def);
        }

        lOfFilter2 = new List<string>();
        lOfFilter2.Add(Label.ALL);
        lOfFilter2.Add(Label.POSITIVE_ONLY);
        lOfFilter2.Add(Label.SUMME + " > 3");
        lOfFilter2.Add(Label.SUMME + " > 4");
        lOfFilter2.Add(Label.SUMME + " > 5");
        lOfFilter2.Add(Label.SUMME + " > 6");
        lOfFilter2.Add(Label.SUMME + " > 7");
        lOfFilter2.Add(Label.SUMME + " > 8");
        lOfFilter2.Add(Label.SUMME + " > 9");
        lOfFilter2.Add(Label.SUMME + " > 10");
        lOfFilter2.Add(Label.SUMME + " > 11");
        lOfFilter2.Add(Label.SUMME + " > 12");
        FBackstoryLabel = GetBackstoryLabel;
        FBackstoryTooltip = GetBackstoryTooltip;
        FBackstoryComparator = IsSameBackstory;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnCancel = true;
        closeOnClickedOutside = true;
        draggable = true;
        layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    internal void RememberOldBackstory()
    {
        oBackAdult = CEditor.API.Pawn.story.GetBackstory((BackstorySlot)1);
        oBackChild = CEditor.API.Pawn.story.GetBackstory(0);
    }

    internal void RecalcSkills()
    {
        var backstory1 = CEditor.API.Pawn.story.GetBackstory((BackstorySlot)1);
        var backstory2 = CEditor.API.Pawn.story.GetBackstory(0);
        foreach (var skill in CEditor.API.Pawn.skills.skills)
        {
            var skillRecord = skill;
            var skillRecord1 = skillRecord;
            skillRecord1.levelInt = skillRecord1.levelInt - (oBackAdult == null ? 0 : oBackAdult.skillGains.Select(x => !x.skill.Equals(skillRecord.def) ? 0 : x.amount).Sum());
            var skillRecord2 = skillRecord;
            skillRecord2.levelInt = skillRecord2.levelInt - (oBackChild == null ? 0 : oBackChild.skillGains.Select(x => !x.skill.Equals(skillRecord.def) ? 0 : x.amount).Sum());
            var skillRecord3 = skillRecord;
            skillRecord3.levelInt = skillRecord3.levelInt + (backstory1 == null ? 0 : backstory1.skillGains.Select(x => !x.skill.Equals(skillRecord.def) ? 0 : x.amount).Sum());
            var skillRecord4 = skillRecord;
            skillRecord4.levelInt = skillRecord4.levelInt + (backstory2 == null ? 0 : backstory2.skillGains.Select(x => !x.skill.Equals(skillRecord.def) ? 0 : x.amount).Sum());
        }
    }

    private string GetBackstoryLabel(BackstoryDef backstory)
    {
        return backstory.TitleCapFor(gender);
    }

    private string GetBackstoryTooltip(BackstoryDef backstory)
    {
        return dicBackstory[backstory];
    }

    private bool IsSameBackstory(BackstoryDef a, BackstoryDef b)
    {
        return a == b;
    }

    private void UpdateDictionary()
    {
        this.dicBackstory = new Dictionary<BackstoryDef, string>();
        var list = DefDatabase<BackstoryDef>.AllDefs.Where(td =>
        {
            if (td == null || (int)td.slot != (isChildhood ? 0 : 1) || string.IsNullOrEmpty(td.title))
                return false;
            return !isFiltered || !td.DisabledWorkTypes.Any();
        }).OrderBy(td => td.title).ToList();
        var flag1 = selectedCategory == null || selectedCategory == Label.ALL;
        var flag2 = selectedFilter.Key == null || selectedFilter.Key == Label.ALL;
        var flag3 = selectedFilter2 == null || selectedFilter2 == Label.ALL;
        foreach (var backstoryDef in list)
        {
            var flag4 = false;
            if ((flag1 ? 1 : backstoryDef.spawnCategories.Contains(selectedCategory) ? 1 : 0) != 0)
            {
                if (flag2)
                {
                    flag4 = true;
                }
                else if (selectedFilter.Value != null)
                {
                    var source = backstoryDef.skillGains.Where(x => x.skill.Equals(selectedFilter.Value));
                    flag4 = false;
                    var num = source.Sum(x => x.amount);
                    if (selectedFilter.Key.EndsWith("+++"))
                    {
                        if (num > 3)
                            flag4 = true;
                    }
                    else if (selectedFilter.Key.EndsWith("++"))
                    {
                        if (num > 2)
                            flag4 = true;
                    }
                    else if (selectedFilter.Key.EndsWith("+") && num > 0)
                    {
                        flag4 = true;
                    }
                }

                if (flag4)
                {
                    if (flag3)
                    {
                        flag4 = true;
                    }
                    else if (selectedFilter2.Contains(" > "))
                    {
                        flag4 = false;
                        var num1 = 0;
                        var num2 = selectedFilter2.SubstringFrom(">").Trim().AsInt32();
                        foreach (var skillGain in backstoryDef.skillGains)
                            num1 += skillGain.amount;
                        if (num1 > num2)
                            flag4 = true;
                    }
                    else if (selectedFilter2 == Label.POSITIVE_ONLY)
                    {
                        flag4 = true;
                        foreach (var skillGain in backstoryDef.skillGains)
                            flag4 &= skillGain.amount > 0;
                    }
                }

                if (flag4)
                {
                    this.dicBackstory.Add(backstoryDef, backstoryDef.FullDescriptionFor(CEditor.API.Pawn).Resolve());
                }
            }
        }

        lOfBackstories = this.dicBackstory.Keys.ToList();
    }

    public override void DoWindowContents(Rect inRect)
    {
        if (this.doOnce)
        {
            SearchTool.SetPosition(this.isChildhood ? SearchTool.SIndex.BackstoryDefChild : SearchTool.SIndex.BackstroryDefAdult, ref this.windowRect, ref this.doOnce, 105);
        }
        float num = this.InitialSize.x - 40f;
        float num2 = this.InitialSize.y - 150f;
        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(0f, 0f, num, 30f), this.isChildhood ? "Childhood".Translate() : "Adulthood".Translate());
        SZWidgets.ButtonImage(num - 25f, 0f, 25f, 25f, "brandom", new Action(this.ARandomBackstory), "", default(Color));
        Text.Font = GameFont.Small;
        Widgets.CheckboxLabeled(new Rect(0f, 30f, num, 30f), Label.NO_BLOCKING_SKILLS, ref this.isFiltered, false, null, null, false, false);
        if (this.isFiltered != this.isFilteredOld)
        {
            this.isFilteredOld = this.isFiltered;
            this.UpdateDictionary();
        }
        SZWidgets.FloatMenuOnButtonText<string>(new Rect(0f, 60f, num, 30f), this.selectedCategory ?? Label.ALL, this.lOfCategories, (string s) => s, new Action<string>(this.ACategoryChanged), "");
        SZWidgets.FloatMenuOnButtonText<string>(new Rect(0f, 90f, num, 30f), this.selectedFilter.Key, this.dicOfFilters.Keys.ToList<string>(), (string s) => s, new Action<string>(this.AFilterChanged), "");
        SZWidgets.FloatMenuOnButtonText<string>(new Rect(0f, 120f, num, 30f), this.selectedFilter2, this.lOfFilter2, (string s) => s, new Action<string>(this.AFilter2Changed), "");
        SZWidgets.ListView<BackstoryDef>(0f, 150f, num, num2 - 85f, this.lOfBackstories, this.FBackstoryLabel, this.FBackstoryTooltip, this.FBackstoryComparator, ref this.selectedBackstory, ref this.scrollPos, false, null, true, false, false, false);
        WindowTool.SimpleAcceptButton(this, new Action(this.DoAndClose));
        if ((this.isChildhood && CEditor.API.Pawn.ageTracker.AgeBiologicalYears < 3) || !this.isChildhood)
        {
            WindowTool.SimpleCustomButton(this, 0, new Action(this.DoRemoveAndClose), "Remove".Translate(), "");
        }
    }

    private void ACategoryChanged(string val)
    {
        selectedCategory = val;
        UpdateDictionary();
    }

    private void AFilterChanged(string val)
    {
        selectedFilter = new KeyValuePair<string, SkillDef>(val, dicOfFilters[val]);
        UpdateDictionary();
    }

    private void AFilter2Changed(string val)
    {
        selectedFilter2 = val;
        UpdateDictionary();
    }

    private void DoRemoveAndClose()
    {
        selectedBackstory = null;
        DoAndClose();
    }

    private void DoAndClose()
    {
        CEditor.API.Pawn.SetBackstory(isChildhood ? selectedBackstory : CEditor.API.Pawn.story?.Childhood, !isChildhood ? selectedBackstory : (BackstoryDef)CEditor.API.Pawn.story?.Adulthood);
        RecalcSkills();
        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(this.isChildhood ? SearchTool.SIndex.BackstoryDefChild : SearchTool.SIndex.BackstroryDefAdult, this.windowRect.position);
        base.Close(doCloseSound);
    }

    public override void OnAcceptKeyPressed()
    {
        base.OnAcceptKeyPressed();
        DoAndClose();
    }

    private void ARandomBackstory()
    {
        selectedBackstory = lOfBackstories.RandomElement();
        SZWidgets.sFind = FBackstoryLabel(selectedBackstory);
    }
}
