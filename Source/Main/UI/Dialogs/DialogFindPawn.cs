// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogFindPawn
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogFindPawn : Window
{
    private bool doOnce;
    private readonly Func<Pawn, string> FGetInfo;
    private Vector2 scrollPos;
    private Pawn selectedPawn;

    internal DialogFindPawn()
    {
        scrollPos = new Vector2();
        selectedPawn = null;
        doOnce = true;
        SearchTool.Update(SearchTool.SIndex.FindPawn);
        FGetInfo = GetInfo;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnCancel = true;
        closeOnClickedOutside = true;
        draggable = true;
        layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => WindowTool.DefaultToolWindow;

    private string GetInfo(Pawn p)
    {
        var str = p.GetPawnName() + " (" + p.KindLabel + "," + p.GetGenderLabel();
        if (p.Faction != null)
            str = str + "," + p.Faction.Name;
        return str + ")";
    }

    public override void DoWindowContents(Rect inRect)
    {
        float num = this.InitialSize.x - 40f;
        float h = this.InitialSize.y - 115f;
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.FindPawn, ref this.windowRect, ref this.doOnce, 0);
        }
        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(0f, 0f, num, 30f), Label.FIND_PAWN);
        Text.Font = GameFont.Small;
        SZWidgets.ListView<Pawn>(0f, 30f, num, h, CEditor.API.ListOf<Pawn>(EType.Pawns), this.FGetInfo, (Pawn p) => p.MainDesc(true, true), (Pawn pA, Pawn pB) => pA == pB, ref this.selectedPawn, ref this.scrollPos, false, new Action<Pawn>(this.ASelectPawn), true, false, false, false);
        WindowTool.SimpleAcceptButton(this, new Action(this.DoAndClose));
    }

    private void ASelectPawn(Pawn p)
    {
        selectedPawn = p;
        CEditor.API.Pawn = p;
    }

    private void DoAndClose()
    {
        bool flag = this.selectedPawn != null;
        if (flag)
        {
            Current.Game.CurrentMap = this.selectedPawn.Map;
            CameraJumper.TryJumpAndSelect(this.selectedPawn, CameraJumper.MovementMode.Pan);
        }
        this.Close(true);
    }
    
    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.FindPawn, this.windowRect.position);
        base.Close(doCloseSound);
    }

    public override void OnAcceptKeyPressed()
    {
        base.OnAcceptKeyPressed();
        DoAndClose();
    }
}
