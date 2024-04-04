using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace CharacterEditor
{
	
	internal static class WindowTool
	{
		
		internal static void SimpleCloseButton(Window w)
		{
			SZWidgets.ButtonText(WindowTool.RAcceptButton(w), "Close".Translate(), delegate()
			{
				w.Close(true);
			}, "");
		}

		
		internal static void SimpleAcceptButton(Window w, Action action)
		{
			SZWidgets.ButtonText(WindowTool.RAcceptButton(w), "OK".Translate(), action, "");
		}

		
		internal static void SimpleSaveButton(Window w, Action action)
		{
			SZWidgets.ButtonText(WindowTool.RAcceptButton(w), "Save".Translate(), action, "");
		}

		
		internal static void SimpleAcceptAndExtend(Window w, Action aOk, Action aReset, Action aResetAll, Action aSave, int widthExtended, string customLabel)
		{
			w.windowRect.width = (CEditor.IsExtendedUI ? ((float)widthExtended) : WindowTool.DefaultToolWindow.x);
			WindowTool.toggleText = (CEditor.IsExtendedUI ? "<<" : ">>");
			int num = WindowTool.X_Accept(w) - 30;
			int num2 = WindowTool.Y_Accept(w);
			int num3 = 30;
			Rect rect = new Rect((float)num, (float)num2, 100f, (float)num3);
			SZWidgets.ButtonText(rect, customLabel, aOk, "");
			num += 100;
			SZWidgets.ButtonText((float)num, (float)num2, (float)num3, (float)num3, WindowTool.toggleText, delegate()
			{
				CEditor.IsExtendedUI = !CEditor.IsExtendedUI;
			}, "");
			num += num3;
			bool isExtendedUI = CEditor.IsExtendedUI;
			if (isExtendedUI)
			{
				int num4 = widthExtended - (int)WindowTool.DefaultToolWindow.x;
				int num5 = num4 / 3;
				SZWidgets.ButtonText((float)num, (float)num2, (float)num5, (float)num3, Label.RESET, aReset, "");
				num += num5;
				SZWidgets.ButtonText((float)num, (float)num2, (float)num5, (float)num3, Label.RESETALL, aResetAll, "");
				num += num5;
				SZWidgets.ButtonText((float)num, (float)num2, (float)num5, (float)num3, Label.SAVE, aSave, "");
			}
		}

		
		internal static void SimpleCustomButton(Window w, int xPos, Action action, string label, string tooltip)
		{
			int num = WindowTool.Y_Accept(w);
			int num2 = 30;
			Rect rect = new Rect((float)xPos, (float)num, 100f, (float)num2);
			SZWidgets.ButtonText(rect, label, action, tooltip);
		}

		
		internal static Rect RAcceptButton(Window w)
		{
			return new Rect((float)WindowTool.X_Accept(w), (float)WindowTool.Y_Accept(w), 100f, 30f);
		}

		
		internal static int X_Accept(Window w)
		{
			return (int)w.InitialSize.x - 136;
		}

		
		internal static int Y_Accept(Window w)
		{
			return (int)w.InitialSize.y - 66;
		}

		
		
		internal static int MaxH
		{
			get
			{
				return (UI.screenHeight < 768) ? UI.screenHeight : ((UI.screenHeight < 1200) ? 768 : 800);
			}
		}

		
		
		internal static int MaxHS
		{
			get
			{
				return 700;
			}
		}

		
		
		internal static int ToolW
		{
			get
			{
				return 320;
			}
		}

		
		
		internal static Vector2 DefaultToolWindow
		{
			get
			{
				return new Vector2((float)WindowTool.ToolW, (float)WindowTool.MaxH);
			}
		}

		
		internal static void Open(Window w)
		{
			w.layer = CEditor.Layer;
			Find.WindowStack.Add(w);
		}

		
		internal static void TopLayerForWindowOf<T>(bool force) where T : Window
		{
			Window w = Find.WindowStack.WindowOfType<T>();
			WindowTool.BringToFront(w, force);
		}

		
		internal static void TopLayerForWindowOfType(string type, bool force)
		{
			Window windowOfType = WindowTool.GetWindowOfType(type);
			WindowTool.BringToFront(windowOfType, force);
		}

		
		internal static void BringToFront(Window w, bool force)
		{
			bool flag = w != null;
			if (flag)
			{
				bool flag2 = w.layer != CEditor.Layer;
				if (flag2)
				{
					w.layer = CEditor.Layer;
					GUI.BringWindowToFront(w.ID);
				}
				else if (force)
				{
					GUI.BringWindowToFront(w.ID);
				}
			}
		}

		
		internal static void BringToFrontMulti(List<Window> l)
		{
			bool flag = l.NullOrEmpty<Window>();
			if (!flag)
			{
				l.Reverse();
				foreach (Window w in l)
				{
					WindowTool.BringToFront(w, false);
				}
			}
		}

		
		internal static int GetIndex(Window w)
		{
			return Find.WindowStack.Windows.IndexOf(w);
		}

		
		internal static Window GetWindowOf<T>() where T : Window
		{
			return Find.WindowStack.WindowOfType<T>();
		}

		
		internal static bool IsOpen<T>() where T : Window
		{
			return Find.WindowStack.IsOpen(typeof(T));
		}

		
		internal static void TryRemove<T>() where T : Window
		{
			Find.WindowStack.TryRemove(typeof(T), true);
		}

		
		internal static Window GetWindowOfType(string type)
		{
			foreach (Window window in Find.WindowStack.Windows)
			{
				bool flag = window.GetType().ToString() == type;
				if (flag)
				{
					return window;
				}
			}
			return null;
		}

		
		internal static void ShowOpenedWindows()
		{
			foreach (Window window in Find.WindowStack.Windows)
			{
				MessageTool.Show(window.GetType().ToString(), null);
			}
		}

		
		internal static List<Window> GetWindowOfStartsWithType(string type)
		{
			List<Window> list = new List<Window>();
			foreach (Window window in Find.WindowStack.Windows)
			{
				bool flag = window.GetType().ToString().StartsWith(type);
				if (flag)
				{
					list.Add(window);
				}
			}
			return list;
		}

		
		internal static List<Window> GetWindowOfEndsWithType(string type)
		{
			List<Window> list = new List<Window>();
			foreach (Window window in Find.WindowStack.Windows)
			{
				bool flag = window.GetType().ToString().EndsWith(type);
				if (flag)
				{
					list.Add(window);
				}
			}
			return list;
		}

		
		internal static void Close(List<Window> l)
		{
			bool flag = l.NullOrEmpty<Window>();
			if (!flag)
			{
				foreach (Window window in l)
				{
					window.Close(true);
				}
			}
		}

		
		internal static string toggleText;
        
	}
}
