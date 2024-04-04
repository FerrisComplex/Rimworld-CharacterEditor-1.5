// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogColorPicker
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogColorPicker : Window
{
    private const int wLeft = 252;
    private const int wRight = 240;
    private bool bInstantClose;
    private Color closestGeneColr = Color.white;
    private GeneDef closestGeneDef;
    private readonly ColorType colorType;
    private bool doOnce;
    private float fAlpha;
    private float fBlue;
    private float fGreen;
    private float fMaxBright;
    private float fMinBright;
    private float fRed;
    private int iAlpha;
    private int iBlue;
    private int iGreen;
    private int iRed;
    private bool isColor1Choosen;
    private readonly bool isPrimaryColor;
    private string oldHex;
    private Color oldSelected;
    private string oldsRGB;
    private double part;
    private readonly Regex regexColor;
    private readonly Regex regexHex;
    private Vector2 scrollPos;
    private Apparel selectedApparel;
    private Color selectedColor;
    private ThingDef selectedDef;
    private GeneDef selectedGeneDef;
    private ThingWithComps selectedWeapon;
    private string sHex;
    private string sRGB;
    private Pawn tempPawn;
    private int Xwidth;

    internal DialogColorPicker(
        ColorType _colorType,
        bool _primaryColor = true,
        Apparel a = null,
        ThingWithComps w = null,
        GeneDef g = null)
    {
        doCloseX = true;
        absorbInputAroundWindow = false;
        draggable = true;
        layer = CEditor.Layer;
        doOnce = true;
        SearchTool.Update(SearchTool.SIndex.ColorPicker);
        isPrimaryColor = _primaryColor;
        isColor1Choosen = isPrimaryColor;
        regexColor = new Regex("^[0-9,]*");
        regexHex = new Regex("^[0-9A-F]*");
        scrollPos = new Vector2();
        colorType = _colorType;
        Init(a, w, g);
    }

    public override Vector2 InitialSize => new(288f, WindowTool.MaxH);

    private int GetWindowWidth()
    {
        return colorType != ColorType.SkinColor && colorType != ColorType.FavColor && colorType != ColorType.EyeColor && colorType != ColorType.GeneColorHair && colorType != ColorType.GeneColorSkinBase && colorType != ColorType.GeneColorSkinOverride ? 528 : 288;
    }

    internal bool Preselect(Apparel a, ThingWithComps w, GeneDef g)
    {
        if (colorType == ColorType.SkinColor)
        {
            if (!tempPawn.HasStoryTracker())
                return false;
            selectedColor = tempPawn.GetSkinColor(isPrimaryColor);
        }
        else if (colorType == ColorType.HairColor)
        {
            if (!tempPawn.HasStoryTracker())
                return false;
            HairTool.ASelectedHairModName(HairTool.selectedHairModName);
            StyleTool.ASelectedBeardModName(StyleTool.selectedBeardModName);
            selectedColor = tempPawn.GetHairColor(isPrimaryColor);
        }
        else if (colorType == ColorType.FavColor)
        {
            if (!tempPawn.HasStoryTracker())
                return false;
            selectedColor = tempPawn.story.favoriteColor.GetValueOrDefault();
        }
        else if (colorType == ColorType.GeneColorHair)
        {
            if (g == null)
                return false;
            selectedGeneDef = g;
            selectedColor = g.hairColorOverride ?? Color.white;
        }
        else if (colorType == ColorType.GeneColorSkinBase)
        {
            if (g == null)
                return false;
            selectedGeneDef = g;
            selectedColor = g.skinColorBase ?? Color.white;
        }
        else if (colorType == ColorType.GeneColorSkinOverride)
        {
            if (g == null)
                return false;
            selectedGeneDef = g;
            selectedColor = g.skinColorOverride ?? Color.white;
        }
        else if (colorType == ColorType.ApparelColor)
        {
            if (!tempPawn.HasApparelTracker())
                return false;
            this.selectedApparel = ((a != null) ? a : this.tempPawn.apparel.WornApparel.FirstOrFallback(null));
            selectedDef = selectedApparel?.def;
            selectedColor = selectedApparel == null ? Color.white : selectedApparel.DrawColor;
            if (selectedApparel != null && (selectedApparel.def.colorGenerator == null || !selectedApparel.def.HasComp(typeof(CompColorable))))
                MessageTool.ShowActionDialog(Label.INFOD_APPAREL, () => ApparelTool.MakeThingColorable(selectedApparel.def), Label.INFOT_MAKECOLORABLE);
        }
        else if (colorType == ColorType.WeaponColor)
        {
            if (!tempPawn.HasEquipmentTracker())
                return false;
            selectedWeapon = w != null ? w : tempPawn.equipment.Primary;
            selectedDef = selectedWeapon?.def;
            selectedColor = selectedWeapon == null ? Color.white : selectedWeapon.DrawColor;
        }
        else
        {
            selectedColor = colorType != ColorType.EyeColor ? Color.white : tempPawn.GetEyeColor();
        }

        return true;
    }

    private void Init(Apparel a, ThingWithComps w, GeneDef g)
    {
        tempPawn = CEditor.API.Pawn;
        selectedApparel = null;
        selectedDef = null;
        selectedGeneDef = null;
        Xwidth = GetWindowWidth();
        bInstantClose = false;
        doOnce = true;
        if (!Preselect(a, w, g))
        {
            bInstantClose = true;
        }
        else
        {
            bInstantClose = false;
            fMinBright = 0.0f;
            fMaxBright = ColorTool.FMAX;
            fRed = selectedColor.r;
            fBlue = selectedColor.b;
            fGreen = selectedColor.g;
            fAlpha = selectedColor.a;
            part = ColorTool.DMAX / ColorTool.DMAXB;
            TextValuesFromSelectedColor();
        }
    }

    public override void DoWindowContents(Rect inRect)
    {
        bool flag = this.bInstantClose;
        if (flag)
        {
            this.Close(true);
        }
        else
        {
            bool flag2 = this.doOnce;
            if (flag2)
            {
                SearchTool.SetPosition(SearchTool.SIndex.ColorPicker, ref this.windowRect, ref this.doOnce, 370);
                this.windowRect.width = (float)this.Xwidth;
            }
            bool flag3 = this.tempPawn.ThingID != CEditor.API.Pawn.ThingID;
            if (flag3)
            {
                this.Init(null, null, null);
            }
            int num = 0;
            this.DrawRadioButtons(0, num);
            this.DrawTitle(0, num, 252, 30);
            num += 40;
            this.DrawColorTable(0, num, 252, 20);
            num += 280;
            this.DrawDerivedColors(0, num, 252, 18);
            num += 37;
            this.DrawColorSlider(0, num, 252, 230);
            num += 190;
            this.DrawTextFields(0, num, 252, 24);
            num += 72;
            this.DrawGeneColors(0, num, 252, 30);
            this.DrawLists(268, 0, 240, (int)this.InitialSize.y - 110);
            WindowTool.SimpleCloseButton(this);
            WindowTool.TopLayerForWindowOf<Dialog_MessageBox>(true);
        }
    }

    private void DrawTitle(int x, int y, int w, int h)
    {
        Text.Font = (GameFont)2;
        if (colorType == ColorType.FavColor || colorType == ColorType.GeneColorHair || colorType == ColorType.GeneColorSkinBase || colorType == ColorType.GeneColorSkinOverride || !tempPawn.HasStoryTracker() || !tempPawn.story.favoriteColor.HasValue)
            return;
        SZWidgets.ButtonTextureTextHighlight2(new Rect(w - 30, y, 30f, 30f), "", "bfavcolor", tempPawn.story.favoriteColor.GetValueOrDefault(), AFromFavColor, CompatibilityTool.GetFavoriteColorTooltip(tempPawn));
    }

    private void AFromFavColor()
    {
        AColorSelected(tempPawn.story.favoriteColor.GetValueOrDefault());
    }

    private void DrawColorTable(int x, int y, int w, int h)
    {
        Widgets.DrawBoxSolid(new Rect(x, y, w, h), selectedColor);
        y += h;
        var index1 = 0;
        var num = 63;
        for (var index2 = 0; index2 < 4; ++index2)
        {
            for (var index3 = 0; index3 < 13; ++index3)
            {
                SZWidgets.ButtonImageCol(x, y, num, h, "bwhite", AColorSelected, ColorTool.ListOfColors.ElementAtOrDefault(index1));
                y += h;
                ++index1;
            }

            x += num;
            y -= 13 * h;
        }
    }

    private void DrawDerivedColors(int x, int y, int w, int h)
    {
        var offset = 0.15f;
        for (var index = 0; index < 14; ++index)
        {
            SZWidgets.ButtonImageCol(x, y, h, h, "bwhite", AColorSelected, ColorTool.GetDerivedColor(selectedColor, offset));
            x += h;
            offset -= 0.03f;
        }
    }

    private void DrawColorSlider(int x, int y, int w, int h)
    {
        var listingX = new Listing_X();
        ((Listing)listingX).Begin(new Rect(x, y, w, h));
        fRed = listingX.Slider(selectedColor.r, 0.0f, ColorTool.IMAX, Color.red);
        fGreen = listingX.Slider(selectedColor.g, 0.0f, ColorTool.IMAX, Color.green);
        fBlue = listingX.Slider(selectedColor.b, 0.0f, ColorTool.IMAX, Color.blue);
        fAlpha = listingX.Slider(selectedColor.a, 0.0f, ColorTool.IMAX, Color.white);
        selectedColor = new Color(fRed, fGreen, fBlue, fAlpha);
        listingX.Gap(2f);
        GUI.color = Color.gray;
        listingX.Label(Label.MIN_RANDOM_BRIGHTNESS);
        listingX.CurY -= 5f;
        fMinBright = listingX.Slider(fMinBright, 0.0f, ColorTool.FMAX);
        listingX.CurY -= 5f;
        listingX.Label(Label.MAX_RANDOM_BRIGHTNESS);
        listingX.CurY -= 5f;
        if (fMaxBright < (double)fMinBright)
            fMaxBright = fMinBright;
        fMaxBright = listingX.Slider(fMaxBright, 0.0f, ColorTool.FMAX);
        if (ColorTool.offsetCX != 1.0 - fMaxBright)
        {
            ColorTool.offsetCX = 1f - fMaxBright;
            ColorTool.lcolors = null;
        }

        ((Listing)listingX).End();
    }

    private void DrawTextFields(int x, int y, int w, int h)
    {
        if (oldSelected != selectedColor)
            TextValuesFromSelectedColor();
        sRGB = Widgets.TextField(new Rect(x, y, w - 40, h), sRGB, 15, regexColor);
        SZWidgets.ButtonImage(x + w - 30, y, 25f, 25f, "brandom", ARandomColor, Label.TIP_DLG_RANDOM_COLOR, new Color());
        y += 37;
        GUI.color = Color.gray;
        sHex = Widgets.TextField(new Rect(x, y, w - 40, h), sHex, 15, regexHex);
        if (colorType == ColorType.HairColor)
            SZWidgets.ButtonImage(x + w - 30, y, 25f, 25f, "brandom", ARandomizeHairAndColor, Label.TIP_DLG_RANDOMIZE_HAIRANDCOLOR, new Color());
        if (!string.IsNullOrEmpty(sRGB) && oldsRGB != sRGB)
            RGBTextToSelectedColor();
        if (string.IsNullOrEmpty(sHex) || !(oldHex != sHex))
            return;
        HEXTextToSelectedColor();
    }

    private void DrawGeneColors(int x, int y, int w, int h)
    {
        bool flag = !ModsConfig.BiotechActive || !this.tempPawn.HasGeneTracker();
        if (!flag)
        {
            List<Gene> list = (this.colorType == ColorType.HairColor) ? this.tempPawn.GetHairGenes() : this.tempPawn.GetSkinGenes();
            int count = list.Count;
            for (int i = 0; i < 8; i++)
            {
                bool flag2 = count > i;
                if (flag2)
                {
                    Gene g = list[i];
                    bool flag3 = this.tempPawn.genes.Xenogenes.Contains(g);
                    Color col = g.def.IconColor;
                    Rect rect = new Rect((float)x, (float)y, (float)h, (float)h);
                    SZWidgets.Image(rect, flag3 ? "UI/Icons/Genes/GeneBackground_Xenogene" : "UI/Icons/Genes/GeneBackground_Endogene");
                    SZWidgets.ButtonHighlight(rect, g.def.iconPath, delegate(Color a)
                    {
                        this.AColorSelectedByGene(col, g);
                    }, col, "select color from gene\n[CTRL]remove gene");
                }
                x += h;
            }
            bool flag4 = this.closestGeneDef != null && (this.colorType == ColorType.HairColor || this.colorType == ColorType.SkinColor);
            if (flag4)
            {
                List<Gene> list2 = (from td in list
                    where td.def == this.closestGeneDef
                    select td).ToList<Gene>();
                bool flag5 = list2.NullOrEmpty<Gene>();
                if (flag5)
                {
                    Rect rect2 = new Rect(0f, (float)(y + h + 5), (float)h, (float)h);
                    SZWidgets.Image(rect2, "UI/Icons/Genes/GeneBackground_Endogene");
                    SZWidgets.ButtonHighlight(rect2, this.closestGeneDef.iconPath, delegate(Color a)
                    {
                        this.AGeneSelectedByColor(this.closestGeneColr, this.closestGeneDef);
                    }, this.closestGeneColr, "add as new gene");
                }
            }
        }
    }

    private void CalcClosestGeneColor()
    {
        if (!ModsConfig.BiotechActive || !tempPawn.HasGeneTracker())
            return;
        closestGeneDef = GeneTool.ClosestColorGene(selectedColor, colorType == ColorType.HairColor);
        closestGeneColr = closestGeneDef.IconColor;
    }

    private void AGeneSelectedByColor(Color color, GeneDef geneDef)
    {
        selectedColor = color;
        if (colorType != ColorType.HairColor && colorType != ColorType.SkinColor)
            return;
        tempPawn.AddGeneAsFirst(geneDef, false);
    }

    private void AColorSelected(Color color)
    {
        selectedColor = color;
    }

    private void DrawRadioButtons(int x, int y)
    {
        if (Widgets.RadioButtonLabeled(new Rect(x, y, 90f, 30f), Label.COLORA, isColor1Choosen))
        {
            isColor1Choosen = true;
            if (colorType == ColorType.HairColor)
                selectedColor = tempPawn.story.HairColor;
            else if (colorType == ColorType.SkinColor)
                selectedColor = tempPawn.GetSkinColor(true);
        }

        x += 100;
        if ((colorType != ColorType.SkinColor && (!CEditor.IsGradientHairActive || colorType != ColorType.HairColor)) || !Widgets.RadioButtonLabeled(new Rect(x, y, 90f, 30f), Label.COLORB, !isColor1Choosen))
            return;
        isColor1Choosen = false;
        if (colorType == ColorType.HairColor)
            selectedColor = tempPawn.GetHairColor(false);
        else if (colorType == ColorType.SkinColor)
            selectedColor = tempPawn.GetSkinColor(false);
    }

    private void DrawLists(int x, int y, int w, int h)
    {
        Text.Font = GameFont.Small;
        Rect rect = new Rect((float)x, (float)y, (float)w, (float)h);
        bool flag = this.colorType == ColorType.HairColor;
        if (flag)
        {
            SZWidgets.ButtonTextureTextHighlight2(new Rect((float)(x + 5), (float)y, (float)(w - 26), 28f), HairTool.onMouseover ? Label.SETONMOUSEOVER : Label.SETONCLICK, null, Color.white, delegate
            {
                HairTool.onMouseover = !HairTool.onMouseover;
            }, Label.TOGGLESELECTIONONMOUSEOVER, true);
            y += 30;
            this.DrawHairSelector(x - 16, ref y, w + 11, h);
            this.DrawBeardSelector(x - 16, ref y, w + 11, h);
        }
        else
        {
            bool flag2 = this.colorType == ColorType.ApparelColor;
            if (flag2)
            {
                List<ThingDef> l = (from td in this.tempPawn.apparel.WornApparel
                    select td.def).ToList<ThingDef>();
                SZWidgets.ListView<ThingDef>(rect, l, (ThingDef apparel) => apparel.label, (ThingDef apparel) => apparel.DescriptionDetailed, (ThingDef appA, ThingDef appB) => appA == appB, ref this.selectedDef, ref this.scrollPos, false, new Action<ThingDef>(this.AApparelSelected), true, false, false, false);
            }
            else
            {
                bool flag3 = this.colorType == ColorType.WeaponColor;
                if (flag3)
                {
                    List<ThingDef> l2 = (from td in this.tempPawn.equipment.AllEquipmentListForReading
                        select td.def).ToList<ThingDef>();
                    SZWidgets.ListView<ThingDef>(rect, l2, (ThingDef weapon) => weapon.label, (ThingDef weapon) => weapon.DescriptionDetailed, (ThingDef wA, ThingDef wB) => wA == wB, ref this.selectedDef, ref this.scrollPos, false, new Action<ThingDef>(this.AWeaponSelected), true, false, false, false);
                }
            }
        }
    }

    private void DrawHairSelector(int x, ref int y, int w, int h)
    {
        bool flag = !this.tempPawn.HasStoryTracker();
        if (!flag)
        {
            Rect rect = new Rect((float)x, (float)y, (float)w, 24f);
            SZWidgets.NavSelectorImageBox(rect, new Action(HairTool.AChooseHairCustom), new Action(HairTool.ARandomHair), null, null, null, new Action(HairTool.AConfigHair), Label.FRISUR + " - " + CEditor.API.Pawn.GetHairName(), null, Label.TIP_RANDOM_HAIR, null, null, default(Color), Label.CLICKTOOPENLIST);
            y += 30;
            bool isHairConfigOpen = HairTool.isHairConfigOpen;
            if (isHairConfigOpen)
            {
                int num = (int)this.InitialSize.y - 83 - y;
                SZWidgets.FloatMenuOnButtonText<string>(new Rect((float)(x + 16), (float)y, (float)(w - 32), 25f), HairTool.selectedHairModName ?? Label.ALL, CEditor.API.Get<HashSet<string>>(EType.ModsHairDef), (string s) => s ?? Label.ALL, new Action<string>(HairTool.ASelectedHairModName), "");
                SZWidgets.ListView<HairDef>(new Rect((float)(x + 16), (float)(y + 25), (float)(w - 32), (float)num), HairTool.lOfHairDefs, (HairDef hd) => hd.LabelCap, (HairDef hd) => hd.description, (HairDef hairA, HairDef hairB) => hairA == hairB, ref this.tempPawn.story.hairDef, ref this.scrollPos, false, new Action<HairDef>(this.AHairSelected), true, false, false, HairTool.onMouseover);
                y += num + 27;
            }
        }
    }

    private void DrawBeardSelector(int x, ref int y, int w, int h)
    {
        bool flag = !this.tempPawn.HasStyleTracker();
        if (!flag)
        {
            Rect rect = new Rect((float)x, (float)y, (float)w, 24f);
            SZWidgets.NavSelectorImageBox(rect, new Action(StyleTool.AChooseBeardCustom), new Action(StyleTool.ARandomBeard), null, null, null, new Action(StyleTool.AConfigBeard), Label.BEARD + " - " + CEditor.API.Pawn.GetBeardName(), null, Label.TIP_RANDOM_BEARD, null, null, default(Color), Label.CLICKTOOPENLIST);
            y += 30;
            bool isBeardConfigOpen = StyleTool.isBeardConfigOpen;
            if (isBeardConfigOpen)
            {
                int num = (int)this.InitialSize.y - 80 - y;
                SZWidgets.FloatMenuOnButtonText<string>(new Rect((float)(x + 16), (float)y, (float)(w - 32), 25f), StyleTool.selectedBeardModName ?? Label.ALL, CEditor.API.Get<HashSet<string>>(EType.ModsBeardDef), (string s) => s ?? Label.ALL, new Action<string>(StyleTool.ASelectedBeardModName), "");
                SZWidgets.ListView<BeardDef>(new Rect((float)(x + 16), (float)(y + 25), (float)(w - 32), (float)num), StyleTool.lOfBeardDefs, (BeardDef hd) => hd.LabelCap, (BeardDef hd) => hd.description, (BeardDef beardA, BeardDef beardB) => beardA == beardB, ref this.tempPawn.style.beardDef, ref this.scrollPos, false, new Action<BeardDef>(this.ABeardSelected), true, false, false, HairTool.onMouseover);
                y += num + 27;
            }
        }
    }

    private void TextValuesFromSelectedColor()
    {
        var num1 = selectedColor.r / part;
        var num2 = selectedColor.g / part;
        var num3 = selectedColor.b / part;
        var num4 = selectedColor.a / part;
        iRed = (int)num1;
        iGreen = (int)num2;
        iBlue = (int)num3;
        iAlpha = (int)num4;
        sRGB = iRed + "," + iGreen + "," + iBlue + "," + iAlpha;
        sHex = iRed.ToString("X2") + "," + iGreen.ToString("X2") + "," + iBlue.ToString("X2") + "," + iAlpha.ToString("X2");
        oldsRGB = sRGB;
        oldHex = sHex;
        oldSelected = selectedColor;
        if (tempPawn.ThingID != CEditor.API.Pawn.ThingID)
        {
            Init(null, null, null);
        }
        else
        {
            if (colorType == ColorType.HairColor)
            {
                tempPawn.SetHairColor(isColor1Choosen, selectedColor);
            }
            else if (colorType == ColorType.ApparelColor)
            {
                if (selectedApparel != null)
                    selectedApparel.DrawColor = selectedColor;
            }
            else if (colorType == ColorType.WeaponColor)
            {
                if (selectedWeapon != null)
                    selectedWeapon.DrawColor = selectedColor;
            }
            else if (colorType == ColorType.SkinColor)
            {
                tempPawn.SetSkinColor(isColor1Choosen, selectedColor);
            }
            else if (colorType == ColorType.FavColor)
            {
                tempPawn.story.favoriteColor = selectedColor;
            }
            else if (colorType == ColorType.GeneColorHair)
            {
                if (selectedGeneDef != null)
                    selectedGeneDef.hairColorOverride = selectedColor;
            }
            else if (colorType == ColorType.GeneColorSkinBase)
            {
                if (selectedGeneDef != null)
                    selectedGeneDef.skinColorBase = selectedColor;
            }
            else if (colorType == ColorType.GeneColorSkinOverride)
            {
                if (selectedGeneDef != null)
                    selectedGeneDef.skinColorOverride = selectedColor;
            }
            else if (colorType == ColorType.EyeColor)
            {
                tempPawn.SetEyeColor(selectedColor);
            }

            CalcClosestGeneColor();
            CEditor.API.UpdateGraphics();
        }
    }

    private void RGBTextToSelectedColor()
    {
        oldsRGB = sRGB;
        var strArray = sRGB.Split(new string[1]
        {
            ","
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length <= 3)
            return;
        var s1 = strArray[0];
        var s2 = strArray[1];
        var s3 = strArray[2];
        var s4 = strArray[3];
        var flag1 = false;
        var flag2 = false;
        var flag3 = false;
        var flag4 = false;
        if (int.TryParse(s1, out iRed))
            flag1 = true;
        if (int.TryParse(s2, out iGreen))
            flag2 = true;
        if (int.TryParse(s3, out iBlue))
            flag3 = true;
        if (int.TryParse(s4, out iAlpha))
            flag4 = true;
        if (iRed > ColorTool.IMAXB)
            iRed = ColorTool.IMAXB;
        if (iGreen > ColorTool.IMAXB)
            iGreen = ColorTool.IMAXB;
        if (iBlue > ColorTool.IMAXB)
            iBlue = ColorTool.IMAXB;
        if (iAlpha > ColorTool.IMAXB)
            iAlpha = ColorTool.IMAXB;
        if (flag1 & flag2 & flag3 & flag4)
        {
            sHex = iRed.ToString("X2") + "," + iGreen.ToString("X2") + "," + iBlue.ToString("X2") + "," + iAlpha.ToString("X2");
            oldHex = sHex;
            selectedColor = new Color((float)part * iRed, (float)part * iGreen, (float)part * iBlue, (float)part * iAlpha);
        }
    }

    private void HEXTextToSelectedColor()
    {
        oldHex = sHex;
        var str1 = sHex.SubstringTo(",");
        var str2 = sHex.SubstringTo(",", 2).SubstringFrom(",");
        var str3 = sHex.SubstringTo(",", 3).SubstringFrom(",", 2);
        var str4 = sHex.SubstringFrom(",", 3);
        var flag1 = true;
        var flag2 = true;
        var flag3 = true;
        var flag4 = true;
        try
        {
            iRed = Convert.ToByte(str1, 16);
        }
        catch
        {
            flag1 = false;
        }

        try
        {
            iGreen = Convert.ToByte(str2, 16);
        }
        catch
        {
            flag2 = false;
        }

        try
        {
            iBlue = Convert.ToByte(str3, 16);
        }
        catch
        {
            flag3 = false;
        }

        try
        {
            iAlpha = Convert.ToByte(str4, 16);
        }
        catch
        {
            flag4 = false;
        }

        if (!(flag1 & flag2 & flag3 & flag4))
            return;
        sRGB = iRed + "," + iGreen + "," + iBlue + "," + iAlpha;
        oldsRGB = sRGB;
        selectedColor = new Color((float)part * iRed, (float)part * iGreen, (float)part * iBlue, (float)part * iAlpha);
    }

    private void AColorSelectedByGene(Color color, Gene g)
    {
        if (colorType == ColorType.HairColor || colorType == ColorType.SkinColor)
        {
            if (Event.current.control)
            {
                selectedColor = tempPawn.RemoveGeneKeepFirst(g).def.IconColor;
                TextValuesFromSelectedColor();
            }
            else
            {
                selectedColor = color;
                tempPawn.MakeGeneFirst(g);
            }
        }
        else
        {
            selectedColor = color;
        }
    }

    private void ARandomColor()
    {
        if (Event.current.control)
            selectedColor = ColorTool.RandomAlphaColor;
        else
            selectedColor = ColorTool.GetRandomColor(fMinBright, fMaxBright);
    }

    private void ARandomizeHairAndColor()
    {
        if (tempPawn == null || tempPawn.story == null)
            return;
        tempPawn.SetHair(true, true, HairTool.selectedHairModName);
        var col1 = Event.current.control ? ColorTool.RandomAlphaColor : ColorTool.GetRandomColor(fMinBright, fMaxBright);
        tempPawn.SetHairColor(true, col1);
        var col2 = Event.current.control ? ColorTool.RandomAlphaColor : ColorTool.GetRandomColor(fMinBright, fMaxBright);
        tempPawn.SetHairColor(false, col2);
        selectedColor = isColor1Choosen ? col1 : col2;
        CEditor.API.UpdateGraphics();
    }

    private void AHairSelected(HairDef hairDef)
    {
        tempPawn.SetHair(hairDef);
        CEditor.API.UpdateGraphics();
    }

    private void ABeardSelected(BeardDef beardDef)
    {
        tempPawn.SetBeard(beardDef);
        CEditor.API.UpdateGraphics();
    }

    private void AApparelSelected(ThingDef apparelDef)
    {
        selectedApparel = tempPawn.apparel.WornApparel.Where(td => td.def == apparelDef).First();
        selectedColor = selectedApparel.DrawColor;
        if (selectedApparel != null && (selectedApparel.def.colorGenerator == null || !selectedApparel.def.HasComp(typeof(CompColorable))))
            MessageTool.ShowActionDialog(Label.INFOD_APPAREL, () => ApparelTool.MakeThingColorable(selectedApparel.def), Label.INFOT_MAKECOLORABLE);
        CEditor.API.UpdateGraphics();
    }

    private void AWeaponSelected(ThingDef weaponDef)
    {
        selectedWeapon = tempPawn.equipment.AllEquipmentListForReading.Where(td => td.def == weaponDef).First();
        selectedColor = selectedWeapon.DrawColor;
        CEditor.API.UpdateGraphics();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.ColorPicker, this.windowRect.position);
        base.Close(doCloseSound);
    }
}
