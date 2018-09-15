//What I need
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
//Maybe?
using RimWorld;
using RimWorld.Planet;
using Verse.AI;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class PawnGroupMaker_Smuggler : PawnGroupMaker_Trader
    {

        private Pawn GenerateTrader(IncidentParms parms, TraderKindDef traderKind)
        {
            Pawn pawn = PawnGenerator.GeneratePawn(this.traders.RandomElementByWeight((PawnGenOption x) => (float)x.selectionWeight).kind, parms.faction);
            pawn.mindState.wantsToTradeWithColony = true;
            PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, true);
            pawn.trader.traderKind = traderKind;
            this.PostGenerate(pawn);
            return pawn;
        }
        
    }
}
