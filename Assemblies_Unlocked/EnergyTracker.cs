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

        public virtual float OverBarFactor
        {
            get
            {
                return 0.5f;
            }
        }

        public virtual float OverBarLossFactor
        {
            get
            {
                return 1f;
            }
        }

        public virtual float MaxEnergyOverload
        {
            get
            {
                return MaxEnergy * (1f + OverBarFactor);
            }
        }

        public virtual int TurnTicks
        {
            get
            {
                return 60;
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


        public virtual float EnergyToRelativeValue(float energyCost = 0)
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

        public virtual void HightlightEnergyCost(Rect rec)
        {

        }

        public int findFloor(float relVal, bool neg = true)
        {
            if (neg)
            {
                if (relVal < 0) return -1;
                else if (relVal > 1) return 1;
            }
            else
            {
                if (relVal < 0) return -1;
                else if (relVal >= 1) return 1;
            }
            return 0;
        }

        public float normalizeVal(float relVal, bool neg = true)
        {
            if (neg)
            {
                if (relVal < 0) return relVal + 1;
                else if (relVal > 1) return relVal - 1;
            }
            else
            {
                if (relVal < 0) return relVal + 1;
                else if (relVal >= 1) return relVal - 1;
            }
            return relVal;
        }

        public virtual string TopRightLabel(SorceryDef sorceryDef)
        {
            return "";
        }

        public override string ToString()
        {
            return "Energy class: "+ this.GetType().Name.ToString();
        }

        public Pawn pawn;

        public EnergyTrackerDef def;

        public SorcerySchemaDef sorcerySchemaDef;

        public float currentEnergy;

        public bool limitLocked = true;
    }
}
