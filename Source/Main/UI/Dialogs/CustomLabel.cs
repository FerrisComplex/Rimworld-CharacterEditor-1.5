// Decompiled with JetBrains decompiler
// Type: CharacterEditor.CustomLabel
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using Verse;

namespace CharacterEditor;

public static class CustomLabel
{
    public static void LoadLabelsFromFile(string path)
    {
        if (path.NullOrEmpty() || !FileIO.Exists(path))
            return;
        var input = FileIO.ReadFile(path);
        if (input == null || input.Length == 0)
            return;
        var s = input.AsStringUNICODE();
        if (s.NullOrEmpty())
            return;
        try
        {
            var strArray = s.SplitNo(";\r\n");
            Log.Message("Loading CharacterEditor labels from file... label-count: " + strArray.Length);
            foreach (var text in strArray)
                SetLabel(text.SubstringTo("=").Trim(), text.SubstringFrom("\"").SubstringTo("\""));
        }
        catch
        {
        }
    }

    public static string GetLabel(string labelName)
    {
        try
        {
            var atype = Reflect.GetAType("CharacterEditor", "Label");
            return (string)atype.GetMemberValue(labelName);
        }
        catch
        {
        }
        return null;
    }

    
    public static void SetLabel(string labelName, string text)
    {
        try
        {
            var atype = Reflect.GetAType("CharacterEditor", "Label");
            atype.SetMemberValue(labelName, text);
        }
        catch
        {
        }
    }
}
