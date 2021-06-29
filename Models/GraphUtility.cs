using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

        public class Comparer : IEqualityComparer<Pair>
        {
            public bool Equals(Pair x, Pair y)
            {
                return (x.A == y.A && x.B == y.B) || (x.B == y.A && x.A == y.B);
            }

            public int GetHashCode([DisallowNull] Pair obj)
            {
                return obj.GetHashCode();
            }
        }
    }

    
}