using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;

namespace ItsSorceryFramework
{
    public class LearningTracker_Tree : LearningTracker
    {
        public LearningTracker_Tree(Pawn pawn) : base(pawn)
        {

        }

        public LearningTracker_Tree(Pawn pawn, LearningTrackerDef def) : base(pawn, def)
        {

        }

        public LearningTracker_Tree(Pawn pawn, LearningTrackerDef def, SorcerySchemaDef schemaDef) : base(pawn, def, schemaDef)
        {

        }

        /*public virtual void InitializeNodes()
        {
            foreach(LearningTreeNodeDef def in DefDatabase<LearningTreeNodeDef>.AllDefs
                .Where(x => x.learningTracker == this.def))
            {
                nodes.Add(def);
            }
        }*/

        public List<LearningTreeNodeDef> allNodes
        {
            get
            {
                if(cachedAllNodes == null)
                {
                    cachedAllNodes = new List<LearningTreeNodeDef>(from def in DefDatabase<LearningTreeNodeDef>.AllDefsListForReading
                                                             where def.learningTracker == this.def
                                                             select def);

                    foreach(LearningTreeNodeDef node in cachedAllNodes)
                    {
                        if (!completion.Keys.Contains(node)) completion[node] = false;
                    }
                }

                return cachedAllNodes;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_Collections.Look(ref nodes, "nodes", LookMode.Def);
            Scribe_Collections.Look(ref completion, "completion", LookMode.Def, LookMode.Value);

        }

        public override void DrawLeftGUI(Rect rect)
        {
            if(selectedNode != null)
            {
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Medium;
                Widgets.Label(rect, selectedNode.LabelCap);

                rect.yMin += 32f;
                Rect desc = new Rect(rect);
                desc.yMax = rect.yMax - 50f;
                Text.Font = GameFont.Small;
                Widgets.Label(desc, selectedNode.description);

                Text.Anchor = TextAnchor.MiddleCenter;
                if (!completion[selectedNode] && Widgets.ButtonText(new Rect(rect.x, rect.yMax-50f, 140, 50), "complete"))
                {
                    completion[selectedNode] = true;
                }
                Text.Anchor = TextAnchor.UpperLeft;
            }
        }

        public override void DrawRightGUI(Rect rect)
        {
            rect.yMin += 32f;

            Rect outRect = rect.ContractedBy(10f);
            Rect viewRect = outRect.ContractedBy(10f);
            Rect groupRect = viewRect.ContractedBy(10f);


            //Widgets.ScrollHorizontal(outRect, ref this.rightScrollPosition, viewRect, 20f);
			//Widgets.BeginScrollView(outRect, ref this.rightScrollPosition, viewRect, true);
			Widgets.BeginGroup(groupRect);
            
            foreach (LearningTreeNodeDef ltnDef in allNodes)
            {
                Rect nodeRect = getNodeRect(ltnDef);

                /*Color BG = selectionBGColor(ltnDef);
                Color border = selectionBorderColor(ltnDef);*/

                if (Widgets.CustomButtonText(ref nodeRect, "", selectionBGColor(ltnDef),
                    new Color(0.8f, 0.85f, 1f), selectionBorderColor(ltnDef), false, 1, true, true))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera(null);
                    this.selectedNode = ltnDef;
                }
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(nodeRect, ltnDef.LabelCap);
                Text.Anchor = TextAnchor.UpperLeft;

                foreach (LearningTreeNodeDef prereq in ltnDef.prereqs)
                {
                    Tuple<Vector2, Vector2> points = lineEnds(prereq, ltnDef, nodeRect);
                    Widgets.DrawLine(points.Item1, points.Item2, selectionLineColor(ltnDef), 2f);
                }

            }

			Widgets.EndGroup();
			//Widgets.EndScrollView();
			//this.scrollPositioner.ScrollHorizontally(ref this.rightScrollPosition, outRect.size);

		}

        private float CoordToPixelsX(float x)
        {
            return x * 190f;
        }

        private float CoordToPixelsY(float y)
        {
            return y * 100f;
        }

        private Rect getNodeRect(LearningTreeNodeDef nodeDef)
        {
            return new Rect(CoordToPixelsX(nodeDef.coordX), CoordToPixelsY(nodeDef.coordY), 140f, 50f);
        }

        private Tuple<Vector2, Vector2> lineEnds(LearningTreeNodeDef start, LearningTreeNodeDef end, Rect nodeRef)
        {
            Vector2 prereq = new Vector2(CoordToPixelsX(start.coordX), CoordToPixelsY(start.coordY));
            prereq.x += nodeRef.width;
            prereq.y += nodeRef.height/2;
            Vector2 current = new Vector2(CoordToPixelsX(end.coordX), CoordToPixelsY(end.coordY));
            current.y += nodeRef.height/2;

            return new Tuple<Vector2, Vector2>(prereq, current);
        }

        private Color selectionBGColor(LearningTreeNodeDef node)
        {
            Color baseCol = TexUI.AvailResearchColor;

            if (completion[node]) baseCol = TexUI.FinishedResearchColor;

            // if the node is the selected one, change background to highlight
            if (selectedNode != null && selectedNode == node) return baseCol + TexUI.HighlightBgResearchColor;

            return baseCol;
        }

        private Color selectionBorderColor(LearningTreeNodeDef node)
        {
            // if the node is the selected one OR a prerequisite, change border to highlight
            if (selectedNode != null)
            {
                if (selectedNode == node) return TexUI.HighlightBorderResearchColor;

                else if (selectedNode.prereqs.NotNullAndContains(node))
                {
                    if(!completion[node]) return TexUI.DependencyOutlineResearchColor;
                    else return TexUI.HighlightLineResearchColor;
                }

            }

            return TexUI.DefaultBorderResearchColor;
        }

        private Color selectionLineColor(LearningTreeNodeDef node)
        {
            if (selectedNode == node) return TexUI.HighlightLineResearchColor;

            return TexUI.DefaultLineResearchColor;
        }

        public List<LearningTreeNodeDef> cachedAllNodes;

        public LearningTreeNodeDef selectedNode;

        //public Color colorSelected = TexUI.DefaultBorderResearchColor;

        public Dictionary<LearningTreeNodeDef, bool> completion = new Dictionary<LearningTreeNodeDef, bool>();

        private ScrollPositioner scrollPositioner = new ScrollPositioner();

        private Vector2 rightScrollPosition = Vector2.zero;
    }
}
