using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace ItsSorceryFramework
{
    public class EnergyTracker_Consumable : EnergyTracker_RPG
    {
        // initalizer- created via activator via SorcerySchema
        public EnergyTracker_Consumable(Pawn pawn, EnergyTrackerDef def) : base(pawn, def)
        {
        }

        public EnergyTracker_Consumable(Pawn pawn, SorcerySchemaDef def) : base(pawn, def)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        /*public virtual int MaxCapacity
        {
            get
            {
                return (int) this.pawn.GetStatValue(def.energyMaxStatDef ?? StatDefOf_ItsSorcery.MaxEnergy_ItsSorcery, true);
            }
        }

        public virtual int CurrentCapacity
        {
            get
            {
                return (int)currentEnergy;
            }
        }*/

        public override void EnergyTrackerTick()
        {
            float tempEnergy = Math.Min(currentEnergy - 1.TicksToSeconds() * EnergyRecoveryRate / def.refreshTicks, MaxEnergy);
            this.currentEnergy = Math.Max(tempEnergy, 0);
        }

        public override bool TryAlterEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            if (!WouldReachLimitEnergy(energyCost, sorceryDef))
            {
                currentEnergy = Math.Max(0, currentEnergy - energyCost);
                return true;
            }
            
            return false;
        }

        public override void DrawOnGUI(Rect rect)
        {
            if (MaxEnergy > 0) base.DrawOnGUI(rect);
            else
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
                Widgets.Label(rect, sorcerySchemaDef.energyTrackerDef.energyStatLabel.CapitalizeFirst() + ": " +
                    currentEnergy);
            }
        }

        

    }
}
