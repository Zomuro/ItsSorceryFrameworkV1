using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ItsSorceryFramework
{
    [DefOf]
    public static class SorcerySchemaDefOf
    {
        public static SorcerySchemaDef SorcerySchema_Base;
        public static SorcerySchemaDef SorcerySchema_Base2;

        static SorcerySchemaDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SorcerySchemaDefOf));
        }
    }
}
