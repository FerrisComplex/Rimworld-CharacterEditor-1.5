// Decompiled with JetBrains decompiler
// Type: CharacterEditor.CompTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using Verse;

namespace CharacterEditor;

internal static class CompTool
{
    internal static HediffComp GetHediffComp(
        Pawn p,
        bool conditionToPass,
        string typeEndsWith)
    {
        if (p == null || p.AllComps.NullOrEmpty() || !conditionToPass)
            return null;
        foreach (var allComp in p.health.hediffSet.GetAllComps())
            if (allComp.GetType().ToString().EndsWith(typeEndsWith))
                return allComp;
        return null;
    }
}
