// Decompiled with JetBrains decompiler
// Type: CharacterEditor.TrackerTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace CharacterEditor;

internal static class TrackerTool
{
    internal static bool HasAbilityTracker(this Pawn p)
    {
        return p != null && p.abilities != null;
    }

    internal static bool HasPsyTracker(this Pawn p)
    {
        return p != null && p.psychicEntropy != null;
    }

    internal static bool HasAgeTracker(this Pawn p)
    {
        return p != null && p.ageTracker != null;
    }

    internal static bool HasApparelTracker(this Pawn pawn)
    {
        return pawn != null && pawn.apparel != null;
    }

    internal static bool HasForcedApparel(this Pawn pawn)
    {
        return pawn != null && pawn.outfits != null && pawn.outfits.forcedHandler != null && !pawn.outfits.forcedHandler.ForcedApparel.NullOrEmpty();
    }

    internal static bool HasHealthTracker(this Pawn p)
    {
        return p != null && p.health != null && p.health.hediffSet != null;
    }

    internal static bool HasGeneTracker(this Pawn p)
    {
        return p != null && p.genes != null;
    }

    internal static bool HasStoryTracker(this Pawn p)
    {
        return p != null && p.story != null;
    }

    internal static bool HasStyleTracker(this Pawn p)
    {
        return p != null && p.style != null;
    }

    internal static bool HasRoyalTitle(this Pawn p)
    {
        return p.HasRoyaltyTracker() && !p.royalty.AllTitlesForReading.NullOrEmpty();
    }

    internal static bool HasRoyaltyTracker(this Pawn p)
    {
        return p != null && p.royalty != null;
    }

    internal static bool HasRecordsTracker(this Pawn p)
    {
        return p != null && p.records != null;
    }

    internal static bool HasRelationTracker(this Pawn p)
    {
        return p != null && p.relations != null;
    }

    internal static bool HasCarryTracker(this Pawn p)
    {
        return p != null && p.carryTracker != null;
    }

    internal static bool HasSkillTracker(this Pawn p)
    {
        return p != null && p.skills != null;
    }

    internal static bool HasInventoryTracker(this Pawn pawn)
    {
        return pawn != null && pawn.inventory != null;
    }

    internal static bool HasMemoryTracker(this Pawn p)
    {
        return p.HasThoughtsTracker() && p.needs.mood.thoughts.memories != null;
    }

    internal static bool HasThoughtsTracker(this Pawn p)
    {
        return p != null && p.needs != null && p.needs.mood != null && p.needs.mood.thoughts != null;
    }

    internal static bool HasSituationalTracker(this Pawn p)
    {
        return p.HasThoughtsTracker() && p.needs.mood.thoughts.situational != null;
    }

    internal static bool HasNeedsTracker(this Pawn p)
    {
        return p != null && p.needs != null;
    }

    internal static bool HasTraitTracker(this Pawn p)
    {
        return p != null && p.story != null && p.story.traits != null;
    }

    internal static bool HasEquipmentTracker(this Pawn pawn)
    {
        return pawn != null && pawn.equipment != null;
    }

    internal static bool HasWorkTracker(this Pawn p)
    {
        return p != null && p.workSettings != null;
    }

    internal static bool HasTimeTracker(this Pawn p)
    {
        return p != null && p.timetable != null;
    }

    internal static void FixInvalidTracker(this Pawn newPawn)
    {
        try
        {
            if ((newPawn == null || newPawn.Faction.IsCreature() || newPawn.kindDef.IsMechanoid() || newPawn.kindDef.IsZombie() || newPawn.kindDef.IsAbomination() ? 1 : newPawn.kindDef.RaceProps.Animal ? 1 : 0) != 0)
                return;
            var flag = false;
            if (newPawn.outfits == null)
            {
                newPawn.outfits = new Pawn_OutfitTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed outfits for " + newPawn.kindDef.defName);
            }

            if (newPawn.outfits != null && (newPawn.outfits.CurrentApparelPolicy == null ? 0 : newPawn.outfits.CurrentApparelPolicy.filter != null ? 1 : 0) != 0)
                newPawn.outfits.CurrentApparelPolicy.filter.SetAllowAll(null);
            if (newPawn.foodRestriction == null)
            {
                newPawn.foodRestriction = new Pawn_FoodRestrictionTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed food restriction for " + newPawn.kindDef.defName);
            }

            if (newPawn.drugs == null)
            {
                newPawn.drugs = new Pawn_DrugPolicyTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed drugs for " + newPawn.kindDef.defName);
            }

            if (newPawn.drafter == null)
            {
                newPawn.drafter = new Pawn_DraftController(newPawn);
                if (flag)
                    MessageTool.Show("fixed drafter for " + newPawn.kindDef.defName);
            }

            if (newPawn.playerSettings == null)
            {
                newPawn.playerSettings = new Pawn_PlayerSettings(newPawn);
                if (flag)
                    MessageTool.Show("fixed player settings for " + newPawn.kindDef.defName);
            }

            if (newPawn.timetable == null)
            {
                newPawn.timetable = new Pawn_TimetableTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed timetable for " + newPawn.kindDef.defName);
            }

            if (newPawn.needs == null)
            {
                newPawn.needs = new Pawn_NeedsTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed needs for " + newPawn.kindDef.defName);
            }

            if (newPawn.mindState == null)
            {
                newPawn.mindState = new Pawn_MindState(newPawn);
                if (flag)
                    MessageTool.Show("fixed mindstate for " + newPawn.kindDef.defName);
                if (newPawn.mindState.mentalStateHandler == null)
                {
                    newPawn.mindState.mentalStateHandler = new MentalStateHandler(newPawn);
                    if (flag)
                        MessageTool.Show("fixed mentatlstatehandler for " + newPawn.kindDef.defName);
                }

                if (newPawn.mindState.mentalBreaker == null)
                {
                    newPawn.mindState.mentalBreaker = new MentalBreaker(newPawn);
                    if (flag)
                        MessageTool.Show("fixed mentalbreaker for " + newPawn.kindDef.defName);
                }

                if (newPawn.mindState.inspirationHandler == null)
                {
                    newPawn.mindState.inspirationHandler = new InspirationHandler(newPawn);
                    if (flag)
                        MessageTool.Show("fixed inspirationhandler for " + newPawn.kindDef.defName);
                }

                if (newPawn.mindState.priorityWork == null)
                {
                    newPawn.mindState.priorityWork = new PriorityWork(newPawn);
                    if (flag)
                        MessageTool.Show("fixed prioritywork for " + newPawn.kindDef.defName);
                }
            }

            if (newPawn.jobs == null)
            {
                newPawn.jobs = new Pawn_JobTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed jobs for " + newPawn.kindDef.defName);
            }

            if (newPawn.inventory == null)
            {
                newPawn.inventory = new Pawn_InventoryTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed inventory for " + newPawn.kindDef.defName);
            }

            if (newPawn.interactions == null)
            {
                newPawn.interactions = new Pawn_InteractionsTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed interactions for " + newPawn.kindDef.defName);
            }

            if (newPawn.health == null)
            {
                newPawn.health = new Pawn_HealthTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed health for " + newPawn.kindDef.defName);
            }

            if (newPawn.guest == null)
            {
                newPawn.guest = new Pawn_GuestTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed guest for " + newPawn.kindDef.defName);
            }

            if (newPawn.filth == null)
            {
                newPawn.filth = new Pawn_FilthTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed filth for " + newPawn.kindDef.defName);
            }

            if (newPawn.equipment == null)
            {
                newPawn.equipment = new Pawn_EquipmentTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed equipment for " + newPawn.kindDef.defName);
            }

            if (newPawn.carryTracker == null)
            {
                newPawn.carryTracker = new Pawn_CarryTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed carry tracker for " + newPawn.kindDef.defName);
            }

            if (newPawn.apparel == null)
            {
                newPawn.apparel = new Pawn_ApparelTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed apparel for " + newPawn.kindDef.defName);
            }

            if (newPawn.abilities == null)
            {
                newPawn.abilities = new Pawn_AbilityTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed abilities for " + newPawn.kindDef.defName);
            }

            if (newPawn.pather == null)
            {
                newPawn.pather = new Pawn_PathFollower(newPawn);
                if (flag)
                    MessageTool.Show("fixed path follower for " + newPawn.kindDef.defName);
            }

            if (newPawn.psychicEntropy == null)
            {
                newPawn.psychicEntropy = new Pawn_PsychicEntropyTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed psychic entropy for " + newPawn.kindDef.defName);
            }

            if (newPawn.records == null)
            {
                newPawn.records = new Pawn_RecordsTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed records for " + newPawn.kindDef.defName);
            }

            if (newPawn.relations == null)
            {
                newPawn.relations = new Pawn_RelationsTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed releations for " + newPawn.kindDef.defName);
            }

            if (newPawn.rotationTracker == null)
            {
                newPawn.rotationTracker = new Pawn_RotationTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed rotation tracker for " + newPawn.kindDef.defName);
            }

            if (newPawn.royalty == null)
            {
                newPawn.royalty = new Pawn_RoyaltyTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed royalty for " + newPawn.kindDef.defName);
            }

            if (newPawn.stances == null)
            {
                newPawn.stances = new Pawn_StanceTracker(newPawn);
                if (flag)
                    MessageTool.Show("fixed stances for " + newPawn.kindDef.defName);
            }

            if (newPawn.thinker == null)
            {
                newPawn.thinker = new Pawn_Thinker(newPawn);
                if (flag)
                    MessageTool.Show("fixed thinker for " + newPawn.kindDef.defName);
            }

            if (newPawn.workSettings == null)
            {
                newPawn.workSettings = new Pawn_WorkSettings(newPawn);
                if (flag)
                    MessageTool.Show("fixed work setting for " + newPawn.kindDef.defName);
            }

            if (newPawn.Faction != Faction.OfPlayer)
                return;
            newPawn.workSettings.EnableAndInitialize();
        }
        catch (Exception ex)
        {
            Log.Message(ex.Message + "\n" + ex.StackTrace);
        }
    }
}
