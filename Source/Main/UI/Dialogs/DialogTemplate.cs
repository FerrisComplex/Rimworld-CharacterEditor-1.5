using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor
{
	
	internal abstract class DialogTemplate<T> : Window where T : Def
	{
		
		
		internal float ELEMENTH
		{
			get
			{
				return SZWidgets.GetGraphicH<T>();
			}
		}

		
		
		internal bool IsGeneDef
		{
			get
			{
				return this.selectedDef.GetType() == typeof(GeneDef);
			}
		}

		
		internal void ToggleRemove()
		{
			this.iTick_AllowRemoveStat = ((this.iTick_AllowRemoveStat > 0) ? 0 : 2000);
		}

		
		
		internal Color RemoveColor
		{
			get
			{
				return (this.iTick_AllowRemoveStat > 0) ? Color.red : Color.white;
			}
		}

		
		
		internal int WPARAM
		{
			get
			{
				return 1000 - (int)WindowTool.DefaultToolWindow.x;
			}
		}

		
		
		public override Vector2 InitialSize
		{
			get
			{
				return WindowTool.DefaultToolWindow;
			}
		}

		
		internal DialogTemplate(SearchTool.SIndex listIdx, string _title, int _xPosOffset = 0)
		{
			this.title = _title;
			this.xPosOffset = _xPosOffset;
			this.scrollPosParam = default(Vector2);
			this.view = new Listing_X();
			this.selected_StatFactor = null;
			this.selected_StatOffset = null;
			this.iTick_AllowRemoveStat = 0;
			this.bRemoveOnClick = false;
			this.bDoOnce = true;
			this.idxList = listIdx;
			this.search = SearchTool.Update(this.idxList);
			this.x = 0;
			this.y = 0;
			this.lMods = this.ListModnames();
			this.Preselection();
			this.doCloseX = true;
			this.absorbInputAroundWindow = true;
			this.closeOnClickedOutside = true;
			this.closeOnCancel = true;
			this.draggable = true;
			this.layer = CEditor.Layer;
		}

		
		internal virtual HashSet<string> ListModnames()
		{
			return DefTool.ListModnamesWithNull<T>(null).ToHashSet<string>();
		}

		
		internal virtual void Preselection()
		{
			this.ASelectedModName(this.search.modName);
			this.selectedDef = this.lDefs.FirstOrDefault<T>();
		}

		
		public override void DoWindowContents(Rect inRect)
		{
			this.SizeAndPosition();
			this.DrawTitle(this.x, this.y, this.frameW, 30);
			this.y += 30;
			bool flag = !this.mInPlacingMode;
			if (flag)
			{
				SZWidgets.ButtonImage((float)(this.frameW - 25), 0f, 25f, 25f, "brandom", new Action(this.ARandomDef), "", default(Color));
			}
			this.DrawDropdownModname(this.x, this.y, this.frameW, 30);
			this.y += 30;
			this.y += this.DrawCustomFilter(this.x, this.y, this.frameW);
			this.y += 2;
			Text.Font = GameFont.Small;
			SZWidgets.ListView<T>((float)this.x, (float)this.y, (float)this.frameW, (float)(this.frameH + 28 - this.y), this.lDefs, (T def) => def.SLabel(), (T def) => def.STooltip<T>(), new Func<T, T, bool>(DefTool.DefNameComparator<T>), ref this.selectedDef, ref this.search.scrollPos, false, null, true, false, false, false);
			bool flag2 = !DefTool.DefNameComparator<T>(this.oldSelectedDef, this.selectedDef);
			if (flag2)
			{
				this.oldSelectedDef = this.selectedDef;
				bool flag3 = Prefs.DevMode && this.selectedDef != null;
				if (flag3)
				{
					MessageTool.Show(this.selectedDef.defName, null);
				}
				this.OnSelectionChanged();
			}
			bool isExtendedUI = CEditor.IsExtendedUI;
			if (isExtendedUI)
			{
				this.DrawParameterBase();
			}
			this.DrawLowerButtons();
		}

		
		internal void DrawParameterBase()
		{
			bool flag = this.selectedDef == null;
			if (!flag)
			{
				this.CalcHSCROLL();
				this.id = 1;
				bool flag2 = this.iTick_AllowRemoveStat > 0;
				if (flag2)
				{
					this.iTick_AllowRemoveStat--;
				}
				this.bRemoveOnClick = (this.iTick_AllowRemoveStat > 0);
				Rect outRect = new Rect(WindowTool.DefaultToolWindow.x - 20f, 0f, (float)(this.WPARAM - 12), (float)(this.frameH + 20));
				Rect rect = new Rect(0f, 0f, outRect.width - 16f, (float)this.hScrollParam);
				Widgets.BeginScrollView(outRect, ref this.scrollPosParam, rect, true);
				Rect rect2 = rect.ContractedBy(4f);
				rect2.y -= 4f;
				rect2.height = (float)this.hScrollParam;
				this.view.Begin(rect2);
				this.view.verticalSpacing = 30f;
				this.DrawParameter();
				this.view.End();
				Widgets.EndScrollView();
			}
		}

		
		internal abstract void CalcHSCROLL();

		
		internal abstract void DrawParameter();

		
		internal abstract void AReset();

		
		internal abstract void AResetAll();

		
		internal abstract void ASave();

		
		internal abstract HashSet<T> TList();

		
		internal virtual void DrawLowerButtons()
		{
			WindowTool.SimpleAcceptAndExtend(this, new Action(this.DoAndClose), new Action(this.AReset), new Action(this.AResetAll), new Action(this.ASave), 1000, this.customAcceptLabel);
		}

		
		private void SizeAndPosition()
		{
			bool flag = this.bDoOnce;
			if (flag)
			{
				SearchTool.SetPosition(this.idxList, ref this.windowRect, ref this.bDoOnce, this.xPosOffset);
			}
			this.frameW = (int)this.InitialSize.x - 40;
			this.frameH = (int)this.InitialSize.y - 115;
			this.y = 0;
			this.x = 0;
		}

		
		internal virtual void DrawTitle(int x, int y, int w, int h)
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect((float)x, (float)y, (float)w, (float)h), this.title);
		}

		
		private void DrawDropdownModname(int x, int y, int w, int h)
		{
			Text.Font = GameFont.Small;
			Rect rect = new Rect((float)x, (float)y, (float)w, (float)h);
			SZWidgets.FloatMenuOnButtonText<string>(rect, FLabel.TString(this.search.modName), this.lMods, new Func<string, string>(FLabel.TString), new Action<string>(this.ASelectedModName), "");
		}

		
		internal virtual void ASelectedModName(string val)
		{
			this.search.modName = val;
			this.lDefs = this.TList();
		}

		
		internal abstract int DrawCustomFilter(int x, int y, int w);

		
		internal abstract void OnAccept();

		
		internal abstract void OnSelectionChanged();

		
		private void DoAndClose()
		{
			bool flag = this.selectedDef != null;
			if (flag)
			{
				this.OnAccept();
			}
			bool flag2 = !this.mInPlacingMode;
			if (flag2)
			{
				this.Close(true);
			}
		}

		
		public override void OnAcceptKeyPressed()
		{
			base.OnAcceptKeyPressed();
			this.DoAndClose();
		}

		
		public override void Close(bool doCloseSound = true)
		{
			SearchTool.Save(this.idxList, this.windowRect.position);
			base.Close(doCloseSound);
		}

		
		private void ARandomDef()
		{
			DefTool.RandomSearchedDef<T>(this.lDefs, ref this.selectedDef);
		}
        

		
		private SearchTool.SIndex idxList;

		
		internal Vector2 scrollPosParam;

		
		private bool bDoOnce;

		
		internal bool bRemoveOnClick;

		
		internal bool mInPlacingMode = false;

		
		internal int id;

		
		internal StatDef selected_StatFactor = null;

		
		internal StatDef selected_StatOffset = null;

		
		private string title;

		
		internal string customAcceptLabel;

		
		internal T selectedDef;

		
		internal T oldSelectedDef;

		
		internal HashSet<T> lDefs;

		
		internal HashSet<string> lMods;

		
		internal SearchTool search;

		
		internal int x;

		
		internal int y;

		
		internal int frameW;

		
		internal int frameH;

		
		internal int xPosOffset;

		
		internal int hScrollParam;

		
		internal Listing_X view;

		
		internal int iTick_AllowRemoveStat;

		
		internal const int WIDTH_EXTENDED = 1000;

		
		internal const int WIDTH_DEFAULT = 500;

		
		internal const float WTITLE = 500f;
        
	}
}
