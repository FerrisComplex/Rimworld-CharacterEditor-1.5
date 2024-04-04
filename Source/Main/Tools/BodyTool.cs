// Decompiled with JetBrains decompiler
// Type: CharacterEditor.BodyTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class BodyTool
{
    internal static List<BodyTypeDef> GetBodyDefList(this Pawn pawn, bool restricted = false)
    {
        if (pawn.IsAlienRace())
        {
            var bodyTypes = pawn.AlienPartGenerator_GetBodyTypes();
            if (bodyTypes != null)
            {
                if ((!restricted ? 0 : pawn.ageTracker != null ? 1 : 0) == 0)
                    return bodyTypes;
                var bodyTypeDefList = new List<BodyTypeDef>();
                foreach (var bodyTypeDef in bodyTypes)
                {
                    if (pawn.ageTracker == null) continue;
                    var flag = pawn.ageTracker.Adult && !bodyTypeDef.defName.ToLower().Contains("baby") && !bodyTypeDef.defName.ToLower().Contains("child");
                    if (flag)
                        flag = pawn.gender != Gender.Female ? pawn.gender != Gender.Male || !bodyTypeDef.defName.ToLower().Contains("female") : !bodyTypeDef.defName.ToLower().Contains("male");
                    if (flag)
                        bodyTypeDefList.Add(bodyTypeDef);
                }

                return bodyTypeDefList;
            }
        }

        return pawn.GetBodyList();
    }

    internal static List<BodyPartGroupDef> ListAllBodyPartGroupDefs(
        bool insertNull)
    {
        var list = DefDatabase<BodyPartGroupDef>.AllDefs.Where(td => td.modContentPack.IsCoreMod).OrderBy(td => td.label).ToList();
        list.AddRange(DefDatabase<BodyPartGroupDef>.AllDefs.Where(td => !td.modContentPack.IsCoreMod).OrderBy(td => td.label).ToList());
        if (insertNull)
            list.Insert(0, null);
        return list;
    }

    internal static List<BodyTypeDef> GetBodyList(this Pawn pawn)
    {
        List<BodyTypeDef> bodyTypeDefList1;
        if ((pawn == null ? 1 : pawn.story == null ? 1 : 0) != 0)
        {
            bodyTypeDefList1 = null;
        }
        else
        {
            var flag1 = pawn.story.bodyType?.modContentPack?.Name == "Alien Vs Predator";
            var bodyTypeDefList2 = CEditor.API.ListOf<BodyTypeDef>(EType.Bodies);
            var bodyTypeDefList3 = new List<BodyTypeDef>();
            foreach (var bodyTypeDef in bodyTypeDefList2)
                try
                {
                    var str = pawn.Drawer.renderer.BodyGraphic.path;
                    if (str.Contains("/Naked"))
                        str = str.SubstringTo("/Naked_", false) + bodyTypeDef.defName + "_south";
                    else if (str.Contains("_Naked_"))
                        str = str.SubstringTo("_Naked_", false) + bodyTypeDef.defName + "_south";
                    else if (str.Contains("Naked_"))
                        str = str.SubstringBackwardTo("Naked_") + "Naked_" + bodyTypeDef.defName + "_south";
                    var flag2 = TextureTool.TestTexturePath(str, false);
                    if (flag1)
                        flag2 = (pawn.kindDef.modContentPack == bodyTypeDef.modContentPack) & flag2;
                    if (flag2)
                        bodyTypeDefList3.Add(bodyTypeDef);
                }
                catch
                {
                }

            bodyTypeDefList1 = bodyTypeDefList3;
        }

        return bodyTypeDefList1;
    }

    internal static string GetBodyTypeDefName(this Pawn p)
    {
        return p == null || p.story == null || p.story.bodyType == null ? "" : p.story.bodyType.defName;
    }

    internal static string GetBodyTypeName(this Pawn pawn)
    {
        TaggedString taggedString;
        if (pawn == null)
        {
            taggedString = new TaggedString();
        }
        else
        {
            var story = pawn.story;
            if (story == null)
            {
                taggedString = new TaggedString();
            }
            else
            {
                var bodyType = story.bodyType;
                taggedString = bodyType != null ? bodyType.defName.Translate() : new TaggedString();
            }
        }



        return taggedString;
    }

    internal static string RemoveRotEndings(this string t)
    {
        return t.Replace("_south", "").Replace("_east", "").Replace("_north", "").Replace("_west", "");
    }

    internal static void SetBodyByDefName(this Pawn p, string defName)
    {
        if ((!p.HasStoryTracker() ? 1 : defName.NullOrEmpty() ? 1 : 0) != 0)
            return;
        var b = DefTool.BodyTypeDef(defName);
        if (b == null)
        {
            if ((p.kindDef.IsAnimal() ? 0 : !p.kindDef.RaceProps.Humanlike ? 1 : 0) == 0)
                return;
            MessageTool.Show("BodyTypeDef not found: " + defName);
        }
        else
        {
            p.SetBody(b);
        }
    }

    internal static void SetBody(this Pawn p, BodyTypeDef b)
    {
        if ((!p.HasStoryTracker() ? 1 : b == null ? 1 : 0) != 0)
            return;
        p.story.bodyType = b;
        p?.Drawer?.renderer?.SetAllGraphicsDirty();
    }

    internal static void SetBody(this Pawn p, bool next, bool random)
    {
        if (!p.HasStoryTracker())
            return;
        var bodyDefList = p.GetBodyDefList();
        var index1 = bodyDefList.IndexOf(p.story.bodyType);
        var index2 = bodyDefList.NextOrPrevIndex(index1, next, random);
        if (bodyDefList.NullOrEmpty())
            return;
        p.SetBody(bodyDefList[index2]);
    }
}
