using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace PathFinder.Models
{
    public static class GraphWritter
    {
        public static async void ToJSON(string filename, Graph graph)
        {
            await File.WriteAllTextAsync(filename, ToJSONString(graph));
        }
        public static string ToJSONString(Graph graph)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            Wrapper wrapper = new(null, graph);
            return JsonConvert.SerializeObject(wrapper);
        }


        public static async void ToJSON(string filename, Graph graph, Dictionary<int, Vector> points)
        {
            await File.WriteAllTextAsync(filename, ToJSONString(graph, points));
        }
        public static string ToJSONString(Graph graph, Dictionary<int, Vector> points)
        {
            if (points is null)
                throw new ArgumentNullException(nameof(points));
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            Wrapper wrapper = new(points, graph);
            return JsonConvert.SerializeObject(wrapper);
        }


        public static void FormJSON(string filename, Graph graph, Dictionary<int, Vector> points)
        {
            using StreamReader fin = new(filename);
            Wrapper wrapper = JsonConvert.DeserializeObject<Wrapper>(fin.ReadToEnd());
            fin.Close();
            graph = wrapper.Graph;
            points = wrapper.Points;
        }
        public static void FormJSON(string filename, Graph graph)
        {
            using StreamReader fin = new(filename);
            Wrapper wrapper = JsonConvert.DeserializeObject<Wrapper>(fin.ReadToEnd());
            fin.Close();
            graph = wrapper.Graph;
        }


        public class Wrapper
        {
            [JsonProperty("points")]
            public Dictionary<int, Vector> Points { get; set; }

            [JsonProperty("graph")]
            public Graph Graph { get; set; }

            public Wrapper(Dictionary<int, Vector> points, Graph graph)
            {
                Points = points;
                Graph = graph;
            }
        }
    }
}
