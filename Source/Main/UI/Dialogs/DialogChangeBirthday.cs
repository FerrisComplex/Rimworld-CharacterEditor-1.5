// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogChangeBirthday
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogChangeBirthday : Window, IPawnable
{
    private bool doOnce;
    private readonly int iMaxLifestage;
    private int iSelctedQuadrum;
    private int iSelectedBioDay;
    private int iSelectedBioHour;
    private int iSelectedBioQuadrum;
    private int iSelectedBioYear;
    private int iSelectedDay;
    private int iSelectedHour;
    private int iSelectedLifestage;
    private int iSelectedYear;
    private Vector2 scrollPos;
    private string selectedBioDay;
    private string selectedBioHour;
    private string selectedBioQuadrum;
    private string selectedBioYear;
    private string selectedDay;
    private string selectedHour;
    private string selectedLifestage;
    private string selectedQuadrum;
    private string selectedYear;
    private readonly long startBioTick;
    private readonly long startTick;
    private readonly long ticksPerDay = 60000;
    private readonly long ticksPerHour = 2500;
    private readonly long ticksPerQuadrum = 900000;
    private readonly long ticksPerYear = 3600000;

    internal DialogChangeBirthday(Pawn p)
    {
        scrollPos = new Vector2();
        doOnce = true;
        SearchTool.Update(SearchTool.SIndex.ChangeBirthday);
        SelectedPawn = p;
        if (!p.RaceProps.lifeStageAges.NullOrEmpty())
        {
            iSelectedLifestage = p.ageTracker.CurLifeStageIndex;
            iMaxLifestage = p.RaceProps.lifeStageAges.Count - 1;
        }

        startTick = -19800059000L;
        var num1 = startTick - SelectedPawn.ageTracker.BirthAbsTicks;
        iSelectedYear = SelectedPawn.ageTracker.BirthYear;
        iSelctedQuadrum = (int)SelectedPawn.ageTracker.BirthQuadrum;
        iSelectedDay = (SelectedPawn.ageTracker.BirthDayOfYear + 1) % 15;
        if (iSelectedDay == 0)
            iSelectedDay = 15;
        var num2 = num1 % ticksPerYear % ticksPerQuadrum % ticksPerDay;
        iSelectedHour = Math.Abs((int)(num2 / ticksPerHour));
        var num3 = num2 % ticksPerHour;
        startBioTick = 0L;
        var num4 = startBioTick - SelectedPawn.ageTracker.AgeBiologicalTicks;
        iSelectedBioYear = SelectedPawn.ageTracker.AgeBiologicalYears;
        var num5 = num4 % ticksPerYear;
        iSelectedBioQuadrum = (int)Math.Abs(num5 / ticksPerQuadrum);
        var num6 = num5 % ticksPerQuadrum;
        iSelectedBioDay = (int)Math.Abs(num6 / ticksPerDay);
        var num7 = num6 % ticksPerDay;
        iSelectedBioHour = (int)Math.Abs(num7 / ticksPerHour);
        var num8 = num7 % ticksPerHour;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnCancel = true;
        closeOnClickedOutside = true;
        draggable = true;
        layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    public Pawn SelectedPawn { get; set; }

    public Pawn SelectedPawn2 { get; set; }

    public Pawn SelectedPawn3 { get; set; }

    public Pawn SelectedPawn4 { get; set; }

    public override void DoWindowContents(Rect inRect)
		{
			int num = 0;
			int num2 = 0;
			int num3 = (int)this.InitialSize.x - 16;
			int num4 = (int)this.InitialSize.y - 16 - 60;
			bool flag = this.doOnce;
			if (flag)
			{
				SearchTool.SetPosition(SearchTool.SIndex.ChangeBirthday, ref this.windowRect, ref this.doOnce, 370);
			}
			Rect outRect = new Rect((float)num, (float)num2, (float)num3, (float)num4);
			Rect rect = new Rect(0f, 0f, outRect.width - 16f, 400f);
			Widgets.BeginScrollView(outRect, ref this.scrollPos, rect, true);
			Rect rect2 = rect.ContractedBy(4f);
			rect2.y -= 4f;
			rect2.height = 600f;
			Listing_X listing_X = new Listing_X();
			listing_X.Begin(rect2);
			listing_X.verticalSpacing = 30f;
			Text.Font = GameFont.Medium;
			listing_X.Label(Label.BIRTHDAY, 200f, 0f, -1f, null);
			listing_X.GapLine(12f);
			listing_X.AddIntSection(Label.YEAR, "", ref this.selectedYear, ref this.iSelectedYear, -9999, 5500, true, "", false);
			listing_X.AddIntSection(Label.QUADRUM, "quadrum", ref this.selectedQuadrum, ref this.iSelctedQuadrum, 0, 3, true, "", false);
			listing_X.AddIntSection(Label.DAY, "", ref this.selectedDay, ref this.iSelectedDay, 1, 15, true, "", false);
			listing_X.AddIntSection(Label.HOUR, "", ref this.selectedHour, ref this.iSelectedHour, 0, 23, true, "", false);
			listing_X.Label(Label.CHRONOAGE + " [" + this.ChronologicalAge().ToString() + "]", -1f, 0f, -1f, null);
			listing_X.Gap(20f);
			listing_X.Label(Label.BIOAGE + " [" + this.iSelectedBioYear.ToString() + "]", -1f, 0f, -1f, null);
			listing_X.GapLine(12f);
			listing_X.AddIntSection(Label.YEARS, "", ref this.selectedBioYear, ref this.iSelectedBioYear, 0, (int)this.SelectedPawn.RaceProps.lifeExpectancy + 30, true, "", false);
			listing_X.AddIntSection(Label.QUARTALS, "", ref this.selectedBioQuadrum, ref this.iSelectedBioQuadrum, 0, 3, true, "", false);
			listing_X.AddIntSection(Label.DAYS, "", ref this.selectedBioDay, ref this.iSelectedBioDay, 0, 14, true, "", false);
			listing_X.AddIntSection(Label.HOURS, "", ref this.selectedBioHour, ref this.iSelectedBioHour, 0, 23, true, "", false);
			listing_X.Gap(12f);
			bool flag2 = this.iMaxLifestage > 0;
			if (flag2)
			{
				try
				{
					this.iSelectedLifestage = this.SelectedPawn.ageTracker.CurLifeStageIndex;
					listing_X.AddIntSection(Label.LIFESTAGE, "DEF" + this.SelectedPawn.RaceProps.lifeStageAges[this.iSelectedLifestage].def.LabelCap, ref this.selectedLifestage, ref this.iSelectedLifestage, 0, this.iMaxLifestage, true, this.SelectedPawn.ageTracker.CurLifeStage.defName, false);
				}
				catch
				{
				}
			}
			listing_X.End();
			Widgets.EndScrollView();
			WindowTool.SimpleAcceptButton(this, new Action(this.DoAndClose));
		}

    private void DoAndClose()
    {
        SelectedPawn.ageTracker.BirthAbsTicks = ChronoTicks();
        SelectedPawn.SetAgeTicks(BioTicks());
        if (iSelectedBioYear < 18 && SelectedPawn.HasStoryTracker())
            SelectedPawn.SetBackstory(SelectedPawn.story.Childhood, null);
        CEditor.API.UpdateGraphics();
        base.Close();
    }

    public override void Close(bool doCloseSound = true)
    {
	    SearchTool.Save(SearchTool.SIndex.ChangeBirthday, this.windowRect.position);
	    base.Close(doCloseSound);
    }

    private int ChronologicalAge()
    {
        return (int)((GenTicks.TicksAbs - ChronoTicks()) / ticksPerYear);
    }

    private long ChronoTicks()
    {
        return startTick + iSelectedYear * ticksPerYear + iSelctedQuadrum * ticksPerQuadrum + iSelectedDay * ticksPerDay + iSelectedHour * ticksPerHour;
    }

    private long BioTicks()
    {
        return startBioTick + iSelectedBioYear * ticksPerYear + iSelectedBioQuadrum * ticksPerQuadrum + iSelectedBioDay * ticksPerDay + iSelectedBioHour * ticksPerHour;
    }
}
