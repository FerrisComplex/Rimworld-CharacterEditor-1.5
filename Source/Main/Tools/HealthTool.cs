// Decompiled with JetBrains decompiler
// Type: CharacterEditor.HealthTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class HealthTool
{
    internal const string HB_All = "All";
    internal const string HB_AllImplants = "AllImplants";
    internal const string HB_AllAddictions = "AllAddictions";
    internal const string HB_AllDiseases = "AllDiseases";
    internal const string HB_AllInjuries = "AllInjuries";
    internal const string HB_AllTime = "AllTime";
    internal const string HB_Arm = "Arm";
    internal const string HB_Brain = "Brain";
    internal const string HB_Clavicle = "Clavicle";
    internal const string HB_Ear = "Ear";
    internal const string HB_Eye = "Eye";
    internal const string HB_Femur = "Femur";
    internal const string HB_Finger = "Finger";
    internal const string HB_Foot = "Foot";
    internal const string HB_Hand = "Hand";
    internal const string HB_Head = "Head";
    internal const string HB_Heart = "Heart";
    internal const string HB_Humerus = "Humerus";
    internal const string HB_Jaw = "Jaw";
    internal const string HB_Kidney = "Kidney";
    internal const string HB_Leg = "Leg";
    internal const string HB_Liver = "Liver";
    internal const string HB_Lung = "Lung";
    internal const string HB_Neck = "Neck";
    internal const string HB_Nose = "Nose";
    internal const string HB_Pelvis = "Pelvis";
    internal const string HB_Radius = "Radius";
    internal const string HB_Shoulder = "Shoulder";
    internal const string HB_Skull = "Skull";
    internal const string HB_Spine = "Spine";
    internal const string HB_Sternum = "Sternum";
    internal const string HB_Stomach = "Stomach";
    internal const string HB_Tibia = "Tibia";
    internal const string HB_Toe = "Toe";
    internal const string HB_Torso = "Torso";
    internal const string HB_UtilitySlot = "UtilitySlot";
    internal const string HB_Utility = "Utility";
    internal const string HB_WholeBody = "WholeBody";
    internal const string H_Abasia = "Abasia";
    internal const string H_AestheticShaper = "AestheticShaper";
    internal const string H_AlcoholTolerance = "AlcoholTolerance";
    internal const string H_Alzheimers = "Alzheimers";
    internal const string H_Anesthetic = "Anesthetic";
    internal const string H_Asthma = "Asthma";
    internal const string H_BadBack = "BadBack";
    internal const string H_Blindness = "Blindness";
    internal const string H_BloodLoss = "BloodLoss";
    internal const string H_Carcinoma = "Carcinoma";
    internal const string H_Cataract = "Cataract";
    internal const string H_CatatonicBreakdown = "CatatonicBreakdown";
    internal const string H_ChemicalDamageModerate = "ChemicalDamageModerate";
    internal const string H_ChemicalDamageSevere = "ChemicalDamageSevere";
    internal const string H_Circadian = "Circadian";
    internal const string H_Cirrhosis = "Cirrhosis";
    internal const string H_Coagulator = "Coagulator";
    internal const string H_Cochlear = "Cochlear";
    internal const string H_CryptosleepSickness = "CryptosleepSickness";
    internal const string H_Dementia = "Dementia";
    internal const string H_Denture = "Denture";
    internal const string H_DrugOverdose = "DrugOverdose";
    internal const string H_ElbowBlade = "ElbowBlade";
    internal const string H_Fangs = "Fangs";
    internal const string H_FibrousMechanites = "FibrousMechanites";
    internal const string H_FieldHand = "FieldHand";
    internal const string H_Flu = "Flu";
    internal const string H_FoodPoisoning = "FoodPoisoning";
    internal const string H_Force = "_Force";
    internal const string H_Frail = "Frail";
    internal const string H_GastroAnalyzer = "GastroAnalyzer";
    internal const string H_GutWorms = "GutWorms";
    internal const string H_HandTalon = "HandTalon";
    internal const string H_Hangover = "Hangover";
    internal const string H_TortureCrown = "TortureCrown";
    internal const string H_BlindFold = "Blindfold";
    internal const string H_HealingEnhancer = "HealingEnhancer";
    internal const string H_Hear = "Hearing";
    internal const string H_HearingLoss = "HearingLoss";
    internal const string H_High = "High";
    internal const string H_HungerMaker = "HungerMaker";
    internal const string H_Immunoenhancer = "Immunoenhancer";
    internal const string H_Joyfuzz = "Joyfuzz";
    internal const string H_Joywire = "Joywire";
    internal const string H_KneeSpike = "KneeSpike";
    internal const string H_LearningAssistant = "LearningAssistant";
    internal const string H_LoveEnhancer = "LoveEnhancer";
    internal const string H_Malaria = "Malaria";
    internal const string H_Malnutrition = "Malnutrition";
    internal const string H_Mindscrew = "Mindscrew";
    internal const string H_Command = "Command";
    internal const string H_MuscleParasites = "MuscleParasites";
    internal const string H_Neurocalcularor = "Neurocalculator";
    internal const string H_NoPain = "NoPain";
    internal const string H_Painstopper = "Painstopper";
    internal const string H_Plague = "Plague";
    internal const string H_PowerClaw = "PowerClaw";
    internal const string H_Pregnant = "Pregnant";
    internal const string H_PregnantHuman = "PregnantHuman";
    internal const string H_Psychic = "Psychic";
    internal const string H_PsychicShock = "PsychicShock";
    internal const string H_PsychicEntropy = "PsychicEntropy";
    internal const string H_NeuralHealRecoveryGain = "NeuralHealRecoveryGain";
    internal const string H_WorkDrive = "WorkDrive";
    internal const string H_PreachHealth = "PreachHealth";
    internal const string H_BerserkTrance = "BerserkTrance";
    internal const string H_GlucosoidRush = "GlucosoidRush";
    internal const string H_ImmunityDrive = "ImmunityDrive";
    internal const string H_WorkFocus = "WorkFocus";
    internal const string H_NeuralSupercharge = "NeuralSupercharge";
    internal const string H_ResurrectionPsychosis = "ResurrectionPsychosis";
    internal const string H_ResurrectionSickness = "ResurrectionSickness";
    internal const string H_SensoryMechanites = "SensoryMechanites";
    internal const string H_Skingland = "skinGland";
    internal const string H_SleepingSickness = "SleepingSickness";
    internal const string H_Smelling = "Smelling";
    internal const string H_SpeedBoost = "SpeedBoost";
    internal const string H_tolerance = "tolerance";
    internal const string H_ToxicBuildup = "ToxicBuildup";
    internal const string H_TraumaSavant = "TraumaSavant";
    internal const string H_VenomTalon = "VenomTalon";
    internal const string H_WoodenFoot = "WoodenFoot";
    internal const string H_WoundInfection = "WoundInfection";
    internal static bool bIsOverridden;

    internal static Dictionary<string, Func<HediffDef, bool>> AllBodyPartChecks
    {
        get
        {
            var dictionary = new Dictionary<string, Func<HediffDef, bool>>();
            dictionary.Add("All", IsForAllParts);
            dictionary.Add("AllImplants", IsForAllParts);
            dictionary.Add("AllAddictions", IsForAllParts);
            dictionary.Add("AllDiseases", IsForAllParts);
            dictionary.Add("AllInjuries", IsForAllParts);
            dictionary.Add("AllTime", IsForAllParts);
            dictionary.Add("Arm", IsForArm);
            dictionary.Add("Brain", IsForBrain);
            dictionary.Add("Clavicle", IsForClavicle);
            dictionary.Add("Ear", IsForEar);
            dictionary.Add("Eye", IsForEye);
            dictionary.Add("Femur", IsForFemur);
            dictionary.Add("Finger", IsForFinger);
            dictionary.Add("Foot", IsForFoot);
            dictionary.Add("Hand", IsForHand);
            dictionary.Add("Head", IsForHead);
            dictionary.Add("Heart", IsForHeart);
            dictionary.Add("Humerus", IsForHumerus);
            dictionary.Add("Jaw", IsForJaw);
            dictionary.Add("Kidney", IsForKidney);
            dictionary.Add("Leg", IsForLeg);
            dictionary.Add("Liver", IsForLiver);
            dictionary.Add("Lung", IsForLung);
            dictionary.Add("Nose", IsForNose);
            dictionary.Add("Pelvis", IsForPelvis);
            dictionary.Add("Radius", IsForRadius);
            dictionary.Add("Shoulder", IsForShoulder);
            dictionary.Add("Skull", IsForSkull);
            dictionary.Add("Spine", IsForSpine);
            dictionary.Add("Sternum", IsForSternum);
            dictionary.Add("Stomach", IsForStomach);
            dictionary.Add("Tibia", IsForTibia);
            dictionary.Add("Toe", IsForToe);
            dictionary.Add("Torso", IsForTorso);
            dictionary.Add("UtilitySlot", IsForUtilitySlot);
            dictionary.Add("WholeBody", IsForWholeBody);
            return dictionary;
        }
    }

    internal static string GetHediffAsSeparatedString(this Hediff h)
    {
        if (h == null || h.def.IsNullOrEmpty())
            return "";
        var output = h.def.defName + "|" + h.Severity + "|" + (h.Part != null && h.Part.def != null && !string.IsNullOrEmpty(h.Part.def.defName) ? h.Part.def.defName : "") + "|";
        if (h.Part == null) 
            output += "|";
        else
            output += h.Part.Index + "|";
        
        output += (h.IsPermanent() ? "1" : "0") + "|";
        output += h.GetLevel() + "|";
        output += h.GetPainValue() + "|";
        output += h.GetDuration() + "|";
        output += h.GetOtherPawn().GetPawnNameAsSeparatedString();
        return output;
    }

    internal static string GetAllHediffsAsSeparatedString(this Pawn p)
    {
        if (p == null || !p.HasHealthTracker() || p.health.hediffSet.hediffs.NullOrEmpty())
            return "";
        var text = "";
        foreach (var hediff in p.health.hediffSet.hediffs)
        {
            if (hediff == null) continue;
            var v = hediff.GetHediffAsSeparatedString();
            if (string.IsNullOrEmpty(v)) continue;
            text += v + ":";
        }

     
        return text.SubstringRemoveLast();
    }

    internal static void SetHediffsFromSeparatedString(this Pawn p, string s)
    {
        if (!p.HasHealthTracker() || s.NullOrEmpty())
            return;
        var str1 = s;
        var separator1 = new string[1] { ":" };
        foreach (var str2 in str1.Split(separator1, StringSplitOptions.None))
        {
            var separator2 = new string[1] { "|" };
            var strArray = str2.Split(separator2, StringSplitOptions.None);
            if (strArray.Length >= 5)
            {
                var level = strArray.Length > 5 ? strArray[5].AsInt32() : -1;
                var pain = strArray.Length > 6 ? strArray[6].AsInt32() : -1;
                var duration = strArray.Length > 7 ? strArray[7].AsInt32() : -1;
                var otherPawnName = strArray.Length > 8 ? strArray[8] : "";
                p.AddHediffByName(strArray[0], strArray[1].AsFloat(), strArray[2], strArray[3].AsInt32(), strArray[4].AsBool(), level, pain, duration, otherPawnName);
            }
        }
    }

    internal static string GetFullLabel(this Hediff h)
    {
        if (h == null)
            return "";
        try
        {
            var str = h.Label ?? "";
            return ((h.Part == null ? "" : h.Part.Label == null ? "" : h.Part.Label + " ") ?? "") + str;
        }
        catch
        {
            return h.def.label;
        }
    }

    internal static bool IsAdjustableSeverity(HediffDef hediff)
    {
        var flag = false;
        if (hediff != null)
        {
            flag = (hediff.injuryProps != null && hediff.hediffClass != typeof(Hediff_MissingPart)) || hediff.IsAddiction || hediff.maxSeverity > 1.0 || (hediff.addedPartProps == null && !(hediff.hediffClass == typeof(Hediff_MissingPart)) && !hediff.countsAsAddedPartOrImplant);
            if (hediff.hediffClass == typeof(Hediff_Psylink))
                flag = false;
        }

        return flag;
    }

    internal static float GetMaxSeverity(HediffDef hediff)
    {
        var num = 0.0f;
        if (hediff == null)
            return 0.0f;
        try
        {
            num = hediff.lethalSeverity >= 0.0 ? hediff.lethalSeverity : hediff.maxSeverity;
            if (num > 99.0)
                num = hediff.stages.CountAllowNull() > 0 ? hediff.IsHediffWithEffect() || hediff.IsHediffPsylink() ? 99f : 1f : 99f;
        }
        catch
        {
        }

        return num;
    }

    internal static bool NeedBodyPart(HediffDef hediff)
    {
        return hediff != null && (!hediff.defName.Contains("_Force") || !(hediff.hediffClass == typeof(HediffWithComps))) && (hediff.hediffClass == typeof(Hediff_AddedPart) || hediff.hediffClass == typeof(Hediff_Injury) || hediff.hediffClass == typeof(HediffWithComps) || hediff.hediffClass == typeof(Hediff_MissingPart) || hediff.hediffClass == typeof(Hediff_Implant));
    }

    internal static BodyPartRecord GetBodyPart(Pawn pawn, Hediff h)
    {
        if (pawn != null && h != null && h.Part != null && h.Part.def != null && pawn.RaceProps.body.AllParts != null)
            try
            {
                foreach (var allPart in pawn.RaceProps.body.AllParts)
                    if (allPart.def.defName == h.Part.def.defName && allPart.Index == h.Part.Index)
                        return allPart;
            }
            catch (Exception ex)
            {
                Log.Error("GetBodyPart: " + ex.Message);
            }

        return null;
    }

    internal static BodyPartRecord GetBodyPartByDefName(
        Pawn pawn,
        string defName,
        int index)
    {
        if (pawn != null && !string.IsNullOrEmpty(defName))
            foreach (var allPart in pawn.RaceProps.body.AllParts)
                if (allPart.def.defName == defName && allPart.Index == index)
                    return allPart;
        return null;
    }

    internal static bool IsFor(HediffDef h, string bodyparttype)
    {
        return AllBodyPartChecks[bodyparttype](h);
    }

    internal static bool IsForAllParts(this HediffDef h)
    {
        return h.hediffClass == typeof(Hediff_MissingPart) || h.defName == "ChemicalDamageModerate" || h.defName == "ChemicalDamageSevere" || h.defName == "MuscleParasites" || h.defName == "FibrousMechanites" || h.defName == "SensoryMechanites" || h.defName == "Carcinoma" || h.defName == "WoundInfection";
    }

    internal static bool IsForArm(this HediffDef h)
    {
        return h.defName.Contains("Arm") || h.defName == "PowerClaw" || h.defName == "ElbowBlade";
    }

    internal static bool IsForBrain(this HediffDef h)
    {
        return h.defName.Contains("Brain") || (h.defName.StartsWith("Psychic") && h.defName != "PsychicEntropy") || (h.HasComp(typeof(HediffComp_Disappears)) && h.defName.Contains("_Psychic")) || h.defName.StartsWith("Circadian") || h.defName == "Dementia" || h.defName == "Alzheimers" || h.defName == "ResurrectionPsychosis" || h.defName == "TraumaSavant" || h.defName == "Joywire" || h.defName == "Painstopper" || h.defName == "Neurocalculator" || h.defName == "LearningAssistant" || h.defName == "Mindscrew" || h.defName == "Joyfuzz" || h.defName == "Abasia" || h.defName == "NoPain" || h.defName == "HungerMaker" || h.defName == "SpeedBoost" || h.defName.EndsWith("Command") || h.IsHediffPsylink() || h.HasComp(typeof(HediffComp_EntropyLink)) || h.HasComp(typeof(HediffComp_Link));
    }

    internal static bool IsForClavicle(this HediffDef h)
    {
        return h.defName.Contains("Clavicle");
    }

    internal static bool IsForEar(this HediffDef h)
    {
        return h.defName.Contains("Ear") || h.defName.Contains("Hearing") || h.defName.Contains("Cochlear") || h.defName == "HearingLoss";
    }

    internal static bool IsForEye(this HediffDef h)
    {
        return h.defName.Contains("Eye") || h.defName == "Cataract" || h.defName == "Blindness";
    }

    internal static bool IsForFemur(this HediffDef h)
    {
        return h.defName.Contains("Femur");
    }

    internal static bool IsForFinger(this HediffDef h)
    {
        return h.defName.Contains("Finger") || h.defName == "VenomTalon";
    }

    internal static bool IsForFoot(this HediffDef h)
    {
        return h.defName.Contains("Foot");
    }

    internal static bool IsForHand(this HediffDef h)
    {
        return h.defName.Contains("Hand");
    }

    internal static bool IsForHead(this HediffDef h)
    {
        return h.defName.Contains("Head") || h.defName == "Hangover" || h.defName == "TortureCrown" || h.defName == "Blindfold";
    }

    internal static bool IsForHeart(this HediffDef h)
    {
        return h.defName.Contains("Heart");
    }

    internal static bool IsForHumerus(this HediffDef h)
    {
        return h.defName.Contains("Humerus");
    }

    internal static bool IsForJaw(this HediffDef h)
    {
        return h.defName.Contains("Jaw") || h.defName.StartsWith("Denture") || h.defName.Contains("Fangs");
    }

    internal static bool IsForKidney(this HediffDef h)
    {
        return h.defName.Contains("Kidney") || h.defName == "Immunoenhancer";
    }

    internal static bool IsForLeg(this HediffDef h)
    {
        return h.defName.Contains("Leg") || h.defName == "KneeSpike";
    }

    internal static bool IsForLiver(this HediffDef h)
    {
        return h.defName.Contains("Liver") || h.defName == "Cirrhosis" || h.defName == "AlcoholTolerance";
    }

    internal static bool IsForLung(this HediffDef h)
    {
        return h.defName.Contains("Lung") || h.defName == "Asthma";
    }

    internal static bool IsForNose(this HediffDef h)
    {
        return h.defName.Contains("Nose") || h.defName.Contains("Smelling") || h.defName == "GastroAnalyzer";
    }

    internal static bool IsForPelvis(this HediffDef h)
    {
        return h.defName.Contains("Pelvis");
    }

    internal static bool IsForRadius(this HediffDef h)
    {
        return h.defName.Contains("Radius");
    }

    internal static bool IsForShoulder(this HediffDef h)
    {
        return h.defName.Contains("Shoulder");
    }

    internal static bool IsForSkull(this HediffDef h)
    {
        return h.defName.Contains("Skull");
    }

    internal static bool IsForSpine(this HediffDef h)
    {
        return h.defName.Contains("Spine") || h.defName == "BadBack";
    }

    internal static bool IsForSternum(this HediffDef h)
    {
        return h.defName.Contains("Sternum");
    }

    internal static bool IsForStomach(this HediffDef h)
    {
        return h.defName.Contains("Stomach") || h.defName == "GutWorms";
    }

    internal static bool IsForTibia(this HediffDef h)
    {
        return h.defName.Contains("Tibia");
    }

    internal static bool IsForToe(this HediffDef h)
    {
        return h.defName.Contains("Toe");
    }

    internal static bool IsForTorso(this HediffDef h)
    {
        return h.defName.Contains("Torso") || h.defName.EndsWith("skinGland") || h.defName == "Coagulator" || h.defName == "HealingEnhancer" || h.defName == "AestheticShaper" || h.defName == "LoveEnhancer";
    }

    internal static bool IsForUtilitySlot(this HediffDef h)
    {
        return h.defName.Contains("Utility");
    }

    internal static bool IsForWholeBody(this HediffDef h)
    {
        return h.IsAddiction || h.defName.ToLower().Contains("tolerance") || h.defName.EndsWith("High") || h.defName == "PsychicEntropy" || h.defName == "PsychicShock" || h.defName == "NeuralHealRecoveryGain" || h.defName == "NeuralSupercharge" || h.defName == "WorkDrive" || h.defName == "ImmunityDrive" || h.defName == "WorkFocus" || h.defName == "PreachHealth" || h.defName == "BerserkTrance" || h.defName == "GlucosoidRush" || h.defName == "CatatonicBreakdown" || h.defName == "BloodLoss" || h.defName.EndsWith("Flu") || h.defName.EndsWith("Plague") || h.defName == "Malaria" || h.defName == "SleepingSickness" || h.defName == "Anesthetic" || h.defName == "Frail" || h.defName == "CryptosleepSickness" || h.defName == "FoodPoisoning" || h.defName == "ToxicBuildup" || h.defName == "Pregnant" || h.defName == "PregnantHuman" || h.defName == "DrugOverdose" || h.defName == "ResurrectionSickness" || h.defName == "Malnutrition";
    }

    internal static void AddHediffByName(
        this Pawn p,
        string defName,
        float severity,
        string bodyPartDefName,
        int bodyPartIndex,
        bool permanent,
        int level,
        int pain,
        int duration,
        string otherPawnName)
    {
        var hediff = DefTool.HediffDef(defName);
        var fromSeparatedString = PawnxTool.GetOtherPawnFromSeparatedString(otherPawnName);
        var bodyPartByDefName = GetBodyPartByDefName(p, bodyPartDefName, bodyPartIndex);
        p.AddHediff2(false, hediff, severity, bodyPartByDefName, permanent, level, pain, duration, fromSeparatedString);
    }

    internal static void AddHediff2(this Pawn pawn, bool random, HediffDef hediff = null, float severity = 1f, BodyPartRecord bpr = null, bool isPermanent = false, int level = -1, int pain = -1, int duration = -1, Pawn otherPawn = null)
    {
        bool flag = pawn == null;
        if (!flag)
        {
            if (random)
            {
                bool flag2 = hediff == null;
                if (flag2)
                {
                    hediff = (from td in DefDatabase<HediffDef>.AllDefs
                        where td.defName != null
                        select td).ToList<HediffDef>().RandomElement<HediffDef>();
                }

                bool flag3 = HealthTool.IsAdjustableSeverity(hediff);
                bool flag4 = flag3;
                if (flag4)
                {
                    float maxSeverity = HealthTool.GetMaxSeverity(hediff);
                    int maxValue = (int)(maxSeverity * 100f);
                    int minValue = (int)(hediff.minSeverity * 100f);
                    severity = (float)CEditor.zufallswert.Next(minValue, maxValue) / 100f;
                    bool flag5 = severity > maxSeverity;
                    if (flag5)
                    {
                        severity = maxSeverity;
                    }
                }

                bool flag6 = hediff.injuryProps != null;
                if (flag6)
                {
                    isPermanent = (CEditor.zufallswert.Next(0, 5) == 0);
                }

                bool flag7 = hediff.IsHediffWithLevel();
                if (flag7)
                {
                    level = CEditor.zufallswert.Next((int)hediff.minSeverity, (int)hediff.maxSeverity);
                }

                bool flag8 = hediff.IsHediffWithComps();
                if (flag8)
                {
                    pain = (int)HealthTool.ConvertSliderToPainCategory(CEditor.zufallswert.Next(0, 3));
                    duration = CEditor.zufallswert.Next(0, 220000);
                }

                bool flag9 = hediff.IsHediffWithOtherPawn();
                if (flag9)
                {
                    otherPawn = Find.WorldPawns.AllPawnsAlive.RandomElement<Pawn>();
                }

                List<BodyPartRecord> listOfAllowedBodyPartRecords = pawn.GetListOfAllowedBodyPartRecords(hediff);
                bool flag10 = !listOfAllowedBodyPartRecords.NullOrEmpty<BodyPartRecord>();
                if (flag10)
                {
                    bpr = listOfAllowedBodyPartRecords.RandomElement<BodyPartRecord>();
                }
            }

            bool flag11 = hediff != null;
            if (flag11)
            {
                try
                {
                    bool flag12 = bpr != null || hediff.IsHediffWithParents();
                    if (flag12)
                    {
                        bool flag13 = !pawn.health.hediffSet.PartIsMissing(bpr) || hediff.IsHediffWithParents();
                        if (flag13)
                        {
                            Hediff hediff2 = HediffMaker.MakeHediff(hediff, pawn, bpr);
                            hediff2.Severity = severity;
                            hediff2.SetLevel(level);
                            hediff2.SetPermanent(isPermanent);
                            hediff2.SetPainValue(pain);
                            hediff2.SetDuration(duration);
                            hediff2.SetOtherPawn(otherPawn);
                            bool flag14 = hediff2.GetType() == typeof(Hediff_Psylink);
                            if (flag14)
                            {
                                pawn.CheckAddPsylink((int)Math.Round((double)severity));
                            }
                            else
                            {
                                pawn.health.AddHediff(hediff2, bpr, null, null);
                                pawn.health.Notify_HediffChanged(hediff2);
                                bool flag15 = hediff2.def.IsHediffPsylink();
                                if (flag15)
                                {
                                    Pawn_PsychicEntropyTracker psychicEntropy = pawn.psychicEntropy;
                                    if (psychicEntropy != null)
                                    {
                                        psychicEntropy.Notify_GainedPsylink();
                                    }

                                    Pawn_PsychicEntropyTracker psychicEntropy2 = pawn.psychicEntropy;
                                    if (psychicEntropy2 != null)
                                    {
                                        psychicEntropy2.PsychicEntropyTrackerTick();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        List<BodyPartRecord> listOfAllowedBodyPartRecords2 = pawn.GetListOfAllowedBodyPartRecords(hediff);
                        bool flag16 = hediff.IsForWholeBody() || listOfAllowedBodyPartRecords2.CountAllowNull<BodyPartRecord>() > 1;
                        if (flag16)
                        {
                            HealthUtility.AdjustSeverity(pawn, hediff, severity);
                        }

                        Hediff hediff3 = pawn.TryGetHediffByDefName(hediff.defName);
                        bool flag17 = hediff3 != null;
                        if (flag17)
                        {
                            hediff3.Severity = severity;
                            hediff3.SetLevel(level);
                            hediff3.SetPermanent(isPermanent);
                            hediff3.SetPainValue(pain);
                            hediff3.SetDuration(duration);
                            hediff3.SetOtherPawn(otherPawn);
                        }
                    }

                    pawn.health.summaryHealth.Notify_HealthChanged();
                }
                catch
                {
                }

                Pawn_NeedsTracker needs = pawn.needs;
                if (needs != null)
                {
                    needs.AddOrRemoveNeedsAsAppropriate();
                }

                CEditor.API.UpdateGraphics();
            }
        }
    }

    internal static Hediff TryGetHediffByDefName(this Pawn p, string defName)
    {
        Hediff hediff1 = null;
        if (p.HasHealthTracker())
            foreach (var hediff2 in p.health.hediffSet.hediffs)
                if (hediff2.def.defName == defName)
                {
                    hediff1 = hediff2;
                    break;
                }

        return hediff1;
    }

    internal static int GetLevel(this Hediff h)
    {
        if (h == null || !h.def.IsHediffWithLevel())
            return -1;
        return ((Hediff_Level)h).level;
    }

    internal static void SetLevel(this Hediff h, int val)
    {
        if (h == null || val < 0 || !h.def.IsHediffWithLevel())
            return;
        ((Hediff_Level)h).SetLevelTo(val);
        h.Severity = val;
    }

    internal static void SetPermanent(this Hediff h, bool permanent)
    {
        if (h == null || h.def.injuryProps == null)
            return;
        var comp = h.TryGetComp<HediffComp_GetsPermanent>();
        if (comp != null)
            comp.IsPermanent = permanent;
    }

    internal static PainCategory ConvertSliderToPainCategory(int val)
    {
        if (val <= 0)
            return 0;
        if (val == 1)
            return (PainCategory)1;
        if (val == 2)
            return (PainCategory)3;
        return PainCategory.HighPain; // 6
    }

    internal static int ConvertPainCategoryToSliderVal(PainCategory val)
    {
        if (val == PainCategory.Painless)
            return 0;
        if (val == PainCategory.LowPain)
            return 1;
        if (val == PainCategory.MediumPain)
            return 2;
        return 3; // PainCategory.HighPain (6)
    }

    internal static int GetPainValue(this Hediff h)
    {
        var num = -1;
        if (h != null && h.def.injuryProps != null)
        {
            var comp = h.TryGetComp<HediffComp_GetsPermanent>();
            if (comp != null)
                num = (int)comp.PainCategory;
        }

        return num;
    }

    internal static void SetPainValue(this Hediff h, int val)
    {
        if (h == null || val < 0 || h.def.injuryProps == null)
            return;
        h.TryGetComp<HediffComp_GetsPermanent>()?.SetPainCategory((PainCategory)val);
    }

    internal static int GetDuration(this Hediff h)
    {
        var num = -1;
        if (h != null)
        {
            var comp = h.TryGetComp<HediffComp_Disappears>();
            if (comp != null)
                num = comp.ticksToDisappear;
        }

        return num;
    }

    internal static void SetDuration(this Hediff h, int val)
    {
        if (h == null || val < 0)
            return;
        var comp = h.TryGetComp<HediffComp_Disappears>();
        if (comp != null)
        {
            comp.ticksToDisappear = val;
            comp.Props.showRemainingTime = true;
        }
    }

    internal static Pawn GetOtherPawn(this Hediff h)
    {
        Pawn pawn = null;
        if (h != null)
        {
            if (h.def.IsHediffWithTarget())
            {
                var hediffWithTarget = h as HediffWithTarget;
                if (hediffWithTarget.target is Pawn)
                    pawn = (Pawn)hediffWithTarget.target;
            }
            else if (h.def.IsHediffLink())
            {
                var comp = h.TryGetComp<HediffComp_Link>();
                if (comp != null)
                    pawn = comp.OtherPawn;
            }
        }

        return pawn;
    }

    internal static void SetOtherPawn(this Hediff h, Pawn p)
    {
        if (h == null || p == null)
            return;
        if (h.def.IsHediffWithTarget())
            (h as HediffWithTarget).target = p;
        if (h.def.IsHediffWithParents())
        {
            (h as HediffWithParents).SetParents(h.pawn, p, PregnancyUtility.GetInheritedGeneSet(p, h.pawn));
        }
        else if (h.def.IsHediffLink())
        {
            var comp = h.TryGetComp<HediffComp_Link>();
            if (comp != null)
                comp.other = p;
        }
    }

    internal static bool IsHediffWithOtherPawn(this HediffDef h)
    {
        if (h == null)
            return false;
        return h.IsHediffWithTarget() || h.IsHediffLink() || h.IsHediffWithParents();
    }

    internal static bool IsHediffLink(this HediffDef h)
    {
        if (h == null)
            return false;
        return h.HasComp(typeof(HediffComp_Link)) || h.HasComp(typeof(HediffComp_EntropyLink));
    }

    internal static bool IsHediffWithTarget(this HediffDef h)
    {
        if (h == null || !(h.hediffClass != null))
            return false;
        return h.hediffClass == typeof(HediffWithTarget) || h.hediffClass.BaseType == typeof(HediffWithTarget);
    }

    internal static bool IsHediffWithParents(this HediffDef h)
    {
        if (h == null || !(h.hediffClass != null))
            return false;
        return h.hediffClass == typeof(HediffWithParents) || h.hediffClass.BaseType == typeof(HediffWithParents);
    }

    internal static bool IsHediffWithComps(this HediffDef h)
    {
        if (h == null || !(h.hediffClass != null))
            return false;
        return h.hediffClass == typeof(HediffWithComps) || h.hediffClass.BaseType == typeof(HediffWithComps);
    }

    internal static bool IsHediffWithLevel(this HediffDef h)
    {
        return h != null && h.hediffClass != null && (h.hediffClass == typeof(Hediff_Level) || h.hediffClass.BaseType == typeof(Hediff_Level) || (h.hediffClass.BaseType != null && h.hediffClass.BaseType.BaseType == typeof(Hediff_Level)));
    }

    internal static bool IsHediffPsylink(this HediffDef h)
    {
        if (h == null || !(h.hediffClass != null))
            return false;
        return h.hediffClass == typeof(Hediff_Psylink) || h.hediffClass.ToString().EndsWith("Hediff_PsycastAbilities");
    }

    internal static bool IsHediffPsycastAbilities(this HediffDef h)
    {
        return h != null && h.hediffClass != null && h.hediffClass.ToString().EndsWith("Hediff_PsycastAbilities");
    }

    internal static bool IsHediffWithEffect(this HediffDef h)
    {
        if (h == null || !(h.hediffClass != null))
            return false;
        return h.hediffClass == typeof(Hediff_DeathrestEffect) || h.hediffClass.BaseType == typeof(Hediff_DeathrestEffect);
    }

    internal static bool IsImplant(this HediffDef h)
    {
        if (h == null || !(h.hediffClass != null))
            return false;
        return h.hediffClass == typeof(Hediff_Implant) || h.hediffClass.BaseType == typeof(Hediff_Implant);
    }

    internal static bool IsAddedPart(this HediffDef h)
    {
        if (h == null || !(h.hediffClass != null))
            return false;
        return h.hediffClass == typeof(Hediff_AddedPart) || h.hediffClass.BaseType == typeof(Hediff_AddedPart);
    }

    internal static bool IsHigh(this HediffDef h)
    {
        if (h == null || !(h.hediffClass != null))
            return false;
        return h.hediffClass == typeof(Hediff_High) || h.hediffClass.BaseType == typeof(Hediff_High);
    }

    internal static void RemoveHediff(this Pawn pawn, Hediff hediff)
    {
        bool flag = pawn == null || hediff == null;
        if (!flag)
        {
            pawn.health.hediffSet.hediffs.Remove(hediff);
            pawn.health.summaryHealth.Notify_HealthChanged();
            pawn.health.Notify_HediffChanged(hediff);
            pawn.needs.AddOrRemoveNeedsAsAppropriate();
            StatsReportUtility.Reset();
        }
    }

    internal static void ResurrectAndHeal(this Pawn pawn)
    {
        bool dead = pawn.Dead;
        if (dead)
        {
            ResurrectionUtility.TryResurrect(pawn);
        }
        try
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            Dictionary<Hediff, bool> dictionary = new Dictionary<Hediff, bool>();
            using (List<Hediff>.Enumerator enumerator = hediffs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    bool flag = enumerator.Current.Part != null && enumerator.Current.def.hediffClass == typeof(Hediff_AddedPart);
                    if (flag)
                    {
                        dictionary.Add(enumerator.Current, false);
                    }
                    else
                    {
                        dictionary.Add(enumerator.Current, true);
                    }
                }
            }
            foreach (Hediff hediff in dictionary.Keys)
            {
                try
                {
                    bool flag2 = dictionary[hediff];
                    if (flag2)
                    {
                        pawn.RemoveHediff(hediff);
                    }
                }
                catch
                {
                    bool dead2 = pawn.Dead;
                    if (dead2)
                    {
                        ResurrectionUtility.TryResurrect(pawn);
                    }
                }
            }
        }
        catch
        {
            MessageTool.Show("failed to heal", null);
        }
    }

    internal static void Medicate(this Pawn pawn)
    {
        bool flag = !pawn.HasHealthTracker();
        if (!flag)
        {
            Medicine medicine = null;
            MedicalCareCategory medicalCareCategory = (pawn.playerSettings != null) ? pawn.playerSettings.medCare : MedicalCareCategory.Best;
            medicalCareCategory = ((medicalCareCategory == MedicalCareCategory.NoCare || medicalCareCategory == MedicalCareCategory.NoMeds) ? MedicalCareCategory.HerbalOrWorse : medicalCareCategory);
            bool flag2 = medicalCareCategory == MedicalCareCategory.HerbalOrWorse;
            if (flag2)
            {
                medicine = (Medicine)ThingMaker.MakeThing(ThingDefOf.MedicineHerbal, null);
            }
            else
            {
                bool flag3 = medicalCareCategory == MedicalCareCategory.NormalOrWorse;
                if (flag3)
                {
                    medicine = (Medicine)ThingMaker.MakeThing(ThingDefOf.MedicineIndustrial, null);
                }
                else
                {
                    bool flag4 = medicalCareCategory == MedicalCareCategory.Best;
                    if (flag4)
                    {
                        medicine = (Medicine)ThingMaker.MakeThing(ThingDefOf.MedicineUltratech, null);
                    }
                }
            }
            TendUtility.DoTend(null, pawn, medicine);
        }
    }

    internal static void Hurt(this Pawn pawn)
    {
        var num = CEditor.zufallswert.Next(1, 3);
        for (var index = 0; index < num; ++index)
        {
            var hediff = DefDatabase<HediffDef>.AllDefs.Where(td => td.defName != null && td.isBad != null && td.injuryProps != null).ToList().RandomElement();
            pawn.AddHediff2(true, hediff);
        }
    }

    internal static void Addictize(this Pawn pawn)
    {
        HediffDef hediff = null;
        for (var index = 0; index < 10; ++index)
        {
            hediff = DefDatabase<HediffDef>.AllDefs.Where(td => td.defName != null && td.IsAddiction).ToList().RandomElement();
            if (!pawn.health.hediffSet.HasHediff(hediff))
                break;
        }

        if (hediff == null)
            return;
        pawn.AddHediff2(true, hediff);
    }

    internal static void Deaddictize(this Pawn pawn)
    {
        foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
        {
            bool flag = hediff.def != null && hediff.def.IsAddiction;
            if (flag)
            {
                pawn.RemoveHediff(hediff);
                break;
            }
        }
    }

    internal static void DamageUntilDeath(this Pawn pawn)
    {
        if (pawn.Dead)
            return;
        if (pawn.Downed)
            HealthUtility.DamageUntilDead(pawn);
        else
            HealthUtility.DamageUntilDowned(pawn);
    }

    internal static void Anaesthetize(this Pawn pawn)
    {
        HealthUtility.TryAnesthetize(pawn);
    }

    internal static void ShowDebugInfo(this Hediff h)
    {
        if (h == null || !Prefs.DevMode)
            return;
        try
        {
            if (h is HediffWithComps)
                foreach (var comp in (h as HediffWithComps).comps)
                    if (comp != null)
                        MessageTool.Show("compType=" + comp.GetType());
            MessageTool.Show("tags=" + h.def.tags.ListToString());
            MessageTool.Show(h.def.defName + "  Class=" + h.def.hediffClass + "  Bclass=" + h.def.hediffClass.BaseType);
        }
        catch
        {
        }
    }

    internal static List<HediffDef> ListOfHediffDef(
        string modname,
        Pawn p,
        BodyPartRecord bpr,
        string filter,
        bool wholeBody)
    {
        var bAll1 = modname.NullOrEmpty();
        var list = DefDatabase<HediffDef>.AllDefs.Where(td =>
        {
            if (td.label.NullOrEmpty())
                return false;
            return bAll1 || td.IsFromMod(modname);
        }).OrderBy(td => td.label).ToList();
        if (filter == Label.HB_ALLIMPLANTS)
            list = list.Where(td => td.IsImplant() || td.IsHediffWithLevel()).ToList();
        else if (filter == Label.HB_ALLADDICTIONS)
            list = list.Where(td => td.IsAddiction || td.IsHigh() || td.HasComp(typeof(HediffComp_DrugEffectFactor)) || td.HasComp(typeof(HediffComp_Effecter))).ToList();
        else if (filter == Label.HB_ALLDISEASES)
            list = list.Where(td => td.isBad != null && td.defName != "NoPain" && td.defName != "SpeedBoost").ToList();
        else if (filter == Label.HB_ALLINJURIES)
            list = list.Where(td => td.injuryProps != null).ToList();
        else if (filter == Label.HB_ALLTIME)
            list = list.Where(td => td.HasComp(typeof(HediffComp_Disappears))).ToList();
        else if (filter == Label.HB_WITHLEVEL)
            list = list.Where(td => td.IsHediffWithLevel()).ToList();
        if (bpr != null)
            list = list.Where(td => p.GetListOfAllowedBodyPartRecords(td).Contains(bpr)).ToList();
        else if (wholeBody)
            list = list.Where(td => td.IsForWholeBody()).ToList();
        return list;
    }

    internal static List<BodyPartRecord> GetListOfAllowedBodyPartRecords(
        this Pawn p,
        HediffDef h)
    {
        if (p == null || h == null)
            return null;
        var l = new List<BodyPartRecord>();
        if (!h.descriptionHyperlinks.NullOrEmpty())
            foreach (var descriptionHyperlink in h.descriptionHyperlinks)
                ResolveHyperlink(descriptionHyperlink, p, ref l, true);
        if (l.NullOrEmpty() && !h.tags.NullOrEmpty())
        {
            var bodyPartRecordList = new List<BodyPartRecord>();
            foreach (var tag in h.tags)
            {
                var partRecordsByName = p.GetListOfBodyPartRecordsByName(tag, h, tag == "All");
                if (!partRecordsByName.NullOrEmpty())
                    l.AddRange(partRecordsByName);
            }
        }

        if (l.NullOrEmpty())
            foreach (var key in AllBodyPartChecks.Keys)
                if (AllBodyPartChecks[key](h))
                {
                    l = p.GetListOfBodyPartRecordsByName(key, h, key == "All");
                    break;
                }

        if (l.NullOrEmpty())
        {
            l = p.GetListOfBodyPartRecordsByName(null, h, true);
            var flag = h.modContentPack == null || !h.modContentPack.IsCoreMod;
            if (h.injuryProps == null && !h.HasComp(typeof(HediffComp_TendDuration)) && h.IsHediffWithComps() | flag)
                l.Insert(0, null);
        }

        return l;
    }

    internal static void ResolveHyperlink(
        DefHyperlink defHyper,
        Pawn p,
        ref List<BodyPartRecord> l,
        bool firstPass)
    {
        if (defHyper.def is RecipeDef)
        {
            var def = (RecipeDef)defHyper.def;
            if (!def.appliedOnFixedBodyParts.NullOrEmpty())
            {
                foreach (var allPart in p.RaceProps.body.AllParts)
                foreach (var appliedOnFixedBodyPart in def.appliedOnFixedBodyParts)
                    if (allPart.def.defName == appliedOnFixedBodyPart.defName && !l.Contains(allPart))
                        l.Add(allPart);
            }
            else if (!def.appliedOnFixedBodyPartGroups.NullOrEmpty())
            {
                foreach (var allPart in p.RaceProps.body.AllParts)
                foreach (var fixedBodyPartGroup in def.appliedOnFixedBodyPartGroups)
                    if (allPart.def.defName == fixedBodyPartGroup.defName && !l.Contains(allPart))
                        l.Add(allPart);
            }
            else
            {
                if (!firstPass || def.descriptionHyperlinks.NullOrEmpty())
                    return;
                foreach (var descriptionHyperlink in def.descriptionHyperlinks)
                    ResolveHyperlink(descriptionHyperlink, p, ref l, false);
            }
        }
        else
        {
            if (!(defHyper.def is ThingDef))
                return;
            var def = (ThingDef)defHyper.def;
            var compByType = def.GetCompByType(typeof(CompProperties_UseEffectInstallImplant));
            if (compByType != null)
            {
                var effectInstallImplant = compByType as CompProperties_UseEffectInstallImplant;
                if (effectInstallImplant.bodyPart != null)
                    foreach (var allPart in p.RaceProps.body.AllParts)
                        if (allPart.def.defName == effectInstallImplant.bodyPart.defName && !l.Contains(allPart))
                            l.Add(allPart);
            }
            else if (firstPass && !def.descriptionHyperlinks.NullOrEmpty())
            {
                foreach (var descriptionHyperlink in def.descriptionHyperlinks)
                    ResolveHyperlink(descriptionHyperlink, p, ref l, false);
            }
        }
    }

    internal static List<BodyPartRecord> GetListOfBodyPartRecordsByName(
        this Pawn p,
        string defName,
        HediffDef h,
        bool all = false)
    {
        var bodyPartRecordList = new List<BodyPartRecord>();
        if (p != null && h != null)
        {
            if (defName == "WholeBody")
                bodyPartRecordList.Add(null);
            else
                foreach (var allPart in p.RaceProps.body.AllParts)
                    if (all)
                    {
                        if (!bodyPartRecordList.Contains(allPart))
                            bodyPartRecordList.Add(allPart);
                    }
                    else if (!h.hediffGivers.NullOrEmpty())
                    {
                        foreach (var hediffGiver in h.hediffGivers)
                            if (hediffGiver.partsToAffect.NullOrEmpty())
                            {
                                if (allPart.def.defName == defName)
                                {
                                    if (!bodyPartRecordList.Contains(allPart)) bodyPartRecordList.Add(allPart);
                                    break;
                                }
                            }
                            else if (hediffGiver.partsToAffect.Contains(allPart.def))
                            {
                                if (!bodyPartRecordList.Contains(allPart)) bodyPartRecordList.Add(allPart);
                                break;
                            }
                    }
                    else if (allPart.def.defName == defName && !bodyPartRecordList.Contains(allPart))
                    {
                        bodyPartRecordList.Add(allPart);
                    }
        }

        return bodyPartRecordList;
    }
}
