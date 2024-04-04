using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor
{
	
	internal class PresetLifeStage
	{
		
		
		internal static OptionS optionS
		{
			get
			{
				return OptionS.CUSTOMLIFESTAGE;
			}
		}

		
		
		public string AsString
		{
			get
			{
				return Preset.AsString<PresetLifeStage.Param>(this.dicParams);
			}
		}

		
		public void SaveCustom()
		{
			bool flag = this.def != null;
			if (flag)
			{
				CEditor.API.SetCustom(PresetLifeStage.optionS, this.AsString, this.def.defName);
				MessageTool.Show(this.def.defName + " " + Label.SETTINGSSAVED, null);
			}
		}

		
		
		public static Dictionary<string, PresetLifeStage> AllDefaults
		{
			get
			{
				return CEditor.API.Get<Dictionary<string, PresetLifeStage>>(EType.LifestagePreset);
			}
		}

		
		
		public static HashSet<LifeStageDef> ListAll
		{
			get
			{
				return DefTool.ListBy<LifeStageDef>((LifeStageDef x) => !x.defName.NullOrEmpty());
			}
		}

		
		public static Dictionary<string, PresetLifeStage> CreateDefaults()
		{
			return Preset.CreateDefaults<PresetLifeStage, LifeStageDef>(PresetLifeStage.ListAll, (LifeStageDef sample) => sample.defName, (LifeStageDef x) => new PresetLifeStage(x), "lifestages");
		}

		
		public static void LoadAllModifications(string custom)
		{
			Preset.LoadAllModifications(custom, delegate(string s)
			{
				new PresetLifeStage(s);
			}, "lifestages");
		}

		
		public static void ResetAllToDefaults()
		{
			Preset.ResetAllToDefault<PresetLifeStage>(PresetLifeStage.AllDefaults, delegate(PresetLifeStage p)
			{
				p.FromDictionary();
			}, PresetLifeStage.optionS, "lifestages");
		}

		
		public static void ResetToDefault(string defName)
		{
			Preset.ResetToDefault<PresetLifeStage>(PresetLifeStage.AllDefaults, delegate(PresetLifeStage p)
			{
				p.FromDictionary();
			}, PresetLifeStage.optionS, defName);
		}

		
		public static void SaveModification(LifeStageDef s)
		{
			new PresetLifeStage(s).SaveCustom();
		}

		
		internal PresetLifeStage(LifeStageDef ldef)
		{
			bool flag = ldef == null;
			if (!flag)
			{
				this.def = ldef;
				this.dicParams = new SortedDictionary<PresetLifeStage.Param, string>();
				try
				{
					this.dicParams.Add(PresetLifeStage.Param.P00_defName, this.def.defName);
					this.dicParams.Add(PresetLifeStage.Param.P01_label, this.def.label);
					this.dicParams.Add(PresetLifeStage.Param.P02_reproductive, this.def.reproductive.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P03_milkable, this.def.milkable.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P04_shearable, this.def.shearable.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P05_caravanRideable, this.def.caravanRideable.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P06_alwaysDowned, this.def.alwaysDowned.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P07_claimable, this.def.claimable.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P08_isInvoluntarySleepNegativeEvent, this.def.involuntarySleepIsNegativeEvent.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P09_canDoRandomMentalBreaks, this.def.canDoRandomMentalBreaks.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P10_canSleepWhileHeld, this.def.canSleepWhileHeld.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P11_canSleepWhenStarving, this.def.canSleepWhenStarving.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P12_canVoluntarilySleep, this.def.canVoluntarilySleep.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P13_canInitiateSocialInteraction, this.def.canInitiateSocialInteraction.ToString());
					SortedDictionary<PresetLifeStage.Param, string> sortedDictionary = this.dicParams;
					PresetLifeStage.Param key = PresetLifeStage.Param.P14_developmentalStage;
					int developmentalStage = (int)this.def.developmentalStage;
					sortedDictionary.Add(key, developmentalStage.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P15_voxPitch, this.def.voxPitch.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P16_voxVolume, this.def.voxVolume.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P17_healthScaleFactor, this.def.healthScaleFactor.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P18_hungerRateFactor, this.def.hungerRateFactor.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P19_marketValueFactor, this.def.marketValueFactor.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P20_foodMaxFactor, this.def.foodMaxFactor.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P21_meleeDamageFactor, this.def.meleeDamageFactor.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P22_bodySizeFactor, this.def.bodySizeFactor.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P23_equipmentDrawFactor, this.def.equipmentDrawDistanceFactor.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P24_bodyWidth, this.def.bodyWidth.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P25_headSizeFactor, this.def.headSizeFactor.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P26_eyeSizeFactor, this.def.eyeSizeFactor.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P27_sittingOffset, this.def.sittingOffset.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P28_bodyDrawOffset, this.def.bodyDrawOffset.ToString());
					this.dicParams.Add(PresetLifeStage.Param.P29_icon, this.def.icon);
					this.dicParams.Add(PresetLifeStage.Param.P30_statFactors, this.def.statFactors.ListToString<StatModifier>());
					this.dicParams.Add(PresetLifeStage.Param.P31_statOffsets, this.def.statOffsets.ListToString<StatModifier>());
				}
				catch (Exception ex)
				{
					Log.Error(ex.Message + "\n" + ex.StackTrace);
				}
			}
		}

		
		internal PresetLifeStage(string custom)
		{
			bool flag = Preset.LoadModification<PresetLifeStage.Param>(custom, ref this.dicParams);
			if (flag)
			{
				bool flag2 = this.FromDictionary();
				if (flag2)
				{
					Log.Message(this.def.defName + " " + Label.MODIFICATIONLOADED);
				}
			}
		}

		
		public bool FromDictionary()
		{
			this.def = DefTool.GetDef<LifeStageDef>(this.dicParams.GetValue(PresetLifeStage.Param.P00_defName));
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
					this.def.label = this.dicParams.GetValue(PresetLifeStage.Param.P01_label);
					this.def.reproductive = this.dicParams.GetValue(PresetLifeStage.Param.P02_reproductive).AsBool();
					this.def.milkable = this.dicParams.GetValue(PresetLifeStage.Param.P03_milkable).AsBool();
					this.def.shearable = this.dicParams.GetValue(PresetLifeStage.Param.P04_shearable).AsBool();
					this.def.caravanRideable = this.dicParams.GetValue(PresetLifeStage.Param.P05_caravanRideable).AsBool();
					this.def.alwaysDowned = this.dicParams.GetValue(PresetLifeStage.Param.P06_alwaysDowned).AsBool();
					this.def.claimable = this.dicParams.GetValue(PresetLifeStage.Param.P07_claimable).AsBool();
					this.def.involuntarySleepIsNegativeEvent = this.dicParams.GetValue(PresetLifeStage.Param.P08_isInvoluntarySleepNegativeEvent).AsBool();
					this.def.canDoRandomMentalBreaks = this.dicParams.GetValue(PresetLifeStage.Param.P09_canDoRandomMentalBreaks).AsBool();
					this.def.canSleepWhileHeld = this.dicParams.GetValue(PresetLifeStage.Param.P10_canSleepWhileHeld).AsBool();
					this.def.canSleepWhenStarving = this.dicParams.GetValue(PresetLifeStage.Param.P11_canSleepWhenStarving).AsBool();
					this.def.canVoluntarilySleep = this.dicParams.GetValue(PresetLifeStage.Param.P12_canVoluntarilySleep).AsBool();
					this.def.canInitiateSocialInteraction = this.dicParams.GetValue(PresetLifeStage.Param.P13_canInitiateSocialInteraction).AsBool();
					DevelopmentalStage developmentalStage;
					bool flag2 = Enum.TryParse<DevelopmentalStage>(this.dicParams.GetValue(PresetLifeStage.Param.P14_developmentalStage), out developmentalStage);
					if (flag2)
					{
						this.def.developmentalStage = developmentalStage;
					}
					this.def.voxPitch = this.dicParams.GetValue(PresetLifeStage.Param.P15_voxPitch).AsFloat();
					this.def.voxVolume = this.dicParams.GetValue(PresetLifeStage.Param.P16_voxVolume).AsFloat();
					this.def.healthScaleFactor = this.dicParams.GetValue(PresetLifeStage.Param.P17_healthScaleFactor).AsFloat();
					this.def.hungerRateFactor = this.dicParams.GetValue(PresetLifeStage.Param.P18_hungerRateFactor).AsFloat();
					this.def.marketValueFactor = this.dicParams.GetValue(PresetLifeStage.Param.P19_marketValueFactor).AsFloat();
					this.def.foodMaxFactor = this.dicParams.GetValue(PresetLifeStage.Param.P20_foodMaxFactor).AsFloat();
					this.def.meleeDamageFactor = this.dicParams.GetValue(PresetLifeStage.Param.P21_meleeDamageFactor).AsFloat();
					this.def.bodySizeFactor = this.dicParams.GetValue(PresetLifeStage.Param.P22_bodySizeFactor).AsFloat();
					this.def.equipmentDrawDistanceFactor = this.dicParams.GetValue(PresetLifeStage.Param.P23_equipmentDrawFactor).AsFloat();
					this.def.bodyWidth = this.dicParams.GetValue(PresetLifeStage.Param.P24_bodyWidth).AsFloatZero();
					this.def.headSizeFactor = this.dicParams.GetValue(PresetLifeStage.Param.P25_headSizeFactor).AsFloatZero();
					this.def.eyeSizeFactor = this.dicParams.GetValue(PresetLifeStage.Param.P26_eyeSizeFactor).AsFloatZero();
					this.def.sittingOffset = this.dicParams.GetValue(PresetLifeStage.Param.P27_sittingOffset).AsFloatZero();
					this.def.bodyDrawOffset = this.dicParams.GetValue(PresetLifeStage.Param.P28_bodyDrawOffset).AsVector3Zero().GetValueOrDefault(Vector3.zero);
					this.def.icon = this.dicParams.GetValue(PresetLifeStage.Param.P29_icon);
					this.def.statFactors = this.dicParams.GetValue(PresetLifeStage.Param.P30_statFactors).StringToListNonDef<StatModifier>();
					this.def.statOffsets = this.dicParams.GetValue(PresetLifeStage.Param.P31_statOffsets).StringToListNonDef<StatModifier>();
					this.def.ResolveReferences();
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

		
		internal SortedDictionary<PresetLifeStage.Param, string> dicParams;

		
		internal LifeStageDef def = null;

		
		internal enum Param
		{
			
			P00_defName,
			
			P01_label,
			
			P02_reproductive,
			
			P03_milkable,
			
			P04_shearable,
			
			P05_caravanRideable,
			
			P06_alwaysDowned,
			
			P07_claimable,
			
			P08_isInvoluntarySleepNegativeEvent,
			
			P09_canDoRandomMentalBreaks,
			
			P10_canSleepWhileHeld,
			
			P11_canSleepWhenStarving,
			
			P12_canVoluntarilySleep,
			
			P13_canInitiateSocialInteraction,
			
			P14_developmentalStage,
			
			P15_voxPitch,
			
			P16_voxVolume,
			
			P17_healthScaleFactor,
			
			P18_hungerRateFactor,
			
			P19_marketValueFactor,
			
			P20_foodMaxFactor,
			
			P21_meleeDamageFactor,
			
			P22_bodySizeFactor,
			
			P23_equipmentDrawFactor,
			
			P24_bodyWidth,
			
			P25_headSizeFactor,
			
			P26_eyeSizeFactor,
			
			P27_sittingOffset,
			
			P28_bodyDrawOffset,
			
			P29_icon,
			
			P30_statFactors,
			
			P31_statOffsets
		}

	}
}
