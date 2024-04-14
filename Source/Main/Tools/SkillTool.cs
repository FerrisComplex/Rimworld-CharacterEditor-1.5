using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CharacterEditor
{
	
	public static class SkillTool
	{
		
		public static string GetAllSkillsAsSeparatedString(this Pawn p)
		{
			bool flag = !p.HasSkillTracker() || p.skills.skills.NullOrEmpty<SkillRecord>();
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				string text = "";
				foreach (SkillRecord s in p.skills.skills)
				{
					text += s.GetSkillAsSeparatedString();
					text += ":";
				}
				text = text.SubstringRemoveLast();
				result = text;
			}
			return result;
		}

		
		public static string GetSkillAsSeparatedString(this SkillRecord s)
		{
			bool flag = s == null || s.def.IsNullOrEmpty();
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				string text = "";
				text = text + s.def.defName + "|";
				text = text + s.Level.ToString() + "|";
				string str = text;
				int passion = (int)s.passion;
				text = str + passion.ToString() + "|";
				text = text + s.xpSinceLastLevel.ToString() + "|";
				text += s.xpSinceMidnight.ToString();
				result = text;
			}
			return result;
		}

		
		public static void SetSkillsFromSeparatedString(this Pawn p, string s)
		{
			bool flag = !p.HasSkillTracker();
			if (!flag)
			{
				foreach (SkillRecord skillRecord in p.skills.skills)
				{
					skillRecord.Level = 0;
					skillRecord.levelInt = 0;
					skillRecord.passion = Passion.None;
					skillRecord.xpSinceLastLevel = 0f;
					skillRecord.xpSinceMidnight = 0f;
				}
				bool flag2 = s.NullOrEmpty();
				if (!flag2)
				{
					string[] array = s.Split(new string[]
					{
						":"
					}, StringSplitOptions.None);
					foreach (string text in array)
					{
						string[] array3 = text.Split(new string[]
						{
							"|"
						}, StringSplitOptions.None);
						bool flag3 = array3.Length == 5;
						if (flag3)
						{
							SkillDef skillDef = DefTool.SkillDef(array3[0]);
							bool flag4 = skillDef != null;
							if (flag4)
							{
								p.SetSkill(skillDef, array3[1].AsInt32(), (Passion)array3[2].AsInt32(), array3[3].AsFloat(), array3[4].AsFloat());
							}
						}
					}
				}
			}
		}

		
		public static void AddSkill(this Pawn p, SkillDef skd, int level, Passion passion, float xpSince, float xpMidnight)
		{
			bool flag = p == null || skd == null;
			if (!flag)
			{
				SkillRecord skillRecord = new SkillRecord(p, skd);
				skillRecord.passion = passion;
				skillRecord.Level = level;
				skillRecord.xpSinceLastLevel = xpSince;
				skillRecord.xpSinceMidnight = xpMidnight;
				p.skills.skills.Add(skillRecord);
			}
		}
        
		
		public static bool GetSkillRaw(this Pawn pawn, SkillDef skillDef, out SkillRecord output)
		{
			for (int index = 0; index < pawn.skills.skills.Count; ++index)
			{
				if (pawn.skills.skills[index].def == skillDef)
				{
					output = pawn.skills.skills[index];
					return true;
				}
			}

			output = null;
			return false;
		}

        
		public static List<SkillRecord> getAllSkills(this Pawn p)
		{
			List<SkillRecord> output = new List<SkillRecord>();
            
            foreach (var v in DefDatabase<SkillDef>.AllDefsListForReading)
            {
	            if (v == null) continue;
	            if (p.GetSkillRaw(v, out var result))
		            output.Add(result);
            }

            return output;
		}

		
		public static void CopySkillFromSkillRecord(this Pawn p, SkillRecord sr)
		{
			p.SetSkill(sr.def, sr.Level, sr.passion, sr.xpSinceLastLevel, sr.xpSinceMidnight);
		}

		
		public static void DisableSkillsFromList(this Pawn p, List<SkillDef> l)
		{
			bool flag = p == null || l.NullOrEmpty<SkillDef>();
			if (!flag)
			{
				foreach (SkillDef skill in l)
				{
					p.SkillDisable(skill, BoolUnknown.True);
				}
			}
		}

		
		public static void EnableAllSkills(this Pawn p)
		{
			bool flag = p == null || p.skills == null || p.skills.skills.NullOrEmpty<SkillRecord>();
			if (!flag)
			{
				p.SetMemberValue("cachedDisabledWorkTypes", new List<WorkTypeDef>());
				p.SetMemberValue("cachedDisabledWorkTypesPermanent", new List<WorkTypeDef>());
				foreach (SkillRecord obj in p.skills.skills)
				{
					obj.SetMemberValue("cachedTotallyDisabled", BoolUnknown.False);
				}
				Pawn_SkillTracker skills = p.skills;
				if (skills != null)
				{
					skills.Notify_SkillDisablesChanged();
				}
				p.Recalculate_WorkTypes();
			}
		}

		
		public static string GetIncapableOf(this Pawn pawn, out string toolTip)
		{
			toolTip = "";
			bool flag = pawn == null || pawn.story == null;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				foreach (WorkTags workTags in pawn.story.DisabledWorkTagsBackstoryAndTraits.GetAllSelectedItems<WorkTags>())
				{
					bool flag2 = workTags > WorkTags.None;
					if (flag2)
					{
						list2.Add(workTags.LabelTranslated());
					}
					foreach (WorkTypeDef workTypeDef in DefDatabase<WorkTypeDef>.AllDefs)
					{
						bool flag3 = (workTypeDef.workTags & workTags) > WorkTags.None;
						if (flag3)
						{
							bool flag4 = !list.Contains(workTypeDef.labelShort);
							if (flag4)
							{
								list.Add(workTypeDef.labelShort);
							}
						}
					}
				}
				toolTip = toolTip + Label.DISABLEDWORKTAGS + ":\n";
				foreach (string str in list)
				{
					toolTip = toolTip + "- " + str + "\n";
				}
				bool flag5 = pawn.skills != null && pawn.skills.skills != null;
				if (flag5)
				{
					toolTip = toolTip + "\n" + Label.DISABLEDSKILLS + ":\n";
					foreach (SkillRecord skillRecord in pawn.skills.skills)
					{
						bool flag6 = pawn.story.Childhood != null && skillRecord.def.IsDisabled(pawn.story.Childhood.workDisables, pawn.story.Childhood.DisabledWorkTypes);
						if (flag6)
						{
							toolTip += "- " + skillRecord.def.LabelCap + " (" + pawn.story.Childhood.TitleCapFor(pawn.gender) + ")" + "\n";
						}
						bool flag7 = pawn.story.Adulthood != null && skillRecord.def.IsDisabled(pawn.story.Adulthood.workDisables, pawn.story.Adulthood.DisabledWorkTypes);
						if (flag7)
						{
							toolTip = string.Concat(new string[]
							{
								toolTip,
								"- ",
								skillRecord.def.skillLabel,
								" (",
								pawn.story.Adulthood.TitleCapFor(pawn.gender),
								")\n"
							});
						}
					}
				}
				string text = "";
				foreach (string str2 in list2)
				{
					text = text + str2 + ", ";
				}
				bool flag8 = text.EndsWith(", ");
				if (flag8)
				{
					text = text.Substring(0, text.Length - 2);
				}
				result = text;
			}
			return result;
		}

		
		public static int GetSkillIndex(this SkillDef skill)
		{
			bool flag = skill == SkillDefOf.Shooting;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = skill == SkillDefOf.Melee;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					bool flag3 = skill == SkillDefOf.Construction;
					if (flag3)
					{
						result = 2;
					}
					else
					{
						bool flag4 = skill == SkillDefOf.Mining;
						if (flag4)
						{
							result = 3;
						}
						else
						{
							bool flag5 = skill == SkillDefOf.Cooking;
							if (flag5)
							{
								result = 4;
							}
							else
							{
								bool flag6 = skill == SkillDefOf.Plants;
								if (flag6)
								{
									result = 5;
								}
								else
								{
									bool flag7 = skill == SkillDefOf.Animals;
									if (flag7)
									{
										result = 6;
									}
									else
									{
										bool flag8 = skill == SkillDefOf.Crafting;
										if (flag8)
										{
											result = 7;
										}
										else
										{
											bool flag9 = skill == SkillDefOf.Artistic;
											if (flag9)
											{
												result = 8;
											}
											else
											{
												bool flag10 = skill == SkillDefOf.Medicine;
												if (flag10)
												{
													result = 9;
												}
												else
												{
													bool flag11 = skill == SkillDefOf.Social;
													if (flag11)
													{
														result = 10;
													}
													else
													{
														bool flag12 = skill == SkillDefOf.Intellectual;
														if (flag12)
														{
															result = 11;
														}
														else
														{
															result = 12;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public static bool ListHasOneOrMoreMatches(List<WorkTags> l1, List<WorkTags> l2)
		{
			foreach (WorkTags workTags in l1)
			{
				foreach (WorkTags workTags2 in l2)
				{
					bool flag = workTags == workTags2;
					if (flag)
					{
						return true;
					}
				}
			}
			return false;
		}

		
		public static List<WorkTags> ListOfDiabledWorkTagsByBackstory(this Pawn p)
		{
			bool flag = p == null || p.story == null;
			List<WorkTags> result;
			if (flag)
			{
				result = new List<WorkTags>();
			}
			else
			{
				result = p.story.DisabledWorkTagsBackstoryAndTraits.GetAllSelectedItems<WorkTags>().ToList<WorkTags>();
			}
			return result;
		}

		
		public static List<WorkTags> ListOfDisablingWorkTagsForSkill(this SkillDef s)
		{
			bool flag = s == null;
			List<WorkTags> result;
			if (flag)
			{
				result = new List<WorkTags>();
			}
			else
			{
				result = s.disablingWorkTags.GetAllSelectedItems<WorkTags>().ToList<WorkTags>();
			}
			return result;
		}

		
		public static void PasteSkills(this Pawn p, List<SkillRecord> l)
		{
			bool flag = p == null || p.skills == null || p.skills.skills.NullOrEmpty<SkillRecord>() || l.NullOrEmpty<SkillRecord>();
			if (!flag)
			{
				foreach (SkillRecord sr in l)
				{
					p.CopySkillFromSkillRecord(sr);
				}
				Pawn_SkillTracker skills = p.skills;
				if (skills != null)
				{
					skills.Notify_SkillDisablesChanged();
				}
				p.Recalculate_WorkTypes();
			}
		}

		
		public static void SetSkill(this Pawn p, SkillDef skd, int level, Passion passion, float xpSince, float xpMidnight)
		{
			bool flag = p == null || skd == null;
			if (!flag)
			{
				foreach (SkillRecord skillRecord in p.skills.skills)
				{
					bool flag2 = skillRecord.def.defName == skd.defName;
					if (flag2)
					{
						skillRecord.passion = passion;
						skillRecord.levelInt = level - skillRecord.Aptitude;
						skillRecord.xpSinceLastLevel = xpSince;
						skillRecord.xpSinceMidnight = xpMidnight;
						break;
					}
				}
			}
		}

		
		public static void SkillDisable(this Pawn p, SkillRecord skillRecord, BoolUnknown val)
		{
			bool flag = p == null || p.skills == null;
			if (!flag)
			{
				skillRecord.SetMemberValue("cachedTotallyDisabled", val);
			}
		}

		
		public static void SkillDisable(this Pawn p, SkillDef skill, BoolUnknown val)
		{
			bool flag = p == null || p.skills == null;
			if (!flag)
			{
				SkillRecord skill2 = p.skills.GetSkill(skill);
				skill2.SetMemberValue("cachedTotallyDisabled", val);
			}
		}
	}
}
