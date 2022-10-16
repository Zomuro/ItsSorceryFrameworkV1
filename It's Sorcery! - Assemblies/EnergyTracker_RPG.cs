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
    public class EnergyTracker_RPG : EnergyTracker
    {
        // initalizer- created via activator via SorcerySchema
        public EnergyTracker_RPG(Pawn pawn, EnergyTrackerDef def) : base(pawn, def)
        {
        }

        public EnergyTracker_RPG(Pawn pawn, SorcerySchemaDef def) : base(pawn, def)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override float MaxEnergy
        {
            get
            {
                return this.pawn.GetStatValue(def.energyMaxStatDef ?? StatDefOf_ItsSorcery.MaxEnergy_ItsSorcery, true);
            }
        }

        public override float MinEnergy
        {
            get
            {
                return this.pawn.GetStatValue(def.energyMinStatDef ?? StatDefOf_ItsSorcery.MinEnergy_ItsSorcery, true);
            }
        }

        public override float EnergyRecoveryRate
        {
            get
            {
                return this.pawn.GetStatValue(def.energyRecoveryStatDef ?? StatDefOf_ItsSorcery.EnergyRecovery_ItsSorcery, true);
            }
        }

        public override float EnergyCostFactor
        {
            get
            {
                return this.pawn.GetStatValue(def.energyCostFactorStatDef ?? StatDefOf_ItsSorcery.EnergyRecovery_ItsSorcery, true);
            }
        }

        public float OverloadLimitFactor
        {
            get
            {
                return Math.Min(Math.Max(def.overloadLimitFactor, 0f), 1f);
            }
        }

        public float OverloadLossFactor
        {
            get
            {
                return Math.Max(def.overloadLossFactor, 0.1f);
            }
        }

        public float MaxEnergyOverload
        {
            get
            {
                return MaxEnergy * (1f + OverloadLimitFactor);
            }
        }

        public override float EnergyToRelativeValue()
        {
            if(currentEnergy < 0)
            {
                return currentEnergy / Math.Abs(MinEnergy);
            }
            
            if(currentEnergy <= MinEnergy)
            {
                return MinEnergy / MaxEnergy;
            }
            if (currentEnergy >= MaxEnergyOverload)
            {
                return MaxEnergyOverload / MaxEnergy;
            }

            return currentEnergy / MaxEnergy;
        }

        public override void EnergyTrackerTick()
        {

            if(currentEnergy <= MaxEnergy) // when energy is under or equal the normal max
            {
                float tempEnergy = Math.Min(currentEnergy + 1.TicksToSeconds() * EnergyRecoveryRate, MaxEnergy);
                this.currentEnergy = Math.Max(tempEnergy, MinEnergy);
            }
            else // when energy is over the normal max
            {
                float tempEnergy = Math.Min(currentEnergy - 1.TicksToSeconds() * EnergyRecoveryRate * OverloadLossFactor, 
                    MaxEnergyOverload);
                this.currentEnergy = Math.Max(tempEnergy, MinEnergy);
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
                currentEnergy = Math.Min(Math.Max(MinEnergy, currentEnergy - energyCost), MaxEnergyOverload);
                return true;
            }
            
            return false;
        }

        public override void DrawOnGUI(Rect rect)
        {
            if(Widgets.ButtonTextSubtle(rect, ""))
            {
                Find.WindowStack.Add(new Dialog_MessageBox("magic", null, null, null, null, null, false, null, null, WindowLayer.Dialog));
            }

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rect, sorcerySchemaDef.LabelCap.ToString());

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect labelBox = new Rect(rect);
            labelBox.width = rect.width / 2;
            labelBox.y = rect.y + rect.height/2;
            labelBox.height = 22;
            Rect barBox = new Rect(labelBox);
            barBox.x = rect.width * 2/5 + rect.x;
            barBox.y = labelBox.y;
            barBox.height = 22;

            Widgets.Label(labelBox, sorcerySchemaDef.energyTrackerDef.energyStatLabel.CapitalizeFirst());

            if(this.EnergyRelativeValue < 0)
            {
                Widgets.FillableBar(barBox, Mathf.Min(this.EnergyRelativeValue + 1, 1f), 
                    GizmoTextureUtility.EmptyBarTex, GizmoTextureUtility.UnderBarTex, true);
            }
            else if(this.EnergyRelativeValue <= 1)
            {
                Widgets.FillableBar(barBox, Mathf.Min(this.EnergyRelativeValue, 1f), GizmoTextureUtility.BarTex,
                    GizmoTextureUtility.EmptyBarTex, true);
            }
            else 
            {
                Widgets.FillableBar(barBox, Mathf.Min(this.EnergyRelativeValue -1f, 1f), GizmoTextureUtility.OverBarTex,
                    GizmoTextureUtility.BarTex, true);
            }

            string energyLabel = this.currentEnergy.ToString("F0") + " / " +this.MaxEnergy.ToString("F0");
            Widgets.Label(barBox, energyLabel);

            Widgets.DrawBoxSolidWithOutline(rect, Color.clear, Color.grey);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public override string DisableCommandReason()
        {
            return def.TranslatedDisableReason ?? "CommandDisableReasonBase_ISF";
        }


    }
}
