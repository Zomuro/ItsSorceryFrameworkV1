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
    public class EnergyTracker_VancianLifetime : EnergyTracker_Vancian
    {
        // initalizer- created via activator via SorcerySchema
        public EnergyTracker_VancianLifetime(Pawn pawn, EnergyTrackerDef def) : base(pawn, def)
        {
            currentEnergy = MaxCasts;
        }

        public EnergyTracker_VancianLifetime(Pawn pawn, SorcerySchemaDef def) : base(pawn, def)
        {
            currentEnergy = MaxCasts;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref vancianCasts, "vancianCasts", LookMode.Def, LookMode.Value);
            Scribe_Values.Look(ref tickCount, "tickCount");
        }

        /*public virtual int RefreshTick
        {
            get
            {
                return def.refreshTicks % 60000;
            }
        }*/

        public virtual int MaxCasts
        {
            get
            {
                return (int) this.pawn.GetStatValue(def.energyMaxStatDef ?? StatDefOf_ItsSorcery.MaxEnergy_ItsSorcery, true);
            }
        }

        public virtual int CastRecoveryRate
        {
            get
            {
                return (int) this.pawn.GetStatValue(def.energyRecoveryStatDef ?? StatDefOf_ItsSorcery.EnergyRecovery_ItsSorcery, true);
            }
        }

        public virtual int currentCasts
        {
            get
            {
                return (int)currentEnergy;
            }
        }

        public override void EnergyTrackerTick()
        {
            if(tickCount > 0)
            {
                tickCount--;
                if(tickCount == 0)
                {
                    tickCount = def.refreshTicks;
                    currentEnergy = Math.Max(0, Math.Min(currentCasts + CastRecoveryRate, MaxCasts));
                }
            }
        }

        public override bool WouldReachLimitEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            if (currentCasts <= 0) return true;
            return false;
        }

        public override bool TryAlterEnergy(float energyCost, SorceryDef sorceryDef = null)
        {
            if (!WouldReachLimitEnergy(energyCost, sorceryDef))
            {
                currentEnergy--;
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

            Text.Anchor = TextAnchor.LowerCenter;
            Widgets.Label(rect, def.TranslatedCastsCount.Translate(currentCasts, MaxCasts));

            Text.Anchor = TextAnchor.UpperLeft;
        }

        public override string TopRightLabel(SorceryDef sorceryDef)
        {
            return (sorceryDef?.sorcerySchema.energyTrackerDef.energyStatLabel.CapitalizeFirst()[0]) + ": " +
                vancianCasts[sorceryDef].ToString() + "/" +
                ((int) Math.Ceiling(sorceryDef.MaximumCasts * this.CastFactor)).ToString();
        }

        

    }
}
