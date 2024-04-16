// Decompiled with JetBrains decompiler
// Type: CharacterEditor.ThingTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

namespace CharacterEditor;

internal static class ThingTool
{
    internal static List<StatModifier> lCopyStatFactors = new();
    internal static List<StatModifier> lCopyStatOffsets = new();
    internal static List<string> lCopyTradeTags = new();
    internal static List<string> lCopyWeaponTags = new();
    internal static List<string> lCopyApparelTags = new();
    internal static List<string> lCopyOutfitTags = new();
    internal static List<StuffCategoryDef> lCopyStuffCategories = new();
    internal static List<WeaponTraitDef> lCopyBladeLinkTraits = new();
    internal static List<ThingDefCountClass> lCopyCosts = new();
    internal static List<ThingDefCountClass> lCopyCostsDiff = new();
    internal static List<ResearchProjectDef> lCopyPrerequisites = new();
    internal static List<ApparelLayerDef> lCopyApparelLayer = new();
    internal static List<BodyPartGroupDef> lCopyBodyPartGroup = new();
    private static bool bAllCategories;
    private static StatCategoryDef selected_StatFactor_CatDef;
    private static StatCategoryDef selected_StatOffset_CatDef;
    internal static HashSet<StatCategoryDef> lCategoryDef_Factors;
    internal static HashSet<StatCategoryDef> lCategoryDef_Offsets;
    internal static HashSet<StatDef> lFreeStatDefFactors;
    internal static HashSet<StatDef> lFreeStatDefOffsets;
    internal static HashSet<StuffCategoryDef> lFreeStuffCategories;
    internal static HashSet<ThingDef> lFreeCosts;
    internal static HashSet<ThingDef> lFreeCostsDiff;
    internal static HashSet<ResearchProjectDef> lFreePrerequisites;
    internal static HashSet<ApparelLayerDef> lFreeApparelLayer;
    internal static HashSet<BodyPartGroupDef> lFreeBodyPartGroup;

    internal static HashSet<Tradeability> AllTradeabilities => CEditor.API.ListOf<Tradeability>(EType.Tradeability).ToHashSet();

    internal static HashSet<TechLevel> AllTechLevels => CEditor.API.ListOf<TechLevel>(EType.TechLevel).ToHashSet();

    internal static HashSet<QualityCategory> AllQualityCategory => CEditor.API.Get<HashSet<QualityCategory>>(EType.QualityCategory);

    internal static HashSet<WeaponTraitDef> AllWeaponTraitDef => CEditor.API.Get<HashSet<WeaponTraitDef>>(EType.WeaponTraitDef);

    internal static HashSet<ResearchProjectDef> AllResearchProjectDef => CEditor.API.Get<HashSet<ResearchProjectDef>>(EType.ResearchProjectDef);

    internal static HashSet<GasType?> AllGasTypes => CEditor.API.ListOf<GasType?>(EType.GasType).ToHashSet();

    internal static HashSet<ThingDef> AllBullets => CEditor.API.Get<HashSet<ThingDef>>(EType.Bullet);

    internal static HashSet<ThingCategoryDef> AllThingCategoryDef => CEditor.API.Get<HashSet<ThingCategoryDef>>(EType.ThingCategoryDef);

    internal static HashSet<ThingCategory> AllThingCategory => CEditor.API.Get<HashSet<ThingCategory>>(EType.ThingCategory);

    internal static HashSet<ApparelLayerDef> AllApparelLayerDef => CEditor.API.Get<HashSet<ApparelLayerDef>>(EType.ApparelLayerDef);

    internal static HashSet<BodyPartGroupDef> AllBodyPartGroupDef => CEditor.API.Get<HashSet<BodyPartGroupDef>>(EType.BodyPartGroupDef);

    internal static HashSet<WeaponType> AllWeaponType => CEditor.API.Get<HashSet<WeaponType>>(EType.WeaponType);

    internal static HashSet<SoundDef> AllExplosionSounds => CEditor.API.Get<HashSet<SoundDef>>(EType.ExplosionSound);

    internal static HashSet<EffecterDef> AllEffecterDefs => CEditor.API.Get<HashSet<EffecterDef>>(EType.EffecterDef);

    internal static HashSet<DamageDef> AllDamageDefs => CEditor.API.Get<HashSet<DamageDef>>(EType.DamageDef);

    internal static HashSet<SoundDef> AllGunRelatedSounds => CEditor.API.Get<HashSet<SoundDef>>(EType.GunRelatedSound);

    internal static HashSet<SoundDef> AllGunShotSounds => CEditor.API.Get<HashSet<SoundDef>>(EType.GunShotSound);

    internal static bool UseAllCategories
    {
        get => bAllCategories;
        set
        {
            bAllCategories = value;
            SelectedThing.thingDef.UpdateFreeLists(FreeList.All);
        }
    }

    internal static StatCategoryDef StatFactorCategory
    {
        get => selected_StatFactor_CatDef;
        set
        {
            selected_StatFactor_CatDef = value;
            SelectedThing.thingDef.UpdateFreeLists(FreeList.CategoryFactors);
            SelectedThing.thingDef.UpdateFreeLists(FreeList.StatFactors);
        }
    }

    internal static StatCategoryDef StatOffsetCategory
    {
        get => selected_StatOffset_CatDef;
        set
        {
            selected_StatOffset_CatDef = value;
            SelectedThing.thingDef.UpdateFreeLists(FreeList.CategoryOffsets);
            SelectedThing.thingDef.UpdateFreeLists(FreeList.StatOffsets);
        }
    }

    internal static Selected SelectedThing { get; set; }

    internal static HashSet<SoundDef> GetExplosionSounds()
    {
        var soundDefSet = new HashSet<SoundDef>();
        soundDefSet.Add(null);
        foreach (var allDef in DefDatabase<ThingDef>.AllDefs)
            if (allDef.Verbs.Count > 0 && allDef.Verbs[0].defaultProjectile != null && allDef.Verbs[0].defaultProjectile.projectile.soundExplode != null)
                soundDefSet.Add(allDef.Verbs[0].defaultProjectile.projectile.soundExplode);
        return soundDefSet;
    }

    internal static HashSet<SoundDef> GetGunRelatedSounds()
    {
        var l = new HashSet<SoundDef>();
        l.Add(null);
        foreach (var allDef in DefDatabase<ThingDef>.AllDefs)
            GrabAllWeaponSounds(allDef, ref l);
        return l.OrderBy(x => x == null ? "" : x.defName).ToHashSet();
    }

    internal static HashSet<SoundDef> GetGunShotSounds()
    {
        var soundDefSet = new HashSet<SoundDef>();
        soundDefSet.Add(null);
        foreach (var allDef in DefDatabase<ThingDef>.AllDefs)
            if (allDef.HasVerb())
                soundDefSet.Add(allDef.Verbs[0].soundCast);
        return soundDefSet.OrderBy(x => x == null ? "" : x.defName).ToHashSet();
    }

    internal static void GrabAllWeaponSounds(ThingDef gun, ref HashSet<SoundDef> l)
    {
        if (gun.Verbs.Count <= 0)
            return;
        if (gun.Verbs[0].defaultProjectile != null)
        {
            if (gun.Verbs[0].defaultProjectile.projectile.soundExplode != null)
                l.Add(gun.Verbs[0].defaultProjectile.projectile.soundExplode);
            if (gun.Verbs[0].defaultProjectile.projectile.soundAmbient != null)
                l.Add(gun.Verbs[0].defaultProjectile.projectile.soundAmbient);
            if (gun.Verbs[0].defaultProjectile.projectile.soundHitThickRoof != null)
                l.Add(gun.Verbs[0].defaultProjectile.projectile.soundHitThickRoof);
            if (gun.Verbs[0].defaultProjectile.projectile.soundImpact != null)
                l.Add(gun.Verbs[0].defaultProjectile.projectile.soundImpact);
            if (gun.Verbs[0].defaultProjectile.projectile.soundImpactAnticipate != null)
                l.Add(gun.Verbs[0].defaultProjectile.projectile.soundImpactAnticipate);
        }

        if (gun.Verbs[0].soundAiming != null)
            l.Add(gun.Verbs[0].soundAiming);
        if (gun.Verbs[0].soundCast != null)
            l.Add(gun.Verbs[0].soundCast);
        if (gun.Verbs[0].soundCastBeam != null)
            l.Add(gun.Verbs[0].soundCastBeam);
        if (gun.Verbs[0].soundCastTail != null)
            l.Add(gun.Verbs[0].soundCastTail);
        if (gun.Verbs[0].soundLanding == null)
            return;
        l.Add(gun.Verbs[0].soundLanding);
    }

    internal static bool HasAnyItem(this Pawn pawn)
    {
        return pawn.HasInventoryTracker() && !pawn.inventory.innerContainer.NullOrEmpty();
    }

    internal static Thing RandomInventoryItem(this Pawn pawn)
    {
        return pawn.inventory.innerContainer.RandomElement();
    }

    internal static void Spawn(this Thing t, Rot4 rot4 = default(Rot4), IntVec3 pos = default(IntVec3))
    {
        if (t != null)
        {
            if (pos == default(IntVec3))
            {
                pos = UI.MouseCell();
            }
            if (!pos.InBounds(Find.CurrentMap))
            {
                pos = Find.CurrentMap.AllCells.RandomElement<IntVec3>();
            }
            GenSpawn.Spawn(t, pos, Find.CurrentMap, rot4, WipeMode.Vanish, false, false);
        }
    }
    internal static string GetAsSeparatedString(this Thing t)
    {
        string result;
        if (t == null || t.def.IsNullOrEmpty())
        {
            result = "";
        }
        else
        {
            string text = "";
            text = text + t.def.defName + "|";
            text = text + t.GetStuffDefName() + "|";
            text = text + t.DrawColor.ColorHexString() + "|";
            text = text + t.GetQuality().ToString() + "|";
            text = text + t.stackCount.ToString() + "|";
            text += t.HitPoints.ToString();
            if (t.StyleDef != null)
            {
                text += "|";
                text += t.StyleDef.defName;
            }
            result = text;
        }
        return result;
    }
    internal static string GetAllItemsAsSeparatedString(this Pawn p)
    {
        string str;
        if ((!p.HasInventoryTracker() ? 1 : p.inventory.innerContainer.NullOrEmpty() ? 1 : 0) != 0)
        {
            str = "";
        }
        else
        {
            var text = "";
            foreach (var t in p.inventory.innerContainer)
            {
                text += t.GetAsSeparatedString();
                text += ":";
            }

            str = text.SubstringRemoveLast();
        }

        return str;
    }

    internal static void SetItemsFromSeparatedString(this Pawn p, string s)
    {
        if (!p.HasInventoryTracker())
            return;
        p.inventory.DestroyAll();
        if (s.NullOrEmpty())
            return;
        foreach (var s1 in s.SplitNo(":"))
        {
            var strArray = s1.SplitNo("|");
            if (strArray.Length >= 6)
            {
                var styledefname = strArray.Length >= 7 ? strArray[6] : "";
                var thing = GenerateItem(Selected.ByName(strArray[0], strArray[1], styledefname, strArray[2].HexStringToColor(), strArray[3].AsInt32(), strArray[4].AsInt32()));
                if (thing != null)
                {
                    thing.HitPoints = strArray[5].AsInt32();
                    p.inventory.innerContainer.TryAdd(thing);
                }
            }
        }
    }

    internal static void DestroyAllItems(this Pawn pawn)
    {
        if (!pawn.HasInventoryTracker())
            return;
        pawn.inventory.innerContainer.ClearAndDestroyContents();
    }

    internal static void DestroyItem(this Pawn pawn, Thing t)
    {
        if ((!pawn.HasInventoryTracker() ? 0 : t != null ? 1 : 0) == 0)
            return;
        pawn.inventory.innerContainer.Remove(t);
        if ((!t.def.IsApparel ? 0 : pawn.HasApparelTracker() ? 1 : 0) != 0)
            pawn.outfits.forcedHandler.ForcedApparel.Remove(t as Apparel);
        t.Destroy();
    }

    internal static List<Thing> ListOfCopyItems(this Pawn pawn)
    {
        return !pawn.HasAnyItem() ? null : pawn.inventory.innerContainer.InnerListForReading.ListFullCopy();
    }

    internal static void PasteCopyItems(this Pawn pawn, List<Thing> l)
    {
        if (pawn.HasInventoryTracker())
        {
            pawn.DestroyAllItems();
            if (!l.NullOrEmpty())
                foreach (var _thing in l)
                {
                    var t = GenerateItem(Selected.ByThing(_thing));
                    pawn.AddItemToInventory(t);
                }
        }

        CEditor.API.UpdateGraphics();
    }

    internal static void AddItemToInventory(this Pawn pawn, Thing t)
    {
        if ((!pawn.HasInventoryTracker() ? 0 : t != null ? 1 : 0) == 0)
            return;
        pawn.inventory.innerContainer.TryAdd(t);
    }

    internal static void TransferToInventory(this Pawn pawn, Thing t)
    {
        if ((!pawn.HasInventoryTracker() ? 0 : t != null ? 1 : 0) == 0)
            return;
        if ((!pawn.HasApparelTracker() || !pawn.HasForcedApparel() ? 0 : t.def.IsApparel ? 1 : 0) != 0)
            pawn.outfits.forcedHandler.ForcedApparel.Remove(t as Apparel);
        if (pawn.inventory.innerContainer == null)
            pawn.inventory.innerContainer = new ThingOwner<Thing>();
        pawn.inventory.innerContainer.TryAddOrTransfer(t);
    }

    internal static void TransferFromInventory(this Pawn pawn, Thing t)
    {
        if ((!pawn.HasInventoryTracker() ? 0 : t != null ? 1 : 0) == 0)
            return;
        if (pawn.inventory.innerContainer == null)
            pawn.inventory.innerContainer = new ThingOwner<Thing>();
        if ((!pawn.HasApparelTracker() ? 0 : t.def.IsApparel ? 1 : 0) != 0)
        {
            pawn.inventory.innerContainer.Remove(t);
            pawn.WearThatApparel(t as Apparel);
        }
        else
        {
            if ((!t.def.IsWeapon ? 0 : pawn.HasEquipmentTracker() ? 1 : 0) == 0)
                return;
            pawn.inventory.innerContainer.Remove(t);
            pawn.AddWeaponToEquipment(t as ThingWithComps, true, false);
        }
    }

    internal static void CreateAndAddItem(this Pawn pawn, Selected s)
    {
        if (!pawn.HasInventoryTracker())
            return;
        var t = GenerateItem(s);
        pawn.AddItemToInventory(t);
    }

    internal static Thing GenerateItem(Selected s)
    {
        Thing thing;
        if ((s == null ? 1 : s.thingDef == null ? 1 : 0) != 0)
        {
            thing = null;
        }
        else if (DefTool.ThingDef(s.thingDef.defName) == null)
        {
            thing = null;
        }
        else
        {
            s.stuff = s.thingDef.ThisOrDefaultStuff(s.stuff);
            if (!s.thingDef.MadeFromStuff)
                s.stuff = null;
            var t = ThingMaker.MakeThing(s.thingDef, s.stuff);
            t.HitPoints = t.MaxHitPoints;
            t.SetQuality(s.quality);
            t.SetDrawColor(s.DrawColor);
            t.stackCount = s.stackVal;
            if (s.style != null)
            {
                t.StyleDef = s.style;
                t.StyleDef.color = s.style.color;
            }

            thing = t;
        }

        return thing;
    }

    internal static Thing GenerateRandomItem(
        ThingCategoryDef tc,
        bool originalColors = true,
        bool randomStack = true)
    {
        return GenerateItem(Selected.Random(ListOfItems(null, tc, ThingCategory.None), originalColors, randomStack));
    }

    internal static bool IsChunk(this ThingDef td)
    {
        if (td == null || td.thingCategories.NullOrEmpty())
            return false;
        return td.thingCategories.Contains(ThingCategoryDefOf.StoneChunks) || td.thingCategories.Contains(ThingCategoryDefOf.Chunks);
    }

    internal static bool IsMineableMineral(this ThingDef td)
    {
        return td != null && td.defName != null && td.building != null && td.building.mineableThing != null && td.building.isResourceRock;
    }

    internal static bool IsMineableRock(this ThingDef td)
    {
        return td != null && td.defName != null && td.building != null && td.building.mineableThing != null && !td.building.isResourceRock;
    }

    internal static ThingDef GetBaseThingDefFromMinified(this ThingDef miniThingDef)
    {
        ThingDef thingDef = null;
        if ((miniThingDef == null || miniThingDef.defName == null ? 0 : miniThingDef.defName.Contains("Minified") ? 1 : 0) != 0)
            thingDef = DefTool.ThingDef(miniThingDef.defName.Replace("Minified", ""));
        return thingDef;
    }

    internal static bool IsMinified(this ThingDef t)
    {
        return t != null && t.defName != null && t.defName.Contains("Minified");
    }

    internal static List<StatCategoryDef> GetAllStatCategoriesApparel()
    {
        var lequip = GetAllStatCategoriesOnEquip();
        var list = DefDatabase<StatCategoryDef>.AllDefs.Where(td => td != StatCategoryDefOf.Building && !lequip.Contains(td)).OrderBy(td => td.label).ToList();
        list.Remove(StatCategoryDefOf.Weapon);
        list.RemoveCategoriesWithoutStats();
        list.Insert(0, null);
        return list;
    }

    internal static List<StatCategoryDef> GetAllStatCategoriesOnEquip()
    {
        var statCategoryDefList = new List<StatCategoryDef>();
        var l = new List<StatCategoryDef>();
        l.Add(StatCategoryDefOf.BasicsPawn);
        l.Add(StatCategoryDefOf.EquippedStatOffsets);
        l.Add(StatCategoryDefOf.PawnCombat);
        l.Add(StatCategoryDefOf.PawnMisc);
        l.Add(StatCategoryDefOf.PawnSocial);
        l.Add(StatCategoryDefOf.PawnWork);
        l.RemoveCategoriesWithoutStats();
        l.Insert(0, null);
        return l;
    }

    internal static List<StatCategoryDef> GetAllStatCategoriesWeapon()
    {
        var lequip = GetAllStatCategoriesOnEquip();
        var list = DefDatabase<StatCategoryDef>.AllDefs.Where(td => td != StatCategoryDefOf.Building && !lequip.Contains(td)).OrderBy(td => td.label).ToList();
        list.Remove(StatCategoryDefOf.Apparel);
        list.RemoveCategoriesWithoutStats();
        list.Insert(0, null);
        return list;
    }

    internal static bool HasStatsForCategory(StatCategoryDef s)
    {
        foreach (var allDef in DefDatabase<StatDef>.AllDefs)
            if (allDef.category == s)
                return true;
        return false;
    }

    internal static void RemoveCategoriesWithoutStats(this List<StatCategoryDef> l)
    {
        if (l == null)
            return;
        for (var index = 0; index < l.Count; ++index)
            if (!HasStatsForCategory(l[index]))
            {
                l.RemoveAt(index);
                --index;
            }
    }

    internal static void RemoveCategoriesWithoutThings(this List<ThingCategoryDef> l)
    {
        if (l == null)
            return;
        var dictionary = new Dictionary<ThingCategoryDef, int>();
        foreach (var key in l)
            dictionary.Add(key, 0);
        foreach (var allDef in DefDatabase<ThingDef>.AllDefs)
            if (!allDef.thingCategories.NullOrEmpty())
                foreach (var thingCategory in allDef.thingCategories)
                    if (dictionary.ContainsKey(thingCategory))
                        dictionary[thingCategory]++;
        foreach (var key in dictionary.Keys)
            if (dictionary[key] == 0)
                l.Remove(key);
    }

    internal static void AddEquipStat(this ThingDef t, StatDef stat, float value)
    {
        if ((t == null ? 0 : stat != null ? 1 : 0) == 0)
            return;
        if (t.HasEquipStat(stat))
        {
            t.UpdateEquipStat(stat, value);
        }
        else
        {
            var statModifier = new StatModifier();
            statModifier.stat = stat;
            statModifier.value = value;
            if (t.equippedStatOffsets == null)
                t.equippedStatOffsets = new List<StatModifier>();
            t.equippedStatOffsets.Add(statModifier);
        }

        t.ResolveReferences();
    }

    internal static void AddStat(this ThingDef t, StatDef stat, float value)
    {
        if ((t == null ? 0 : stat != null ? 1 : 0) == 0)
            return;
        if (t.HasStat(stat))
        {
            t.UpdateStat(stat, value);
        }
        else
        {
            var statModifier = new StatModifier();
            statModifier.stat = stat;
            statModifier.value = value;
            if (t.statBases == null)
                t.statBases = new List<StatModifier>();
            t.statBases.Add(statModifier);
        }

        t.ResolveReferences();
    }

    internal static List<StatDef> GetAllApparelStatDefs()
    {
        var lequip = GetAllStatCategoriesOnEquip();
        return DefDatabase<StatDef>.AllDefs.Where(td => !lequip.Contains(td.category) || td.category == StatCategoryDefOf.Apparel).OrderBy(td => td.label).ToList();
    }

    internal static List<StatDef> GetAllOnEquipStatDefs()
    {
        var lequip = GetAllStatCategoriesOnEquip();
        return DefDatabase<StatDef>.AllDefs.Where(td => lequip.Contains(td.category)).OrderBy(td => td.label).ToList();
    }

    internal static List<StatDef> GetAllWeaponStatDefs()
    {
        var lequip = GetAllStatCategoriesOnEquip();
        return DefDatabase<StatDef>.AllDefs.Where(td => !lequip.Contains(td.category) || td.category == StatCategoryDefOf.Weapon).OrderBy(td => td.label).ToList();
    }

    internal static float GetEquipStatValue(this ThingDef t, StatDef stat)
    {
        if ((t == null ? 0 : t.equippedStatOffsets != null ? 1 : 0) != 0)
            foreach (var equippedStatOffset in t.equippedStatOffsets)
                if (equippedStatOffset.stat == stat)
                    return equippedStatOffset.value;
        return 0.0f;
    }

    internal static float GetStatValue(this ThingDef t, StatDef stat)
    {
        if ((t == null ? 0 : t.statBases != null ? 1 : 0) != 0)
            foreach (var statBase in t.statBases)
                if (statBase.stat == stat)
                    return statBase.value;
        return 0.0f;
    }

    internal static bool HasEquipStat(this ThingDef t, StatDef stat)
    {
        if ((t == null ? 0 : t.equippedStatOffsets != null ? 1 : 0) != 0)
            foreach (var equippedStatOffset in t.equippedStatOffsets)
                if (equippedStatOffset.stat == stat)
                    return true;
        return false;
    }

    internal static bool HasStat(this ThingDef t, StatDef stat)
    {
        if ((t == null ? 0 : t.statBases != null ? 1 : 0) != 0)
            foreach (var statBase in t.statBases)
                if (statBase.stat == stat)
                    return true;
        return false;
    }

    internal static void PasteEquipStats(this ThingDef t, List<StatModifier> l)
    {
        if ((t == null ? 0 : l != null ? 1 : 0) == 0)
            return;
        if (t.equippedStatOffsets == null)
            t.equippedStatOffsets = new List<StatModifier>();
        foreach (var statModifier in l)
            t.AddEquipStat(statModifier.stat, statModifier.value);
        t.ResolveReferences();
    }

    internal static void PasteStats(this ThingDef t, List<StatModifier> l)
    {
        if ((t == null ? 0 : l != null ? 1 : 0) == 0)
            return;
        if (t.statBases == null)
            t.statBases = new List<StatModifier>();
        foreach (var statModifier in l)
            t.AddStat(statModifier.stat, statModifier.value);
        t.ResolveReferences();
    }

    internal static void RemoveEquipStat(this ThingDef t, StatDef stat)
    {
        if ((t == null ? 0 : t.equippedStatOffsets != null ? 1 : 0) == 0)
            return;
        foreach (var equippedStatOffset in t.equippedStatOffsets)
            if (equippedStatOffset.stat == stat)
            {
                t.equippedStatOffsets.Remove(equippedStatOffset);
                break;
            }

        if (t.equippedStatOffsets.Count == 0)
            t.equippedStatOffsets = null;
        t.ResolveReferences();
    }

    internal static void RemoveStat(this ThingDef t, StatDef stat)
    {
        if ((t == null ? 0 : t.statBases != null ? 1 : 0) == 0)
            return;
        foreach (var statBase in t.statBases)
            if (statBase.stat == stat)
            {
                t.statBases.Remove(statBase);
                break;
            }

        if (t.statBases.Count == 0)
            t.statBases = null;
        t.ResolveReferences();
    }

    internal static void UpdateEquipStat(this ThingDef t, StatDef stat, float value)
    {
        if ((t == null ? 0 : t.equippedStatOffsets != null ? 1 : 0) == 0)
            return;
        if (t.HasEquipStat(stat))
        {
            foreach (var equippedStatOffset in t.equippedStatOffsets)
                if (equippedStatOffset.stat == stat)
                {
                    equippedStatOffset.value = value;
                    break;
                }
        }
        else
        {
            t.AddEquipStat(stat, value);
        }

        t.ResolveReferences();
    }

    internal static void UpdateStat(this ThingDef t, StatDef stat, float value)
    {
        if ((t == null ? 0 : t.statBases != null ? 1 : 0) == 0)
            return;
        if (t.HasStat(stat))
        {
            foreach (var statBase in t.statBases)
                if (statBase.stat == stat)
                {
                    statBase.value = value;
                    break;
                }
        }
        else
        {
            t.AddStat(stat, value);
        }

        t.ResolveReferences();
    }

    internal static ThingDef RandomAllowedStuff(this ThingDef t)
    {
        return t == null || !t.MadeFromStuff || t.stuffCategories == null ? null : GenStuff.AllowedStuffsFor(t).ToList().RandomElement();
    }

    internal static ThingDef ThisOrDefaultStuff(this ThingDef t, ThingDef stuff)
    {
        return t == null || !t.MadeFromStuff || stuff != null ? stuff : GenStuff.DefaultStuffFor(t);
    }

    internal static ThingStyleDef RandomAllowedStyle(this ThingDef thingDef)
    {
        return ListOfThingStyleDefs(thingDef, null, true).RandomElement();
    }

    internal static string GetStuffDefName(this Thing t)
    {
        return t.Stuff == null ? "" : t.Stuff.defName;
    }

    internal static void AddStuff(this ThingDef t, StuffCategoryDef stuffcat)
    {
        if ((t == null ? 0 : stuffcat != null ? 1 : 0) == 0)
            return;
        if (t.stuffCategories == null)
            t.stuffCategories = new List<StuffCategoryDef>();
        if (!t.HasStuff(stuffcat))
            t.stuffCategories.Add(stuffcat);
        t.ResolveReferences();
    }

    internal static List<StuffCategoryDef> GetAllStuffCategories()
    {
        return DefDatabase<StuffCategoryDef>.AllDefs.OrderBy(td => td.label).ToList();
    }

    internal static bool HasStuff(this ThingDef t, StuffCategoryDef stuffcat)
    {
        return (t == null ? 0 : t.stuffCategories != null ? 1 : 0) != 0 && t.stuffCategories.Contains(stuffcat);
    }

    internal static void PasteStuff(this ThingDef t, List<StuffCategoryDef> l)
    {
        if ((t == null ? 0 : l != null ? 1 : 0) == 0)
            return;
        if (t.stuffCategories == null)
            t.stuffCategories = new List<StuffCategoryDef>();
        foreach (var stuffcat in l)
            t.AddStuff(stuffcat);
    }

    internal static void RemoveStuff(this ThingDef t, StuffCategoryDef stuffCat)
    {
        if ((t == null ? 0 : t.stuffCategories != null ? 1 : 0) == 0)
            return;
        foreach (var stuffCategory in t.stuffCategories)
            if (stuffCategory == stuffCat)
            {
                t.stuffCategories.Remove(stuffCategory);
                break;
            }

        if (t.stuffCategories.Count == 0)
            t.stuffCategories = null;
        t.ResolveReferences();
    }

    internal static ThingDef GetStuff(
        this ThingDef thingDef,
        ref HashSet<ThingDef> lOfStuff,
        ref int stuffIndex,
        bool random = false)
    {
        lOfStuff = new HashSet<ThingDef>();
        ThingDef thingDef1;
        if ((thingDef == null ? 1 : !thingDef.MadeFromStuff ? 1 : 0) != 0)
        {
            thingDef1 = null;
        }
        else
        {
            lOfStuff = GenStuff.AllowedStuffsFor(thingDef).OrderBy(d =>
            {
                var stuffProps = d.stuffProps;
                return stuffProps == null ? null : stuffProps.categories.FirstOrDefault().SDefname();
            }).ThenBy(t => t.label).ToHashSet();
            if (random)
                stuffIndex = CEditor.zufallswert.Next(lOfStuff.Count);
            if (!lOfStuff.NullOrEmpty())
            {
                if (lOfStuff.Count > stuffIndex)
                {
                    thingDef1 = lOfStuff.At(stuffIndex);
                }
                else
                {
                    stuffIndex = 0;
                    thingDef1 = lOfStuff.At(stuffIndex);
                }
            }
            else
            {
                thingDef1 = null;
            }
        }

        return thingDef1;
    }

    internal static bool IsStackable(this ThingDef def)
    {
        if (def == null)
            return false;
        return def.CountAsResource || def.IsChunk();
    }

    internal static void UpdateStackLimit(this ThingDef def)
    {
        if (!def.IsStackable())
            return;
        def.deepCountPerCell = def.stackLimit;
        if (def.CountAsResource)
            return;
        def.resourceReadoutAlwaysShow = def.deepCountPerCell > 1;
        def.resourceReadoutPriority = ResourceCountPriority.Middle;
        def.drawGUIOverlay = true;
        def.passability = Traversability.Standable;
        ResourceCounter.ResetDefs();
    }

    internal static void UpdateCostStuffCount(this ThingDef product)
    {
        if (product == null)
            return;
        foreach (var allDef in DefDatabase<RecipeDef>.AllDefs)
            if (allDef.ProducedThingDef == product && !allDef.ingredients.NullOrEmpty())
                allDef.ingredients[0].SetBaseCount(product.costStuffCount);
    }

    internal static void UpdateRecipes(this ThingDef product)
    {
        if (product == null)
            return;
        foreach (var allDef in DefDatabase<RecipeDef>.AllDefs)
            if (allDef.ProducedThingDef == product)
            {
                if (product.recipeMaker != null)
                    allDef.researchPrerequisite = product.recipeMaker.researchPrerequisite;
                if (allDef.fixedIngredientFilter != null)
                    allDef.fixedIngredientFilter.SetDisallowAll();
                if (!product.stuffCategories.NullOrEmpty())
                {
                    if (allDef.ingredients != null)
                        allDef.ingredients.Clear();
                    allDef.ClearCachedData();
                    var ingredientCount = new IngredientCount();
                    ingredientCount.filter.allowedHitPointsConfigurable = false;
                    ingredientCount.filter.allowedQualitiesConfigurable = false;
                    var list = GenStuff.AllowedStuffsFor(product).ToList();
                    if (!list.NullOrEmpty())
                    {
                        if (allDef.fixedIngredientFilter == null)
                            allDef.fixedIngredientFilter = new ThingFilter();
                        foreach (var thingDef in list)
                        {
                            ingredientCount.filter.AllowedThingDefs.AddItem(thingDef);
                            ingredientCount.filter.SetAllow(thingDef, true);
                            ingredientCount.filter.SetAllowAllWhoCanMake(thingDef);
                            ingredientCount.filter.Allows(thingDef);
                            allDef.fixedIngredientFilter.AllowedThingDefs.AddItem(thingDef);
                            allDef.fixedIngredientFilter.SetAllow(thingDef, true);
                            allDef.fixedIngredientFilter.SetAllowAllWhoCanMake(thingDef);
                            allDef.fixedIngredientFilter.Allows(thingDef);
                        }
                    }

                    ingredientCount.SetBaseCount(product.costStuffCount);
                    if (allDef.ingredients == null)
                        allDef.ingredients = new List<IngredientCount>();
                    allDef.ingredients.Add(ingredientCount);
                    ingredientCount.filter.ResolveReferences();
                    ingredientCount.ResolveReferences();
                }
                else if (!product.costList.NullOrEmpty())
                {
                    if (allDef.ingredients != null)
                        allDef.ingredients.Clear();
                    allDef.ClearCachedData();
                    if (allDef.ingredients == null)
                        allDef.ingredients = new List<IngredientCount>();
                    foreach (var cost in product.costList)
                    {
                        var ingredientCount = new IngredientCount();
                        ingredientCount.filter.AllowedThingDefs.AddItem(cost.thingDef);
                        ingredientCount.filter.SetAllow(cost.thingDef, true);
                        ingredientCount.filter.SetAllowAllWhoCanMake(cost.thingDef);
                        ingredientCount.filter.Allows(cost.thingDef);
                        ingredientCount.SetBaseCount(cost.count);
                        allDef.ingredients.Add(ingredientCount);
                        ingredientCount.filter.ResolveReferences();
                        ingredientCount.ResolveReferences();
                    }
                }
                else if ((product.costListForDifficulty == null ? 0 : !product.costListForDifficulty.costList.NullOrEmpty() ? 1 : 0) != 0)
                {
                    if (allDef.ingredients != null)
                        allDef.ingredients.Clear();
                    allDef.ClearCachedData();
                    if (allDef.ingredients == null)
                        allDef.ingredients = new List<IngredientCount>();
                    foreach (var cost in product.costListForDifficulty.costList)
                    {
                        var ingredientCount = new IngredientCount();
                        ingredientCount.filter.AllowedThingDefs.AddItem(cost.thingDef);
                        ingredientCount.filter.SetAllow(cost.thingDef, true);
                        ingredientCount.filter.SetAllowAllWhoCanMake(cost.thingDef);
                        ingredientCount.filter.Allows(cost.thingDef);
                        ingredientCount.SetBaseCount(cost.count);
                        allDef.ingredients.Add(ingredientCount);
                        ingredientCount.filter.ResolveReferences();
                        ingredientCount.ResolveReferences();
                    }
                }

                if (allDef.fixedIngredientFilter != null)
                {
                    allDef.fixedIngredientFilter.allowedHitPointsConfigurable = false;
                    allDef.fixedIngredientFilter.allowedQualitiesConfigurable = false;
                    allDef.fixedIngredientFilter.ResolveReferences();
                }

                allDef.ResolveReferences();
            }

        product.SetMemberValue("allRecipesCached", null);
        if ((product.recipeMaker == null ? 0 : !product.recipeMaker.recipeUsers.NullOrEmpty() ? 1 : 0) == 0)
            return;
        foreach (Editable recipeUser in product.recipeMaker.recipeUsers)
            recipeUser.ResolveReferences();
    }

    [Obsolete("do not use", false)]
    internal static void AddCost(this ThingDef t, ThingDef tcost, int value)
    {
        if (t == null)
            return;
        if (t.HasCost(tcost))
        {
            t.UpdateCost(tcost, value);
        }
        else
        {
            var thingDefCountClass = new ThingDefCountClass(tcost, value);
            if (t.costList == null)
                t.costList = new List<ThingDefCountClass>();
            t.costList.Add(thingDefCountClass);
        }

        t.UpdateRecipes();
        t.ResolveReferences();
    }

    internal static List<ThingDef> GetAllCostThingDefs()
    {
        return DefDatabase<ThingDef>.AllDefs.Where(td => !td.label.NullOrEmpty() && !td.IsWeapon && !td.IsApparel && td.CountAsResource && td.mineable == null && td.plant == null && !td.IsSmoothed && !td.IsDrug && !td.IsIngestible).OrderBy(td => td.label).ToList();
    }

    [Obsolete("do not use", false)]
    internal static bool HasCost(this ThingDef t, ThingDef tcost)
    {
        if ((t == null ? 0 : t.costList != null ? 1 : 0) != 0)
            foreach (var cost in t.costList)
                if (cost.thingDef == tcost)
                    return true;
        return false;
    }

    [Obsolete("do not use", false)]
    internal static void PasteCost(this ThingDef t, List<ThingDefCountClass> l)
    {
        if ((t == null ? 0 : l != null ? 1 : 0) == 0)
            return;
        if (t.costList == null)
            t.costList = new List<ThingDefCountClass>();
        foreach (var thingDefCountClass in l)
            t.costList.Add(new ThingDefCountClass(thingDefCountClass.thingDef, thingDefCountClass.count));
    }

    [Obsolete("do not use", false)]
    internal static void RemoveCost(this ThingDef t, ThingDef tcost)
    {
        if ((t == null ? 0 : t.costList != null ? 1 : 0) == 0)
            return;
        foreach (var cost in t.costList)
            if (cost.thingDef == tcost)
            {
                t.costList.Remove(cost);
                break;
            }

        if (t.costList.Count == 0)
            t.costList = null;
        t.UpdateRecipes();
        t.ResolveReferences();
    }

    [Obsolete("do not use", false)]
    internal static void UpdateCost(this ThingDef t, ThingDef tcost, int value)
    {
        if ((t == null ? 0 : t.costList != null ? 1 : 0) == 0)
            return;
        if (t.HasCost(tcost))
        {
            foreach (var cost in t.costList)
                if (cost.thingDef == tcost)
                    cost.count = value;
        }
        else
        {
            t.AddCost(tcost, value);
        }

        t.UpdateRecipes();
        t.ResolveReferences();
    }

    internal static void AddCompColorable(this ThingDef t)
    {
        if ((t == null ? 1 : t.HasComp(typeof(CompColorable)) ? 1 : 0) != 0)
            return;
        var compProperties = new CompProperties();
        compProperties.compClass = typeof(CompColorable);
        if (t.comps == null)
            t.comps = new List<CompProperties>();
        t.comps.Add(compProperties);
        compProperties.ResolveReferences(t);
        t.ResolveReferences();
        t.PostLoad();
    }

    internal static void AddCompExplosive(this ThingDef t)
    {
        if (t == null || WeaponTool.GetCompExplosive(t) != null)
            return;
        var propertiesExplosive = new CompProperties_Explosive();
        propertiesExplosive.compClass = typeof(CompProperties_Explosive);
        if (t.comps == null)
            t.comps = new List<CompProperties>();
        t.comps.Add(propertiesExplosive);
        propertiesExplosive.ResolveReferences(t);
        t.ResolveReferences();
        t.PostLoad();
    }

    internal static void RemoveCompExplosive(this ThingDef t)
    {
        if (t == null)
            return;
        var compExplosive = WeaponTool.GetCompExplosive(t);
        if (compExplosive == null)
            return;
        t.comps.Remove(compExplosive);
        t.ResolveReferences();
        t.PostLoad();
    }

    internal static Color GetColor(this ThingDef t, ThingDef stuff)
    {
        return t != null ? (stuff == null ? 0 : t.MadeFromStuff ? 1 : 0) == 0 ? (t.mineable == null ? 0 : t.graphicData != null ? 1 : 0) == 0 ? t.colorGenerator == null ? t.graphicData == null ? Color.white : t.graphicData.color : t.colorGenerator.ExemplaryColor : t.graphicData.colorTwo : stuff.stuffProps.color : Color.white;
    }

    internal static void SetDrawColor(this Thing t, Color col)
    {
        if (t == null)
            return;
        t.TryGetComp<CompColorable>()?.SetColor(col);
    }

    internal static void AddCompQuality(this ThingDef t)
    {
        if ((t == null ? 1 : t.HasComp(typeof(CompQuality)) ? 1 : 0) != 0)
            return;
        var compProperties = new CompProperties();
        compProperties.compClass = typeof(CompQuality);
        if (t.comps == null)
            t.comps = new List<CompProperties>();
        t.comps.Add(compProperties);
        compProperties.ResolveReferences(t);
        t.ResolveReferences();
        t.PostLoad();
    }

    internal static void CopyQuality(this Thing t1, Thing fromT2)
    {
        if ((t1 == null ? 1 : fromT2 == null ? 1 : 0) != 0 || !fromT2.TryGetQuality(out var qualityCategory))
            return;
        t1.SetQuality((int)qualityCategory);
    }

    internal static void SetQuality(this Thing t, int quali)
    {
        if (t == null)
            return;
        t.TryGetComp<CompQuality>()?.SetQuality((QualityCategory)(byte)quali, (ArtGenerationContext)1);
    }

    internal static int GetQuality(this Thing t)
    {
        return !t.TryGetQuality(out var qualityCategory) ? 0 : (int)qualityCategory;
    }

    internal static CompProperties GetCompByType(
        this ThingDef thingDef,
        string typeString)
    {
        if ((thingDef == null ? 0 : thingDef.comps != null ? 1 : 0) != 0)
            foreach (var comp in thingDef.comps)
                if (comp.GetType().ToString() == typeString)
                    return comp;
        return null;
    }

    internal static CompProperties GetCompByType(this ThingDef thingDef, Type type)
    {
        if ((thingDef == null ? 0 : thingDef.comps != null ? 1 : 0) != 0)
            foreach (var comp in thingDef.comps)
                if (comp.GetType() == type)
                    return comp;
        return null;
    }

    internal static void Reinvent(this Pawn pawn, Selected selected, int createRandomAmount = 1)
    {
        if (!pawn.HasInventoryTracker())
            return;
        if (selected == null)
        {
            try
            {
                pawn.inventory.DestroyAll();
            }
            catch
            {
            }

            for (var index = 0; index < createRandomAmount; ++index)
            {
                var randomItem = GenerateRandomItem(null);
                pawn.AddItemToInventory(randomItem);
            }
        }
        else
        {
            var t = GenerateItem(selected);
            pawn.AddItemToInventory(t);
        }
    }

    internal static void ReplaceItem(this Pawn pawn, Thing t)
    {
        if (!pawn.HasInventoryTracker())
            return;
        var randomItem = GenerateRandomItem(t.def.thingCategories.FirstOrDefault());
        pawn.inventory.innerContainer.Remove(t);
        pawn.AddItemToInventory(randomItem);
    }

    internal static int GetSilverAmountNear(IntVec3 pos, Map map)
    {
        var num1 = 0;
        int num2;
        if (map == null)
        {
            num2 = num1;
        }
        else
        {
            var x = map.Size.x;
            var z = map.Size.z;
            for (var index1 = 0; index1 < x; ++index1)
            for (var index2 = 0; index2 < z; ++index2)
                foreach (var thing in new IntVec3(index1, pos.y, index2).GetThingList(map))
                    if (thing?.def == ThingDefOf.Silver)
                    {
                        num1 += thing.stackCount;
                        break;
                    }

            num2 = num1;
        }

        return num2;
    }

    internal static void ReduceSilverAmount(IntVec3 pos, Map map, int buyPrice)
    {
        if (map == null)
            return;
        var x = map.Size.x;
        var z = map.Size.z;
        for (var index1 = 0; index1 < x; ++index1)
        {
            for (var index2 = 0; index2 < z; ++index2)
            {
                foreach (var thing1 in new IntVec3(index1, pos.y, index2).GetThingList(map))
                    if (thing1?.def == ThingDefOf.Silver)
                    {
                        if (thing1.stackCount > buyPrice)
                        {
                            var thing2 = thing1;
                            thing2.stackCount = thing2.stackCount - buyPrice;
                            buyPrice = 0;
                            break;
                        }

                        buyPrice -= thing1.stackCount;
                        thing1.Destroy();
                        break;
                    }

                if (buyPrice <= 0)
                    break;
            }

            if (buyPrice <= 0)
                break;
        }
    }

    internal static List<Selected> GrabAllThingsFromMap(Map map, bool doDestroy = true)
    {
        List<Selected> list = new List<Selected>();
        int x = map.Size.x;
        int z = map.Size.z;
        IntVec3 intVec = UI.MouseCell();
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++)
            {
                List<Thing> thingList = new IntVec3(i, intVec.y, j).GetThingList(map);
                if (!thingList.NullOrEmpty<Thing>())
                {
                    for (int k = thingList.Count - 1; k >= 0; k--)
                    {
                        Thing thing = thingList[k];
                        if (thing != null && thing.def != null && thing.def.category == Verse.ThingCategory.Item && thing.def.building == null)
                        {
                            list.Add(Selected.ByThing(thing));
                            if (doDestroy)
                            {
                                thing.Destroy(DestroyMode.Vanish);
                            }
                        }
                    }
                }
            }
        }
        return list;
    }

    internal static Thing FindThingOnMap(Selected s, Map map, int amount, bool doDestroy = false)
    {
        if (s.location != default(IntVec3))
        {
            List<Thing> thingList = s.location.GetThingList(map);
            if (!thingList.NullOrEmpty<Thing>())
            {
                for (int i = thingList.Count - 1; i >= 0; i--)
                {
                    Thing thing = thingList[i];
                    if (thing != null && thing.def != null && thing.def.defName == s.thingDef.defName && thing.stackCount == amount)
                    {
                        if (doDestroy)
                        {
                            thing.Destroy(DestroyMode.Vanish);
                        }
                        return thing;
                    }
                }
            }
        }
        int x = map.Size.x;
        int z = map.Size.z;
        IntVec3 intVec = UI.MouseCell();
        for (int j = 0; j < x; j++)
        {
            for (int k = 0; k < z; k++)
            {
                List<Thing> thingList2 = new IntVec3(j, intVec.y, k).GetThingList(map);
                if (!thingList2.NullOrEmpty<Thing>())
                {
                    for (int l = thingList2.Count - 1; l >= 0; l--)
                    {
                        Thing thing2 = thingList2[l];
                        if (thing2 != null && thing2.def != null && thing2.def.defName == s.thingDef.defName && thing2.stackCount == amount)
                        {
                            if (doDestroy)
                            {
                                thing2.Destroy(DestroyMode.Vanish);
                            }
                            return thing2;
                        }
                    }
                }
            }
        }
        return null;
    }

    internal static Selected GrabThingFromMap(Selected s, Map map, int amount)
    {
        var selected1 = Selected.ByThingDef(s.thingDef);
        selected1.stackVal = 0;
        Selected selected2;
        if ((map == null ? 1 : s.thingDef == null ? 1 : 0) != 0)
        {
            selected2 = selected1;
        }
        else
        {
            var x = map.Size.x;
            var z = map.Size.z;
            var intVec3 = UI.MouseCell();
            for (var index1 = 0; index1 < x; ++index1)
            {
                for (var index2 = 0; index2 < z; ++index2)
                {
                    var thingList = new IntVec3(index1, intVec3.y, index2).GetThingList(map);
                    if (!thingList.NullOrEmpty())
                        for (var index3 = thingList.Count - 1; index3 >= 0; --index3)
                        {
                            var thing1 = thingList[index3];
                            if ((thing1 == null ? 0 : thing1.def != null ? 1 : 0) != 0 && thing1.def.defName == s.thingDef.defName)
                            {
                                if (thing1.stackCount >= amount)
                                {
                                    var thing2 = thing1;
                                    thing2.stackCount = thing2.stackCount - amount;
                                    selected1.stackVal = amount;
                                    amount = 0;
                                    break;
                                }

                                amount -= thing1.stackCount;
                                selected1.stackVal += thing1.stackCount;
                                thing1.Destroy();
                                break;
                            }
                        }

                    if (amount <= 0)
                        break;
                }

                if (amount <= 0)
                    break;
            }

            selected2 = selected1;
        }

        return selected2;
    }

    internal static void BeginBuyItem(Selected selected)
    {
        if ((selected == null ? 0 : selected.thingDef != null ? 1 : 0) == 0)
            return;
        var silverAmountNear = GetSilverAmountNear(UI.MouseCell(), Find.CurrentMap);
        selected.UpdateBuyPrice();
        MessageTool.Show(Label.SILVER_NEAR + silverAmountNear + Label.SILVER_NEEDED + selected.buyPrice, silverAmountNear < selected.buyPrice ? MessageTypeDefOf.RejectInput : MessageTypeDefOf.PositiveEvent);
        if (silverAmountNear < selected.buyPrice)
            return;
        WindowTool.Open(Dialog_MessageBox.CreateConfirmation(new TaggedString(string.Format(Label.BUY_1, new string[2]
        {
            selected.thingDef.label,
            selected.buyPrice.ToString()
        })), () => selected.DmoBuyAndPlaceItem()));
    }

    internal static void DmoBuyAndPlaceItem(this Selected s)
    {
        if (s.thingDef != null)
        {
            DebugMenuOption debugMenuOption = new DebugMenuOption(Label.PLACE_ITEM, DebugMenuOptionMode.Tool, delegate()
            {
                IntVec3 intVec = UI.MouseCell();
                if (ThingTool.GetSilverAmountNear(intVec, Find.CurrentMap) >= s.buyPrice)
                {
                    SoundDefOf.ExecuteTrade.PlayOneShotOnCamera(null);
                    ThingTool.ReduceSilverAmount(intVec, Find.CurrentMap, s.buyPrice);
                    if (s.thingDef.race != null)
                    {
                        PlacingTool.DoPlaceMultiplePawns(s, CEditor.API.DicFactions.GetValue(CEditor.ListName));
                    }
                    else if (!s.thingDef.MadeFromStuff && !s.thingDef.HasComp(typeof(CompQuality)))
                    {
                        DebugThingPlaceHelper.DebugSpawn(s.thingDef, intVec, s.stackVal, false, s.style, true, null);
                    }
                    else
                    {
                        for (int i = 0; i < s.stackVal; i++)
                        {
                            Thing thing = ThingMaker.MakeThing(s.thingDef, s.stuff);
                            thing.SetQuality(s.quality);
                            thing.SetStyleDef(s.style);
                            if (thing.CanStackWith(thing))
                            {
                                GenPlace.TryPlaceThing(thing, UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
                            }
                            else
                            {
                                GenPlace.TryPlaceThing(thing.MakeMinified(), UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
                            }
                        }
                    }
                    EditWindow_Log.wantsToOpen = false;
                    return;
                }
                DebugTools.curTool = null;
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }
    }

    internal static void CreateBuilding(string defName, string label, string dsc, Type thingClass, string textur)
		{
			ThingDef thingDef = DefTool.ThingDef(defName);
			if (thingDef == null)
			{
				try
				{
					thingDef = new ThingDef();
					thingDef.defName = defName;
					thingDef.label = label;
					thingDef.description = dsc;
					thingDef.apparel = null;
					thingDef.designatorDropdown = DefTool.DesignatorDropdownGroupDef("Zombrella");
					thingDef.designationCategory = DefTool.DesignationCategoryDef("Misc");
					thingDef.thingClass = thingClass;
					thingDef.category = Verse.ThingCategory.Building;
					thingDef.altitudeLayer = AltitudeLayer.Building;
					thingDef.passability = Traversability.Impassable;
					thingDef.thingCategories = new List<ThingCategoryDef>();
					thingDef.thingCategories.Add(ThingCategoryDef.Named("BuildingsMisc"));
					ThingCategoryDef.Named("BuildingsMisc").childThingDefs.Add(thingDef);
					thingDef.placeWorkers = new List<Type>();
					thingDef.placeWorkers.Add(typeof(PlaceWorker_Heater));
					thingDef.drawPlaceWorkersWhileSelected = true;
					ThingDef thingDef2 = DefTool.ThingDef(textur, ThingDefOf.Grave);
					thingDef.graphicData = new GraphicData();
					thingDef.graphicData.texPath = thingDef2.graphicData.texPath;
					thingDef.graphicData.graphicClass = typeof(Graphic_Multi);
					if (thingClass == typeof(CharacterEditorGrave))
					{
						thingDef.graphicData.drawSize = new Vector2(3f, 4f);
						thingDef.graphicData.color = new Color(0.4f, 0.4f, 0.5f);
					}
					else
					{
						thingDef.graphicData.drawSize = new Vector2(1f, 2f);
						thingDef.graphicData.color = new Color(0.3f, 0.3f, 0.4f);
					}
					thingDef.graphicData.shadowData = new ShadowData();
					thingDef.graphicData.shadowData.volume = new Vector3(0.83f, 0.3f, 1.7f);
					thingDef.size = new IntVec2(1, 2);
					thingDef.placingDraggableDimensions = 1;
					thingDef.rotatable = true;
					thingDef.selectable = true;
					thingDef.useHitPoints = true;
					thingDef.leaveResourcesWhenKilled = false;
					thingDef.destroyable = true;
					thingDef.blockWind = true;
					thingDef.blockLight = true;
					thingDef.building = new BuildingProperties();
					thingDef.building.claimable = true;
					thingDef.building.alwaysDeconstructible = false;
					if (thingClass == typeof(CharacterEditorGrave))
					{
						thingDef.building.fullGraveGraphicData = new GraphicData();
						thingDef.building.fullGraveGraphicData.texPath = thingDef2.building.fullGraveGraphicData.texPath;
						thingDef.building.fullGraveGraphicData.graphicClass = typeof(Graphic_Multi);
						thingDef.building.fullGraveGraphicData.drawSize = new Vector2(3f, 4f);
						thingDef.building.fullGraveGraphicData.color = new Color(0.4f, 0.4f, 0.5f);
					}
					thingDef.defaultPlacingRot = Rot4.South;
					thingDef.terrainAffordanceNeeded = TerrainAffordanceDefOf.Light;
					if (thingClass != typeof(CharacterEditorGrave))
					{
						thingDef.constructionSkillPrerequisite = 5;
					}
					if (thingClass == typeof(CharacterEditorGrave))
					{
						thingDef.constructEffect = EffecterDefOf.ConstructDirt;
					}
					else
					{
						thingDef.constructEffect = EffecterDefOf.ConstructMetal;
					}
					thingDef.soundImpactDefault = DefTool.SoundDef("BulletImpact_Metal");
					thingDef.repairEffect = EffecterDefOf.ConstructMetal;
					thingDef.hasInteractionCell = true;
					thingDef.interactionCellOffset = new IntVec3(1, 0, 0);
					thingDef.tickerType = TickerType.Normal;
					thingDef.AddStat(StatDefOf.MaxHitPoints, 500f);
					if (thingClass != typeof(CharacterEditorGrave))
					{
						thingDef.AddStat(StatDefOf.MarketValue, 2000f);
					}
					thingDef.AddStat(StatDefOf.WorkToBuild, 5000f);
					thingDef.AddStat(StatDefOf.Flammability, 0.2f);
					thingDef.costList = new List<ThingDefCountClass>();
					thingDef.costList.Add(new ThingDefCountClass(ThingDefOf.ComponentIndustrial, 4));
					if (thingClass == typeof(CharacterEditorGrave))
					{
						thingDef.costList.Add(new ThingDefCountClass(ThingDefOf.WoodLog, 200));
					}
					else
					{
						thingDef.costList.Add(new ThingDefCountClass(ThingDefOf.Steel, 350));
						thingDef.costList.Add(new ThingDefCountClass(ThingDefOf.Plasteel, 50));
					}
					thingDef.modContentPack = thingDef.designatorDropdown.modContentPack;
					if (thingClass == typeof(CharacterEditorGrave))
					{
						thingDef.minifiedDef = null;
					}
					else
					{
						thingDef.minifiedDef = ThingDefOf.MinifiedThing;
					}

                    try
                    {
                        thingDef.RegisterBuildingDef();
                        thingDef.designationCategory.ResolveReferences();
                        thingDef.designationCategory.PostLoad();
                    }
                    catch (Exception e)
                    {
                        Log.Message("Failed to generate building def!" + "\n" + e.ToString());
                    }
                }
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						Log.Error(ex.Message + "\n" + ex.StackTrace);
					}
				}
			}
		}

    internal static void RegisterBuildingDef(this ThingDef td)
    {
        if (td != null && DefTool.ThingDef(td.defName) == null)
        {
            Type typeFromHandle = typeof(ThingDefGenerator_Buildings);
            object[] param = new object[]
            {
                td,
                false
            };
            td.frameDef = (ThingDef)typeFromHandle.CallMethod("NewFrameDef_Thing", param);
            td.frameDef.ResolveReferences();
            td.frameDef.PostLoad();
            object[] param2 = new object[]
            {
                td,
                false,
                td.blueprintDef, // this one might jsut need to be null?
                false
            };
            td.blueprintDef = (ThingDef)typeFromHandle.CallMethod("NewBlueprintDef_Thing", param2);
            td.blueprintDef.entityDefToBuild = td;
            td.blueprintDef.ResolveReferences();
            td.blueprintDef.PostLoad();
            ThingDef thingDef = null;
            if (td.minifiedDef != null)
            {
                object[] param3 = new object[]
                {
                    td,
                    true,
                    td.blueprintDef,
                    false
                };
                thingDef = (ThingDef)typeFromHandle.CallMethod("NewBlueprintDef_Thing", param3);
                thingDef.ResolveReferences();
                thingDef.PostLoad();
            }
            td.ResolveReferences();
            td.PostLoad();
            DefTool.GiveShortHashTo(td, typeof(ThingDef));
            DefTool.GiveShortHashTo(td.blueprintDef, typeof(ThingDef));
            DefTool.GiveShortHashTo(td.frameDef, typeof(ThingDef));
            DefDatabase<ThingDef>.Add(td);
            if (td.minifiedDef != null)
            {
                DefDatabase<ThingDef>.Add(thingDef);
            }
            DefDatabase<ThingDef>.Add(td.blueprintDef);
            DefDatabase<ThingDef>.Add(td.frameDef);
        }
    }

    internal static List<string> GetAllApparelTags()
    {
        var dic = new SortedDictionary<string, int>();
        foreach (var allDef in DefDatabase<ThingDef>.AllDefs)
            if (allDef.IsApparel)
                dic.AddFromList(allDef.apparel.tags);
        foreach (var allDef in DefDatabase<PawnKindDef>.AllDefs)
            dic.AddFromList(allDef.apparelTags);
        return dic.Keys.ToList();
    }

    internal static List<string> GetAllOutfitTags()
    {
        var dic = new SortedDictionary<string, int>();
        foreach (var allDef in DefDatabase<ThingDef>.AllDefs)
            if (allDef.IsApparel)
                dic.AddFromList(allDef.apparel.defaultOutfitTags);
        return dic.Keys.ToList();
    }

    internal static List<string> GetAllTradeTags()
    {
        var dic = new SortedDictionary<string, int>();
        foreach (var allDef in DefDatabase<ThingDef>.AllDefs)
            dic.AddFromList(allDef.tradeTags);
        return dic.Keys.ToList();
    }

    internal static List<string> GetAllWeaponTags()
    {
        var dic = new SortedDictionary<string, int>();
        foreach (var allDef in DefDatabase<ThingDef>.AllDefs)
            if (allDef.IsWeapon)
                dic.AddFromList(allDef.weaponTags);
        foreach (var allDef in DefDatabase<PawnKindDef>.AllDefs)
            dic.AddFromList(allDef.weaponTags);
        return dic.Keys.ToList();
    }

    internal static HashSet<int> ListOfQualityInts()
    {
        var intSet = new HashSet<int>();
        foreach (var allQualityCategory in QualityUtility.AllQualityCategories)
            intSet.Add((int)allQualityCategory);
        return intSet;
    }

    internal static HashSet<ThingDef> ListOfItems(
        string modname,
        ThingCategoryDef tc,
        ThingCategory tc2)
    {
        return DefTool.ListBy(DefTool.CONDITION_IS_ITEM(modname, tc, tc2));
    }

    internal static HashSet<ThingDef> ListOfItemsWithNull(string modname, ThingCategoryDef tc, ThingCategory tc2)
    {
        HashSet<ThingDef> hashSet = new HashSet<ThingDef>();
        hashSet.Add(null);
        hashSet.AddRange(ThingTool.ListOfItems(modname, tc, tc2));
        return hashSet;
    }


    internal static ThingStyleDef GetStyle(
        this ThingDef thingDef,
        ref HashSet<ThingStyleDef> l,
        ref int styleIndex,
        bool random)
    {
        l = new HashSet<ThingStyleDef>();
        ThingStyleDef thingStyleDef;
        if ((thingDef == null ? 1 : !thingDef.CanBeStyled() ? 1 : 0) != 0)
        {
            thingStyleDef = null;
        }
        else
        {
            l = ListOfThingStyleDefs(thingDef, null, true);
            if (random)
                styleIndex = CEditor.zufallswert.Next(l.Count);
            if (!l.NullOrEmpty())
            {
                if (l.Count <= styleIndex)
                    styleIndex = 0;
                thingStyleDef = l.At(styleIndex);
            }
            else
            {
                thingStyleDef = null;
            }
        }

        return thingStyleDef;
    }

    internal static HashSet<ThingStyleDef> ListOfThingStyleDefs(
        ThingDef thingDef,
        string modname,
        bool withNull)
    {
        var thingStyleDefSet1 = new HashSet<ThingStyleDef>();
        if (withNull)
            thingStyleDefSet1.Add(null);
        HashSet<ThingStyleDef> thingStyleDefSet2;
        if (thingDef == null)
        {
            thingStyleDefSet2 = thingStyleDefSet1;
        }
        else
        {
            modname.NullOrEmpty();
            foreach (var styleCategoryDef in DefTool.ListByMod<StyleCategoryDef>(modname))
            foreach (var thingDefStyle in styleCategoryDef.thingDefStyles)
                if (thingDef == thingDefStyle.ThingDef)
                    thingStyleDefSet1.Add(thingDefStyle.StyleDef);
            thingStyleDefSet2 = thingStyleDefSet1;
        }

        return thingStyleDefSet2;
    }

    internal static void CopyStatOffsets(this ThingDef t)
    {
        t.equippedStatOffsets.CopyList(ref lCopyStatOffsets);
    }

    internal static void CopyStatFactors(this ThingDef t)
    {
        t.statBases.CopyList(ref lCopyStatFactors);
    }

    internal static void CopyTradeTags(this ThingDef t)
    {
        t.tradeTags.CopyList(ref lCopyTradeTags);
    }

    internal static void CopyWeaponTags(this ThingDef t)
    {
        t.weaponTags.CopyList(ref lCopyWeaponTags);
    }

    internal static void CopyStuffCategories(this ThingDef t)
    {
        t.stuffCategories.CopyList(ref lCopyStuffCategories);
    }

    internal static void CopyBladeLinkTraits(this Thing t)
    {
        var comp = t.TryGetComp<CompBladelinkWeapon>();
        if (comp != null)
            comp.TraitsListForReading.CopyList(ref lCopyBladeLinkTraits);
        else
            lCopyBladeLinkTraits = new List<WeaponTraitDef>();
    }

    internal static void CopyCosts(this ThingDef t)
    {
        t.costList.CopyList(ref lCopyCosts);
    }

    internal static void CopyCostsDiff(this ThingDef t)
    {
        t.costListForDifficulty.costList.CopyList(ref lCopyCostsDiff);
    }

    internal static void CopyPrerequisites(this ThingDef t)
    {
        t.researchPrerequisites.CopyList(ref lCopyPrerequisites);
    }

    internal static void CopyApparelLayer(this ThingDef t)
    {
        t.apparel.layers.CopyList(ref lCopyApparelLayer);
    }

    internal static void CopyBodyPartGroup(this ThingDef t)
    {
        t.apparel.bodyPartGroups.CopyList(ref lCopyBodyPartGroup);
    }

    internal static void PasteStatFactors(this ThingDef t)
    {
        if (t.statBases == null)
            t.statBases = new List<StatModifier>();
        t.statBases.PasteListNonDef(lCopyStatFactors, DefTool.CompareStatModifier, DefTool.SetStatModifier, DefTool.DefGetterStatModifier, DefTool.ValGetterStatModifier);
        t.UpdateFreeLists(FreeList.StatFactors);
    }

    internal static void PasteStatOffsets(this ThingDef t)
    {
        if (t.equippedStatOffsets == null)
            t.equippedStatOffsets = new List<StatModifier>();
        t.equippedStatOffsets.PasteListNonDef(lCopyStatOffsets, DefTool.CompareStatModifier, DefTool.SetStatModifier, DefTool.DefGetterStatModifier, DefTool.ValGetterStatModifier);
        t.UpdateFreeLists(FreeList.StatOffsets);
    }

    internal static void PasteStuffCategories(this ThingDef t)
    {
        if (t.stuffCategories == null)
            t.stuffCategories = new List<StuffCategoryDef>();
        t.stuffCategories.PasteList(lCopyStuffCategories);
        t.UpdateFreeLists(FreeList.StuffCategories);
    }

    internal static void PasteBladeLinkTraits(this Thing t)
    {
        var comp = t.TryGetComp<CompBladelinkWeapon>();
        if (comp == null)
            return;
        comp.TraitsListForReading.PasteList(lCopyBladeLinkTraits);
    }

    internal static void PasteCosts(this ThingDef t)
    {
        if (t.costList == null)
            t.costList = new List<ThingDefCountClass>();
        t.costList.PasteListNonDef(lCopyCosts, DefTool.CompareThingDefCountClass, DefTool.SetThingDefCountClass, DefTool.DefGetterThingDefCountClass, DefTool.ValGetterThingDefCountClass);
        t.UpdateFreeLists(FreeList.Costs);
    }

    internal static void PasteCostsDiff(this ThingDef t)
    {
        if (t.costListForDifficulty.costList == null)
            t.costListForDifficulty.costList = new List<ThingDefCountClass>();
        t.costListForDifficulty.costList.PasteListNonDef(lCopyCostsDiff, DefTool.CompareThingDefCountClass, DefTool.SetThingDefCountClass, DefTool.DefGetterThingDefCountClass, DefTool.ValGetterThingDefCountClass);
        t.UpdateFreeLists(FreeList.CostsDiff);
    }

    internal static void PastePrerequisites(this ThingDef t)
    {
        if (t.researchPrerequisites == null)
            t.researchPrerequisites = new List<ResearchProjectDef>();
        t.researchPrerequisites.PasteList(lCopyPrerequisites);
        t.UpdateFreeLists(FreeList.Prerequisites);
    }

    internal static void PasteApparelLayer(this ThingDef t)
    {
        if (t.apparel.layers == null)
            t.apparel.layers = new List<ApparelLayerDef>();
        t.apparel.layers.PasteList(lCopyApparelLayer);
        t.UpdateFreeLists(FreeList.ApparelLayer);
    }

    internal static void PasteBodyPartGroup(this ThingDef t)
    {
        if (t.apparel.bodyPartGroups == null)
            t.apparel.bodyPartGroups = new List<BodyPartGroupDef>();
        t.apparel.bodyPartGroups.PasteList(lCopyBodyPartGroup);
        t.UpdateFreeLists(FreeList.BodyPartGroup);
    }

    internal static void RemoveStatOffset(this ThingDef t, StatDef def)
    {
        var by = t.equippedStatOffsets.FindBy(DefTool.CompareStatModifier, def);
        if (by == null)
            return;
        t.equippedStatOffsets.Remove(by);
        t.UpdateFreeLists(FreeList.StatOffsets);
    }

    internal static void RemoveStatFactor(this ThingDef t, StatDef def)
    {
        var by = t.statBases.FindBy(DefTool.CompareStatModifier, def);
        if (by == null)
            return;
        t.statBases.Remove(by);
        t.UpdateFreeLists(FreeList.StatFactors);
    }

    internal static void RemoveStuffCategorie(this ThingDef t, StuffCategoryDef def)
    {
        StuffCategoryDef stuffCategoryDef = t.stuffCategories.FindByDef(def);
        if (stuffCategoryDef != null)
        {
            t.stuffCategories.Remove(stuffCategoryDef);
            t.UpdateFreeLists(ThingTool.FreeList.StuffCategories);
        }
    }

    internal static void RemoveBladeLinkTrait(this Thing t, WeaponTraitDef def)
    {
        CompBladelinkWeapon compBladelinkWeapon = t.TryGetComp<CompBladelinkWeapon>();
        if (compBladelinkWeapon != null)
        {
            compBladelinkWeapon.TraitsListForReading.Remove(def);
        }
    }
    internal static void RemoveCosts(this ThingDef t, ThingDef def)
    {
        var by = t.costList.FindBy(DefTool.CompareThingDefCountClass, def);
        if (by == null)
            return;
        t.costList.Remove(by);
        t.UpdateFreeLists(FreeList.Costs);
    }

    internal static void RemoveCostsDiff(this ThingDef t, ThingDef def)
    {
        var by = t.costListForDifficulty.costList.FindBy(DefTool.CompareThingDefCountClass, def);
        if (by == null)
            return;
        t.costListForDifficulty.costList.Remove(by);
        t.UpdateFreeLists(FreeList.CostsDiff);
    }

    internal static void RemovePrerequisite(this ThingDef t, ResearchProjectDef r)
    {
        if (t.researchPrerequisites == null)
            return;
        t.researchPrerequisites.Remove(r);
        t.UpdateFreeLists(FreeList.Prerequisites);
    }

    internal static void RemoveApparelLayer(this ThingDef t, ApparelLayerDef a)
    {
        if (t.apparel.layers == null)
            return;
        t.apparel.layers.Remove(a);
        t.UpdateFreeLists(FreeList.ApparelLayer);
    }

    internal static void RemoveBodyPartGroup(this ThingDef t, BodyPartGroupDef b)
    {
        if (t.apparel.bodyPartGroups == null)
            return;
        t.apparel.bodyPartGroups.Remove(b);
        t.UpdateFreeLists(FreeList.BodyPartGroup);
    }

    internal static void UpdateFreeLists(this ThingDef t, ThingTool.FreeList f)
		{
			if (f == ThingTool.FreeList.All || f == ThingTool.FreeList.CategoryFactors)
			{
				ThingTool.lCategoryDef_Factors = ThingTool.ListOfStatCategoryDef_ForStatFactor(ThingTool.UseAllCategories);
			}
			if (f == ThingTool.FreeList.All || f == ThingTool.FreeList.CategoryOffsets)
			{
				ThingTool.lCategoryDef_Offsets = ThingTool.ListOfStatCategoryDef_ForStatOffset(ThingTool.UseAllCategories);
			}
			if (t != null)
			{
				t.ResolveReferences();
				if (f == ThingTool.FreeList.All || f == ThingTool.FreeList.StatFactors)
				{
					ICollection<StatModifier> statBases = t.statBases;
					HashSet<StatCategoryDef> lselectedCat;
					if (ThingTool.selected_StatFactor_CatDef != null)
					{
						(lselectedCat = new HashSet<StatCategoryDef>()).Add(ThingTool.selected_StatFactor_CatDef);
					}
					else
					{
						lselectedCat = ThingTool.lCategoryDef_Factors;
					}
					ThingTool.lFreeStatDefFactors = statBases.ListOfFreeCustom(DefTool.StatDefs_Selection(lselectedCat), DefTool.CompareStatModifier, DefTool.CompareStatCategoryNot, ThingTool.selected_StatFactor_CatDef);
				}
				if (f == ThingTool.FreeList.All || f == ThingTool.FreeList.StatOffsets)
				{
					ICollection<StatModifier> equippedStatOffsets = t.equippedStatOffsets;
					HashSet<StatCategoryDef> lselectedCat2;
					if (ThingTool.selected_StatOffset_CatDef != null)
					{
						(lselectedCat2 = new HashSet<StatCategoryDef>()).Add(ThingTool.selected_StatOffset_CatDef);
					}
					else
					{
						lselectedCat2 = ThingTool.lCategoryDef_Offsets;
					}
					ThingTool.lFreeStatDefOffsets = equippedStatOffsets.ListOfFreeCustom(DefTool.StatDefs_Selection(lselectedCat2), DefTool.CompareStatModifier, DefTool.CompareStatCategoryNot, ThingTool.selected_StatOffset_CatDef);
				}
				if (f == ThingTool.FreeList.All || f == ThingTool.FreeList.StuffCategories)
				{
					ThingTool.lFreeStuffCategories = t.stuffCategories.ListOfFreeDef<StuffCategoryDef>();
				}
				if (f == ThingTool.FreeList.All || f == ThingTool.FreeList.Costs)
				{
					ThingTool.lFreeCosts = t.costList.ListOfFreeCustom<ThingDefCountClass, ThingDef, ThingDef>(CEditor.API.ListOf<ThingDef>(EType.CostItems), DefTool.CompareThingDefCountClass, null, null);
					ThingTool.lFreeCosts = ThingTool.lFreeCosts.OrderBy(delegate(ThingDef d)
					{
						StuffProperties stuffProps = d.stuffProps;
						if (stuffProps == null)
						{
							return null;
						}
						return stuffProps.categories.FirstOrDefault<StuffCategoryDef>().SDefname();
					}).ThenBy(delegate(ThingDef d)
					{
						List<ThingCategoryDef> thingCategories = d.thingCategories;
						if (thingCategories == null)
						{
							return null;
						}
						return thingCategories.FirstOrDefault<ThingCategoryDef>().SDefname();
					}).ThenBy((ThingDef d) => d.label).ToHashSet<ThingDef>();
				}
				if ((f == ThingTool.FreeList.All || f == ThingTool.FreeList.CostsDiff) && t.costListForDifficulty != null)
				{
					ThingTool.lFreeCostsDiff = t.costListForDifficulty.costList.ListOfFreeCustom<ThingDefCountClass, ThingDef, ThingDef>(CEditor.API.ListOf<ThingDef>(EType.CostItems), DefTool.CompareThingDefCountClass, null, null);
				}
				if (f == ThingTool.FreeList.All || f == ThingTool.FreeList.Prerequisites)
				{
					ThingTool.lFreePrerequisites = t.researchPrerequisites.ListOfFreeDef<ResearchProjectDef>();
				}
				if ((f == ThingTool.FreeList.All || f == ThingTool.FreeList.ApparelLayer) && t.apparel != null)
				{
					ThingTool.lFreeApparelLayer = t.apparel.layers.ListOfFreeDef<ApparelLayerDef>();
				}
				if ((f == ThingTool.FreeList.All || f == ThingTool.FreeList.BodyPartGroup) && t.apparel != null)
				{
					ThingTool.lFreeBodyPartGroup = t.apparel.bodyPartGroups.ListOfFreeDef<BodyPartGroupDef>();
				}
				if (f == ThingTool.FreeList.Costs || f == ThingTool.FreeList.Prerequisites || f == ThingTool.FreeList.CostsDiff)
				{
					t.UpdateRecipes();
				}
			}
		}

    internal static void SetResearchPrerequisite(this ThingDef t, ResearchProjectDef r)
    {
        if (t.recipeMaker == null)
            return;
        t.recipeMaker.researchPrerequisite = r;
        t.UpdateRecipes();
    }

    internal static void SetStatOffset(this ThingDef t, StatDef s, float offset)
    {
        DefTool.Set<StatModifier, StatDef, float>(ref t.equippedStatOffsets, DefTool.CompareStatModifier, new Action<List<StatModifier>, StatModifier, StatDef, float>(DefTool.SetStatModifier), s, offset);
        t.UpdateFreeLists(ThingTool.FreeList.StatOffsets);
    }

    internal static void SetStatFactor(this ThingDef t, StatDef s, float factor)
    {
        DefTool.Set<StatModifier, StatDef, float>(ref t.statBases, DefTool.CompareStatModifier, new Action<List<StatModifier>, StatModifier, StatDef, float>(DefTool.SetStatModifier), s, factor);
        t.UpdateFreeLists(ThingTool.FreeList.StatFactors);
    }


    internal static void SetStuffCategorie(this ThingDef t, StuffCategoryDef cat, Selected s = null)
    {
        if (t.stuffCategories == null)
            t.stuffCategories = new List<StuffCategoryDef>();
        t.stuffCategories.AddDef(cat);
        t.UpdateFreeLists(FreeList.StuffCategories);
        s?.UpdateStuffList();
    }

    internal static void SetBladeLinkTrait(this Thing thing, WeaponTraitDef t)
    {
        thing.TryGetComp<CompBladelinkWeapon>()?.TraitsListForReading.Add(t);
    }

    
    internal static void SetCosts(this ThingDef t, ThingDef cost, int val)
    {
        DefTool.Set<ThingDefCountClass, ThingDef, int>(ref t.costList, DefTool.CompareThingDefCountClass, new Action<List<ThingDefCountClass>, ThingDefCountClass, ThingDef, int>(DefTool.SetThingDefCountClass), cost, val);
        t.UpdateFreeLists(ThingTool.FreeList.Costs);
    }

    
    internal static void SetCostsDiff(this ThingDef t, ThingDef cost, int val)
    {
        DefTool.Set<ThingDefCountClass, ThingDef, int>(ref t.costListForDifficulty.costList, DefTool.CompareThingDefCountClass, new Action<List<ThingDefCountClass>, ThingDefCountClass, ThingDef, int>(DefTool.SetThingDefCountClass), cost, val);
        t.UpdateFreeLists(ThingTool.FreeList.CostsDiff);
    }
    internal static void SetPrerequisite(this ThingDef t, ResearchProjectDef r)
    {
        if (t.researchPrerequisites == null)
            t.researchPrerequisites = new List<ResearchProjectDef>();
        t.researchPrerequisites.Add(r);
        t.UpdateFreeLists(FreeList.Prerequisites);
    }

    internal static void SetApparelLayer(this ThingDef t, ApparelLayerDef a)
    {
        if (t.apparel.layers == null)
            t.apparel.layers = new List<ApparelLayerDef>();
        t.apparel.layers.Add(a);
        t.UpdateFreeLists(FreeList.ApparelLayer);
    }

    internal static void SetBodyPartGroup(this ThingDef t, BodyPartGroupDef b)
    {
        if (t.apparel.bodyPartGroups == null)
            t.apparel.bodyPartGroups = new List<BodyPartGroupDef>();
        t.apparel.bodyPartGroups.Add(b);
        t.UpdateFreeLists(FreeList.BodyPartGroup);
    }

    internal static HashSet<StatCategoryDef> ListOfStatCategoryDef_ForStatFactor(
        bool all)
    {
        var lcat = DefTool.StatCategoryDefs_Selection(List_StatCategories_ToSkip(all));
        lcat.RemoveCategoriesWithNoStatDef();
        return lcat;
    }

    internal static HashSet<StatCategoryDef> ListOfStatCategoryDef_ForStatOffset(
        bool all)
    {
        var lcat = DefTool.StatCategoryDefs_Selection(List_StatCategories_ToSkip(all));
        lcat.RemoveCategoriesWithNoStatDef();
        return lcat;
    }

    internal static HashSet<StatCategoryDef> List_StatCategories_ToSkip(
        bool allowAll)
    {
        var statCategoryDefSet1 = new HashSet<StatCategoryDef>();
        HashSet<StatCategoryDef> statCategoryDefSet2;
        if (allowAll)
        {
            statCategoryDefSet2 = statCategoryDefSet1;
        }
        else
        {
            statCategoryDefSet1.Add(StatCategoryDefOf.Genetics);
            statCategoryDefSet1.Add(StatCategoryDefOf.BasicsNonPawn);
            statCategoryDefSet1.Add(StatCategoryDefOf.Terrain);
            statCategoryDefSet1.Add(StatCategoryDefOf.Apparel);
            statCategoryDefSet1.Add(StatCategoryDefOf.Ability);
            statCategoryDefSet1.Add(StatCategoryDefOf.Meditation);
            statCategoryDefSet1.Add(DefTool.GetDef<StatCategoryDef>("Mechanitor"));
            statCategoryDefSet2 = statCategoryDefSet1;
        }

        return statCategoryDefSet2;
    }

    internal enum FreeList
    {
        CategoryFactors,
        CategoryOffsets,
        StatFactors,
        StatOffsets,
        StuffCategories,
        Costs,
        CostsDiff,
        BladeLink,
        Prerequisites,
        ApparelLayer,
        BodyPartGroup,
        All
    }
}
