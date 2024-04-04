// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogCapsuleUI
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CharacterEditor;

internal class DialogCapsuleUI : Window
{
    private const bool showBeta = false;
    internal static List<Pawn> interstellarAnimals = new();
    internal static List<Pawn> interstellarPawns = new();
    private static List<ScenPart> lParts;
    private static List<ScenPart> lPartsAnimal;
    private static List<ScenPart> lPartsInterstellar;
    private static List<ScenPart> lPartsMap;
    private static List<ScenPart> lPartsScatter;
    private static List<ScenPart> lPartsTaken;
    private bool addToOwn;
    private bool addToTaken;
    private int astronautPic;
    private bool bInterstellarView;
    private bool bShowRemove;
    private bool bShowShift;
    private int check = -50;
    private Dictionary<int, string> dicSlots;
    private bool doOnce;
    private int iTick;
    private float moveX;
    private float moveY;
    private System.Random random = new System.Random();
    private bool right;
    private Vector2 scrollPos_Animal;
    private Vector2 scrollPos_Interstellar;
    private Vector2 scrollPos_Map;
    private Vector2 scrollPos_Scatter;
    private Vector2 scrollPos_Taken;
    private ScenPart selectedPart_Animal;
    private ScenPart selectedPart_Interstellar;
    private ScenPart selectedPart_Map;
    private ScenPart selectedPart_Scatter;
    private ScenPart selectedPart_Taken;
    private bool up;
    private readonly float WTITLE = 300f;

    internal DialogCapsuleUI()
    {
        scrollPos_Animal = new Vector2();
        scrollPos_Map = new Vector2();
        scrollPos_Interstellar = new Vector2();
        scrollPos_Taken = new Vector2();
        scrollPos_Scatter = new Vector2();
        selectedPart_Animal = null;
        selectedPart_Map = null;
        selectedPart_Interstellar = null;
        selectedPart_Taken = null;
        selectedPart_Scatter = null;
        lPartsMap = new List<ScenPart>();
        if (lPartsInterstellar == null)
            lPartsInterstellar = new List<ScenPart>();
        bShowRemove = false;
        bInterstellarView = !CEditor.InStartingScreen;
        doOnce = true;
        astronautPic = 1;
        SearchTool.Update(SearchTool.SIndex.Capsule);
        UpdateLists();
        InitSlots();
        if (!CEditor.InStartingScreen)
            ScanMap();
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnCancel = true;
        closeOnClickedOutside = true;
        draggable = true;
        layer = CEditor.Layer;
    }

    public override Vector2 InitialSize => new(1120f, WindowTool.MaxH);

    private Rect rectLoad => new(0.0f, (int)base.InitialSize.y - 70, 120f, 30f);

    private Rect rectSave => new(120f, (int)base.InitialSize.y - 70, 120f, 30f);

    private Rect rectGrab => new(240f, (int)base.InitialSize.y - 70, 120f, 30f);

    private Rect rectEntladen => new(360f, (int)base.InitialSize.y - 70, 120f, 30f);

    private Rect rectLiftOff => new(480f, (int)base.InitialSize.y - 70, 120f, 30f);

    private Rect rectLiftOff2 => new(600f, (int)base.InitialSize.y - 70, 120f, 30f);

    internal static string SLOTPATH(int index)
    {
        return pathCapsuleSets() + Path.DirectorySeparatorChar + "capsulesetup" + index + ".txt";
    }

    internal static string pathCapsuleSets()
    {
        var str = GenFilePaths.ConfigFolderPath.Replace("Config", "");
        var path = str.Remove(str.Length - 1) + Path.DirectorySeparatorChar + "CharacterEditor";
        FileIO.CheckOrCreateDir(path);
        return path;
    }

    private void InitSlots()
    {
        dicSlots = new Dictionary<int, string>();
        var numCapsuleSlots = CEditor.API.NumCapsuleSlots;
        for (var key = 0; key < numCapsuleSlots; ++key)
            dicSlots.Add(key, "");
        UpdateSlots();
    }

    private void UpdateSlots()
    {
        var numCapsuleSlots = CEditor.API.NumCapsuleSlots;
        for (var index = 0; index < numCapsuleSlots; ++index)
            if (FileIO.Exists(SLOTPATH(index)))
            {
                var s = FileIO.ReadFile(SLOTPATH(index)).AsString(Encoding.UTF8);
                if (!s.NullOrEmpty())
                {
                    var strArray = s.SplitNo("\n");
                    if (strArray.Length > 1)
                        dicSlots[index] = strArray[0];
                }
            }
    }

    public override void DoWindowContents(Rect inRect)
    {
        bool flag = this.doOnce;
        if (flag)
        {
            SearchTool.SetPosition(SearchTool.SIndex.Capsule, ref this.windowRect, ref this.doOnce, 105);
        }
        GUI.DrawTextureWithTexCoords(new Rect(this.moveX, this.moveY, 1332f, 850f), ContentFinder<Texture2D>.Get("bcapsule_back", true), new Rect(0f, 0f, 1f, 1f));
        this.DrawUIObjects();
        bool flag2 = !this.bInterstellarView;
        if (flag2)
        {
            this.DrawTakenContainer(13, 428, 392, 256);
        }
        else
        {
            this.DrawInterstellarContainer(13, 428, 392, 256);
        }
        bool flag3 = !this.bInterstellarView;
        if (flag3)
        {
            this.DrawScatterContainer(673, 428, 392, 256);
        }
        else
        {
            this.DrawMapContainer(673, 428, 392, 256);
        }
        bool flag4 = !this.bInterstellarView;
        if (flag4)
        {
            this.DrawAnimalContainer(805, 148, 240, 190);
        }
        this.DrawButtons(0, (int)this.InitialSize.y - 70, (int)this.InitialSize.x - 32, 30);
        WindowTool.SimpleCloseButton(this);
        this.ShakingImage();
    }

    public override void Close(bool doCloseSound = true)
    {
        SearchTool.Save(SearchTool.SIndex.Capsule, this.windowRect.position);
        foreach (ScenPart scenPart in ScenarioTool.ScenarioParts)
        {
            scenPart.ExposeData();
        }
        base.Close(doCloseSound);
    }

    private void ShakingImage()
    {
        bool flag = this.astronautPic == 2;
        if (!flag)
        {
            bool flag2 = this.right;
            if (flag2)
            {
                this.moveX += 0.15f;
            }
            else
            {
                this.moveX -= 0.15f;
            }
            bool flag3 = this.moveX <= -200f;
            if (flag3)
            {
                this.right = true;
            }
            else
            {
                bool flag4 = this.moveX >= 0f;
                if (flag4)
                {
                    this.right = false;
                }
            }
            int num = this.random.Next((int)this.moveY, 2);
            this.up = (num > 0 || num > this.check);
            bool flag5 = this.up && this.moveY >= -100f;
            if (flag5)
            {
                this.moveY -= 0.15f;
            }
            else
            {
                bool flag6 = !this.up && this.moveY < 0f;
                if (flag6)
                {
                    this.moveY += 0.15f;
                }
            }
            this.iTick++;
            bool flag7 = this.iTick > 1000;
            if (flag7)
            {
                this.iTick = 0;
                this.check = this.random.Next(-100, 30);
            }
        }
    }

    private void DrawUIObjects()
		{
			Text.Font = GameFont.Medium;
			bool inStartingScreen = CEditor.InStartingScreen;
			if (inStartingScreen)
			{
				Widgets.Label(new Rect(0f, 0f, this.WTITLE, 30f), Label.RETTUNGSKAPSEL);
			}
			else
			{
				Widgets.DrawHighlight(new Rect(0f, 0f, this.WTITLE, 30f));
				SZWidgets.ButtonTextureTextHighlight(new Rect(0f, 0f, this.WTITLE, 30f), this.bInterstellarView ? Label.INTERSTELLARLINK : Label.RETTUNGSKAPSEL, null, ColorTool.colAsche, delegate
				{
					this.bInterstellarView = !this.bInterstellarView;
				}, Label.INTERSTELLARLINK_DESC);
			}
			Text.Font = GameFont.Small;
			Rect rect = new Rect(480f, 50f, 120f, 30f);
			Widgets.DrawBoxSolid(rect, new Color(0.8f, 0.2f, 0.2f, 0.5f));
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect, CEditor.InStartingScreen ? Label.START : "Scan Map");
			bool inStartingScreen2 = CEditor.InStartingScreen;
			if (inStartingScreen2)
			{
				SZWidgets.ButtonInvisible(rect, new Action(this.AStart), "");
			}
			else
			{
				SZWidgets.ButtonInvisible(rect, new Action(this.ScanMap), "");
			}
			bool flag = Mouse.IsOver(rect);
			if (flag)
			{
				Widgets.DrawHighlight(rect);
			}
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect2 = new Rect(15f, 50f, 160f, 160f);
			bool flag2 = Mouse.IsOver(rect2);
			if (flag2)
			{
				GUI.DrawTexture(rect2, ContentFinder<Texture2D>.Get("bastronaut1a", true));
			}
			else
			{
				GUI.DrawTexture(rect2, ContentFinder<Texture2D>.Get("bastronaut" + this.astronautPic.ToString(), true));
			}
			SZWidgets.ButtonInvisible(new Rect(15f, 50f, 160f, 160f), new Action(this.AOnAstronaut), "");
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(150f, 176f, 30f, 30f), ScenarioTool.CurrentTakenPawnCount.ToString());
			bool flag3 = !this.bInterstellarView;
			if (flag3)
			{
				Widgets.DrawBoxSolid(new Rect(802f, 110f, 248f, 250f), new Color(0.2f, 0.2f, 0.2f, 0.5f));
				Widgets.Label(new Rect(807f, 115f, 200f, 30f), Label.TIERKAPSEL);
			}
			Widgets.DrawBoxSolid(new Rect(10f, 390f, 400f, 320f), new Color(0.2f, 0.2f, 0.2f, 0.5f));
			Widgets.Label(new Rect(15f, 395f, 395f, 30f), this.bInterstellarView ? Label.BACKUPCONTAINER : Label.KAPSELCONTAINER);
			Widgets.DrawBoxSolid(new Rect(670f, 390f, 400f, 320f), new Color(0.2f, 0.2f, 0.2f, 0.5f));
			Widgets.Label(new Rect(675f, 395f, 395f, 30f), this.bInterstellarView ? Label.AUFDERKARTE : Label.TEILEIMSCHIFFSWRACK);
			Text.Font = GameFont.Small;
		}

    private void DrawButtons(int x, int y, int w, int h)
    {
        if (bInterstellarView)
        {
            SZWidgets.ButtonText(rectEntladen, Label.UNLOAD, Entladen, Label.UNLOAD_DESC);
            SZWidgets.ButtonText(rectGrab, Label.LOAD, Beladen, Label.LOAD_DESC);
            SZWidgets.ButtonText(rectLiftOff, Label.STARTTONEWWORLDS, NeuenPlanetSuchen, Label.TONEWWORLD_DESC);
        }
        else
        {
            SZWidgets.FloatMenuOnButtonText(rectLoad, new TaggedString("Load".Translate()), dicSlots.Keys.ToList(), s => Label.LOADSLOT + s + "\n" + dicSlots[s], AOnLoadSlot);
            SZWidgets.FloatMenuOnButtonText(rectSave, new TaggedString("Save".Translate()), dicSlots.Keys.ToList(), s => Label.SAVESLOT + s + "\n" + dicSlots[s], AOnSaveSlot);
        }
    }

    private void DrawInterstellarContainer(int x, int y, int w, int h)
    {
        SZWidgets.ButtonImage((float)(x + w - 25), (float)(y - 30), 24f, 24f, "UI/Buttons/Dev/Add", new Action(this.AAddOwnParts), "", default(Color));
        SZWidgets.ButtonImage((float)(x + w - 50), (float)(y - 30), 24f, 24f, "bminus", new Action(this.AAllowRemoveParts), "", this.bShowRemove ? Color.red : Color.white);
        SZWidgets.ButtonImage((float)(x + w - 75), (float)(y - 30), 24f, 24f, "bmoveright", new Action(this.AShowShift), "", this.bShowShift ? Color.red : Color.white);
        SZWidgets.FullListviewScenPart(new Rect((float)x, (float)y, (float)w, (float)h), DialogCapsuleUI.lPartsInterstellar, this.bShowRemove, delegate(ScenPart p)
        {
            DialogCapsuleUI.lPartsInterstellar.Remove(p);
        }, this.bShowShift ? "bmoveright" : null, new Action<ScenPart>(this.ADropWithPod), false, true, ref this.scrollPos_Interstellar, ref this.selectedPart_Interstellar);
    }

    private void DrawTakenContainer(int x, int y, int w, int h)
    {
        SZWidgets.ButtonImage((float)(x + w - 25), (float)(y - 30), 24f, 24f, "UI/Buttons/Dev/Add", new Action(this.AAddParts), "", default(Color));
        SZWidgets.ButtonImage((float)(x + w - 50), (float)(y - 30), 24f, 24f, "bminus", new Action(this.AAllowRemoveParts), "", this.bShowRemove ? Color.red : Color.white);
        SZWidgets.ButtonImage((float)(x + w - 75), (float)(y - 30), 24f, 24f, "bmoveright", new Action(this.AShowShift), "", this.bShowShift ? Color.red : Color.white);
        SZWidgets.FullListviewScenPart(new Rect((float)x, (float)y, (float)w, (float)h), DialogCapsuleUI.lPartsTaken, this.bShowRemove, delegate(ScenPart p)
        {
            DialogCapsuleUI.lPartsTaken.Remove(p);
            DialogCapsuleUI.lParts.Remove(p);
        }, this.bShowShift ? "bmoveright" : null, new Action<ScenPart>(this.AMoveToMap), false, true, ref this.scrollPos_Taken, ref this.selectedPart_Taken);
    }

    private void DrawScatterContainer(int x, int y, int w, int h)
    {
        SZWidgets.ButtonImage((float)(x + w - 25), (float)(y - 30), 24f, 24f, "UI/Buttons/Dev/Add", new Action(this.AAddPartsMap), "", default(Color));
        SZWidgets.ButtonImage((float)(x + w - 50), (float)(y - 30), 24f, 24f, "bminus", new Action(this.AAllowRemoveParts), "", this.bShowRemove ? Color.red : Color.white);
        SZWidgets.ButtonImage((float)(x + w - 75), (float)(y - 30), 24f, 24f, "bmoveleft", new Action(this.AShowShift), "", this.bShowShift ? Color.red : Color.white);
        SZWidgets.FullListviewScenPart(new Rect((float)x, (float)y, (float)w, (float)h), DialogCapsuleUI.lPartsScatter, this.bShowRemove, delegate(ScenPart p)
        {
            DialogCapsuleUI.lPartsScatter.Remove(p);
            DialogCapsuleUI.lParts.Remove(p);
        }, this.bShowShift ? "bmoveleft" : null, new Action<ScenPart>(this.AMoveToContainer), false, true, ref this.scrollPos_Scatter, ref this.selectedPart_Scatter);
    }

    private void DrawMapContainer(int x, int y, int w, int h)
    {
        SZWidgets.ButtonImage((float)(x + w - 50), (float)(y - 30), 24f, 24f, "bminus", new Action(this.AAllowRemoveParts), "", this.bShowRemove ? Color.red : Color.white);
        SZWidgets.ButtonImage((float)(x + w - 75), (float)(y - 30), 24f, 24f, "bmoveleft", new Action(this.AShowShift), "", this.bShowShift ? Color.red : Color.white);
        SZWidgets.FullListviewScenPart(new Rect((float)x, (float)y, (float)w, (float)h), DialogCapsuleUI.lPartsMap, this.bShowRemove, delegate(ScenPart p)
        {
            Selected selectedScenarioPart = p.GetSelectedScenarioPart<ScenPart>();
            DialogCapsuleUI.lPartsMap.Remove(p);
            ThingTool.FindThingOnMap(selectedScenarioPart, Find.CurrentMap, selectedScenarioPart.stackVal, true);
        }, this.bShowShift ? "bmoveleft" : null, new Action<ScenPart>(this.AMoveToInterstellar), true, true, ref this.scrollPos_Map, ref this.selectedPart_Map);
    }
    private void DrawAnimalContainer(int x, int y, int w, int h)
    {
        SZWidgets.ButtonImage((float)(x + w - 25), (float)(y - 30), 24f, 24f, "UI/Buttons/Dev/Add", new Action(this.AAddPartsAnimal), "", default(Color));
        SZWidgets.ButtonImage((float)(x + w - 50), (float)(y - 30), 24f, 24f, "bminus", new Action(this.AAllowRemoveParts), "", default(Color));
        SZWidgets.FullListviewScenPart(new Rect((float)x, (float)y, (float)w, (float)h), DialogCapsuleUI.lPartsAnimal, this.bShowRemove, delegate(ScenPart p)
        {
            DialogCapsuleUI.lPartsAnimal.Remove(p);
            DialogCapsuleUI.lParts.Remove(p);
        }, null, null, false, true, ref this.scrollPos_Animal, ref this.selectedPart_Animal);
    }

    private static void UpdateLists()
    {
        lParts = ScenarioTool.ScenarioParts;
        lPartsTaken = new List<ScenPart>();
        lPartsScatter = new List<ScenPart>();
        lPartsAnimal = new List<ScenPart>();
        foreach (var lPart in lParts)
            if (lPart.IsSupportedScenarioPart())
            {
                if (lPart.IsScatterAnywherePart())
                    lPartsScatter.Add(lPart);
                else if (lPart.IsScenarioAnimal())
                    lPartsAnimal.Add(lPart);
                else
                    lPartsTaken.Add(lPart);
            }
    }

    private void ScanMap()
    {
        DialogCapsuleUI.lPartsMap.Clear();
        List<Selected> list = ThingTool.GrabAllThingsFromMap(Find.CurrentMap, false);
        list = (from x in list
            orderby x.thingDef.BaseMarketValue descending, x.stackVal descending
            select x).ToList<Selected>();
        foreach (Selected s in list)
        {
            ScenarioTool.MergeNonScenarioPart(s, ref DialogCapsuleUI.lPartsMap, false);
        }
    }

    private void InsOrbit()
    {
        AOnSaveSlot(-1);
        CEditor.bStartNewGame2 = true;
        CEditor.bGamePlus = true;
        CEditor.oldScenario = Find.Scenario;
        CEditor.oldStoryteller = Find.Storyteller;
        Find.WindowStack.Add(new ITickWin2());
        base.Close();
        CEditor.API.CloseEditor();
    }

    private void NeuenPlanetSuchen()
    {
        for (var index = ScenarioTool.ScenarioParts.Count - 1; index >= 0; --index)
        {
            var scenarioPart = ScenarioTool.ScenarioParts[index];
            if (scenarioPart.IsScenarioAnimal())
            {
                lPartsAnimal.Remove(scenarioPart);
                lParts.Remove(scenarioPart);
            }
        }

        foreach (var spawnedColonyAnimal in Find.CurrentMap.mapPawns.SpawnedColonyAnimals)
        {
            var s = Selected.ByThing(spawnedColonyAnimal);
            s.age = spawnedColonyAnimal.ageTracker.AgeBiologicalYears;
            s.gender = spawnedColonyAnimal.gender;
            s.pawnName = spawnedColonyAnimal.Name;
            ScenarioTool.AddScenarioPartAnimal(s);
        }

        AOnSaveSlot(-1);
        CEditor.bStartNewGame = true;
        CEditor.bGamePlus = true;
        CEditor.oldScenario = null;
        CEditor.oldStoryteller = null;
        SoundDefOf.Archotech_Invoked.PlayOneShot(new Building_ArchonexusCore());
        ScreenFader.StartFade(Color.white, 6f);
        Find.WindowStack.Add(new ITickWin());
        base.Close();
        CEditor.API.CloseEditor();
    }

    private void Entladen()
    {
        var l = new List<Selected>();
        for (var index = lPartsInterstellar.Count - 1; index >= 0; --index)
        {
            var selectedScenarioPart = lPartsInterstellar[index].GetSelectedScenarioPart();
            l.Add(selectedScenarioPart);
            lPartsInterstellar.Remove(lPartsInterstellar[index]);
        }

        PlacingTool.DropAllSelectedWithPod(l, new IntVec3());
        base.Close();
        CEditor.API.CloseEditor();
    }

    private void Beladen()
    {
        Map currentMap = Find.CurrentMap;
        List<Selected> list = ThingTool.GrabAllThingsFromMap(currentMap, true);
        foreach (Selected selected in list)
        {
            FleckMaker.ThrowAirPuffUp(selected.location.ToVector3(), currentMap);
            FleckMaker.ThrowMicroSparks(selected.location.ToVector3(), currentMap);
            FleckMaker.ThrowHeatGlow(selected.location, currentMap, 2f);
            FleckMaker.ThrowLightningGlow(selected.location.ToVector3(), currentMap, 5f);
            ScenarioTool.MergeNonScenarioPart(selected, ref DialogCapsuleUI.lPartsInterstellar, true);
        }
        this.ScanMap();
    }

    private void AOnLoadSlot(int index)
    {
        ScenarioTool.LoadCapsuleSetup(SLOTPATH(index));
        UpdateLists();
    }

    private void AOnSaveSlot(int index)
    {
        ScenarioTool.SaveCapsuleSetup(SLOTPATH(index));
        UpdateSlots();
    }

    private void AMoveToInterstellar(ScenPart part)
    {
        DialogCapsuleUI.lPartsMap.Remove(part);
        Selected selectedScenarioPart = part.GetSelectedScenarioPart<ScenPart>();
        Thing thing = ThingTool.FindThingOnMap(selectedScenarioPart, Find.CurrentMap, selectedScenarioPart.stackVal, true);
        FleckMaker.ThrowAirPuffUp(selectedScenarioPart.location.ToVector3(), Find.CurrentMap);
        FleckMaker.ThrowMicroSparks(selectedScenarioPart.location.ToVector3(), Find.CurrentMap);
        FleckMaker.ThrowHeatGlow(selectedScenarioPart.location, Find.CurrentMap, 2f);
        FleckMaker.ThrowLightningGlow(selectedScenarioPart.location.ToVector3(), Find.CurrentMap, 5f);
        bool flag = thing != null;
        if (flag)
        {
            DialogCapsuleUI.lPartsInterstellar.Add(part);
        }
        else
        {
            MessageTool.Show("try to rescan. could not find " + selectedScenarioPart.thingDef.defName + " x" + selectedScenarioPart.stackVal.ToString(), MessageTypeDefOf.RejectInput);
        }
    }

    private void ADropWithPod(ScenPart part)
    {
        lPartsInterstellar.Remove(part);
        PlacingTool.DropSelectedWithPod(part.GetSelectedScenarioPart(), new IntVec3());
        base.Close();
        CEditor.API.CloseEditor();
    }

    private void AMoveToMap(ScenPart part)
    {
        lParts.Remove(part);
        ScenarioTool.AddScenarioPartMerged(part.GetSelectedScenarioPart(), false);
        UpdateLists();
    }

    private void AMoveToContainer(ScenPart part)
    {
        lParts.Remove(part);
        ScenarioTool.AddScenarioPartMerged(part.GetSelectedScenarioPart(), true);
        UpdateLists();
    }

    private void AStart()
    {
        var windowOf = WindowTool.GetWindowOf<Page_ConfigureStartingPawns>();
        if (windowOf != null)
            ((Page)windowOf).nextAct();
        base.Close(false);
        CEditor.API.CloseEditor();
    }

    private void AOnAstronaut()
    {
        astronautPic = astronautPic == 2 ? 1 : 2;
    }

    internal void ExternalAddThing(Selected s)
    {
        bool flag = this.addToOwn;
        if (flag)
        {
            ScenarioTool.MergeNonScenarioPart(s, ref DialogCapsuleUI.lPartsInterstellar, true);
            this.addToOwn = false;
        }
        else
        {
            ScenarioTool.AddScenarioPartMerged(s, this.addToTaken);
            DialogCapsuleUI.UpdateLists();
        }
    }

    private void AAddOwnParts()
    {
        addToOwn = true;
        WindowTool.Open(new DialogObjects(DialogType.Object, this));
    }

    private void AAddParts()
    {
        addToTaken = true;
        WindowTool.Open(new DialogObjects(DialogType.Object, this));
    }

    private void AAddPartsMap()
    {
        addToTaken = false;
        WindowTool.Open(new DialogObjects(DialogType.Object, this));
    }

    private void AAddPartsAnimal()
    {
        addToTaken = false;
        addToOwn = false;
        WindowTool.Open(new DialogObjects(DialogType.Object, this, addAnimals: true));
    }

    private void AAllowRemoveParts()
    {
        bShowRemove = !bShowRemove;
    }

    private void AShowShift()
    {
        bShowShift = !bShowShift;
    }
}
