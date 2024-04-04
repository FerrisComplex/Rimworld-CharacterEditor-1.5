using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CharacterEditor
{
    internal class DialogXenoType : GeneCreationDialogBase
    {
        
        public override Vector2 InitialSize
        {
            get { return new Vector2(1036f, (float)WindowTool.MaxH); }
        }


        
        protected override List<GeneDef> SelectedGenes
        {
            get { return this.selectedGenes; }
        }


        
        protected override string Header
        {
            get { return "CreateXenotype".Translate().CapitalizeFirst(); }
        }


        
        protected override string AcceptButtonLabel
        {
            get { return "SaveAndApply".Translate().CapitalizeFirst(); }
        }


        internal DialogXenoType(Pawn _pawn)
        {
            this.predefinedXenoDef = null;
            this.pawn = _pawn;
            if (!this.pawn.genes.xenotypeName.NullOrEmpty())
            {
                this.xenotypeName = this.pawn.genes.xenotypeName;
            }
            else
            {
                this.xenotypeName = "";
            }

            this.doOnce = true;
            SearchTool.Update(SearchTool.SIndex.XenoType);
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnAccept = false;
            this.closeOnCancel = true;
            this.closeOnClickedOutside = true;
            this.layer = CEditor.Layer;
            this.draggable = true;
            this.alwaysUseFullBiostatsTableHeight = true;
            this.searchWidgetOffsetX = (float)((double)GeneCreationDialogBase.ButSize.x * 2.0 + 4.0);
            foreach (GeneCategoryDef key in DefDatabase<GeneCategoryDef>.AllDefs)
            {
                this.collapsedCategories.Add(key, false);
            }

            this.OnGenesChanged();
        }


        public override void PostOpen()
        {
            if (this.doOnce)
            {
                SearchTool.SetPosition(SearchTool.SIndex.XenoType, ref this.windowRect, ref this.doOnce, 0);
            }

            if (!ModsConfig.BiotechActive)
            {
                this.Close(false);
            }
            else
            {
                base.PostOpen();
            }

            if ((this.pawn.genes != null || this.pawn.genes.Xenotype.IsNullOrEmpty()) && DefDatabase<XenotypeDef>.AllDefs.Contains(this.pawn.genes.Xenotype) && this.pawn.genes.Xenotype != XenotypeDefOf.Baseliner)
            {
                this.ALoadXenotypeDef(this.pawn.genes.Xenotype);
                return;
            }

            List<GeneDef> list = new List<GeneDef>();
            foreach (Gene gene in this.pawn.genes.Xenogenes)
            {
                list.Add(gene.def);
            }

            CustomXenotype customXenotype = new CustomXenotype();
            CustomXenotype customXenotype2 = customXenotype;
            string xenotypeName = this.pawn.genes.xenotypeName;
            customXenotype2.name = ((xenotypeName != null) ? xenotypeName.Trim() : null);
            if (customXenotype.name.NullOrEmpty())
            {
                customXenotype.name = "";
            }

            customXenotype.genes.AddRange(list);
            customXenotype.inheritable = this.pawn.genes.Xenotype.inheritable;
            customXenotype.iconDef = this.pawn.genes.iconDef;
            if (customXenotype.name.NullOrEmpty())
            {
                this.ALoadCustomXenotype(customXenotype);
                return;
            }

            this.DoFileInteraction(customXenotype.name);
        }


        public override void Close(bool doCloseSound = true)
        {
            SearchTool.Save(SearchTool.SIndex.XenoType, this.windowRect.position);
            base.Close(doCloseSound);
        }


        protected override void DrawGenes(Rect rect)
        {
            this.hoveredAnyGene = false;
            GUI.BeginGroup(rect);
            float num = 0f;
            this.DrawSection(new Rect(0f, 0f, rect.width, this.selectedHeight), this.selectedGenes, "SelectedGenes".Translate(), ref num, ref this.selectedHeight, false, rect, ref this.selectedCollapsed);
            if (!this.selectedCollapsed.Value)
            {
                num += 10f;
            }

            float num2 = num;
            Widgets.Label(0f, ref num, rect.width, "Genes".Translate().CapitalizeFirst(), default(TipSignal));
            float num3 = num + 10f;
            float height = (float)((double)num3 - (double)num2 - 4.0);
            if (Widgets.ButtonText(new Rect((float)((double)rect.width - 150.0 - 16.0), num2, 150f, height), "CollapseAllCategories".Translate(), true, true, true, null))
            {
                SoundDefOf.TabClose.PlayOneShotOnCamera(null);
                foreach (GeneCategoryDef key in DefDatabase<GeneCategoryDef>.AllDefs)
                {
                    this.collapsedCategories[key] = true;
                }
            }

            if (Widgets.ButtonText(new Rect((float)((double)rect.width - 300.0 - 4.0 - 16.0), num2, 150f, height), "ExpandAllCategories".Translate(), true, true, true, null))
            {
                SoundDefOf.TabOpen.PlayOneShotOnCamera(null);
                foreach (GeneCategoryDef key2 in DefDatabase<GeneCategoryDef>.AllDefs)
                {
                    this.collapsedCategories[key2] = false;
                }
            }

            float num4 = num3;
            Rect rect2 = new Rect(0f, num3, rect.width - 16f, this.scrollHeight);
            Widgets.BeginScrollView(new Rect(0f, num3, rect.width, rect.height - num3), ref this.scrollPosition, rect2, true);
            Rect containingRect = rect2;
            containingRect.y = num3 + this.scrollPosition.y;
            containingRect.height = rect.height;
            bool? flag = null;
            this.DrawSection(rect, GeneUtility.GenesInOrder, null, ref num3, ref this.unselectedHeight, true, containingRect, ref flag);
            if (Event.current.type == EventType.Layout)
            {
                this.scrollHeight = num3 - num4;
            }

            Widgets.EndScrollView();
            GUI.EndGroup();
            if (!this.hoveredAnyGene)
            {
                this.hoveredGene = null;
            }
        }


        private void DrawSection(Rect rect, List<GeneDef> genes, string label, ref float curY, ref float sectionHeight, bool adding, Rect containingRect, ref bool? collapsed)
        {
            float num = 4f;
            if (!label.NullOrEmpty())
            {
                Rect rect2 = new Rect(0f, curY, rect.width, Text.LineHeight);
                rect2.xMax -= (adding ? 16f : (Text.CalcSize("ClickToAddOrRemove".Translate()).x + 4f));
                if (collapsed != null)
                {
                    Rect position = new Rect(rect2.x, rect2.y + (float)(((double)rect2.height - 18.0) / 2.0), 18f, 18f);
                    GUI.DrawTexture(position, collapsed.Value ? TexButton.Reveal : TexButton.Collapse);
                    if (Widgets.ButtonInvisible(rect2, true))
                    {
                        bool? flag = !collapsed;
                        collapsed = flag;
                        if (collapsed.Value)
                        {
                            SoundDefOf.TabClose.PlayOneShotOnCamera(null);
                        }
                        else
                        {
                            SoundDefOf.TabOpen.PlayOneShotOnCamera(null);
                        }
                    }

                    if (Mouse.IsOver(rect2))
                    {
                        Widgets.DrawHighlight(rect2);
                    }

                    rect2.xMin += position.width;
                }

                Widgets.Label(rect2, label);
                if (!adding)
                {
                    Text.Anchor = TextAnchor.UpperRight;
                    GUI.color = ColoredText.SubtleGrayColor;
                    Widgets.Label(new Rect(rect2.xMax - 18f, curY, rect.width - rect2.width, Text.LineHeight), "ClickToAddOrRemove".Translate());
                    GUI.color = Color.white;
                    Text.Anchor = TextAnchor.UpperLeft;
                }

                curY += Text.LineHeight + 3f;
            }

            bool? flag2 = collapsed;
            bool flag3 = true;
            if (flag2.GetValueOrDefault() == flag3 & flag2 != null)
            {
                if (Event.current.type == EventType.Layout)
                {
                    sectionHeight = 0f;
                    return;
                }
            }
            else
            {
                float num2 = curY;
                bool flag4 = false;
                float num3 = (float)(34.0 + (double)GeneCreationDialogBase.GeneSize.x + 8.0);
                float num4 = rect.width - 16f;
                float num5 = num3 + 4f;
                float b = (float)(((double)num4 - (double)num5 * (double)Mathf.Floor(num4 / num5)) / 2.0);
                Rect rect3 = new Rect(0f, curY, rect.width, sectionHeight);
                if (!adding)
                {
                    Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor, null);
                }

                curY += 4f;
                if (!genes.Any<GeneDef>())
                {
                    Text.Anchor = TextAnchor.MiddleCenter;
                    GUI.color = ColoredText.SubtleGrayColor;
                    Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
                    GUI.color = Color.white;
                    Text.Anchor = TextAnchor.UpperLeft;
                }
                else
                {
                    GeneCategoryDef geneCategoryDef = null;
                    int num6 = 0;
                    for (int i = 0; i < genes.Count; i++)
                    {
                        GeneDef geneDef = genes[i];
                        if ((!adding || !this.quickSearchWidget.filter.Active || (this.matchingGenes.Contains(geneDef) && !this.selectedGenes.Contains(geneDef)) || this.matchingCategories.Contains(geneDef.displayCategory)) && (this.ignoreRestrictions || geneDef.biostatArc <= 0))
                        {
                            bool flag5 = false;
                            if ((double)num + (double)num3 > (double)num4)
                            {
                                num = 4f;
                                curY += (float)((double)GeneCreationDialogBase.GeneSize.y + 8.0 + 4.0);
                                flag5 = true;
                            }

                            bool flag6 = this.quickSearchWidget.filter.Active && (this.matchingGenes.Contains(geneDef) || this.matchingCategories.Contains(geneDef.displayCategory));
                            bool flag7 = this.collapsedCategories[geneDef.displayCategory] && !flag6;
                            if (adding && geneCategoryDef != geneDef.displayCategory)
                            {
                                if (!flag5 && flag4)
                                {
                                    num = 4f;
                                    curY += (float)((double)GeneCreationDialogBase.GeneSize.y + 8.0 + 4.0);
                                }

                                geneCategoryDef = geneDef.displayCategory;
                                Rect rect4 = new Rect(num, curY, rect.width - 8f, Text.LineHeight);
                                if (!flag6)
                                {
                                    Rect position2 = new Rect(rect4.x, rect4.y + (float)(((double)rect4.height - 18.0) / 2.0), 18f, 18f);
                                    GUI.DrawTexture(position2, flag7 ? TexButton.Reveal : TexButton.Collapse);
                                    if (Widgets.ButtonInvisible(rect4, true))
                                    {
                                        this.collapsedCategories[geneDef.displayCategory] = !this.collapsedCategories[geneDef.displayCategory];
                                        if (this.collapsedCategories[geneDef.displayCategory])
                                        {
                                            SoundDefOf.TabClose.PlayOneShotOnCamera(null);
                                        }
                                        else
                                        {
                                            SoundDefOf.TabOpen.PlayOneShotOnCamera(null);
                                        }
                                    }

                                    if (num6 % 2 == 1)
                                    {
                                        Widgets.DrawLightHighlight(rect4);
                                    }

                                    if (Mouse.IsOver(rect4))
                                    {
                                        Widgets.DrawHighlight(rect4);
                                    }

                                    rect4.xMin += position2.width;
                                }

                                Widgets.Label(rect4, geneCategoryDef.LabelCap);
                                curY += rect4.height;
                                if (!flag7)
                                {
                                    GUI.color = Color.grey;
                                    Widgets.DrawLineHorizontal(num, curY, rect.width - 8f);
                                    GUI.color = Color.white;
                                    curY += 10f;
                                }

                                num6++;
                            }

                            if (adding && flag7)
                            {
                                flag4 = false;
                                if (Event.current.type == EventType.Layout)
                                {
                                    sectionHeight = curY - num2;
                                }
                            }
                            else
                            {
                                num = Mathf.Max(num, b);
                                flag4 = true;
                                if (this.DrawGene(geneDef, !adding, ref num, curY, num3, containingRect, flag6))
                                {
                                    if (this.selectedGenes.Contains(geneDef))
                                    {
                                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                                        this.selectedGenes.Remove(geneDef);
                                    }
                                    else
                                    {
                                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                                        this.selectedGenes.Add(geneDef);
                                    }

                                    if (!this.xenotypeNameLocked)
                                    {
                                        this.xenotypeName = GeneUtility.GenerateXenotypeNameFromGenes(this.SelectedGenes);
                                    }

                                    this.OnGenesChanged();
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!adding || flag4)
                {
                    curY += GeneCreationDialogBase.GeneSize.y + 12f;
                }

                if (Event.current.type == EventType.Layout)
                {
                    sectionHeight = curY - num2;
                }
            }
        }


        private bool DrawGene(GeneDef geneDef, bool selectedSection, ref float curX, float curY, float packWidth, Rect containingRect, bool isMatch)
        {
            bool flag = false;
            Rect rect = new Rect(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
            bool result;
            if (!containingRect.Overlaps(rect))
            {
                curX = rect.xMax + 4f;
                result = false;
            }
            else
            {
                bool selected = !selectedSection && this.selectedGenes.Contains(geneDef);
                bool overridden = this.leftChosenGroups.Any((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef));
                Widgets.DrawOptionBackground(rect, selected);
                curX += 4f;
                GeneUIUtility.DrawBiostats(geneDef.biostatCpx, geneDef.biostatMet, geneDef.biostatArc, ref curX, curY, 4f);
                Rect rect2 = new Rect(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y);
                if (isMatch)
                {
                    Widgets.DrawStrongHighlight(rect2.ExpandedBy(6f), null);
                }

                GeneUIUtility.DrawGeneDef(geneDef, rect2, this.inheritable ? GeneType.Endogene : GeneType.Xenogene, () => this.GeneTip(geneDef, selectedSection), false, false, overridden);
                curX += GeneCreationDialogBase.GeneSize.x + 4f;
                if (Mouse.IsOver(rect))
                {
                    this.hoveredGene = geneDef;
                    this.hoveredAnyGene = true;
                }
                else if (this.hoveredGene != null && geneDef.ConflictsWith(this.hoveredGene))
                {
                    Widgets.DrawLightHighlight(rect);
                }

                if (Widgets.ButtonInvisible(rect, true))
                {
                    flag = true;
                }

                curX = Mathf.Max(curX, rect.xMax + 4f);
                result = flag;
            }

            return result;
        }


        private string GeneTip(GeneDef geneDef, bool selectedSection)
        {
            string text = null;
            if (selectedSection)
            {
                if (this.leftChosenGroups.Any((GeneLeftChosenGroup x) => x.leftChosen == geneDef))
                {
                    text = DialogXenoType.internalTest(this.leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == geneDef));
                }
                else if (this.cachedOverriddenGenes.Contains(geneDef))
                {
                    text = DialogXenoType.internalTest(this.leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef)));
                }
                else if (this.randomChosenGroups.ContainsKey(geneDef))
                {
                    text = ("GeneWillBeRandomChosen".Translate() + ":\n" + (from x in this.randomChosenGroups[geneDef]
                        select x.label).ToLineList("  - ", true)).Colorize(ColoredText.TipSectionTitleColor);
                }
            }

            if (this.selectedGenes.Contains(geneDef) && geneDef.prerequisite != null && !this.selectedGenes.Contains(geneDef.prerequisite))
            {
                if (!text.NullOrEmpty())
                {
                    text += "\n\n";
                }

                text += ("MessageGeneMissingPrerequisite".Translate(geneDef.label).CapitalizeFirst() + ": " + geneDef.prerequisite.LabelCap).Colorize(ColorLibrary.RedReadable);
            }

            if (!text.NullOrEmpty())
            {
                text += "\n\n";
            }

            return text + (this.selectedGenes.Contains(geneDef) ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
        }


        protected override void PostXenotypeOnGUI(float curX, float curY)
        {
            TaggedString taggedString = "GenesAreInheritable".Translate();
            TaggedString taggedString2 = "IgnoreRestrictions".Translate();
            float width = (float)((double)Mathf.Max(Text.CalcSize(taggedString).x, Text.CalcSize(taggedString2).x) + 4.0 + 24.0);
            Rect rect = new Rect(curX, curY, width, Text.LineHeight);
            Widgets.CheckboxLabeled(rect, taggedString, ref this.inheritable, false, null, null, false, false);
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                TooltipHandler.TipRegion(rect, "GenesAreInheritableDesc".Translate());
            }

            rect.y += Text.LineHeight;
            int num = this.ignoreRestrictions ? 1 : 0;
            Widgets.CheckboxLabeled(rect, taggedString2, ref this.ignoreRestrictions, false, null, null, false, false);
            int num2 = this.ignoreRestrictions ? 1 : 0;
            if (num != num2)
            {
                if (this.ignoreRestrictions)
                {
                    if (!DialogXenoType.ignoreRestrictionsConfirmationSent)
                    {
                        DialogXenoType.ignoreRestrictionsConfirmationSent = true;
                        WindowTool.Open(new Dialog_MessageBox("IgnoreRestrictionsConfirmation".Translate(), "Yes".Translate(), delegate() { }, "No".Translate(), delegate() { this.ignoreRestrictions = false; }, null, false, null, null, WindowLayer.Dialog));
                    }
                }
                else
                {
                    this.selectedGenes.RemoveAll((GeneDef x) => x.biostatArc > 0);
                    this.OnGenesChanged();
                }
            }

            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                TooltipHandler.TipRegion(rect, "IgnoreRestrictionsDesc".Translate());
            }

            this.postXenotypeHeight += rect.yMax - curY;
        }


        protected override void OnGenesChanged()
        {
            this.selectedGenes.SortGeneDefs();
            base.OnGenesChanged();
            if (this.predefinedXenoDef != null)
            {
                foreach (GeneDef item in this.predefinedXenoDef.AllGenes)
                {
                    if (!this.selectedGenes.Contains(item))
                    {
                        MessageTool.ShowInDebug("predefined unloaded");
                        this.predefinedXenoDef = null;
                        break;
                    }
                }

                int num = this.selectedGenes.CountAllowNull<GeneDef>();
                int num2 = this.predefinedXenoDef.AllGenes.CountAllowNull<GeneDef>();
                if (num != num2)
                {
                    MessageTool.ShowInDebug("predefined unloaded");
                    this.predefinedXenoDef = null;
                }
            }
        }


        private void ALoadCustomXenotype(CustomXenotype xenotype)
        {
            MessageTool.ShowInDebug("loading custom xenotype " + xenotype.name);
            this.predefinedXenoDef = null;
            this.xenotypeName = xenotype.name;
            this.xenotypeNameLocked = false;
            this.selectedGenes.Clear();
            this.selectedGenes.AddRange(xenotype.genes);
            this.inheritable = xenotype.inheritable;
            this.iconDef = xenotype.IconDef;
            this.OnGenesChanged();
            this.ignoreRestrictions = (this.selectedGenes.Any((GeneDef x) => x.biostatArc > 0) || !this.WithinAcceptableBiostatLimits(false));
        }


        private void ALoadXenotypeDef(XenotypeDef xenotype)
        {
            MessageTool.ShowInDebug("loading xenotypeDef " + xenotype.label);
            this.predefinedXenoDef = xenotype;
            this.xenotypeName = xenotype.label;
            this.xenotypeNameLocked = false;
            this.selectedGenes.Clear();
            this.selectedGenes.AddRange(xenotype.genes);
            this.inheritable = xenotype.inheritable;
            this.iconDef = XenotypeIconDefOf.Basic;
            this.OnGenesChanged();
            this.ignoreRestrictions = (this.selectedGenes.Any((GeneDef g) => g.biostatArc > 0) || !this.WithinAcceptableBiostatLimits(false));
        }


        protected void DoFileInteraction(string fileName)
        {
            string filePath = GenFilePaths.AbsFilePathForXenotype(fileName);
            PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.Xenotype, delegate
            {
                CustomXenotype xenotype;
                if (GameDataSaveLoader.TryLoadXenotype(filePath, out xenotype))
                {
                    this.ALoadCustomXenotype(xenotype);
                }
            }, false);
        }


        protected override void DrawSearchRect(Rect rect)
        {
            base.DrawSearchRect(rect);
            if (Widgets.ButtonText(new Rect(rect.xMax - GeneCreationDialogBase.ButSize.x, rect.y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "LoadCustom".Translate(), true, true, true, null))
            {
                WindowTool.Open(new Dialog_XenotypeList_Load(delegate(CustomXenotype xenotype) { this.ALoadCustomXenotype(xenotype); }));
            }

            if (Widgets.ButtonText(new Rect((float)((double)rect.xMax - (double)GeneCreationDialogBase.ButSize.x * 2.0 - 4.0), rect.y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "LoadPremade".Translate(), true, true, true, null))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (XenotypeDef xenotype2 in from c in DefDatabase<XenotypeDef>.AllDefs
                         orderby -c.displayPriority
                         select c)
                {
                    XenotypeDef xenotype = xenotype2;
                    list.Add(new FloatMenuOption(xenotype.LabelCap, delegate() { this.ALoadXenotypeDef(xenotype); }, xenotype.Icon, XenotypeDef.IconColor, MenuOptionPriority.Default, delegate(Rect r) { TooltipHandler.TipRegion(r, xenotype.descriptionShort ?? xenotype.description); }, null, 0f, null, null, true, 0, HorizontalJustification.Left, false));
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }
        }


        protected override void DoBottomButtons(Rect rect)
        {
            SZWidgets.ButtonText(new Rect(rect.xMax - GeneCreationDialogBase.ButSize.x - 10f, rect.y, GeneCreationDialogBase.ButSize.x + 10f, GeneCreationDialogBase.ButSize.y), this.AcceptButtonLabel, delegate() { this.ACheckSaveAnd(true); }, "");
            SZWidgets.ButtonText(new Rect(rect.x, rect.y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "Close".Translate(), delegate() { this.Close(true); }, "");
            SZWidgets.ButtonText(new Rect(rect.x + rect.width - 270f, rect.y, 110f, 38f), Label.SAVE, delegate() { this.ACheckSaveAnd(false); }, "");
            if (this.leftChosenGroups.Any<GeneLeftChosenGroup>())
            {
                int num = this.leftChosenGroups.Sum((GeneLeftChosenGroup x) => x.overriddenGenes.Count);
                GeneLeftChosenGroup geneLeftChosenGroup = this.leftChosenGroups[0];
                string text = "GenesConflict".Translate() + ": " + "GenesConflictDesc".Translate(geneLeftChosenGroup.leftChosen.Named("FIRST"), geneLeftChosenGroup.overriddenGenes[0].Named("SECOND")).CapitalizeFirst() + ((num > 1) ? (" +" + (num - 1).ToString()) : string.Empty);
                float x2 = Text.CalcSize(text).x;
                GUI.color = ColorLibrary.RedReadable;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(new Rect((float)((double)rect.xMax - (double)GeneCreationDialogBase.ButSize.x - (double)x2 - 4.0), rect.y, x2, rect.height), text);
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
            }
        }


        protected override bool CanAccept()
        {
            bool result;
            if (!base.CanAccept())
            {
                result = false;
            }
            else if (!this.selectedGenes.Any<GeneDef>())
            {
                result = true;
            }
            else
            {
                for (int i = 0; i < this.selectedGenes.Count; i++)
                {
                    if (this.selectedGenes[i].prerequisite != null && !this.selectedGenes.Contains(this.selectedGenes[i].prerequisite))
                    {
                        MessageTool.Show("MessageGeneMissingPrerequisite".Translate(this.selectedGenes[i].label).CapitalizeFirst() + ": " + this.selectedGenes[i].prerequisite.LabelCap, MessageTypeDefOf.RejectInput);
                        return false;
                    }
                }

                if (GenFilePaths.AllCustomXenotypeFiles.EnumerableCount() >= 200)
                {
                    MessageTool.Show("MessageTooManyCustomXenotypes".Translate(), MessageTypeDefOf.RejectInput);
                    result = false;
                }
                else if (this.ignoreRestrictions || !this.leftChosenGroups.Any<GeneLeftChosenGroup>())
                {
                    result = true;
                }
                else
                {
                    MessageTool.Show("MessageConflictingGenesPresent".Translate(), MessageTypeDefOf.RejectInput);
                    result = false;
                }
            }

            return result;
        }


        protected override void Accept()
        {
            this.ASaveAnd(true);
        }


        private void ACheckSaveAnd(bool apply)
        {
            if (this.CanAccept())
            {
                this.ASaveAnd(apply);
            }
        }


        private void ASaveAnd(bool use)
        {
            IEnumerable<string> warnings = this.GetWarnings();
            if (warnings.Any<string>())
            {
                WindowTool.Open(Dialog_MessageBox.CreateConfirmation("XenotypeBreaksLimits".Translate() + ":\n" + warnings.ToLineList("  - ", true) + "\n\n" + "SaveAnyway".Translate(), delegate() { this.AcceptInner(use); }, false, null, WindowLayer.Dialog));
                return;
            }

            this.AcceptInner(use);
        }


        private void AcceptInner(bool saveAndUse)
        {
            if (this.xenotypeName.NullOrEmpty())
            {
                MessageTool.ShowInDebug("please choose a xenotype name!");
                return;
            }

            CustomXenotype customXenotype = new CustomXenotype();
            CustomXenotype customXenotype2 = customXenotype;
            string xenotypeName = this.xenotypeName;
            customXenotype2.name = ((xenotypeName != null) ? xenotypeName.Trim() : null);
            customXenotype.genes.AddRange(this.selectedGenes);
            customXenotype.inheritable = this.inheritable;
            customXenotype.iconDef = this.iconDef;
            string absPath = GenFilePaths.AbsFilePathForXenotype(GenFile.SanitizedFileName(customXenotype.name));
            LongEventHandler.QueueLongEvent(delegate() { GameDataSaveLoader.SaveXenotype(customXenotype, absPath); }, "SavingLongEvent", false, null, true, null);
            if (saveAndUse)
            {
                this.pawn.SetPawnXenotype(customXenotype, !this.inheritable);
            }

            this.Close(true);
        }


        private IEnumerable<string> GetWarnings()
        {
            if (this.ignoreRestrictions)
            {
                if (this.arc > 0 && this.inheritable)
                {
                    yield return "XenotypeBreaksLimits_Archites".Translate();
                }

                if (this.met > GeneTuning.BiostatRange.TrueMax)
                {
                    yield return "XenotypeBreaksLimits_Exceeds".Translate("Metabolism".Translate().ToLower().Named("STAT"), this.met.Named("VALUE"), GeneTuning.BiostatRange.TrueMax.Named("MAX"));
                }
                else if (this.met < GeneTuning.BiostatRange.TrueMin)
                {
                    yield return "XenotypeBreaksLimits_Below".Translate("Metabolism".Translate().ToLower().Named("STAT"), this.met.Named("VALUE"), GeneTuning.BiostatRange.TrueMin.Named("MIN"));
                }
            }

            yield break;
        }


        protected override void UpdateSearchResults()
        {
            this.quickSearchWidget.noResultsMatched = false;
            this.matchingGenes.Clear();
            this.matchingCategories.Clear();
            if (this.quickSearchWidget.filter.Active)
            {
                foreach (GeneDef geneDef in GeneUtility.GenesInOrder)
                {
                    if (!this.selectedGenes.Contains(geneDef))
                    {
                        if (this.quickSearchWidget.filter.Matches(geneDef.label))
                        {
                            this.matchingGenes.Add(geneDef);
                        }

                        if (this.quickSearchWidget.filter.Matches(geneDef.displayCategory.label) && !this.matchingCategories.Contains(geneDef.displayCategory))
                        {
                            this.matchingCategories.Add(geneDef.displayCategory);
                        }
                    }
                }

                this.quickSearchWidget.noResultsMatched = (!this.matchingGenes.Any<GeneDef>() && !this.matchingCategories.Any<GeneCategoryDef>());
            }
        }


        static DialogXenoType()
        {
        }


        private static string internalTest(GeneLeftChosenGroup group)
        {
            if (group == null)
            {
                return null;
            }

            return ("GeneLeftmostActive".Translate() + ":\n  - " + group.leftChosen.LabelCap + " (" + "Active".Translate() + ")" + "\n" + (from x in @group.overriddenGenes
                select (x.label + " (" + "Suppressed".Translate() + ")").Colorize(ColorLibrary.RedReadable)).ToLineList("  - ", true)).Colorize(ColoredText.TipSectionTitleColor);
        }


        private List<GeneDef> selectedGenes = new List<GeneDef>();


        private bool inheritable;


        private bool? selectedCollapsed = new bool?(false);


        private List<GeneCategoryDef> matchingCategories = new List<GeneCategoryDef>();


        private Dictionary<GeneCategoryDef, bool> collapsedCategories = new Dictionary<GeneCategoryDef, bool>();


        private bool hoveredAnyGene;


        private GeneDef hoveredGene;


        private static bool ignoreRestrictionsConfirmationSent;


        private const int MaxCustomXenotypes = 200;


        private static readonly Color OutlineColorSelected = new Color(1f, 1f, 0.7f, 1f);


        private Pawn pawn;


        private XenotypeDef predefinedXenoDef;


        private bool doOnce;
    }
}
