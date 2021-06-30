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


        /// <summary>
        /// Returns node associated with this id or null if node wasn't found.
        /// </summary>
        public Node this[int id]
        {
            get => !Nodes.ContainsKey(id) ? null : Nodes[id];
        }

        /// <summary>
        /// Returns weight of link between nodes associated with these IDs or 0 if nodes not connected or not exist.
        /// </summary>
        public int this[int id_1, int id_2]
        {
            get => !(Nodes.ContainsKey(id_1) && Nodes.ContainsKey(id_2)) ? 0 : Nodes[id_1].Links[Nodes[id_2]];
        }


        /// <summary>
        /// Returns true if node present in this graph.
        /// </summary>
        public bool Contains(int id) => Nodes.ContainsKey(id);


        /// <summary>Attempts to add new node with this id.</summary>
        /// <returns>Returns true if node was added successfully, else returns fasle.</returns>
        public bool AddNode(int id)
        {
            return Nodes.TryAdd(id, new Node(id));
        }

        /// <summary>
        /// Returns node associated with this id or null if node wasn't found. Same as Graph[id] and may be removed soon.
        /// </summary>
        public Node GetNode(int id)
        {
            return this[id];
        }
        
        /// <summary>Attempts to remove node with this id.</summary>
        /// <returns>Returns true if node was removed successfully, else returns fasle.</returns>
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


        /// <summary>Attempts to connect nodes with these id.</summary>
        /// <returns>Returns true if nodes were connected successfully, else returns fasle.</returns>
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
        
        /// <summary>Attempts to disconnect nodes with these id.</summary>
        /// <returns>Returns true if nodes were disconnected successfully, else returns fasle.</returns>
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
       
        /// <summary>
        /// Same as Connect() but this will create new nodes if nodes with such IDs doesn't present in graph.
        /// </summary>
        public void ConnectAnyway(int id_1, int id_2, int weight)
        {
            if (!Contains(id_1))
                AddNode(id_1);
            if (!Contains(id_2))
                AddNode(id_2);

            this[id_1].Connect(this[id_2], weight);
            this[id_2].Connect(this[id_1], weight);
        }


        /// <summary>Gets shortest path between two nodes.</summary>
        /// <returns>List with nodes' IDs.</returns>
        public List<int> FindPath(int id_1, int id_2)
        {
            Models.PathFinder finder = new(this);
            return finder.FindShortestPath(id_1, id_2);
        }
    }
}