using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ItsSorceryFramework
{
    // Add the tracker/gizmo to the pawn list
    [StaticConstructorOnStartup]
    public class Comp_ItsSorcery : ThingComp
    {
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (parent is Pawn p)
			{
				pawn = p;
				CompInitalize();
			}
		}

		public virtual void CompInitalize()
		{
			if(pawn != null)
            {
				if(schemaTracker is null) schemaTracker = new Pawn_SorcerySchemaTracker(pawn);

				// temporary
				if (!schemaTracker.sorcerySchemas.Any() && pawn.IsColonist)
				{
					schemaTracker.sorcerySchemas.Add(SorcerySchemaUtility.InitializeSorcerySchema(pawn, SorcerySchemaDefOf.SorcerySchema_Base));
					schemaTracker.sorcerySchemas.Add(SorcerySchemaUtility.InitializeSorcerySchema(pawn, SorcerySchemaDefOf.SorcerySchema_Base2));
					//schemaTracker.sorcerySchemas.Add(SorcerySchemaUtility.InitializeSorcerySchema(pawn, SorcerySchemaDefOf.SorcerySchema_Base));
					//schemaTracker.sorcerySchemas.Add(SorcerySchemaUtility.InitializeSorcerySchema(pawn, SorcerySchemaDefOf.SorcerySchema_Base));
					//schemaTracker.sorcerySchemas.Add(SorcerySchemaUtility.InitializeSorcerySchema(pawn, SorcerySchemaDefOf.SorcerySchema_Base));
					//schemaTracker.sorcerySchemas.Add(SorcerySchemaUtility.InitializeSorcerySchema(pawn, SorcerySchemaDefOf.SorcerySchema_Base));
				}
				
			}
			
		}

		public override void CompTick()
		{
			if (schemaTracker != null) schemaTracker.SchemaTrackerTick();
		}

		public override void PostExposeData()
		{
			Scribe_Deep.Look(ref schemaTracker, true, "schemaTracker", this.parent as Pawn);
		}

		public Pawn pawn;

		public Pawn_SorcerySchemaTracker schemaTracker;

	}
}
