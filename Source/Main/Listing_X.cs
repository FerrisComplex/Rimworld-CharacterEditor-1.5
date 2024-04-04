using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor
{
    internal class Listing_X : Listing
    {
        internal Listing_X(GameFont font)
        {
            this.font = font;
        }


        internal Listing_X()
        {
            this.font = GameFont.Small;
        }
        
        internal float CurY
        {
            get { return this.curY; }
            set { this.curY = value; }
        }

        
        internal float CurX
        {
            get { return this.curX; }
            set { this.curX = value; }
        }


        public override void Begin(Rect rect)
        {
            base.Begin(rect);
            Text.Font = this.font;
            this.texRemove = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
        }


        internal Listing_Standard BeginSection(float height)
        {
            Rect rect = base.GetRect(height + 8f, 1f);
            Widgets.DrawMenuSection(rect);
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(rect.ContractedBy(4f));
            return listing_Standard;
        }


        internal void FloatMenuOnButtonImage<T>(float xOff, float yOff, float w, float h, string texPath, List<T> l, Func<T, string> labelGetter, Action<T> action)
        {
            if (Widgets.ButtonImage(new Rect(this.curX + xOff, this.curY + yOff, w, h), ContentFinder<Texture2D>.Get(texPath, true), true, null))
            {
                SZWidgets.FloatMenuOnRect<T>(l, labelGetter, action, null, true);
            }
        }


        internal bool ButtonImage(float xOff, float yOff, float w, float h, string texPath, Action action, Color? color = null)
        {
            bool flag = Widgets.ButtonImage(new Rect(this.curX + xOff, this.curY + yOff, w, h), ContentFinder<Texture2D>.Get(texPath, true), color ?? Color.white, true, null);
            if (flag && action != null)
            {
                action();
            }

            return flag;
        }


        internal bool ButtonImage<T>(float xOff, float yOff, float w, float h, string texPath, Color color, Action<T> action, T val, string toolTip = "")
        {
            Rect rect = new Rect(this.curX + xOff, this.curY + yOff, w, h);
            bool flag = Widgets.ButtonImage(rect, ContentFinder<Texture2D>.Get(texPath, true), color, true, null);
            if (flag && action != null)
            {
                action(val);
            }

            if (!toolTip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(rect, toolTip);
            }

            return flag;
        }


        internal bool ButtonImage(Texture2D tex, float xOffset, float width, float height, Action action)
        {
            base.NewColumnIfNeeded(height);
            bool flag = Widgets.ButtonImage(new Rect(this.curX + xOffset, this.curY, width, height), tex, true, null);
            base.Gap(height + this.verticalSpacing);
            if (flag && action != null)
            {
                action();
            }

            return flag;
        }


        internal bool ButtonText(string label, float x, float y, float w, float h, Action action, string highlightTag = null)
        {
            Rect rect = new Rect(x, y, w, h);
            bool flag = Widgets.ButtonText(rect, label, true, false, true, null);
            if (highlightTag != null)
            {
                UIHighlighter.HighlightOpportunity(rect, highlightTag);
            }

            if (flag && action != null)
            {
                action();
            }

            return flag;
        }


        internal bool ButtonText(string label, string highlightTag = null)
        {
            Rect rect = base.GetRect(30f, 1f);
            bool result = Widgets.ButtonText(rect, label, true, false, true, null);
            if (highlightTag != null)
            {
                UIHighlighter.HighlightOpportunity(rect, highlightTag);
            }

            base.Gap(this.verticalSpacing);
            return result;
        }


        internal bool ButtonTextLabeled(string label, string buttonLabel)
        {
            Rect rect = base.GetRect(30f, 1f);
            Widgets.Label(rect.LeftHalf(), label);
            bool result = Widgets.ButtonText(rect.RightHalf(), buttonLabel, true, false, true, null);
            base.Gap(this.verticalSpacing);
            return result;
        }


        internal void CheckboxLabeledWithDefault(string label, float xOff, float width, ref bool checkOn, bool defaultVal, string tooltip = null)
        {
            Rect rect = this.BaseCheckboxLabeled(label, xOff, width - 24f, ref checkOn, tooltip, false);
            Rect rect2 = new Rect(rect.x + rect.width, rect.y, 24f, 24f);
            if (Widgets.ButtonImage(rect2, ContentFinder<Texture2D>.Get("bdefault", true), true, null))
            {
                checkOn = defaultVal;
            }

            TooltipHandler.TipRegion(rect2, CharacterEditor.Label.O_SETTODEFAULT);
            base.Gap(this.verticalSpacing);
        }


        internal void LabelEdit(int id, string text, ref string value, GameFont font)
        {
            SZWidgets.LabelEdit(base.GetRect(22f, 1f), id, text, ref value, font, false);
            base.Gap(4f);
        }


        internal void CheckboxLabeledNoGap(string label, float xOff, float width, ref bool checkOn)
        {
            this.BaseCheckboxLabeled(label, xOff, width, ref checkOn, null, false);
        }


        internal void CheckboxLabeled(string label, float xOff, float width, ref bool checkOn, string tooltip = null, int gap = -1)
        {
            this.BaseCheckboxLabeled(label, xOff, width, ref checkOn, tooltip, false);
            if (gap < 0)
            {
                base.Gap(this.verticalSpacing);
                return;
            }

            base.Gap((float)gap);
        }


        private Rect BaseCheckboxLabeled(string label, float xOff, float width, ref bool checkOn, string tooltip = null, bool nearText = false)
        {
            float lineHeight = Text.LineHeight;
            Rect rect = base.GetRect(lineHeight, 1f);
            rect.width = width;
            rect.x += xOff;
            Widgets.DrawBoxSolid(rect, ColorTool.colAsche);
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }

            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }

            Widgets.CheckboxLabeled(rect, label, ref checkOn, false, null, null, nearText, false);
            return rect;
        }


        internal bool CheckboxLabeledSelectable(string label, ref bool selected, ref bool checkOn)
        {
            float lineHeight = Text.LineHeight;
            bool result = Widgets.CheckboxLabeledSelectable(base.GetRect(lineHeight, 1f), label, ref selected, ref checkOn, null, 1f);
            base.Gap(this.verticalSpacing);
            return result;
        }


        public override void End()
        {
            base.End();
            if (this.labelScrollbarPositions != null)
            {
                for (int i = this.labelScrollbarPositions.Count - 1; i >= 0; i--)
                {
                    if (!this.labelScrollbarPositionsSetThisFrame.Contains(this.labelScrollbarPositions[i].First))
                    {
                        this.labelScrollbarPositions.RemoveAt(i);
                    }
                }

                this.labelScrollbarPositionsSetThisFrame.Clear();
            }
        }


        internal void EndSection(Listing_Standard listing)
        {
            listing.End();
        }


        internal void FloatMenuButtonWithLabelDef<T>(string label, float wLabel, float wDropbox, string currentVal, ICollection<T> l, Func<T, string> labelGetter, Action<T> action, float gap = -1f) where T : Def
        {
            Rect rect = new Rect(this.curX, this.curY + 4f, wLabel, 30f);
            Rect rect2 = new Rect(this.curX + wLabel, this.curY, wDropbox, 30f);
            Widgets.Label(rect, label);
            if (!l.EnumerableNullOrEmpty<T>())
            {
                SZWidgets.FloatMenuOnButtonText<T>(rect2, currentVal, l, labelGetter, action, "");
            }

            if (gap == -1f)
            {
                base.Gap(this.verticalSpacing);
                return;
            }

            base.Gap(gap);
        }


        internal void FloatMenuButtonWithLabel<T>(string label, float wLabel, float wDropbox, string currentVal, List<T> l, Func<T, string> labelGetter, Action<T> action, float gap = -1f)
        {
            Rect rect = new Rect(this.curX, this.curY + 4f, wLabel, 30f);
            Rect rect2 = new Rect(this.curX + wLabel, this.curY, wDropbox, 30f);
            Widgets.Label(rect, label);
            if (!l.NullOrEmpty<T>())
            {
                SZWidgets.FloatMenuOnButtonText<T>(rect2, currentVal, l, labelGetter, action, "");
            }

            if (gap == -1f)
            {
                base.Gap(this.verticalSpacing);
                return;
            }

            base.Gap(gap);
        }


        internal void IntAdjuster(ref int val, int countChange, int min = 0)
        {
            Rect rect = base.GetRect(24f, 1f);
            rect.width = 42f;
            if (Widgets.ButtonText(rect, "-" + countChange.ToString(), true, false, true, null))
            {
                val -= countChange * GenUI.CurrentAdjustmentMultiplier();
                if (val < min)
                {
                    val = min;
                }
            }

            rect.x += rect.width + 2f;
            if (Widgets.ButtonText(rect, "+" + countChange.ToString(), true, false, true, null))
            {
                val += countChange * GenUI.CurrentAdjustmentMultiplier();
                if (val < min)
                {
                    val = min;
                }
            }

            base.Gap(this.verticalSpacing);
        }


        internal void IntEntry(ref int val, ref string editBuffer, int multiplier = 1)
        {
            Widgets.IntEntry(base.GetRect(24f, 1f), ref val, ref editBuffer, multiplier);
            base.Gap(this.verticalSpacing);
        }


        internal void IntRange(ref IntRange range, int min, int max)
        {
            Widgets.IntRange(base.GetRect(28f, 1f), (int)base.CurHeight, ref range, min, max, null, 0);
            base.Gap(this.verticalSpacing);
        }


        internal void IntSetter(ref int val, int target, string label)
        {
            if (Widgets.ButtonText(base.GetRect(24f, 1f), label, true, false, true, null))
            {
                val = target;
            }

            base.Gap(this.verticalSpacing);
        }


        internal void Label(float xOff, float yOff, float w, float h, string text, GameFont font = GameFont.Small, string tooltip = "")
        {
            Text.Font = font;
            Rect rect = new Rect(this.curX + xOff, this.curY + yOff, w, h);
            Widgets.Label(rect, text);
            Text.Font = GameFont.Small;
            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }
        }


        internal void LabelSimple(string label, float x, float y, float w, float h, string tooltip = null)
        {
            Rect rect = new Rect(x, y, w, h);
            Widgets.Label(rect, label);
            if (tooltip != null)
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }
        }


        internal void Label(string text, float width = -1f, float yGap = -1f, float maxHeight = -1f, string tooltip = null)
        {
            float num = Text.CalcHeight(text, base.ColumnWidth);
            bool flag = false;
            if (maxHeight >= 0f && num > maxHeight)
            {
                num = maxHeight;
                flag = true;
            }

            Rect rect = base.GetRect(num, 1f);
            if (width >= 0f)
            {
                rect.width = width;
            }

            if (flag)
            {
                Vector2 labelScrollbarPosition = this.GetLabelScrollbarPosition(this.curX, this.curY);
                Widgets.LabelScrollable(rect, text, ref labelScrollbarPosition, false, true, false);
                this.SetLabelScrollbarPosition(this.curX, this.curY, labelScrollbarPosition);
            }
            else
            {
                Widgets.Label(rect, text);
            }

            if (tooltip != null)
            {
                TooltipHandler.TipRegion(rect, tooltip);
            }

            if (yGap == -1f)
            {
                base.Gap(this.verticalSpacing);
                return;
            }

            base.Gap(yGap);
        }


        internal bool Listview(float width, string defName, string name, string tooltip, bool withRemove, Action<string> action)
        {
            bool result;
            if (name == null)
            {
                result = false;
            }
            else
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect rect = new Rect(this.curX, this.curY, width - this.DefSelectionLineHeight, this.DefSelectionLineHeight);
                Text.WordWrap = false;
                Widgets.Label(rect, name);
                Text.WordWrap = true;
                if (!string.IsNullOrEmpty(tooltip))
                {
                    TipSignal tip = new TipSignal(() => tooltip, 275);
                    TooltipHandler.TipRegion(rect, tip);
                }

                Text.Anchor = TextAnchor.UpperLeft;
                this.curY += this.DefSelectionLineHeight;
                if (withRemove)
                {
                    bool flag = Widgets.ButtonImage(new Rect(rect.x + rect.width, rect.y, this.DefSelectionLineHeight, this.DefSelectionLineHeight), this.texRemove, Color.grey, true, null);
                    if (flag && action != null)
                    {
                        action(defName);
                    }

                    GUI.color = Color.white;
                    result = flag;
                }
                else
                {
                    result = Widgets.ButtonInvisible(rect, true);
                }
            }

            return result;
        }


        internal bool ListviewTDC(float width, ThingDefCountClass tdc, bool selected, bool withRemove, Action<string> action, ThingDef t)
        {
            bool result;
            if (tdc == null || tdc.thingDef == null)
            {
                result = false;
            }
            else
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect rect = new Rect(this.curX, this.curY, width - this.DefSelectionLineHeight, this.DefSelectionLineHeight);
                Text.WordWrap = false;
                Widgets.Label(rect, tdc.thingDef.label + ": " + tdc.count.ToString());
                Text.WordWrap = true;
                if (selected)
                {
                    this.curY += this.DefSelectionLineHeight;
                    int value = tdc.count;
                    value = SZWidgets.NumericIntBox(this.curX, this.curY, 70f, 30f, value, 0, 10000);
                    t.UpdateCost(tdc.thingDef, value);
                    this.curY += this.DefSelectionLineHeight;
                }

                if (!string.IsNullOrEmpty(tdc.thingDef.description))
                {
                    TipSignal tip = new TipSignal(() => tdc.thingDef.description, 21275);
                    TooltipHandler.TipRegion(rect, tip);
                }

                Text.Anchor = TextAnchor.UpperLeft;
                this.curY += this.DefSelectionLineHeight;
                if (withRemove)
                {
                    if (Widgets.ButtonImage(new Rect(rect.x + rect.width, rect.y, this.DefSelectionLineHeight, this.DefSelectionLineHeight), this.texRemove, Color.grey, true, null) && action != null)
                    {
                        action(tdc.thingDef.defName);
                    }

                    GUI.color = Color.white;
                }

                result = Widgets.ButtonInvisible(rect, true);
            }

            return result;
        }


        internal void GapLineCustom(float gapVorLinie, float gapNachLinie)
        {
            float y = this.curY + gapVorLinie / 2f;
            Color color = GUI.color;
            GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
            Widgets.DrawLineHorizontal(this.curX, y, base.ColumnWidth);
            GUI.color = color;
            this.curY += gapVorLinie / 2f + gapNachLinie;
        }


        internal bool ListviewPart(float width, float itemH, ScenPart part, bool selected, bool removeActive, Action<ScenPart> action, string shiftIcon, Action<ScenPart> onShift, bool showPosition = false)
        {
            bool result;
            if (!part.IsSupportedScenarioPart())
            {
                result = false;
            }
            else
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect rect = new Rect(this.curX, this.curY, width - itemH + 7f, itemH);
                Rect rect2 = new Rect(this.curX, this.curY, itemH, itemH);
                Rect rect3 = new Rect(this.curX + width - itemH - itemH, this.curY, itemH, itemH);
                Rect rect4 = new Rect(this.curX + 5f + itemH, this.curY, width, itemH);
                Rect rect5 = new Rect(this.curX, this.curY, width - itemH - itemH, itemH);
                Widgets.DrawBoxSolid(rect, this.alternate ? new Color(0.3f, 0.3f, 0.3f, 0.5f) : new Color(0.2f, 0.2f, 0.2f, 0.5f));
                this.alternate = !this.alternate;
                Text.WordWrap = false;
                Selected selectedScenarioPart = part.GetSelectedScenarioPart<ScenPart>();
                if (part.IsScenarioAnimal())
                {
                    if (selectedScenarioPart.pkd != null)
                    {
                        Widgets.ThingIcon(rect2, selectedScenarioPart.pkd.race, null, null, 1f, null, null);
                        Widgets.Label(rect4, FLabel.PawnKindWithGenderAndAge(selectedScenarioPart));
                    }
                    else
                    {
                        Widgets.Label(rect, part.Label + ": " + selectedScenarioPart.stackVal.ToString());
                    }
                }
                else if (selectedScenarioPart.thingDef != null)
                {
                    GUI.color = selectedScenarioPart.GetTColor(null);
                    GUI.DrawTexture(rect2, selectedScenarioPart.GetTexture2D, ScaleMode.ScaleAndCrop, true);
                    GUI.color = Color.white;
                    Widgets.Label(rect4, FLabel.ThingLabel(selectedScenarioPart));
                }

                Text.WordWrap = true;
                if (selected)
                {
                    if (showPosition)
                    {
                        try
                        {
                            Reflect.GetAType("Verse", "CameraJumper").CallMethod("JumpLocalInternal", new object[]
                            {
                                selectedScenarioPart.location,
                                CameraJumper.MovementMode.Pan
                            });
                            goto IL_285;
                        }
                        catch
                        {
                            goto IL_285;
                        }
                    }

                    selectedScenarioPart.oldStackVal = selectedScenarioPart.stackVal;
                    selectedScenarioPart.stackVal = SZWidgets.NumericIntBox(this.curX + width - 140f - (float)(removeActive ? 25 : 0), this.curY + 2f, 70f, 26f, selectedScenarioPart.stackVal, 1, 20000);
                    if (selectedScenarioPart.stackVal != selectedScenarioPart.oldStackVal)
                    {
                        part.SetScenarioPartCount(selectedScenarioPart.stackVal);
                    }
                }

                IL_285:
                if (Mouse.IsOver(rect5))
                {
                    TooltipHandler.TipRegion(rect5, selectedScenarioPart.thingDef.STooltip<ThingDef>());
                }

                Text.Anchor = TextAnchor.UpperLeft;
                if (removeActive)
                {
                    if (Widgets.ButtonImage(new Rect(rect.x + rect.width - itemH, rect.y, itemH, itemH), this.texRemove, Color.grey, true, null) && action != null)
                    {
                        action(part);
                    }

                    GUI.color = Color.white;
                }
                else if (!shiftIcon.NullOrEmpty())
                {
                    SZWidgets.ButtonImageVar<ScenPart>(rect3, shiftIcon, onShift, part, "");
                }

                result = Widgets.ButtonInvisible(rect, true);
            }

            return result;
        }


        internal void FullListViewParam2<T, TDef>(List<T> l, ref T selected, Func<T, TDef> defGetter, Func<T, string> labelGetter, bool bRemoveOnClick, Action<T> removeAction) where TDef : Def
        {
            if (!l.NullOrEmpty<T>())
            {
                for (int i = 0; i < l.Count; i++)
                {
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Rect rect = new Rect(this.curX, this.curY, 400f - this.DefSelectionLineHeight, this.DefSelectionLineHeight);
                    Widgets.Label(rect, labelGetter(l[i]));
                    Text.Anchor = TextAnchor.UpperLeft;
                    this.curY += this.DefSelectionLineHeight;
                    if (bRemoveOnClick)
                    {
                        this.BlockRemove<T>(rect, l[i], ref selected, removeAction);
                    }
                    else
                    {
                        this.BlockSelectClick<T>(rect, l[i], ref selected);
                    }
                }
            }
        }


        internal void FullListViewFloat(int w, List<float> l, bool bRemoveOnClick, Action<float> removeAction)
        {
            if (!l.NullOrEmpty<float>())
            {
                float num = 0f;
                for (int i = 0; i < l.Count; i++)
                {
                    float val = l[i];
                    this.ListViewFloat((float)w, val, ref num, bRemoveOnClick, removeAction);
                }
            }
        }


        internal void ListViewFloat(float w, float val, ref float selectedVal, bool bRemoveOnClick, Action<float> removeAction)
        {
            Text.Font = GameFont.Small;
            Rect rect = new Rect(this.curX, this.curY, w - this.DefSelectionLineHeight, this.DefSelectionLineHeight);
            Widgets.Label(rect, val.ToString());
            this.curY += this.DefSelectionLineHeight;
            if (bRemoveOnClick)
            {
                this.BlockRemove<float>(rect, val, ref selectedVal, removeAction);
                return;
            }

            this.BlockSelectClick<float>(rect, val, ref selectedVal);
        }


        internal void FullListViewString(int w, List<string> l, bool bRemoveOnClick, Action<string> removeAction)
        {
            if (!l.NullOrEmpty<string>())
            {
                string text = null;
                for (int i = 0; i < l.Count; i++)
                {
                    string val = l[i];
                    this.ListViewString((float)w, val, ref text, bRemoveOnClick, removeAction);
                }
            }
        }


        internal void ListViewString(float w, string val, ref string selectedVal, bool bRemoveOnClick, Action<string> removeAction)
        {
            Text.Font = GameFont.Small;
            Rect rect = new Rect(this.curX, this.curY, w - this.DefSelectionLineHeight, this.DefSelectionLineHeight);
            Text.WordWrap = false;
            Widgets.Label(rect, val ?? "");
            Text.WordWrap = true;
            TipSignal tip = new TipSignal(() => val, 275);
            TooltipHandler.TipRegion(rect, tip);
            this.curY += this.DefSelectionLineHeight;
            if (bRemoveOnClick)
            {
                this.BlockRemove<string>(rect, val, ref selectedVal, removeAction);
                return;
            }

            this.BlockSelectClick<string>(rect, val, ref selectedVal);
        }


        internal void FullListViewParam1<T>(List<T> l, ref T selected, bool bRemoveOnClick, Action<T> removeAction) where T : Def
        {
            if (!l.NullOrEmpty<T>())
            {
                for (int i = 0; i < l.Count; i++)
                {
                    T def = l[i];
                    this.ListViewParam1<T>(400f, def, ref selected, bRemoveOnClick, removeAction);
                }
            }
        }


        internal void FullListViewWorkTags(WorkTags workTags, bool bRemoveOnClick, Action<WorkTags> removeAction)
        {
            if (workTags != WorkTags.None)
            {
                List<WorkTags> list = workTags.GetAllSelectedItems<WorkTags>().ToList<WorkTags>();
                for (int i = 0; i < list.Count; i++)
                {
                    WorkTags workTags2 = list[i];
                    if (workTags2 != WorkTags.None)
                    {
                        Text.Font = GameFont.Small;
                        Text.Anchor = TextAnchor.MiddleLeft;
                        Rect rect = new Rect(this.curX, this.curY, 400f - this.DefSelectionLineHeight, this.DefSelectionLineHeight);
                        Text.WordWrap = false;
                        string label = workTags2.LabelTranslated().CapitalizeFirst();
                        Widgets.Label(rect, label);
                        Text.WordWrap = true;
                        Text.Anchor = TextAnchor.UpperLeft;
                        this.curY += this.DefSelectionLineHeight;
                        if (bRemoveOnClick)
                        {
                            if (Widgets.ButtonImage(new Rect(rect.x + rect.width, rect.y, this.DefSelectionLineHeight, this.DefSelectionLineHeight), this.texRemove, Color.grey, true, null) && removeAction != null)
                            {
                                removeAction(workTags2);
                            }

                            GUI.color = Color.white;
                        }
                        else
                        {
                            Widgets.ButtonInvisible(rect, true);
                        }
                    }
                }
            }
        }


        internal void FullListViewParam<T, T2>(List<T2> l, ref T selected, Func<T2, T> defGetter, Func<T2, float> valueGetter, Func<T2, float> secValueGetter, Func<T2, float> minGetter, Func<T2, float> maxGetter, bool isInt, bool bRemoveOnClick, Action<T2, float> valueSetter, Action<T2, float> secValueSetter, Action<T> removeAction) where T : Def
        {
            if (!l.NullOrEmpty<T2>())
            {
                for (int i = 0; i < l.Count; i++)
                {
                    T2 t = l[i];
                    float arg = valueGetter(t);
                    float arg2 = (secValueGetter != null) ? secValueGetter(t) : 0f;
                    this.ListViewParam<T>(400f, defGetter(t), ref selected, ref arg, ref arg2, minGetter(t), maxGetter(t), isInt, bRemoveOnClick, removeAction);
                    if (selected == defGetter(t))
                    {
                        valueSetter(t, arg);
                        if (secValueSetter != null)
                        {
                            secValueSetter(t, arg2);
                        }
                    }
                }
            }
        }


        private Rect BlockLabel<T>(int offset, float width, T def, float value, float secValue) where T : Def
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect rect = new Rect(this.curX + (float)offset, this.curY, width - this.DefSelectionLineHeight - (float)offset, this.DefSelectionLineHeight);
            Text.WordWrap = false;
            if (value != -3.4028235E+38f)
            {
                if (typeof(T) == typeof(PawnCapacityDef))
                {
                    Widgets.Label(rect, string.Concat(new string[]
                    {
                        def.SLabel(),
                        " offset: ",
                        value.ToString(),
                        "      factor: ",
                        secValue.ToString()
                    }));
                }
                else
                {
                    Widgets.Label(rect, def.SLabel() + ": " + value.ToString());
                }
            }
            else if (typeof(T) == typeof(HeadTypeDef))
            {
                Widgets.Label(rect, def.SDefname());
            }
            else
            {
                Widgets.Label(rect, def.SLabel());
            }

            Text.WordWrap = true;
            return rect;
        }


        private void BlockTooltip<T>(Rect rect, T def) where T : Def
        {
            string tooltip = def.STooltip<T>();
            if (!string.IsNullOrEmpty(tooltip))
            {
                TipSignal tip = new TipSignal(() => tooltip, 275);
                TooltipHandler.TipRegion(rect, tip);
            }
        }


        private void BlockRemove<T>(Rect rect, T def, ref T selectedDef, Action<T> removeAction)
        {
            if (Widgets.ButtonImage(new Rect(rect.x + rect.width, rect.y, this.DefSelectionLineHeight, this.DefSelectionLineHeight), this.texRemove, Color.grey, true, null) && removeAction != null)
            {
                removeAction(def);
                selectedDef = default(T);
            }

            GUI.color = Color.white;
        }


        private void BlockSelectClick<T>(Rect rect, T def, ref T selectedDef)
        {
            if (Widgets.ButtonInvisible(rect, true))
            {
                selectedDef = ((selectedDef != null) ? default(T) : def);
            }
        }


        private void BLockNumericValue<T>(bool isInt, ref float value, ref float secValue, float min, float max)
        {
            this.curY += this.DefSelectionLineHeight;
            if (isInt)
            {
                value = (float)SZWidgets.NumericIntBox(this.curX, this.curY, 80f, 30f, (int)value, (int)min, (int)max);
            }
            else
            {
                value = SZWidgets.NumericFloatBox(this.curX, this.curY, 70f, 30f, value, min, max);
            }

            if (typeof(T) == typeof(PawnCapacityDef))
            {
                secValue = SZWidgets.NumericFloatBox(this.curX + 150f, this.curY, 70f, 30f, secValue, min, max);
            }

            this.curY += this.DefSelectionLineHeight;
        }


        internal void ListViewParam1<T>(float width, T def, ref T selectedDef, bool bRemoveOnClick, Action<T> removeAction) where T : Def
        {
            float minValue = float.MinValue;
            this.ListViewParam<T>(width, def, ref selectedDef, ref minValue, ref minValue, minValue, minValue, false, bRemoveOnClick, removeAction);
        }


        internal void ListViewParam<T>(float width, T def, ref T selectedDef, ref float value, ref float secValue, float min, float max, bool isInt, bool bRemoveOnClick, Action<T> removeAction) where T : Def
        {
            UnityEngine.Object ticon = def.GetTIcon(null);
            int offset = 0;
            if (ticon != null)
            {
                Rect position = new Rect(this.curX, this.curY, this.DefSelectionLineHeight, this.DefSelectionLineHeight);
                GUI.color = def.GetTColor(null);
                GUI.DrawTexture(position, def.GetTIcon(null));
                GUI.color = Color.white;
                offset = (int)this.DefSelectionLineHeight;
            }

            Rect rect = this.BlockLabel<T>(offset, width, def, value, secValue);
            if (def == selectedDef && value != -3.4028235E+38f)
            {
                this.BLockNumericValue<T>(isInt, ref value, ref secValue, min, max);
            }

            this.BlockTooltip<T>(rect, def);
            Text.Anchor = TextAnchor.UpperLeft;
            this.curY += this.DefSelectionLineHeight;
            if (bRemoveOnClick)
            {
                this.BlockRemove<T>(rect, def, ref selectedDef, removeAction);
                return;
            }

            this.BlockSelectClick<T>(rect, def, ref selectedDef);
        }


        internal bool ListviewSM(float width, StatModifier sm, bool selected, bool withRemove, Action<string> action)
        {
            bool result;
            if (sm == null || sm.stat == null)
            {
                result = false;
            }
            else
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect rect = new Rect(this.curX, this.curY, width - this.DefSelectionLineHeight, this.DefSelectionLineHeight);
                Text.WordWrap = false;
                Widgets.Label(rect, sm.stat.label + ": " + sm.value.ToString());
                Text.WordWrap = true;
                if (selected)
                {
                    this.curY += this.DefSelectionLineHeight;
                    float min = (sm.value < sm.stat.minValue) ? (sm.value - 10f) : sm.stat.minValue;
                    sm.value = SZWidgets.NumericFloatBox(this.curX, this.curY, 70f, 30f, sm.value, min, sm.stat.maxValue);
                    this.curY += this.DefSelectionLineHeight;
                }

                string tooltip = sm.stat.label + "\n" + sm.stat.category.label.Colorize(Color.yellow) + "\n\n";
                tooltip += sm.stat.description;
                TipSignal tip = new TipSignal(() => tooltip, (int)sm.stat.shortHash);
                TooltipHandler.TipRegion(rect, tip);
                Text.Anchor = TextAnchor.UpperLeft;
                this.curY += this.DefSelectionLineHeight;
                if (withRemove)
                {
                    if (Widgets.ButtonImage(new Rect(rect.x + rect.width, rect.y, this.DefSelectionLineHeight, this.DefSelectionLineHeight), this.texRemove, Color.grey, true, null) && action != null)
                    {
                        action(sm.stat.defName);
                    }

                    GUI.color = Color.white;
                }

                result = Widgets.ButtonInvisible(rect, true);
            }

            return result;
        }


        private string GetFormattedValue(string format, float value)
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


        internal void AddMultiplierSection(string paramName, string format, ref string selectedName, float baseValue, ref float value, float min = -3.4028235E+38f, float max = 3.4028235E+38f, bool small = false)
        {
            Text.Font = (small ? GameFont.Small : GameFont.Medium);
            Rect butRect = new Rect(this.curX, this.curY, this.listingRect.width - 130f, 30f);
            Rect rect = new Rect(this.curX, this.curY, this.listingRect.width, 30f);
            float num = baseValue * value;
            string s = paramName + this.GetFormattedValue(format, num);
            Widgets.Label(rect, s.Colorize((num > 0f) ? Color.green : ((num == 0f) ? Color.grey : Color.red)));
            if (paramName == selectedName)
            {
                Text.Font = GameFont.Small;
                value = SZWidgets.NumericFloatBox(this.curX + this.listingRect.width - 105f, this.curY + 4f, 70f, 25f, value, min, float.MaxValue);
            }

            this.curY += 30f;
            Rect rect2 = new Rect(this.curX, this.curY, this.listingRect.width, 20f);
            Widgets.DrawBoxSolid(new Rect(rect2.x, rect2.y, rect2.width, rect2.height / 2f), new Color(0.195f, 0.195f, 0.193f));
            value = Widgets.HorizontalSlider(rect2, value, min, max, false, null, null, null, -1f);
            if (value.ToString().SubstringFrom(".", true).Length > 2)
            {
                value = (float)Math.Round((double)value, 2);
            }

            this.curY += 20f;
            if (Widgets.ButtonInvisible(butRect, true))
            {
                selectedName = ((selectedName == paramName) ? null : paramName);
            }
        }


        internal void AddSection(string paramName, string format, ref string selectedName, ref float value, float min = -3.4028235E+38f, float max = 3.4028235E+38f, bool small = false, string toolTip = "")
        {
            Text.Font = (small ? GameFont.Small : GameFont.Medium);
            Rect butRect = new Rect(this.curX, this.curY, this.listingRect.width - 130f, 30f);
            Rect rect = new Rect(this.curX, this.curY, this.listingRect.width, 30f);
            paramName = (paramName ?? "");
            Widgets.Label(rect, paramName + this.GetFormattedValue(format, value));
            if (!toolTip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(rect, toolTip);
            }

            if (paramName == selectedName)
            {
                Text.Font = GameFont.Small;
                value = SZWidgets.NumericFloatBox(this.curX + this.listingRect.width - 105f, this.curY + 4f, 70f, 25f, value, min, float.MaxValue);
            }

            this.curY += 30f;
            Rect rect2 = new Rect(this.curX, this.curY, this.listingRect.width, 20f);
            value = Widgets.HorizontalSlider(rect2, value, min, max, false, null, null, null, -1f);
            if (value.ToString().SubstringFrom(".", true).Length > 2)
            {
                value = (float)Math.Round((double)value, 2);
            }

            this.curY += 20f;
            if (Widgets.ButtonInvisible(butRect, true))
            {
                selectedName = ((selectedName == paramName) ? null : paramName);
            }
        }


        internal void AddIntSection(string paramName, string format, ref string selectedName, ref int value, int min = -2147483648, int max = 2147483647, bool small = false, string toolTip = "", bool tiny = false)
        {
            Text.Font = (tiny ? GameFont.Tiny : (small ? GameFont.Small : GameFont.Medium));
            Rect rect = new Rect(this.curX, this.curY, this.listingRect.width - 130f, 30f);
            Widgets.Label(new Rect(this.curX, this.curY, this.listingRect.width, 30f), paramName + this.GetFormattedValue(format, (float)value));
            if (paramName == selectedName)
            {
                Text.Font = GameFont.Small;
                value = SZWidgets.NumericIntBox(this.curX + this.listingRect.width - 105f, this.curY + 4f, 70f, 25f, value, min, int.MaxValue);
            }

            this.curY += 30f;
            Rect rect2 = new Rect(this.curX, this.curY, this.listingRect.width, 20f);
            value = (int)Widgets.HorizontalSlider(rect2, (float)value, (float)min, (float)max, false, null, null, null, -1f);
            this.curY += 20f;
            if (Widgets.ButtonInvisible(rect, true))
            {
                selectedName = ((selectedName == paramName) ? null : paramName);
            }

            if (!toolTip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(rect, toolTip);
            }
        }


        internal bool RadioButton(string label, bool active, float tabIn = 0f, string tooltip = null)
        {
            float lineHeight = Text.LineHeight;
            Rect rect = base.GetRect(lineHeight, 1f);
            rect.xMin += tabIn;
            if (!tooltip.NullOrEmpty())
            {
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }

                TooltipHandler.TipRegion(rect, tooltip);
            }

            bool result = Widgets.RadioButtonLabeled(rect, label, active, false);
            base.Gap(this.verticalSpacing);
            return result;
        }


        internal bool SelectableFast(string text, bool selected, string tooltip)
        {
            Rect rect = new Rect(this.curX, this.curY, this.listingRect.width, this.DefSelectionLineHeight);
            if (selected)
            {
                GUI.DrawTexture(rect, TexUI.TextBGBlack);
                GUI.color = Color.green;
                Widgets.DrawHighlight(rect);
            }
            else
            {
                GUI.color = Color.white;
            }

            Widgets.Label(rect, text);
            TooltipHandler.TipRegion(rect, tooltip);
            this.curY += this.DefSelectionLineHeight;
            return Widgets.ButtonInvisible(rect, true);
        }


        internal bool SelectableAbility(string name, bool selected, string tooltip, AbilityDef def, Color selColor)
        {
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            float width = this.listingRect.width;
            Rect position = new Rect(this.curX, this.curY, 64f, 64f);
            Rect rect = new Rect(position.x, position.y, width, 64f);
            GUI.DrawTexture(position, def.uiIcon);
            TooltipHandler.TipRegion(rect, tooltip);
            if (selected)
            {
                GUI.DrawTexture(rect, TexUI.TextBGBlack);
                GUI.color = selColor;
                Widgets.DrawHighlight(rect);
            }

            GUI.color = Color.white;
            Widgets.Label(new Rect(position.x + 68f, position.y + 23f, width - 64f, 28f), name);
            return Widgets.ButtonInvisible(rect, false);
        }


        internal bool SelectableGene(string name, bool selected, string tooltip, GeneDef def, Color selColor)
        {
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            float width = this.listingRect.width;
            Rect position = new Rect(this.curX, this.curY, 64f, 64f);
            Rect rect = new Rect(position.x, position.y, width, 64f);
            GUI.DrawTexture(position, def.Icon, ScaleMode.StretchToFill, true, 0f, def.IconColor, 0f, 0f);
            TooltipHandler.TipRegion(rect, tooltip);
            if (selected)
            {
                GUI.DrawTexture(rect, TexUI.TextBGBlack);
                GUI.color = selColor;
                Widgets.DrawHighlight(rect);
            }

            GUI.color = Color.white;
            Widgets.Label(new Rect(position.x + 68f, position.y + 23f, width - 72f, 38f), name);
            return Widgets.ButtonInvisible(rect, false);
        }


        internal bool SelectableThing(string name, bool selected, string tooltip, ThingDef def, Color selColor)
        {
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            float width = this.listingRect.width;
            Rect position = new Rect(this.curX, this.curY, 64f, 64f);
            Rect rect = new Rect(position.x, position.y, width, 64f);
            Texture2D ticon = def.GetTIcon(null);
            if (ticon != null)
            {
                GUI.DrawTexture(position, ticon, ScaleMode.StretchToFill, true, 0f, def.uiIconColor, 0f, 0f);
            }

            TooltipHandler.TipRegion(rect, tooltip);
            if (selected)
            {
                GUI.DrawTexture(rect, TexUI.TextBGBlack);
                GUI.color = selColor;
                Widgets.DrawHighlight(rect);
            }

            GUI.color = Color.white;
            Widgets.Label(new Rect(position.x + 68f, position.y + 23f, width - 72f, 38f), name);
            return Widgets.ButtonInvisible(rect, false);
        }


        internal bool SelectableText<T>(string name, bool isThing, bool isAbility, bool isGene, bool isHair, bool isBeard, bool selected, string tooltip, bool withRemove, bool isWhite, T def, Color selColor, bool hasIcon = false, bool selectOnMouseOver = false)
        {
            bool result;
            if (name == null)
            {
                result = false;
            }
            else
            {
                float width = this.listingRect.width;
                Rect rect = new Rect(this.curX, this.curY, width, this.DefSelectionLineHeight);
                try
                {
                    if (isAbility)
                    {
                        result = this.SelectableAbility(name, selected, tooltip, def as AbilityDef, selColor);
                    }
                    else if (isGene)
                    {
                        result = this.SelectableGene(name, selected, tooltip, def as GeneDef, selColor);
                    }
                    else if (isThing)
                    {
                        result = this.SelectableThing(name, selected, tooltip, def as ThingDef, selColor);
                    }
                    else
                    {
                        if (selected)
                        {
                            GUI.DrawTexture(rect, TexUI.TextBGBlack);
                            GUI.color = selColor;
                            Widgets.DrawHighlight(rect);
                        }

                        if (hasIcon)
                        {
                            Rect outerRect = new Rect(this.curX, this.curY, this.DefSelectionLineHeight, this.DefSelectionLineHeight);
                            Texture2D ticon = def.GetTIcon(null);
                            if (ticon != null)
                            {
                                GUI.color = def.GetTColor(null);
                                Widgets.DrawTextureFitted(outerRect, ticon, 1f);
                            }
                        }

                        Text.WordWrap = false;
                        GUI.color = (isWhite ? (selected ? Color.green : Color.white) : Color.gray);
                        if (hasIcon || isThing)
                        {
                            Widgets.Label(new Rect(rect.x + this.DefSelectionLineHeight, rect.y, rect.width, rect.height), name);
                        }
                        else
                        {
                            Widgets.Label(rect, name);
                        }

                        GUI.color = Color.white;
                        if (!string.IsNullOrEmpty(tooltip))
                        {
                            TooltipHandler.TipRegion(rect, tooltip);
                        }

                        Text.WordWrap = true;
                        bool flag = false;
                        if (isHair)
                        {
                            Rect rect2 = new Rect(this.curX, this.curY + this.DefSelectionLineHeight, 192f, 64f);
                            Widgets.DrawBoxSolid(rect2, ColorTool.colAsche);
                            string texPath = (def as HairDef).texPath;
                            if (!texPath.NullOrEmpty())
                            {
                                Graphic g = GraphicDatabase.Get<Graphic_Multi>(texPath);
                                Rect position = new Rect(this.curX, this.curY + this.DefSelectionLineHeight, 64f, 64f);
                                Texture2D textureFromMulti = g.GetTextureFromMulti("_south");
                                GUI.color = Color.white;
                                if (textureFromMulti != null)
                                {
                                    GUI.DrawTexture(position, textureFromMulti);
                                }

                                Rect position2 = new Rect(this.curX + 64f, this.curY + this.DefSelectionLineHeight, 64f, 64f);
                                Texture2D textureFromMulti2 = g.GetTextureFromMulti("_east");
                                if (textureFromMulti2 == null)
                                {
                                    textureFromMulti2 = g.GetTextureFromMulti("_west");
                                }

                                GUI.color = Color.white;
                                if (textureFromMulti2 != null)
                                {
                                    GUI.DrawTexture(position2, textureFromMulti2);
                                }

                                Rect position3 = new Rect(this.curX + 128f, this.curY + this.DefSelectionLineHeight, 64f, 64f);
                                Texture2D textureFromMulti3 = g.GetTextureFromMulti("_north");
                                GUI.color = Color.white;
                                if (textureFromMulti3 != null)
                                {
                                    GUI.DrawTexture(position3, textureFromMulti3);
                                }
                            }

                            if (selectOnMouseOver && Mouse.IsOver(rect2))
                            {
                                flag = true;
                            }
                        }

                        if (isBeard)
                        {
                            Graphic g2 = GraphicDatabase.Get<Graphic_Multi>((def as BeardDef).texPath);
                            Rect rect3 = new Rect(this.curX, this.curY + this.DefSelectionLineHeight, 192f, 64f);
                            Widgets.DrawBoxSolid(rect3, ColorTool.colAsche);
                            Rect position4 = new Rect(this.curX, this.curY + this.DefSelectionLineHeight, 64f, 64f);
                            Texture2D textureFromMulti4 = g2.GetTextureFromMulti("_south");
                            GUI.color = Color.white;
                            GUI.DrawTexture(position4, textureFromMulti4);
                            Rect position5 = new Rect(this.curX + 64f, this.curY + this.DefSelectionLineHeight, 64f, 64f);
                            Texture2D textureFromMulti5 = g2.GetTextureFromMulti("_east");
                            if (textureFromMulti5 == null)
                            {
                                textureFromMulti5 = g2.GetTextureFromMulti("_west");
                            }

                            GUI.color = Color.white;
                            GUI.DrawTexture(position5, textureFromMulti5);
                            Rect position6 = new Rect(this.curX + 128f, this.curY + this.DefSelectionLineHeight, 64f, 64f);
                            Texture2D textureFromMulti6 = g2.GetTextureFromMulti("_north");
                            GUI.DrawTexture(position6, textureFromMulti6);
                            if (selectOnMouseOver && Mouse.IsOver(rect3))
                            {
                                flag = true;
                            }
                        }

                        if (withRemove)
                        {
                            result = Widgets.ButtonImage(new Rect(rect.x + rect.width - this.DefSelectionLineHeight - 12f, rect.y, this.DefSelectionLineHeight, this.DefSelectionLineHeight), this.texRemove, true, null);
                        }
                        else
                        {
                            result = ((selectOnMouseOver && flag) || Widgets.ButtonInvisible(rect, false));
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }


        internal int SelectableHorizontal(string name, bool selected, string tooltip = "", RenderTexture image = null, ThingDef thingDef = null, HairDef hairDef = null, Vector2 imageSize = default(Vector2), bool withRemove = false, float selectHeight = 22f, Color backColor = default(Color))
        {
            int result;
            if (name == null)
            {
                result = 0;
            }
            else
            {
                Text.Font = GameFont.Small;
                float num = this.listingRect.width - this.DefSelectionLineHeight;
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect rect = new Rect(this.curX, this.curY, num + this.DefSelectionLineHeight, selectHeight);
                if (selected)
                {
                    GUI.DrawTexture(rect, TexUI.TextBGBlack);
                    GUI.color = Color.green;
                    Widgets.DrawHighlight(rect);
                }

                Rect rect2;
                if (thingDef != null)
                {
                    rect2 = new Rect(this.curX + 25f, this.curY, num, this.DefSelectionLineHeight);
                }
                else if (selectHeight == 22f)
                {
                    rect2 = new Rect(this.curX, this.curY, num, this.DefSelectionLineHeight);
                }
                else
                {
                    rect2 = new Rect(this.curX, this.curY, num + this.DefSelectionLineHeight, this.DefSelectionLineHeight);
                }

                Text.WordWrap = false;
                if (backColor != default(Color))
                {
                    GUI.color = backColor;
                }

                Widgets.Label(rect2, name);
                GUI.color = Color.white;
                if (!string.IsNullOrEmpty(tooltip))
                {
                    TooltipHandler.TipRegion(rect2, tooltip);
                }

                Text.WordWrap = true;
                if (image != null)
                {
                    Rect position;
                    if (imageSize == default(Vector2))
                    {
                        position = new Rect(this.curX, this.curY + this.DefSelectionLineHeight, 64f, 90f);
                    }
                    else
                    {
                        position = new Rect(this.curX, this.curY + this.DefSelectionLineHeight, imageSize.x, imageSize.y);
                    }

                    GUI.color = Color.white;
                    GUI.DrawTexture(position, image);
                }

                Text.Anchor = TextAnchor.UpperLeft;
                if (image != null)
                {
                    if (imageSize == default(Vector2))
                    {
                        this.curX += 100f;
                    }
                    else
                    {
                        this.curX += imageSize.x;
                    }
                }

                if (withRemove)
                {
                    if (Widgets.ButtonImage(new Rect(rect.x + rect.width - this.DefSelectionLineHeight, rect.y, this.DefSelectionLineHeight, this.DefSelectionLineHeight), this.texRemove, true, null))
                    {
                        result = 2;
                    }
                    else if (Widgets.ButtonInvisible(rect, false))
                    {
                        result = 1;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else if (Widgets.ButtonInvisible(rect, false))
                {
                    result = 1;
                }
                else
                {
                    result = 0;
                }
            }

            return result;
        }


        internal int Selectable(string name, bool selected, string tooltip = "", RenderTexture image = null, ThingDef thingDef = null, HairDef hairDef = null, Vector2 imageSize = default(Vector2), bool withRemove = false, float selectHeight = 22f, Color backColor = default(Color), Color selectedColor = default(Color), bool autoincrement = true)
        {
            int result;
            if (name == null)
            {
                result = 0;
            }
            else
            {
                Text.Font = GameFont.Small;
                float num = this.listingRect.width - this.DefSelectionLineHeight;
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect rect = new Rect(this.curX, this.curY, num + this.DefSelectionLineHeight, selectHeight);
                if (selected)
                {
                    GUI.DrawTexture(rect, TexUI.TextBGBlack);
                    GUI.color = selectedColor;
                    Widgets.DrawHighlight(rect);
                }

                GUI.color = Color.white;
                if (thingDef != null)
                {
                    Widgets.ThingIcon(new Rect(this.curX, this.curY, this.DefSelectionLineHeight, this.DefSelectionLineHeight), thingDef, null, null, 1f, null, null);
                }

                Rect rect2;
                if (thingDef != null)
                {
                    rect2 = new Rect(this.curX + 25f, this.curY, num, this.DefSelectionLineHeight);
                }
                else if (selectHeight == 22f)
                {
                    rect2 = new Rect(this.curX, this.curY, num, this.DefSelectionLineHeight);
                }
                else
                {
                    rect2 = new Rect(this.curX, this.curY, num + this.DefSelectionLineHeight, (name.Length > 10) ? (this.DefSelectionLineHeight + 12f) : this.DefSelectionLineHeight);
                }

                if (backColor != default(Color))
                {
                    GUI.color = backColor;
                }

                Widgets.Label(rect2, name);
                GUI.color = Color.white;
                if (!string.IsNullOrEmpty(tooltip))
                {
                    TooltipHandler.TipRegion(rect, tooltip);
                }

                if (image != null)
                {
                    float num2 = (this.listingRect.width - imageSize.x) / 2f;
                    Rect position;
                    if (imageSize == default(Vector2))
                    {
                        position = new Rect(this.curX, this.curY + this.DefSelectionLineHeight, 64f, 90f);
                    }
                    else
                    {
                        position = new Rect(this.curX + num2, this.curY + 11f, imageSize.x, imageSize.y);
                    }

                    GUI.color = Color.white;
                    GUI.DrawTexture(position, image);
                }

                if (hairDef != null)
                {
                    Rect position2 = new Rect(this.curX, this.curY + this.DefSelectionLineHeight, 64f, 64f);
                    Graphic g = GraphicDatabase.Get<Graphic_Multi>(hairDef.texPath);
                    Texture2D textureFromMulti = g.GetTextureFromMulti("_south");
                    GUI.color = Color.white;
                    GUI.DrawTexture(position2, textureFromMulti);
                    Rect position3 = new Rect(this.curX + 64f, this.curY + this.DefSelectionLineHeight, 64f, 64f);
                    Texture2D textureFromMulti2 = g.GetTextureFromMulti("_east");
                    if (textureFromMulti2 == null)
                    {
                        textureFromMulti2 = g.GetTextureFromMulti("_west");
                    }

                    GUI.color = Color.white;
                    GUI.DrawTexture(position3, textureFromMulti2);
                    Rect position4 = new Rect(this.curX + 128f, this.curY + this.DefSelectionLineHeight, 64f, 64f);
                    Texture2D textureFromMulti3 = g.GetTextureFromMulti("_north");
                    GUI.color = Color.white;
                    GUI.DrawTexture(position4, textureFromMulti3);
                }

                Text.Anchor = TextAnchor.UpperLeft;
                if (autoincrement)
                {
                    this.curY += this.DefSelectionLineHeight;
                    if (image != null)
                    {
                        if (imageSize == default(Vector2))
                        {
                            this.curY += 90f;
                        }
                        else
                        {
                            this.curY += imageSize.y - this.DefSelectionLineHeight;
                        }
                    }
                    else if (hairDef != null)
                    {
                        this.curY += 70f;
                    }
                }

                if (withRemove)
                {
                    if (Widgets.ButtonImage(new Rect(rect.x + rect.width - this.DefSelectionLineHeight, rect.y + (float)((name.Length > 20) ? 6 : 0), this.DefSelectionLineHeight, this.DefSelectionLineHeight), this.texRemove, true, null))
                    {
                        result = 2;
                    }
                    else if (Widgets.ButtonInvisible(rect, false))
                    {
                        result = 1;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else if (Widgets.ButtonInvisible(rect, false))
                {
                    result = 1;
                }
                else
                {
                    result = 0;
                }
            }

            return result;
        }


        internal bool TableLine(Texture2D col0Icon, string col1Val, string col2Val, string col3Val, Texture2D col4Tex, string tooltip)
        {
            Text.Font = GameFont.Small;
            float num = (float)((int)((this.listingRect.width - this.DefSelectionLineHeight) / 3f));
            float defSelectionLineHeight = this.DefSelectionLineHeight;
            Rect position = new Rect(this.curX, this.curY, defSelectionLineHeight, defSelectionLineHeight);
            Rect rect = new Rect(this.curX + defSelectionLineHeight, this.curY, num, defSelectionLineHeight);
            Rect rect2 = new Rect(this.curX + num + defSelectionLineHeight, this.curY, num, defSelectionLineHeight);
            Rect rect3 = new Rect(this.curX + num * 2f + defSelectionLineHeight, this.curY, num, defSelectionLineHeight);
            Rect butRect = new Rect(this.curX + num * 3f + defSelectionLineHeight, this.curY, defSelectionLineHeight, defSelectionLineHeight);
            GUI.DrawTexture(position, col0Icon);
            Widgets.Label(rect, col1Val);
            Widgets.Label(rect2, col2Val);
            Widgets.Label(rect3, col3Val);
            bool result = Widgets.ButtonImage(butRect, col4Tex, true, null);
            if (!string.IsNullOrEmpty(tooltip))
            {
                TipSignal tip = new TipSignal(() => tooltip, 2347778);
                TooltipHandler.TipRegion(rect2, tip);
            }

            this.curY += this.DefSelectionLineHeight;
            return result;
        }


        internal bool SelectableThought(string name, Texture2D icon, Color iconColor, float valopin = -3.4028235E+38f, float valmood = -3.4028235E+38f, string tooltip = "", string pName = "", bool withRemove = true)
        {
            Text.Font = GameFont.Small;
            float num = this.listingRect.width - this.DefSelectionLineHeight;
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect rect = new Rect(this.curX, this.curY, num + this.DefSelectionLineHeight, this.DefSelectionLineHeight);
            GUI.color = iconColor;
            if (icon != null)
            {
                GUI.DrawTexture(new Rect(rect.x, rect.y, 24f, 24f), icon);
            }

            Text.WordWrap = false;
            Rect rect2 = new Rect(this.curX + 30f, this.curY, num, this.DefSelectionLineHeight);
            Widgets.DrawBoxSolid(rect2, this.alternate ? new Color(0.15f, 0.15f, 0.15f) : new Color(0.1f, 0.1f, 0.1f));
            this.alternate = !this.alternate;
            Widgets.Label(rect2, name);
            if (tooltip != null)
            {
                TipSignal tip = new TipSignal(() => tooltip, 275);
                TooltipHandler.TipRegion(rect2, tip);
            }

            if (!string.IsNullOrEmpty(pName))
            {
                Widgets.Label(new Rect(rect2.x + rect2.width - 190f, rect2.y - 1f, 120f, 24f), pName);
            }

            bool flag = valopin != float.MinValue;
            float num2 = flag ? valopin : valmood;
            int num3 = (num2 < 0f) ? 4 : 0;
            Rect rect3 = new Rect(rect2.x + rect2.width - 80f - (float)num3, rect2.y, 48f, 24f);
            if (num2 == 0f)
            {
                GUI.color = (flag ? ColorTool.colBeige : this.NoEffectColor);
            }
            else if (num2 > 0f)
            {
                GUI.color = (flag ? ColorTool.colLightBlue : this.MoodColor);
            }
            else
            {
                GUI.color = (flag ? ColorTool.colPink : this.MoodColorNegative);
            }

            Widgets.Label(rect3, num2.ToString("##0"));
            bool result = false;
            if (withRemove)
            {
                result = Widgets.ButtonImage(new Rect(rect2.x + rect2.width - 42f, rect2.y, 24f, 24f), ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true), true, null);
            }

            Text.WordWrap = true;
            Text.Anchor = TextAnchor.UpperLeft;
            this.curY += this.DefSelectionLineHeight;
            return result;
        }


        internal float Slider(float val, float min, float max, Color color)
        {
            GUI.color = color;
            float result = Widgets.HorizontalSlider(base.GetRect(22f, 1f), val, min, max, false, null, null, null, -1f);
            base.Gap(this.verticalSpacing);
            return result;
        }


        internal float Slider(float val, float min, float max)
        {
            float result = Widgets.HorizontalSlider(base.GetRect(22f, 1f), val, min, max, false, null, null, null, -1f);
            base.Gap(this.verticalSpacing);
            return result;
        }


        internal float Slider(float val, float min, float max, float width)
        {
            Rect rect = base.GetRect(22f, 1f);
            rect.width = width;
            return Widgets.HorizontalSlider(rect, val, min, max, false, null, null, null, -1f);
        }


        internal float SliderWithNumeric(float val, float min, float max, int decimals)
        {
            float num = (float)Math.Round((double)Widgets.HorizontalSlider(base.GetRect(22f, 1f), val, min, max, false, null, null, null, -1f), decimals);
            Rect rect = base.GetRect(22f, 1f);
            rect.width = 70f;
            num = SZWidgets.NumericFloatBox(rect, num, float.MinValue, float.MaxValue);
            base.Gap(4f);
            return num;
        }


        internal int SliderWithNumeric(int val, int min, int max)
        {
            int num = (int)Widgets.HorizontalSlider(base.GetRect(22f, 1f), (float)val, (float)min, (float)max, false, null, null, null, -1f);
            Rect rect = base.GetRect(22f, 1f);
            rect.width = 70f;
            num = SZWidgets.NumericIntBox(rect, num, int.MinValue, int.MaxValue);
            base.Gap(4f);
            return num;
        }


        internal string TextEntry(string text, int lineCount = 1)
        {
            Rect rect = base.GetRect(Text.LineHeight * (float)lineCount, 1f);
            string result;
            if (lineCount == 1)
            {
                result = Widgets.TextField(rect, text);
            }
            else
            {
                result = Widgets.TextArea(rect, text, false);
            }

            base.Gap(this.verticalSpacing);
            return result;
        }


        internal string TextEntryLabeledWithDefaultAndCopy(string label, string text, string defaultVal)
        {
            Rect rect = base.GetRect(Text.LineHeight * 1f, 1f);
            rect.width -= 50f;
            Rect rect2 = rect.LeftHalf().Rounded();
            Rect rect3 = rect.RightHalf().Rounded();
            Widgets.Label(rect2, label);
            string text2 = Widgets.TextField(rect3, text);
            string[] array = text2.SplitNoEmpty(";");
            if (array.Length > 1)
            {
                TooltipHandler.TipRegion(rect2, label + "\n" + array.Length.ToString() + " saved entities");
            }
            else
            {
                TooltipHandler.TipRegion(rect2, label + "\n" + array.Length.ToString() + " saved entity");
            }

            Rect rect4 = new Rect(rect.x + rect.width, rect.y, 24f, 24f);
            if (Widgets.ButtonImage(rect4, ContentFinder<Texture2D>.Get("bdefault", true), true, null))
            {
                text2 = defaultVal;
            }

            TooltipHandler.TipRegion(rect4, CharacterEditor.Label.O_SETTODEFAULT);
            Rect rect5 = new Rect(rect.x + rect.width + 25f, rect.y, 24f, 24f);
            if (Widgets.ButtonImage(rect5, ContentFinder<Texture2D>.Get("UI/Buttons/Copy", true), true, null))
            {
                Clipboard.CopyToClip(text);
            }

            TooltipHandler.TipRegion(rect5, CharacterEditor.Label.O_COPYTOCLIPBOARD);
            base.Gap(this.verticalSpacing);
            return text2;
        }


        internal string TextEntryLabeled(string label, string text, int lineCount = 1)
        {
            string result = Widgets.TextEntryLabeled(base.GetRect(Text.LineHeight * (float)lineCount, 1f), label, text);
            base.Gap(this.verticalSpacing);
            return result;
        }


        internal void TextFieldNumeric<T>(float xStart, float width, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
        {
            Rect rect = base.GetRect(Text.LineHeight, 1f);
            rect.x = xStart;
            rect.width = width;
            Widgets.TextFieldNumeric<T>(rect, ref val, ref buffer, min, max);
            base.Gap(this.verticalSpacing);
        }


        internal void TextFieldNumericLabeled<T>(string label, float xStart, float width, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
        {
            Rect rect = base.GetRect(Text.LineHeight, 1f);
            rect.x = xStart;
            rect.width = width;
            Widgets.TextFieldNumericLabeled<T>(rect, label, ref val, ref buffer, min, max);
            base.Gap(this.verticalSpacing);
        }


        private Vector2 GetLabelScrollbarPosition(float x, float y)
        {
            Vector2 zero;
            if (this.labelScrollbarPositions == null)
            {
                zero = Vector2.zero;
            }
            else
            {
                for (int i = 0; i < this.labelScrollbarPositions.Count; i++)
                {
                    Vector2 first = this.labelScrollbarPositions[i].First;
                    if (first.x == x && first.y == y)
                    {
                        return this.labelScrollbarPositions[i].Second;
                    }
                }

                zero = Vector2.zero;
            }

            return zero;
        }


        private void SetLabelScrollbarPosition(float x, float y, Vector2 scrollbarPosition)
        {
            if (this.labelScrollbarPositions == null)
            {
                this.labelScrollbarPositions = new List<Pair<Vector2, Vector2>>();
                this.labelScrollbarPositionsSetThisFrame = new List<Vector2>();
            }

            this.labelScrollbarPositionsSetThisFrame.Add(new Vector2(x, y));
            for (int i = 0; i < this.labelScrollbarPositions.Count; i++)
            {
                Vector2 first = this.labelScrollbarPositions[i].First;
                if (first.x == x && first.y == y)
                {
                    this.labelScrollbarPositions[i] = new Pair<Vector2, Vector2>(new Vector2(x, y), scrollbarPosition);
                    return;
                }
            }

            this.labelScrollbarPositions.Add(new Pair<Vector2, Vector2>(new Vector2(x, y), scrollbarPosition));
        }


        internal void NavSelectorColor(int width, string label, string tip, Color? color, Action onClicked)
        {
            Rect rect = base.GetRect(22f, 1f);
            rect.width = (float)width;
            Text.Font = GameFont.Small;
            SZWidgets.LabelBackground(Listing_X.RectSolid(rect, false), label, ColorTool.colAsche, 0, tip, default(Color));
            SZWidgets.ButtonSolid(Listing_X.RectRandom(rect), color ?? Color.clear, onClicked, "");
        }


        internal Rect GetRect2(int w, int h = 22)
        {
            Rect rect = base.GetRect((float)h, 1f);
            rect.width = (float)w;
            return rect;
        }


        private static Rect RectNext(Rect rect)
        {
            return new Rect(rect.x + rect.width - 22f, rect.y + 2f, 22f, 22f);
        }


        private static Rect RectOnClick(Rect rect, bool hasTexture, int offset, bool showEdit)
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


        private static Rect RectSolid(Rect rect, bool showEdit)
        {
            return new Rect(rect.x + (float)(showEdit ? 21 : 0), rect.y, rect.width - (float)(showEdit ? 40 : 19), 24f);
        }


        private static Rect RectTexture(Rect rect, bool showEdit)
        {
            return new Rect(rect.x + (float)(showEdit ? 25 : 0), rect.y, 24f, 24f);
        }


        private static Rect RectToggle(Rect rect)
        {
            return new Rect(rect.x + rect.width - 42f, rect.y, 22f, 22f);
        }


        private static Rect RectToggleLeft(Rect rect)
        {
            return new Rect(rect.x + rect.width - 67f, rect.y, 22f, 22f);
        }


        internal float DefSelectionLineHeight = 22f;


        private GameFont font;


        private Color MoodColor = new Color(0.1f, 1f, 0.1f);


        private Color MoodColorNegative = new Color(0.8f, 0.4f, 0.4f);


        private Color NoEffectColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);


        private List<Pair<Vector2, Vector2>> labelScrollbarPositions;


        private List<Vector2> labelScrollbarPositionsSetThisFrame;


        private Texture2D texRemove;


        private bool alternate = true;
    }
}
