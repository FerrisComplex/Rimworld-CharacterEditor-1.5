// Decompiled with JetBrains decompiler
// Type: CharacterEditor.FLabel
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class FLabel
{
    internal static Func<ScenPart, string> ScenPartTooltip => p =>
    {
        var selectedScenarioPart = p.GetSelectedScenarioPart();
        return p.IsScenarioAnimal() ? selectedScenarioPart.pkd != null ? selectedScenarioPart.pkd.STooltip() : p.STooltip() : selectedScenarioPart.thingDef != null ? selectedScenarioPart.thingDef.STooltip() : "";
    };

    internal static Func<ScenPart, string> ScenPartLabel => p =>
    {
        var selectedScenarioPart = p.GetSelectedScenarioPart();
        return p.IsScenarioAnimal() ? selectedScenarioPart.pkd != null ? PawnKindWithGenderAndAge(selectedScenarioPart) : p.Label + ": " + selectedScenarioPart.stackVal : selectedScenarioPart.thingDef != null ? ThingLabel(selectedScenarioPart) : "";
    };

    internal static Func<float, string> BodySizeFactor => x => Label.BODYSIZEFACTOR + " " + x;

    internal static Func<float?, string> BodyWidth => x => Label.BODYWIDTH + " " + x;

    internal static Func<float?, string> HeadSizeFactor => x => Label.HEADSIZEFACTOR + " " + x;

    internal static Func<float?, string> EyeSizeFactor => x => Label.EYESIZEFACTOR + " " + x;

    internal static Func<float, string> VoicePitch => x => Label.VOICEPITCH + " " + x;

    internal static Func<float, string> VoiceVolume => x => Label.VOICEVOLUME + " " + x;

    internal static Func<float, string> HealthScaleFactor => x => Label.HEALTHSCALEFACTOR + " " + x;

    internal static Func<float, string> HungerRateFactor => x => Label.HUNGERRATEFACTOR + " " + x;

    internal static Func<float, string> MarketValueFactor => x => Label.MARKETFACTOR + " " + x;

    internal static Func<float, string> FoodMaxFactor => x => Label.FOODMAXFACTOR + " " + x;

    internal static Func<float, string> MeleeDamageFactor => x => Label.MELEEDAMAGEFACTOR + " " + x;

    internal static Func<Selected, string> PawnKindWithGenderAndAge => s => PawnKindDefLabel(s.pkd) + " x" + s.stackVal + " " + GenderLabel(s.gender) + "[" + s.age + "]";

    internal static Func<Selected, string> ThingLabel => s => GenLabel.ThingLabel(s.thingDef, s.stuff, s.stackVal).CapitalizeFirst() + (s.HasQuality ? " (" + ((QualityCategory)(byte)s.quality).GetLabel() + ")" : "");

    internal static Func<int, string> GenderLabelInt => x => GenderLabel((Gender)(byte)x);

    internal static Func<int, string> BiologicalAge => x => Label.BIOAGE + ": " + x;

    internal static Func<Gender, string> GenderLabel => x =>
    {
        if (x == Gender.Female)
            return "Female".Translate().ToString();
        return x != Gender.Male ? Label.RANDOMCHOSEN : "Male".Translate().ToString();
    };

    internal static Func<PawnKindDef, string> PawnKindDefLabel => x => x.race == null ? "" : x.race.label;

    internal static Func<WeaponType, string> WeaponType => type => WeaponTool.GetNameForWeaponType(type);

    internal static Func<int, string> Menge => x => Label.COUNT + x;

    internal static Func<int, string> HitPoints => x => new TaggedString(nameof(HitPoints).Translate(x));

    internal static Func<int, string> DamageAmountBase => x => Label.DAMAGEAMOUNTBASE + GetFormattedValue("", x);

    internal static Func<int, string> CostStuffCount => x => Label.COSTSTUFFCOUNT + " " + x;

    internal static Func<int, string> StackLimit => x => Label.O_STACKLIMIT + " " + x;

    internal static Func<GeneticBodyType?, string> GeneticBodytype => x => x.HasValue ? Enum.GetName(typeof(GeneticBodyType), x) : "";

    internal static Func<EndogeneCategory, string> EndogeneCat => x => Enum.GetName(typeof(EndogeneCategory), x);

    internal static Func<Tradeability, string> Tradeability => x => Enum.GetName(typeof(Tradeability), x);

    internal static Func<TechLevel, string> TechLevel => x => Enum.GetName(typeof(TechLevel), x);

    internal static Func<GasType?, string> GasType => x => x.HasValue ? Enum.GetName(typeof(GasType), x) : "";

    internal static Func<SkillDef, string> PassionModAdd => x => x != null ? nameof(PassionModAdd).Translate( x).ToString() : Label.NONE;

    internal static Func<SkillDef, string> PassionModDrop => x => x != null ? nameof(PassionModDrop).Translate( x).ToString() : Label.NONE;

    internal static Func<SoundDef, string> Sound => x => x.SDefname();

    internal static Func<float, string> BeamWidth => x => Label.BEAMSTREUUNG + GetFormattedValue("cells", x);

    internal static Func<float, string> BeamFullWidthRange => x => Label.BEAMFULLWIDTHRANGE + GetFormattedValue("cells", x);

    internal static Func<float, string> CooldownTime => x => new TaggedString(nameof(CooldownTime).Translate() + GetFormattedValue("s", x));

    internal static Func<float, string> AccuracyLong => x => new TaggedString("Base".Translate() + " " + StatDefOf.AccuracyLong.label + GetFormattedValue("%", x));

    internal static Func<float, string> AccuracyMedium => x => new TaggedString("Base".Translate() + " " + StatDefOf.AccuracyMedium.label + GetFormattedValue("%", x));

    internal static Func<float, string> AccuracyShort => x => new TaggedString("Base".Translate() + " " + StatDefOf.AccuracyShort.label + GetFormattedValue("%", x));

    internal static Func<float, string> AccuracyTouch => x => new TaggedString("Base".Translate() + " " + StatDefOf.AccuracyTouch.label + GetFormattedValue("%", x));

    internal static Func<float, string> Spraying => x => Label.SPRAYING + GetFormattedValue("cells", x);

    internal static Func<float, string> SprayingMortar => x => Label.SPRAYING + Label.CLASSICMORTAR + GetFormattedValue("cells", x);

    internal static Func<float, string> ConsumeFuelPerBurst => x => Label.CONSUMEFUELPERBURST + GetFormattedValue("", x);

    internal static Func<float, string> ConsumeFuelPerShot => x => Label.CONSUMEFUELPERSHOT + GetFormattedValue("", x);

    internal static Func<float, string> BulletPostExplosionSpawnChance => x => Label.POSTEXPLOSIONSPAWNCHANCE + GetFormattedValue("%", x);

    internal static Func<float, string> BulletExplosionSpawnChance => x => Label.PREEXPLOSIONSPAWNCHANCE + GetFormattedValue("%", x);

    internal static Func<int, string> BulletPostExplosionSpawnThingCount => x => Label.POSTEXPLOSIONSPAWNTHINGCOUNT + GetFormattedValue("", x);

    internal static Func<int, string> BulletExplosionSpawnThingCount => x => Label.PREEXPLOSIONSPAWNTHINGCOUNT + GetFormattedValue("", x);

    internal static Func<int, string> BulletExplosionDelay => x => Label.EXPLOSIONDELAY + GetFormattedValue("s", x);

    internal static Func<int, string> BulletNumExtraHitCels => x => Label.NUMEXTRAHITCELLS + GetFormattedValue("cells", x);

    internal static Func<float, string> BulletExplosionRadius => x => Label.EXPL_RADIUS + GetFormattedValue("cells", x);

    internal static Func<float, string> ArmorPenetrationBase => x => new TaggedString("ArmorPenetration".Translate() + GetFormattedValue("", x));

    internal static Func<int, string> Complexity => x => Label.COMPLEXITY.Colorize(GeneUtility.GCXColor) + " " + x.ToStringWithSign();

    internal static Func<int, string> Metabolism => x => Label.METABOLICEFFICIENCY.Colorize(GeneUtility.METColor) + " " + x.ToStringWithSign();

    internal static Func<int, string> ArchitesRequired => x => Label.ARCHITECAPSULES.Colorize(GeneUtility.ARCColor) + " " + x.ToStringWithSign();

    internal static Func<float, string> LovinMTBFactor => x => Label.LOVINMTBFACTOR + " " + x;

    internal static Func<float, string> MinAgeActive => x => Label.STARTSATAGE + " " + x;

    internal static Func<float, string> DisplayOrderInCat => x => Label.DISPLAYODERINCATEGORY + " " + x;

    internal static Func<float, string> SelectionWeightDark => x => Label.SELECTIONWEIGHTDARKSKIN + " " + x;

    internal static Func<float, string> SelectionWeight => x => Label.SELECTIONWEIGHT + " " + x;

    internal static Func<float, string> RandomBrightnessFactor => x => Label.RANDOMBRIGHTNESSFACTOR + " " + x;

    internal static Func<float, string> MarketValue => x => Label.MARKETVALUEFACTOR + " " + x;

    internal static Func<float, string> PrisonBreak => x => x >= 0.0 ? Label.PRISONBREAKINTERVAL + " x" + x.ToStringPercent() : "WillNeverPrisonBreak".Translate().ToString();

    internal static Func<float, string> FoodPoisonChance => x => new TaggedString(x == 1.0 ? "Stat_Hediff_FoodPoisoningChanceFactor_Name".Translate() : x <= 0.0 ? "FoodPoisoningImmune".Translate() : "Stat_Hediff_FoodPoisoningChanceFactor_Name".Translate() + " x" + x.ToStringPercent());

    internal static Func<float, string> PainOffset => x => new TaggedString("Pain".Translate() + " " + (x * 100f).ToString("+###0;-###0") + "%");

    internal static Func<float, string> PainFactor => x => new TaggedString("Pain".Translate() + " x" + x.ToStringPercent());

    internal static Func<float, string> AddictionChance => x =>
    {
        if (GeneTool.SelectedGene.chemical == null)
            return Label.ADDICTIONCHANCEFACTOR;
        return x <= 0.0 ? new TaggedString("AddictionImmune".Translate( GeneTool.SelectedGene.chemical)) : new TaggedString("AddictionChanceFactor".Translate( GeneTool.SelectedGene.chemical) + " x" + x.ToStringPercent());
    };

    internal static Func<float, string> OverdoseChance => x => GeneTool.SelectedGene.chemical == null ? Label.OVERDOSECHANCEFACTOR : new TaggedString("OverdoseChanceFactor".Translate( GeneTool.SelectedGene.chemical) + " x" + x.ToStringPercent());

    internal static Func<float, string> ToleranceFactor => x => GeneTool.SelectedGene.chemical == null ? Label.TOLERANCEBUILDUPFACTOR : new TaggedString("ToleranceBuildupFactor".Translate(GeneTool.SelectedGene.chemical) + " x"+ x.ToStringPercent());

    internal static Func<float, string> ResourceLoss => x => !GeneTool.SelectedGene.resourceLabel.NullOrEmpty() ? "ResourceLossPerDay".Translate(GeneTool.SelectedGene.resourceLabel.Named("RESOURCE"), (-Mathf.RoundToInt((float)(GeneTool.SelectedGene.resourceLossPerDay * 100.0))).ToStringWithSign().Named("OFFSET")).ToString() : Label.RESOURCELOSSPERDAY;

    internal static Func<float, string> MissingRomanceChance => x => new TaggedString(x != 1.0f ? ("MissingGeneRomanceChance".Translate(GeneTool.SelectedGene.label.Named("GENE")) + " x" + x.ToStringPercent()) : "MissingGeneRomanceChance".Translate());

    internal static Func<float, string> MentalBreakMTB => x => x != 0.0 ? Label.MENTALBREAKMTBDAYS + " " + x : Label.MENTALBREAKMTBDAYS;

    internal static Func<float, string> MentalBreakChance => x =>
    {
        if (x == 1.0)
            return Label.MENTALBREAKCHANCEFACTOR;
        return x > 0.0 ? "AggroMentalBreakSelectionChanceFactor".Translate().ToString() + " x" + x.ToStringPercent() : "NeverAggroMentalBreak".Translate().ToString();
    };

    internal static Func<float, string> SocialFightChance => x => x > 0.0 ? Label.SOCIALFIGHTCHANCEFACTOR + " x" + x.ToStringPercent() : "WillNeverSocialFight".Translate().ToString();

    internal static string DefDescription<T>(T def) where T : Def
    {
        return def.STooltip();
    }

    internal static string DefLabel<T>(T def) where T : Def
    {
        return "[" + def.SLabel() + "]";
    }

    internal static string DefLabelSimple<T>(T def) where T : Def
    {
        var str = def.SLabel();
        return str == Label.NONE ? Label.ALL : str;
    }

    internal static string DefName<T>(T def) where T : Def
    {
        return "[" + def.SDefname() + "]";
    }

    internal static string TString(string t)
    {
        return !t.NullOrEmpty() ? t : Label.ALL;
    }

    internal static string EnumNameAndAll<T>(T e)
    {
        return !(e.ToString() == "None") ? e.ToString() : Label.ALL;
    }

    internal static string GetFormattedValue(string format, float value)
    {
        if (format == "%")
            return " [" + Math.Round(100.0 * value, 0) + " %]";
        if (format == "s")
            return " [" + value + " s]";
        if (format == "ticks")
            return " [" + value + " ticks]";
        if (format == "rpm")
            return " [" + (value == 0.0 ? Label.INFINITE : Math.Round(60.0 / value * 60.0, 0).ToString()) + " rpm]";
        if (format == "cps")
            return " [" + (value == 0.0 ? Label.INFINITE : value.ToString()) + " cps]";
        if (format == "cells")
            return " [" + value + " cells]";
        if (format.StartsWith("max"))
            return " [" + value + "/" + format.SubstringFrom("max") + "]";
        if (format == "int")
            return " [" + (int)Math.Round(value) + "]";
        if (format == "quadrum")
            return " [" + Enum.GetName(typeof(Quadrum), (int)value) + "]";
        if (format == "addict")
            return " " + (100.0 - Math.Round(100.0 * value, 0)) + " %";
        if (format == "high")
            return " " + value.ToStringPercent("F0");
        if (format.StartsWith("DEF"))
            return " [" + format.SubstringFrom("DEF") + "]";
        if (format.StartsWith("dauer"))
            return " " + format.SubstringFrom("dauer");
        if (format == "pain")
        {
            var val = (int)value;
            return val == 0 ? Label.PAINLESS : ("PainCategory_" + HealthTool.ConvertSliderToPainCategory(val)).Translate().ToString();
        }

        if (!format.StartsWith("comp"))
            return " [" + value + "]";
        return !format.Contains("%") ? " " + value.ToStringPercent("F0") + " " + format.SubstringFrom("comp") : " " + format.SubstringFrom("comp");
    }
}
