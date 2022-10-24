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
    public class EnergyTracker_Inverted : EnergyTracker
    {
        // initalizer- created via activator via SorcerySchema
        public EnergyTracker_Inverted(Pawn pawn, EnergyTrackerDef def) : base(pawn, def)
        {
        }

        public EnergyTracker_Inverted(Pawn pawn, SorcerySchemaDef def) : base(pawn, def)
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
                return this.pawn.GetStatValue(def.energyCostFactorStatDef ?? StatDefOf_ItsSorcery.EnergyCostFactor_ItsSorcery, true);
            }
        }

        public override float OverBarFactor
        {
            get
            {
                return Math.Min(Math.Max(def.overBarFactor, 0f), 1f);
            }
        }

        public override float OverBarLossFactor
        {
            get
            {
                return Math.Max(def.overBarLossFactor, 0.1f);
            }
        }

        public override float MaxEnergyOverload
        {
            get
            {
                return MaxEnergy * (1f + OverBarFactor);
            }
        }

        public override float EnergyToRelativeValue(float energyCost = 0)
        {
            float tempCurrentEnergy = currentEnergy + energyCost;

            /*if (tempCurrentEnergy < 0)
            {
                return tempCurrentEnergy / Math.Abs(MinEnergy);
            }
            
            if(tempCurrentEnergy <= MinEnergy)
            {
                return MinEnergy / MaxEnergy;
            }
            if (tempCurrentEnergy >= MaxEnergyOverload)
            {
                return MaxEnergyOverload / MaxEnergy;
            }

            return tempCurrentEnergy / MaxEnergy;*/

            if (tempCurrentEnergy <= MinEnergy)
            {
                return MinEnergy / MaxEnergy;
            }
            if (tempCurrentEnergy >= MaxEnergyOverload)
            {
                return MaxEnergyOverload / MaxEnergy;
            }
            return tempCurrentEnergy / MaxEnergy;
        }

        public override void EnergyTrackerTick()
        {
            if (currentEnergy < 0) // when energy below 0
            {
                float tempEnergy = Math.Min(currentEnergy + 1.TicksToSeconds() * EnergyRecoveryRate,
                    0);
                this.currentEnergy = Math.Max(tempEnergy, MinEnergy);
            }
            else if (currentEnergy <= MaxEnergy) // when energy is under or equal the normal max
            {
                float tempEnergy = Math.Min(currentEnergy - 1.TicksToSeconds() * EnergyRecoveryRate, MaxEnergy);
                this.currentEnergy = Math.Max(tempEnergy, 0);
            }
            else // when energy is over the normal max
            {
                float tempEnergy = Math.Min(currentEnergy - 1.TicksToSeconds() * EnergyRecoveryRate * OverBarLossFactor, 
                    MaxEnergyOverload);
                this.currentEnergy = Math.Max(tempEnergy, MinEnergy);
            }
        }

        public override bool WouldReachLimitEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            if (currentEnergy + energyCost > MaxEnergyOverload && limitLocked) return true;
            return false;
        }

        public override bool TryAlterEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            if (!WouldReachLimitEnergy(energyCost))
            {
                currentEnergy = Math.Min(Math.Max(MinEnergy, currentEnergy + energyCost), MaxEnergyOverload);
                this.ApplyHediffSeverity(this.EnergyToRelativeValue());
                return true;
            }
            
            return false;
        }

        public override void ApplyHediffSeverity(float relVal)
        {
            if (MaxEnergyOverload == MaxEnergy) return;
            float relValNorm = relVal - 1f;
            float maxRelVal = MaxEnergyOverload / MaxEnergy - 1f;

            HediffDef hediffDef = this.def.sideEffect;
            if (hediffDef == null) return;
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
            if (relVal > 1f)
            {
                //Log.Message("test this thing god why aren't you working");
                if (hediff == null) HealthUtility.AdjustSeverity(this.pawn, hediffDef, relValNorm / maxRelVal);
                else if (relValNorm / maxRelVal > hediff.Severity) hediff.Severity = relValNorm / maxRelVal;
            }

            if (hediff != null && hediff.Severity >= hediff.def.maxSeverity)
            {
                Messages.Message("MessagePastLimit".Translate(this.pawn.Named("PAWN")),
                    this.pawn, MessageTypeDefOf.NegativeEvent, true);
            }

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
                    GizmoTextureUtility.EmptyBarTex, GizmoTextureUtility.OverBarTex, true);
            }
            else if(this.EnergyRelativeValue <= 1)
            {
                Widgets.FillableBar(barBox, Mathf.Min(this.EnergyRelativeValue, 1f), GizmoTextureUtility.BarTex,
                    GizmoTextureUtility.EmptyBarTex, true);
            }
            else 
            {
                /*Widgets.FillableBar(barBox, Mathf.Min((this.EnergyRelativeValue - 1f) / (MaxEnergyOverload / MaxEnergy - 1), 1f), 
                    GizmoTextureUtility.UnderBarTex,
                    GizmoTextureUtility.BarTex, true);*/
                Widgets.FillableBar(barBox, Mathf.Min((this.EnergyRelativeValue - 1), 1f),
                    GizmoTextureUtility.OverBarTex,
                    GizmoTextureUtility.BarTex, true);
            }

            string energyLabel = this.currentEnergy.ToString("F0") + " / " +this.MaxEnergy.ToString("F0");
            Widgets.Label(barBox, energyLabel);

            Widgets.DrawBoxSolidWithOutline(rect, Color.clear, Color.grey);
            Text.Anchor = TextAnchor.UpperLeft;

            HightlightEnergyCost(barBox);
        }

        public override void HightlightEnergyCost(Rect rect)
        {
            MainTabWindow_Inspect mainTabWindow_Inspect = (MainTabWindow_Inspect)MainButtonDefOf.Inspect.TabWindow;
            Command_Sorcery command_Sorcery = ((mainTabWindow_Inspect != null) ? mainTabWindow_Inspect.LastMouseoverGizmo : null) as Command_Sorcery;
            SorceryDef sorceryDef = (command_Sorcery?.Ability as Sorcery)?.sorceryDef;

            if (sorceryDef == null || sorceryDef.sorcerySchema.energyTrackerDef != this.def) return;
            //Log.Message("test");

            Rect highlight = rect.ContractedBy(3f);
            float max = highlight.xMax;
            float min = highlight.xMin;
            float width = highlight.width;

            float relativeEnergyDiff = this.EnergyToRelativeValue(sorceryDef.EnergyCost * EnergyCostFactor);
            float relativeEnergy = this.EnergyRelativeValue;

            // used to make random blinking effect on highlight
            float num = Mathf.Repeat(Time.time, 0.85f);
            float num2 = 1f;
            if (num < 0.1f)
            {
                num2 = num / 0.1f;
            }
            else if (num >= 0.25f)
            {
                num2 = 1f - (num - 0.25f) / 0.6f;
            }

            if (sorceryDef.EnergyCost * EnergyCostFactor < 0f)
            {
                if (findFloor(relativeEnergy) != findFloor(relativeEnergyDiff)) // if current relative qi > qi cost of ability
                {
                    highlight.xMin = Widgets.AdjustCoordToUIScalingFloor(min);
                    highlight.xMax = Widgets.AdjustCoordToUIScalingFloor(min + normalizeVal(relativeEnergy) * width);
                }
                else // if current relative qi < qi cost of ability
                {
                    highlight.xMin = Widgets.AdjustCoordToUIScalingFloor(min + normalizeVal(relativeEnergyDiff) * width);
                    highlight.xMax = Widgets.AdjustCoordToUIScalingFloor(min + normalizeVal(relativeEnergy) * width);
                }
                //rect6.xMin = Widgets.AdjustCoordToUIScalingFloor(rect6.xMin + num3 * width);
                //rect6.width = Widgets.AdjustCoordToUIScalingFloor(Mathf.Max(Mathf.Min(num4, 1f) - num3, 0f) * width);
                GUI.color = new Color(1f, 1f, 1f, num2 * 0.7f);
                GenUI.DrawTextureWithMaterial(highlight, GizmoTextureUtility.UnderBarTex, null, default(Rect));
                GUI.color = Color.white;
            }

            else
            {
                if (findFloor(relativeEnergy, false) != findFloor(relativeEnergyDiff, false))
                {
                    highlight.xMin = Widgets.AdjustCoordToUIScalingFloor(min + normalizeVal(relativeEnergy, false) * width);
                    highlight.xMax = Widgets.AdjustCoordToUIScalingFloor(max);
                }
                else // if current relative qi < qi cost of ability
                {
                    highlight.xMin = Widgets.AdjustCoordToUIScalingFloor(min + normalizeVal(relativeEnergy, false) * width);
                    highlight.xMax = Widgets.AdjustCoordToUIScalingFloor(Math.Min(min + normalizeVal(relativeEnergyDiff, false) * width, max));
                }
                //rect6.xMin = Widgets.AdjustCoordToUIScalingFloor(rect6.xMin + num3 * width);
                //rect6.width = Widgets.AdjustCoordToUIScalingFloor(Mathf.Max(Mathf.Min(num4, 1f) - num3, 0f) * width);
                GUI.color = new Color(1f, 1f, 1f, num2 * 0.7f);
                GenUI.DrawTextureWithMaterial(highlight, GizmoTextureUtility.OverBarTex, null, default(Rect));
                GUI.color = Color.white;
            }

        }

        public override string TopRightLabel(SorceryDef sorceryDef)
        {
            return (sorceryDef?.sorcerySchema.energyTrackerDef.energyStatLabel.CapitalizeFirst()[0]) + ": " +
                    Math.Round(sorceryDef.EnergyCost * this.EnergyCostFactor, 2).ToString();
        }

        public override string DisableCommandReason()
        {
            return def.TranslatedDisableReason ?? "CommandDisableReasonBase_ISF";
        }


    }
}
