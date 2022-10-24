﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace ItsSorceryFramework
{
    public class EnergyTracker_Vancian : EnergyTracker
    {
        // initalizer- created via activator via SorcerySchema
        public EnergyTracker_Vancian(Pawn pawn, EnergyTrackerDef def) : base(pawn, def)
        {
            InitalizeSorceries();
            tickCount = def.refreshTicks;
        }

        public EnergyTracker_Vancian(Pawn pawn, SorcerySchemaDef def) : base(pawn, def)
        {
            InitalizeSorceries();
            tickCount = this.def.refreshTicks;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref vancianCasts, "vancianCasts", LookMode.Def, LookMode.Value);
            Scribe_Values.Look(ref tickCount, "tickCount");
        }

        public virtual void InitalizeSorceries()
        {
            foreach(SorceryDef sd in from sorceryDef in DefDatabase<SorceryDef>.AllDefs 
                                     where sorceryDef.sorcerySchema.energyTrackerDef == def
                                     select sorceryDef)
            {
                if(!vancianCasts.ContainsKey(sd)) vancianCasts.Add(sd, (int) Math.Ceiling(sd.MaximumCasts * CastFactor));
            }
        }

        public virtual float CastFactor
        {
            get
            {
                return this.pawn.GetStatValue(def.CastFactorStatDef ?? StatDefOf_ItsSorcery.CastFactor_ItsSorcery, true);
            }
        }

        public override void EnergyTrackerTick()
        {
            if(tickCount > 0)
            {
                tickCount--;
                if(tickCount == 0)
                {
                    tickCount = this.def.refreshTicks;
                    this.RefreshAllCasts();
                }
            }
        }

        public virtual void RefreshAllCasts()
        {
            foreach (var sorceryPair in vancianCasts)
            {
                vancianCasts[sorceryPair.Key] = (int) Math.Ceiling(sorceryPair.Key.MaximumCasts * CastFactor);
            }
        }

        public override bool WouldReachLimitEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            if (vancianCasts[sorceryDef] <= 0) return true;
            return false;
        }

        public override bool TryAlterEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            if (!WouldReachLimitEnergy(energyCost, sorceryDef))
            {
                vancianCasts[sorceryDef]--;
                return true;
            }
            
            return false;
        }

        public override void DrawOnGUI(Rect rect)
        {
            if (Widgets.ButtonTextSubtle(rect, ""))
            {
                Find.WindowStack.Add(new Dialog_MessageBox("magic", null, null, null, null, null, false, null, null, WindowLayer.Dialog));
            }

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rect, sorcerySchemaDef.LabelCap.ToString());

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, def.TranslatedRefreshNotif.Translate(GenDate.ToStringTicksToPeriod(tickCount)));
            
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public override string TopRightLabel(SorceryDef sorceryDef)
        {
            return (sorceryDef?.sorcerySchema.energyTrackerDef.energyStatLabel.CapitalizeFirst()[0]) + ": " +
                vancianCasts[sorceryDef].ToString() + "/" +
                ((int) Math.Ceiling(sorceryDef.MaximumCasts * this.CastFactor)).ToString();
        }

        public Dictionary<SorceryDef, int> vancianCasts = new Dictionary<SorceryDef, int> ();

        public int tickCount = 0;

    }
}
