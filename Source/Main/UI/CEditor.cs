using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace CharacterEditor
{
    internal sealed class CEditor
    {
        private CEditor()
        {
            Log.Message(Reflect.APP_NAME_AND_VERISON + " initializing...");
            CEditor.zufallswert = new System.Random(50);
            CEditor.IsFacialStuffActive = (ModLister.HasActiveModWithName("Facial Stuff 1.0") || ModLister.HasActiveModWithName("Facial Stuff 1.1") || ModLister.HasActiveModWithName("Facial Stuff 1.2"));
            CEditor.IsFacesOfTheRimActive = Extension.HasMID("Capi.FacesOfTheRim");
            CEditor.IsPsychologyActive = (Extension.HasMID("Community.Psychology.UnofficialUpdate") || ModLister.HasActiveModWithName("Psychology (unofficial 1.1/1.2) ") || ModLister.HasActiveModWithName("Psychology"));
            CEditor.IsPersonalitiesActive = Extension.HasMID("hahkethomemah.simplepersonalities");
            CEditor.IsRJWActive = Extension.HasMID("rim.job.world");
            CEditor.IsDualWieldActive = (Extension.HasMID("Roolo.DualWield") || ModLister.HasActiveModWithName("Dual Wield"));
            CEditor.IsFacialAnimationActive = (Extension.HasMID("Nals.FacialAnimation") || ModLister.HasActiveModWithName("[NL] Facial Animation - WIP"));
            CEditor.IsGradientHairActive = (Extension.HasMID("automatic.gradienthair") || ModLister.HasActiveModWithName("Gradient Hair"));
            CEditor.IsAlienRaceActive = (Extension.HasMID("erdelf.HumanoidAlienRaces") || ModLister.HasActiveModWithName("Humanoid Alien Races 2.0") || ModLister.HasActiveModWithName("Humanoid Alien Races") || ModLister.HasActiveModWithName("Humanoid Alien Races ~ Dev"));
            CEditor.IsAVPActive = (Extension.HasMID("Ogliss.AlienVsPredator") || ModLister.HasActiveModWithName("Alien Vs Predator"));
            CEditor.IsCombatExtendedActive = (Extension.HasMID("CETeam.CombatExtended") || ModLister.HasActiveModWithName("Combat Extended"));
            CEditor.IsTerraformRimworldActive = (Extension.HasMID("void.terraformrimworld") || ModLister.HasActiveModWithName("TerraformRimworld"));
            CEditor.IsAgeMattersActive = Extension.HasMID("Troopersmith1.AgeMatters");
            CEditor.IsExtendedUI = false;
            CEditor.bOnMap = true;
            CEditor.IsRandom = false;
            CEditor.DontAsk = false;
            CEditor.PKD = null;
            CEditor.RACE = null;
            CEditor.ListName = "";
            this.data = new CEditor.ModData();
            Log.Message("character editor instance created");
        }


        internal static CEditor API { get; set; }


        internal static WindowLayer Layer
        {
            get { return WindowLayer.Dialog; }
        }


        internal static bool IsFacialStuffActive { get; set; }


        internal static bool IsFacesOfTheRimActive { get; set; }


        internal static bool IsPsychologyActive { get; set; }


        internal static bool IsPersonalitiesActive { get; set; }


        internal static bool IsRJWActive { get; set; }


        internal static bool IsDualWieldActive { get; set; }


        internal static bool IsFacialAnimationActive { get; set; }


        internal static bool IsGradientHairActive { get; set; }


        internal static bool IsAlienRaceActive { get; set; }


        internal static bool IsAVPActive { get; set; }


        internal static bool IsCombatExtendedActive { get; set; }


        internal static bool IsTerraformRimworldActive { get; set; }


        internal static bool IsAgeMattersActive { get; set; }


        internal static bool IsExtendedUI { get; set; }


        internal static bool IsBodysizeActive { get; set; }


        internal static bool InStartingScreen => Current.ProgramState != ProgramState.Playing || CEditor.bGamePlus;


        internal static bool OnMap
        {
            get { return !CEditor.InStartingScreen && CEditor.bOnMap; }
            set { CEditor.bOnMap = value; }
        }


        internal static bool IsRandom { get; set; }


        internal static bool IsRaceSpecificHead { get; set; }


        internal static bool DontAsk { get; set; }


        internal static PawnKindDef PKD { get; set; }


        internal static ThingDef RACE { get; set; }


        internal static Func<ThingDef, string> RACENAME => (ThingDef s) => (s == null) ? Label.ALL : ((s.label == null) ? "" : s.LabelCap.ToString());


        internal static Func<ThingDef, string> RACEDESC => (ThingDef s) => (s != null && s.description != null) ? s.description : "";


        internal static Func<PawnKindDef, string> PKDNAME => (PawnKindDef s) => (s == null) ? Label.ALL : ((s.label == null) ? "" : s.LabelCap.ToString());


        internal static Func<PawnKindDef, string> PKDTOOLTIP => (PawnKindDef s) => (s != null && s.description != null) ? s.description : "";


        internal static string ListName { get; set; }


        internal int NumSlots => this.data.Get<CEditor.ModOptions>(EType.Settings).Get(OptionI.NUMPAWNSLOTS);


        internal int NumCapsuleSlots => this.data.Get<CEditor.ModOptions>(EType.Settings).Get(OptionI.NUMCAPSULESETS);


        internal int MaxSliderVal => this.data.Get<CEditor.ModOptions>(EType.Settings).Get(OptionI.STACKLIMIT);


        internal Pawn Pawn
        {
            get { return this.data.p; }
            set
            {
                this.data.p = value;
                bool flag = !CEditor.InStartingScreen;
                if (flag)
                {
                    bool flag2 = value != null && value.Spawned;
                    if (flag2)
                    {
                        Selector selector = Find.Selector;
                        if (selector != null)
                        {
                            selector.ClearSelection();
                        }

                        Selector selector2 = Find.Selector;
                        if (selector2 != null)
                        {
                            selector2.Select(value, true, true);
                        }
                    }
                    else
                    {
                        Selector selector3 = Find.Selector;
                        if (selector3 != null)
                        {
                            selector3.ClearSelection();
                        }
                    }
                }

                this.data.UpdateGraphics();
            }
        }


        internal void UpdateGraphics()
        {
            this.data.UpdateGraphics();
        }


        internal int EditorPosX => (int)this.data.Get<CEditor.EditorUI>(EType.EditorUI).windowRect.position.x;


        internal int EditorPosY => (int)this.data.Get<CEditor.EditorUI>(EType.EditorUI).windowRect.position.y;


        internal void EditorMoveRight()
        {
            this.data.Get<CEditor.EditorUI>(EType.EditorUI).windowRect.position = new Vector2((float)(UI.screenWidth / 2 + 200), (float)(UI.screenHeight / 2) - this.data.Get<CEditor.EditorUI>(EType.EditorUI).InitialSize.y / 2f);
        }


        internal void OnSettingsChanged(bool updateRender = false, bool updateKeyCode = false)
        {
            this.data.OnSettingsChanged(updateRender, updateKeyCode);
        }


        internal void ConfigEditor()
        {
            this.data.Get<CEditor.ModOptions>(EType.Settings).Configurate();
        }


        internal void CloseEditor()
        {
            this.data.Get<CEditor.EditorUI>(EType.EditorUI).Close(true);
        }


        internal void StartEditor(Pawn pawn = null)
        {
            this.data.StartEditor(pawn);
        }


        internal void Toggle(OptionB b)
        {
            this.data.Get<CEditor.ModOptions>(EType.Settings).Toggle(b);
            this.data.UpdateUIParameter();
        }


        internal bool Has(EType t)
        {
            return this.data.Has(t);
        }


        internal T Get<T>(EType t)
        {
            return this.data.Get<T>(t);
        }


        internal Dictionary<string, Faction> DicFactions
        {
            get { return this.Get<Dictionary<string, Faction>>(EType.Factions); }
        }


        internal List<T> ListOf<T>(EType t)
        {
            return this.data.ListOf<T>(t);
        }


        internal bool GetO(OptionB b)
        {
            return this.data.Get<CEditor.ModOptions>(EType.Settings).Get(b);
        }


        internal int GetI(OptionI i)
        {
            return this.data.Get<CEditor.ModOptions>(EType.Settings).Get(i);
        }


        internal string GetSlot(int i)
        {
            return this.data.Get<CEditor.ModOptions>(EType.Settings).GetSlot(i);
        }


        internal void SetSlot(int i, string val, bool andSave)
        {
            this.data.Get<CEditor.ModOptions>(EType.Settings).SetSlot(i, val, andSave);
        }


        internal string GetCustom(OptionS s)
        {
            return (s == OptionS.HOTKEYEDITOR || s == OptionS.HOTKEYTELEPORT) ? null : this.data.Get<CEditor.ModOptions>(EType.Settings).Get(s);
        }


        internal void SetCustom(OptionS option, string val, string defName)
        {
            this.data.Get<CEditor.ModOptions>(EType.Settings).SetCustom(option, val, defName);
        }


        internal List<StatDef> ListOfStatDef(StatCategoryDef s, bool isWeapon, bool isEquip)
        {
            bool flag = s == null;
            List<StatDef> result;
            if (flag)
            {
                if (isEquip)
                {
                    result = this.data.ListOf<StatDef>(EType.StatDefOnEquip);
                }
                else if (isWeapon)
                {
                    result = this.data.ListOf<StatDef>(EType.StatDefWeapon);
                }
                else
                {
                    result = this.data.ListOf<StatDef>(EType.StatDefApparel);
                }
            }
            else
            {
                List<StatDef> list = new List<StatDef>();
                foreach (StatDef statDef in DefDatabase<StatDef>.AllDefs)
                {
                    bool flag2 = statDef.category == s;
                    if (flag2)
                    {
                        list.Add(statDef);
                    }
                }

                result = list;
            }

            return result;
        }


        internal static void Initialize(Mod mod)
        {
            CEditor.pack = mod.Content;
            Harmony harmony = new Harmony("rimworld.mod.charactereditor");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            harmony.Patch(AccessTools.Method(typeof(Page_ConfigureStartingPawns), "DoWindowContents", null, null), null, new HarmonyMethod(typeof(CEditor), "AddCharacterEditorButton", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Page_ConfigureStartingPawns), "PreOpen", null, null), null, new HarmonyMethod(typeof(CEditor), "GamePlusPreOpen", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Dialog_AdvancedGameConfig), "DoWindowContents", null, null), null, new HarmonyMethod(typeof(CEditor), "AddMapSizeSlider", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(MainMenuDrawer), "Init", null, null), null, new HarmonyMethod(typeof(CEditor), "OnMainMenuInit", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(MainMenuDrawer), "MainMenuOnGUI", null, null), null, new HarmonyMethod(typeof(CEditor), "OnMainMenuOnGUI", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(UIRoot_Entry), "DoMainMenu", null, null), null, new HarmonyMethod(typeof(CEditor), "OnDoingMainMenu", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Map), "FinalizeInit", null, null), null, new HarmonyMethod(typeof(CEditor), "OnMapLoaded", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Game), "LoadGame", null, null), null, new HarmonyMethod(typeof(CEditor), "OnSavegameLoaded", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Gene), "PostAdd", null, null), null, new HarmonyMethod(typeof(CEditor), "OnPostAddGene", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Gene), "PostRemove", null, null), null, new HarmonyMethod(typeof(CEditor), "OnPostRemoveGene", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Pawn_AgeTracker), "RecalculateLifeStageIndex", null, null), null, new HarmonyMethod(typeof(CEditor), "OnRecalcIndex", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Pawn_AgeTracker), "CalculateInitialGrowth", null, null), null, new HarmonyMethod(typeof(CEditor), "OnPreRecalcIndex", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(ShaderUtility), "GetSkinShaderAbstract", null, null), new HarmonyMethod(typeof(CEditor), "GetBetterShader", null), null, null, null);
        }


        internal static void StartNewGame2()
        {
            CEditor.bStartNewGame = false;
            CEditor.bStartNewGame2 = false;
            Current.Game.Scenario = CEditor.oldScenario;
            Current.Game.storyteller = CEditor.oldStoryteller;
            Current.ProgramState = ProgramState.MapInitializing;
            bool flag = Current.Game.InitData == null;
            if (flag)
            {
                Current.Game.InitData = new GameInitData();
            }

            Current.Game.InitData.startedFromEntry = true;
            Find.GameInitData.startingTile = -1;
            Find.WorldInterface.SelectedTile = -1;
            Page_SelectStartingSite page_SelectStartingSite = new Page_SelectStartingSite();
            Page_ConfigureStartingPawns page_ConfigureStartingPawns = new Page_ConfigureStartingPawns();
            PageStart next = new PageStart();
            page_SelectStartingSite.next = page_ConfigureStartingPawns;
            page_ConfigureStartingPawns.next = next;
            page_ConfigureStartingPawns.prev = page_SelectStartingSite;
            Find.WindowStack.Add(page_SelectStartingSite);
        }


        internal static void StartNewGame()
        {
            CEditor.bStartNewGame = false;
            CEditor.bStartNewGame2 = false;
            Find.WindowStack.Add(new Page_SelectScenario());
        }


        private static void OnDoingMainMenu()
        {
            bool flag = Label.currentLanguage != LanguageDatabase.activeLanguage.FriendlyNameEnglish.ToLower();
            if (flag)
            {
                CEditor.API.data.ReInitSettings();
            }
        }


        private static void CheckAddButton()
        {
            try
            {
                List<MainButtonDef> memberValue = Find.MainButtonsRoot.GetMemberValue<List<MainButtonDef>>("allButtonsInOrder", null);
                bool flag = memberValue != null;
                if (flag)
                {
                    MainButtonDef item = CEditor.API.data.Get<MainButtonDef>(EType.MainButton);
                    MainButtonDef item2 = CEditor.API.data.Get<MainButtonDef>(EType.TeleButton);
                    bool flag2 = !memberValue.Contains(item);
                    if (flag2)
                    {
                        memberValue.Insert(memberValue.Count - 1, item);
                    }

                    bool flag3 = !memberValue.Contains(item2);
                    if (flag3)
                    {
                        memberValue.Insert(memberValue.Count - 1, item2);
                    }
                }
            }
            catch
            {
                Log.Error("could not add character editor button");
            }
        }


        private static void OnMapLoaded()
        {
            CEditor.CheckAddButton();
            CEditor.API.OnSettingsChanged(false, true);
        }


        private static void OnSavegameLoaded()
        {
            bool isBodysizeActive = CEditor.IsBodysizeActive;
            if (isBodysizeActive)
            {
                RaceTool.CheckAllPawnsOnStartupAndReapplyBodySize();
            }
        }


        public static bool GetBetterShader(ref Shader __result, bool skinColorOverriden, bool dead)
        {
            if (CEditor.API.GetO(OptionB.USEFIXEDSHADER))
            {
                __result = ShaderDatabase.CutoutSkin;
                return false;
            }

            __result = (skinColorOverriden ? ShaderDatabase.CutoutSkinColorOverride : ShaderDatabase.CutoutSkin);
            return true;
        }


        private static void OnPreRecalcIndex(Pawn_AgeTracker __instance)
        {
            bool isBodysizeActive = CEditor.IsBodysizeActive;
            if (isBodysizeActive)
            {
                Pawn memberValue = __instance.GetMemberValue<Pawn>("pawn", null);
                memberValue.RememberBackstory();
            }
        }


        private static void OnRecalcIndex(Pawn_AgeTracker __instance)
        {
            bool isBodysizeActive = CEditor.IsBodysizeActive;
            if (isBodysizeActive)
            {
                Pawn memberValue = __instance.GetMemberValue<Pawn>("pawn", null);
                memberValue.TryApplyOrKeepBodySize(null);
            }
        }


        private static void OnPostAddGene(Gene __instance)
        {
            bool isBodysizeActive = CEditor.IsBodysizeActive;
            if (isBodysizeActive)
            {
                bool flag = __instance.IsBodySizeGene();
                if (flag)
                {
                    RaceTool.RemoveOldBodySizeRedundants(__instance);
                    __instance.pawn.TryApplyOrKeepBodySize(__instance);
                }
            }
        }


        private static void OnPostRemoveGene(Gene __instance)
        {
            bool isBodysizeActive = CEditor.IsBodysizeActive;
            if (isBodysizeActive)
            {
                bool flag = __instance.IsBodySizeGene();
                if (flag)
                {
                    __instance.pawn.TryApplyOrKeepBodySize(null);
                }
            }
        }


        private static void OnMainMenuOnGUI()
        {
            bool flag = CEditor.bCreateDefaults;
            if (flag)
            {
                CEditor.bCreateDefaults = false;
                CEditor.API.Get<CEditor.ModOptions>(EType.Settings).CreateDefaultLists();
            }

            bool flag2 = CEditor.bUpdateCustoms;
            if (flag2)
            {
                CEditor.bUpdateCustoms = false;
                CEditor.API.Get<CEditor.ModOptions>(EType.Settings).UpdatingCustoms();
            }

            bool flag3 = CEditor.bStartNewGame;
            if (flag3)
            {
                CEditor.StartNewGame();
            }
        }


        private static void OnMainMenuInit()
        {
            bool flag = CEditor.API == null;
            if (flag)
            {
                CEditor.API = new CEditor();
            }

            bool flag2 = CEditor.API == null;
            if (flag2)
            {
                Log.Error("failed to create instance for character editor!");
            }
            else
            {
                ApparelTool.AllowApparelToBeColorable();
                CEditor.API.OnSettingsChanged(false, true);
                Log.Message(Reflect.APP_NAME_AND_VERISON + " ...done");
            }
        }


        private static void GamePlusPreOpen(Page_ConfigureStartingPawns __instance)
        {
            bool flag = CEditor.bGamePlus;
            if (flag)
            {
                CEditor.bGamePlus = false;
                CEditor.API.StartEditor(null);
                bool flag2 = Find.GameInitData.playerFaction == null;
                if (flag2)
                {
                    Find.GameInitData.playerFaction = Faction.OfPlayer;
                }

                ScenarioTool.LoadCapsuleSetup(DialogCapsuleUI.SLOTPATH(-1));
                PortraitsCache.Clear();
                PortraitsCache.PortraitsCacheUpdate();
                bool flag3 = Find.CurrentMap != null;
                if (flag3)
                {
                    Find.CurrentMap.MapUpdate();
                }

                __instance.PreOpen();
                CEditor.API.CloseEditor();
            }
        }


        private static void AddCharacterEditorButton(Page_ConfigureStartingPawns __instance, Rect rect)
        {
            CEditor.pstartInstance = __instance;
            CEditor.pstartInstance.absorbInputAroundWindow = false;
            bool o = CEditor.API.GetO(OptionB.SHOWICONINSTARTING);
            if (o)
            {
                Vector2 vector = Text.CalcSize(CEditor.pstartInstance.PageTitle);
                Text.Font = GameFont.Medium;
                Widgets.DrawBoxSolid(new Rect(0f, 0f, vector.x + 140f, 40f), Widgets.WindowBGFillColor);
                SZWidgets.ButtonImage(new Rect(0f, 0f, 40f, 40f), "bastronaut1", delegate() { CEditor.API.StartEditor(null); }, Label.O_CHARACTEREDITORUI);
                Widgets.Label(new Rect(45f, 0f, vector.x + 100f, 40f), CEditor.pstartInstance.PageTitle);
                Text.Font = GameFont.Small;
            }

            bool o2 = CEditor.API.GetO(OptionB.SHOWBUTTONINSTARTING);
            if (o2)
            {
                SZWidgets.ButtonText(new Rect(rect.x + rect.width - 300f, rect.y + 645f, 150f, 38f), Label.O_CHARACTEREDITORUI, delegate() { CEditor.API.StartEditor(null); }, "");
            }
        }


        private static void AddMapSizeSlider(Dialog_AdvancedGameConfig __instance, Rect inRect)
        {
            bool o = CEditor.API.GetO(OptionB.SHOWMAPVARS);
            if (o)
            {
                Listing_X listing_X = new Listing_X();
                listing_X.Begin(new Rect(0f, 280f, 655f, 150f));
                try
                {
                    string text = "";
                    listing_X.AddIntSection("MapSize".Translate(), "", ref text, ref Find.GameInitData.mapSize, 30, 1000, true, "", false);
                    bool flag = Find.GameInitData.startingTile >= 0;
                    if (flag)
                    {
                        bool flag2 = CEditor.selectedTile != Find.GameInitData.startingTile;
                        if (flag2)
                        {
                            CEditor.selectedTile = Find.GameInitData.startingTile;
                            CEditor.pDensity = Find.WorldGrid[Find.GameInitData.startingTile].biome.plantDensity;
                            CEditor.aDensity = Find.WorldGrid[Find.GameInitData.startingTile].biome.animalDensity;
                        }

                        listing_X.AddSection(Label.PLANTDENSITY, "", ref text, ref CEditor.pDensity, 0f, 100f, true, "");
                        listing_X.AddSection(Label.ANIMALDENSITY, "", ref text, ref CEditor.aDensity, 0f, 100f, true, "");
                        bool flag3 = CEditor.selectedTile == Find.GameInitData.startingTile && CEditor.selectedTile >= 0;
                        if (flag3)
                        {
                            Find.WorldGrid[CEditor.selectedTile].biome.plantDensity = CEditor.pDensity;
                            Find.WorldGrid[CEditor.selectedTile].biome.animalDensity = CEditor.aDensity;
                        }
                    }
                }
                catch
                {
                }

                listing_X.End();
            }
        }


        static CEditor()
        {
        }


        private static ModContentPack pack;


        private CEditor.ModData data;


        internal static System.Random zufallswert;


        private static bool bOnMap;


        private static float pDensity = 1f;


        private static float aDensity = 1f;


        private static int selectedTile = -1;


        private static bool bCreateDefaults = true;


        private static bool bUpdateCustoms = true;


        internal static bool bStartNewGame = false;


        internal static bool bStartNewGame2 = false;


        internal static bool bGamePlus = false;


        internal static Scenario oldScenario;


        internal static Storyteller oldStoryteller;


        private static Page_ConfigureStartingPawns pstartInstance = null;


        private sealed class ModOptions : ModSettings
        {
            private string PATH_MODDIR { get; set; }


            private string PATH_SETTINGS => this.PATH_MODDIR + Path.DirectorySeparatorChar.ToString() + "options.txt";


            private string PATH_PAWNS => this.PATH_MODDIR + Path.DirectorySeparatorChar.ToString() + "pawnslots.txt";


            internal ModOptions()
            {
                this.ReInit();
            }


            internal void ReInit()
            {
                this.PATH_MODDIR = GenFilePaths.ConfigFolderPath.Replace("Config", "CharacterEditor");
                FileIO.CheckOrCreateDir(this.PATH_MODDIR);
                this.dicSlots = new Dictionary<int, string>();
                this.dicString = new Dictionary<OptionS, CEditor.ModOptions.SData>();
                this.dicInt = new Dictionary<OptionI, CEditor.ModOptions.IData>();
                this.dicBool = new Dictionary<OptionB, CEditor.ModOptions.BData>();
                Label.UpdateLabels();
                this.Load(this.PATH_SETTINGS);
                this.LoadSlots(this.PATH_PAWNS);
                this.UpdateNumSlots();
            }


            private void RescueOldPawns()
            {
                string text = Path.GetDirectoryName(GenFilePaths.ConfigFolderPath.SubstringBackwardTo(Path.DirectorySeparatorChar.ToString(), true) + Path.DirectorySeparatorChar.ToString() + "HugsLib");
                text = text + Path.DirectorySeparatorChar.ToString() + "ModSettings.xml";
                bool flag = !FileIO.Exists(text);
                if (!flag)
                {
                    try
                    {
                        string text2 = FileIO.ReadFile(text).AsString(Encoding.UTF8);
                        text2 = text2.SubstringFrom("<P_CE_CustomPawn", true).SubstringTo("</CharacterEditor>", true);
                        bool flag2 = !text2.NullOrEmpty();
                        if (flag2)
                        {
                            string[] array = text2.SplitNo("<P_CE_CustomPawn");
                            bool flag3 = array.Length != 0;
                            if (flag3)
                            {
                                foreach (string text3 in array)
                                {
                                    string input = text3.SubstringFrom("CustomPawn", true).SubstringTo(">", true);
                                    string val = text3.SubstringFrom(">", true).SubstringTo("<", true);
                                    int id = input.AsInt32();
                                    this.SetSlot(id, val, false);
                                }

                                Log.Message("old pawn slots imported");
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }


            private void Load(string filepath)
            {
                bool flag = FileIO.Exists(filepath);
                string text;
                if (flag)
                {
                    text = FileIO.ReadFile(filepath).AsString(Encoding.UTF8);
                }
                else
                {
                    text = "\n\n\n\n\n\n\n\n\n";
                }

                bool flag2 = text.NullOrEmpty();
                if (flag2)
                {
                    text = "\n\n\n\n\n\n\n\n\n";
                }

                string[] array = text.SplitNo("\n");
                for (int i = 0; i < array.Length; i++)
                {
                    bool flag3 = i == 0;
                    if (flag3)
                    {
                        this.LoadBoolSettings(array[i]);
                    }

                    bool flag4 = i == 1;
                    if (flag4)
                    {
                        this.LoadIntSettings(array[i]);
                    }

                    bool flag5 = i == 2;
                    if (flag5)
                    {
                        this.LoadStringSettings(array[i]);
                    }

                    bool flag6 = i == 3;
                    if (flag6)
                    {
                    }

                    bool flag7 = i == 4;
                    if (flag7)
                    {
                    }

                    bool flag8 = i == 5;
                    if (flag8)
                    {
                    }

                    bool flag9 = i == 6;
                    if (flag9)
                    {
                        this.dicString.Add(OptionS.CUSTOMGENE, new CEditor.ModOptions.SData(Label.O_SAVEDGENECHANGES, Label.O_DESC_SAVEDGENECHANGES, array[i], ""));
                    }

                    bool flag10 = i == 7;
                    if (flag10)
                    {
                        this.dicString.Add(OptionS.CUSTOMOBJECT, new CEditor.ModOptions.SData(Label.O_SAVEDITEMCHANGES, Label.O_DESC_SAVEDITEMCHANGES, array[i], ""));
                    }

                    bool flag11 = i == 8;
                    if (flag11)
                    {
                        this.dicString.Add(OptionS.CUSTOMLIFESTAGE, new CEditor.ModOptions.SData(Label.O_SAVEDLSCHANGES, Label.O_DESC_SAVEDLSCHANGES, array[i], ""));
                    }
                }

                bool flag12 = !this.dicString.ContainsKey(OptionS.CUSTOMGENE);
                if (flag12)
                {
                    this.dicString.Add(OptionS.CUSTOMGENE, new CEditor.ModOptions.SData(Label.O_SAVEDGENECHANGES, Label.O_DESC_SAVEDGENECHANGES, "", ""));
                }

                bool flag13 = !this.dicString.ContainsKey(OptionS.CUSTOMOBJECT);
                if (flag13)
                {
                    this.dicString.Add(OptionS.CUSTOMOBJECT, new CEditor.ModOptions.SData(Label.O_SAVEDITEMCHANGES, Label.O_DESC_SAVEDITEMCHANGES, "", ""));
                }

                bool flag14 = !this.dicString.ContainsKey(OptionS.CUSTOMLIFESTAGE);
                if (flag14)
                {
                    this.dicString.Add(OptionS.CUSTOMLIFESTAGE, new CEditor.ModOptions.SData(Label.O_SAVEDLSCHANGES, Label.O_DESC_SAVEDLSCHANGES, "", ""));
                }

                bool flag15 = !FileIO.Exists(filepath);
                if (flag15)
                {
                    this.SaveSettings();
                }
            }


            private void LoadSlots(string filepath)
            {
                string text = "";
                try
                {
                    bool flag = FileIO.Exists(filepath);
                    if (flag)
                    {
                        text = FileIO.ReadFile(filepath).AsString(Encoding.UTF8);
                    }
                }
                catch
                {
                }

                Log.Message("loading pawn slot content from file...");
                this.bDoRescue = text.NullOrEmpty();
                bool flag2 = text == null;
                if (flag2)
                {
                    text = "";
                }

                string[] array = text.SplitNo("\n");
                this.dicSlots = new Dictionary<int, string>();
                CEditor.ModOptions.IData value = this.dicInt.GetValue(OptionI.NUMPAWNSLOTS);
                int num = (value != null) ? value.Value : 0;
                for (int i = 0; i < num; i++)
                {
                    this.dicSlots.Add(i, (!text.NullOrEmpty() && array.Length >= i + 1) ? array[i] : "");
                }

                bool flag3 = this.bDoRescue;
                if (flag3)
                {
                    this.RescueOldPawns();
                }

                bool flag4 = !FileIO.Exists(filepath);
                if (flag4)
                {
                    this.SaveSlots();
                }
            }


            private string PawnSlotsToString()
            {
                string text = "";
                foreach (int key in this.dicSlots.Keys)
                {
                    text = text + this.dicSlots[key] + "\n";
                }

                return text;
            }


            private void LoadStringSettings(string line)
            {
                string[] array = line.SplitNo("|");
                this.dicString = new Dictionary<OptionS, CEditor.ModOptions.SData>();
                this.dicString.Add(OptionS.HOTKEYEDITOR, new CEditor.ModOptions.SData(Label.O_HOTKEYEDITOR, Label.O_DESC_HOTKEYEDITOR, (!line.NullOrEmpty() && array.Length >= 1) ? array[0] : "None", "None"));
                this.dicString.Add(OptionS.HOTKEYTELEPORT, new CEditor.ModOptions.SData(Label.O_HOTKEYTELEPORT, Label.O_DESC_HOTKEYTELEPORT, (array.Length >= 2) ? array[1] : "None", "None"));
            }


            private string StringSettingsToString()
            {
                string text = "";
                int num = 2;
                foreach (OptionS key in this.dicString.Keys)
                {
                    bool flag = num > 0;
                    if (flag)
                    {
                        text = text + this.dicString[key].Value + "|";
                    }

                    num--;
                }

                text += "\n";
                text += "\n";
                text += "\n";
                text += "\n";
                text = text + this.dicString[OptionS.CUSTOMGENE].Value + "\n";
                text = text + this.dicString[OptionS.CUSTOMOBJECT].Value + "\n";
                text = text + this.dicString[OptionS.CUSTOMLIFESTAGE].Value + "\n";
                return text;
            }


            private void LoadIntSettings(string line)
            {
                string[] array = line.SplitNo("|");
                this.dicInt = new Dictionary<OptionI, CEditor.ModOptions.IData>();
                this.dicInt.Add(OptionI.RESOLUTION, new CEditor.ModOptions.IData(Label.O_RESOLUTION, Label.O_DESC_RESOLUTION, (!line.NullOrEmpty() && array.Length >= 1) ? array[0].AsInt32() : 800, 800));
                this.dicInt.Add(OptionI.STACKLIMIT, new CEditor.ModOptions.IData(Label.O_STACKLIMIT, Label.O_DESC_STACKLIMIT, (array.Length >= 2) ? array[1].AsInt32() : 100, 100));
                this.dicInt.Add(OptionI.NUMCAPSULESETS, new CEditor.ModOptions.IData(Label.O_NUMCAPSULE, Label.O_DESC_NUMCAPSULE, (array.Length >= 3) ? array[2].AsInt32() : 10, 10));
                this.dicInt.Add(OptionI.NUMPAWNSLOTS, new CEditor.ModOptions.IData(Label.O_NUMSLOTS, Label.O_DESC_NUMSLOTS, (array.Length >= 4) ? array[3].AsInt32() : 90, 90));
                this.dicInt.Add(OptionI.VERSION, new CEditor.ModOptions.IData("Version", "", (array.Length >= 5) ? array[4].AsInt32() : Reflect.VERSION_BUILD, Reflect.VERSION_BUILD));
            }


            private string IntSettingsToString()
            {
                string str = "";
                foreach (OptionI key in this.dicInt.Keys)
                {
                    str = str + this.dicInt[key].Value.ToString() + "|";
                }

                return str + "\n";
            }


            private void LoadBoolSettings(string line)
            {
                string[] array = line.SplitNo("|");
                this.dicBool = new Dictionary<OptionB, CEditor.ModOptions.BData>();
                this.dicBool.Add(OptionB.HDRARGB, new CEditor.ModOptions.BData(Label.O_HDR, Label.O_DESC_HDR, line.NullOrEmpty() || array.Length < 1 || array[0].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.CREATERACESPECIFIC, new CEditor.ModOptions.BData(Label.O_CREATERACESPECIFIC, Label.O_DESC_CREATERACESPECIFIC, array.Length < 2 || array[1].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.CREATENUDE, new CEditor.ModOptions.BData(Label.O_CREATENUDE, Label.O_DESC_CREATENUDE, array.Length >= 3 && array[2].AsBoolWithDefault(false), false));
                this.dicBool.Add(OptionB.CREATENOWEAPON, new CEditor.ModOptions.BData(Label.O_CREATENOWEAPON, Label.O_DESC_CREATENOWEAPON, array.Length >= 4 && array[3].AsBoolWithDefault(false), false));
                this.dicBool.Add(OptionB.CREATENOINV, new CEditor.ModOptions.BData(Label.O_CREATENOINV, Label.O_DESC_CREATENOINV, array.Length >= 5 && array[4].AsBoolWithDefault(false), false));
                this.dicBool.Add(OptionB.PAUSEGAME, new CEditor.ModOptions.BData(Label.O_PAUSEGAME, Label.O_DESC_PAUSEGAME, array.Length < 6 || array[5].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.SHOWTABS, new CEditor.ModOptions.BData(Label.O_EDITTABS, Label.O_DESC_EDITTABS, array.Length < 7 || array[6].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.SHOWPAWNLIST, new CEditor.ModOptions.BData(Label.O_PAWNLIST, Label.O_DESC_PAWNLIST, array.Length < 8 || array[7].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.SHOWMINI, new CEditor.ModOptions.BData(Label.O_MINI, Label.O_DESC_MINI, array.Length >= 9 && array[8].AsBoolWithDefault(false), false));
                this.dicBool.Add(OptionB.SHOWINMENU, new CEditor.ModOptions.BData(Label.O_SHOWINMAINTABS, Label.O_DESC_SHOWINMAINTABS, array.Length < 10 || array[9].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.ZOMBOBJECTS, new CEditor.ModOptions.BData(Label.O_DISABLEOBJ, Label.O_DESC_DISABLEOBJ, array.Length < 11 || array[10].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.SHOWMAPVARS, new CEditor.ModOptions.BData(Label.O_MAPSIZE, Label.O_DESC_MAPSIZE, array.Length >= 12 && array[11].AsBoolWithDefault(false), false));
                this.dicBool.Add(OptionB.DOAPPARELCHECK, new CEditor.ModOptions.BData(Label.O_DOAPPARELCHECK, Label.O_DESC_DOAPPARELCHECK, array.Length < 13 || array[12].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.STAYINCASCET, new CEditor.ModOptions.BData(Label.O_STAYINCASCET, Label.O_DESC_STAYINCASCET, array.Length >= 14 && array[13].AsBoolWithDefault(false), false));
                this.dicBool.Add(OptionB.SHOWDEADLOGO, new CEditor.ModOptions.BData(Label.O_SHOWDEADLOGO, Label.O_DESC_SHOWDEADLOGO, array.Length < 15 || array[14].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.USESORTEDPAWNLIST, new CEditor.ModOptions.BData(Label.O_USESORTEDPAWNLIST, Label.O_DESC_SORTEDPAWNLIST, array.Length < 16 || array[15].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.MOREPAWNNAMES, new CEditor.ModOptions.BData(Label.O_MOREPAWNNAMES, Label.O_DESC_MOREPAWNNAMES, array.Length < 17 || array[16].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.USECHAOSABILITY, new CEditor.ModOptions.BData(Label.O_CHAOSABILITY, Label.O_DESC_CHAOSABILITY, array.Length < 18 || array[17].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.SHOWICONINSTARTING, new CEditor.ModOptions.BData(Label.O_SHOWICONINSTARTING, Label.O_DESC_SHOWICONINSTARTING, array.Length < 19 || array[18].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.SHOWBUTTONINSTARTING, new CEditor.ModOptions.BData(Label.O_SHOWBUTTONINSTARTING, Label.O_DESC_SHOWBUTTONINSTARTING, array.Length < 20 || array[19].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.SHOWBODYSIZEGENES, new CEditor.ModOptions.BData(Label.O_SHOWBODYSIZEGENES, Label.O_DESC_SHOWBODYSIZEGENES, array.Length < 21 || array[20].AsBoolWithDefault(true), true));
                this.dicBool.Add(OptionB.USEFIXEDSHADER, new CEditor.ModOptions.BData(Label.O_USEFIXEDSHADER, Label.O_DESC_USEFIXEDSHADER, array.Length >= 22 && array[21].AsBoolWithDefault(false), false));
                this.dicBool.Add(OptionB.ADDCOMPCOLORABLE, new CEditor.ModOptions.BData(Label.O_COMPCOLORABLE, Label.O_DESC_COMPCOLORABLE, array.Length >= 23 && array[22].AsBoolWithDefault(false), false));
            }


            private string BoolSettingsToString()
            {
                string str = "";
                foreach (OptionB key in this.dicBool.Keys)
                {
                    str = str + (this.dicBool[key].Value ? "1" : "0") + "|";
                }

                return str + "\n";
            }


            private void UpdateNumSlots()
            {
                try
                {
                    int value = this.dicInt[OptionI.NUMPAWNSLOTS].Value;
                    bool flag = this.dicSlots.Count < value;
                    if (flag)
                    {
                        for (int i = 0; i < value; i++)
                        {
                            bool flag2 = !this.dicSlots.ContainsKey(i);
                            if (flag2)
                            {
                                this.dicSlots.Add(i, "");
                            }
                        }
                    }
                    else
                    {
                        bool flag3 = this.dicSlots.Count > value;
                        if (flag3)
                        {
                            int count = this.dicSlots.Count;
                            for (int j = count; j > 0; j--)
                            {
                                bool flag4 = j > value;
                                if (flag4)
                                {
                                    this.dicSlots.Remove(j);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.StackTrace);
                }
            }


            private void SaveSettings()
            {
                FileIO.CheckOrCreateDir(this.PATH_MODDIR);
                string text = this.BoolSettingsToString() + this.IntSettingsToString() + this.StringSettingsToString();
                FileIO.WriteFile(this.PATH_SETTINGS, text.AsBytes(Encoding.UTF8));
                this.UpdateNumSlots();
            }


            private void SaveSlots()
            {
                FileIO.CheckOrCreateDir(this.PATH_MODDIR);
                string text = this.PawnSlotsToString();
                FileIO.WriteFile(this.PATH_PAWNS, text.AsBytes(Encoding.UTF8));
            }


            internal void CreateDefaultLists()
            {
                try
                {
                    Log.Message("CE is trying to create default parameter lists ...");
                    CEditor.API.Get<Dictionary<string, PresetGene>>(EType.GenePreset);
                    CEditor.API.Get<Dictionary<string, PresetObject>>(EType.ObjectPreset);
                    CEditor.API.Get<Dictionary<string, PresetObject>>(EType.TurretPreset);
                    Log.Message("...lists created");
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message + "\n" + ex.StackTrace);
                }
            }


            internal void UpdatingCustoms()
            {
                try
                {
                    Log.Message("CE is trying to apply modified parameters...");
                    CEditor.ModOptions.IData value = this.dicInt.GetValue(OptionI.VERSION);
                    bool flag = value == null || value.Value < 1206;
                    bool flag2 = flag;
                    if (flag2)
                    {
                        Log.Message("version < 1206 - clearing customs to defaults...");
                        this.dicString[OptionS.CUSTOMGENE].Value = "";
                        this.dicInt[OptionI.VERSION].Value = Reflect.VERSION_BUILD;
                        this.SaveSettings();
                    }
                    else
                    {
                        bool flag3 = value.Value != Reflect.VERSION_BUILD;
                        if (flag3)
                        {
                            this.dicInt[OptionI.VERSION].Value = Reflect.VERSION_BUILD;
                            this.SaveSettings();
                        }
                    }

                    PresetGene.LoadAllModifications(this.dicString[OptionS.CUSTOMGENE].Value);
                    PresetObject.LoadAllModifications(this.dicString[OptionS.CUSTOMOBJECT].Value);
                    PresetLifeStage.LoadAllModifications(this.dicString[OptionS.CUSTOMLIFESTAGE].Value);
                    Log.Message("...done");
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message + "\n" + ex.StackTrace);
                }
            }


            internal void Configurate()
            {
                WindowTool.Open(new CEditor.ModOptions.DialogConfigurate());
            }


            public override void ExposeData()
            {
            }


            internal bool Get(OptionB option)
            {
                return this.dicBool[option].Value;
            }


            internal int Get(OptionI option)
            {
                return this.dicInt[option].Value;
            }


            internal string Get(OptionS option)
            {
                return this.dicString[option].Value;
            }


            internal string GetSlot(int id)
            {
                return this.dicSlots.GetValue(id);
            }


            internal void SetSlot(int id, string val, bool andSave)
            {
                bool flag = this.dicSlots.ContainsKey(id);
                if (flag)
                {
                    this.dicSlots[id] = val;
                }

                if (andSave)
                {
                    this.SaveSlots();
                }
            }


            internal void SetCustom(OptionS option, string val, string defName)
            {
                bool flag = option == OptionS.HOTKEYEDITOR || option == OptionS.HOTKEYTELEPORT;
                if (!flag)
                {
                    bool flag2 = !defName.NullOrEmpty();
                    if (flag2)
                    {
                        string text = this.Get(option);
                        bool flag3 = text.Contains(defName + ",");
                        if (flag3)
                        {
                            string text2 = text.SubstringFrom(defName + ",", false);
                            text2 = text2.SubstringTo(";", false);
                            this.dicString[option].Value = text.Replace(text2, "");
                        }

                        this.dicString[option].Value = this.dicString[option].Value + val;
                    }
                    else
                    {
                        this.dicString[option].Value = "";
                    }

                    this.SaveSettings();
                }
            }


            internal void Toggle(OptionB option)
            {
                bool flag = option == OptionB.SHOWPAWNLIST || option == OptionB.SHOWTABS || option == OptionB.SHOWMINI;
                if (flag)
                {
                    this.dicBool[option].Value = !this.dicBool[option].Value;
                }
            }


            private Dictionary<int, string> dicSlots;


            private Dictionary<OptionS, CEditor.ModOptions.SData> dicString;


            private Dictionary<OptionI, CEditor.ModOptions.IData> dicInt;


            private Dictionary<OptionB, CEditor.ModOptions.BData> dicBool;


            private bool bDoRescue;


            private class BData
            {
                public BData(string title, string desc, bool val, bool defVal)
                {
                    this.Title = title;
                    this.Descr = desc;
                    this.Value = val;
                    this.Default = defVal;
                }


                public string Title;


                public string Descr;


                public bool Default;


                public bool Value;
            }


            private class IData
            {
                public IData(string title, string desc, int val, int defVal)
                {
                    this.Title = title;
                    this.Descr = desc;
                    this.Value = val;
                    this.Default = defVal;
                }


                public string Title;


                public string Descr;


                public int Default;


                public int Value;
            }


            private class SData
            {
                public SData(string title, string desc, string val, string defVal)
                {
                    this.Title = title;
                    this.Descr = desc;
                    this.Value = val;
                    this.Default = defVal;
                }


                public string Title;


                public string Descr;


                public string Default;


                public string Value;
            }


            private class DialogConfigurate : Window
            {
                public override Vector2 InitialSize
                {
                    get { return new Vector2(800f, (float)WindowTool.MaxH); }
                }


                internal DialogConfigurate()
                {
                    this.doOnce = true;
                    this.framwW = (int)this.InitialSize.x - 410;
                    this.frameH = (int)this.InitialSize.y - 70;
                    this.mo = CEditor.API.Get<CEditor.ModOptions>(EType.Settings);
                    this.doCloseX = true;
                    this.absorbInputAroundWindow = false;
                    this.draggable = true;
                    this.layer = CEditor.Layer;
                }


                private Rect RectAccept
                {
                    get { return new Rect(this.InitialSize.x - 140f, (float)this.frameH, 100f, 28f); }
                }


                private Rect RectBottom
                {
                    get { return new Rect((float)this.x, (float)this.frameH, (float)this.buttonWidth, 28f); }
                }


                private Rect RectConfig
                {
                    get { return new Rect((float)(this.x + this.framwW - 42), (float)this.y, 24f, 24f); }
                }


                private Rect RectFrame
                {
                    get { return new Rect((float)this.x, (float)this.y, (float)this.framwW, (float)(this.frameH - this.y - 15)); }
                }


                private Rect RectFullWidth
                {
                    get { return new Rect((float)this.x, (float)this.y, this.InitialSize.x - 32f, 28f); }
                }


                private Rect RectHalfWidth
                {
                    get { return new Rect((float)this.x, (float)this.y, (float)this.framwW, 24f); }
                }


                private Rect RectHotkey
                {
                    get { return new Rect((float)(this.x + 130), (float)this.y, (float)(this.framwW - 155), 24f); }
                }


                private Rect RectHotkeyLabel
                {
                    get { return new Rect((float)this.x, (float)this.y, 130f, 24f); }
                }


                private Rect RectImage
                {
                    get { return new Rect((float)this.x, (float)this.y, 64f, 64f); }
                }


                private Rect RectNumeric
                {
                    get { return new Rect((float)this.x, (float)this.y, 130f, 24f); }
                }


                private Rect RectRemoveHotkey
                {
                    get { return new Rect((float)(this.x + this.framwW - 24), (float)this.y, 24f, 24f); }
                }


                private Rect RectSlider
                {
                    get { return new Rect((float)(this.x + 100), (float)this.y, (float)(this.framwW - 100), 24f); }
                }


                private Rect RectSliderLabel
                {
                    get { return new Rect((float)this.x, (float)(this.y + 2), 130f, 24f); }
                }


                private Rect RectSolid
                {
                    get { return new Rect((float)(this.x + 25), (float)this.y, (float)this.framwW, 24f); }
                }


                public override void DoWindowContents(Rect inRect)
                {
                    this.x = 0;
                    this.y = 0;
                    this.DrawTitle();
                    this.DrawHotkey();
                    this.DrawHotkeyTeleport();
                    this.y += 3;
                    this.DrawNumeric();
                    this.DrawStrings();
                    this.DrawBoolean(390);
                    this.DrawButtons();
                }


                private void DoOnce()
                {
                    bool flag = this.doOnce;
                    if (flag)
                    {
                        this.doOnce = false;
                        Vector2 mousePositionOnUI = UI.MousePositionOnUI;
                        float num = (float)CEditor.API.EditorPosY;
                        this.windowRect.position = new Vector2(mousePositionOnUI.x + 250f, num);
                    }
                }


                private void DoAndClose()
                {
                    this.mo.SaveSettings();
                    this.mo.SaveSlots();
                    this.Close(true);
                }


                private void AResetAll()
                {
                    foreach (OptionS key in this.mo.dicString.Keys)
                    {
                        this.mo.dicString[key].Value = this.mo.dicString[key].Default;
                    }

                    foreach (OptionB key2 in this.mo.dicBool.Keys)
                    {
                        this.mo.dicBool[key2].Value = this.mo.dicBool[key2].Default;
                    }

                    foreach (OptionI key3 in this.mo.dicInt.Keys)
                    {
                        this.mo.dicInt[key3].Value = this.mo.dicInt[key3].Default;
                    }
                }


                private void AConfirmDelete()
                {
                    MessageTool.ShowActionDialog(Label.ALLSLOTSWILLBECLEARED, new Action(this.ADeleteSlots), null, WindowLayer.Dialog);
                }


                private void ADeleteCustoms()
                {
                    PresetPawn.ClearAllCustoms();
                }


                private void ADeleteSlots()
                {
                    PresetPawn.ClearAllSlots();
                }


                private void AExportSlots()
                {
                    string text = "";
                    foreach (int key in this.mo.dicSlots.Keys)
                    {
                        text = text + this.mo.dicSlots[key] + "\n";
                    }

                    bool flag = FileIO.WriteFile(FileIO.PATH_PAWNEX, text.AsBytes(Encoding.UTF8));
                    if (flag)
                    {
                        MessageTool.Show("export successful to " + FileIO.PATH_PAWNEX, null);
                    }
                }


                private void AImportSlots()
                {
                    string text = FileIO.ReadFile(FileIO.PATH_PAWNEX).AsString(Encoding.UTF8);
                    bool flag = !text.NullOrEmpty();
                    if (flag)
                    {
                        string[] array = text.SplitNo("\n");
                        int count = this.mo.dicSlots.Keys.Count;
                        for (int i = 0; i < count; i++)
                        {
                            bool flag2 = array.Length > i;
                            if (flag2)
                            {
                                this.mo.dicSlots[i] = array[i];
                            }
                        }

                        MessageTool.Show("import successful from " + FileIO.PATH_PAWNEX, null);
                    }
                }


                private void AMinusInt(int index)
                {
                    this.mo.dicInt[(OptionI)index].Value--;
                }


                private void APlusInt(int index)
                {
                    this.mo.dicInt[(OptionI)index].Value++;
                }


                private void BoolChanged(OptionB b)
                {
                    bool flag = b == OptionB.SHOWINMENU || b == OptionB.ZOMBOBJECTS || b == OptionB.SHOWBODYSIZEGENES || b == OptionB.SHOWPAWNLIST || b == OptionB.SHOWTABS || b == OptionB.PAUSEGAME || b == OptionB.SHOWMINI;
                    if (flag)
                    {
                        CEditor.API.OnSettingsChanged(false, false);
                    }
                    else
                    {
                        bool flag2 = b == OptionB.HDRARGB;
                        if (flag2)
                        {
                            CEditor.API.OnSettingsChanged(true, false);
                        }
                    }
                }


                private void DrawBoolean(int xPos)
                {
                    Text.Font = GameFont.Small;
                    float height = (float)(this.mo.dicBool.Keys.Count * 27);
                    Rect outRect = new Rect(390f, 30f, 380f, (float)(this.frameH - 30));
                    Rect rect = new Rect(0f, 0f, outRect.width - 16f, height);
                    Widgets.BeginScrollView(outRect, ref this.scrollPos1, rect, true);
                    Rect rect2 = rect.ContractedBy(4f);
                    rect2.y -= 4f;
                    rect2.height = height;
                    Listing_X listing_X = new Listing_X();
                    listing_X.Begin(rect2);
                    listing_X.verticalSpacing = 5f;
                    listing_X.DefSelectionLineHeight = 27f;
                    foreach (OptionB optionB in this.mo.dicBool.Keys)
                    {
                        bool value = this.mo.dicBool[optionB].Value;
                        listing_X.CheckboxLabeledWithDefault(this.mo.dicBool[optionB].Title, 2f, rect.width - 20f, ref value, this.mo.dicBool[optionB].Default, this.mo.dicBool[optionB].Descr);
                        bool flag = value != this.mo.dicBool[optionB].Value;
                        if (flag)
                        {
                            this.mo.dicBool[optionB].Value = value;
                            this.BoolChanged(optionB);
                        }
                    }

                    listing_X.End();
                    Widgets.EndScrollView();
                    this.y += (int)outRect.height + 10;
                }


                private void DrawButtons()
                {
                    Text.Font = GameFont.Small;
                    SZWidgets.ButtonText(this.RectBottom, Label.TODEFAULTS, new Action(this.AResetAll), Label.TIP_TODEFAULTS);
                    this.x += this.buttonWidth;
                    SZWidgets.ButtonText(this.RectBottom, "Delete".Translate(), new Action(this.AConfirmDelete), Label.DELETE_SLOTS);
                    this.x += this.buttonWidth;
                    SZWidgets.ButtonText(this.RectBottom, Label.EXPORT, new Action(this.AExportSlots), Label.EXPORT_SLOTS);
                    this.x += this.buttonWidth;
                    SZWidgets.ButtonText(this.RectBottom, Label.IMPORT, new Action(this.AImportSlots), Label.IMPORT_SLOTS);
                    this.x += this.buttonWidth;
                    WindowTool.SimpleSaveButton(this, new Action(this.DoAndClose));
                }


                private void DrawNumeric()
                {
                    int value = this.mo.dicInt[OptionI.RESOLUTION].Value;
                    foreach (OptionI optionI in this.mo.dicInt.Keys)
                    {
                        bool flag = optionI == OptionI.VERSION;
                        if (flag)
                        {
                            bool devMode = Prefs.DevMode;
                            if (devMode)
                            {
                                SZWidgets.Label(this.RectSliderLabel, this.mo.dicInt[optionI].Title + " " + this.mo.dicInt[optionI].Value.ToString(), null, this.mo.dicInt[optionI].Descr);
                            }
                        }
                        else
                        {
                            bool flag2 = optionI == OptionI.STACKLIMIT;
                            if (flag2)
                            {
                                continue;
                            }

                            SZWidgets.Label(this.RectSliderLabel, this.mo.dicInt[optionI].Title, null, this.mo.dicInt[optionI].Descr);
                            SZWidgets.ButtonTextVar<int>((float)this.x + this.RectNumeric.width, (float)this.y, 28f, 24f, "+", new Action<int>(this.APlusInt), (int)optionI);
                            int max = (optionI == OptionI.RESOLUTION) ? 1600 : 400;
                            this.mo.dicInt[optionI].Value = SZWidgets.NumericTextField((float)this.x + this.RectNumeric.width + 10f, (float)this.y, (float)this.framwW - this.RectNumeric.width - 60f, 24f, this.mo.dicInt[optionI].Value, 1, max);
                            SZWidgets.ButtonTextVar<int>((float)(this.x + this.framwW - 30), (float)this.y, 28f, 24f, "-", new Action<int>(this.AMinusInt), (int)optionI);
                        }

                        this.y += 28;
                    }

                    bool flag3 = value != this.mo.dicInt[OptionI.RESOLUTION].Value;
                    if (flag3)
                    {
                        CEditor.API.OnSettingsChanged(true, false);
                    }

                    this.y += 8;
                }


                private void DrawStrings()
                {
                    Text.Font = GameFont.Small;
                    float height = (float)((this.mo.dicString.Keys.Count + this.mo.dicSlots.Keys.Count - 2) * 27);
                    Rect rectFrame = this.RectFrame;
                    Rect rect = new Rect(0f, 0f, rectFrame.width - 16f, height);
                    Widgets.BeginScrollView(rectFrame, ref this.scrollPos2, rect, true);
                    Rect rect2 = rect.ContractedBy(4f);
                    rect2.y -= 4f;
                    rect2.height = height;
                    Listing_X listing_X = new Listing_X();
                    listing_X.Begin(rect2);
                    listing_X.verticalSpacing = 5f;
                    listing_X.DefSelectionLineHeight = 27f;
                    foreach (OptionS optionS in this.mo.dicString.Keys)
                    {
                        bool flag = optionS == OptionS.HOTKEYEDITOR || optionS == OptionS.HOTKEYTELEPORT;
                        if (!flag)
                        {
                            string text = this.mo.dicString[optionS].Value;
                            text = listing_X.TextEntryLabeledWithDefaultAndCopy(this.mo.dicString[optionS].Title, text, this.mo.dicString[optionS].Default);
                            bool flag2 = text != this.mo.dicString[optionS].Value;
                            if (flag2)
                            {
                                this.mo.dicString[optionS].Value = text;
                            }
                        }
                    }

                    foreach (int num in this.mo.dicSlots.Keys)
                    {
                        string text2 = this.mo.dicSlots[num];
                        text2 = listing_X.TextEntryLabeledWithDefaultAndCopy(string.Format(Label.O_PAWNSLOT, num.ToString()), text2, "");
                        bool flag3 = text2 != this.mo.dicSlots[num];
                        if (flag3)
                        {
                            this.mo.SetSlot(num, text2, false);
                        }
                    }

                    listing_X.End();
                    Widgets.EndScrollView();
                    this.y += (int)rectFrame.height + 30;
                }


                private void DrawTitle()
                {
                    Text.Font = GameFont.Medium;
                    Widgets.Label(this.RectFullWidth, Reflect.APP_ATTRIBUTE_TITLE + Label.OPTIONS);
                    Text.Font = GameFont.Tiny;
                    Widgets.Label(new Rect(this.InitialSize.x - 160f, 0f, 150f, 22f), Reflect.APP_VERISON_AND_DATE);
                    this.y += 32;
                    Text.Font = GameFont.Small;
                }


                private void RemoveHotkey(string kbdName, OptionS s)
                {
                    KeyBindingDef keyDef = DefTool.KeyBindingDef(kbdName);
                    KeyPrefs.KeyPrefsData.SetBinding(keyDef, KeyPrefs.BindingSlot.A, KeyCode.None);
                    this.mo.dicString[s].Value = KeyCode.None.ToStringReadable();
                    KeyPrefs.Save();
                }


                private void ChangeHotkey(string kbdName)
                {
                    Text.Font = GameFont.Medium;
                    KeyBindingDef keyDef = DefTool.KeyBindingDef(kbdName);
                    WindowTool.Open(new Dialog_DefineBinding(KeyPrefs.KeyPrefsData, keyDef, KeyPrefs.BindingSlot.A));
                    KeyPrefs.Save();
                }


                private void DrawHotkey(string kbdName, OptionS s, string title, string descr)
                {
                    this.y += 5;
                    KeyCode k;
                    Enum.TryParse<KeyCode>(kbdName, out k);
                    KeyBindingDef keyBindingDef = DefTool.KeyBindingDef(kbdName);
                    bool flag = keyBindingDef == null;
                    if (flag)
                    {
                        keyBindingDef = DefTool.TryCreateKeyBinding(kbdName, k.ToStringReadable(), title, descr);
                    }

                    KeyCode k2 = (keyBindingDef != null) ? KeyPrefs.KeyPrefsData.GetBoundKeyCode(keyBindingDef, KeyPrefs.BindingSlot.A) : KeyCode.None;
                    SZWidgets.Label(this.RectHotkeyLabel, title, null, descr);
                    SZWidgets.ButtonText(this.RectHotkey, (keyBindingDef == null) ? Label.NONE : k2.ToStringReadable(), delegate() { this.ChangeHotkey(kbdName); }, descr);
                    SZWidgets.ButtonImage(this.RectRemoveHotkey, "UI/Buttons/Delete", delegate() { this.RemoveHotkey(kbdName, s); }, "");
                    bool flag2 = keyBindingDef != null && k2.ToStringReadable() != this.mo.dicString[s].Value;
                    if (flag2)
                    {
                        this.mo.dicString[s].Value = k2.ToStringReadable();
                    }

                    this.y += 24;
                }


                private void DrawHotkey()
                {
                    this.DrawHotkey("HotkeyEditor", OptionS.HOTKEYEDITOR, Label.O_HOTKEYEDITOR, Label.O_DESC_HOTKEYEDITOR);
                }


                private void DrawHotkeyTeleport()
                {
                    this.DrawHotkey("HotkeyTeleport", OptionS.HOTKEYTELEPORT, Label.O_HOTKEYTELEPORT, Label.O_DESC_HOTKEYTELEPORT);
                }


                private int buttonWidth = 90;


                private bool doOnce;


                private int frameH;


                private int framwW;


                private Vector2 scrollPos1 = default(Vector2);


                private Vector2 scrollPos2 = default(Vector2);


                private int x;


                private int y;


                private CEditor.ModOptions mo;
            }
        }


        private sealed class ModData
        {
            internal ModData()
            {
                this.dicData = new Dictionary<EType, ServiceContainer>();
                this.p = null;
                this.useZombrella = false;
                bool flag = this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionB.MOREPAWNNAMES);
                if (flag)
                {
                    Label.AddNames(CEditor.pack.RootDir.Replace("\\", "/"));
                }
            }


            ~ModData()
            {
                try
                {
                    foreach (EType key in this.dicData.Keys)
                    {
                        this.dicData[key].Dispose();
                    }

                    this.dicData.Clear();
                }
                finally
                {
                }
            }


            internal List<T> ListOf<T>(EType eType)
            {
                bool flag = !this.dicData.ContainsKey(eType);
                if (flag)
                {
                    this.CreateList(eType);
                }
                else
                {
                    bool flag2 = this.dicData[eType] == null;
                    if (flag2)
                    {
                        this.dicData.Remove(eType);
                        this.CreateList(eType);
                    }
                }

                bool flag3 = this.dicData.ContainsKey(eType);
                List<T> result;
                if (flag3)
                {
                    result = (List<T>)this.dicData[eType].GetService(typeof(List<T>));
                }
                else
                {
                    result = new List<T>();
                }

                return result;
            }


            internal T Get<T>(EType eType)
            {
                if (!this.dicData.ContainsKey(eType))
                {
                    Log.Message("Did not contain eType attempting to add " + eType);
                    this.CreateType(eType);
                }
                else if (this.dicData[eType] == null)
                {
                    this.dicData.Remove(eType);
                    this.CreateType(eType);
                }

                T result;
                if (this.dicData.ContainsKey(eType))
                {
                    result = (T)((object)this.dicData[eType].GetService(typeof(T)));
                }
                else
                {
                    result = default(T);
                }

                return result;
            }


            internal bool Has(EType eType)
            {
                return this.dicData.ContainsKey(eType);
            }


            internal void OnSettingsChanged(bool updateRenderOnly, bool updateKeyCode)
            {
                try
                {
                    Log.Message("checking editor settings...");
                    if (updateRenderOnly)
                    {
                        this.Get<Capturer>(EType.Capturer).ChangeRenderTextureParamter(this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionI.RESOLUTION), this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionB.HDRARGB));
                        this.UpdateGraphics();
                    }
                    else
                    {
                        CEditor.IsBodysizeActive = CEditor.API.GetO(OptionB.SHOWBODYSIZEGENES);
                        GeneTool.EnDisableBodySizeGenes();
                        this.UpdateMainButton();
                        this.UpdateArchitectObjects();
                        this.UpdateUIParameter();
                        if (updateKeyCode)
                        {
                            this.UpdateKeyBindings();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.StackTrace);
                }
            }


            internal void UpdateGraphics()
            {
                this.Get<Capturer>(EType.Capturer).UpdatePawnGraphic(this.p);
            }


            internal void StartEditor(Pawn pawn)
            {
                CEditor.EditorUI type = this.Get<CEditor.EditorUI>(EType.EditorUI);
                if (type == null)
                {
                    Log.Message("Failed to get Type got null instead, cancelling window open!");
                    return;
                }

                type.Start(pawn);
            }


            internal void ReInitSettings()
            {
                this.Get<CEditor.ModOptions>(EType.Settings).ReInit();
            }


            private void CreateType(EType eType)
            {
                try
                {
                    if (eType == EType.EditorUI)
                    {
                        this.Get<Capturer>(EType.Capturer);
                        this.Get<HashSet<string>>(EType.GraphicPaths);
                        CEditor.EditorUI gui = new CEditor.EditorUI();
                        this.AddClassContainer<CEditor.EditorUI>(gui, eType);
                    }
                    else if (eType == EType.Capturer)
                    {
                        this.AddClassContainer<Capturer>(new Capturer(), eType);
                    }
                    else if (eType == EType.ModsThingDef)
                    {
                        this.AddClassContainer<HashSet<string>>(DefTool.ListModnamesWithNull<ThingDef>(null), eType);
                    }
                    else if (eType == EType.ModsWeapons)
                    {
                        this.AddClassContainer<HashSet<string>>(DefTool.ListModnamesWithNull<ThingDef>((ThingDef y) => y.IsWeapon), eType);
                    }
                    else if (eType == EType.ModsApparel)
                    {
                        this.AddClassContainer<HashSet<string>>(DefTool.ListModnamesWithNull<ThingDef>((ThingDef y) => y.IsApparel), eType);
                    }
                    else if (eType == EType.ModsHediffDef)
                    {
                        this.AddClassContainer<HashSet<string>>(DefTool.ListModnamesWithNull<HediffDef>(null), eType);
                    }
                    else if (eType == EType.ModsTraitDef)
                    {
                        this.AddClassContainer<HashSet<string>>(DefTool.ListModnamesWithNull<TraitDef>(null), eType);
                    }
                    else if (eType == EType.ModsHairDef)
                    {
                        this.AddClassContainer<HashSet<string>>(DefTool.ListModnamesWithNull<HairDef>(null), eType);
                    }
                    else if (eType == EType.ModsBeardDef)
                    {
                        this.AddClassContainer<HashSet<string>>(DefTool.ListModnamesWithNull<BeardDef>(null), eType);
                    }
                    else if (eType == EType.ModsAbilityDef)
                    {
                        this.AddClassContainer<HashSet<string>>(DefTool.ListModnamesWithNull<AbilityDef>(null), eType);
                    }
                    else if (eType == EType.GraphicPaths)
                    {
                        this.AddClassContainer<HashSet<string>>(ApparelTool.CreateListOfGraphicPaths(), eType);
                    }
                    else
                    {
                        if (eType == EType.ThoughtMemory || eType == EType.ThoughtMemorySocial || eType == EType.ThoughtSituational || eType == EType.ThoughtSituationalSocial || eType == EType.ThoughtUnsupported || eType == EType.ThoughtsAll)
                        {
                            Dictionary<EType, HashSet<ThoughtDef>> allThoughtLists = MindTool.GetAllThoughtLists();
                            foreach (var key in allThoughtLists)
                                if(key.Value != null)
                                    this.AddClassContainer(key.Value, key.Key);
                            
                        } else if (eType == EType.ThingCategoryDef)
                        {
                            this.AddClassContainer<HashSet<ThingCategoryDef>>((from d in DefTool.AllDefsWithLabelWithNull<ThingCategoryDef>(null)
                                orderby d != null && d.iconPath.NullOrEmpty()
                                select d).ToHashSet<ThingCategoryDef>(), eType);
                        }
                        else if (eType == EType.ThingCategory)
                        {
                            EnumTool.GetAllEnumsOfType<ThingCategory>();
                            this.AddClassContainer<HashSet<ThingCategory>>(EnumTool.GetAllEnumsOfType<ThingCategory>().ToHashSet<ThingCategory>(), eType);
                        }
                        else if (eType == EType.ApparelLayerDef)
                        {
                            this.AddClassContainer<HashSet<ApparelLayerDef>>(ApparelTool.ListOfApparelLayerDefs(true).ToHashSet<ApparelLayerDef>(), eType);
                        }
                        else if (eType == EType.BodyPartGroupDef)
                        {
                            this.AddClassContainer<HashSet<BodyPartGroupDef>>(BodyTool.ListAllBodyPartGroupDefs(true).ToHashSet<BodyPartGroupDef>(), eType);
                        }
                        else if (eType == EType.WeaponType)
                        {
                            this.AddClassContainer<HashSet<WeaponType>>(EnumTool.GetAllEnumsOfType<WeaponType>().ToHashSet<WeaponType>(), eType);
                        }
                        else if (eType == EType.QualityCategory)
                        {
                            this.AddClassContainer<HashSet<QualityCategory>>(EnumTool.GetAllEnumsOfType<QualityCategory>().ToHashSet<QualityCategory>(), eType);
                        }
                        else if (eType == EType.WeaponTraitDef)
                        {
                            this.AddClassContainer<HashSet<WeaponTraitDef>>(DefTool.ListBy<WeaponTraitDef>((WeaponTraitDef t) => !t.defName.NullOrEmpty()).ToHashSet<WeaponTraitDef>(), eType);
                        }
                        else if (eType == EType.ResearchProjectDef)
                        {
                            this.AddClassContainer<HashSet<ResearchProjectDef>>(DefTool.ListByModWithNull<ResearchProjectDef>(null, (ResearchProjectDef t) => !t.defName.NullOrEmpty()).ToHashSet<ResearchProjectDef>(), eType);
                        }
                        else if (eType == EType.Bullet)
                        {
                            this.AddClassContainer<HashSet<ThingDef>>(DefTool.AllDefsWithLabelWithNull<ThingDef>((ThingDef b) => b.IsBullet()), eType);
                        }
                        else if (eType == EType.Factions)
                        {
                            this.AddClassContainer<Dictionary<string, Faction>>(FactionTool.GetDicOfFactions(true, true, true), eType);
                        }
                        else if (eType == EType.ExplosionSound)
                        {
                            this.AddClassContainer<HashSet<SoundDef>>(ThingTool.GetExplosionSounds(), eType);
                        }
                        else if (eType == EType.GunRelatedSound)
                        {
                            this.AddClassContainer<HashSet<SoundDef>>(ThingTool.GetGunRelatedSounds(), eType);
                        }
                        else if (eType == EType.GunShotSound)
                        {
                            this.AddClassContainer<HashSet<SoundDef>>(ThingTool.GetGunShotSounds(), eType);
                        }
                        else if (eType == EType.DamageDef)
                        {
                            this.AddClassContainer<HashSet<DamageDef>>(DefTool.AllDefsWithLabelWithNull<DamageDef>(null), eType);
                        }
                        else if (eType == EType.EffecterDef)
                        {
                            this.AddClassContainer<HashSet<EffecterDef>>(DefTool.AllDefsWithNameWithNull<EffecterDef>(null), eType);
                        }
                        else if (eType == EType.TurretPreset)
                        {
                            Log.Message("creating default parameter list for turrets...");
                            this.AddClassContainer<Dictionary<string, PresetObject>>(PresetObject.CreateDefaultTurrets(), eType);
                        }
                        else if (eType == EType.ObjectPreset)
                        {
                            Log.Message("creating default parameter list for objects...");
                            this.AddClassContainer<Dictionary<string, PresetObject>>(PresetObject.CreateDefaultObjects(), eType);
                        }
                        else if (eType == EType.GenePreset)
                        {
                            Log.Message("creating default parameter list for genes...");
                            this.AddClassContainer<Dictionary<string, PresetGene>>(PresetGene.CreateDefaults(), eType);
                        }
                        else if (eType == EType.LifestagePreset)
                        {
                            Log.Message("creating default parameter list for lifestages...");
                            this.AddClassContainer<Dictionary<string, PresetLifeStage>>(PresetLifeStage.CreateDefaults(), eType);
                        }
                        else if (eType == EType.Search)
                        {
                            this.AddClassContainer<Dictionary<SearchTool.SIndex, SearchTool>>(new Dictionary<SearchTool.SIndex, SearchTool>(), eType);
                        }
                        else if (eType == EType.Settings)
                        {
                            this.AddClassContainer<CEditor.ModOptions>(new CEditor.ModOptions(), eType);
                        }
                        else if (eType == EType.UIContainers)
                        {
                            this.AddClassContainer<Dictionary<int, Building_CryptosleepCasket>>(new Dictionary<int, Building_CryptosleepCasket>
                            {
                                {
                                    0,
                                    null
                                }
                            }, eType);
                        }
                        else if (eType == EType.MainButton)
                        {
                            MainButtonDef createMainButton = DefTool.GetCreateMainButton("HotkeyEditor", Label.CHARACTER, Label.MAINBUTTON_DESCR, typeof(MainTabWindow_CharacterEditor), CEditor.pack, this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionS.HOTKEYEDITOR), this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionB.SHOWINMENU));
                            this.AddClassContainer<MainButtonDef>(createMainButton, eType);
                        }
                        else if (eType == EType.TeleButton)
                        {
                            MainButtonDef createMainButton2 = DefTool.GetCreateMainButton("HotkeyTeleport", Label.TELEPORT, "quick teleport", typeof(Teleport_Character), CEditor.pack, this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionS.HOTKEYTELEPORT), false);
                            this.AddClassContainer<MainButtonDef>(createMainButton2, eType);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Log.Error("Could not create default type. " + ex.Message + "\n" + ex.StackTrace);
                }
            }


            private void CreateList(EType eType)
            {
                try
                {
                    bool flag = eType == EType.Pawns;
                    if (flag)
                    {
                        this.AddServiceContainer<Pawn>(new List<Pawn>(), eType);
                    }
                    else
                    {
                        bool flag2 = eType == EType.PawnKindHuman;
                        if (flag2)
                        {
                            this.AddServiceContainer<PawnKindDef>(PawnKindTool.GetHumanlikes(), eType);
                        }
                        else
                        {
                            bool flag3 = eType == EType.PawnKindAnimal;
                            if (flag3)
                            {
                                this.AddServiceContainer<PawnKindDef>(PawnKindTool.GetAnimals(), eType);
                            }
                            else
                            {
                                bool flag4 = eType == EType.PawnKindOther;
                                if (flag4)
                                {
                                    this.AddServiceContainer<PawnKindDef>(PawnKindTool.GetOther(), eType);
                                }
                                else
                                {
                                    bool flag5 = eType == EType.PawnKindListed;
                                    if (flag5)
                                    {
                                        this.AddServiceContainer<PawnKindDef>(PawnKindTool.ListOfPawnKindDef(Faction.OfPlayer, Label.COLONISTS, null), eType);
                                    }
                                    else
                                    {
                                        bool flag6 = eType == EType.Bodies;
                                        if (flag6)
                                        {
                                            this.AddServiceContainer<BodyTypeDef>(DefTool.ListAll<BodyTypeDef>(), eType);
                                        }
                                        else
                                        {
                                            bool flag7 = eType == EType.MentalStates;
                                            if (flag7)
                                            {
                                                this.AddServiceContainer<MentalStateDef>(MindTool.GetAllMentalStates(), eType);
                                            }
                                            else
                                            {
                                                bool flag8 = eType == EType.Inspirations;
                                                if (flag8)
                                                {
                                                    this.AddServiceContainer<InspirationDef>(MindTool.GetAllInspirations(), eType);
                                                }
                                                else
                                                {
                                                    bool flag9 = eType == EType.ApparelTags;
                                                    if (flag9)
                                                    {
                                                        this.AddServiceContainer<string>(ThingTool.GetAllApparelTags(), eType);
                                                    }
                                                    else
                                                    {
                                                        bool flag10 = eType == EType.OutfitTags;
                                                        if (flag10)
                                                        {
                                                            this.AddServiceContainer<string>(ThingTool.GetAllOutfitTags(), eType);
                                                        }
                                                        else
                                                        {
                                                            bool flag11 = eType == EType.WeaponTags;
                                                            if (flag11)
                                                            {
                                                                this.AddServiceContainer<string>(ThingTool.GetAllWeaponTags(), eType);
                                                            }
                                                            else
                                                            {
                                                                bool flag12 = eType == EType.TradeTags;
                                                                if (flag12)
                                                                {
                                                                    this.AddServiceContainer<string>(ThingTool.GetAllTradeTags(), eType);
                                                                }
                                                                else
                                                                {
                                                                    bool flag13 = eType == EType.ExclusionTags;
                                                                    if (flag13)
                                                                    {
                                                                        this.AddServiceContainer<string>(GeneTool.GetAllExclusionTags(), eType);
                                                                    }
                                                                    else
                                                                    {
                                                                        bool flag14 = eType == EType.HairTags;
                                                                        if (flag14)
                                                                        {
                                                                            this.AddServiceContainer<string>(GeneTool.GetAllHairTags(), eType);
                                                                        }
                                                                        else
                                                                        {
                                                                            bool flag15 = eType == EType.BeardTags;
                                                                            if (flag15)
                                                                            {
                                                                                this.AddServiceContainer<string>(GeneTool.GetAllBeardTags(), eType);
                                                                            }
                                                                            else
                                                                            {
                                                                                bool flag16 = eType == EType.StuffCategory;
                                                                                if (flag16)
                                                                                {
                                                                                    this.AddServiceContainer<StuffCategoryDef>(ThingTool.GetAllStuffCategories(), EType.StuffCategory);
                                                                                }
                                                                                else
                                                                                {
                                                                                    bool flag17 = eType == EType.TechLevel;
                                                                                    if (flag17)
                                                                                    {
                                                                                        this.AddServiceContainer<TechLevel>(EnumTool.GetAllEnumsOfType<TechLevel>(), EType.TechLevel);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        bool flag18 = eType == EType.Tradeability;
                                                                                        if (flag18)
                                                                                        {
                                                                                            this.AddServiceContainer<Tradeability>(EnumTool.GetAllEnumsOfType<Tradeability>(), EType.Tradeability);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            bool flag19 = eType == EType.GasType;
                                                                                            if (flag19)
                                                                                            {
                                                                                                List<GasType> allEnumsOfType = EnumTool.GetAllEnumsOfType<GasType>();
                                                                                                List<GasType?> list = new List<GasType?>();
                                                                                                list.Add(null);
                                                                                                foreach (GasType value in allEnumsOfType)
                                                                                                {
                                                                                                    list.Add(new GasType?(value));
                                                                                                }

                                                                                                this.AddServiceContainer<GasType?>(list, EType.GasType);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                bool flag20 = eType == EType.StatCategoryApparel;
                                                                                                if (flag20)
                                                                                                {
                                                                                                    this.AddServiceContainer<StatCategoryDef>(ThingTool.GetAllStatCategoriesApparel(), eType);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    bool flag21 = eType == EType.StatCategoryWeapon;
                                                                                                    if (flag21)
                                                                                                    {
                                                                                                        this.AddServiceContainer<StatCategoryDef>(ThingTool.GetAllStatCategoriesWeapon(), eType);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        bool flag22 = eType == EType.StatCategoryOnEquip;
                                                                                                        if (flag22)
                                                                                                        {
                                                                                                            this.AddServiceContainer<StatCategoryDef>(ThingTool.GetAllStatCategoriesOnEquip(), eType);
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            bool flag23 = eType == EType.CostItems;
                                                                                                            if (flag23)
                                                                                                            {
                                                                                                                this.AddServiceContainer<ThingDef>(ThingTool.GetAllCostThingDefs(), EType.CostItems);
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                bool flag24 = eType == EType.StatDefWeapon;
                                                                                                                if (flag24)
                                                                                                                {
                                                                                                                    this.AddServiceContainer<StatDef>(ThingTool.GetAllWeaponStatDefs(), eType);
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    bool flag25 = eType == EType.StatDefApparel;
                                                                                                                    if (flag25)
                                                                                                                    {
                                                                                                                        this.AddServiceContainer<StatDef>(ThingTool.GetAllApparelStatDefs(), eType);
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        bool flag26 = eType == EType.StatDefOnEquip;
                                                                                                                        if (flag26)
                                                                                                                        {
                                                                                                                            this.AddServiceContainer<StatDef>(ThingTool.GetAllOnEquipStatDefs(), eType);
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Could not create default list. " + ex.Message + "\n" + ex.StackTrace);
                }
            }


            private void AddClassContainer<T>(T t, EType eType)
            {
                if(this.dicData.TryAdd(eType, new ServiceContainer()))
                    this.dicData[eType].AddService(typeof(T), t);
            }


            private void AddServiceContainer<T>(List<T> l, EType eType)
            {
                if(this.dicData.TryAdd(eType, new ServiceContainer()))
                    this.dicData[eType].AddService(l.GetType(), l);
            }


            internal void UpdateUIParameter()
            {
                CEditor.EditorUI editorUI = this.Get<CEditor.EditorUI>(EType.EditorUI);
                bool flag = editorUI != null;
                if (flag)
                {
                    Rect windowRect = editorUI.windowRect;
                    bool flag2 = true;
                    if (flag2)
                    {
                        editorUI.windowRect.width = editorUI.InitialSize.x;
                        editorUI.windowRect.height = editorUI.InitialSize.y;
                    }

                    editorUI.forcePause = this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionB.PAUSEGAME);
                }
            }


            internal void UpdateMainButton()
            {
                MainButtonDef mainButtonDef = this.Get<MainButtonDef>(EType.MainButton);
                bool flag = mainButtonDef != null;
                if (flag)
                {
                    mainButtonDef.buttonVisible = this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionB.SHOWINMENU);
                }

                this.Get<MainButtonDef>(EType.TeleButton);
            }


            private void UpdateKeyBindings()
            {
                try
                {
                    string text = this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionS.HOTKEYTELEPORT);
                    string text2 = this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionS.HOTKEYEDITOR);
                    KeyBindingDef keyBindingDef = DefTool.KeyBindingDef("HotkeyTeleport");
                    KeyBindingDef keyBindingDef2 = DefTool.KeyBindingDef("HotkeyEditor");
                    bool flag = keyBindingDef == null;
                    if (flag)
                    {
                        keyBindingDef = DefTool.TryCreateKeyBinding("HotkeyTeleport", text, Label.O_HOTKEYTELEPORT, Label.O_DESC_HOTKEYTELEPORT);
                    }

                    bool flag2 = keyBindingDef2 == null;
                    if (flag2)
                    {
                        keyBindingDef2 = DefTool.TryCreateKeyBinding("HotkeyEditor", text2, Label.O_HOTKEYEDITOR, Label.O_DESC_HOTKEYEDITOR);
                    }

                    this.Get<MainButtonDef>(EType.TeleButton).hotKey = keyBindingDef;
                    this.Get<MainButtonDef>(EType.MainButton).hotKey = keyBindingDef2;
                    KeyCode keyCode;
                    bool flag3 = Enum.TryParse<KeyCode>(text, out keyCode);
                    if (flag3)
                    {
                        KeyPrefs.KeyPrefsData.SetBinding(DefTool.KeyBindingDef("HotkeyTeleport"), KeyPrefs.BindingSlot.A, keyCode);
                        KeyPrefs.KeyPrefsData.CheckConflictsFor(DefTool.KeyBindingDef("HotkeyTeleport"), KeyPrefs.BindingSlot.A);
                    }

                    bool flag4 = Enum.TryParse<KeyCode>(text2, out keyCode);
                    if (flag4)
                    {
                        KeyPrefs.KeyPrefsData.SetBinding(DefTool.KeyBindingDef("HotkeyEditor"), KeyPrefs.BindingSlot.A, keyCode);
                        KeyPrefs.KeyPrefsData.CheckConflictsFor(DefTool.KeyBindingDef("HotkeyEditor"), KeyPrefs.BindingSlot.A);
                    }

                    KeyPrefs.Save();
                }
                catch
                {
                }
            }


            private void UpdateArchitectObjects()
            {
                bool flag = this.Get<CEditor.ModOptions>(EType.Settings).Get(OptionB.ZOMBOBJECTS);
                bool flag2 = this.useZombrella != flag;
                if (flag2)
                {
                    this.useZombrella = flag;
                    ThingDef thingDef = DefTool.ThingDef("Zombrella");
                    ThingDef thingDef2 = DefTool.ThingDef("Zombgrella");
                    bool flag3 = this.useZombrella;
                    if (flag3)
                    {
                        bool flag4 = thingDef == null;
                        if (flag4)
                        {
                            ThingTool.CreateBuilding("Zombrella", "Zombrella", Label.DESC_CASCET, typeof(CharacterEditorCascet), "CryptosleepCasket");
                        }

                        bool flag5 = thingDef2 == null;
                        if (flag5)
                        {
                            ThingTool.CreateBuilding("Zombgrella", "Zombgrella", Label.DESC_GRAVE, typeof(CharacterEditorGrave), "Grave");
                        }
                    }

                    thingDef = DefTool.ThingDef("Zombrella");
                    thingDef2 = DefTool.ThingDef("Zombgrella");
                    bool flag6 = this.useZombrella;
                    if (flag6)
                    {
                        bool flag7 = thingDef != null;
                        if (flag7)
                        {
                            thingDef.designatorDropdown = DefTool.DesignatorDropdownGroupDef("Zombrella");
                            thingDef.designationCategory = DefTool.DesignationCategoryDef("Misc");
                        }

                        bool flag8 = thingDef2 != null;
                        if (flag8)
                        {
                            thingDef2.designatorDropdown = DefTool.DesignatorDropdownGroupDef("Zombrella");
                            thingDef2.designationCategory = DefTool.DesignationCategoryDef("Misc");
                        }
                    }
                    else
                    {
                        bool flag9 = thingDef != null;
                        if (flag9)
                        {
                            thingDef.designatorDropdown = null;
                            thingDef.designationCategory = null;
                        }

                        bool flag10 = thingDef2 != null;
                        if (flag10)
                        {
                            thingDef2.designatorDropdown = null;
                            thingDef2.designationCategory = null;
                        }
                    }

                    DesignatorDropdownGroupDef designatorDropdownGroupDef = DefTool.DesignatorDropdownGroupDef("Zombrella");
                    if (designatorDropdownGroupDef != null)
                    {
                        designatorDropdownGroupDef.ResolveReferences();
                    }

                    DesignationCategoryDef designationCategoryDef = DefTool.DesignationCategoryDef("Misc");
                    if (designationCategoryDef != null)
                    {
                        designationCategoryDef.ResolveReferences();
                    }

                    try
                    {
                        ThingCategoryDef.Named("BuildingsMisc").ResolveReferences();
                        ThingCategoryDef.Named("BuildingsMisc").PostLoad();
                        ThingCategoryDefOf.Root.ResolveReferences();
                        ThingCategoryDefOf.Root.PostLoad();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.StackTrace);
                    }
                }
            }


            private bool useZombrella;


            internal Pawn p;


            private readonly Dictionary<EType, ServiceContainer> dicData;
        }


        private sealed class EditorUI : Window
        {
            private T Get<T>(CEditor.EditorUI.TabType tabType)
            {
                bool flag = !this.dicData.ContainsKey(tabType);
                if (flag)
                {
                    this.CreateType(tabType);
                }
                else
                {
                    bool flag2 = this.dicData[tabType] == null;
                    if (flag2)
                    {
                        this.dicData.Remove(tabType);
                        this.CreateType(tabType);
                    }
                }

                bool flag3 = this.dicData.ContainsKey(tabType);
                T result;
                if (flag3)
                {
                    result = (T)((object)this.dicData[tabType].GetService(typeof(T)));
                }
                else
                {
                    result = default(T);
                }

                return result;
            }


            private void CreateType(CEditor.EditorUI.TabType tabType)
            {
                bool flag = tabType == CEditor.EditorUI.TabType.BlockBio;
                if (flag)
                {
                    this.AddClassContainer<CEditor.EditorUI.BlockBio>(new CEditor.EditorUI.BlockBio(), tabType);
                }
                else
                {
                    bool flag2 = tabType == CEditor.EditorUI.TabType.BlockHealth;
                    if (flag2)
                    {
                        this.AddClassContainer<CEditor.EditorUI.BlockHealth>(new CEditor.EditorUI.BlockHealth(), tabType);
                    }
                    else
                    {
                        bool flag3 = tabType == CEditor.EditorUI.TabType.BlockInfo;
                        if (flag3)
                        {
                            this.AddClassContainer<CEditor.EditorUI.BlockInfo>(new CEditor.EditorUI.BlockInfo(), tabType);
                        }
                        else
                        {
                            bool flag4 = tabType == CEditor.EditorUI.TabType.BlockInventory;
                            if (flag4)
                            {
                                this.AddClassContainer<CEditor.EditorUI.BlockInventory>(new CEditor.EditorUI.BlockInventory(), tabType);
                            }
                            else
                            {
                                bool flag5 = tabType == CEditor.EditorUI.TabType.BlockLog;
                                if (flag5)
                                {
                                    this.AddClassContainer<CEditor.EditorUI.BlockLog>(new CEditor.EditorUI.BlockLog(), tabType);
                                }
                                else
                                {
                                    bool flag6 = tabType == CEditor.EditorUI.TabType.BlockNeeds;
                                    if (flag6)
                                    {
                                        this.AddClassContainer<CEditor.EditorUI.BlockNeeds>(new CEditor.EditorUI.BlockNeeds(), tabType);
                                    }
                                    else
                                    {
                                        bool flag7 = tabType == CEditor.EditorUI.TabType.BlockPawnList;
                                        if (flag7)
                                        {
                                            this.AddClassContainer<CEditor.EditorUI.BlockPawnList>(new CEditor.EditorUI.BlockPawnList(), tabType);
                                        }
                                        else
                                        {
                                            bool flag8 = tabType == CEditor.EditorUI.TabType.BlockPerson;
                                            if (flag8)
                                            {
                                                this.AddClassContainer<CEditor.EditorUI.BlockPerson>(new CEditor.EditorUI.BlockPerson(), tabType);
                                            }
                                            else
                                            {
                                                bool flag9 = tabType == CEditor.EditorUI.TabType.BlockRecords;
                                                if (flag9)
                                                {
                                                    this.AddClassContainer<CEditor.EditorUI.BlockRecords>(new CEditor.EditorUI.BlockRecords(), tabType);
                                                }
                                                else
                                                {
                                                    bool flag10 = tabType == CEditor.EditorUI.TabType.BlockSocial;
                                                    if (flag10)
                                                    {
                                                        this.AddClassContainer<CEditor.EditorUI.BlockSocial>(new CEditor.EditorUI.BlockSocial(), tabType);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            private void AddClassContainer<T>(T t, CEditor.EditorUI.TabType tabType)
            {
                this.dicData.Add(tabType, new ServiceContainer());
                this.dicData[tabType].AddService(typeof(T), t);
            }


            public override Vector2 InitialSize
            {
                get
                {
                    int num = this.SizeRand + this.SizeW0 + this.SizeW1 + this.SizeW2 + this.SizeRand;
                    int num2 = CEditor.API.GetO(OptionB.SHOWMINI) ? 320 : WindowTool.MaxH;
                    return new Vector2((float)num, (float)num2);
                }
            }


            public int SizeRand
            {
                get { return 18; }
            }


            public int SizeH
            {
                get { return (int)(this.InitialSize.y - 36f); }
            }


            public int SizeW0
            {
                get { return CEditor.API.GetO(OptionB.SHOWPAWNLIST) ? 100 : 0; }
            }


            public int SizeW1
            {
                get { return 250; }
            }


            public int SizeW2
            {
                get { return CEditor.API.GetO(OptionB.SHOWTABS) ? 840 : -10; }
            }


            public int PersonX
            {
                get { return this.SizeW0; }
            }


            public int PersonY
            {
                get { return 0; }
            }


            public int PersonW
            {
                get { return this.SizeW1 - 10; }
            }


            public int PersonH
            {
                get { return this.SizeH; }
            }


            public int TabX
            {
                get { return this.SizeW0 + this.SizeW1; }
            }


            public int TabY
            {
                get { return 30; }
            }


            public int TabW
            {
                get { return this.SizeW2; }
            }


            public int TabH
            {
                get { return this.SizeH - this.TabY; }
            }


            internal EditorUI() : base()
            {
                this.dicData = new Dictionary<CEditor.EditorUI.TabType, ServiceContainer>();
                this.currentTab = CEditor.EditorUI.TabType.BlockBio;
                this.doOnce = true;
                SearchTool.Update(SearchTool.SIndex.Editor);
                this.doCloseX = true;
                this.forcePause = CEditor.API.GetO(OptionB.PAUSEGAME);
                this.onlyOneOfTypeAllowed = true;
                this.resizeable = false;
                this.preventCameraMotion = false;
                this.closeOnAccept = false;
                this.draggable = true;
                this.layer = CEditor.Layer;
                SoundTool.PlayThis(SoundDefOf.Tick_Low);
            }


            ~EditorUI()
            {
                try
                {
                    foreach (CEditor.EditorUI.TabType key in this.dicData.Keys)
                    {
                        this.dicData[key].Dispose();
                    }

                    this.dicData.Clear();
                }
                finally
                {
                }
            }


            private void PreselectPawn(Pawn pawn)
            {
                if (pawn == null)
                {
                    object obj;
                    if (!CEditor.InStartingScreen)
                    {
                        Selector selector = Find.Selector;
                        obj = ((selector != null) ? selector.FirstSelectedObject : null);
                    }
                    else
                    {
                        obj = null;
                    }

                    object obj2 = obj;
                    if (obj2 != null)
                    {
                        Pawn pawn2 = (obj2.GetType() == typeof(Pawn)) ? (obj2 as Pawn) : null;
                        Corpse corpse = (obj2.GetType() == typeof(Corpse)) ? (obj2 as Corpse) : null;
                        Building_AncientCryptosleepCasket building_AncientCryptosleepCasket = (obj2.GetType() == typeof(Building_AncientCryptosleepCasket)) ? (obj2 as Building_AncientCryptosleepCasket) : null;
                        Building_CryptosleepCasket building_CryptosleepCasket = (obj2.GetType() == typeof(Building_CryptosleepCasket)) ? (obj2 as Building_CryptosleepCasket) : null;
                        CharacterEditorCascet characterEditorCascet = (obj2.GetType() == typeof(CharacterEditorCascet)) ? (obj2 as CharacterEditorCascet) : null;
                        CharacterEditorGrave characterEditorGrave = (obj2.GetType() == typeof(CharacterEditorGrave)) ? (obj2 as CharacterEditorGrave) : null;
                        if (pawn2 != null)
                        {
                            CEditor.API.Pawn = pawn2;
                        }
                        else if (corpse != null)
                        {
                            CEditor.API.Pawn = corpse.InnerPawn;
                        }
                        else if (characterEditorCascet != null && characterEditorCascet.ContainedThing != null)
                        {
                            CEditor.API.Pawn = ((characterEditorCascet.ContainedThing.GetType() == typeof(Pawn)) ? (characterEditorCascet.ContainedThing as Pawn) : null);
                            CEditor.API.Get<Dictionary<int, Building_CryptosleepCasket>>(EType.UIContainers)[0] = characterEditorCascet;
                        }
                        else if (characterEditorGrave != null && characterEditorGrave.ContainedThing != null)
                        {
                            CEditor.API.Pawn = ((characterEditorGrave.ContainedThing.GetType() == typeof(Pawn)) ? (characterEditorGrave.ContainedThing as Pawn) : null);
                            CEditor.API.Get<Dictionary<int, Building_CryptosleepCasket>>(EType.UIContainers)[0] = characterEditorGrave;
                        }
                        else if (building_AncientCryptosleepCasket != null && building_AncientCryptosleepCasket.ContainedThing != null)
                        {
                            CEditor.API.Pawn = ((building_AncientCryptosleepCasket.ContainedThing.GetType() == typeof(Pawn)) ? (building_AncientCryptosleepCasket.ContainedThing as Pawn) : null);
                        }
                        else if (building_CryptosleepCasket != null && building_CryptosleepCasket.ContainedThing != null)
                        {
                            CEditor.API.Pawn = ((building_CryptosleepCasket.ContainedThing.GetType() == typeof(Pawn)) ? (building_CryptosleepCasket.ContainedThing as Pawn) : null);
                        }
                    }
                }
                else
                {
                    CEditor.API.Pawn = pawn;
                }

                this.Get<CEditor.EditorUI.BlockPawnList>(CEditor.EditorUI.TabType.BlockPawnList).ReloadList();
            }


            internal void Start(Pawn pawn)
            {
                if (WindowTool.IsOpen<CEditor.EditorUI>())
                {
                    WindowTool.BringToFront(this, true);
                    return;
                }

                Log.Message("Preselecting pawn!");
                try
                {
                    this.PreselectPawn(pawn);
                }
                catch (Exception obj)
                {
                    Log.Message("Failed to preselect pawn!");
                    Log.Message(obj);
                }

                Log.Message("Setting current tab to BlockBio");
                this.currentTab = CEditor.EditorUI.TabType.BlockBio;
                Log.Message("Opening this");
                WindowTool.Open(this);
            }


            public override void Close(bool doCloseSound = true)
            {
                this.currentTab = CEditor.EditorUI.TabType.None;
                CEditor.API.Pawn = null;
                bool flag = !CEditor.InStartingScreen && !CEditor.API.GetO(OptionB.STAYINCASCET);
                if (flag)
                {
                    try
                    {
                        Building_CryptosleepCasket building_CryptosleepCasket = CEditor.API.Get<Dictionary<int, Building_CryptosleepCasket>>(EType.UIContainers)[0];
                        if (building_CryptosleepCasket != null)
                        {
                            building_CryptosleepCasket.EjectContents();
                        }
                    }
                    catch
                    {
                    }
                }

                WindowTool.TryRemove<DialogColorPicker>();
                WindowTool.TryRemove<DialogChangeHeadAddons>();
                bool inStartingScreen = CEditor.InStartingScreen;
                if (inStartingScreen)
                {
                    try
                    {
                        CEditor.pstartInstance.SetMemberValue("curPawn", Find.GameInitData.startingAndOptionalPawns.FirstOrFallback(null));
                    }
                    catch
                    {
                    }
                }

                SearchTool.Save(SearchTool.SIndex.Editor, this.windowRect.position);
                this.doOnce = true;
                base.Close(doCloseSound);
            }


            private void DrawTabs(int x, int y, int w, int h)
            {
                bool flag = w <= 0 || CEditor.API.Pawn == null;
                if (!flag)
                {
                    int num = w / 8;
                    GUI.color = ((this.currentTab == CEditor.EditorUI.TabType.BlockBio) ? Color.yellow : Color.white);
                    SZWidgets.ButtonTextVar<CEditor.EditorUI.TabType>((float)x, (float)y, (float)num, (float)h, "TabCharacter".Translate(), new Action<CEditor.EditorUI.TabType>(this.ATabwechsel), CEditor.EditorUI.TabType.BlockBio);
                    x += num;
                    GUI.color = ((this.currentTab == CEditor.EditorUI.TabType.BlockInventory) ? Color.yellow : Color.white);
                    SZWidgets.ButtonTextVar<CEditor.EditorUI.TabType>((float)x, (float)y, (float)num, (float)h, "Inventory".Translate(), new Action<CEditor.EditorUI.TabType>(this.ATabwechsel), CEditor.EditorUI.TabType.BlockInventory);
                    x += num;
                    GUI.color = ((this.currentTab == CEditor.EditorUI.TabType.BlockHealth) ? Color.yellow : Color.white);
                    SZWidgets.ButtonTextVar<CEditor.EditorUI.TabType>((float)x, (float)y, (float)num, (float)h, "Health".Translate(), new Action<CEditor.EditorUI.TabType>(this.ATabwechsel), CEditor.EditorUI.TabType.BlockHealth);
                    x += num;
                    GUI.color = ((this.currentTab == CEditor.EditorUI.TabType.BlockNeeds) ? Color.yellow : Color.white);
                    SZWidgets.ButtonTextVar<CEditor.EditorUI.TabType>((float)x, (float)y, (float)num, (float)h, "TabNeeds".Translate(), new Action<CEditor.EditorUI.TabType>(this.ATabwechsel), CEditor.EditorUI.TabType.BlockNeeds);
                    x += num;
                    GUI.color = ((this.currentTab == CEditor.EditorUI.TabType.BlockSocial) ? Color.yellow : Color.white);
                    SZWidgets.ButtonTextVar<CEditor.EditorUI.TabType>((float)x, (float)y, (float)num, (float)h, "TabSocial".Translate(), new Action<CEditor.EditorUI.TabType>(this.ATabwechsel), CEditor.EditorUI.TabType.BlockSocial);
                    x += num;
                    GUI.color = ((this.currentTab == CEditor.EditorUI.TabType.BlockLog) ? Color.yellow : Color.white);
                    SZWidgets.ButtonTextVar<CEditor.EditorUI.TabType>((float)x, (float)y, (float)num, (float)h, "TabLog".Translate(), new Action<CEditor.EditorUI.TabType>(this.ATabwechsel), CEditor.EditorUI.TabType.BlockLog);
                    x += num;
                    GUI.color = ((this.currentTab == CEditor.EditorUI.TabType.BlockInfo) ? Color.yellow : Color.white);
                    SZWidgets.ButtonTextVar<CEditor.EditorUI.TabType>((float)x, (float)y, (float)num, (float)h, Label.INFO, new Action<CEditor.EditorUI.TabType>(this.ATabwechsel), CEditor.EditorUI.TabType.BlockInfo);
                    x += num;
                    GUI.color = ((this.currentTab == CEditor.EditorUI.TabType.BlockRecords) ? Color.yellow : Color.white);
                    SZWidgets.ButtonTextVar<CEditor.EditorUI.TabType>((float)x, (float)y, (float)num, (float)h, "TabRecords".Translate(), new Action<CEditor.EditorUI.TabType>(this.ATabwechsel), CEditor.EditorUI.TabType.BlockRecords);
                }
            }


            private void DrawSelectedTab(int x, int y, int w, int h)
            {
                bool flag = w <= 0;
                if (!flag)
                {
                    bool flag2 = CEditor.API.Pawn == null;
                    if (!flag2)
                    {
                        Rect rect = new Rect((float)x, (float)y, (float)w, (float)h);
                        CEditor.EditorUI.coord c;
                        c.x = x;
                        c.y = y;
                        c.w = w;
                        c.h = h;
                        GUI.color = Color.white;
                        bool flag3 = this.currentTab == CEditor.EditorUI.TabType.BlockBio;
                        if (flag3)
                        {
                            this.Get<CEditor.EditorUI.BlockBio>(CEditor.EditorUI.TabType.BlockBio).Draw(c);
                        }
                        else
                        {
                            bool flag4 = this.currentTab == CEditor.EditorUI.TabType.BlockInventory;
                            if (flag4)
                            {
                                this.Get<CEditor.EditorUI.BlockInventory>(CEditor.EditorUI.TabType.BlockInventory).Draw(c);
                            }
                            else
                            {
                                bool flag5 = this.currentTab == CEditor.EditorUI.TabType.BlockHealth;
                                if (flag5)
                                {
                                    this.Get<CEditor.EditorUI.BlockHealth>(CEditor.EditorUI.TabType.BlockHealth).Draw(c);
                                }
                                else
                                {
                                    bool flag6 = this.currentTab == CEditor.EditorUI.TabType.BlockNeeds;
                                    if (flag6)
                                    {
                                        this.Get<CEditor.EditorUI.BlockNeeds>(CEditor.EditorUI.TabType.BlockNeeds).Draw(c);
                                    }
                                    else
                                    {
                                        bool flag7 = this.currentTab == CEditor.EditorUI.TabType.BlockSocial;
                                        if (flag7)
                                        {
                                            this.Get<CEditor.EditorUI.BlockSocial>(CEditor.EditorUI.TabType.BlockSocial).Draw(c);
                                        }
                                        else
                                        {
                                            bool flag8 = this.currentTab == CEditor.EditorUI.TabType.BlockLog;
                                            if (flag8)
                                            {
                                                this.Get<CEditor.EditorUI.BlockLog>(CEditor.EditorUI.TabType.BlockLog).Draw(c);
                                            }
                                            else
                                            {
                                                bool flag9 = this.currentTab == CEditor.EditorUI.TabType.BlockInfo;
                                                if (flag9)
                                                {
                                                    this.Get<CEditor.EditorUI.BlockInfo>(CEditor.EditorUI.TabType.BlockInfo).Draw(c);
                                                }
                                                else
                                                {
                                                    bool flag10 = this.currentTab == CEditor.EditorUI.TabType.BlockRecords;
                                                    if (flag10)
                                                    {
                                                        this.Get<CEditor.EditorUI.BlockRecords>(CEditor.EditorUI.TabType.BlockRecords).Draw(c);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            public override void DoWindowContents(Rect inRect)
            {
                bool flag = this.currentTab == CEditor.EditorUI.TabType.None;
                if (!flag)
                {
                    bool flag2 = !CEditor.InStartingScreen && this.doOnce;
                    if (flag2)
                    {
                        SearchTool.SetPosition(SearchTool.SIndex.Editor, ref this.windowRect, ref this.doOnce, 0);
                    }

                    bool flag3 = this.windowRect.height > 320f;
                    if (flag3)
                    {
                        SZWidgets.ButtonImage((float)this.PersonX, (float)(this.PersonH - 22), 22f, 22f, CEditor.API.GetO(OptionB.SHOWPAWNLIST) ? "bforward" : "bbackward", new Action(this.AShowList), "", default(Color));
                        SZWidgets.ButtonImage((float)(this.PersonX + this.SizeW1 - 32), (float)(this.PersonH - 22), 22f, 22f, CEditor.API.GetO(OptionB.SHOWTABS) ? "bbackward" : "bforward", new Action(this.AShowTabs), "", default(Color));
                    }

                    CEditor.EditorUI.coord c;
                    c.x = 0;
                    c.y = 0;
                    c.w = this.SizeW0;
                    c.h = this.SizeH;
                    this.Get<CEditor.EditorUI.BlockPawnList>(CEditor.EditorUI.TabType.BlockPawnList).Draw(c);
                    CEditor.EditorUI.coord c2;
                    c2.x = this.PersonX;
                    c2.y = this.PersonY;
                    c2.w = this.PersonW;
                    c2.h = this.PersonH;
                    this.Get<CEditor.EditorUI.BlockPerson>(CEditor.EditorUI.TabType.BlockPerson).Draw(c2);
                    this.DrawTabs(this.TabX, 0, this.TabW, this.TabY);
                    this.DrawSelectedTab(this.TabX, this.TabY, this.TabW, this.TabH);
                    int index = WindowTool.GetIndex(this);
                    int index2 = WindowTool.GetIndex(WindowTool.GetWindowOf<DialogColorPicker>());
                    int index3 = WindowTool.GetIndex(WindowTool.GetWindowOf<DialogChangeHeadAddons>());
                    WindowTool.TopLayerForWindowOf<DialogColorPicker>(index > index2);
                    WindowTool.TopLayerForWindowOf<DialogChangeHeadAddons>(index > index3);
                    bool inStartingScreen = CEditor.InStartingScreen;
                    if (inStartingScreen)
                    {
                        bool flag4 = !Find.GameInitData.startingAndOptionalPawns.IsSame(this.Get<CEditor.EditorUI.BlockPawnList>(CEditor.EditorUI.TabType.BlockPawnList).ListOfPawns);
                        if (flag4)
                        {
                            this.Get<CEditor.EditorUI.BlockPawnList>(CEditor.EditorUI.TabType.BlockPawnList).ReloadList();
                        }
                    }

                    this.closeOnClickedOutside = CEditor.InStartingScreen;
                }
            }


            private void AShowList()
            {
                CEditor.API.Toggle(OptionB.SHOWPAWNLIST);
            }


            private void AShowTabs()
            {
                CEditor.API.Toggle(OptionB.SHOWTABS);
            }


            private void ATabwechsel(CEditor.EditorUI.TabType tab)
            {
                this.currentTab = tab;
                bool flag = this.currentTab == CEditor.EditorUI.TabType.BlockInfo;
                if (flag)
                {
                    StatsReportUtility.Notify_QuickSearchChanged();
                }
            }


            private bool doOnce;


            private CEditor.EditorUI.TabType currentTab;


            private readonly Dictionary<CEditor.EditorUI.TabType, ServiceContainer> dicData;


            private enum TabType
            {
                BlockBio,

                BlockHealth,

                BlockInfo,

                BlockInventory,

                BlockLog,

                BlockNeeds,

                BlockPawnList,

                BlockPerson,

                BlockRecords,

                BlockSocial,

                None
            }


            private struct coord
            {
                internal int x;


                internal int y;


                internal int w;


                internal int h;
            }


            private class BlockBio
            {
                private string GetTraitTooltip(Trait t)
                {
                    string result;
                    try
                    {
                        result = t.TipString(CEditor.API.Pawn);
                    }
                    catch
                    {
                        result = "";
                    }

                    return result;
                }


                internal BlockBio()
                {
                    this.iTickInputName = 0;
                    this.iTickInputAge = 0;
                    this.iTickInputSkill = 0;
                    this.iChronoAge = 0;
                    this.iAge = 0;
                    this.ageBuffer = "";
                    this.chronoAgeBuffer = "";
                    this.uniqueID = null;
                    this.FTraitTooltip = new Func<Trait, string>(this.GetTraitTooltip);
                    this.bRemoveTrait = false;
                    this.bRemoveAbility = false;
                    this.scrollTraits = default(Vector2);
                    this.lCopyTraits = new List<Trait>();
                    this.lCopyAbilities = new List<Ability>();
                    this.lOfCopySkills = new List<SkillRecord>();
                    Game game = Current.Game;
                    World world = (game != null) ? game.World : null;
                    this.lOfColonists = ((world == null) ? null : PawnxTool.GetPawnList(Label.COLONISTS, true, Faction.OfPlayer));
                    this.lOfRoyalTitles = DefTool.ListByModWithNull<RoyalTitleDef>(null, (RoyalTitleDef def) => !def.defName.NullOrEmpty()).ToHashSet<RoyalTitleDef>();
                    this.selectedTrainer = null;
                    this.currentTitle = null;
                    this.regexInt = new Regex("^[0-9]*");
                }


                internal void Draw(CEditor.EditorUI.coord c)
                {
                    int x = c.x;
                    int y = c.y;
                    int w = c.w;
                    int h = c.h;
                    if (CEditor.API.Pawn != null)
                    {
                        this.DrawName(x + 20, y + 30, w, 30);
                        this.DrawDesc(x + 20, y + 60, w, 30);
                        this.DrawBackstory(x + 20, y + 100);
                        this.DrawIncapableOf(x + 20, y + 205);
                        this.DrawTraits(x + 20, y + 305, h);
                        this.DrawSkills(x + 330, y + 100);
                        this.DrawAbilities(x + 330, y + 500);
                        this.DrawPsycasts(x + 590, y + 380);
                        this.DrawTraining(x + 20, y + 100, w, h - 100);
                        this.DrawFaction(x + 600, y + 30, 250);
                        this.DrawIdeo(x + 600, y + 60, 250);
                        this.DrawXeno(x + 600, y + 90, 250);
                        this.DrawTitle(x + 600, y + 120, 250);
                        this.DrawExtendables(x + 600, y + 150, 30);
                        this.DrawCapsule(x + 590, y + 200, 200, 144);
                        this.DrawLowerButtons(x, y, w, h);
                    }
                }


                private void DrawLowerButtons(int x, int y, int w, int h)
                {
                    try
                    {
                        bool flag = CEditor.API.Pawn.Faction != Faction.OfPlayer && CEditor.API.Pawn.Faction != Faction.OfMechanoids && !CEditor.API.Pawn.Dead;
                        if (flag)
                        {
                            GUI.color = Color.white;
                            SZWidgets.ButtonText(new Rect((float)(x + w - 120), (float)(y + h - 30), 120f, 30f), Label.RECRUIT, new Action(this.ARecruit), "");
                            SZWidgets.ButtonText(new Rect((float)(x + w - 240), (float)(y + h - 30), 120f, 30f), Label.ENSLAVE, new Action(this.AEnslave), "");
                        }
                    }
                    catch (Exception ex)
                    {
                        bool devMode = Prefs.DevMode;
                        if (devMode)
                        {
                            Log.Error(ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }


                private void ARecruit()
                {
                    bool flag = CEditor.API.Pawn.guest != null;
                    if (flag)
                    {
                        CEditor.API.Pawn.guest.Recruitable = true;
                    }

                    DebugActionsUtility.DustPuffFrom(CEditor.API.Pawn);
                    CEditor.API.Pawn.RecruitPawn();
                }


                private void AEnslave()
                {
                    DebugActionsUtility.DustPuffFrom(CEditor.API.Pawn);
                    CEditor.API.Pawn.EnslavePawn();
                }


                private void DrawBiologicalAge(int x, int y)
                {
                    this.iAge = CEditor.API.Pawn.ageTracker.AgeBiologicalYears;
                    SZWidgets.ButtonImage((float)x, (float)y, 24f, 24f, "bbackward", new Action(this.ASubAge), "", default(Color));
                    x += 25;
                    Text.WordWrap = false;
                    Widgets.Label(new Rect((float)x, (float)(y + 1), 150f, 24f), "AgeBiological".Translate().ToString().SubstringTo(":", false));
                    Text.WordWrap = true;
                    x += 150;
                    this.ageBuffer = Widgets.TextField(new Rect((float)x, (float)y, 50f, 24f), this.ageBuffer, 4, this.regexInt);
                    x += 50;
                    SZWidgets.ButtonImage((float)x, (float)y, 24f, 24f, "bforward", new Action(this.AAddAge), "", default(Color));
                    int num = 0;
                    bool flag = int.TryParse(this.ageBuffer, out num);
                    if (flag)
                    {
                        bool flag2 = this.iAge != num && num > 0;
                        if (flag2)
                        {
                            CEditor.API.Pawn.SetAge(num);
                            CEditor.API.UpdateGraphics();
                        }
                    }
                }


                private void DrawChronologicalAge(int x, int y)
                {
                    this.iChronoAge = CEditor.API.Pawn.ageTracker.AgeChronologicalYears;
                    SZWidgets.ButtonImage((float)x, (float)y, 24f, 24f, "bbackward", new Action(this.ASubChronoAge), "", default(Color));
                    x += 25;
                    Text.WordWrap = false;
                    Widgets.Label(new Rect((float)x, (float)(y + 1), 165f, 24f), "AgeChronological".Translate().ToString().SubstringTo(":", false));
                    Text.WordWrap = true;
                    x += 165;
                    this.chronoAgeBuffer = Widgets.TextField(new Rect((float)x, (float)y, 50f, 24f), this.chronoAgeBuffer, 5, this.regexInt);
                    x += 50;
                    SZWidgets.ButtonImage((float)x, (float)y, 24f, 24f, "bforward", new Action(this.AAddChronoAge), "", default(Color));
                    x += 30;
                    SZWidgets.ButtonImage((float)x, (float)y, 24f, 24f, "bstar", new Action(this.AAddBirthdayTick), "", default(Color));
                    int num = 0;
                    bool flag = int.TryParse(this.chronoAgeBuffer, out num);
                    if (flag)
                    {
                        bool flag2 = this.iChronoAge.ToString() != this.chronoAgeBuffer && num > 0;
                        if (flag2)
                        {
                            CEditor.API.Pawn.SetChronoAge(num);
                        }
                    }
                }


                private void DrawDesc(int x, int y, int w, int h)
                {
                    try
                    {
                        Text.Font = GameFont.Small;
                        Rect rect = new Rect((float)x, (float)y, (float)(w - 260), (float)(h - 4));
                        Rect rect2 = new Rect(rect.x + 100f, rect.y, rect.width - 300f, rect.height);
                        Rect rect3 = new Rect((float)x, (float)y, 100f, 25f);
                        bool flag = this.iTickInputAge > 0;
                        if (flag)
                        {
                            SZWidgets.ButtonInvisible(new Rect((float)x, (float)(y + h), (float)w, 600f), delegate { this.iTickInputAge = 0; }, "");
                            bool flag2 = this.uniqueID != CEditor.API.Pawn.ThingID;
                            if (flag2)
                            {
                                this.iTickInputAge = 0;
                            }
                            else
                            {
                                this.DrawBiologicalAge((int)rect.x - 25, (int)rect.y);
                                this.DrawChronologicalAge((int)rect.x + 250, (int)rect.y);
                                this.iTickInputAge--;
                            }
                        }
                        else
                        {
                            SZWidgets.ButtonInvisible(rect3, new Action(this.AConfirmRaceChange), CEditor.RACEDESC(CEditor.API.Pawn.def));
                            SZWidgets.ButtonInvisible(rect2, new Action(this.ABeginAgeChange), "");
                            Widgets.Label(rect, CEditor.API.Pawn.GetPawnDescription(null));
                        }

                        TooltipHandler.TipRegion(rect3, CEditor.RACEDESC(CEditor.API.Pawn.def));
                        TooltipHandler.TipRegion(rect2, () => CEditor.API.Pawn.ageTracker.AgeTooltipString, 6873641);
                    }
                    catch (Exception ex)
                    {
                        bool devMode = Prefs.DevMode;
                        if (devMode)
                        {
                            Log.Error(ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }


                private void DrawNameTripe(Rect rName)
                {
                    GUI.BeginGroup(rName);
                    NameTriple nameTriple = CEditor.API.Pawn.Name as NameTriple;
                    GUI.color = Color.white;
                    string text = Widgets.TextField(new Rect(0f, 0f, 100f, 28f), nameTriple.First, 17, CharacterCardUtility.ValidNameRegex);
                    bool flag = nameTriple.Nick == text || nameTriple.Nick == nameTriple.Last;
                    if (flag)
                    {
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);
                    }

                    string text2 = Widgets.TextField(new Rect(105f, 0f, 100f, 28f), nameTriple.Nick, 17, CharacterCardUtility.ValidNameRegex);
                    GUI.color = Color.white;
                    string text3 = Widgets.TextField(new Rect(210f, 0f, 100f, 28f), nameTriple.Last, 17, CharacterCardUtility.ValidNameRegex);
                    SZWidgets.ButtonImageCol(new Rect(320f, 2f, 25f, 25f), "brandom", new Action(this.AChangeNameTriple), Color.white, Label.TIP_CHANGE_NAME);
                    SZWidgets.ButtonImageCol(new Rect(350f, 2f, 25f, 25f), "brandom", new Action(this.AChangeNameTripleAll), ColorLibrary.Beige, Label.TIP_CHANGE_NAME_ALL);
                    bool flag2 = nameTriple.First != text || nameTriple.Nick != text2 || nameTriple.Last != text3;
                    if (flag2)
                    {
                        CEditor.API.Pawn.Name = new NameTriple(text, text2, text3);
                        this.iTickInputName = 1200;
                    }

                    TooltipHandler.TipRegion(new Rect(0f, 0f, 100f, 30f), "FirstNameDesc".Translate());
                    TooltipHandler.TipRegion(new Rect(105f, 0f, 100f, 30f), "ShortIdentifierDesc".Translate());
                    TooltipHandler.TipRegion(new Rect(210f, 0f, 100f, 30f), "LastNameDesc".Translate());
                    GUI.EndGroup();
                    GUI.color = Color.white;
                }


                private void ResetTickInput()
                {
                    this.iTickInputName = 1200;
                }


                private void AChangeNameTriple()
                {
                    CEditor.API.Pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(CEditor.API.Pawn, NameStyle.Full, null, false, null);
                    this.ResetTickInput();
                }


                private void AChangeNameTripleAll()
                {
                    GenderPossibility gp = (CEditor.API.Pawn.gender == Gender.Male) ? GenderPossibility.Male : ((CEditor.API.Pawn.gender == Gender.Female) ? GenderPossibility.Female : GenderPossibility.Either);
                    CEditor.API.Pawn.Name = PawnNameDatabaseSolid.GetListForGender(gp).RandomElement<NameTriple>();
                    this.ResetTickInput();
                }


                private void DrawNameSingle(Rect rName)
                {
                    GUI.BeginGroup(rName);
                    bool flag = CEditor.API.Pawn.Name == null;
                    if (flag)
                    {
                        CEditor.API.Pawn.Name = new NameSingle(CEditor.API.Pawn.kindDef.label, false);
                    }

                    NameSingle nameSingle = CEditor.API.Pawn.Name as NameSingle;
                    string text = nameSingle.Name ?? CEditor.API.Pawn.kindDef.label;
                    GUI.color = Color.white;
                    Rect rect = new Rect(0f, 0f, 200f, 28f);
                    text = Widgets.TextField(rect, text, 64, CharacterCardUtility.ValidNameRegex);
                    bool flag2 = nameSingle.Name != text;
                    if (flag2)
                    {
                        CEditor.API.Pawn.Name = new NameSingle(text, false);
                        this.iTickInputName = 1200;
                    }

                    Rect rect2 = new Rect(rect.x + rect.width + 10f, rect.y, rect.height, rect.height);
                    bool flag3 = Widgets.ButtonImage(rect2, ContentFinder<Texture2D>.Get("brandom", true), true);
                    if (flag3)
                    {
                        CEditor.API.Pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(CEditor.API.Pawn, NameStyle.Full, null, false, null);
                    }

                    TooltipHandler.TipRegion(rect, "FirstNameDesc".Translate());
                    GUI.EndGroup();
                }


                private void DrawName(int x, int y, int w, int h)
                {
                    try
                    {
                        Rect rect = new Rect((float)x, (float)y, (float)w, (float)h);
                        bool flag = this.iTickInputName > 0;
                        if (flag)
                        {
                            SZWidgets.ButtonInvisible(new Rect((float)x, (float)(y + h), (float)w, 600f), delegate { this.iTickInputName = 0; }, "");
                            Name name = CEditor.API.Pawn.Name;
                            bool flag2 = ((name != null) ? name.GetType() : null) == typeof(NameTriple);
                            if (flag2)
                            {
                                this.DrawNameTripe(rect);
                            }
                            else
                            {
                                this.DrawNameSingle(rect);
                            }

                            this.iTickInputName--;
                        }
                        else
                        {
                            Text.Font = GameFont.Medium;
                            this.currentTitle = (CEditor.API.Pawn.HasRoyalTitle() ? CEditor.API.Pawn.royalty.AllTitlesForReading.First<RoyalTitle>() : null);
                            bool flag3 = this.currentTitle != null;
                            if (flag3)
                            {
                                int num = this.currentTitle.def.LabelCap.Length * 12;
                                string toolTip = (string)typeof(CharacterCardUtility).CallMethod("GetTitleTipString", new object[]
                                {
                                    CEditor.API.Pawn,
                                    this.currentTitle.faction,
                                    this.currentTitle,
                                    CEditor.API.Pawn.royalty.GetFavor(this.currentTitle.faction)
                                });
                                SZWidgets.Label(rect, this.currentTitle.def.GetLabelCapFor(CEditor.API.Pawn) + " " + CEditor.API.Pawn.GetPawnNameColored(true), null, "");
                                SZWidgets.ButtonInvisibleVar<RoyalTitle>(new Rect((float)x, (float)y, (float)num, (float)h), new Action<RoyalTitle>(this.AShowTitle), this.currentTitle, toolTip);
                                SZWidgets.ButtonInvisible(new Rect((float)(x + num), (float)y, 300f, (float)h), new Action(this.ABeginEditName), "Rename".Translate());
                            }
                            else
                            {
                                SZWidgets.Label(rect, CEditor.API.Pawn.GetPawnNameColored(true), null, "");
                                SZWidgets.ButtonInvisible(new Rect((float)x, (float)y, 300f, (float)h), new Action(this.ABeginEditName), "Rename".Translate());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        bool devMode = Prefs.DevMode;
                        if (devMode)
                        {
                            Log.Error(ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }


                private void ARemoveTitle()
                {
                    bool flag = this.currentTitle != null;
                    if (flag)
                    {
                        CEditor.API.Pawn.RemoveTitle(this.currentTitle);
                    }
                }


                private void ABeginEditName()
                {
                    this.iTickInputName = 1200;
                }


                private void AShowTitle(RoyalTitle r)
                {
                    WindowTool.Open(new Dialog_InfoCard(r.def, r.faction, null));
                }


                private void ASetTitle(RoyalTitleDef r)
                {
                    bool flag = r == null;
                    if (flag)
                    {
                        this.ARemoveTitle();
                    }
                    else
                    {
                        CEditor.API.Pawn.SetTitle(r.defName);
                    }
                }


                private void AConfirmRaceChange()
                {
                    WindowTool.Open(new DialogChangeRace(CEditor.API.Pawn));
                }


                private void ABeginAgeChange()
                {
                    this.uniqueID = CEditor.API.Pawn.ThingID;
                    this.iAge = CEditor.API.Pawn.ageTracker.AgeBiologicalYears;
                    this.ageBuffer = this.iAge.ToString();
                    this.iChronoAge = CEditor.API.Pawn.ageTracker.AgeChronologicalYears;
                    this.chronoAgeBuffer = this.iChronoAge.ToString();
                    this.iTickInputAge = 1200;
                }


                private void AAddAge()
                {
                    this.iAge++;
                    CEditor.API.Pawn.SetAge(this.iAge);
                    this.ageBuffer = this.iAge.ToString();
                    this.iTickInputAge = 1200;
                    CEditor.API.UpdateGraphics();
                }


                private void ASubAge()
                {
                    bool flag = this.iAge > 0;
                    if (flag)
                    {
                        this.iAge--;
                    }

                    CEditor.API.Pawn.SetAge(this.iAge);
                    this.ageBuffer = this.iAge.ToString();
                    this.iTickInputAge = 1200;
                    CEditor.API.UpdateGraphics();
                }


                private void AAddChronoAge()
                {
                    bool flag = this.iChronoAge < 15498;
                    if (flag)
                    {
                        this.iChronoAge++;
                    }

                    CEditor.API.Pawn.SetChronoAge(this.iChronoAge);
                    this.chronoAgeBuffer = this.iChronoAge.ToString();
                    this.iTickInputAge = 1200;
                }


                private void ASubChronoAge()
                {
                    bool flag = this.iChronoAge > 0;
                    if (flag)
                    {
                        this.iChronoAge--;
                    }

                    CEditor.API.Pawn.SetChronoAge(this.iChronoAge);
                    this.chronoAgeBuffer = this.iChronoAge.ToString();
                    this.iTickInputAge = 1200;
                }


                private void AAddBirthdayTick()
                {
                    this.iTickInputAge = 0;
                    WindowTool.Open(new DialogChangeBirthday(CEditor.API.Pawn));
                }


                private void DrawBackstory(int x, int y)
                {
                    try
                    {
                        bool flag = CEditor.API.Pawn.story != null;
                        if (flag)
                        {
                            Text.Font = GameFont.Medium;
                            Widgets.Label(new Rect((float)x, (float)y, 250f, 30f), "Backstory".Translate());
                            y += 35;
                            Rect rect = new Rect((float)(x - 24), (float)y, 320f, 30f);
                            bool flag2 = Mouse.IsOver(rect);
                            if (flag2)
                            {
                                BackstoryDef childhood = CEditor.API.Pawn.story.Childhood;
                                this.tooltip1 = ((childhood != null) ? childhood.FullDescriptionFor(CEditor.API.Pawn).Resolve() : null);
                            }

                            Rect rect2 = rect;
                            Action onClicked = new Action(this.AChangeChildhoodUI);
                            Action onRandom = new Action(this.ARandomChildhood);
                            Action onPrev = new Action(this.ASetPrevChildhood);
                            Action onNext = new Action(this.ASetNextChildhood);
                            Action onTextureClick = null;
                            Action onToggle = null;
                            TaggedString t = "Childhood".Translate() + ": ";
                            BackstoryDef childhood2 = CEditor.API.Pawn.story.Childhood;
                            SZWidgets.NavSelectorImageBox(rect2, onClicked, onRandom, onPrev, onNext, onTextureClick, onToggle, t + ((childhood2 != null) ? childhood2.TitleCapFor(CEditor.API.Pawn.gender) : null), this.tooltip1, null, null, null, default(Color), null);
                            y += 30;
                            Rect rect3 = new Rect((float)(x - 24), (float)y, 320f, 30f);
                            bool flag3 = Mouse.IsOver(rect3);
                            if (flag3)
                            {
                                BackstoryDef adulthood = CEditor.API.Pawn.story.Adulthood;
                                this.tooltip2 = ((adulthood != null) ? adulthood.FullDescriptionFor(CEditor.API.Pawn).Resolve() : null);
                            }

                            Rect rect4 = rect3;
                            Action onClicked2 = new Action(this.AChangeAdulthoodUI);
                            Action onRandom2 = new Action(this.ARandomAdulthood);
                            Action onPrev2 = new Action(this.ASetPrevAdulthood);
                            Action onNext2 = new Action(this.ASetNextAdulthood);
                            Action onTextureClick2 = null;
                            Action onToggle2 = null;
                            TaggedString t2 = "Adulthood".Translate() + ": ";
                            BackstoryDef adulthood2 = CEditor.API.Pawn.story.Adulthood;
                            SZWidgets.NavSelectorImageBox(rect4, onClicked2, onRandom2, onPrev2, onNext2, onTextureClick2, onToggle2, t2 + ((adulthood2 != null) ? adulthood2.TitleCapFor(CEditor.API.Pawn.gender) : null), this.tooltip2, null, null, null, default(Color), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        bool devMode = Prefs.DevMode;
                        if (devMode)
                        {
                            Log.Error(ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }


                private void ARandomChildhood()
                {
                    this.RememberOldBackstory();
                    CEditor.API.Pawn.SetBackstory(true, true, true, false);
                    this.RecalcSkills();
                }


                private void ARandomAdulthood()
                {
                    this.RememberOldBackstory();
                    CEditor.API.Pawn.SetBackstory(true, true, false, false);
                    this.RecalcSkills();
                }


                private void ASetNextChildhood()
                {
                    this.RememberOldBackstory();
                    CEditor.API.Pawn.SetBackstory(true, false, true, false);
                    this.RecalcSkills();
                }


                private void ASetPrevChildhood()
                {
                    this.RememberOldBackstory();
                    CEditor.API.Pawn.SetBackstory(false, false, true, false);
                    this.RecalcSkills();
                }


                private void AChangeChildhoodUI()
                {
                    WindowTool.Open(new DialogChangeBackstory(true));
                }


                private void AChangeAdulthoodUI()
                {
                    WindowTool.Open(new DialogChangeBackstory(false));
                }


                private void ASetNextAdulthood()
                {
                    this.RememberOldBackstory();
                    CEditor.API.Pawn.SetBackstory(true, false, false, false);
                    this.RecalcSkills();
                }


                private void ASetPrevAdulthood()
                {
                    this.RememberOldBackstory();
                    CEditor.API.Pawn.SetBackstory(false, false, false, false);
                    this.RecalcSkills();
                }


                public void RememberOldBackstory()
                {
                    this.oBackAdult = CEditor.API.Pawn.story.GetBackstory(BackstorySlot.Adulthood);
                    this.oBackChild = CEditor.API.Pawn.story.GetBackstory(BackstorySlot.Childhood);
                }


                public void RecalcSkills()
                {
                    BackstoryDef backstory = CEditor.API.Pawn.story.GetBackstory(BackstorySlot.Adulthood);
                    BackstoryDef backstory2 = CEditor.API.Pawn.story.GetBackstory(BackstorySlot.Childhood);
                    using (List<SkillRecord>.Enumerator enumerator = CEditor.API.Pawn.skills.skills.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            SkillRecord skillRecord = enumerator.Current;
                            skillRecord.levelInt -= ((this.oBackAdult == null)
                                ? 0
                                : this.oBackAdult.skillGains.Sum(delegate(SkillGain x)
                                {
                                    if (x.skill != skillRecord.def)
                                    {
                                        return 0;
                                    }

                                    return x.amount;
                                }));
                            skillRecord.levelInt -= ((this.oBackChild == null)
                                ? 0
                                : this.oBackChild.skillGains.Sum(delegate(SkillGain x)
                                {
                                    if (x.skill != skillRecord.def)
                                    {
                                        return 0;
                                    }

                                    return x.amount;
                                }));
                            skillRecord.levelInt += ((backstory == null)
                                ? 0
                                : backstory.skillGains.Sum(delegate(SkillGain x)
                                {
                                    if (x.skill != skillRecord.def)
                                    {
                                        return 0;
                                    }

                                    return x.amount;
                                }));
                            skillRecord.levelInt += ((backstory2 == null)
                                ? 0
                                : backstory2.skillGains.Sum(delegate(SkillGain x)
                                {
                                    if (x.skill != skillRecord.def)
                                    {
                                        return 0;
                                    }

                                    return x.amount;
                                }));
                        }
                    }
                }


                private void DrawIncapableOf(int x, int y)
                {
                    try
                    {
                        bool flag = CEditor.API.Pawn.story != null;
                        if (flag)
                        {
                            Text.Font = GameFont.Medium;
                            Widgets.Label(new Rect((float)x, (float)y, 270f, 30f), "IncapableOf".Translate(CEditor.API.Pawn));
                            y += 30;
                            Text.Font = GameFont.Small;
                            Widgets.Label(new Rect((float)x, (float)y, 270f, 60f), CEditor.API.Pawn.GetIncapableOf(out this.ttip));
                            SZWidgets.ButtonInvisible(new Rect((float)x, (float)y, 270f, 60f), null, this.ttip);
                        }
                    }
                    catch (Exception ex)
                    {
                        bool devMode = Prefs.DevMode;
                        if (devMode)
                        {
                            Log.Error(ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }


                private void DrawFaction(int x, int y, int w)
                {
                    GUI.color = Color.white;
                    bool flag = CEditor.API.Pawn.Faction != null;
                    if (flag)
                    {
                        try
                        {
                            string text = CEditor.API.Pawn.Faction.Name.CapitalizeFirst().Colorize(CEditor.API.Pawn.Faction.Color);
                            Faction faction = CEditor.API.Pawn.Faction;
                            string text2;
                            if (faction == null)
                            {
                                text2 = null;
                            }
                            else
                            {
                                FactionDef def = faction.def;
                                text2 = ((def != null) ? def.factionIconPath : null);
                            }

                            string texPath = text2 ?? "bwhite";
                            Color facionColor = CEditor.API.Pawn.GetFacionColor();
                            string factionFullDesc = CEditor.API.Pawn.GetFactionFullDesc(null);
                            SZWidgets.ButtonTextureTextHighlight2(new Rect((float)x, (float)y, (float)w, 30f), text, texPath, facionColor, new Action(this.AChangeFaction), factionFullDesc, true);
                        }
                        catch (Exception ex)
                        {
                            bool devMode = Prefs.DevMode;
                            if (devMode)
                            {
                                Log.Error(ex.Message + "\n" + ex.StackTrace);
                            }
                        }
                    }
                    else
                    {
                        SZWidgets.ButtonTextureTextHighlight2(new Rect((float)x, (float)y, (float)w, 30f), Label.NONE, "bwhite", Color.white, new Action(this.AChangeFaction), "", true);
                    }
                }


                private void DrawIdeo(int x, int y, int w)
                {
                    Pawn pawn = CEditor.API.Pawn;
                    bool flag = ModsConfig.IdeologyActive && pawn.ideo != null;
                    if (flag)
                    {
                        Window windowOf = WindowTool.GetWindowOf<Dialog_ConfigureIdeo>();
                        bool flag2 = windowOf != null;
                        if (flag2)
                        {
                            this.selectedIdeo = IdeoUIUtility.selected;
                        }

                        string text = (pawn.Ideo != null) ? pawn.Ideo.name : "";
                        Texture2D icon = (pawn.Ideo != null) ? pawn.Ideo.Icon : Texture2D.whiteTexture;
                        Color color = (pawn.Ideo != null) ? pawn.Ideo.Color : Color.white;
                        Precept_Role precept_Role = (pawn.Ideo != null) ? pawn.Ideo.GetRole(CEditor.API.Pawn) : null;
                        string text2 = (precept_Role != null) ? precept_Role.GetTip() : ((pawn.Ideo != null) ? pawn.Ideo.description : "");
                        string pawnName = pawn.GetPawnName(false);
                        bool flag3 = !pawnName.NullOrEmpty() && pawn.ideo != null && pawn.Ideo != null && !pawn.Dead;
                        if (flag3)
                        {
                            string text3 = "CertaintyInIdeo".Translate(pawn.Named("PAWN"), pawn.Ideo.Named("IDEO")) + ": " + pawn.ideo.Certainty.ToStringPercent() + "\n\n" + "CertaintyChangePerDay".Translate() + ": " + pawn.ideo.CertaintyChangePerDay.ToStringPercent() + "\n\n";
                            bool flag4 = !text3.NullOrEmpty();
                            if (flag4)
                            {
                                text2 = text2 + "\n\n" + pawnName.Colorize(ColorTool.colBeige) + text3.Replace(pawnName, "");
                            }
                        }

                        SZWidgets.ButtonTextureTextHighlight(new Rect((float)x, (float)y, (float)(w + 100), 30f), text, icon, color, new Action(this.AChangeIdeo), text2);
                    }
                }


                private void DrawTitle(int x, int y, int w)
                {
                    bool flag = ModsConfig.RoyaltyActive && CEditor.API.Pawn.HasRoyaltyTracker();
                    if (flag)
                    {
                        try
                        {
                            SZWidgets.ButtonTextureTextHighlight2(new Rect((float)x, (float)y, (float)w, 30f), (this.currentTitle != null) ? this.currentTitle.Label : "", "UI/Buttons/Renounce", ColorTool.colBeige, null, Label.SETROYALTITLE, false);
                            SZWidgets.FloatMenuOnButtonInvisible<RoyalTitleDef>(new Rect((float)x, (float)y, (float)w, 30f), this.lOfRoyalTitles, (RoyalTitleDef rtitle) => (rtitle == null) ? Label.NONE : rtitle.GetLabelCapFor(CEditor.API.Pawn), new Action<RoyalTitleDef>(this.ASetTitle), Label.SETROYALTITLE);
                        }
                        catch (Exception ex)
                        {
                            bool devMode = Prefs.DevMode;
                            if (devMode)
                            {
                                Log.Error(ex.Message + "\n" + ex.StackTrace);
                            }
                        }
                    }
                }


                private void DrawExtendables(int x, int y, int w)
                {
                    int x2 = this.DrawFavorite(x, y, w);
                    try
                    {
                        x2 = this.DrawPersonality(x2, y, w);
                        x2 = this.DrawRJW(x2, y, w);
                    }
                    catch
                    {
                    }
                }


                private int DrawFavorite(int x, int y, int w)
                {
                    bool flag = !CEditor.API.Pawn.HasStoryTracker() || CEditor.API.Pawn.story.favoriteColor == null;
                    int result;
                    if (flag)
                    {
                        result = x;
                    }
                    else
                    {
                        SZWidgets.ButtonTextureTextHighlight2(new Rect((float)x, (float)y, (float)w, (float)w), "", "bfavcolor", CEditor.API.Pawn.story.favoriteColor.GetValueOrDefault(), new Action(this.AChangeFavColorUI), CompatibilityTool.GetFavoriteColorTooltip(CEditor.API.Pawn), true);
                        result = x + w + 2;
                    }

                    return result;
                }


                private int DrawRJW(int x, int y, int w)
                {
                    bool flag = !CEditor.IsRJWActive;
                    int result;
                    if (flag)
                    {
                        result = x;
                    }
                    else
                    {
                        ThingComp rjwcomp = CEditor.API.Pawn.GetRJWComp();
                        bool flag2 = rjwcomp == null;
                        if (flag2)
                        {
                            result = x;
                        }
                        else
                        {
                            SZWidgets.ButtonImage(new Rect((float)x, (float)y, (float)w, (float)w), "RJW-LOGO", new Action(this.AChangeRJW), CompatibilityTool.GetRJWTooltip(CEditor.API.Pawn));
                            result = x + w + 2;
                        }
                    }

                    return result;
                }


                private void AChangeRJW()
                {
                    CompatibilityTool.OpenRJWDialog(CEditor.API.Pawn);
                }


                private int DrawPersonality(int x, int y, int w)
                {
                    bool flag = !CEditor.IsPersonalitiesActive;
                    int result;
                    if (flag)
                    {
                        result = x;
                    }
                    else
                    {
                        ThingComp persoComp = CEditor.API.Pawn.GetPersoComp();
                        bool flag2 = persoComp == null;
                        if (flag2)
                        {
                            result = x;
                        }
                        else
                        {
                            SZWidgets.ButtonImage(new Rect((float)x, (float)y, (float)(w - 2), (float)(w - 2)), "bmemory", new Action(this.AChangePersonality), CompatibilityTool.GetPersonalitiesTooltip(CEditor.API.Pawn));
                            result = x + w + 2;
                        }
                    }

                    return result;
                }


                private void AChangePersonality()
                {
                    CompatibilityTool.OpenPersonalitiesDialog(CEditor.API.Pawn);
                }


                private void AChangeFaction()
                {
                    WindowTool.Open(new DialogChangeFaction());
                }


                private void AChangeIdeo()
                {
                    Type atype = Reflect.GetAType("RimWorld", "IdeoUIUtility");
                    atype.SetMemberValue("showAll", true);
                    atype.SetMemberValue("devEditMode", true);
                    CEditor.API.Get<CEditor.EditorUI>(EType.EditorUI).layer = WindowLayer.Dialog;
                    bool flag = CEditor.InStartingScreen && CEditor.API.Pawn.Ideo != null && CEditor.API.Pawn.Ideo.Fluid;
                    if (flag)
                    {
                        MessageTool.Show("Do not click on back button or you will need to restart your game. This is not a bug from the Editor, but because of the fluid ideo.", MessageTypeDefOf.CautionInput);
                    }

                    WindowTool.Open(new Dialog_ConfigureIdeo(new List<Pawn>
                    {
                        CEditor.API.Pawn
                    }, new Action(this.AOpenIdeoConfig), false));
                    IdeoUIUtility.SetSelected(CEditor.API.Pawn.Ideo);
                }


                private void AOpenIdeoConfig()
                {
                    bool flag = this.selectedIdeo != null;
                    if (flag)
                    {
                        CEditor.API.Pawn.ideo.SetIdeo(this.selectedIdeo);
                    }

                    Type atype = Reflect.GetAType("RimWorld", "IdeoUIUtility");
                    try
                    {
                        atype.SetMemberValue("showAll", false);
                        atype.SetMemberValue("devEditMode", false);
                    }
                    catch
                    {
                    }

                    CEditor.API.Get<CEditor.EditorUI>(EType.EditorUI).layer = CEditor.Layer;
                    MeditationFocusTypeAvailabilityCache.ClearFor(CEditor.API.Pawn);
                }


                private void AChangeFavColorUI()
                {
                    WindowTool.Open(new DialogColorPicker(ColorType.FavColor, true, null, null, null));
                }


                private void DrawTraits(int x, int y, int h)
                {
                    try
                    {
                        bool flag = CEditor.API.Pawn.story != null;
                        if (flag)
                        {
                            Text.Font = GameFont.Medium;
                            Widgets.Label(new Rect((float)x, (float)y, 200f, 30f), "Traits".Translate());
                            y += 3;
                            SZWidgets.ButtonImage((float)(x + 250), (float)y, 24f, 24f, "UI/Buttons/Dev/Add", new Action(this.AAddTrait), "", default(Color));
                            SZWidgets.ButtonImageCol((float)(x + 225), (float)y, 24f, 24f, "bminus", new Action<Color>(this.AToggleRemoveTrait), this.bRemoveTrait ? Color.red : Color.white, "");
                            SZWidgets.ButtonImage((float)(x + 200), (float)y, 18f, 24f, "UI/Buttons/Copy", new Action(this.ACopyTraits), "", default(Color));
                            bool flag2 = !this.lCopyTraits.NullOrEmpty<Trait>();
                            if (flag2)
                            {
                                SZWidgets.ButtonImage((float)(x + 180), (float)y, 18f, 24f, "UI/Buttons/Paste", new Action(this.APasteTraits), "", default(Color));
                            }

                            bool isRandom = CEditor.IsRandom;
                            if (isRandom)
                            {
                                SZWidgets.ButtonImage((float)(x + 153), (float)y, 25f, 25f, "brandom", new Action(this.ARandomTraits), "", default(Color));
                            }

                            y += 25;
                            bool flag3 = CEditor.API.Pawn.story.traits != null;
                            if (flag3)
                            {
                                SZWidgets.TraitListView((float)x, (float)y, 280f, (float)(h - y), CEditor.API.Pawn.story.traits.allTraits, ref this.scrollTraits, 25, new Action<Trait>(this.AOnTraitClick), new Action<Trait>(this.ARandomTrait), new Action<Trait>(this.APrevTrait), new Action<Trait>(this.ANextTrait), this.FTraitLabel, this.FTraitTooltip);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        bool devMode = Prefs.DevMode;
                        if (devMode)
                        {
                            Log.Error(ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }


                private void AOnTraitClick(Trait val)
                {
                    bool flag = this.bRemoveTrait;
                    if (flag)
                    {
                        CEditor.API.Pawn.RemoveTrait(val);
                    }
                    else
                    {
                        WindowTool.Open(new DialogAddTrait(val));
                    }
                }


                private void ARandomTrait(Trait val)
                {
                    List<KeyValuePair<TraitDef, TraitDegreeData>> list = TraitTool.ListOfTraitsKeyValuePair(null, null, null);
                    int index = list.IndexOf(new KeyValuePair<TraitDef, TraitDegreeData>(val.def, val.CurrentData));
                    index = list.NextOrPrevIndex(index, true, true);
                    KeyValuePair<TraitDef, TraitDegreeData> keyValuePair = list[index];
                    CEditor.API.Pawn.AddTrait(keyValuePair.Key, keyValuePair.Value, false, true, val);
                }


                private void ANextTrait(Trait val)
                {
                    List<KeyValuePair<TraitDef, TraitDegreeData>> list = TraitTool.ListOfTraitsKeyValuePair(null, null, null);
                    int index = list.IndexOf(new KeyValuePair<TraitDef, TraitDegreeData>(val.def, val.CurrentData));
                    index = list.NextOrPrevIndex(index, true, false);
                    KeyValuePair<TraitDef, TraitDegreeData> keyValuePair = list[index];
                    CEditor.API.Pawn.AddTrait(keyValuePair.Key, keyValuePair.Value, false, true, val);
                }


                private void APrevTrait(Trait val)
                {
                    List<KeyValuePair<TraitDef, TraitDegreeData>> list = TraitTool.ListOfTraitsKeyValuePair(null, null, null);
                    int index = list.IndexOf(new KeyValuePair<TraitDef, TraitDegreeData>(val.def, val.CurrentData));
                    index = list.NextOrPrevIndex(index, false, false);
                    KeyValuePair<TraitDef, TraitDegreeData> keyValuePair = list[index];
                    CEditor.API.Pawn.AddTrait(keyValuePair.Key, keyValuePair.Value, false, true, val);
                }


                private void ARandomTraits()
                {
                    CEditor.API.Pawn.story.traits.allTraits.Clear();
                    int num = CEditor.zufallswert.Next(0, 11);
                    for (int i = 0; i < num; i++)
                    {
                        CEditor.API.Pawn.AddTrait(null, null, true, true, null);
                    }
                }


                private void AAddTrait()
                {
                    WindowTool.Open(new DialogAddTrait(null));
                }


                private void AToggleRemoveTrait(Color col)
                {
                    this.bRemoveTrait = !this.bRemoveTrait;
                }


                private void ACopyTraits()
                {
                    this.lCopyTraits = new List<Trait>();
                    foreach (Trait item in CEditor.API.Pawn.story.traits.allTraits)
                    {
                        this.lCopyTraits.Add(item);
                    }
                }


                private void APasteTraits()
                {
                    bool flag = !this.lCopyTraits.NullOrEmpty<Trait>();
                    if (flag)
                    {
                        foreach (Trait trait in this.lCopyTraits)
                        {
                            CEditor.API.Pawn.story.traits.allTraits.Add(new Trait(trait.def, trait.Degree, false));
                        }

                        MeditationFocusTypeAvailabilityCache.ClearFor(CEditor.API.Pawn);
                    }
                }


                private int CURRENT_SKILL_PAGE = 0;
                private readonly int MAX_SKILLS_PER_PAGE = 12;

                private void ANextSkills()
                {
                    if (CEditor.API.Pawn == null || CEditor.API.Pawn.skills == null) return;
                    double maxPages = CEditor.API.Pawn.skills.skills.Count / (double)MAX_SKILLS_PER_PAGE;
                    var realMaxPage = maxPages % 1 > 0 ? ((int)maxPages + 1) : (int)maxPages;

                    if (CURRENT_SKILL_PAGE + 1 <= realMaxPage)
                        CURRENT_SKILL_PAGE += 1;
                }

                private void APrevSkills()
                {
                    if (CEditor.API.Pawn == null || CEditor.API.Pawn.skills == null) return;
                    if (CURRENT_SKILL_PAGE > 0)
                        CURRENT_SKILL_PAGE -= 1;
                }
                
                
                private static FieldInfo levelLabelWidthField = typeof(SkillUI).GetField("levelLabelWidth", BindingFlags.NonPublic | BindingFlags.Static);

                public static void DrawSkillsOf(Pawn p, List<SkillRecord> skills, Vector2 offset, SkillUI.SkillDrawMode mode, Rect container, int startIndex = 0, int maxIndex = -1)
                {
                    Verse.Text.Font = GameFont.Small;
                    if (p.DevelopmentalStage.Baby())
                    {
                        Color color = GUI.color;
                        GUI.color = Color.gray;
                        int anchor = (int)Verse.Text.Anchor;
                        Verse.Text.Anchor = TextAnchor.MiddleCenter;
                        Widgets.Label(new Rect(offset.x, offset.y, 230f, container.height), "SkillsDevelopLaterBaby".Translate());
                        GUI.color = color;
                        Verse.Text.Anchor = (TextAnchor)anchor;
                    }
                    else
                    {
                        float levelLabelWidth = -1;
                        for (int index = startIndex; index < maxIndex; ++index)
                        {
                            float x = Verse.Text.CalcSize(skills[index].def.skillLabel.CapitalizeFirst()).x;
                            if (x > levelLabelWidth)
                                levelLabelWidth = x;
                        }

                        try
                        {
                            // Set field width, this is private so try doing it via reflection :shrug:
                            levelLabelWidthField.SetValue(null, levelLabelWidth);
                        }
                        catch (Exception e)
                        {
                            Log.Message("Failed to set levelLabelWidth in SkillUI \n" + e);
                        }
                        for (int index = startIndex; index < maxIndex; ++index)
                        {
                
                            float y = (float)(index - startIndex) * 27f + offset.y;
                            SkillUI.DrawSkill(skills[index], new Vector2(offset.x, y), mode);
                        }
                    }
                }


                private void DrawSkills(int x, int y)
                {
                    try
                    {
                        if (CEditor.API.Pawn.skills != null)
                        {
                            var pawnSkills = CEditor.API.Pawn.getAllSkills();

                            int _tmp = (pawnSkills.Count() / MAX_SKILLS_PER_PAGE);
                            int maxPage = (int)_tmp + (_tmp - (int)_tmp > 0 ? 1 : 0);

                            int wantedIndex = Math.Max(0, CURRENT_SKILL_PAGE * MAX_SKILLS_PER_PAGE);
                            int startIndex = Math.Min(wantedIndex, pawnSkills.Count);
                            int maxIndex = Math.Min(startIndex + MAX_SKILLS_PER_PAGE, pawnSkills.Count);
                            Text.Font = GameFont.Medium;
                            Widgets.Label(new Rect((float)x, (float)y, 200f, 30f), "Skills".Translate());
                            y += 35;
                            BackstoryDef backstory = CEditor.API.Pawn.story.GetBackstory(BackstorySlot.Adulthood);
                            BackstoryDef backstory2 = CEditor.API.Pawn.story.GetBackstory(BackstorySlot.Childhood);
                            float num = (float)Reflect.GetAType("RimWorld", "SkillUI").GetMemberValue("levelLabelWidth");
                            float num2 = (234f - num - 35f) / 20f;

                            for (int i = startIndex; i < startIndex + MAX_SKILLS_PER_PAGE; i++)
                            {
                                try
                                {
                                    var skillRecord = pawnSkills[i];
                                    int num3 = (int)skillRecord.GetMemberConstValue("MaxLevel");
                                    num2 = (234f - num - 35f) / (float)num3;
                                    int num6 = (backstory == null) ? 0 : backstory.skillGains.Sum(t => (t.skill != skillRecord.def) ? 0 : t.amount);
                                    int num4 = (backstory2 == null) ? 0 : backstory2.skillGains.Sum(t => (t.skill != skillRecord.def) ? 0 : t.amount);
                                    int traitOffsetForSkill = CEditor.API.Pawn.GetTraitOffsetForSkill(skillRecord.def);
                                    int aptitude = skillRecord.Aptitude;
                                    int num5 = num6 + num4 + traitOffsetForSkill + aptitude;
                                    //    skillRecord.def.GetSkillIndex() -> i
                                    SZWidgets.LabelBackground(new Rect((float)x + num + 35f, (y + i * 27), num2 * (float)num5, 24f), "", (num5 < 0) ? new Color(0.5f, 0f, 0f, 0.25f) : new Color(0f, 0.5f, 0f, 0.15f), 0, "", default(Color));
                                }
                                catch (Exception)
                                {
                                }
                            }


                            try
                            {
                                DrawSkillsOf(CEditor.API.Pawn, pawnSkills, new Vector2((float)x, (float)y), SkillUI.SkillDrawMode.Menu, new Rect((float)x, (float)y, 200f, 30f), startIndex, maxIndex);
                            }
                            catch (Exception ex)
                            {
                                if (Prefs.DevMode)
                                {
                                    Log.Error(ex.Message + "\n" + ex.StackTrace);
                                }
                            }

                            for (int i = startIndex; i < maxIndex; i++)
                            {
                                try
                                {
                                    SkillRecord skillRecord = pawnSkills[i];
                                    float y3 = (float)(y + (i - startIndex) * 27);
                                    SZWidgets.ButtonInvisibleVar<SkillRecord>(new Rect((float)x, y3, num + 35f, 24f), new Action<SkillRecord>(this.ATogglePassion), skillRecord, "");
                                    if (this.iTickInputSkill > 0)
                                    {
                                        skillRecord.levelInt = SZWidgets.NumericTextField((float)x + num + 18f, y3, 40f, 24f, skillRecord.levelInt, 0, (int)skillRecord.GetMemberConstValue("MaxLevel"));
                                        this.iTickInputSkill--;
                                    }
                                    else
                                    {
                                        SZWidgets.ButtonInvisibleVar<SkillRecord>(new Rect((float)x + num + 40f, y3, (float)(x + 180) - ((float)x + num + 40f), 24f), new Action<SkillRecord>(this.ASetSkillNumeric), skillRecord, "");
                                    }

                                    SZWidgets.ButtonImageVar<SkillRecord>((float)(x + 210), y3, 24f, 24f, "UI/Buttons/Dev/Add", new Action<SkillRecord>(this.AAddSkillLevel), skillRecord, "");
                                    SZWidgets.ButtonImageVar<SkillRecord>((float)(x + 185), y3, 24f, 24f, "bminus", new Action<SkillRecord>(this.ASubSkillLevel), skillRecord, "");
                                    TooltipHandler.TipRegion(new Rect((float)x, y3, num + 80f, 24f), CEditor.API.Pawn.GetTooltipForSkillpoints(skillRecord));
                                }
                                catch (Exception)
                                {
                                }
                            }

                            SZWidgets.ButtonImage((float)(x + 210), (float)(y - 32), 18f, 24f, "UI/Buttons/Copy", new Action(this.ACopySkills), "", default(Color));
                            int XIndexNow = 190;
                            if (!this.lOfCopySkills.NullOrEmpty<SkillRecord>())
                            {
                                SZWidgets.ButtonImage((float)(x + 190), (float)(y - 32), 18f, 24f, "UI/Buttons/Paste", new Action(this.APasteSkills), "", default(Color));
                                XIndexNow -= 20;
                            }

                            if (CEditor.IsRandom)
                            {
                                SZWidgets.ButtonImage((float)(x + XIndexNow), (float)(y - 32), 18f, 24f, "brandom", new Action(this.ARandomSkills), "", default(Color));
                                XIndexNow -= 20;
                            }

                            //Log.Message("Testing size: " + (startIndex + MAX_SKILLS_PER_PAGE) + " < " + pawnSkills.Count);
                            if (startIndex + MAX_SKILLS_PER_PAGE < pawnSkills.Count)
                            {
                                SZWidgets.ButtonImage((float)(x + XIndexNow), (float)(y - 32), 18f, 24f, "bforward", new Action(this.ANextSkills), "Next Skills", default(Color));
                                XIndexNow -= 20;
                            }

                            if (startIndex > 0)
                            {
                                SZWidgets.ButtonImage((float)(x + XIndexNow), (float)(y - 32), 18f, 24f, "bbackward", new Action(this.APrevSkills), "Previous Skills", default(Color));
                                XIndexNow -= 20;
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        if (Prefs.DevMode)
                        {
                            Log.Error(ex2.Message + "\n" + ex2.StackTrace);
                        }
                    }
                }


                private void ACopySkills()
                {
                    this.lOfCopySkills = new List<SkillRecord>();
                    foreach (SkillRecord item in CEditor.API.Pawn.getAllSkills())
                    {
                        this.lOfCopySkills.Add(item);
                    }
                }


                private void APasteSkills()
                {
                    CEditor.API.Pawn.PasteSkills(this.lOfCopySkills);
                }


                private void ARandomSkills()
                {
                    foreach (SkillRecord skillRecord in CEditor.API.Pawn.skills.skills)
                    {
                        bool flag = !skillRecord.TotallyDisabled;
                        if (flag)
                        {
                            int maxValue = CEditor.zufallswert.Next(0, 21);
                            skillRecord.Level = CEditor.zufallswert.Next(0, maxValue);
                            skillRecord.passion = (Passion)CEditor.zufallswert.Next(0, 3);
                        }
                    }
                }


                

                private void ATogglePassion(SkillRecord record)
                {
                    var current = (int)record.passion;
                    if (current == (int)Passion.Major)
                        record.passion = (Passion)0;
                    else
                        record.passion = (Passion)(current + 1);
                }


                private void ASetSkillNumeric(SkillRecord record)
                {
                    this.iTickInputSkill = 9000;
                }


                private void AAddSkillLevel(SkillRecord record)
                {
                    record.levelInt++;
                }


                private void ASubSkillLevel(SkillRecord record)
                {
                    record.levelInt--;
                }


                private void DrawAbilities(int x, int y)
                {
                    try
                    {
                        bool flag = CEditor.API.Pawn.abilities != null;
                        if (flag)
                        {
                            Text.Font = GameFont.Medium;
                            Widgets.Label(new Rect((float)x, (float)(y - 2), 200f, 30f), Label.PSYTALENTE);
                            SZWidgets.ButtonImage((float)(x + 210), (float)y, 24f, 24f, "UI/Buttons/Dev/Add", new Action(this.AAddAbility), "", default(Color));
                            SZWidgets.ButtonImageCol((float)(x + 185), (float)y, 24f, 24f, "bminus", new Action<Color>(this.AToggleRemoveAbility), this.bRemoveAbility ? Color.red : Color.white, "");
                            SZWidgets.ButtonImage((float)(x + 165), (float)y, 18f, 24f, "UI/Buttons/Copy", new Action(this.ACopyAbilities), "", default(Color));
                            bool flag2 = !this.lCopyAbilities.NullOrEmpty<Ability>();
                            if (flag2)
                            {
                                SZWidgets.ButtonImage((float)(x + 145), (float)y, 18f, 24f, "UI/Buttons/Paste", new Action(this.APasteAbilities), "", default(Color));
                            }

                            bool isRandom = CEditor.IsRandom;
                            if (isRandom)
                            {
                                SZWidgets.ButtonImage((float)(x + 117), (float)y, 25f, 25f, "brandom", new Action(this.ARandomAbilities), "", default(Color));
                            }
                            else
                            {
                                bool flag3 = !CEditor.InStartingScreen && CEditor.API.Pawn.HasPsylink;
                                if (flag3)
                                {
                                    SZWidgets.ButtonImage((float)(x + 117), (float)y, 25f, 25f, "UI/Buttons/DragHash", new Action(this.ATogglePsyvalues), "", default(Color));
                                }
                            }

                            y += 30;
                            Text.Font = GameFont.Small;
                            GUI.color = Color.white;
                            List<Ability> abilities = CEditor.API.Pawn.abilities.abilities;
                            SZContainers.DrawElementStack<Ability>(new Rect((float)x, (float)y, 500f, 50f), abilities, this.bRemoveAbility, delegate(Ability abil) { CEditor.API.Pawn.abilities.RemoveAbility(abil.def); }, (Ability abil) => abil.def);
                        }
                    }
                    catch (Exception ex)
                    {
                        bool devMode = Prefs.DevMode;
                        if (devMode)
                        {
                            Log.Error(ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }


                private void ATogglePsyvalues()
                {
                    this.bShowPsyValues = !this.bShowPsyValues;
                }


                private void ARandomAbilities()
                {
                    CEditor.API.Pawn.abilities.abilities.Clear();
                    int num = CEditor.zufallswert.Next(0, 11);
                    List<AbilityDef> source = DefTool.ListByMod<AbilityDef>(null).ToList<AbilityDef>();
                    for (int i = 0; i < num; i++)
                    {
                        CEditor.API.Pawn.abilities.GainAbility(source.RandomElement<AbilityDef>());
                    }

                    CEditor.API.Pawn.abilities.Notify_TemporaryAbilitiesChanged();
                }


                private void AShowXenoType()
                {
                    WindowTool.Open(new DialogViewXenoGenes(CEditor.API.Pawn));
                }


                private void AConfigXenoType()
                {
                    WindowTool.Open(new DialogXenoType(CEditor.API.Pawn));
                }


                private void AAddAbility()
                {
                    WindowTool.Open(new DialogAddAbility());
                }


                private void AToggleRemoveAbility(Color col)
                {
                    this.bRemoveAbility = !this.bRemoveAbility;
                }


                private void ACopyAbilities()
                {
                    this.lCopyAbilities = new List<Ability>();
                    foreach (Ability item in CEditor.API.Pawn.abilities.abilities)
                    {
                        this.lCopyAbilities.Add(item);
                    }
                }


                private void APasteAbilities()
                {
                    bool flag = !this.lCopyAbilities.NullOrEmpty<Ability>();
                    if (flag)
                    {
                        foreach (Ability ability in this.lCopyAbilities)
                        {
                            CEditor.API.Pawn.abilities.GainAbility(ability.def);
                        }
                    }
                }


                private void DrawPsycasts(int x, int y)
                {
                    bool flag = !this.bShowPsyValues || CEditor.API.Pawn.psychicEntropy == null || CEditor.API.Pawn.psychicEntropy.Psylink == null;
                    if (!flag)
                    {
                        float entropyValue = CEditor.API.Pawn.psychicEntropy.EntropyValue;
                        float currentPsyfocus = CEditor.API.Pawn.psychicEntropy.CurrentPsyfocus;
                        float height = 100f;
                        Rect outRect = new Rect((float)x, (float)y, 250f, 100f);
                        Rect rect = new Rect(0f, 0f, outRect.width - 16f, height);
                        Widgets.BeginScrollView(outRect, ref this.scrollPosParam, rect, true);
                        Rect rect2 = rect.ContractedBy(4f);
                        rect2.y -= 4f;
                        rect2.height = height;
                        Listing_X listing_X = new Listing_X();
                        listing_X.Begin(rect2);
                        listing_X.verticalSpacing = 30f;
                        listing_X.AddSection(Label.ENTROPY, "", ref this.selectedParamName, ref entropyValue, 0f, (float)Math.Round((double)CEditor.API.Pawn.psychicEntropy.MaxEntropy), true, "");
                        listing_X.AddSection(Label.PSYFOCUS, "%", ref this.selectedParamName, ref currentPsyfocus, 0f, 1f, true, "");
                        listing_X.End();
                        Widgets.EndScrollView();
                        bool flag2 = entropyValue != CEditor.API.Pawn.psychicEntropy.EntropyValue;
                        if (flag2)
                        {
                            CEditor.API.Pawn.SetEntropy(entropyValue);
                        }

                        bool flag3 = currentPsyfocus != CEditor.API.Pawn.psychicEntropy.CurrentPsyfocus;
                        if (flag3)
                        {
                            CEditor.API.Pawn.SetPsyfocus(currentPsyfocus);
                        }
                    }
                }


                private void DrawTraining(int x, int y, int w, int h)
                {
                    try
                    {
                        if (CEditor.API.Pawn.RaceProps.Animal && CEditor.API.Pawn.Faction != null && CEditor.API.Pawn.training != null)
                        {
                            GUI.color = Color.white;
                            if (CEditor.API.Pawn.training.HasLearned(TrainableDefOf.Obedience))
                            {
                                Widgets.Label(new Rect((float)x, (float)y, 100f, (float)h), "Master".Translate() + ":");
                                SZWidgets.FloatMenuOnButtonText<Pawn>(new Rect((float)(x + 80), (float)(y - 5), 120f, 30f), this.selectedTrainer.GetPawnName(false), this.lOfColonists, (Pawn p) => p.GetPawnName(false), new Action<Pawn>(this.ASelectMaster), "");
                            }

                            y += 30;
                            Widgets.Label(new Rect((float)x, (float)y, 300f, 30f), CEditor.API.Pawn.GetTrainabilityLabel());
                            y += 30;
                            Widgets.Label(new Rect((float)x, (float)y, 300f, 30f), CEditor.API.Pawn.GetWildnessLabel());
                            y += 30;
                            foreach (TrainableDef trainableDef in DefDatabase<TrainableDef>.AllDefs)
                            {
                                bool wanted = CEditor.API.Pawn.training.GetWanted(trainableDef);
                                bool flag3 = wanted;
                                Rect rect = new Rect((float)x, (float)y, 150f, 30f);
                                Widgets.DrawHighlightIfMouseover(rect);
                                AcceptanceReport canTrain = CEditor.API.Pawn.training.CanAssignToTrain(trainableDef);
                                this.DoTrainableTooltip(rect, CEditor.API.Pawn, trainableDef, canTrain);
                                GUI.color = (canTrain.Accepted ? Color.white : Color.gray);
                                Widgets.CheckboxLabeled(rect, trainableDef.label, ref flag3, false, null, null, false, false);
                                if (flag3 != wanted)
                                {
                                    CEditor.API.Pawn.training.SetWantedRecursive(trainableDef, flag3);
                                }

                                int trainingSteps = this.GetTrainingSteps(trainableDef);
                                Widgets.Label(new Rect((float)(x + 160), (float)y, 30f, 30f), trainingSteps.ToString() + " / " + trainableDef.steps.ToString());
                                if (trainingSteps < trainableDef.steps)
                                {
                                    SZWidgets.ButtonImageVar<TrainableDef>((float)(x + 200), (float)y, 24f, 24f, "UI/Buttons/Dev/Add", new Action<TrainableDef>(this.ATrain), trainableDef, "");
                                }

                                y += 30;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + "\n" + ex.StackTrace);
                    }
                }


                private void DoTrainableTooltip(Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain)
                {
                    TooltipHandler.TipRegion(rect, delegate()
                    {
                        string text = td.LabelCap + "\n\n" + td.description;
                        bool flag = !canTrain.Accepted;
                        if (flag)
                        {
                            text = text + "\n\n" + canTrain.Reason;
                        }
                        else
                        {
                            bool flag2 = !td.prerequisites.NullOrEmpty<TrainableDef>();
                            if (flag2)
                            {
                                text += "\n";
                                for (int i = 0; i < td.prerequisites.Count; i++)
                                {
                                    bool flag3 = !pawn.training.HasLearned(td.prerequisites[i]);
                                    if (flag3)
                                    {
                                        text = text + "\n" + "TrainingNeedsPrerequisite".Translate().Formatted(td.prerequisites[i].LabelCap);
                                    }
                                }
                            }
                        }

                        return text;
                    }, (int)(rect.y * 612f + rect.x));
                }


                private int GetTrainingSteps(TrainableDef td)
                {
                    return (int)CEditor.API.Pawn.training.CallMethod("GetSteps", new object[]
                    {
                        td
                    });
                }


                private void ASelectMaster(Pawn p)
                {
                    this.selectedTrainer = p;
                    CEditor.API.Pawn.playerSettings.Master = p;
                }


                private void ATrain(TrainableDef trainableDef)
                {
                    CEditor.API.Pawn.training.Train(trainableDef, null, false);
                }


                private void DrawXeno(int x, int y, int w)
                {
                    bool flag = !CEditor.API.Pawn.HasStoryTracker() || !ModsConfig.BiotechActive;
                    if (!flag)
                    {
                        SZWidgets.ButtonTextureTextHighlight(new Rect((float)x, (float)y, 30f, 30f), "", CEditor.API.Pawn.genes.XenotypeIcon, Color.white, new Action(this.AShowXenoType), "ViewGenes".Translate());
                        x = x + 30 + 2;
                        SZWidgets.ButtonTextureTextHighlight2(new Rect((float)x, (float)y, (float)w, 30f), CEditor.API.Pawn.genes.XenotypeLabelCap, null, Color.white, new Action(this.AConfigXenoType), "XenotypeEditor".Translate() + "\n\n" + CEditor.API.Pawn.genes.XenotypeDescShort, true);
                    }
                }


                private void DrawCapsule(int x, int y, int w, int h)
                {
                    bool inStartingScreen = CEditor.InStartingScreen;
                    if (inStartingScreen)
                    {
                        SZWidgets.ButtonImage((float)x, (float)y, (float)w, (float)h, "bcapsule", new Action(this.ACapsuleUI), "", default(Color));
                    }
                }


                private void ACapsuleUI()
                {
                    WindowTool.Open(new DialogCapsuleUI());
                }


                private int iTickInputName;


                internal int iTickInputAge;


                private int iTickInputSkill;


                private int iChronoAge;


                private int iAge;


                private string ageBuffer;


                private string chronoAgeBuffer;


                private string ttip;


                private string selectedParamName = "";


                private Regex regexInt;


                private Vector2 scrollPosParam = default(Vector2);


                private bool bRemoveAbility;


                private bool bRemoveTrait;


                private bool bShowPsyValues = false;


                private Vector2 scrollTraits;


                private List<Trait> lCopyTraits;


                private List<Ability> lCopyAbilities;


                private List<SkillRecord> lOfCopySkills;


                private List<Pawn> lOfColonists;


                private RoyalTitle currentTitle;


                private BackstoryDef oBackAdult;


                private BackstoryDef oBackChild;


                private HashSet<RoyalTitleDef> lOfRoyalTitles;


                private Pawn selectedTrainer;


                private Ideo selectedIdeo = null;


                private string uniqueID;


                private Func<Trait, string> FTraitLabel = delegate(Trait t)
                {
                    string result;
                    if ((result = t.LabelCap) == null)
                    {
                        result = (t.Label ?? t.def.label);
                    }

                    return result;
                };


                private Func<Trait, Trait, bool> FTraitComparator = (Trait tA, Trait tB) => tA == tB;


                private Func<Trait, string> FTraitTooltip;


                private string tooltip1 = "";


                private string tooltip2 = "";
            }


            private class BlockHealth
            {
                private bool ShowHidden
                {
                    get { return (bool)typeof(HealthCardUtility).GetMemberValue("showAllHediffs"); }
                    set
                    {
                        typeof(HealthCardUtility).SetMemberValue("showAllHediffs", value);
                        this.bShowHidden = value;
                    }
                }


                internal BlockHealth()
                {
                    this.scrollH = 0f;
                    this.bShowHidden = false;
                    this.bHighlight = true;
                    this.bShowRemove = false;
                    this.lCopyHealth = new List<Hediff>();
                    this.texBleeding = ContentFinder<Texture2D>.Get("UI/Icons/Medical/Bleeding", false);
                }


                internal void Draw(CEditor.EditorUI.coord c)
                {
                    int x = c.x;
                    int y = c.y;
                    int w = c.w;
                    int h = c.h;
                    bool flag = CEditor.API.Pawn == null;
                    if (!flag)
                    {
                        this.DrawHealth(new Rect((float)(x + 5), (float)(y + 25), (float)w, (float)(h - 60)));
                        this.DrawUpper(x, y, w, h);
                        this.DrawLower(x + w, y + h - 30, w, h);
                    }
                }


                private void DrawHealth(Rect outRect)
                {
                    outRect = outRect.Rounded();
                    Rect rect = new Rect(outRect.x, outRect.y, outRect.width * 0.375f, outRect.height).Rounded();
                    Rect rect2 = new Rect(rect.xMax, outRect.y, outRect.width - rect.width, outRect.height);
                    rect.yMin += 11f;
                    try
                    {
                        HealthCardUtility.DrawHealthSummary(rect, CEditor.API.Pawn, false, CEditor.API.Pawn);
                    }
                    catch
                    {
                    }

                    this.DrawHediffListing(rect2.ContractedBy(10f), CEditor.API.Pawn, true);
                }


                private void DrawHediffListing(Rect rect, Pawn pawn, bool showBloodLoss)
                {
                    GUI.color = Color.white;
                    GUI.BeginGroup(rect);
                    float lineHeight = Text.LineHeight;
                    Rect outRect = new Rect(0f, 0f, rect.width, rect.height - lineHeight);
                    Rect viewRect = new Rect(0f, 0f, rect.width - 16f, this.scrollH);
                    Rect rect2 = rect;
                    bool flag = viewRect.height > outRect.height;
                    if (flag)
                    {
                        rect2.width -= 16f;
                    }

                    Widgets.BeginScrollView(outRect, ref this.scrollPos, viewRect, true);
                    try
                    {
                        GUI.color = Color.white;
                        float num = 0f;
                        this.bHighlight = true;
                        bool flag2 = false;
                        foreach (IGrouping<BodyPartRecord, Hediff> diffs in this.VisibleHediffGroupsInOrder(pawn, showBloodLoss))
                        {
                            try
                            {
                                flag2 = true;
                                this.DrawHediffRow(rect2, pawn, diffs, ref num);
                            }
                            catch
                            {
                            }
                        }

                        bool flag3 = !flag2;
                        if (flag3)
                        {
                            Widgets.NoneLabelCenteredVertically(new Rect(0f, 0f, viewRect.width, outRect.height), "(" + "NoHealthConditions".Translate() + ")");
                            num = outRect.height - 1f;
                        }

                        bool flag4 = Event.current.type == EventType.Layout;
                        if (flag4)
                        {
                            this.scrollH = num;
                        }
                    }
                    catch
                    {
                        MessageTool.Show("health conditions are malformed -> consider to use 'full heal'", null);
                    }

                    Widgets.EndScrollView();
                    try
                    {
                        float bleedRateTotal = pawn.health.hediffSet.BleedRateTotal;
                        bool flag5 = bleedRateTotal > 0.01f;
                        if (flag5)
                        {
                            Rect rect3 = new Rect(0f, rect.height - lineHeight, rect.width, lineHeight);
                            string text = string.Concat(new string[]
                            {
                                "BleedingRate".Translate(),
                                ": ",
                                bleedRateTotal.ToStringPercent(),
                                " / ",
                                "Day".Translate()
                            });
                            int num2 = HealthUtility.TicksUntilDeathDueToBloodLoss(pawn);
                            string str = "TimeToDeath".Translate().Formatted(num2.ToStringTicksToPeriod(true, false, true, true, false));
                            bool flag6 = num2 < 60000;
                            if (flag6)
                            {
                                text = text + " (" + str + ")";
                            }
                            else
                            {
                                text = text + " (" + "WontBleedOutSoon".Translate() + ")";
                            }

                            Widgets.Label(rect3, text);
                        }

                        GUI.EndGroup();
                    }
                    catch
                    {
                        pawn.health.hediffSet.Clear();
                    }

                    GUI.color = Color.white;
                }


                private IEnumerable<IGrouping<BodyPartRecord, Hediff>> VisibleHediffGroupsInOrder(Pawn pawn, bool showBloodLoss)
                {
                    return from x in this.VisibleHediffs(pawn, showBloodLoss) group x by x.Part into x orderby this.GetListPriority(x.First<Hediff>().Part) descending select x;
                }


                private IEnumerable<Hediff> VisibleHediffs(Pawn pawn, bool showBloodLoss)
                {
                    ;
                    if (!this.bShowHidden)
                    {
                        List<Hediff_MissingPart> mpca = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
                        for (int i = 0; i < mpca.Count; ++i)
                            yield return mpca[i];
                        IEnumerable<Hediff> visibleDiffs = pawn.health.hediffSet.hediffs.Where(d =>
                        {
                            if (d is Hediff_MissingPart || !d.Visible)
                                return false;
                            return showBloodLoss || d.def != HediffDefOf.BloodLoss;
                        });
                        foreach (Hediff hediff in visibleDiffs)
                        {
                            Hediff diff = hediff;
                            yield return diff;
                        }
                    }
                    else
                    {
                        foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                        {
                            Hediff diff2 = hediff;
                            yield return diff2;
                        }
                    }
                }


                private float GetListPriority(BodyPartRecord rec)
                {
                    bool flag = rec == null;
                    float result;
                    if (flag)
                    {
                        result = 9999999f;
                    }
                    else
                    {
                        result = (float)((int)rec.height * 10000) + rec.coverageAbsWithChildren;
                    }

                    return result;
                }


                private void DrawHediffRow(Rect rect, Pawn pawn, IEnumerable<Hediff> diffs, ref float curY)
                {
                    float num = rect.width * 0.375f;
                    float width = rect.width - num - this.lastMaxIconsTotalWidth;
                    BodyPartRecord part = diffs.First<Hediff>().Part;
                    float a = (part != null) ? Text.CalcHeight(part.LabelCap, num) : Text.CalcHeight("WholeBody".Translate(), num);
                    float b = 0f;
                    float num2 = curY;
                    float num3 = 0f;
                    foreach (IGrouping<int, Hediff> source in from x in diffs
                             group x by x.UIGroupKey)
                    {
                        int num4 = source.Count<Hediff>();
                        string text = source.First<Hediff>().LabelCap;
                        bool flag = num4 != 1;
                        if (flag)
                        {
                            text = text + " x" + num4.ToString();
                        }

                        num3 += Text.CalcHeight(text, width);
                    }

                    b = num3;
                    Rect rect2 = new Rect(0f, curY, rect.width, Mathf.Max(a, b));
                    this.DoRightRowHighlight(rect2);
                    bool flag2 = part != null;
                    if (flag2)
                    {
                        GUI.color = HealthUtility.GetPartConditionLabel(pawn, part).Second;
                        Widgets.Label(new Rect(0f, curY, num, 100f), part.LabelCap);
                    }
                    else
                    {
                        GUI.color = HealthUtility.RedColor;
                        Widgets.Label(new Rect(0f, curY, num, 100f), "WholeBody".Translate());
                    }

                    GUI.color = Color.white;
                    foreach (IGrouping<int, Hediff> grouping in from x in diffs
                             group x by x.UIGroupKey)
                    {
                        int num5 = 0;
                        Hediff hediff4 = null;
                        Texture2D bleedingIcon = null;
                        TextureAndColor stateIcon = null;
                        float totalBleedRate = 0f;
                        foreach (Hediff hediff2 in grouping)
                        {
                            bool flag3 = num5 == 0;
                            if (flag3)
                            {
                                hediff4 = hediff2;
                            }

                            stateIcon = hediff2.StateIcon;
                            bool bleeding = hediff2.Bleeding;
                            if (bleeding)
                            {
                                bleedingIcon = ContentFinder<Texture2D>.Get("UI/Icons/Medical/Bleeding", true);
                            }

                            totalBleedRate += hediff2.BleedRate;
                            num5++;
                        }

                        string text2 = hediff4.LabelCap;
                        bool flag4 = num5 != 1;
                        if (flag4)
                        {
                            text2 = text2 + " x" + num5.ToStringCached();
                        }

                        float num6 = Text.CalcHeight(text2, width);
                        Rect rect3 = new Rect(num, curY, width, num6);
                        Rect rect4 = new Rect(rect3.x, rect3.y, rect3.width - 30f, rect3.height);
                        Widgets.DrawHighlightIfMouseover(rect4);
                        GUI.color = hediff4.LabelColor;
                        Widgets.Label(rect3, text2);
                        SZWidgets.ButtonInvisibleVar<Hediff>(rect4, new Action<Hediff>(this.AEditHediff), hediff4, "");
                        GUI.color = Color.white;
                        Rect iconsRect = new Rect(rect3.x + 10f, rect3.y, (float)((double)rect.width - (double)num - 10.0), rect3.height);
                        bool flag5 = this.bShowRemove;
                        if (flag5)
                        {
                            SZWidgets.ButtonImageVar<Hediff>(rect3.x + rect3.width - 30f, rect3.y, 24f, 24f, "UI/Buttons/Delete", new Action<Hediff>(this.BRemoveHediff), hediff4, "");
                        }

                        List<GenUI.AnonymousStackElement> list = new List<GenUI.AnonymousStackElement>();
                        Hediff localHediff = hediff4;
                        list.Add(new GenUI.AnonymousStackElement
                        {
                            drawer = delegate(Rect r)
                            {
                                r = new Rect(iconsRect.x + (float)((double)iconsRect.width - ((double)r.x - (double)iconsRect.x) - 20.0), r.y, 20f, 20f);
                                Widgets.InfoCardButton(r, localHediff.def);
                            },
                            width = 20f
                        });
                        bool flag6 = bleedingIcon;
                        if (flag6)
                        {
                            list.Add(new GenUI.AnonymousStackElement
                            {
                                drawer = delegate(Rect r)
                                {
                                    r = new Rect(iconsRect.x + (float)((double)iconsRect.width - ((double)r.x - (double)iconsRect.x) - 20.0), r.y, 20f, 20f);
                                    GUI.DrawTexture(r.ContractedBy(GenMath.LerpDouble(0f, 0.6f, 5f, 0f, Mathf.Min(totalBleedRate, 1f))), bleedingIcon);
                                },
                                width = 20f
                            });
                        }

                        bool hasValue = stateIcon.HasValue;
                        if (hasValue)
                        {
                            list.Add(new GenUI.AnonymousStackElement
                            {
                                drawer = delegate(Rect r)
                                {
                                    r = new Rect(iconsRect.x + (float)((double)iconsRect.width - ((double)r.x - (double)iconsRect.x) - 20.0), r.y, 20f, 20f);
                                    GUI.color = stateIcon.Color;
                                    GUI.DrawTexture(r, stateIcon.Texture);
                                    GUI.color = Color.white;
                                },
                                width = 20f
                            });
                        }

                        GenUI.DrawElementStack<GenUI.AnonymousStackElement>(iconsRect, num6, list, delegate(Rect r, GenUI.AnonymousStackElement obj) { obj.drawer(r); }, (GenUI.AnonymousStackElement obj) => obj.width, 4f, 5f, true);
                        this.lastMaxIconsTotalWidth = Mathf.Max(this.lastMaxIconsTotalWidth, list.Sum((GenUI.AnonymousStackElement x) => x.width + 5f) - 5f);
                        bool flag7 = Mouse.IsOver(rect3);
                        if (flag7)
                        {
                            int num7 = 0;
                            foreach (Hediff hediff3 in grouping)
                            {
                                Hediff hediff = hediff3;
                                TooltipHandler.TipRegion(rect3, new TipSignal(() => hediff.GetTooltip(pawn, Prefs.DevMode), (int)curY + 7857 + num7++, TooltipPriority.Default));
                            }

                            bool flag8 = part != null;
                            if (flag8)
                            {
                                string str = (string)typeof(HealthCardUtility).CallMethod("GetTooltip", new object[]
                                {
                                    pawn,
                                    part
                                });
                                TooltipHandler.TipRegion(rect3, str);
                            }
                        }

                        bool flag9 = Widgets.ButtonInvisible(rect3, false);
                        if (flag9)
                        {
                            typeof(HealthCardUtility).CallMethod("EntryClicked", new object[]
                            {
                                diffs,
                                pawn
                            });
                        }

                        curY += num6;
                    }

                    GUI.color = Color.white;
                    curY = num2 + Mathf.Max(a, b);
                    this.OnClickOrHover(rect2, diffs, pawn, part);
                }


                private void OnClickOrHover(Rect rect, IEnumerable<Hediff> diffs, Pawn pawn, BodyPartRecord part)
                {
                    bool flag = Widgets.ButtonInvisible(rect, false);
                    if (flag)
                    {
                        typeof(HealthCardUtility).CallMethod("EntryClicked", new object[]
                        {
                            diffs,
                            pawn
                        });
                    }

                    string str = (string)typeof(HealthCardUtility).CallMethod("GetTooltip", new object[]
                    {
                        pawn,
                        part
                    });
                    TooltipHandler.TipRegion(rect, str);
                }


                private void DoRightRowHighlight(Rect rowRect)
                {
                    bool flag = this.bHighlight;
                    if (flag)
                    {
                        GUI.color = CEditor.EditorUI.BlockHealth.StaticHighlightColor;
                        GUI.DrawTexture(rowRect, TexUI.HighlightTex);
                    }

                    this.bHighlight = !this.bHighlight;
                }


                private void DrawUpper(int x, int y, int w, int h)
                {
                    y += 5;
                    Widgets.DrawBoxSolid(new Rect((float)(x + 5), (float)y, 850f, 31f), CEditor.EditorUI.BlockHealth.backgr);
                    Widgets.DrawBoxSolid(new Rect((float)(x + 5), (float)(y + 31), 250f, 1f), Color.gray);
                    SZWidgets.CheckBoxOnChange(new Rect((float)(x + 176), (float)y, 135f, 25f), Label.SHOW_HIDDEN, this.bShowHidden, new Action<bool>(this.AHiddenChanged));
                    SZWidgets.ButtonImage((float)(x + w - 30), (float)y, 24f, 24f, "UI/Buttons/Dev/Add", new Action(this.AAddHediff), "", default(Color));
                    SZWidgets.ButtonImage((float)(x + w - 55), (float)y, 24f, 24f, "bminus", new Action(this.ARemoveHediff), "", default(Color));
                    SZWidgets.ButtonImage((float)(x + w - 80), (float)y, 18f, 24f, "UI/Buttons/Copy", new Action(this.ACopyHealth), "", default(Color));
                    bool flag = !this.lCopyHealth.NullOrEmpty<Hediff>();
                    if (flag)
                    {
                        SZWidgets.ButtonImage((float)(x + w - 100), (float)y, 18f, 24f, "UI/Buttons/Paste", new Action(this.APasteHealth), "", default(Color));
                    }

                    bool isRandom = CEditor.IsRandom;
                    if (isRandom)
                    {
                        SZWidgets.ButtonImage((float)(x + w - 130), (float)y, 25f, 25f, "brandom", new Action(this.ARandomHealth), "", default(Color));
                    }
                }


                private void BRemoveHediff(Hediff hediff)
                {
                    CEditor.API.Pawn.RemoveHediff(hediff);
                    CEditor.API.UpdateGraphics();
                }


                private void ARandomHealth()
                {
                    int num = CEditor.zufallswert.Next(1, 11);
                    for (int i = 0; i < num; i++)
                    {
                        CEditor.API.Pawn.AddHediff2(true, null, 1f, null, false, -1, -1, -1, null);
                    }
                }


                private void AHiddenChanged(bool val)
                {
                    this.ShowHidden = val;
                }


                private void AEditHediff(Hediff val)
                {
                    WindowTool.Open(new DialogAddHediff(val));
                }


                private void AAddHediff()
                {
                    WindowTool.Open(new DialogAddHediff(null));
                }


                private void ARemoveHediff()
                {
                    this.bShowRemove = !this.bShowRemove;
                }


                private void ACopyHealth()
                {
                    this.lCopyHealth = new List<Hediff>();
                    foreach (Hediff item in CEditor.API.Pawn.health.hediffSet.hediffs)
                    {
                        this.lCopyHealth.Add(item);
                    }
                }


                private void APasteHealth()
                {
                    bool flag = !this.lCopyHealth.NullOrEmpty<Hediff>();
                    if (flag)
                    {
                        foreach (Hediff hediff in this.lCopyHealth)
                        {
                            BodyPartRecord bodyPart = HealthTool.GetBodyPart(CEditor.API.Pawn, hediff);
                            bool flag2 = (hediff.Part != null && bodyPart != null) || (hediff.Part == null && bodyPart == null);
                            if (flag2)
                            {
                                CEditor.API.Pawn.AddHediff2(false, hediff.def, hediff.Severity, bodyPart, hediff.IsPermanent(), hediff.GetLevel(), hediff.GetPainValue(), hediff.GetDuration(), hediff.GetOtherPawn());
                            }
                        }

                        CEditor.API.UpdateGraphics();
                    }
                }


                private void DrawLower(int x, int y, int w, int h)
                {
                    bool dead = CEditor.API.Pawn.Dead;
                    if (dead)
                    {
                        SZWidgets.ButtonText((float)(x - 130), (float)y, 130f, 30f, Label.RESURRECT, new Action(this.ARessurect), "");
                    }
                    else
                    {
                        SZWidgets.ButtonText((float)(x - 130), (float)y, 130f, 30f, Label.FULLHEAL, new Action(this.AFullHeal), Label.TIP_HEAL);
                        SZWidgets.ButtonText((float)(x - 260), (float)y, 130f, 30f, Label.MEDICATE, new Action(this.AMedicate), "");
                        SZWidgets.ButtonText((float)(x - 390), (float)y, 130f, 30f, Label.ANAESTHETIZE, new Action(this.AAnaesthetize), "");
                        SZWidgets.ButtonText((float)(x - 520), (float)y, 130f, 30f, Label.HURT, new Action(this.AHurt), Label.TIP_HURT);
                    }
                }


                private void ARessurect()
                {
                    ResurrectionUtility.TryResurrect(CEditor.API.Pawn);
                    bool flag = !CEditor.API.Pawn.Spawned;
                    if (flag)
                    {
                        CEditor.API.Pawn.SpawnPawn(null, CEditor.API.Pawn.Position);
                    }

                    CEditor.API.UpdateGraphics();
                }


                private void AFullHeal()
                {
                    bool alt = Event.current.alt;
                    if (alt)
                    {
                        HealthUtility.HealNonPermanentInjuriesAndRestoreLegs(CEditor.API.Pawn);
                    }
                    else
                    {
                        WindowTool.Open(new DialogFullheal());
                    }

                    CEditor.API.UpdateGraphics();
                }


                private void AMedicate()
                {
                    CEditor.API.Pawn.Medicate();
                    CEditor.API.UpdateGraphics();
                }


                private void AAnaesthetize()
                {
                    CEditor.API.Pawn.Anaesthetize();
                    CEditor.API.UpdateGraphics();
                }


                private void AHurt()
                {
                    bool flag = Event.current.alt || CEditor.InStartingScreen;
                    if (flag)
                    {
                        CEditor.API.Pawn.Hurt();
                    }
                    else
                    {
                        CEditor.API.Pawn.DamageUntilDeath();
                    }

                    CEditor.API.UpdateGraphics();
                }


                static BlockHealth()
                {
                }


                private static readonly Color StaticHighlightColor = new Color(0.75f, 0.75f, 0.85f, 1f);


                private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);


                private static readonly Color backgr = new Color(0.0823f, 0.098f, 0.113f);


                private Vector2 scrollPos;


                private float scrollH;


                private bool bShowHidden;


                private bool bHighlight;


                private bool bShowRemove;


                private List<Hediff> lCopyHealth;


                private Texture2D texBleeding;


                private float lastMaxIconsTotalWidth = 20f;
            }


            private class BlockInfo
            {
                internal BlockInfo()
                {
                    this.uniqueID = null;
                    this.lfe = new List<Faction>();
                }


                internal void Draw(CEditor.EditorUI.coord c)
                {
                    int x = c.x;
                    int y = c.y;
                    int w = c.w;
                    int h = c.h;
                    bool flag = CEditor.API.Pawn == null;
                    if (!flag)
                    {
                        bool flag2 = this.uniqueID != CEditor.API.Pawn.ThingID;
                        if (flag2)
                        {
                            this.uniqueID = CEditor.API.Pawn.ThingID;
                            this.lfe = CEditor.API.Pawn.FactionEnemies();
                            bool flag3 = CEditor.API.Pawn.Dead && (CEditor.API.Pawn.Corpse == null || !CEditor.API.Pawn.Corpse.Spawned || CEditor.API.Pawn.Corpse.Discarded);
                            if (flag3)
                            {
                                return;
                            }

                            StatsReportUtility.Notify_QuickSearchChanged();
                        }

                        try
                        {
                            bool dead = CEditor.API.Pawn.Dead;
                            if (dead)
                            {
                                StatsReportUtility.DrawStatsReport(new Rect((float)(x + 10), (float)y, (float)(w - 10), (float)(h - 30)), CEditor.API.Pawn.Corpse);
                            }
                            else
                            {
                                StatsReportUtility.DrawStatsReport(new Rect((float)(x + 10), (float)y, (float)(w - 10), (float)(h - 30)), CEditor.API.Pawn);
                            }
                        }
                        catch
                        {
                        }

                        this.DrawEnemies(x, y, w, h);
                    }
                }


                private void DrawEnemies(int x, int y, int w, int h)
                {
                    bool flag = CEditor.API.Pawn == null;
                    if (!flag)
                    {
                        try
                        {
                            int num = this.lfe.CountAllowNull<Faction>();
                            int num2 = x + w;
                            int num3 = x + w - 80 - this.lfe.Count * 22;
                            num3 = ((num3 < x) ? x : num3);
                            y = h;
                            Widgets.Label(new Rect((float)num3, (float)y, 80f, 30f), "EnemyOf".Translate());
                            SZWidgets.ButtonInvisible(new Rect((float)num3, (float)y, (float)w, 20f), new Action(this.AOpenFactionCard), "");
                            num3 += 80;
                            foreach (Faction faction in this.lfe)
                            {
                                Rect rect = new Rect((float)num3, (float)y, 20f, 20f);
                                bool flag2 = Mouse.IsOver(rect);
                                string toolTip;
                                if (flag2)
                                {
                                    toolTip = CEditor.API.Pawn.GetFactionFullDesc(faction);
                                }
                                else
                                {
                                    toolTip = "";
                                }

                                SZWidgets.ButtonImageCol2<Faction>(rect, faction.def.factionIconPath ?? "bwhite", new Action<Faction>(this.ASetRelation), faction, faction.GetFacionColor(), toolTip);
                                num3 += 22;
                                bool flag3 = num3 > num2;
                                if (flag3)
                                {
                                    break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }


                private void ASetRelation(Faction f)
                {
                }


                private void AOpenFactionCard()
                {
                }


                private string uniqueID;


                private List<Faction> lfe;
            }


            private class BlockInventory
            {
                private Rect GetCopyRect(Rect rBase, float y)
                {
                    return new Rect(rBase.x + rBase.width - 20f, rBase.y + y, 18f, 24f);
                }


                private Rect GetPasteRect(Rect rBase, float y)
                {
                    return new Rect(rBase.x + rBase.width - 43f, rBase.y + y, 18f, 24f);
                }


                internal BlockInventory()
                {
                    this.fCount = 0f;
                    this.scrollH = 0f;
                    this.workingInvList = new List<Thing>();
                    this.lOfCopyOutfits = new List<Apparel>();
                    this.lOfCopyItems = new List<Thing>();
                    this.lOfCopyWeapons = new List<ThingWithComps>();
                }


                internal void Draw(CEditor.EditorUI.coord c)
                {
                    int x = c.x;
                    int y = c.y;
                    int w = c.w;
                    int h = c.h;
                    bool flag = CEditor.API.Pawn == null;
                    if (!flag)
                    {
                        this.DrawInvData(new Rect((float)(x + 10), (float)(y + 10), (float)(w - 10), (float)(h - 40)));
                        this.DrawLowerButtons(x + w, y + h - 30);
                    }
                }


                private void DrawInventory(Rect rView)
                {
                    this.fCount += 20f;
                    SZWidgets.ButtonImage(this.GetCopyRect(rView, this.fCount), "UI/Buttons/Copy", new Action(this.ACopyInv), "");
                    bool flag = !this.lOfCopyItems.NullOrEmpty<Thing>();
                    if (flag)
                    {
                        SZWidgets.ButtonImage(this.GetPasteRect(rView, this.fCount), "UI/Buttons/Paste", new Action(this.APasteInv), "");
                    }

                    Widgets.ListSeparator(ref this.fCount, rView.width, "Inventory".Translate());
                    this.workingInvList.Clear();
                    this.workingInvList.AddRange(CEditor.API.Pawn.inventory.innerContainer);
                    for (int i = 0; i < this.workingInvList.Count; i++)
                    {
                        this.DrawThingRow(ref this.fCount, rView.width, this.workingInvList[i], DialogType.Object);
                    }

                    this.workingInvList.Clear();
                    bool flag2 = Event.current.type == EventType.Layout;
                    if (flag2)
                    {
                        this.scrollH = this.fCount + 30f;
                    }
                }


                private void DrawApparel(Rect rView)
                {
                    this.fCount += 20f;
                    SZWidgets.ButtonImage(this.GetCopyRect(rView, this.fCount), "UI/Buttons/Copy", new Action(this.ACopyApparel), "");
                    bool flag = !this.lOfCopyOutfits.NullOrEmpty<Apparel>();
                    if (flag)
                    {
                        SZWidgets.ButtonImage(this.GetPasteRect(rView, this.fCount), "UI/Buttons/Paste", new Action(this.APasteApparel), "");
                    }

                    Widgets.ListSeparator(ref this.fCount, rView.width, "Apparel".Translate());
                    Pawn_ApparelTracker apparel = CEditor.API.Pawn.apparel;
                    bool flag2 = apparel != null && apparel.WornApparelCount > 0;
                    if (flag2)
                    {
                        try
                        {
                            List<Apparel> list = (from ap in CEditor.API.Pawn.apparel.WornApparel
                                orderby ap.def.apparel.bodyPartGroups[0].listOrder descending
                                select ap).ToList<Apparel>();
                            for (int i = 0; i < list.Count; i++)
                            {
                                this.DrawThingRow(ref this.fCount, rView.width, list[i], DialogType.Apparel);
                            }
                        }
                        catch
                        {
                            List<Apparel> list2 = CEditor.API.Pawn.apparel.WornApparel.ToList<Apparel>();
                            foreach (Apparel apparel2 in list2)
                            {
                                bool flag3 = !CEditor.API.Pawn.ApparelGraphicTest(apparel2, true, true);
                                if (flag3)
                                {
                                    CEditor.API.Pawn.TransferToInventory(apparel2);
                                }
                            }

                            CEditor.API.UpdateGraphics();
                        }
                    }
                }


                private void ARandomThing(Thing t, DialogType type)
                {
                    bool flag = type == DialogType.Apparel;
                    if (flag)
                    {
                        CEditor.API.Pawn.ReplaceAndWearRandomApparel(t as Apparel, null, false);
                    }
                    else
                    {
                        bool flag2 = type == DialogType.Weapon;
                        if (flag2)
                        {
                            CEditor.API.Pawn.Reequip(null, CEditor.API.Pawn.IsPrimaryWeapon(t as ThingWithComps) ? 0 : 1, false);
                        }
                        else
                        {
                            CEditor.API.Pawn.ReplaceItem(t);
                        }
                    }

                    CEditor.API.UpdateGraphics();
                }


                private void DrawEquipment(Rect rView)
                {
                    this.fCount += 20f;
                    SZWidgets.ButtonImage(this.GetCopyRect(rView, this.fCount), "UI/Buttons/Copy", new Action(this.ACopyWeapon), "");
                    bool flag = !this.lOfCopyWeapons.NullOrEmpty<ThingWithComps>();
                    if (flag)
                    {
                        SZWidgets.ButtonImage(this.GetPasteRect(rView, this.fCount), "UI/Buttons/Paste", new Action(this.APasteWeapon), "");
                    }

                    Widgets.ListSeparator(ref this.fCount, rView.width, "Equipment".Translate());
                    Pawn pawn = CEditor.API.Pawn;
                    bool flag2;
                    if (pawn == null)
                    {
                        flag2 = false;
                    }
                    else
                    {
                        Pawn_EquipmentTracker equipment = pawn.equipment;
                        bool? flag3 = (equipment != null) ? new bool?(equipment.AllEquipmentListForReading.NullOrEmpty<ThingWithComps>()) : null;
                        bool flag4 = false;
                        flag2 = (flag3.GetValueOrDefault() == flag4 & flag3 != null);
                    }

                    bool flag5 = flag2;
                    if (flag5)
                    {
                        List<ThingWithComps> allEquipmentListForReading = CEditor.API.Pawn.equipment.AllEquipmentListForReading;
                        for (int i = 0; i < allEquipmentListForReading.Count; i++)
                        {
                            this.DrawThingRow(ref this.fCount, rView.width, CEditor.API.Pawn.equipment.AllEquipmentListForReading[i], DialogType.Weapon);
                        }
                    }
                }


                private void ARandomWeapon(int i)
                {
                    CEditor.API.Pawn.Reequip(null, i, false);
                }


                private void DrawArmorRating(Rect rView)
                {
                    this.fCount += 20f;
                    SZWidgets.ButtonImage(this.GetCopyRect(rView, this.fCount), "UI/Buttons/Copy", new Action(this.ACopyAll), "");
                    bool flag = !this.lOfCopyOutfits.NullOrEmpty<Apparel>();
                    if (flag)
                    {
                        SZWidgets.ButtonImage(this.GetPasteRect(rView, this.fCount), "UI/Buttons/Paste", new Action(this.APasteAll), "");
                    }

                    Widgets.ListSeparator(ref this.fCount, rView.width, "OverallArmor".Translate());
                    this.TryDrawOverallArmor(ref this.fCount, rView.width, StatDefOf.ArmorRating_Sharp, "ArmorSharp".Translate());
                    this.TryDrawOverallArmor(ref this.fCount, rView.width, StatDefOf.ArmorRating_Blunt, "ArmorBlunt".Translate());
                    this.TryDrawOverallArmor(ref this.fCount, rView.width, StatDefOf.ArmorRating_Heat, "ArmorHeat".Translate());
                }


                private void DrawCarryAndComfy(Rect rView)
                {
                    this.fCount = 0f;
                    this.TryDrawMassInfo(ref this.fCount, rView.width);
                    this.TryDrawComfyTemperatureRange(ref this.fCount, rView.width);
                }


                private void DrawInvData(Rect rect)
                {
                    GUI.BeginGroup(rect);
                    Text.Font = GameFont.Small;
                    GUI.color = Color.white;
                    Rect outRect = new Rect(0f, 0f, rect.width, rect.height);
                    Rect rect2 = new Rect(0f, 0f, rect.width - 16f, this.scrollH);
                    Widgets.BeginScrollView(outRect, ref this.scrollPos, rect2, true);
                    this.DrawCarryAndComfy(rect2);
                    this.DrawArmorRating(rect2);
                    this.DrawEquipment(rect2);
                    this.DrawApparel(rect2);
                    this.DrawInventory(rect2);
                    Widgets.EndScrollView();
                    GUI.EndGroup();
                    Text.Anchor = TextAnchor.UpperLeft;
                }


                private void ACopyAll()
                {
                    this.lOfCopyOutfits = CEditor.API.Pawn.ListOfCopyOutfits();
                    this.lOfCopyItems = CEditor.API.Pawn.ListOfCopyItems();
                    this.lOfCopyWeapons = CEditor.API.Pawn.ListOfCopyWeapons();
                }


                private void APasteAll()
                {
                    CEditor.API.Pawn.PasteCopyOutfits(this.lOfCopyOutfits);
                    CEditor.API.Pawn.PasteCopyItems(this.lOfCopyItems);
                    CEditor.API.Pawn.PasteCopyWeapons(this.lOfCopyWeapons);
                }


                private void ACopyWeapon()
                {
                    this.lOfCopyWeapons = CEditor.API.Pawn.ListOfCopyWeapons();
                }


                private void APasteWeapon()
                {
                    CEditor.API.Pawn.PasteCopyWeapons(this.lOfCopyWeapons);
                }


                private void ACopyApparel()
                {
                    this.lOfCopyOutfits = CEditor.API.Pawn.ListOfCopyOutfits();
                }


                private void APasteApparel()
                {
                    CEditor.API.Pawn.PasteCopyOutfits(this.lOfCopyOutfits);
                }


                private void ACopyInv()
                {
                    this.lOfCopyItems = CEditor.API.Pawn.ListOfCopyItems();
                }


                private void APasteInv()
                {
                    CEditor.API.Pawn.PasteCopyItems(this.lOfCopyItems);
                }


                private void DrawLowerButtons(int x, int y)
                {
                    int num = 120;
                    SZWidgets.ButtonText((float)(x - num), (float)y, 120f, 30f, Label.UNDRESS, new Action(this.AUndress), Label.TIP_UNDRESS);
                    num += 120;
                    SZWidgets.ButtonText((float)(x - num), (float)y, 120f, 30f, Label.REDRESS, new Action(this.ARedress), Label.TIP_REDRESS);
                    num += 120;
                    SZWidgets.ButtonText((float)(x - num), (float)y, 120f, 30f, Label.REEQUIP, new Action(this.AReequip), Label.TIP_REEQUIP);
                    num += 120;
                    SZWidgets.ButtonText((float)(x - num), (float)y, 120f, 30f, Label.REINVENT, new Action(this.AReinvent), Label.TIP_REINVENT);
                    num += 120;
                    SZWidgets.ButtonText((float)(x - num), (float)y, 120f, 30f, "Equipment".Translate() + " +", new Action(this.AAddGun), Label.TIP_ADD_EQUIP);
                    num += 120;
                    SZWidgets.ButtonText((float)(x - num), (float)y, 120f, 30f, "Apparel".Translate() + " +", new Action(this.AAddApparel), Label.TIP_ADD_APPAREL);
                    num += 120;
                    SZWidgets.ButtonText((float)(x - num), (float)y, 120f, 30f, "Inventory".Translate() + " +", new Action(this.AAddItem), Label.TIP_ADD_ITEM);
                }


                private void AUndress()
                {
                    Pawn pawn = CEditor.API.Pawn;
                    List<Apparel> list;
                    if (pawn == null)
                    {
                        list = null;
                    }
                    else
                    {
                        Pawn_ApparelTracker apparel = pawn.apparel;
                        list = ((apparel != null) ? apparel.WornApparel : null);
                    }

                    List<Apparel> list2 = list;
                    bool flag = !list2.NullOrEmpty<Apparel>();
                    if (flag)
                    {
                        int index = list2.IndexOf(list2.RandomElement<Apparel>());
                        bool alt = Event.current.alt;
                        if (alt)
                        {
                            CEditor.API.Pawn.apparel.WornApparel[index].Destroy(DestroyMode.Vanish);
                        }
                        else
                        {
                            bool inStartingScreen = CEditor.InStartingScreen;
                            if (inStartingScreen)
                            {
                                CEditor.API.Pawn.MoveDressToInv(CEditor.API.Pawn.apparel.WornApparel[index].def.apparel.layers.LastOrDefault<ApparelLayerDef>());
                            }
                            else
                            {
                                CEditor.API.Pawn.apparel.TryDrop(list2[index]);
                            }
                        }

                        CEditor.API.UpdateGraphics();
                    }
                }


                private void ARedress()
                {
                    CEditor.API.Pawn.Redress(null, true, -1, true);
                    CEditor.API.UpdateGraphics();
                }


                private void AReequip()
                {
                    CEditor.API.Pawn.Reequip(null, -1, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AReinvent()
                {
                    CEditor.API.Pawn.Reinvent(null, CEditor.zufallswert.Next(1, 10));
                    CEditor.API.UpdateGraphics();
                }


                private void AAddGun()
                {
                    WindowTool.Open(new DialogObjects(DialogType.Weapon, null, null, false));
                }


                private void AAddApparel()
                {
                    WindowTool.Open(new DialogObjects(DialogType.Apparel, null, null, false));
                }


                private void AAddItem()
                {
                    WindowTool.Open(new DialogObjects(DialogType.Object, null, null, false));
                }


                private void InterfaceDrop(Thing t, bool shift)
                {
                    ThingWithComps thingWithComps = t as ThingWithComps;
                    Apparel apparel = t as Apparel;
                    bool flag = apparel != null && CEditor.API.Pawn.apparel != null && CEditor.API.Pawn.apparel.WornApparel.Contains(apparel);
                    if (flag)
                    {
                        if (shift)
                        {
                            CEditor.API.Pawn.TransferToInventory(apparel);
                        }
                        else
                        {
                            bool flag2 = CEditor.InStartingScreen || Event.current.alt;
                            if (flag2)
                            {
                                CEditor.API.Pawn.TransferToInventory(apparel);
                                CEditor.API.Pawn.DestroyApparel(apparel);
                            }
                            else
                            {
                                CEditor.API.Pawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.RemoveApparel, apparel), new JobTag?(JobTag.Misc), false);
                            }
                        }
                    }
                    else
                    {
                        bool flag3 = thingWithComps != null && CEditor.API.Pawn.equipment != null && CEditor.API.Pawn.IsEquippedByPawn(thingWithComps);
                        if (flag3)
                        {
                            if (shift)
                            {
                                CEditor.API.Pawn.TransferToInventory(thingWithComps);
                            }
                            else
                            {
                                bool flag4 = CEditor.InStartingScreen || Event.current.alt;
                                if (flag4)
                                {
                                    CEditor.API.Pawn.TransferToInventory(thingWithComps);
                                    CEditor.API.Pawn.DestroyEquipment(thingWithComps);
                                }
                                else
                                {
                                    CEditor.API.Pawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.DropEquipment, thingWithComps), new JobTag?(JobTag.Misc), false);
                                }
                            }
                        }
                        else if (shift)
                        {
                            bool isApparel = t.def.IsApparel;
                            if (isApparel)
                            {
                                bool flag5 = !CEditor.API.Pawn.apparel.CanWearWithoutDroppingAnything(t.def);
                                if (flag5)
                                {
                                    CEditor.API.Pawn.MoveDressToInv(t.def.apparel.LastLayer);
                                }
                            }

                            CEditor.API.Pawn.TransferFromInventory(t);
                        }
                        else
                        {
                            bool flag6 = CEditor.InStartingScreen || Event.current.alt;
                            if (flag6)
                            {
                                CEditor.API.Pawn.DestroyItem(t);
                            }
                            else
                            {
                                bool destroyOnDrop = t.def.destroyOnDrop;
                                if (destroyOnDrop)
                                {
                                    CEditor.API.Pawn.inventory.innerContainer.Remove(t);
                                }
                                else
                                {
                                    Thing thing;
                                    CEditor.API.Pawn.inventory.innerContainer.TryDrop(t, CEditor.API.Pawn.Position, CEditor.API.Pawn.Map, ThingPlaceMode.Near, out thing, null, null);
                                }
                            }
                        }
                    }
                }


                private void InterfaceIngest(Thing t)
                {
                    Job job = new Job(JobDefOf.Ingest, t);
                    job.count = Mathf.Min(t.stackCount, t.def.ingestible.maxNumToIngestAtOnce);
                    job.count = Mathf.Min(job.count, FoodUtility.WillIngestStackCountOf(CEditor.API.Pawn, t.def, t.GetStatValue(StatDefOf.Nutrition, true, -1)));
                    CEditor.API.Pawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
                }


                private void AShowInfo(Thing thing)
                {
                    WindowTool.Open(new Dialog_InfoCard(thing, null));
                }


                private void ADropOrDestroy(Thing thing)
                {
                    SoundTool.PlayThis(SoundDefOf.Tick_High);
                    this.InterfaceDrop(thing, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AShift(Thing thing)
                {
                    SoundTool.PlayThis(SoundDefOf.Tick_High);
                    this.InterfaceDrop(thing, true);
                    CEditor.API.UpdateGraphics();
                }


                private void AIngestThing(Thing thing)
                {
                    SoundTool.PlayThis(SoundDefOf.Tick_High);
                    this.InterfaceIngest(thing);
                    CEditor.API.UpdateGraphics();
                }


                private string GetThingLabel(Thing thing)
                {
                    string text = thing.LabelCap;
                    Apparel apparel = thing as Apparel;
                    bool flag = apparel != null && CEditor.API.Pawn.outfits != null && CEditor.API.Pawn.outfits.forcedHandler.IsForced(apparel);
                    if (flag)
                    {
                        text += ", " + "ApparelForcedLower".Translate();
                    }

                    return text;
                }


                private void DrawThingRow(ref float y, float width, Thing thing, DialogType type)
                {
                    try
                    {
                        Rect rect = new Rect(0f, y, width, 28f);
                        Text.Anchor = TextAnchor.MiddleLeft;
                        GUI.color = ITab_Pawn_Gear.ThingLabelColor;
                        Text.WordWrap = false;
                        Widgets.Label(new Rect(36f, y, rect.width - 36f, rect.height), this.GetThingLabel(thing));
                        Text.WordWrap = true;
                        SZWidgets.ButtonImageVar<Thing>(rect.width - 28f, y, 28f, 28f, "UI/Buttons/InfoButton", new Action<Thing>(this.AShowInfo), thing, "DefInfoTip".Translate());
                        rect.width -= 28f;
                        SZWidgets.ButtonImageVar<Thing>(rect.width - 28f, y, 28f, 28f, "UI/Buttons/Drop", new Action<Thing>(this.ADropOrDestroy), thing, "DropThing".Translate() + Label.TIP_DESTROYDROP);
                        rect.width -= 28f;
                        SZWidgets.ButtonImageVar<Thing>(rect.width - 28f, y, 28f, 28f, "bpfeil", new Action<Thing>(this.AShift), thing, "");
                        rect.width -= 28f;
                        bool isRandom = CEditor.IsRandom;
                        if (isRandom)
                        {
                            bool flag = Widgets.ButtonImage(new Rect(rect.width - 28f, y, 28f, 28f), ContentFinder<Texture2D>.Get("brandom", true), true);
                            if (flag)
                            {
                                this.ARandomThing(thing, type);
                            }

                            rect.width -= 28f;
                        }

                        try
                        {
                            bool flag2 = (thing.def.IsNutritionGivingIngestible || thing.def.IsNonMedicalDrug) && thing.IngestibleNow && CEditor.API.Pawn.RaceProps.CanEverEat(thing);
                            if (flag2)
                            {
                                SZWidgets.ButtonImageVar<Thing>(rect.width - 28f, y, 28f, 28f, "UI/Buttons/Ingest", new Action<Thing>(this.AIngestThing), thing, "");
                                rect.width -= 28f;
                            }
                        }
                        catch
                        {
                        }

                        Rect rect2 = rect;
                        rect2.xMin = rect2.xMax - 60f;
                        CaravanThingsTabUtility.DrawMass(thing, rect2);
                        rect.width -= 60f;
                        bool flag3 = Mouse.IsOver(rect);
                        if (flag3)
                        {
                            GUI.color = ITab_Pawn_Gear.HighlightColor;
                            GUI.DrawTexture(rect, TexUI.HighlightTex);
                            SZWidgets.ButtonInvisible(rect, delegate { WindowTool.Open(new DialogObjects(type, null, (ThingWithComps)thing, false)); }, "");
                        }

                        Widgets.ThingIcon(new Rect(4f, y, 32f, 32f), thing, 1f, null, false);
                        this.DoThingToolTip(rect, thing);
                        y += 32f;
                    }
                    catch
                    {
                    }
                }


                private void DoThingToolTip(Rect rect, Thing thing)
                {
                    string text = thing.DescriptionDetailed;
                    bool useHitPoints = thing.def.useHitPoints;
                    if (useHitPoints)
                    {
                        text = string.Concat(new string[]
                        {
                            text,
                            "\n",
                            thing.HitPoints.ToString(),
                            " / ",
                            thing.MaxHitPoints.ToString()
                        });
                    }

                    TooltipHandler.TipRegion(rect, text);
                }


                private void TryDrawComfyTemperatureRange(ref float curY, float width)
                {
                    bool dead = CEditor.API.Pawn.Dead;
                    if (!dead)
                    {
                        Rect rect = new Rect(0f, curY, width, 22f);
                        float statValue = CEditor.API.Pawn.GetStatValue(StatDefOf.ComfyTemperatureMin, true, -1);
                        float statValue2 = CEditor.API.Pawn.GetStatValue(StatDefOf.ComfyTemperatureMax, true, -1);
                        Widgets.Label(rect, string.Concat(new string[]
                        {
                            "ComfyTemperatureRange".Translate(),
                            ": ",
                            statValue.ToStringTemperature("F0"),
                            " ~ ",
                            statValue2.ToStringTemperature("F0")
                        }));
                        curY += 22f;
                    }
                }


                private void TryDrawMassInfo(ref float curY, float width)
                {
                    bool dead = CEditor.API.Pawn.Dead;
                    if (!dead)
                    {
                        try
                        {
                            Rect rect = new Rect(0f, curY, width, 22f);
                            float num = (float)Math.Round((double)MassUtility.GearAndInventoryMass(CEditor.API.Pawn), 2);
                            float num2 = (float)Math.Round((double)MassUtility.Capacity(CEditor.API.Pawn, null), 2);
                            string label = "MassCarried".Translate(new NamedArgument(num, "0.##"), new NamedArgument(num2, "0.##"));
                            Widgets.Label(rect, label);
                        }
                        catch
                        {
                        }

                        curY += 22f;
                    }
                }


                private void TryDrawOverallArmor(ref float curY, float width, StatDef stat, string label)
                {
                    float num = 0f;
                    float num2 = Mathf.Clamp01(CEditor.API.Pawn.GetStatValue(stat, true, -1) / 2f);
                    List<BodyPartRecord> allParts = CEditor.API.Pawn.RaceProps.body.AllParts;
                    Pawn_ApparelTracker apparel = CEditor.API.Pawn.apparel;
                    List<Apparel> list = (apparel != null) ? apparel.WornApparel : null;
                    for (int i = 0; i < allParts.Count; i++)
                    {
                        float num3 = 1f - num2;
                        bool flag = list != null;
                        if (flag)
                        {
                            for (int j = 0; j < list.Count; j++)
                            {
                                bool flag2 = list[j].def.apparel.CoversBodyPart(allParts[i]);
                                if (flag2)
                                {
                                    float num4 = Mathf.Clamp01(list[j].GetStatValue(stat, true, -1) / 2f);
                                    num3 *= 1f - num4;
                                }
                            }
                        }

                        num += allParts[i].coverageAbs * (1f - num3);
                    }

                    num = Mathf.Clamp(num * 2f, 0f, 2f);
                    Rect rect = new Rect(0f, curY, width, 100f);
                    Widgets.Label(rect, label.Truncate(120f, null));
                    rect.xMin += 120f;
                    Widgets.Label(rect, num.ToStringPercent());
                    curY += 22f;
                }


                private Vector2 scrollPos;


                private float fCount;


                private float scrollH;


                private const float ICONH = 32f;


                private const float BUTTONH = 28f;


                private List<Thing> workingInvList;


                private List<Apparel> lOfCopyOutfits;


                private List<Thing> lOfCopyItems;


                private List<ThingWithComps> lOfCopyWeapons;
            }


            private class BlockLog
            {
                internal BlockLog()
                {
                    this.wLog = 0f;
                    this.hLog = 0f;
                    this.yLine = 0f;
                    this.LogInit();
                }


                private void LogInit()
                {
                    this.logCachedDisplayLastTick = -1;
                    this.logCachedPlayLastTick = -1;
                    this.logCachedDisplay = null;
                    this.logData = new ITab_Pawn_Log_Utility.LogDrawData();
                    this.logSeek = null;
                    Pawn pawn = CEditor.API.Pawn;
                    this.uniqueID = ((pawn != null) ? pawn.ThingID : null);
                }


                internal void Draw(CEditor.EditorUI.coord c)
                {
                    int x = c.x;
                    int y = c.y;
                    int w = c.w;
                    int h = c.h;
                    bool flag = CEditor.API.Pawn == null;
                    if (!flag)
                    {
                        bool flag2 = this.uniqueID != CEditor.API.Pawn.ThingID;
                        if (flag2)
                        {
                            this.LogInit();
                        }

                        bool flag3 = this.logCachedDisplay == null || this.logCachedDisplayLastTick != CEditor.API.Pawn.records.LastBattleTick || this.logCachedPlayLastTick != Find.PlayLog.LastTick;
                        if (flag3)
                        {
                            this.logCachedDisplay = ITab_Pawn_Log_Utility.GenerateLogLinesFor(CEditor.API.Pawn, true, true, true, 1000).ToList<ITab_Pawn_Log_Utility.LogLineDisplayable>();
                            this.logCachedDisplayLastTick = CEditor.API.Pawn.records.LastBattleTick;
                            this.logCachedPlayLastTick = Find.PlayLog.LastTick;
                        }

                        Rect outRect = new Rect((float)(x + 10), (float)(y + 5), (float)(w - 10), (float)(h - 5));
                        this.wLog = outRect.width - 26f;
                        this.hLog = 0f;
                        foreach (ITab_Pawn_Log_Utility.LogLineDisplayable logLineDisplayable in this.logCachedDisplay)
                        {
                            bool flag4 = logLineDisplayable.Matches(this.logSeek);
                            if (flag4)
                            {
                                this.scrollPos.y = this.hLog - (logLineDisplayable.GetHeight(this.wLog) + outRect.height) / 2f;
                            }

                            this.hLog += logLineDisplayable.GetHeight(this.wLog);
                        }

                        this.logSeek = null;
                        bool flag5 = this.hLog > 0f;
                        if (flag5)
                        {
                            Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, this.hLog);
                            this.logData.StartNewDraw();
                            Widgets.BeginScrollView(outRect, ref this.scrollPos, viewRect, true);
                            this.yLine = 0f;
                            foreach (ITab_Pawn_Log_Utility.LogLineDisplayable logLineDisplayable2 in this.logCachedDisplay)
                            {
                                logLineDisplayable2.Draw(this.yLine, this.wLog, this.logData);
                                this.yLine += logLineDisplayable2.GetHeight(this.wLog);
                            }

                            Widgets.EndScrollView();
                        }
                        else
                        {
                            Text.Anchor = TextAnchor.MiddleCenter;
                            Text.Font = GameFont.Medium;
                            GUI.color = Color.grey;
                            Widgets.Label(new Rect(outRect.x, 0f, outRect.width, outRect.height), "(" + "NoRecentEntries".Translate() + ")");
                            Text.Anchor = TextAnchor.UpperLeft;
                            GUI.color = Color.white;
                        }
                    }
                }


                private Vector2 scrollPos;


                private float wLog;


                private float hLog;


                private float yLine;


                private int logCachedDisplayLastTick;


                private int logCachedPlayLastTick;


                private List<ITab_Pawn_Log_Utility.LogLineDisplayable> logCachedDisplay;


                private ITab_Pawn_Log_Utility.LogDrawData logData;


                private LogEntry logSeek;


                private string uniqueID;
            }


            private class BlockNeeds
            {
                internal BlockNeeds()
                {
                    this.wNeedBar = 0;
                    this.hNeedBar = 0;
                    this.yStartMemories = 0;
                }


                internal void Draw(CEditor.EditorUI.coord c)
                {
                    int x = c.x;
                    int y = c.y;
                    int w = c.w;
                    int h = c.h;
                    bool flag = !CEditor.API.Pawn.HasNeedsTracker() || CEditor.API.Pawn.Dead;
                    if (!flag)
                    {
                        this.yStartMemories = this.DrawNeedBars(x, y, w, h);
                        this.yStartMemories = ((CEditor.API.Pawn.needs.AllNeeds.Count > 12) ? this.yStartMemories : (y + 80));
                        this.DrawMemories(x + this.wNeedBar + 30, this.yStartMemories, this.wNeedBar + 225, h - this.yStartMemories - 36);
                        this.DrawThoughtButtons(x + w - 50, this.yStartMemories, w, h);
                        this.DrawLowerButtons(x, y + h - 30, w, h);
                    }
                }


                private int DrawNeedBars(int x, int y, int w, int h)
                {
                    this.wNeedBar = w / 3;
                    this.hNeedBar = h / 11;
                    bool flag = !CEditor.InStartingScreen || !CEditor.IsAVPActive;
                    if (flag)
                    {
                        foreach (Need need in CEditor.API.Pawn.needs.AllNeeds)
                        {
                            bool flag2 = need.def.defName == "Mood";
                            if (flag2)
                            {
                                GUI.color = (((double)need.CurLevelPercentage < 0.2) ? (GUI.color = Color.red) : Color.white);
                                try
                                {
                                    need.DrawOnGUI(new Rect((float)(x + this.wNeedBar), (float)y, (float)(this.wNeedBar * 2), (float)this.hNeedBar), int.MaxValue, -1f, true, true, null, true);
                                }
                                catch
                                {
                                }

                                SZWidgets.ButtonImageVar<Need>((float)(x + this.wNeedBar + 11), (float)(y + 30), 24f, 24f, "bbackward", new Action<Need>(this.ASubNeed), need, "");
                                SZWidgets.ButtonImageVar<Need>((float)(x + this.wNeedBar * 3 - 36), (float)(y + 30), 24f, 24f, "bforward", new Action<Need>(this.AAddNeed), need, "");
                            }
                        }
                    }

                    int num = 0;
                    foreach (Need need2 in CEditor.API.Pawn.needs.AllNeeds)
                    {
                        bool flag3 = need2.def.defName != "Mood";
                        if (flag3)
                        {
                            bool flag4 = num == 11;
                            if (flag4)
                            {
                                x += this.wNeedBar;
                                y = 80;
                            }

                            GUI.color = (((double)need2.CurLevelPercentage < 0.2) ? (GUI.color = Color.red) : Color.white);
                            need2.DrawOnGUI(new Rect((float)x, (float)y, (float)this.wNeedBar, (float)this.hNeedBar), int.MaxValue, -1f, true, true, null, true);
                            SZWidgets.ButtonImageVar<Need>((float)(x + 11), (float)(y + 30), 24f, 24f, "bbackward", new Action<Need>(this.ASubNeed), need2, "");
                            SZWidgets.ButtonImageVar<Need>((float)(x + this.wNeedBar - 36), (float)(y + 30), 24f, 24f, "bforward", new Action<Need>(this.AAddNeed), need2, "");
                            y += 60;
                            num++;
                        }
                    }

                    return y;
                }


                private void AAddNeed(Need need)
                {
                    need.CurLevelPercentage += 0.05f;
                }


                private void ASubNeed(Need need)
                {
                    need.CurLevelPercentage -= 0.05f;
                }


                private void DrawMemories(int x, int y, int w, int h)
                {
                    List<Thought> thoughtsSorted = CEditor.API.Pawn.GetThoughtsSorted();
                    Text.Font = GameFont.Small;
                    GUI.color = Color.white;
                    SZWidgets.Label(new Rect((float)x, (float)y, (float)w, 30f), "MemoryLower".Translate() + ": " + thoughtsSorted.Count.ToString(), null, "");
                    y += 30;
                    int num = ThoughtTool.CountOfDefs<Thought_MemorySocial>(thoughtsSorted, out var thought, "AteNon");
                    this.listH = (thoughtsSorted.Count - num + 1) * 22;
                    Rect outRect = new Rect((float)x, (float)y, (float)w, (float)h);
                    Rect rect = new Rect(0f, 0f, outRect.width - 16f, (float)this.listH);
                    Widgets.BeginScrollView(outRect, ref this.scrollPos, rect, true);
                    Rect rect2 = rect.ContractedBy(0f);
                    rect2.height = (float)this.listH;
                    Listing_X listing_X = new Listing_X();
                    listing_X.Begin(rect2);

                    using (List<Thought>.Enumerator enumerator = thoughtsSorted.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Thought current = enumerator.Current;
                            if (current != null && (!current.def.IsClassOf<Thought_MemorySocial>() || !current.def.defName.StartsWith("AteNon")))
                            {
                                this.LabelHelper(current, out var label, out string _, out var otherPawnName, out var tooltip);
                                this.IconAndOffsetHelper(current, out var opinionOffset, out var moodOffset, out var icon, out var iconColor);
                                if (listing_X.SelectableThought(label, icon, iconColor, opinionOffset, moodOffset, tooltip, otherPawnName, this.bToggleShowRemoveThought))
                                    CEditor.API.Pawn.RemoveThought(enumerator.Current);
                            }
                        }
                    }

                    bool flag4 = thought != null;
                    if (flag4)
                    {
                        this.LabelHelper(thought, out var name2, out var text2, out var pName2, out var tooltip2, num);
                        this.IconAndOffsetHelper(thought, out var valopin2, out var valmood2, out var icon2, out var iconColor2);
                        bool flag5 = listing_X.SelectableThought(name2, icon2, iconColor2, valopin2, valmood2, tooltip2, pName2, this.bToggleShowRemoveThought);
                        if (flag5)
                            CEditor.API.Pawn.RemoveThought(thought);
                    }

                    listing_X.End();
                    Widgets.EndScrollView();
                }


                private void LabelHelper(Thought t, out string label, out string desc, out string otherPawnName, out string tooltip, int count = 0)
                {
                    label = t.GetThoughtLabel();
                    desc = ((t.CurStageIndex < 0) ? t.GetThoughtDescription() : t.Description);
                    tooltip = desc;
                    bool devMode = Prefs.DevMode;
                    if (devMode)
                    {
                        tooltip = ((desc == null) ? "\n\n" : (desc + "\n\n"));
                        tooltip = string.Concat(new string[]
                        {
                            tooltip,
                            t.def.defName,
                            "\n",
                            t.def.ThoughtClass.ToString(),
                            "\n"
                        });
                        bool flag = t.CurStageIndex >= 0;
                        if (flag)
                        {
                            try
                            {
                                int num = (int)t.def.stages[t.CurStageIndex].baseMoodEffect;
                                int num2 = (int)t.def.stages[t.CurStageIndex].baseOpinionOffset;
                                tooltip = string.Concat(new string[]
                                {
                                    tooltip,
                                    "Stage ",
                                    t.CurStageIndex.ToString(),
                                    "\nBaseMood: ",
                                    num.ToString().Colorize((num == 0) ? ColorLibrary.Grey : ((num > 0) ? ColorLibrary.Green : ColorLibrary.Red))
                                });
                                tooltip = tooltip + " BaseOpinion: " + num2.ToString().Colorize((num2 == 0) ? ColorLibrary.Grey : ((num2 > 0) ? ColorLibrary.Green : ColorLibrary.Red));
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            tooltip = tooltip + "Stage " + t.CurStageIndex.ToString();
                        }
                    }

                    string arg;
                    Pawn otherPawn = t.GetOtherPawn(out arg);
                    otherPawnName = otherPawn.GetPawnName(false);
                    try
                    {
                        bool flag2 = t.def.defName == "WrongApparelGender";
                        if (flag2)
                        {
                            label = string.Format(label, t.pawn.GetOppositeGender().ToString().Translate());
                        }

                        bool flag3 = t.def.defName == "DeadMansApparel";
                        if (flag3)
                        {
                            label = t.LabelCap.CapitalizeFirst();
                        }

                        bool flag4 = label.Contains("{0}");
                        if (flag4)
                        {
                            bool flag5 = !otherPawnName.NullOrEmpty();
                            if (flag5)
                            {
                                bool flag6 = label.Contains("{1}");
                                if (flag6)
                                {
                                    label = string.Format(label, arg, otherPawnName);
                                }
                                else
                                {
                                    label = string.Format(label, otherPawnName);
                                }
                            }
                        }

                        bool flag7 = label.Contains("{TITLE}") || label.Contains("{TITLE_label}");
                        if (flag7)
                        {
                            bool flag8 = t.def.IsClassOf<Thought_MemoryRoyalTitle>();
                            if (flag8)
                            {
                                Thought_MemoryRoyalTitle thought_MemoryRoyalTitle = (Thought_MemoryRoyalTitle)t;
                                string newValue = (CEditor.API.Pawn.gender == Gender.Female) ? thought_MemoryRoyalTitle.titleDef.labelFemale : thought_MemoryRoyalTitle.titleDef.label;
                                label = label.Replace("{TITLE}", newValue).Replace("{TITLE_label}", newValue);
                            }
                            else
                            {
                                label = label.Replace("{TITLE}", CEditor.API.Pawn.GetMainTitle()).Replace("{TITLE_label}", CEditor.API.Pawn.GetMainTitle());
                            }
                        }
                        else
                        {
                            bool flag9 = t.def.IsClassOf<Thought_IdeoRoleEmpty>();
                            if (flag9)
                            {
                                Thought_IdeoRoleEmpty thought_IdeoRoleEmpty = (Thought_IdeoRoleEmpty)t;
                                label = label.Replace("{ROLE}", thought_IdeoRoleEmpty.Role.LabelCap).Replace("{ROLE_label}", thought_IdeoRoleEmpty.Role.LabelCap);
                            }
                            else
                            {
                                bool flag10 = t.def.IsClassOf<Thought_IdeoRoleLost>();
                                if (flag10)
                                {
                                    Thought_IdeoRoleLost thought_IdeoRoleLost = (Thought_IdeoRoleLost)t;
                                    label = label.Replace("{ROLE}", thought_IdeoRoleLost.Role.LabelCap).Replace("{ROLE_label}", thought_IdeoRoleLost.Role.LabelCap);
                                }
                                else
                                {
                                    bool flag11 = t.def.IsClassOf<Thought_IdeoRoleApparelRequirementNotMet>();
                                    if (flag11)
                                    {
                                        Thought_IdeoRoleApparelRequirementNotMet thought_IdeoRoleApparelRequirementNotMet = (Thought_IdeoRoleApparelRequirementNotMet)t;
                                        label = label.Replace("{ROLE}", thought_IdeoRoleApparelRequirementNotMet.Role.LabelCap).Replace("{ROLE_label}", thought_IdeoRoleApparelRequirementNotMet.Role.LabelCap);
                                    }
                                    else
                                    {
                                        bool flag12 = t.def.IsClassOf<Thought_Situational_WearingDesiredApparel>();
                                        if (flag12)
                                        {
                                            Thought_Situational_WearingDesiredApparel thought_Situational_WearingDesiredApparel = (Thought_Situational_WearingDesiredApparel)t;
                                            Precept_Apparel precept_Apparel = (Precept_Apparel)thought_Situational_WearingDesiredApparel.sourcePrecept;
                                            label = label.Replace("{APPAREL}", precept_Apparel.apparelDef.LabelCap).Replace("{APPAREL_label}", precept_Apparel.apparelDef.LabelCap);
                                        }
                                        else
                                        {
                                            bool flag13 = t.def.IsClassOf<Thought_PsychicHarmonizer>();
                                            if (flag13)
                                            {
                                                Thought_PsychicHarmonizer thought_PsychicHarmonizer = (Thought_PsychicHarmonizer)t;
                                                label = thought_PsychicHarmonizer.LabelCap;
                                            }
                                            else
                                            {
                                                bool flag14 = t.def.IsClassOf<Thought_RelicAtRitual>();
                                                if (flag14)
                                                {
                                                    Thought_RelicAtRitual thought_RelicAtRitual = (Thought_RelicAtRitual)t;
                                                    label = label.Replace("{RELICNAME}", thought_RelicAtRitual.relicName).Replace("{RELICNAME_label}", thought_RelicAtRitual.relicName);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        bool flag15 = count > 0;
                        if (flag15)
                        {
                            label = label + " x" + count.ToString();
                        }
                    }
                    catch
                    {
                    }
                }


                private void IconAndOffsetHelper(Thought t, out float opinionOffset, out float moodOffset, out Texture2D icon, out Color iconColor)
                {
                    bool flag = t.def.IsSocial && t.def.IsSituational;
                    opinionOffset = t.GetOpinionOffset();
                    moodOffset = t.TryGetMoodOffset();
                    Texture2D memberValue = t.def.GetMemberValue<Texture2D>("iconInt", null);
                    icon = ((memberValue != null) ? memberValue : ((opinionOffset != float.MinValue || flag) ? ContentFinder<Texture2D>.Get("bsocial", true) : ((t.def.Icon != null) ? t.def.Icon : ((moodOffset > 0f) ? ContentFinder<Texture2D>.Get("Things/Mote/ThoughtSymbol/GenericGood", true) : ContentFinder<Texture2D>.Get("Things/Mote/ThoughtSymbol/GenericBad", true)))));
                    iconColor = (flag ? ColorLibrary.Aquamarine : Color.white);
                }


                private void DrawThoughtButtons(int x, int y, int w, int h)
                {
                    SZWidgets.ButtonImage((float)x, (float)y, 24f, 24f, "UI/Buttons/Dev/Add", new Action(this.ADoAddThought), "", default(Color));
                    SZWidgets.ButtonImage((float)(x - 25), (float)y, 24f, 24f, "bminus", new Action(this.ADoRemoveThought), "", default(Color));
                }


                private void ADoAddThought()
                {
                    WindowTool.Open(new DialogAddThought(null));
                }


                private void ADoRemoveThought()
                {
                    this.bToggleShowRemoveThought = !this.bToggleShowRemoveThought;
                }


                private void DrawLowerButtons(int x, int y, int w, int h)
                {
                    GUI.color = Color.white;
                    SZWidgets.ButtonText((float)(x + w - 120), (float)y, 120f, 30f, Label.FULLNEEDS, new Action(this.AFullNeeds), "");
                    SZWidgets.ButtonText((float)(x + w - 230), (float)y, 110f, 30f, Label.NOMEMORIES, new Action(this.AClearAllMemory), "");
                }


                private void AClearAllMemory()
                {
                    CEditor.API.Pawn.ClearAllThoughts();
                }


                private void AFullNeeds()
                {
                    foreach (Need need in CEditor.API.Pawn.needs.AllNeeds)
                    {
                        need.CurLevelPercentage = 1f;
                    }
                }


                private Vector2 scrollPos;


                private int wNeedBar;


                private int hNeedBar;


                private int yStartMemories;


                private int listH;


                private bool bToggleShowRemoveThought = false;
            }


            private class BlockRecords
            {
                internal BlockRecords()
                {
                }


                internal void Draw(CEditor.EditorUI.coord c)
                {
                    int x = c.x;
                    int y = c.y;
                    int w = c.w;
                    int h = c.h;
                    bool flag = CEditor.API.Pawn == null;
                    if (!flag)
                    {
                        RecordTool.DrawRecordCard(new Rect((float)(x + 10), (float)y, (float)(w - 10), (float)h), CEditor.API.Pawn);
                        this.DrawMiniCapsule(x + 110, y + h - 160);
                    }
                }


                private void DrawMiniCapsule(int x, int y)
                {
                    bool flag = !CEditor.InStartingScreen;
                    if (flag)
                    {
                        SZWidgets.ButtonImage((float)x, (float)y, 200f, 144f, "bcapsule", delegate() { WindowTool.Open(new DialogCapsuleUI()); }, "", default(Color));
                    }
                }
            }


            private class BlockSocial : IPawnable
            {
                internal BlockSocial()
                {
                    this.bShowRemove = false;
                    this.dpr = null;
                    this.selectedInspiration = null;
                    this.selectedMentalState = null;
                    this.selectedIndirectRelationDef = null;
                    this.selectedPawnToSwap = null;
                    this.SelectedPawn = null;
                    this.SelectedPawn2 = null;
                    this.SelectedPawn3 = null;
                    this.SelectedPawn4 = null;
                    this.selectedGender = Gender.Male;
                    this.selectedGender2 = Gender.Male;
                    this.selectedGender3 = Gender.Female;
                    this.selectedGender4 = Gender.Male;
                    this.selRelation1 = PawnRelationDefOf.Parent;
                    this.selRelation2 = null;
                    this.selRelation3 = null;
                    this.selRelation4 = null;
                    this.imgSize = new Vector2(140f, 196f);
                    Type atype = Reflect.GetAType("RimWorld", "SocialCardUtility");
                    bool flag = atype != null;
                    if (flag)
                    {
                        atype.SetMemberValue("showAllRelations", true);
                    }

                    this.lOfRelations = DefTool.ListBy<PawnRelationDef>((PawnRelationDef r) => !r.implied).ToList<PawnRelationDef>();
                    this.lOfRelations.Add(PawnRelationDefOf.Grandparent);
                    this.lOfRelations.Add(PawnRelationDefOf.Sibling);
                    this.lOfRelations.Add(PawnRelationDefOf.HalfSibling);
                    this.lOfRelations.Add(PawnRelationDefOf.Child);
                    this.lOfRelations.Add(PawnRelationDefOf.UncleOrAunt);
                    this.lOfIndirectRelations = null;
                    this.lOfMentalStates = CEditor.API.ListOf<MentalStateDef>(EType.MentalStates);
                    this.lOfInspirations = CEditor.API.ListOf<InspirationDef>(EType.Inspirations);
                    this.lOfRelatedPawns = null;
                }


                public Pawn SelectedPawn { get; set; }


                public Pawn SelectedPawn2 { get; set; }


                public Pawn SelectedPawn3 { get; set; }


                public Pawn SelectedPawn4 { get; set; }


                private Func<InspirationDef, string> FGetInspirationLabel
                {
                    get { return (InspirationDef i) => (i == null) ? Label.NONE : i.LabelCap; }
                }


                private Func<MentalStateDef, string> FGetMentalLabel
                {
                    get { return (MentalStateDef m) => (m == null) ? Label.NONE : m.LabelCap; }
                }


                private Func<ThoughtDef, string> FGetThoughtLabel
                {
                    get { return (ThoughtDef t) => (t == null) ? Label.NONE : t.GetThoughtLabel(0, null); }
                }


                private Func<PawnRelationDef, string> FGetRelationLabel
                {
                    get { return (PawnRelationDef r) => this.GetLabelSelectedPawn(r); }
                }


                private Func<PawnRelationDef, string> FGetRelationLabel2
                {
                    get { return (PawnRelationDef r) => this.GetLabelSelectedPawn2(r); }
                }


                private Func<PawnRelationDef, string> FGetRelationLabel3
                {
                    get { return (PawnRelationDef r) => this.GetLabelSelectedPawn3(r); }
                }


                private Func<PawnRelationDef, string> FGetRelationLabel4
                {
                    get { return (PawnRelationDef r) => this.GetLabelSelectedPawn4(r); }
                }


                private string GetLabelSelectedPawn(PawnRelationDef r)
                {
                    return (this.selectedGender == Gender.Male) ? r.label : (r.labelFemale.NullOrEmpty() ? r.label : r.labelFemale);
                }


                private string GetLabelSelectedPawn2(PawnRelationDef r)
                {
                    bool flag = this.selRelation1 == PawnRelationDefOf.Child;
                    string result;
                    if (flag)
                    {
                        result = ((this.selectedGender2 == Gender.Male) ? r.label : (r.labelFemale.NullOrEmpty() ? r.label : r.labelFemale)) + "(" + this.SelectedPawn.GetPawnName(false) + ")";
                    }
                    else
                    {
                        result = ((this.selectedGender2 == Gender.Male) ? r.label : (r.labelFemale.NullOrEmpty() ? r.label : r.labelFemale)) + "(" + CEditor.API.Pawn.GetPawnName(false) + ")";
                    }

                    return result;
                }


                private string GetLabelSelectedPawn3(PawnRelationDef r)
                {
                    bool flag = this.selRelation1 == PawnRelationDefOf.UncleOrAunt || this.selRelation1 == PawnRelationDefOf.Child;
                    string result;
                    if (flag)
                    {
                        result = ((this.selectedGender3 == Gender.Male) ? r.label : (r.labelFemale.NullOrEmpty() ? r.label : r.labelFemale)) + "(" + ((this.SelectedPawn != null) ? this.SelectedPawn.GetPawnName(false) : "") + ")";
                    }
                    else
                    {
                        result = ((this.selectedGender3 == Gender.Male) ? r.label : (r.labelFemale.NullOrEmpty() ? r.label : r.labelFemale)) + "(" + CEditor.API.Pawn.GetPawnName(false) + ")";
                    }

                    return result;
                }


                private string GetLabelSelectedPawn4(PawnRelationDef r)
                {
                    bool flag = this.selRelation1 == PawnRelationDefOf.UncleOrAunt;
                    string result;
                    if (flag)
                    {
                        result = ((this.selectedGender4 == Gender.Male) ? r.label : (r.labelFemale.NullOrEmpty() ? r.label : r.labelFemale)) + "(" + ((this.SelectedPawn != null) ? this.SelectedPawn.GetPawnName(false) : "") + ")";
                    }
                    else
                    {
                        result = ((this.selectedGender4 == Gender.Male) ? r.label : (r.labelFemale.NullOrEmpty() ? r.label : r.labelFemale)) + "(" + ((this.SelectedPawn != null) ? this.SelectedPawn.GetPawnName(false) : "") + ")";
                    }

                    return result;
                }


                internal void Draw(CEditor.EditorUI.coord c)
                {
                    int x = c.x;
                    int num = c.y;
                    int w = c.w;
                    int h = c.h;
                    bool flag = !CEditor.API.Pawn.HasRelationTracker();
                    if (!flag)
                    {
                        num += 20;
                        this.DrawRelations(x, ref num, w, h);
                        this.DrawCard(x, ref num, w, h);
                    }
                }


                private void AAddIndirectRelation()
                {
                    bool flag = this.selRelation1 == PawnRelationDefOf.Child;
                    if (flag)
                    {
                        this.SelectedPawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn2);
                        this.SelectedPawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn3);
                    }
                    else
                    {
                        bool flag2 = this.selRelation1 == PawnRelationDefOf.HalfSibling;
                        if (flag2)
                        {
                            CEditor.API.Pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn2);
                            CEditor.API.Pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn3);
                            this.SelectedPawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn3);
                            this.SelectedPawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn4);
                        }
                        else
                        {
                            bool flag3 = this.selRelation1 == PawnRelationDefOf.Sibling;
                            if (flag3)
                            {
                                this.SelectedPawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn2);
                                this.SelectedPawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn3);
                                CEditor.API.Pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn2);
                                CEditor.API.Pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn3);
                            }
                            else
                            {
                                bool flag4 = this.selRelation1 == PawnRelationDefOf.Grandparent;
                                if (flag4)
                                {
                                    this.SelectedPawn2.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn);
                                    bool flag5 = CEditor.API.Pawn.GetFirstParentForPawn(this.selectedGender2) == null;
                                    if (flag5)
                                    {
                                        CEditor.API.Pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn2);
                                    }
                                }
                                else
                                {
                                    bool flag6 = this.selRelation1 == PawnRelationDefOf.UncleOrAunt;
                                    if (flag6)
                                    {
                                        this.SelectedPawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn3);
                                        this.SelectedPawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn4);
                                        this.SelectedPawn2.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn3);
                                        this.SelectedPawn2.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn4);
                                        CEditor.API.Pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.SelectedPawn2);
                                    }
                                }
                            }
                        }
                    }

                    this.SelectedPawn = null;
                    this.SelectedPawn2 = null;
                    this.SelectedPawn3 = null;
                    this.SelectedPawn4 = null;
                    this.selRelation1 = PawnRelationDefOf.Parent;
                }


                private void AAddRelation()
                {
                    CEditor.API.Pawn.relations.AddDirectRelation(this.selRelation1, this.SelectedPawn);
                    this.SelectedPawn = null;
                    this.SelectedPawn2 = null;
                    this.SelectedPawn3 = null;
                    this.SelectedPawn4 = null;
                }


                private void AAddThought()
                {
                    WindowTool.Open(new DialogAddThought(Label.TH_SOCIAL));
                }


                private void AChangeInspiration(InspirationDef i)
                {
                    this.selectedInspiration = i;
                    bool flag = CEditor.API.Pawn.Inspiration != null;
                    if (flag)
                    {
                        CEditor.API.Pawn.Inspiration.PostEnd();
                        CEditor.API.Pawn.mindState.inspirationHandler.EndInspiration(CEditor.API.Pawn.Inspiration);
                    }

                    bool flag2 = i != null;
                    if (flag2)
                    {
                        CEditor.API.Pawn.mindState.inspirationHandler.TryStartInspiration(i, null, true);
                    }
                }


                private void AChangeMentalState(MentalStateDef m)
                {
                    this.selectedMentalState = m;
                    CEditor.API.Pawn.Notify_Teleported(true, true);
                    bool flag = CEditor.API.Pawn.MentalState != null;
                    if (flag)
                    {
                        CEditor.API.Pawn.MentalState.PostEnd();
                        CEditor.API.Pawn.mindState.mentalStateHandler.Reset();
                    }

                    bool flag2 = m == MentalStateDefOf.SocialFighting;
                    if (flag2)
                    {
                        List<Pawn> source = (from td in Find.CurrentMap.mapPawns.FreeColonists
                            where td != CEditor.API.Pawn
                            select td).ToList<Pawn>();
                        Pawn otherPawn = source.RandomElement<Pawn>();
                        CEditor.API.Pawn.interactions.StartSocialFight(otherPawn, "MessageSocialFight");
                    }
                    else
                    {
                        CEditor.API.Pawn.mindState.mentalStateHandler.TryStartMentalState(m);
                    }
                }


                private void AOnGenderChange()
                {
                    this.selectedGender = ((this.selectedGender == Gender.Female) ? Gender.Male : Gender.Female);
                    this.SelectedPawn = null;
                }


                private void AOnGenderChange2()
                {
                    this.selectedGender2 = ((this.selectedGender2 == Gender.Female) ? Gender.Male : Gender.Female);
                    this.SelectedPawn2 = null;
                }


                private void AOnGenderChange3()
                {
                    this.selectedGender3 = ((this.selectedGender3 == Gender.Female) ? Gender.Male : Gender.Female);
                    this.SelectedPawn3 = null;
                }


                private void AOnGenderChange4()
                {
                    this.selectedGender4 = ((this.selectedGender4 == Gender.Female) ? Gender.Male : Gender.Female);
                    this.SelectedPawn4 = null;
                }


                private void AOnImgAction()
                {
                    WindowTool.Open(new DialogChoosePawn(this, 1, this.selectedGender, ""));
                }


                private void AOnRelationSelected(PawnRelationDef pr)
                {
                    this.selRelation1 = pr;
                    bool flag = Prefs.DevMode && this.selRelation1 != null;
                    if (flag)
                    {
                        MessageTool.Show(this.selRelation1.defName + " " + ((this.selectedGender == Gender.Male) ? this.selRelation1.label : this.selRelation1.labelFemale), null);
                    }

                    this.selectedGender2 = Gender.Male;
                    this.selectedGender3 = Gender.Female;
                    this.selectedGender4 = Gender.Male;
                    this.SelectedPawn = null;
                    this.SelectedPawn2 = null;
                    this.SelectedPawn3 = null;
                    this.SelectedPawn4 = null;
                }


                private void ASelectOtherPawn()
                {
                    CEditor.API.Pawn = this.selectedPawnToSwap;
                }


                private void AToggleRemove()
                {
                    this.bShowRemove = !this.bShowRemove;
                }


                private void CreateDirect(int x, int y, int w, int h)
                {
                    bool flag = !CEditor.API.Pawn.HasRelationTracker() || CEditor.API.Pawn.relations.DirectRelations.NullOrEmpty<DirectPawnRelation>();
                    if (!flag)
                    {
                        SZWidgets.ScrollView(x, y, w, h, CEditor.API.Pawn.relations.DirectRelations.Count, 196, ref this.scrollDirectRelated, new Action<Listing_X>(this.DrawDirect));
                    }
                }


                private void CreateIndirect(int x, int y, int w, int h)
                {
                    bool flag = !CEditor.API.Pawn.HasRelationTracker();
                    if (!flag)
                    {
                        int objCount;
                        this.lOfRelatedPawns = CEditor.API.Pawn.GetRelatedPawns(out objCount);
                        SZWidgets.ScrollView(x, y, w, h, objCount, 196, ref this.scrollIndirectRelated, new Action<Listing_X>(this.DrawIndirect));
                    }
                }


                private void DrawCard(int x, ref int y, int w, int h)
                {
                    Rect rect = new Rect((float)(x + 400), (float)y, (float)(w - 400), (float)(h - 20));
                    Widgets.DrawBoxSolid(rect, ColorTool.colDarkDimGray);
                    SocialCardUtility.DrawRelationsAndOpinions(rect, CEditor.API.Pawn);
                    y += h - y;
                }


                private void DrawDirect(Listing_X view)
                {
                    try
                    {
                        using (List<DirectPawnRelation>.Enumerator enumerator = ((Pawn_RelationsTracker)CEditor.API.Pawn.relations).DirectRelations.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                if (enumerator.Current != null)
                                {
                                    Pawn otherPawn = enumerator.Current.otherPawn;
                                    RenderTexture image = PortraitsCache.Get(otherPawn, this.imgSize, (Rot4)Rot4.South, new Vector3(), 1f, true, true, true, true, (IReadOnlyDictionary<Apparel, Color>)null, new Color?(), false, new PawnHealthState?());
                                    string name = enumerator.Current.RelationLabelDirect();
                                    string tooltip = CEditor.API.Pawn.RelationTooltip(otherPawn);
                                    bool selected = this.dpr == enumerator.Current;
                                    switch (view.Selectable(name, selected, tooltip, image, imageSize: this.imgSize, withRemove: this.bShowRemove, selectHeight: 180f, backColor: Color.white, selectedColor: ColorTool.colLightGray))
                                    {
                                        case 1:
                                            selectedPawnToSwap = otherPawn;
                                            dpr = enumerator.Current;
                                            break;
                                        case 2:
                                            API.Pawn.relations.RemoveDirectRelation(enumerator.Current);
                                            dpr = null;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }


                private void DrawIndirect(Listing_X view)
                {
                    try
                    {
                        foreach (Pawn pawn in this.lOfRelatedPawns)
                        {
                            this.lOfIndirectRelations = CEditor.API.Pawn.GetRelations(pawn).ToList<PawnRelationDef>();
                            foreach (PawnRelationDef pawnRelationDef in this.lOfIndirectRelations)
                            {
                                bool flag = !pawnRelationDef.implied;
                                if (!flag)
                                {
                                    RenderTexture image = PortraitsCache.Get(pawn, this.imgSize, Rot4.South, default(Vector3), 1f, true, true, true, true, null, null, false, null);
                                    string name = pawnRelationDef.RelationLabelIndirect(pawn);
                                    string tooltip = CEditor.API.Pawn.RelationTooltip(pawn);
                                    bool selected = this.selectedPawnToSwap == pawn && this.selectedIndirectRelationDef == pawnRelationDef;
                                    int num = view.Selectable(name, selected, tooltip, image, null, null, this.imgSize, false, 180f, Color.white, ColorTool.colLightGray, true);
                                    bool flag2 = num == 1;
                                    if (flag2)
                                    {
                                        this.selectedPawnToSwap = pawn;
                                        this.selectedIndirectRelationDef = pawnRelationDef;
                                        this.dpr = null;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }


                private void HandleChild()
                {
                    this.showPawn3 = true;
                    this.showPawn4 = false;
                    this.allowGender2 = false;
                    this.allowGender3 = false;
                    this.allowGender4 = false;
                    this.selRelation2 = PawnRelationDefOf.Parent;
                    this.selRelation3 = PawnRelationDefOf.Parent;
                    bool flag = CEditor.API.Pawn.gender == Gender.Male;
                    if (flag)
                    {
                        this.SelectedPawn2 = CEditor.API.Pawn;
                    }
                    else
                    {
                        this.SelectedPawn3 = CEditor.API.Pawn;
                    }

                    this.selectedGender2 = Gender.Male;
                    this.selectedGender3 = Gender.Female;
                    bool flag2 = this.SelectedPawn == CEditor.API.Pawn || this.SelectedPawn == this.SelectedPawn2 || this.SelectedPawn == this.SelectedPawn3;
                    if (flag2)
                    {
                        this.SelectedPawn = null;
                    }

                    bool flag3 = this.SelectedPawn2 == this.SelectedPawn;
                    if (flag3)
                    {
                        this.SelectedPawn2 = null;
                    }

                    bool flag4 = this.SelectedPawn3 == this.SelectedPawn;
                    if (flag4)
                    {
                        this.SelectedPawn3 = null;
                    }

                    this.canAddRelation = (this.SelectedPawn != null && this.SelectedPawn2 != null && this.SelectedPawn3 != null);
                }


                private void HandleUncle()
                {
                    this.showPawn3 = true;
                    this.showPawn4 = true;
                    this.allowGender2 = true;
                    this.allowGender3 = false;
                    this.allowGender4 = false;
                    this.selRelation2 = PawnRelationDefOf.Parent;
                    this.selRelation3 = PawnRelationDefOf.Parent;
                    this.selRelation4 = PawnRelationDefOf.Parent;
                    Pawn firstParentForPawn = CEditor.API.Pawn.GetFirstParentForPawn(this.selectedGender2);
                    bool flag = firstParentForPawn != null;
                    if (flag)
                    {
                        this.SelectedPawn2 = firstParentForPawn;
                    }

                    Pawn firstParentForPawn2 = this.SelectedPawn.GetFirstParentForPawn(this.selectedGender3);
                    bool flag2 = firstParentForPawn2 != null;
                    if (flag2)
                    {
                        this.SelectedPawn3 = firstParentForPawn2;
                    }

                    Pawn firstParentForPawn3 = this.SelectedPawn.GetFirstParentForPawn(this.selectedGender4);
                    bool flag3 = firstParentForPawn3 != null;
                    if (flag3)
                    {
                        this.SelectedPawn4 = firstParentForPawn3;
                    }

                    bool flag4 = this.SelectedPawn == CEditor.API.Pawn || this.SelectedPawn == this.SelectedPawn2 || this.SelectedPawn == this.SelectedPawn3 || this.SelectedPawn == this.SelectedPawn4;
                    if (flag4)
                    {
                        this.SelectedPawn = null;
                    }

                    bool flag5 = this.SelectedPawn3 == CEditor.API.Pawn || this.SelectedPawn3 == this.SelectedPawn || this.SelectedPawn3 == this.SelectedPawn2 || this.SelectedPawn3 == this.SelectedPawn4;
                    if (flag5)
                    {
                        this.SelectedPawn3 = null;
                    }

                    bool flag6 = this.SelectedPawn4 == CEditor.API.Pawn || this.SelectedPawn4 == this.SelectedPawn || this.SelectedPawn4 == this.SelectedPawn2 || this.SelectedPawn4 == this.SelectedPawn3;
                    if (flag6)
                    {
                        this.SelectedPawn4 = null;
                    }

                    this.canAddRelation = (this.SelectedPawn != null && this.SelectedPawn2 != null && this.SelectedPawn3 != null && this.SelectedPawn4 != null);
                }


                private void HandleSibling()
                {
                    this.showPawn3 = true;
                    this.showPawn4 = false;
                    this.allowGender2 = true;
                    this.allowGender3 = true;
                    this.allowGender4 = false;
                    this.selRelation2 = PawnRelationDefOf.Parent;
                    this.selRelation3 = PawnRelationDefOf.Parent;
                    bool flag = this.selectedGender2 == Gender.Male;
                    if (flag)
                    {
                        this.selectedGender3 = Gender.Female;
                    }
                    else
                    {
                        this.selectedGender3 = Gender.Male;
                    }

                    Pawn firstParentForPawn = CEditor.API.Pawn.GetFirstParentForPawn(this.selectedGender2);
                    bool flag2 = firstParentForPawn != null;
                    if (flag2)
                    {
                        this.SelectedPawn2 = firstParentForPawn;
                    }

                    Pawn firstParentForPawn2 = CEditor.API.Pawn.GetFirstParentForPawn(this.selectedGender3);
                    bool flag3 = firstParentForPawn2 != null;
                    if (flag3)
                    {
                        this.SelectedPawn3 = firstParentForPawn2;
                    }

                    bool flag4 = this.SelectedPawn3 == null;
                    if (flag4)
                    {
                        firstParentForPawn2 = this.SelectedPawn.GetFirstParentForPawn(this.selectedGender3);
                        bool flag5 = firstParentForPawn2 != null;
                        if (flag5)
                        {
                            this.SelectedPawn3 = firstParentForPawn2;
                        }
                    }

                    bool flag6 = this.SelectedPawn2 == null;
                    if (flag6)
                    {
                        firstParentForPawn = this.SelectedPawn.GetFirstParentForPawn(this.selectedGender2);
                        bool flag7 = firstParentForPawn != null;
                        if (flag7)
                        {
                            this.SelectedPawn2 = firstParentForPawn;
                        }
                    }

                    bool flag8 = this.SelectedPawn == CEditor.API.Pawn || this.SelectedPawn == this.SelectedPawn2 || this.SelectedPawn == this.SelectedPawn3;
                    if (flag8)
                    {
                        this.SelectedPawn = null;
                    }

                    bool flag9 = this.SelectedPawn3 == this.SelectedPawn2;
                    if (flag9)
                    {
                        this.SelectedPawn3 = null;
                    }

                    this.canAddRelation = (this.SelectedPawn != null && this.SelectedPawn2 != null && this.SelectedPawn3 != null);
                }


                private void HandleHalfSibling()
                {
                    this.showPawn3 = true;
                    this.showPawn4 = true;
                    this.allowGender2 = false;
                    this.allowGender3 = true;
                    this.allowGender4 = false;
                    this.selRelation2 = PawnRelationDefOf.Parent;
                    this.selRelation3 = PawnRelationDefOf.Parent;
                    this.selRelation4 = PawnRelationDefOf.Parent;
                    bool flag = this.selectedGender3 == Gender.Male;
                    if (flag)
                    {
                        this.selectedGender2 = Gender.Female;
                        this.selectedGender4 = Gender.Female;
                    }
                    else
                    {
                        this.selectedGender2 = Gender.Male;
                        this.selectedGender4 = Gender.Male;
                    }

                    Pawn commonParent = RelationTool.GetCommonParent(CEditor.API.Pawn, this.SelectedPawn, this.selectedGender3);
                    bool flag2 = commonParent != null;
                    if (flag2)
                    {
                        this.SelectedPawn3 = commonParent;
                    }

                    bool flag3 = commonParent == null;
                    if (flag3)
                    {
                        Pawn firstParentForPawn = CEditor.API.Pawn.GetFirstParentForPawn(this.selectedGender3);
                        bool flag4 = firstParentForPawn != null;
                        if (flag4)
                        {
                            this.SelectedPawn3 = firstParentForPawn;
                        }

                        bool flag5 = this.SelectedPawn3 == null;
                        if (flag5)
                        {
                            firstParentForPawn = this.SelectedPawn.GetFirstParentForPawn(this.selectedGender3);
                            bool flag6 = firstParentForPawn != null;
                            if (flag6)
                            {
                                this.SelectedPawn3 = firstParentForPawn;
                            }
                        }
                    }

                    Pawn firstParentForPawn2 = CEditor.API.Pawn.GetFirstParentForPawn(this.selectedGender2);
                    bool flag7 = firstParentForPawn2 != null;
                    if (flag7)
                    {
                        this.SelectedPawn2 = firstParentForPawn2;
                    }

                    Pawn firstParentForPawn3 = this.SelectedPawn.GetFirstParentForPawn(this.selectedGender4);
                    bool flag8 = firstParentForPawn3 != null;
                    if (flag8)
                    {
                        this.SelectedPawn4 = firstParentForPawn3;
                    }

                    bool flag9 = this.SelectedPawn == CEditor.API.Pawn || this.SelectedPawn == this.SelectedPawn2 || this.SelectedPawn == this.SelectedPawn3 || this.SelectedPawn == this.SelectedPawn4;
                    if (flag9)
                    {
                        this.SelectedPawn = null;
                    }

                    bool flag10 = RelationTool.AreThosePawnSisBro(CEditor.API.Pawn, this.SelectedPawn);
                    if (flag10)
                    {
                        this.SelectedPawn = null;
                    }

                    bool flag11 = this.SelectedPawn2 == this.SelectedPawn4;
                    if (flag11)
                    {
                        this.SelectedPawn4 = null;
                    }

                    this.canAddRelation = (this.SelectedPawn != null && this.SelectedPawn2 != null && this.SelectedPawn3 != null && this.SelectedPawn4 != null);
                }


                private void HandleGrandparent()
                {
                    this.showPawn3 = false;
                    this.showPawn4 = false;
                    this.allowGender2 = true;
                    this.allowGender3 = false;
                    this.allowGender4 = false;
                    this.selRelation2 = PawnRelationDefOf.Parent;
                    Pawn firstParentForPawn = CEditor.API.Pawn.GetFirstParentForPawn(this.selectedGender2);
                    bool flag = firstParentForPawn != null;
                    if (flag)
                    {
                        this.SelectedPawn2 = firstParentForPawn;
                    }

                    bool flag2 = this.SelectedPawn == CEditor.API.Pawn || this.SelectedPawn == this.SelectedPawn2;
                    if (flag2)
                    {
                        this.SelectedPawn = null;
                    }

                    this.canAddRelation = (this.SelectedPawn != null && this.SelectedPawn2 != null);
                }


                private void DrawRelations(int x, ref int y, int w, int h)
                {
                    Widgets.DrawBoxSolid(new Rect((float)x, (float)y, 400f, (float)h), ColorTool.colAsche);
                    this.DrawUpper(x, y, w, h);
                    SZWidgets.FloatMenuOnLabelAndImage<PawnRelationDef>(new Rect((float)(x + 10), (float)y, 110f, 100f), ColorTool.colDarkDimGray, "bplus2", this.SelectedPawn, ColorTool.colDimGray, this.lOfRelations, this.FGetRelationLabel, this.selRelation1, new Action<PawnRelationDef>(this.AOnRelationSelected), new Action(this.AOnImgAction), true);
                    SZWidgets.ButtonImage(new Rect((float)(x + 100), (float)(y - 20), 20f, 20f), (this.selectedGender == Gender.Female) ? "bfemale" : "bmale", new Action(this.AOnGenderChange), "");
                    int num = 0;
                    bool flag = this.selRelation1 != null;
                    if (flag)
                    {
                        bool implied = this.selRelation1.implied;
                        if (implied)
                        {
                            bool flag2 = this.selRelation1 == PawnRelationDefOf.Child;
                            if (flag2)
                            {
                                this.HandleChild();
                            }
                            else
                            {
                                bool flag3 = this.selRelation1 == PawnRelationDefOf.Sibling;
                                if (flag3)
                                {
                                    this.HandleSibling();
                                }
                                else
                                {
                                    bool flag4 = this.selRelation1 == PawnRelationDefOf.HalfSibling;
                                    if (flag4)
                                    {
                                        this.HandleHalfSibling();
                                    }
                                    else
                                    {
                                        bool flag5 = this.selRelation1 == PawnRelationDefOf.Grandparent;
                                        if (flag5)
                                        {
                                            this.HandleGrandparent();
                                        }
                                        else
                                        {
                                            bool flag6 = this.selRelation1 == PawnRelationDefOf.UncleOrAunt;
                                            if (flag6)
                                            {
                                                this.HandleUncle();
                                            }
                                        }
                                    }
                                }
                            }

                            num += 130;
                            Text.WordWrap = true;
                            SZWidgets.LabelBackground(new Rect((float)(x + 10), (float)(y + num - 20), 110f, 20f), this.GetLabelSelectedPawn2(this.selRelation2), ColorTool.colDarkDimGray, 0, "", default(Color));
                            SZWidgets.FloatMenuOnLabelAndImage<PawnRelationDef>(new Rect((float)(x + 10), (float)(y + num), 110f, 100f), ColorTool.colDarkDimGray, "bplus2", this.SelectedPawn2, ColorTool.colDimGray, this.lOfRelations, this.FGetRelationLabel2, this.selRelation2, null, new Action(this.AOnParent2Selected), this.allowGender2);
                            bool flag7 = this.allowGender2;
                            if (flag7)
                            {
                                SZWidgets.ButtonImage(new Rect((float)(x + 100), (float)(y - 20 + num), 20f, 20f), (this.selectedGender2 == Gender.Female) ? "bfemale" : "bmale", new Action(this.AOnGenderChange2), "");
                            }

                            bool flag8 = this.showPawn3;
                            if (flag8)
                            {
                                SZWidgets.LabelBackground(new Rect((float)(x + 150), (float)(y + num - 20), 110f, 20f), this.GetLabelSelectedPawn3(this.selRelation3), ColorTool.colDarkDimGray, 0, "", default(Color));
                                SZWidgets.FloatMenuOnLabelAndImage<PawnRelationDef>(new Rect((float)(x + 150), (float)(y + num), 110f, 100f), ColorTool.colDarkDimGray, "bplus2", this.SelectedPawn3, ColorTool.colDimGray, this.lOfRelations, this.FGetRelationLabel3, this.selRelation3, null, new Action(this.AOnParent3Selected), this.allowGender3);
                                bool flag9 = this.allowGender3;
                                if (flag9)
                                {
                                    SZWidgets.ButtonImage(new Rect((float)(x + 240), (float)(y - 20 + num), 20f, 20f), (this.selectedGender3 == Gender.Female) ? "bfemale" : "bmale", new Action(this.AOnGenderChange3), "");
                                }
                            }

                            bool flag10 = this.showPawn4;
                            if (flag10)
                            {
                                SZWidgets.LabelBackground(new Rect((float)(x + 290), (float)(y + num - 20), 110f, 20f), this.GetLabelSelectedPawn4(this.selRelation4), ColorTool.colDarkDimGray, 0, "", default(Color));
                                SZWidgets.FloatMenuOnLabelAndImage<PawnRelationDef>(new Rect((float)(x + 290), (float)(y + num), 110f, 100f), ColorTool.colDarkDimGray, "bplus2", this.SelectedPawn4, ColorTool.colDimGray, this.lOfRelations, this.FGetRelationLabel4, this.selRelation4, null, new Action(this.AOnParent4Selected), this.allowGender4);
                                bool flag11 = this.allowGender4;
                                if (flag11)
                                {
                                    SZWidgets.ButtonImage(new Rect((float)(x + 380), (float)(y - 20 + num), 20f, 20f), (this.selectedGender4 == Gender.Female) ? "bfemale" : "bmale", new Action(this.AOnGenderChange4), "");
                                }
                            }

                            bool flag12 = this.canAddRelation;
                            if (flag12)
                            {
                                SZWidgets.ButtonImage(new Rect((float)(x + 120), (float)(y + 35), 20f, 20f), "bplus2", new Action(this.AAddIndirectRelation), "");
                            }
                        }
                        else
                        {
                            bool flag13 = CEditor.API.Pawn == this.SelectedPawn;
                            if (flag13)
                            {
                                this.SelectedPawn = null;
                            }

                            bool flag14 = this.SelectedPawn != null;
                            if (flag14)
                            {
                                SZWidgets.ButtonImage(new Rect((float)(x + 120), (float)(y + 35), 20f, 20f), "bplus2", new Action(this.AAddRelation), "");
                            }
                        }
                    }

                    this.CreateDirect(x + 5, y + num + 110, 190, h - 130);
                    this.CreateIndirect(x + 200, y + num + 110, 190, h - 130);
                }


                private void AOnParent2Selected()
                {
                    WindowTool.Open(new DialogChoosePawn(this, 2, this.selectedGender2, ""));
                }


                private void AOnParent3Selected()
                {
                    WindowTool.Open(new DialogChoosePawn(this, 3, this.selectedGender3, ""));
                }


                private void AOnParent4Selected()
                {
                    WindowTool.Open(new DialogChoosePawn(this, 4, this.selectedGender4, ""));
                }


                private void DrawUpper(int x, int y, int w, int h)
                {
                    bool flag = !CEditor.InStartingScreen;
                    if (flag)
                    {
                        bool flag2 = CEditor.API.Pawn.MentalState == null;
                        if (flag2)
                        {
                            this.selectedMentalState = null;
                        }
                        else
                        {
                            bool flag3 = CEditor.API.Pawn.MentalState != null && CEditor.API.Pawn.MentalState.def != this.selectedMentalState;
                            if (flag3)
                            {
                                this.selectedMentalState = CEditor.API.Pawn.MentalState.def;
                            }
                        }

                        bool flag4 = CEditor.API.Pawn.Inspiration == null;
                        if (flag4)
                        {
                            this.selectedInspiration = null;
                        }
                        else
                        {
                            bool flag5 = CEditor.API.Pawn.Inspiration != null && CEditor.API.Pawn.Inspiration.def != this.selectedInspiration;
                            if (flag5)
                            {
                                this.selectedInspiration = CEditor.API.Pawn.Inspiration.def;
                            }
                        }

                        SZWidgets.FloatMenuOnButtonImage<MentalStateDef>(new Rect((float)(x + 400 - 35), (float)y, 30f, 30f), ContentFinder<Texture2D>.Get("bmental", true), this.lOfMentalStates, this.FGetMentalLabel, new Action<MentalStateDef>(this.AChangeMentalState), CEditor.API.Pawn.GetMentalStateTooltip());
                        SZWidgets.FloatMenuOnLabel<MentalStateDef>(new Rect((float)(x + 400 - 195), (float)y, 160f, 30f), ColorTool.colDarkDimGray, this.lOfMentalStates, this.FGetMentalLabel, this.selectedMentalState, new Action<MentalStateDef>(this.AChangeMentalState), CEditor.API.Pawn.GetMentalStateTooltip());
                        SZWidgets.FloatMenuOnButtonImage<InspirationDef>(new Rect((float)(x + 400 - 35), (float)(y + 33), 30f, 30f), ContentFinder<Texture2D>.Get("binspire", true), this.lOfInspirations, this.FGetInspirationLabel, new Action<InspirationDef>(this.AChangeInspiration), CEditor.API.Pawn.GetMentalStateTooltip());
                        SZWidgets.FloatMenuOnLabel<InspirationDef>(new Rect((float)(x + 400 - 195), (float)(y + 33), 160f, 30f), ColorTool.colDarkDimGray, this.lOfInspirations, this.FGetInspirationLabel, this.selectedInspiration, new Action<InspirationDef>(this.AChangeInspiration), CEditor.API.Pawn.GetInspirationTooltip());
                    }

                    SZWidgets.ButtonImage(new Rect((float)(x + 400 - 70), (float)(y + 66), 30f, 30f), "bminus2", new Action(this.AToggleRemove), "");
                    bool flag6 = this.selectedPawnToSwap != null;
                    if (flag6)
                    {
                        SZWidgets.ButtonImage(new Rect((float)(x + 400 - 105), (float)(y + 66), 30f, 30f), "UI/Buttons/DragHash", new Action(this.ASelectOtherPawn), Label.SWAP_TO_PAWN);
                    }

                    SZWidgets.ButtonImage(new Rect((float)(x + 400 - 35), (float)(y + 66), 30f, 30f), "bmemory", new Action(this.AAddThought), "");
                }


                private const int LIST_OBJ_H = 196;


                private const int LIST_OBJ_W = 140;


                private const int LIST_SELECTED_H = 180;


                private const int widthLeft = 400;


                private bool bShowRemove;


                private DirectPawnRelation dpr;


                private Vector2 imgSize;


                private List<PawnRelationDef> lOfIndirectRelations;


                private List<InspirationDef> lOfInspirations;


                private List<MentalStateDef> lOfMentalStates;


                private List<Pawn> lOfRelatedPawns;


                private List<PawnRelationDef> lOfRelations;


                private Vector2 scrollDirectRelated = default(Vector2);


                private Vector2 scrollIndirectRelated = default(Vector2);


                private Gender selectedGender;


                private Gender selectedGender2;


                private Gender selectedGender3;


                private Gender selectedGender4;


                private PawnRelationDef selectedIndirectRelationDef;


                private InspirationDef selectedInspiration;


                private MentalStateDef selectedMentalState;


                private Pawn selectedPawnToSwap;


                private PawnRelationDef selRelation1;


                private PawnRelationDef selRelation2;


                private PawnRelationDef selRelation3;


                private PawnRelationDef selRelation4;


                private bool allowGender2 = false;


                private bool allowGender3 = false;


                private bool allowGender4 = false;


                private bool showPawn3 = true;


                private bool showPawn4 = true;


                private bool canAddRelation = true;
            }


            private class BlockPerson
            {
                internal BlockPerson()
                {
                    this.iTickRemoveDeadLogo = 0;
                    this.iTickTool = 0;
                    this.bIsBodyOpen = false;
                    this.bIsHairOpen = false;
                    CEditor.IsRaceSpecificHead = false;
                    this.bIsHeadOpen = false;
                    this.bShowClothes = true;
                    this.bShowHat = true;
                    this.isAlien = false;
                    this.isApparelConfigOpen = false;
                    this.isBeardConfigOpen = false;
                    this.isBodyTattooConfigOpen = false;
                    this.isEyeConfigOpen = false;
                    this.isFaceTattooConfigOpen = false;
                    this.isHaircolorConfigOpen = false;
                    this.isHairConfigOpen = false;
                    this.isPrimaryColor = true;
                    this.isPrimarySkinColor = true;
                    this.isSkinConfigOpen = false;
                    this.isWeaponConfigOpen = false;
                    this.randomDeadColor = Color.clear;
                    this.apparelCurrent = null;
                    this.weaponCurrent = null;
                    this.selectedModName = null;
                    this.lOfHairDefs = HairTool.GetHairList(this.selectedModName);
                    this.lOfBeardDefs = StyleTool.GetBeardList(this.selectedModName);
                    TattooDefOf.NoTattoo_Face.noGraphic = true;
                    TattooDefOf.NoTattoo_Face.texPath = "bclear";
                    TattooDefOf.NoTattoo_Body.noGraphic = true;
                    TattooDefOf.NoTattoo_Body.texPath = "bclear";
                    BeardDefOf.NoBeard.noGraphic = true;
                    BeardDefOf.NoBeard.texPath = "bclear";
                }


                private Rect RectFullSolid
                {
                    get { return new Rect((float)this.x, (float)this.y, (float)this.w, 24f); }
                }


                internal void Draw(CEditor.EditorUI.coord c)
                {
                    try
                    {
                        this.x = c.x;
                        this.y = c.y;
                        this.w = c.w;
                        this.h = c.h;
                        if (CEditor.API.Pawn != null)
                        {
                            this.isAlien = CEditor.API.Pawn.IsAlienRace();
                            this.apparelCurrent = CEditor.API.Pawn.ThisOrFirstWornApparel(this.apparelCurrent);
                            this.weaponCurrent = CEditor.API.Pawn.ThisOrFirstWeapon(this.weaponCurrent);
                            this.DrawTop();
                            this.DrawImage();
                            this.DrawMainIcons();
                            this.y += 300;
                            this.DrawHaare();
                            this.DrawHead();
                            this.DrawBody();
                            this.DrawWeaponSelector();
                            this.DrawApparelSelector();
                            this.DrawDeadLogo();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Message("Got Exception drawing");
                        Log.Message(ex.ToString());
                    }
                }


                private void DrawApparelSelector()
                {
                    bool flag = !CEditor.API.Pawn.HasApparelTracker() || CEditor.API.Pawn.apparel.WornApparel.NullOrEmpty<Apparel>();
                    if (!flag)
                    {
                        try
                        {
                            List<Apparel> list = (from ap in CEditor.API.Pawn.apparel.WornApparel
                                orderby this.GetDrawOrder(ap) descending
                                select ap).ToList<Apparel>();
                            for (int i = 0; i < list.Count; i++)
                            {
                                Apparel apparel = list[i];
                                SZWidgets.NavSelectorImageBox2<Apparel>(this.RectFullSolid, apparel, new Action<Apparel>(this.AChangeApparelUI), new Action<Apparel>(this.ARandomApparel), new Action<Apparel>(this.ASetPrevApparel), new Action<Apparel>(this.ASetNextApparel), new Action<Apparel>(this.AOnTextureApparel), new Action<Apparel>(this.AConfigApparelcolor), apparel.def.label.CapitalizeFirst(), apparel.GetTooltip().text, Label.TIP_RANDOM_APPAREL, null, null, default(Color));
                                this.y += 30;
                                bool flag2 = this.isApparelConfigOpen && this.apparelCurrent == apparel;
                                if (flag2)
                                {
                                    bool flag3 = apparel.TryGetComp<CompColorable>() != null;
                                    if (flag3)
                                    {
                                        SZWidgets.ColorBox(new Rect((float)(this.x + 25), (float)this.y, (float)(this.w - 50), 95f), apparel.DrawColor, new Action<Color>(this.AOnApparelColorChanged), true);
                                        this.y += 95;
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }


                private void DrawBody()
                {
                    if (CEditor.API.Pawn.HasStoryTracker())
                    {
                        SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AOnBodyOpen), null, null, null, null, new Action(this.AOnBodyOpen), BodyPartDefOf.Torso.label.CapitalizeFirst(), null, null, null, null, ColorTool.colBeige, null);
                        this.y += 30;
                        if (this.bIsBodyOpen)
                        {
                            this.DrawBodySelector();
                            this.DrawSkinColorSelector();
                            this.DrawBodyTattooSelector();
                        }
                    }
                }


                private void DrawDeadLogo()
                {
                    bool o = CEditor.API.GetO(OptionB.SHOWDEADLOGO);
                    if (o)
                    {
                        bool flag = CEditor.API.Pawn != null && CEditor.API.Pawn.Dead && this.iTickRemoveDeadLogo == 0;
                        if (flag)
                        {
                            GUI.color = this.randomDeadColor;
                            SZWidgets.ButtonImage(new Rect((float)(this.x + 45), 60f, 150f, 150f), "bdead", new Action(this.ARemoveDeadLogo), "");
                        }
                        else
                        {
                            bool flag2 = this.iTickRemoveDeadLogo > 0;
                            if (flag2)
                            {
                                this.iTickRemoveDeadLogo--;
                            }
                        }
                    }
                }


                private void DrawHaare()
                {
                    bool flag = !CEditor.API.Pawn.HasStoryTracker();
                    if (!flag)
                    {
                        SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AOnHairOpen), null, null, null, null, new Action(this.AOnHairOpen), Label.HAIR, null, null, null, null, ColorTool.colBeige, null);
                        this.y += 30;
                        bool flag2 = this.bIsHairOpen;
                        if (flag2)
                        {
                            this.DrawHairSelector();
                            this.DrawBeardSelector();
                            this.DrawHairColorSelector();
                            this.DrawGradientSelector();
                        }
                    }
                }


                private void DrawHead()
                {
                    bool flag = !CEditor.API.Pawn.HasStoryTracker();
                    if (!flag)
                    {
                        SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AOnHeadOpen), null, null, null, null, new Action(this.AOnHeadOpen), BodyPartDefOf.Head.label.CapitalizeFirst(), null, null, null, null, ColorTool.colBeige, null);
                        this.y += 30;
                        bool flag2 = this.bIsHeadOpen;
                        if (flag2)
                        {
                            this.DrawFaceTattooSelector();
                            this.deftest = CEditor.API.Pawn.FA_GetCurrentDef(FacialTool.FACE);
                            bool flag3 = this.deftest == null;
                            if (flag3)
                            {
                                SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AChooseHeadCustom), new Action(this.ARandomHead), new Action(this.ASetPrevHead), new Action(this.ASetNextHead), null, null, BodyPartDefOf.Head.label.CapitalizeFirst() + ": " + CEditor.API.Pawn.GetHeadName(null), null, Label.TIP_RANDOM_HEAD, null, null, default(Color), null);
                                this.y += 30;
                            }
                            else
                            {
                                SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AFACustomHead), new Action(this.AFARandomHead), new Action(this.AFASetPrevHead), new Action(this.AFASetNextHead), null, null, BodyPartDefOf.Head.label.CapitalizeFirst() + ": " + CEditor.API.Pawn.FA_GetCurrentDefName(FacialTool.FACE), null, Label.TIP_RANDOM_HEAD, null, null, default(Color), null);
                                this.y += 30;
                                SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AFACustomEye), new Action(this.AFARandomEye), new Action(this.AFASetPrevEye), new Action(this.AFASetNextEye), null, null, Label.HB_Eye + ": " + CEditor.API.Pawn.FA_GetCurrentDefName(FacialTool.EYE), null, null, null, null, default(Color), null);
                                this.y += 30;
                                this.DrawEyeColorSelection();
                                SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AFACustomLid), new Action(this.AFARandomLid), new Action(this.AFASetPrevLid), new Action(this.AFASetNextLid), null, null, Label.LID + ": " + CEditor.API.Pawn.FA_GetCurrentDefName(FacialTool.LID), null, null, null, null, default(Color), null);
                                this.y += 30;
                                SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AFACustomBrow), new Action(this.AFARandomBrow), new Action(this.AFASetPrevBrow), new Action(this.AFASetNextBrow), null, null, Label.BROW + ": " + CEditor.API.Pawn.FA_GetCurrentDefName(FacialTool.BROW), null, null, null, null, default(Color), null);
                                this.y += 30;
                                SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AFACustomMouth), new Action(this.AFARandomMouth), new Action(this.AFASetPrevMouth), new Action(this.AFASetNextMouth), null, null, Label.MOUTH + ": " + CEditor.API.Pawn.FA_GetCurrentDefName(FacialTool.MOUTH), null, null, null, null, default(Color), null);
                                this.y += 30;
                                SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AFACustomSkin), new Action(this.AFARandomSkin), new Action(this.AFASetPrevSkin), new Action(this.AFASetNextSkin), null, null, Label.SKIN + CEditor.API.Pawn.FA_GetCurrentDefName(FacialTool.SKIN), null, null, null, null, default(Color), null);
                                this.y += 30;
                            }
                        }
                    }
                }


                private void DrawImage()
                {
                    SZWidgets.ButtonImage((float)this.x, (float)(this.y + 200), 22f, 22f, "bbackward", new Action(this.AChoosePrevPawn), "", default(Color));
                    SZWidgets.ButtonImage((float)(this.x + this.w - 22), (float)(this.y + 200), 22f, 22f, "bforward", new Action(this.AChooseNextPawn), "", default(Color));
                    SZWidgets.ButtonInvisible(new Rect((float)(this.x + 30), 60f, (float)(this.w - 60), 85f), new Action(this.AChangeHairUI), "");
                    SZWidgets.ButtonInvisibleVar<Apparel>(new Rect((float)(this.x + 30), 145f, (float)(this.w - 60), 100f), new Action<Apparel>(this.AChangeApparelUI), null, "");
                    GUI.color = Color.white;
                    CEditor.API.Get<Capturer>(EType.Capturer).DrawPawnImage(CEditor.API.Pawn, this.x + 21, this.y + 20);
                }


                private void DrawMainIcons()
                {
                    SZWidgets.ButtonImage((float)(this.x + 2), 260f, 24f, 24f, "UI/Buttons/Dev/Reload", new Action(this.ALoadPawn), Label.TIP_LOAD_PAWN_FROM_SLOT, default(Color));
                    SZWidgets.ButtonImage((float)(this.x + 28), 260f, 24f, 24f, "bsave", new Action(this.ASavePawn), Label.TIP_SAVE_PAWN_TO_SLOT, default(Color));
                    if (!CEditor.InStartingScreen)
                    {
                        SZWidgets.ButtonImage((float)(this.x + 56), 260f, 24f, 24f, "UI/Buttons/DragHash", new Action(this.AJumpToPawn), "", default(Color));
                    }

                    if (!CEditor.InStartingScreen)
                    {
                        SZWidgets.ButtonImage((float)(this.x + 84), 260f, 24f, 24f, "bteleport", new Action(this.ABeginTeleportSelectPawn), Label.TIP_TELEPORT, default(Color));
                    }

                    if (this.isAlien)
                    {
                        SZWidgets.ButtonImage((float)(this.x + 109), 260f, 26f, 26f, "bheadaddon", new Action(this.AChangeHeadAddons), "", default(Color));
                    }

                    Pawn pawn = CEditor.API.Pawn;
                    if (pawn != null && pawn.gender == Gender.Male)
                    {
                        if (Widgets.ButtonImage(new Rect((float)this.x + 132f, 259f, 28f, 28f), ContentFinder<Texture2D>.Get("bmale", true), true, null))
                        {
                            CEditor.API.Pawn.SetPawnGender(Gender.Female);
                        }
                    }
                    else
                    {
                        Pawn pawn2 = CEditor.API.Pawn;
                        if (pawn2 != null && pawn2.gender == Gender.Female)
                        {
                            if (Widgets.ButtonImage(new Rect((float)this.x + 132f, 259f, 28f, 28f), ContentFinder<Texture2D>.Get("bfemale", true), true, null))
                            {
                                Pawn pawn5 = CEditor.API.Pawn;
                                Pawn pawn3 = CEditor.API.Pawn;
                                bool flag8;
                                if (pawn3 == null)
                                {
                                    flag8 = false;
                                }
                                else
                                {
                                    RaceProperties raceProps = pawn3.RaceProps;
                                    bool? flag9 = (raceProps != null) ? new bool?(raceProps.hasGenders) : null;
                                    bool flag10 = false;
                                    flag8 = (flag9.GetValueOrDefault() == flag10 & flag9 != null);
                                }

                                pawn5.SetPawnGender(flag8 ? Gender.None : Gender.Male);
                            }
                        }
                        else
                        {
                            Pawn pawn4 = CEditor.API.Pawn;
                            if (pawn4 != null && pawn4.gender == Gender.None && Widgets.ButtonImage(new Rect((float)this.x + 132f, 259f, 28f, 28f), ContentFinder<Texture2D>.Get("bnone", true), true, null))
                            {
                                CEditor.API.Pawn.SetPawnGender(Gender.Male);
                            }
                        }
                    }

                    SZWidgets.ButtonImageCol((float)(this.x + 160), 262f, 24f, 24f, "bnoclothes", new Action<Color>(this.AToggleNude), this.bShowClothes ? Color.white : Color.grey, "");
                    SZWidgets.ButtonImageCol((float)(this.x + 184), 260f, 24f, 24f, "bnohats", new Action<Color>(this.AToggleHats), this.bShowHat ? Color.white : Color.grey, "");
                    SZWidgets.ButtonImageCol((float)(this.x + 210), 262f, 24f, 24f, "brotate", new Action<Color>(this.ARotate), Color.white, "");
                    if (CEditor.IsPsychologyActive)
                    {
                        SZWidgets.ButtonImage((float)(this.x + 210), 232f, 24f, 24f, "bpsychology", new Action(this.AStartPsychologyUI), "", default(Color));
                    }

                    if (CEditor.IsFacialStuffActive)
                    {
                        SZWidgets.ButtonImage((float)(this.x + 2), 232f, 24f, 24f, "bfacial", new Action(this.AStartFacialEditorUI), "", default(Color));
                    }
                }


                private void DrawTop()
                {
                    Text.Font = GameFont.Small;
                    Widgets.Label(new Rect((float)(this.x + 5), (float)(this.y + 3), (float)this.w, 24f), CEditor.API.Pawn.GetPawnName(false));
                    this.y += 2;
                    bool isRandom = CEditor.IsRandom;
                    if (isRandom)
                    {
                        Widgets.DrawBoxSolid(new Rect((float)this.x, (float)(this.y - 2), (float)(this.x + this.w - 10), 26f), new Color(0.2f, 0.2f, 0.2f));
                        Rect rect = new Rect((float)(this.x + this.w - 238), (float)(this.y + 25), 24f, 24f);
                        SZWidgets.ButtonImageCol(rect, "bconfig", delegate() { CEditor.API.ConfigEditor(); }, Color.white, "");
                        Rect rect2 = new Rect((float)(this.x + this.w - 23), (float)(this.y + 25), 24f, 24f);
                        SZWidgets.ButtonImageCol(rect2, "bplus2", new Action(this.AAddPawn), Color.white, Label.TIP_ADD_PAWN);
                        Rect rect3 = new Rect((float)(this.x + this.w - 23), (float)(this.y + 52), 24f, 24f);
                        SZWidgets.ButtonImageCol(rect3, "bminus2", new Action(this.ARemovePawn), Color.white, Label.TIP_DELETE_PAWN);
                        Rect rect4 = new Rect((float)(this.x + this.w - 49), (float)this.y, 22f, 22f);
                        SZWidgets.ButtonImageCol(rect4, "brandom", new Action(this.ARandomizePawn), Color.white, (this.iTickTool <= 0) ? Label.TIP_RANDOMIZE_PAWN : "");
                        Rect rect5 = new Rect((float)(this.x + this.w - 76), (float)this.y, 22f, 22f);
                        SZWidgets.ButtonImageCol(rect5, "breplace", new Action(this.ARandomizePawnKeepRace), Color.white, (this.iTickTool <= 0) ? Label.TIP_RANDOMIZE_PAWNKEEPRACE : "");
                        Rect rect6 = new Rect((float)(this.x + this.w - 103), (float)this.y, 22f, 22f);
                        SZWidgets.ButtonImageCol(rect6, "bgender", new Action(this.ARandomizeBodyParts), Color.white, (this.iTickTool <= 0) ? Label.TIP_RANDOMIZE_BODYPARTS : "");
                        Rect rect7 = new Rect((float)(this.x + this.w - 130), (float)this.y, 22f, 22f);
                        SZWidgets.ButtonImageCol(rect7, "barmory", new Action(this.ARandomizeEquip), Color.white, (this.iTickTool <= 0) ? Label.TIP_RANDOMIZE_EQUIP : "");
                        Rect rect8 = new Rect((float)(this.x + this.w - 157), (float)this.y, 22f, 22f);
                        SZWidgets.ButtonImageCol(rect8, "bskills", new Action(this.ARandomizeBio), Color.white, (this.iTickTool <= 0) ? Label.TIP_RANDOMIZE_BIO : "");
                        Rect rect9 = new Rect((float)(this.x + this.w - 184), (float)this.y, 22f, 22f);
                        SZWidgets.ButtonImageCol(rect9, "bresurrect", new Action(this.AQuickResurrect), Color.white, (this.iTickTool <= 0) ? Label.TIP_QUICKHEAL : "");
                        Rect rect10 = new Rect((float)(this.x + this.w - 211), (float)this.y, 22f, 22f);
                        SZWidgets.ButtonImage(rect10, "bclone", new Action(this.AClonePawn), CEditor.InStartingScreen ? Label.TIP_CLONE.SubstringTo("\n", true) : Label.TIP_CLONE);
                        Rect rect11 = new Rect((float)(this.x + this.w - 238), (float)this.y, 22f, 22f);
                        SZWidgets.ButtonImage(rect11, "UI/Buttons/DevRoot/OpenInspector", new Action(this.AFindPawn), "");
                        bool flag = this.iTickTool > 0;
                        if (flag)
                        {
                            this.iTickTool--;
                        }
                    }

                    SZWidgets.ButtonImage((float)(this.x + this.w - 24), (float)this.y, 24f, 24f, CEditor.IsRandom ? "bractive" : "brinactive", new Action(this.AToggleR), Label.SHOWCREATION, default(Color));
                    bool inStartingScreen = CEditor.InStartingScreen;
                    if (inStartingScreen)
                    {
                        SZWidgets.ButtonImage((float)this.x, (float)(this.y + 50), 24f, 24f, "bmoveup", new Action(this.AMoveUp), "", default(Color));
                        SZWidgets.ButtonImage((float)this.x, (float)(this.y + 75), 24f, 24f, "bmovedown", new Action(this.AMoveDown), "", default(Color));
                    }
                }


                private void AAddPawn()
                {
                    Faction value = CEditor.API.DicFactions.GetValue(CEditor.ListName);
                    List<PawnKindDef> lpkd = PawnKindTool.ListOfPawnKindDef(CEditor.API.DicFactions.GetValue(CEditor.ListName), CEditor.ListName, this.selectedModName);
                    PawnxTool.AddOrCreateNewPawn(CEditor.PKD.ThisOrFromList(lpkd), value, CEditor.RACE, null, default(IntVec3), false, Gender.None);
                }


                private void ARemovePawn()
                {
                    CEditor.API.Pawn.Delete(false);
                }


                private void ABeginTeleportSelectPawn()
                {
                    CEditor.API.EditorMoveRight();
                    bool control = Event.current.control;
                    if (control)
                    {
                        PlacingTool.BeginTeleportCustomPawn();
                    }
                    else
                    {
                        PlacingTool.BeginTeleportPawn(CEditor.API.Pawn);
                    }
                }


                private void AClonePawn()
                {
                    PresetPawn ppn = CEditor.API.Pawn.ClonePawn();
                    PawnxTool.AddOrCreateExistingPawn(ppn);
                }


                private void AMoveDown()
                {
                    int num = CEditor.API.ListOf<Pawn>(EType.Pawns).IndexOf(CEditor.API.Pawn);
                    int num2 = CEditor.API.ListOf<Pawn>(EType.Pawns).Count - 1;
                    num = ((num < num2) ? (num + 1) : num2);
                    CEditor.API.ListOf<Pawn>(EType.Pawns).Remove(CEditor.API.Pawn);
                    CEditor.API.ListOf<Pawn>(EType.Pawns).Insert(num, CEditor.API.Pawn);
                    PawnxTool.Notify_CheckStartPawnsListChanged();
                }


                private void AMoveUp()
                {
                    int num = CEditor.API.ListOf<Pawn>(EType.Pawns).IndexOf(CEditor.API.Pawn);
                    num = ((num > 0) ? (num - 1) : 0);
                    CEditor.API.ListOf<Pawn>(EType.Pawns).Remove(CEditor.API.Pawn);
                    CEditor.API.ListOf<Pawn>(EType.Pawns).Insert(num, CEditor.API.Pawn);
                    PawnxTool.Notify_CheckStartPawnsListChanged();
                }


                private void AQuickResurrect()
                {
                    bool alt = Event.current.alt;
                    if (alt)
                    {
                        HealthUtility.HealNonPermanentInjuriesAndRestoreLegs(CEditor.API.Pawn);
                    }

                    bool shift = Event.current.shift;
                    if (shift)
                    {
                        CEditor.API.Pawn.Medicate();
                    }

                    bool control = Event.current.control;
                    if (control)
                    {
                        CEditor.API.Pawn.Anaesthetize();
                    }

                    bool capsLock = Event.current.capsLock;
                    if (capsLock)
                    {
                        CEditor.API.Pawn.DamageUntilDeath();
                    }

                    bool flag = !Event.current.alt && !Event.current.shift && !Event.current.control && !Event.current.capsLock;
                    if (flag)
                    {
                        CEditor.API.Pawn.ResurrectAndHeal();
                    }

                    CEditor.API.UpdateGraphics();
                }


                private void ARandomizeBio()
                {
                    this.iTickTool = 100;
                    CEditor.API.Pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(CEditor.API.Pawn, NameStyle.Full, null, false, null);
                    CEditor.API.Pawn.SetBackstory(true, true, true, false);
                    CEditor.API.Pawn.SetBackstory(true, true, false, false);
                    CEditor.API.Pawn.SetAge(CEditor.zufallswert.Next(CEditor.API.Pawn.RaceProps.Animal ? 14 : 70));
                    CEditor.API.Pawn.SetChronoAge(CEditor.zufallswert.Next(2000));
                    bool flag = CEditor.API.Pawn.skills != null;
                    if (flag)
                    {
                        foreach (SkillRecord skillRecord in CEditor.API.Pawn.skills.skills)
                        {
                            bool flag2 = !skillRecord.TotallyDisabled;
                            if (flag2)
                            {
                                int maxValue = CEditor.zufallswert.Next(0, 21);
                                skillRecord.Level = CEditor.zufallswert.Next(0, maxValue);
                                skillRecord.passion = (Passion)CEditor.zufallswert.Next(0, 3);
                            }
                        }
                    }

                    bool flag3 = CEditor.API.Pawn.story != null;
                    if (flag3)
                    {
                        CEditor.API.Pawn.story.traits.allTraits.Clear();
                        int num = CEditor.zufallswert.Next(0, 11);
                        for (int i = 0; i < num; i++)
                        {
                            CEditor.API.Pawn.AddTrait(null, null, true, true, null);
                        }
                    }
                }


                private void ARandomizeBodyParts()
                {
                    this.iTickTool = 100;
                    bool alt = Event.current.alt;
                    if (alt)
                    {
                        CEditor.API.Pawn.gender = Gender.Female;
                    }
                    else
                    {
                        bool capsLock = Event.current.capsLock;
                        if (capsLock)
                        {
                            CEditor.API.Pawn.gender = Gender.Male;
                        }
                        else
                        {
                            CEditor.API.Pawn.gender = ((CEditor.zufallswert.Next(100) > 50) ? Gender.Male : Gender.Female);
                        }
                    }

                    CEditor.API.Pawn.SetHead(true, true);
                    CEditor.API.Pawn.SetBody(true, true);
                    bool flag = CEditor.API.Pawn.story != null;
                    if (flag)
                    {
                        bool flag2 = Event.current.alt || Event.current.capsLock;
                        if (flag2)
                        {
                            PawnxTool.ForceGenderizedBodyTypeDef(CEditor.API.Pawn.gender);
                        }

                        bool flag3 = !CEditor.API.Pawn.story.bodyType.IsFromMod("Alien Vs Predator");
                        if (flag3)
                        {
                            CEditor.API.Pawn.SetHair(true, true, null);
                            this.ARandomHairColor();
                        }
                    }

                    bool flag4 = CEditor.API.Pawn.HasStyleTracker();
                    if (flag4)
                    {
                        int num = CEditor.zufallswert.Next(100);
                        bool flag5 = num > 50 || CEditor.API.Pawn.gender == Gender.Female;
                        if (flag5)
                        {
                            CEditor.API.Pawn.SetBeard(BeardDefOf.NoBeard);
                        }
                        else
                        {
                            CEditor.API.Pawn.SetBeard(true, true);
                        }

                        CEditor.API.Pawn.SetFaceTattoo(true, true);
                        CEditor.API.Pawn.SetBodyTattoo(true, true);
                    }

                    CEditor.API.UpdateGraphics();
                }


                private void ARandomizeEquip()
                {
                    this.iTickTool = 100;
                    bool flag = !Event.current.alt && !Event.current.shift;
                    bool flag2 = Event.current.alt || flag;
                    if (flag2)
                    {
                        CEditor.API.Pawn.Redress(null, false, -1, false);
                    }

                    bool flag3 = Event.current.shift || flag;
                    if (flag3)
                    {
                        CEditor.API.Pawn.Reequip(null, -1, false);
                    }

                    bool control = Event.current.control;
                    if (control)
                    {
                        Pawn_ApparelTracker apparel = CEditor.API.Pawn.apparel;
                        bool flag4 = apparel != null && apparel.WornApparelCount > 0;
                        if (flag4)
                        {
                            foreach (Apparel apparel2 in CEditor.API.Pawn.apparel.WornApparel)
                            {
                                apparel2.DrawColor = ColorTool.RandomAlphaColor;
                            }
                        }
                    }

                    CEditor.API.UpdateGraphics();
                }


                private void ARandomizePawn()
                {
                    this.iTickTool = 100;
                    Faction value = CEditor.API.DicFactions.GetValue(CEditor.ListName);
                    IntVec3 position = CEditor.API.Pawn.Position;
                    CEditor.API.Pawn.Delete(false);
                    PawnxTool.AddOrCreateNewPawn(CEditor.PKD.ThisOrFromList(this.GetPawnKindDefs()), value, CEditor.RACE, null, position, false, Gender.None);
                }


                private void ARandomizePawnKeepRace()
                {
                    this.iTickTool = 100;
                    Faction value = CEditor.API.DicFactions.GetValue(CEditor.ListName);
                    bool alt = Event.current.alt;
                    if (alt)
                    {
                        PawnxTool.ReplacePawnWithPawnOfSameRace(Gender.Female, value);
                    }
                    else
                    {
                        bool capsLock = Event.current.capsLock;
                        if (capsLock)
                        {
                            PawnxTool.ReplacePawnWithPawnOfSameRace(Gender.Male, value);
                        }
                        else
                        {
                            PawnxTool.ReplacePawnWithPawnOfSameRace(Gender.None, value);
                        }
                    }
                }


                private void AToggleR()
                {
                    CEditor.IsRandom = !CEditor.IsRandom;
                }


                private List<PawnKindDef> GetPawnKindDefs()
                {
                    return PawnKindTool.ListOfPawnKindDef(CEditor.API.DicFactions.GetValue(CEditor.ListName), CEditor.ListName, this.selectedModName);
                }


                private void AChooseNextPawn()
                {
                    int index = CEditor.API.ListOf<Pawn>(EType.Pawns).IndexOf(CEditor.API.Pawn);
                    index = CEditor.API.ListOf<Pawn>(EType.Pawns).NextOrPrevIndex(index, true, false);
                    CEditor.API.Pawn = CEditor.API.ListOf<Pawn>(EType.Pawns)[index];
                }


                private void AChoosePrevPawn()
                {
                    int index = CEditor.API.ListOf<Pawn>(EType.Pawns).IndexOf(CEditor.API.Pawn);
                    index = CEditor.API.ListOf<Pawn>(EType.Pawns).NextOrPrevIndex(index, false, false);
                    CEditor.API.Pawn = CEditor.API.ListOf<Pawn>(EType.Pawns)[index];
                }


                private void AChangeHeadAddons()
                {
                    WindowTool.Open(new DialogChangeHeadAddons());
                }


                private void AFindPawn()
                {
                    WindowTool.Open(new DialogFindPawn());
                }


                private void AJumpToPawn()
                {
                    CEditor.API.Get<CEditor.EditorUI>(EType.EditorUI).windowRect.position = new Vector2((float)(UI.screenWidth / 2 + 200), (float)(UI.screenHeight / 2) - CEditor.API.Get<CEditor.EditorUI>(EType.EditorUI).InitialSize.y / 2f);
                    CameraJumper.TryJumpAndSelect(CEditor.API.Pawn, CameraJumper.MovementMode.Pan);
                }


                private void ALoadPawn()
                {
                    List<FloatMenuOption> floatMenuOptionList = new List<FloatMenuOption>();
                    int numSlots = API.NumSlots;
                    for (int index = 0; index < numSlots; ++index)
                    {
                        int currentSlot = index;
                        FloatMenuOption floatMenuOption = new FloatMenuOption(this.SlotLabel(currentSlot, false), (() => new PresetPawn().LoadPawn(currentSlot, true)));
                        floatMenuOptionList.Add(floatMenuOption);
                    }

                    WindowTool.Open(new FloatMenu(floatMenuOptionList));
                }

                private void ARotate(Color col)
                {
                    CEditor.API.Get<Capturer>(EType.Capturer).RotateAndCapture(CEditor.API.Pawn);
                }


                private void ASavePawn()
                {
                    List<FloatMenuOption> floatMenuOptionList = new List<FloatMenuOption>();
                    int numSlots = CEditor.API.NumSlots;
                    for (int index = 0; index < numSlots; ++index)
                    {
                        int currentSlot = index;
                        floatMenuOptionList.Add(new FloatMenuOption(SlotLabel(currentSlot, true), () =>
                        {
                            int num = currentSlot;
                            try
                            {
                                if (Event.current.control)
                                    CEditor.API.SetSlot(num, "", true);
                                else
                                    new PresetPawn().SavePawn(CEditor.API.Pawn, num);
                            }
                            catch (Exception e)
                            {
                                Log.Message("Got exception while saving!");
                                Log.Message(e);
                            }
                        }));
                    }

                    WindowTool.Open(new FloatMenu(floatMenuOptionList));
                }


                private void AStartFacialEditorUI()
                {
                    WindowTool.Open(new DialogFacialStuff());
                }


                private void AStartPsychologyUI()
                {
                    WindowTool.Open(new DialogPsychology());
                }


                private void AToggleHats(Color col)
                {
                    Capturer capturer = CEditor.API.Get<Capturer>(EType.Capturer);
                    bool bHats = capturer.bHats;
                    if (bHats)
                    {
                        List<ApparelLayerDef> list = ApparelTool.ListOfApparelLayerDefs(false);
                        list = (from td in list
                            where !td.defName.NullOrEmpty() && td.defName.ToLower().Contains("head")
                            select td).ToList<ApparelLayerDef>();
                        foreach (ApparelLayerDef layerOnly in list)
                        {
                            CEditor.API.Pawn.MoveDressToInv(layerOnly);
                        }
                    }
                    else
                    {
                        List<ApparelLayerDef> list2 = ApparelTool.ListOfApparelLayerDefs(false);
                        list2 = (from td in list2
                            where !td.defName.NullOrEmpty() && td.defName.ToLower().Contains("head")
                            select td).ToList<ApparelLayerDef>();
                        foreach (ApparelLayerDef layerOnly2 in list2)
                        {
                            CEditor.API.Pawn.MoveDressFromInv(layerOnly2);
                        }
                    }

                    capturer.ToggleHatAndCapture(CEditor.API.Pawn);
                    this.bShowHat = capturer.bHats;
                }


                private void AToggleNude(Color col)
                {
                    Capturer capturer = CEditor.API.Get<Capturer>(EType.Capturer);
                    bool bNude = capturer.bNude;
                    if (bNude)
                    {
                        CEditor.API.Pawn.MoveDressToInv(null);
                    }
                    else
                    {
                        CEditor.API.Pawn.MoveDressFromInv(null);
                    }

                    capturer.ToggleNudeAndCapture(CEditor.API.Pawn);
                    this.bShowClothes = capturer.bNude;
                }


                private string SlotLabel(int slot, bool isSave)
                {
                    string text = (isSave ? Label.SAVESLOT : Label.LOADSLOT) + slot.ToString() + " ";
                    string slot2 = CEditor.API.GetSlot(slot);
                    bool flag = slot2.Contains("*");
                    if (flag)
                    {
                        text += slot2.SubstringTo("*", true);
                    }
                    else
                    {
                        text += slot2.SubstringTo(",", true);
                    }

                    return text;
                }


                private void AOnHeadOpen()
                {
                    this.bIsHeadOpen = !this.bIsHeadOpen;
                }


                private void AOnHeadOpen2()
                {
                    CEditor.IsRaceSpecificHead = !CEditor.IsRaceSpecificHead;
                }


                private void AFACustomSkin()
                {
                    SZWidgets.FloatMenuOnRect<string>(CEditor.API.Pawn.FA_GetDefStringList(FacialTool.SKIN), (string s) => s, new Action<string>(this.AFASetCustomSkin), null, true);
                }


                private void AFARandomSkin()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.SKIN, true, true);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetCustomSkin(string val)
                {
                    CEditor.API.Pawn.FA_SetDefByName(FacialTool.SKIN, val);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetNextSkin()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.SKIN, true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetPrevSkin()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.SKIN, false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFACustomMouth()
                {
                    SZWidgets.FloatMenuOnRect<string>(CEditor.API.Pawn.FA_GetDefStringList(FacialTool.MOUTH), (string s) => s, new Action<string>(this.AFASetCustomMouth), null, true);
                }


                private void AFARandomMouth()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.MOUTH, true, true);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetCustomMouth(string val)
                {
                    CEditor.API.Pawn.FA_SetDefByName(FacialTool.MOUTH, val);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetNextMouth()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.MOUTH, true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetPrevMouth()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.MOUTH, false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFACustomBrow()
                {
                    SZWidgets.FloatMenuOnRect<string>(CEditor.API.Pawn.FA_GetDefStringList(FacialTool.BROW), (string s) => s, new Action<string>(this.AFASetCustomBrow), null, true);
                }


                private void AFARandomBrow()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.BROW, true, true);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetCustomBrow(string val)
                {
                    CEditor.API.Pawn.FA_SetDefByName(FacialTool.BROW, val);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetNextBrow()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.BROW, true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetPrevBrow()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.BROW, false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFACustomLid()
                {
                    SZWidgets.FloatMenuOnRect<string>(CEditor.API.Pawn.FA_GetDefStringList(FacialTool.LID), (string s) => s, new Action<string>(this.AFASetCustomLid), null, true);
                }


                private void AFARandomLid()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.LID, true, true);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetCustomLid(string val)
                {
                    CEditor.API.Pawn.FA_SetDefByName(FacialTool.LID, val);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetNextLid()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.LID, true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetPrevLid()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.LID, false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFACustomEye()
                {
                    SZWidgets.FloatMenuOnRect<string>(CEditor.API.Pawn.FA_GetDefStringList(FacialTool.EYE), (string s) => s, new Action<string>(this.AFASetCustomEye), null, true);
                }


                private void AFARandomEye()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.EYE, true, true);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetCustomEye(string val)
                {
                    CEditor.API.Pawn.FA_SetDefByName(FacialTool.EYE, val);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetNextEye()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.EYE, true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetPrevEye()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.EYE, false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFACustomHead()
                {
                    SZWidgets.FloatMenuOnRect<string>(CEditor.API.Pawn.FA_GetDefStringList(FacialTool.FACE), (string s) => s, new Action<string>(this.AFASetCustomFace), null, true);
                }


                private void AFARandomHead()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.FACE, true, true);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetCustomFace(string val)
                {
                    CEditor.API.Pawn.FA_SetDefByName(FacialTool.FACE, val);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetNextHead()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.FACE, true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AFASetPrevHead()
                {
                    CEditor.API.Pawn.FA_SetDef(FacialTool.FACE, false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AChooseHeadCustom()
                {
                    SZWidgets.FloatMenuOnRect<HeadTypeDef>(CEditor.API.Pawn.GetHeadDefList(false), (HeadTypeDef s) => s.label.NullOrEmpty() ? s.defName : s.label.ToString(), new Action<HeadTypeDef>(this.ASetHeadDefCustom), null, true);
                }


                private void ARandomHead()
                {
                    CEditor.API.Pawn.SetHead(true, true);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetHeadDefCustom(HeadTypeDef val)
                {
                    CEditor.API.Pawn.SetHeadTypeDef(val);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetNextHead()
                {
                    CEditor.API.Pawn.SetHead(true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetPrevHead()
                {
                    CEditor.API.Pawn.SetHead(false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void DrawEyeColorSelection()
                {
                    SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AChangeEyeUI), new Action(this.ARandomEyeColor), null, null, null, new Action(this.AConfigEyeColor), Label.EYECOLOR, null, null, null, null, default(Color), null);
                    this.y += 30;
                    bool flag = this.isEyeConfigOpen;
                    if (flag)
                    {
                        SZWidgets.ColorBox(new Rect((float)(this.x + 25), (float)this.y, (float)(this.w - 50), 95f), CEditor.API.Pawn.GetEyeColor(), new Action<Color>(this.AOnEyeColorChanged), false);
                        this.y += 95;
                    }
                }


                private void AChangeEyeUI()
                {
                    WindowTool.Open(new DialogColorPicker(ColorType.EyeColor, true, null, null, null));
                }


                private void AConfigEyeColor()
                {
                    this.isEyeConfigOpen = !this.isEyeConfigOpen;
                }


                private void AOnEyeColorChanged(Color col)
                {
                    CEditor.API.Pawn.SetEyeColor(col);
                    CEditor.API.UpdateGraphics();
                }


                private void ARandomEyeColor()
                {
                    CEditor.API.Pawn.SetEyeColor(ColorTool.RandomColor);
                    CEditor.API.UpdateGraphics();
                }


                private void DrawFaceTattooSelector()
                {
                    bool flag = !CEditor.API.Pawn.HasStyleTracker();
                    if (!flag)
                    {
                        SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AChooseFaceTattooCustom), new Action(this.ARandomFaceTattoo), new Action(this.ASetPrevFaceTattoo), new Action(this.ASetNextFaceTattoo), null, null, Label.TATTOO + CEditor.API.Pawn.GetFaceTattooName(), null, Label.TIP_RANDOM_FACETATTOO, null, null, default(Color), null);
                        this.y += 30;
                    }
                }


                private void AChooseFaceTattooCustom()
                {
                    SZWidgets.FloatMenuOnRect<TattooDef>(StyleTool.GetFaceTattooList(null), (TattooDef s) => s.LabelCap, new Action<TattooDef>(this.ASetFaceTattooCustom), null, true);
                }


                private void AConfigFaceTattoo()
                {
                    this.isFaceTattooConfigOpen = !this.isFaceTattooConfigOpen;
                }


                private void AFaceTattooSelected(TattooDef tattooDef)
                {
                    CEditor.API.Pawn.SetFaceTattoo(tattooDef);
                    CEditor.API.UpdateGraphics();
                }


                private void ARandomFaceTattoo()
                {
                    CEditor.API.Pawn.SetFaceTattoo(true, true);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetFaceTattooCustom(TattooDef tattooDef)
                {
                    CEditor.API.Pawn.SetFaceTattoo(tattooDef);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetNextFaceTattoo()
                {
                    CEditor.API.Pawn.SetFaceTattoo(true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetPrevFaceTattoo()
                {
                    CEditor.API.Pawn.SetFaceTattoo(false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void AOnHairOpen()
                {
                    this.bIsHairOpen = !this.bIsHairOpen;
                }


                private void DrawBeardSelector()
                {
                    bool flag = !CEditor.API.Pawn.HasStyleTracker();
                    if (!flag)
                    {
                        SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(StyleTool.AChooseBeardCustom), new Action(StyleTool.ARandomBeard), new Action(this.ASetPrevBeard), new Action(this.ASetNextBeard), null, new Action(this.AConfigBeard), Label.BEARD + " - " + CEditor.API.Pawn.GetBeardName(), null, Label.TIP_RANDOM_BEARD, null, null, default(Color), null);
                        this.y += 30;
                        bool flag2 = this.isBeardConfigOpen;
                        if (flag2)
                        {
                            SZWidgets.FloatMenuOnButtonText<string>(new Rect((float)(this.x + 16), (float)this.y, (float)(this.w - 32), 25f), this.selectedModName ?? Label.ALL, CEditor.API.Get<HashSet<string>>(EType.ModsBeardDef), (string s) => s ?? Label.ALL, new Action<string>(this.ASelectedModName), "");
                            SZWidgets.ListView<BeardDef>(new Rect((float)(this.x + 16), (float)(this.y + 25), (float)(this.w - 32), 330f), this.lOfBeardDefs, (BeardDef hd) => hd.LabelCap, (BeardDef hd) => hd.description, (BeardDef beardA, BeardDef beardB) => beardA == beardB, ref CEditor.API.Pawn.style.beardDef, ref this.scrollPos, false, new Action<BeardDef>(this.ABeardSelected), true, false, false, false);
                            this.y += 357;
                        }
                    }
                }


                private void ABeardSelected(BeardDef b)
                {
                    CEditor.API.Pawn.SetBeard(b);
                    CEditor.API.UpdateGraphics();
                }


                private void AConfigBeard()
                {
                    this.isBeardConfigOpen = !this.isBeardConfigOpen;
                }


                private void ASetNextBeard()
                {
                    CEditor.API.Pawn.SetBeard(true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetPrevBeard()
                {
                    CEditor.API.Pawn.SetBeard(false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void DrawGradientSelector()
                {
                    bool isGradientHairActive = CEditor.IsGradientHairActive;
                    if (isGradientHairActive)
                    {
                        SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AChooseGradientCustom), new Action(this.ARandomGradient), new Action(this.ASetPrevGradient), new Action(this.ASetNextGradient), null, null, CEditor.API.Pawn.GetGradientMask(), null, null, null, null, default(Color), null);
                        this.y += 30;
                    }
                }


                private void DrawHairColorSelector()
                {
                    SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AChangeHairUI), new Action(this.ARandomHairColor), new Action(this.AToggleColor), new Action(this.AToggleColor), null, new Action(this.AConfigHaircolor), Label.HAIRCOLOR + " - " + (this.isPrimaryColor ? Label.COLORA : Label.COLORB), null, Label.TIP_RANDOM_HAIRCOLOR, null, null, default(Color), null);
                    this.y += 30;
                    bool flag = this.isHaircolorConfigOpen;
                    if (flag)
                    {
                        SZWidgets.ColorBox(new Rect((float)(this.x + 25), (float)this.y, (float)(this.w - 50), 95f), CEditor.API.Pawn.GetHairColor(this.isPrimaryColor), new Action<Color>(this.AOnHairColorChanged), false);
                        this.y += 95;
                    }
                }


                private void DrawHairSelector()
                {
                    SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(HairTool.AChooseHairCustom), new Action(HairTool.ARandomHair), new Action(this.ASetPrevHair), new Action(this.ASetNextHair), null, new Action(this.AConfigHair), Label.FRISUR + " - " + CEditor.API.Pawn.GetHairName(), null, Label.TIP_RANDOM_HAIR, null, null, default(Color), null);
                    this.y += 30;
                    bool flag = this.isHairConfigOpen;
                    if (flag)
                    {
                        SZWidgets.FloatMenuOnButtonText<string>(new Rect((float)(this.x + 16), (float)this.y, (float)(this.w - 32), 25f), this.selectedModName ?? Label.ALL, CEditor.API.Get<HashSet<string>>(EType.ModsHairDef), (string s) => s ?? Label.ALL, new Action<string>(this.ASelectedModName), "");
                        SZWidgets.ListView<HairDef>(new Rect((float)(this.x + 16), (float)(this.y + 25), (float)(this.w - 32), 360f), this.lOfHairDefs, (HairDef hd) => hd.LabelCap, (HairDef hd) => hd.description, (HairDef hairA, HairDef hairB) => hairA == hairB, ref CEditor.API.Pawn.story.hairDef, ref this.scrollPos, false, new Action<HairDef>(this.AHairSelected), true, false, false, false);
                        this.y += 387;
                    }
                }


                private void AConfigHair()
                {
                    this.isHairConfigOpen = !this.isHairConfigOpen;
                }


                private void AHairSelected(HairDef h)
                {
                    CEditor.API.Pawn.SetHair(h);
                    CEditor.API.UpdateGraphics();
                }


                private void ASelectedModName(string val)
                {
                    this.selectedModName = val;
                    this.lOfHairDefs = HairTool.GetHairList(this.selectedModName);
                    this.lOfBeardDefs = StyleTool.GetBeardList(this.selectedModName);
                }


                private void ASetNextHair()
                {
                    WindowTool.TryRemove<DialogColorPicker>();
                    CEditor.API.Pawn.SetHair(true, false, null);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetPrevHair()
                {
                    WindowTool.TryRemove<DialogColorPicker>();
                    CEditor.API.Pawn.SetHair(false, false, null);
                    CEditor.API.UpdateGraphics();
                }


                private void AChangeHairUI()
                {
                    WindowTool.Open(new DialogColorPicker(ColorType.HairColor, this.isPrimaryColor, null, null, null));
                }


                private void AConfigHaircolor()
                {
                    this.isHaircolorConfigOpen = !this.isHaircolorConfigOpen;
                }


                private void AOnHairColorChanged(Color col)
                {
                    CEditor.API.Pawn.SetHairColor(this.isPrimaryColor, col);
                    CEditor.API.UpdateGraphics();
                }


                private void ARandomHairColor()
                {
                    bool flag = !Event.current.alt && !Event.current.shift;
                    bool flag2 = flag || Event.current.shift;
                    if (flag2)
                    {
                        CEditor.API.Pawn.SetHairColor(false, Event.current.control ? ColorTool.RandomAlphaColor : ColorTool.RandomColor);
                    }

                    bool flag3 = flag || Event.current.alt;
                    if (flag3)
                    {
                        CEditor.API.Pawn.SetHairColor(true, Event.current.control ? ColorTool.RandomAlphaColor : ColorTool.RandomColor);
                    }

                    CEditor.API.UpdateGraphics();
                }


                private void AToggleColor()
                {
                    this.isPrimaryColor = !this.isPrimaryColor;
                }


                private void AChooseGradientCustom()
                {
                    SZWidgets.FloatMenuOnRect<string>(HairTool.GetAllGradientHairs(), (string s) => s, new Action<string>(this.ASetHairCustom), null, true);
                }


                private void ARandomGradient()
                {
                    CEditor.API.Pawn.RandomizeGradientMask();
                }


                private void ASetHairCustom(string val)
                {
                    CEditor.API.Pawn.SetGradientMask(val);
                }


                private void ASetNextGradient()
                {
                    List<string> allGradientHairs = HairTool.GetAllGradientHairs();
                    int index = allGradientHairs.IndexOf(CEditor.API.Pawn.GetGradientMask());
                    index = allGradientHairs.NextOrPrevIndex(index, true, false);
                    CEditor.API.Pawn.SetGradientMask(allGradientHairs[index]);
                }


                private void ASetPrevGradient()
                {
                    List<string> allGradientHairs = HairTool.GetAllGradientHairs();
                    int index = allGradientHairs.IndexOf(CEditor.API.Pawn.GetGradientMask());
                    index = allGradientHairs.NextOrPrevIndex(index, false, false);
                    CEditor.API.Pawn.SetGradientMask(allGradientHairs[index]);
                }


                private void AOnBodyOpen()
                {
                    this.bIsBodyOpen = !this.bIsBodyOpen;
                }


                private void DrawBodySelector()
                {
                    SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AChooseBodyCustom), new Action(this.ARandomBody), new Action(this.ASetPrevBody), new Action(this.ASetNextBody), null, null, Label.FORM + ": " + CEditor.API.Pawn.GetBodyTypeName(), null, Label.TIP_RANDOM_BODY, null, null, default(Color), null);
                    this.y += 30;
                }


                private void AChooseBodyCustom()
                {
                    SZWidgets.FloatMenuOnRect<BodyTypeDef>(CEditor.API.Pawn.GetBodyDefList(false), (BodyTypeDef s) => s.defName.Translate(), new Action<BodyTypeDef>(this.ASetBodyCustom), null, true);
                }


                private void ARandomBody()
                {
                    CEditor.API.Pawn.SetBody(true, true);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetBodyCustom(BodyTypeDef b)
                {
                    CEditor.API.Pawn.SetBody(b);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetNextBody()
                {
                    CEditor.API.Pawn.SetBody(true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetPrevBody()
                {
                    CEditor.API.Pawn.SetBody(false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void DrawBodyTattooSelector()
                {
                    bool flag = !CEditor.API.Pawn.HasStyleTracker();
                    if (!flag)
                    {
                        SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AChooseBodyTattooCustom), new Action(this.ARandomBodyTattoo), new Action(this.ASetPrevBodyTattoo), new Action(this.ASetNextBodyTattoo), null, null, Label.TATTOO + CEditor.API.Pawn.GetBodyTattooName(), null, Label.TIP_RANDOM_BODYTATTOO, null, null, default(Color), null);
                        this.y += 30;
                    }
                }


                private void ABodyTattooSelected(TattooDef tattooDef)
                {
                    CEditor.API.Pawn.SetBodyTattoo(tattooDef);
                    CEditor.API.UpdateGraphics();
                }


                private void AChooseBodyTattooCustom()
                {
                    SZWidgets.FloatMenuOnRect<TattooDef>(StyleTool.GetBodyTattooList(null), (TattooDef s) => s.LabelCap, new Action<TattooDef>(this.ASetBodyTattooCustom), null, true);
                }


                private void AConfigBodyTattoo()
                {
                    this.isBodyTattooConfigOpen = !this.isBodyTattooConfigOpen;
                }


                private void ARandomBodyTattoo()
                {
                    CEditor.API.Pawn.SetBodyTattoo(true, true);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetBodyTattooCustom(TattooDef tattooDef)
                {
                    CEditor.API.Pawn.SetBodyTattoo(tattooDef);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetNextBodyTattoo()
                {
                    CEditor.API.Pawn.SetBodyTattoo(true, false);
                    CEditor.API.UpdateGraphics();
                }


                private void ASetPrevBodyTattoo()
                {
                    CEditor.API.Pawn.SetBodyTattoo(false, false);
                    CEditor.API.UpdateGraphics();
                }


                private void DrawSkinColorSelector()
                {
                    SZWidgets.NavSelectorImageBox(this.RectFullSolid, new Action(this.AChangeSkinUI), new Action(this.ARandomSkinColor), new Action(this.AToggleSkinColor), new Action(this.AToggleSkinColor), null, new Action(this.AConfigSkincolor), Label.SKIN + (this.isPrimarySkinColor ? Label.COLORA : Label.COLORB), null, Label.TIP_RANDOM_SKINCOLOR, null, null, default(Color), null);
                    this.y += 30;
                    bool flag = this.isSkinConfigOpen;
                    if (flag)
                    {
                        SZWidgets.ColorBox(new Rect((float)(this.x + 25), (float)this.y, (float)(this.w - 50), 95f), CEditor.API.Pawn.GetSkinColor(this.isPrimarySkinColor), new Action<Color>(this.AOnSkinColorChanged), false);
                        this.y += 95;
                    }
                }


                private void AChangeSkinUI()
                {
                    WindowTool.Open(new DialogColorPicker(ColorType.SkinColor, this.isPrimaryColor, null, null, null));
                }


                private void AConfigSkincolor()
                {
                    this.isSkinConfigOpen = !this.isSkinConfigOpen;
                }


                private void AOnMelaninChanged(float val)
                {
                }


                private void AOnSkinColorChanged(Color col)
                {
                    CEditor.API.Pawn.SetSkinColor(this.isPrimarySkinColor, col);
                    CEditor.API.UpdateGraphics();
                }


                private void AOnSkinColorChanged2(Color col)
                {
                    CEditor.API.Pawn.story.SkinColorBase = col;
                    CEditor.API.UpdateGraphics();
                }


                private void ARandomSkinColor()
                {
                    bool flag = !Event.current.alt && !Event.current.shift;
                    bool flag2 = flag || Event.current.alt;
                    if (flag2)
                    {
                        bool flag3 = CEditor.API.Pawn.story != null;
                        if (flag3)
                        {
                            CEditor.API.Pawn.story.SkinColorBase = ColorTool.RandomColor;
                        }

                        bool flag4 = this.isAlien;
                        if (flag4)
                        {
                            bool control = Event.current.control;
                            if (control)
                            {
                                CEditor.API.Pawn.SetSkinColor(true, ColorTool.RandomAlphaColor);
                            }
                            else
                            {
                                CEditor.API.Pawn.SetSkinColor(true, ColorTool.RandomColor);
                            }
                        }
                    }

                    bool flag5 = flag || Event.current.shift;
                    if (flag5)
                    {
                        bool flag6 = this.isAlien;
                        if (flag6)
                        {
                            bool control2 = Event.current.control;
                            if (control2)
                            {
                                CEditor.API.Pawn.SetSkinColor(false, ColorTool.RandomAlphaColor);
                            }
                            else
                            {
                                CEditor.API.Pawn.SetSkinColor(false, ColorTool.RandomColor);
                            }
                        }
                    }

                    CEditor.API.UpdateGraphics();
                }


                private void AToggleSkinColor()
                {
                    this.isPrimarySkinColor = !this.isPrimarySkinColor;
                }


                private void AChangeApparelUI(Apparel a)
                {
                    WindowTool.Open(new DialogColorPicker(ColorType.ApparelColor, true, a, null, null));
                }


                private void AConfigApparelcolor(Apparel a)
                {
                    this.isApparelConfigOpen = !this.isApparelConfigOpen;
                    this.apparelCurrent = a;
                }


                private void AOnApparelColorChanged(Color col)
                {
                    this.apparelCurrent.DrawColor = col;
                    CEditor.API.UpdateGraphics();
                }


                private void AOnTextureApparel(Apparel a)
                {
                    WindowTool.Open(new DialogObjects(DialogType.Apparel, null, a, false));
                }


                private void ARandomApparel(Apparel a)
                {
                    this.NextOrRandomApparel(a, true, true, false);
                }


                private void ASetNextApparel(Apparel a)
                {
                    this.NextOrRandomApparel(a, true, false, false);
                }


                private void ASetPrevApparel(Apparel a)
                {
                    this.NextOrRandomApparel(a, false, false, false);
                }


                private ApparelLayerDef GetBestLayer(List<ApparelLayerDef> l)
                {
                    return l.NullOrEmpty<ApparelLayerDef>() ? null : (l.Contains(ApparelLayerDefOf.Shell) ? ApparelLayerDefOf.Shell : (l.Contains(ApparelLayerDefOf.Middle) ? ApparelLayerDefOf.Middle : l.LastOrDefault<ApparelLayerDef>()));
                }


                private int GetDrawOrder(Apparel ap)
                {
                    ApparelLayerDef bestLayer = this.GetBestLayer(ap.def.apparel.layers);
                    return (bestLayer == null) ? 0 : bestLayer.drawOrder;
                }


                private ApparelLayerDef GetFreeLayer(List<ApparelLayerDef> lUsedLayer, Apparel a)
                {
                    ApparelLayerDef apparelLayerDef = this.GetBestLayer(a.def.apparel.layers);
                    bool flag = lUsedLayer.Contains(apparelLayerDef);
                    if (flag)
                    {
                        foreach (ApparelLayerDef apparelLayerDef2 in a.def.apparel.layers)
                        {
                            bool flag2 = !lUsedLayer.Contains(apparelLayerDef2);
                            if (flag2)
                            {
                                apparelLayerDef = apparelLayerDef2;
                            }
                        }
                    }

                    return apparelLayerDef;
                }


                private List<ApparelLayerDef> GetUsedLayers(Apparel a)
                {
                    List<ApparelLayerDef> list = new List<ApparelLayerDef>();
                    foreach (Apparel apparel in CEditor.API.Pawn.apparel.WornApparel)
                    {
                        bool flag = apparel != a;
                        if (flag)
                        {
                            ApparelLayerDef bestLayer = this.GetBestLayer(apparel.def.apparel.layers);
                            bool flag2 = !list.Contains(bestLayer);
                            if (flag2)
                            {
                                list.Add(bestLayer);
                            }
                        }
                    }

                    return list;
                }


                private void NextOrRandomApparel(Apparel a, bool next, bool random, bool doSkip = false)
                {
                    List<ApparelLayerDef> usedLayers = this.GetUsedLayers(a);
                    ApparelLayerDef freeLayer = this.GetFreeLayer(usedLayers, a);
                    bool flag = a.IsForNeck();
                    bool flag2 = a.IsForLegs();
                    bool flag3 = a.IsForEyes();
                    bool flag4 = false;
                    bool flag5 = usedLayers.Contains(ApparelLayerDefOf.Middle) && flag2;
                    if (flag5)
                    {
                        flag4 = true;
                    }

                    bool flag6 = usedLayers.Contains(ApparelLayerDefOf.Overhead);
                    if (flag6)
                    {
                        flag4 = true;
                    }

                    BodyPartGroupDef bodyPartGroupDef = flag2 ? BodyPartGroupDefOf.Legs : (flag ? DefTool.BodyPartGroupDef("Neck") : (flag3 ? DefTool.BodyPartGroupDef("Eyes") : a.def.apparel.bodyPartGroups.FirstOrDefault<BodyPartGroupDef>()));
                    HashSet<ThingDef> hashSet = ApparelTool.ListOfApparel(null, freeLayer, flag4 ? bodyPartGroupDef : null);
                    int num = 0;
                    foreach (ThingDef thingDef in hashSet)
                    {
                        bool flag7 = thingDef.defName == a.def.defName;
                        if (flag7)
                        {
                            break;
                        }

                        num++;
                    }

                    num = hashSet.NextOrPrevIndex(num, next, random);
                    CEditor.API.Pawn.apparel.Remove(a);
                    int count = hashSet.Count;
                    for (int i = 0; i < count; i++)
                    {
                        bool flag8 = !CEditor.API.Pawn.apparel.CanWearWithoutDroppingAnything(hashSet.At(num)) || !CEditor.API.Pawn.ApparalGraphicTest2(hashSet.At(num), false);
                        if (!flag8)
                        {
                            break;
                        }

                        num = hashSet.NextOrPrevIndex(num, next, random);
                    }

                    ThingDef thingDef2 = hashSet.At(num);
                    bool devMode = Prefs.DevMode;
                    if (devMode)
                    {
                        MessageTool.Show(string.Concat(new string[]
                        {
                            "wear... ",
                            thingDef2.defName,
                            " ",
                            (num + 1).ToString(),
                            "/",
                            hashSet.Count.ToString()
                        }), null);
                    }

                    this.WearSelectedApparel(Selected.ByThingDef(thingDef2), a.DrawColor);
                }


                private void WearSelectedApparel(Selected s, Color oldColor)
                {
                    Apparel apparel = ApparelTool.GenerateApparel(s);
                    bool alt = Event.current.alt;
                    if (alt)
                    {
                        apparel.DrawColor = oldColor;
                    }
                    else
                    {
                        bool shift = Event.current.shift;
                        if (shift)
                        {
                            apparel.DrawColor = ColorTool.RandomColor;
                        }
                        else
                        {
                            bool control = Event.current.control;
                            if (control)
                            {
                                apparel.DrawColor = ColorTool.RandomAlphaColor;
                            }
                        }
                    }

                    CEditor.API.Pawn.WearThatApparel(apparel);
                    CEditor.API.UpdateGraphics();
                }


                private void DrawWeaponSelector()
                {
                    bool flag = !CEditor.API.Pawn.HasEquipmentTracker() || this.weaponCurrent == null;
                    if (!flag)
                    {
                        try
                        {
                            List<ThingWithComps> allEquipmentListForReading = CEditor.API.Pawn.equipment.AllEquipmentListForReading;
                            for (int i = 0; i < allEquipmentListForReading.Count; i++)
                            {
                                ThingWithComps thingWithComps = allEquipmentListForReading[i];
                                SZWidgets.NavSelectorImageBox2<ThingWithComps>(this.RectFullSolid, thingWithComps, new Action<ThingWithComps>(this.AChangeWeaponUI), new Action<ThingWithComps>(this.ARandomWeapon), new Action<ThingWithComps>(this.ASetPrevWeapon), new Action<ThingWithComps>(this.ASetNextWeapon), new Action<ThingWithComps>(this.AOnTextureWeapon), new Action<ThingWithComps>(this.AConfigWeaponcolor), thingWithComps.def.label.CapitalizeFirst(), thingWithComps.GetTooltip().text, Label.TIP_RANDOMIZE_EQUIP, null, null, default(Color));
                                this.y += 30;
                                bool flag2 = this.isWeaponConfigOpen && this.weaponCurrent == thingWithComps;
                                if (flag2)
                                {
                                    bool flag3 = thingWithComps.TryGetComp<CompColorable>() != null;
                                    if (flag3)
                                    {
                                        SZWidgets.ColorBox(new Rect((float)(this.x + 25), (float)this.y, (float)(this.w - 50), 95f), thingWithComps.DrawColor, new Action<Color>(this.AOnWeaponColorChanged), true);
                                        this.y += 95;
                                    }
                                    else
                                    {
                                        thingWithComps.def.AddCompColorable();
                                        thingWithComps.InitializeComps();
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }


                private void AChangeWeaponUI(ThingWithComps wp)
                {
                    bool flag = wp.def.colorGenerator == null || !wp.def.HasComp(typeof(CompColorable));
                    if (flag)
                    {
                        MessageTool.ShowActionDialog(Label.INFOD_WEAPON, delegate
                        {
                            ApparelTool.MakeThingwColorable(wp);
                            WindowTool.Open(new DialogColorPicker(ColorType.WeaponColor, true, null, wp, null));
                        }, Label.INFOT_MAKECOLORABLE, WindowLayer.Dialog);
                    }
                    else
                    {
                        WindowTool.Open(new DialogColorPicker(ColorType.WeaponColor, true, null, wp, null));
                    }
                }


                private void AConfigWeaponcolor(ThingWithComps wp)
                {
                    this.isWeaponConfigOpen = !this.isWeaponConfigOpen;
                    this.weaponCurrent = wp;
                }


                private void AOnTextureWeapon(ThingWithComps wp)
                {
                    WindowTool.Open(new DialogObjects(DialogType.Weapon, null, wp, false));
                }


                private void AOnWeaponColorChanged(Color col)
                {
                    this.weaponCurrent.DrawColor = col;
                    CEditor.API.UpdateGraphics();
                }


                private void ARandomWeapon(ThingWithComps wp)
                {
                    this.NextOrRandomWeapon(wp, true, true);
                }


                private void ASetNextWeapon(ThingWithComps wp)
                {
                    this.NextOrRandomWeapon(wp, true, false);
                }


                private void ASetPrevWeapon(ThingWithComps wp)
                {
                    this.NextOrRandomWeapon(wp, false, false);
                }


                private void EquipSelectedWeapon(Selected selectedWeapon, bool primary)
                {
                    CEditor.API.Pawn.Reequip(selectedWeapon, primary ? 0 : 1, false);
                    CEditor.API.UpdateGraphics();
                }


                private void NextOrRandomWeapon(ThingWithComps wp, bool next, bool random)
                {
                    HashSet<ThingDef> hashSet = WeaponTool.ListOfWeapons(null, WeaponType.Melee);
                    HashSet<ThingDef> other = WeaponTool.ListOfWeapons(null, WeaponType.Ranged);
                    hashSet.AddRange(other);
                    int num = 0;
                    foreach (ThingDef thingDef in hashSet)
                    {
                        bool flag = thingDef.defName == wp.def.defName;
                        if (flag)
                        {
                            break;
                        }

                        num++;
                    }

                    num = hashSet.NextOrPrevIndex(num, next, random);
                    ThingDef thingDef2 = hashSet.At(num);
                    bool devMode = Prefs.DevMode;
                    if (devMode)
                    {
                        MessageTool.Show(string.Concat(new string[]
                        {
                            "equip... ",
                            thingDef2.defName,
                            " ",
                            (num + 1).ToString(),
                            "/",
                            hashSet.Count.ToString()
                        }), null);
                    }

                    this.EquipSelectedWeapon(Selected.ByThingDef(thingDef2), wp == CEditor.API.Pawn.equipment.Primary);
                }


                private void ARemoveDeadLogo()
                {
                    this.randomDeadColor = ColorTool.RandomColor;
                    this.iTickRemoveDeadLogo = 180;
                }


                private Vector2 scrollPos;


                private bool isPrimaryColor;


                private bool isPrimarySkinColor;


                private Apparel apparelCurrent;


                private bool bIsBodyOpen;


                private bool bIsHairOpen;


                private bool bIsHeadOpen;


                private bool bShowClothes;


                private bool bShowHat;


                private object deftest;


                private bool isAlien;


                private bool isApparelConfigOpen;


                private bool isBeardConfigOpen;


                private bool isBodyTattooConfigOpen;


                private bool isEyeConfigOpen;


                private bool isFaceTattooConfigOpen;


                private bool isHaircolorConfigOpen;


                private bool isHairConfigOpen;


                private bool isSkinConfigOpen;


                private bool isWeaponConfigOpen;


                private int iTickRemoveDeadLogo;


                private int iTickTool;


                private HashSet<BeardDef> lOfBeardDefs;


                private HashSet<HairDef> lOfHairDefs;


                private Color randomDeadColor;


                private string selectedModName;


                private ThingWithComps weaponCurrent;


                private int x;


                private int y;


                private int w;


                private int h;
            }


            private class BlockPawnList
            {
                internal BlockPawnList()
                {
                    this.bShowRemovePawn = false;
                    this.colParam = "n.a.";
                    this.capturer = CEditor.API.Get<Capturer>(EType.Capturer);
                    this.lraces = new HashSet<ThingDef>();
                    this.lraces.Add(null);
                }


                private List<PawnKindDef> ListOfPawnKinds
                {
                    get { return CEditor.API.ListOf<PawnKindDef>(EType.PawnKindListed); }
                }


                internal List<Pawn> ListOfPawns
                {
                    get { return CEditor.API.ListOf<Pawn>(EType.Pawns); }
                }


                private Dictionary<string, Faction> DicFactions
                {
                    get { return CEditor.API.Get<Dictionary<string, Faction>>(EType.Factions); }
                }


                private bool IsPlayerFaction
                {
                    get { return CEditor.ListName == Label.COLONISTS; }
                }


                internal void Draw(CEditor.EditorUI.coord c)
                {
                    int x = c.x;
                    int y = c.y;
                    int w = c.w;
                    int h = c.h;
                    bool flag = w <= 0;
                    if (!flag)
                    {
                        this.CheckPawnFromSelector();
                        this.DrawListSelector(x, ref y, w);
                        this.DrawOnMap(x, y, w);
                        this.DrawListCount(x, y, w);
                        this.DrawTopBottons(x, ref y, w);
                        this.DrawList(x, y, w, h);
                    }
                }


                private void DrawListSelector(int x, ref int y, int w)
                {
                    Text.Font = GameFont.Tiny;
                    SZWidgets.FloatMenuOnButtonText<string>(new Rect((float)x, (float)y, (float)w, 25f), CEditor.ListName, this.DicFactions.Keys.ToList<string>(), (string s) => s, delegate(string listname) { this.ChangeList(listname); }, "");
                    y += 25;
                    bool isRandom = CEditor.IsRandom;
                    if (isRandom)
                    {
                        SZWidgets.FloatMenuOnButtonText<ThingDef>(new Rect((float)x, (float)y, (float)w, 25f), CEditor.RACENAME(CEditor.RACE), this.lraces, CEditor.RACENAME, new Action<ThingDef>(this.ARaceChanged), "");
                        y += 25;
                        SZWidgets.FloatMenuOnButtonText<PawnKindDef>(new Rect((float)x, (float)y, (float)w, 25f), CEditor.PKDNAME(CEditor.PKD), this.ListOfPawnKinds, CEditor.PKDNAME, new Action<PawnKindDef>(this.APKDChanged), "");
                        y += 25;
                    }
                }


                private void DrawOnMap(int x, int y, int w)
                {
                    bool flag = !CEditor.InStartingScreen;
                    if (flag)
                    {
                        SZWidgets.ButtonImageCol(new Rect((float)(x + 2), (float)(y + 4), 16f, 16f), "bworld", new Action(this.AChangedOnMap), CEditor.OnMap ? Color.grey : Color.white, Label.ONMAP);
                    }
                }


                private void DrawListCount(int x, int y, int w)
                {
                    try
                    {
                        Listing_X listing_X = new Listing_X();
                        listing_X.Begin(new Rect((float)(CEditor.InStartingScreen ? x : (x + 24)), (float)(y + 2), 88f, (float)(CEditor.InStartingScreen ? 40 : 20)));
                        bool flag = CEditor.InStartingScreen && this.IsPlayerFaction && !Find.GameInitData.startingAndOptionalPawns.NullOrEmpty<Pawn>();
                        if (flag)
                        {
                            listing_X.AddIntSection("", "max" + Find.GameInitData.startingAndOptionalPawns.Count.ToString(), ref this.colParam, ref Find.GameInitData.startingPawnCount, 1, Find.GameInitData.startingAndOptionalPawns.Count, true, "", false);
                        }
                        else
                        {
                            bool flag2 = !this.ListOfPawns.NullOrEmpty<Pawn>();
                            if (flag2)
                            {
                                listing_X.Label(0f, 0f, 80f, 20f, this.ListOfPawns.Count.ToString(), GameFont.Tiny, "");
                            }
                        }

                        listing_X.End();
                    }
                    catch
                    {
                        CEditor.bGamePlus = false;
                        CEditor.bStartNewGame = false;
                        CEditor.bStartNewGame2 = false;
                        this.ChangeList(CEditor.ListName);
                    }
                }


                private void DrawTopBottons(int x, ref int y, int w)
                {
                    bool flag = CEditor.IsRandom || this.ListOfPawns.NullOrEmpty<Pawn>();
                    if (flag)
                    {
                        x += 25;
                        SZWidgets.ButtonImage((float)(w - x), (float)y, 25f, 25f, "UI/Buttons/Dev/Add", new Action(this.AAddStartPawn), "", default(Color));
                        x += 25;
                        SZWidgets.ButtonImage((float)(w - x), (float)y, 25f, 25f, "bminus", new Action(this.AAllowRemovePawn), CEditor.InStartingScreen ? Label.TIP_REMOVE_PAWN.SubstringTo("\n", true) : Label.TIP_REMOVE_PAWN, default(Color));
                        x += 25;
                    }

                    y += (CEditor.InStartingScreen ? 40 : 26);
                }


                private void DrawList(int x, int y, int w, int h)
                {
                    bool flag = this.ListOfPawns.NullOrEmpty<Pawn>();
                    if (!flag)
                    {
                        Text.Font = GameFont.Small;
                        float num = 112f;
                        Rect outRect = new Rect(0f, (float)y, (float)w, (float)(h - y));
                        Rect rect = new Rect(0f, (float)y, outRect.width - 16f, (float)this.ListOfPawns.Count * num);
                        Widgets.BeginScrollView(outRect, ref this.scrollPos, rect, true);
                        Rect rect2 = rect.ContractedBy(4f);
                        rect2.height = rect.height;
                        Listing_X listing_X = new Listing_X();
                        listing_X.Begin(rect2);
                        try
                        {
                            int num2 = 0;
                            for (int i = 0; i < this.ListOfPawns.Count; i++)
                            {
                                bool flag2 = listing_X.CurY + num > this.scrollPos.y && listing_X.CurY - 700f < this.scrollPos.y;
                                if (flag2)
                                {
                                    Pawn pawn = this.ListOfPawns[i];
                                    Color backColor = (pawn.Faction == null) ? Color.white : pawn.Faction.Color;
                                    bool flag3 = CEditor.InStartingScreen && this.IsPlayerFaction;
                                    if (flag3)
                                    {
                                        num2++;
                                        backColor = ((num2 > Find.GameInitData.startingPawnCount) ? Color.white : pawn.Faction.Color);
                                    }

                                    int num3 = listing_X.Selectable(pawn.LabelShort, false, "", this.capturer.GetRenderTexture(pawn, true), null, null, default(Vector2), this.bShowRemovePawn, 100f, backColor, ColorTool.colLightGray, false);
                                    bool dead = pawn.Dead;
                                    if (dead)
                                    {
                                        bool o = CEditor.API.GetO(OptionB.SHOWDEADLOGO);
                                        if (o)
                                        {
                                            Rect position = new Rect(0f, listing_X.CurY - 100f, rect.width, 50f);
                                            GUI.DrawTexture(position, ContentFinder<Texture2D>.Get("bdead", true));
                                        }
                                    }

                                    bool flag4 = num3 == 1;
                                    if (flag4)
                                    {
                                        CEditor.API.Pawn = pawn;
                                    }
                                    else
                                    {
                                        bool flag5 = num3 == 2;
                                        if (flag5)
                                        {
                                            pawn.Delete(false);
                                        }
                                    }
                                }

                                listing_X.CurY += num;
                            }
                        }
                        catch
                        {
                        }

                        listing_X.End();
                        Widgets.EndScrollView();
                    }
                }


                private void CheckPawnFromSelector()
                {
                    bool flag = !CEditor.InStartingScreen;
                    if (flag)
                    {
                        Selector selector = Find.Selector;
                        Pawn pawn = (selector != null) ? selector.FirstPawnFromSelector() : null;
                        bool flag2 = pawn != null && CEditor.API.Pawn != pawn;
                        if (flag2)
                        {
                            CEditor.API.Pawn = pawn;
                        }
                    }

                    bool flag3 = CEditor.API.Pawn == null;
                    if (flag3)
                    {
                        CEditor.API.Pawn = this.ListOfPawns.FirstOrFallback(null);
                    }
                }


                private void AChangedOnMap()
                {
                    SoundTool.PlayThis(SoundDefOf.Click);
                    CEditor.OnMap = !CEditor.OnMap;
                    this.ReloadList();
                }


                internal void ChangeList(string listname)
                {
                    CEditor.ListName = (listname.NullOrEmpty() ? Label.COLONISTS : listname);
                    this.DicFactions.Clear();
                    this.DicFactions.Merge(FactionTool.GetDicOfFactions(true, true, true));
                    this.ListOfPawns.Clear();
                    List<Pawn> pawnList = PawnxTool.GetPawnList(listname, CEditor.OnMap, this.DicFactions.GetValue(listname));
                    bool flag = pawnList != null;
                    if (flag)
                    {
                        this.ListOfPawns.AddRange(pawnList);
                    }

                    this.UpdateRaceList();
                    bool flag2 = CEditor.API.Pawn == null;
                    if (flag2)
                    {
                        CEditor.API.Pawn = this.ListOfPawns.FirstOrFallback(null);
                    }
                }


                internal void ReloadList()
                {
                    this.ChangeList(CEditor.ListName);
                }


                private void UpdateRaceList()
                {
                    bool nonhumanlike = CEditor.ListName == Label.COLONYANIMALS || CEditor.ListName == Label.WILDANIMALS;
                    bool humanlike = CEditor.ListName == Label.HUMANOID || CEditor.ListName == Label.COLONISTS;
                    this.lraces.Clear();
                    this.lraces.Add(null);
                    this.lraces.AddRange(PawnKindTool.ListOfRaces(humanlike, nonhumanlike));
                    this.ARaceChanged(null);
                }


                private void ARaceChanged(ThingDef race)
                {
                    CEditor.RACE = race;
                    bool nonhumanlike = CEditor.ListName == Label.COLONYANIMALS || CEditor.ListName == Label.WILDANIMALS;
                    bool humanlike = CEditor.ListName == Label.HUMANOID || CEditor.ListName == Label.COLONISTS;
                    this.ListOfPawnKinds.Clear();
                    this.ListOfPawnKinds.Add(null);
                    this.ListOfPawnKinds.AddRange(PawnKindTool.ListOfPawnKindDefByRace(race, humanlike, nonhumanlike));
                    this.APKDChanged(null);
                }


                private void APKDChanged(PawnKindDef pkd)
                {
                    CEditor.PKD = pkd;
                }


                private void AAllowRemovePawn()
                {
                    bool flag = !CEditor.InStartingScreen && Event.current.alt;
                    if (flag)
                    {
                        CEditor.API.EditorMoveRight();
                        PlacingTool.DeletePawnFromCustomPosition();
                    }
                    else
                    {
                        this.bShowRemovePawn = !this.bShowRemovePawn;
                    }
                }


                private void AAddStartPawn()
                {
                    Faction value = this.DicFactions.GetValue(CEditor.ListName);
                    PawnxTool.AddOrCreateNewPawn(CEditor.PKD.ThisOrFromList(this.ListOfPawnKinds), value, CEditor.RACE, null, default(IntVec3), false, Gender.None);
                    bool flag = value.IsZombie();
                    if (flag)
                    {
                        this.ReloadList();
                    }
                }


                private Vector2 scrollPos;


                private bool bShowRemovePawn;


                private string colParam;


                private Capturer capturer;


                private HashSet<ThingDef> lraces;
            }
        }
    }
}
