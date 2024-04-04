// Decompiled with JetBrains decompiler
// Type: CharacterEditor.Selected
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class Selected
{
    internal int age = 1;
    internal int buyPrice;
    internal Color DrawColor;
    internal Gender gender = 0;
    internal IntVec3 location;
    internal HashSet<ThingDef> lOfStuff;
    internal HashSet<ThingStyleDef> lOfStyle;
    internal int oldStackVal;
    internal int oldStuffIndex;
    internal int oldstyleIndex;
    internal Name pawnName;
    internal PawnKindDef pkd;
    internal int quality;
    internal int stackVal;
    internal ThingDef stuff;
    internal int stuffIndex;
    internal ThingStyleDef style;
    internal int styleIndex;
    internal Thing tempThing;
    internal ThingDef thingDef;

    private Selected(ThingDef t)
    {
        tempThing = null;
        Init(t);
    }

    private Selected(Thing t)
    {
        tempThing = t;
        location = t.Position;
        Init(t.def, false, t);
    }

    internal Texture2D GetTexture2D => thingDef.GetTexture(stackVal >= thingDef.stackLimit ? thingDef.stackLimit : stackVal, style, new Rot4());

    internal bool HasQuality => thingDef != null && thingDef.HasComp(typeof(CompQuality));

    internal bool HasStack => thingDef != null && thingDef.stackLimit > 1;

    internal bool HasRace => thingDef != null && thingDef.race != null;

    internal Func<ThingStyleDef, string> StyleLabelGetter => x =>
    {
        if (x == null)
            return Label.NONE;
        return !x.label.NullOrEmpty() ? x.label.CapitalizeFirst() : x.defName;
    };

    internal Func<ThingStyleDef, string> StyleDescrGetter => x => x == null ? null : x.description;

    internal Func<ThingDef, string> StuffLabelGetter => x =>
    {
        if (x == null)
            return Label.NONE;
        return !x.label.NullOrEmpty() ? x.label.CapitalizeFirst() : x.defName;
    };

    internal Func<ThingDef, string> StuffDescrGetter => x => x == null ? null : x.description;

    internal static Selected Random(
        HashSet<ThingDef> l,
        bool originalColors,
        bool randomStakcount = false)
    {
        if (l.NullOrEmpty())
            return null;
        var t = l.RandomElement();
        if (t == null)
            return null;
        var selected = new Selected(t);
        selected.DrawColor = originalColors ? selected.DrawColor : ColorTool.RandomColor;
        if (randomStakcount)
            selected.stackVal = CEditor.zufallswert.Next(1, selected.thingDef.stackLimit);
        return selected;
    }

    internal static Selected ByName(
        string defname,
        string stuffdefname,
        string styledefname,
        Color col,
        int quali,
        int stack)
    {
        var t = defname.NullOrEmpty() ? null : DefTool.ThingDef(defname);
        var _stuff = stuffdefname.NullOrEmpty() ? null : DefTool.ThingDef(stuffdefname);
        var _style = styledefname.NullOrEmpty() ? null : DefTool.ThingStyleDef(styledefname);
        var selected = new Selected(t);
        selected.Set(_stuff, _style, col, quali, stack);
        return selected;
    }

    internal static Selected ByThing(Thing _thing)
    {
        return new Selected(_thing);
    }

    internal static Selected ByThingDef(ThingDef _thingDef)
    {
        return new Selected(_thingDef);
    }

    private void Set(ThingDef _stuff, ThingStyleDef _style, Color col, int quali, int _stackVal)
    {
        this.stuff = _stuff;
        this.style = _style;
        this.stackVal = _stackVal;
        this.stuffIndex = ((this.stuff != null) ? this.lOfStuff.IndexOf(this.stuff) : 0);
        this.styleIndex = ((this.style != null) ? this.lOfStyle.IndexOf(this.style) : 0);
        this.quality = ((quali < 0) ? ((int)QualityUtility.AllQualityCategories.RandomElement<QualityCategory>()) : quali);
        this.DrawColor = col;
    }

    internal void SetStyle(bool next, bool random)
    {
        styleIndex = lOfStyle.NextOrPrevIndex(styleIndex, next, random);
        oldstyleIndex = styleIndex;
        style = lOfStyle.At(styleIndex);
        UpdateBuyPrice();
    }

    internal void SetStyle(ThingStyleDef _style)
    {
        styleIndex = lOfStyle.IndexOf(_style);
        oldstyleIndex = styleIndex;
        style = _style;
        UpdateBuyPrice();
    }

    internal void CheckSetStyle()
    {
        if (styleIndex == oldstyleIndex)
            return;
        SetStyle(lOfStyle.At(styleIndex));
    }

    internal void SetStuff(bool next, bool random)
    {
        stuffIndex = lOfStuff.NextOrPrevIndex(stuffIndex, next, random);
        oldStuffIndex = stuffIndex;
        stuff = lOfStuff.At(stuffIndex);
        DrawColor = thingDef.GetColor(stuff);
        UpdateBuyPrice();
    }

    internal void SetStuff(ThingDef _stuff)
    {
        stuffIndex = lOfStuff.IndexOf(_stuff);
        oldStuffIndex = stuffIndex;
        stuff = _stuff;
        DrawColor = thingDef.GetColor(stuff);
        UpdateBuyPrice();
    }

    internal void CheckSetStuff()
    {
        if (stuffIndex == oldStuffIndex)
            return;
        SetStuff(lOfStuff.At(stuffIndex));
    }

    internal void UpdateStuffList()
    {
        this.stuff = ((this.thingDef != null) ? this.thingDef.GetStuff(ref this.lOfStuff, ref this.stuffIndex, false) : null);
        this.UpdateBuyPrice();
        ThingDef thingDef = this.thingDef;
        if (thingDef != null)
        {
            thingDef.UpdateRecipes();
        }
    }

    internal void Init(ThingDef _thingDef, bool random = true, Thing thing = null, Action postProcess = null)
    {
        this.thingDef = _thingDef;
        int num = CEditor.zufallswert.Next(100);
        this.gender = ((num > 50) ? Gender.Female : Gender.Male);
        num = CEditor.zufallswert.Next(30);
        this.age = num;
        this.pkd = ((this.thingDef != null && this.thingDef.race != null) ? this.thingDef.race.AnyPawnKind : null);
        bool flag = thing != null;
        if (flag)
        {
            this.pkd = ((thing.def.race != null) ? ((thing.GetType() == typeof(Pawn)) ? ((Pawn)thing).kindDef : thing.def.race.AnyPawnKind) : null);
        }
        this.stuff = ((this.thingDef != null) ? this.thingDef.GetStuff(ref this.lOfStuff, ref this.stuffIndex, random) : null);
        this.DrawColor = ((this.stuff != null) ? this.thingDef.GetColor(this.stuff) : ((this.thingDef != null) ? this.thingDef.uiIconColor : Color.white));
        this.style = ((this.thingDef != null) ? this.thingDef.GetStyle(ref this.lOfStyle, ref this.styleIndex, random) : null);
        this.RandomQuality();
        bool flag2 = thing != null;
        if (flag2)
        {
            SZWidgets.tDefName = thing.def.defName;
            this.Set(thing.Stuff, thing.StyleDef, thing.DrawColor, thing.GetQuality(), thing.stackCount);
        }
        this.stuffIndex = ((this.stuff == null) ? 0 : this.stuffIndex);
        this.oldStuffIndex = ((this.stuff == null) ? 0 : this.stuffIndex);
        this.styleIndex = ((this.style == null) ? 0 : this.styleIndex);
        this.oldstyleIndex = ((this.style == null) ? 0 : this.styleIndex);
        this.buyPrice = 0;
        this.stackVal = ((thing != null) ? thing.stackCount : 1);
        this.oldStackVal = ((thing != null) ? thing.stackCount : 1);
        this.UpdateBuyPrice();
        bool flag3 = postProcess != null;
        if (flag3)
        {
            postProcess();
        }
    }

    internal void UpdateBuyPrice()
    {
        if (thingDef == null)
            return;
        double num;
        if (thingDef.HasComp(typeof(CompQuality)))
        {
            num = 0.333333333 + quality * 0.333333333;
            if (quality == 6)
                ++num;
        }
        else
        {
            num = 1.0;
        }

        var a = thingDef.GetStatValueAbstract(StatDefOf.MarketValue, stuff) * num * stackVal;
        if (a < 1.0)
            buyPrice = 1;
        else
            buyPrice = (int)Math.Round(a);
    }

    internal void RandomQuality()
    {
        quality = (int)QualityUtility.AllQualityCategories.RandomElement();
    }

    internal Selected NewCopy()
    {
        var selected = new Selected(thingDef);
        selected.age = age;
        selected.buyPrice = buyPrice;
        selected.DrawColor = DrawColor;
        selected.gender = gender;
        lOfStuff.CopyHashSet(ref selected.lOfStuff);
        lOfStyle.CopyHashSet(ref selected.lOfStyle);
        selected.oldStackVal = oldStackVal;
        selected.oldStuffIndex = oldstyleIndex;
        selected.oldstyleIndex = oldstyleIndex;
        selected.pkd = pkd;
        selected.quality = quality;
        selected.stackVal = stackVal;
        selected.stuff = stuff;
        selected.stuffIndex = stuffIndex;
        selected.style = style;
        selected.styleIndex = styleIndex;
        selected.tempThing = tempThing;
        selected.thingDef = thingDef;
        return selected;
    }

    internal List<Selected> SplitInStacks()
    {
        if (thingDef == null)
            return null;
        if (thingDef.stackLimit >= this.stackVal)
            return new List<Selected> { this };
        var selectedList = new List<Selected>();
        var stackVal = this.stackVal;
        do
        {
            var selected = NewCopy();
            selected.stackVal = stackVal > thingDef.stackLimit ? thingDef.stackLimit : stackVal;
            selected.oldStackVal = selected.stackVal;
            stackVal -= thingDef.stackLimit;
            selectedList.Add(selected);
        } while (stackVal > 0);

        return selectedList;
    }

    internal List<Thing> Generate()
    {
        if (thingDef == null)
            return null;
        if (thingDef.IsApparel)
        {
            var thingList = new List<Thing>();
            thingList.Add(ApparelTool.GenerateApparel(this));
            return thingList;
        }

        if (thingDef.IsWeapon)
        {
            var thingList = new List<Thing>();
            thingList.Add(WeaponTool.GenerateWeapon(this));
            return thingList;
        }

        var thingList1 = new List<Thing>();
        foreach (var splitInStack in SplitInStacks())
        {
            var thing = ThingTool.GenerateItem(splitInStack);
            thingList1.Add(thing);
        }

        return thingList1;
    }
}
