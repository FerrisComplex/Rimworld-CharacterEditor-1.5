// Decompiled with JetBrains decompiler
// Type: CharacterEditor.CharacterEditorMod
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using UnityEngine;
using Verse;

namespace CharacterEditor;

[StaticConstructorOnStartup]
public class CharacterEditorMod : Mod
{
    internal const string MODID = "rimworld.mod.charactereditor";

    public CharacterEditorMod(ModContentPack content)
        : base(content)
    {
        CEditor.Initialize(this);
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        CEditor.API.ConfigEditor();
        WindowTool.Close(WindowTool.GetWindowOfEndsWithType("ModSettings"));
    }

    public override string SettingsCategory()
    {
        return "CharacterEditor";
    }
}
