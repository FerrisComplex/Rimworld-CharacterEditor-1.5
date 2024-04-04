using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace CharacterEditor
{
	
	internal class ScenPart_ScatterThingsNearPlayerExtra : ScenPart_ThingCount
	{
		
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look<ThingStyleDef>(ref this.styleDef, "styleDef");
			Scribe_Values.Look<bool>(ref this.allowRoofed, "allowRoofed", false, false);
		}

		
		
		protected bool NearPlayerStart
		{
			get
			{
				return true;
			}
		}

		
		public override string Summary(Scenario scen)
		{
			return ScenSummaryList.SummaryWithList(scen, "PlayerStartsWith", ScenPart_StartingThing_Defined.PlayerStartWithIntro);
		}

		
		public override IEnumerable<string> GetSummaryListEntries(string tag)
		{
			bool flag = tag == "PlayerStartsWith";
			if (flag)
			{
				yield return GenLabel.ThingLabel(this.thingDef, this.stuff, this.count).CapitalizeFirst();
			}
			yield break;
		}

		
		public override void GenerateIntoMap(Map map)
		{
			bool flag = Find.GameInitData != null;
			if (flag)
			{
				new GenStep_ScatterThings2
				{
					nearPlayerStart = this.NearPlayerStart,
					allowFoggedPositions = !this.NearPlayerStart,
					thingDef = this.thingDef,
					stuff = this.stuff,
					styleDef = this.styleDef,
					count = this.count,
					spotMustBeStandable = true,
					minSpacing = 5f,
					clusterSize = ((this.thingDef != null && this.thingDef.category == Verse.ThingCategory.Building) ? 1 : 4),
					allowRoofed = this.allowRoofed
				}.Generate(map, default(GenStepParams));
			}
		}

		
		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (this.allowRoofed ? 1 : 0);
		}

		
		public ScenPart_ScatterThingsNearPlayerExtra()
		{
		}

		
		public IntVec3 location;

		
		public ThingStyleDef styleDef;

		
		public bool allowRoofed = true;
	}
}
