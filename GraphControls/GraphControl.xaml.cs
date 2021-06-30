using PathFinder.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PathFinder.GraphControls
{
    /// <summary>
    /// Логика взаимодействия для GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl
    {
        private GraphView view;
        private List<int> path;

        private Dictionary<Pair, Line> lines;

        private int selec_a, selec_b;
        public int Selection_A
        {
            get => selec_a;
            set
            {
                selec_a = value;
                txt_select.Text = SelectionStr;
            }
        }
        public int Selection_B
        {
            get => selec_b;
            set
            {
                selec_b = value;
                txt_select.Text = SelectionStr;
            }
        }
        public int NodesCount
        {
            get => view.NodesCount;
        }


        public GraphControl()
        {
            InitializeComponent();
            view = new();
            selec_a = -1;
            selec_b = -1;
            lines = new();
            txt_select.Text = SelectionStr;
        }


        public delegate void SelectionReseted(GraphControl sender, SelectionResetedArgs e);
        public event SelectionReseted OnSelectionReseted;



        /// <summary>
        /// Complitely redraws control. Does not affect graph model.
        /// </summary>
        private void Draw()
        {
            canvas.Children.Clear();
            lines.Clear();
            txt_graphTable.Text = view.GraphString();

            foreach (var p_1 in view.Points)
            {
                DrawNode(p_1.Value, p_1.Key);
                foreach (var p_2 in view.GetLinkedPoints(p_1.Key))
                {
                    Pair pair = new(p_1.Key, p_2.Key);
                    if (!lines.ContainsKey(pair))
                        DrawLink(p_1.Value, p_2.Value, pair);
                }
            }
        }

        /// <summary>Draws one VertexControl representing some graph node.</summary>
        /// <param name="v">Node postition</param>
        /// <param name="id">Node id</param>
        private void DrawNode(Vector v, int id)
        {
            VertexControl ver = new(id, ToNodeSelection(id));
            canvas.Children.Add(ver);
            Canvas.SetLeft(ver, v.X);
            Canvas.SetTop(ver, v.Y);
            Panel.SetZIndex(ver, 2);

            ver.OnSelectionChanged += OnSelectionChanged;
            ver.OnRemoved += OnNodeRemoved;
            ver.OnConnected += OnConnected;
            OnSelectionReseted += ver.OnSelectionReseted;
        }

        /// <summary>Draws one link (Line) and adds it to lines dictionary</summary>
        /// <param name="v1">Start position</param>
        /// <param name="v2">End position</param>
        /// <param name="pair">Pair with ids of nodes that will be connected by this link</param>
        private void DrawLink(Vector v1, Vector v2, Pair pair)
        {
            Line l = new()
            {
                X1 = v1.X, X2 = v2.X,
                Y1 = v1.Y, Y2 = v2.Y,
                Stroke = Brushes.Gray,
            };
            lines.Add(pair, l);
            canvas.Children.Add(l);
            Canvas.SetLeft(l, 12.5);
            Canvas.SetTop(l, 12.5);
            Canvas.SetZIndex(l, 1);
        }
        
        /// <summary>
        /// Redraws links associated with found path
        /// </summary>
        private void DrawPath(List<int> path, Brush brush, double thickness = 1)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                lines[new(path[i], path[i + 1])].Stroke = brush;
                lines[new(path[i], path[i + 1])].StrokeThickness = thickness;
            }
        }



        /// <summary>
        /// Removes node and associated links without redrawing scene, also resets selections.
        /// </summary>
        private void OnNodeRemoved(VertexControl sender, int id)
        {
            canvas.Children.Remove(sender);
            foreach (var p_2 in view.GetLinkedPoints(id))
            {
                Pair pair = new(id, p_2.Key);
                canvas.Children.Remove(lines[pair]);
                lines.Remove(pair);
            }
                
            view.RemoveNode(id);
            Selection_A = -1;
            Selection_B = -1;
            txt_graphTable.Text = view.GraphString();
        }

        /// <summary>
        /// Changes selections and redraws all nodes to match new selection.
        /// </summary>
        private void OnSelectionChanged(VertexControl sender, SelectionChangedArgs e)
        {
            switch (e.newSelection)
            {
                case NodeSelection.A:
                    Selection_A = e.Id;
                    OnSelectionReseted(this, new(selec_a, e.newSelection));
                    break;
                case NodeSelection.B:
                    Selection_B = e.Id;
                    OnSelectionReseted(this, new(selec_b, e.newSelection));
                    break;
                default:
                    if (e.lastSelection == NodeSelection.A) Selection_A = -1;
                    else if (e.lastSelection == NodeSelection.B) Selection_B = -1;
                    OnSelectionReseted(this, new(e.Id, e.newSelection));
                    break;
            }
        }
        
        /// <summary>
        /// Connects two nodes.
        /// </summary>
        private void OnConnected(VertexControl sender, int id)
        {
            if (selec_a == -1) Selection_A = id;
            else if (selec_b == -1) Selection_B = id;

            if (selec_b != -1) Connect();
        }
        
        /// <summary>
        /// Redraws scene when canvas had loaded for the first time.
        /// </summary>
        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
        }
        
        /// <summary>
        /// Adds new node to view and calls DrawNode() to draw it. Resets selections.
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            int id = view.AddNode((Vector)Mouse.GetPosition(this));
            if (id != -1)
                DrawNode(view.Points[id], id);
            txt_graphTable.Text = view.GraphString();
            Selection_A = -1;
            Selection_B = -1;
        }



        /// <summary>Draws shortest path between selected nodes.</summary>
        /// <returns>True if path was found successfuly, false if path wasn't found or nodes wasn't selected correctly.</returns>
        public bool GetPath()
        {
            if (selec_a != -1 && selec_b != -1)
            {   
                if (path != null) DrawPath(path, Brushes.Gray);

                path = view.GetPath(selec_a, selec_b);
                if (path != null || path.Count != 0)
                {
                    DrawPath(path, Brushes.Brown, 2.5);
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>Tries to connect selected nodes.</summary>
        /// <returns>True if nodes were successfuly connected.</returns>
        public bool Connect()
        {
            bool result = view.Connect(selec_a, selec_b);
            if (result)
            {
                DrawLink(view.Points[selec_a], view.Points[selec_b], new(selec_a, selec_b));
            }
            return result;
        }

        /// <summary>Tries to disconnect selected nodes.</summary>
        /// <returns>True if nodes were successfuly disconnected.</returns>
        public bool Disconnect()
        {
            bool result = view.Disconnect(selec_a, selec_b);
            if (result)
            {
                Pair p = new(selec_a, selec_b);
                canvas.Children.Remove(lines[p]);
                lines.Remove(p);
            }
            return result;
        }

        /// <summary>Tries to disconnect all nodes.</summary>
        /// <returns>True if nodes were successfuly disconnected, false if no nodes were found in graph.</returns>
        public bool DisconnectAll()
        {
            if (view.NodesCount == 0)
                return false;

            view.DisconnectAll();
            Draw();
            return true;
        }
        
        /// <summary>
        /// Resets graph model and redraws control.
        /// </summary>
        public void Reset(GraphBuilder.GridTypes type = GraphBuilder.GridTypes.Empty)
        {
            view = new(new(type, new(15, 15, canvas.ActualWidth * 0.85, canvas.ActualHeight * 0.85), 10));
            Selection_A = -1;
            Selection_B = -1;
            Draw();
        }


        private string SelectionStr => $"A = {selec_a}\nB = {selec_b}";
        private NodeSelection ToNodeSelection(int id)
        {
            return selec_a == id ? NodeSelection.A : selec_b == id ? NodeSelection.B : NodeSelection.None;
        }

        /// <summary>
        /// Returns table representation of graph model.
        /// </summary>
        public string TableStr() => view.GraphString();
        
        /// <summary>
        /// Return IDs of all nodes.
        /// </summary>
        public List<int> GetNodes() => new(view.Points.Keys);
        
        /// <summary>
        /// Saves graph model associated with this control to JSON-file.
        /// </summary>
        public void SaveJSON(string filename)
        {
            view.SaveJSON(filename);
        }
       
        /// <summary>
        /// Loads graph model from JSON-file and redraws control.
        /// </summary>
        public void LoadJSON(string filename)
        {
            view.LoadJSON(filename);
            Draw();
        }
    }
}