using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ItsSorceryFramework
{
    public class EnergyTrackerDef : Def
    {
        // EnergyTracker class
        public Type energyTrackerClass = typeof(EnergyTracker);
        
        // EnergyTracker stats
        public StatDef energyMaxStatDef;

        public StatDef energyMinStatDef;

        public string energyStatLabel = "energy";

        public StatDef energyRecoveryStatDef;

        public StatDef energyCostFactorStatDef;

        public int refreshTick = 0;

        public int refreshDays = 1;

        public float overloadLimitFactor = 0f;

        public float overloadLossFactor = 1f;

        public int turnTicks = 60;

        public string TranslatedDisableReason;

        public ThingDef sorceryAmmo;

        public HediffDef sideEffect;
    }
}
