// Decompiled with JetBrains decompiler
// Type: CharacterEditor.PageStart
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using RimWorld;
using UnityEngine;

namespace CharacterEditor;

internal class PageStart : Page
{
    public override void DoWindowContents(Rect inRect)
    {
        PageUtility.InitGameStart();
        Close();
    }
}
