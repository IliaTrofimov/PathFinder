using System;
using System.Collections.Generic;

namespace PathFinder
{
    /// <summary>
    /// Class for graph node
    /// </summary>
    public class Node
    {
        public int Id { get; private set; }

        /// <summary>
        /// Collection of connected nodes and weights
        /// </summary>
        public Dictionary<Node, int> Links { get; private set; }


        public Node(int id = 0)
        {
            Id = id;
            Links = new();
        }


        /// <summary>
        /// Connects two nodes. Returns true if successfull.
        /// </summary>
        public bool Connect(Node node, int weight) => Links.TryAdd(node, weight);

        /// <summary>
        /// Disconnects two nodes. Returns true if successfull.
        /// </summary>
        public bool Disconnect(Node node) => Links.Remove(node);

        /// <summary>
        /// Returns list of IDs of nodes that connected to this node
        /// </summary>
        public List<int> GetLinkedIDs()
        {
            List<int> links = new(Links.Count);
            foreach (var l in Links)
                links.Add(l.Key.Id);
            return links;
        }
        public bool IsLinked(Node node) => Links.ContainsKey(node);
    }


    /// <summary>
    /// Class of non-oriented graph
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// Dictionary with graph's nodes. Keys are meant to be equal with Node.Id
        /// </summary>
        public Dictionary<int, Node> Nodes { get; private set; }
        public int NodesCount => Nodes.Count;


        public Graph()
        {
            Nodes = new();
        }


        public Node this[int id]
        {
            get => !Nodes.ContainsKey(id) ? null : Nodes[id];
        }
        public int this[int id_1, int id_2]
        {
            get => !(Nodes.ContainsKey(id_1) && Nodes.ContainsKey(id_2)) ? 0 : Nodes[id_1].Links[Nodes[id_2]];
        }


        public bool Contains(int id) => Nodes.ContainsKey(id);


        public bool AddNode(int id)
        {
            return Nodes.TryAdd(id, new Node(id));
        }
        public Node GetNode(int id)
        {
            return this[id];
        }
        public bool RemoveNode(int id)
        {
            var node_rem = GetNode(id);
            if (node_rem != null)
            {
                foreach (var node in Nodes)
                    node.Value.Disconnect(node_rem);
                return Nodes.Remove(id);
            }
            return false;
        }


        public bool Connect(int id_1, int id_2, int weight)
        {
            var n1 = GetNode(id_1);
            if (n1 != null)
            {
                var n2 = GetNode(id_2);
                if (n2 != null)
                {
                    n1.Connect(n2, weight);
                    n2.Connect(n1, weight);
                    return true;
                }
            }
            return false;
        }
        public bool Disconnect(int id_1, int id_2)
        {
            var n1 = GetNode(id_1);
            if (n1 != null)
            {
                var n2 = GetNode(id_2);
                if (n2 != null)
                {
                    n1.Disconnect(n2);
                    n2.Disconnect(n1);
                    return true;
                }
            }
            return false;
        }
        public void ConnectAnyway(int id_1, int id_2, int weight)
        {
            if (!Contains(id_1))
                AddNode(id_1);
            if (!Contains(id_2))
                AddNode(id_2);

            this[id_1].Connect(this[id_2], weight);
            this[id_2].Connect(this[id_1], weight);
        }


        public List<int> FindPath(int id_1, int id_2)
        {
            Models.PathFinder finder = new(this);
            return finder.FindShortestPath(id_1, id_2);
        }
    }
}