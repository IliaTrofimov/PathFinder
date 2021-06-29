using PathFinder.GraphControls;
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
using System.Windows.Shapes;

namespace PathFinder
{
    /// <summary>
    /// Логика взаимодействия для SelectWindow.xaml
    /// </summary>
    public partial class SelectWindow : Window
    {
        public SelectWindow(List<int> nodes)
        {
            InitializeComponent();
            list_a.ItemsSource = nodes;
            list_b.ItemsSource = nodes;
        }

        public delegate void ConnectionChanged(object sender, ConnectionChangedArgs e);
        public event ConnectionChanged OnConnectionChanged;

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (list_a.SelectedItem != null && list_b.SelectedItem != null &&
                (int)list_a.SelectedItem != (int)list_b.SelectedItem)
            {
                OnConnectionChanged(this, new((int)list_a.SelectedItem, (int)list_b.SelectedItem, ((Control)sender).Uid == "C"));
            }
            else
            {
                System.Media.SystemSounds.Asterisk.Play();
            }
        }
    }

    public class ConnectionChangedArgs
    {
        public int Id_1, Id_2;
        public bool IsConnected;

        public ConnectionChangedArgs(int id_1, int id_2, bool isConnected)
        {
            Id_1 = id_1;
            Id_2 = id_2;
            IsConnected = isConnected;
        }
    }
}
