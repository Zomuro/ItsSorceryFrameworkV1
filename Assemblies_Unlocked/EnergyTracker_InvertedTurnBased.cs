﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace ItsSorceryFramework
{
    public class EnergyTracker_InvertedTurnBased : EnergyTracker_RPG
    {
        // initalizer- created via activator via SorcerySchema
        public EnergyTracker_InvertedTurnBased(Pawn pawn, EnergyTrackerDef def) : base(pawn, def)
        {
        }

        public EnergyTracker_InvertedTurnBased(Pawn pawn, SorcerySchemaDef def) : base(pawn, def)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override int TurnTicks
        {
            get
            {
                return Math.Max(def.turnTicks, 1);
            }
        }

        public override void EnergyTrackerTick()
        {
            if (Find.TickManager.TicksGame % TurnTicks == 0)
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

                if (Find.Selector.FirstSelectedObject == pawn && pawn.Drafted) Find.TickManager.Pause();
                
            }

            countdownTick = Find.TickManager.TicksGame % TurnTicks;
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
            Rect labelBox = new Rect(rect);
            labelBox.width = rect.width / 2;
            labelBox.y = rect.y + rect.height / 2;
            labelBox.height = 22;
            Rect barBox = new Rect(labelBox);
            barBox.x = rect.width * 2 / 5 + rect.x;
            barBox.y = labelBox.y;
            barBox.height = 22;

            Widgets.Label(labelBox, sorcerySchemaDef.energyTrackerDef.energyStatLabel.CapitalizeFirst());

            if (this.EnergyRelativeValue < 0)
            {
                Widgets.FillableBar(barBox, Mathf.Min(this.EnergyRelativeValue + 1, 1f),
                    GizmoTextureUtility.EmptyBarTex, GizmoTextureUtility.OverBarTex, true);
            }
            else if (this.EnergyRelativeValue <= 1)
            {
                Widgets.FillableBar(barBox, Mathf.Min(this.EnergyRelativeValue, 1f), GizmoTextureUtility.BarTex,
                    GizmoTextureUtility.EmptyBarTex, true);
            }
            else
            {
                Widgets.FillableBar(barBox, Mathf.Min((this.EnergyRelativeValue - 1f) / (MaxEnergyOverload / MaxEnergy - 1), 1f),
                    GizmoTextureUtility.UnderBarTex,
                    GizmoTextureUtility.BarTex, true);
            }

            string energyLabel = this.currentEnergy.ToString("F0") + " / " + this.MaxEnergy.ToString("F0");
            string countdown = " ("+ (TurnTicks - countdownTick).ToStringSecondsFromTicks()+")";
            Widgets.Label(barBox, energyLabel + countdown);

            Widgets.DrawBoxSolidWithOutline(rect, Color.clear, Color.grey);
            Text.Anchor = TextAnchor.UpperLeft;

            HightlightEnergyCost(barBox);
        }


        public int countdownTick = 0;

    }
}
