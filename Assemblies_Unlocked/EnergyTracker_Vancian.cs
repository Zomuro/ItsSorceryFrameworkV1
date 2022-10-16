using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ItsSorceryFramework
{
    public class EnergyTracker_Vancian : EnergyTracker
    {
        // initalizer- created via activator via SorcerySchema
        public EnergyTracker_Vancian(Pawn pawn, EnergyTrackerDef def) : base(pawn, def)
        {
            this.pawn = pawn;
            this.def = def;
            InitalizeSorceries();
            dayCount = this.RefreshDays;
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public virtual void InitalizeSorceries()
        {
            foreach(SorceryDef sd in from sorceryDef in DefDatabase<SorceryDef>.AllDefs 
                                     where sorceryDef.sorcerySchema.energyTrackerDef == def
                                     select sorceryDef)
            {
                vancianCasts.Add(sd, sd.MaximumCasts);
            }
        }

        public virtual int RefreshTick
        {
            get
            {
                return def.refreshTick % 60000;
            }
        }

        public virtual int RefreshDays
        {
            get
            {
                return Math.Max(0, def.refreshDays);
            }
        }

        public override float EnergyCostFactor
        {
            get
            {
                if (def.energyCostFactorStatDef is null) return this.pawn.GetStatValue(StatDefOf_ItsSorcery.EnergyRecovery_ItsSorcery, true);
                return this.pawn.GetStatValue(def.energyCostFactorStatDef, true);
            }
        }

        public override void EnergyTrackerTick()
        {
            if(GenLocalDate.DayTick(Find.AnyPlayerHomeMap) == RefreshTick)
            {
                dayCount--;
                if(dayCount == 0)
                {
                    dayCount = this.RefreshDays;
                    this.RefreshAllCasts();
                }
            }
        }

        public virtual void RefreshAllCasts()
        {
            foreach (var sorceryPair in vancianCasts)
            {
                vancianCasts[sorceryPair.Key] = sorceryPair.Key.MaximumCasts;
            }
        }

        public override bool WouldReachLimitEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            if (currentEnergy - energyCost < MinEnergy) return true;
            return false;
        }

        public override bool TryAlterEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            if (!WouldReachLimitEnergy(energyCost))
            {
                currentEnergy = Math.Max(MinEnergy, currentEnergy - energyCost);
                return true;
            }
            
            return false;
        }

        public Dictionary<SorceryDef, int> vancianCasts = new Dictionary<SorceryDef, int> ();

        public int dayCount = 0;

    }
}
