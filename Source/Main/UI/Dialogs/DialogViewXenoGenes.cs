// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogViewXenoGenes
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

internal class DialogViewXenoGenes : Window
{
    private const float HeaderHeight = 30f;
    private bool bDoOnce;
    private bool bIsXeno;
    private List<GeneDef> lallgenes;
    private List<CustomXenotype> lcustomxeontypes;
    private readonly List<XenotypeDef> lxenotypes;
    private Vector2 scrollPosition;
    private readonly Pawn target;

    internal DialogViewXenoGenes(Pawn target)
    {
        this.target = target;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnCancel = true;
        closeOnClickedOutside = true;
        draggable = true;
        layer = CEditor.Layer;
        bDoOnce = true;
        SearchTool.Update(SearchTool.SIndex.ViewXenoType);
        lallgenes = DefDatabase<GeneDef>.AllDefs.OrderBy(x => x.defName).ToList();
        lxenotypes = DefDatabase<XenotypeDef>.AllDefs.Where(x => !x.defName.NullOrEmpty()).OrderBy(x => -x.displayPriority).ToList();
        InitCustomList();
    }

    public override Vector2 InitialSize => new(736f, WindowTool.MaxHS);

    private void InitCustomList()
    {
        lcustomxeontypes = GeneTool.GetAllCustomXenotypes();
    }

    public override void PostOpen()
    {
        if (!ModsConfig.BiotechActive)
            base.Close(false);
        else
            base.PostOpen();
    }

    public override void DoWindowContents(Rect inRect)
		{
			bool flag = this.bDoOnce;
			if (flag)
			{
				SearchTool.SetPosition(SearchTool.SIndex.ViewXenoType, ref this.windowRect, ref this.bDoOnce, 370);
			}
			inRect.yMax -= Window.CloseButSize.y;
			GUI.color = Color.white;
			inRect.yMin += 34f;
			Vector2 zero = Vector2.zero;
			int num = (int)inRect.x;
			int num2 = (int)inRect.height + 34;
			SZWidgets.ButtonImageCol(new Rect((float)num, 0f, 30f, 30f), this.bIsXeno ? "bdnax" : "bdnae", delegate()
			{
				this.bIsXeno = !this.bIsXeno;
			}, Color.white, Label.TOGGLEENDOXENO);
			num += 32;
			Rect rect = new Rect((float)num, 0f, 30f, 30f);
			SZWidgets.Image(rect, this.bIsXeno ? "UI/Icons/Genes/GeneBackground_Xenogene" : "UI/Icons/Genes/GeneBackground_Endogene");
			SZWidgets.ButtonImage(rect, "bplus2", new Action(this.AOpenAddDialog), "");
			num += 32;
			Rect rect2 = new Rect((float)num, 0f, 30f, 30f);
			SZWidgets.Image(rect2, this.bIsXeno ? "UI/Icons/Genes/GeneBackground_Xenogene" : "UI/Icons/Genes/GeneBackground_Endogene");
			SZWidgets.FloatMenuOnButtonImage<Gene>(rect2, ContentFinder<Texture2D>.Get("bminus2", true), this.bIsXeno ? this.target.genes.Xenogenes : this.target.genes.Endogenes, (Gene gene) => gene.LabelCap.ToString(), new Action<Gene>(this.ARemoveEndoGene), "");
			num += 32;
			Rect rect3 = new Rect((float)num, 0f, 30f, 30f);
			SZWidgets.Image(rect3, this.bIsXeno ? "UI/Icons/Genes/GeneBackground_Xenogene" : "UI/Icons/Genes/GeneBackground_Endogene");
			SZWidgets.ButtonImage(rect3, "breset", new Action(this.AResetGenes), Label.TIPCLEARGENES);
			num += 32;
			Rect rect4 = new Rect((float)num, 0f, 30f, 30f);
			SZWidgets.Image(rect4, this.bIsXeno ? "UI/Icons/Genes/GeneBackground_Xenogene" : "UI/Icons/Genes/GeneBackground_Endogene");
			SZWidgets.FloatMixedMenuOnButtonImage<XenotypeDef, CustomXenotype>(rect4, this.target.genes.XenotypeIcon, this.lxenotypes, this.lcustomxeontypes, (XenotypeDef xt) => xt.LabelCap.ToString(), (CustomXenotype c) => c.name, new Action<XenotypeDef>(this.AChangeXenotype), new Action<CustomXenotype>(this.ALoadCustomXenotype), Label.LOADXENOTYPEKEEP);
			num += 32;
			Text.Font = GameFont.Medium;
			SZWidgets.Label(new Rect(170f, 0f, 400f, 30f), Label.GENETICS + " - " + this.target.GetPawnNameColored(true), null, "");
			Text.Font = GameFont.Small;
			GeneUIUtility.DrawGenesInfo(inRect, this.target, this.InitialSize.y, ref zero, ref this.scrollPosition, null);
			WindowTool.SimpleCloseButton(this);
			Rect rect5 = WindowTool.RAcceptButton(this);
			rect5.y -= 80f;
			rect5.x -= 50f;
			rect5.width += 50f;
			rect5.height = 50f;
			bool flag2 = Mouse.IsOver(rect5);
			if (flag2)
			{
				TooltipHandler.TipRegion(rect5, GeneTool.PrintIfXenotypeIsPrefered(this.target));
			}
		}


    private void ALoadCustomXenotype(CustomXenotype c)
    {
        target.SetPawnXenotype(c, bIsXeno);
        GeneTool.PrintIfXenotypeIsPrefered(target);
    }

    private void AChangeXenotype(XenotypeDef def)
    {
        target.SetPawnXenotype(def, bIsXeno);
        GeneTool.PrintIfXenotypeIsPrefered(target);
    }

    private void AResetGenes()
    {
        target.ClearGenes(bIsXeno, Event.current.control);
        CEditor.API.UpdateGraphics();
    }

    private void AOpenAddDialog()
    {
        WindowTool.Open(new DialogGenery(bIsXeno));
    }

    private void ARemoveEndoGene(Gene gene)
    {
        target.RemoveGeneKeepFirst(gene);
        GeneTool.PrintIfXenotypeIsPrefered(CEditor.API.Pawn);
        CEditor.API.UpdateGraphics();
    }

    public override void Close(bool doCloseSound = true)
    {
	    SearchTool.Save(SearchTool.SIndex.ViewXenoType, this.windowRect.position);
	    base.Close(doCloseSound);
    }
}
