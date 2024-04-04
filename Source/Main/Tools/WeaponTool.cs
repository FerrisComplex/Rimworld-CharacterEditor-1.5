using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace CharacterEditor
{
	
	internal static class WeaponTool
	{
		
		internal static bool HasAnyWeapon(this Pawn pawn)
		{
			return pawn.HasEquipmentTracker() && !pawn.equipment.AllEquipmentListForReading.NullOrEmpty<ThingWithComps>();
		}

		
		internal static bool IsEquippedByPawn(this Pawn pawn, ThingWithComps w)
		{
			return pawn.HasAnyWeapon() && pawn.equipment.AllEquipmentListForReading.Contains(w);
		}

		
		internal static bool HasAnyWeaponTags(this ThingDef weapon)
		{
			return weapon != null && !weapon.weaponTags.NullOrEmpty<string>();
		}

		
		internal static bool DoesExplosion(this ThingDef weapon)
		{
			return weapon.HasVerb() && weapon.Verbs[0].CausesExplosion;
		}

		
		internal static bool HasVerb(this ThingDef weapon)
		{
			return weapon != null && !weapon.Verbs.NullOrEmpty<VerbProperties>();
		}

		
		internal static bool HasProjectile(this ThingDef weapon)
		{
			return weapon != null && !weapon.Verbs.NullOrEmpty<VerbProperties>() && weapon.Verbs[0].defaultProjectile != null && weapon.Verbs[0].defaultProjectile.projectile != null;
		}

		
		internal static bool HasSoundcast(this ThingDef weapon)
		{
			return weapon != null && !weapon.Verbs.NullOrEmpty<VerbProperties>() && weapon.Verbs[0].soundCast != null;
		}

		
		internal static ThingWithComps RandomEquippedWeapon(this Pawn pawn)
		{
			return pawn.HasAnyWeapon() ? pawn.equipment.AllEquipmentListForReading.RandomElement<ThingWithComps>() : null;
		}

		
		internal static ThingWithComps ThisOrFirstWeapon(this Pawn p, ThingWithComps w)
		{
			return p.HasAnyWeapon() ? (p.IsEquippedByPawn(w) ? w : p.equipment.AllEquipmentListForReading.FirstOrDefault<ThingWithComps>()) : null;
		}

		
		internal static string GetAllWeaponsAsSeparatedString(this Pawn p)
		{
			bool flag = !p.HasEquipmentTracker() || p.equipment.AllEquipmentListForReading.NullOrEmpty<ThingWithComps>();
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				string text = "";
				foreach (ThingWithComps t in p.equipment.AllEquipmentListForReading)
				{
					text += t.GetAsSeparatedString();
					text += ":";
				}
				text = text.SubstringRemoveLast();
				result = text;
			}
			return result;
		}

		
		internal static void SetWeaponsFromSeparatedString(this Pawn p, string s)
		{
			bool flag = !p.HasEquipmentTracker();
			if (!flag)
			{
				p.equipment.DestroyAllEquipment(DestroyMode.Vanish);
				bool flag2 = s.NullOrEmpty();
				if (!flag2)
				{
					string[] array = s.SplitNo(":");
					foreach (string s2 in array)
					{
						string[] array3 = s2.SplitNo("|");
						bool flag3 = array3.Length >= 6;
						if (flag3)
						{
							string styledefname = (array3.Length >= 7) ? array3[6] : "";
							ThingWithComps thingWithComps = WeaponTool.GenerateWeapon(Selected.ByName(array3[0], array3[1], styledefname, array3[2].HexStringToColor(), array3[3].AsInt32(), array3[4].AsInt32()));
							bool flag4 = thingWithComps != null;
							if (flag4)
							{
								thingWithComps.HitPoints = array3[5].AsInt32();
								p.equipment.AddEquipment(thingWithComps);
							}
						}
					}
				}
			}
		}

		
		internal static void DestroyAllEquipment(this Pawn pawn)
		{
			bool flag = !pawn.HasEquipmentTracker();
			if (!flag)
			{
				pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
			}
		}

		
		internal static void DestroyEquipment(this Pawn pawn, ThingWithComps weapon)
		{
			bool flag = !pawn.HasEquipmentTracker();
			if (!flag)
			{
				pawn.equipment.DestroyEquipment(weapon);
			}
		}

		
		internal static List<ThingWithComps> ListOfCopyWeapons(this Pawn pawn)
		{
			return pawn.HasAnyWeapon() ? pawn.equipment.AllEquipmentListForReading.ListFullCopy<ThingWithComps>() : null;
		}

		
		internal static void PasteCopyWeapons(this Pawn pawn, List<ThingWithComps> l)
		{
			bool flag = pawn.HasEquipmentTracker();
			if (flag)
			{
				pawn.DestroyAllEquipment();
				bool flag2 = !l.NullOrEmpty<ThingWithComps>();
				if (flag2)
				{
					bool firstWeapon = true;
					foreach (ThingWithComps thing in l)
					{
						ThingWithComps w = WeaponTool.GenerateWeapon(Selected.ByThing(thing));
						pawn.AddWeaponToEquipment(w, firstWeapon, true);
						firstWeapon = false;
					}
				}
			}
			CEditor.API.UpdateGraphics();
		}

		
		internal static void CreateAndWearEquipment(this Pawn pawn, Selected s, bool firstWeapon)
		{
			bool flag = !pawn.HasEquipmentTracker();
			if (!flag)
			{
				ThingWithComps w = WeaponTool.GenerateWeapon(s);
				pawn.AddWeaponToEquipment(w, firstWeapon, true);
			}
		}

		
		internal static void AddWeaponToEquipment(this Pawn pawn, ThingWithComps w, bool firstWeapon, bool destroyOld = true)
		{
			bool flag = !pawn.HasEquipmentTracker() || w == null;
			if (!flag)
			{
				if (firstWeapon)
				{
					bool flag2 = pawn.equipment.Primary != null;
					if (flag2)
					{
						if (destroyOld)
						{
							pawn.equipment.Primary.Destroy(DestroyMode.Vanish);
						}
						else
						{
							ThingWithComps primary = pawn.equipment.Primary;
							pawn.equipment.Remove(primary);
							pawn.AddItemToInventory(primary);
						}
					}
					pawn.equipment.AddEquipment(w);
				}
				else
				{
					bool isDualWieldActive = CEditor.IsDualWieldActive;
					if (isDualWieldActive)
					{
						try
						{
							Type atype = Reflect.GetAType("DualWield", "Ext_Pawn_EquipmentTracker");
							atype.CallMethod("MakeRoomForOffHand", new object[]
							{
								pawn.equipment,
								w
							});
							atype.CallMethod("AddOffHandEquipment", new object[]
							{
								pawn.equipment,
								w
							});
						}
						catch
						{
						}
					}
				}
			}
		}

		
		internal static ThingWithComps GenerateRandomWeapon(bool originalColors = true)
		{
			WeaponType weaponType = (CEditor.zufallswert.Next(0, 100) > 50) ? WeaponType.Ranged : WeaponType.Melee;
			HashSet<ThingDef> l = WeaponTool.ListOfWeapons(null, weaponType);
			return WeaponTool.GenerateWeapon(Selected.Random(l, originalColors, false));
		}

		
		internal static ThingWithComps GenerateWeapon(Selected s)
		{
			bool flag = s == null || s.thingDef == null;
			ThingWithComps result;
			if (flag)
			{
				result = null;
			}
			else
			{
				ThingDef thingDef = DefTool.ThingDef(s.thingDef.defName);
				bool flag2 = thingDef == null || !thingDef.IsWeapon;
				if (flag2)
				{
					result = null;
				}
				else
				{
					s.stuff = s.thingDef.ThisOrDefaultStuff(s.stuff);
					bool flag3 = !s.thingDef.MadeFromStuff;
					if (flag3)
					{
						s.stuff = null;
					}
					ThingWithComps thingWithComps = (ThingWithComps)ThingMaker.MakeThing(s.thingDef, s.stuff);
					thingWithComps.HitPoints = thingWithComps.MaxHitPoints;
					thingWithComps.SetQuality(s.quality);
					thingWithComps.SetDrawColor(s.DrawColor);
					thingWithComps.stackCount = s.stackVal;
					bool flag4 = s.style != null;
					if (flag4)
					{
						thingWithComps.StyleDef = s.style;
						thingWithComps.StyleDef.color = s.style.color;
					}
					result = thingWithComps;
				}
			}
			return result;
		}

		
		internal static bool IsTurret(this ThingDef t)
		{
			bool flag = t == null;
			return !flag && (t.thingClass == typeof(Building_TurretGun) || (t.thingClass != null && CEditor.IsCombatExtendedActive && t.thingClass.ToString().Contains("Building_TurretGun")) || (t.building != null && t.building.turretGunDef != null));
		}

		
		internal static bool IsTurretGun(this ThingDef t)
		{
			return t != null && t.weaponTags != null && t.weaponTags.Contains("TurretGun");
		}

		
		internal static bool IsBullet(this ThingDef t)
		{
			return t != null && t.projectile != null;
		}

		
		internal static void PasteWeaponTag(this ThingDef weapon, List<string> l)
		{
			bool flag = weapon == null || l.NullOrEmpty<string>();
			if (!flag)
			{
				bool flag2 = weapon.weaponTags == null;
				if (flag2)
				{
					weapon.weaponTags = new List<string>();
				}
				foreach (string tag in l)
				{
					weapon.AddWeaponTag(tag);
				}
			}
		}

		
		internal static bool HasWeaponTag(this ThingDef weapon, string tag)
		{
			return weapon.HasAnyWeaponTags() && weapon.weaponTags.Contains(tag);
		}

		
		internal static void AddWeaponTag(this ThingDef t, string tag)
		{
			bool flag = t == null || tag == null;
			if (!flag)
			{
				bool flag2 = t.weaponTags == null;
				if (flag2)
				{
					t.weaponTags = new List<string>();
				}
				bool flag3 = !t.HasWeaponTag(tag);
				if (flag3)
				{
					t.weaponTags.Add(tag);
				}
				t.ResolveReferences();
			}
		}

		
		internal static void RemoveWeaponTag(this ThingDef weapon, string tag)
		{
			bool flag = !weapon.HasAnyWeaponTags();
			if (!flag)
			{
				foreach (string a in weapon.weaponTags)
				{
					bool flag2 = a == tag;
					if (flag2)
					{
						weapon.weaponTags.Remove(tag);
						break;
					}
				}
				weapon.ResolveReferences();
			}
		}

		
		internal static void SetBullet(this ThingDef weapon, ThingDef bullet)
		{
			bool flag = !weapon.HasVerb();
			if (!flag)
			{
				weapon.Verbs[0].defaultProjectile = bullet;
				bool flag2 = weapon.Verbs[0].defaultProjectile != null;
				if (flag2)
				{
					weapon.Verbs[0].defaultProjectile.ResolveReferences();
				}
			}
		}

		
		internal static string GetBulletDefName(this ThingDef weapon)
		{
			return (!weapon.HasProjectile()) ? "" : weapon.Verbs[0].defaultProjectile.defName;
		}

		
		internal static string GetSoundCastDefName(this ThingDef weapon)
		{
			return (!weapon.HasSoundcast()) ? "" : weapon.Verbs[0].soundCast.defName;
		}

		
		internal static float GetBulletExplosionRadius(this ThingDef weapon)
		{
			return (!weapon.HasProjectile()) ? 0f : weapon.Verbs[0].defaultProjectile.projectile.explosionRadius;
		}

		
		internal static float GetBulletStoppingPower(this ThingDef weapon)
		{
			return (!weapon.HasProjectile()) ? 0f : weapon.Verbs[0].defaultProjectile.projectile.stoppingPower;
		}

		
		internal static float GetBulletSpeed(this ThingDef weapon)
		{
			return (!weapon.HasProjectile()) ? 0f : weapon.Verbs[0].defaultProjectile.projectile.speed;
		}

		
		internal static void SetBulletSpeed(this ThingDef weapon, float speed)
		{
			bool flag = !weapon.HasProjectile();
			if (!flag)
			{
				weapon.Verbs[0].defaultProjectile.projectile.speed = speed;
			}
		}

		
		internal static void SetStoppingPower(this ThingDef weapon, float power)
		{
			bool flag = !weapon.HasProjectile();
			if (!flag)
			{
				weapon.Verbs[0].defaultProjectile.projectile.stoppingPower = power;
			}
		}

		
		internal static void SetExplosionRadius(this ThingDef weapon, float radius)
		{
			bool flag = !weapon.HasProjectile();
			if (!flag)
			{
				weapon.Verbs[0].defaultProjectile.projectile.explosionRadius = radius;
			}
		}

		
		internal static bool GetWeaponTargetGround(this ThingDef weapon)
		{
			return weapon.HasVerb() && weapon.Verbs[0].targetParams != null && weapon.Verbs[0].targetParams.canTargetLocations;
		}

		
		internal static int GetDmg(this ThingDef weapon)
		{
			bool flag = weapon == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool isMeleeWeapon = weapon.IsMeleeWeapon;
				if (isMeleeWeapon)
				{
					float num = weapon.GetStatValue(StatDefOf.MeleeWeapon_DamageMultiplier);
					num = (weapon.HasStat(StatDefOf.MeleeWeapon_DamageMultiplier) ? num : 1f);
					result = ((weapon.tools != null) ? ((int)(weapon.tools[0].power * num)) : 0);
				}
				else
				{
					float num2 = weapon.GetStatValue(StatDefOf.RangedWeapon_DamageMultiplier);
					num2 = (weapon.HasStat(StatDefOf.RangedWeapon_DamageMultiplier) ? num2 : 1f);
					bool flag2 = weapon.IsTurret();
					if (flag2)
					{
						result = ((weapon.building.turretGunDef.Verbs.Count > 0 && weapon.building.turretGunDef.Verbs[0].defaultProjectile != null) ? weapon.building.turretGunDef.Verbs[0].defaultProjectile.projectile.GetDamageAmount(num2, null) : 0);
					}
					else
					{
						int num3 = (weapon.Verbs.Count > 0 && weapon.Verbs[0].defaultProjectile != null && weapon.Verbs[0].defaultProjectile.projectile != null) ? weapon.Verbs[0].defaultProjectile.projectile.GetDamageAmount(num2, null) : 0;
						result = num3;
					}
				}
			}
			return result;
		}

		
		internal static void SetTargetParams(this ThingDef weapon, bool canTargetGround)
		{
			bool flag = !weapon.HasVerb();
			if (!flag)
			{
				bool flag2 = weapon.Verbs[0].targetParams == null;
				if (flag2)
				{
					weapon.Verbs[0].targetParams = new TargetingParameters();
				}
				weapon.Verbs[0].targetParams.canTargetLocations = canTargetGround;
			}
		}

		
		internal static ThingDef GetTurretDef(this ThingDef gun)
		{
			bool flag = gun == null || gun.defName.NullOrEmpty();
			ThingDef result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Dictionary<string, ThingDef> dicGunAndTurret = PresetObject.DicGunAndTurret;
				bool flag2 = dicGunAndTurret != null;
				if (flag2)
				{
					string text = dicGunAndTurret.KeyByValue(gun);
					bool flag3 = !text.NullOrEmpty();
					if (flag3)
					{
						result = DefTool.ThingDef(text);
					}
					else
					{
						bool flag4 = dicGunAndTurret.ContainsKey(gun.defName);
						if (flag4)
						{
							result = gun;
						}
						else
						{
							result = null;
						}
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		internal static ThingDef GetRealWeaponDef(this ThingDef gun)
		{
			bool flag = gun == null || gun.defName.NullOrEmpty();
			ThingDef result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Dictionary<string, ThingDef> dicGunAndTurret = PresetObject.DicGunAndTurret;
				bool flag2 = dicGunAndTurret != null;
				if (flag2)
				{
					string str = dicGunAndTurret.KeyByValue(gun);
					bool flag3 = !str.NullOrEmpty();
					if (flag3)
					{
						result = gun;
					}
					else
					{
						bool flag4 = dicGunAndTurret.ContainsKey(gun.defName);
						if (flag4)
						{
							result = dicGunAndTurret[gun.defName];
						}
						else
						{
							result = gun;
						}
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		internal static Selected SelectorForPawnSpecificWeapon(Pawn pawn)
		{
			Selected result;
			try
			{
				string modname = pawn.kindDef.GetModName();
				WeaponType weaponType = (WeaponType)CEditor.zufallswert.Next(0, 2);
				HashSet<ThingDef> source = WeaponTool.ListOfWeapons(null, weaponType);
				HashSet<ThingDef> hashSet = WeaponTool.ListOfWeapons(modname, weaponType);
				bool flag = hashSet.NullOrEmpty<ThingDef>() || hashSet.Count < 4;
				HashSet<ThingDef> l;
				if (flag)
				{
					l = (from td in source
					where td.IsFromMod(modname) || (td.weaponTags != null && (from t in td.weaponTags
					select pawn.kindDef.weaponTags.Contains(t)) != null)
					select td).ToHashSet<ThingDef>();
				}
				else
				{
					l = hashSet;
				}
				result = Selected.Random(l, true, false);
			}
			catch
			{
				result = null;
			}
			return result;
		}

		
		internal static bool IsPrimaryWeapon(this Pawn pawn, ThingWithComps w)
		{
			return w != null && w == CEditor.API.Pawn.equipment.Primary;
		}

		
		internal static void Reequip(this Pawn pawn, Selected selected, int primary = 0, bool pawnSpecific = false)
		{
			bool flag = !pawn.HasEquipmentTracker();
			if (!flag)
			{
				primary = ((primary < 0) ? CEditor.zufallswert.Next(0, CEditor.IsDualWieldActive ? 3 : 1) : primary);
				int num = pawn.equipment.AllEquipmentListForReading.CountAllowNull<ThingWithComps>();
				bool flag2 = primary != 0 && num > 0;
				if (flag2)
				{
					for (int i = 0; i < pawn.equipment.AllEquipmentListForReading.CountAllowNull<ThingWithComps>(); i++)
					{
						ThingWithComps thingWithComps = pawn.equipment.AllEquipmentListForReading[i];
						bool flag3 = pawn.equipment.Primary == thingWithComps && (primary == 0 || primary >= 2);
						if (flag3)
						{
							pawn.equipment.DestroyEquipment(thingWithComps);
						}
						bool flag4 = pawn.equipment.Primary != thingWithComps && (primary == 1 || primary >= 2);
						if (flag4)
						{
							pawn.equipment.DestroyEquipment(thingWithComps);
						}
					}
				}
				bool flag5 = pawnSpecific && selected == null;
				if (flag5)
				{
					selected = WeaponTool.SelectorForPawnSpecificWeapon(pawn);
				}
				ThingWithComps thingWithComps2 = null;
				int num2 = 3;
				ThingWithComps w;
				do
				{
					w = ((selected == null) ? WeaponTool.GenerateRandomWeapon(true) : WeaponTool.GenerateWeapon(selected));
					bool flag6 = primary <= 0 || (primary == 1 && pawn.equipment.Primary.DestroyedOrNull()) || CEditor.InStartingScreen;
					if (flag6)
					{
						pawn.AddWeaponToEquipment(w, true, true);
					}
					else
					{
						bool flag7 = primary == 1;
						if (flag7)
						{
							pawn.AddWeaponToEquipment(w, false, true);
						}
						else
						{
							bool flag8 = primary == 2;
							if (flag8)
							{
								pawn.AddWeaponToEquipment(w, true, true);
								thingWithComps2 = WeaponTool.GenerateRandomWeapon(true);
								pawn.AddWeaponToEquipment(thingWithComps2, false, true);
							}
						}
					}
					num2--;
					primary = CEditor.zufallswert.Next(0, CEditor.IsDualWieldActive ? 3 : 1);
				}
				while (!pawn.IsEquippedByPawn(w) && !pawn.IsEquippedByPawn(thingWithComps2) && num2 > 0);
				bool flag9 = selected != null;
				if (flag9)
				{
					bool flag10 = primary == 1 && pawn.equipment.Primary != null;
					if (flag10)
					{
						pawn.equipment.Primary.DrawColor = selected.DrawColor;
					}
					bool flag11 = primary == 2 && thingWithComps2 != null;
					if (flag11)
					{
						thingWithComps2.DrawColor = selected.DrawColor;
					}
				}
			}
		}

		
		internal static void SetCompRefuelable(ThingDef turretDef, ThingDef weapon, int value)
		{
			bool flag = !CEditor.IsCombatExtendedActive && turretDef != null && turretDef.comps != null;
			if (flag)
			{
				CompProperties compByType = turretDef.GetCompByType(typeof(CompProperties_Refuelable));
				bool flag2 = compByType != null;
				if (flag2)
				{
					(compByType as CompProperties_Refuelable).fuelCapacity = (float)value;
				}
			}
			else
			{
				bool flag3 = CEditor.IsCombatExtendedActive && weapon != null && weapon.comps != null;
				if (flag3)
				{
					CompProperties compByType2 = weapon.GetCompByType("CombatExtended.CompProperties_AmmoUser");
					bool flag4 = compByType2 != null;
					if (flag4)
					{
						compByType2.SetMemberValue("magazineSize", value);
					}
				}
			}
		}

		
		internal static void SetCompReloadTime(ThingDef turretDef, ThingDef weapon, float value)
		{
			bool flag = CEditor.IsCombatExtendedActive && weapon != null && weapon.comps != null;
			if (flag)
			{
				CompProperties compByType = weapon.GetCompByType("CombatExtended.CompProperties_AmmoUser");
				bool flag2 = compByType != null;
				if (flag2)
				{
					compByType.SetMemberValue("reloadTime", value);
				}
			}
		}

		
		internal static float GetCompReloadTime(ThingDef turretDef, ThingDef weapon)
		{
			bool flag = CEditor.IsCombatExtendedActive && weapon != null && weapon.comps != null;
			if (flag)
			{
				CompProperties compByType = weapon.GetCompByType("CombatExtended.CompProperties_AmmoUser");
				bool flag2 = compByType != null;
				if (flag2)
				{
					return compByType.GetMemberValue("reloadTime", 0f);
				}
			}
			return 0f;
		}

		
		internal static CompProperties_Explosive GetCompExplosive(ThingDef t)
		{
			return (CompProperties_Explosive)t.GetCompByType(typeof(CompProperties_Explosive));
		}

		
		internal static int GetCompRefuelable(ThingDef turretDef, ThingDef weapon)
		{
			bool flag = !CEditor.IsCombatExtendedActive && turretDef != null && turretDef.comps != null;
			if (flag)
			{
				CompProperties compByType = turretDef.GetCompByType(typeof(CompProperties_Refuelable));
				bool flag2 = compByType != null;
				if (flag2)
				{
					return (int)(compByType as CompProperties_Refuelable).fuelCapacity;
				}
			}
			else
			{
				bool flag3 = CEditor.IsCombatExtendedActive && weapon != null && weapon.comps != null;
				if (flag3)
				{
					CompProperties compByType2 = weapon.GetCompByType("CombatExtended.CompProperties_AmmoUser");
					bool flag4 = compByType2 != null;
					if (flag4)
					{
						return compByType2.GetMemberValue("magazineSize", 0);
					}
				}
			}
			return 0;
		}

		
		internal static string GetNameForWeaponType(WeaponType type)
		{
			bool flag = type == WeaponType.Melee;
			string result;
			if (flag)
			{
				result = Label.MELEE;
			}
			else
			{
				bool flag2 = type == WeaponType.Ranged;
				if (flag2)
				{
					result = Label.RANGED;
				}
				else
				{
					bool flag3 = type == WeaponType.Turret;
					if (flag3)
					{
						result = Label.TURRET;
					}
					else
					{
						bool flag4 = type == WeaponType.TurretGun;
						if (flag4)
						{
							result = Label.TURRETGUN;
						}
						else
						{
							result = "";
						}
					}
				}
			}
			return result;
		}

		
		internal static List<ThingDef> ListOfTurrets()
		{
			return (from td in DefDatabase<ThingDef>.AllDefs
			where !td.defName.NullOrEmpty() && !td.label.NullOrEmpty() && td.IsTurret()
			orderby td.label
			select td).ToList<ThingDef>();
		}

		
		internal static HashSet<ThingDef> ListOfWeapons(string modname, WeaponType weaponType)
		{
			Dictionary<string, ThingDef> dicTurrets = PresetObject.DicGunAndTurret;
			bool bAll1 = modname.NullOrEmpty();
			return (from td in DefDatabase<ThingDef>.AllDefs
			where !td.label.NullOrEmpty() && ((td.IsMeleeWeapon && weaponType == WeaponType.Melee) || (td.IsRangedWeapon && weaponType == WeaponType.Ranged && !dicTurrets.Values.Contains(td)) || (td.IsTurret() && weaponType == WeaponType.Turret) || (td.IsRangedWeapon && weaponType == WeaponType.TurretGun && dicTurrets.Values.Contains(td))) && (bAll1 || td.IsFromMod(modname))
			orderby td.label
			select td).ToHashSet<ThingDef>();
		}

		
		internal static HashSet<ThingDef> ListOfBullets(ThingCategoryDef tc, string modname)
		{
			bool bAll1 = modname.NullOrEmpty();
			bool bAll2 = tc == null;
			return (from td in DefDatabase<ThingDef>.AllDefs
			where td.IsBullet() && !td.label.NullOrEmpty() && (bAll1 || td.IsFromMod(modname)) && (bAll2 || (!td.thingCategories.NullOrEmpty<ThingCategoryDef>() && td.thingCategories.Contains(tc)))
			orderby td.label
			select td).ToHashSet<ThingDef>();
		}

		
		internal static List<SoundDef> ListOfSounds(bool all)
		{
			List<SoundDef> result;
			if (all)
			{
				List<SoundDef> list = (from td in DefDatabase<SoundDef>.AllDefs
				where !td.sustain
				orderby td.defName
				select td).ToList<SoundDef>();
				list.RemoveDuplicates(null);
				result = list;
			}
			else
			{
				List<SoundDef> list2 = (from td in DefDatabase<ThingDef>.AllDefs
				where td.IsWeapon && td.HasSoundcast()
				orderby td.Verbs[0].soundCast.defName
				select td.Verbs[0].soundCast).ToList<SoundDef>();
				list2.RemoveDuplicates(null);
				result = list2;
			}
			return result;
		}
	}
}
