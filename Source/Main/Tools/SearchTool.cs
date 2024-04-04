// Decompiled with JetBrains decompiler
// Type: CharacterEditor.SearchTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class SearchTool
{
    internal ApparelLayerDef apparelLayerDef;
    internal BodyPartGroupDef bodyPartGroupDef;
    internal string filter1;
    internal string filter2;
    internal string find;
    internal string findOld;
    internal string modName;
    internal object ofilter1;
    internal object ofilter2;
    internal Vector2 onScreenPos;
    internal Vector2 scrollPos;
    internal ThingCategory thingCategory;
    internal ThingCategoryDef thingCategoryDef;
    internal WeaponType weaponType;

    internal SearchTool()
    {
        find = "";
        findOld = "";
        modName = null;
        filter1 = null;
        filter2 = null;
        ofilter1 = null;
        ofilter2 = null;
        scrollPos = new Vector2();
        onScreenPos = new Vector2();
        weaponType = WeaponType.Ranged;
        thingCategoryDef = null;
        thingCategory = ThingCategory.None;
        apparelLayerDef = null;
        bodyPartGroupDef = null;
    }

    internal string SelectedModName
    {
        get => modName.NullOrEmpty() ? Label.ALL : modName;
        set => modName = value;
    }

    internal string SelectedFilter1
    {
        get => filter1.NullOrEmpty() ? Label.ALL : filter1;
        set => filter1 = value;
    }

    internal string SelectedFilter2
    {
        get => filter2.NullOrEmpty() ? Label.ALL : filter2;
        set => filter2 = value;
    }

    internal static SearchTool GetInstance(SIndex uniqueIdx)
    {
        return CEditor.API.Get<Dictionary<SIndex, SearchTool>>(EType.Search)[uniqueIdx];
    }

    internal static void ClearSearch(SIndex uniqueIdx)
    {
        var dictionary = CEditor.API.Get<Dictionary<SIndex, SearchTool>>(EType.Search);
        if (!dictionary.ContainsKey(uniqueIdx))
            dictionary.Add(uniqueIdx, new SearchTool());
        SZWidgets.sFind = "";
        SZWidgets.sFindOld = "";
        dictionary[uniqueIdx].find = "";
        dictionary[uniqueIdx].findOld = "";
        dictionary[uniqueIdx].modName = "";
    }

    internal static SearchTool Update(SIndex uniqueIdx)
    {
        SZWidgets.bFocusOnce = true;
        var dictionary = CEditor.API.Get<Dictionary<SIndex, SearchTool>>(EType.Search);
        if (!dictionary.ContainsKey(uniqueIdx))
            dictionary.Add(uniqueIdx, new SearchTool());
        SZWidgets.sFind = dictionary[uniqueIdx].find;
        SZWidgets.sFindOld = "";
        SZWidgets.lSimilar = new List<string>();
        return dictionary[uniqueIdx];
    }

    internal static void SetPosition(SearchTool.SIndex uniqueIdx, ref Rect r, ref bool doOnce, int offset)
    {
        Dictionary<SearchTool.SIndex, SearchTool> dictionary = CEditor.API.Get<Dictionary<SearchTool.SIndex, SearchTool>>(EType.Search);
        doOnce = false;
        Vector2 vector = dictionary[uniqueIdx].onScreenPos;
        float y = dictionary[SearchTool.SIndex.Editor].onScreenPos.y;
        bool flag = vector != default(Vector2);
        if (flag)
        {
            r.position = vector;
        }
        else
        {
            r.position = new Vector2((float)(CEditor.API.EditorPosX + offset), (float)CEditor.API.EditorPosY);
        }
    }

    internal static void Save(SIndex uniqueIdx, Vector2 loc)
    {
        var dictionary = CEditor.API.Get<Dictionary<SIndex, SearchTool>>(EType.Search);
        dictionary[uniqueIdx].onScreenPos = loc;
        dictionary[uniqueIdx].find = SZWidgets.sFind;
        dictionary[uniqueIdx].findOld = SZWidgets.sFindOld;
        SZWidgets.lSimilar.Clear();
        SZWidgets.bFocusOnce = true;
    }

    internal enum SIndex
    {
        AbilityDef = 1,
        Animal = 2,
        OtherPawn = 3,
        GeneDef = 4,
        HediffDef = 5,
        ThoughtDef = 6,
        TraitDef = 7,
        Weapon = 8,
        BackstoryDefChild = 9,
        BackstroryDefAdult = 10, // 0x0000000A
        Race = 11, // 0x0000000B
        FindPawn = 12, // 0x0000000C
        Editor = 13, // 0x0000000D
        Capsule = 14, // 0x0000000E
        ChangeFaction = 15, // 0x0000000F
        ChangeHeadAddons = 16, // 0x00000010
        ChoosePart = 17, // 0x00000011
        AddStat = 18, // 0x00000012
        ColorPicker = 19, // 0x00000013
        FullHeal = 20, // 0x00000014
        Psychology = 21, // 0x00000015
        ViewXenoType = 22, // 0x00000016
        XenoType = 23, // 0x00000017
        ChangeBirthday = 24 // 0x00000018
    }
}
