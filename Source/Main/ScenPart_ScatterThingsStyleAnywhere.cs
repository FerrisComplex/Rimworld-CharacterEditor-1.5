using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace CharacterEditor
{
	
	public class ScenPart_ScatterThingsStyleAnywhere : ScenPart_ScatterThings
	{
		
		
		protected override bool NearPlayerStart
		{
			get
			{
				return false;
			}
		}

		
		public override string Summary(Scenario scen)
		{
			return ScenSummaryList.SummaryWithList(scen, "MapScatteredWith", "ScenPart_MapScatteredWith".Translate());
		}

		
		public override IEnumerable<string> GetSummaryListEntries(string tag)
		{
			bool flag = tag == "MapScatteredWith";
			if (flag)
			{
				yield return GenLabel.ThingLabel(this.thingDef, this.stuff, this.count).CapitalizeFirst();
			}
			yield break;
		}

		
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look<ThingStyleDef>(ref this.styleDef, "styleDef");
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
					clusterSize = ((this.thingDef.category == Verse.ThingCategory.Building) ? 1 : 4),
					allowRoofed = this.allowRoofed
				}.Generate(map, default(GenStepParams));
			}
		}

		
		public ScenPart_ScatterThingsStyleAnywhere()
		{
		}

		
		public IntVec3 location;

		
		public ThingStyleDef styleDef;

		
		public const string MapScatteredWithTag = "MapScatteredWith";
            
	}
}
