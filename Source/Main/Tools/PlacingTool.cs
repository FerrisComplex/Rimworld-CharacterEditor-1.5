using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CharacterEditor
{
    
    internal static class PlacingTool
    {
        
        internal static void DropThingWithPod(Thing t)
        {
            DropPodUtility.DropThingGroupsNear(UI.MouseCell(), Find.CurrentMap, new List<List<Thing>>
            {
                new List<Thing>
                {
                    t
                }
            }, 110, false, false, true, true, true, false, null);
        }

        
        internal static void DropAllSelectedWithPod(List<Selected> l, IntVec3 loc = default(IntVec3))
        {
            DebugMenuOption debugMenuOption = new DebugMenuOption(Label.DROP + Label.ALL, DebugMenuOptionMode.Tool, delegate()
            {
                foreach (Selected s in l)
                {
                    PlacingTool.DoDrop(s, loc);
                }

                DebugTools.curTool = null;
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        internal static void DropSelectedWithPod(Selected s, IntVec3 loc = default(IntVec3))
        {
            DebugMenuOption debugMenuOption = new DebugMenuOption(Label.DROP + s.thingDef.SLabel(), DebugMenuOptionMode.Tool, delegate()
            {
                PlacingTool.DoDrop(s, loc);
                DebugTools.curTool = null;
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        private static void DoDrop(Selected s, IntVec3 loc)
        {
            List<Thing> item = s.Generate();
            List<List<Thing>> list = new List<List<Thing>>();
            list.Add(item);
            DropPodUtility.DropThingGroupsNear((loc == default(IntVec3)) ? UI.MouseCell() : loc, Find.CurrentMap, list, 60, false, false, true, true, true, false, null);
        }

        
        internal static void DropPawnWithPod(Pawn p, PresetPawn ppn)
        {
            DebugMenuOption debugMenuOption = new DebugMenuOption(Label.PLACE_PAWN + " droppod", DebugMenuOptionMode.Tool, delegate()
            {
                List<List<Thing>> thingsGroups = new List<List<Thing>>
                {
                    new List<Thing>
                    {
                        p
                    }
                };
                if (!p.IsColonist)
                {
                    p.CreatePawnLord(default(IntVec3));
                }

                DropPodUtility.DropThingGroupsNear(UI.MouseCell(), Find.CurrentMap, thingsGroups, 110, false, false, true, true, true, false, null);
                PawnxTool.PostProcess(p, ppn);
                PlacingTool.TryCloneOrAbort(p);
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        internal static void DoEffect(Pawn p, IntVec3 pos)
        {
            try
            {
                Ability ability = CEditor.API.GetO(OptionB.USECHAOSABILITY) ? AbilityUtility.MakeAbility(DefTool.AbilityDef("ChaosSkip"), p) : null;
                if (ability != null)
                {
                    float warmupTime = ability.VerbProperties.First<VerbProperties>().warmupTime;
                    FloatRange randomRange = new FloatRange(0f, 0f);
                    foreach (AbilityComp abilityComp in ability.comps)
                    {
                        if (abilityComp.props.GetType() == typeof(CompProperties_AbilityTeleport))
                        {
                            randomRange = (abilityComp.props as CompProperties_AbilityTeleport).randomRange;
                            (abilityComp.props as CompProperties_AbilityTeleport).randomRange = new FloatRange(0f, 0f);
                        }
                    }

                    ability.VerbProperties.First<VerbProperties>().warmupTime = 0f;
                    ability.VerbTracker.PrimaryVerb.DrawHighlight(UI.MouseCell());
                    ability.VerbTracker.PrimaryVerb.TryStartCastOn(p, false, true, false, false);
                    ability.VerbTracker.PrimaryVerb.DrawHighlight(UI.MouseCell());
                    DefTool.SoundDef("PsychicSoothePulserCast").PlayOneShot(new TargetInfo(UI.MouseCell(), Find.CurrentMap, false));
                    p.jobs.ClearQueuedJobs(true);
                    p.jobs.SetMemberValue("jobsGivenThisTick", 0);
                    ability.VerbProperties.First<VerbProperties>().warmupTime = warmupTime;
                    foreach (AbilityComp abilityComp2 in ability.comps)
                    {
                        if (abilityComp2.props.GetType() == typeof(CompProperties_AbilityTeleport))
                        {
                            (abilityComp2.props as CompProperties_AbilityTeleport).randomRange = randomRange;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        
        internal static void PlaceInCustomPosition(Pawn p, PresetPawn ppn)
        {
            string label = Event.current.capsLock ? Label.CLONE_PAWN : Label.PLACE_PAWN;
            DebugMenuOption debugMenuOption = new DebugMenuOption(label, DebugMenuOptionMode.Tool, delegate()
            {
                p.SpawnPawn(ppn, UI.MouseCell());
                PlacingTool.DoEffect(p, UI.MouseCell());
                PlacingTool.TryCloneOrAbort(p);
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        internal static void PlaceMultiplePawnsInCustomPosition(Selected s, Faction f)
        {
            CEditor.API.EditorMoveRight();
            string label = Event.current.capsLock ? Label.CLONE_PAWN : Label.PLACE_PAWN;
            DebugMenuOption debugMenuOption = new DebugMenuOption(label, DebugMenuOptionMode.Tool, delegate() { PlacingTool.DoPlaceMultiplePawns(s, f); });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        internal static void DoPlaceMultiplePawns(Selected s, Faction f)
        {
            PlacingTool.lastKnownPosition = UI.MouseCell();
            for (int i = 0; i < s.stackVal; i++)
            {
                Pawn pawn = PawnxTool.CreateNewPawn(s.pkd, f, s.pkd.race, true);
                if (s.gender > Gender.None)
                {
                    pawn.gender = s.gender;
                }

                if (s.age >= 0)
                {
                    pawn.SetAge(s.age);
                }

                pawn.SpawnPawn(null, PlacingTool.lastKnownPosition);
            }
        }

        
        internal static void PlaceNearPawn(Selected s, Pawn p)
        {
            PlacingTool.JustPlace(s, p.Position, default(Rot4));
        }

        
        internal static void JustPlace(Selected s, IntVec3 pos = default(IntVec3), Rot4 rot = default(Rot4))
        {
            foreach (Thing t in s.Generate())
            {
                t.Spawn(rot, pos);
            }
        }

        
        internal static void PlaceInCustomPosition(Selected s, Action<Selected> onPlacedAction)
        {
            DebugMenuOption debugMenuOption = new DebugMenuOption("place object", DebugMenuOptionMode.Tool, delegate()
            {
                PlacingTool.JustPlace(s, default(IntVec3), PlacingTool.rotation);
                if (onPlacedAction != null)
                {
                    onPlacedAction(s);
                }
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        internal static void PlaceInPosition(Pawn p, PresetPawn ppn, IntVec3 pos)
        {
            p.SpawnPawn(ppn, pos);
        }

        
        internal static void TryCloneOrAbort(Pawn p)
        {
            if (Event.current.capsLock)
            {
                PawnxTool.AddOrCreateExistingPawn(p.ClonePawn());
                return;
            }

            DebugTools.curTool = null;
        }

        
        internal static void DeletePawnFromCustomPosition()
        {
            DebugMenuOption debugMenuOption = new DebugMenuOption(Label.DELETE_PAWN, DebugMenuOptionMode.Tool, delegate()
            {
                Find.CurrentMap.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(Find.CurrentMap, UI.MouseCell()));
                UI.MouseCell().DeletePawnsInCell();
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        internal static void BeginTeleportCustomPawn()
        {
            Selector selector = Find.Selector;
            if (selector != null)
            {
                selector.ClearSelection();
            }

            DebugMenuOption debugMenuOption = new DebugMenuOption(Label.SELECT_PAWN, DebugMenuOptionMode.Tool, delegate()
            {
                Pawn pawn = UI.MouseCell().FirstPawnInCellArea();
                if (pawn != null)
                {
                    PlacingTool.TeleportPawnAndReselect(pawn);
                }
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        internal static void BeginTeleportPawn(Pawn p)
        {
            DebugMenuOption debugMenuOption = new DebugMenuOption(Label.TELEPORT, DebugMenuOptionMode.Tool, delegate()
            {
                p.TeleportPawn(default(IntVec3));
                PlacingTool.DoEffect(p, UI.MouseCell());
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        private static void TeleportPawnAndReselect(Pawn p)
        {
            DebugMenuOption debugMenuOption = new DebugMenuOption(Label.TELEPORT, DebugMenuOptionMode.Tool, delegate()
            {
                p.TeleportPawn(default(IntVec3));
                PlacingTool.DoEffect(p, UI.MouseCell());
                PlacingTool.BeginTeleportCustomPawn();
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        internal static void Destroy()
        {
            DebugMenuOption debugMenuOption = new DebugMenuOption(Label.DESTROY, DebugMenuOptionMode.Tool, delegate()
            {
                IntVec3 intVec = UI.MouseCell();
                Find.CurrentMap.roofGrid.SetRoof(intVec, null);
                PlacingTool.ClearCell(intVec, Find.CurrentMap);
            });
            DebugTools.curTool = new DebugTool(debugMenuOption.label, debugMenuOption.method, null);
        }

        
        internal static void ClearCell(IntVec3 pos, Map map)
        {
            try
            {
                Dictionary<int, Thing> dictionary = new Dictionary<int, Thing>();
                foreach (Thing value in pos.GetThingList(map))
                {
                    dictionary.Add(dictionary.Count, value);
                }

                foreach (int key in dictionary.Keys)
                {
                    Thing thing = dictionary[key];
                    if (thing != null && thing.def != null)
                    {
                        if (thing.def.destroyable)
                        {
                            thing.Destroy(DestroyMode.Vanish);
                            break;
                        }

                        if (thing.def.category == Verse.ThingCategory.Building)
                        {
                            thing.DeSpawn(DestroyMode.Vanish);
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        
        static PlacingTool()
        {
        }

        
        internal static IntVec3 lastKnownPosition;

        
        internal static Rot4 rotation;
    }
}
