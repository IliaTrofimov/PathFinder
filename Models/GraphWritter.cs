using PathFinder;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
 
public class GraphWritter : IXmlSerializable
{
    public Graph Graph;
    public Dictionary<int, Vector> Points;

    public GraphWritter()
    {
        Graph = new();
        Points = new();
    }
    public GraphWritter(Graph graph, Dictionary<int, Vector> points)
    {
        Graph = graph;
        Points = points;
    }

    public XmlSchema GetSchema() { return null; }

    public void ReadXml(XmlReader reader)
    {
        if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "GraphWritter")
        {   
            if (reader.ReadToDescendant("Nodes"))
            {
                var readerP = reader.ReadSubtree();
                if (readerP.ReadToDescendant("Node"))
                {
                    VectorConverter converter = new();
                    while (readerP.MoveToContent() == XmlNodeType.Element)
                    {
                        int id = Convert.ToInt32(readerP["Id"]);
                        Graph.AddNode(id); 
                        Points.Add(id, (Vector)converter.ConvertFromInvariantString(readerP["P"]));
                        readerP.ReadToFollowing("Node");
                    }
                }
                if (reader.ReadToFollowing("Links"))
                {
                    if (reader.ReadToDescendant("Start"))
                    {
                        while (reader.MoveToContent() == XmlNodeType.Element)
                        {
                            int id_1 = Convert.ToInt32(reader["Id"]);
                            if (reader.ReadToDescendant("End"))
                            {
                                while (reader.MoveToContent() == XmlNodeType.Element)
                                {
                                    int id_2 = Convert.ToInt32(reader["Id"]);
                                    Graph.Connect(id_1, id_2, (int)(Points[id_1] - Points[id_2]).Length);
                                    if(!reader.ReadToNextSibling("End")) break;
                                }
                            }
                            reader.ReadToFollowing("Start");
                        }
                    }
                }
            }
        }
        reader.Read();
    }

    public void WriteXml(XmlWriter writer)
    {
        VectorConverter converter = new();
        writer.WriteStartElement("Nodes");
        foreach(var p in Points)
        {
            writer.WriteStartElement("Node");
            writer.WriteAttributeString("Id", p.Key.ToString());
            writer.WriteAttributeString("P", converter.ConvertToInvariantString(p.Value));
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        writer.WriteStartElement("Links");
        foreach (var n in Graph.Nodes)
        {
            writer.WriteStartElement("Start");
            writer.WriteAttributeString("Id", n.Key.ToString());
            foreach(int l in n.Value.LinkedIDs)
            {
                writer.WriteStartElement("End");
                writer.WriteAttributeString("Id", l.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }
}