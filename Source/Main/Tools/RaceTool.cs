using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace CharacterEditor
{
	
	public static class RaceTool
	{
		
		public static bool IsBodySize(this LifeStageAge lsa)
		{
			return lsa != null && lsa.def != null && !lsa.def.defName.NullOrEmpty() && lsa.def.defName.StartsWith("SZHumanSize_");
		}

		
		public static bool IsBodySizeGene(this Gene g)
		{
			return g != null && g.def.IsBodySizeGene();
		}

		
		public static LifeStageDef GetLifeStageDef(this Gene g)
		{
			if (g == null)
			{
				return null;
			}
			return g.def.GetLifeStageDef();
		}

		
		public static bool CanHaveChangedBodySize(this Pawn p)
		{
			return p != null && p.HasAgeTracker() && p.HasGeneTracker() && p.HasAdultAge() && p.RaceProps.Humanlike && p.DevelopmentalStage == DevelopmentalStage.Adult;
		}

		
		public static Gene FirstBodySizeGene(this Pawn p)
		{
			return p.genes.GenesListForReading.FirstOrFallback((Gene x) => x.IsBodySizeGene(), null);
		}

		
		public static bool HasBodySizeGenes(this Pawn p)
		{
			return p.FirstBodySizeGene() != null;
		}

		
		public static bool HasChildOrBabyBody(this Pawn p)
		{
			return p.HasStoryTracker() && (p.story.bodyType == BodyTypeDefOf.Baby || p.story.bodyType == BodyTypeDefOf.Child);
		}

		
		public static bool HasAdultAge(this Pawn p)
		{
			return p.ageTracker.AgeBiologicalYears - p.GetMinAgeForAdult() >= 0;
		}

		
		public static int GetMinAgeForAdult(this Pawn p)
		{
			LifeStageAge lifeStageAge = p.RaceProps.lifeStageAges.FirstOrDefault((LifeStageAge l) => l.def.defName != null && !l.def.defName.Contains("Teenager") && l.def.developmentalStage.Adult());
			if (lifeStageAge != null)
			{
				return (int)lifeStageAge.minAge;
			}
			return 0;
		}

		
		public static void CheckAllPawnsOnStartupAndReapplyBodySize()
		{
			Map currentMap = Find.CurrentMap;
			List<Pawn> list;
			if (currentMap == null)
			{
				list = null;
			}
			else
			{
				MapPawns mapPawns = currentMap.mapPawns;
				list = ((mapPawns != null) ? mapPawns.AllPawns : null);
			}
			List<Pawn> list2 = list;
			if (!list2.NullOrEmpty<Pawn>())
			{
				foreach (Pawn p in list2)
				{
					p.TryApplyOrKeepBodySize(null);
				}
			}
		}

		
		public static void RemoveOldBodySizeRedundants(Gene gene)
		{
			if (gene.IsBodySizeGene())
			{
				Pawn pawn = gene.pawn;
				for (int i = pawn.genes.Xenogenes.Count - 1; i >= 0; i--)
				{
					if (pawn.genes.Xenogenes[i] != gene && pawn.genes.Xenogenes[i].IsBodySizeGene())
					{
						pawn.genes.Xenogenes.Remove(pawn.genes.Xenogenes[i]);
					}
				}
				for (int j = pawn.genes.Endogenes.Count - 1; j >= 0; j--)
				{
					if (pawn.genes.Endogenes[j] != gene && pawn.genes.Endogenes[j].IsBodySizeGene())
					{
						pawn.genes.Endogenes.Remove(pawn.genes.Endogenes[j]);
					}
				}
			}
		}

		
		public static void TryToFallbackLifeAge(Pawn p)
		{
			if (p != null && p.HasAgeTracker() && p.ageTracker.CurLifeStageRace.IsBodySize())
			{
				RaceTool.bRecalcTriggered = true;
				p.ageTracker.DebugSetGrowth(0.9f);
				p.ageTracker.CallMethod("RecalculateLifeStageIndex", null);
			}
		}

		
		public static void RememberBackstory(this Pawn p)
		{
			if (p != null && p.HasStoryTracker())
			{
				RaceTool.flippingBackstory = new KeyValuePair<string, BackstoryDef>(p.ThingID, p.story.Adulthood);
			}
		}

		
		public static void TryApplyOrKeepBodySize(this Pawn p, Gene gene = null)
		{
			if (p != null)
			{
				if (RaceTool.bRecalcTriggered)
				{
					RaceTool.bRecalcTriggered = !RaceTool.bRecalcTriggered;
					return;
				}
				if (!p.CanHaveChangedBodySize())
				{
					RaceTool.TryToFallbackLifeAge(p);
					return;
				}
				Gene gene2 = (gene == null) ? p.FirstBodySizeGene() : gene;
				if (gene2 == null)
				{
					RaceTool.TryToFallbackLifeAge(p);
					return;
				}
				gene2.pawn.KeepOrSet_LifeStage(gene2.GetLifeStageDef());
			}
		}

		
		private static void KeepOrSet_LifeStage(this Pawn p, LifeStageDef ld)
		{
			if (p != null && ld != null && p.DevelopmentalStage == DevelopmentalStage.Adult && !p.HasChildOrBabyBody())
			{
				LifeStageAge lifeStageAge = p.RaceProps.GetLifeStageAgeByDef(ld);
				if (lifeStageAge == null)
				{
					lifeStageAge = p.TryCreateLifeStageAge(ld);
				}
				if (lifeStageAge != null)
				{
					int recentLifeStageAgeIndex_ForLSDef = p.RaceProps.GetRecentLifeStageAgeIndex_ForLSDef(ld);
					if (recentLifeStageAgeIndex_ForLSDef >= 0)
					{
						if (p.ThingID == RaceTool.flippingBackstory.Key)
						{
							p.story.Adulthood = RaceTool.flippingBackstory.Value;
						}
						p.ageTracker.SetMemberValue("growth", 1f);
						p.ageTracker.SetMemberValue("cachedLifeStageIndex", recentLifeStageAgeIndex_ForLSDef);
						CEditor.API.UpdateGraphics();
					}
				}
			}
		}

		
		public static int GetRecentLifeStageAgeIndex_ForLSDef(this RaceProperties p, LifeStageDef ld)
		{
			int result;
			if (p.lifeStageAges.NullOrEmpty<LifeStageAge>())
			{
				result = -1;
			}
			else
			{
				for (int i = p.lifeStageAges.Count - 1; i >= 0; i--)
				{
					if (p.lifeStageAges[i].def == ld)
					{
						return i;
					}
				}
				result = -1;
			}
			return result;
		}

		
		private static LifeStageAge TryCreateLifeStageAge(this Pawn p, LifeStageDef ld)
		{
			LifeStageAge result;
			if (ld == null || p == null || p.RaceProps == null)
			{
				result = null;
			}
			else
			{
				LifeStageAge lastDefaultLifeStageAge = p.RaceProps.GetLastDefaultLifeStageAge();
				if (lastDefaultLifeStageAge == null)
				{
					result = null;
				}
				else
				{
					LifeStageAge lifeStageAge = new LifeStageAge();
					lifeStageAge.def = ld;
					lifeStageAge.minAge = float.MaxValue;
					lifeStageAge.soundCall = lastDefaultLifeStageAge.soundCall;
					lifeStageAge.soundAmbience = lastDefaultLifeStageAge.soundAmbience;
					lifeStageAge.soundAngry = lastDefaultLifeStageAge.soundAngry;
					lifeStageAge.soundDeath = lastDefaultLifeStageAge.soundDeath;
					lifeStageAge.soundWounded = lastDefaultLifeStageAge.soundWounded;
					p.RaceProps.lifeStageAges.Add(lifeStageAge);
					p.RaceProps.ResolveReferencesSpecial();
					lifeStageAge.def.ResolveReferences();
					lifeStageAge.def.PostLoad();
					p.def.ResolveReferences();
					result = lifeStageAge;
				}
			}
			return result;
		}

		
		public static LifeStageAge GetLifeStageAgeByDef(this RaceProperties p, LifeStageDef def)
		{
			LifeStageAge result;
			if (p.lifeStageAges.NullOrEmpty<LifeStageAge>())
			{
				result = null;
			}
			else
			{
				for (int i = p.lifeStageAges.Count - 1; i >= 0; i--)
				{
					if (p.lifeStageAges[i].def == def)
					{
						return p.lifeStageAges[i];
					}
				}
				result = null;
			}
			return result;
		}

		
		public static LifeStageAge GetLastDefaultLifeStageAge(this RaceProperties p)
		{
			LifeStageAge result;
			if (p.lifeStageAges.NullOrEmpty<LifeStageAge>())
			{
				result = null;
			}
			else
			{
				for (int i = p.lifeStageAges.Count - 1; i >= 0; i--)
				{
					if (!p.lifeStageAges[i].IsBodySize())
					{
						return p.lifeStageAges[i];
					}
				}
				result = null;
			}
			return result;
		}

		
		public static void ChangeRace(Pawn pawn, PawnKindDef pkd, bool keepRaceSpecificClothes)
		{
			PresetPawn presetPawn = new PresetPawn();
			presetPawn.SavePawn(pawn, -1);
			presetPawn.dicParams[PresetPawn.Param.P01_pawnkinddef] = pkd.defName;
			MessageTool.Show("ppn kind=" + pkd.defName, null);
			presetPawn.GeneratePawn(true, !keepRaceSpecificClothes);
			Pawn pawn2 = presetPawn.pawn;
			MessageTool.Show("pawnNeu kind=" + pawn2.kindDef.defName, null);
			pawn2.ChangeKind(pkd);
			IntVec3 position = pawn.Position;
			bool isPrisoner = pawn.IsPrisoner;
			pawn.Delete(true);
			if (!CEditor.InStartingScreen)
			{
				pawn2.TeleportPawn(position);
			}
			if (pawn2.story != null && pawn2.kindDef != null && pawn2.story.headType != null)
			{
				HashSet<HeadTypeDef> headDefList = pawn2.GetHeadDefList(true);
				pawn2.SetHeadTypeDef(headDefList.RandomElement<HeadTypeDef>());
			}
			if (pawn2.story != null && pawn2.kindDef != null && pawn2.story.bodyType != null)
			{
				List<BodyTypeDef> bodyDefList = pawn2.GetBodyDefList(true);
				pawn2.SetBody(bodyDefList.RandomElement<BodyTypeDef>());
			}
			if (isPrisoner)
			{
				if (pawn2.Faction == Faction.OfPlayer)
				{
					pawn2.SetFaction(null, null);
				}
				if (pawn2.guest.Released)
				{
					pawn2.guest.Released = false;
					pawn2.guest.SetNoInteraction();
				}
				if (!pawn2.IsPrisonerOfColony)
				{
					pawn2.guest.CapturedBy(Faction.OfPlayer, null);
				}
			}
			PawnxTool.PostProcess(pawn2, presetPawn);
		}

		
		static RaceTool()
		{
		}

		
		public static bool bRecalcTriggered;

		
		private static KeyValuePair<string, BackstoryDef> flippingBackstory;
	}
}
