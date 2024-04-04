using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor
{
	
	internal static class DefTool
	{
		
		internal static KeyBindingDef TryCreateKeyBinding(string defName, string keyCode, string label, string desc)
		{
			KeyBindingDef keyBindingDef = null;
			try
			{
				bool flag = !keyCode.NullOrEmpty();
				if (flag)
				{
					keyBindingDef = new KeyBindingDef();
					keyBindingDef.category = KeyBindingCategoryDefOf.MainTabs;
					keyBindingDef.defaultKeyCodeA = keyCode.AsKeyCode();
					keyBindingDef.defaultKeyCodeB = KeyCode.None;
					keyBindingDef.defName = defName;
					keyBindingDef.description = desc;
					keyBindingDef.label = label;
					keyBindingDef.ResolveReferences();
					keyBindingDef.PostLoad();
					DefTool.GiveShortHashTo(keyBindingDef, typeof(KeyBindingDef));
					DefDatabase<Verse.KeyBindingDef>.Add(keyBindingDef);
					KeyPrefs.KeyPrefsData.keyPrefs.Add(keyBindingDef, new KeyBindingData(keyBindingDef.defaultKeyCodeA, keyBindingDef.defaultKeyCodeB));
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.StackTrace);
			}
			return keyBindingDef;
		}

		
		internal static MainButtonDef GetCreateMainButton(string defName, string label, string desc, Type typeClass, ModContentPack pack, string keyCode, bool isVisible)
		{
			MainButtonDef mainButtonDef = DefTool.MainButtonDef(defName);
			bool flag = mainButtonDef == null;
			if (flag)
			{
				KeyBindingDef hotKey = DefTool.TryCreateKeyBinding(defName, keyCode, label, desc);
				mainButtonDef = new MainButtonDef();
				mainButtonDef.buttonVisible = isVisible;
				mainButtonDef.defName = defName;
				mainButtonDef.description = desc;
				mainButtonDef.label = label;
				mainButtonDef.tabWindowClass = typeClass;
				mainButtonDef.order = MainButtonDefOf.Menu.order - 1;
				mainButtonDef.hotKey = hotKey;
				mainButtonDef.validWithoutMap = false;
				mainButtonDef.modContentPack = pack;
				mainButtonDef.iconPath = "beditoricon";
				mainButtonDef.minimized = true;
				mainButtonDef.ResolveReferences();
				mainButtonDef.PostLoad();
				DefDatabase<RimWorld.MainButtonDef>.Add(mainButtonDef);
			}
			return mainButtonDef;
		}

		
		internal static void GiveShortHashTo(Def def, Type defType)
		{
			DefTool.giveShortHash(def, defType, (DefTool.takenShortHashes())[defType]);
		}

		
		internal static T GetDef<T>(string defName) where T : Def
		{
			return (!defName.NullOrEmpty()) ? DefDatabase<T>.GetNamed(defName, false) : default(T);
		}

		
		internal static BackstoryDef BackstoryDef(string defName)
		{
			return DefTool.GetDef<BackstoryDef>(defName);
		}

		
		internal static MainButtonDef MainButtonDef(string defName)
		{
			return DefTool.GetDef<MainButtonDef>(defName);
		}

		
		internal static PawnKindDef PawnKindDef(string defName)
		{
			return DefTool.GetDef<PawnKindDef>(defName);
		}

		
		internal static PawnKindDef PawnKindDef(string defName, PawnKindDef fallback)
		{
			return DefTool.PawnKindDef(defName) ?? fallback;
		}

		
		internal static PawnRelationDef PawnRelationDef(string defName)
		{
			return DefTool.GetDef<PawnRelationDef>(defName);
		}

		
		internal static BodyTypeDef BodyTypeDef(string defName)
		{
			return DefTool.GetDef<BodyTypeDef>(defName);
		}

		
		internal static BeardDef BeardDef(string defName)
		{
			return DefTool.GetDef<BeardDef>(defName);
		}

		
		internal static TattooDef TattooDef(string defName)
		{
			return DefTool.GetDef<TattooDef>(defName);
		}

		
		internal static AbilityDef AbilityDef(string defName)
		{
			return DefTool.GetDef<AbilityDef>(defName);
		}

		
		internal static GeneDef GeneDef(string defName)
		{
			return DefTool.GetDef<GeneDef>(defName);
		}

		
		internal static XenotypeDef XenotypeDef(string defName)
		{
			return DefTool.GetDef<XenotypeDef>(defName);
		}

		
		internal static HairDef HairDef(string defName)
		{
			return DefTool.GetDef<HairDef>(defName);
		}

		
		internal static HediffDef HediffDef(string defName)
		{
			return DefTool.GetDef<HediffDef>(defName);
		}

		
		internal static RecordDef RecordDef(string defName)
		{
			return DefTool.GetDef<RecordDef>(defName);
		}

		
		internal static SkillDef SkillDef(string defName)
		{
			return DefTool.GetDef<SkillDef>(defName);
		}

		
		internal static PawnCapacityDef PawnCapacityDef(string defName)
		{
			return DefTool.GetDef<PawnCapacityDef>(defName);
		}

		
		internal static ScenPartDef ScenPartDef(string defName)
		{
			return DefTool.GetDef<ScenPartDef>(defName);
		}

		
		internal static ThingDef ThingDef(string defName)
		{
			return DefTool.GetDef<ThingDef>(defName);
		}

		
		internal static ThingStyleDef ThingStyleDef(string defName)
		{
			return DefTool.GetDef<ThingStyleDef>(defName);
		}

		
		internal static ThingDef ThingDef(string defName, ThingDef fallback)
		{
			return DefTool.ThingDef(defName) ?? fallback;
		}

		
		internal static NeedDef NeedDef(string defName)
		{
			return DefTool.GetDef<NeedDef>(defName);
		}

		
		internal static ThoughtDef ThoughtDef(string defName)
		{
			return DefTool.GetDef<ThoughtDef>(defName);
		}

		
		internal static TraitDef TraitDef(string defName)
		{
			return DefTool.GetDef<TraitDef>(defName);
		}

		
		internal static WorkTypeDef WorkTypeDef(string defName)
		{
			return DefTool.GetDef<WorkTypeDef>(defName);
		}

		
		internal static StatDef StatDef(string defName)
		{
			return DefTool.GetDef<StatDef>(defName);
		}

		
		internal static JobDef JobDef(string defName)
		{
			return DefTool.GetDef<JobDef>(defName);
		}

		
		internal static SoundDef SoundDef(string defName)
		{
			return DefTool.GetDef<SoundDef>(defName);
		}

		
		internal static StuffCategoryDef StuffCategoryDef(string defName)
		{
			return DefTool.GetDef<StuffCategoryDef>(defName);
		}

		
		internal static BodyPartGroupDef BodyPartGroupDef(string defName)
		{
			return DefTool.GetDef<BodyPartGroupDef>(defName);
		}

		
		internal static ApparelLayerDef ApparelLayerDef(string defName)
		{
			return DefTool.GetDef<ApparelLayerDef>(defName);
		}

		
		internal static DesignatorDropdownGroupDef DesignatorDropdownGroupDef(string defName)
		{
			return DefTool.GetDef<DesignatorDropdownGroupDef>(defName);
		}

		
		internal static DesignationCategoryDef DesignationCategoryDef(string defName)
		{
			return DefTool.GetDef<DesignationCategoryDef>(defName);
		}

		
		internal static KeyBindingDef KeyBindingDef(string defName)
		{
			return DefTool.GetDef<KeyBindingDef>(defName);
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
			return (d != null && d.modContentPack != null) ? d.modContentPack.Name : "";
		}

		
		internal static bool IsNullOrEmpty(this Def d)
		{
			return d == null || d.defName.NullOrEmpty();
		}

		
		internal static List<T> ListAll<T>() where T : Def
		{
			return (from x in DefDatabase<T>.AllDefs
			orderby x.label
			select x).ToList<T>();
		}

		
		internal static HashSet<string> ListModnamesWithNull<T>(Func<T, bool> condition = null) where T : Def
		{
			HashSet<string> hashSet = new HashSet<string>();
			hashSet.Add(null);
			hashSet.AddRange((condition == null) ? DefTool.ListModnames<T>() : DefTool.ListModnames<T>(condition));
			return hashSet;
		}

		
		internal static HashSet<string> ListModnames<T>(Func<T, bool> condition) where T : Def
		{
			return (from x in DefDatabase<T>.AllDefs
			where x.modContentPack != null && condition(x)
			orderby x.modContentPack.Name
			select x.modContentPack.Name).ToHashSet<string>();
		}

		
		internal static HashSet<string> ListModnames<T>() where T : Def
		{
			return (from x in DefDatabase<T>.AllDefs
			where x.modContentPack != null
			orderby x.modContentPack.Name
			select x.modContentPack.Name).ToHashSet<string>();
		}

		
		internal static HashSet<T> ListByModWithNull<T>(string name, Func<T, bool> condition = null) where T : Def
		{
			HashSet<T> hashSet = new HashSet<T>();
			hashSet.Add(default(T));
			hashSet.AddRange((condition == null) ? DefTool.ListByMod<T>(name) : DefTool.ListByMod<T>(name, condition));
			return hashSet;
		}

		
		internal static HashSet<T> ListByMod<T>(string name, Func<T, bool> condition) where T : Def
		{
			return (from x in DefDatabase<T>.AllDefs
			where !x.IsNullOrEmpty() && (name.NullOrEmpty() || x.IsFromMod(name)) && condition(x)
			orderby x.label
			select x).ToHashSet<T>();
		}

		
		internal static HashSet<T> ListByMod<T>(string name) where T : Def
		{
			return (from x in DefDatabase<T>.AllDefs
			where !x.IsNullOrEmpty() && (name.NullOrEmpty() || x.IsFromMod(name))
			orderby x.label
			select x).ToHashSet<T>();
		}

		
		internal static HashSet<T> ListBy<T>(Func<T, bool> condition) where T : Def
		{
			return (from x in DefDatabase<T>.AllDefs
			where condition(x)
			orderby x.label
			select x).ToHashSet<T>();
		}

		
		internal static HashSet<T> AllDefsWithName<T>(Func<T, bool> condition = null) where T : Def
		{
			return (from x in DefDatabase<T>.AllDefs
			where !x.defName.NullOrEmpty() && (condition == null || condition(x))
			orderby x.defName
			select x).ToHashSet<T>();
		}

		
		internal static HashSet<T> AllDefsWithLabel<T>(Func<T, bool> condition = null) where T : Def
		{
			return (from x in DefDatabase<T>.AllDefs
			where !x.defName.NullOrEmpty() && !x.label.NullOrEmpty() && (condition == null || condition(x))
			orderby x.label
			select x).ToHashSet<T>();
		}

		
		internal static HashSet<T> AllDefsWithNameWithNull<T>(Func<T, bool> condition = null) where T : Def
		{
			HashSet<T> hashSet = new HashSet<T>();
			hashSet.Add(default(T));
			hashSet.AddRange(DefTool.AllDefsWithName<T>(condition));
			return hashSet;
		}

		
		internal static HashSet<T> AllDefsWithLabelWithNull<T>(Func<T, bool> condition = null) where T : Def
		{
			HashSet<T> hashSet = new HashSet<T>();
			hashSet.Add(default(T));
			hashSet.AddRange(DefTool.AllDefsWithLabel<T>(condition));
			return hashSet;
		}

		
		internal static T GetNext<T>(this HashSet<T> list, T current)
		{
			int index = list.IndexOf(current);
			index = list.NextOrPrevIndex(index, true, false);
			return list.ElementAt(index);
		}

		
		internal static T GetPrev<T>(this HashSet<T> list, T current)
		{
			int index = list.IndexOf(current);
			index = list.NextOrPrevIndex(index, false, false);
			return list.ElementAt(index);
		}

		
		
		internal static Func<ThingCategoryDef, bool> CONDITION_NO_CORPSE
		{
			get
			{
				return (ThingCategoryDef t) => !t.defName.ToLower().Contains("corpse");
			}
		}

		
		internal static Func<ThingDef, bool> CONDITION_IS_ITEM(string modname, ThingCategoryDef tc, ThingCategory tc2)
		{
			return (ThingDef t) => !t.label.NullOrEmpty() && !t.IsBlueprint && !t.IsCorpse && !t.IsFrame && !t.isFrameInt && !t.IsMinified() && t.projectile == null && t.mote == null && (modname.NullOrEmpty() || t.IsFromMod(modname)) && (tc == null || (!t.thingCategories.NullOrEmpty<ThingCategoryDef>() && t.thingCategories.Contains(tc))) && (tc2 == ThingCategory.None || (tc2 == ThingCategory.Mineable && (t.IsMineableMineral() || t.IsMineableRock())) || (tc2 == ThingCategory.Animal && t.race != null && t.race.Animal) || (tc2 == ThingCategory.Mechanoid && t.race != null && t.race.IsMechanoid) || (tc2 == ThingCategory.HumanOrAlien && t.race != null && !t.race.IsMechanoid && !t.race.Animal) || t.category.ToString() == tc2.ToString());
		}

		
		internal static Func<StatDef, bool> CONDITION_STATDEFS_APPAREL(List<StatCategoryDef> lnok)
		{
			return (StatDef t) => !lnok.Contains(t.category) || t.category == StatCategoryDefOf.Apparel;
		}

		
		internal static Func<StatDef, bool> CONDITION_STATDEFS_WEAPON(List<StatCategoryDef> lnok)
		{
			return (StatDef t) => !lnok.Contains(t.category) || t.category == StatCategoryDefOf.Weapon;
		}

		
		internal static Func<StatDef, bool> CONDITION_STATDEFS_GENE(List<StatCategoryDef> lnok)
		{
			return (StatDef t) => !lnok.Contains(t.category);
		}

		
		internal static string SLabel(this ThoughtStage t)
		{
			return t.label.NullOrEmpty() ? "" : t.label;
		}

		
		internal static string SLabel_PlusDefName(this Def d)
		{
			string text = d.SLabel();
			text = ((text == Label.NONE) ? Label.ALL : text);
			return text + ((d != null) ? (" [" + d.defName + "]") : "");
		}

		
		internal static string SLabel(this Def d)
		{
			return (d == null) ? Label.NONE : (d.LabelCap.NullOrEmpty() ? (d.label.NullOrEmpty() ? "" : d.label) : d.LabelCap.ToString());
		}

		
		internal static string SDefname(this Def d)
		{
			return (d == null || d.defName.NullOrEmpty()) ? "" : d.defName;
		}

		
		internal static string STooltip<T>(this T def)
		{
			bool flag = def == null;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				string text = "";
				bool flag2 = typeof(T) == typeof(StatDef);
				if (flag2)
				{
					StatDef statDef = def as StatDef;
					text = ((statDef.label != null) ? statDef.label.CapitalizeFirst() : "");
					text += "\n";
					text += ((statDef.category != null && statDef.category.label != null) ? statDef.category.label.Colorize(Color.yellow) : "");
					text += ((statDef.category != null && statDef.category.defName != null) ? (" [" + statDef.category.defName + "]").Colorize(Color.gray) : "");
					text += "\n\n";
					text += (statDef.description.NullOrEmpty() ? "" : statDef.description);
				}
				else
				{
					bool flag3 = typeof(T) == typeof(GeneDef);
					if (flag3)
					{
						GeneDef geneDef = def as GeneDef;
						text += geneDef.DescriptionFull;
						text += "\n\n";
						text += geneDef.Modname().Colorize(Color.gray);
					}
					else
					{
						bool flag4 = typeof(T) == typeof(Gene);
						if (flag4)
						{
							Gene gene = def as Gene;
							text += gene.def.DescriptionFull;
							text += "\n\n";
							text += gene.def.Modname().Colorize(Color.gray);
						}
						else
						{
							bool flag5 = typeof(T) == typeof(HediffDef);
							if (flag5)
							{
								HediffDef hediffDef = def as HediffDef;
								text += hediffDef.Description;
								text += "\n\n";
								text += hediffDef.Modname().Colorize(Color.gray);
							}
							else
							{
								bool flag6 = typeof(T) == typeof(XenotypeDef);
								if (flag6)
								{
									XenotypeDef xenotypeDef = def as XenotypeDef;
									text += (xenotypeDef.descriptionShort ?? xenotypeDef.description);
									text += "\n\n";
									text += xenotypeDef.Modname().Colorize(Color.gray);
								}
								else
								{
									bool flag7 = typeof(T) == typeof(GeneticTraitData);
									if (flag7)
									{
										GeneticTraitData gtd = def as GeneticTraitData;
										text = TraitTool.GetGeneticTraitTooltip(gtd);
									}
									else
									{
										bool flag8 = typeof(T) == typeof(Ability);
										if (flag8)
										{
											Ability ability = def as Ability;
											text += ability.def.GetTooltip(null);
											text += "\n\n";
											text += ability.def.Modname().Colorize(Color.gray);
										}
										else
										{
											bool flag9 = typeof(T) == typeof(AbilityDef);
											if (flag9)
											{
												AbilityDef abilityDef = def as AbilityDef;
												text += abilityDef.GetTooltip(null);
												text += "\n\n";
												text += abilityDef.Modname().Colorize(Color.gray);
											}
											else
											{
												bool flag10 = typeof(T) == typeof(DamageDef);
												if (flag10)
												{
													DamageDef damageDef = def as DamageDef;
													text += (damageDef.description.NullOrEmpty() ? "" : (damageDef.description + "\n\n"));
													text = text + damageDef.SDefname() + "\n";
													text += damageDef.Modname().Colorize(Color.gray);
												}
												else
												{
													bool flag11 = typeof(T) == typeof(ResearchProjectDef);
													if (flag11)
													{
														ResearchProjectDef researchProjectDef = def as ResearchProjectDef;
														text = researchProjectDef.GetTip();
													}
													else
													{
														bool flag12 = DefTool.IsDef<T>();
														if (flag12)
														{
															Def def2 = def as Def;
															text += (def2.description.NullOrEmpty() ? "" : (def2.description + "\n\n"));
															text += def2.Modname().Colorize(Color.gray);
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				result = text;
			}
			return result;
		}

		
		internal static bool IsDef<T>()
		{
			return typeof(T) == typeof(Def) || typeof(T).BaseType == typeof(Def) || (typeof(T).BaseType != null && typeof(T).BaseType.BaseType == typeof(Def));
		}

		
		
		internal static Func<StatCategoryDef, string> CategoryLabel
		{
			get
			{
				return (StatCategoryDef cat) => cat.SLabel_PlusDefName();
			}
		}

		
		internal static string Modname(this Def t)
		{
			return (t != null && t.modContentPack != null && t.modContentPack.Name != null) ? t.modContentPack.Name : "";
		}

		
		internal static HashSet<StatDef> StatDefs_Selection(HashSet<StatCategoryDef> lselectedCat)
		{
			return DefTool.ListBy<StatDef>((StatDef x) => !x.defName.NullOrEmpty() && (lselectedCat.NullOrEmpty<StatCategoryDef>() || lselectedCat.Contains(x.category) || x.category == null));
		}

		
		internal static HashSet<StatCategoryDef> StatCategoryDefs_Selection(HashSet<StatCategoryDef> lskip)
		{
			return DefTool.ListByModWithNull<StatCategoryDef>(null, (StatCategoryDef x) => !x.defName.NullOrEmpty() && !lskip.Contains(x));
		}

		
		internal static void RemoveCategoriesWithNoStatDef(this HashSet<StatCategoryDef> lcat)
		{
			HashSet<StatDef> hashSet = DefTool.StatDefs_Selection(lcat);
			for (int i = lcat.Count - 1; i >= 0; i--)
			{
				StatCategoryDef statCategoryDef = lcat.At(i);
				bool flag = false;
				foreach (StatDef statDef in hashSet)
				{
					bool flag2 = statDef.category == statCategoryDef;
					if (flag2)
					{
						flag = true;
						break;
					}
				}
				bool flag3 = !flag;
				if (flag3)
				{
					lcat.Remove(statCategoryDef);
				}
			}
		}

		
		internal static bool DefNameComparator<T>(T d1, T d2) where T : Def
		{
			T t = d1;
			string a = (t != null) ? t.defName : null;
			T t2 = d2;
			return a == ((t2 != null) ? t2.defName : null);
		}

		
		
		internal static Func<StatModifier, StatDef> DefGetterStatModifier
		{
			get
			{
				return (StatModifier sm) => sm.stat;
			}
		}

		
		
		internal static Func<DamageFactor, DamageDef> DefGetterDamageFacotr
		{
			get
			{
				return (DamageFactor df) => df.damageDef;
			}
		}

		
		
		internal static Func<GeneticTraitData, TraitDef> DefGetterGeneticTraitData
		{
			get
			{
				return (GeneticTraitData gtd) => gtd.def;
			}
		}

		
		
		internal static Func<Aptitude, SkillDef> DefGetterAptitude
		{
			get
			{
				return (Aptitude a) => a.skill;
			}
		}

		
		
		internal static Func<ThingDefCountClass, ThingDef> DefGetterThingDefCountClass
		{
			get
			{
				return (ThingDefCountClass t) => t.thingDef;
			}
		}

		
		
		internal static Func<DamageFactor, DamageDef> DefGetterDamageFactor
		{
			get
			{
				return (DamageFactor d) => d.damageDef;
			}
		}

		
		
		internal static Func<PawnCapacityModifier, PawnCapacityDef> DefGetterPawnCapacityModifier
		{
			get
			{
				return (PawnCapacityModifier pcm) => pcm.capacity;
			}
		}

		
		
		internal static Func<StatModifier, float> ValGetterStatModifier
		{
			get
			{
				return (StatModifier sm) => sm.value;
			}
		}

		
		
		internal static Func<DamageFactor, float> ValGetterDamageFacotr
		{
			get
			{
				return (DamageFactor df) => df.factor;
			}
		}

		
		
		internal static Func<GeneticTraitData, int> ValGetterGeneticTraitData
		{
			get
			{
				return (GeneticTraitData gtd) => gtd.degree;
			}
		}

		
		
		internal static Func<Aptitude, int> ValGetterAptitude
		{
			get
			{
				return (Aptitude a) => a.level;
			}
		}

		
		
		internal static Func<ThingDefCountClass, int> ValGetterThingDefCountClass
		{
			get
			{
				return (ThingDefCountClass t) => t.count;
			}
		}

		
		
		internal static Func<DamageFactor, float> ValGetterDamageFactor
		{
			get
			{
				return (DamageFactor d) => d.factor;
			}
		}

		
		
		internal static Func<PawnCapacityModifier, float> ValGetterPCMoffset
		{
			get
			{
				return (PawnCapacityModifier pcm) => pcm.offset;
			}
		}

		
		
		internal static Func<PawnCapacityModifier, float> ValGetterPCMfactor
		{
			get
			{
				return (PawnCapacityModifier pcm) => pcm.postFactor;
			}
		}

		
		internal static void RandomSearchedDef<T>(ICollection<T> l, ref T def) where T : Def
		{
			def = l.RandomElement<T>();
			SZWidgets.sFind = def.SLabel();
		}

		
		internal static Texture2D GetTIcon<T>(this T def, Selected s = null)
		{
			Texture2D texture2D = null;
			bool flag = typeof(T) == typeof(ThingDef);
			if (flag)
			{
				texture2D = (def as ThingDef).uiIcon;
			}
			else
			{
				bool flag2 = typeof(T) == typeof(Ability);
				if (flag2)
				{
					texture2D = (def as Ability).def.uiIcon;
				}
				else
				{
					bool flag3 = typeof(T) == typeof(AbilityDef);
					if (flag3)
					{
						texture2D = (def as AbilityDef).uiIcon;
					}
					else
					{
						bool flag4 = typeof(T) == typeof(HairDef);
						if (flag4)
						{
							texture2D = (def as HairDef).Icon;
						}
						else
						{
							bool flag5 = typeof(T) == typeof(BeardDef);
							if (flag5)
							{
								texture2D = (def as BeardDef).Icon;
							}
							else
							{
								bool flag6 = typeof(T) == typeof(GeneDef);
								if (flag6)
								{
									texture2D = (def as GeneDef).Icon;
								}
								else
								{
									bool flag7 = typeof(T) == typeof(XenotypeDef);
									if (flag7)
									{
										texture2D = (def as XenotypeDef).Icon;
									}
									else
									{
										bool flag8 = typeof(T) == typeof(TattooDef);
										if (flag8)
										{
											texture2D = (def as TattooDef).Icon;
										}
										else
										{
											bool flag9 = typeof(T) == typeof(ScenPart);
											if (flag9)
											{
												s = (def as ScenPart).GetSelectedScenarioPart<ScenPart>();
												texture2D = SZWidgets.IconForStyleCustom(s, s.style);
											}
											else
											{
												bool flag10 = typeof(T) == typeof(ThingStyleDef);
												if (flag10)
												{
													texture2D = SZWidgets.IconForStyleCustom(s, def as ThingStyleDef);
												}
												else
												{
													bool flag11 = typeof(T) == typeof(ThingCategoryDef);
													if (flag11)
													{
														texture2D = (def as ThingCategoryDef).icon;
													}
													else
													{
														bool flag12 = typeof(T) == typeof(CustomXenotype);
														if (flag12)
														{
															XenotypeIconDef iconDef = (def as CustomXenotype).IconDef;
															texture2D = ((iconDef != null) ? iconDef.Icon : null);
														}
														else
														{
															bool flag13 = typeof(T) == typeof(Gene);
															if (flag13)
															{
																texture2D = (def as Gene).def.Icon;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return (texture2D == BaseContent.BadTex) ? null : texture2D;
		}

		
		internal static Color GetTColor<T>(this T def, ThingDef stuff = null)
		{
			bool flag = typeof(T) == typeof(GeneDef);
			Color result;
			if (flag)
			{
				result = (def as GeneDef).IconColor;
			}
			else
			{
				bool flag2 = typeof(T) == typeof(XenotypeDef);
				if (flag2)
				{
					result = RimWorld.XenotypeDef.IconColor;
				}
				else
				{
					bool flag3 = typeof(T) == typeof(ThingDef);
					if (flag3)
					{
						ThingDef thingDef = def as ThingDef;
						bool flag4 = stuff != null;
						if (flag4)
						{
							result = thingDef.GetColor(stuff);
						}
						else
						{
							result = thingDef.uiIconColor;
						}
					}
					else
					{
						bool flag5 = typeof(T) == typeof(Selected);
						if (flag5)
						{
							Selected selected = def as Selected;
							result = selected.thingDef.GetTColor(selected.stuff);
						}
						else
						{
							bool flag6 = typeof(T) == typeof(CustomXenotype);
							if (flag6)
							{
								CustomXenotype customXenotype = def as CustomXenotype;
								result = (customXenotype.inheritable ? RimWorld.XenotypeDef.IconColor : ColorTool.colSkyBlue);
							}
							else
							{
								result = Color.white;
							}
						}
					}
				}
			}
			return result;
		}

		
		internal static T FindByDef<T>(this ICollection<T> l, T def) where T : Def
		{
			return (!l.EnumerableNullOrEmpty<T>()) ? (from s in l
			where s.defName == def.defName
			select s).FirstOrFallback(default(T)) : default(T);
		}

		
		internal static T FindBy<T, TDef>(this ICollection<T> l, Func<T, TDef, bool> comparator, TDef def) where TDef : Def
		{
			return (!l.EnumerableNullOrEmpty<T>()) ? (from s in l
			where comparator(s, def)
			select s).FirstOrFallback(default(T)) : default(T);
		}

		
		internal static T FindBy<T>(this ICollection<T> l, Func<T, T, bool> comparator, T obj)
		{
			return (!l.EnumerableNullOrEmpty<T>()) ? (from s in l
			where comparator(s, obj)
			select s).FirstOrFallback(default(T)) : default(T);
		}

		
		internal static void SetMulti<T1, T, T2, T3>(ref List<T1> l, Func<T1, T, bool> comparator, Action<List<T1>, T1, T, T2, T3> valueSetter, T def, T2 value1, T3 value2) where T : Def
		{
			bool flag = def == null;
			if (!flag)
			{
				bool flag2 = l == null;
				if (flag2)
				{
					l = new List<T1>();
				}
				T1 arg = l.FindBy(comparator, def);
				valueSetter(l, arg, def, value1, value2);
			}
		}

		
		internal static void SetMulti<T1, T, T2, T3>(this List<T1> l, Func<T1, T, bool> comparator, Action<List<T1>, T1, T, T2, T3> valueSetter, T def, T2 value1, T3 value2) where T : Def
		{
			bool flag = def == null;
			if (!flag)
			{
				bool flag2 = l == null;
				if (flag2)
				{
					l = new List<T1>();
				}
				T1 arg = l.FindBy(comparator, def);
				valueSetter(l, arg, def, value1, value2);
			}
		}

		
		internal static void Set<T1, T, T2>(ref List<T1> l, Func<T1, T, bool> comparator, Action<List<T1>, T1, T, T2> valueSetter, T def, T2 value) where T : Def
		{
			bool flag = def == null;
			if (!flag)
			{
				bool flag2 = l == null;
				if (flag2)
				{
					l = new List<T1>();
				}
				T1 arg = l.FindBy(comparator, def);
				valueSetter(l, arg, def, value);
			}
		}

		
		internal static void Set<T1, T, T2>(this List<T1> l, Func<T1, T, bool> comparator, Action<List<T1>, T1, T, T2> valueSetter, T def, T2 value) where T : Def
		{
			bool flag = def == null;
			if (!flag)
			{
				bool flag2 = l == null;
				if (flag2)
				{
					l = new List<T1>();
				}
				T1 arg = l.FindBy(comparator, def);
				valueSetter(l, arg, def, value);
			}
		}

		
		internal static void Set<T1, T, T2>(ref List<T1> l, Func<T1, T1, bool> comparator, Action<List<T1>, T1, T, T2> valueSetter, T1 obj, T def, T2 value)
		{
			bool flag = def == null;
			if (!flag)
			{
				bool flag2 = l == null;
				if (flag2)
				{
					l = new List<T1>();
				}
				T1 arg = l.FindBy(comparator, obj);
				valueSetter(l, arg, def, value);
			}
		}

		
		internal static void Set<T1, T, T2>(this List<T1> l, Func<T1, T1, bool> comparator, Action<List<T1>, T1, T, T2> valueSetter, T1 obj, T def, T2 value)
		{
			bool flag = def == null;
			if (!flag)
			{
				bool flag2 = l == null;
				if (flag2)
				{
					l = new List<T1>();
				}
				T1 arg = l.FindBy(comparator, obj);
				valueSetter(l, arg, def, value);
			}
		}

		
		internal static void AddDef<T>(this List<T> l, T def) where T : Def
		{
			bool flag = l == null;
			if (flag)
			{
				l = new List<T>();
			}
			bool flag2 = !l.Contains(def);
			if (flag2)
			{
				l.Add(def);
			}
		}

		
		internal static void SetStatModifier(List<StatModifier> l, StatModifier sm, StatDef def, float value)
		{
			bool flag = sm != null;
			if (flag)
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
			bool flag = a != null;
			if (flag)
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

		
		internal static void SetThingDefCountClass(List<ThingDefCountClass> l, ThingDefCountClass c, ThingDef def, int value)
		{
			bool flag = c != null;
			if (flag)
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

		
		internal static void SetGeneticTraitData(List<GeneticTraitData> l, GeneticTraitData gtd, TraitDef def, int value)
		{
			bool flag = gtd != null;
			if (flag)
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

		
		internal static void SetDamageFactor(List<DamageFactor> l, DamageFactor df, DamageDef def, float value)
		{
			bool flag = df != null;
			if (flag)
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

		
		internal static void SetPawnCapacityModifier(List<PawnCapacityModifier> l, PawnCapacityModifier pcm, PawnCapacityDef def, float offset, float factor)
		{
			bool flag = pcm != null;
			if (flag)
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

		
		
		internal static Func<DamageFactor, DamageDef, bool> CompareDamageFactor
		{
			get
			{
				return (DamageFactor a, DamageDef b) => a.damageDef.defName == b.defName;
			}
		}

		
		
		internal static Func<StatModifier, StatDef, bool> CompareStatModifier
		{
			get
			{
				return (StatModifier a, StatDef b) => a.stat.defName == b.defName;
			}
		}

		
		
		internal static Func<ThingDefCountClass, ThingDef, bool> CompareThingDefCountClass
		{
			get
			{
				return (ThingDefCountClass a, ThingDef b) => a.thingDef.defName == b.defName;
			}
		}

		
		
		internal static Func<Aptitude, SkillDef, bool> CompareAptitude
		{
			get
			{
				return (Aptitude a, SkillDef b) => a.skill.defName == b.defName;
			}
		}

		
		
		internal static Func<GeneticTraitData, GeneticTraitData, bool> CompareGeneticTraitData
		{
			get
			{
				return (GeneticTraitData a, GeneticTraitData b) => b != null && a.def.defName == b.def.defName && a.degree == b.degree;
			}
		}

		
		
		internal static Func<PawnCapacityModifier, PawnCapacityDef, bool> ComparePawnCapacityModifier
		{
			get
			{
				return (PawnCapacityModifier a, PawnCapacityDef b) => a.capacity.defName == b.defName;
			}
		}

		
		
		internal static Func<StatDef, StatCategoryDef, bool> CompareStatCategoryNot
		{
			get
			{
				return (StatDef a, StatCategoryDef b) => a.category != b;
			}
		}

		
		// Note: this type is marked as 'beforefieldinit'.
		static DefTool()
		{
		}

		
		private static readonly DefTool.GiveShortHash giveShortHash = AccessTools.MethodDelegate<DefTool.GiveShortHash>(AccessTools.Method(typeof(ShortHashGiver), "GiveShortHash", null, null), null, true);

		
		internal static readonly AccessTools.FieldRef<Dictionary<Type, HashSet<ushort>>> takenShortHashes = AccessTools.StaticFieldRefAccess<Dictionary<Type, HashSet<ushort>>>(AccessTools.Field(typeof(ShortHashGiver), "takenHashesPerDeftype"));

		
		// (Invoke) Token: 0x06000851 RID: 2129
		private delegate void GiveShortHash(Def def, Type defType, HashSet<ushort> takenHashes);

	}
}
