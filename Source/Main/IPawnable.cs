// Decompiled with JetBrains decompiler
// Type: CharacterEditor.IPawnable
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using Verse;

namespace CharacterEditor;

internal interface IPawnable
{
    Pawn SelectedPawn { get; set; }

    Pawn SelectedPawn2 { get; set; }

    Pawn SelectedPawn3 { get; set; }

    Pawn SelectedPawn4 { get; set; }
}