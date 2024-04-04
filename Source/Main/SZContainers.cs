// Decompiled with JetBrains decompiler
// Type: CharacterEditor.SZContainers
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class SZContainers
{
    internal static bool DrawElementStack<T>(Rect rect, List<T> l, bool bRemoveOnClick, Action<T> removeAction, Func<T, Def> defGetter = null)
    {
        bool flag = l.NullOrEmpty<T>();
        bool result;
        if (flag)
        {
            result = false;
        }
        else
        {
            try
            {
                GenUI.DrawElementStack<T>(rect, 32f, l, delegate(Rect r, T def)
                {
                    GUI.DrawTexture(r, BaseContent.ClearTex);
                    bool flag2 = Mouse.IsOver(r);
                    if (flag2)
                    {
                        Widgets.DrawHighlight(r);
                        string text = def.STooltip<T>();
                        TipSignal tip = new TipSignal(text, 987654);
                        TooltipHandler.TipRegion(r, tip);
                    }
                    Texture2D texture2D = def.GetTIcon(null);
                    texture2D = (texture2D ?? BaseContent.BadTex);
                    bool flag3 = Widgets.ButtonImage(r, texture2D, false);
                    if (flag3)
                    {
                        bool bRemoveOnClick2 = bRemoveOnClick;
                        if (bRemoveOnClick2)
                        {
                            removeAction(def);
                            throw new Exception("removed");
                        }
                        bool flag4 = defGetter != null;
                        if (flag4)
                        {
                            WindowTool.Open(new Dialog_InfoCard(defGetter(def), null));
                        }
                    }
                }, (T def) => 32f, 4f, 5f, false);
            }
            catch
            {
            }
            result = true;
        }
        return result;
    }

}
