// Decompiled with JetBrains decompiler
// Type: CharacterEditor.FacialTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CharacterEditor;

internal static class FacialTool
{
    internal static string FACE = "HeadControllerComp";
    internal static string EYE = "EyeballControllerComp";
    internal static string LID = "LidControllerComp";
    internal static string BROW = "BrowControllerComp";
    internal static string MOUTH = "MouthControllerComp";
    internal static string SKIN = "SkinControllerComp";
    internal static string FACETYPE = "faceType";

    internal static string GetFacialAnimationParams(this Pawn p)
    {
        return (p == null ? 1 : !CEditor.IsFacialAnimationActive ? 1 : 0) == 0 ? "" + p.FA_GetCurrentDefName(FACE) + "|" + p.FA_GetCurrentDefName(EYE) + "|" + p.FA_GetCurrentDefName(LID) + "|" + p.FA_GetCurrentDefName(BROW) + "|" + p.FA_GetCurrentDefName(MOUTH) + "|" + p.FA_GetCurrentDefName(SKIN) : "";
    }

    internal static void SetFacialAnimationParams(this Pawn p, string s)
    {
        if ((p == null || !CEditor.IsFacialAnimationActive ? 1 : s.NullOrEmpty() ? 1 : 0) != 0)
            return;
        var strArray = s.SplitNo("|");
        if (strArray.Length < 6)
            return;
        p.FA_SetDefByName(FACE, strArray[0].Trim());
        p.FA_SetDefByName(EYE, strArray[1].Trim());
        p.FA_SetDefByName(LID, strArray[2].Trim());
        p.FA_SetDefByName(BROW, strArray[3].Trim());
        p.FA_SetDefByName(MOUTH, strArray[4].Trim());
        p.FA_SetDefByName(SKIN, strArray[5].Trim());
    }

    internal static ThingComp GetRJWComp(this Pawn p)
    {
        ThingComp thingComp;
        if ((p == null || p.AllComps.NullOrEmpty() ? 1 : !CEditor.IsRJWActive ? 1 : 0) != 0)
        {
            thingComp = null;
        }
        else
        {
            foreach (var allComp in p.AllComps)
                if (allComp.GetType().ToString().StartsWith("rjw") && allComp.GetType().ToString().EndsWith("CompRJW"))
                    return allComp;
            thingComp = null;
        }

        return thingComp;
    }

    internal static ThingComp GetPersoComp(this Pawn p)
    {
        ThingComp thingComp;
        if ((p == null || p.AllComps.NullOrEmpty() ? 1 : !CEditor.IsPersonalitiesActive ? 1 : 0) != 0)
        {
            thingComp = null;
        }
        else
        {
            foreach (var allComp in p.AllComps)
                if (allComp.GetType().ToString().StartsWith("SPM1") && allComp.GetType().ToString().EndsWith("CompEnneagram"))
                    return allComp;
            thingComp = null;
        }

        return thingComp;
    }

    internal static ThingComp FA_GetControllerComp(this Pawn p, string controller)
    {
        ThingComp thingComp;
        if ((p == null || p.AllComps.NullOrEmpty() ? 1 : !CEditor.IsFacialAnimationActive ? 1 : 0) != 0)
        {
            thingComp = null;
        }
        else
        {
            foreach (var allComp in p.AllComps)
                if (allComp.GetType().ToString().StartsWith("FacialAnimation") && allComp.GetType().ToString().EndsWith(controller))
                    return allComp;
            thingComp = null;
        }

        return thingComp;
    }

    internal static string FA_GetCurrentDefName(this Pawn p, string controller)
    {
        var currentDef = p.FA_GetCurrentDef(controller);
        return currentDef == null ? "" : ((Def)currentDef).defName;
    }

    internal static object FA_GetCurrentDef(this Pawn p, string controller)
    {
        var controllerComp = p.FA_GetControllerComp(controller);
        return controllerComp == null ? null : controllerComp.GetMemberValue<object>(FACETYPE, null);
    }

    internal static List<string> FA_GetDefStringList(this Pawn p, string controller)
    {
        var stringList = new List<string>();
        var currentDef = p.FA_GetCurrentDef(controller);
        if (currentDef != null)
        {
            var defList = FA_FilterOutIncompatible(p, controller, FA_GetDefList(currentDef));
            if (defList != null)
                foreach (var def in defList)
                    stringList.Add(def.defName);
        }

        return stringList;
    }

    internal static List<Def> FA_GetDefList(object curDef)
    {
        return GenDefDatabase.GetAllDefsInDatabaseForDef(curDef.GetType()).ToList();
    }

    internal static List<Def> FA_FilterOutIncompatible(Pawn p, string controller, List<Def> l)
    {
        List<Def> result;
        if (p == null || l == null || l.Count == 1)
        {
            result = l;
        }
        else
        {
            List<Def> list = new List<Def>();
            if (controller == FacialTool.FACE)
            {
                if (p.gender == Gender.Male)
                {
                    using (List<Def>.Enumerator enumerator = l.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Def def = enumerator.Current;
                            if (def.defName != "HeadPointy")
                            {
                                list.Add(def);
                            }
                        }

                        goto IL_C5;
                    }
                }

                foreach (Def def2 in l)
                {
                    if (def2.defName != "HeadSquare")
                    {
                        list.Add(def2);
                    }
                }

                IL_C5:
                result = list;
            }
            else if (controller == FacialTool.EYE)
            {
                if (p.gender == Gender.Male)
                {
                    foreach (Def def3 in l)
                    {
                        if (def3.defName != "EyeDull")
                        {
                            list.Add(def3);
                        }
                    }

                    result = list;
                }
                else
                {
                    result = l;
                }
            }
            else if (controller == FacialTool.LID)
            {
                if (p.gender == Gender.Male)
                {
                    foreach (Def def4 in l)
                    {
                        if (def4.defName != "LidPointy" && def4.defName != "LidFlashy" && def4.defName != "LidQuite" && def4.defName != "LidSleepy")
                        {
                            list.Add(def4);
                        }
                    }

                    result = list;
                }
                else
                {
                    result = l;
                }
            }
            else
            {
                result = l;
            }
        }

        return result;
    }

    internal static Def FA_GetDefByName(Pawn p, string controller, string defName)
    {
        var currentDef = p.FA_GetCurrentDef(controller);
        return currentDef != null ? FA_FilterOutIncompatible(p, controller, FA_GetDefList(currentDef)).Where(td => td.defName == defName).FirstOrDefault() : null;
    }

    internal static void FA_SetDefByName(this Pawn p, string controller, string defName)
    {
        var controllerComp = p.FA_GetControllerComp(controller);
        if (controllerComp == null)
            return;
        controllerComp.SetMemberValue(FACETYPE, FA_GetDefByName(p, controller, defName));
        controllerComp.PostExposeData();
    }

    internal static bool FA_SetDef(this Pawn p, string controller, bool next, bool random)
    {
        var controllerComp = p.FA_GetControllerComp(controller);
        if (controllerComp != null)
        {
            var memberValue = controllerComp.GetMemberValue<object>(FACETYPE, null);
            if (memberValue != null)
            {
                string defName = ((Def)memberValue).defName;
                var l = FA_FilterOutIncompatible(p, controller, FA_GetDefList(memberValue));
                if (l != null)
                {
                    var index1 = 0;
                    for (var index2 = 0; index2 < l.Count; ++index2)
                        if (l[index2].defName == defName)
                        {
                            index1 = index2;
                            break;
                        }

                    var index3 = l.NextOrPrevIndex(index1, next, random);
                    controllerComp.SetMemberValue(FACETYPE, l[index3]);
                    return true;
                }
            }

            controllerComp.PostExposeData();
            p?.Drawer?.renderer?.SetAllGraphicsDirty();
        }

        return false;
    }
}
