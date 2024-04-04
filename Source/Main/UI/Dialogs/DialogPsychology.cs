// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogPsychology
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using UnityEngine;
using Verse;

namespace CharacterEditor;

public class DialogPsychology : Window
{
    private bool doOnce;
    private readonly Type psychologyUI;
    private readonly Rect rect;

    public DialogPsychology()
    {
        doOnce = true;
        SearchTool.Update(SearchTool.SIndex.Psychology);
        try
        {
            psychologyUI = Reflect.GetAType("Psychology", "PsycheCardUtility");
        }
        catch
        {
        }

        rect = new Rect(0.0f, 20f, 500f, 680f);
        absorbInputAroundWindow = true;
        closeOnCancel = true;
        closeOnClickedOutside = true;
        draggable = true;
        layer = CEditor.Layer;
        doCloseX = true;
    }

    public override Vector2 InitialSize => new(500f, WindowTool.MaxH);

    public override void DoWindowContents(Rect inRect)
    {
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.Psychology, ref this.windowRect, ref this.doOnce, 0);
        }
        try
        {
            this.psychologyUI.CallMethod("DrawPsycheMenuCard", new object[]
            {
                this.rect,
                CEditor.API.Pawn
            });
            this.psychologyUI.CallMethod("DrawDebugOptions", new object[]
            {
                inRect,
                CEditor.API.Pawn
            });
            int index = WindowTool.GetIndex(this);
            WindowTool.TopLayerForWindowOfType("Psychology.Dialog_EditPsyche", false);
            SZWidgets.ButtonText(new Rect((float)((double)inRect.width * 0.600000023841858 - 84.0), 0f, 108f, 22f), "Modify", new Action(this.AModify), "changes will be applied after some passed time");
        }
        catch
        {
            this.Close(true);
        }
    }

    private void AModify()
    {
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.Psychology, this.windowRect.position);
        base.Close(doCloseSound);
    }
}
