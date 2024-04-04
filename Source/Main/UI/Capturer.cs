// Decompiled with JetBrains decompiler
// Type: CharacterEditor.Capturer
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Object = UnityEngine.Object;

namespace CharacterEditor;

internal class Capturer
{
    private const int imageW = 200;
    private const int imageH = 280;
    internal bool bHats;
    internal bool bNude;
    internal bool bUpdateGraphics;
    private int iCurrentRotation;
    private RenderTexture image;
    private readonly List<Rot4> lRotation;
    internal int renderH = 700;
    internal RenderTextureFormat renderTextureFormat;
    internal int renderW = 500;
    private readonly Vector2 v2 = new(200f, 280f);

    internal Capturer()
    {
        image = null;
        var rot4List = new List<Rot4>();
        rot4List.Add(Rot4.South);
        rot4List.Add(Rot4.West);
        rot4List.Add(Rot4.North);
        rot4List.Add(Rot4.East);
        lRotation = rot4List;
        iCurrentRotation = 0;
        bNude = true;
        bHats = true;
        bUpdateGraphics = false;
    }

    internal Pawn Pawn { get; set; }

    internal void RotateAndCapture(Pawn pawn)
    {
        iCurrentRotation = lRotation.NextOrPrevIndex(iCurrentRotation, true, false);
        UpdatePawnGraphic(pawn);
    }

    internal void ToggleNudeAndCapture(Pawn pawn)
    {
        bNude = !bNude;
        UpdatePawnGraphic(pawn);
    }

    internal void ToggleHatAndCapture(Pawn pawn)
    {
        bHats = !bHats;
        UpdatePawnGraphic(pawn);
    }

    internal void UpdatePawnGraphic(Pawn pawn)
    {
        bUpdateGraphics = true;
        image = GetRenderTexture(pawn, false);
    }

    internal RenderTexture GetRenderTexture(Pawn pawn, bool fromCache)
    {
        RenderTexture renderTexture;
        if (pawn == null)
        {
            renderTexture = null;
        }
        else if (fromCache)
        {
            renderTexture = PortraitsCache.Get(pawn, v2, lRotation[iCurrentRotation]);
        }
        else
        {
            if (image == null)
                image = new RenderTexture(renderW, renderH, 32, renderTextureFormat);
            if (bUpdateGraphics)
            {
                PrepareForRender(pawn);
                Render(pawn);
                bUpdateGraphics = false;
            }

            renderTexture = image;
        }

        return renderTexture;
    }

    internal void ChangeRenderTextureParamter(int resolution, bool isARGB)
    {
        renderW = resolution;
        renderH = (int)(renderW * 1.4);
        renderTextureFormat = isARGB ? 0 : (RenderTextureFormat)9;
        if (image != null)
            image.Release();
        image = new RenderTexture(renderW, renderH, 32, renderTextureFormat);
    }

    internal static void PrepareForRender(Pawn pawn)
    {
        try
        {
            PortraitsCache.SetDirty(pawn);
            if (pawn.kindDef.defName == "Zombie")
            {
                pawn.Drawer.renderer.EnsureGraphicsInitialized();
            }
            else
            {
                if (pawn.Drawer == null || pawn.Drawer.renderer == null)
                    return;
                pawn.Drawer.renderer.EnsureGraphicsInitialized();
            }
        }
        catch
        {
        }
    }

    internal void Render(Pawn pawn)
    {
        float num = 0f;
        Vector3 vector = default(Vector3);
        if (pawn.Dead || pawn.Downed)
        {
            num = 85f;
            vector.x -= 0.18f;
            vector.z -= 0.18f;
        }
        try
        {
            PawnCacheCameraManager.PawnCacheRenderer.RenderPawn(pawn, this.image, Vector3.zero, 1f, num, this.lRotation[this.iCurrentRotation], true, true, true, true, default(Vector3), null, null, false);
        }
        catch (Exception ex)
        {
            Log.Error(ex.StackTrace);
        }
    }

    internal void DrawPawnImage(Pawn pawn, int x, int y)
    {
        image = GetRenderTexture(pawn, false);
        if (image == null) return;
        GUI.DrawTexture(new Rect(x, y, 200f, 280f), image, (ScaleMode)2);
    }
}
