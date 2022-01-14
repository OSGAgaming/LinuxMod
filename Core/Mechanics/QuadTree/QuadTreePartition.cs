using LinuxMod.Core.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace LinuxMod.Core.Mechanics
{
    public class QuadTreePartition
    {
        public HashSet<IQuadAgent> Agents = new HashSet<IQuadAgent>();
        public bool HasChildren = false;
        public Rectangle Space;
        public int TreeDepth;
        public Dictionary<Point, QuadTreePartition> Children = null;

        public void Refresh()
        {
            Children = null;
            HasChildren = false;
        }

        public void AddAgent(IQuadAgent agent)
        {
            Agents.Add(agent);
            if (Agents.Count > 1 && TreeDepth <= QuadTree.RecurseLimit) Split();
        }

        public void Split()
        {
            if (TreeDepth > QuadTree.RecurseLimit) return;

            HasChildren = true;
            Point HL = new Point(Space.Width / 2, Space.Height / 2);

            Children = new Dictionary<Point, QuadTreePartition>();

            Children.Add(new Point(0, 0), new QuadTreePartition(new Rectangle(Space.X, Space.Y, HL.X, HL.Y)));
            Children.Add(new Point(0, 1), new QuadTreePartition(new Rectangle(Space.X, Space.Y + HL.X, HL.X, HL.Y)));
            Children.Add(new Point(1, 0), new QuadTreePartition(new Rectangle(Space.X + HL.X, Space.Y, HL.X, HL.Y)));
            Children.Add(new Point(1, 1), new QuadTreePartition(new Rectangle(Space.X + HL.X, Space.Y + HL.X, HL.X, HL.Y)));

            foreach(KeyValuePair<Point, QuadTreePartition> children in Children)
            {
                children.Value.TreeDepth = TreeDepth + 1;
            }

            Agents.Clear();

            foreach (IQuadAgent agent in Agents)
            {
                Vector2 relativeSpace = agent.Position - Space.Location.ToVector2();
                Point p = new Point((int)(relativeSpace.X / HL.X) * HL.X, (int)(relativeSpace.Y / HL.Y) * HL.Y);

                Children[p].AddAgent(agent);
            }
        }

        public QuadTreePartition SearchPartition(Vector2 position)
        {
            if (!Space.Contains(position.ToPoint())) return null;

            if (!HasChildren) return this;
            else
            {
                Vector2 relativePosition = position - Space.Location.ToVector2();
                int HD = Space.Width / 2;
                Point Cell = new Point((int)(relativePosition.X / HD) * HD, (int)(relativePosition.Y / HD) * HD);

                return Children[Cell];
            }
        }

        public QuadTreePartition(Rectangle Space)
        {
            this.Space = Space;
        }
    }
}