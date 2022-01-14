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
    public class QuadTree
    {
        public static readonly int RecurseLimit = 4;

        QuadTreePartition RootCell;
        public void AddAgent(IQuadAgent agent)
        {
            Vector2 position = agent.Position;
            QuadTreePartition Child = SearchPartitionRecursive(position);
            Child.AddAgent(agent);
        }

        public QuadTreePartition SearchPartition(Vector2 position)
        {
            if (!RootCell.Space.Contains(position.ToPoint())) return null;

            if (!RootCell.HasChildren) return RootCell;
            else
            {
                Vector2 relativePosition = position - RootCell.Space.Location.ToVector2();
                int HD = RootCell.Space.Width / 2;
                Point Cell = new Point((int)(relativePosition.X / HD) * HD, (int)(relativePosition.Y / HD) * HD);

                return RootCell.Children[Cell];
            }
        }

        public QuadTreePartition SearchPartitionRecursive(Vector2 position)
        {
            QuadTreePartition Child = SearchPartition(position);
            if (!Child.HasChildren) return Child;
            else return Child.SearchPartition(position);
        }


        public QuadTree(Rectangle Bounds)
        {
            RootCell = new QuadTreePartition(Bounds);
        }
    }
}