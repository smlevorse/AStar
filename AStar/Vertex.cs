using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AStar
{
    public class Vertex
    {
        public int X;
        public int Y;
        public Color Color;
        public double Priority;
        public double G;
        public Vertex PathNode;

        public Vertex(int x, int y, Color color = default(Color))
        {
            X = x;
            Y = y;
            Color = color;
            G = double.MaxValue;
        }
    }
}
