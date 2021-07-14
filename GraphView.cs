using PathFinder.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PathFinder
{
    class GraphView
    {
        private Graph graph;
        public Dictionary<int, Vector> Points { get => points; private set => points = value; }
        private Dictionary<int, Vector> points;
        public int NodesCount => Points.Count;
        private int avaliable_id;
        

        public GraphView()
        {
            graph = new();
            Points = new();
            avaliable_id = 0;
        }

        public GraphView(GraphBuilder builder)
        {
            builder.Build(out graph, out points);
            avaliable_id = points.Count + 1;
        }

       
        public int AddNode(Vector p)
        {
            if (Points.ContainsKey(avaliable_id) && graph.Contains(avaliable_id))
                return -1;

            Points.Add(avaliable_id, p);
            graph.AddNode(avaliable_id);
            avaliable_id = Points.Keys.Last() + 1;
            return avaliable_id - 1;
        }
        public bool RemoveNode(int id)
        {
            if (Points.ContainsKey(id) && graph.Contains(id))
            {
                Points.Remove(id);
                graph.RemoveNode(id);
                avaliable_id = id;
            }
            return false;
        }
        public void Clear()
        {
            Points.Clear();
            graph.Nodes.Clear();
            avaliable_id = 0;
        }
        public void ResizePoints(double offset_mult)
        {
            if (offset_mult <= 0)
                throw new System.ArgumentException("Offset multiplier must be greater then 0");
            Dictionary<int, Vector> temp = new(points.Count);
            foreach (var p in points)
                temp.Add(p.Key, new(p.Value.X * offset_mult, p.Value.Y * offset_mult));
            points = temp;
        }


        public bool Connect(int id_1, int id_2)
        {
            if (!(Points.ContainsKey(id_1) && Points.ContainsKey(id_2)))
                return false;
            return graph.Connect(id_1, id_2, (int)(Points[id_1] - Points[id_2]).Length);
        }
        public bool Disconnect(int id_1, int id_2)
        {
            return graph.Disconnect(id_1, id_2);
        }
        public void DisconnectAll()
        {
            foreach (var node in graph.Nodes.Values)
                node.Links.Clear();
        }


        public List<int> GetPath(int id_1, int id_2)
        {
            try
            {
                return new Models.PathFinder(graph).FindShortestPath(id_1, id_2);
            }
            catch (GraphException)
            {
                return new();
            }
        }
        public Dictionary<int, Vector> GetLinkedPoints(int id)
        {
            Dictionary<int, Vector> points = new();
            foreach (var p in graph[id].GetLinkedIDs())
                points.Add(p, Points[p]);
            return points;
        }
        public string GraphString()
        {
            string str = "";
            foreach (var n in graph.Nodes)
            {
                str += $"V{n.Key} - ({Points[n.Key].X:f0}, {Points[n.Key].Y:f0}):\t";
                foreach (var l in n.Value.Links)
                    str += $" {l.Key.Id}";
                str += "\n";
            }
            return str;
        }


        public string JSON()
        {
            return GraphWritter.ToJSONString(graph, Points);
        }
        public void SaveJSON(string filename)
        {
            GraphWritter.ToJSON(filename, graph, Points);
        }
        public void LoadJSON(string filename)
        {
            GraphWritter.FormJSON(filename, graph, Points);
        }
    }
}
