// Decompiled with JetBrains decompiler
// Type: CharacterEditor.DialogObjects
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class DialogObjects : DialogTemplate<ThingDef>
{
    private const int W400 = 400;
    private float armorPenetrationBase;
    private int ceWeaponFuelCapacity;
    private float ceWeaponReloadTime;
    private string dummy = "";
    private float forcedMissRadius;
    private float forcedMissRadiusMortar;
    private int gender;
    private bool HasBullet;
    private bool HasVerb;
    private int iTick120 = 120;
    private readonly HashSet<string> lFactions;
    private HashSet<ThingDef> lOfThingsByCat = new();
    private HashSet<ThingDef> lOfThingsByCat2 = new();
    private readonly HashSet<DialogType> lType;
    private readonly DialogCapsuleUI mCapsuleUI;
    private readonly bool mChangeFirstWeapon;
    private int mCostTemp;
    private DialogType mDialogType;
    private bool mDoesExplosion;
    private int mHscroll;
    private int mStackLimit;
    private ThingWithComps mThing;
    private ThingDef mTurretDef;
    private bool mUseAllSounds;
    private int rangeDamageAmountBase;
    private ApparelLayerDef selected_ApparelLayer;
    private BodyPartGroupDef selected_BodyPartGroup;
    private ThingCategoryDef selected_Cat;
    private ThingCategoryDef selected_Cat2;
    private ThingDef selected_CostDef;
    private ThingDef selected_CostDefDiff;
    private ResearchProjectDef selected_ResearchDef;
    private StuffCategoryDef selected_StuffCategorie;
    private ThingCategory selected_TextCat;
    private ThingCategory selected_TextCat2;
    private WeaponTraitDef selected_WeaponTrait;
    private string tempFaction;
    private readonly Pawn tempPawn;
    private int WFULL;

    internal DialogObjects(
        DialogType type,
        DialogCapsuleUI capsuleUI = null,
        ThingWithComps thing = null,
        bool addAnimals = false)
        : base(SearchTool.SIndex.Weapon, Label.ADD_WEAPON, 105)
    {
        customAcceptLabel = new TaggedString("OK".Translate());
        tempPawn = CEditor.API.Pawn;
        lFactions = CEditor.API.DicFactions.Keys.ToHashSet();
        tempFaction = CEditor.ListName;
        mThing = thing;
        mDialogType = type;
        mCapsuleUI = capsuleUI;
        mChangeFirstWeapon = IsPrimary(thing);
        mHscroll = 0;
        if (mThing != null)
            search.weaponType = mThing.def != null ? mThing.def.IsRangedWeapon ? WeaponType.Ranged : mThing.def.IsMeleeWeapon ? WeaponType.Melee : mThing.def.IsTurret() ? WeaponType.Turret : WeaponType.TurretGun : WeaponType.Ranged;
        lType = EnumTool.GetAllEnumsOfType<DialogType>().ToHashSet();
        lMods = ListModnames();
        base.Preselection();
        if (mThing != null)
        {
            SearchTool.ClearSearch(SearchTool.SIndex.Weapon);
            selectedDef = mThing.def;
        }

        lOfThingsByCat = ThingTool.ListOfItemsWithNull(null, null, ThingCategory.None);
        lOfThingsByCat2 = ThingTool.ListOfItemsWithNull(null, null, ThingCategory.None);
        if (!addAnimals)
            return;
        search.modName = null;
        search.thingCategoryDef = ThingCategoryDefOf.Animals;
        search.thingCategory = ThingCategory.None;
        lDefs = TList();
    }

    private bool IsTurret => mTurretDef != null;

    private VerbProperties sverb => selectedDef.Verbs[0];

    private ProjectileProperties sbullet => selectedDef.Verbs[0].defaultProjectile.projectile;

    private int RangeDamageBase
    {
        get => sbullet.GetMemberValue("damageAmountBase", 0);
        set => sbullet.SetMemberValue("damageAmountBase", value);
    }

    private float PenetrationBase
    {
        get => sbullet.GetMemberValue("armorPenetrationBase", 0.0f);
        set => sbullet.SetMemberValue("armorPenetrationBase", value);
    }

    private float MissRadius
    {
        get => sverb.GetMemberValue("forcedMissRadius", 0.0f);
        set => sverb.SetMemberValue("forcedMissRadius", value);
    }

    private float MissRadiusMortar
    {
        get => sverb.GetMemberValue("forcedMissRadiusClassicMortars", 0.0f);
        set => sverb.SetMemberValue("forcedMissRadiusClassicMortars", value);
    }

    private int CEWeaponFuelCapacity
    {
        get => WeaponTool.GetCompRefuelable(mTurretDef, selectedDef);
        set => WeaponTool.SetCompRefuelable(mTurretDef, selectedDef, value);
    }

    private float CEWeaponReloadTime
    {
        get => WeaponTool.GetCompReloadTime(mTurretDef, selectedDef);
        set => WeaponTool.SetCompReloadTime(mTurretDef, selectedDef, value);
    }

    private bool IsPrimary(ThingWithComps t)
    {
        return t != null && tempPawn.HasEquipmentTracker() && tempPawn.equipment.Primary != null && tempPawn.equipment.Primary.ThingID == t.ThingID;
    }

    internal override HashSet<string> ListModnames()
    {
        return mDialogType != DialogType.Weapon ? mDialogType != DialogType.Apparel ? mDialogType != DialogType.Object ? new HashSet<string>() : DefTool.ListModnamesWithNull(DefTool.CONDITION_IS_ITEM(null, null, ThingCategory.None)) : DefTool.ListModnamesWithNull((Func<ThingDef, bool>)(t => t.IsApparel)).ToHashSet() : DefTool.ListModnamesWithNull((Func<ThingDef, bool>)(t => t.IsWeapon)).ToHashSet();
    }

    internal override HashSet<ThingDef> TList()
    {
        return mDialogType != DialogType.Weapon ? mDialogType != DialogType.Apparel ? mDialogType != DialogType.Object ? new HashSet<ThingDef>() : ThingTool.ListOfItems(search.modName, search.thingCategoryDef, search.thingCategory) : ApparelTool.ListOfApparel(search.modName, search.apparelLayerDef, search.bodyPartGroupDef) : WeaponTool.ListOfWeapons(search.modName, search.weaponType);
    }

    internal override void AReset()
    {
        PresetObject.ResetToDefault(selectedDef?.defName);
    }

    internal override void AResetAll()
    {
        PresetObject.ResetAllToDefaults();
    }

    internal override void ASave()
    {
        PresetObject.SaveModification(selectedDef);
    }

    internal override void CalcHSCROLL()
    {
        hScrollParam = 4000;
        if (mHscroll <= 800)
            return;
        hScrollParam = mHscroll;
    }

    internal override void Preselection()
    {
    }

   internal override int DrawCustomFilter(int x, int y, int w)
		{
			Text.Font = GameFont.Small;
			Rect rect = new Rect(x, y, w, 30f);
			int result = 0;
			if (this.mDialogType == DialogType.Weapon)
			{
				SZWidgets.FloatMenuOnButtonText<WeaponType>(rect, FLabel.WeaponType(this.search.weaponType), ThingTool.AllWeaponType, FLabel.WeaponType, delegate(WeaponType v)
				{
					this.search.weaponType = v;
					this.lDefs = this.TList();
				}, "");
				result = 30;
			}
			else if (this.mDialogType == DialogType.Apparel)
			{
				SZWidgets.FloatMenuOnButtonText<ApparelLayerDef>(rect, Label.LAYER + ": " + FLabel.DefLabelSimple<ApparelLayerDef>(this.search.apparelLayerDef), ThingTool.AllApparelLayerDef, new Func<ApparelLayerDef, string>(FLabel.DefLabelSimple<ApparelLayerDef>), delegate(ApparelLayerDef v)
				{
					this.search.apparelLayerDef = v;
					this.lDefs = this.TList();
				}, "");
				SZWidgets.FloatMenuOnButtonText<BodyPartGroupDef>(rect.RectPlusY(30), Label.BODYPARTGROUPS + ": " + FLabel.DefLabelSimple<BodyPartGroupDef>(this.search.bodyPartGroupDef), ThingTool.AllBodyPartGroupDef, new Func<BodyPartGroupDef, string>(FLabel.DefLabelSimple<BodyPartGroupDef>), delegate(BodyPartGroupDef v)
				{
					this.search.bodyPartGroupDef = v;
					this.lDefs = this.TList();
				}, "");
				result = 60;
			}
			else if (this.mDialogType == DialogType.Object)
			{
				SZWidgets.FloatMenuOnButtonText<ThingCategoryDef>(rect, FLabel.DefLabelSimple<ThingCategoryDef>(this.search.thingCategoryDef), ThingTool.AllThingCategoryDef, new Func<ThingCategoryDef, string>(FLabel.DefLabelSimple<ThingCategoryDef>), delegate(ThingCategoryDef v)
				{
					this.search.thingCategoryDef = v;
					this.lDefs = this.TList();
				}, "");
				SZWidgets.FloatMenuOnButtonText<ThingCategory>(rect.RectPlusY(30), FLabel.EnumNameAndAll<ThingCategory>(this.search.thingCategory), ThingTool.AllThingCategory, new Func<ThingCategory, string>(FLabel.EnumNameAndAll<ThingCategory>), delegate(ThingCategory v)
				{
					this.search.thingCategory = v;
					this.lDefs = this.TList();
				}, "");
				result = 60;
			}
			return result;
		}

    internal override void OnAccept()
    {
        if (mInPlacingMode)
        {
            if (ThingTool.SelectedThing.pkd != null)
                PlacingTool.PlaceMultiplePawnsInCustomPosition(ThingTool.SelectedThing, CEditor.API.DicFactions.GetValue(tempFaction));
            else
                PlacingTool.PlaceInCustomPosition(ThingTool.SelectedThing, null);
        }
        else if (mCapsuleUI != null)
        {
            mCapsuleUI.ExternalAddThing(ThingTool.SelectedThing);
        }
        else
        {
            if (ThingTool.SelectedThing.tempThing != null)
                return;
            if (mDialogType == DialogType.Weapon)
            {
                if (search.weaponType == WeaponType.Turret)
                {
                    tempPawn.AddItemToInventory(ThingTool.GenerateItem(ThingTool.SelectedThing));
                }
                else
                {
                    if (ThingTool.SelectedThing.tempThing != null)
                        return;
                    tempPawn.Reequip(ThingTool.SelectedThing, mChangeFirstWeapon ? 0 : 1);
                }
            }
            else if (mDialogType == DialogType.Apparel)
            {
                var conflictedApparelList = tempPawn.GetConflictedApparelList(ThingTool.SelectedThing.thingDef);
                if (conflictedApparelList.Count > 1)
                    MessageTool.ShowCustomDialog(conflictedApparelList.ListToString(), ThingTool.SelectedThing.thingDef.label.CapitalizeFirst() + " " + Label.WILL_REPLACE, null, Confirmed, null);
                else
                    Confirmed();
            }
            else
            {
                if (mDialogType != DialogType.Object)
                    return;
                if (ThingTool.SelectedThing.pkd != null)
                {
                    if (!CEditor.InStartingScreen)
                        PlacingTool.PlaceMultiplePawnsInCustomPosition(ThingTool.SelectedThing, CEditor.API.DicFactions.GetValue(tempFaction));
                    else
                        MessageTool.Show("not available in starting screen", MessageTypeDefOf.RejectInput);
                }
                else
                {
                    tempPawn.AddItemToInventory(ThingTool.GenerateItem(ThingTool.SelectedThing));
                }
            }
        }
    }

    private void Confirmed()
    {
        Apparel a;
        if (!this.tempPawn.CreateAndWearApparel(ThingTool.SelectedThing, out a, true))
        {
            this.tempPawn.AskToWearIncompatibleApparel(a);
        }
        CEditor.API.UpdateGraphics();
    }

    internal override void OnSelectionChanged()
    {
        mHscroll = 0;
        if (mThing != null)
        {
            ThingTool.SelectedThing = Selected.ByThing(mThing);
            mThing = null;
        }
        else
        {
            ThingTool.SelectedThing = Selected.ByThingDef(selectedDef);
        }

        ThingTool.SelectedThing.thingDef.UpdateFreeLists(ThingTool.FreeList.All);
        if (selectedDef == null)
            return;
        mTurretDef = selectedDef.IsTurret() ? selectedDef.GetTurretDef() : null;
        SoundTool.PlayThis(selectedDef.soundInteract);
    }

    internal override void DrawLowerButtons()
    {
        base.DrawLowerButtons();
        if (CEditor.InStartingScreen)
            return;
        if (!mInPlacingMode)
        {
            SZWidgets.ButtonImage(new Rect(frameW - 60, 0.0f, 30f, 30f), "bnone", APlacingMode, Label.ACTIVATEPLACINGMODE);
            WindowTool.SimpleCustomButton(this, 0, ABuy, Label.BUY, Label.PRICE + ThingTool.SelectedThing.buyPrice);
        }
        else
        {
            SZWidgets.ButtonImage(new Rect(frameW - 30, 0.0f, 30f, 30f), "brotate", ARotate, "rotation" + TextureTool.Rot4ToString(PlacingTool.rotation));
            SZWidgets.ButtonText(new Rect(0.0f, (float)(((Window)this).InitialSize.y - 66.0), 130f, 30f), Label.DESTROY, ADestroy);
        }
    }

    private void ADestroy()
    {
        PlacingTool.Destroy();
    }

    private void APlacingMode()
    {
        this.doWindowBackground = true;
        this.absorbInputAroundWindow = false;
        this.preventCameraMotion = false;
        this.preventDrawTutor = false;
        this.draggable = false;
        this.closeOnClickedOutside = false;
        this.windowRect.position = new Vector2(0f, 0f);
        PlacingTool.rotation = Rot4.North;
        this.mInPlacingMode = true;
        CEditor.API.CloseEditor();
    }

    private void ARotate()
    {
        if (PlacingTool.rotation == Rot4.North)
        {
            PlacingTool.rotation = Rot4.East;
            return;
        }
        if (PlacingTool.rotation == Rot4.East)
        {
            PlacingTool.rotation = Rot4.South;
            return;
        }
        if (PlacingTool.rotation == Rot4.South)
        {
            PlacingTool.rotation = Rot4.West;
            return;
        }
        if (PlacingTool.rotation == Rot4.West)
        {
            PlacingTool.rotation = Rot4.North;
            return;
        }
        PlacingTool.rotation = Rot4.North;
    }

    private void ABuy()
    {
        ThingTool.BeginBuyItem(ThingTool.SelectedThing);
        CEditor.API.CloseEditor();
        ((Window)this).Close();
    }

    private string DialogLabel(DialogType t)
    {
        string str;
        switch (t)
        {
            case DialogType.Object:
                str = Label.OBJECT;
                break;
            case DialogType.Weapon:
                str = Label.WEAPON;
                break;
            case DialogType.Apparel:
                str = new TaggedString("Apparel".Translate());
                break;
            default:
                str = "";
                break;
        }

        return str;
    }

    internal override void DrawTitle(int x, int y, int w, int h)
    {
        SZWidgets.NonDefSelectorSimple(new Rect(0.0f, 0.0f, 80f, 30f), 80, lType, ref mDialogType, "", ty => DialogLabel(ty), v =>
        {
            mDialogType = v;
            lMods = ListModnames();
            lDefs = TList();
        });
    }

    internal override void DrawParameter()
    {
        if (selectedDef == null)
            return;
        HasBullet = selectedDef.HasProjectile();
        HasVerb = selectedDef.HasVerb();
        WFULL = WPARAM - 12;
        DrawLabel();
        DrawDamageLabel();
        DrawObjectPic();
        DrawBulletBig(400);
        view.CurY += 128f;
        DrawBulletSmall(400);
        DrawNavSelectors(400);
        DrawSoundCast(400);
        DrawHitpoints(400);
        DrawDamageSelectors(400);
        DrawRangedBig(400);
        DrawStatFactors(WFULL);
        DrawStatOffsets(WFULL);
        DrawStuffCategories(WFULL);
        DrawCostStuffCount(400);
        DrawCosts(WFULL);
        DrawCostsDiff(WFULL);
        DrawLayers(WFULL);
        DrawCoveredBodyParts(WFULL);
        DrawApparelTags(WFULL);
        DrawOutfitTags(WFULL);
        DrawWeaponTags(WFULL);
        DrawTradeTags(WFULL);
        DrawResearchBuilding(WFULL);
        DrawWeaponTraits(WFULL);
        DrawEnums(400);
        DrawRangedSmall(400);
        DrawOther(400);
        DrawSounds(400);
        DrawExtras(400);
        mHscroll = (int)view.CurY + 50;
        UpdateCachedDescription();
    }

    private void UpdateCachedDescription()
    {
        if (iTick120 <= 0)
        {
            ThingTool.SelectedThing.thingDef.SetMemberValue("descriptionDetailedCached", null);
            ThingTool.SelectedThing.thingDef.SetMemberValue("cachedLabelCap", null);
            iTick120 = 120;
        }
        else
        {
            --iTick120;
        }
    }

    private void DrawOther(int w)
    {
        if (this.selectedDef.IsStackable())
        {
            this.mStackLimit = this.selectedDef.stackLimit;
            SZWidgets.LabelIntFieldSlider(this.view, w, 99999, FLabel.StackLimit, ref this.selectedDef.stackLimit, 1, 2000);
            if (this.mStackLimit != this.selectedDef.stackLimit)
            {
                this.mStackLimit = this.selectedDef.stackLimit;
                this.selectedDef.UpdateStackLimit();
            }
        }
        this.view.Gap(2f);
        this.view.CheckboxLabeled(Label.STEALABLE, 0f, (float)w, ref this.selectedDef.stealable, null, 2);
    }

    private void DrawEnums(int w)
    {
        if (this.selectedDef.recipeMaker != null)
        {
            SZWidgets.DefSelectorSimple<ResearchProjectDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllResearchProjectDef, ref this.selectedDef.recipeMaker.researchPrerequisite, Label.RECIPEPREREQUISITE, new Func<ResearchProjectDef, string>(FLabel.DefLabel<ResearchProjectDef>), delegate(ResearchProjectDef p)
            {
                this.selectedDef.SetResearchPrerequisite(p);
            }, false, null, null, true, "");
            this.view.Gap(2f);
        }
        SZWidgets.NonDefSelectorSimple<Tradeability>(this.view.GetRect(22f, 1f), w, ThingTool.AllTradeabilities, ref this.selectedDef.tradeability, Label.TRADEABILITY + " ", FLabel.Tradeability, delegate(Tradeability s)
        {
            this.selectedDef.tradeability = s;
        }, false, null, null);
        this.view.Gap(2f);
        SZWidgets.NonDefSelectorSimple<TechLevel>(this.view.GetRect(22f, 1f), w, ThingTool.AllTechLevels, ref this.selectedDef.techLevel, Label.TECHLEVEL + " ", FLabel.TechLevel, delegate(TechLevel s)
        {
            this.selectedDef.techLevel = s;
        }, false, null, null);
        this.view.Gap(2f);
    }

    private void DrawExtras(int w)
    {
        Text.Font = (GameFont)2;
        view.GapLine();
        view.Label(Label.GRAPHICS, 500f, 0.0f);
        GUI.color = Color.gray;
        Text.Font = 0;
        Text.WordWrap = false;
        if ((selectedDef.graphicData == null ? 0 : selectedDef.graphicData.texPath != null ? 1 : 0) != 0)
        {
            view.Label(Label.TEXPATH + selectedDef.graphicData.texPath, 1000f, 0.0f);
            view.Label(Label.DRAWSIZE + selectedDef.graphicData.drawSize, 1000f, 0.0f);
        }

        if ((selectedDef.apparel == null ? 0 : selectedDef.apparel.wornGraphicPath != null ? 1 : 0) != 0)
            view.Label(Label.WORNPATH + selectedDef.apparel.wornGraphicPath, 1000f, 0.0f);
        Text.WordWrap = true;
        Text.Font = (GameFont)1;
        GUI.color = Color.white;
    }

    private void DrawCostStuffCount(int w)
    {
        if (this.selectedDef.race == null)
        {
            this.mCostTemp = this.selectedDef.costStuffCount;
            Listing_X view = this.view;
            int id = this.id;
            this.id = id + 1;
            SZWidgets.LabelIntFieldSlider(view, w, id, FLabel.CostStuffCount, ref this.selectedDef.costStuffCount, 0, 99999);
            if (this.mCostTemp != this.selectedDef.costStuffCount)
            {
                this.selectedDef.UpdateRecipes();
            }
            this.view.Gap(8f);
        }
    }

    private void DrawDamageLabel()
    {
        if (this.selectedDef.IsWeapon || this.selectedDef.HasVerb())
        {
            string text = "";
            if (this.HasBullet)
            {
                text = this.sverb.defaultProjectile.SLabel();
                if (!text.NullOrEmpty())
                {
                    text += " ";
                }
            }
            int num = 0;
            try
            {
                num = this.selectedDef.GetDmg();
            }
            catch
            {
            }
            this.view.Label(0f, 0f, 500f, 30f, text + "Damage".Translate() + " [" + num.ToString() + "]", GameFont.Medium, "");
            this.view.Gap(30f);
        }
    }

    private void DrawObjectPic()
    {
        GUI.color = this.selectedDef.GetTColor(ThingTool.SelectedThing.stuff);
        float num = (this.selectedDef.graphicData != null) ? this.selectedDef.graphicData.drawSize.x : 1f;
        float num2 = (this.selectedDef.graphicData != null) ? this.selectedDef.graphicData.drawSize.y : 1f;
        GUI.DrawTextureWithTexCoords(new Rect(this.view.CurX, this.view.CurY, (float)((int)(128f / num2 * num)), 128f), this.selectedDef.GetTexture(1, ThingTool.SelectedThing.style, Rot4.South), new Rect(0f, 0f, 1f, 1f));
        if (this.selectedDef.IsTurret())
        {
            ThingDef realWeaponDef = this.selectedDef.GetRealWeaponDef();
            GUI.color = realWeaponDef.uiIconColor;
            GUI.DrawTextureWithTexCoords(new Rect(this.view.CurX, this.view.CurY - 10f, 128f, 128f), realWeaponDef.GetTexture(1, null, default(Rot4)), new Rect(0f, 0f, 1f, 1f));
        }
        GUI.color = Color.white;
    }

    private void DrawBulletBig(int w)
    {
        if (this.HasVerb)
        {
            Rect rect = new Rect(this.view.CurX + 200f, this.view.CurY + 30f, 64f, 64f);
            GUI.DrawTexture(rect, this.sverb.defaultProjectile.GetTexture(1, null, default(Rot4)));
            SZWidgets.DefSelectorSimpleTex<ThingDef>(rect, w, ThingTool.AllBullets, ref this.sverb.defaultProjectile, "", new Func<ThingDef, string>(FLabel.DefLabel<ThingDef>), null, false, this.sverb.defaultProjectile.GetTexture(1, null, default(Rot4)), delegate(ThingDef s)
            {
                this.selectedDef.SetBullet(s);
            }, false, "");
        }
    }

    private void DrawBulletSmall(int w)
    {
        if (this.HasVerb)
        {
            SZWidgets.DefSelectorSimpleTex<ThingDef>(this.view.GetRect(22f, 1f), 400, ThingTool.AllBullets, ref this.sverb.defaultProjectile, "", new Func<ThingDef, string>(FLabel.DefLabel<ThingDef>), delegate(ThingDef b)
            {
                this.selectedDef.SetBullet(b);
                SoundTool.PlayThis(b.soundInteract);
            }, true, this.sverb.defaultProjectile.GetTexture(1, null, default(Rot4)), null, true, "");
            this.view.Gap(2f);
        }
    }

    private void DrawNavSelectors(int w)
    {
        this.view.CurY = SZWidgets.NavSelectorQuality(new Rect(0f, this.view.CurY, (float)w, 22f), ThingTool.SelectedThing, ThingTool.AllQualityCategory);
        this.view.CurY = SZWidgets.NavSelectorStuff(new Rect(0f, this.view.CurY, (float)w, 22f), ThingTool.SelectedThing);
        this.view.CurY = SZWidgets.NavSelectorStyle(new Rect(0f, this.view.CurY, (float)w, 22f), ThingTool.SelectedThing);
        if (ThingTool.SelectedThing != null && ThingTool.SelectedThing.HasRace)
        {
            this.gender = (int)ThingTool.SelectedThing.gender;
            Listing_X view = this.view;
            int id = this.id;
            this.id = id + 1;
            SZWidgets.LabelIntFieldSlider(view, w, id, FLabel.GenderLabelInt, ref this.gender, 0, 2);
            ThingTool.SelectedThing.gender = (Gender)this.gender;
            this.view.Gap(2f);
            Listing_X view2 = this.view;
            id = this.id;
            this.id = id + 1;
            SZWidgets.LabelIntFieldSlider(view2, w, id, FLabel.BiologicalAge, ref ThingTool.SelectedThing.age, 1, 100);
            this.view.Gap(2f);
            SZWidgets.NonDefSelectorSimple<string>(this.view.GetRect(22f, 1f), w, this.lFactions, ref this.tempFaction, "Faction".Translate() + " ", (string s) => s, delegate(string s)
            {
                this.tempFaction = s;
            }, false, null, null);
            this.view.Gap(2f);
            ThingTool.SelectedThing.pkd = ThingTool.SelectedThing.thingDef.race.AnyPawnKind;
        }
        if (ThingTool.SelectedThing.HasStack)
        {
            Listing_X view3 = this.view;
            int id2 = this.id;
            this.id = id2 + 1;
            SZWidgets.LabelIntFieldSlider(view3, w, id2, FLabel.Menge, ref ThingTool.SelectedThing.stackVal, 1, this.selectedDef.stackLimit);
            if (ThingTool.SelectedThing.tempThing != null)
            {
                ThingTool.SelectedThing.tempThing.stackCount = ThingTool.SelectedThing.stackVal;
            }
            this.view.Gap(8f);
        }
    }

    private void DrawSoundCast(int w)
    {
        if (!HasVerb)
            return;
        // ISSUE: cast to a reference type
        SZWidgets.DefSelectorSimple<SoundDef>(view.GetRect(22f), w, ThingTool.AllGunShotSounds, ref sverb.soundCast, Label.SOUNDCAST, FLabel.Sound, (Action<SoundDef>)(s => sverb.soundCast = SoundTool.GetAndPlay(s)), true, "bsound", new Action<SoundDef>(SoundTool.PlayThis));
        view.Gap(2f);
    }

    private void DrawHitpoints(int w)
    {
        if (ThingTool.SelectedThing.tempThing == null)
            return;
        var hitPoints1 = ThingTool.SelectedThing.tempThing.HitPoints;
        var view = this.view;
        var num = this.id++;
        var w1 = w;
        var id = num;
        var hitPoints2 = FLabel.HitPoints;
        ref var local = ref hitPoints1;
        var maxHitPoints = ThingTool.SelectedThing.tempThing.MaxHitPoints;
        SZWidgets.LabelIntFieldSlider(view, w1, id, hitPoints2, ref local, 0, maxHitPoints);
        ThingTool.SelectedThing.tempThing.HitPoints = hitPoints1;
        if (HasBullet)
            return;
        this.view.Gap(8f);
    }

    private void DrawDamageSelectors(int w)
    {
        if (!HasBullet)
            return;
        // ISSUE: cast to a reference type
        SZWidgets.DefSelectorSimple<DamageDef>(view.GetRect(22f), w, ThingTool.AllDamageDefs, ref sbullet.damageDef, Label.DAMAGEDEF, new Func<DamageDef, string>(FLabel.DefLabel), (Action<DamageDef>)(d => sbullet.damageDef = d));
        view.Gap(2f);
        rangeDamageAmountBase = RangeDamageBase;
        var view1 = view;
        var num1 = id++;
        var w1 = w;
        var id1 = num1;
        var damageAmountBase = FLabel.DamageAmountBase;
        ref var local1 = ref rangeDamageAmountBase;
        SZWidgets.LabelIntFieldSlider(view1, w1, id1, damageAmountBase, ref local1, 0, 100);
        RangeDamageBase = rangeDamageAmountBase;
        this.armorPenetrationBase = PenetrationBase;
        var view2 = view;
        var num2 = id++;
        var w2 = w;
        var id2 = num2;
        var armorPenetrationBase = FLabel.ArmorPenetrationBase;
        ref var local2 = ref this.armorPenetrationBase;
        SZWidgets.LabelFloatFieldSlider(view2, w2, id2, armorPenetrationBase, ref local2, 0.0f, 100f, 2);
        PenetrationBase = this.armorPenetrationBase;
    }
private void DrawRangedSmall(int w)
		{
			if (this.HasVerb)
			{
				Text.Font = GameFont.Small;
				this.forcedMissRadius = this.MissRadius;
				Listing_X view = this.view;
				int id = this.id;
				this.id = id + 1;
				SZWidgets.LabelFloatFieldSlider(view, w, id, FLabel.Spraying, ref this.forcedMissRadius, 0f, 100f, 1);
				this.MissRadius = this.forcedMissRadius;
				if (this.sverb.GetMemberValue("isMortar", false))
				{
					this.forcedMissRadiusMortar = this.MissRadiusMortar;
					Listing_X view2 = this.view;
					id = this.id;
					this.id = id + 1;
					SZWidgets.LabelFloatFieldSlider(view2, w, id, FLabel.SprayingMortar, ref this.forcedMissRadiusMortar, 0f, 10f, 1);
					this.MissRadiusMortar = this.forcedMissRadiusMortar;
				}
				Listing_X view3 = this.view;
				id = this.id;
				this.id = id + 1;
				SZWidgets.LabelFloatFieldSlider(view3, w, id, FLabel.BeamWidth, ref this.sverb.beamWidth, 0f, 10f, 1);
				Listing_X view4 = this.view;
				id = this.id;
				this.id = id + 1;
				SZWidgets.LabelFloatFieldSlider(view4, w, id, FLabel.BeamFullWidthRange, ref this.sverb.beamFullWidthRange, 0f, 10f, 1);
				SZWidgets.DefSelectorSimple<DamageDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllDamageDefs, ref this.sverb.beamDamageDef, Label.BEAMDAMAGEDEF, new Func<DamageDef, string>(FLabel.DefLabel<DamageDef>), delegate(DamageDef s)
				{
					this.sverb.beamDamageDef = s;
				}, false, null, null, true, "");
				this.view.Gap(2f);
				if (this.HasBullet)
				{
					Listing_X view5 = this.view;
					id = this.id;
					this.id = id + 1;
					SZWidgets.LabelFloatFieldSlider(view5, w, id, FLabel.BulletExplosionRadius, ref this.sbullet.explosionRadius, 0f, 50f, 1);
					Listing_X view6 = this.view;
					id = this.id;
					this.id = id + 1;
					SZWidgets.LabelIntFieldSlider(view6, w, id, FLabel.BulletExplosionDelay, ref this.sbullet.explosionDelay, 0, 10);
					Listing_X view7 = this.view;
					id = this.id;
					this.id = id + 1;
					SZWidgets.LabelIntFieldSlider(view7, w, id, FLabel.BulletNumExtraHitCels, ref this.sbullet.numExtraHitCells, 0, 8);
					this.view.Gap(10f);
					this.mDoesExplosion = this.sverb.CausesExplosion;
					this.view.CheckboxLabeled(Label.ISEXPLOSIVEREADONLY, 0f, (float)w, ref this.mDoesExplosion, null, 2);
					SZWidgets.DefSelectorSimple<ThingCategoryDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllThingCategoryDef, ref this.selected_Cat, Label.FILTERCATEGORY, new Func<ThingCategoryDef, string>(FLabel.DefLabel<ThingCategoryDef>), delegate(ThingCategoryDef cat)
					{
						this.lOfThingsByCat = ThingTool.ListOfItems(null, cat, this.selected_TextCat);
						this.selected_Cat = cat;
					}, false, null, null, true, "");
					this.view.Gap(2f);
					SZWidgets.NonDefSelectorSimple<ThingCategory>(this.view.GetRect(22f, 1f), w, ThingTool.AllThingCategory, ref this.selected_TextCat, Label.FILTERTYPE, new Func<ThingCategory, string>(FLabel.EnumNameAndAll<ThingCategory>), delegate(ThingCategory cat)
					{
						this.lOfThingsByCat = ThingTool.ListOfItems(null, this.selected_Cat, cat);
						this.selected_TextCat = cat;
					}, false, null, null);
					this.view.Gap(2f);
					SZWidgets.DefSelectorSimple<ThingDef>(this.view.GetRect(22f, 1f), w, this.lOfThingsByCat, ref this.sbullet.preExplosionSpawnThingDef, Label.PREEXPLOSIONSPAWNTHING, new Func<ThingDef, string>(FLabel.DefLabel<ThingDef>), delegate(ThingDef t)
					{
						this.sbullet.preExplosionSpawnThingDef = t;
						this.selectedDef.ResolveReferences();
					}, false, null, null, true, "");
					this.view.Gap(2f);
					Listing_X view8 = this.view;
					id = this.id;
					this.id = id + 1;
					SZWidgets.LabelIntFieldSlider(view8, w, id, FLabel.BulletExplosionSpawnThingCount, ref this.sbullet.preExplosionSpawnThingCount, 0, 20);
					Listing_X view9 = this.view;
					id = this.id;
					this.id = id + 1;
					SZWidgets.LabelFloatFieldSlider(view9, w, id, FLabel.BulletExplosionSpawnChance, ref this.sbullet.preExplosionSpawnChance, 0f, 100f, 1);
					this.view.Gap(10f);
					SZWidgets.DefSelectorSimple<ThingCategoryDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllThingCategoryDef, ref this.selected_Cat2, Label.FILTERCATEGORY, new Func<ThingCategoryDef, string>(FLabel.DefLabel<ThingCategoryDef>), delegate(ThingCategoryDef cat)
					{
						this.lOfThingsByCat2 = ThingTool.ListOfItems(null, cat, this.selected_TextCat2);
						this.selected_Cat2 = cat;
					}, false, null, null, true, "");
					this.view.Gap(2f);
					SZWidgets.NonDefSelectorSimple<ThingCategory>(this.view.GetRect(22f, 1f), w, ThingTool.AllThingCategory, ref this.selected_TextCat2, Label.FILTERTYPE, new Func<ThingCategory, string>(FLabel.EnumNameAndAll<ThingCategory>), delegate(ThingCategory cat)
					{
						this.lOfThingsByCat2 = ThingTool.ListOfItems(null, this.selected_Cat2, cat);
						this.selected_TextCat2 = cat;
					}, false, null, null);
					this.view.Gap(2f);
					SZWidgets.DefSelectorSimple<ThingDef>(this.view.GetRect(22f, 1f), w, this.lOfThingsByCat2, ref this.sbullet.postExplosionSpawnThingDef, Label.POSTEXPLOSIONSPAWNTHING, new Func<ThingDef, string>(FLabel.DefLabel<ThingDef>), delegate(ThingDef t)
					{
						this.sbullet.postExplosionSpawnThingDef = t;
						this.selectedDef.ResolveReferences();
					}, false, null, null, true, "");
					this.view.Gap(2f);
					Listing_X view10 = this.view;
					id = this.id;
					this.id = id + 1;
					SZWidgets.LabelIntFieldSlider(view10, w, id, FLabel.BulletPostExplosionSpawnThingCount, ref this.sbullet.postExplosionSpawnThingCount, 0, 20);
					Listing_X view11 = this.view;
					id = this.id;
					this.id = id + 1;
					SZWidgets.LabelFloatFieldSlider(view11, w, id, FLabel.BulletPostExplosionSpawnChance, ref this.sbullet.postExplosionSpawnChance, 0f, 100f, 1);
					SZWidgets.NonDefSelectorSimple<GasType?>(this.view.GetRect(22f, 1f), w, ThingTool.AllGasTypes, ref this.sbullet.postExplosionGasType, Label.GASTYPE, FLabel.GasType, delegate(GasType? g)
					{
						this.sbullet.postExplosionGasType = g;
						this.selectedDef.ResolveReferences();
					}, false, null, null);
					this.view.Gap(2f);
					SZWidgets.DefSelectorSimple<EffecterDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllEffecterDefs, ref this.sbullet.explosionEffect, Label.EXPLOSIONEFFECT, new Func<EffecterDef, string>(FLabel.DefName<EffecterDef>), delegate(EffecterDef e)
					{
						this.sbullet.explosionEffect = e;
					}, false, null, null, true, "");
					this.view.Gap(2f);
					SZWidgets.DefSelectorSimple<EffecterDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllEffecterDefs, ref this.sbullet.landedEffecter, Label.LANDEDEFFECT, new Func<EffecterDef, string>(FLabel.DefName<EffecterDef>), delegate(EffecterDef e)
					{
						this.sbullet.landedEffecter = e;
					}, false, null, null, true, "");
					this.view.Gap(2f);
				}
				Listing_X view12 = this.view;
				id = this.id;
				this.id = id + 1;
				SZWidgets.LabelFloatFieldSlider(view12, w, id, FLabel.ConsumeFuelPerShot, ref this.sverb.consumeFuelPerShot, 0f, 100f, 2);
				Listing_X view13 = this.view;
				id = this.id;
				this.id = id + 1;
				SZWidgets.LabelFloatFieldSlider(view13, w, id, FLabel.ConsumeFuelPerBurst, ref this.sverb.consumeFuelPerBurst, 0f, 100f, 2);
				this.view.Gap(10f);
				if (this.HasBullet)
				{
					this.view.CheckboxLabeled(Label.APPLYDAMAGETOEXPLOSIONCELLNEIGHBORS, 0f, (float)w, ref this.sbullet.applyDamageToExplosionCellsNeighbors, null, 2);
					this.view.CheckboxLabeled(Label.FLYOVERHEAD, 0f, (float)w, ref this.sbullet.flyOverhead, null, 2);
				}
				this.view.CheckboxLabeled(Label.REQUIRELINEOFSIGHT, 0f, (float)w, ref this.sverb.requireLineOfSight, null, 2);
				this.view.CheckboxLabeled(Label.BEAMTARGETGROUND, 0f, (float)w, ref this.sverb.beamTargetsGround, null, 2);
				if (this.sverb.targetParams != null)
				{
					this.view.CheckboxLabeled(Label.TARGETGROUND, 0f, (float)w, ref this.sverb.targetParams.canTargetLocations, null, 2);
				}
			}
		}

		
		private void DrawSounds(int w)
		{
			this.view.Gap(10f);
			if (this.HasBullet)
			{
				SZWidgets.DefSelectorSimple<SoundDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllGunRelatedSounds, ref this.sbullet.soundExplode, Label.SOUNDEXPLODE, FLabel.Sound, delegate(SoundDef s)
				{
					this.sbullet.soundExplode = SoundTool.GetAndPlay(s);
				}, true, "bsound", new Action<SoundDef>(SoundTool.PlayThis), true, "");
				this.view.Gap(2f);
				SZWidgets.DefSelectorSimple<SoundDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllGunRelatedSounds, ref this.sbullet.soundImpact, Label.SOUNDIMPACT, FLabel.Sound, delegate(SoundDef s)
				{
					this.sbullet.soundImpact = SoundTool.GetAndPlay(s);
				}, true, "bsound", new Action<SoundDef>(SoundTool.PlayThis), true, "");
				this.view.Gap(2f);
			}
			if (this.HasVerb)
			{
				SZWidgets.DefSelectorSimple<SoundDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllGunRelatedSounds, ref this.sverb.soundAiming, Label.SOUNDAIMING, FLabel.Sound, delegate(SoundDef s)
				{
					this.sverb.soundAiming = SoundTool.GetAndPlay(s);
				}, true, "bsound", new Action<SoundDef>(SoundTool.PlayThis), true, "");
				this.view.Gap(2f);
				SZWidgets.DefSelectorSimple<SoundDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllGunRelatedSounds, ref this.sverb.soundCastBeam, Label.SOUNDCASTBEAM, FLabel.Sound, delegate(SoundDef s)
				{
					this.sverb.soundCastBeam = SoundTool.GetAndPlay(s);
				}, true, "bsound", new Action<SoundDef>(SoundTool.PlayThis), true, "");
				this.view.Gap(2f);
				SZWidgets.DefSelectorSimple<SoundDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllGunRelatedSounds, ref this.sverb.soundCastTail, Label.SOUNDCASTTAIL, FLabel.Sound, delegate(SoundDef s)
				{
					this.sverb.soundCastTail = SoundTool.GetAndPlay(s);
				}, true, "bsound", new Action<SoundDef>(SoundTool.PlayThis), true, "");
				this.view.Gap(2f);
				SZWidgets.DefSelectorSimple<SoundDef>(this.view.GetRect(22f, 1f), w, ThingTool.AllGunRelatedSounds, ref this.sverb.soundLanding, Label.SOUNDLANDING, FLabel.Sound, delegate(SoundDef s)
				{
					this.sverb.soundLanding = SoundTool.GetAndPlay(s);
				}, true, "bsound", new Action<SoundDef>(SoundTool.PlayThis), true, "");
				this.view.Gap(2f);
			}
		}

		
		private void DrawRangedBig(int w)
		{
			if (this.HasVerb)
			{
				Text.Font = GameFont.Medium;
				this.view.GapLine(12f);
				if (this.HasBullet)
				{
					this.view.AddSection(Label.BULLET_SPEED, "cps", ref this.dummy, ref this.sbullet.speed, 0f, 150f, false, "");
				}
				this.view.AddIntSection("BurstShotFireRate".Translate(), "rpm", ref this.dummy, ref this.sverb.ticksBetweenBurstShots, 0, 100, false, "", false);
				this.view.AddIntSection("BurstShotCount".Translate(), "", ref this.dummy, ref this.sverb.burstShotCount, 0, 150, false, "", false);
				if (CEditor.IsCombatExtendedActive || (this.IsTurret && this.HasBullet))
				{
					this.ceWeaponFuelCapacity = this.CEWeaponFuelCapacity;
					this.view.AddIntSection(Label.MAGAZIN, "", ref this.dummy, ref this.ceWeaponFuelCapacity, 0, 500, false, "", false);
					this.CEWeaponFuelCapacity = this.ceWeaponFuelCapacity;
				}
				this.view.AddSection("Range".Translate(), "", ref this.dummy, ref this.sverb.range, 0f, 100f, false, "");
				this.view.AddSection(Label.MIN + "Range".Translate(), "", ref this.dummy, ref this.sverb.minRange, 0f, 50f, false, "");
				if (CEditor.IsCombatExtendedActive)
				{
					this.ceWeaponReloadTime = this.CEWeaponReloadTime;
					this.view.AddSection(Label.RELOADTIME, "s", ref this.dummy, ref this.ceWeaponReloadTime, 0f, 30f, false, "");
					this.CEWeaponReloadTime = this.ceWeaponReloadTime;
				}
				if (this.selectedDef.HasStat(StatDefOf.RangedWeapon_Cooldown))
				{
					float statValue = this.selectedDef.GetStatValue(StatDefOf.RangedWeapon_Cooldown);
					this.view.AddSection(StatDefOf.RangedWeapon_Cooldown.LabelCap, "s", ref this.dummy, ref statValue, 0f, 30f, false, "");
					this.selectedDef.SetStatBaseValue(StatDefOf.RangedWeapon_Cooldown, statValue);
				}
				else if (this.IsTurret)
				{
					this.view.AddSection(StatDefOf.RangedWeapon_Cooldown.LabelCap, "s", ref this.dummy, ref this.mTurretDef.building.turretBurstCooldownTime, 0f, 30f, false, "");
				}
				else
				{
					Listing_X view = this.view;
					int id = this.id;
					this.id = id + 1;
					SZWidgets.LabelFloatFieldSlider(view, w, id, FLabel.CooldownTime, ref this.sverb.defaultCooldownTime, 0f, 100f, 2);
				}
				if (this.IsTurret)
				{
					float min = this.mTurretDef.building.turretBurstWarmupTime.min;
					this.view.AddSection(Label.WARMUP, "s", ref this.dummy, ref min, 0f, 30f, false, "");
					this.mTurretDef.building.turretBurstWarmupTime.min = min;
					float max = this.mTurretDef.building.turretBurstWarmupTime.max;
					this.view.AddSection(Label.WARMUPMAX, "s", ref this.dummy, ref max, 0f, 30f, false, "");
					this.mTurretDef.building.turretBurstWarmupTime.max = max;
				}
				else
				{
					this.view.AddSection(Label.WARMUP, "s", ref this.dummy, ref this.sverb.warmupTime, 0f, 30f, false, "");
				}
				if (this.HasBullet)
				{
					this.view.AddSection("StoppingPower".Translate(), "s", ref this.dummy, ref this.sbullet.stoppingPower, 0f, 10f, false, "");
				}
				if (this.selectedDef.HasStat(StatDefOf.AccuracyTouch))
				{
					float statValue2 = this.selectedDef.GetStatValue(StatDefOf.AccuracyTouch);
					this.view.AddSection(StatDefOf.AccuracyTouch.label, "%", ref this.dummy, ref statValue2, 0f, 1f, false, "");
					this.selectedDef.SetStatBaseValue(StatDefOf.AccuracyTouch, statValue2);
				}
				else
				{
					Listing_X view2 = this.view;
					int id2 = this.id;
					this.id = id2 + 1;
					SZWidgets.LabelFloatFieldSlider(view2, w, id2, FLabel.AccuracyTouch, ref this.sverb.accuracyTouch, 0f, 100f, 2);
				}
				if (this.selectedDef.HasStat(StatDefOf.AccuracyShort))
				{
					float statValue3 = this.selectedDef.GetStatValue(StatDefOf.AccuracyShort);
					this.view.AddSection(StatDefOf.AccuracyShort.label, "%", ref this.dummy, ref statValue3, 0f, 1f, false, "");
					this.selectedDef.SetStatBaseValue(StatDefOf.AccuracyShort, statValue3);
				}
				else
				{
					Listing_X view3 = this.view;
					int id3 = this.id;
					this.id = id3 + 1;
					SZWidgets.LabelFloatFieldSlider(view3, w, id3, FLabel.AccuracyShort, ref this.sverb.accuracyShort, 0f, 100f, 2);
				}
				if (this.selectedDef.HasStat(StatDefOf.AccuracyMedium))
				{
					float statValue4 = this.selectedDef.GetStatValue(StatDefOf.AccuracyMedium);
					this.view.AddSection(StatDefOf.AccuracyMedium.label, "%", ref this.dummy, ref statValue4, 0f, 1f, false, "");
					this.selectedDef.SetStatBaseValue(StatDefOf.AccuracyMedium, statValue4);
				}
				else
				{
					Listing_X view4 = this.view;
					int id4 = this.id;
					this.id = id4 + 1;
					SZWidgets.LabelFloatFieldSlider(view4, w, id4, FLabel.AccuracyMedium, ref this.sverb.accuracyMedium, 0f, 100f, 2);
				}
				if (this.selectedDef.HasStat(StatDefOf.AccuracyLong))
				{
					float statValue5 = this.selectedDef.GetStatValue(StatDefOf.AccuracyLong);
					this.view.AddSection(StatDefOf.AccuracyLong.label, "%", ref this.dummy, ref statValue5, 0f, 1f, false, "");
					this.selectedDef.SetStatBaseValue(StatDefOf.AccuracyLong, statValue5);
					return;
				}
				Listing_X view5 = this.view;
				int id5 = this.id;
				this.id = id5 + 1;
				SZWidgets.LabelFloatFieldSlider(view5, w, id5, FLabel.AccuracyLong, ref this.sverb.accuracyLong, 0f, 100f, 2);
			}
		}

		
		private void DrawStuffCategories(int w)
		{
			if (this.selectedDef.race == null)
			{
				this.view.Label(0f, 0f, 500f, 30f, Label.STUFFPROPS, GameFont.Medium, "");
				this.view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate()
				{
					SZWidgets.FloatMenuOnRect<StuffCategoryDef>(ThingTool.lFreeStuffCategories, (StuffCategoryDef cat) => cat.SLabel(), delegate(StuffCategoryDef cat)
					{
						this.selectedDef.SetStuffCategorie(cat, ThingTool.SelectedThing);
					}, null, true);
				}, null);
				this.view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(base.ToggleRemove), new Color?(base.RemoveColor));
				this.view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", delegate()
				{
					this.selectedDef.CopyStuffCategories();
				}, null);
				if (!ThingTool.lCopyStuffCategories.NullOrEmpty<StuffCategoryDef>())
				{
					this.view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", delegate()
					{
						this.selectedDef.PasteStuffCategories();
					}, null);
				}
				this.view.Gap(30f);
				this.view.FullListViewParam1<StuffCategoryDef>(this.selectedDef.stuffCategories, ref this.selected_StuffCategorie, this.bRemoveOnClick, delegate(StuffCategoryDef cat)
				{
					this.selectedDef.RemoveStuffCategorie(cat);
				});
				this.view.GapLine(25f);
			}
		}

		
		private void DrawCosts(int w)
		{
			if (this.selectedDef.race == null)
			{
				this.view.Label(0f, 0f, 500f, 30f, Label.COSTS, GameFont.Medium, Label.TIP_BUILDINGCOSTS);
				this.view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate()
				{
					SZWidgets.FloatMenuOnRect<ThingDef>(ThingTool.lFreeCosts, (ThingDef cost) => cost.SLabel(), delegate(ThingDef cost)
					{
						this.selectedDef.SetCosts(cost, 0);
					}, null, true);
				}, null);
				this.view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(base.ToggleRemove), new Color?(base.RemoveColor));
				this.view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", delegate()
				{
					this.selectedDef.CopyCosts();
				}, null);
				if (!ThingTool.lCopyCosts.NullOrEmpty<ThingDefCountClass>())
				{
					this.view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", delegate()
					{
						this.selectedDef.PasteCosts();
					}, null);
				}
				this.view.Gap(30f);
				this.view.FullListViewParam<ThingDef, ThingDefCountClass>(this.selectedDef.costList, ref this.selected_CostDef, (ThingDefCountClass t) => t.thingDef, (ThingDefCountClass t) => (float)t.count, null, (ThingDefCountClass t) => 0f, (ThingDefCountClass t) => 99999f, true, this.bRemoveOnClick, delegate(ThingDefCountClass t, float val)
				{
					this.selectedDef.SetCosts(t.thingDef, (int)val);
				}, null, delegate(ThingDef t)
				{
					this.selectedDef.RemoveCosts(t);
				});
				this.view.GapLine(25f);
			}
		}

		
		private void DrawCostsDiff(int w)
		{
			if (this.selectedDef.costListForDifficulty != null)
			{
				this.view.Label(0f, 0f, 500f, 30f, Label.COSTS + Label.FORDIFFICULTY, GameFont.Medium, "");
				this.view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate()
				{
					SZWidgets.FloatMenuOnRect<ThingDef>(ThingTool.lFreeCostsDiff, (ThingDef cost) => cost.SLabel(), delegate(ThingDef cost)
					{
						this.selectedDef.SetCostsDiff(cost, 0);
					}, null, true);
				}, null);
				this.view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(base.ToggleRemove), new Color?(base.RemoveColor));
				this.view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", delegate()
				{
					this.selectedDef.CopyCostsDiff();
				}, null);
				if (!ThingTool.lCopyCostsDiff.NullOrEmpty<ThingDefCountClass>())
				{
					this.view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", delegate()
					{
						this.selectedDef.PasteCostsDiff();
					}, null);
				}
				this.view.Gap(30f);
				this.view.FullListViewParam<ThingDef, ThingDefCountClass>(this.selectedDef.costListForDifficulty.costList, ref this.selected_CostDefDiff, (ThingDefCountClass t) => t.thingDef, (ThingDefCountClass t) => (float)t.count, null, (ThingDefCountClass t) => 0f, (ThingDefCountClass t) => 99999f, true, this.bRemoveOnClick, delegate(ThingDefCountClass t, float val)
				{
					this.selectedDef.SetCostsDiff(t.thingDef, (int)val);
				}, null, delegate(ThingDef t)
				{
					this.selectedDef.RemoveCostsDiff(t);
				});
				this.view.GapLine(25f);
			}
		}

		
		private void DrawLayers(int w)
		{
			if (this.selectedDef.apparel != null)
			{
				this.view.Label(0f, 0f, 500f, 30f, Label.APPARELLAYER, GameFont.Medium, "");
				this.view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate()
				{
					SZWidgets.FloatMenuOnRect<ApparelLayerDef>(ThingTool.lFreeApparelLayer, new Func<ApparelLayerDef, string>(FLabel.DefLabel<ApparelLayerDef>), delegate(ApparelLayerDef lay)
					{
						this.selectedDef.SetApparelLayer(lay);
					}, null, true);
				}, null);
				this.view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(base.ToggleRemove), new Color?(base.RemoveColor));
				this.view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", delegate()
				{
					this.selectedDef.CopyApparelLayer();
				}, null);
				if (!ThingTool.lCopyApparelLayer.NullOrEmpty<ApparelLayerDef>())
				{
					this.view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", delegate()
					{
						this.selectedDef.PasteApparelLayer();
					}, null);
				}
				this.view.Gap(30f);
				this.view.FullListViewParam1<ApparelLayerDef>(this.selectedDef.apparel.layers, ref this.selected_ApparelLayer, this.bRemoveOnClick, delegate(ApparelLayerDef t)
				{
					this.selectedDef.RemoveApparelLayer(t);
				});
				this.view.GapLine(25f);
			}
		}

		
		private void DrawCoveredBodyParts(int w)
		{
			if (this.selectedDef.apparel != null)
			{
				this.view.Label(0f, 0f, 500f, 30f, Label.BODYPARTGROUPS, GameFont.Medium, "");
				this.view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate()
				{
					SZWidgets.FloatMenuOnRect<BodyPartGroupDef>(ThingTool.lFreeBodyPartGroup, new Func<BodyPartGroupDef, string>(FLabel.DefLabel<BodyPartGroupDef>), delegate(BodyPartGroupDef b)
					{
						this.selectedDef.SetBodyPartGroup(b);
					}, null, true);
				}, null);
				this.view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(base.ToggleRemove), new Color?(base.RemoveColor));
				this.view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", delegate()
				{
					this.selectedDef.CopyBodyPartGroup();
				}, null);
				if (!ThingTool.lCopyBodyPartGroup.NullOrEmpty<BodyPartGroupDef>())
				{
					this.view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", delegate()
					{
						this.selectedDef.PasteBodyPartGroup();
					}, null);
				}
				this.view.Gap(30f);
				this.view.FullListViewParam1<BodyPartGroupDef>(this.selectedDef.apparel.bodyPartGroups, ref this.selected_BodyPartGroup, this.bRemoveOnClick, delegate(BodyPartGroupDef t)
				{
					this.selectedDef.RemoveBodyPartGroup(t);
				});
				this.view.GapLine(25f);
			}
		}

		
		private void DrawResearchBuilding(int w)
		{
			if (this.selectedDef.building != null)
			{
				this.view.Label(0f, 0f, 500f, 30f, Label.BUILDINGPREREQUISITES, GameFont.Medium, "");
				this.view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate()
				{
					SZWidgets.FloatMenuOnRect<ResearchProjectDef>(ThingTool.lFreePrerequisites, (ResearchProjectDef p) => p.SLabel(), delegate(ResearchProjectDef p)
					{
						this.selectedDef.SetPrerequisite(p);
					}, null, true);
				}, null);
				this.view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(base.ToggleRemove), new Color?(base.RemoveColor));
				this.view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", delegate()
				{
					this.selectedDef.CopyPrerequisites();
				}, null);
				if (!ThingTool.lCopyPrerequisites.NullOrEmpty<ResearchProjectDef>())
				{
					this.view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", delegate()
					{
						this.selectedDef.PastePrerequisites();
					}, null);
				}
				this.view.Gap(30f);
				this.view.FullListViewParam1<ResearchProjectDef>(this.selectedDef.researchPrerequisites, ref this.selected_ResearchDef, this.bRemoveOnClick, delegate(ResearchProjectDef t)
				{
					this.selectedDef.RemovePrerequisite(t);
				});
				this.view.GapLine(25f);
			}
		}

		
		private void DrawTradeTags(int w)
		{
			SZWidgets.DrawStringList(ref this.selectedDef.tradeTags, CEditor.API.ListOf<string>(EType.TradeTags), w, this.view, Label.TRADETAGS, ref ThingTool.lCopyTradeTags, delegate(string s)
			{
				List<string> tradeTags = this.selectedDef.tradeTags;
				if (tradeTags != null)
				{
					tradeTags.Remove(s);
				}
			}, delegate(string s)
			{
				Extension.AddElem<string>(ref this.selectedDef.tradeTags, s);
			});
		}

		
		private void DrawApparelTags(int w)
		{
			if (this.selectedDef.apparel != null)
			{
				SZWidgets.DrawStringList(ref this.selectedDef.apparel.tags, CEditor.API.ListOf<string>(EType.ApparelTags), w, this.view, Label.APPARELTAGS, ref ThingTool.lCopyApparelTags, delegate(string s)
				{
					List<string> tags = this.selectedDef.apparel.tags;
					if (tags != null)
					{
						tags.Remove(s);
					}
				}, delegate(string s)
				{
					Extension.AddElem<string>(ref this.selectedDef.apparel.tags, s);
				});
			}
		}

		
		private void DrawOutfitTags(int w)
		{
			if (this.selectedDef.apparel != null)
			{
				SZWidgets.DrawStringList(ref this.selectedDef.apparel.defaultOutfitTags, CEditor.API.ListOf<string>(EType.OutfitTags), w, this.view, Label.OUTFITTAGS, ref ThingTool.lCopyOutfitTags, delegate(string s)
				{
					List<string> defaultOutfitTags = this.selectedDef.apparel.defaultOutfitTags;
					if (defaultOutfitTags != null)
					{
						defaultOutfitTags.Remove(s);
					}
				}, delegate(string s)
				{
					Extension.AddElem<string>(ref this.selectedDef.apparel.defaultOutfitTags, s);
				});
			}
		}

		
		private void DrawWeaponTags(int w)
		{
			SZWidgets.DrawStringList(ref this.selectedDef.weaponTags, CEditor.API.ListOf<string>(EType.WeaponTags), w, this.view, Label.WEAPONTAGS, ref ThingTool.lCopyWeaponTags, delegate(string s)
			{
				List<string> weaponTags = this.selectedDef.weaponTags;
				if (weaponTags != null)
				{
					weaponTags.Remove(s);
				}
			}, delegate(string s)
			{
				Extension.AddElem<string>(ref this.selectedDef.weaponTags, s);
			});
		}

		
		private void DrawWeaponTraits(int w)
		{
			if (ThingTool.SelectedThing.tempThing != null)
			{
				CompBladelinkWeapon compBladelinkWeapon = ThingTool.SelectedThing.tempThing.TryGetComp<CompBladelinkWeapon>();
				if (compBladelinkWeapon != null)
				{
					this.view.Label(0f, 0f, 500f, 30f, Label.BLADELINKTRAITS, GameFont.Medium, "");
					this.view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate()
					{
						SZWidgets.FloatMenuOnRect<WeaponTraitDef>(ThingTool.AllWeaponTraitDef, (WeaponTraitDef t) => t.SLabel(), delegate(WeaponTraitDef t)
						{
							ThingTool.SelectedThing.tempThing.SetBladeLinkTrait(t);
						}, null, true);
					}, null);
					this.view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(base.ToggleRemove), new Color?(base.RemoveColor));
					this.view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", delegate()
					{
						ThingTool.SelectedThing.tempThing.CopyBladeLinkTraits();
					}, null);
					if (!ThingTool.lCopyBladeLinkTraits.NullOrEmpty<WeaponTraitDef>())
					{
						this.view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", delegate()
						{
							ThingTool.SelectedThing.tempThing.PasteBladeLinkTraits();
						}, null);
					}
					this.view.Gap(30f);
					this.view.FullListViewParam1<WeaponTraitDef>(compBladelinkWeapon.TraitsListForReading, ref this.selected_WeaponTrait, this.bRemoveOnClick, delegate(WeaponTraitDef t)
					{
						ThingTool.SelectedThing.tempThing.RemoveBladeLinkTrait(t);
					});
					this.view.GapLine(25f);
					bool memberValue = compBladelinkWeapon.GetMemberValue("biocoded", false);
					this.view.CheckboxLabeled(Label.BIOCODED, 0f, 400f, ref memberValue, null, 2);
					if (!memberValue && compBladelinkWeapon.CodedPawn != null)
					{
						compBladelinkWeapon.UnCode();
						return;
					}
					if (memberValue && compBladelinkWeapon.CodedPawn != this.tempPawn)
					{
						compBladelinkWeapon.CodeFor(this.tempPawn);
					}
				}
			}
		}

		
		internal void DrawLabel()
		{
			GUI.color = ColorTool.colBeige;
			int id;
			if (ThingTool.SelectedThing.tempThing != null)
			{
				CompGeneratedNames compGeneratedNames = ThingTool.SelectedThing.tempThing.TryGetComp<CompGeneratedNames>();
				if (compGeneratedNames != null)
				{
					string name = compGeneratedNames.Name;
					Rect rect = this.view.GetRect(30f, 1f);
					id = this.id;
					this.id = id + 1;
					SZWidgets.LabelEdit(rect, id, "", ref name, GameFont.Medium, true);
					compGeneratedNames.SetMemberValue("name", name);
				}
			}
			Rect rect2 = this.view.GetRect(30f, 1f);
			id = this.id;
			this.id = id + 1;
			SZWidgets.LabelEdit(rect2, id, "", ref this.selectedDef.label, GameFont.Medium, true);
			GUI.color = Color.white;
			this.view.Gap(4f);
		}

		
		internal void DrawStatFactors(int w)
		{
			this.view.GapLine(10f);
			this.view.Label(0f, 0f, 500f, 30f, Label.STAT_FACTORS, GameFont.Medium, "");
			this.view.ButtonImage<bool>((float)(w - 380), 5f, 24f, 24f, "bnone", ThingTool.UseAllCategories ? Color.white : Color.gray, delegate(bool b)
			{
				ThingTool.UseAllCategories = !ThingTool.UseAllCategories;
			}, ThingTool.UseAllCategories, Label.TIP_TOGGLECATEGORIES);
			Text.Font = GameFont.Tiny;
			this.view.FloatMenuButtonWithLabelDef<StatCategoryDef>("", (float)(w - 350), 200f, DefTool.CategoryLabel(ThingTool.StatFactorCategory), ThingTool.lCategoryDef_Factors, DefTool.CategoryLabel, delegate(StatCategoryDef cat)
			{
				ThingTool.StatFactorCategory = cat;
			}, 0f);
			Text.Font = GameFont.Small;
			this.view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate()
			{
				SZWidgets.FloatMenuOnRect<StatDef>(ThingTool.lFreeStatDefFactors, (StatDef s) => s.SLabel(), delegate(StatDef stat)
				{
					ThingTool.SelectedThing.thingDef.SetStatFactor(stat, 0f);
				}, null, true);
			}, null);
			this.view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(base.ToggleRemove), new Color?(base.RemoveColor));
			this.view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", delegate()
			{
				ThingTool.SelectedThing.thingDef.CopyStatFactors();
			}, null);
			if (!ThingTool.lCopyStatFactors.NullOrEmpty<StatModifier>())
			{
				this.view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", delegate()
				{
					ThingTool.SelectedThing.thingDef.PasteStatFactors();
				}, null);
			}
			this.view.Gap(30f);
			this.view.FullListViewParam<StatDef, StatModifier>(ThingTool.SelectedThing.thingDef.statBases, ref this.selected_StatFactor, (StatModifier s) => s.stat, (StatModifier s) => s.value, null, delegate(StatModifier s)
			{
				if (!ThingTool.UseAllCategories)
				{
					return s.stat.minValue;
				}
				return float.MinValue;
			}, delegate(StatModifier s)
			{
				if (!ThingTool.UseAllCategories)
				{
					return s.stat.maxValue;
				}
				return float.MaxValue;
			}, false, this.bRemoveOnClick, delegate(StatModifier s, float val)
			{
				s.value = val;
			}, null, delegate(StatDef stat)
			{
				ThingTool.SelectedThing.thingDef.RemoveStatFactor(stat);
			});
			this.view.GapLine(25f);
		}

		
		internal void DrawStatOffsets(int w)
		{
			this.view.Label(0f, 0f, 500f, 30f, Label.STAT_OFFSETS, GameFont.Medium, "");
			Text.Font = GameFont.Tiny;
			this.view.FloatMenuButtonWithLabelDef<StatCategoryDef>("", (float)(w - 350), 200f, DefTool.CategoryLabel(ThingTool.StatOffsetCategory), ThingTool.lCategoryDef_Offsets, DefTool.CategoryLabel, delegate(StatCategoryDef cat)
			{
				ThingTool.StatOffsetCategory = cat;
			}, 0f);
			Text.Font = GameFont.Small;
			this.view.ButtonImage((float)(w - 60), 5f, 24f, 24f, "UI/Buttons/Dev/Add", delegate()
			{
				SZWidgets.FloatMenuOnRect<StatDef>(ThingTool.lFreeStatDefOffsets, (StatDef s) => s.SLabel(), delegate(StatDef stat)
				{
					ThingTool.SelectedThing.thingDef.SetStatOffset(stat, 0f);
				}, null, true);
			}, null);
			this.view.ButtonImage((float)(w - 85), 5f, 24f, 24f, "bminus", new Action(base.ToggleRemove), new Color?(base.RemoveColor));
			this.view.ButtonImage((float)(w - 110), 5f, 18f, 24f, "UI/Buttons/Copy", delegate()
			{
				ThingTool.SelectedThing.thingDef.CopyStatOffsets();
			}, null);
			if (!ThingTool.lCopyStatOffsets.NullOrEmpty<StatModifier>())
			{
				this.view.ButtonImage((float)(w - 130), 5f, 18f, 24f, "UI/Buttons/Paste", delegate()
				{
					ThingTool.SelectedThing.thingDef.PasteStatOffsets();
				}, null);
			}
			this.view.Gap(30f);
			this.view.FullListViewParam<StatDef, StatModifier>(ThingTool.SelectedThing.thingDef.equippedStatOffsets, ref this.selected_StatOffset, (StatModifier s) => s.stat, (StatModifier s) => s.value, null, delegate(StatModifier s)
			{
				if (!ThingTool.UseAllCategories)
				{
					return s.stat.minValue;
				}
				return float.MinValue;
			}, delegate(StatModifier s)
			{
				if (!ThingTool.UseAllCategories)
				{
					return s.stat.maxValue;
				}
				return float.MaxValue;
			}, false, this.bRemoveOnClick, delegate(StatModifier s, float val)
			{
				s.value = val;
			}, null, delegate(StatDef stat)
			{
				ThingTool.SelectedThing.thingDef.RemoveStatOffset(stat);
			});
			this.view.GapLine(25f);
		}
}
