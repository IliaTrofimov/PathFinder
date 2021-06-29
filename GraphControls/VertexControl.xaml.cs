using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PathFinder.GraphControls
{
    /// <summary>
    /// Логика взаимодействия для VertexControl.xaml
    /// </summary>
    public partial class VertexControl : UserControl
    {
        public Brush BaseBrush
        {
            get => base_brush;
            set
            {
                base_ellipse.Fill = value;
                base_brush = value;
            }
        }
        private Brush base_brush;
        public NodeSelection Selection
        {
            get => selection;
            set
            {
                selection = value;
                BaseBrush = SelectionColor(value);
                Tag = tag;
            }
        }
        private NodeSelection selection;
        public new int Tag
        {
            get => tag;
            set
            {
                label.Text = $"V{tag}";
                tag = value;
            }
        }
        private int tag;


        public VertexControl(int tag = 1, NodeSelection selection = NodeSelection.None)
        {
            InitializeComponent();
            Tag = tag;
            Selection = selection;
            base_brush = SelectionColor(selection);
            menu_clear.Visibility = selection != NodeSelection.None ? Visibility.Visible : Visibility.Collapsed;
        }


        public delegate void SelectionChanged(VertexControl sender, SelectionChangedArgs e);
        public event SelectionChanged OnSelectionChanged;

        public delegate void LinksSelectionChanged(VertexControl sender, int id);
        public event LinksSelectionChanged OnLinksSelectionChanged;

        public delegate void Removed(VertexControl sender, int id);
        public event LinksSelectionChanged OnRemoved;

        public delegate void Connected(VertexControl sender, int id);
        public event Connected OnConnected;


        private void Base_MouseEnter(object sender, MouseEventArgs e)
        {
            base_ellipse.Fill = Brushes.Azure;
        }
        private void Base_MouseLeave(object sender, MouseEventArgs e)
        {
            base_ellipse.Fill = base_brush;
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            NodeSelection lastSelection = selection;
            MenuItem menu = sender as MenuItem;
            switch (menu.Uid)
            {
                case "A":
                    Selection = NodeSelection.A;
                    menu_clear.Visibility = Visibility.Visible;
                    menu.Visibility = Visibility.Collapsed;
                    menu_selectB.Visibility = Visibility.Visible;
                    break;
                case "B":
                    Selection = NodeSelection.B;
                    menu_clear.Visibility = Visibility.Visible;
                    menu.Visibility = Visibility.Collapsed;
                    menu_selectA.Visibility = Visibility.Visible;
                    break;
                default:
                    Selection = NodeSelection.None;
                    menu.Visibility = Visibility.Collapsed;
                    menu_selectA.Visibility = Visibility.Visible;
                    menu_selectB.Visibility = Visibility.Visible;
                    break;
            }
            OnSelectionChanged(this, new(tag, lastSelection, selection));
        }
        private void Links_Click(object sender, RoutedEventArgs e)
        {
            OnLinksSelectionChanged(this, tag);
        }
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            OnRemoved(this, tag);
        }
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            OnConnected(this, tag);
        }


        public void OnSelectionReseted(GraphControl sender, SelectionResetedArgs e)
        {
            if (e.Id != tag && e.newSelection == selection)
            {
                Selection = NodeSelection.None;
                menu_clear.Visibility = Visibility.Collapsed;
                menu_selectA.Visibility = Visibility.Visible;
                menu_selectB.Visibility = Visibility.Visible;
            }
        }
        public void OnPathRedrawn(GraphControl sender, SelectionResetedArgs e)
        {
            // TODO: optimize
            if (e.Id == tag)
                Selection = NodeSelection.Path;
        }


        public static Brush SelectionColor(NodeSelection s) => s switch
        {
            NodeSelection.A => Brushes.Crimson,
            NodeSelection.B => Brushes.CornflowerBlue,
            NodeSelection.Path => Brushes.SaddleBrown,
            _ or NodeSelection.None => Brushes.LightYellow
        };
    }


    public enum NodeSelection { A, B, Path, None }
    public class SelectionChangedArgs : SelectionResetedArgs
    {
        public NodeSelection lastSelection;

        public SelectionChangedArgs(int id, NodeSelection lastSelection, NodeSelection newSelection) : base(id, newSelection)
        {
            this.lastSelection = lastSelection;
        }
    }
}