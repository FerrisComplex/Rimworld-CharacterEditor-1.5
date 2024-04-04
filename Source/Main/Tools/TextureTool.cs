// Decompiled with JetBrains decompiler
// Type: CharacterEditor.TextureTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class TextureTool
{
    internal static Texture2D GetTextureFromStackCount(this ThingDef t)
    {
        Texture2D texture2D = null;
        if (t != null && t.graphic != null && t.graphic.GetType() == typeof(Graphic_StackCount))
            texture2D = ContentFinder<Texture2D>.Get((t.graphic as Graphic_StackCount).SubGraphicForStackCount(100, t).path, false);
        return texture2D;
    }

    internal static Texture2D GetTextureFromMulti(this Graphic g, string rotate = "_south")
    {
        return ContentFinder<Texture2D>.Get((g as Graphic_Multi).path + rotate, false);
    }

    internal static string Rot4ToString(Rot4 rot4)
    {
        if (rot4 == Rot4.North)
            return "_north";
        if (rot4 == Rot4.South)
            return "_south";
        if (rot4 == Rot4.East)
            return "_east";
        return rot4 == Rot4.West ? "_west" : "_north";
    }

    internal static Texture2D GetTexture(this ThingDef t, int stackCount = 1, ThingStyleDef tsd = null, Rot4 rotation = default(Rot4))
    {
        Texture2D texture2D = null;
        bool flag = texture2D == null && t != null;
        if (flag)
        {
            GraphicData graphicData = (tsd != null) ? tsd.graphicData : t.graphicData;
            graphicData = (graphicData ?? t.graphicData);
            bool flag2 = graphicData != null;
            if (flag2)
            {
                bool flag3 = graphicData.graphicClass == typeof(Graphic_Multi);
                if (flag3)
                {
                    texture2D = TextureTool.GetGraphicForMulti(graphicData, rotation);
                }
                else
                {
                    bool flag4 = graphicData.graphicClass == typeof(Graphic_Random);
                    if (flag4)
                    {
                        texture2D = TextureTool.GetGraphicForRandom(graphicData);
                    }
                    else
                    {
                        bool flag5 = graphicData.graphicClass == typeof(Graphic_StackCount);
                        if (flag5)
                        {
                            texture2D = TextureTool.GetGraphicForStackCount(graphicData, t, stackCount);
                        }
                        else
                        {
                            bool flag6 = graphicData.graphicClass == typeof(Graphic_Single);
                            if (flag6)
                            {
                                texture2D = TextureTool.GetGraphicForSingle(graphicData);
                            }
                            else
                            {
                                texture2D = TextureTool.GetGraphicForUndefined(graphicData);
                            }
                        }
                    }
                }
            }
        }
        bool flag7 = texture2D == null && t != null && t.uiIcon != null;
        Texture2D result;
        if (flag7)
        {
            result = t.uiIcon;
        }
        else
        {
            result = texture2D;
        }
        return result;
    }

    internal static Texture2D GetGraphicForUndefined(GraphicData g)
    {
        return g == null ? null : ContentFinder<Texture2D>.Get(g.texPath, false);
    }

    internal static Texture2D GetGraphicForMulti(GraphicData g, Rot4 rotation)
    {
        string str = TextureTool.Rot4ToString(rotation);
        Texture2D texture2D = ContentFinder<Texture2D>.Get((g.Graphic as Graphic_Multi).path + str, false);
        bool flag = texture2D == null;
        if (flag)
        {
            bool flag2 = rotation == Rot4.West;
            if (flag2)
            {
                texture2D = ContentFinder<Texture2D>.Get((g.Graphic as Graphic_Multi).path + "_east", false);
            }
            bool flag3 = texture2D == null;
            if (flag3)
            {
                texture2D = ContentFinder<Texture2D>.Get((g.Graphic as Graphic_Multi).path + "_north", false);
            }
        }
        return texture2D;
    }

    internal static Texture2D GetGraphicForRandom(GraphicData g)
    {
        return ContentFinder<Texture2D>.Get((g.Graphic as Graphic_Random).FirstSubgraphic().path, false);
    }

    internal static Texture2D GetGraphicForSingle(GraphicData g)
    {
        if (g.Graphic == null)
            return ContentFinder<Texture2D>.Get(g.texPath, false);
        try
        {
            return ContentFinder<Texture2D>.Get((g.Graphic as Graphic_Single).path, false);
        }
        catch
        {
            return ContentFinder<Texture2D>.Get(g.texPath, false);
        }
    }

    internal static Texture2D GetGraphicForStackCount(
        GraphicData g,
        ThingDef t,
        int stackVal)
    {
        try
        {
            return ContentFinder<Texture2D>.Get((g.Graphic as Graphic_StackCount).SubGraphicForStackCount(stackVal, t).path, false);
        }
        catch
        {
            return ContentFinder<Texture2D>.Get(g.Graphic.path);
        }
    }

    internal static void SetGraphicDataSingle(this ThingDef td, string texPath, string uiTexPath)
    {
        if (td == null)
            return;
        td.graphicData = new GraphicData();
        td.graphicData.texPath = texPath;
        td.graphicData.shaderType = ShaderTypeDefOf.MetaOverlay;
        td.graphicData.graphicClass = typeof(Graphic_Single);
        td.uiIconPath = uiTexPath;
    }

    internal static bool TestTexturePath(string path, bool showError = true)
    {
        Texture2D x = ContentFinder<Texture2D>.Get(path, false);
        bool flag = x == null && showError;
        if (flag)
        {
            MessageTool.Show("Missing Texture=" + path, MessageTypeDefOf.RejectInput);
        }
        return x != null;
    }
}
