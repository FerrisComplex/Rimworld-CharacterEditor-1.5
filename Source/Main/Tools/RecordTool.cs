using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor
{
	
	internal static class RecordTool
	{
		
		internal static string GetAsSeparatedString(this RecordDef r, float val)
		{
			bool flag = r == null;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				string text = "";
				text = text + r.defName + "|";
				text += val.ToString();
				result = text;
			}
			return result;
		}

		
		internal static string GetAllRecordsAsSeparatedString(this Pawn p)
		{
			bool flag = !p.HasRecordsTracker() || p.GetPawnRecords().EnumerableNullOrEmpty<KeyValuePair<RecordDef, float>>();
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				string text = "";
				foreach (KeyValuePair<RecordDef, float> keyValuePair in p.GetPawnRecords())
				{
					text += keyValuePair.Key.GetAsSeparatedString(keyValuePair.Value);
					text += ":";
				}
				text = text.SubstringRemoveLast();
				result = text;
			}
			return result;
		}

		
		internal static void SetRecords(this Pawn p, string s)
		{
			bool flag = s.NullOrEmpty() || !p.HasRecordsTracker();
			if (!flag)
			{
				string[] array = s.SplitNo(":");
				DefMap<RecordDef, float> pawnRecords = p.GetPawnRecords();
				foreach (string s2 in array)
				{
					string[] array3 = s2.SplitNo("|");
					bool flag2 = array3.Length == 2;
					if (flag2)
					{
						RecordDef recordDef = DefTool.RecordDef(array3[0]);
						bool flag3 = recordDef != null;
						if (flag3)
						{
							float value = array3[1].AsFloat();
							pawnRecords[recordDef] = value;
						}
					}
				}
				p.SetPawnRecords(pawnRecords);
				p.records.RecordsTick();
			}
		}

		
		internal static void SetPawnRecords(this Pawn p, DefMap<RecordDef, float> dic)
		{
			bool flag = p.HasRecordsTracker();
			if (flag)
			{
				p.records.SetMemberValue("records", dic);
			}
		}

		
		internal static DefMap<RecordDef, float> GetPawnRecords(this Pawn p)
		{
			return p.HasRecordsTracker() ? p.records.GetMemberValue<DefMap<RecordDef, float>>("records", null) : null;
		}

		
		internal static void DrawRecordCard(Rect rect, Pawn p)
		{
			Text.Font = GameFont.Small;
			List<RecordDef> allDefsListForReading = DefDatabase<RecordDef>.AllDefsListForReading;
			List<RecordDef> list = (from td in allDefsListForReading
			where td.type == RecordType.Time
			select td).ToList<RecordDef>();
			List<RecordDef> list2 = (from td in allDefsListForReading
			where td.type == RecordType.Int
			select td).ToList<RecordDef>();
			List<RecordDef> list3 = (from td in allDefsListForReading
			where td.type == RecordType.Float
			select td).ToList<RecordDef>();
			int count = list.Count;
			int count2 = list2.Count;
			int count3 = list3.Count;
			int num = Mathf.Max(count, count2 + count3);
			RecordTool.elemH = 21;
			float height = (float)(num * RecordTool.elemH) + 50f;
			Rect outRect = new Rect(rect);
			Rect rect2 = new Rect(0f, 0f, outRect.width - 16f, height);
			Widgets.BeginScrollView(outRect, ref RecordTool.scrollPos, rect2, true);
			rect2 = rect2.ContractedBy(4f);
            rect2.height = height;
			Rect rect3 = rect2;
			rect3.width *= 0.5f;
			Rect rect4 = rect2;
			rect4.x = rect3.xMax;
			rect4.width = rect2.width - rect4.x;
			rect3.xMax -= 6f;
			rect4.xMin += 6f;
			RecordTool.DrawLeftRect(rect3, list, p);
			RecordTool.DrawRightRect(rect4, list2, list3, p);
			Widgets.EndScrollView();
		}

		
		internal static void DrawLeftRect(Rect rect, List<RecordDef> l, Pawn p)
		{
			float num = 0f;
			Widgets.BeginGroup(rect);
			Widgets.ListSeparator(ref num, rect.width, "TimeRecordsCategory".Translate());
			foreach (RecordDef r in l)
			{
				num += RecordTool.DrawRecord(8f, num, rect.width - 8f, r, p);
			}
			Widgets.EndGroup();
		}

		
		internal static void DrawRightRect(Rect rect, List<RecordDef> li, List<RecordDef> lf, Pawn p)
		{
			float num = 0f;
			Widgets.BeginGroup(rect);
			Widgets.ListSeparator(ref num, rect.width, "MiscRecordsCategory".Translate());
			foreach (RecordDef r in li)
			{
				num += RecordTool.DrawRecord(8f, num, rect.width - 8f, r, p);
			}
			foreach (RecordDef r2 in lf)
			{
				num += RecordTool.DrawRecord(8f, num, rect.width - 8f, r2, p);
			}
			Widgets.EndGroup();
		}

		
		internal static float DrawRecord(float x, float y, float w, RecordDef r, Pawn p)
		{
			float num = w * 0.4f;
			string label = (r.type != RecordType.Time) ? p.records.GetValue(r).ToString("0.##") : p.records.GetAsInt(r).ToStringTicksToPeriod(true, false, true, true, false);
			Rect rect = new Rect(8f, y, w, (float)RecordTool.elemH);
			bool flag = Mouse.IsOver(rect);
			if (flag)
			{
				Widgets.DrawHighlight(rect);
			}
			Rect rect2 = rect;
			rect2.width -= num;
			Widgets.Label(rect2, r.LabelCap);
			SZWidgets.ButtonInvisible(rect2, delegate
			{
				RecordTool.selectedRecord = null;
			}, "");
			Rect rect3 = rect;
			rect3.x = rect2.xMax;
			rect3.width = num;
			bool flag2 = RecordTool.selectedRecord == r;
			if (flag2)
			{
				bool flag3 = r.type == RecordType.Int;
				if (flag3)
				{
					RecordTool.oldIVal = p.records.GetAsInt(r);
					int num2 = SZWidgets.NumericIntBox(rect3.x, rect3.y, 90f, rect3.height, RecordTool.oldIVal, 0, int.MaxValue);
					bool flag4 = num2 != RecordTool.oldIVal;
					if (flag4)
					{
						p.SetRecordValue(r, (float)num2);
					}
				}
				else
				{
					bool flag5 = r.type == RecordType.Float;
					if (flag5)
					{
						RecordTool.oldFVal = p.records.GetValue(r);
						float num3 = SZWidgets.NumericFloatBox(rect3.x, rect3.y, 90f, rect3.height, RecordTool.oldFVal, 0f, 1E+09f);
						bool flag6 = num3 != RecordTool.oldFVal;
						if (flag6)
						{
							p.SetRecordValue(r, num3);
						}
					}
					else
					{
						bool flag7 = r.type == RecordType.Time;
						if (flag7)
						{
							RecordTool.oldFVal = p.records.GetValue(r);
							long num4 = (long)(RecordTool.oldFVal / 60f);
							long num5 = SZWidgets.NumericLongBox(rect3.x, rect3.y, 90f, rect3.height, num4, 0L, long.MaxValue);
							bool flag8 = num5 != num4;
							if (flag8)
							{
								p.SetRecordValue(r, (float)(num5 * 60L));
							}
						}
					}
				}
			}
			else
			{
				Widgets.Label(rect3, label);
				SZWidgets.ButtonInvisibleMouseOverVar<RecordDef>(rect, delegate(RecordDef record)
				{
					RecordTool.selectedRecord = record;
				}, r, r.description);
			}
			return rect.height;
		}

		
		internal static void SetRecordValue(this Pawn p, RecordDef r, float val)
		{
			DefMap<RecordDef, float> memberValue = p.records.GetMemberValue<DefMap<RecordDef,float>>("records", null);
			bool flag = memberValue != null;
			if (flag)
			{
				memberValue[r] = val;
			}
		}

		
		internal static Vector2 scrollPos;

		
		internal static int elemH;

		
		internal static int oldIVal;

		
		internal static float oldFVal;

		
		internal static RecordDef selectedRecord;
	}
}
