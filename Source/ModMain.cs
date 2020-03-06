using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWriter
{
    [StaticConstructorOnStartup]
    public class CurrentSettings : ModSettings
    {
    }
    
    public class ModMain : Mod
    {
        public bool simpleRecipes;
        
        public ModMain(ModContentPack content) : base(content)
        {
            ApplySettings();
        }
        
        
        public override string SettingsCategory() => "RimWriter";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var rectHeight = inRect.height * 0.15f;
            var rectGap = 7f;
            var checkboxRect = new Rect(inRect.x, inRect.y, inRect.width * 0.25f, rectHeight);
            Widgets.CheckboxLabeled(checkboxRect,
                "RimWriter_ComplexRecipes".Translate(), ref this.simpleRecipes);
            TooltipHandler.TipRegion(checkboxRect, () => "RimWriter_ComplexRecipesTip".Translate(), 67233415);
            if (Widgets.ButtonText(
                new Rect(inRect.x, inRect.y + rectHeight + rectGap, inRect.width * 0.25f, rectHeight),
                "RimWriter_ApplySettings".Translate()))
            {
                ApplySettings();
            }
        }

        public override void WriteSettings()
        {
            Scribe_Values.Look(ref this.simpleRecipes, "complexRecipes", false);
            base.WriteSettings();
        }

        private void ApplySettings()
        {
            Predicate<ThingDefCountClass> woodPred = x => x.thingDef == ThingDefOf.WoodLog;
            Predicate<ThingDefCountClass> pagesPred = x =>
                x.thingDef == GetDef("Jecrell_PageBook") ||
                x.thingDef == GetDef("Jecrell_PageHandwritten") ||
                x.thingDef == GetDef("Jecrell_PageJournalHandwritten") ||
                x.thingDef == GetDef("Jecrell_PageJournal") ||
                x.thingDef == GetDef("Jecrell_Page");
            var pagesTDC = new ThingDefCountClass(GetDef("Jecrell_Page"), 200);
            var logsTDC = new ThingDefCountClass(ThingDefOf.WoodLog, 10);
            if (!simpleRecipes)
            {
                if (GetDef("Jecrell_BookJournal") is ThingDef bookJournal)
                {
                    bookJournal.costList.RemoveAll(woodPred);
                    bookJournal.costList.Add(pagesTDC);
                }

                if (GetDef("Jecrell_BookJournalHandwritten") is ThingDef bookJournalHandwritten)
                {
                    bookJournalHandwritten.costList.RemoveAll(woodPred);
                    bookJournalHandwritten.costList.Add(pagesTDC);
                }

                if (GetDef("Jecrell_Book") is ThingDef bookTyped)
                {
                    bookTyped.costList.RemoveAll(woodPred);
                    bookTyped.costList.Add(pagesTDC);
                }

                if (GetDef("Jecrell_BookHandwritten") is ThingDef bookHandwritten)
                {
                    bookHandwritten.costList.RemoveAll(woodPred);
                    bookHandwritten.costList.Add(pagesTDC);
                }
            }
            else
            {
                if (GetDef("Jecrell_BookJournal") is ThingDef bookJournal)
                {
                    bookJournal.costList.RemoveAll(pagesPred);
                    bookJournal.costList.Add(logsTDC);
                }

                if (GetDef("Jecrell_BookJournalHandwritten") is ThingDef bookJournalHandwritten)
                {
                    bookJournalHandwritten.costList.RemoveAll(pagesPred);
                    bookJournalHandwritten.costList.Add(logsTDC);
                }

                if (GetDef("Jecrell_Book") is ThingDef bookTyped)
                {
                    bookTyped.costList.RemoveAll(pagesPred);
                    bookTyped.costList.Add(logsTDC);
                }

                if (GetDef("Jecrell_BookHandwritten") is ThingDef bookHandwritten)
                {
                    bookHandwritten.costList.RemoveAll(pagesPred);
                    bookHandwritten.costList.Add(logsTDC);
                }
            }
        }

        private static ThingDef GetDef(string jecrellBookjournal)
        {
            return DefDatabase<ThingDef>.GetNamedSilentFail(jecrellBookjournal);
        }
    }
}