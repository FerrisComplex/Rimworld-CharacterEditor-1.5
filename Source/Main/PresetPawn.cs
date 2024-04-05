// Decompiled with JetBrains decompiler
// Type: CharacterEditor.PresetPawn
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;

namespace CharacterEditor;

internal class PresetPawn
{
    internal bool bPlace;
    internal SortedDictionary<Param, string> dicParams = new SortedDictionary<Param, string>();
    internal Pawn pawn;
    private bool setBodyParts;

    internal PresetPawn()
    {
        Log.Message("Creating preset pawn!");
        pawn = null;
        bPlace = false;
        setBodyParts = true;
    }

    private int ParamCount => Enum.GetNames(typeof(Param)).EnumerableCount();

    private int MinParams => 34;

    private string AsString
    {
        get
        {
            var text = "";
            foreach (var key in dicParams.Keys)
                text = text + dicParams[key] + "*";
            return text.SubstringRemoveLast();
        }
    }

    private string Save(int slot)
    {
        var asString = AsString;
        CEditor.API.SetSlot(slot, asString, true);
        return asString;
    }

    private bool Load(int slot, string custom)
    {
        if (!custom.NullOrEmpty())
            return LoadString(custom);
        custom = CEditor.API.GetSlot(slot);
        return LoadString(custom);
    }

    private bool LoadString(string custom)
    {
        dicParams = new SortedDictionary<Param, string>();
        if (!string.IsNullOrEmpty(custom))
        {
            var strArray = custom.Trim().SplitNo("*");
            if (strArray.Length >= MinParams)
                foreach (var str in strArray)
                    if (dicParams.Count < ParamCount)
                    {
                        //Log.Message("Loading " + Enum.GetName(typeof(Param), dicParams.Count) + " as " + str);
                        dicParams.Add((Param)dicParams.Count, str);
                    }
        }

        if (dicParams.Count == 0)
            MessageTool.Show("no data or not readable");
        return (uint)dicParams.Count > 0U;
    }

    internal Pawn LoadPawn(int slot, bool choosePlace, string custom = "")
    {
        pawn = null;
        bPlace = choosePlace;
        if (Load(slot, custom))
        {
            GeneratePawn();
            PawnxTool.AddOrCreateExistingPawn(this);
        }
        return pawn;
    }

    internal string SavePawn(Pawn p, int slot)
    {
        dicParams = new SortedDictionary<Param, string>();
        if (p == null)
            return "";
        dicParams.Add(Param.P00_id, p.GetPawnName());
        dicParams.Add(Param.P01_pawnkinddef, p.GetPawnKindDefname());
        dicParams.Add(Param.P02_faction, p.GetPawnFactionAsSeparatedString());
        dicParams.Add(Param.P03_name, p.GetPawnNameAsSeparatedString());
        dicParams.Add(Param.P04_ageticks, p.GetAgeTicks().ToString());
        dicParams.Add(Param.P05_chronoticks, p.GetChronoAgeTicks().ToString());
        dicParams.Add(Param.P06_gender, p.GetGenderInt());
        dicParams.Add(Param.P07_bodydefname, p.GetBodyTypeDefName());
        dicParams.Add(Param.P08_headpath, "");
        dicParams.Add(Param.P09_crowntype, "0");
        dicParams.Add(Param.P10_crown, "");
        dicParams.Add(Param.P11_headaddons, p.GetAllHeadAddonsAsSeparatedString());
        dicParams.Add(Param.P12_hairdefname, p.GetHairDefName());
        dicParams.Add(Param.P13_gradientmask, p.GetGradientMask());
        dicParams.Add(Param.P14_haircolor1, p.GetHairColor(true).ColorHexString());
        dicParams.Add(Param.P15_haircolor2, p.GetHairColor(false).ColorHexString());
        dicParams.Add(Param.P16_melanin, "-1".ToString());
        dicParams.Add(Param.P17_skincolor1, p.GetSkinColor(true).ColorHexString());
        dicParams.Add(Param.P18_skincolor2, p.GetSkinColor(false).ColorHexString());
        dicParams.Add(Param.P19_eyecolor, p.GetEyeColor().ColorHexString());
        dicParams.Add(Param.P20_skills, p.GetAllSkillsAsSeparatedString());
        dicParams.Add(Param.P21_traits, p.GetAllTraitsAsSeparatedString());
        dicParams.Add(Param.P22_hediffs, p.GetAllHediffsAsSeparatedString());
        dicParams.Add(Param.P23_backstorychild, p.GetBackstrory(true));
        dicParams.Add(Param.P24_backstoryadult, p.GetBackstrory(false));
        dicParams.Add(Param.P25_workpriorities, p.GetWorkPrioritiesAsSeparatedString());
        dicParams.Add(Param.P26_weaponlist, p.GetAllWeaponsAsSeparatedString());
        dicParams.Add(Param.P27_apparellist, p.GetAllApparelAsSeparatedString());
        dicParams.Add(Param.P28_itemlist, p.GetAllItemsAsSeparatedString());
        dicParams.Add(Param.P29_records, p.GetAllRecordsAsSeparatedString());
        dicParams.Add(Param.p30_needs, p.GetAllNeedsAsSeparatedString());
        dicParams.Add(Param.P31_memories, p.GetAllMemoriesAsSeparatedString());
        dicParams.Add(Param.P32_situationalmemories, p.GetAllSituationalMemoriesAsSeparatedString());
        dicParams.Add(Param.P33_relations, p.GetRelationsAsSeparatedString());
        dicParams.Add(Param.P34_royaltitles, p.GetRoyalTitleAsSeparatedString());
        dicParams.Add(Param.P35_beard, p.GetBeardDefName());
        dicParams.Add(Param.P36_facetattoo, p.GetFaceTattooDefName());
        dicParams.Add(Param.P37_bodytattoo, p.GetBodyTattooDefName());
        dicParams.Add(Param.P38_entropy, p.GetEntropy().ToString());
        dicParams.Add(Param.P39_psyfocus, p.GetPsyfocus().ToString());
        dicParams.Add(Param.P40_psyabilities, p.GetPsyAbilitiesAsSeparatedString());
        dicParams.Add(Param.P41_favcolor, p.GetFavColor().ColorHexString());
        dicParams.Add(Param.P42_ideoculture, p.GetPawnCultureDefName());
        dicParams.Add(Param.P43_ideoname, p.GetPawnIdeoName());
        dicParams.Add(Param.P44_modids, p.GetUsedModIds());
        dicParams.Add(Param.P45_facialanimation, p.GetFacialAnimationParams());
        dicParams.Add(Param.P46_personality, p.GetPersonality());
        dicParams.Add(Param.P47_headtypedef, p.GetHeadTypeDefName());
        dicParams.Add(Param.P48_xenotypedef, p.GetXenoTypeDefName());
        dicParams.Add(Param.P49_customxenoname, p.GetXenoCustomName());
        dicParams.Add(Param.P50_endogene, p.GetEndogeneAsSeparatedString());
        dicParams.Add(Param.P51_xenogene, p.GetXenogeneAsSeparatedString());
        dicParams.Add(Param.P52_psyche, p.GetPsyche());
        if (Prefs.DevMode)
            MessageTool.Show(PawnxTool.GetNamesOfUsedMods(dicParams.GetValue(Param.P44_modids)));
        return Save(slot);
    }

    internal void GeneratePawn(bool _setBodyParts = true, bool setApparel = true)
    {
        setBodyParts = _setBodyParts;
        var pkd = DefTool.PawnKindDef(dicParams.GetValue(Param.P01_pawnkinddef), PawnKindDefOf.Colonist);
        var raceDef = pkd == null ? null : pkd.race;
        var bySeparatedString = FactionTool.GetBySeparatedString(dicParams.GetValue(Param.P02_faction), Faction.OfPlayer);
        pawn = PawnxTool.CreateNewPawn(pkd, bySeparatedString, raceDef);
        if (pawn == null)
            return;
        pawn.ResurrectAndHeal();
        if (pawn.HasHealthTracker())
        {
            pawn.health.hediffSet.Clear();
            pawn.health.hediffSet.hediffs.Clear();
        }

        if (pawn.HasTraitTracker())
            pawn.story.traits.allTraits.Clear();
        if (pawn.HasAbilityTracker())
            pawn.abilities.abilities.Clear();
        pawn.SetNameFromSeparatedString(dicParams.GetValue(Param.P03_name));
        pawn.SetAgeTicks(dicParams.GetValue(Param.P04_ageticks).AsLong());
        pawn.SetChronoAgeTicks(dicParams.GetValue(Param.P05_chronoticks).AsLong());
        pawn.SetGenderInt(dicParams.GetValue(Param.P06_gender).AsInt32());
        if (setBodyParts)
        {
            pawn.SetBodyByDefName(dicParams.GetValue(Param.P07_bodydefname));
            if (dicParams.Count > 47)
                pawn.SetHeadTypeDef(dicParams.GetValue(Param.P47_headtypedef));
            pawn.SetHeadAddonsFromSeparatedString(dicParams.GetValue(Param.P11_headaddons));
            var hairDef = DefTool.HairDef(dicParams.GetValue(Param.P12_hairdefname));
            if (hairDef != null)
                pawn.SetHair(hairDef);
            else if (!pkd.IsAnimal() && !pkd.RaceProps.Humanlike)
                MessageTool.Show("Hair not found:" + dicParams.GetValue(Param.P12_hairdefname));
        }

        pawn.SetMelanin(dicParams.GetValue(Param.P16_melanin).AsFloat());
        if (dicParams.Count > 49)
            pawn.PresetXenoType(dicParams.GetValue(Param.P48_xenotypedef), dicParams.GetValue(Param.P49_customxenoname));
        if (dicParams.Count > 51)
            pawn.SetXenogeneFromSeparatedString(dicParams.GetValue(Param.P51_xenogene));
        if (dicParams.Count > 50)
            pawn.SetEndogeneFromSeparatedString(dicParams.GetValue(Param.P50_endogene));
        pawn.SetGradientMask(dicParams.GetValue(Param.P13_gradientmask));
        pawn.SetHairColor(true, dicParams.GetValue(Param.P14_haircolor1).HexStringToColor());
        pawn.SetHairColor(false, dicParams.GetValue(Param.P15_haircolor2).HexStringToColor());
        pawn.SetSkinColor(true, dicParams.GetValue(Param.P17_skincolor1).HexStringToColor());
        pawn.SetSkinColor(false, dicParams.GetValue(Param.P18_skincolor2).HexStringToColor());
        pawn.SetEyeColor(dicParams.GetValue(Param.P19_eyecolor).HexStringToColor());
        pawn.SetTraitsFromSeparatedString(dicParams.GetValue(Param.P21_traits));
        pawn.SetHediffsFromSeparatedString(dicParams.GetValue(Param.P22_hediffs));
        pawn.SetBackstory(BackstoryTool.GetBackstory(dicParams.GetValue(Param.P23_backstorychild)), BackstoryTool.GetBackstory(dicParams.GetValue(Param.P24_backstoryadult)));
        pawn.SetSkillsFromSeparatedString(dicParams.GetValue(Param.P20_skills));
        pawn.SetWorkPrioritiesFromSeparatedString(dicParams.GetValue(Param.P25_workpriorities));
        pawn.SetWeaponsFromSeparatedString(dicParams.GetValue(Param.P26_weaponlist));
        if (setApparel)
            pawn.SetApparelFromSeparatedString(dicParams.GetValue(Param.P27_apparellist));
        pawn.SetItemsFromSeparatedString(dicParams.GetValue(Param.P28_itemlist));
        pawn.SetRecords(dicParams.GetValue(Param.P29_records));
        pawn.SetNeeds(dicParams.GetValue(Param.p30_needs));
        pawn.SetMemories(dicParams.GetValue(Param.P31_memories));
        pawn.SetSituationalMemories(dicParams.GetValue(Param.P32_situationalmemories));
        pawn.SetRelationsFromSeparatedString(dicParams.GetValue(Param.P33_relations));
        pawn.SetGradientMask(dicParams.GetValue(Param.P13_gradientmask));
        pawn.SetHairColor(true, dicParams.GetValue(Param.P14_haircolor1).HexStringToColor());
        pawn.SetHairColor(false, dicParams.GetValue(Param.P15_haircolor2).HexStringToColor());
        pawn.SetEyeColor(dicParams.GetValue(Param.P19_eyecolor).HexStringToColor());
        if (ModsConfig.RoyaltyActive && setBodyParts)
            pawn.SetRoyalTitleFromSeparatedString(dicParams.GetValue(Param.P34_royaltitles));
        if (dicParams.Count > 35)
            pawn.SetBeard(DefTool.BeardDef(dicParams.GetValue(Param.P35_beard)));
        if (dicParams.Count > 36)
            pawn.SetFaceTattoo(DefTool.TattooDef(dicParams.GetValue(Param.P36_facetattoo)));
        if (dicParams.Count > 37)
            pawn.SetBodyTattoo(DefTool.TattooDef(dicParams.GetValue(Param.P37_bodytattoo)));
        if (pawn.HasPsyTracker())
        {
            if (ModsConfig.RoyaltyActive)
            {
                if (dicParams.Count > 38)
                    pawn.psychicEntropy.TryAddEntropy(dicParams.GetValue(Param.P38_entropy).AsFloat());
                if (dicParams.Count > 39)
                    pawn.SetPsyfocus(dicParams.GetValue(Param.P39_psyfocus).AsFloat());
            }

            if (dicParams.Count > 40)
                pawn.SetPsyAbilitiesFromSeparatedString(dicParams.GetValue(Param.P40_psyabilities));
        }

        if (dicParams.Count > 41)
            pawn.SetFavColor(dicParams.GetValue(Param.P41_favcolor).HexStringToColor());
        if (ModsConfig.IdeologyActive && dicParams.Count > 43)
            pawn.SetPawnIdeo(dicParams.GetValue(Param.P42_ideoculture), dicParams.GetValue(Param.P43_ideoname));
        if (dicParams.Count > 44)
        {
            var namesOfInactiveMods = PawnxTool.GetNamesOfInactiveMods(dicParams.GetValue(Param.P44_modids));
            if (!namesOfInactiveMods.NullOrEmpty())
            {
                MessageTool.Show("Missing Mods: " + namesOfInactiveMods);
                if (!CEditor.InStartingScreen)
                    SoundDefOf.Quest_Failed.PlayOneShot(new TargetInfo(UI.MouseCell(), Find.CurrentMap));
            }
        }

        if (dicParams.Count > 45)
            pawn.SetFacialAnimationParams(dicParams.GetValue(Param.P45_facialanimation));
        if (dicParams.Count > 46)
            pawn.SetPersonality(dicParams.GetValue(Param.P46_personality));
        if (dicParams.Count > 52)
            pawn.SetPsyche(dicParams.GetValue(Param.P52_psyche));
        if (setBodyParts)
            pawn.SetBodyByDefName(dicParams.GetValue(Param.P07_bodydefname));
        try
        {
            var bleedRateTotal = pawn.health.hediffSet.BleedRateTotal;
        }
        catch
        {
            pawn.health.hediffSet.Clear();
        }
    }

    internal void PostProcess()
    {
        var capturer = CEditor.API.Get<Capturer>(EType.Capturer);
        capturer.UpdatePawnGraphic(pawn);
        pawn.SetNameFromSeparatedString(dicParams.GetValue(Param.P03_name));
        if (dicParams.Count > 45)
            pawn.SetFacialAnimationParams(dicParams.GetValue(Param.P45_facialanimation));
        capturer.UpdatePawnGraphic(pawn);
    }

    internal static void ClearAllSlots()
    {
        for (var i = CEditor.API.NumSlots - 1; i > 0; --i)
            CEditor.API.SetSlot(i, "", i == 0);
    }

    internal static void ClearAllCustoms()
    {
        CEditor.API.SetCustom(OptionS.CUSTOMGENE, "", "");
        CEditor.API.SetCustom(OptionS.CUSTOMOBJECT, "", "");
        CEditor.API.SetCustom(OptionS.CUSTOMLIFESTAGE, "", "");
    }

    internal static void RescueOldPawns()
    {
    }

    internal enum Param
    {
        P00_id,
        P01_pawnkinddef,
        P02_faction,
        P03_name,
        P04_ageticks,
        P05_chronoticks,
        P06_gender,
        P07_bodydefname,
        P08_headpath,
        P09_crowntype,
        P10_crown,
        P11_headaddons,
        P12_hairdefname,
        P13_gradientmask,
        P14_haircolor1,
        P15_haircolor2,
        P16_melanin,
        P17_skincolor1,
        P18_skincolor2,
        P19_eyecolor,
        P20_skills,
        P21_traits,
        P22_hediffs,
        P23_backstorychild,
        P24_backstoryadult,
        P25_workpriorities,
        P26_weaponlist,
        P27_apparellist,
        P28_itemlist,
        P29_records,
        p30_needs,
        P31_memories,
        P32_situationalmemories,
        P33_relations,
        P34_royaltitles,
        P35_beard,
        P36_facetattoo,
        P37_bodytattoo,
        P38_entropy,
        P39_psyfocus,
        P40_psyabilities,
        P41_favcolor,
        P42_ideoculture,
        P43_ideoname,
        P44_modids,
        P45_facialanimation,
        P46_personality,
        P47_headtypedef,
        P48_xenotypedef,
        P49_customxenoname,
        P50_endogene,
        P51_xenogene,
        P52_psyche
    }
}
