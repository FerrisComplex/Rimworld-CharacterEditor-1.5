// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogChangeHeadAddons
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogChangeHeadAddons : Window
{
    private object[] aAddons;
    private int[] aOldAddonVariants;
    private bool doOnce;
    private List<Graphic> lAddonGraphics;
    private List<int> lAddonVariants;
    private List<int> lMax;
    private List<int> lOldAddonVariants;
    private List<string> lPaths;
    private string paramName;
    private Vector2 scrollPosParam;
    private Pawn tempPawn;
    private Listing_X view;

    internal DialogChangeHeadAddons()
    {
        doCloseX = true;
        absorbInputAroundWindow = false;
        draggable = true;
        layer = CEditor.Layer;
        doOnce = true;
        SearchTool.Update(SearchTool.SIndex.ChangeHeadAddons);
        Init();
    }

    public override Vector2 InitialSize => new(400f, WindowTool.MaxH);

    internal void Init()
    {
        scrollPosParam = new Vector2();
        paramName = null;
        view = new Listing_X();
        tempPawn = CEditor.API.Pawn;
        lMax = new List<int>();
        lPaths = new List<string>();
        lAddonVariants = tempPawn.AlienRaceComp_GetAddonVariants();
        lAddonGraphics = tempPawn.AlienRaceComp_GetAddonGraphics();
        if (!lAddonGraphics.NullOrEmpty())
            for (var index = 0; index < lAddonGraphics.Count; ++index)
            {
                var str = lAddonGraphics[index].path;
                var s = str.Substring(str.Length - 1);
                var result = 0;
                if (int.TryParse(s, out result))
                    str = str.Substring(0, str.Length - 1);
                lPaths.Add(str);
                do
                {
                    ++result;
                } while (TextureTool.TestTexturePath(str + result + "_south", false));

                --result;
                lMax.Add(result);
            }

        aOldAddonVariants = new int[lAddonVariants.Count];
        lOldAddonVariants = new List<int>();
        aAddons = tempPawn.AlienPartGenerator_GetBodyAddonsAsArray();
    }

    public override void DoWindowContents(Rect inRect)
    {
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.ChangeHeadAddons, ref this.windowRect, ref this.doOnce, 0);
        }
        bool flag2 = this.tempPawn.ThingID != CEditor.API.Pawn.ThingID;
        if (flag2)
        {
            this.Init();
        }
        float x = 0f;
        float num = 0f;
        float num2 = this.InitialSize.x - 16f;
        float num3 = this.InitialSize.y - 16f;
        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(x, num, num2, 30f), Label.HEAD_ADDONS);
        num += 30f;
        Text.Font = GameFont.Small;
        Widgets.Label(new Rect(x, num, num2 - 20f, 70f), Label.ADDONS_INFO + Label.ATTENTION.Colorize(ColorTool.colRed) + Label.ADDONS_INFO2);
        num += 70f;
        this.DrawAddons(x, num, num2 - 20f, num3 - 180f);
        WindowTool.SimpleCloseButton(this);
    }

    private void DrawAddons(float x, float y, float w, float h)
    {
        bool flag = this.lAddonVariants.NullOrEmpty<int>();
        if (!flag)
        {
            int num = this.lAddonVariants.Count * 155;
            Rect outRect = new Rect(x, y, w, h);
            Rect rect = new Rect(0f, 0f, outRect.width - 16f, (float)num);
            Widgets.BeginScrollView(outRect, ref this.scrollPosParam, rect, true);
            Rect rect2 = rect.ContractedBy(4f);
            rect2.y -= 4f;
            rect2.height = (float)num;
            this.view.Begin(rect2);
            this.view.verticalSpacing = 30f;
            Text.Font = GameFont.Small;
            this.lAddonVariants.CopyTo(this.aOldAddonVariants);
            this.lOldAddonVariants = this.aOldAddonVariants.ToList<int>();
            for (int i = 0; i < this.lAddonVariants.Count; i++)
            {
                this.DrawAddon(i, w);
            }
            this.view.End();
            Widgets.EndScrollView();
            bool flag2 = this.tempPawn.ThingID == CEditor.API.Pawn.ThingID;
            if (flag2)
            {
                bool flag3 = !this.lOldAddonVariants.SequenceEqual(this.lAddonVariants);
                if (flag3)
                {
                    this.tempPawn.AlienRaceComp_SetAddonVariants(this.lAddonVariants);
                    CEditor.API.UpdateGraphics();
                }
            }
        }
    }
    

    private void DrawAddon(int i, float w)
    {
        if (tempPawn.ThingID != CEditor.API.Pawn.ThingID)
            return;
        try
        {
            var path = AlienRaceTool.BodyAddon_GetPath(aAddons[i]);
            if (path.NullOrEmpty())
                path = lAddonGraphics[i].path;
            var paramName = path.Colorize(ColorTool.colBeige);
            var lAddonVariant = lAddonVariants[i];
            var variantCountMax = AlienRaceTool.BodyAddon_GetVariantCountMax(aAddons[i]);
            if (variantCountMax == 0)
                variantCountMax = lMax[i];
            view.AddIntSection(paramName, "", ref this.paramName, ref lAddonVariant, 0, variantCountMax, true);
            lAddonVariants[i] = lAddonVariant;
            view.ButtonImage(0.0f, 0.0f, 20f, 20f, "bfemale", AlienRaceTool.BodyAddon_GetDrawForFemale(aAddons[i]) ? Color.white : Color.gray, ARemoveAddonFemale, i);
            view.ButtonImage(22f, 0.0f, 20f, 20f, "bmale", AlienRaceTool.BodyAddon_GetDrawForMale(aAddons[i]) ? Color.white : Color.gray, ARemoveAddonMale, i);
            var drawSize = AlienRaceTool.BodyAddon_GetDrawSize(aAddons[i]);
            var x = drawSize.x;
            view.AddSection("            " + Label.DRAWSIZE, "", ref this.paramName, ref x, 0.0f, 2f, true);
            if ((double)x != drawSize.x)
                AOnDrawSizeChanged(i, x);
            var rotation = AlienRaceTool.BodyAddon_GetRotation(aAddons[i]);
            var num = (int)rotation;
            view.AddIntSection(Label.ROTATION, "", ref this.paramName, ref num, -360, 360, true);
            if (num != (int)rotation)
                AOnRotationChanged(i, num);
            view.Gap(5f);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message + "\n" + ex.StackTrace);
        }
    }

    private void AOnDrawSizeChanged(int i, float val)
    {
        AlienRaceTool.BodyAddon_SetDrawSize(aAddons[i], val);
        CEditor.API.UpdateGraphics();
    }

    private void AOnRotationChanged(int i, float val)
    {
        AlienRaceTool.BodyAddon_SetRotation(aAddons[i], val);
        CEditor.API.UpdateGraphics();
    }

    private void ARemoveAddonMale(int i)
    {
        tempPawn.AlienPartGenerator_BodyAddon_Toggle_DrawFor(i, false);
        CEditor.API.UpdateGraphics();
    }

    private void ARemoveAddonFemale(int i)
    {
        tempPawn.AlienPartGenerator_BodyAddon_Toggle_DrawFor(i, true);
        CEditor.API.UpdateGraphics();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.ChangeHeadAddons, this.windowRect.position);
        base.Close(doCloseSound);
    }

    public override void OnAcceptKeyPressed()
    {
        base.OnAcceptKeyPressed();
        base.Close();
    }
}
