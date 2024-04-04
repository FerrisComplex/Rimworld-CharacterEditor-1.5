// Decompiled with JetBrains decompiler
// Type: CharacterEditor.ITickWin
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class ITickWin : Window
{
    private bool doClose;
    private int timeOut = 3000;

    public override Vector2 InitialSize => new(1f, 1f);

    public override void DoWindowContents(Rect inRect)
    {
        if (doClose)
        {
            Close();
        }
        else if (timeOut > 0)
        {
            --timeOut;
        }
        else
        {
            doClose = true;
            Find.CurrentMap.Parent.Abandon();
            GenScene.GoToMainMenu();
            Close();
        }
    }
}
