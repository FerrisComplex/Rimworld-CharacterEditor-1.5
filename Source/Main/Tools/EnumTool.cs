// Decompiled with JetBrains decompiler
// Type: CharacterEditor.EnumTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace CharacterEditor;

internal static class EnumTool
{
    internal static List<T> GetAllEnumsOfType<T>()
    {
        return Enum.GetValues(typeof(T)).OfType<T>().ToList();
    }

    internal static HashSet<string> GetEnumsNamesAsStringHashSet<T>()
    {
        return Enum.GetNames(typeof(T)).ToHashSet();
    }
}