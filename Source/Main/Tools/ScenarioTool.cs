// Decompiled with JetBrains decompiler
// Type: CharacterEditor.ScenarioTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class ScenarioTool
{
    internal static int CurrentTakenPawnCount => !CEditor.InStartingScreen ? CurrentPawnList.Count : Find.GameInitData.startingPawnCount;

    internal static List<Pawn> CurrentPawnList => !CEditor.InStartingScreen ? PawnxTool.GetPawnList(Label.COLONISTS, true, Faction.OfPlayer) : Find.GameInitData.startingAndOptionalPawns;

    internal static List<ScenPart> ScenarioParts => Find.Scenario.GetMemberValue("parts", (List<ScenPart>)null);

    internal static void LoadCapsuleSetup(string filepath)
    {
        if (!FileIO.Exists(filepath))
            return;
        var s = FileIO.ReadFile(filepath).AsString(Encoding.UTF8);
        if (s.NullOrEmpty())
            return;
        var strArray = s.SplitNo("\n");
        if (strArray.NullOrEmpty())
        {
            MessageTool.Show("can't load scenario file. reason-> no data in file=" + filepath);
        }
        else
        {
            try
            {
                PawnxTool.DeleteAllPawns(Label.COLONISTS, true, Faction.OfPlayer);
            }
            catch
            {
            }

            var dictionary = new Dictionary<Pawn, string>();
            var pawnCount = 0;
            for (var index = 0; index < strArray.Length; ++index)
                try
                {
                    switch (index)
                    {
                        case 0:
                            SetScenarioParameterFromSeparatedString(strArray[index], out pawnCount);
                            continue;
                        case 1:
                            SetScenaioPartsFromSeparatedString(strArray[index]);
                            continue;
                        default:
                            var presetPawn = new PresetPawn();
                            var key = presetPawn.LoadPawn(-1, false, strArray[index]);
                            dictionary.Add(key, presetPawn.dicParams.GetValue(PresetPawn.Param.P33_relations));
                            continue;
                    }
                }
                catch
                {
                    MessageTool.Show("error while parsing scenario file. reason -> wrong or invalid data format");
                }

            if (CEditor.InStartingScreen)
                Find.GameInitData.startingPawnCount = pawnCount;
            foreach (var key in dictionary.Keys)
                key.SetRelationsFromSeparatedString(dictionary[key]);
        }
    }

    internal static void SaveCapsuleSetup(string filepath)
    {
        if (filepath.NullOrEmpty())
            return;
        var scenarioParameter = GetAllScenarioParameter();
        var asSeparatedString = GetAllScenarioPartsAsSeparatedString();
        var str1 = "";
        var text1 = "";
        foreach (var currentPawn in CurrentPawnList)
        {
            var str2 = new PresetPawn().SavePawn(currentPawn, -1);
            text1 = text1 + currentPawn.GetPawnName() + ",";
            str1 = str1 + "\n" + str2;
        }

        var str3 = text1.SubstringRemoveLast();
        var text2 = scenarioParameter + str3 + asSeparatedString + str1;
        FileIO.WriteFile(filepath, text2.AsBytes(Encoding.UTF8));
    }

    internal static int GetTypeReplacer<T>(T sd) where T : ScenPart
    {
        return !(sd.GetType() == typeof(ScenPart_StartingThing_Defined)) ? !(sd.GetType() == typeof(ScenPart_ScatterThingsAnywhere)) ? !(sd.GetType() == typeof(ScenPart_ScatterThingsNearPlayerStart)) ? !(sd.GetType() == typeof(ScenPart_StartingAnimal)) ? !(sd.GetType() == typeof(ScenPart_StartingThingStyle_Defined)) ? !(sd.GetType() == typeof(ScenPart_ScatterThingsStyleAnywhere)) ? !(sd.GetType() == typeof(ScenPart_StartingAnimalExtra)) ? !(sd.GetType() == typeof(ScenPart_ScatterThingsNearPlayerExtra)) ? 0 : 8 : 7 : 6 : 5 : 4 : 3 : 2 : 1;
    }

    internal static string GetScenarioPartString<T>(T sd) where T : ScenPart
    {
        var str = "";
        var selectedScenarioPart = sd.GetSelectedScenarioPart();
        return !sd.IsScenarioAnimal() ? str + GetTypeReplacer(sd) + "|" + (selectedScenarioPart == null || selectedScenarioPart.thingDef == null ? "|" : selectedScenarioPart.thingDef.SDefname() + "|") + (selectedScenarioPart == null || selectedScenarioPart.stuff == null ? "|" : selectedScenarioPart.stuff.SDefname() + "|") + (selectedScenarioPart == null || selectedScenarioPart.style == null ? "|" : selectedScenarioPart.style.SDefname() + "|") + "|" + (selectedScenarioPart != null ? selectedScenarioPart.quality + "|" : "|") + (selectedScenarioPart != null ? selectedScenarioPart.stackVal.ToString() : "") : str + GetTypeReplacer(sd) + "|" + (selectedScenarioPart == null || selectedScenarioPart.pkd == null || selectedScenarioPart.pkd.defName.NullOrEmpty() ? "|" : selectedScenarioPart.pkd.defName + "|") + (selectedScenarioPart != null ? selectedScenarioPart.stackVal + "|" : "|") + (selectedScenarioPart != null ? (int)selectedScenarioPart.gender + "|" : "|") + (selectedScenarioPart != null ? selectedScenarioPart.age + "|" : "|") + (selectedScenarioPart == null || selectedScenarioPart.pawnName == null ? "" : (selectedScenarioPart.pawnName as NameSingle).Name);
    }

    internal static string GetAllScenarioParameter()
    {
        return "" + ScenarioTool.CurrentPawnList.FirstOrFallback(null).Faction.Name + "|" + (CEditor.InStartingScreen ? Find.GameInitData.startingPawnCount : ScenarioTool.CurrentPawnList.Count).ToString() + "|";
    }

    internal static string GetAllScenarioPartsAsSeparatedString()
    {
        var scenarioParts = ScenarioParts;
        string str;
        if (scenarioParts.NullOrEmpty())
        {
            str = "";
        }
        else
        {
            var text = "\n";
            foreach (var sd in scenarioParts)
            {
                text += GetScenarioPartString(sd);
                text += ":";
            }

            str = text.SubstringRemoveLast();
        }

        return str;
    }

    internal static void SetScenarioParameterFromSeparatedString(string s, out int pawnCount)
    {
        pawnCount = 0;
        if (s.NullOrEmpty())
            return;
        var array = s.SplitNo("|");
        Faction.OfPlayer.Name = array.GetStringValue(0);
        pawnCount = array.GetStringValue(1).AsInt32();
    }

    internal static void SetScenaioPartsFromSeparatedString(string s)
    {
        if (s.NullOrEmpty())
            return;
        RemoveAllSupportedScenarioPartsFromList();
        var scenarioParts = ScenarioParts;
        foreach (var s1 in s.SplitNo(":"))
            if (!s1.NullOrEmpty())
            {
                var array = s1.SplitNo("|");
                var stringValue = array.GetStringValue(0);
                var num = stringValue == "4" ? 1 : stringValue == "7" ? 1 : 0;
                var flag1 = stringValue == "1" || stringValue == "3" || stringValue == "5";
                var flag2 = stringValue == "2" || stringValue == "6";
                var flag3 = stringValue == "3" || stringValue == "8";
                if (num != 0)
                {
                    var s2 = Selected.ByThingDef(null);
                    s2.pkd = DefTool.PawnKindDef(array.GetStringValue(1));
                    s2.stackVal = array.GetStringValue(2).AsInt32();
                    Gender result;
                    Enum.TryParse(array.GetStringValue(3), out result);
                    s2.gender = result;
                    s2.age = array.GetStringValue(4).AsInt32();
                    s2.pawnName = new NameSingle(array.GetStringValue(5));
                    AddScenePart(CreateScenePart_Animal(s2));
                }
                else
                {
                    var s2 = Selected.ByName(array.GetStringValue(1), array.GetStringValue(2), array.GetStringValue(3), array.GetStringValue(4).HexStringToColor(), array.GetStringValue(5).AsInt32(), array.GetStringValue(6).AsInt32());
                    if (flag1)
                        AddScenePart(CreateScenePart_Defined(s2));
                    else if (flag2)
                        AddScenePart(CreateScenePart_Anywhere(s2));
                    else if (flag3)
                        AddScenePart(CreateScenePart_NearPlayer(s2));
                }
            }
    }

    internal static void RemoveAllSupportedScenarioPartsFromList()
    {
        var scenarioParts = ScenarioParts;
        if (scenarioParts.NullOrEmpty())
            return;
        for (var index = scenarioParts.Count - 1; index >= 0; --index)
            if (scenarioParts[index].IsSupportedScenarioPart())
                scenarioParts.Remove(scenarioParts[index]);
    }

    internal static Selected GetSelectedScenarioPart<T>(this T part) where T : ScenPart
    {
        var selected1 = Selected.ByThingDef(null);
        Selected selected2;
        if (!part.IsSupportedScenarioPart())
        {
            selected2 = selected1;
        }
        else
        {
            if (part.IsScenarioAnimal())
            {
                selected1.age = part.GetMemberValue("age", 1);
                selected1.pkd = part.GetMemberValue("animalKind", (PawnKindDef)null);
                selected1.stackVal = part.GetMemberValue("count", 1);
                selected1.gender = part.GetMemberValue("gender", (Gender)0);
                selected1.pawnName = part.GetMemberValue("pawnName", (Name)null);
                selected1.location = part.GetMemberValue("location", new IntVec3());
            }
            else
            {
                selected1.thingDef = part.GetMemberValue("thingDef", (ThingDef)null);
                selected1.stuff = part.GetMemberValue("stuff", (ThingDef)null);
                selected1.style = part.GetMemberValue("styleDef", (ThingStyleDef)null);
                QualityCategory? memberValue = part.GetMemberValue("quality", (QualityCategory)2);
                selected1.quality = memberValue.HasValue ? (int)memberValue.Value : 0;
                selected1.stackVal = part.GetMemberValue("count", 1);
                selected1.location = part.GetMemberValue("location", new IntVec3());
            }

            selected2 = selected1;
        }

        return selected2;
    }

    internal static void SetScenarioParts(List<ScenPart> l)
    {
        Find.Scenario.SetMemberValue("parts", l);
    }

    internal static int GetScenarioPartCount<T>(this T part) where T : ScenPart
    {
        return part.GetMemberValue("count", 0);
    }

    internal static void SetScenarioPartCount<T>(this T part, int count) where T : ScenPart
    {
        if (!part.IsSupportedScenarioPart())
            return;
        part.SetMemberValue(nameof(count), count);
    }

    internal static ScenPart_StartingThingStyle_Defined CreateScenePart_Defined(
        Selected s)
    {
        var thingStyleDefined = new ScenPart_StartingThingStyle_Defined();
        thingStyleDefined.def = DefTool.GetDef<ScenPartDef>("StartingThing_Defined");
        thingStyleDefined.SetMemberValue("thingDef", s.thingDef);
        thingStyleDefined.SetMemberValue("stuff", s.stuff);
        thingStyleDefined.SetMemberValue("styleDef", s.style);
        thingStyleDefined.SetMemberValue("quality", (QualityCategory)(byte)s.quality);
        thingStyleDefined.SetMemberValue("count", s.stackVal);
        thingStyleDefined.location = s.location;
        return thingStyleDefined;
    }

    internal static ScenPart_ScatterThingsStyleAnywhere CreateScenePart_Anywhere(
        Selected s)
    {
        var thingsStyleAnywhere = new ScenPart_ScatterThingsStyleAnywhere();
        thingsStyleAnywhere.def = DefTool.GetDef<ScenPartDef>("ScatterThingsAnywhere");
        thingsStyleAnywhere.SetMemberValue("thingDef", s.thingDef);
        thingsStyleAnywhere.SetMemberValue("stuff", s.stuff);
        thingsStyleAnywhere.SetMemberValue("styleDef", s.style);
        thingsStyleAnywhere.SetMemberValue("quality", (QualityCategory)(byte)s.quality);
        thingsStyleAnywhere.SetMemberValue("count", s.stackVal);
        thingsStyleAnywhere.location = s.location;
        return thingsStyleAnywhere;
    }

    internal static ScenPart_ScatterThingsNearPlayerExtra CreateScenePart_NearPlayer(
        Selected s)
    {
        var thingsNearPlayerExtra = new ScenPart_ScatterThingsNearPlayerExtra();
        thingsNearPlayerExtra.def = DefTool.GetDef<ScenPartDef>("ScatterThingsNearPlayerStart");
        thingsNearPlayerExtra.SetMemberValue("thingDef", s.thingDef);
        thingsNearPlayerExtra.SetMemberValue("stuff", s.stuff);
        thingsNearPlayerExtra.SetMemberValue("styleDef", s.style);
        thingsNearPlayerExtra.SetMemberValue("quality", (QualityCategory)(byte)s.quality);
        thingsNearPlayerExtra.SetMemberValue("count", s.stackVal);
        thingsNearPlayerExtra.location = s.location;
        return thingsNearPlayerExtra;
    }

    internal static ScenPart_StartingAnimalExtra CreateScenePart_Animal(
        Selected s)
    {
        var startingAnimalExtra = new ScenPart_StartingAnimalExtra();
        startingAnimalExtra.def = DefTool.GetDef<ScenPartDef>("StartingAnimal");
        startingAnimalExtra.SetMemberValue("animalKind", s.pkd);
        startingAnimalExtra.SetMemberValue("count", s.stackVal);
        startingAnimalExtra.SetMemberValue("gender", s.gender);
        startingAnimalExtra.SetMemberValue("age", s.age);
        startingAnimalExtra.SetMemberValue("pawnName", s.pawnName);
        startingAnimalExtra.location = s.location;
        return startingAnimalExtra;
    }

    internal static void AddScenePart<T>(T scenPart) where T : ScenPart
    {
        if (!scenPart.IsSupportedScenarioPart())
            return;
        var l = ScenarioParts ?? new List<ScenPart>();
        try
        {
            l.Add(scenPart);
            SetScenarioParts(l);
        }
        catch
        {
            MessageTool.Show("couldn't add scenario parts", MessageTypeDefOf.RejectInput);
        }
    }

    internal static void MergeNonScenarioPart(Selected s, ref List<ScenPart> l, bool doMerge = true)
    {
        if (s.stackVal <= 0)
            return;
        var flag = false;
        if (doMerge)
            foreach (var part in l)
                if (s.pkd != null)
                {
                    var memberValue = part.GetMemberValue("animalKind", (PawnKindDef)null);
                    if ((memberValue == null ? 0 : memberValue.defName == s.pkd.defName ? 1 : 0) != 0 && part.GetMemberValue("age", 0) == s.age && part.GetMemberValue("gender", (Gender)0) == s.gender)
                    {
                        var count = part.GetScenarioPartCount() + s.stackVal;
                        part.SetScenarioPartCount(count);
                        flag = true;
                        break;
                    }
                }
                else
                {
                    var memberValue = part.GetMemberValue("thingDef", (ThingDef)null);
                    if ((memberValue == null || s.HasQuality ? 0 : memberValue.defName == s.thingDef.defName ? 1 : 0) != 0)
                    {
                        var count = part.GetScenarioPartCount() + s.stackVal;
                        part.SetScenarioPartCount(count);
                        flag = true;
                        break;
                    }
                }

        if (flag)
            return;
        if (s.pkd != null)
            l.Add(CreateScenePart_Animal(s));
        else
            l.Add(CreateScenePart_Defined(s));
    }

    internal static void AddScenarioPartMerged(Selected s, bool addToTaken)
    {
        if (s.stackVal <= 0)
            return;
        var flag = false;
        foreach (var scenarioPart in ScenarioParts)
            if (s.pkd != null)
            {
                var memberValue = scenarioPart.GetMemberValue("animalKind", (PawnKindDef)null);
                if ((memberValue == null ? 0 : memberValue.defName == s.pkd.defName ? 1 : 0) != 0 && scenarioPart.GetMemberValue("age", 0) == s.age && scenarioPart.GetMemberValue("gender", (Gender)0) == s.gender)
                {
                    var count = scenarioPart.GetScenarioPartCount() + s.stackVal;
                    scenarioPart.SetScenarioPartCount(count);
                    flag = true;
                    break;
                }
            }
            else
            {
                var memberValue = scenarioPart.GetMemberValue("thingDef", (ThingDef)null);
                if ((memberValue == null || s.HasQuality ? 0 : memberValue.defName == s.thingDef.defName ? 1 : 0) != 0 && ((addToTaken && scenarioPart.IsScenarioDefined()) || (addToTaken && scenarioPart.IsScenarioNearPlayer()) ? 1 : addToTaken ? 0 : scenarioPart.IsScatterAnywherePart() ? 1 : 0) != 0)
                {
                    var count = scenarioPart.GetScenarioPartCount() + s.stackVal;
                    scenarioPart.SetScenarioPartCount(count);
                    flag = true;
                    break;
                }
            }

        if (flag)
            return;
        if (s.pkd != null)
            AddScenarioPartAnimal(s);
        else if (addToTaken)
            AddScenarioPartTaken(s);
        else
            AddScenarioPartMap(s);
    }

    private static void AddScenarioPartTaken(Selected s)
    {
        AddScenePart(CreateScenePart_Defined(s));
    }

    private static void AddScenarioPartMap(Selected s)
    {
        AddScenePart(CreateScenePart_Anywhere(s));
    }

    private static void AddScenarioPartTakenNear(Selected s)
    {
        AddScenePart(CreateScenePart_NearPlayer(s));
    }

    internal static void AddScenarioPartAnimal(Selected s)
    {
        AddScenePart(CreateScenePart_Animal(s));
    }

    internal static bool IsScenarioAnimal(this ScenPart part)
    {
        if (part == null)
            return false;
        return part.GetType() == typeof(ScenPart_StartingAnimal) || part.GetType() == typeof(ScenPart_StartingAnimalExtra);
    }

    internal static bool IsScatterAnywherePart(this ScenPart part)
    {
        if (part == null)
            return false;
        return part.GetType() == typeof(ScenPart_ScatterThingsAnywhere) || part.GetType() == typeof(ScenPart_ScatterThingsStyleAnywhere);
    }

    internal static bool IsScenarioDefined(this ScenPart part)
    {
        if (part == null)
            return false;
        return part.GetType() == typeof(ScenPart_StartingThing_Defined) || part.GetType() == typeof(ScenPart_StartingThingStyle_Defined);
    }

    internal static bool IsScenarioNearPlayer(this ScenPart part)
    {
        if (part == null)
            return false;
        return part.GetType() == typeof(ScenPart_ScatterThingsNearPlayerStart) || part.GetType() == typeof(ScenPart_ScatterThingsNearPlayerExtra);
    }

    internal static bool IsSupportedScenarioPart(this ScenPart part)
    {
        if (part == null)
            return false;
        return part.GetType() == typeof(ScenPart_StartingAnimal) || part.GetType() == typeof(ScenPart_StartingAnimalExtra) || part.GetType() == typeof(ScenPart_StartingThing_Defined) || part.GetType() == typeof(ScenPart_StartingThingStyle_Defined) || part.GetType() == typeof(ScenPart_ScatterThingsStyleAnywhere) || part.GetType() == typeof(ScenPart_ScatterThingsAnywhere) || part.GetType() == typeof(ScenPart_ScatterThingsNearPlayerStart) || part.GetType() == typeof(ScenPart_ScatterThingsNearPlayerExtra);
    }
}
