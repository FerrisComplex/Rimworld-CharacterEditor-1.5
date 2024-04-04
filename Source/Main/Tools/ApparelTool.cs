// Decompiled with JetBrains decompiler
// Type: CharacterEditor.ApparelTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Object = UnityEngine.Object;

namespace CharacterEditor;

internal static class ApparelTool
{
    internal static bool IsApparelForNeck(this ThingDef a)
    {
        return a != null && a.apparel.layers != null && a.apparel.layers.Contains(ApparelLayerDefOf.Overhead) && a.HasBodyPartGroupDefName("Neck");
    }

    internal static bool IsForNeck(this Apparel a)
    {
        return a != null && a.def.apparel.layers != null && a.def.apparel.layers.Contains(ApparelLayerDefOf.Overhead) && a.def.HasBodyPartGroupDefName("Neck");
    }

    internal static bool IsForLegs(this Apparel a)
    {
        return a != null && !a.def.apparel.bodyPartGroups.NullOrEmpty() && a.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs);
    }

    internal static bool IsForEyes(this Apparel a)
    {
        return a != null && a.def.apparel.layers != null && a.def.apparel.layers.Contains(ApparelLayerDefOf.EyeCover);
    }

    internal static bool HasAnyApparel(this Pawn pawn)
    {
        return pawn.HasApparelTracker() && !pawn.apparel.WornApparel.NullOrEmpty();
    }

    internal static bool IsWornByPawn(this Pawn pawn, Apparel a)
    {
        return pawn.HasAnyApparel() && pawn.apparel.WornApparel.Contains(a);
    }

    internal static bool HasThisApparelDef(this Pawn pawn, ThingDef t)
    {
        if (pawn.HasAnyApparel())
            foreach (Thing thing in pawn.apparel.WornApparel)
                if (thing.def == t)
                    return true;

        return false;
    }

    internal static Apparel RandomWornApparel(this Pawn pawn)
    {
        return !pawn.HasAnyApparel() ? null : pawn.apparel.WornApparel.RandomElement();
    }

    internal static Apparel ThisOrFirstWornApparel(this Pawn pawn, Apparel apparel)
    {
        if (pawn == null || pawn.apparel == null || pawn.apparel.WornApparel.NullOrEmpty())
            return null;
        return apparel == null || !pawn.apparel.WornApparel.Contains(apparel) ? pawn.apparel.WornApparel.FirstOrFallback() : apparel;
    }

    internal static string GetAllApparelAsSeparatedString(this Pawn p)
    {
        if (!p.HasApparelTracker() || p.apparel.WornApparel.NullOrEmpty())
            return "";
        var text = "";
        foreach (var apparel in p.apparel.WornApparel)
        {
            text += apparel.GetAsSeparatedString();
            text += ":";
        }

        return text.SubstringRemoveLast();
    }

    internal static void SetApparelFromSeparatedString(this Pawn p, string s)
    {
        if (!p.HasApparelTracker())
            return;
        p.apparel.DestroyAll();
        if (s.NullOrEmpty())
            return;
        foreach (var s1 in s.SplitNo(":"))
        {
            var strArray = s1.SplitNo("|");
            if (strArray.Length >= 6)
            {
                var styledefname = strArray.Length >= 7 ? strArray[6] : "";
                var apparel = GenerateApparel(Selected.ByName(strArray[0], strArray[1], styledefname, strArray[2].HexStringToColor(), strArray[3].AsInt32(), strArray[4].AsInt32()));
                if (apparel != null)
                {
                    apparel.HitPoints = strArray[5].AsInt32();
                    p.WearThatApparel(apparel);
                }
            }
        }
    }

    internal static void TestAllApparel(this Pawn pawn)
    {
        if (!pawn.HasApparelTracker() || !pawn.HasAnyApparel())
            return;
        for (var index = pawn.apparel.WornApparel.ToList().Count - 1; index >= 0; --index)
        {
            var a = pawn.apparel.WornApparel[index];
            if (!pawn.ApparelGraphicTest(a, false))
                pawn.TransferToInventory(a);
        }
    }

    internal static void DestroyAllApparel(this Pawn pawn)
    {
        if (!pawn.HasApparelTracker())
            return;
        pawn.apparel.DestroyAll();
    }

    internal static void DestroyApparel(this Pawn pawn, Apparel apparel)
    {
        if (!pawn.HasApparelTracker())
            return;
        pawn.outfits.forcedHandler.SetForced(apparel, false);
        pawn.apparel.WornApparel.Remove(apparel);
        apparel.Destroy();
    }

    internal static List<Apparel> ListOfCopyOutfits(this Pawn pawn)
    {
        return !pawn.HasAnyApparel() ? null : pawn.apparel.WornApparel.ListFullCopy();
    }

    internal static void PasteCopyOutfits(this Pawn pawn, List<Apparel> l)
    {
        if (pawn.HasApparelTracker())
        {
            pawn.apparel.WornApparel.Clear();
            if (!l.NullOrEmpty())
            {
                foreach (var apparel in l)
                    pawn.CreateAndWearApparel(Selected.ByThing(apparel));
            }
        }

        CEditor.API.UpdateGraphics();
    }

    internal static List<BodyPartGroupDef> GetCoveredBodyPartGroupDefs(
        this Pawn pawn)
    {
        var l = new List<BodyPartGroupDef>();
        foreach (var apparel in pawn.apparel.WornApparel)
            l.AddFromList(apparel.def.apparel.bodyPartGroups);
        return l;
    }

    internal static List<Apparel> GetApparelWithThisLayer(
        this Pawn pawn,
        ApparelLayerDef ald)
    {
        return pawn.apparel.WornApparel.Where(td => td.def.apparel.layers.Contains(ald)).ToList();
    }

    internal static int CountApparelThatWillBeReplacedByThisApparel(
        this Pawn pawn,
        ThingDef apparelDef)
    {
        var num = 0;
        try
        {
            for (var index = 0; index < pawn.apparel.WornApparel.Count; ++index)
                if (!ApparelUtility.CanWearTogether(apparelDef, pawn.apparel.WornApparel[index].def, pawn.RaceProps.body))
                    ++num;
        }
        catch
        {
        }

        return num;
    }

    internal static bool CreateAndWearApparel(this Pawn pawn, Selected s)
    {
        return pawn.CreateAndWearApparel(s, out var _, false);
    }

    internal static bool CreateAndWearApparel(
        this Pawn pawn,
        Selected s,
        out Apparel a,
        bool showError)
    {
        a = null;
        if (pawn.HasApparelTracker())
        {
            a = GenerateApparel(s);
            if (pawn.CanWearApparel(a, showError))
            {
                pawn.WearThatApparel(a);
                return true;
            }
        }

        return false;
    }

    internal static void ReplaceAndWearRandomApparel(
        this Pawn pawn,
        Apparel a,
        ApparelLayerDef ald = null,
        bool pawnSpecific = false)
    {
        if (!pawn.HasApparelTracker())
            return;
        var apparelLayerDef = a != null ? a.def.apparel.layers.FirstOrDefault() : ald;
        var bpd = a != null ? a.def.apparel.bodyPartGroups.FirstOrDefault() : null;
        if (pawn.GetApparelWithThisLayer(apparelLayerDef).CountAllowNull() == 1)
        {
            var str = bpd == null || bpd.defName == null ? "" : bpd.defName.ToLower();
            if (!str.StartsWith("left") && !str.StartsWith("right") && !str.EndsWith("left") && !str.EndsWith("right"))
                bpd = null;
        }

        var l = ListOfApparel(null, apparelLayerDef, bpd);
        var hasApparelTags = pawn.kindDef.apparelTags != null;
        if (pawnSpecific)
            l = l.Where(td =>
            {
                if (td.IsFromMod(pawn.kindDef.GetModName()))
                    return true;
                return hasApparelTags && td.apparel.tags != null && td.apparel.tags.Select(t => pawn.kindDef.apparelTags.Contains(t)) != null;
            }).ToHashSet();
        Selected s = null;
        var num = 10;
        if (l.Count <= 1 && ald == ApparelLayerDefOf.EyeCover)
            return;
        do
        {
            for (var index = 0; index < 5; ++index)
            {
                s = Selected.Random(l, Event.current.alt);
                if (pawn.CountApparelThatWillBeReplacedByThisApparel(s.thingDef) <= 1)
                    break;
            }

            --num;
        } while (!pawn.CreateAndWearApparel(s) && num > 0);
    }

    internal static bool CanWearApparel(this Pawn pawn, Apparel a, bool showError)
    {
        try
        {
            if (!pawn.HasApparelTracker() || a == null || !ApparelUtility.HasPartsToWear(pawn, a.def))
                return false;
            return pawn.IsAnimal() || pawn.ApparelGraphicTest(a, showError);
        }
        catch
        {
            return pawn.apparel.CanWearWithoutDroppingAnything(a.def);
        }
    }

    internal static void ReplaceThatApparel(this Pawn pawn, Apparel old, Apparel neu)
    {
        if (!pawn.HasApparelTracker() || pawn.story == null || neu == null || !ApparelUtility.HasPartsToWear(pawn, neu.def))
            return;
        pawn.AllowAllApparel(neu);
        pawn.apparel.Remove(old);
        pawn.apparel.GetMemberValue("wornApparel", (ThingOwner<Apparel>)null).TryAdd(neu);
    }

    internal static void WearThatApparel(this Pawn pawn, Apparel a)
    {
        if (!pawn.HasApparelTracker() || a == null || !ApparelUtility.HasPartsToWear(pawn, a.def))
            return;
        pawn.AllowAllApparel(a);
        if (pawn.HasStoryTracker() && (pawn.story.bodyType == BodyTypeDefOf.Child || pawn.story.bodyType == BodyTypeDefOf.Baby))
            pawn.apparel.Wear(a, false);
        else
            pawn.ForceWearThatApparel(a);
    }

    internal static void ForceWearThatApparel(this Pawn pawn, Apparel a)
    {
        a.DeSpawnOrDeselect();
        if (CompBiocodable.IsBiocoded(a) && !CompBiocodable.IsBiocodedFor(a, pawn))
        {
            var comp = a.TryGetComp<CompBiocodable>();
            Log.Warning(pawn + " tried to wear " + a + " but it is biocoded for " + comp.CodedPawnLabel + " .");
        }
        else
        {
            for (var index = pawn.apparel.WornApparelCount - 1; index >= 0; --index)
            {
                var apparel = pawn.apparel.WornApparel[index];
                if (!ApparelUtility.CanWearTogether(a.def, apparel.def, pawn.RaceProps.body))
                    pawn.apparel.Remove(apparel);
            }
        }

        if (a.Wearer != null)
            Log.Warning(pawn + " is trying to wear " + a + " but this apparel already has a wearer (" + a.Wearer + "). This may or may not cause bugs.");
        pawn.apparel.GetMemberValue("wornApparel", (ThingOwner<Apparel>)null).TryAdd(a, false);
    }

    internal static void AskToWearIncompatibleApparel(this Pawn pawn, Apparel a)
    {
        if (!pawn.HasStoryTracker() || (!a.def.IsFromCoreMod() && !a.def.IsFromMod("Royalty")) || pawn.story.bodyType.IsFromCoreMod() || pawn.story.bodyType.IsFromMod("Royalty"))
            return;
        WindowTool.Open(Dialog_MessageBox.CreateConfirmation(new TaggedString("core or royalty apparel that is not compatible to the current body type found - try to wear it anyway?\n\nif you do, you will get texture error messages on savegame load. but they should be harmless."), () =>
        {
            ApparelGraphicRecordGetter.TryGetGraphicApparel(a, pawn.story.bodyType, out var apparelGraphicRecord);
            pawn.AllowAllApparel(a);
            pawn.apparel.Wear(a, false);
            CEditor.API.UpdateGraphics();
        }));
    }

    internal static Apparel GenerateApparel(Selected s)
    {
        if (s == null || s.thingDef == null)
            return null;
        var thingDef = DefTool.ThingDef(s.thingDef.defName);
        if (thingDef == null || !thingDef.IsApparel)
            return null;
        s.stuff = s.thingDef.ThisOrDefaultStuff(s.stuff);
        if (!s.thingDef.MadeFromStuff)
            s.stuff = null;
        var apparel = (Apparel)ThingMaker.MakeThing(s.thingDef, s.stuff);
        apparel.HitPoints = apparel.MaxHitPoints;
        apparel.SetQuality(s.quality);
        apparel.SetDrawColor(s.DrawColor);
        apparel.stackCount = s.stackVal;
        if (s.style != null)
        {
            apparel.StyleDef = s.style;
            apparel.StyleDef.color = s.style.color;
        }

        return apparel;
    }

    internal static List<Apparel> GetConflictedApparelList(
        this Pawn pawn,
        ThingDef apparelToCheck)
    {
        var apparelList = new List<Apparel>();
        try
        {
            foreach (var bodyPartGroup in apparelToCheck.apparel.bodyPartGroups)
            foreach (var apparel in CEditor.API.Pawn.apparel.WornApparel)
                if (!ApparelUtility.CanWearTogether(apparel.def, apparelToCheck, CEditor.API.Pawn.RaceProps.body) && !apparelList.Contains(apparel))
                    apparelList.Add(apparel);
        }
        catch
        {
        }

        return apparelList;
    }

    internal static bool RenderAsPack(ThingDef a)
    {
        return a.apparel.LastLayer.IsUtilityLayer && (a.apparel.wornGraphicData == null || a.apparel.wornGraphicData.renderUtilityAsPack);
    }

    internal static string GetApparelPath(ThingDef a, Pawn p)
    {
        var str = "";
        if (a != null && a.apparel != null)
        {
            var flag = false;
            if (a.apparel.layers != null && a.apparel.layers.Contains(ApparelLayerDefOf.Overhead))
            {
                str = a.apparel.wornGraphicPath;
                flag = true;
            }

            if (string.IsNullOrEmpty(str) && a.apparel.wornGraphicPath.NullOrEmpty())
            {
                str = a.apparel.wornGraphicPath;
                flag = true;
            }

            if (string.IsNullOrEmpty(str) && RenderAsPack(a))
            {
                str = a.apparel.wornGraphicPath;
                flag = true;
            }

            if (string.IsNullOrEmpty(str) && a.apparel.wornGraphicPath == BaseContent.PlaceholderImagePath)
            {
                str = a.apparel.wornGraphicPath;
                flag = true;
            }

            if (string.IsNullOrEmpty(str) && !flag && p.story != null && p.story.bodyType != null && p.story.bodyType.defName != null)
                str = a.apparel.wornGraphicPath + "_" + p.story.bodyType.defName;
        }

        return str;
    }

    internal static bool ApparalGraphicTest2(this Pawn pawn, ThingDef a, bool showError)
    {
        var flag = false;
        var text = "";
        if (pawn.HasApparelTracker() && a != null && ApparelUtility.HasPartsToWear(pawn, a))
        {
            if (a.apparel.layers.Contains(ApparelLayerDefOf.Overhead) || a.apparel.layers.Contains(ApparelLayerDefOf.EyeCover) || a.apparel.layers.Contains(ApparelLayerDefOf.Belt) || ((a.IsFromCoreMod() || a.IsFromMod("Royalty") || a.IsFromMod("Ideology")) && (pawn.story.bodyType.IsFromCoreMod() || pawn.story.bodyType.IsFromMod("Royalty") || pawn.story.bodyType.IsFromMod("Ideology"))))
                return true;
            text = GetApparelPath(a, pawn);
            try
            {
                var str1 = text.SubstringBackwardTo("/");
                var str2 = text.SubstringBackwardFrom("/");
                var list = ContentFinder<Texture2D>.GetAllInFolder(str1).ToList();
                if (Prefs.DevMode)
                    Log.Message("searching for compatible apparel in subpath=" + str1 + " .. found=" + list.CountAllowNull() + " .. checking for matching entity=" + str2);
                foreach (Object @object in list)
                    if (@object.name.Contains(str2))
                    {
                        flag = true;
                        break;
                    }

                if (!flag && a.apparel.layers.Contains(ApparelLayerDefOf.Overhead) && a.apparel.layers.Count == 1)
                    return true;
            }
            catch
            {
            }
        }

        if (!flag & showError)
            MessageTool.Show(Label.NOT_COMPATIBLE_APPAREL + ", missing texture=" + text, MessageTypeDefOf.RejectInput);
        return flag;
    }

    internal static bool ApparelGraphicTest(this Pawn pawn, Apparel a, bool showError, bool force = false)
    {
        return (!force && !CEditor.API.GetO(OptionB.DOAPPARELCHECK)) || pawn.ApparalGraphicTest2(a.def, showError);
    }

    internal static bool HasSameCover(this ThingDef a, ThingDef b)
    {
        if (a == null || b == null)
            return false;
        foreach (var bodyPartGroup1 in a.apparel.bodyPartGroups)
        foreach (var bodyPartGroup2 in b.apparel.bodyPartGroups)
            if (bodyPartGroup1 == bodyPartGroup2)
                return true;

        return false;
    }

    internal static bool HasSameApparelLayer(this ThingDef a, ThingDef b)
    {
        if (a == null || b == null)
            return false;
        foreach (var layer1 in a.apparel.layers)
        foreach (var layer2 in b.apparel.layers)
            if (layer1 == layer2)
                return true;

        return false;
    }

    internal static void PasteApparelLayer(this ThingDef t, List<ApparelLayerDef> l)
    {
        if (t == null || l == null)
            return;
        if (t.apparel.layers == null)
            t.apparel.layers = new List<ApparelLayerDef>();
        foreach (var apparelLayerDef in l)
            t.AddApparelLayer(apparelLayerDef);
    }

    internal static bool HasApparelLayer(this ThingDef t, ApparelLayerDef apparelLayerDef)
    {
        return t != null && t.apparel.layers != null && t.apparel.layers.Contains(apparelLayerDef);
    }

    internal static void AddApparelLayer(this ThingDef t, ApparelLayerDef apparelLayerDef)
    {
        if (t == null || apparelLayerDef == null)
            return;
        if (t.apparel.layers == null)
            t.apparel.layers = new List<ApparelLayerDef>();
        if (!t.HasApparelLayer(apparelLayerDef))
            t.apparel.layers.Add(apparelLayerDef);
        t.ResolveReferences();
    }

    internal static void PasteBodyPartGroup(this ThingDef t, List<BodyPartGroupDef> l)
    {
        if (t == null || l == null)
            return;
        if (t.apparel.bodyPartGroups == null)
            t.apparel.bodyPartGroups = new List<BodyPartGroupDef>();
        foreach (var bodyPartGroupDef in l)
            t.AddBodyPart(bodyPartGroupDef);
    }

    internal static bool HasBodyPartGroupDefName(this ThingDef t, string defName)
    {
        if (t != null && t.apparel.bodyPartGroups != null)
            foreach (Def bodyPartGroup in t.apparel.bodyPartGroups)
                if (bodyPartGroup.defName == defName)
                    return true;

        return false;
    }

    internal static bool HasBodyPartGroup(this ThingDef t, BodyPartGroupDef bodyPartGroupDef)
    {
        return t != null && t.apparel.bodyPartGroups != null && t.apparel.bodyPartGroups.Contains(bodyPartGroupDef);
    }

    internal static void AddBodyPart(this ThingDef t, BodyPartGroupDef bodyPartGroupDef)
    {
        if (t == null || bodyPartGroupDef == null)
            return;
        if (t.apparel.bodyPartGroups == null)
            t.apparel.bodyPartGroups = new List<BodyPartGroupDef>();
        if (!t.HasBodyPartGroup(bodyPartGroupDef))
            t.apparel.bodyPartGroups.Add(bodyPartGroupDef);
        t.ResolveReferences();
    }

    internal static void PasteApparelTag(this ThingDef t, List<string> l)
    {
        if (t == null || l == null)
            return;
        if (t.apparel.tags == null)
            t.apparel.tags = new List<string>();
        foreach (var tag in l)
            t.AddApparelTag(tag);
    }

    internal static bool HasApparelTag(this ThingDef t, string tag)
    {
        return t != null && t.apparel.tags != null && t.apparel.tags.Contains(tag);
    }

    internal static void AddApparelTag(this ThingDef t, string tag)
    {
        if (t == null || tag == null)
            return;
        if (t.apparel.tags == null)
            t.apparel.tags = new List<string>();
        if (!t.HasApparelTag(tag))
            t.apparel.tags.Add(tag);
        t.ResolveReferences();
    }

    internal static void RemoveApparelTag(this ThingDef t, string tag)
    {
        if (t == null || t.apparel.tags == null)
            return;
        foreach (var tag1 in t.apparel.tags)
            if (tag1 == tag)
            {
                t.apparel.tags.Remove(tag1);
                break;
            }

        if (t.apparel.tags.Count == 0)
            t.apparel.tags = null;
        t.ResolveReferences();
    }

    internal static void PasteOutfitTag(this ThingDef t, List<string> l)
    {
        if (t == null || l == null)
            return;
        if (t.apparel.defaultOutfitTags == null)
            t.apparel.defaultOutfitTags = new List<string>();
        foreach (var tag in l)
            t.AddOutfitTag(tag);
    }

    internal static bool HasOutfitTag(this ThingDef t, string tag)
    {
        return t != null && t.apparel.defaultOutfitTags != null && t.apparel.defaultOutfitTags.Contains(tag);
    }

    internal static void AddOutfitTag(this ThingDef t, string tag)
    {
        if (t == null || tag == null)
            return;
        if (t.apparel.defaultOutfitTags == null)
            t.apparel.defaultOutfitTags = new List<string>();
        if (!t.HasOutfitTag(tag))
            t.apparel.defaultOutfitTags.Add(tag);
        t.ResolveReferences();
    }

    internal static void RemoveOutfitTag(this ThingDef t, string tag)
    {
        if (t == null || t.apparel.defaultOutfitTags == null)
            return;
        foreach (var defaultOutfitTag in t.apparel.defaultOutfitTags)
            if (defaultOutfitTag == tag)
            {
                t.apparel.defaultOutfitTags.Remove(defaultOutfitTag);
                break;
            }

        if (t.apparel.defaultOutfitTags.Count == 0)
            t.apparel.defaultOutfitTags = null;
        t.ResolveReferences();
    }

    internal static bool RenderAsPack(Apparel apparel)
    {
        return apparel.def.apparel.LastLayer == ApparelLayerDefOf.Belt && (apparel.def.apparel.wornGraphicData == null || apparel.def.apparel.wornGraphicData.renderUtilityAsPack);
    }

    internal static void FixForBelts(ThingDef t, StatDef stat)
    {
        if (t == null || t.apparel == null || (stat != StatDefOf.EnergyShieldEnergyMax && stat != StatDefOf.EnergyShieldRechargeRate))
            return;
        var mt = t.apparel.layers.Contains(ApparelLayerDefOf.Belt) ? MessageTypeDefOf.SilentInput : MessageTypeDefOf.RejectInput;
        MessageTool.Show(Label.ONLYFORSHIELD, mt);
        t.ResolveReferences();
        t.PostLoad();
    }

    internal static List<ApparelLayerDef> ListOfRandomApparelLayerDefs(
        int numberOfLayers = -1)
    {
        var apparelLayerDefList = ListOfApparelLayerDefs(false);
        numberOfLayers = numberOfLayers < 0 ? CEditor.zufallswert.Next(2, apparelLayerDefList.Count) : numberOfLayers;
        numberOfLayers = numberOfLayers > 8 ? 8 : numberOfLayers;
        return apparelLayerDefList.TakeRandom(numberOfLayers).ToList();
    }

    internal static void Redress(
        this Pawn pawn,
        Selected selected,
        bool originalColors,
        int numberOfLayers = 1,
        bool pawnSpecific = false)
    {
        if (!pawn.HasApparelTracker())
            return;
        if (selected == null)
        {
            pawn.DestroyAllApparel();
            var apparelLayerDefList = ListOfRandomApparelLayerDefs(numberOfLayers);
            if (pawnSpecific)
            {
                if (!apparelLayerDefList.Contains(ApparelLayerDefOf.OnSkin))
                    apparelLayerDefList.Add(ApparelLayerDefOf.OnSkin);
                if (!apparelLayerDefList.Contains(ApparelLayerDefOf.Shell))
                    apparelLayerDefList.Add(ApparelLayerDefOf.Shell);
            }

            Event.current.alt = originalColors;
            foreach (var ald in apparelLayerDefList)
                pawn.ReplaceAndWearRandomApparel(null, ald, pawnSpecific);
            Event.current.alt = false;
        }
        else
        {
            pawn.CreateAndWearApparel(selected);
        }
    }

    internal static void AllowAllApparel(this Pawn pawn, Apparel apparel = null)
    {
        if (pawn == null)
            return;
        if (pawn.outfits == null)
            pawn.outfits = new Pawn_OutfitTracker(pawn);
        if (pawn.outfits.CurrentApparelPolicy == null)
            pawn.outfits.CurrentApparelPolicy = new ApparelPolicy();
        if (pawn.apparel == null)
            pawn.apparel = new Pawn_ApparelTracker(pawn);
        if (apparel != null)
        {
            (pawn.outfits.CurrentApparelPolicy.filter).SetAllow(apparel.def, true);
        }
        else
        {
            if (pawn.apparel.WornApparel.NullOrEmpty())
                return;
            foreach (var apparel1 in pawn.apparel.WornApparel)
                (pawn.outfits.CurrentApparelPolicy.filter).SetAllow(apparel1.def, true);
        }
    }

    internal static Dictionary<ApparelLayerDef, HashSet<ThingDef>> CreateDicOfApparel()
    {
        var dictionary = new Dictionary<ApparelLayerDef, HashSet<ThingDef>>();
        foreach (var ofApparelLayerDef in ListOfApparelLayerDefs(false))
        {
            var thingDefSet = ListOfApparel(null, ofApparelLayerDef, null);
            dictionary.Add(ofApparelLayerDef, thingDefSet);
        }

        return dictionary;
    }

    internal static HashSet<string> CreateListOfGraphicPaths()
    {
        return ((IEnumerable<KeyValuePair<GraphicRequest, Graphic>>)typeof(GraphicDatabase).GetMemberValue("allGraphics")).Select(td => td.Value.path).ToHashSet();
    }

    internal static List<ThingDef> GetAllOnSkn()
    {
        return DefDatabase<ThingDef>.AllDefs.Where(td => td.IsApparel && td.apparel.LastLayer == ApparelLayerDefOf.OnSkin).OrderBy(td => td.label).ToList();
    }

    internal static List<ThingDef> GetAllBelt()
    {
        return DefDatabase<ThingDef>.AllDefs.Where(td => td.IsApparel && td.apparel.LastLayer == ApparelLayerDefOf.Belt).OrderBy(td => td.label).ToList();
    }

    internal static List<ThingDef> GetAllMiddle()
    {
        return DefDatabase<ThingDef>.AllDefs.Where(td => td.IsApparel && td.apparel.LastLayer == ApparelLayerDefOf.Middle).OrderBy(td => td.label).ToList();
    }

    internal static List<ThingDef> GetAllOverhead()
    {
        return DefDatabase<ThingDef>.AllDefs.Where(td => td.IsApparel && td.apparel.LastLayer == ApparelLayerDefOf.Overhead).OrderBy(td => td.label).ToList();
    }

    internal static List<ThingDef> GetAllShell()
    {
        return DefDatabase<ThingDef>.AllDefs.Where(td => td.IsApparel && td.apparel.LastLayer == ApparelLayerDefOf.Shell).OrderBy(td => td.label).ToList();
    }

    internal static List<ApparelLayerDef> ListOfApparelLayerDefs(bool insertNull)
    {
        var list = DefDatabase<ApparelLayerDef>.AllDefs.Where(td => td.IsFromCoreMod()).OrderBy(td => td.label).ToList();
        list.AddRange(DefDatabase<ApparelLayerDef>.AllDefs.Where(td => !td.IsFromCoreMod()).OrderBy(td => td.label).ToList());
        var apparelLayerDefList = new List<ApparelLayerDef>();
        foreach (var ld in list)
            if (!ListOfApparel(null, ld, null).NullOrEmpty())
                apparelLayerDefList.Add(ld);

        if (insertNull)
            apparelLayerDefList.Insert(0, null);
        return apparelLayerDefList;
    }

    internal static HashSet<ThingDef> ListOfApparel(
        string modname,
        ApparelLayerDef ld,
        BodyPartGroupDef bpd)
    {
        var bAll1 = modname.NullOrEmpty();
        var bAll2 = ld == null;
        var bAll3 = bpd == null;
        return DefDatabase<ThingDef>.AllDefs.Where(td =>
        {
            if (!td.IsApparel || td.defName.NullOrEmpty() || td.label.NullOrEmpty() || (!bAll1 && !td.IsFromMod(modname)) || (!bAll2 && (td.apparel.layers.NullOrEmpty() || !td.apparel.layers.Contains(ld))))
                return false;
            if (bAll3)
                return true;
            return !td.apparel.bodyPartGroups.NullOrEmpty() && td.apparel.bodyPartGroups.Contains(bpd);
        }).OrderBy(td => td.label).ToHashSet();
    }

    internal static void MoveDressToInv(this Pawn p, ApparelLayerDef layerOnly)
    {
        if (!CEditor.API.Pawn.HasApparelTracker() || !CEditor.API.Pawn.HasInventoryTracker())
            return;
        var num = CEditor.API.Pawn.HasApparelTracker() ? CEditor.API.Pawn.apparel.WornApparel.CountAllowNull() : 0;
        if (num <= 0)
            return;
        for (var index = num - 1; index >= 0; --index)
        {
            Thing thing = CEditor.API.Pawn.apparel.WornApparel[index];
            if (thing.def.IsApparel && (layerOnly == null || thing.def.apparel.layers.Contains(layerOnly)))
                CEditor.API.Pawn.TransferToInventory(thing);
        }
    }

    internal static void MoveDressFromInv(this Pawn p, ApparelLayerDef layerOnly)
    {
        if (!CEditor.API.Pawn.HasApparelTracker() || !CEditor.API.Pawn.HasInventoryTracker())
            return;
        p.MoveDressToInv(layerOnly);
        var num = CEditor.API.Pawn.inventory.innerContainer.CountAllowNull();
        if (num <= 0)
            return;
        for (var index = num - 1; index >= 0; --index)
        {
            var thing = CEditor.API.Pawn.inventory.innerContainer[index];
            if (thing.def.IsApparel && ((layerOnly == null && CEditor.API.Pawn.apparel.CanWearWithoutDroppingAnything(thing.def)) || (layerOnly != null && thing.def.apparel.layers.Contains(layerOnly))))
            {
                CEditor.API.Pawn.TransferFromInventory(thing);
                if (layerOnly != null)
                    break;
            }
        }
    }

    internal static void AllowApparelToBeColorable()
    {
        if (!CEditor.API.GetO(OptionB.ADDCOMPCOLORABLE))
            return;
        try
        {
            Log.Message("allowing apparel to be colorable...");
            foreach (var thingDef in DefTool.ListAll<ThingDef>())
                if (thingDef != null && thingDef.defName != null && thingDef.IsApparel && thingDef.apparel.LastLayer != ApparelLayerDefOf.Belt)
                {
                    if (thingDef.colorGenerator == null)
                    {
                        var generatorOptions = new ColorGenerator_Options();
                        generatorOptions.options.Add(new ColorOption
                        {
                            weight = 10.0f,
                            only = Color.white
                        });
                        thingDef.colorGenerator = generatorOptions;
                        thingDef.apparel.useWornGraphicMask = false;
                    }

                    if (!thingDef.HasComp(typeof(CompColorable)))
                        thingDef.AddCompColorable();
                    thingDef.ResolveReferences();
                    thingDef.PostLoad();
                }
        }
        catch (Exception ex)
        {
            Log.Error(ex.StackTrace);
        }
    }

    internal static void MakeThingColorable(ThingDef t)
    {
        if (t.colorGenerator == null)
        {
            var generatorOptions = new ColorGenerator_Options();
            generatorOptions.options.Add(new ColorOption
            {
                weight = 10.0f,
                only = Color.white
            });
            t.colorGenerator = generatorOptions;
            if (t.apparel != null)
                t.apparel.useWornGraphicMask = false;
        }

        if (!t.HasComp(typeof(CompColorable)))
            t.AddCompColorable();
        t.ResolveReferences();
        t.PostLoad();
    }

    internal static void MakeThingwColorable(ThingWithComps t)
    {
        MakeThingColorable(t.def);
        InitializeCompColorable(t);
    }

    private static void InitializeCompColorable(ThingWithComps t)
    {
        try
        {
            var instance = (ThingComp)Activator.CreateInstance(typeof(CompColorable));
            instance.parent = t;
            t.GetMemberValue("comps", (List<ThingComp>)null)?.Add(instance);
            var compProperties = t.def.comps.First(x => x.compClass == typeof(CompColorable));
            instance.Initialize(compProperties);
        }
        catch (Exception ex)
        {
            Log.Error("Could not instantiate or initialize a ThingComp: " + ex);
        }
    }
}
