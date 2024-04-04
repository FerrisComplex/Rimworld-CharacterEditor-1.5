// Decompiled with JetBrains decompiler
// Type: CharacterEditor.ScenPart_StartingAnimalExtra
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class ScenPart_StartingAnimalExtra : ScenPart
{
    public const float VeneratedAnimalWeight = 8f;
    private static readonly List<Pair<int, float>> PetCountChances;
    public int age = 1;
    public PawnKindDef animalKind;
    private float bondToRandomPlayerPawnChance = 0.5f;
    public int count = 1;
    private string countBuf;
    public Gender gender = (Gender)2;
    public IntVec3 location;
    public Name pawnName;

    static ScenPart_StartingAnimalExtra()
    {
        var pairList = new List<Pair<int, float>>();
        pairList.Add(new Pair<int, float>(1, 20f));
        pairList.Add(new Pair<int, float>(2, 10f));
        pairList.Add(new Pair<int, float>(3, 5f));
        pairList.Add(new Pair<int, float>(4, 3f));
        pairList.Add(new Pair<int, float>(5, 1f));
        pairList.Add(new Pair<int, float>(6, 1f));
        pairList.Add(new Pair<int, float>(7, 1f));
        pairList.Add(new Pair<int, float>(8, 1f));
        pairList.Add(new Pair<int, float>(9, 1f));
        pairList.Add(new Pair<int, float>(10, 0.1f));
        pairList.Add(new Pair<int, float>(11, 0.1f));
        pairList.Add(new Pair<int, float>(12, 0.1f));
        pairList.Add(new Pair<int, float>(13, 0.1f));
        pairList.Add(new Pair<int, float>(14, 0.1f));
        PetCountChances = pairList;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look<PawnKindDef>(ref this.animalKind, "animalKind");
        Scribe_Values.Look<int>(ref this.count, "count", 0, false);
        Scribe_Values.Look<Gender>(ref this.gender, "gender", Gender.Female, false);
        Scribe_Values.Look<int>(ref this.age, "age", 1, false);
        Scribe_Deep.Look<Name>(ref this.pawnName, "pawnName", null);
        Scribe_Values.Look<float>(ref this.bondToRandomPlayerPawnChance, "bondToRandomPlayerPawnChance", 0f, false);
    }


    public override void DoEditInterface(Listing_ScenEdit listing)
    {
        Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 2f);
        Listing_Standard listing_Standard = new Listing_Standard();
        listing_Standard.Begin(scenPartRect.TopHalf());
        listing_Standard.ColumnWidth = scenPartRect.width;
        listing_Standard.TextFieldNumeric<int>(ref this.count, ref this.countBuf, 1f, 1E+09f);
        listing_Standard.End();
        bool flag = !Widgets.ButtonText(scenPartRect.BottomHalf(), this.CurrentAnimalLabel().CapitalizeFirst(), true, true, true, null);
        if (!flag)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            list.Add(new FloatMenuOption("RandomPet".Translate().CapitalizeFirst(), delegate()
            {
                this.animalKind = null;
            }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
            foreach (PawnKindDef localKind2 in this.PossibleAnimals(false))
            {
                PawnKindDef localKind = localKind2;
                list.Add(new FloatMenuOption(localKind.LabelCap, delegate()
                {
                    this.animalKind = localKind;
                }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
    private IEnumerable<PawnKindDef> PossibleAnimals(bool checkForTamer = true)
    {
        return DefDatabase<PawnKindDef>.AllDefs.Where(td =>
        {
            if (!td.RaceProps.Animal)
                return false;
            return !checkForTamer || CanKeepPetTame(td);
        });
    }

    private static bool CanKeepPetTame(PawnKindDef def)
    {
        return (double)((Pawn_SkillTracker)GenCollection.MaxBy<Pawn, int>(Find.GameInitData.startingAndOptionalPawns.Take(Find.GameInitData.startingPawnCount), (c => ((Pawn_SkillTracker)c.skills).GetSkill(SkillDefOf.Animals).Level)).skills).GetSkill((SkillDef)SkillDefOf.Animals).Level >= (double)((BuildableDef)def.race).GetStatValueAbstract((StatDef)StatDefOf.MinimumHandlingSkill, (ThingDef)null);
    }

    private IEnumerable<PawnKindDef> RandomPets()
    {
        return PossibleAnimals().Where(td => td.RaceProps.petness > 0.0);
    }

    private string CurrentAnimalLabel()
    {
        return animalKind == null ? "RandomPet".TranslateSimple() : animalKind.label;
    }

    public override string Summary(Scenario scen)
    {
        return ScenSummaryList.SummaryWithList(scen, "PlayerStartsWith", ScenPart_StartingThing_Defined.PlayerStartWithIntro);
    }

    public override IEnumerable<string> GetSummaryListEntries(string tag)
    {
        if (tag == "PlayerStartsWith")
            yield return CurrentAnimalLabel().CapitalizeFirst() + " x" + count;
    }

    public override void Randomize()
    {
        animalKind = Rand.Value >= 0.5 ? GenCollection.RandomElement<PawnKindDef>(PossibleAnimals(false)) : null;
        var pair =GenCollection.RandomElementByWeight<Pair<int, float>>(PetCountChances, (pa => (pa).Second));
        count = (pair).First;
        bondToRandomPlayerPawnChance = 0.0f;
    }

    public override bool TryMerge(ScenPart other)
    {
        if (!(other is ScenPart_StartingAnimalExtra startingAnimalExtra) || startingAnimalExtra.animalKind != animalKind)
            return false;
        count += startingAnimalExtra.count;
        return true;
    }

    private float PetWeight(PawnKindDef animal)
    {
        var primaryIdeo = Find.GameInitData.playerFaction.ideos?.PrimaryIdeo;
        if (primaryIdeo != null)
            foreach (var veneratedAnimal in primaryIdeo.VeneratedAnimals)
                if (veneratedAnimal == animal.race)
                    return 8f;
        return animal.RaceProps.petness;
    }

    public override IEnumerable<Thing> PlayerStartingThings()
    {
        for (var i = 0; i < count; ++i)
            if (animalKind.IsAnimal() || animalKind == null)
            {
                var kindDef = animalKind == null ? GenCollection.RandomElementByWeight<PawnKindDef>(RandomPets(), (td => this.PetWeight(td))) : animalKind;
                var animal = PawnGenerator.GeneratePawn(kindDef, Faction.OfPlayer);
                if (animal.Name == null || animal.Name.Numerical)
                    animal.Name = PawnBioAndNameGenerator.GeneratePawnName(animal);
                animal.SetAge(age);
                if (gender != null && animal.gender != gender)
                    animal.gender = gender;
                if (pawnName != null)
                    animal.Name = pawnName;
                int num;
                if (Rand.Value < (double)bondToRandomPlayerPawnChance)
                {
                    var train = animal.training.CanAssignToTrain(TrainableDefOf.Obedience);
                    num = train.Accepted ? 1 : 0;
                }
                else
                {
                    num = 0;
                }

                if (num != 0)
                {
                    Pawn pawn = GenCollection.RandomElementWithFallback(Find.GameInitData.startingAndOptionalPawns.Take(Find.GameInitData.startingPawnCount).Where((p => TrainableUtility.CanBeMaster(p, animal, false) && !(p.story.traits).HasTrait(TraitDefOf.Psychopath))), null);
                    if (pawn != null)
                    {
                        animal.training.Train(TrainableDefOf.Obedience, null, true);
                        animal.training.SetWantedRecursive(TrainableDefOf.Obedience, true);
                        animal.playerSettings.Master = pawn;
                        if (pawn.Ideo == null || pawn.Ideo.MemberWillingToDo(new HistoryEvent(HistoryEventDefOf.Bonded, pawn.Named(HistoryEventArgsNames.Doer))))
                            pawn.relations.AddDirectRelation(PawnRelationDefOf.Bond, animal);
                    }

                    pawn = null;
                }

                yield return animal;
                kindDef = null;
            }
            else
            {
                Pawn pawn = null;
                try
                {
                    pawn = PawnxTool.CreateNewPawn(animalKind, Faction.OfPlayer, animalKind.race);
                    pawn.SetAge(age);
                    if (gender != null && pawn.gender != gender)
                        pawn.gender = gender;
                    if (pawnName != null)
                        pawn.Name = pawnName;
                }
                catch
                {
                }

                yield return pawn;
                pawn = null;
            }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode() ^ (animalKind != null ? animalKind.GetHashCode() : 0) ^ count ^ bondToRandomPlayerPawnChance.GetHashCode();
    }
}
