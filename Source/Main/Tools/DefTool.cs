// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DefTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Object = System.Object;

namespace CharacterEditor;

internal static class DefTool
{
    private static readonly GiveShortHash giveShortHash = AccessTools.MethodDelegate<GiveShortHash>(AccessTools.Method(typeof(ShortHashGiver), "GiveShortHash"));
    internal static readonly AccessTools.FieldRef<Dictionary<Type, HashSet<ushort>>> takenShortHashes = AccessTools.StaticFieldRefAccess<Dictionary<Type, HashSet<ushort>>>(AccessTools.Field(typeof(ShortHashGiver), "takenHashesPerDeftype"));

    internal static Func<ThingCategoryDef, bool> CONDITION_NO_CORPSE => t => !t.defName.ToLower().Contains("corpse");

    internal static Func<StatCategoryDef, string> CategoryLabel => cat => cat.SLabel_PlusDefName();

    internal static Func<StatModifier, StatDef> DefGetterStatModifier => sm => sm.stat;

    internal static Func<DamageFactor, DamageDef> DefGetterDamageFacotr => df => df.damageDef;

    internal static Func<GeneticTraitData, TraitDef> DefGetterGeneticTraitData => gtd => gtd.def;

    internal static Func<Aptitude, SkillDef> DefGetterAptitude => a => a.skill;

    internal static Func<ThingDefCountClass, ThingDef> DefGetterThingDefCountClass => t => t.thingDef;

    internal static Func<DamageFactor, DamageDef> DefGetterDamageFactor => d => d.damageDef;

    internal static Func<PawnCapacityModifier, PawnCapacityDef> DefGetterPawnCapacityModifier => pcm => pcm.capacity;

    internal static Func<StatModifier, float> ValGetterStatModifier => sm => sm.value;

    internal static Func<DamageFactor, float> ValGetterDamageFacotr => df => df.factor;

    internal static Func<GeneticTraitData, int> ValGetterGeneticTraitData => gtd => gtd.degree;

    internal static Func<Aptitude, int> ValGetterAptitude => a => a.level;

    internal static Func<ThingDefCountClass, int> ValGetterThingDefCountClass => t => t.count;

    internal static Func<DamageFactor, float> ValGetterDamageFactor => d => d.factor;

    internal static Func<PawnCapacityModifier, float> ValGetterPCMoffset => pcm => pcm.offset;

    internal static Func<PawnCapacityModifier, float> ValGetterPCMfactor => pcm => pcm.postFactor;

    internal static Func<DamageFactor, DamageDef, bool> CompareDamageFactor => (a, b) => a.damageDef.defName == b.defName;

    internal static Func<StatModifier, StatDef, bool> CompareStatModifier => (a, b) => a.stat.defName == b.defName;

    internal static Func<ThingDefCountClass, ThingDef, bool> CompareThingDefCountClass => (a, b) => a.thingDef.defName == b.defName;

    internal static Func<Aptitude, SkillDef, bool> CompareAptitude => (a, b) => a.skill.defName == b.defName;

    internal static Func<GeneticTraitData, GeneticTraitData, bool> CompareGeneticTraitData => (a, b) => b != null && a.def.defName == b.def.defName && a.degree == b.degree;

    internal static Func<PawnCapacityModifier, PawnCapacityDef, bool> ComparePawnCapacityModifier => (a, b) => a.capacity.defName == b.defName;

    internal static Func<StatDef, StatCategoryDef, bool> CompareStatCategoryNot => (a, b) => a.category != b;

    internal static KeyBindingDef TryCreateKeyBinding(
        string defName,
        string keyCode,
        string label,
        string desc)
    {
        KeyBindingDef key = null;
        try
        {
            if (!keyCode.NullOrEmpty())
            {
                key = new KeyBindingDef();
                key.category = KeyBindingCategoryDefOf.MainTabs;
                key.defaultKeyCodeA = keyCode.AsKeyCode();
                key.defaultKeyCodeB = 0;
                key.defName = defName;
                key.description = desc;
                key.label = label;
                key.ResolveReferences();
                key.PostLoad();
                GiveShortHashTo(key, typeof(KeyBindingDef));
                DefDatabase<KeyBindingDef>.Add(key);
                KeyPrefs.KeyPrefsData.keyPrefs.Add(key, new KeyBindingData(key.defaultKeyCodeA, key.defaultKeyCodeB));
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.StackTrace);
        }

        return key;
    }

    internal static MainButtonDef GetCreateMainButton(
        string defName,
        string label,
        string desc,
        Type typeClass,
        ModContentPack pack,
        string keyCode,
        bool isVisible)
    {
        var mainButtonDef = MainButtonDef(defName);
        if (mainButtonDef == null)
        {
            var keyBinding = TryCreateKeyBinding(defName, keyCode, label, desc);
            mainButtonDef = new MainButtonDef();
            mainButtonDef.buttonVisible = isVisible;
            mainButtonDef.defName = defName;
            mainButtonDef.description = desc;
            mainButtonDef.label = label;
            mainButtonDef.tabWindowClass = typeClass;
            mainButtonDef.order = MainButtonDefOf.Menu.order - 1;
            mainButtonDef.hotKey = keyBinding;
            mainButtonDef.validWithoutMap = false;
            mainButtonDef.modContentPack = pack;
            mainButtonDef.iconPath = "beditoricon";
            mainButtonDef.minimized = true;
            mainButtonDef.ResolveReferences();
            mainButtonDef.PostLoad();
            DefDatabase<MainButtonDef>.Add(mainButtonDef);
        }

        return mainButtonDef;
    }

    internal static void GiveShortHashTo(Def def, Type defType)
    {
        giveShortHash(def, defType, takenShortHashes.Invoke()[defType]);
    }

    internal static T GetDef<T>(string defName) where T : Def
    {
        return !defName.NullOrEmpty() ? DefDatabase<T>.GetNamed(defName, false) : default;
    }

    internal static BackstoryDef BackstoryDef(string defName)
    {
        return GetDef<BackstoryDef>(defName);
    }

    internal static MainButtonDef MainButtonDef(string defName)
    {
        return GetDef<MainButtonDef>(defName);
    }

    internal static PawnKindDef PawnKindDef(string defName)
    {
        return GetDef<PawnKindDef>(defName);
    }

    internal static PawnKindDef PawnKindDef(string defName, PawnKindDef fallback)
    {
        return PawnKindDef(defName) ?? fallback;
    }

    internal static PawnRelationDef PawnRelationDef(string defName)
    {
        return GetDef<PawnRelationDef>(defName);
    }

    internal static BodyTypeDef BodyTypeDef(string defName)
    {
        return GetDef<BodyTypeDef>(defName);
    }

    internal static BeardDef BeardDef(string defName)
    {
        return GetDef<BeardDef>(defName);
    }

    internal static TattooDef TattooDef(string defName)
    {
        return GetDef<TattooDef>(defName);
    }

    internal static AbilityDef AbilityDef(string defName)
    {
        return GetDef<AbilityDef>(defName);
    }

    internal static GeneDef GeneDef(string defName)
    {
        return GetDef<GeneDef>(defName);
    }

    internal static XenotypeDef XenotypeDef(string defName)
    {
        return GetDef<XenotypeDef>(defName);
    }

    internal static HairDef HairDef(string defName)
    {
        return GetDef<HairDef>(defName);
    }

    internal static HediffDef HediffDef(string defName)
    {
        return GetDef<HediffDef>(defName);
    }

    internal static RecordDef RecordDef(string defName)
    {
        return GetDef<RecordDef>(defName);
    }

    internal static SkillDef SkillDef(string defName)
    {
        return GetDef<SkillDef>(defName);
    }

    internal static PawnCapacityDef PawnCapacityDef(string defName)
    {
        return GetDef<PawnCapacityDef>(defName);
    }

    internal static ScenPartDef ScenPartDef(string defName)
    {
        return GetDef<ScenPartDef>(defName);
    }

    internal static ThingDef ThingDef(string defName)
    {
        return GetDef<ThingDef>(defName);
    }

    internal static ThingStyleDef ThingStyleDef(string defName)
    {
        return GetDef<ThingStyleDef>(defName);
    }

    internal static ThingDef ThingDef(string defName, ThingDef fallback)
    {
        return ThingDef(defName) ?? fallback;
    }

    internal static NeedDef NeedDef(string defName)
    {
        return GetDef<NeedDef>(defName);
    }

    internal static ThoughtDef ThoughtDef(string defName)
    {
        return GetDef<ThoughtDef>(defName);
    }

    internal static TraitDef TraitDef(string defName)
    {
        return GetDef<TraitDef>(defName);
    }

    internal static WorkTypeDef WorkTypeDef(string defName)
    {
        return GetDef<WorkTypeDef>(defName);
    }

    internal static StatDef StatDef(string defName)
    {
        return GetDef<StatDef>(defName);
    }

    internal static JobDef JobDef(string defName)
    {
        return GetDef<JobDef>(defName);
    }

    internal static SoundDef SoundDef(string defName)
    {
        return GetDef<SoundDef>(defName);
    }

    internal static StuffCategoryDef StuffCategoryDef(string defName)
    {
        return GetDef<StuffCategoryDef>(defName);
    }

    internal static BodyPartGroupDef BodyPartGroupDef(string defName)
    {
        return GetDef<BodyPartGroupDef>(defName);
    }

    internal static ApparelLayerDef ApparelLayerDef(string defName)
    {
        return GetDef<ApparelLayerDef>(defName);
    }

    internal static DesignatorDropdownGroupDef DesignatorDropdownGroupDef(
        string defName)
    {
        return GetDef<DesignatorDropdownGroupDef>(defName);
    }

    internal static DesignationCategoryDef DesignationCategoryDef(
        string defName)
    {
        return GetDef<DesignationCategoryDef>(defName);
    }

    internal static KeyBindingDef KeyBindingDef(string defName)
    {
        return GetDef<KeyBindingDef>(defName);
    }

    internal static bool IsFromMod(this Def d, string modname)
    {
        return d != null && d.modContentPack != null && d.modContentPack.Name == modname;
    }

    internal static bool IsFromCoreMod(this Def d)
    {
        return d != null && d.modContentPack != null && d.modContentPack.IsCoreMod;
    }

    internal static string GetModName(this Def d)
    {
        return d == null || d.modContentPack == null ? "" : d.modContentPack.Name;
    }

    internal static bool IsNullOrEmpty(this Def d)
    {
        return d == null || d.defName.NullOrEmpty();
    }

    internal static List<T> ListAll<T>() where T : Def
    {
        return DefDatabase<T>.AllDefs.OrderBy(x => x.label).ToList();
    }

    internal static HashSet<string> ListModnamesWithNull<T>(Func<T, bool> condition = null) where T : Def
    {
        var stringSet = new HashSet<string>();
        stringSet.Add(null);
        stringSet.AddRange( condition == null ? ListModnames<T>() : ListModnames(condition));
        return stringSet;
    }

    internal static HashSet<string> ListModnames<T>(Func<T, bool> condition) where T : Def
    {
        return DefDatabase<T>.AllDefs.Where(x => x.modContentPack != null && condition(x)).OrderBy(x => x.modContentPack.Name).Select(x => x.modContentPack.Name).ToHashSet();
    }

    internal static HashSet<string> ListModnames<T>() where T : Def
    {
        return DefDatabase<T>.AllDefs.Where(x => x.modContentPack != null).OrderBy(x => x.modContentPack.Name).Select(x => x.modContentPack.Name).ToHashSet();
    }

    internal static HashSet<T> ListByModWithNull<T>(string name, Func<T, bool> condition = null) where T : Def
    {
        var objSet = new HashSet<T>();
        objSet.Add(default);
        objSet.AddRange(condition == null ? ListByMod<T>(name) : ListByMod(name, condition));
        return objSet;
    }

    internal static HashSet<T> ListByMod<T>(string name, Func<T, bool> condition) where T : Def
    {
        return DefDatabase<T>.AllDefs.Where(x => !x.IsNullOrEmpty() && (name.NullOrEmpty() || x.IsFromMod(name)) && condition(x)).OrderBy(x => x.label).ToHashSet();
    }

    internal static HashSet<T> ListByMod<T>(string name) where T : Def
    {
        return DefDatabase<T>.AllDefs.Where(x =>
        {
            if (x.IsNullOrEmpty())
                return false;
            return name.NullOrEmpty() || x.IsFromMod(name);
        }).OrderBy(x => x.label).ToHashSet();
    }

    internal static HashSet<T> ListBy<T>(Func<T, bool> condition) where T : Def
    {
        return DefDatabase<T>.AllDefs.Where(x => condition(x)).OrderBy(x => x.label).ToHashSet();
    }

    internal static HashSet<T> AllDefsWithName<T>(Func<T, bool> condition = null) where T : Def
    {
        return DefDatabase<T>.AllDefs.Where(x =>
        {
            if (x.defName.NullOrEmpty())
                return false;
            return condition == null || condition(x);
        }).OrderBy(x => x.defName).ToHashSet();
    }

    internal static HashSet<T> AllDefsWithLabel<T>(Func<T, bool> condition = null) where T : Def
    {
        return DefDatabase<T>.AllDefs.Where(x =>
        {
            if (x.defName.NullOrEmpty() || x.label.NullOrEmpty())
                return false;
            return condition == null || condition(x);
        }).OrderBy(x => x.label).ToHashSet();
    }

    internal static HashSet<T> AllDefsWithNameWithNull<T>(Func<T, bool> condition = null) where T : Def
    {
        var objSet = new HashSet<T>();
        objSet.Add(default);
        objSet.AddRange(AllDefsWithName(condition));
        return objSet;
    }

    internal static HashSet<T> AllDefsWithLabelWithNull<T>(Func<T, bool> condition = null) where T : Def
    {
        var objSet = new HashSet<T>();
        objSet.Add(default);
        objSet.AddRange(AllDefsWithLabel(condition));
        return objSet;
    }

    internal static T GetNext<T>(this HashSet<T> list, T current)
    {
        var index1 = list.IndexOf(current);
        var index2 = list.NextOrPrevIndex(index1, true, false);
        return list.ElementAt(index2);
    }

    internal static T GetPrev<T>(this HashSet<T> list, T current)
    {
        var index1 = list.IndexOf(current);
        var index2 = list.NextOrPrevIndex(index1, false, false);
        return list.ElementAt(index2);
    }

    internal static Func<ThingDef, bool> CONDITION_IS_ITEM(
        string modname,
        ThingCategoryDef tc,
        ThingCategory tc2)
    {
        return t =>
        {
            if (t.label.NullOrEmpty() || t.IsBlueprint || t.IsCorpse || t.IsFrame || t.isFrameInt != null || t.IsMinified() || t.projectile != null || t.mote != null || (!modname.NullOrEmpty() && !t.IsFromMod(modname)) || (tc != null && (t.thingCategories.NullOrEmpty() || !t.thingCategories.Contains(tc))))
                return false;
            return tc2 == ThingCategory.None || (tc2 == ThingCategory.Mineable && (t.IsMineableMineral() || t.IsMineableRock())) || (tc2 == ThingCategory.Animal && t.race != null && t.race.Animal) || (tc2 == ThingCategory.Mechanoid && t.race != null && t.race.IsMechanoid) || (tc2 == ThingCategory.HumanOrAlien && t.race != null && !t.race.IsMechanoid && !t.race.Animal) || t.category.ToString() == tc2.ToString();
        };
    }

    internal static Func<StatDef, bool> CONDITION_STATDEFS_APPAREL(
        List<StatCategoryDef> lnok)
    {
        return t => !lnok.Contains(t.category) || t.category == StatCategoryDefOf.Apparel;
    }

    internal static Func<StatDef, bool> CONDITION_STATDEFS_WEAPON(
        List<StatCategoryDef> lnok)
    {
        return t => !lnok.Contains(t.category) || t.category == StatCategoryDefOf.Weapon;
    }

    internal static Func<StatDef, bool> CONDITION_STATDEFS_GENE(
        List<StatCategoryDef> lnok)
    {
        return t => !lnok.Contains(t.category);
    }

    internal static string SLabel(this ThoughtStage t)
    {
        return !t.label.NullOrEmpty() ? t.label : "";
    }

    internal static string SLabel_PlusDefName(this Def d)
    {
        var str = d.SLabel();
        return (str == Label.NONE ? Label.ALL : str) + (d != null ? " [" + d.defName + "]" : "");
    }

    internal static string SLabel(this Def d)
    {
        if (d == null)
            return Label.NONE;
        var labelCap = d.LabelCap;
        if (!labelCap.NullOrEmpty())
            return d.LabelCap.ToString();
        return !d.label.NullOrEmpty() ? d.label : "";
    }

    internal static string SDefname(this Def d)
    {
        return d != null && !d.defName.NullOrEmpty() ? d.defName : "";
    }

    internal static string STooltip<T>(this T def)
    {
        if (def == null)
            return "";
        var str = "";
        if (typeof(T) == typeof(StatDef))
        {
            var statDef = def as StatDef;
            str = (statDef.label != null ? statDef.label.CapitalizeFirst() : "") + "\n" + (statDef.category == null || statDef.category.label == null ? "" : statDef.category.label.Colorize(Color.yellow)) + (statDef.category == null || statDef.category.defName == null ? "" : (" [" + statDef.category.defName + "]").Colorize(Color.gray)) + "\n\n" + (statDef.description.NullOrEmpty() ? "" : statDef.description);
        }
        else if (typeof(T) == typeof(GeneDef))
        {
            var geneDef = def as GeneDef;
            str = str + geneDef.DescriptionFull + "\n\n" + geneDef.Modname().Colorize(Color.gray);
        }
        else if (typeof(T) == typeof(Gene))
        {
            var gene = def as Gene;
            str = str + gene.def.DescriptionFull + "\n\n" + gene.def.Modname().Colorize(Color.gray);
        }
        else if (typeof(T) == typeof(HediffDef))
        {
            var hediffDef = def as HediffDef;
            str = str + hediffDef.Description + "\n\n" + hediffDef.Modname().Colorize(Color.gray);
        }
        else if (typeof(T) == typeof(XenotypeDef))
        {
            var xenotypeDef = def as XenotypeDef;
            str = str + (xenotypeDef.descriptionShort ?? xenotypeDef.description) + "\n\n" + xenotypeDef.Modname().Colorize(Color.gray);
        }
        else if (typeof(T) == typeof(GeneticTraitData))
        {
            str = TraitTool.GetGeneticTraitTooltip(def as GeneticTraitData);
        }
        else if (typeof(T) == typeof(Ability))
        {
            var ability = def as Ability;
            str = str + ability.def.GetTooltip() + "\n\n" + ability.def.Modname().Colorize(Color.gray);
        }
        else if (typeof(T) == typeof(AbilityDef))
        {
            var abilityDef = def as AbilityDef;
            str = str + abilityDef.GetTooltip() + "\n\n" + abilityDef.Modname().Colorize(Color.gray);
        }
        else if (typeof(T) == typeof(DamageDef))
        {
            var damageDef = def as DamageDef;
            str = str + (damageDef.description.NullOrEmpty() ? "" : damageDef.description + "\n\n") + damageDef.SDefname() + "\n" + damageDef.Modname().Colorize(Color.gray);
        }
        else if (typeof(T) == typeof(ResearchProjectDef))
        {
            str = (def as ResearchProjectDef).GetTip();
        }
        else if (IsDef<T>())
        {
            var t = def as Def;
            str = str + (t.description.NullOrEmpty() ? "" : t.description + "\n\n") + t.Modname().Colorize(Color.gray);
        }

        return str;
    }

    internal static bool IsDef<T>()
    {
        return typeof(T) == typeof(Def) || typeof(T).BaseType == typeof(Def) || (typeof(T).BaseType != null && typeof(T).BaseType != null && typeof(T).BaseType.BaseType == typeof(Def));
    }

    internal static string Modname(this Def t)
    {
        return t == null || t.modContentPack == null || t.modContentPack.Name == null ? "" : t.modContentPack.Name;
    }

    internal static HashSet<StatDef> StatDefs_Selection(
        HashSet<StatCategoryDef> lselectedCat)
    {
        return ListBy((Func<StatDef, bool>)(x =>
        {
            if (x.defName.NullOrEmpty())
                return false;
            return lselectedCat.NullOrEmpty() || lselectedCat.Contains(x.category) || x.category == null;
        }));
    }

    internal static HashSet<StatCategoryDef> StatCategoryDefs_Selection(
        HashSet<StatCategoryDef> lskip)
    {
        return ListByModWithNull(null, (Func<StatCategoryDef, bool>)(x => !x.defName.NullOrEmpty() && !lskip.Contains(x)));
    }

    internal static void RemoveCategoriesWithNoStatDef(this HashSet<StatCategoryDef> lcat)
    {
        var statDefSet = StatDefs_Selection(lcat);
        for (var index = lcat.Count - 1; index >= 0; --index)
        {
            var statCategoryDef = lcat.At(index);
            var flag = false;
            foreach (var statDef in statDefSet)
                if (statDef.category == statCategoryDef)
                {
                    flag = true;
                    break;
                }

            if (!flag)
                lcat.Remove(statCategoryDef);
        }
    }

    internal static bool DefNameComparator<T>(T d1, T d2) where T : Def
    {
        return d1?.defName == d2?.defName;
    }

    internal static void RandomSearchedDef<T>(ICollection<T> l, ref T def) where T : Def
    {
        def = l.RandomElement();
        SZWidgets.sFind = def.SLabel();
    }

    internal static Texture2D GetTIcon<T>(this T def, Selected s = null)
    {
        Texture2D texture2D = null;
        if (typeof(T) == typeof(ThingDef))
        {
            texture2D = (def as ThingDef).uiIcon;
        }
        else if (typeof(T) == typeof(Ability))
        {
            texture2D = (def as Ability).def.uiIcon;
        }
        else if (typeof(T) == typeof(AbilityDef))
        {
            texture2D = (def as AbilityDef).uiIcon;
        }
        else if (typeof(T) == typeof(HairDef))
        {
            texture2D = (def as HairDef).Icon;
        }
        else if (typeof(T) == typeof(BeardDef))
        {
            texture2D = (def as BeardDef).Icon;
        }
        else if (typeof(T) == typeof(GeneDef))
        {
            texture2D = (def as GeneDef).Icon;
        }
        else if (typeof(T) == typeof(XenotypeDef))
        {
            texture2D = (def as XenotypeDef).Icon;
        }
        else if (typeof(T) == typeof(TattooDef))
        {
            texture2D = (def as TattooDef).Icon;
        }
        else if (typeof(T) == typeof(ScenPart))
        {
            s = (def as ScenPart).GetSelectedScenarioPart();
            texture2D = SZWidgets.IconForStyleCustom(s, s.style);
        }
        else if (typeof(T) == typeof(ThingStyleDef))
        {
            texture2D = SZWidgets.IconForStyleCustom(s, def as ThingStyleDef);
        }
        else if (typeof(T) == typeof(ThingCategoryDef))
        {
            texture2D = (def as ThingCategoryDef).icon;
        }
        else if (typeof(T) == typeof(CustomXenotype))
        {
            texture2D = (def as CustomXenotype).IconDef?.Icon;
        }
        else if (typeof(T) == typeof(Gene))
        {
            texture2D = (def as Gene).def.Icon;
        }

        return texture2D == null ? null : texture2D.Equals(BaseContent.BadTex) ? null : texture2D;
    }

    internal static Color GetTColor<T>(this T def, ThingDef stuff = null)
    {
        if (typeof(T) == typeof(GeneDef))
            return (def as GeneDef).IconColor;
        if (typeof(T) == typeof(XenotypeDef))
            return RimWorld.XenotypeDef.IconColor;
        if (typeof(T) == typeof(ThingDef))
        {
            var thingDef = def as ThingDef;
            return stuff != null ? thingDef.GetColor(stuff) : thingDef.uiIconColor;
        }

        if (typeof(T) == typeof(Selected))
        {
            var selected = def as Selected;
            if(selected != null)
                return selected.thingDef.GetTColor(selected.stuff);
        }

        return typeof(T) == typeof(CustomXenotype) ? (def as CustomXenotype).inheritable != null ? RimWorld.XenotypeDef.IconColor : ColorTool.colSkyBlue : Color.white;
    }

    internal static T FindByDef<T>(this ICollection<T> l, T def) where T : Def
    {
        return l.EnumerableNullOrEmpty() ? default : l.Where(s => s.defName == def.defName).FirstOrFallback();
    }

    internal static T FindBy<T, TDef>(
        this ICollection<T> l,
        Func<T, TDef, bool> comparator,
        TDef def)
        where TDef : Def
    {
        return l.EnumerableNullOrEmpty() ? default : l.Where(s => comparator(s, def)).FirstOrFallback();
    }

    internal static T FindBy<T>(this ICollection<T> l, Func<T, T, bool> comparator, T obj)
    {
        return l.EnumerableNullOrEmpty() ? default : l.Where(s => comparator(s, obj)).FirstOrFallback();
    }

    internal static void SetMulti<T1, T, T2, T3>(
        ref List<T1> l,
        Func<T1, T, bool> comparator,
        Action<List<T1>, T1, T, T2, T3> valueSetter,
        T def,
        T2 value1,
        T3 value2)
        where T : Def
    {
        if (def == null)
            return;
        if (l == null)
            l = new List<T1>();
        var by = l.FindBy(comparator, def);
        valueSetter(l, by, def, value1, value2);
    }

    internal static void SetMulti<T1, T, T2, T3>(
        this List<T1> l,
        Func<T1, T, bool> comparator,
        Action<List<T1>, T1, T, T2, T3> valueSetter,
        T def,
        T2 value1,
        T3 value2)
        where T : Def
    {
        if (def == null)
            return;
        if (l == null)
            l = new List<T1>();
        var by = l.FindBy(comparator, def);
        valueSetter(l, by, def, value1, value2);
    }

    internal static void Set<T1, T, T2>(
        ref List<T1> l,
        Func<T1, T, bool> comparator,
        Action<List<T1>, T1, T, T2> valueSetter,
        T def,
        T2 value)
        where T : Def
    {
        if (def == null)
            return;
        if (l == null)
            l = new List<T1>();
        var by = l.FindBy(comparator, def);
        valueSetter(l, by, def, value);
    }

    internal static void Set<T1, T, T2>(
        this List<T1> l,
        Func<T1, T, bool> comparator,
        Action<List<T1>, T1, T, T2> valueSetter,
        T def,
        T2 value)
        where T : Def
    {
        if (def == null)
            return;
        if (l == null)
            l = new List<T1>();
        var by = l.FindBy(comparator, def);
        valueSetter(l, by, def, value);
    }

    internal static void Set<T1, T, T2>(
        ref List<T1> l,
        Func<T1, T1, bool> comparator,
        Action<List<T1>, T1, T, T2> valueSetter,
        T1 obj,
        T def,
        T2 value)
    {
        if (def == null)
            return;
        if (l == null)
            l = new List<T1>();
        var by = l.FindBy(comparator, obj);
        valueSetter(l, by, def, value);
    }

    internal static void Set<T1, T, T2>(
        this List<T1> l,
        Func<T1, T1, bool> comparator,
        Action<List<T1>, T1, T, T2> valueSetter,
        T1 obj,
        T def,
        T2 value)
    {
        if (def == null)
            return;
        if (l == null)
            l = new List<T1>();
        var by = l.FindBy(comparator, obj);
        valueSetter(l, by, def, value);
    }

    internal static void AddDef<T>(this List<T> l, T def) where T : Def
    {
        if (l == null)
            l = new List<T>();
        if (l.Contains(def)) return;
        l.Add(def);
    }

    internal static void SetStatModifier(
        List<StatModifier> l,
        StatModifier sm,
        StatDef def,
        float value)
    {
        if (sm != null)
        {
            sm.value = value;
        }
        else
        {
            sm = new StatModifier();
            sm.stat = def;
            sm.value = value;
            l.Add(sm);
        }
    }

    internal static void SetAptitude(List<Aptitude> l, Aptitude a, SkillDef def, int value)
    {
        if (a != null)
        {
            a.level = value;
        }
        else
        {
            a = new Aptitude();
            a.skill = def;
            a.level = value;
            l.Add(a);
        }
    }

    internal static void SetThingDefCountClass(
        List<ThingDefCountClass> l,
        ThingDefCountClass c,
        ThingDef def,
        int value)
    {
        if (c != null)
        {
            c.count = value;
        }
        else
        {
            c = new ThingDefCountClass();
            c.thingDef = def;
            c.count = value;
            l.Add(c);
        }
    }

    internal static void SetGeneticTraitData(
        List<GeneticTraitData> l,
        GeneticTraitData gtd,
        TraitDef def,
        int value)
    {
        if (gtd != null)
        {
            gtd.degree = value;
        }
        else
        {
            gtd = new GeneticTraitData();
            gtd.def = def;
            gtd.degree = value;
            l.Add(gtd);
        }
    }

    internal static void SetDamageFactor(
        List<DamageFactor> l,
        DamageFactor df,
        DamageDef def,
        float value)
    {
        if (df != null)
        {
            df.factor = value;
        }
        else
        {
            df = new DamageFactor();
            df.damageDef = def;
            df.factor = value;
            l.Add(df);
        }
    }

    internal static void SetPawnCapacityModifier(
        List<PawnCapacityModifier> l,
        PawnCapacityModifier pcm,
        PawnCapacityDef def,
        float offset,
        float factor)
    {
        if (pcm != null)
        {
            pcm.offset = offset;
            pcm.postFactor = factor;
        }
        else
        {
            pcm = new PawnCapacityModifier();
            pcm.capacity = def;
            pcm.offset = offset;
            pcm.postFactor = factor;
            l.Add(pcm);
        }
    }

    private delegate void GiveShortHash(Def def, Type defType, HashSet<ushort> takenHashes);
}
