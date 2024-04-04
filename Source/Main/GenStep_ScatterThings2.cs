// Decompiled with JetBrains decompiler
// Type: CharacterEditor.GenStep_ScatterThings2
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CharacterEditor;

public class GenStep_ScatterThings2 : GenStep_ScatterThings
{
    private static readonly List<Rot4> tmpRotations2 = new();
    private List<Rot4> possibleRotationsInt2;
    public ThingStyleDef styleDef;

    private List<Rot4> PossibleRotations2
    {
        get
        {
            if (possibleRotationsInt2 == null)
            {
                possibleRotationsInt2 = new List<Rot4>();
                if (thingDef.rotatable)
                {
                    possibleRotationsInt2.Add(Rot4.North);
                    possibleRotationsInt2.Add(Rot4.East);
                    possibleRotationsInt2.Add(Rot4.South);
                    possibleRotationsInt2.Add(Rot4.West);
                }
                else
                {
                    possibleRotationsInt2.Add(Rot4.North);
                }
            }

            return possibleRotationsInt2;
        }
    }

    public bool TryGetRandomValidRotation2(IntVec3 loc, Map map, out Rot4 rot)
    {
        List<Rot4> possibleRotations = this.PossibleRotations2;
        for (int i = 0; i < possibleRotations.Count; i++)
        {
            bool flag = this.IsRotationValid2(loc, possibleRotations[i], map);
            if (flag)
            {
                GenStep_ScatterThings2.tmpRotations2.Add(possibleRotations[i]);
            }
        }
        bool flag2 = GenStep_ScatterThings2.tmpRotations2.TryRandomElement(out rot);
        bool result;
        if (flag2)
        {
            GenStep_ScatterThings2.tmpRotations2.Clear();
            result = true;
        }
        else
        {
            rot = Rot4.Invalid;
            result = false;
        }
        return result;
    }

    private bool IsRotationValid2(IntVec3 loc, Rot4 rot, Map map)
    {
        bool flag = !GenAdj.OccupiedRect(loc, rot, this.thingDef.size).InBounds(map);
        bool result;
        if (flag)
        {
            result = false;
        }
        else
        {
            bool flag2 = GenSpawn.WouldWipeAnythingWith(loc, rot, this.thingDef, map, (Thing x) => x.def == this.thingDef || (x.def.category != Verse.ThingCategory.Plant && x.def.category != Verse.ThingCategory.Filth));
            result = !flag2;
        }
        return result;
    }

    protected override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int stackCount = 1)
    {
        Rot4 rot;
        bool flag = !this.TryGetRandomValidRotation2(loc, map, out rot);
        if (flag)
        {
            string str = "Could not find any valid rotation for ";
            ThingDef thingDef = this.thingDef;
            Log.Warning(str + ((thingDef != null) ? thingDef.ToString() : null));
        }
        else
        {
            bool flag2 = this.clearSpaceSize > 0;
            if (flag2)
            {
                foreach (IntVec3 c in GridShapeMaker.IrregularLump(loc, map, this.clearSpaceSize))
                {
                    Building edifice = c.GetEdifice(map);
                    if (edifice != null)
                    {
                        edifice.Destroy(DestroyMode.Vanish);
                    }
                }
            }
            Thing thing = ThingMaker.MakeThing(this.thingDef, this.stuff);
            thing.StyleDef = this.styleDef;
            bool minifiable = this.thingDef.Minifiable;
            if (minifiable)
            {
                thing = thing.MakeMinified();
            }
            bool flag3 = thing.def.category == Verse.ThingCategory.Item;
            if (flag3)
            {
                thing.stackCount = stackCount;
                thing.SetForbidden(true, false);
                Thing thing2;
                GenPlace.TryPlaceThing(thing, loc, map, ThingPlaceMode.Near, out thing2, null, null, default(Rot4));
                bool flag4 = this.nearPlayerStart && thing2 != null && thing2.def.category == Verse.ThingCategory.Item && TutorSystem.TutorialMode;
                if (flag4)
                {
                    Find.TutorialState.AddStartingItem(thing2);
                }
            }
            else
            {
                GenSpawn.Spawn(thing, loc, map, rot, WipeMode.Vanish, false);
            }
            bool flag5 = this.filthDef == null;
            if (!flag5)
            {
                foreach (IntVec3 c2 in thing.OccupiedRect().ExpandedBy(this.filthExpandBy))
                {
                    bool flag6 = Rand.Chance(this.filthChance) && c2.InBounds(thing.Map);
                    if (flag6)
                    {
                        FilthMaker.TryMakeFilth(c2, thing.Map, this.filthDef, 1, FilthSourceFlags.None, true);
                    }
                }
            }
        }
    }
}
