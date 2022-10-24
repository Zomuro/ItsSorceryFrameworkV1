using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;
using UnityEngine;

namespace ItsSorceryFramework
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("Zomuro.ItsSorcery.Framework");

            // FloatMenu_EnergyTracker_Consumable
            // if a pawn has a SorcerySchema with a Consumable class EnergyTracker, show the float menu
            harmony.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders"), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(AddHumanlikeOrders_EnergyTracker_Consumable)));

            /*
            // SubspaceCheck
            // if a projectile is supposed to impact a pawn with a hediff, "stores it" to be fired later
            harmony.Patch(AccessTools.Method(typeof(Projectile), "ImpactSomething"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(SubspaceCheck)), null);

            // ModifiedMeleeDamageAmount // , typeof(Thing), typeof(HediffComp_VerbGiver)
            // might depend on which version of the method i patch? no idea
            harmony.Patch(AccessTools.Method(typeof(VerbProperties), "AdjustedMeleeDamageAmount",
                    new[] { typeof(Verb), typeof(Pawn) }),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ModifiedMeleeDamageAmount)));

            // ModifiedApplyMeleeDamageToTarget
            // postfix to apply dmg to multiple enemies based on caster Sword insight
            // , new[] { typeof(LocalTargetInfo) }
            harmony.Patch(AccessTools.Method(typeof(Verb_MeleeAttackDamage), "ApplyMeleeDamageToTarget"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ModifiedApplyMeleeDamage)));

            // ModifiedApplyArmor
            // transpiler to alter armor rating to be changed by armorratingfactor
            harmony.Patch(AccessTools.Method(typeof(ArmorUtility), "ApplyArmor"),
                null, null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ModifiedApplyArmor)));

            // ModifiedApplyArmor_GearTab
            // transpiler to alter armor rating in pawn's gear tab to be changed by armorratingfactor
            harmony.Patch(AccessTools.Method(typeof(ITab_Pawn_Gear), "TryDrawOverallArmor"),
                null, null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ModifiedTryDrawOverallArmor)));

            // ShieldBeltBreak_Yaoma
            // lets other damage types break shields instantly
            harmony.Patch(AccessTools.Method(typeof(ShieldBelt), "CheckPreAbsorbDamage", new[] { typeof(DamageInfo) }),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ShieldBeltBreak_Yaoma)));

            // StunApplied_Yaoma
            // lets other damage types stun
            harmony.Patch(AccessTools.Method(typeof(StunHandler), "Notify_DamageApplied", new[] { typeof(DamageInfo) }),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(StunApplied_Yaoma)));

            //TakeDamage_Nullify
            // gives a chance to avoid damage
            harmony.Patch(AccessTools.Method(typeof(Thing), "TakeDamage", new[] { typeof(DamageInfo) }),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(TakeDamage_Nullify)));

            // ArrayBarrier_PassCheck
            // prevents enemy pawns from passing array barrier
            harmony.Patch(AccessTools.Method(typeof(Building_Door), "CanPhysicallyPass", new[] { typeof(Pawn) }),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(ArrayBarrier_PassCheck)));

            // ArrayBarrier_InterceptCheck
            // blocks projectiles at array barrier if conditions are fufilled
            harmony.Patch(AccessTools.Method(typeof(Projectile), "CheckForFreeIntercept", new[] { typeof(IntVec3) }),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ArrayBarrier_InterceptCheck)));

            // FemaleBB_Monogender_Yaoma
            // if the pawn's race is Yaoma (this race!), the body type is FemaleBB
            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "GenerateBodyType"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(FemaleBB_Monogender_Yaoma)));

            //ButcherProduct_AddQiCore
            // depending on the cultivation level of the pawn, add corresponding qi core to butcher products
            harmony.Patch(AccessTools.Method(typeof(Corpse), "ButcherProducts"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ButcherProducts_AddQiCore)));

            // StorytellerTick_Daji_Yaoma
            // postfix- check if storyteller is Daji & time at midnight to fire the unique incident
            harmony.Patch(AccessTools.Method(typeof(Storyteller), "StorytellerTick"),
                null, new HarmonyMethod(typeof(HarmonyPatches), nameof(StorytellerTick_Post_Yaoma)));

            // StorytellerTick_Queued_Yaoma
            // alters the way incidents occur to use queued incidents + make a letter appear to "predict" the queued incident
            harmony.Patch(AccessTools.Method(typeof(Storyteller), "StorytellerTick"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(StorytellerTick_Pre_Yaoma)));

            // Technique_GainAbility
            // prefixes GainAbility in Pawn_AbilityTracker to resolve adding abilities via debug
            harmony.Patch(AccessTools.Method(typeof(Pawn_AbilityTracker), "GainAbility"),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(GainAbility_Yaoma)));*/
        }

        public static void AddHumanlikeOrders_EnergyTracker_Consumable(Vector3 __0, Pawn __1, List<FloatMenuOption> __2)
        {
            Comp_ItsSorcery comp = __1.TryGetComp<Comp_ItsSorcery>() as Comp_ItsSorcery;
            String text = "";
            foreach (SorcerySchema schema in from schema in comp.schemaTracker.sorcerySchemas
                                         where schema.energyTracker.GetType() == typeof(EnergyTracker_Consumable)
                                         select schema)
            {
                EnergyTracker energyTracker = schema.energyTracker;
                Dictionary<ThingDef, float> ammoRef = schema.energyTracker.def.sorceryAmmoDict;
                foreach (ThingDef thingDef in ammoRef.Keys)
                {
                    Thing ammo = __0.ToIntVec3().GetFirstThing(__1.Map, thingDef);
                    if (ammo == null)
                    {
                        continue;
                    }

                    if (!__1.CanReach(ammo, PathEndMode.ClosestTouch, Danger.Deadly, false, false, TraverseMode.ByPawn))
                    {
                        text = "ISF_Charge".Translate(schema.def.LabelCap.ToString(), ammo.def.LabelCap.ToString())
                            + "ISF_ChargeNoPath".Translate();
                        __2.Add(new FloatMenuOption(text, null, MenuOptionPriority.Default,
                            null, null, 0f, null, null, true, 0));
                    }
                    else if (energyTracker.MaxEnergy != 0 &&
                        energyTracker.currentEnergy == energyTracker.MaxEnergy)
                    {
                        text = "ISF_Charge".Translate(schema.def.LabelCap.ToString(), ammo.def.LabelCap.ToString())
                            + "ISF_ChargeFull".Translate();
                        __2.Add(new FloatMenuOption(text, null, MenuOptionPriority.Default, 
                            null, null, 0f, null, null, true, 0));
                    }
                    else
                    {
                        int count = 0;
                        int endcount = ammo.stackCount;
                        float gain = endcount * ammoRef[ammo.def];
                        if (energyTracker.MaxEnergy == 0)
                        {
                            text = "ISF_Charge".Translate(schema.def.LabelCap.ToString(), ammo.def.ToString())
                            + "ISF_ChargeCalc".Translate(ammo.stackCount, ammo.def.ToString(),
                                ammo.stackCount * ammoRef[ammo.def],
                                energyTracker.def.energyStatLabel);
                        }
                        else
                        {
                            count = (int)Math.Ceiling((energyTracker.MaxEnergy - energyTracker.currentEnergy) / ammoRef[ammo.def]);
                            endcount = Math.Min(count, ammo.stackCount);
                            gain = Math.Min(endcount * ammoRef[ammo.def], energyTracker.MaxEnergy - energyTracker.currentEnergy);
                            text = "ISF_Charge".Translate(schema.def.LabelCap.ToString(), ammo.def.ToString())
                            + "ISF_ChargeCalc".Translate(endcount, ammo.def.ToString(),
                                gain, energyTracker.def.energyStatLabel);
                        }
                            
                        Action chargeSchema = delegate ()
                        {
                            __1.jobs.TryTakeOrderedJob(JobGiver_Charge.MakeChargeEnergyJob(__1, schema, ammo, endcount), 
                                new JobTag?(JobTag.Misc), false);
                        };
                        __2.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text, chargeSchema, 
                            MenuOptionPriority.Default, null, null, 0f, null, null, true, 0), __1, ammo, "ReservedBy", null));
                    }
                }
            }

            return;
        }

    }
}
