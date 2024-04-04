// Decompiled with JetBrains decompiler
// Type: CharacterEditor.Preset
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using Verse;

namespace CharacterEditor;

internal static class Preset
{
    internal static string AsString<T>(SortedDictionary<T, string> dicParams)
    {
        var text = "";
        foreach (var key in dicParams.Keys)
            text = text + dicParams[key] + ",";
        return text.SubstringRemoveLast() + ";";
    }

    internal static void ResetAllToDefault<T>(
        Dictionary<string, T> allDefaults,
        Action<T> defaultAction,
        OptionS optionS,
        string type)
    {
        foreach (var obj in allDefaults.Values)
            defaultAction(obj);
        CEditor.API.SetCustom(optionS, "", "");
        MessageTool.Show("reset all " + type + " to originals done");
    }

    internal static void ResetToDefault<T>(
        Dictionary<string, T> allDefaults,
        Action<T> defaultAction,
        OptionS optionS,
        string identifier)
    {
        T obj;
        if (identifier.NullOrEmpty() || !allDefaults.TryGetValue(identifier, out obj))
            return;
        defaultAction(obj);
        CEditor.API.SetCustom(optionS, "", identifier);
        MessageTool.Show("reset " + identifier + " done");
    }

    internal static void LoadAllModifications(
        string custom,
        Action<string> loadAction,
        string type)
    {
        if (string.IsNullOrEmpty(custom))
            Log.Message("no modifications for " + type);
        else
            try
            {
                foreach (var str in custom.Trim().SplitNoEmpty(";"))
                    loadAction(str);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
            }
    }

    internal static bool LoadModification<T>(
        string custom,
        ref SortedDictionary<T, string> dicParams)
    {
        dicParams = new SortedDictionary<T, string>();
        if (!string.IsNullOrEmpty(custom))
        {
            var num = Enum.GetNames(typeof(T)).EnumerableCount();
            foreach (var str in custom.SplitNo(","))
                if (dicParams.Count < num)
                    dicParams.Add((T)Enum.Parse(typeof(T), dicParams.Count.ToString()), str);
        }

        return (uint)dicParams.Count > 0U;
    }

    internal static void SaveModification<TPreset, TSample>(
        TSample t,
        Func<TSample, TPreset> createAction,
        Action<TPreset> saveAction)
    {
        if (t == null)
            return;
        try
        {
            var preset = createAction(t);
            saveAction(preset);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message + "\n" + ex.StackTrace);
        }
    }

    internal static Dictionary<string, TPreset> CreateDefaults<TPreset, TSample>(
        HashSet<TSample> list,
        Func<TSample, string> idGetter,
        Func<TSample, TPreset> createAction,
        string type)
    {
        var dictionary = new Dictionary<string, TPreset>();
        foreach (var sample in list)
            if (idGetter(sample) != null)
                dictionary.Add(idGetter(sample), createAction(sample));
        Log.Message(dictionary.Count + " default entities for " + type + " created");
        return dictionary;
    }
}