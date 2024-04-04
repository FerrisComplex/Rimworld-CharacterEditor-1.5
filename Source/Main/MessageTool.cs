// Decompiled with JetBrains decompiler
// Type: CharacterEditor.MessageTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class MessageTool
{
    internal static void Show(string info, MessageTypeDef mt = null)
    {
        Messages.Message(info, mt ?? MessageTypeDefOf.SilentInput, false);
    }

    internal static void ShowInDebug(string info)
    {
        if (!Prefs.DevMode)
            return;
        Show(info);
    }

    internal static void ShowDialog(string s, bool doRestart = false)
    {
        if (doRestart)
            WindowTool.Open(Dialog_MessageBox.CreateConfirmation(new TaggedString(s), () =>
            {
                GameDataSaveLoader.SaveGame("autosave_last");
                GenCommandLine.Restart();
            }));
        else
            WindowTool.Open(Dialog_MessageBox.CreateConfirmation(new TaggedString(s), null));
    }

    internal static void ShowActionDialog(
        string s,
        Action confirmedAction,
        string title = null,
        WindowLayer layer = WindowLayer.Dialog)
    {
        WindowTool.Open(Dialog_MessageBox.CreateConfirmation(new TaggedString(s), confirmedAction, false, title, layer));
    }

    internal static void ShowCustomDialog(
        string s,
        string title,
        Action onAbort,
        Action onConfirm,
        Action onNext)
    {
        if (CEditor.DontAsk)
        {
            if (onNext != null)
                onNext();
            else
                onConfirm();
        }
        else
        {
            WindowTool.Open(new CustomDialog(s, title, onAbort, onConfirm, onNext));
        }
    }
}
