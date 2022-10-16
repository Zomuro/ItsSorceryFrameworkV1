using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ItsSorceryFramework
{
    public class SorcerySchema
    {
        public SorcerySchema(Pawn pawn, SorcerySchemaDef def)
        {
            this.pawn = pawn;
            this.def = def;
            this.InitializeTrackers();
        }
        
        public virtual void InitializeTrackers()
        {
            this.energyTracker = Activator.CreateInstance(def.energyTrackerDef.energyTrackerClass,
                new object[] { pawn, def}) as EnergyTracker;
            Log.Message(energyTracker.def.defName);
            Log.Message(energyTracker.ToString());
        }

        public virtual void SchemaTick()
        {
            if (energyTracker != null) 
            {
                energyTracker.EnergyTrackerTick();
            }
            //Log.Message("how is it empty");
            
        }

        public Pawn pawn;

        public SorcerySchemaDef def;

        public EnergyTracker energyTracker;

        public List<LearningTracker> learningTrackers;

        public ProgressTracker progressTracker;
    }
}
