// Decompiled with JetBrains decompiler
// Type: CharacterEditor.Clipboard
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Runtime.InteropServices;

namespace CharacterEditor;

internal static class Clipboard
{
    [DllImport("user32.dll")]
    internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll")]
    internal static extern bool CloseClipboard();

    [DllImport("user32.dll")]
    internal static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

    internal static void CopyToClip(string text)
    {
        OpenClipboard(IntPtr.Zero);
        SetClipboardData(13U, Marshal.StringToHGlobalUni(text));
        CloseClipboard();
    }
}