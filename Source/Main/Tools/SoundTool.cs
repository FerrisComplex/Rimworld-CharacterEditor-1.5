using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;
using Verse.Sound;

namespace CharacterEditor
{
	
	internal static class SoundTool
	{
		
		internal static void PlayThis(SoundDef def)
		{
			bool flag = def == null;
			if (!flag)
			{
				TargetInfo source = CEditor.InStartingScreen ? new TargetInfo(default(IntVec3), null, true) : new TargetInfo(UI.MouseMapPosition().ToIntVec3(), Find.CurrentMap, false);
				bool sustain = def.sustain;
				if (!sustain)
				{
					bool flag2 = !def.subSounds.NullOrEmpty<SubSoundDef>();
					if (flag2)
					{
						for (int i = 0; i < def.subSounds.Count; i++)
						{
							def.subSounds[i].TryPlay(source);
						}
						def.PlayOneShot(source);
					}
				}
			}
		}

		
		internal static void SetAndPlayPrev(ref SoundDef source, HashSet<SoundDef> l, Pawn p)
		{
			bool flag = source == null;
			if (!flag)
			{
				source = l.GetPrev(source);
				source.PlayPawnSound(p);
			}
		}

		
		internal static void SetAndPlayNext(ref SoundDef source, HashSet<SoundDef> l, Pawn p)
		{
			source = l.GetNext(source);
			source.PlayPawnSound(p);
		}

		
		internal static void SetAndPlayPawnSound(ref SoundDef source, SoundDef value, Pawn p)
		{
			source = value;
			source.PlayPawnSound(p);
		}

		
		internal static SoundDef GetAndPlay(SoundDef value)
		{
			SoundTool.PlayThis(value);
			return value;
		}

		
		internal static SoundDef GetAndPlayPawnSoundCur(SoundDef value)
		{
			value.PlayPawnSound(CEditor.API.Pawn);
			return value;
		}

		
		internal static void PlayPawnSoundCur(SoundDef value)
		{
			value.PlayPawnSound(CEditor.API.Pawn);
		}

		
		internal static void PlayPawnSound(this SoundDef def, Pawn p)
		{
			LifeStageUtility.PlayNearestLifestageSound(p, (LifeStageAge ls) => def, (GeneDef g) => def, (x) => def);
		}
	}
}
