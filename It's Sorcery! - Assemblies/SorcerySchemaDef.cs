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
    public class SorcerySchemaDef : Def
    {
        // don't know if we need this: most of the custom stuff arises from the trackers linked to the schema
        //public Type sorcerySchemaClass = typeof(SorcerySchema);

        //public StatDef energyStat; 

        public EnergyTrackerDef energyTrackerDef;

        public List<LearningTrackerDef> learningTrackerDefs;

        public ProgressTrackerDef progressTrackerDef;

        public List<SorcerySchemaDef> incompatibleSchemas;

        
    }
}
