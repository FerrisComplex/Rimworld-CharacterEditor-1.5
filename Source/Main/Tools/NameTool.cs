using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using RimWorld;
using Verse;

namespace CharacterEditor
{
    
    internal static class NameTool
    {
        
        internal static string GetGenderInt(this Pawn pawn)
        {
            int gender = (int)pawn.gender;
            return gender.ToString();
        }

        
        internal static void SetGenderInt(this Pawn pawn, int gender)
        {
            bool flag = pawn == null || pawn.kindDef == null;
            if (!flag)
            {
                Gender gender2 = (Gender)gender;
                bool flag2 = (pawn.kindDef.RaceProps.hasGenders && gender2 != Gender.None) || (!pawn.kindDef.RaceProps.hasGenders && gender2 == Gender.None);
                if (flag2)
                {
                    pawn.gender = gender2;
                }
            }
        }

        
        internal static void SetPawnGender(this Pawn pawn, Gender gender)
        {
            bool flag = pawn == null;
            if (!flag)
            {
                Gender gender2 = pawn.gender;
                pawn.gender = gender;
                bool flag2 = !pawn.SetHead(true, false);
                if (flag2)
                {
                    pawn.gender = gender2;
                }

                CEditor.API.UpdateGraphics();
            }
        }

        
        internal static string GetPawnDescription(this Pawn pawn, Pawn otherPawn = null)
        {
            bool flag = pawn == null;
            string result;
            if (flag)
            {
                result = "";
            }
            else
            {
                string text = pawn.KindLabel ?? "";
                string text2;
                if (pawn == null)
                {
                    text2 = null;
                }
                else
                {
                    ThingDef def = pawn.def;
                    text2 = ((def != null) ? def.LabelCap.ToString() : null);
                }

                string text3 = text2;
                text3 = (text3 ?? "");
                text = text.CapitalizeFirst();
                text3 = text3.CapitalizeFirst();
                bool flag2 = text != text3 && !text.NullOrEmpty();
                if (flag2)
                {
                    text3 += ", ";
                    text3 += text;
                }

                text3 += ", ";
                text3 += pawn.gender.GetLabel(pawn.RaceProps.Animal).CapitalizeFirst();
                text3 += ", ";
                text3 += ((!pawn.HasAgeTracker()) ? "" : "AgeIndicator".Translate(pawn.ageTracker.AgeNumberString));
                bool flag3 = pawn.Faction != null;
                if (flag3)
                {
                    text3 += ", ";
                    text3 += pawn.Faction.Name.CapitalizeFirst().Colorize(pawn.Faction.Color);
                    bool flag4 = otherPawn != null && otherPawn.Faction != null;
                    if (flag4)
                    {
                        FactionRelationKind kind = (pawn.Faction == otherPawn.Faction) ? FactionRelationKind.Ally : pawn.Faction.RelationKindWith(otherPawn.Faction);
                        text3 = text3 + " " + (((pawn.Faction == otherPawn.Faction) ? 100 : pawn.Faction.GoodwillWith(otherPawn.Faction)).ToString() + " " + kind.GetLabel() + "\n").Colorize(kind.GetColor());
                    }
                }

                result = text3;
            }

            return result;
        }

        
        internal static string GetRoyalTitleAsSeparatedString(this Pawn p)
        {
            bool flag = !p.HasRoyaltyTracker();
            string result;
            if (flag)
            {
                result = "";
            }
            else
            {
                string text = "";
                bool flag2 = !p.royalty.MainTitle().IsNullOrEmpty();
                if (flag2)
                {
                    text = p.royalty.MainTitle().defName;
                }

                result = text;
            }

            return result;
        }

        
        internal static void SetRoyalTitleFromSeparatedString(this Pawn p, string s)
        {
            bool flag = !p.HasRoyaltyTracker();
            if (!flag)
            {
                bool flag2 = !p.royalty.AllTitlesForReading.NullOrEmpty<RoyalTitle>();
                if (flag2)
                {
                    foreach (RoyalTitle title in p.royalty.AllTitlesForReading)
                    {
                        p.RemoveTitle(title);
                    }
                }

                bool flag3 = s.NullOrEmpty();
                if (!flag3)
                {
                    p.SetTitle(s);
                }
            }
        }

        
        internal static void SetTitle(this Pawn pawn, string defName)
        {
            bool flag = !pawn.HasRoyaltyTracker();
            if (!flag)
            {
                RoyalTitleDef def = DefTool.GetDef<RoyalTitleDef>(defName);
                bool flag2 = def != null;
                if (flag2)
                {
                    Faction faction = (from f in Find.FactionManager.AllFactions
                        where f.def.RoyalTitlesAwardableInSeniorityOrderForReading.Count > 0
                        select f).ToList<Faction>().RandomElement<Faction>();
                    pawn.royalty.SetTitle(faction, def, true, false, true);
                }
            }
        }

        
        internal static string GetMainTitle(this Pawn pawn)
        {
            return pawn.HasRoyalTitle() ? pawn.royalty.MainTitle().GetLabelCapFor(pawn) : "";
        }

        
        internal static void RemoveTitle(this Pawn pawn, RoyalTitle title)
        {
            NameTool.Tmp_TitleTool tool = new NameTool.Tmp_TitleTool(pawn, title);
            List<RoyalTitlePermitDef> list;
            List<RoyalTitlePermitDef> list2;
            RoyalTitleUtility.FindLostAndGainedPermits(title.def, null, out list, out list2);
            StringBuilder stringBuilder = new StringBuilder();
            if (list2.Count > 0)
            {
                stringBuilder.AppendLine("RenounceTitleWillLoosePermits".Translate(pawn.Named("PAWN")) + ":");
                foreach (RoyalTitlePermitDef royalTitlePermitDef in list2)
                {
                    RoyalTitleDef royalTitleDef = tool.FirstTitleWithPermit(royalTitlePermitDef);
                    if (royalTitleDef != null)
                    {
                        stringBuilder.AppendLine("- " + royalTitlePermitDef.LabelCap + " (" + royalTitleDef.GetLabelFor(pawn) + ")");
                    }
                }

                stringBuilder.AppendLine();
            }

            if (!title.faction.def.renounceTitleMessage.NullOrEmpty())
            {
                stringBuilder.AppendLine(title.faction.def.renounceTitleMessage);
            }

            WindowTool.Open(Dialog_MessageBox.CreateConfirmation("RenounceTitleDescription".Translate(pawn.Named("PAWN"), "TitleOfFaction".Translate(title.def.GetLabelCapFor(pawn), title.faction.GetCallLabel()).Named("TITLE"), stringBuilder.ToString().TrimEndNewlines().Named("EFFECTS")), delegate() { tool.UpdateTitleAndPermits(); }, true, null, WindowLayer.Dialog));
        }

        
        internal static string GetPawnName(this Pawn pawn, bool needFull = false)
        {
            bool flag = pawn == null || pawn.Name == null;
            string result;
            if (flag)
            {
                result = "";
            }
            else
            {
                bool flag2 = pawn.Name.GetType() == typeof(NameSingle);
                if (flag2)
                {
                    result = ((NameSingle)pawn.Name).Name;
                }
                else
                {
                    bool flag3 = pawn.Name.GetType() == typeof(NameTriple);
                    if (flag3)
                    {
                        if (needFull)
                        {
                            result = ((NameTriple)pawn.Name).ToStringFull;
                        }
                        else
                        {
                            result = ((NameTriple)pawn.Name).ToStringShort;
                        }
                    }
                    else
                    {
                        result = "";
                    }
                }
            }

            return result;
        }

        
        internal static string GetPawnNameColored(this Pawn p, bool needFull = false)
        {
            bool flag = p == null;
            string result;
            if (flag)
            {
                result = "";
            }
            else
            {
                string pawnName = p.GetPawnName(needFull);
                Name name = p.Name;
                string text = (name != null) ? name.ToStringShort : null;
                text = (text ?? "");
                bool flag2 = !pawnName.NullOrEmpty();
                if (flag2)
                {
                    bool flag3 = pawnName.Contains("'" + text + "'");
                    if (flag3)
                    {
                        result = pawnName.SubstringTo("'", false) + text.Colorize(ColorTool.colTan) + pawnName.SubstringBackwardFrom("'", false);
                    }
                    else
                    {
                        result = pawnName.Replace(text, text.Colorize(ColorTool.colTan));
                    }
                }
                else
                {
                    result = (pawnName ?? text);
                }
            }

            return result;
        }

        
        internal static NameTriple GetPawnNameFromSeparatedString(string s)
        {
            bool flag = !s.NullOrEmpty();
            if (flag)
            {
                string[] array = s.Split(new string[]
                {
                    "?"
                }, StringSplitOptions.None);
                bool flag2 = array.Length == 3;
                if (flag2)
                {
                    string first = array[0];
                    string nick = array[1];
                    string last = array[2];
                    return new NameTriple(first, nick, last);
                }
            }

            return PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Either).RandomElement<NameTriple>();
        }

        
        internal static NameSingle GetPawnNameFromSeparatedString(string s, PawnKindDef pkdFallback)
        {
            bool flag = !s.NullOrEmpty();
            NameSingle result;
            if (flag)
            {
                result = new NameSingle(s, false);
            }
            else
            {
                result = new NameSingle((pkdFallback == null) ? "" : pkdFallback.label, false);
            }

            return result;
        }

        
        internal static string GetPawnNameAsSeparatedString(this Pawn pawn)
        {
            bool flag = pawn == null || pawn.Name == null;
            string result;
            if (flag)
            {
                result = "";
            }
            else
            {
                bool flag2 = pawn.Name.GetType() == typeof(NameTriple);
                if (flag2)
                {
                    NameTriple nameTriple = pawn.Name as NameTriple;
                    result = string.Concat(new string[]
                    {
                        nameTriple.First,
                        "?",
                        nameTriple.Nick,
                        "?",
                        nameTriple.Last
                    });
                }
                else
                {
                    bool flag3 = pawn.Name.GetType() == typeof(NameSingle);
                    if (flag3)
                    {
                        NameSingle nameSingle = pawn.Name as NameSingle;
                        result = nameSingle.Name;
                    }
                    else
                    {
                        result = "";
                    }
                }
            }

            return result;
        }

        
        internal static void SetNameFromSeparatedString(this Pawn p, string s)
        {
            bool flag = p == null || s.NullOrEmpty() || p.Name == null;
            if (!flag)
            {
                bool flag2 = p.Name.GetType() == typeof(NameSingle);
                if (flag2)
                {
                    p.SetName(NameTool.GetPawnNameFromSeparatedString(s, p.kindDef));
                }
                else
                {
                    p.SetName(NameTool.GetPawnNameFromSeparatedString(s));
                }
            }
        }

        
        internal static void SetName(this Pawn pawn, Name name)
        {
            bool flag = pawn == null || name == null;
            if (!flag)
            {
                bool flag2 = name.GetType() == typeof(NameTriple);
                if (flag2)
                {
                    NameTriple nameTriple = name as NameTriple;
                    pawn.Name = new NameTriple(nameTriple.First, nameTriple.Nick, nameTriple.Last);
                }
                else
                {
                    bool flag3 = name.GetType() == typeof(NameSingle);
                    if (flag3)
                    {
                        NameSingle nameSingle = name as NameSingle;
                        pawn.Name = new NameSingle(nameSingle.Name, nameSingle.Numerical);
                    }
                }
            }
        }

        
        internal static void SetName(this Pawn pawn, string first, string nick, string last)
        {
            bool flag = pawn == null;
            if (!flag)
            {
                pawn.Name = new NameTriple(first, nick, last);
            }
        }

        
        internal static Pawn GetPawnByNameTriple(NameTriple name)
        {
            foreach (Pawn pawn in PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead)
            {
                bool flag = pawn != null && pawn.Name != null && pawn.Name.GetType() == typeof(NameTriple);
                if (flag)
                {
                    bool flag2 = pawn.Name.ToStringFull == name.ToStringFull;
                    if (flag2)
                    {
                        return pawn;
                    }
                }
            }

            return null;
        }

        
        internal static Pawn GetPawnByNameSingle(NameSingle name)
        {
            bool flag = name == null;
            Pawn result;
            if (flag)
            {
                result = null;
            }
            else
            {
                foreach (Pawn pawn in PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead)
                {
                    bool flag2 = pawn != null && pawn.Name != null && pawn.Name.GetType() == typeof(NameSingle);
                    if (flag2)
                    {
                        bool flag3 = pawn.Name.ToStringFull == name.ToStringFull;
                        if (flag3)
                        {
                            return pawn;
                        }
                    }
                }

                result = null;
            }

            return result;
        }

        
        private class Tmp_TitleTool
        {
            
            public Tmp_TitleTool(Pawn pawn, RoyalTitle title)
            {
                this.pawn = pawn;
                this.title = title;
            }

            
            internal RoyalTitleDef FirstTitleWithPermit(RoyalTitlePermitDef permitDef)
            {
                return this.title.faction.def.RoyalTitlesAwardableInSeniorityOrderForReading.FirstOrDefault((RoyalTitleDef t) => t.permits != null && t.permits.Contains(permitDef));
            }

            
            internal void UpdateTitleAndPermits()
            {
                this.pawn.royalty.SetTitle(this.title.faction, null, false, false, true);
                this.pawn.royalty.ResetPermitsAndPoints(this.title.faction, this.title.def);
            }

            
            public RoyalTitle title;

            
            public Pawn pawn;
        }
    }
}
