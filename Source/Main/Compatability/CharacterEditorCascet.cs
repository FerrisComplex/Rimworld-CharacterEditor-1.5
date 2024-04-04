// Decompiled with JetBrains decompiler
// Type: CharacterEditor.CharacterEditorCascet
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CharacterEditor;

public class CharacterEditorCascet : Building_CryptosleepCasket
{
    private Pawn innerPawn;

    public CharacterEditorCascet()
    {
        innerPawn = null;
    }

    public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
    {
        var flag = base.TryAcceptThing(thing, allowSpecialEffects);
        if (flag)
        {
            innerPawn = thing as Pawn;
            CEditor.API.Get<Dictionary<int, Building_CryptosleepCasket>>(EType.UIContainers)[0] = this;
            CEditor.API.StartEditor(innerPawn);
        }

        return flag;
    }
}
