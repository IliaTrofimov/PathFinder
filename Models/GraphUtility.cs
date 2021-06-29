using System.Collections.Generic;


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
}