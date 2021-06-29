using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PathFinder.GraphControls
{
    /// <summary>
    /// Логика взаимодействия для LinkControl.xaml
    /// </summary>
    public partial class LinkControl : UserControl
    {
        public Point P1
        {
            get => new() { X = base_line.X1, Y = base_line.Y1 };
            set
            {
                base_line.X1 = value.X;
                base_line.Y1 = value.Y;
            }
        }
        public Point P2
        {
            get => new() { X = base_line.X2, Y = base_line.Y2 };
            set
            {
                base_line.X2 = value.X;
                base_line.Y2 = value.Y;
            }
        }

        public LinkSelection Selection
        {
            get => selection;
            set
            {
                selection = value;
                base_line.Stroke = GetSelectionBrush(selection);
            }
        }
        private LinkSelection selection;

        public bool IsLabelEnabled
        {
            get => label.Visibility == Visibility.Visible;
            set => label.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public int NodeA { get; set; }
        public int NodeB { get; set; }


        public LinkControl(int id_a, int id_b)
        {
            InitializeComponent();
            NodeA = id_a;
            NodeB = id_b;
        }


        public void OnRedrawing(GraphControl sender, LinksRedrawingArgs e)
        {
            if(NodeA == e.Id_1 && NodeB == e.Id_2 || )
        }


        public static Brush GetSelectionBrush(LinkSelection selection) => selection switch
        {
            LinkSelection.None => Brushes.Gray,
            LinkSelection.Hover => Brushes.BurlyWood,
            LinkSelection.Path => Brushes.RosyBrown
        };
    }

    public enum LinkSelection { None, Hover, Path }
}
