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

        public StatDef CastFactorStatDef;

        public float overBarFactor = 0f;

        public float overBarLossFactor = 1f;

        public float underBarLossFactor = 1f;

        // TurnBased EnergyTrackers
        public int turnTicks = 60;

        // Vancian EnergyTracker Stats
        public string TranslatedRefreshNotif = "EnergyTrackerVancianRefresh_ItsSorcery";

        public int refreshTicks = 60000;

        //VancianLifetime EnergyTracker string
        public string TranslatedCastsCount = "EnergyTrackerVancianCastsLeft_ItsSorcery";

        // Command disable string
        public string TranslatedDisableReason;

        //public ThingDef sorceryAmmo;

        //public float sorceryAmmoFactor = 1f;

        public Dictionary<ThingDef, float> sorceryAmmoDict;

        public HediffDef sideEffect;
    }
}
