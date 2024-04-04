// Decompiled with JetBrains decompiler
// Type: CharacterEditor.WorkTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CharacterEditor;

internal static class WorkTool
{
    internal static List<WorkTypeDef> GetAllWorkTypeDefs()
    {
        return DefDatabase<WorkTypeDef>.AllDefs.ToList();
    }

    internal static string GetWorkPrioritiesAsSeparatedString(this Pawn p)
    {
        if (!p.HasWorkTracker())
            return "";
        var text = "";
        var allWorkTypeDefs = GetAllWorkTypeDefs();
        p.workSettings.EnableAndInitializeIfNotAlreadyInitialized();
        foreach (var workTypeDef in allWorkTypeDefs)
        {
            text = text + workTypeDef.defName + "|";
            text += p.workSettings.GetPriority(workTypeDef).ToString();
            text += ":";
        }

        return text.SubstringRemoveLast();
    }

    internal static void SetWorkPrioritiesFromSeparatedString(this Pawn p, string s)
    {
        if (!p.HasWorkTracker())
            return;
        p.workSettings.EnableAndInitializeIfNotAlreadyInitialized();
        p.workSettings.DisableAll();
        if (s.NullOrEmpty())
            return;
        foreach (var s1 in s.SplitNo(":"))
        {
            var strArray = s1.SplitNo("|");
            if (strArray.Length == 2)
            {
                var workTypeDef = DefTool.WorkTypeDef(strArray[0]);
                if (workTypeDef != null)
                {
                    var num = strArray[1].AsInt32();
                    p.workSettings.SetPriority(workTypeDef, num);
                }
            }
        }
    }
}
