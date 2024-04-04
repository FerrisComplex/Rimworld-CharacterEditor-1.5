// Decompiled with JetBrains decompiler
// Type: CharacterEditor.PosTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class PosTool
{
    internal static void DeletePawnsInCell(this IntVec3 cell)
    {
        var p = cell.FirstPawnFromCell();
        if (p == null)
            return;
        p.Delete();
    }

    internal static Pawn FirstPawnFromCell(this IntVec3 cell)
    {
        var pawn = cell.FirstFromCell<Pawn>();
        if (pawn != null)
            return pawn;
        return cell.FirstFromCell<Corpse>()?.InnerPawn;
    }

    internal static Pawn FirstPawnInCellArea(this IntVec3 pos)
    {
        var pawn = pos.FirstPawnFromCell();
        if (pawn != null)
            return pawn;
        var intVec3List = new List<IntVec3>();
        intVec3List.Add(pos);
        intVec3List.Add(new IntVec3(pos.x, pos.y, pos.z));
        intVec3List.Add(new IntVec3(pos.x, pos.y, pos.z + 1));
        intVec3List.Add(new IntVec3(pos.x, pos.y, pos.z - 1));
        intVec3List.Add(new IntVec3(pos.x + 1, pos.y, pos.z));
        intVec3List.Add(new IntVec3(pos.x + 1, pos.y, pos.z + 1));
        intVec3List.Add(new IntVec3(pos.x + 1, pos.y, pos.z - 1));
        intVec3List.Add(new IntVec3(pos.x - 1, pos.y, pos.z));
        intVec3List.Add(new IntVec3(pos.x - 1, pos.y, pos.z + 1));
        intVec3List.Add(new IntVec3(pos.x - 1, pos.y, pos.z - 1));
        foreach (var cell in intVec3List)
        {
            pawn = cell.FirstPawnFromCell();
            if (pawn != null)
                break;
        }

        return pawn;
    }

    internal static T FirstFromCell<T>(this IntVec3 cell)
    {
        return Find.CurrentMap == null || !cell.InBounds(Find.CurrentMap) ? default : Find.CurrentMap.thingGrid.ThingsAt(cell).ToList().OfType<T>().FirstOrFallback();
    }

    internal static Pawn FirstPawnFromSelector(this Selector selector)
    {
        if (selector != null && selector.FirstSelectedObject != null)
        {
            var firstSelectedObject = selector.FirstSelectedObject;
            if (firstSelectedObject.GetType() == typeof(Pawn))
                return firstSelectedObject as Pawn;
            if (firstSelectedObject.GetType() == typeof(Corpse))
                return (firstSelectedObject as Corpse).InnerPawn;
        }

        return null;
    }

    internal static ThingDef FirstThingFromSelector(this Selector selector)
    {
        if (selector != null && selector.FirstSelectedObject != null)
        {
            var firstSelectedObject = selector.FirstSelectedObject;
            if (firstSelectedObject.GetType() == typeof(ThingDef))
                return firstSelectedObject as ThingDef;
        }

        return null;
    }

    internal static void CheckAndSetScrollPos<T>(this Vector2 scrollPos, List<T> l, T tSelected, float elemenH, float maxH)
    {
        bool flag = tSelected != null && !l.NullOrEmpty<T>();
        if (flag)
        {
            int num = l.IndexOf(tSelected);
            bool flag2 = num >= 0;
            if (flag2)
            {
                Vector2 vector = new Vector2(scrollPos.x, (float)num * elemenH);
                float num2 = vector.y - scrollPos.y;
                bool flag3 = num2 < 0f || num2 > maxH;
                if (flag3)
                {
                    scrollPos.x = vector.x;
                    scrollPos.y = vector.y;
                }
            }
        }
    }


    
    internal static void CheckAndSetScrollPos<T>(this Vector2 scrollPos, HashSet<T> l, T tSelected, float elemenH, float maxH)
    {
        bool flag = tSelected != null && !l.NullOrEmpty<T>();
        if (flag)
        {
            int num = l.FirstIndexOf((T y) => tSelected.Equals(y));
            bool flag2 = num >= 0;
            if (flag2)
            {
                Vector2 vector = new Vector2(scrollPos.x, (float)num * elemenH);
                float num2 = vector.y - scrollPos.y;
                bool flag3 = num2 < 0f || num2 > maxH;
                if (flag3)
                {
                    scrollPos.x = vector.x;
                    scrollPos.y = vector.y;
                }
            }
        }
    }
}
