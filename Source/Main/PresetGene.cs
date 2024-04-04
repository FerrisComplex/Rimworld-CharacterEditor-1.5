using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace CharacterEditor
{
	
	internal class PresetGene
	{
		
		
		internal static OptionS optionS
		{
			get
			{
				return OptionS.CUSTOMGENE;
			}
		}

		
		
		public string AsString
		{
			get
			{
				return Preset.AsString<PresetGene.Param>(this.dicParams);
			}
		}

		
		public void SaveCustom()
		{
			bool flag = this.def != null;
			if (flag)
			{
				CEditor.API.SetCustom(PresetGene.optionS, this.AsString, this.def.defName);
				MessageTool.Show(this.def.defName + " " + Label.SETTINGSSAVED, null);
			}
		}

		
		
		public static Dictionary<string, PresetGene> AllDefaults
		{
			get
			{
				return CEditor.API.Get<Dictionary<string, PresetGene>>(EType.GenePreset);
			}
		}

		
		
		public static HashSet<GeneDef> ListAll
		{
			get
			{
				return DefTool.ListBy<GeneDef>((GeneDef x) => !x.defName.NullOrEmpty());
			}
		}

		
		public static Dictionary<string, PresetGene> CreateDefaults()
		{
			return Preset.CreateDefaults<PresetGene, GeneDef>(PresetGene.ListAll, (GeneDef sample) => sample.defName, (GeneDef x) => new PresetGene(x), "genes");
		}

		
		public static void LoadAllModifications(string custom)
		{
			Preset.LoadAllModifications(custom, delegate(string s)
			{
				new PresetGene(s);
			}, "genes");
		}

		
		public static void ResetAllToDefaults()
		{
			Preset.ResetAllToDefault<PresetGene>(PresetGene.AllDefaults, delegate(PresetGene p)
			{
				p.FromDictionary();
			}, PresetGene.optionS, "genes");
		}

		
		public static void ResetToDefault(string defName)
		{
			Preset.ResetToDefault<PresetGene>(PresetGene.AllDefaults, delegate(PresetGene p)
			{
				p.FromDictionary();
			}, PresetGene.optionS, defName);
		}

		
		public static void SaveModification(GeneDef s)
		{
			new PresetGene(s).SaveCustom();
		}

		
		internal PresetGene(GeneDef g)
		{
			bool flag = g == null;
			if (!flag)
			{
				this.def = g;
				this.dicParams = new SortedDictionary<PresetGene.Param, string>();
				try
				{
					this.dicParams.Add(PresetGene.Param.P00_defName, this.def.defName);
					this.dicParams.Add(PresetGene.Param.P01_label, this.def.label);
					this.dicParams.Add(PresetGene.Param.P02_statFactors, this.def.statFactors.ListToString<StatModifier>());
					this.dicParams.Add(PresetGene.Param.P03_statOffsets, this.def.statOffsets.ListToString<StatModifier>());
					this.dicParams.Add(PresetGene.Param.P04_aptitudes, this.def.aptitudes.ListToString<Aptitude>());
					this.dicParams.Add(PresetGene.Param.P05_capacities, this.def.capMods.ListToString<PawnCapacityModifier>());
					this.dicParams.Add(PresetGene.Param.P06_abilities, this.def.abilities.ListToString<AbilityDef>());
					this.dicParams.Add(PresetGene.Param.P07_traits, this.def.forcedTraits.ListToString<GeneticTraitData>());
					this.dicParams.Add(PresetGene.Param.P08_suppressedTraits, this.def.suppressedTraits.ListToString<GeneticTraitData>());
					this.dicParams.Add(PresetGene.Param.P09_immunities, this.def.makeImmuneTo.ListToString<HediffDef>());
					this.dicParams.Add(PresetGene.Param.P10_protections, this.def.hediffGiversCannotGive.ListToString<HediffDef>());
					this.dicParams.Add(PresetGene.Param.P11_disabledNeeds, this.def.disablesNeeds.ListToString<NeedDef>());
					SortedDictionary<PresetGene.Param, string> sortedDictionary = this.dicParams;
					PresetGene.Param key = PresetGene.Param.P12_disabledWorkTags;
					int num = (int)this.def.disabledWorkTags;
					sortedDictionary.Add(key, num.ToString());
					this.dicParams.Add(PresetGene.Param.P13_damageFactors, this.def.damageFactors.ListToString<DamageFactor>());
					this.dicParams.Add(PresetGene.Param.P14_causedNeeds, this.def.causesNeed.SDefname());
					this.dicParams.Add(PresetGene.Param.P15_chemical, this.def.chemical.SDefname());
					this.dicParams.Add(PresetGene.Param.P16_forcedHair, this.def.forcedHair.SDefname());
					this.dicParams.Add(PresetGene.Param.P17_hairColorOverride, this.def.hairColorOverride.NullableColorHexString());
					this.dicParams.Add(PresetGene.Param.P18_skinColorBase, this.def.skinColorBase.NullableColorHexString());
					this.dicParams.Add(PresetGene.Param.P19_skinColorOverride, this.def.skinColorOverride.NullableColorHexString());
					this.dicParams.Add(PresetGene.Param.P20_biostatArc, this.def.biostatArc.ToString());
					this.dicParams.Add(PresetGene.Param.P21_biostatCpx, this.def.biostatCpx.ToString());
					this.dicParams.Add(PresetGene.Param.P22_biostatMet, this.def.biostatMet.ToString());
					this.dicParams.Add(PresetGene.Param.P23_addictionChanceFactor, this.def.addictionChanceFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P24_mentalBreakChanceFactor, this.def.aggroMentalBreakSelectionChanceFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P25_foodPoisioningChanceFactor, this.def.foodPoisoningChanceFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P26_lovinMTBFactor, this.def.lovinMTBFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P27_marketValueFactor, this.def.marketValueFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P28_mentalBreakMtbDays, this.def.mentalBreakMtbDays.ToString());
					this.dicParams.Add(PresetGene.Param.P29_minAgeActive, this.def.minAgeActive.ToString());
					this.dicParams.Add(PresetGene.Param.P30_minMelanin, this.def.minMelanin.ToString());
					this.dicParams.Add(PresetGene.Param.P31_missingGeneRomanceChanceFactor, this.def.missingGeneRomanceChanceFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P32_painFactor, this.def.painFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P33_painOffset, this.def.painOffset.ToString());
					this.dicParams.Add(PresetGene.Param.P34_prisonBreakMtbFactor, this.def.prisonBreakMTBFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P35_randomBrightnessFactor, this.def.randomBrightnessFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P36_resourceLossPerDay, this.def.resourceLossPerDay.ToString());
					this.dicParams.Add(PresetGene.Param.P37_selectionWeight, this.def.selectionWeight.ToString());
					this.dicParams.Add(PresetGene.Param.P38_selectionWeightFactorDarkSkin, this.def.selectionWeightFactorDarkSkin.ToString());
					this.dicParams.Add(PresetGene.Param.P39_socialFightChanceFactor, this.def.socialFightChanceFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P40_canGenerateInGeneSet, this.def.canGenerateInGeneSet.ToString());
					this.dicParams.Add(PresetGene.Param.P41_dislikesSunLight, this.def.dislikesSunlight.ToString());
					this.dicParams.Add(PresetGene.Param.P42_dontMindRawFood, this.def.dontMindRawFood.ToString());
					this.dicParams.Add(PresetGene.Param.P43_ignoreDarkness, this.def.ignoreDarkness.ToString());
					this.dicParams.Add(PresetGene.Param.P44_immuneToToxGasExposure, this.def.immuneToToxGasExposure.ToString());
					this.dicParams.Add(PresetGene.Param.P45_neverGrayHair, this.def.neverGrayHair.ToString());
					this.dicParams.Add(PresetGene.Param.P46_prevenetPermanent_Wounds, this.def.preventPermanentWounds.ToString());
					this.dicParams.Add(PresetGene.Param.P47_randomChosen, this.def.randomChosen.ToString());
					this.dicParams.Add(PresetGene.Param.P48_removeOnRedress, this.def.removeOnRedress.ToString());
					this.dicParams.Add(PresetGene.Param.P49_showGizmoOnWorldView, this.def.showGizmoOnWorldView.ToString());
					this.dicParams.Add(PresetGene.Param.P50_sterilize, this.def.sterilize.ToString());
					this.dicParams.Add(PresetGene.Param.P51_womenCanHaveBeards, this.def.womenCanHaveBeards.ToString());
					this.dicParams.Add(PresetGene.Param.P52_overdoseChanceFactor, this.def.overdoseChanceFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P53_toleranceBuildupFactor, this.def.toleranceBuildupFactor.ToString());
					this.dicParams.Add(PresetGene.Param.P54_displayOrderInCategory, this.def.displayOrderInCategory.ToString());
					this.dicParams.Add(PresetGene.Param.P55_passOnDirectly, this.def.passOnDirectly.ToString());
					this.dicParams.Add(PresetGene.Param.P56_showGizmoWhenDrafted, this.def.showGizmoWhenDrafted.ToString());
					this.dicParams.Add(PresetGene.Param.P57_showGizmoOnMultiSelect, this.def.showGizmoOnMultiSelect.ToString());
					this.dicParams.Add(PresetGene.Param.P58_soundCall, this.def.soundCall.SDefname());
					this.dicParams.Add(PresetGene.Param.P59_soundDeath, this.def.soundDeath.SDefname());
					this.dicParams.Add(PresetGene.Param.P60_soundWounded, this.def.soundWounded.SDefname());
					this.dicParams.Add(PresetGene.Param.P61_historyEventDef, this.def.deathHistoryEvent.SDefname());
					this.dicParams.Add(PresetGene.Param.P62_prerequisiteDef, this.def.prerequisite.SDefname());
					this.dicParams.Add(PresetGene.Param.P63_labelAdj, this.def.labelShortAdj);
					this.dicParams.Add(PresetGene.Param.P64_iconPath, this.def.iconPath);
					this.dicParams.Add(PresetGene.Param.P65_resourceLabel, this.def.resourceLabel);
					this.dicParams.Add(PresetGene.Param.P66_resourceDescription, this.def.resourceDescription);
					this.dicParams.Add(PresetGene.Param.P67_geneCategory, this.def.displayCategory.SDefname());
					SortedDictionary<PresetGene.Param, string> sortedDictionary2 = this.dicParams;
					PresetGene.Param key2 = PresetGene.Param.P68_endogeneCategory;
					num = (int)this.def.endogeneCategory;
					sortedDictionary2.Add(key2, num.ToString());
					this.dicParams.Add(PresetGene.Param.P69_passionMod, this.def.passionMod.AsListString());
					this.dicParams.Add(PresetGene.Param.P70_geneticBodyType, (this.def.bodyType == null) ? "" : this.def.bodyType.ToString());
					this.dicParams.Add(PresetGene.Param.P71_customEffectDescriptions, this.def.customEffectDescriptions.ListToString<string>());
					this.dicParams.Add(PresetGene.Param.P72_resourceGizmoThresholds, this.def.resourceGizmoThresholds.ListToString<float>());
					this.dicParams.Add(PresetGene.Param.P73_forcedHeadTypes, this.def.forcedHeadTypes.ListToString<HeadTypeDef>());
					this.dicParams.Add(PresetGene.Param.P74_exclusionTags, this.def.exclusionTags.ListToString<string>());
					this.dicParams.Add(PresetGene.Param.P75_hairTagFilter, this.def.hairTagFilter.AsListString());
					this.dicParams.Add(PresetGene.Param.P76_beardTagFilter, this.def.beardTagFilter.AsListString());
				}
				catch (Exception ex)
				{
					Log.Error(ex.Message + "\n" + ex.StackTrace);
				}
			}
		}

		
		internal PresetGene(string custom)
		{
			bool flag = Preset.LoadModification<PresetGene.Param>(custom, ref this.dicParams);
			if (flag)
			{
				bool flag2 = this.FromDictionary();
				if (flag2)
				{
					Log.Message(this.def.defName + " " + Label.MODIFICATIONLOADED);
				}
			}
		}

		
		private bool FromDictionary()
		{
			this.def = DefTool.GetDef<GeneDef>(this.dicParams.GetValue(PresetGene.Param.P00_defName));
			bool flag = this.def == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				try
				{
					this.def.label = this.dicParams.GetValue(PresetGene.Param.P01_label);
					this.def.statFactors = this.dicParams.GetValue(PresetGene.Param.P02_statFactors).StringToListNonDef<StatModifier>();
					this.def.statOffsets = this.dicParams.GetValue(PresetGene.Param.P03_statOffsets).StringToListNonDef<StatModifier>();
					this.def.aptitudes = this.dicParams.GetValue(PresetGene.Param.P04_aptitudes).StringToListNonDef<Aptitude>();
					this.def.capMods = this.dicParams.GetValue(PresetGene.Param.P05_capacities).StringToListNonDef<PawnCapacityModifier>();
					this.def.abilities = this.dicParams.GetValue(PresetGene.Param.P06_abilities).StringToList<AbilityDef>();
					this.def.forcedTraits = this.dicParams.GetValue(PresetGene.Param.P07_traits).StringToListNonDef<GeneticTraitData>();
					this.def.suppressedTraits = this.dicParams.GetValue(PresetGene.Param.P08_suppressedTraits).StringToListNonDef<GeneticTraitData>();
					this.def.makeImmuneTo = this.dicParams.GetValue(PresetGene.Param.P09_immunities).StringToList<HediffDef>();
					this.def.hediffGiversCannotGive = this.dicParams.GetValue(PresetGene.Param.P10_protections).StringToList<HediffDef>();
					this.def.disablesNeeds = this.dicParams.GetValue(PresetGene.Param.P11_disabledNeeds).StringToList<NeedDef>();
					WorkTags disabledWorkTags;
					bool flag2 = Enum.TryParse<WorkTags>(this.dicParams.GetValue(PresetGene.Param.P12_disabledWorkTags), out disabledWorkTags);
					if (flag2)
					{
						this.def.disabledWorkTags = disabledWorkTags;
					}
					this.def.damageFactors = this.dicParams.GetValue(PresetGene.Param.P13_damageFactors).StringToListNonDef<DamageFactor>();
					this.def.causesNeed = DefTool.GetDef<NeedDef>(this.dicParams.GetValue(PresetGene.Param.P14_causedNeeds));
					this.def.chemical = DefTool.GetDef<ChemicalDef>(this.dicParams.GetValue(PresetGene.Param.P15_chemical));
					this.def.forcedHair = DefTool.GetDef<HairDef>(this.dicParams.GetValue(PresetGene.Param.P16_forcedHair));
					this.def.hairColorOverride = this.dicParams.GetValue(PresetGene.Param.P17_hairColorOverride).HexStringToColorNullable();
					this.def.skinColorBase = this.dicParams.GetValue(PresetGene.Param.P18_skinColorBase).HexStringToColorNullable();
					this.def.skinColorOverride = this.dicParams.GetValue(PresetGene.Param.P19_skinColorOverride).HexStringToColorNullable();
					this.def.biostatArc = this.dicParams.GetValue(PresetGene.Param.P20_biostatArc).AsInt32();
					this.def.biostatCpx = this.dicParams.GetValue(PresetGene.Param.P21_biostatCpx).AsInt32();
					this.def.biostatMet = this.dicParams.GetValue(PresetGene.Param.P22_biostatMet).AsInt32();
					this.def.addictionChanceFactor = this.dicParams.GetValue(PresetGene.Param.P23_addictionChanceFactor).AsFloat();
					this.def.aggroMentalBreakSelectionChanceFactor = this.dicParams.GetValue(PresetGene.Param.P24_mentalBreakChanceFactor).AsFloat();
					this.def.foodPoisoningChanceFactor = this.dicParams.GetValue(PresetGene.Param.P25_foodPoisioningChanceFactor).AsFloat();
					this.def.lovinMTBFactor = this.dicParams.GetValue(PresetGene.Param.P26_lovinMTBFactor).AsFloat();
					this.def.marketValueFactor = this.dicParams.GetValue(PresetGene.Param.P27_marketValueFactor).AsFloat();
					this.def.mentalBreakMtbDays = this.dicParams.GetValue(PresetGene.Param.P28_mentalBreakMtbDays).AsFloat();
					this.def.minAgeActive = this.dicParams.GetValue(PresetGene.Param.P29_minAgeActive).AsFloat();
					this.def.minMelanin = this.dicParams.GetValue(PresetGene.Param.P30_minMelanin).AsFloat();
					this.def.missingGeneRomanceChanceFactor = this.dicParams.GetValue(PresetGene.Param.P31_missingGeneRomanceChanceFactor).AsFloat();
					this.def.painFactor = this.dicParams.GetValue(PresetGene.Param.P32_painFactor).AsFloat();
					this.def.painOffset = this.dicParams.GetValue(PresetGene.Param.P33_painOffset).AsFloat();
					this.def.prisonBreakMTBFactor = this.dicParams.GetValue(PresetGene.Param.P34_prisonBreakMtbFactor).AsFloat();
					this.def.randomBrightnessFactor = this.dicParams.GetValue(PresetGene.Param.P35_randomBrightnessFactor).AsFloat();
					this.def.resourceLossPerDay = this.dicParams.GetValue(PresetGene.Param.P36_resourceLossPerDay).AsFloat();
					this.def.selectionWeight = this.dicParams.GetValue(PresetGene.Param.P37_selectionWeight).AsFloat();
					this.def.selectionWeightFactorDarkSkin = this.dicParams.GetValue(PresetGene.Param.P38_selectionWeightFactorDarkSkin).AsFloat();
					this.def.socialFightChanceFactor = this.dicParams.GetValue(PresetGene.Param.P39_socialFightChanceFactor).AsFloat();
					this.def.canGenerateInGeneSet = this.dicParams.GetValue(PresetGene.Param.P40_canGenerateInGeneSet).AsBool();
					this.def.dislikesSunlight = this.dicParams.GetValue(PresetGene.Param.P41_dislikesSunLight).AsBool();
					this.def.dontMindRawFood = this.dicParams.GetValue(PresetGene.Param.P42_dontMindRawFood).AsBool();
					this.def.ignoreDarkness = this.dicParams.GetValue(PresetGene.Param.P43_ignoreDarkness).AsBool();
					this.def.immuneToToxGasExposure = this.dicParams.GetValue(PresetGene.Param.P44_immuneToToxGasExposure).AsBool();
					this.def.neverGrayHair = this.dicParams.GetValue(PresetGene.Param.P45_neverGrayHair).AsBool();
					this.def.preventPermanentWounds = this.dicParams.GetValue(PresetGene.Param.P46_prevenetPermanent_Wounds).AsBool();
					this.def.randomChosen = this.dicParams.GetValue(PresetGene.Param.P47_randomChosen).AsBool();
					this.def.removeOnRedress = this.dicParams.GetValue(PresetGene.Param.P48_removeOnRedress).AsBool();
					this.def.showGizmoOnWorldView = this.dicParams.GetValue(PresetGene.Param.P49_showGizmoOnWorldView).AsBool();
					this.def.sterilize = this.dicParams.GetValue(PresetGene.Param.P50_sterilize).AsBool();
					this.def.womenCanHaveBeards = this.dicParams.GetValue(PresetGene.Param.P51_womenCanHaveBeards).AsBool();
					this.def.overdoseChanceFactor = this.dicParams.GetValue(PresetGene.Param.P52_overdoseChanceFactor).AsFloat();
					this.def.toleranceBuildupFactor = this.dicParams.GetValue(PresetGene.Param.P53_toleranceBuildupFactor).AsFloat();
					this.def.displayOrderInCategory = this.dicParams.GetValue(PresetGene.Param.P54_displayOrderInCategory).AsFloat();
					this.def.passOnDirectly = this.dicParams.GetValue(PresetGene.Param.P55_passOnDirectly).AsBool();
					this.def.showGizmoWhenDrafted = this.dicParams.GetValue(PresetGene.Param.P56_showGizmoWhenDrafted).AsBool();
					this.def.showGizmoOnMultiSelect = this.dicParams.GetValue(PresetGene.Param.P57_showGizmoOnMultiSelect).AsBool();
					this.def.soundCall = DefTool.GetDef<SoundDef>(this.dicParams.GetValue(PresetGene.Param.P58_soundCall));
					this.def.soundDeath = DefTool.GetDef<SoundDef>(this.dicParams.GetValue(PresetGene.Param.P59_soundDeath));
					this.def.soundWounded = DefTool.GetDef<SoundDef>(this.dicParams.GetValue(PresetGene.Param.P60_soundWounded));
					this.def.deathHistoryEvent = DefTool.GetDef<HistoryEventDef>(this.dicParams.GetValue(PresetGene.Param.P61_historyEventDef));
					this.def.prerequisite = DefTool.GetDef<GeneDef>(this.dicParams.GetValue(PresetGene.Param.P62_prerequisiteDef));
					this.def.labelShortAdj = this.dicParams.GetValue(PresetGene.Param.P63_labelAdj);
					this.def.iconPath = this.dicParams.GetValue(PresetGene.Param.P64_iconPath);
					this.def.resourceLabel = this.dicParams.GetValue(PresetGene.Param.P65_resourceLabel);
					this.def.resourceDescription = this.dicParams.GetValue(PresetGene.Param.P66_resourceDescription);
					this.def.displayCategory = DefTool.GetDef<GeneCategoryDef>(this.dicParams.GetValue(PresetGene.Param.P67_geneCategory));
					EndogeneCategory endogeneCategory;
					bool flag3 = Enum.TryParse<EndogeneCategory>(this.dicParams.GetValue(PresetGene.Param.P68_endogeneCategory), out endogeneCategory);
					if (flag3)
					{
						this.def.endogeneCategory = endogeneCategory;
					}
					this.def.passionMod = this.dicParams.GetValue(PresetGene.Param.P69_passionMod).StringToListNonDef<PassionMod>().FirstOrFallback(null);
					GeneticBodyType value;
					bool flag4 = Enum.TryParse<GeneticBodyType>(this.dicParams.GetValue(PresetGene.Param.P70_geneticBodyType), out value);
					if (flag4)
					{
						this.def.bodyType = new GeneticBodyType?(value);
					}
					this.def.customEffectDescriptions = this.dicParams.GetValue(PresetGene.Param.P71_customEffectDescriptions).StringToList();
					this.def.resourceGizmoThresholds = this.dicParams.GetValue(PresetGene.Param.P72_resourceGizmoThresholds).StringToFList();
					this.def.forcedHeadTypes = this.dicParams.GetValue(PresetGene.Param.P73_forcedHeadTypes).StringToList<HeadTypeDef>();
					this.def.exclusionTags = this.dicParams.GetValue(PresetGene.Param.P74_exclusionTags).StringToList();
					this.def.hairTagFilter = this.dicParams.GetValue(PresetGene.Param.P75_hairTagFilter).StringToTagFilter();
					this.def.beardTagFilter = this.dicParams.GetValue(PresetGene.Param.P76_beardTagFilter).StringToTagFilter();
					this.def.ResolveReferences();
					this.def.DoAllGeneActions();
				}
				catch (Exception ex)
				{
					Log.Error(ex.Message + "\n" + ex.StackTrace);
					return false;
				}
				result = true;
			}
			return result;
		}

		
		internal SortedDictionary<PresetGene.Param, string> dicParams;

		
		internal GeneDef def = null;

		
		internal enum Param
		{
			
			P00_defName,
			
			P01_label,
			
			P02_statFactors,
			
			P03_statOffsets,
			
			P04_aptitudes,
			
			P05_capacities,
			
			P06_abilities,
			
			P07_traits,
			
			P08_suppressedTraits,
			
			P09_immunities,
			
			P10_protections,
			
			P11_disabledNeeds,
			
			P12_disabledWorkTags,
			
			P13_damageFactors,
			
			P14_causedNeeds,
			
			P15_chemical,
			
			P16_forcedHair,
			
			P17_hairColorOverride,
			
			P18_skinColorBase,
			
			P19_skinColorOverride,
			
			P20_biostatArc,
			
			P21_biostatCpx,
			
			P22_biostatMet,
			
			P23_addictionChanceFactor,
			
			P24_mentalBreakChanceFactor,
			
			P25_foodPoisioningChanceFactor,
			
			P26_lovinMTBFactor,
			
			P27_marketValueFactor,
			
			P28_mentalBreakMtbDays,
			
			P29_minAgeActive,
			
			P30_minMelanin,
			
			P31_missingGeneRomanceChanceFactor,
			
			P32_painFactor,
			
			P33_painOffset,
			
			P34_prisonBreakMtbFactor,
			
			P35_randomBrightnessFactor,
			
			P36_resourceLossPerDay,
			
			P37_selectionWeight,
			
			P38_selectionWeightFactorDarkSkin,
			
			P39_socialFightChanceFactor,
			
			P40_canGenerateInGeneSet,
			
			P41_dislikesSunLight,
			
			P42_dontMindRawFood,
			
			P43_ignoreDarkness,
			
			P44_immuneToToxGasExposure,
			
			P45_neverGrayHair,
			
			P46_prevenetPermanent_Wounds,
			
			P47_randomChosen,
			
			P48_removeOnRedress,
			
			P49_showGizmoOnWorldView,
			
			P50_sterilize,
			
			P51_womenCanHaveBeards,
			
			P52_overdoseChanceFactor,
			
			P53_toleranceBuildupFactor,
			
			P54_displayOrderInCategory,
			
			P55_passOnDirectly,
			
			P56_showGizmoWhenDrafted,
			
			P57_showGizmoOnMultiSelect,
			
			P58_soundCall,
			
			P59_soundDeath,
			
			P60_soundWounded,
			
			P61_historyEventDef,
			
			P62_prerequisiteDef,
			
			P63_labelAdj,
			
			P64_iconPath,
			
			P65_resourceLabel,
			
			P66_resourceDescription,
			
			P67_geneCategory,
			
			P68_endogeneCategory,
			
			P69_passionMod,
			
			P70_geneticBodyType,
			
			P71_customEffectDescriptions,
			
			P72_resourceGizmoThresholds,
			
			P73_forcedHeadTypes,
			
			P74_exclusionTags,
			
			P75_hairTagFilter,
			
			P76_beardTagFilter
		}
	}
}
