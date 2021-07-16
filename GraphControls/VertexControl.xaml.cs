using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

namespace PathFinder.GraphControls
{
    /// <summary>
    /// Логика взаимодействия для VertexControl.xaml
    /// </summary>
    public partial class VertexControl : UserControl
    {
        private Brush base_brush;
        public NodeSelection Selection
        {
            get => selection;
            set
            {
                selection = value;
                base_brush = SelectionColor(value);
                base_ellipse.Fill = base_brush;
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

        public double SizeMultiplier
        {
            get => size_mult;
            set
            {
                if (value < 0)
                    throw new System.ArgumentException("Size multiplier cannot be negative");
                size_mult = value;
                grid.Height = 25 * size_mult;
                grid.Width = 25 * size_mult;
                label.Visibility = size_mult < 0.8 ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        private double size_mult;


        public VertexControl(int tag = 1, double mult = 1.0)
        {
            InitializeComponent();
            Tag = tag;
            Selection = NodeSelection.None;
            SizeMultiplier = mult;
            menu_clear.Visibility = selection != NodeSelection.None ? Visibility.Visible : Visibility.Collapsed;
        }
        public VertexControl(Panel root, Point point, int tag = 1, double mult = 1.0) 
        {
            InitializeComponent();
            Tag = tag;
            Selection = NodeSelection.None;
            SizeMultiplier = mult;
            menu_clear.Visibility = selection != NodeSelection.None ? Visibility.Visible : Visibility.Collapsed;

            root.Children.Add(this);
            Canvas.SetLeft(this, point.X);
            Canvas.SetTop(this, point.Y);
            Panel.SetZIndex(this, 2);
        }

        public delegate void SelectionChanged(VertexControl sender, SelectionChangedArgs e);
        public event SelectionChanged OnSelectionChanged;

        public delegate void Removed(VertexControl sender, int id);
        public event Removed OnRemoved;

        public delegate void Connected(VertexControl sender, int id);
        public event Connected OnConnected;


        /// <summary>
        /// Higlights control when mouse hovering over it.
        /// </summary>
        private void Base_MouseEnter(object sender, MouseEventArgs e)
        {
            base_ellipse.Fill = Brushes.CadetBlue;
        }
       
        /// <summary>
        /// Returns base color to control when mouse stops hovering over it.
        /// </summary>
        private void Base_MouseLeave(object sender, MouseEventArgs e)
        {
            base_ellipse.Fill = base_brush;
        }

        /// <summary>
        /// Changes control's selection when user selects new one in context menu. 
        /// Also calls OnSelectionChanged to notify other VertexControls.
        /// </summary>
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
        
        /// <summary>
        /// Calls OnRemoved to notify parent panel about node removal.
        /// </summary>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            OnRemoved(this, tag);
        }
        
        /// <summary>
        /// Calls OnConnected to notify parent panel.
        /// </summary>
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            OnConnected(this, tag);
        }


        /// <summary>
        /// Resets selection to match new selection told by associated GraphControl.
        /// </summary>
        public void OnSelectionReseted(GraphControl _, SelectionResetedArgs e)
        {
            if (e.Id != tag && e.newSelection == selection)
            {
                Selection = NodeSelection.None;
                menu_clear.Visibility = Visibility.Collapsed;
                menu_selectA.Visibility = Visibility.Visible;
                menu_selectB.Visibility = Visibility.Visible;
            }
        }
        

        /// <summary>
        /// Returns Brush associated with exact NodeSelection.
        /// </summary>
        public static Brush SelectionColor(NodeSelection s) => s switch
        {
            NodeSelection.A => Brushes.Crimson,
            NodeSelection.B => Brushes.CornflowerBlue,
            _ or NodeSelection.None => Brushes.SlateGray
        };
    }
}