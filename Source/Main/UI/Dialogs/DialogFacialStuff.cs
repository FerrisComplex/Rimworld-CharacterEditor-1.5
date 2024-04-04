// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogFacialStuff
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogFacialStuff : Window
{
    private readonly Type facialUI = null;

    internal DialogFacialStuff()
    {
        try
        {
            facialUI = Reflect.GetAType("FacialStuff", "Harmony.HarmonyPatchesFS");
            facialUI.CallMethod("OpenStylingWindow", new object[1]
            {
                CEditor.API.Pawn
            });
        }
        catch
        {
        }

        absorbInputAroundWindow = true;
        closeOnCancel = true;
        closeOnClickedOutside = true;
        layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => new(256f, 65f);

    public override void DoWindowContents(Rect inRect)
    {
        List<Window> windowOfStartsWithType = WindowTool.GetWindowOfStartsWithType("FacialStuff.FaceEditor.");
        WindowTool.BringToFrontMulti(windowOfStartsWithType);
        Window windowOfType = WindowTool.GetWindowOfType("FacialStuff.FaceEditor.Dialog_FaceStyling");
        bool flag = windowOfType == null;
        if (flag)
        {
            CEditor.API.UpdateGraphics();
            this.Close(true);
        }
    }
}
