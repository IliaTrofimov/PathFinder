using System;
using System.Collections.Generic;
using System.Windows;

namespace PathFinder.Models
{
    public class PathFinder
    {
        Graph graph;
        List<NodeInfo> infos;

        public PathFinder(Graph graph)
        {
            this.graph = graph;
        }

        private void InitInfo()
        {
            infos = new List<NodeInfo>();
            foreach (var v in graph.Nodes)
            {
                infos.Add(new NodeInfo(v.Value));
            }
        }
        private NodeInfo GetVertexInfo(Node v)
        {
            foreach (var i in infos)
                if (i.Node.Equals(v)) return i;
            return null;
        }

        private NodeInfo FindUnvisitedVertexWithMinSum()
        {
            var minValue = int.MaxValue;
            NodeInfo minVertexInfo = null;
            foreach (var i in infos)
            {
                if (!i.IsMarked && i.TotalWeight < minValue)
                {
                    minVertexInfo = i;
                    minValue = i.TotalWeight;
                }
            }

            return minVertexInfo;
        }

        public List<int> FindShortestPath(int id_1, int id_2)
        {
            return FindShortestPath(graph.GetNode(id_1), graph.GetNode(id_2));
        }


        private List<int> FindShortestPath(Node startVertex, Node finishVertex)
        {
            InitInfo();
            var first = GetVertexInfo(startVertex);
            first.TotalWeight = 0;
            while (true)
            {
                var current = FindUnvisitedVertexWithMinSum();
                if (current == null)
                    break;

                SetSumToNextVertex(current);
            }

            return GetPath(startVertex, finishVertex);
        }
        private void SetSumToNextVertex(NodeInfo info)
        {
            info.IsMarked = true;
            foreach (var l in info.Node.Links)
            {
                var nextInfo = GetVertexInfo(l.Key);
                var sum = info.TotalWeight + l.Value;
                if (sum < nextInfo.TotalWeight)
                {
                    nextInfo.TotalWeight = sum;
                    nextInfo.Previous = info.Node;
                }
            }
        }

        private List<int> GetPath(Node startVertex, Node endVertex)
        {
            List<int> path = new() { endVertex.Id };
            while (startVertex != endVertex)
            {
                endVertex = GetVertexInfo(endVertex).Previous;
                path.Add(endVertex.Id);
            }
            return path;
        }
    }

    public class NodeInfo
    {
        public Node Node { get; set; }
        public Node Previous { get; set; }
        public bool IsMarked { get; set; }
        public int TotalWeight { get; set; }

        public NodeInfo(Node node)
        {
            Node = node;
            Previous = null;
            IsMarked = false;
            TotalWeight = int.MaxValue;
        }
    }

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
        public override string ToString() => $"{A}, {B}";
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
        public enum GridTypes { Rectangle, Triangle, Romb, Empty }

        public GridTypes Type;
        public Rect Area;
        public int Desnity;

        public GraphBuilder(GridTypes type, Rect area, int desnity)
        {
            Type = type;
            Area = area;
            Desnity = desnity;
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
                default:
                    graph = new();
                    points = new();
                    break;
            }
        }

        public static void RectangleGrid(Rect area, out Graph graph, out Dictionary<int, Vector> points, int density = 10)
        {
            if (density <= 0)
                throw new ArgumentException("Density must be greater then 0");
            graph = new();
            points = new();
            int key = 0;

            for (double y = area.Y; y < area.Y + area.Height; y += area.Height / density)
            {
                for (double x = area.X; x < area.X+ area.Width; x += area.Width / density)
                {
                    points.Add(key, new(x, y));
                    graph.AddNode(key);
                    key++;
                }
            }

            int i = 0;
            for (; i < key - density; i++)
            {
                if((i + 1) % density != 0)
                    graph.Connect(i, i + 1, (int)(points[i] - points[i + 1]).Length);
                graph.Connect(i, i + density, (int)(points[i] - points[i + density]).Length);
            }
            for(; i < key - 1; i++)
                graph.Connect(i, i + 1, (int)(points[i] - points[i + 1]).Length);
        }

        public static void TriangleGrid(Rect area, out Graph graph, out Dictionary<int, Vector> points, int density = 10)
        {
            if (density <= 0)
                throw new ArgumentException("Density must be greater then 0");
            graph = new();
            points = new();
            int key = 0;

            for (double y = area.Y; y < area.Y + area.Height; y += area.Height / density)
            {
                for (double x = area.X; x < area.X + area.Width; x += area.Width / density)
                {
                    points.Add(key, new(x, y));
                    graph.AddNode(key);
                    key++;
                }
            }

            int i = 0;
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
    }
}