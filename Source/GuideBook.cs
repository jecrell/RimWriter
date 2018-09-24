using System;
using Harmony;
using RimWorld;
using Verse;

namespace RimWriter
{
    public class GuideBook : ThingBook
    {
        private SkillDef skillDef;
        private const float learnRate = 0.085f;
        private CompQuality compQuality;
        public CompQuality CompQuality => this.TryGetComp<CompQuality>();
        public CompArt CompArt => this.TryGetComp<CompArt>();

        public float QualityRate
        {
            get
            {
                switch (CompQuality.Quality)
                {
                    case QualityCategory.Awful:
                        return 0.25f;
                    case QualityCategory.Poor:
                        return 0.5f;
                    case QualityCategory.Normal:
                        return 0.75f;
                    case QualityCategory.Good:
                        return 1.0f;
                    case QualityCategory.Excellent:
                        return 1.25f;
                    case QualityCategory.Masterwork:
                        return 1.5f;
                    case QualityCategory.Legendary:
                        return 1.75f;
                }
                return 0.5f;
            }
        }
        
        public float LearnRate
        {
            get
            {
                return learnRate * QualityRate;
            }
        }

        public GuideBook()
        {
            if (skillDef == null)
            {
                skillDef = DefDatabase<SkillDef>.GetRandom();
            }           
        }
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (skillDef == null)
            {
                skillDef = DefDatabase<SkillDef>.GetRandom();
            }
        }

        public void Teach(Pawn pawn)
        {
            pawn?.skills?.Learn(skillDef, LearnRate);
        }
        
        
        public override string Label
        {
            get
            {
                if (CompArt != null)
                {
                    var authorNameInt = Traverse.Create(CompArt).Field("authorNameInt").GetValue<string>();
                    return (authorNameInt.NullOrEmpty())
                        ? "RimWriter_GuideTitle".Translate(skillDef.LabelCap) + " (" + CompQuality.Quality.GetLabel() + ")"
                        : "RimWriter_GuideTitleWithAuthor".Translate(new object[]{CompArt.AuthorName, skillDef.LabelCap}) + " (" + CompQuality.Quality.GetLabel() + ")";
                }

                return base.Label;
            }
        }

        public override string DescriptionFlavor
        {
            get => "RimWriter_GuideSkillDesc".Translate(new object[]{skillDef.label, QualityRate.ToStringPercent()}) + "\n" + base.DescriptionFlavor;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref skillDef, "skillDef");
        }
    }
}