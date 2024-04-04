// Decompiled with JetBrains decompiler
// Type: CharacterEditor.CustomDialog
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class CustomDialog : Window
{
    private const float TitleHeight = 42f;
    protected const float ButtonHeight = 35f;
    internal Action buttonAbortAction;
    internal Action buttonAcceptAction;
    internal Action buttonNextAction;
    private float creationRealTime = -1f;
    private Vector2 scrollPosition = Vector2.zero;
    internal string text;
    internal string title;

    internal CustomDialog(
        string text,
        string title,
        Action onAbort,
        Action onConfirm,
        Action onNext)
    {
        this.title = title;
        this.text = text;
        buttonAbortAction = onAbort;
        buttonAcceptAction = onConfirm;
        buttonNextAction = onNext;
        layer = CEditor.Layer;
        forcePause = true;
        absorbInputAroundWindow = true;
        creationRealTime = RealTime.LastRealTime;
        onlyOneOfTypeAllowed = false;
        closeOnAccept = true;
        closeOnCancel = true;
    }

    public override Vector2 InitialSize => new(640f, 460f);

    public override void DoWindowContents(Rect inRect)
    {
        float num = inRect.y;
        bool flag = !this.title.NullOrEmpty();
        if (flag)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, num, inRect.width, 42f), this.title);
            num += 42f;
        }
        Text.Font = GameFont.Small;
        Rect outRect = new Rect(inRect.x, num, inRect.width, (float)((double)inRect.height - 35.0 - 5.0) - num);
        float width = outRect.width - 16f;
        Rect viewRect = new Rect(0f, 0f, width, Text.CalcHeight(this.text, width));
        Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect, true);
        Widgets.Label(new Rect(0f, 0f, viewRect.width, viewRect.height), this.text);
        Widgets.EndScrollView();
        GUI.color = Color.white;
        float num2 = this.InitialSize.y - 70f;
        SZWidgets.CheckBoxOnChange(new Rect(inRect.x + 420f, num2 - 30f, 180f, 30f), Label.ALWAYS_SKIP, CEditor.DontAsk, new Action<bool>(this.ASetAlwaysSkip));
        SZWidgets.ButtonText(new Rect(inRect.x, num2, 180f, 30f), "Cancel".Translate(), new Action(this.AOnAbort), "");
        SZWidgets.ButtonText(new Rect(inRect.x + (float)((this.buttonNextAction == null) ? 420 : 210), num2, 180f, 30f), "Confirm".Translate(), new Action(this.AOnAccept), "");
        bool flag2 = this.buttonNextAction != null;
        if (flag2)
        {
            SZWidgets.ButtonText(new Rect(inRect.x + 420f, num2, 180f, 30f), Label.SKIP, new Action(this.AOnNext), "");
        }
    }

    private void ASetAlwaysSkip(bool val)
    {
        CEditor.DontAsk = val;
    }

    private void AOnAbort()
    {
        if (buttonAbortAction != null)
            buttonAbortAction();
        Close();
    }

    private void AOnAccept()
    {
        if (buttonAcceptAction != null)
            buttonAcceptAction();
        Close();
    }

    private void AOnNext()
    {
        if (buttonNextAction != null)
            buttonNextAction();
        Close();
    }

    public override void OnCancelKeyPressed()
    {
        AOnAbort();
    }

    public override void OnAcceptKeyPressed()
    {
        AOnAccept();
    }
}
