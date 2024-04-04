// Decompiled with JetBrains decompiler
// Type: CharacterEditor.ColorTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterEditor;

internal static class ColorTool
{
    internal static readonly Color colWhite = new(1f, 1f, 1f);
    internal static readonly Color colLightGray = new(0.82f, 0.824f, 0.831f);
    internal static readonly Color colGray = new(0.714f, 0.718f, 0.733f);
    internal static readonly Color colDarkGray = new(0.506f, 0.51f, 0.525f);
    internal static readonly Color colGraphite = new(0.345f, 0.345f, 0.353f);
    internal static readonly Color colDimGray = new(0.245f, 0.245f, 0.245f);
    internal static readonly Color colDarkDimGray = new(0.175f, 0.175f, 0.175f);
    internal static readonly Color colAsche = new(0.115f, 0.115f, 0.115f);
    internal static readonly Color colBlack = new(0.0f, 0.0f, 0.0f);
    internal static readonly Color colNavyBlue = new(0.0f, 0.082f, 0.267f);
    internal static readonly Color colDarkBlue = new(0.137f, 0.235f, 0.486f);
    internal static readonly Color colRoyalBlue = new(0.157f, 0.376f, 0.678f);
    internal static readonly Color colBlue = new(0.004f, 0.42f, 0.718f);
    internal static readonly Color colPureBlue = new(0.0f, 0.0f, 1f);
    internal static readonly Color colLightBlue = new(0.129f, 0.569f, 0.816f);
    internal static readonly Color colSkyBlue = new(0.58f, 0.757f, 0.91f);
    internal static readonly Color colMaroon = new(0.373f, 0.0f, 0.125f);
    internal static readonly Color colBurgundy = new(0.478f, 0.153f, 0.255f);
    internal static readonly Color colDarkRed = new(0.545f, 0.0f, 0.0f);
    internal static readonly Color colRed = new(0.624f, 0.039f, 0.055f);
    internal static readonly Color colPureRed = new(1f, 0.0f, 0.0f);
    internal static readonly Color colLightRed = new(0.784f, 0.106f, 0.216f);
    internal static readonly Color colHotPink = new(0.863f, 0.345f, 0.631f);
    internal static readonly Color colPink = new(0.969f, 0.678f, 0.808f);
    internal static readonly Color colDarkPurple = new(0.251f, 0.157f, 0.384f);
    internal static readonly Color colPurple = new(0.341f, 0.176f, 0.561f);
    internal static readonly Color colLightPurple = new(0.631f, 0.576f, 0.784f);
    internal static readonly Color colTeal = new(0.11f, 0.576f, 0.592f);
    internal static readonly Color colTurquoise = new(0.027f, 0.51f, 0.58f);
    internal static readonly Color colDarkBrown = new(0.282f, 0.2f, 0.125f);
    internal static readonly Color colBrown = new(0.388f, 0.204f, 0.102f);
    internal static readonly Color colLightBrown = new(0.58f, 0.353f, 0.196f);
    internal static readonly Color colTawny = new(0.784f, 0.329f, 0.098f);
    internal static readonly Color colBlaze = new(0.941f, 0.29f, 0.141f);
    internal static readonly Color colOrange = new(0.949f, 0.369f, 0.133f);
    internal static readonly Color colLightOrange = new(0.973f, 0.58f, 0.133f);
    internal static readonly Color colGold = new(0.824f, 0.624f, 0.055f);
    internal static readonly Color colYellowGold = new(1f, 0.761f, 0.051f);
    internal static readonly Color colYellow = new(1f, 0.859f, 0.004f);
    internal static readonly Color colDarkYellow = new(0.953f, 0.886f, 0.227f);
    internal static readonly Color colChartreuse = new(0.922f, 0.91f, 0.067f);
    internal static readonly Color colLightYellow = new(1f, 0.91f, 0.51f);
    internal static readonly Color colDarkGreen = new(0.0f, 0.345f, 0.149f);
    internal static readonly Color colGreen = new(0.137f, 0.663f, 0.29f);
    internal static readonly Color colPureGreen = new(0.0f, 1f, 0.0f);
    internal static readonly Color colLimeGreen = new(0.682f, 0.82f, 0.208f);
    internal static readonly Color colLightGreen = new(0.541f, 0.769f, 0.537f);
    internal static readonly Color colDarkOlive = new(0.255f, 0.282f, 0.149f);
    internal static readonly Color colOlive = new(0.451f, 0.463f, 0.294f);
    internal static readonly Color colOliveDrab = new(0.357f, 0.337f, 0.263f);
    internal static readonly Color colFoilageGreen = new(0.482f, 0.498f, 0.443f);
    internal static readonly Color colTan = new(0.718f, 0.631f, 0.486f);
    internal static readonly Color colBeige = new(0.827f, 0.741f, 0.545f);
    internal static readonly Color colKhaki = new(0.933f, 0.835f, 0.678f);
    internal static readonly Color colPeach = new(0.996f, 0.859f, 0.733f);
    internal static float offsetCX = 0.0f;
    internal static List<Color> lcolors;
    internal static int IMAX = 1;
    internal static int IMAXB = byte.MaxValue;
    internal static float FMAX = 1f;
    internal static float FMAXB = byte.MaxValue;
    internal static double DMAX = 1.0;
    internal static double DMAXB = byte.MaxValue;

    internal static List<Color> ListOfColors
    {
        get
        {
            if (lcolors == null)
            {
                var colorList = new List<Color>();
                colorList.Add(GetDerivedColor(colWhite, offsetCX));
                colorList.Add(GetDerivedColor(colLightGray, offsetCX));
                colorList.Add(GetDerivedColor(colGray, offsetCX));
                colorList.Add(GetDerivedColor(colDarkGray, offsetCX));
                colorList.Add(GetDerivedColor(colGraphite, offsetCX));
                colorList.Add(GetDerivedColor(colBlack, offsetCX));
                colorList.Add(GetDerivedColor(colNavyBlue, offsetCX));
                colorList.Add(GetDerivedColor(colDarkBlue, offsetCX));
                colorList.Add(GetDerivedColor(colRoyalBlue, offsetCX));
                colorList.Add(GetDerivedColor(colBlue, offsetCX));
                colorList.Add(GetDerivedColor(colPureBlue, offsetCX));
                colorList.Add(GetDerivedColor(colLightBlue, offsetCX));
                colorList.Add(GetDerivedColor(colSkyBlue, offsetCX));
                colorList.Add(GetDerivedColor(colMaroon, offsetCX));
                colorList.Add(GetDerivedColor(colBurgundy, offsetCX));
                colorList.Add(GetDerivedColor(colDarkRed, offsetCX));
                colorList.Add(GetDerivedColor(colRed, offsetCX));
                colorList.Add(GetDerivedColor(colPureRed, offsetCX));
                colorList.Add(GetDerivedColor(colLightRed, offsetCX));
                colorList.Add(GetDerivedColor(colHotPink, offsetCX));
                colorList.Add(GetDerivedColor(colPink, offsetCX));
                colorList.Add(GetDerivedColor(colDarkPurple, offsetCX));
                colorList.Add(GetDerivedColor(colPurple, offsetCX));
                colorList.Add(GetDerivedColor(colLightPurple, offsetCX));
                colorList.Add(GetDerivedColor(colTeal, offsetCX));
                colorList.Add(GetDerivedColor(colTurquoise, offsetCX));
                colorList.Add(GetDerivedColor(colDarkBrown, offsetCX));
                colorList.Add(GetDerivedColor(colBrown, offsetCX));
                colorList.Add(GetDerivedColor(colLightBrown, offsetCX));
                colorList.Add(GetDerivedColor(colTawny, offsetCX));
                colorList.Add(GetDerivedColor(colBlaze, offsetCX));
                colorList.Add(GetDerivedColor(colOrange, offsetCX));
                colorList.Add(GetDerivedColor(colLightOrange, offsetCX));
                colorList.Add(GetDerivedColor(colGold, offsetCX));
                colorList.Add(GetDerivedColor(colYellowGold, offsetCX));
                colorList.Add(GetDerivedColor(colYellow, offsetCX));
                colorList.Add(GetDerivedColor(colDarkYellow, offsetCX));
                colorList.Add(GetDerivedColor(colChartreuse, offsetCX));
                colorList.Add(GetDerivedColor(colLightYellow, offsetCX));
                colorList.Add(GetDerivedColor(colDarkGreen, offsetCX));
                colorList.Add(GetDerivedColor(colGreen, offsetCX));
                colorList.Add(GetDerivedColor(colPureGreen, offsetCX));
                colorList.Add(GetDerivedColor(colLimeGreen, offsetCX));
                colorList.Add(GetDerivedColor(colLightGreen, offsetCX));
                colorList.Add(GetDerivedColor(colDarkOlive, offsetCX));
                colorList.Add(GetDerivedColor(colOlive, offsetCX));
                colorList.Add(GetDerivedColor(colOliveDrab, offsetCX));
                colorList.Add(GetDerivedColor(colFoilageGreen, offsetCX));
                colorList.Add(GetDerivedColor(colTan, offsetCX));
                colorList.Add(GetDerivedColor(colBeige, offsetCX));
                colorList.Add(GetDerivedColor(colKhaki, offsetCX));
                colorList.Add(GetDerivedColor(colPeach, offsetCX));
                lcolors = colorList;
            }

            return lcolors;
        }
    }

    internal static Color RandomColor => GetRandomColor(maxbright: FMAX);

    internal static Color RandomAlphaColor => GetRandomColor(maxbright: FMAX, andAlpha: true);

    internal static string ColorHexString(this Color c)
    {
        var num1 = DMAX / DMAXB;
        var num2 = (int)(c.r / num1);
        var num3 = (int)(c.g / num1);
        var num4 = (int)(c.b / num1);
        var num5 = (int)(c.a / num1);
        return num2.ToString("X2") + "-" + num3.ToString("X2") + "-" + num4.ToString("X2") + "-" + num5.ToString("X2");
    }

    internal static string NullableColorHexString(this Color? c)
    {
        return c.HasValue ? c.Value.ColorHexString() : "";
    }

    internal static Color? HexStringToColorNullable(this string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return new Color?();
        var color = new Color();
        var strArray = hex.SplitNo("-");
        if (strArray.Length >= 4)
            try
            {
                var num = DMAX / DMAXB;
                var int32_1 = Convert.ToInt32(strArray[0], 16);
                var int32_2 = Convert.ToInt32(strArray[1], 16);
                var int32_3 = Convert.ToInt32(strArray[2], 16);
                var int32_4 = Convert.ToInt32(strArray[3], 16);
                color.r = ((float)num * int32_1);
                color.g = ((float)num * int32_2);
                color.b = ((float)num * int32_3);
                color.a = ((float)num * int32_4);
            }
            catch
            {
            }

        return color;
    }

    internal static Color HexStringToColor(this string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return Color.white;
        var color = new Color();
        var strArray = hex.SplitNo("-");
        if (strArray.Length >= 4)
            try
            {
                var num = DMAX / DMAXB;
                var int32_1 = Convert.ToInt32(strArray[0], 16);
                var int32_2 = Convert.ToInt32(strArray[1], 16);
                var int32_3 = Convert.ToInt32(strArray[2], 16);
                var int32_4 = Convert.ToInt32(strArray[3], 16);
                color.r = ((float)num * int32_1);
                color.g = ((float)num * int32_2);
                color.b = ((float)num * int32_3);
                color.a = ((float)num * int32_4);
            }
            catch
            {
            }

        return color;
    }

    internal static Color GetDerivedColor(Color color, float offset)
    {
        var num1 = color.r - offset;
        var num2 = color.g - offset;
        var num3 = color.b - offset;
        var num4 = num1 < 0.0 ? 0.0f : num1;
        var num5 = num2 < 0.0 ? 0.0f : num2;
        var num6 = num3 < 0.0 ? 0.0f : num3;
        return new Color(num4 > (double)IMAX ? IMAX : num4, num5 > (double)IMAX ? IMAX : num5, num6 > (double)IMAX ? IMAX : num6, color.a);
    }

    internal static Color GetRandomColor(float minbright = 0.0f, float maxbright = 1f, bool andAlpha = false)
    {
        if (maxbright < (double)minbright)
            maxbright = minbright;
        var num1 = DMAX / DMAXB;
        var minValue1 = (int)(minbright / num1);
        var maxValue1 = (int)(maxbright / num1);
        var minValue2 = (int)(0.5 / num1);
        var maxValue2 = (int)(1.0 / num1);
        if (minValue1 > maxValue1)
            minValue1 = maxValue1 - 1;
        if (minValue1 < 0)
            minValue1 = 0;
        var num2 = minValue1 == maxValue1 ? minValue1 : CEditor.zufallswert.Next(minValue1, maxValue1);
        var num3 = minValue1 == maxValue1 ? minValue1 : CEditor.zufallswert.Next(minValue1, maxValue1);
        var num4 = minValue1 == maxValue1 ? minValue1 : CEditor.zufallswert.Next(minValue1, maxValue1);
        var num5 = minValue2 == maxValue2 ? minValue2 : CEditor.zufallswert.Next(minValue2, maxValue2);
        var num6 = (float)num1 * num5;
        return new Color((float)num1 * num2, (float)num1 * num3, (float)num1 * num4, andAlpha ? num6 : 1f);
    }
}
