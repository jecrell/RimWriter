using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWriter;

public class ITab_Inventory : ITab
{
    private const float StandardLineHeight = 22f;

    private const float ThingIconSize = 28f;

    private const float ThingLeftX = 36f;

    private const float ThingRowHeight = 28f;

    private const float TopPadding = 20f;

    private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private static readonly Color ThingLabelColor = new Color(0.9f, 0.9f, 0.9f, 1f);

    private static readonly List<Thing> workingInvList = new List<Thing>();

    private Vector2 scrollPosition = Vector2.zero;

    private float scrollViewHeight;

    // private Pawn SelPawnForGear
    // {
    // get
    // {
    // if (base.SelPawn != null)
    // {
    // return base.SelPawn;
    // }
    // Corpse corpse = base.SelThing as Corpse;
    // if (corpse != null)
    // {
    // return corpse.InnerPawn;
    // }
    // throw new InvalidOperationException("Gear tab on non-pawn non-corpse " + base.SelThing);
    // }
    // }
    public ITab_Inventory()
    {
        size = new Vector2(460f, 450f);
        labelKey = "RimWriter_Inventory";
    }

    public override bool IsVisible => SelStorage.TryGetInnerInteractableThingOwner().Count > 0;

    private bool CanControl => SelStorage.Spawned && SelStorage.Faction == Faction.OfPlayer;

    private Building_InternalStorage SelStorage
    {
        get
        {
            if (SelThing is Building_InternalStorage bld)
            {
                return bld;
            }

            return null;
        }
    }

    protected override void FillTab()
    {
        Text.Font = GameFont.Small;
        var rect = new Rect(0f, 20f, size.x, size.y - 20f);
        var rect2 = rect.ContractedBy(10f);
        var position = new Rect(rect2.x, rect2.y, rect2.width, rect2.height);
        GUI.BeginGroup(position);
        Text.Font = GameFont.Small;
        GUI.color = Color.white;
        var outRect = new Rect(0f, 0f, position.width, position.height);
        var viewRect = new Rect(0f, 0f, position.width - 16f, scrollViewHeight);
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        var num = 0f;

        // this.TryDrawMassInfo(ref num, viewRect.width);
        // this.TryDrawComfyTemperatureRange(ref num, viewRect.width);
        // if (this.SelPawnForGear.apparel != null)
        // {
        // bool flag = false;
        // this.TryDrawAverageArmor(ref num, viewRect.width, StatDefOf.ArmorRating_Blunt, "ArmorBlunt".Translate(), ref flag);
        // this.TryDrawAverageArmor(ref num, viewRect.width, StatDefOf.ArmorRating_Sharp, "ArmorSharp".Translate(), ref flag);
        // this.TryDrawAverageArmor(ref num, viewRect.width, StatDefOf.ArmorRating_Heat, "ArmorHeat".Translate(), ref flag);
        // this.TryDrawAverageArmor(ref num, viewRect.width, StatDefOf.ArmorRating_Electric, "ArmorElectric".Translate(), ref flag);
        // }
        // if (this.SelPawnForGear.equipment != null)
        // {
        // Widgets.ListSeparator(ref num, viewRect.width, "Equipment".Translate());
        // foreach (ThingWithComps current in this.SelPawnForGear.equipment.AllEquipmentListForReading)
        // {
        // this.DrawThingRow(ref num, viewRect.width, current, false);
        // }
        // }
        // if (this.SelPawnForGear.apparel != null)
        // {
        // Widgets.ListSeparator(ref num, viewRect.width, "Apparel".Translate());
        // foreach (Apparel current2 in from ap in this.SelPawnForGear.apparel.WornApparel
        // orderby ap.def.apparel.bodyPartGroups[0].listOrder descending
        // select ap)
        // {
        // this.DrawThingRow(ref num, viewRect.width, current2, false);
        // }
        // }
        if (SelStorage.TryGetInnerInteractableThingOwner() is { } t)
        {
            Widgets.ListSeparator(ref num, viewRect.width, "Inventory".Translate());
            workingInvList.Clear();
            workingInvList.AddRange(t);
            foreach (var thing in workingInvList)
            {
                DrawThingRow(ref num, viewRect.width, thing, true);
            }
        }

        if (Event.current.type == EventType.Layout)
        {
            scrollViewHeight = num + 30f;
        }

        Widgets.EndScrollView();
        GUI.EndGroup();
        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;
    }

    private void DrawThingRow(ref float y, float width, Thing thing, bool inventory = false)
    {
        var rect = new Rect(0f, y, width, 28f);
        Widgets.InfoCardButton(rect.width - 24f, y, thing);
        rect.width -= 24f;
        if (CanControl)
        {
            var rect2 = new Rect(rect.width - 24f, y, 24f, 24f);
            TooltipHandler.TipRegion(rect2, "DropThing".Translate());
            if (Widgets.ButtonImage(rect2, ITabButton.Drop))
            {
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
                InterfaceDrop(thing);
            }

            rect.width -= 24f;
        }

        var rect4 = rect;
        rect4.xMin = rect4.xMax - 60f;

        CaravanThingsTabUtility.DrawMass(thing, rect4);
        rect.width -= 60f;
        if (Mouse.IsOver(rect))
        {
            GUI.color = HighlightColor;
            GUI.DrawTexture(rect, TexUI.HighlightTex);
        }

        if (thing.def.DrawMatSingle != null && thing.def.DrawMatSingle.mainTexture != null)
        {
            Widgets.ThingIcon(new Rect(4f, y, 28f, 28f), thing);
        }

        Text.Anchor = TextAnchor.MiddleLeft;
        GUI.color = ThingLabelColor;
        var rect5 = new Rect(36f, y, rect.width - 36f, rect.height);
        var text = thing.LabelCap;
        Text.WordWrap = false;
        Widgets.Label(rect5, text.Truncate(rect5.width));
        Text.WordWrap = true;
        var text2 = thing.LabelCap;
        if (thing.def.useHitPoints)
        {
            var text3 = text2;
            text2 = $"{text3}\n{thing.HitPoints} / {thing.MaxHitPoints}";
        }

        TooltipHandler.TipRegion(rect, text2);
        y += 28f;
    }

    // private void TryDrawMassInfo(ref float curY, float width)
    // {
    // if (!this.selStorage.Spawned)
    // {
    // return;
    // }
    // Rect rect = new Rect(0f, curY, width, 22f);
    // float num = MassUtility.GearAndInventoryMass(this.selStorage);
    // float num2 = MassUtility.Capacity(this.SelPawnForGear);
    // Widgets.Label(rect, "MassCarried".Translate(new object[]
    // {
    // num.ToString("0.##"),
    // num2.ToString("0.##")
    // }));
    // curY += 22f;
    // }
    private void InterfaceDrop(Thing t)
    {
        SelStorage.TryDrop(t);
    }
}