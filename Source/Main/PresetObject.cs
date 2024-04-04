using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace CharacterEditor
{
	
	internal class PresetObject
	{
		
		
		internal static OptionS optionS
		{
			get
			{
				return OptionS.CUSTOMOBJECT;
			}
		}

		
		
		public string AsString
		{
			get
			{
				return Preset.AsString<PresetObject.Param>(this.dicParams);
			}
		}

		
		public void SaveCustom()
		{
			bool flag = this.def != null;
			if (flag)
			{
				CEditor.API.SetCustom(PresetObject.optionS, this.AsString, this.def.defName);
			}
			MessageTool.Show(this.def.defName + " " + Label.SETTINGSSAVED, null);
		}

		
		
		public static Dictionary<string, PresetObject> AllDefaultObjects
		{
			get
			{
				return CEditor.API.Get<Dictionary<string, PresetObject>>(EType.ObjectPreset);
			}
		}

		
		
		public static Dictionary<string, PresetObject> AllDefaultTurrets
		{
			get
			{
				return CEditor.API.Get<Dictionary<string, PresetObject>>(EType.TurretPreset);
			}
		}

		
		
		public static HashSet<ThingDef> ListAllObjects
		{
			get
			{
				return ThingTool.ListOfItems(null, null, ThingCategory.None);
			}
		}

		
		
		public static HashSet<ThingDef> ListAllTurrets
		{
			get
			{
				return DefTool.ListBy<ThingDef>((ThingDef x) => !x.defName.NullOrEmpty() && x.IsTurret() && !x.label.NullOrEmpty() && x.building != null && x.building.turretGunDef != null);
			}
		}

		
		
		internal static Dictionary<string, ThingDef> DicGunAndTurret
		{
			get
			{
				Dictionary<string, ThingDef> dictionary = new Dictionary<string, ThingDef>();
				HashSet<ThingDef> listAllTurrets = PresetObject.ListAllTurrets;
				bool flag = listAllTurrets.NullOrEmpty<ThingDef>();
				Dictionary<string, ThingDef> result;
				if (flag)
				{
					result = dictionary;
				}
				else
				{
					foreach (ThingDef thingDef in listAllTurrets)
					{
						dictionary.Add(thingDef.defName, thingDef.building.turretGunDef);
					}
					result = dictionary;
				}
				return result;
			}
		}

		
		public static Dictionary<string, PresetObject> CreateDefaultObjects()
		{
			return Preset.CreateDefaults<PresetObject, ThingDef>(PresetObject.ListAllObjects, (ThingDef s) => s.defName, (ThingDef x) => new PresetObject(x), "objects");
		}

		
		public static Dictionary<string, PresetObject> CreateDefaultTurrets()
		{
			return Preset.CreateDefaults<PresetObject, ThingDef>(PresetObject.ListAllTurrets, (ThingDef s) => s.defName, (ThingDef x) => new PresetObject(x), "turrets");
		}

		
		public static void LoadAllModifications(string custom)
		{
			Preset.LoadAllModifications(custom, delegate(string s)
			{
				new PresetObject(s);
			}, "objects");
		}

		
		public static void ResetAllToDefaults()
		{
			Preset.ResetAllToDefault<PresetObject>(PresetObject.AllDefaultObjects, delegate(PresetObject p)
			{
				p.FromDictionary();
			}, PresetObject.optionS, "objects");
			Preset.ResetAllToDefault<PresetObject>(PresetObject.AllDefaultTurrets, delegate(PresetObject p)
			{
				p.FromDictionary();
			}, PresetObject.optionS, "turrets");
		}

		
		public static void ResetToDefault(string defName)
		{
			bool flag = PresetObject.AllDefaultTurrets.ContainsKey(defName);
			if (flag)
			{
				Preset.ResetToDefault<PresetObject>(PresetObject.AllDefaultTurrets, delegate(PresetObject p)
				{
					p.FromDictionary();
				}, PresetObject.optionS, defName);
			}
			else
			{
				bool flag2 = PresetObject.AllDefaultObjects.ContainsKey(defName);
				if (flag2)
				{
					Preset.ResetToDefault<PresetObject>(PresetObject.AllDefaultObjects, delegate(PresetObject p)
					{
						p.FromDictionary();
					}, PresetObject.optionS, defName);
				}
				else
				{
					MessageTool.Show("default preset not found!", MessageTypeDefOf.RejectInput);
				}
			}
		}

		
		public static void SaveModification(ThingDef s)
		{
			new PresetObject(s).SaveCustom();
		}

		
		internal PresetObject(ThingDef gun)
		{
			bool flag = gun == null;
			if (!flag)
			{
				ThingDef turretDef = gun.GetTurretDef();
				this.def = ((turretDef == null || PresetObject.DicGunAndTurret.Values.Contains(gun)) ? gun : turretDef);
				bool flag2 = this.def == null;
				if (!flag2)
				{
					this.dicParams = new SortedDictionary<PresetObject.Param, string>();
					try
					{
						this.dicParams.Add(PresetObject.Param.P00_defName, this.def.defName);
						this.dicParams.Add(PresetObject.Param.P01_label, this.def.label);
						SortedDictionary<PresetObject.Param, string> sortedDictionary = this.dicParams;
						PresetObject.Param key = PresetObject.Param.P02_techLevel;
						int num = (int)this.def.techLevel;
						sortedDictionary.Add(key, num.ToString());
						SortedDictionary<PresetObject.Param, string> sortedDictionary2 = this.dicParams;
						PresetObject.Param key2 = PresetObject.Param.P03_tradeability;
						num = (int)this.def.tradeability;
						sortedDictionary2.Add(key2, num.ToString());
						this.dicParams.Add(PresetObject.Param.P04_stealable, this.def.stealable.ToString());
						this.dicParams.Add(PresetObject.Param.P05_costStuffCount, this.def.costStuffCount.ToString());
						this.dicParams.Add(PresetObject.Param.P06_stuffCategories, this.def.stuffCategories.ListToString<StuffCategoryDef>());
						this.dicParams.Add(PresetObject.Param.P07_statBases, this.def.statBases.ListToString<StatModifier>());
						this.dicParams.Add(PresetObject.Param.P08_statOffsets, this.def.equippedStatOffsets.ListToString<StatModifier>());
						this.dicParams.Add(PresetObject.Param.P09_costList, this.def.costList.ListToString<ThingDefCountClass>());
						this.dicParams.Add(PresetObject.Param.P10_costListDifficulty, (this.def.costListForDifficulty != null) ? this.def.costListForDifficulty.costList.ListToString<ThingDefCountClass>() : "");
						this.dicParams.Add(PresetObject.Param.P11_tradeTags, this.def.tradeTags.ListToString<string>());
						this.dicParams.Add(PresetObject.Param.P12_weaponTags, this.def.weaponTags.ListToString<string>());
						bool flag3 = this.def.HasProjectile();
						this.dicParams.Add(PresetObject.Param.P13_bulletDefName, this.HasDefaultProjectile ? this.sverb.defaultProjectile.defName : "");
						this.dicParams.Add(PresetObject.Param.P14_bulletDamageDef, flag3 ? ((this.sbullet.damageDef != null) ? this.sbullet.damageDef.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P15_bulletDamageAmountBase, flag3 ? this.sbullet.GetMemberValueAsString<int>("damageAmountBase", "") : "");
						this.dicParams.Add(PresetObject.Param.P16_bulletSpeed, flag3 ? this.sbullet.speed.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P17_bulletStoppingPower, flag3 ? this.sbullet.stoppingPower.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P18_bulletArmorPenetrationBase, flag3 ? this.sbullet.GetMemberValueAsString<float>("armorPenetrationBase", "") : "");
						this.dicParams.Add(PresetObject.Param.P19_bulletExplosionRadius, flag3 ? this.sbullet.explosionRadius.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P20_bulletExplosionDelay, flag3 ? this.sbullet.explosionDelay.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P21_bulletNumExtraHitCells, flag3 ? this.sbullet.numExtraHitCells.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P22_bulletPreExplosionSpawnThingDef, flag3 ? ((this.sbullet.preExplosionSpawnThingDef != null) ? this.sbullet.preExplosionSpawnThingDef.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P23_bulletPreExplosionSpawnThingCount, flag3 ? this.sbullet.preExplosionSpawnThingCount.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P24_bulletPreExplosionSpawnChance, flag3 ? this.sbullet.preExplosionSpawnChance.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P25_bulletPostExplosionSpawnThingDef, flag3 ? ((this.sbullet.postExplosionSpawnThingDef != null) ? this.sbullet.postExplosionSpawnThingDef.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P26_bulletPostExplosionSpawnThingCount, flag3 ? this.sbullet.postExplosionSpawnThingCount.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P27_bulletPostExplosionSpawnChance, flag3 ? this.sbullet.postExplosionSpawnChance.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P28_bulletPostExplosionGasType, flag3 ? ((this.sbullet.postExplosionGasType != null) ? ((int)this.sbullet.postExplosionGasType.Value).ToString() : "") : "");
						this.dicParams.Add(PresetObject.Param.P29_bulletExplosionEffect, flag3 ? ((this.sbullet.explosionEffect != null) ? this.sbullet.explosionEffect.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P30_bulletLandedEffect, flag3 ? ((this.sbullet.landedEffecter != null) ? this.sbullet.landedEffecter.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P35_bulletFlyOverhead, flag3 ? this.sbullet.flyOverhead.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P36_bulletSoundExplode, flag3 ? ((this.sbullet.soundExplode != null) ? this.sbullet.soundExplode.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P37_bulletSoundImpact, flag3 ? ((this.sbullet.soundImpact != null) ? this.sbullet.soundImpact.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P38_bulletDamageToCellsNeighbors, flag3 ? this.sbullet.applyDamageToExplosionCellsNeighbors.ToString() : "");
						bool flag4 = this.def.HasVerb();
						this.dicParams.Add(PresetObject.Param.P39_beamTargetsGround, flag4 ? this.sverb.beamTargetsGround.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P40_ticksBetweenBurstShots, flag4 ? this.sverb.ticksBetweenBurstShots.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P41_burstShotCount, flag4 ? this.sverb.burstShotCount.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P42_weaponRange, flag4 ? this.sverb.range.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P43_weaponMinRange, flag4 ? this.sverb.minRange.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P44_forcedMissRadius, flag4 ? this.sverb.GetMemberValueAsString<float>("forcedMissRadius", "") : "");
						this.dicParams.Add(PresetObject.Param.P45_defaultCooldownTime, flag4 ? this.sverb.defaultCooldownTime.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P46_warmupTime, flag4 ? this.sverb.warmupTime.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P47_baseAccuracyTouch, flag4 ? this.sverb.accuracyTouch.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P48_baseAccuracyShort, flag4 ? this.sverb.accuracyShort.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P49_baseAccuracyMedium, flag4 ? this.sverb.accuracyMedium.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P50_baseAccuracyLong, flag4 ? this.sverb.accuracyLong.ToString() : "");
						bool flag5 = flag4 && this.sverb.GetMemberValue("isMortar", false);
						this.dicParams.Add(PresetObject.Param.P51_forcedMissRadiusMortar, (flag4 && turretDef != null && flag5) ? this.sverb.GetMemberValueAsString<float>("forcedMissRadiusClassicMortars", "") : "");
						this.dicParams.Add(PresetObject.Param.P52_requireLineOfSight, flag4 ? this.sverb.requireLineOfSight.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P53_targetGround, this.def.GetWeaponTargetGround().ToString());
						this.dicParams.Add(PresetObject.Param.P54_beamWidth, flag4 ? this.sverb.beamWidth.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P55_beamFullWidthRange, flag4 ? this.sverb.beamFullWidthRange.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P56_beamDamageDef, flag4 ? ((this.sverb.beamDamageDef != null) ? this.sverb.beamDamageDef.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P57_consumeFuelPerShot, flag4 ? this.sverb.consumeFuelPerShot.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P58_consumeFuelPerBurst, flag4 ? this.sverb.consumeFuelPerBurst.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P61_soundCast, flag4 ? ((this.sverb.soundCast != null) ? this.sverb.soundCast.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P62_soundAiming, flag4 ? ((this.sverb.soundAiming != null) ? this.sverb.soundAiming.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P63_soundCastBeam, flag4 ? ((this.sverb.soundCastBeam != null) ? this.sverb.soundCastBeam.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P64_soundCastTail, flag4 ? ((this.sverb.soundCastTail != null) ? this.sverb.soundCastTail.defName : "") : "");
						this.dicParams.Add(PresetObject.Param.P65_soundLanding, flag4 ? ((this.sverb.soundLanding != null) ? this.sverb.soundLanding.defName : "") : "");
						bool flag6 = turretDef != null && turretDef.building != null;
						this.dicParams.Add(PresetObject.Param.P66_turretWarmupTimeMin, flag6 ? turretDef.building.turretBurstWarmupTime.min.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P67_turretWarmupTimeMax, flag6 ? turretDef.building.turretBurstWarmupTime.max.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P68_turretBurstCooldownTime, flag6 ? turretDef.building.turretBurstCooldownTime.ToString() : "");
						this.dicParams.Add(PresetObject.Param.P69_recipePrerequisite, (this.def.recipeMaker != null) ? this.def.recipeMaker.researchPrerequisite.SDefname() : "");
						this.dicParams.Add(PresetObject.Param.P70_buildingPrerequisites, this.def.researchPrerequisites.ListToString<ResearchProjectDef>());
						this.dicParams.Add(PresetObject.Param.P71_CEfuelCapacity, WeaponTool.GetCompRefuelable(turretDef, gun).ToString());
						this.dicParams.Add(PresetObject.Param.P72_CEreloadTime, WeaponTool.GetCompReloadTime(turretDef, gun).ToString());
						bool flag7 = this.def.apparel != null;
						this.dicParams.Add(PresetObject.Param.P73_apparelLayer, flag7 ? this.def.apparel.layers.ListToString<ApparelLayerDef>() : "");
						this.dicParams.Add(PresetObject.Param.P74_bodyPartGroup, flag7 ? this.def.apparel.bodyPartGroups.ListToString<BodyPartGroupDef>() : "");
						this.dicParams.Add(PresetObject.Param.P75_apparelTags, flag7 ? this.def.apparel.tags.ListToString<string>() : "");
						this.dicParams.Add(PresetObject.Param.P76_outfitTags, flag7 ? this.def.apparel.defaultOutfitTags.ListToString<string>() : "");
						this.dicParams.Add(PresetObject.Param.P77_stackLimit, this.def.stackLimit.ToString());
					}
					catch (Exception ex)
					{
						Log.Error(ex.Message + "\n" + ex.StackTrace);
					}
				}
			}
		}

		
		internal PresetObject(string custom)
		{
			bool flag = Preset.LoadModification<PresetObject.Param>(custom, ref this.dicParams);
			if (flag)
			{
				bool flag2 = this.FromDictionary();
				if (flag2)
				{
					Log.Message(this.def.defName + " " + Label.MODIFICATIONLOADED);
				}
			}
		}

		
		
		private VerbProperties sverb
		{
			get
			{
				return this.def.Verbs[0];
			}
		}

		
		
		private ProjectileProperties sbullet
		{
			get
			{
				return this.def.Verbs[0].defaultProjectile.projectile;
			}
		}

		
		
		private bool HasDefaultProjectile
		{
			get
			{
				return this.def.Verbs.Count > 0 && this.def.Verbs[0].defaultProjectile != null;
			}
		}

		
		private bool FromDictionary()
		{
			ThingDef thingDef = DefTool.GetDef<ThingDef>(this.dicParams.GetValue(PresetObject.Param.P00_defName));
			bool flag = thingDef == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				try
				{
					ThingDef turretDef = thingDef.GetTurretDef();
					this.def = ((turretDef == null || PresetObject.DicGunAndTurret.Values.Contains(thingDef)) ? thingDef : turretDef);
					bool flag2 = this.def == null;
					if (flag2)
					{
						return false;
					}
					this.def.label = this.dicParams.GetValue(PresetObject.Param.P01_label);
					TechLevel techLevel;
					bool flag3 = Enum.TryParse<TechLevel>(this.dicParams.GetValue(PresetObject.Param.P02_techLevel), out techLevel);
					if (flag3)
					{
						this.def.techLevel = techLevel;
					}
					Tradeability tradeability;
					bool flag4 = Enum.TryParse<Tradeability>(this.dicParams.GetValue(PresetObject.Param.P03_tradeability), out tradeability);
					if (flag4)
					{
						this.def.tradeability = tradeability;
					}
					this.def.stealable = this.dicParams.GetValue(PresetObject.Param.P04_stealable).AsBool();
					this.def.costStuffCount = this.dicParams.GetValue(PresetObject.Param.P05_costStuffCount).AsInt32();
					this.def.stuffCategories = this.dicParams.GetValue(PresetObject.Param.P06_stuffCategories).StringToList<StuffCategoryDef>();
					this.def.statBases = this.dicParams.GetValue(PresetObject.Param.P07_statBases).StringToListNonDef<StatModifier>();
					this.def.equippedStatOffsets = this.dicParams.GetValue(PresetObject.Param.P08_statOffsets).StringToListNonDef<StatModifier>();
					this.def.costList = this.dicParams.GetValue(PresetObject.Param.P09_costList).StringToListNonDef<ThingDefCountClass>();
					bool flag5 = !this.dicParams.GetValue(PresetObject.Param.P10_costListDifficulty).NullOrEmpty();
					if (flag5)
					{
						bool flag6 = this.def.costListForDifficulty == null;
						if (flag6)
						{
							this.def.costListForDifficulty = new CostListForDifficulty();
						}
						this.def.costListForDifficulty.costList = this.dicParams.GetValue(PresetObject.Param.P10_costListDifficulty).StringToListNonDef<ThingDefCountClass>();
					}
					this.def.tradeTags = this.dicParams.GetValue(PresetObject.Param.P11_tradeTags).StringToList();
					this.def.weaponTags = this.dicParams.GetValue(PresetObject.Param.P12_weaponTags).StringToList();
					this.def.stackLimit = this.dicParams.GetValue(PresetObject.Param.P77_stackLimit).AsInt32();
					this.def.UpdateStackLimit();
					bool flag7 = this.def.HasVerb();
					if (flag7)
					{
						this.sverb.defaultProjectile = DefTool.GetDef<ThingDef>(this.dicParams.GetValue(PresetObject.Param.P13_bulletDefName));
						bool flag8 = this.def.HasProjectile();
						if (flag8)
						{
							this.sbullet.damageDef = DefTool.GetDef<DamageDef>(this.dicParams.GetValue(PresetObject.Param.P14_bulletDamageDef));
							this.sbullet.SetMemberValue("damageAmountBase", this.dicParams.GetValue(PresetObject.Param.P15_bulletDamageAmountBase).AsInt32());
							this.sbullet.speed = this.dicParams.GetValue(PresetObject.Param.P16_bulletSpeed).AsFloat();
							this.sbullet.stoppingPower = this.dicParams.GetValue(PresetObject.Param.P17_bulletStoppingPower).AsFloat();
							this.sbullet.SetMemberValue("armorPenetrationBase", this.dicParams.GetValue(PresetObject.Param.P18_bulletArmorPenetrationBase).AsFloat());
							this.sbullet.explosionRadius = this.dicParams.GetValue(PresetObject.Param.P19_bulletExplosionRadius).AsFloat();
							this.sbullet.explosionDelay = this.dicParams.GetValue(PresetObject.Param.P20_bulletExplosionDelay).AsInt32();
							this.sbullet.numExtraHitCells = this.dicParams.GetValue(PresetObject.Param.P21_bulletNumExtraHitCells).AsInt32();
							this.sbullet.preExplosionSpawnThingDef = DefTool.GetDef<ThingDef>(this.dicParams.GetValue(PresetObject.Param.P22_bulletPreExplosionSpawnThingDef));
							this.sbullet.preExplosionSpawnThingCount = this.dicParams.GetValue(PresetObject.Param.P23_bulletPreExplosionSpawnThingCount).AsInt32();
							this.sbullet.preExplosionSpawnChance = this.dicParams.GetValue(PresetObject.Param.P24_bulletPreExplosionSpawnChance).AsFloat();
							this.sbullet.postExplosionSpawnThingDef = DefTool.GetDef<ThingDef>(this.dicParams.GetValue(PresetObject.Param.P25_bulletPostExplosionSpawnThingDef));
							this.sbullet.postExplosionSpawnThingCount = this.dicParams.GetValue(PresetObject.Param.P26_bulletPostExplosionSpawnThingCount).AsInt32();
							this.sbullet.postExplosionSpawnChance = this.dicParams.GetValue(PresetObject.Param.P27_bulletPostExplosionSpawnChance).AsFloat();
							GasType value;
							bool flag9 = Enum.TryParse<GasType>(this.dicParams.GetValue(PresetObject.Param.P28_bulletPostExplosionGasType), out value);
							if (flag9)
							{
								this.sbullet.postExplosionGasType = new GasType?(value);
							}
							this.sbullet.explosionEffect = DefTool.GetDef<EffecterDef>(this.dicParams.GetValue(PresetObject.Param.P29_bulletExplosionEffect));
							this.sbullet.landedEffecter = DefTool.GetDef<EffecterDef>(this.dicParams.GetValue(PresetObject.Param.P30_bulletLandedEffect));
							this.sbullet.flyOverhead = this.dicParams.GetValue(PresetObject.Param.P35_bulletFlyOverhead).AsBool();
							this.sbullet.soundExplode = DefTool.GetDef<SoundDef>(this.dicParams.GetValue(PresetObject.Param.P36_bulletSoundExplode));
							this.sbullet.soundImpact = DefTool.GetDef<SoundDef>(this.dicParams.GetValue(PresetObject.Param.P37_bulletSoundImpact));
							this.sbullet.applyDamageToExplosionCellsNeighbors = this.dicParams.GetValue(PresetObject.Param.P38_bulletDamageToCellsNeighbors).AsBool();
						}
						this.sverb.beamTargetsGround = this.dicParams.GetValue(PresetObject.Param.P39_beamTargetsGround).AsBool();
						this.sverb.ticksBetweenBurstShots = this.dicParams.GetValue(PresetObject.Param.P40_ticksBetweenBurstShots).AsInt32();
						this.sverb.burstShotCount = this.dicParams.GetValue(PresetObject.Param.P41_burstShotCount).AsInt32();
						this.sverb.range = this.dicParams.GetValue(PresetObject.Param.P42_weaponRange).AsFloat();
						this.sverb.minRange = this.dicParams.GetValue(PresetObject.Param.P43_weaponMinRange).AsFloat();
						this.sverb.SetMemberValue("forcedMissRadius", this.dicParams.GetValue(PresetObject.Param.P44_forcedMissRadius).AsFloat());
						this.sverb.defaultCooldownTime = this.dicParams.GetValue(PresetObject.Param.P45_defaultCooldownTime).AsFloat();
						this.sverb.warmupTime = this.dicParams.GetValue(PresetObject.Param.P46_warmupTime).AsFloat();
						this.sverb.accuracyTouch = this.dicParams.GetValue(PresetObject.Param.P47_baseAccuracyTouch).AsFloat();
						this.sverb.accuracyShort = this.dicParams.GetValue(PresetObject.Param.P48_baseAccuracyShort).AsFloat();
						this.sverb.accuracyMedium = this.dicParams.GetValue(PresetObject.Param.P49_baseAccuracyMedium).AsFloat();
						this.sverb.accuracyLong = this.dicParams.GetValue(PresetObject.Param.P50_baseAccuracyLong).AsFloat();
						this.sverb.requireLineOfSight = this.dicParams.GetValue(PresetObject.Param.P52_requireLineOfSight).AsBool();
						this.def.SetTargetParams(this.dicParams.GetValue(PresetObject.Param.P53_targetGround).AsBool());
						this.sverb.beamWidth = this.dicParams.GetValue(PresetObject.Param.P54_beamWidth).AsFloat();
						this.sverb.beamFullWidthRange = this.dicParams.GetValue(PresetObject.Param.P55_beamFullWidthRange).AsFloat();
						this.sverb.beamDamageDef = DefTool.GetDef<DamageDef>(this.dicParams.GetValue(PresetObject.Param.P56_beamDamageDef));
						this.sverb.consumeFuelPerShot = this.dicParams.GetValue(PresetObject.Param.P57_consumeFuelPerShot).AsFloat();
						this.sverb.consumeFuelPerBurst = this.dicParams.GetValue(PresetObject.Param.P58_consumeFuelPerBurst).AsFloat();
						this.sverb.soundCast = DefTool.GetDef<SoundDef>(this.dicParams.GetValue(PresetObject.Param.P61_soundCast));
						this.sverb.soundAiming = DefTool.GetDef<SoundDef>(this.dicParams.GetValue(PresetObject.Param.P62_soundAiming));
						this.sverb.soundCastBeam = DefTool.GetDef<SoundDef>(this.dicParams.GetValue(PresetObject.Param.P63_soundCastBeam));
						this.sverb.soundCastTail = DefTool.GetDef<SoundDef>(this.dicParams.GetValue(PresetObject.Param.P64_soundCastTail));
						this.sverb.soundLanding = DefTool.GetDef<SoundDef>(this.dicParams.GetValue(PresetObject.Param.P65_soundLanding));
						WeaponTool.SetCompRefuelable(null, this.def, this.dicParams.GetValue(PresetObject.Param.P71_CEfuelCapacity).AsInt32());
						WeaponTool.SetCompReloadTime(null, this.def, this.dicParams.GetValue(PresetObject.Param.P72_CEreloadTime).AsFloat());
						bool memberValue = this.sverb.GetMemberValue("isMortar", false);
						bool flag10 = turretDef != null && memberValue;
						if (flag10)
						{
							this.sverb.SetMemberValue("forcedMissRadiusClassicMortars", this.dicParams.GetValue(PresetObject.Param.P51_forcedMissRadiusMortar).AsFloat());
						}
					}
					this.def.SetResearchPrerequisite(DefTool.GetDef<ResearchProjectDef>(this.dicParams.GetValue(PresetObject.Param.P69_recipePrerequisite)));
					this.def.researchPrerequisites = this.dicParams.GetValue(PresetObject.Param.P70_buildingPrerequisites).StringToList<ResearchProjectDef>();
					bool flag11 = turretDef != null;
					if (flag11)
					{
						bool flag12 = this.def.building != null;
						if (flag12)
						{
							this.def.building.turretBurstWarmupTime.min = this.dicParams.GetValue(PresetObject.Param.P66_turretWarmupTimeMin).AsFloat();
							this.def.building.turretBurstWarmupTime.max = this.dicParams.GetValue(PresetObject.Param.P67_turretWarmupTimeMax).AsFloat();
							this.def.building.turretBurstCooldownTime = this.dicParams.GetValue(PresetObject.Param.P68_turretBurstCooldownTime).AsFloat();
						}
						WeaponTool.SetCompRefuelable(this.def, thingDef, this.dicParams.GetValue(PresetObject.Param.P71_CEfuelCapacity).AsInt32());
						WeaponTool.SetCompReloadTime(this.def, thingDef, this.dicParams.GetValue(PresetObject.Param.P72_CEreloadTime).AsFloat());
						this.def.ResolveReferences();
					}
					bool flag13 = this.def.apparel != null;
					if (flag13)
					{
						this.def.apparel.layers = this.dicParams.GetValue(PresetObject.Param.P73_apparelLayer).StringToList<ApparelLayerDef>();
						this.def.apparel.bodyPartGroups = this.dicParams.GetValue(PresetObject.Param.P74_bodyPartGroup).StringToList<BodyPartGroupDef>();
						this.def.apparel.tags = this.dicParams.GetValue(PresetObject.Param.P75_apparelTags).StringToList();
						this.def.apparel.defaultOutfitTags = this.dicParams.GetValue(PresetObject.Param.P76_outfitTags).StringToList();
					}
					bool hasDefaultProjectile = this.HasDefaultProjectile;
					if (hasDefaultProjectile)
					{
						this.sverb.defaultProjectile.ResolveReferences();
					}
					this.def.UpdateRecipes();
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

		
		internal SortedDictionary<PresetObject.Param, string> dicParams;

		
		internal ThingDef def = null;

		
		internal enum Param
		{
			
			P00_defName,
			
			P01_label,
			
			P02_techLevel,
			
			P03_tradeability,
			
			P04_stealable,
			
			P05_costStuffCount,
			
			P06_stuffCategories,
			
			P07_statBases,
			
			P08_statOffsets,
			
			P09_costList,
			
			P10_costListDifficulty,
			
			P11_tradeTags,
			
			P12_weaponTags,
			
			P13_bulletDefName,
			
			P14_bulletDamageDef,
			
			P15_bulletDamageAmountBase,
			
			P16_bulletSpeed,
			
			P17_bulletStoppingPower,
			
			P18_bulletArmorPenetrationBase,
			
			P19_bulletExplosionRadius,
			
			P20_bulletExplosionDelay,
			
			P21_bulletNumExtraHitCells,
			
			P22_bulletPreExplosionSpawnThingDef,
			
			P23_bulletPreExplosionSpawnThingCount,
			
			P24_bulletPreExplosionSpawnChance,
			
			P25_bulletPostExplosionSpawnThingDef,
			
			P26_bulletPostExplosionSpawnThingCount,
			
			P27_bulletPostExplosionSpawnChance,
			
			P28_bulletPostExplosionGasType,
			
			P29_bulletExplosionEffect,
			
			P30_bulletLandedEffect,
			
			P35_bulletFlyOverhead,
			
			P36_bulletSoundExplode,
			
			P37_bulletSoundImpact,
			
			P38_bulletDamageToCellsNeighbors,
			
			P39_beamTargetsGround,
			
			P40_ticksBetweenBurstShots,
			
			P41_burstShotCount,
			
			P42_weaponRange,
			
			P43_weaponMinRange,
			
			P44_forcedMissRadius,
			
			P45_defaultCooldownTime,
			
			P46_warmupTime,
			
			P47_baseAccuracyTouch,
			
			P48_baseAccuracyShort,
			
			P49_baseAccuracyMedium,
			
			P50_baseAccuracyLong,
			
			P51_forcedMissRadiusMortar,
			
			P52_requireLineOfSight,
			
			P53_targetGround,
			
			P54_beamWidth,
			
			P55_beamFullWidthRange,
			
			P56_beamDamageDef,
			
			P57_consumeFuelPerShot,
			
			P58_consumeFuelPerBurst,
			
			P61_soundCast,
			
			P62_soundAiming,
			
			P63_soundCastBeam,
			
			P64_soundCastTail,
			
			P65_soundLanding,
			
			P66_turretWarmupTimeMin,
			
			P67_turretWarmupTimeMax,
			
			P68_turretBurstCooldownTime,
			
			P69_recipePrerequisite,
			
			P70_buildingPrerequisites,
			
			P71_CEfuelCapacity,
			
			P72_CEreloadTime,
			
			P73_apparelLayer,
			
			P74_bodyPartGroup,
			
			P75_apparelTags,
			
			P76_outfitTags,
			
			P77_stackLimit
		}
	}
}
