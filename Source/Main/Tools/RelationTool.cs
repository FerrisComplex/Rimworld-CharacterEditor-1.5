// Decompiled with JetBrains decompiler
// Type: CharacterEditor.RelationTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class RelationTool
{
    internal static bool AreThosePawnSisBro(Pawn a, Pawn b)
    {
        var commonParent1 = GetCommonParent(a, b, (Gender)1);
        var commonParent2 = GetCommonParent(a, b, (Gender)2);
        return commonParent1 != null && commonParent2 != null;
    }

    internal static Pawn GetCommonParent(Pawn a, Pawn b, Gender gender)
    {
        Pawn pawn = null;
        if (a == null || b == null || a.relations == null || b.relations == null)
            return pawn;
        foreach (var directRelation1 in a.relations.DirectRelations)
            if (directRelation1.def == PawnRelationDefOf.Parent)
                foreach (var directRelation2 in b.relations.DirectRelations)
                    if (directRelation2.def == PawnRelationDefOf.Parent && directRelation1.otherPawn == directRelation2.otherPawn && directRelation1.otherPawn.gender == gender)
                        return pawn;
        return null;
    }

    internal static Pawn GetFirstParentForPawn(this Pawn p, Gender g)
    {
        if (p == null)
            return null;
        foreach (var relatedPawn in p.relations.RelatedPawns)
            if (relatedPawn.gender == g && p.relations.GetDirectRelation(PawnRelationDefOf.Parent, relatedPawn) != null)
                return relatedPawn;
        return null;
    }

    internal static List<Pawn> GetRelatedPawns(this Pawn p, out int countImpliedByOtherPawn)
    {
        countImpliedByOtherPawn = 0;
        var pawnList = new List<Pawn>();
        if (!p.HasRelationTracker() || p.relations.RelatedPawns.EnumerableNullOrEmpty())
            return pawnList;
        foreach (var relatedPawn in p.relations.RelatedPawns)
        {
            pawnList.Add(relatedPawn);
            foreach (var pawnRelationDef in p.GetRelations(relatedPawn).ToList())
                if (pawnRelationDef.implied)
                    ++countImpliedByOtherPawn;
        }

        return pawnList;
    }

    internal static string GetRelationAsSeparatedString(this DirectPawnRelation d)
    {
        return d == null || d.def.IsNullOrEmpty() ? "" : "" + d.def.defName + "|" + d.otherPawn.GetPawnNameAsSeparatedString() + "|" + d.startTicks;
    }

    internal static string GetRelationsAsSeparatedString(this Pawn p)
    {
        if (!p.HasRelationTracker() || p.relations.DirectRelations.NullOrEmpty())
            return "";
        var text = "";
        foreach (var directRelation in p.relations.DirectRelations)
        {
            text += directRelation.GetRelationAsSeparatedString();
            text += ":";
        }

        return text.SubstringRemoveLast();
    }

    internal static string RelationLabelDirect(this DirectPawnRelation dr)
    {
        return dr == null || dr.otherPawn == null ? "" : (dr.otherPawn.gender == Gender.Female ? dr.def.labelFemale : dr.def.label) + " " + dr.otherPawn.GetPawnName(true);
    }

    internal static string RelationLabelIndirect(this PawnRelationDef prd, Pawn otherPawn)
    {
        return prd == null || otherPawn == null ? "" : (otherPawn.gender == Gender.Female ? prd.labelFemale : prd.label) + " " + otherPawn.GetPawnName(true);
    }

    internal static string RelationTooltip(this Pawn pawn, Pawn otherPawn)
    {
        return otherPawn.GetPawnDescription(pawn) + "\n" + GetPawnRowTooltip(otherPawn, pawn);
    }

    internal static void SetRelationsFromSeparatedString(this Pawn p, string s)
    {
        if (s.NullOrEmpty() || !p.HasRelationTracker())
            return;
        var strArray1 = s.SplitNo(":");
        p.relations.ClearAllRelations();
        foreach (var s1 in strArray1)
        {
            var strArray2 = s1.SplitNo("|");
            if (strArray2.Length == 3)
            {
                var pawnRelationDef = DefTool.PawnRelationDef(strArray2[0]);
                if (pawnRelationDef != null)
                {
                    var fromSeparatedString = PawnxTool.GetOtherPawnFromSeparatedString(strArray2[1]);
                    if (fromSeparatedString != null)
                    {
                        p.relations.AddDirectRelation(pawnRelationDef, fromSeparatedString);
                        if (p.relations.DirectRelationExists(pawnRelationDef, fromSeparatedString))
                        {
                            var directPawnRelation = p.relations.DirectRelations.Last();
                            if (directPawnRelation.def.defName == pawnRelationDef.defName && directPawnRelation.otherPawn == fromSeparatedString)
                                directPawnRelation.startTicks = strArray2[2].AsInt32();
                        }
                    }
                }
            }
        }
    }

    private static string GetPawnRowTooltip(Pawn otherPawn, Pawn pawn)
    {
        var stringBuilder1 = new StringBuilder();
        if (otherPawn.RaceProps.Humanlike && pawn.RaceProps.Humanlike)
        {
            stringBuilder1.AppendLine(pawn.relations.OpinionExplanation(otherPawn));
            stringBuilder1.AppendLine();
            stringBuilder1.Append(new TaggedString("SomeonesOpinionOfMe".Translate( otherPawn.LabelShort, (Thing)otherPawn)));
            stringBuilder1.Append(": ");
            stringBuilder1.Append(otherPawn.relations.OpinionOf(pawn).ToStringWithSign());
        }
        else
        {
            stringBuilder1.AppendLine(otherPawn.LabelCapNoCount);
            var pawnSituationLabel = SocialCardUtility.GetPawnSituationLabel(otherPawn, pawn);
            if (!pawnSituationLabel.NullOrEmpty())
                stringBuilder1.AppendLine(pawnSituationLabel);
            stringBuilder1.AppendLine("--------------");
            var str = "";
            if (otherPawn.relations.DirectRelations.Count == 0)
                return otherPawn.relations.OpinionOf(pawn) < -20 ? new TaggedString("Rival".Translate()) : otherPawn.relations.OpinionOf(pawn) > 20 ? new TaggedString("Friend".Translate()) : new TaggedString("Acquaintance".Translate());
            for (var index = 0; index < otherPawn.relations.DirectRelations.Count; ++index)
            {
                var def = otherPawn.relations.DirectRelations[index].def;
                str = str.NullOrEmpty() ? def.GetGenderSpecificLabelCap(otherPawn) : str + ", " + def.GetGenderSpecificLabel(otherPawn);
            }

            stringBuilder1.Append(str);
        }

        stringBuilder1.AppendLine();
        var stringBuilder2 = stringBuilder1;
        var num = pawn.relations.CompatibilityWith(otherPawn);
        var str1 = "Compatibility: " + num.ToString("F2");
        stringBuilder2.AppendLine(str1);
        var stringBuilder3 = stringBuilder1;
        num = pawn.relations.SecondaryRomanceChanceFactor(otherPawn);
        var str2 = "RomanceChanceFactor: " + num.ToString("F2");
        stringBuilder3.Append(str2);
        return stringBuilder1.ToString();
    }
}
