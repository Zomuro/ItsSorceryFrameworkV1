using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ItsSorceryFramework
{
    public class LearningTreeNodeDef : Def
    {
		public float ViewX
		{
			get
			{
				return this.coordX;
			}
		}

		public float ViewY
		{
			get
			{
				return this.coordY;
			}
		}

		public List<LearningTreeNodeDef> prereqs = new List<LearningTreeNodeDef>();

		public List<ResearchProjectDef> prereqsResearch = new List<ResearchProjectDef>();

		public LearningTrackerDef learningTracker;

		public float coordX = 0;

		public float coordY = 0;

		public int level;

		public bool condVisiblePrereq = true;

	}
}
