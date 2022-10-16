using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace ItsSorceryFramework
{
    public class EnergyTracker : IExposable
    {
        // initalizer- created via activator via SorcerySchema
        public EnergyTracker(Pawn pawn, EnergyTrackerDef def) 
        {
            this.pawn = pawn;
            this.def = def;
            this.sorcerySchemaDef = null;

            // maybe put initalize gizmo here idunno
        }

        public EnergyTracker(Pawn pawn, SorcerySchemaDef def)
        {
            this.pawn = pawn;
            this.def = def.energyTrackerDef;
            this.sorcerySchemaDef = def;

            // maybe put initalize gizmo here idunno
        }

        public virtual void ExposeData()
        {
            Scribe_Deep.Look(ref pawn, "pawn");
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look<float>(ref this.currentEnergy, "currentEnergy", 0f, false);
        }

        public virtual float MaxEnergy
        {
            get
            {
                return this.pawn.GetStatValue(def.energyMaxStatDef, true);
            }
        }

        public virtual float MinEnergy
        {
            get
            {
                return this.pawn.GetStatValue(def.energyMinStatDef, true);
            }
        }

        public virtual float EnergyRecoveryRate
        {
            get
            {
                return this.pawn.GetStatValue(def.energyRecoveryStatDef, true);
            }
        }

        public virtual float EnergyCostFactor
        {
            get
            {
                return this.pawn.GetStatValue(def.energyCostFactorStatDef, true);
            }
        }

        public virtual void EnergyTrackerTick()
        {
            
        }

        public virtual float EnergyRelativeValue
        {
            get
            {
                return this.EnergyToRelativeValue();
            }
        }

        public virtual float EnergyToRelativeValue()
        {
            return 0f;
        }

        public virtual bool WouldReachLimitEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            return false;
        }

        public virtual bool TryAlterEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            currentEnergy = Math.Max(0f, currentEnergy - energyCost);
            return true;
        }

        public virtual void EmptyEnergy()
        {
            this.currentEnergy = 0f;
        }

        public virtual void ApplyHediffSeverity(float newSev)
        {

        }

        public virtual string DisableCommandReason()
        {
            return "CommandDisableReasonBase_ISF";
        }

        public virtual void DrawOnGUI(Rect rect)
        {

        }

        public override string ToString()
        {
            return "Energy class: "+ this.GetType().Name.ToString();
        }

        public Pawn pawn;

        public EnergyTrackerDef def;

        public SorcerySchemaDef sorcerySchemaDef;

        public float currentEnergy;

        
    }
}
