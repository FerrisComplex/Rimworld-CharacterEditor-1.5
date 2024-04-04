// Decompiled with JetBrains decompiler
// Type: CharacterEditor.CharacterEditorGrave
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace CharacterEditor;

public class CharacterEditorGrave : Building_CryptosleepCasket
{
    private Graphic cachedGraphicFull;
    private Pawn innerPawn;

    public CharacterEditorGrave()
    {
        innerPawn = null;
        cachedGraphicFull = null;
    }

    public override Graphic Graphic
    {
        get
        {
            if (innerPawn == null && !((Building_Casket)this).HasAnyContents)
                return ((Thing)this).Graphic;
            cachedGraphicFull = def.building.fullGraveGraphicData.GraphicColoredFor(this);
            if (def.building.fullGraveGraphicData == null)
                return ((Thing)this).Graphic;
            if (cachedGraphicFull == null)
                cachedGraphicFull = def.building.fullGraveGraphicData.GraphicColoredFor(this);
            return cachedGraphicFull;
        }
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
    {
        bool flag = this.innerContainer.Count == 0;
        if (flag)
        {
            bool flag2 = !myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly, false, false, TraverseMode.ByPawn);
            if (flag2)
            {
                FloatMenuOption failer = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
                yield return failer;
                failer = null;
            }
            else
            {
                string jobStr = CharacterEditor.Label.ENTER_ZOMBGRELLA;
                Action jobAction = delegate()
                {
                    JobDef jobDef = DefTool.JobDef("EnterZGrave");
                    bool flag3 = jobDef != null;
                    if (flag3)
                    {
                        Job job = new Job(jobDef, this);
                        myPawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
                    }
                };
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(jobStr, jobAction, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0), myPawn, this, "ReservedBy", null);
                jobStr = null;
                jobAction = null;
            }
        }
        yield break;
    }

    public override void EjectContents()
    {
        ThingDef filth_Slime = ThingDefOf.Filth_Slime;
        foreach (Thing thing in ((IEnumerable<Thing>)this.innerContainer))
        {
            Pawn pawn = thing as Pawn;
            bool flag = pawn != null;
            if (flag)
            {
                PawnComponentsUtility.AddComponentsForSpawn(pawn);
                pawn.filth.GainFilth(filth_Slime);
                this.innerPawn = null;
            }
        }
        bool flag2 = !base.Destroyed;
        if (flag2)
        {
            SoundDef soundDef = DefTool.SoundDef("Bridge_CollapseWater");
            bool flag3 = soundDef != null;
            if (flag3)
            {
                soundDef.PlayOneShot(SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), MaintenanceType.None));
            }
        }
        this.innerContainer.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near, null, null, true);
        this.contentsKnown = true;
    }

    public bool TryToEnter(Thing thing)
    {
        bool flag = !this.Accepts(thing);
        bool result;
        if (flag)
        {
            result = false;
        }
        else
        {
            bool flag2 = thing.holdingOwner != null;
            bool flag3;
            if (flag2)
            {
                thing.holdingOwner.TryTransferToContainer(thing, this.innerContainer, thing.stackCount, true);
                flag3 = true;
            }
            else
            {
                flag3 = this.innerContainer.TryAdd(thing, true);
            }
            bool flag4 = flag3;
            if (flag4)
            {
                bool flag5 = thing.Faction != null && thing.Faction.IsPlayer;
                if (flag5)
                {
                    this.contentsKnown = true;
                }
                result = true;
            }
            else
            {
                result = false;
            }
        }
        return result;
    }

    public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
    {
        bool flag = this.TryToEnter(thing);
        bool result;
        if (flag)
        {
            SoundDefOf.Corpse_Drop.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            this.cachedGraphicFull = this.def.building.fullGraveGraphicData.GraphicColoredFor(this);
            this.innerPawn = (thing as Pawn);
            CEditor.API.Get<Dictionary<int, Building_CryptosleepCasket>>(EType.UIContainers)[0] = this;
            CEditor.API.StartEditor(this.innerPawn);
            result = true;
        }
        else
        {
            result = false;
        }
        return result;
    }
}
