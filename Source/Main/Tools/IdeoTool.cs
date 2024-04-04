// Decompiled with JetBrains decompiler
// Type: CharacterEditor.IdeoTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using Verse;

namespace CharacterEditor;

internal static class IdeoTool
{
    internal static bool HasIdeoTracker(this Pawn p)
    {
        return p != null && p.ideo != null;
    }

    internal static string GetPawnCultureDefName(this Pawn pawn)
    {
        return pawn.HasIdeoTracker() && pawn.Ideo != null ? pawn.Ideo.culture != null ? pawn.Ideo.culture.defName : "" : "";
    }

    internal static string GetPawnIdeoName(this Pawn pawn)
    {
        return pawn.HasIdeoTracker() && pawn.Ideo != null ? pawn.Ideo.name : "";
    }

    internal static void SetPawnIdeo(this Pawn pawn, string cultureDefName, string ideoName)
    {
        if (!pawn.HasIdeoTracker())
            return;
        var ideosListForReading = Find.IdeoManager.IdeosListForReading;
        foreach (var ideo in ideosListForReading)
            if (ideo.culture.defName == cultureDefName && ideo.name == ideoName)
            {
                pawn.ideo.SetIdeo(ideo);
                return;
            }

        foreach (var ideo in ideosListForReading)
            if (ideo.culture.defName == cultureDefName)
                pawn.ideo.SetIdeo(ideo);
    }
}