// Decompiled with JetBrains decompiler
// Type: CharacterEditor.ScenPart_StartingThingStyle_Defined
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CharacterEditor;

public class ScenPart_StartingThingStyle_Defined : ScenPart_ThingCount
{
    public const string PlayerStartWithTag = "PlayerStartsWith";
    public IntVec3 location;
    public ThingStyleDef styleDef;

    public static string PlayerStartWithIntro => new TaggedString("ScenPart_StartWith".Translate());

    public override string Summary(Scenario scen)
    {
        return ScenSummaryList.SummaryWithList(scen, "PlayerStartsWith", PlayerStartWithIntro);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look<ThingStyleDef>(ref this.styleDef, "styleDef");
    }
    
    public override IEnumerable<string> GetSummaryListEntries(string tag)
    {
        if (tag == "PlayerStartsWith")
            yield return GenLabel.ThingLabel(thingDef, stuff, count).CapitalizeFirst();
    }

    public override IEnumerable<Thing> PlayerStartingThings()
    {
        var thing = ThingMaker.MakeThing(thingDef, stuff);
        if (quality.HasValue)
            thing.TryGetComp<CompQuality>()?.SetQuality(quality.Value, 0);
        if (thingDef.Minifiable)
            thing = thing.MakeMinified();
        if (styleDef != null)
            thing.StyleDef = styleDef;
        if (thingDef.ingestible != null && thingDef.IsIngestible && thingDef.ingestible.IsMeal)
            FoodUtility.GenerateGoodIngredients(thing, Faction.OfPlayer.ideos.PrimaryIdeo);
        thing.stackCount = count;
        yield return thing;
    }
}
