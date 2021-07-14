using System.Collections.Generic;

namespace PathFinder.Models
{
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

    public class PathFinder
    {
        private Graph graph;
        private List<NodeInfo> infos;

        public PathFinder(Graph graph)
        {
            this.graph = graph;
        }

        private void InitInfo()
        {
            infos = new List<NodeInfo>(graph.NodesCount);
            foreach (var v in graph.Nodes)
                infos.Add(new NodeInfo(v.Value));
        }
        private NodeInfo GetVertexInfo(Node v)
        {
            foreach (var i in infos)
                if (i.Node.Equals(v)) return i;
            return null;
        }

        private NodeInfo MinSumUnvisited()
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
            Node startVertex = graph[id_1];
            Node finishVertex = graph[id_2];
            if (startVertex is null || finishVertex is null)
                throw new NodeDoesNotExistException();

            InitInfo();
            var first = GetVertexInfo(startVertex);
            first.TotalWeight = 0;
            for(var current = MinSumUnvisited(); current != null; current = MinSumUnvisited())
                SetSumToNextVertex(current);
            
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
                if (endVertex is null)
                    throw new NotConnectedException();
                path.Add(endVertex.Id);
            }
            return path;
        }
    }
}