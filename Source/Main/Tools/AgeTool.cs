// Decompiled with JetBrains decompiler
// Type: CharacterEditor.AgeTool
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using Verse;

namespace CharacterEditor;

internal static class AgeTool
{
    internal static void SetChronoAgeTicks(this Pawn p, long val)
    {
        if (!p.HasAgeTracker())
            return;
        p.ageTracker.SetMemberValue("birthAbsTicksInt", val);
    }

    internal static long GetAgeTicks(this Pawn p)
    {
        return !p.HasAgeTracker() ? 0L : p.ageTracker.AgeBiologicalTicks;
    }

    internal static long GetChronoAgeTicks(this Pawn p)
    {
        return !p.HasAgeTracker() ? 0L : p.ageTracker.BirthAbsTicks;
    }

    internal static void SetAgeTicks(this Pawn p, long ageTicks)
    {
        if (!p.HasAgeTracker())
            return;
        p.ageTracker.AgeBiologicalTicks = ageTicks;
        p.Recalculate_WorkTypes();
        CompatibilityTool.UpdateLifestage(p);
    }

    internal static void SetAge(this Pawn p, int age)
    {
        if (!p.HasAgeTracker())
            return;
        p.ageTracker.AgeBiologicalTicks = age * 3600000L;
        p.Recalculate_WorkTypes();
        CompatibilityTool.UpdateLifestage(p);
    }

    internal static void Recalculate_WorkTypes(this Pawn p)
    {
        p.workSettings?.Notify_DisabledWorkTypesChanged();
        p.Notify_DisabledWorkTypesChanged();
        p.RaceProps.ResolveReferencesSpecial();
        p.def.ResolveReferences();
    }

    internal static void SetChronoAge(this Pawn p, int age)
    {
        if (!p.HasAgeTracker())
            return;
        p.ageTracker.SetMemberValue("birthAbsTicksInt", GenTicks.TicksAbs - age * 3600000L);
    }

    internal static void SetChronoAgeDay(this Pawn p, int age, int ticks)
    {
        if (!p.HasAgeTracker())
            return;
        p.ageTracker.SetMemberValue("birthAbsTicksInt", GenTicks.TicksAbs - age * 3600000L - ticks * 60000L);
    }
}
