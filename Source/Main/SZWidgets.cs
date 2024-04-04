using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CharacterEditor
{
	
	internal static class SZWidgets
	{
		
		private static void DefSelectorSimpleBase<T>(Rect r, int w, HashSet<T> l, ref T def, string labelInfo, Func<T, string> labelGetter, Action<T> onSelect, bool hasLR = false, bool hasLIcon = false, Action<T> onLIcon = null, bool drawLabel = true) where T : Def
		{
			r.width = (float)w;
			Text.Font = GameFont.Small;
			if (drawLabel)
			{
				Rect rect = r.RectLabel(hasLR, hasLIcon, false);
				SZWidgets.LabelBackground(rect, labelInfo + labelGetter(def), ColorTool.colAsche, 0, "", default(Color));
				string toolTip = FLabel.DefDescription<T>(def);
				SZWidgets.ButtonInvisibleMouseOver(rect, delegate
				{
					SZWidgets.FloatMenuOnRect<T>(l, labelGetter, onSelect, null, true);
				}, toolTip);
			}
			if (hasLR)
			{
				if (Widgets.ButtonImage(SZWidgets.RectPrevious(r), ContentFinder<Texture2D>.Get("bbackward", true), true, null))
				{
					def = l.GetPrev(def);
					if (onSelect != null)
					{
						onSelect(def);
					}
				}
				if (Widgets.ButtonImage(SZWidgets.RectNext(r), ContentFinder<Texture2D>.Get("bforward", true), true, null))
				{
					def = l.GetNext(def);
					if (onSelect != null)
					{
						onSelect(def);
					}
				}
			}
		}

		
		internal static void DefSelectorSimpleBullet<T>(Rect r, int posX, int posY, int w, HashSet<T> l, ref T def, string labelInfo, Func<T, string> labelGetter, Action<T> onSelect, bool hasLR = false, Texture2D texLIcon = null, float angle = 0f, Action<T> onLIcon = null, bool drawLabel = true) where T : Def
		{
			SZWidgets.DefSelectorSimpleBase<T>(r, w, l, ref def, labelInfo, labelGetter, onSelect, hasLR, texLIcon != null, onLIcon, drawLabel);
			if (texLIcon != null)
			{
				if (!drawLabel)
				{
					SZWidgets.ButtonInvisibleMouseOver(r, delegate
					{
						SZWidgets.FloatMenuOnRect<T>(l, labelGetter, onLIcon, null, true);
					}, FLabel.DefDescription<T>(def));
					return;
				}
				SZWidgets.ButtonInvisibleVar<T>(r, onLIcon, def, def.STooltip<T>());
			}
		}

		
		internal static void DefSelectorSimpleTex<T>(Rect r, int w, HashSet<T> l, ref T def, string labelInfo, Func<T, string> labelGetter, Action<T> onSelect, bool hasLR = false, Texture2D texLIcon = null, Action<T> onLIcon = null, bool drawLabel = true, string tooltip = "") where T : Def
		{
			SZWidgets.DefSelectorSimpleBase<T>(r, w, l, ref def, labelInfo, labelGetter, onSelect, hasLR, texLIcon != null, onLIcon, drawLabel);
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(r, tooltip);
			}
			if (texLIcon != null)
			{
				Rect rect = r.RectIconLeft(hasLR);
				if (!drawLabel)
				{
					SZWidgets.Image(r.RectIconLeft(hasLR), texLIcon);
					SZWidgets.ButtonInvisibleMouseOver(rect, delegate
					{
						SZWidgets.FloatMenuOnRect<T>(l, labelGetter, onLIcon, null, true);
					}, FLabel.DefDescription<T>(def));
					return;
				}
				SZWidgets.ButtonImageVar<T>(rect, texLIcon, onLIcon, def, def.STooltip<T>());
			}
		}

		
		internal static void DefSelectorSimple<T>(Rect r, int w, HashSet<T> l, ref T def, string labelInfo, Func<T, string> labelGetter, Action<T> onSelect, bool hasLR = false, string texLIcon = null, Action<T> onLIcon = null, bool drawLabel = true, string tooltip = "") where T : Def
		{
			SZWidgets.DefSelectorSimpleBase<T>(r, w, l, ref def, labelInfo, labelGetter, onSelect, hasLR, texLIcon != null, onLIcon, drawLabel);
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(r, tooltip);
			}
			if (texLIcon != null)
			{
				Rect rect = r.RectIconLeft(hasLR);
				if (!drawLabel)
				{
					SZWidgets.Image(r.RectIconLeft(hasLR), texLIcon);
					SZWidgets.ButtonInvisibleMouseOver(rect, delegate
					{
						SZWidgets.FloatMenuOnRect<T>(l, labelGetter, onLIcon, null, true);
					}, FLabel.DefDescription<T>(def));
					return;
				}
				SZWidgets.ButtonImageVar<T>(rect, texLIcon, onLIcon, def, def.STooltip<T>());
			}
		}

		
		internal static void NonDefSelectorSimple<T>(Rect r, int w, HashSet<T> l, ref T val, string labelInfo, Func<T, string> labelGetter, Action<T> onSelect, bool hasLR = false, string texLIcon = null, Action<T> onLIcon = null)
		{
			r.width = (float)w;
			Text.Font = GameFont.Small;
			bool flag = texLIcon != null;
			Rect rect = r.RectLabel(hasLR, flag, false);
			SZWidgets.LabelBackground(rect, labelInfo + labelGetter(val), ColorTool.colAsche, 0, "", default(Color));
			SZWidgets.ButtonInvisibleMouseOver(rect, delegate
			{
				SZWidgets.FloatMenuOnRect<T>(l, labelGetter, onSelect, null, true);
			}, "");
			if (hasLR)
			{
				if (Widgets.ButtonImage(SZWidgets.RectPrevious(r), ContentFinder<Texture2D>.Get("bbackward", true), true, null))
				{
					val = l.GetPrev(val);
					if (onSelect != null)
					{
						onSelect(val);
					}
				}
				if (Widgets.ButtonImage(SZWidgets.RectNext(r), ContentFinder<Texture2D>.Get("bforward", true), true, null))
				{
					val = l.GetNext(val);
					if (onSelect != null)
					{
						onSelect(val);
					}
				}
			}
			if (flag)
			{
				SZWidgets.ButtonImageVar<T>(r.RectIconLeft(hasLR), texLIcon, onLIcon, val, "");
			}
		}

		
		internal static Rect RectPlusY(this Rect rect, int y)
		{
			return new Rect(rect.x, rect.y + (float)y, rect.width, rect.height);
		}

		
		internal static Rect RectLablelI(this Rect rect, int inputW)
		{
			return new Rect(rect.x, rect.y, rect.width - (float)inputW - rect.height * 2f, rect.height);
		}

		
		internal static Rect RectInput(this Rect rect, int inputW)
		{
			return new Rect(rect.x + rect.width - rect.height - (float)inputW, rect.y, (float)inputW, rect.height);
		}

		
		internal static Rect RectMinus(this Rect rect, int inputW)
		{
			return new Rect(rect.x + rect.width - rect.height * 2f - (float)inputW, rect.y, rect.height, rect.height);
		}

		
		internal static Rect RectPlus(this Rect rect)
		{
			return new Rect(rect.x + rect.width - rect.height, rect.y, rect.height, rect.height);
		}

		
		internal static Rect RectSlider(this Rect rect)
		{
			return new Rect(rect.x, rect.y + rect.height, rect.width, rect.height);
		}

		
		internal static Rect RectLabel(this Rect rect, bool inEditMode, bool hasLeftIcon, bool hasRightIcon)
		{
			return new Rect(rect.x + (float)SZWidgets.OffsetEditLeft(inEditMode) + SZWidgets.OffsetIcon(hasLeftIcon, rect.height), rect.y, rect.width - (float)SZWidgets.OffsetEditBoth(inEditMode) - SZWidgets.OffsetIcon(hasLeftIcon, rect.height) - SZWidgets.OffsetIcon(hasRightIcon, rect.height), rect.height);
		}

		
		internal static Rect RectIconLeft(this Rect rect, bool inEditMode)
		{
			return new Rect(rect.x + (float)SZWidgets.OffsetEditLeft(inEditMode), rect.y, rect.height, rect.height);
		}

		
		internal static Rect RectIconRight(this Rect rect, bool inEditMode)
		{
			return new Rect(rect.x + rect.width - (float)SZWidgets.OffsetEditLeft(inEditMode) - rect.height, rect.y, rect.height, rect.height);
		}

		
		private static int OffsetEditLeft(bool inEditMode)
		{
			if (!inEditMode)
			{
				return 0;
			}
			return 21;
		}

		
		private static int OffsetEditBoth(bool inEditMode)
		{
			if (!inEditMode)
			{
				return 0;
			}
			return 42;
		}

		
		private static float OffsetIcon(bool hasIcon, float h)
		{
			if (!hasIcon)
			{
				return 0f;
			}
			return h;
		}

		
		private static void GetEditRect(Rect rLabel, out Rect rValue, out Rect rLeft)
		{
			rLeft = new Rect(rLabel);
			rLeft.width -= 80f;
			rValue = new Rect(rLabel);
			rValue.x += rLeft.width;
			rValue.width = 80f;
		}

		
		private static void GetEditRects(Rect rLabel, string label, out Rect rValue, out Rect rLeft, out Rect rRight)
		{
			Vector2 vector = Text.CalcSize(label);
			rValue = new Rect(rLabel);
			rValue.x += vector.x + 15f;
			rValue.width = 80f;
			rLeft = new Rect(rLabel);
			rLeft.width = vector.x + 15f;
			rRight = new Rect(rLabel);
			rRight.x = rValue.x + rValue.width;
			rRight.width = rLabel.width - rRight.x + (float)SZWidgets.OffsetEditLeft(true);
		}

		
		private static void ToggleParameterSlider(int id)
		{
			SZWidgets.iUID = ((SZWidgets.iUID == id) ? -1 : id);
		}

		
		internal static void LabelFloatZeroFieldSlider(Listing_X view, int w, int id, Func<float?, string> FLabelValue, ref float? value, float min, float max, int decimals)
		{
			SZWidgets.LabelFloatZeroFieldSlider(view.GetRect(22f, 1f), w, id, FLabelValue, ref value, min, max, decimals, view);
		}

		
		internal static void LabelFloatZeroFieldSlider(Rect r, int w, int id, Func<float?, string> FLabelValue, ref float? value, float min, float max, int decimals, Listing_X view = null)
		{
			bool flag = SZWidgets.iUID == id;
			int inputW = 80;
			r.width = (float)w;
			Rect obj = flag ? r.RectLablelI(inputW) : r;
			Text.Font = GameFont.Small;
			var rect = obj;
			SZWidgets.LabelBackground(rect, FLabelValue(value), ColorTool.colAsche, 0, "", default(Color));
			SZWidgets.ButtonInvisibleMouseOver(rect, delegate
			{
				SZWidgets.ToggleParameterSlider(id);
			}, "");
			if (flag)
			{
				if (Widgets.ButtonText(r.RectMinus(inputW), "-", true, true, true, null) && value != null)
				{
					value -= 1f;
					value = new float?((float)Math.Round((double)value.Value, decimals));
				}
				if (Widgets.ButtonText(r.RectPlus(), "+", true, true, true, null) && value != null)
				{
					value += 1f;
					value = new float?((float)Math.Round((double)value.Value, decimals));
				}
				Rect rect2 = (view != null) ? view.GetRect(r.height, 1f) : r.RectSlider();
				float value2 = (value != null) ? value.Value : 0f;
				value2 = SZWidgets.FloatSlider(rect2, value2, min, max, decimals);
				value2 = SZWidgets.FloatField(r.RectInput(inputW), value2, float.MinValue, float.MaxValue);
				value = new float?(value2);
			}
			if (view != null)
			{
				view.Gap(2f);
			}
		}

		
		internal static void LabelFloatFieldSlider(Listing_X view, int w, int id, Func<float, string> FLabelValue, ref float value, float min, float max, int decimals)
		{
			SZWidgets.LabelFloatFieldSlider(view.GetRect(22f, 1f), w, id, FLabelValue, ref value, min, max, decimals, view);
		}

		
		internal static void LabelFloatFieldSlider(Rect r, int w, int id, Func<float, string> FLabelValue, ref float value, float min, float max, int decimals, Listing_X view = null)
		{
			bool flag = SZWidgets.iUID == id;
			int inputW = 80;
			r.width = (float)w;
			Rect obj = flag ? r.RectLablelI(inputW) : r;
			Text.Font = GameFont.Small;
			Rect rect = obj;
			SZWidgets.LabelBackground(rect, FLabelValue(value), ColorTool.colAsche, 0, "", default(Color));
			SZWidgets.ButtonInvisibleMouseOver(rect, delegate
			{
				SZWidgets.ToggleParameterSlider(id);
			}, "");
			if (flag)
			{
				if (Widgets.ButtonText(r.RectMinus(inputW), "-", true, true, true, null))
				{
					value -= 1f;
					value = (float)Math.Round((double)value, decimals);
				}
				if (Widgets.ButtonText(r.RectPlus(), "+", true, true, true, null))
				{
					value += 1f;
					value = (float)Math.Round((double)value, decimals);
				}
				Rect rect2 = (view != null) ? view.GetRect(r.height, 1f) : r.RectSlider();
				value = SZWidgets.FloatSlider(rect2, value, min, max, decimals);
				value = SZWidgets.FloatField(r.RectInput(inputW), value, float.MinValue, float.MaxValue);
			}
			if (view != null)
			{
				view.Gap(2f);
			}
		}

		
		internal static void LabelIntFieldSlider(Listing_X view, int w, int id, Func<int, string> FLabelValue, ref int value, int min, int max)
		{
			SZWidgets.LabelIntFieldSlider(view.GetRect(22f, 1f), w, id, FLabelValue, ref value, min, max, view);
		}

		
		internal static void LabelIntFieldSlider(Rect r, int w, int id, Func<int, string> FLabelValue, ref int value, int min, int max, Listing_X view = null)
		{
			bool flag = SZWidgets.iUID == id;
			int inputW = 80;
			r.width = (float)w;
			Rect obj = flag ? r.RectLablelI(inputW) : r;
			Text.Font = GameFont.Small;
			var rect = obj;
			SZWidgets.LabelBackground(rect, FLabelValue(value), ColorTool.colAsche, 0, "", default(Color));
			SZWidgets.ButtonInvisibleMouseOver(rect, delegate
			{
				SZWidgets.ToggleParameterSlider(id);
			}, "");
			if (flag)
			{
				if (Widgets.ButtonText(r.RectMinus(inputW), "-", true, true, true, null))
				{
					value--;
				}
				if (Widgets.ButtonText(r.RectPlus(), "+", true, true, true, null))
				{
					value++;
				}
				Rect rect2 = (view != null) ? view.GetRect(r.height, 1f) : r.RectSlider();
				value = SZWidgets.IntSlider(rect2, value, min, max);
				value = SZWidgets.IntField(r.RectInput(inputW), value, int.MinValue, int.MaxValue);
			}
			if (view != null)
			{
				view.Gap(2f);
			}
		}

		
		internal static float FloatSlider(Rect rect, float value, float min, float max, int decimals)
		{
			return (float)Math.Round((double)Widgets.HorizontalSlider(rect, value, min, max, false, null, null, null, -1f), decimals);
		}

		
		internal static int IntSlider(Rect rect, int value, int min, int max)
		{
			return (int)Widgets.HorizontalSlider(rect, (float)value, (float)min, (float)max, false, null, null, null, -1f);
		}

		
		internal static float FloatField(Rect rect, float value, float min, float max)
		{
			string text = value.ToString();
			if (text.EndsWith("."))
			{
				text += "0";
			}
			else if (!text.Contains("."))
			{
				text += ".0";
			}
			text = Widgets.TextField(rect, text, 32, null);
			if (text.EndsWith("."))
			{
				text += "0";
			}
			else if (!text.Contains("."))
			{
				text += ".0";
			}
			float num = 0f;
			float.TryParse(text, out num);
			if (num < min)
			{
				value = min;
			}
			else if (num > max)
			{
				value = max;
			}
			else
			{
				value = num;
			}
			return value;
		}

		
		internal static int IntField(Rect rect, int value, int min, int max)
		{
			string text = value.ToString();
			text = Widgets.TextField(rect, text, 32, null);
			int num = 0;
			int.TryParse(text, out num);
			if (num < min)
			{
				value = min;
			}
			else if (num > max)
			{
				value = max;
			}
			else
			{
				value = num;
			}
			return value;
		}

		
		private static string GetFormattedValue(string format, float value)
		{
			string result;
			if (format == "%")
			{
				result = " [" + Math.Round((double)(100f * value), 0).ToString() + " %]";
			}
			else if (format == "s")
			{
				result = " [" + value.ToString() + " s]";
			}
			else if (format == "ticks")
			{
				result = " [" + value.ToString() + " ticks]";
			}
			else if (format == "rpm")
			{
				result = " [" + ((value == 0f) ? CharacterEditor.Label.INFINITE : Math.Round((double)(60f / value * 60f), 0).ToString()) + " rpm]";
			}
			else if (format == "cps")
			{
				result = " [" + ((value == 0f) ? CharacterEditor.Label.INFINITE : value.ToString()) + " cps]";
			}
			else if (format == "cells")
			{
				result = " [" + value.ToString() + " cells]";
			}
			else if (format.StartsWith("max"))
			{
				result = string.Concat(new string[]
				{
					" [",
					value.ToString(),
					"/",
					format.SubstringFrom("max", true),
					"]"
				});
			}
			else if (format == "int")
			{
				result = " [" + ((int)Math.Round((double)value)).ToString() + "]";
			}
			else if (format == "quadrum")
			{
				result = " [" + Enum.GetName(typeof(Quadrum), (int)value) + "]";
			}
			else if (format == "addict")
			{
				result = " " + (100.0 - Math.Round((double)(100f * value), 0)).ToString() + " %";
			}
			else if (format == "high")
			{
				result = " " + value.ToStringPercent("F0");
			}
			else if (format.StartsWith("DEF"))
			{
				result = " [" + format.SubstringFrom("DEF", true) + "]";
			}
			else if (format.StartsWith("dauer"))
			{
				result = " " + format.SubstringFrom("dauer", true);
			}
			else if (format == "pain")
			{
				int num = (int)value;
				result = ((num == 0) ? CharacterEditor.Label.PAINLESS : ("PainCategory_" + HealthTool.ConvertSliderToPainCategory(num).ToString()).Translate().ToString());
			}
			else if (format.StartsWith("comp"))
			{
				if (!format.Contains("%"))
				{
					result = " " + value.ToStringPercent("F0") + " " + format.SubstringFrom("comp", true);
				}
				else
				{
					result = " " + format.SubstringFrom("comp", true);
				}
			}
			else
			{
				result = " [" + value.ToString() + "]";
			}
			return result;
		}

		
		internal static void FloatMenuOnButtonImage<T>(Rect rect, Texture2D tex, ICollection<T> l, Func<T, string> labelGetter, Action<T> action, string toolTip = "")
		{
			GUI.color = ((!Mouse.IsOver(rect)) ? Color.white : GenUI.MouseoverColor);
			GUI.DrawTexture(rect, tex);
			GUI.color = Color.white;
			if (Widgets.ButtonInvisible(rect, true))
			{
				SZWidgets.FloatMenuOnRect<T>(l, labelGetter, action, null, true);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}

		
		internal static void FloatMenuOnButtonInvisible<T>(Rect rect, ICollection<T> l, Func<T, string> labelGetter, Action<T> action, string toolTip = "")
		{
			if (Widgets.ButtonInvisible(rect, true))
			{
				SZWidgets.FloatMenuOnRect<T>(l, labelGetter, action, null, true);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}

		
		internal static void FloatMenuOnButtonStuffOrStyle<T>(Rect rectThing, Rect rectClickable, ICollection<T> l, Func<T, string> labelGetter, Selected s, Action<T> action) where T : Def
		{
			if (Mouse.IsOver(rectClickable))
			{
				Widgets.DrawHighlight(rectClickable);
			}
			if (typeof(T) == typeof(ThingStyleDef))
			{
				GUI.DrawTexture(rectThing, SZWidgets.IconForStyle(s));
				TooltipHandler.TipRegion(rectThing, s.style.STooltip<ThingStyleDef>());
			}
			else
			{
				GUI.DrawTexture(rectThing, SZWidgets.IconForStuff(s));
				TooltipHandler.TipRegion(rectThing, s.stuff.STooltip<ThingDef>());
			}
			if (Widgets.ButtonInvisible(rectClickable, true))
			{
				SZWidgets.FloatMenuOnRect<T>(l, labelGetter, action, s, true);
			}
		}

		
		internal static void FloatMenuOnButtonText<T>(Rect rect, string curVal, ICollection<T> l, Func<T, string> labelGetter, Action<T> action, string toolTip = "")
		{
			if (Widgets.ButtonText(rect, curVal, true, true, true, null))
			{
				SZWidgets.FloatMenuOnRect<T>(l, labelGetter, action, null, true);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}

		
		internal static void FloatMenuOnLabel<T>(Rect rect, Color color, ICollection<T> l, Func<T, string> labelGetter, T selected, Action<T> action, string toolTip = "")
		{
			SZWidgets.LabelBackground(rect, labelGetter(selected), color, 0, "", default(Color));
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawLightHighlight(rect);
			}
			SZWidgets.FloatMenuOnButtonInvisible<T>(new Rect(rect.x, rect.y, rect.width - 20f, rect.height), l, labelGetter, action, toolTip);
		}

		
		internal static void FloatMenuOnLabelAndImage<T>(Rect rect, Color imgColor, string texPath, Pawn pawnForImage, Color lblColor, ICollection<T> l, Func<T, string> labelGetter, T selected, Action<T> action, Action imgAction, bool showFloatMenu = true) where T : Def
		{
			Widgets.DrawBoxSolid(rect, imgColor);
			if (pawnForImage != null)
			{
				RenderTexture image = PortraitsCache.Get(pawnForImage, new Vector2(rect.width, rect.height), Rot4.South, default(Vector3), 1f, true, true, true, true, null, null, false, null);
				GUI.DrawTexture(rect, image);
			}
			else
			{
				SZWidgets.Image(rect, texPath);
			}
			if (showFloatMenu)
			{
				SZWidgets.FloatMenuOnLabel<T>(new Rect(rect.x, rect.y - 20f, rect.width, 20f), lblColor, l, labelGetter, selected, action, "");
			}
			if (Widgets.ButtonInvisible(rect, true) && imgAction != null)
			{
				imgAction();
			}
		}

		
		internal static List<FloatMenuOption> FloatMenuOnRect<T>(ICollection<T> l, Func<T, string> labelGetter, Action<T> action, Selected s = null, bool doWindow = true)
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			if (!l.EnumerableNullOrEmpty<T>())
			{
				using (IEnumerator<T> enumerator = l.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						T element = enumerator.Current;
						FloatMenuOption floatMenuOption = new FloatMenuOption(labelGetter(element), delegate()
						{
							if (action != null)
							{
								action(element);
							}
						}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
						if (element != null)
						{
							floatMenuOption.SetFMOIcon(element.GetTIcon(s));
							floatMenuOption.iconColor = element.GetTColor(null);
							floatMenuOption.tooltip = new TipSignal?(element.STooltip<T>());
						}
						list.Add(floatMenuOption);
					}
				}
				if (doWindow)
				{
					WindowTool.Open(new FloatMenu(list));
				}
			}
			return list;
		}

		
		internal static void FloatMixedMenuOnButtonImage<T1, T2>(Rect rect, Texture2D tex, List<T1> l1, List<T2> l2, Func<T1, string> labelGetter1, Func<T2, string> labelGetter2, Action<T1> action1, Action<T2> action2, string toolTip = "")
		{
			GUI.color = ((!Mouse.IsOver(rect)) ? Color.white : GenUI.MouseoverColor);
			GUI.DrawTexture(rect, tex);
			GUI.color = Color.white;
			if (Widgets.ButtonInvisible(rect, true))
			{
				List<FloatMenuOption> list = SZWidgets.FloatMenuOnRect<T1>(l1, labelGetter1, action1, null, false);
				list.AddRange(SZWidgets.FloatMenuOnRect<T2>(l2, labelGetter2, action2, null, false));
				WindowTool.Open(new FloatMenu(list));
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}

		
		internal static Texture2D IconForStuff(Selected s)
		{
			if (s == null || s.stuff == null)
			{
				return null;
			}
			return Widgets.GetIconFor(s.stuff, s.stuff, null, null);
		}

		
		internal static Texture2D IconForStyle(Selected s)
		{
			if (s == null || s.thingDef == null || s.stuff == null || s.style == null)
			{
				return null;
			}
			return Widgets.GetIconFor(s.thingDef, s.stuff, s.style, null);
		}

		
		internal static Texture2D IconForStyleCustom(Selected s, ThingStyleDef style)
		{
			if (s == null || s.thingDef == null || s.stuff == null || style == null)
			{
				return null;
			}
			return Widgets.GetIconFor(s.thingDef, s.stuff, style, null);
		}

		
		private static void SetFMOIcon(this FloatMenuOption fmo, Texture2D t)
		{
			if (t != null)
			{
				fmo.SetMemberValue("itemIcon", t);
			}
		}

		
		internal static void FlipTextureHorizontally(Texture2D original)
		{
			Color[] pixels = original.GetPixels();
			Color[] array = new Color[pixels.Length];
			int width = original.width;
			int height = original.height;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					array[i + j * width] = pixels[width - i - 1 + j * width];
				}
			}
			original.SetPixels(array);
			original.Apply();
		}

		
		internal static void FlipTextureVertically(Texture2D original)
		{
			Color[] pixels = original.GetPixels();
			Color[] array = new Color[pixels.Length];
			int width = original.width;
			int height = original.height;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					array[i + j * width] = pixels[i + (height - j - 1) * width];
				}
			}
			original.SetPixels(array);
			original.Apply();
		}

		
		internal static void ButtonImageTex(Rect rect, Texture2D tex, Action action)
		{
			if (!(tex == null) && Widgets.ButtonImage(rect, tex, true, null) && action != null)
			{
				action();
			}
		}

		
		internal static void ButtonImage(Rect rect, string texPath, Action action, string tooolTip = "")
		{
			if (!texPath.NullOrEmpty())
			{
				if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), true, null) && action != null)
				{
					action();
				}
				if (!tooolTip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, tooolTip);
				}
			}
		}

		
		internal static void ButtonImage(float x, float y, float w, float h, string texPath, Action action, string toolTip = "", Color col = default(Color))
		{
			if (!texPath.NullOrEmpty())
			{
				Rect rect = new Rect(x, y, w, h);
				bool flag;
				if (col == default(Color))
				{
					flag = Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), true, null);
				}
				else
				{
					flag = Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), col, true, null);
				}
				if (flag && action != null)
				{
					action();
				}
				if (!toolTip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, toolTip);
				}
			}
		}

		
		internal static void ButtonImageCol(Rect rect, string texPath, Action action, Color color, string toolTip = "")
		{
			if (!texPath.NullOrEmpty())
			{
				if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), color, true, null) && action != null)
				{
					action();
				}
				if (!toolTip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, toolTip);
				}
			}
		}

		
		internal static void ButtonHighlight(Rect rect, string texPath, Action<Color> action, Color color, string toolTip = "")
		{
			if (!texPath.NullOrEmpty())
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawBoxSolid(rect, new Color(color.r, color.g, color.b, 0.4f));
				}
				if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), color, color, true, null) && action != null)
				{
					action(color);
				}
				if (!toolTip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, toolTip);
				}
			}
		}

		
		internal static void ButtonHighlight(float x, float y, float w, float h, string texPath, Action<Color> action, Color color, string toolTip = "")
		{
			if (!texPath.NullOrEmpty())
			{
				Rect rect = new Rect(x, y, w, h);
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawBoxSolid(rect, new Color(color.r, color.g, color.b, 0.4f));
				}
				if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), color, color, true, null) && action != null)
				{
					action(color);
				}
				if (!toolTip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, toolTip);
				}
			}
		}

		
		internal static void ButtonImageCol(float x, float y, float w, float h, string texPath, Action<Color> action, Color color, string toolTip = "")
		{
			if (!texPath.NullOrEmpty())
			{
				Rect rect = new Rect(x, y, w, h);
				if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), color, true, null) && action != null)
				{
					action(color);
				}
				if (!toolTip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, toolTip);
				}
			}
		}

		
		internal static void ButtonImageCol2<T>(Rect rect, string texPath, Action<T> action, T value, Color color, string toolTip = "")
		{
			if (!texPath.NullOrEmpty())
			{
				if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), color, true, null) && action != null)
				{
					action(value);
				}
				if (!toolTip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, toolTip);
				}
			}
		}

		
		internal static void ButtonImageVar<T>(Rect rect, Texture2D tex, Action<T> action, T value, string toolTip = "")
		{
			if (!(tex == null))
			{
				if (Widgets.ButtonImage(rect, tex, true, null) && action != null)
				{
					action(value);
				}
				if (!toolTip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, toolTip);
				}
			}
		}

		
		internal static void ButtonImageVar<T>(Rect rect, string texPath, Action<T> action, T value, string toolTip = "")
		{
			if (!texPath.NullOrEmpty())
			{
				if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), true, null) && action != null)
				{
					action(value);
				}
				if (!toolTip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, toolTip);
				}
			}
		}

		
		internal static void ButtonImageVar<T>(float x, float y, float w, float h, string texPath, Action<T> action, T value, string toolTip = "")
		{
			if (!texPath.NullOrEmpty())
			{
				Rect rect = new Rect(x, y, w, h);
				if (Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), true, null) && action != null)
				{
					action(value);
				}
				if (!toolTip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, toolTip);
				}
			}
		}

		
		internal static void ButtonInvisible(Rect rect, Action action, string toolTip = "")
		{
			if (Widgets.ButtonInvisible(rect, true) && action != null)
			{
				action();
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}

		
		internal static void ButtonInvisibleMouseOver(Rect rect, Action action, string toolTip = "")
		{
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			if (Widgets.ButtonInvisible(rect, true) && action != null)
			{
				action();
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}

		
		internal static void ButtonInvisibleMouseOverVar<T>(Rect rect, Action<T> action, T val, string toolTip = "")
		{
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			if (Widgets.ButtonInvisible(rect, true) && action != null)
			{
				action(val);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}

		
		internal static void ButtonInvisibleVar<T>(Rect rect, Action<T> action, T value, string toolTip = "")
		{
			if (Widgets.ButtonInvisible(rect, true) && action != null)
			{
				action(value);
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}

		
		internal static void ButtonSolid(Rect rect, Color color, Action action, string tooltip = "")
		{
			Widgets.DrawRectFast(rect, color, null);
			SZWidgets.ButtonInvisible(rect, action, tooltip);
			GUI.color = Color.white;
		}

		
		internal static void ButtonText(Rect rect, string label, Action action, string toolTip = "")
		{
			if (Widgets.ButtonText(rect, label, true, true, true, null) && action != null)
			{
				action();
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}

		
		internal static void ButtonText(float x, float y, float w, float h, string label, Action action, string toolTip = "")
		{
			Rect rect = new Rect(x, y, w, h);
			if (Widgets.ButtonText(rect, label, true, true, true, null) && action != null)
			{
				action();
			}
			if (!toolTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, toolTip);
			}
		}

		
		internal static void ButtonTextureTextHighlight(Rect rect, string text, Texture2D icon, Color color, Action action, string toolTip = "")
		{
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			GUI.color = color;
			GUI.DrawTexture(new Rect(rect.x, rect.y, rect.height, rect.height), icon);
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
			float num = rect.height - 10f - rect.height / 2f;
			Widgets.Label(new Rect(rect.x + rect.height + 5f, rect.y + num, rect.width - rect.height - 5f, rect.height), text);
			SZWidgets.ButtonInvisible(rect, action, toolTip);
		}

		
		internal static void ButtonTextureTextHighlight2(Rect rect, string text, string texPath, Color color, Action action, string toolTip = "", bool withButton = true)
		{
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			if (texPath != null)
			{
				GUI.color = color;
				GUI.DrawTexture(new Rect(rect.x, rect.y, rect.height, rect.height), ContentFinder<Texture2D>.Get(texPath, true));
				GUI.color = Color.white;
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			float num = rect.height - 10f - rect.height / 2f;
			if (texPath == null)
			{
				Widgets.Label(rect, text);
			}
			else
			{
				Widgets.Label(new Rect(rect.x + rect.height + 5f, rect.y + num, rect.width - rect.height - 5f, rect.height), text);
			}
			Text.Anchor = TextAnchor.UpperLeft;
			if (withButton)
			{
				SZWidgets.ButtonInvisible(rect, action, toolTip);
			}
		}

		
		internal static void ButtonTextVar<T>(float x, float y, float w, float h, string label, Action<T> action, T value)
		{
			if (Widgets.ButtonText(new Rect(x, y, w, h), label, true, true, true, null) && action != null)
			{
				action(value);
			}
		}

		
		internal static void ButtonThingVar<T>(Rect rect, T val, Action<T> action, string tooltip)
		{
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			SZWidgets.ThingDrawer(rect, val as Thing);
			SZWidgets.ButtonInvisibleVar<T>(rect, action, val, tooltip);
		}

		
		internal static void CheckBoxOnChange(Rect rect, string label, bool checkState, Action<bool> action)
		{
			bool flag = checkState;
			Widgets.CheckboxLabeled(rect, label, ref checkState, false, null, null, false, false);
			if (flag != checkState && action != null)
			{
				action(checkState);
			}
		}

		
		internal static void ColorBox(Rect rect, Color col, Action<Color> action, bool halfAlfa = false)
		{
			Listing_X listing_X = new Listing_X();
			listing_X.Begin(rect);
			GUI.color = Color.red;
			float num = listing_X.Slider(col.r, 0f, (float)ColorTool.IMAX);
			GUI.color = Color.green;
			float num2 = listing_X.Slider(col.g, 0f, (float)ColorTool.IMAX);
			GUI.color = Color.blue;
			float num3 = listing_X.Slider(col.b, 0f, (float)ColorTool.IMAX);
			GUI.color = Color.white;
			float num4 = listing_X.Slider(col.a, halfAlfa ? 0.49f : 0f, (float)ColorTool.IMAX);
			bool flag = col.r != num || col.g != num2 || col.b != num3 || col.a != num4;
			listing_X.End();
			if (flag && action != null)
			{
				action(new Color(num, num2, num3, num4));
			}
		}

		
		internal static void Image(Rect rect, string texPath)
		{
			GUI.DrawTexture(rect, ContentFinder<Texture2D>.Get(texPath, true));
		}

		
		internal static void Image(Rect rect, Texture2D tex)
		{
			GUI.DrawTexture(rect, tex);
		}

		
		internal static void LabelEdit(Rect rect, int id, string text, ref string value, GameFont font, bool capitalize = false)
		{
			Text.Font = font;
			Widgets.DrawBoxSolid(rect, ColorTool.colAsche);
			if (SZWidgets.iLabelId != id)
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				Rect rect2 = new Rect(rect);
				rect2.x += 3f;
				if (capitalize)
				{
					Widgets.Label(rect2, text.NullOrEmpty() ? value.CapitalizeFirst() : (text + " " + value.CapitalizeFirst()));
				}
				else
				{
					Widgets.Label(rect2, text.NullOrEmpty() ? value : (text + " " + value));
				}
				SZWidgets.ButtonInvisible(rect, delegate
				{
					SZWidgets.iLabelId = id;
				}, "");
				TooltipHandler.TipRegion(rect, value);
				return;
			}
			Rect rect3 = new Rect(rect);
			rect3.width = rect3.height;
			rect3.x = rect.width - rect3.height;
			SZWidgets.ButtonImage(rect3, "UI/Buttons/DragHash", delegate()
			{
				SZWidgets.iLabelId = -1;
			}, "");
			value = Widgets.TextField(rect, value, 256, CharacterEditor.Label.ValidNameRegex);
		}

		
		internal static void Label(Rect rect, string text, Action action = null, string tooltip = "")
		{
			if (text != null)
			{
				Widgets.Label(rect, text);
			}
			if (action != null)
			{
				SZWidgets.ButtonInvisible(rect, action, "");
			}
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
		}

		
		internal static void Label(float x, float y, float w, float h, string text, Action action = null)
		{
			Rect rect = new Rect(x, y, w, h);
			Widgets.Label(rect, text);
			if (action != null)
			{
				SZWidgets.ButtonInvisible(rect, action, "");
			}
		}

		
		internal static void LabelBackground(Rect rect, string text, Color col, int offset = 0, string tooltip = "", Color colText = default(Color))
		{
			Widgets.DrawBoxSolid(rect, col);
			if (text == null)
			{
				text = "";
			}
			Rect rect2 = new Rect(rect.x + 3f + (float)offset, rect.y, rect.width - 3f, rect.height);
			if (rect.height > 20f && text.Length <= 22)
			{
				rect2.y += (rect.height - 20f) / 2f;
			}
			if (colText != default(Color))
			{
				Color color = GUI.color;
				GUI.color = colText;
				Widgets.Label(rect2, text);
				GUI.color = color;
			}
			else
			{
				Widgets.Label(rect2, text);
			}
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect2, tooltip);
			}
		}

		
		internal static void LabelCol<T>(Rect rect, string text, Color col, Action<T> action, T value, string tooltip = "")
		{
			Color color = GUI.color;
			GUI.color = col;
			Widgets.Label(rect, text);
			GUI.color = color;
			if (action != null)
			{
				SZWidgets.ButtonInvisibleVar<T>(rect, action, value, tooltip);
			}
		}

		
		internal static void TraitListView(float x, float y, float w, float h, List<Trait> l, ref Vector2 scrollPos, int elemH, Action<Trait> onClick, Action<Trait> onRandom, Action<Trait> onPrev, Action<Trait> onNext, Func<Trait, string> Flabel, Func<Trait, string> Ftooltip)
		{
			if (!l.NullOrEmpty<Trait>())
			{
				Rect outRect = new Rect(x, y, w, h);
				float height = (float)(l.Count * elemH + 20);
				Rect rect = new Rect(0f, 0f, outRect.width - 16f, height);
				Widgets.BeginScrollView(outRect, ref scrollPos, rect, true);
				Rect rect2 = rect.ContractedBy(6f);
				rect2.height = height;
				Color green = Color.green;
				Listing_X listing_X = new Listing_X();
				rect2.width += 18f;
				listing_X.Begin(rect2);
				listing_X.DefSelectionLineHeight = (float)elemH;
				for (int i = 0; i < l.Count; i++)
				{
					if (listing_X.CurY + (float)elemH > scrollPos.y && listing_X.CurY - h < scrollPos.y)
					{
						Trait trait = l[i];
						Color colText = (trait.sourceGene != null) ? ColorTool.colSkyBlue : Color.white;
						Rect rect3 = new Rect(listing_X.CurX, listing_X.CurY, outRect.width - 16f, (float)elemH);
						string tooltip = "";
						if (Mouse.IsOver(rect3))
						{
							tooltip = Ftooltip(trait);
						}
						SZWidgets.NavSelectorVar<Trait>(rect3, trait, onClick, onRandom, onPrev, onNext, null, Flabel(trait), tooltip, null, colText);
					}
					listing_X.CurY += 25f;
				}
				listing_X.End();
				Widgets.EndScrollView();
			}
		}

		
		internal static void AToggleSearch()
		{
			SZWidgets.bToggleSearch = !SZWidgets.bToggleSearch;
		}

		
		internal static ICollection<T> CreateSearch<T>(float x, ref float y, float w, float h, ICollection<T> l, Func<T, string> labelGetter)
		{
			ICollection<T> collection = new List<T>();
			try
			{
				SZWidgets.sFind = Widgets.TextField(new Rect(x, y, w, h), SZWidgets.sFind, 256, null);
				bool flag = char.IsUpper((!SZWidgets.sFind.NullOrEmpty()) ? SZWidgets.sFind.First<char>() : ' ');
				string text = flag ? SZWidgets.sFind : SZWidgets.sFind.ToLower();
				foreach (T t in l)
				{
					if (text.NullOrEmpty())
					{
						collection.Add(t);
					}
					else if (flag)
					{
						if (labelGetter(t).StartsWith(text))
						{
							collection.Add(t);
						}
					}
					else
					{
						string text2 = labelGetter(t).ToLower();
						if (text2.StartsWith(text) || text2.Contains(text))
						{
							collection.Add(t);
						}
					}
				}
			}
			catch
			{
			}
			y += 4f;
			return collection;
		}

		
		internal static float GetGraphicH<T>()
		{
			bool flag = typeof(T) == typeof(AbilityDef);
			bool flag2 = typeof(T) == typeof(HairDef);
			bool flag3 = typeof(T) == typeof(BeardDef);
			bool flag4 = typeof(T) == typeof(ThingDef);
			bool flag5 = typeof(T) == typeof(GeneDef);
			bool flag6 = typeof(T) == typeof(Pawn);
			return (float)((flag || flag5 || flag2 || flag3 || flag4) ? 64 : (flag6 ? 90 : 0));
		}

		
		internal static void ListView<T>(float x, float y, float w, float h, ICollection<T> l, Func<T, string> labelGetter, Func<T, string> tooltipGetter, Func<T, T, bool> comparator, ref T selectedThing, ref Vector2 scrollPos, bool withRemove = false, Action<T> action = null, bool withSearch = true, bool drawSection = false, bool hasIcon = false, bool selectOnMouseOver = false)
		{
			if (l != null)
			{
				bool flag = typeof(T) == typeof(AbilityDef);
				bool isHair = typeof(T) == typeof(HairDef);
				bool isBeard = typeof(T) == typeof(BeardDef);
				bool flag2 = typeof(T) == typeof(ThingDef);
				bool flag3 = typeof(T) == typeof(GeneDef);
				bool flag4 = typeof(T) == typeof(Pawn);
				bool flag5 = typeof(T) == typeof(ScenPart);
				float num = (float)((flag || flag3 || flag2) ? 0 : ((flag4 || flag5) ? 22 : 32));
				float graphicH = SZWidgets.GetGraphicH<T>();
				float num2 = withSearch ? 25f : 0f;
				ICollection<T> collection = withSearch ? SZWidgets.CreateSearch<T>(x, ref y, w, num2, l, labelGetter) : l;
				float height = 10f + (float)collection.Count * (num + graphicH);
				Rect rect = new Rect(x, y + num2, w, h - num2);
				Rect rect2 = new Rect(0f, 0f, rect.width - 16f, height);
				if (drawSection)
				{
					Widgets.DrawMenuSection(rect);
				}
				Widgets.BeginScrollView(rect, ref scrollPos, rect2, true);
				Rect rect3 = rect2.ContractedBy(6f);
				rect3.height = height;
				Color selColor = drawSection ? Color.blue : Color.green;
				Listing_X listing_X = new Listing_X();
				rect3.width += 18f;
				listing_X.Begin(rect3);
				listing_X.DefSelectionLineHeight = num;
				try
				{
					Text.Font = GameFont.Small;
					Text.Anchor = TextAnchor.MiddleLeft;
					if (flag4)
					{
						using (IEnumerator<T> enumerator = collection.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								T t = enumerator.Current;
								Pawn pawn = t as Pawn;
								bool selected = comparator(selectedThing, t);
								string tooltip = tooltipGetter(t);
								if (listing_X.Selectable(pawn.GetPawnName(true), selected, tooltip, PortraitsCache.Get(pawn, new Vector2(128f, 180f), Rot4.South, default(Vector3), 1f, true, true, true, true, null, null, false, null), null, null, default(Vector2), false, num + graphicH, (pawn.Faction == null) ? Color.white : pawn.Faction.Color, ColorTool.colLightGray, true) == 1)
								{
									selectedThing = t;
									if (action != null)
									{
										action(selectedThing);
									}
									else
									{
										SoundDefOf.Mouseover_Category.PlayOneShotOnCamera(null);
									}
								}
							}
							goto IL_3EF;
						}
					}
					using (IEnumerator<T> enumerator2 = collection.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (listing_X.CurY + num + graphicH > scrollPos.y && listing_X.CurY - 700f < scrollPos.y)
							{
								string tooltip2 = tooltipGetter(enumerator2.Current);
								string name = labelGetter(enumerator2.Current);
								bool selected2 = comparator(enumerator2.Current, selectedThing);
								bool isWhite = true;
								if (listing_X.SelectableText<T>(name, flag2, flag, flag3, isHair, isBeard, selected2, tooltip2, withRemove, isWhite, enumerator2.Current, selColor, hasIcon, selectOnMouseOver))
								{
									selectedThing = enumerator2.Current;
									if (action != null)
									{
										action(selectedThing);
									}
								}
							}
							listing_X.CurY += num;
							listing_X.CurY += graphicH;
						}
					}
					IL_3EF:
					Text.Anchor = TextAnchor.UpperLeft;
				}
				catch
				{
				}
				listing_X.End();
				Widgets.EndScrollView();
			}
		}

		
		internal static void ListView<T>(Rect rect, ICollection<T> l, Func<T, string> labelGetter, Func<T, string> tooltipGetter, Func<T, T, bool> comparator, ref T selectedThing, ref Vector2 scrollPos, bool withRemove = false, Action<T> action = null, bool withSearch = true, bool drawSection = false, bool isHead = false, bool selectOnMouse = false)
		{
			SZWidgets.ListView<T>(rect.x, rect.y, rect.width, rect.height, l, labelGetter, tooltipGetter, comparator, ref selectedThing, ref scrollPos, withRemove, action, withSearch, drawSection, isHead, selectOnMouse);
		}

		
		internal static void FullListviewScenPart(Rect rect, List<ScenPart> l, bool withRemove, Action<ScenPart> removeAction, string shiftIcon, Action<ScenPart> onShift, bool showPosition, bool withSearch, ref Vector2 scrollPos, ref ScenPart selectedPart)
		{
			if (l != null)
			{
				float x = rect.x;
				float y = rect.y;
				float width = rect.width;
				float height = rect.height;
				float num = 32f;
				float num2 = withSearch ? 25f : 0f;
				List<ScenPart> list = withSearch ? SZWidgets.CreateSearch<ScenPart>(rect.x, ref y, width, num2, l, FLabel.ScenPartLabel).ToList<ScenPart>() : l;
				float height2 = 10f + (float)list.Count * num;
				Rect outRect = new Rect(x, y + num2, width, height - num2);
				Rect rect2 = new Rect(0f, 0f, outRect.width - 16f, height2);
				Widgets.BeginScrollView(outRect, ref scrollPos, rect2, true);
				Rect rect3 = rect2.ContractedBy(6f);
				rect3.height = height2;
				Color green = Color.green;
				Listing_X listing_X = new Listing_X();
				rect3.width += 18f;
				listing_X.Begin(rect3);
				listing_X.DefSelectionLineHeight = num;
				try
				{
					Text.Font = GameFont.Small;
					Text.Anchor = TextAnchor.MiddleLeft;
					using (List<ScenPart>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (listing_X.CurY + num > scrollPos.y && listing_X.CurY - 700f < scrollPos.y && listing_X.ListviewPart(width, num, enumerator.Current, enumerator.Current == selectedPart, withRemove, removeAction, shiftIcon, onShift, showPosition))
							{
								selectedPart = ((selectedPart != null) ? null : enumerator.Current);
							}
							listing_X.CurY += num;
						}
					}
					Text.Anchor = TextAnchor.UpperLeft;
				}
				catch
				{
				}
				listing_X.End();
				Widgets.EndScrollView();
			}
		}

		
		internal static float NavSelectorCount(Rect rect, Selected s, int max)
		{
			Text.Font = GameFont.Small;
			if (Widgets.ButtonImage(SZWidgets.RectPrevious(rect), ContentFinder<Texture2D>.Get("bbackward", true), true, null))
			{
				s.stackVal--;
				s.oldStackVal = s.stackVal;
				s.UpdateBuyPrice();
			}
			if (Widgets.ButtonImage(SZWidgets.RectNext(rect), ContentFinder<Texture2D>.Get("bforward", true), true, null))
			{
				s.stackVal++;
				s.oldStackVal = s.stackVal;
				s.UpdateBuyPrice();
			}
			SZWidgets.LabelBackground(SZWidgets.RectSolid(rect, true), CharacterEditor.Label.COUNT + s.stackVal.ToString(), ColorTool.colAsche, 0, "", default(Color));
			if (Widgets.ButtonImage(SZWidgets.RectToggle(rect), ContentFinder<Texture2D>.Get("UI/Buttons/DragHash", true), true, null))
			{
				SZWidgets.bCountOpen = !SZWidgets.bCountOpen;
			}
			int num = 0;
			if (SZWidgets.bCountOpen)
			{
				num++;
				s.stackVal = (int)Widgets.HorizontalSlider(SZWidgets.RectSlider(rect, num), (float)s.stackVal, 1f, (float)max, false, null, null, null, -1f);
				if (s.stackVal != s.oldStackVal)
				{
					s.oldStackVal = s.stackVal;
					s.UpdateBuyPrice();
				}
			}
			if (s.tempThing != null)
			{
				s.tempThing.stackCount = s.stackVal;
			}
			return rect.y + 27f + (float)(num * 26);
		}

		
		internal static void NavSelectorImageBox(Rect rect, Action onClicked, Action onRandom, Action onPrev, Action onNext, Action onTextureClick, Action onToggle, string label, string tipLabel = null, string tipRandom = null, string tipTexture = null, string texturePath = null, Color colTex = default(Color), string tipToggle = null)
		{
			if (onPrev != null)
			{
				SZWidgets.ButtonImage(SZWidgets.RectPrevious(rect), "bbackward", onPrev, "");
			}
			if (onNext != null)
			{
				SZWidgets.ButtonImage(SZWidgets.RectNext(rect), "bforward", onNext, "");
			}
			Text.Font = GameFont.Small;
			bool flag = texturePath != null;
			bool flag2 = onToggle != null;
			SZWidgets.LabelBackground(SZWidgets.RectSolid(rect, true), label, ColorTool.colAsche, flag ? 25 : 0, "", colTex);
			SZWidgets.ButtonImage(SZWidgets.RectTexture(rect), texturePath, onTextureClick, tipTexture);
			int num = CEditor.IsRandom ? 25 : 0;
			num += (flag2 ? 25 : 0);
			SZWidgets.ButtonInvisibleMouseOver(SZWidgets.RectOnClick(rect, flag, num, true), onClicked, tipLabel);
			if (CEditor.IsRandom && onRandom != null)
			{
				SZWidgets.ButtonImage(SZWidgets.RectRandom(rect), "brandom", onRandom, tipRandom);
				SZWidgets.ButtonImage(SZWidgets.RectToggleLeft(rect), flag2 ? "UI/Buttons/DragHash" : null, onToggle, tipToggle);
				return;
			}
			SZWidgets.ButtonImage(SZWidgets.RectToggle(rect), flag2 ? "UI/Buttons/DragHash" : null, onToggle, tipToggle);
		}

		
		internal static void NavSelectorVar<T>(Rect rect, T val, Action<T> onClick, Action<T> onRandom, Action<T> onPrev, Action<T> onNext, Action<T> onToggle, string label, string tooltip, string tipRandom, Color colText)
		{
			bool isRandom = CEditor.IsRandom;
			if (isRandom && onPrev != null)
			{
				SZWidgets.ButtonImageVar<T>(SZWidgets.RectPrevious(rect), "bbackward", onPrev, val, "");
			}
			if (isRandom && onNext != null)
			{
				SZWidgets.ButtonImageVar<T>(SZWidgets.RectNext(rect), "bforward", onNext, val, "");
			}
			Text.Font = GameFont.Small;
			bool flag = onToggle != null;
			int offset = CEditor.IsRandom ? 25 : 0;
			SZWidgets.LabelBackground(SZWidgets.RectSolid(rect, isRandom), label, ColorTool.colAsche, 0, "", colText);
			if (onClick != null)
			{
				SZWidgets.ButtonInvisibleMouseOverVar<T>(SZWidgets.RectOnClick(rect, false, offset, isRandom), onClick, val, tooltip);
			}
			if (CEditor.IsRandom)
			{
				SZWidgets.ButtonImageVar<T>(SZWidgets.RectRandom(rect), "brandom", onRandom, val, tipRandom);
				if (flag)
				{
					SZWidgets.ButtonImageVar<T>(SZWidgets.RectToggleLeft(rect), flag ? "UI/Buttons/DragHash" : null, onToggle, val, "");
					return;
				}
			}
			else if (flag)
			{
				SZWidgets.ButtonImageVar<T>(SZWidgets.RectToggle(rect), flag ? "UI/Buttons/DragHash" : null, onToggle, val, "");
			}
		}

		
		internal static void NavSelectorImageBox2<T>(Rect rect, T val, Action<T> onClicked, Action<T> onRandom, Action<T> onPrev, Action<T> onNext, Action<T> onTextureClick, Action<T> onToggle, string label, string tipLabel = null, string tipRandom = null, string tipTexture = null, string texturePath = null, Color colTex = default(Color))
		{
			if (onPrev != null)
			{
				SZWidgets.ButtonImageVar<T>(SZWidgets.RectPrevious(rect), "bbackward", onPrev, val, "");
			}
			if (onNext != null)
			{
				SZWidgets.ButtonImageVar<T>(SZWidgets.RectNext(rect), "bforward", onNext, val, "");
			}
			Text.Font = GameFont.Small;
			bool flag = val is Thing;
			bool flag2 = flag || texturePath != null;
			bool flag3 = onToggle != null;
			SZWidgets.LabelBackground(SZWidgets.RectSolid(rect, true), label, ColorTool.colAsche, flag2 ? 25 : 0, "", default(Color));
			int num = CEditor.IsRandom ? 25 : 0;
			num += (flag3 ? 25 : 0);
			if (flag)
			{
				SZWidgets.ButtonThingVar<T>(SZWidgets.RectTexture(rect), val, onTextureClick, tipTexture);
			}
			else
			{
				SZWidgets.ButtonImageVar<T>(SZWidgets.RectTexture(rect), texturePath, onTextureClick, val, tipTexture);
			}
			SZWidgets.ButtonInvisibleMouseOverVar<T>(SZWidgets.RectOnClick(rect, flag2, num, true), onClicked, val, tipLabel);
			if (CEditor.IsRandom)
			{
				SZWidgets.ButtonImageVar<T>(SZWidgets.RectRandom(rect), "brandom", onRandom, val, tipRandom);
				SZWidgets.ButtonImageVar<T>(SZWidgets.RectToggleLeft(rect), flag3 ? "UI/Buttons/DragHash" : null, onToggle, val, "");
				return;
			}
			SZWidgets.ButtonImageVar<T>(SZWidgets.RectToggle(rect), flag3 ? "UI/Buttons/DragHash" : null, onToggle, val, "");
		}

		
		internal static float NavSelectorQuality(Rect rect, Selected s, HashSet<QualityCategory> lOfQuality)
		{
			float result;
			if (s == null || !s.HasQuality)
			{
				result = rect.y;
			}
			else
			{
				if (Widgets.ButtonImage(SZWidgets.RectPrevious(rect), ContentFinder<Texture2D>.Get("bbackward", true), true, null))
				{
					s.quality = lOfQuality.NextOrPrevIndex(s.quality, false, false);
					s.UpdateBuyPrice();
				}
				if (Widgets.ButtonImage(SZWidgets.RectNext(rect), ContentFinder<Texture2D>.Get("bforward", true), true, null))
				{
					s.quality = lOfQuality.NextOrPrevIndex(s.quality, true, false);
					s.UpdateBuyPrice();
				}
				Text.Font = GameFont.Small;
				SZWidgets.LabelBackground(SZWidgets.RectSolid(rect, true), CharacterEditor.Label.QUALITY + ((QualityCategory)s.quality).GetLabel().CapitalizeFirst(), ColorTool.colAsche, 0, "", default(Color));
				if (Widgets.ButtonImage(SZWidgets.RectToggle(rect), ContentFinder<Texture2D>.Get("UI/Buttons/DragHash", true), true, null))
				{
					SZWidgets.bQualityOpen = !SZWidgets.bQualityOpen;
				}
				Rect rect2 = SZWidgets.RectClickableT(rect);
				if (Mouse.IsOver(rect2))
				{
					Widgets.DrawHighlight(rect2);
				}
				SZWidgets.FloatMenuOnButtonInvisible<QualityCategory>(rect2, lOfQuality, (QualityCategory q) => q.GetLabel(), delegate(QualityCategory q)
				{
					s.quality = (int)q;
					s.UpdateBuyPrice();
				}, "");
				int num = 0;
				if (SZWidgets.bQualityOpen)
				{
					num++;
					s.quality = (int)Widgets.HorizontalSlider(SZWidgets.RectSlider(rect, num), (float)s.quality, 0f, (float)(lOfQuality.Count - 1), false, null, null, null, -1f);
				}
				if (s.tempThing != null)
				{
					s.tempThing.SetQuality(s.quality);
				}
				result = rect.y + 27f + (float)(num * 26);
			}
			return result;
		}

		
		internal static float NavSelectorStuff(Rect rect, Selected s)
		{
			float result;
			if (s == null || s.thingDef == null)
			{
				result = rect.y;
			}
			else
			{
				bool madeFromStuff = s.thingDef.MadeFromStuff;
				if (!madeFromStuff)
				{
					result = rect.y;
				}
				else
				{
					Text.Font = GameFont.Small;
					if (Widgets.ButtonImage(SZWidgets.RectPrevious(rect), ContentFinder<Texture2D>.Get("bbackward", true), true, null) && madeFromStuff)
					{
						s.SetStuff(false, false);
					}
					if (Widgets.ButtonImage(SZWidgets.RectNext(rect), ContentFinder<Texture2D>.Get("bforward", true), true, null) && madeFromStuff)
					{
						s.SetStuff(true, false);
					}
					SZWidgets.LabelBackground(SZWidgets.RectSolid(rect, true), CharacterEditor.Label.STUFF + s.StuffLabelGetter(s.stuff), ColorTool.colAsche, madeFromStuff ? 25 : 0, "", default(Color));
					if (Widgets.ButtonImage(SZWidgets.RectToggle(rect), ContentFinder<Texture2D>.Get("UI/Buttons/DragHash", true), true, null))
					{
						SZWidgets.bStuffOpen = !SZWidgets.bStuffOpen;
					}
					if (madeFromStuff)
					{
						GUI.color = s.GetTColor(null);
						SZWidgets.FloatMenuOnButtonStuffOrStyle<ThingDef>(SZWidgets.RectTexture(rect), SZWidgets.RectClickableT(rect), s.lOfStuff, s.StuffLabelGetter, s, delegate(ThingDef stuff)
						{
							s.SetStuff(stuff);
						});
						GUI.color = Color.white;
					}
					if (s.tempThing != null)
					{
						s.tempThing.SetStuffDirect(s.stuff);
					}
					int num = 0;
					if (SZWidgets.bStuffOpen && madeFromStuff)
					{
						num++;
						s.stuffIndex = (int)Widgets.HorizontalSlider(SZWidgets.RectSlider(rect, num), (float)s.stuffIndex, 0f, (float)(s.lOfStuff.Count - 1), false, null, null, null, -1f);
						s.CheckSetStuff();
					}
					result = rect.y + 27f + (float)(num * 26);
				}
			}
			return result;
		}

		
		internal static float NavSelectorStyle(Rect rect, Selected s)
		{
			float result;
			if (s == null || s.thingDef == null)
			{
				result = rect.y;
			}
			else
			{
				bool flag = s.thingDef.CanBeStyled() && s.lOfStyle.Count > 1;
				if (!flag)
				{
					result = rect.y;
				}
				else
				{
					Text.Font = GameFont.Small;
					if (Widgets.ButtonImage(SZWidgets.RectPrevious(rect), ContentFinder<Texture2D>.Get("bbackward", true), true, null) && flag)
					{
						s.SetStyle(false, false);
					}
					if (Widgets.ButtonImage(SZWidgets.RectNext(rect), ContentFinder<Texture2D>.Get("bforward", true), true, null) && flag)
					{
						s.SetStyle(true, false);
					}
					string str = flag ? s.StyleLabelGetter(s.style) : "";
					SZWidgets.LabelBackground(SZWidgets.RectSolid(rect, true), CharacterEditor.Label.STYLE + str, ColorTool.colAsche, flag ? 25 : 0, "", default(Color));
					if (Widgets.ButtonImage(SZWidgets.RectToggle(rect), ContentFinder<Texture2D>.Get("UI/Buttons/DragHash", true), true, null))
					{
						SZWidgets.bStyleOpen = !SZWidgets.bStyleOpen;
					}
					if (flag)
					{
						SZWidgets.FloatMenuOnButtonStuffOrStyle<ThingStyleDef>(SZWidgets.RectTexture(rect), SZWidgets.RectClickableT(rect), s.lOfStyle, s.StyleLabelGetter, s, delegate(ThingStyleDef style)
						{
							s.SetStyle(style);
						});
					}
					int num = 0;
					if (SZWidgets.bStyleOpen && flag)
					{
						num++;
						s.styleIndex = (int)Widgets.HorizontalSlider(SZWidgets.RectSlider(rect, num), (float)s.styleIndex, 0f, (float)(s.lOfStyle.Count - 1), false, null, null, null, -1f);
						s.CheckSetStyle();
					}
					if (s.tempThing != null)
					{
						s.tempThing.SetStyleDef(s.style);
					}
					result = rect.y + 27f + (float)(num * 26);
				}
			}
			return result;
		}

		
		internal static float NumericFloatBox(Rect rect, float value, float min, float max)
		{
			return SZWidgets.NumericFloatBox(rect.x, rect.y, rect.width, rect.height, value, min, max);
		}

		
		internal static float NumericFloatBox(float x, float y, float w, float h, float value, float min, float max)
		{
			Rect butRect = new Rect(x, y, 25f, h);
			Rect rect = new Rect(x + 20f, y, w, h);
			Rect butRect2 = new Rect(x + w + 15f, y, 25f, h);
			if (Widgets.ButtonImage(butRect, ContentFinder<Texture2D>.Get("bbackward", true), true, null))
			{
				value -= 1f;
				value = (float)Math.Round((double)value, 2);
			}
			if (Widgets.ButtonImage(butRect2, ContentFinder<Texture2D>.Get("bforward", true), true, null))
			{
				value += 1f;
				value = (float)Math.Round((double)value, 2);
			}
			string text = value.ToString();
			if (text.EndsWith("."))
			{
				text += "0";
			}
			else if (!text.Contains("."))
			{
				text += ".0";
			}
			text = Widgets.TextField(rect, text, 32, null);
			if (text.EndsWith("."))
			{
				text += "0";
			}
			else if (!text.Contains("."))
			{
				text += ".0";
			}
			float num = 0f;
			if (float.TryParse(text, out num))
			{
				if (num < min)
				{
					value = min;
				}
				else if (num > max)
				{
					value = max;
				}
				else
				{
					value = num;
				}
			}
			return value;
		}

		
		internal static long NumericLongBox(Rect rect, long value, long min, long max)
		{
			return SZWidgets.NumericLongBox(rect.x, rect.y, rect.width, rect.height, value, min, max);
		}

		
		internal static long NumericLongBox(float x, float y, float w, float h, long value, long min, long max)
		{
			Rect butRect = new Rect(x, y, 25f, h);
			Rect rect = new Rect(x + 20f, y, w, h);
			Rect butRect2 = new Rect(x + w + 15f, y, 25f, h);
			if (Widgets.ButtonImage(butRect, ContentFinder<Texture2D>.Get("bbackward", true), true, null))
			{
				value -= 1L;
			}
			if (Widgets.ButtonImage(butRect2, ContentFinder<Texture2D>.Get("bforward", true), true, null))
			{
				value += 1L;
			}
			string text = value.ToString();
			text = Widgets.TextField(rect, text, 32, null);
			long num = 0L;
			if (long.TryParse(text, out num))
			{
				if (num < min)
				{
					value = min;
				}
				else if (num > max)
				{
					value = max;
				}
				else
				{
					value = num;
				}
			}
			return value;
		}

		
		internal static int NumericIntBox(Rect rect, int value, int min, int max)
		{
			return SZWidgets.NumericIntBox(rect.x, rect.y, rect.width, rect.height, value, min, max);
		}

		
		internal static int NumericIntBox(float x, float y, float w, float h, int value, int min, int max)
		{
			Rect butRect = new Rect(x, y, 25f, h);
			Rect rect = new Rect(x + 20f, y, w, h);
			Rect butRect2 = new Rect(x + w + 15f, y, 25f, h);
			if (Widgets.ButtonImage(butRect, ContentFinder<Texture2D>.Get("bbackward", true), true, null))
			{
				value--;
			}
			if (Widgets.ButtonImage(butRect2, ContentFinder<Texture2D>.Get("bforward", true), true, null))
			{
				value++;
			}
			string text = value.ToString();
			text = Widgets.TextField(rect, text, 32, null);
			int num = 0;
			if (int.TryParse(text, out num))
			{
				if (num < min)
				{
					value = min;
				}
				else if (num > max)
				{
					value = max;
				}
				else
				{
					value = num;
				}
			}
			return value;
		}

		
		internal static int NumericTextField(Rect rect, int value, int min, int max)
		{
			string text = value.ToString();
			text = Widgets.TextField(rect, text, 32, null);
			int num = 0;
			if (int.TryParse(text, out num))
			{
				if (num < min)
				{
					value = min;
				}
				else if (num > max)
				{
					value = max;
				}
				else
				{
					value = num;
				}
			}
			return value;
		}

		
		internal static int NumericTextField(float x, float y, float w, float h, int value, int min, int max)
		{
			Rect rect = new Rect(x + 20f, y, w, h);
			string text = value.ToString();
			text = Widgets.TextField(rect, text, 32, null);
			int num = 0;
			if (int.TryParse(text, out num))
			{
				if (num < min)
				{
					value = min;
				}
				else if (num > max)
				{
					value = max;
				}
				else
				{
					value = num;
				}
			}
			return value;
		}

		
		internal static void ScrollView(int x, int y, int w, int h, int objCount, int objH, ref Vector2 scrollPos, Action<Listing_X> drawFunction)
		{
			Rect outRect = new Rect((float)x, (float)y, (float)w, (float)h);
			Rect rect = new Rect(0f, (float)y, outRect.width - 16f, (float)(objCount * objH));
			Widgets.BeginScrollView(outRect, ref scrollPos, rect, true);
			Rect rect2 = rect.ContractedBy(4f);
			rect2.height = (float)(objCount * objH);
			Listing_X listing_X = new Listing_X();
			listing_X.Begin(rect2);
			drawFunction(listing_X);
			listing_X.End();
			Widgets.EndScrollView();
		}

		
		internal static void SimpleMultiplierSlider(Rect rect, string label, string format, bool showNumeric, float baseValue, ref float currentVal, float min, float max)
		{
			Listing_X listing_X = new Listing_X();
			listing_X.Begin(rect);
			string text = showNumeric ? label : "";
			listing_X.AddMultiplierSection(label, format, ref text, baseValue, ref currentVal, min, max, true);
			listing_X.End();
		}

		
		internal static void SimpleSlider(Rect rect, string label, ref float currentVal, float min, float max)
		{
			Listing_X listing_X = new Listing_X();
			listing_X.Begin(rect);
			string text = label;
			listing_X.AddSection(label, "", ref text, ref currentVal, min, max, true, "");
			listing_X.End();
		}

		
		internal static void SingleSlinder(Rect rect, float currentVal, float min, float max, Action<float> action)
		{
			Listing_X listing_X = new Listing_X();
			listing_X.Begin(rect);
			float num = listing_X.Slider(currentVal, min, max);
			bool flag = num != currentVal;
			listing_X.End();
			if (flag && action != null)
			{
				action(num);
			}
		}

		
		internal static string TextArea(Rect rect, string text, int max, Regex regex)
		{
			if (text == null)
			{
				text = "";
			}
			string text2 = GUI.TextArea(rect, text, max, Text.CurTextAreaStyle);
			string result;
			if (text2.Length <= max && regex != null && regex.IsMatch(text2))
			{
				result = text2;
			}
			else
			{
				result = text;
			}
			return result;
		}

		
		internal static void ThingDrawer(Rect rect, Thing t)
		{
			Widgets.ThingIcon(rect, t, 1f, null, false);
			GUI.color = Color.white;
		}

		
		private static Rect RectClickableT(Rect rect)
		{
			return new Rect(rect.x + 21f, rect.y, rect.width - 64f, 24f);
		}

		
		private static Rect RectNext(Rect rect)
		{
			return new Rect(rect.x + rect.width - 22f, rect.y + 2f, 22f, 22f);
		}

		
		private static Rect RectOnClick(Rect rect, bool hasTexture, int offset = 0, bool showEdit = true)
		{
			return new Rect(rect.x + (float)(showEdit ? 21 : 0) + (float)(hasTexture ? 25 : 0), rect.y, rect.width - (float)(showEdit ? 40 : 19) - (float)(hasTexture ? 25 : 0) - (float)offset, 24f);
		}

		
		private static Rect RectPrevious(Rect rect)
		{
			return new Rect(rect.x, rect.y + 2f, 22f, 22f);
		}

		
		private static Rect RectRandom(Rect rect)
		{
			return new Rect(rect.x + rect.width - 42f, rect.y, 22f, 22f);
		}

		
		private static Rect RectSlider(Rect rect, int i)
		{
			return new Rect(rect.x, rect.y + 10f + (float)i * rect.height, rect.width, rect.height);
		}

		
		private static Rect RectSolid(Rect rect, bool showEdit = true)
		{
			return new Rect(rect.x + (float)(showEdit ? 21 : 0), rect.y, rect.width - (float)(showEdit ? 40 : 19), 24f);
		}

		
		private static Rect RectTexture(Rect rect)
		{
			return new Rect(rect.x + 25f, rect.y, 24f, 24f);
		}

		
		private static Rect RectToggle(Rect rect)
		{
			return new Rect(rect.x + rect.width - 42f, rect.y, 22f, 22f);
		}

		
		private static Rect RectToggleLeft(Rect rect)
		{
			return new Rect(rect.x + rect.width - 67f, rect.y, 22f, 22f);
		}

		
		private static void CheckAddTempTextToList(ref List<string> l)
		{
			if (SZWidgets.iLabelId != SZWidgets.iTempTextID)
			{
				if (!SZWidgets.tempText.NullOrEmpty())
				{
					if (l == null)
					{
						l = new List<string>();
					}
					l.Add(SZWidgets.tempText);
				}
				SZWidgets.tempText = "";
				SZWidgets.iShowId = 0;
				SZWidgets.iLabelId = -1;
			}
		}

		
		private static void CheckAddTempTextToFList(ref List<float> l)
		{
			if (SZWidgets.iLabelId != SZWidgets.iTempTextID)
			{
				float item;
				if (float.TryParse(SZWidgets.tempText, out item))
				{
					if (l == null)
					{
						l = new List<float>();
					}
					l.Add(item);
				}
				SZWidgets.tempText = "";
				SZWidgets.iShowId = 0;
				SZWidgets.iLabelId = -1;
			}
		}

		
		internal static void ActivateLabelEdit(int id)
		{
			SZWidgets.iShowId = id;
			SZWidgets.iLabelId = SZWidgets.iTempTextID;
		}

		
		internal static void AddLabelEditToList(Listing_X view, int id, ref List<string> l, Action action)
		{
			if (SZWidgets.iShowId == id)
			{
				if (action != null)
				{
					action();
				}
				SZWidgets.LabelEdit(view.GetRect(22f, 1f), SZWidgets.iTempTextID, "", ref SZWidgets.tempText, GameFont.Small, false);
				SZWidgets.CheckAddTempTextToList(ref l);
			}
		}

		
		internal static void AddLabelEditToList(Listing_X view, int id, ref List<float> l, Action action)
		{
			if (SZWidgets.iShowId == id)
			{
				if (action != null)
				{
					action();
				}
				SZWidgets.LabelEdit(view.GetRect(22f, 1f), SZWidgets.iTempTextID, "", ref SZWidgets.tempText, GameFont.Small, false);
				SZWidgets.CheckAddTempTextToFList(ref l);
			}
		}

		
		
		internal static Color RemoveColor
		{
			get
			{
				if (!SZWidgets.bRemoveOnClick)
				{
					return Color.white;
				}
				return Color.red;
			}
		}

		
		internal static void ToggleRemove()
		{
			SZWidgets.bRemoveOnClick = !SZWidgets.bRemoveOnClick;
		}

		
		internal static void DrawTagFilter(ref TagFilter t, List<string> lSamples, int w, Listing_X view, string title, ref List<string> copyList, Action<string> remove, Action<string> add)
		{
			view.Label(0f, 0f, (float)(w - 28), 30f, title, GameFont.Medium, "");
			view.FloatMenuOnButtonImage<string>((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", lSamples, (string s) => s, add);
			view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(SZWidgets.ToggleRemove), new Color?(SZWidgets.RemoveColor));
			if (view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", null, null) && t != null)
			{
				t.tags.CopyList(ref copyList);
			}
			if (!copyList.NullOrEmpty<string>() && view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", null, null))
			{
				if (t == null)
				{
					t = new TagFilter();
				}
				t.tags.PasteList(copyList);
			}
			view.Gap(30f);
			if (t != null)
			{
				view.FullListViewString(w - 28, t.tags, SZWidgets.bRemoveOnClick, remove);
			}
			view.GapLine(25f);
		}

		
		internal static void DrawStringList(ref List<string> l, List<string> lSamples, int w, Listing_X view, string title, ref List<string> copyList, Action<string> remove, Action<string> add)
		{
			view.Label(0f, 0f, (float)(w - 28), 30f, title, GameFont.Medium, "");
			view.FloatMenuOnButtonImage<string>((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", lSamples, (string s) => s, add);
			view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(SZWidgets.ToggleRemove), new Color?(SZWidgets.RemoveColor));
			if (view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", null, null))
			{
				l.CopyList(ref copyList);
			}
			if (!copyList.NullOrEmpty<string>() && view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", null, null))
			{
				if (l.NullOrEmpty<string>())
				{
					l = new List<string>();
				}
				l.PasteList(copyList);
			}
			view.Gap(30f);
			view.FullListViewString(w - 28, l, SZWidgets.bRemoveOnClick, remove);
			view.GapLine(25f);
		}

		
		internal static void DrawStringListCustom(ref List<string> l, int id, int w, Listing_X view, string title, ref List<string> copyList, Action<string> remove, Action actionBeforeAdding = null)
		{
			view.Label(0f, 0f, (float)(w - 28), 30f, title, GameFont.Medium, "");
			view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate()
			{
				SZWidgets.ActivateLabelEdit(id);
			}, null);
			view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(SZWidgets.ToggleRemove), new Color?(SZWidgets.RemoveColor));
			if (view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", null, null))
			{
				l.CopyList(ref copyList);
			}
			if (!copyList.NullOrEmpty<string>() && view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", null, null))
			{
				if (l.NullOrEmpty<string>())
				{
					l = new List<string>();
				}
				l.PasteList(copyList);
			}
			view.Gap(30f);
			SZWidgets.AddLabelEditToList(view, id, ref l, actionBeforeAdding);
			view.FullListViewString(w - 28, l, SZWidgets.bRemoveOnClick, remove);
			view.GapLine(25f);
		}

		
		static SZWidgets()
		{
		}

		
		private static int iUID = -1;

		
		internal static bool bCountOpen = false;

		
		internal static bool bQualityOpen = false;

		
		internal static bool bStuffOpen = false;

		
		internal static bool bStyleOpen = false;

		
		internal static List<string> lSimilar = new List<string>();

		
		internal static string sFind = "";

		
		internal static string sFindOld = "";

		
		internal static string sFindTemp = "";

		
		internal static string tDefName = null;

		
		internal static int waitTimer = 0;

		
		internal static int iLabelId = -1;

		
		internal static bool bToggleSearch = false;

		
		internal static bool bFocusOnce = true;

		
		private static int iShowId = 0;

		
		private static string tempText = "";

		
		private static int iTempTextID = 1000;

		
		internal static bool bRemoveOnClick = false;
        
	}
}
