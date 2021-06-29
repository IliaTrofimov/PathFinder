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

        private int selec_a, selec_b;
        public int Selection_A
        {
            get => selec_a;
            set
            {
                selec_a = value;
                if (selec_a != -1 || selec_b != -1)
                    menu_addconnect.Visibility = Visibility.Visible;
            }
        }
        public int Selection_B
        {
            get => selec_b;
            set
            {
                selec_b = value;
                if (selec_a != -1 || selec_b != -1)
                    menu_addconnect.Visibility = Visibility.Visible;
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
            txt_select.Text = SelectionStr;
        }


        public delegate void SelectionReseted(GraphControl sender, SelectionResetedArgs e);
        public event SelectionReseted OnSelectionReseted;
        public event SelectionReseted OnPathRedrawn;

        public delegate void LinkRedrawing(GraphControl sender, LinksRedrawingArgs e);
        public event LinkRedrawing OnLinkRedrawing;

        public delegate void LinkRemoving(GraphControl sender, int nodeId);
        public event LinkRemoving OnLinkRemoving;


        private void Draw()
        {
            canvas.Children.Clear();
            txt_select.Text = SelectionStr;
            foreach (var p_1 in view.Points)
            {
                DrawNode(p_1.Value, p_1.Key);
                foreach (var p_2 in view.GetLinkedPoints(p_1.Key))
                    DrawLink(p_1.Value, p_2);
            }
        }
        private void DrawNode(Vector v, int id, bool is_path = false)
        {
            VertexControl ver = new(id, ToNodeSelection(id));
            ver.BaseBrush = is_path ? Brushes.SandyBrown : Brushes.LightYellow;
            canvas.Children.Add(ver);
            Canvas.SetLeft(ver, v.X);
            Canvas.SetTop(ver, v.Y);
            Panel.SetZIndex(ver, 2);

            ver.OnSelectionChanged += OnSelectionChanged;
            ver.OnLinksSelectionChanged += OnLinksSelectionChanged;
            ver.OnRemoved += OnNodeRemoved;
            ver.OnConnected += OnConnected;
            OnSelectionReseted += ver.OnSelectionReseted;
            OnPathRedrawn += ver.OnPathRedrawn;
        }
        private void DrawLink(Vector v1, Vector v2)
        {
            Line l = new() { X1 = v1.X, X2 = v2.X, Y1 = v1.Y, Y2 = v2.Y, Stroke = Brushes.Gray };
            canvas.Children.Add(l);
            Canvas.SetLeft(l, 12.5);
            Canvas.SetTop(l, 12.5);
            Canvas.SetZIndex(l, 1);
        }
        private void DrawPath()
        {
            foreach (var id in view.Points.Keys)
                if(path.Contains(id)) OnPathRedrawn(this, new(id, NodeSelection.Path));
        }


        private void OnNodeRemoved(VertexControl sender, int id)
        {
            canvas.Children.Remove(sender);
            view.RemoveNode(id);
            if (selec_a == id) Selection_A = -1;
            if (selec_b == id) Selection_B = -1;
            txt_graphTable.Text = view.GraphString();
            Draw();
        }
        private void OnLinksSelectionChanged(VertexControl sender, int id)
        {
            
        }
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
            txt_select.Text = SelectionStr;
        }
        private void OnConnected(VertexControl sender, int id)
        {
            if (selec_a == -1) Selection_A = id;
            else if (selec_b == -1) Selection_B = id;

            if (selec_b != -1) Connect();
        }


        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            int id = view.AddNode((Vector)Mouse.GetPosition(this));
            if (id != -1)
                DrawNode(view.Points[id], id);
            txt_graphTable.Text = view.GraphString();
        }
        private void AddConnect_Click(object sender, RoutedEventArgs e)
        {
            int id = view.AddNode((Vector)Mouse.GetPosition(this));
            if (selec_a != -1)
                view.Connect(selec_a, id);
            else if (selec_b != -1)
                view.Connect(selec_b, id);

            Draw();
        }


        public bool GetPath()
        {
            if (selec_a == -1 || selec_b == -1)
                return false;
            path = view.GetPath(selec_a, selec_b);
            if (path != null || path.Count != 0)
            {
                DrawPath();
                return true;
            }
            return false;
        }
        public bool Connect()
        {
            bool result = view.Connect(selec_a, selec_b);
            Draw();
            return result;
        }
        public bool Disconnect()
        {
            bool result = view.Disconnect(selec_a, selec_b);
            Draw();
            return result;
        }
        public bool DisconnectAll()
        {
            if (view.NodesCount == 0)
                return false;

            view.DisconnectAll();
            Draw();
            return true;
        }
        public void Reset()
        {
            view = new();
            Selection_A = -1;
            Selection_B = -1;
            Draw();
        }


        private string SelectionStr => $"A = {selec_a}\nB = {selec_b}";
        private NodeSelection ToNodeSelection(int id)
        {
            return selec_a == id ? NodeSelection.A : selec_b == id ? NodeSelection.B : NodeSelection.None;
        }


        public string TableStr() => view.GraphString();
        public List<int> GetNodes() => new(view.Points.Keys);
        public void SaveJSON(string filename)
        {
            view.SaveJSON(filename);
        }
        public void LoadJSON(string filename)
        {
            view.LoadJSON(filename);
            Draw();
        }
    }

    public class SelectionResetedArgs
    {
        public int Id;
        public NodeSelection newSelection;

        public SelectionResetedArgs(int id, NodeSelection newSelection)
        {
            this.Id = id;
            this.newSelection = newSelection;
        }
    }
    public class LinksRedrawingArgs
    {
        public int Id_1, Id_2;
        public LinkSelection Selection;

        public LinksRedrawingArgs(int id_1, int id_2, LinkSelection selection)
        {
            Id_1 = id_1;
            Id_2 = id_2;
            Selection = selection;
        }
    }
}