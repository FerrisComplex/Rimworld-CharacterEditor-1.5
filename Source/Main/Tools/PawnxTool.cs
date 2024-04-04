using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace CharacterEditor
{
	
	internal static class PawnxTool
	{
		
		internal static bool IsAnimal(this Pawn p)
		{
			return p != null && p.kindDef.IsAnimal();
		}

		
		internal static Pawn GetOtherPawnFromSeparatedString(string s)
		{
			Pawn result = null;
			bool flag = !s.NullOrEmpty();
			if (flag)
			{
				bool flag2 = s.Contains("?");
				if (flag2)
				{
					result = NameTool.GetPawnByNameTriple(NameTool.GetPawnNameFromSeparatedString(s));
				}
				else
				{
					result = NameTool.GetPawnByNameSingle(NameTool.GetPawnNameFromSeparatedString(s, null));
				}
			}
			return result;
		}

		
		internal static void CreatePawnLord(this Pawn newPawn, IntVec3 forcePosition = default(IntVec3))
		{
			bool flag = newPawn == null;
			if (!flag)
			{
				Faction faction = newPawn.Faction;
				bool flag2 = faction != null && faction != Faction.OfPlayer;
				if (flag2)
				{
					Lord lord = null;
					try
					{
						bool flag3 = newPawn.Map.mapPawns.SpawnedPawnsInFaction(faction).Any((Pawn p) => p != newPawn);
						if (flag3)
						{
							Pawn p2 = (Pawn)GenClosest.ClosestThing_Global(newPawn.Position, newPawn.Map.mapPawns.SpawnedPawnsInFaction(faction), 99999f, (Thing p) => p != newPawn && ((Pawn)p).GetLord() != null, null);
							lord = p2.GetLord();
						}
					}
					catch
					{
					}
					bool flag4 = lord == null;
					if (flag4)
					{
						LordJob_DefendPoint lordJob = new LordJob_DefendPoint((forcePosition == default(IntVec3)) ? newPawn.Position : forcePosition);
						lord = LordMaker.MakeNewLord(faction, lordJob, Find.CurrentMap, null);
					}
					lord.AddPawn(newPawn);
				}
			}
		}

		
		internal static void SpawnOrPassPawn(this Pawn p, Faction f, PresetPawn ppn, IntVec3 pos = default(IntVec3), bool doPlace = false)
		{
			bool inStartingScreen = CEditor.InStartingScreen;
			if (inStartingScreen)
			{
				bool flag = CEditor.ListName == Label.COLONISTS;
				if (flag)
				{
					p.PostCompsAfterCreate(ppn);
				}
				else
				{
					p.PassToWorld(ppn, pos);
				}
			}
			else
			{
				bool flag2 = !CEditor.OnMap;
				if (flag2)
				{
					p.PassToWorld(ppn, pos);
				}
				else
				{
					bool shift = Event.current.shift;
					if (shift)
					{
						CEditor.API.EditorMoveRight();
						PlacingTool.DropPawnWithPod(p, ppn);
					}
					else
					{
						bool flag3 = doPlace || (ppn != null && ppn.bPlace) || (ppn == null && Event.current.control);
						if (flag3)
						{
							CEditor.API.EditorMoveRight();
							PlacingTool.PlaceInCustomPosition(p, ppn);
						}
						else
						{
							PlacingTool.PlaceInPosition(p, ppn, pos);
						}
					}
				}
			}
		}

		
		internal static void PostCompsAfterCreate(this Pawn p, PresetPawn ppn)
		{
			List<ThingComp> memberValue = p.GetMemberValue<List<ThingComp>>("comps", null);
			bool flag = memberValue != null;
			if (flag)
			{
				for (int i = 0; i < memberValue.Count; i++)
				{
					memberValue[i].PostSpawnSetup(true);
				}
			}
			PawnxTool.PostProcess(p, ppn);
		}

		
		internal static void PassToWorld(this Pawn p, PresetPawn ppn, IntVec3 pos = default(IntVec3))
		{
			bool flag = p == null;
			if (!flag)
			{
				bool flag2 = p.Faction.IsZombie();
				bool flag3 = flag2;
				if (flag3)
				{
					bool flag4 = !CEditor.InStartingScreen;
					if (flag4)
					{
						p.ZombieWorldCreationFixInSpace();
					}
					else
					{
						p.SpawnAsZombie(pos);
					}
				}
				else
				{
					bool flag5 = !Find.World.worldPawns.Contains(p);
					if (flag5)
					{
						Find.World.worldPawns.PassToWorld(p, PawnDiscardDecideMode.Decide);
					}
				}
				Dictionary<string, Faction> dicFactions = CEditor.API.DicFactions;
				bool flag6 = dicFactions != null;
				if (flag6)
				{
					Faction value = dicFactions.GetValue(CEditor.ListName);
					bool flag7 = value != null;
					if (flag7)
					{
						p.SetFaction(value, null);
					}
				}
				PawnxTool.PostProcess(p, ppn);
			}
		}

		
		internal static void SpawnPawn(this Pawn newPawn, PresetPawn ppn, IntVec3 pos = default(IntVec3))
		{
			bool flag = newPawn == null || CEditor.InStartingScreen;
			if (!flag)
			{
				try
				{
					bool flag2 = newPawn.Dead && newPawn.Corpse != null && newPawn.Corpse.Spawned;
					if (flag2)
					{
						newPawn.Corpse.DeSpawn(DestroyMode.Vanish);
					}
					bool spawned = newPawn.Spawned;
					if (spawned)
					{
						newPawn.DeSpawn(DestroyMode.Vanish);
					}
					bool flag3 = pos == default(IntVec3);
					if (flag3)
					{
						pos = UI.MouseCell();
						pos.x -= 5;
					}
					bool flag4 = !pos.InBounds(Find.CurrentMap);
					if (flag4)
					{
						pos = Find.CurrentMap.AllCells.RandomElement<IntVec3>();
					}
					bool flag5 = newPawn.Faction.IsZombie();
					bool flag6 = flag5;
					if (flag6)
					{
						newPawn.SpawnAsZombie(pos);
					}
					else
					{
						GenSpawn.Spawn(newPawn, pos, Find.CurrentMap, Rot4.South, WipeMode.Vanish, false);
						newPawn.CreatePawnLord(default(IntVec3));
					}
					bool flag7 = newPawn.Faction == Faction.OfPlayer;
					if (flag7)
					{
						newPawn.playerSettings.hostilityResponse = HostilityResponseMode.Attack;
					}
					PawnxTool.PostProcess(newPawn, ppn);
				}
				catch (Exception ex)
				{
					Log.Error(ex.Message + "\n" + ex.StackTrace);
				}
			}
		}

		
		internal static void PostProcess(Pawn p, PresetPawn ppn)
		{
			bool flag = p == null;
			if (!flag)
			{
				MeditationFocusTypeAvailabilityCache.ClearFor(p);
				StatsReportUtility.Reset();
				List<Pawn> list = CEditor.API.ListOf<Pawn>(EType.Pawns);
				bool flag2 = !list.Contains(p);
				if (flag2)
				{
					list.Insert(0, p);
				}
				CEditor.API.Pawn = p;
				bool inStartingScreen = CEditor.InStartingScreen;
				if (inStartingScreen)
				{
					PawnxTool.Notify_CheckStartPawnsListChanged();
				}
				bool flag3 = ppn != null;
				if (flag3)
				{
					ppn.PostProcess();
				}
				else
				{
					Capturer capturer = CEditor.API.Get<Capturer>(EType.Capturer);
					capturer.UpdatePawnGraphic(p);
				}
			}
		}

		
		internal static void SpawnAsZombie(this Pawn pawn, IntVec3 pos)
		{
			try
			{
				bool flag = pawn.story != null;
				if (flag)
				{
					pawn.story.traits.allTraits.Clear();
				}
				pawn.inventory.DestroyAll(DestroyMode.Vanish);
				pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
				GenSpawn.Spawn(pawn, pos, Find.CurrentMap, Rot4.South, WipeMode.Vanish, false);
				object[] param = new object[]
				{
					pawn,
					false
				};
				Reflect.GetAType("ZombieLand", "Tools").CallMethod("ConvertToZombie", param);
				bool dead = pawn.Dead;
				if (dead)
				{
					pawn.Delete(false);
				}
			}
			catch
			{
			}
		}

		
		internal static void DiscardGeneratedPawn(this Pawn p)
		{
			bool flag = p == null;
			if (!flag)
			{
				try
				{
					typeof(PawnGenerator).CallMethod("DiscardGeneratedPawn", new object[]
					{
						p
					});
				}
				catch
				{
				}
			}
		}

		
		internal static void Delete(this Pawn p, bool force = false)
		{
			bool flag = p == null;
			if (!flag)
			{
				try
				{
					List<Pawn> list = CEditor.API.ListOf<Pawn>(EType.Pawns);
					bool flag2 = !list.NullOrEmpty<Pawn>() && list.Contains(p);
					if (flag2)
					{
						bool flag3 = !force && CEditor.InStartingScreen && list.Count == 1;
						if (flag3)
						{
							MessageTool.Show("can't delete below the minimum number", MessageTypeDefOf.RejectInput);
							return;
						}
						p.ideo = null;
						list.Remove(p);
						CEditor.API.Pawn = list.FirstOrFallback(null);
						bool flag4 = CEditor.InStartingScreen && CEditor.ListName == Label.COLONISTS;
						if (flag4)
						{
							PawnxTool.Notify_CheckStartPawnsListChanged();
						}
					}
					bool flag5 = p.Dead && p.Corpse != null && p.Corpse.Spawned;
					if (flag5)
					{
						p.Corpse.DeSpawn(DestroyMode.Vanish);
					}
					bool spawned = p.Spawned;
					if (spawned)
					{
						p.DeSpawn(DestroyMode.Vanish);
					}
					bool flag6 = !p.Destroyed;
					if (flag6)
					{
						p.Destroy(DestroyMode.Vanish);
					}
					Find.World.worldPawns.RemoveAndDiscardPawnViaGC(p);
				}
				catch (Exception ex)
				{
					Log.Error(ex.Message + "\n" + ex.StackTrace);
				}
			}
		}

		
		internal static void DeleteAllPawns(string listname, bool onMap, Faction faction)
		{
			List<Pawn> pawnList = PawnxTool.GetPawnList(listname, onMap, faction);
			for (int i = pawnList.Count - 1; i >= 0; i--)
			{
				pawnList[i].Delete(true);
			}
			PawnxTool.Notify_CheckStartPawnsListChanged();
			CEditor.API.UpdateGraphics();
		}

		
		internal static void Notify_CheckStartPawnsListChanged()
		{
			bool flag = CEditor.InStartingScreen && CEditor.ListName == Label.COLONISTS;
			if (flag)
			{
				Find.GameInitData.startingAndOptionalPawns.Clear();
				Find.GameInitData.startingAndOptionalPawns.AddRange(CEditor.API.ListOf<Pawn>(EType.Pawns));
				bool flag2 = Find.GameInitData.startingPawnCount > Find.GameInitData.startingAndOptionalPawns.Count;
				if (flag2)
				{
					Find.GameInitData.startingPawnCount = Find.GameInitData.startingAndOptionalPawns.Count;
				}
			}
		}

		
		internal static bool FactionIsNotInsectNotMecha(Faction f)
		{
			return f != Faction.OfInsects && f != Faction.OfMechanoids;
		}

		
		internal static bool KeyIsHumanoid(bool notInsectNotMecha, string key)
		{
			return notInsectNotMecha && key != Label.COLONYANIMALS && key != Label.WILDANIMALS;
		}

		
		internal static bool KeyIsAnimal(bool notInsectNotMecha, string key, Faction f)
		{
			return notInsectNotMecha && key != Label.COLONISTS && (key == Label.COLONYANIMALS || key == Label.WILDANIMALS || (f != null && f.def.defName != "Zombies"));
		}

		
		internal static bool KeyIsOther(string key, Faction f)
		{
			return f != null && key != Label.COLONISTS && key != Label.COLONYANIMALS;
		}

		
		internal static bool FactionIsXeno(Faction f)
		{
			return f != null && f.Name.ToLower().Contains("xenomorph");
		}

		
		internal static bool IsOnMap(this Pawn p)
		{
			return p != null && (p.Spawned || (p.Corpse != null && p.Corpse.Spawned));
		}

		
		internal static List<Pawn> GetPawnList(string key, bool onMap, Faction f = null)
		{
			List<Pawn> result;
			try
			{
				bool flag = CEditor.InStartingScreen && key == Label.COLONISTS;
				if (flag)
				{
					List<Pawn> list = Find.GameInitData.startingAndOptionalPawns.ListFullCopy<Pawn>();
					result = list;
				}
				else
				{
					bool flag2;
					bool flag3;
					bool allFac;
					bool humanoid;
					bool animal;
					bool other;
					PawnxTool.SetPawnKindFlags(key, f, out flag2, out humanoid, out animal, out other, out flag3, out allFac);
					Map currentMap = Find.CurrentMap;
					List<Pawn> list2;
					if (currentMap == null)
					{
						list2 = null;
					}
					else
					{
						MapPawns mapPawns = currentMap.mapPawns;
						list2 = ((mapPawns != null) ? mapPawns.AllPawns : null);
					}
					List<Pawn> list3 = list2;
					bool flag4 = list3 == null;
					if (flag4)
					{
						list3 = new List<Pawn>();
					}
					World world = Find.World;
					List<Pawn> list4;
					if (world == null)
					{
						list4 = null;
					}
					else
					{
						WorldPawns worldPawns = world.worldPawns;
						list4 = ((worldPawns != null) ? worldPawns.AllPawnsAliveOrDead : null);
					}
					List<Pawn> list5 = list4;
					bool flag5 = list5 == null;
					if (flag5)
					{
						list5 = new List<Pawn>();
					}
					List<Pawn> list6 = list3.Concat(list5).ToList<Pawn>();
					bool flag6 = list6 == null;
					if (flag6)
					{
						list6 = new List<Pawn>();
					}
					bool o = CEditor.API.GetO(OptionB.USESORTEDPAWNLIST);
					List<Pawn> list7;
					if (o)
					{
						list7 = (from td in list6
						where (allFac || td.Faction == f) && ((humanoid && td.RaceProps.Humanlike) || (animal && td.RaceProps.Animal) || (other && !td.RaceProps.Animal && !td.RaceProps.Humanlike)) && (!onMap || td.Spawned || (td.Corpse != null && td.Corpse.Spawned))
						select td).OrderBy(delegate(Pawn td)
						{
							Faction faction = td.Faction;
							return (faction != null) ? faction.Name : null;
						}).ThenBy((Pawn td) => td.LabelShort).ToList<Pawn>();
					}
					else
					{
						list7 = (from td in list6
						where (allFac || td.Faction == f) && ((humanoid && td.RaceProps.Humanlike) || (animal && td.RaceProps.Animal) || (other && !td.RaceProps.Animal && !td.RaceProps.Humanlike)) && (!onMap || td.Spawned || (td.Corpse != null && td.Corpse.Spawned))
						select td).ToList<Pawn>();
					}
					result = list7;
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		
		internal static void SetPawnKindFlags(string key, Faction f, out bool notInsectNotMecha, out bool humanoid, out bool animal, out bool other, out bool xeno, out bool allFac)
		{
			notInsectNotMecha = PawnxTool.FactionIsNotInsectNotMecha(f);
			humanoid = PawnxTool.KeyIsHumanoid(notInsectNotMecha, key);
			animal = PawnxTool.KeyIsAnimal(notInsectNotMecha, key, f);
			other = (PawnxTool.KeyIsOther(key, f) && f.def.defName != "Zombies");
			xeno = PawnxTool.FactionIsXeno(f);
			allFac = (f == null);
		}

		
		internal static void TeleportPawn(this Pawn pawn, IntVec3 pos = default(IntVec3))
		{
			bool flag = pawn == null;
			if (!flag)
			{
				pos = ((pos == default(IntVec3)) ? UI.MouseCell() : pos);
				bool flag2 = !pos.InBounds(Find.CurrentMap);
				if (!flag2)
				{
					bool flag3 = !pawn.Spawned || !pawn.Position.InBounds(Find.CurrentMap) || (pawn.Dead && (pawn.Corpse == null || pawn.Corpse.Position.InBounds(Find.CurrentMap)));
					bool flag4 = flag3;
					if (flag4)
					{
						pawn.SpawnPawn(null, pos);
					}
					bool dead = pawn.Dead;
					if (dead)
					{
						pawn.Corpse.Position = pos;
					}
					else
					{
						pawn.Position = pos;
						pawn.Notify_Teleported(true, true);
						pawn.Position = pos;
						pawn.Notify_Teleported(true, true);
					}
				}
			}
		}

		
		internal static string GetTrainabilityLabel(this Pawn p)
		{
			return "CreatureTrainability".Translate().Formatted(p.def.label) + ": " + p.RaceProps.trainability.LabelCap;
		}

		
		internal static string GetWildnessLabel(this Pawn p)
		{
			return "CreatureWildness".Translate().Formatted(p.def.label) + ": " + p.RaceProps.wildness.ToStringPercent();
		}

		
		internal static void RecruitPawn(this Pawn p)
		{
			bool flag = p == null || p.Faction == Faction.OfPlayer || !p.RaceProps.Humanlike;
			if (!flag)
			{
				bool inStartingScreen = CEditor.InStartingScreen;
				Pawn recruiter;
				if (inStartingScreen)
				{
					recruiter = Find.GameInitData.startingAndOptionalPawns.RandomElement<Pawn>();
				}
				else
				{
					recruiter = PawnxTool.GetPawnList(Label.COLONISTS, CEditor.OnMap, Faction.OfPlayer).RandomElement<Pawn>();
				}
				bool inStartingScreen2 = CEditor.InStartingScreen;
				if (inStartingScreen2)
				{
					p.SetFaction(Faction.OfPlayer, recruiter);
				}
				else
				{
					InteractionWorker_RecruitAttempt.DoRecruit(recruiter, p, true);
				}
				bool inStartingScreen3 = CEditor.InStartingScreen;
				if (inStartingScreen3)
				{
					Find.GameInitData.startingAndOptionalPawns.Add(p);
					CEditor.API.ListOf<Pawn>(EType.Pawns).Add(p);
				}
				else
				{
					DebugActionsUtility.DustPuffFrom(p);
				}
				p.AllowAllApparel(null);
			}
		}

		
		internal static void EnslavePawn(this Pawn p)
		{
			bool flag = p == null || p.Faction == Faction.OfPlayer || !p.RaceProps.Humanlike;
			if (!flag)
			{
				bool inStartingScreen = CEditor.InStartingScreen;
				Pawn warden;
				if (inStartingScreen)
				{
					warden = Find.GameInitData.startingAndOptionalPawns.RandomElement<Pawn>();
				}
				else
				{
					warden = PawnxTool.GetPawnList(Label.COLONISTS, CEditor.OnMap, Faction.OfPlayer).RandomElement<Pawn>();
				}
				GenGuest.EnslavePrisoner(warden, p);
				bool inStartingScreen2 = CEditor.InStartingScreen;
				if (inStartingScreen2)
				{
					Find.GameInitData.startingAndOptionalPawns.Add(p);
					CEditor.API.ListOf<Pawn>(EType.Pawns).Add(p);
				}
				else
				{
					DebugActionsUtility.DustPuffFrom(p);
				}
				p.AllowAllApparel(null);
			}
		}

		
		internal static string GetNamesOfUsedMods(string modids)
		{
			HashSet<string> l = PawnxTool.ModStringToHashSet(modids);
			return PawnxTool.ModListIdsToString(l);
		}

		
		internal static string GetNamesOfInactiveMods(string modids)
		{
			string text = "";
			HashSet<string> hashSet = PawnxTool.ModStringToHashSet(modids);
			try
			{
				foreach (string text2 in hashSet)
				{
					string text3 = text2.ToLower();
					bool flag = false;
					string str = text3;
					foreach (ModMetaData modMetaData in ModLister.AllInstalledMods)
					{
						bool flag2 = modMetaData.PackageId.ToLower().Equals(text3);
						if (flag2)
						{
							bool active = modMetaData.Active;
							if (active)
							{
								flag = true;
							}
							else
							{
								str = modMetaData.Name;
							}
							break;
						}
					}
					bool flag3 = !flag;
					if (flag3)
					{
						text = text + str + "|";
					}
				}
			}
			catch
			{
			}
			return text.SubstringRemoveLast();
		}

		
		internal static HashSet<string> ModStringToHashSet(string s)
		{
			bool flag = s.NullOrEmpty();
			HashSet<string> result;
			if (flag)
			{
				result = new HashSet<string>();
			}
			else
			{
				HashSet<string> hashSet = new HashSet<string>();
				try
				{
					s = s.Replace("[", "").Replace("]", "");
					string[] array = s.SplitNo("|");
					foreach (string item in array)
					{
						hashSet.Add(item);
					}
				}
				catch
				{
				}
				result = hashSet;
			}
			return result;
		}

		
		internal static string ModListIdsToString(HashSet<string> l)
		{
			string text = "";
			foreach (string text2 in l)
			{
				string value = text2.ToLower();
				foreach (ModMetaData modMetaData in ModLister.AllInstalledMods)
				{
					bool flag = modMetaData.PackageId.ToLower().Equals(value);
					if (flag)
					{
						text = text + modMetaData.Name + "|";
						break;
					}
				}
			}
			return text.SubstringRemoveLast();
		}

		
		internal static string GetUsedModIds(this Pawn p)
		{
			HashSet<string> hashSet = new HashSet<string>();
			bool flag = p != null;
			if (flag)
			{
				try
				{
					hashSet.Add(p.kindDef.modContentPack.PackageId);
					bool flag2 = p.HasStoryTracker() && p.story.bodyType != null;
					if (flag2)
					{
						hashSet.Add(p.story.bodyType.modContentPack.PackageId);
					}
					bool flag3 = p.HasStoryTracker() && p.story.hairDef != null;
					if (flag3)
					{
						hashSet.Add(p.story.hairDef.modContentPack.PackageId);
					}
					bool flag4 = p.HasStoryTracker();
					if (flag4)
					{
						BackstoryDef backstory = p.story.GetBackstory(BackstorySlot.Childhood);
						BackstoryDef backstory2 = p.story.GetBackstory(BackstorySlot.Adulthood);
						bool flag5 = backstory.nameMaker != null;
						if (flag5)
						{
							hashSet.Add(backstory.nameMaker.modContentPack.PackageId);
						}
						bool flag6 = backstory2.nameMaker != null;
						if (flag6)
						{
							hashSet.Add(backstory2.nameMaker.modContentPack.PackageId);
						}
					}
					bool flag7 = p.HasEquipmentTracker();
					if (flag7)
					{
						foreach (ThingWithComps thingWithComps in p.equipment.AllEquipmentListForReading)
						{
							hashSet.Add(thingWithComps.def.modContentPack.PackageId);
						}
					}
					bool flag8 = p.HasApparelTracker();
					if (flag8)
					{
						foreach (Apparel apparel in p.apparel.WornApparel)
						{
							hashSet.Add(apparel.def.modContentPack.PackageId);
						}
					}
					bool flag9 = p.HasInventoryTracker();
					if (flag9)
					{
						foreach (Thing thing in p.inventory.innerContainer)
						{
							hashSet.Add(thing.def.modContentPack.PackageId);
						}
					}
					bool flag10 = p.HasStyleTracker();
					if (flag10)
					{
						bool flag11 = p.style.beardDef != null;
						if (flag11)
						{
							hashSet.Add(p.style.beardDef.modContentPack.PackageId);
						}
						bool flag12 = p.style.BodyTattoo != null;
						if (flag12)
						{
							hashSet.Add(p.style.BodyTattoo.modContentPack.PackageId);
						}
						bool flag13 = p.style.FaceTattoo != null;
						if (flag13)
						{
							hashSet.Add(p.style.FaceTattoo.modContentPack.PackageId);
						}
					}
					bool flag14 = p.HasAbilityTracker();
					if (flag14)
					{
						foreach (Ability ability in p.abilities.abilities)
						{
							hashSet.Add(ability.def.modContentPack.PackageId);
						}
					}
					bool flag15 = p.HasTraitTracker();
					if (flag15)
					{
						foreach (Trait trait in p.story.traits.allTraits)
						{
							hashSet.Add(trait.def.modContentPack.PackageId);
						}
					}
					bool flag16 = p.HasHealthTracker();
					if (flag16)
					{
						foreach (Hediff hediff in p.health.hediffSet.hediffs)
						{
							hashSet.Add(hediff.def.modContentPack.PackageId);
						}
					}
					bool flag17 = p.HasMemoryTracker();
					if (flag17)
					{
						foreach (Thought_Memory thought_Memory in p.needs.mood.thoughts.memories.Memories)
						{
							hashSet.Add(thought_Memory.def.modContentPack.PackageId);
						}
					}
					bool isGradientHairActive = CEditor.IsGradientHairActive;
					if (isGradientHairActive)
					{
						hashSet.Add("automatic.gradienthair");
					}
					bool isDualWieldActive = CEditor.IsDualWieldActive;
					if (isDualWieldActive)
					{
						hashSet.Add("Roolo.DualWield");
					}
					bool isFacialAnimationActive = CEditor.IsFacialAnimationActive;
					if (isFacialAnimationActive)
					{
						hashSet.Add("Nals.FacialAnimation");
					}
					bool isAlienRaceActive = CEditor.IsAlienRaceActive;
					if (isAlienRaceActive)
					{
						hashSet.Add("erdelf.HumanoidAlienRaces");
					}
					bool isCombatExtendedActive = CEditor.IsCombatExtendedActive;
					if (isCombatExtendedActive)
					{
						hashSet.Add("CETeam.CombatExtended");
					}
					bool flag18 = !p.GetRoyalTitleAsSeparatedString().NullOrEmpty();
					if (flag18)
					{
						hashSet.Add(RoyalTitleDefOf.Knight.modContentPack.PackageId);
					}
				}
				catch
				{
				}
			}
			return "[" + hashSet.ToList<string>().ListToString<string>() + "]";
		}

		
		internal static bool IsZombie(this Pawn p)
		{
			return p.Faction.IsZombie();
		}

		
		internal static void ReplacePawnWithPawnOfSameRace(Gender gender, Faction faction)
		{
			PawnKindDef kindDef = CEditor.API.Pawn.kindDef;
			ThingDef def = CEditor.API.Pawn.def;
			IntVec3 position = CEditor.API.Pawn.Position;
			int num = 10;
			BodyTypeDef bodyTypeDef = (gender == Gender.Female) ? BodyTypeDefOf.Female : BodyTypeDefOf.Male;
			do
			{
				num--;
				CEditor.API.Pawn.Delete(false);
				PawnxTool.AddOrCreateNewPawn(kindDef, faction, def, null, position, false, Gender.None);
				bool flag = gender == Gender.Female && CEditor.API.Pawn.story.bodyType == BodyTypeDefOf.Male;
				if (!flag)
				{
					bool flag2 = gender == Gender.Male && CEditor.API.Pawn.story.bodyType == BodyTypeDefOf.Female;
					if (!flag2)
					{
						bool flag3 = gender == Gender.None;
						if (flag3)
						{
							break;
						}
						CEditor.API.Pawn.gender = gender;
					}
				}
			}
			while (CEditor.API.Pawn.gender != gender && num > 0);
			bool flag4 = CEditor.API.Pawn.HasStyleTracker();
			if (flag4)
			{
				bool flag5 = gender == Gender.Female;
				if (flag5)
				{
					CEditor.API.Pawn.SetBeard(BeardDefOf.NoBeard);
				}
			}
		}

		
		internal static void ForceGenderizedBodyTypeDef(Gender gender)
		{
			CEditor.API.Pawn.gender = gender;
			bool flag = gender == Gender.Female && CEditor.API.Pawn.story.bodyType == BodyTypeDefOf.Male && PawnxTool.IsBodyInList(BodyTypeDefOf.Female);
			if (flag)
			{
				CEditor.API.Pawn.SetBody(BodyTypeDefOf.Female);
			}
			else
			{
				bool flag2 = gender == Gender.Male && CEditor.API.Pawn.story.bodyType == BodyTypeDefOf.Female && PawnxTool.IsBodyInList(BodyTypeDefOf.Male);
				if (flag2)
				{
					CEditor.API.Pawn.SetBody(BodyTypeDefOf.Male);
				}
			}
		}

		
		internal static bool IsBodyInList(BodyTypeDef b)
		{
			bool flag = CEditor.API.Pawn == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				List<BodyTypeDef> bodyDefList = CEditor.API.Pawn.GetBodyDefList(false);
				bool flag2 = !bodyDefList.NullOrEmpty<BodyTypeDef>();
				result = (flag2 && bodyDefList.Contains(b));
			}
			return result;
		}

		
		internal static Pawn CreateNewPawn(PawnKindDef pkd, Faction f, ThingDef raceDef, bool forceFaction = false)
		{
			bool flag = pkd == null;
			Pawn result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Pawn pawn = null;
				bool flag2 = !forceFaction;
				if (flag2)
				{
					f = f.ThisOrDefault();
					bool flag3 = pkd.IsDroidColonist();
					bool flag4 = f.IsAbomination();
					bool flag5 = flag3 || (flag4 && pkd.IsDorfbewohner());
					bool flag6 = flag5;
					if (flag6)
					{
						f = Faction.OfPlayer;
					}
					bool flag7 = CEditor.ListName == Label.WILDANIMALS;
					if (flag7)
					{
						f = null;
					}
				}
				bool flag8 = f.CanCreateRelations(pkd);
				PawnGenerationContext pawnGenerationContext = (f == Faction.OfPlayer) ? PawnGenerationContext.PlayerStarter : PawnGenerationContext.NonPlayer;
				FloatRange value = new FloatRange(12.1f, 13f);
				Faction faction = f;
				PawnGenerationContext pawnGenerationContext2 = pawnGenerationContext;
				int num = -1;
				bool flag9 = true;
				bool flag10 = false;
				bool flag11 = false;
				bool flag12 = true;
				bool tutorialMode = TutorSystem.TutorialMode;
				float num2 = 20f;
				bool flag13 = false;
				bool flag14 = true;
				bool flag15 = true;
				bool flag16 = true;
				bool flag17 = true;
				bool flag18 = false;
				bool flag19 = false;
				bool flag20 = false;
				bool flag21 = false;
				float num3 = 0f;
				float num4 = 0f;
				Pawn pawn2 = null;
				float num5 = 1f;
				Predicate<Pawn> predicate = null;
				Predicate<Pawn> predicate2 = null;
				IEnumerable<TraitDef> enumerable = null;
				IEnumerable<TraitDef> enumerable2 = null;
				XenotypeDef xenotypeDef = ModsConfig.BiotechActive ? XenotypeDefOf.Baseliner : null;
				FloatRange? floatRange = ModsConfig.BiotechActive ? new FloatRange?(value) : null;
				PawnGenerationRequest request = new PawnGenerationRequest(pkd, faction, pawnGenerationContext2, num, flag9, flag10, flag11, flag12, tutorialMode, num2, flag13, flag14, flag15, flag16, flag17, flag18, flag19, flag20, flag21, num3, num4, pawn2, num5, predicate, predicate2, enumerable, enumerable2, null, null, null, null, null, null, null, null, false, false, false, false, null, null, xenotypeDef, null, null, 0f, DevelopmentalStage.Adult, null, floatRange, null, false);
				int num6 = 5;
				do
				{
					try
					{
						pawn = PawnGenerator.GeneratePawn(request);
					}
					catch
					{
						num6--;
						request = new PawnGenerationRequest(pkd, f, pawnGenerationContext, -1, true, false, false, false, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, false);
					}
				}
				while (pawn == null && num6 > 0);
				bool flag22 = pawn.relations != null;
				if (flag22)
				{
					pawn.relations.everSeenByPlayer = true;
				}
				PawnComponentsUtility.AddComponentsForSpawn(pawn);
				bool inStartingScreen = CEditor.InStartingScreen;
				if (inStartingScreen)
				{
					Type atype = Reflect.GetAType("Verse", "StartingPawnUtility");
					atype.CallMethod("GeneratePossessions", new object[]
					{
						pawn
					});
				}
				bool flag23 = !CEditor.API.GetO(OptionB.CREATERACESPECIFIC);
				if (flag23)
				{
					try
					{
						bool flag24 = pawn.HasStoryTracker();
						if (flag24)
						{
							pawn.Reequip(null, -1, true);
						}
						pawn.Redress(null, true, -1, true);
					}
					catch
					{
					}
				}
				else
				{
					try
					{
						string modName = pawn.kindDef.GetModName();
						HashSet<HairDef> hashSet = DefTool.ListByMod<HairDef>(modName);
						bool flag25 = !hashSet.EnumerableNullOrEmpty<HairDef>();
						if (flag25)
						{
							pawn.SetHair(hashSet.RandomElement<HairDef>());
						}
					}
					catch
					{
					}
				}
				bool o = CEditor.API.GetO(OptionB.CREATENUDE);
				if (o)
				{
					pawn.DestroyAllApparel();
				}
				bool o2 = CEditor.API.GetO(OptionB.CREATENOWEAPON);
				if (o2)
				{
					pawn.DestroyAllEquipment();
				}
				bool o3 = CEditor.API.GetO(OptionB.CREATENOINV);
				if (o3)
				{
					pawn.DestroyAllItems();
				}
				pawn.FixInvalidTracker();
				pawn.FixInvalidNames();
				pawn.ChangeFaction(f);
				result = pawn;
			}
			return result;
		}

		
		internal static Pawn GeneratePawn(PawnKindDef pkd, Faction f)
		{
			int num = 10;
			Pawn pawn = null;
			do
			{
				num--;
				try
				{
					pawn = PawnGenerator.GeneratePawn(pkd, f);
				}
				catch
				{
				}
			}
			while (pawn == null && num > 0);
			return pawn;
		}

		
		internal static Pawn GeneratePawn(PawnGenerationRequest pgr)
		{
			int num = 10;
			Pawn pawn = null;
			do
			{
				num--;
				try
				{
					pawn = PawnGenerator.GeneratePawn(pgr);
				}
				catch
				{
				}
			}
			while (pawn == null && num > 0);
			return pawn;
		}

		
		internal static PresetPawn ClonePawn(this Pawn p)
		{
			PresetPawn presetPawn = new PresetPawn();
			presetPawn.SavePawn(p, -1);
			presetPawn.GeneratePawn(true, true);
			return presetPawn;
		}

		
		internal static void AddOrCreateExistingPawn(PresetPawn ppn)
		{
			bool flag = ppn == null;
			if (!flag)
			{
				PawnxTool.AddOrCreateNewPawn(ppn.pawn.kindDef, ppn.pawn.Faction, ppn.pawn.def, ppn, default(IntVec3), false, Gender.None);
			}
		}

		
		internal static bool CheckIfSpaceZombie(Faction f)
		{
			bool flag = f.IsZombie();
			bool flag2 = flag && CEditor.InStartingScreen && !PawnxTool.CanCreateZombieInSpace;
			bool result;
			if (flag2)
			{
				MessageTool.Show("can't create zombies in space!", null);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		internal static Pawn AddOrCreateNewPawn(PawnKindDef pkd, Faction f, ThingDef raceDef, PresetPawn ppn, IntVec3 pos = default(IntVec3), bool doPlace = false, Gender gender = Gender.None)
		{
			bool flag = pkd == null;
			Pawn result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = PawnxTool.CheckIfSpaceZombie(f);
				if (flag2)
				{
					result = null;
				}
				else
				{
					Pawn pawn = (ppn == null) ? PawnxTool.CreateNewPawn(pkd, f, raceDef, false) : ppn.pawn;
					bool flag3 = gender > Gender.None;
					if (flag3)
					{
						pawn.gender = gender;
					}
					bool flag4 = pawn != null;
					if (flag4)
					{
						pawn.SpawnOrPassPawn(f, ppn, pos, doPlace);
					}
					result = pawn;
				}
			}
			return result;
		}

		
		internal static void FixInvalidNames(this Pawn p)
		{
			bool flag = p == null;
			if (!flag)
			{
				bool flag2 = p.Name != null && !p.Name.IsValid && p.Name.GetType() == typeof(NameTriple);
				if (flag2)
				{
					NameTriple nameTriple = p.Name as NameTriple;
					bool flag3 = nameTriple.Last.NullOrEmpty();
					if (flag3)
					{
						p.Name = new NameTriple(nameTriple.First, nameTriple.Nick, CEditor.zufallswert.Next(0, 99).ToString("2"));
					}
				}
			}
		}

		
		internal static void ZombieWorldCreationFixInSpace(this Pawn p)
		{
		}

		
		internal static bool CanCreateZombieInSpace;
	}
}
