using System;
using System.Collections.Generic;
using System.Windows;

namespace PathFinder.Models
{
    /// <summary>
    /// Represents pair of int values with no order. For example Pair(1,2) equals Pair(2,1).
    /// </summary>
    public class Pair
    {
        public int A;
        public int B;

        public Pair(int a, int b)
        {
            A = a;
            B = b;
        }
        public override string ToString() => $"{{{A}, {B}}}";
        public override bool Equals(object obj)
        {
            return obj is Pair p && ((A == p.A && B == p.B) || (B == p.A && A == p.B));
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Math.Min(A, B), Math.Max(A, B));
        }
    }


    public class GraphBuilder
    {
        public enum GridTypes { Rectangle, Triangle, Romb, Disconnected, Empty }

        public GridTypes Type;
        public Rect Area;
        public int Desnity;
        public int FirstID;

        public GraphBuilder(GridTypes type, Rect area, int desnity, int first_id = 0)
        {
            Type = type;
            Area = area;
            Desnity = desnity;
            FirstID = first_id;
        }

        public void Build(out Graph graph, out Dictionary<int, Vector> points)
        {
            switch (Type)
            {
                case GridTypes.Rectangle:
                    RectangleGrid(Area, out graph, out points, Desnity);
                    break;
                case GridTypes.Triangle:
                    TriangleGrid(Area, out graph, out points, Desnity);
                    break;
                case GridTypes.Romb:
                    RombGrid(Area, out graph, out points, Desnity);
                    break;
                case GridTypes.Disconnected:
                    DisconnectedGrid(Area, out graph, out points, Desnity);
                    break;
                default:
                    graph = new();
                    points = new();
                    break;
            }
        }

        public static Dictionary<int, Vector> PlacePoints(Rect area, int density, int first_id = 0)
        {
            if (density <= 0)
                throw new ArgumentException("Density must be greater then 0");
            Dictionary<int, Vector> points = new();
            int key = first_id;
            for (double y = area.Y; y < area.Y + area.Height; y += area.Height / density)
            {
                for (double x = area.X; x <= area.X + area.Width - area.Width / density; x += area.Width / density)
                {
                    points.Add(key, new(x, y));
                    key++;
                }
            }
            return points;
        }

        public static void DisconnectedGrid(Rect area, out Graph graph, out Dictionary<int, Vector> points, int density = 10, int first_id = 0)
        {
            if (density <= 0)
                throw new ArgumentException("Density must be greater then 0");
            graph = new();
            points = new();
            int key = first_id;

            for (double y = area.Y; y < area.Y + area.Height; y += area.Height / density)
            {
                for (double x = area.X; x <= area.X + area.Width - area.Width / density; x += area.Width / density)
                {
                    points.Add(key, new(x, y));
                    graph.AddNode(key);
                    key++;
                }
            }
        }

        public static void RectangleGrid(Rect area, out Graph graph, out Dictionary<int, Vector> points, int density = 10, int first_id = 0)
        {
            DisconnectedGrid(area, out graph, out points, density);
            int key = graph.NodesCount;

            int i = first_id;
            for (; i < key - density; i++)
            {
                if((i + 1) % density != 0)
                    graph.Connect(i, i + 1, (int)(points[i] - points[i + 1]).Length);
                graph.Connect(i, i + density, (int)(points[i] - points[i + density]).Length);
            }
            for(; i < key - 1; i++)
                graph.Connect(i, i + 1, (int)(points[i] - points[i + 1]).Length);
        }

        public static void TriangleGrid(Rect area, out Graph graph, out Dictionary<int, Vector> points, int density = 10, int first_id = 0)
        {
            DisconnectedGrid(area, out graph, out points, density);
            int key = graph.NodesCount;

            int i = first_id;
            for (; i < key - density; i++)
            {
                if ((i + 1) % density != 0)
                {
                    graph.Connect(i, i + 1, (int)(points[i] - points[i + 1]).Length);
                    graph.Connect(i, i + 1 + density, (int)(points[i] - points[i + 1 + density]).Length);
                }
                if (i % density != 0)
                {
                    graph.Connect(i, i - 1 + density, (int)(points[i] - points[i - 1 + density]).Length);
                }

                graph.Connect(i, i + density, (int)(points[i] - points[i + density]).Length);
            }
            for (; i < key - 1; i++)
                graph.Connect(i, i + 1, (int)(points[i] - points[i + 1]).Length);
        }

        public static void RombGrid(Rect area, out Graph graph, out Dictionary<int, Vector> points, int density = 10, int first_id = 0)
        {
            DisconnectedGrid(area, out graph, out points, density);
            int key = graph.NodesCount;

            int i = first_id;
            for (; i < key - density; i++)
            {
                if ((i + 1) % density != 0)
                {
                    graph.Connect(i, i + 1, (int)(points[i] - points[i + 1]).Length);
                    graph.Connect(i, i + 1 + density, (int)(points[i] - points[i + 1 + density]).Length);
                }

                graph.Connect(i, i + density, (int)(points[i] - points[i + density]).Length);
            }
            for (; i < key - 1; i++)
                graph.Connect(i, i + 1, (int)(points[i] - points[i + 1]).Length);
        }
    }
}