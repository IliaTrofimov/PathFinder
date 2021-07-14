using System.Windows;
using System.Windows.Controls;


namespace PathFinder.GraphControls
{
    /// <summary>
    /// Логика взаимодействия для SelectionArea.xaml
    /// </summary>
    public partial class SelectionArea : UserControl
    {
        public Rect Rect { get; } 

        public SelectionArea(Canvas root, Rect area)
        {
            InitializeComponent();
            Rect = area;
            Width = area.Width;
            Height = area.Height;
            Panel.SetZIndex(this, 0);
            Canvas.SetLeft(this, area.X);
            Canvas.SetTop(this, area.Y);
            root.Children.Add(this);
        }
    }
}
