// Decompiled with JetBrains decompiler
// Type: CharacterEditor.CompatibilityTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Text;
using Verse;

namespace CharacterEditor;

public static class CompatibilityTool
{
    public static string GetRJWTooltip(Pawn pawn)
    {
        return "";
    }

    public static void OpenRJWDialog(Pawn pawn)
    {
        var atype = Reflect.GetAType("rjw", "Dialog_Sexcard");
        if (!(atype != null))
            return;
        var instance = Activator.CreateInstance(atype, pawn);
        if (instance == null)
            return;
        WindowTool.Open(instance as Window);
    }

    public static string GetPersonalitiesTooltip(Pawn pawn)
    {
        var str = "";
        try
        {
            str = (string)pawn.GetPersoComp().CallMethod("GetDescription", null);
        }
        catch
        {
        }

        return str;
    }

    public static void OpenPersonalitiesDialog(Pawn pawn)
    {
        var atype = Reflect.GetAType("SPM1.UI", "Dialog_PersonalityEditor");
        if (!(atype != null))
            return;
        atype.CallMethod("OpenDialogFor", new object[1]
        {
            pawn
        });
    }

    public static string GetPsyche(this Pawn pawn)
    {
        var text = "";
        string str;
        if ((pawn == null ? 1 : !CEditor.IsPsychologyActive ? 1 : 0) != 0)
        {
            str = text;
        }
        else
        {
            var atype = Reflect.GetAType("Psychology", "PsycheCardUtility");
            if (atype != null)
                text = (string)atype.CallMethod("GetPsychology", new object[1]
                {
                    pawn
                });
            if (Prefs.DevMode)
                Log.Message("getting psychology of " + pawn.GetPawnName() + " =" + text);
            str = text.AsBase64(Encoding.UTF8);
        }

        return str;
    }

    public static void SetPsyche(this Pawn pawn, string data)
    {
        if ((pawn == null ? 1 : !CEditor.IsPsychologyActive ? 1 : 0) != 0)
            return;
        var str = data.Base64ToString(Encoding.UTF8);
        if (Prefs.DevMode)
            Log.Message("setting psychology of " + pawn.GetPawnName() + " =" + str);
        var atype = Reflect.GetAType("Psychology", "PsycheCardUtility");
        if (!(atype != null))
            return;
        atype.CallMethod("SetPsychology", new object[2]
        {
            pawn,
            str
        });
    }

    public static string GetPersonality(this Pawn pawn)
    {
        var text = "";
        string str;
        if ((pawn == null ? 1 : !CEditor.IsPersonalitiesActive ? 1 : 0) != 0)
        {
            str = text;
        }
        else
        {
            try
            {
                var atype = Reflect.GetAType("SPM1", "Extensions");
                if (atype != null)
                    text = (string)atype.CallMethod("ExtractPersonality", new object[1]
                    {
                        pawn
                    });
                if (Prefs.DevMode)
                    Log.Message("getting personality of " + pawn.GetPawnName() + " =" + text);
                return text.AsBase64(Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Log.Message("getting personality failed - this is not an issue of the editor! " + ex.Message + "\n" + ex.StackTrace);
            }

            str = "";
        }

        return str;
    }

    public static void SetPersonality(this Pawn pawn, string data)
    {
        if ((pawn == null ? 1 : !CEditor.IsPersonalitiesActive ? 1 : 0) != 0)
            return;
        try
        {
            var str = data.Base64ToString(Encoding.UTF8);
            if (Prefs.DevMode)
                Log.Message("setting personality of " + pawn.GetPawnName() + " =" + str);
            var atype = Reflect.GetAType("SPM1", "Extensions");
            if (!(atype != null))
                return;
            atype.CallMethod("IntractPersonality", new object[2]
            {
                pawn,
                str
            });
        }
        catch (Exception ex)
        {
            Log.Message("setting personality failed - this is not an issue of the editor! " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    public static string GetFavoriteColorTooltip(Pawn pawn)
    {
        var str1 = "";
        try
        {
            var str2 = string.Empty;
            if (pawn.Ideo != null && !pawn.Ideo.hidden)
                str2 = new TaggedString("OrIdeoColor".Translate(pawn.Named("PAWN")));
            var taggedString = "FavoriteColorTooltip".Translate(pawn.Named("PAWN"), 0.6f.ToStringPercent().Named("PERCENTAGE"), str2.Named("ORIDEO"));
            str1 = taggedString.Resolve();
        }
        catch
        {
        }

        return str1;
    }

    public static void UpdateLifestage(Pawn p)
    {
        if (!CEditor.IsAgeMattersActive)
            return;
        var hediffComp = CompTool.GetHediffComp(p, CEditor.IsAgeMattersActive, "LifeStageHediffAssociation");
        if (hediffComp == null)
            return;
        try
        {
            hediffComp.CallMethod("UpdateHediffDependingOnLifeStage", null);
        }
        catch
        {
        }
    }
}
