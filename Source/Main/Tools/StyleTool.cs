// Decompiled with JetBrains decompiler
// Type: CharacterEditor.StyleTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CharacterEditor;

internal static class StyleTool
{
    internal static string selectedBeardModName;
    internal static HashSet<BeardDef> lOfBeardDefs;
    internal static bool isBeardConfigOpen;

    internal static string GetBeardDefName(this Pawn p)
    {
        return !p.HasStyleTracker() || p.style.beardDef == null ? "" : p.style.beardDef.defName;
    }

    internal static string GetBeardName(this Pawn p)
    {
        return !p.HasStyleTracker() || p.style.beardDef == null ? "" : p.style.beardDef.LabelCap.ToString();
    }

    internal static HashSet<BeardDef> GetBeardList(string modname)
    {
        return DefTool.ListByMod<BeardDef>(modname).OrderBy((Func<BeardDef, bool>)(b => !b.noGraphic)).ToHashSet();
    }

    internal static bool SetBeard(this Pawn p, bool next, bool random)
    {
        bool flag;
        if (!p.HasStyleTracker())
        {
            flag = false;
        }
        else
        {
            var list = GetBeardList(null).ToList();
            if (list.EnumerableNullOrEmpty())
            {
                flag = false;
            }
            else
            {
                var beardDef = p.style.beardDef;
                var index1 = list.IndexOf(beardDef);
                var index2 = list.NextOrPrevIndex(index1, next, random);
                var b = list[index2];
                p.SetBeard(b);
                flag = true;
            }
        }

        return flag;
    }

    internal static void SetBeard(this Pawn p, BeardDef b)
    {
        if ((!p.HasStyleTracker() ? 1 : b == null ? 1 : 0) != 0)
            return;
        if (b == BeardDefOf.NoBeard)
            FixForCATMod_BeardsNotSettableToNoBeard(p, b);
        else
            p.style.beardDef = b;
        p?.Drawer?.renderer?.SetAllGraphicsDirty();
    }

    internal static void FixForCATMod_BeardsNotSettableToNoBeard(Pawn p, BeardDef b)
    {
        BeardDefOf.NoBeard.noGraphic = false;
        BeardDefOf.NoBeard.texPath = "bclear";
        p.style.beardDef = b;
        CEditor.API.UpdateGraphics();
        BeardDefOf.NoBeard.noGraphic = true;
    }

    internal static string GetFaceTattooDefName(this Pawn p)
    {
        return !p.HasStyleTracker() || p.style.FaceTattoo == null ? "" : p.style.FaceTattoo.defName;
    }

    internal static string GetFaceTattooName(this Pawn p)
    {
        return !p.HasStyleTracker() || p.style.FaceTattoo == null ? "" : p.style.FaceTattoo.LabelCap.ToString();
    }

    internal static HashSet<TattooDef> GetFaceTattooList(string modname)
    {
        return DefTool.ListByMod(modname, (Func<TattooDef, bool>)(x => x.tattooType == 0)).OrderBy((Func<TattooDef, bool>)(x => !x.noGraphic)).ToHashSet();
    }

    internal static bool SetFaceTattoo(this Pawn p, bool next, bool random)
    {
        bool flag;
        if (!p.HasStyleTracker())
        {
            flag = false;
        }
        else
        {
            var list = GetFaceTattooList(null).ToList();
            if (list.EnumerableNullOrEmpty())
            {
                flag = false;
            }
            else
            {
                var faceTattoo = p.style.FaceTattoo;
                var index1 = list.IndexOf(faceTattoo);
                var index2 = list.NextOrPrevIndex(index1, next, random);
                var t = list[index2];
                p.SetFaceTattoo(t);
                flag = true;
            }
        }

        return flag;
    }

    internal static void SetFaceTattoo(this Pawn p, TattooDef t)
    {
        if ((!p.HasStyleTracker() ? 1 : t == null ? 1 : 0) != 0)
            return;
        p.style.SetMemberValue("faceTattoo", t);
        p?.Drawer?.renderer?.SetAllGraphicsDirty();
    }

    internal static string GetBodyTattooDefName(this Pawn p)
    {
        return !p.HasStyleTracker() || p.style.BodyTattoo == null ? "" : p.style.BodyTattoo.defName;
    }

    internal static string GetBodyTattooName(this Pawn p)
    {
        return !p.HasStyleTracker() || p.style.BodyTattoo == null ? "" : p.style.BodyTattoo.LabelCap.ToString();
    }

    internal static HashSet<TattooDef> GetBodyTattooList(string modname)
    {
        return DefTool.ListByMod(modname, (Func<TattooDef, bool>)(x => x.tattooType == TattooType.Body)).OrderBy((Func<TattooDef, bool>)(x => !x.noGraphic)).ToHashSet();
    }

    internal static bool SetBodyTattoo(this Pawn p, bool next, bool random)
    {
        bool flag;
        if (!p.HasStyleTracker())
        {
            flag = false;
        }
        else
        {
            var list = GetBodyTattooList(null).ToList();
            if (list.EnumerableNullOrEmpty())
            {
                flag = false;
            }
            else
            {
                var bodyTattoo = p.style.BodyTattoo;
                var index1 = list.IndexOf(bodyTattoo);
                var index2 = list.NextOrPrevIndex(index1, next, random);
                var t = list[index2];
                p.SetBodyTattoo(t);
                flag = true;
            }
        }

        return flag;
    }

    internal static void SetBodyTattoo(this Pawn p, TattooDef t)
    {
        if ((!p.HasStyleTracker() ? 1 : t == null ? 1 : 0) != 0)
            return;
        p.style.SetMemberValue("bodyTattoo", t);
        p?.Drawer?.renderer?.SetAllGraphicsDirty();
    }

    internal static void AChooseBeardCustom()
    {
        SZWidgets.FloatMenuOnRect(GetBeardList(null), s => new TaggedString(s.LabelCap), ASetBeardCustom);
    }

    internal static void ASetBeardCustom(BeardDef beardDef)
    {
        CEditor.API.Pawn.SetBeard(beardDef);
        CEditor.API.UpdateGraphics();
    }

    internal static void ARandomBeard()
    {
        CEditor.API.Pawn.SetBeard(true, true);
        CEditor.API.UpdateGraphics();
    }

    internal static void ASelectedBeardModName(string val)
    {
        selectedBeardModName = val;
        lOfBeardDefs = GetBeardList(selectedBeardModName);
    }

    internal static void AConfigBeard()
    {
        isBeardConfigOpen = !isBeardConfigOpen;
    }
}
