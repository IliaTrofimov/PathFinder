using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace PathFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void ConnectWindow_Click(object sender, RoutedEventArgs e)
        {
            SelectWindow selectWindow = new(graph.GetNodes());
            selectWindow.Show();
            selectWindow.OnConnectionChanged += (object _, ConnectionChangedArgs e) =>
            {
                graph.Selection_A = e.Id_1;
                graph.Selection_B = e.Id_2;
                bool result = false;

                if (e.IsConnected) result = graph.Connect();
                else result = graph.Disconnect();

                if (!result) System.Media.SystemSounds.Asterisk.Play();
                
                graph.Selection_A = -1;
                graph.Selection_B = -1;
            };  
        }
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!graph.Connect())
                System.Media.SystemSounds.Asterisk.Play();
        }
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (!graph.Disconnect())
                System.Media.SystemSounds.Asterisk.Play();
        }
        private void DisconnectAll_Click(object sender, RoutedEventArgs e)
        {
            if (!graph.DisconnectAll())
                System.Media.SystemSounds.Asterisk.Play();
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Models.GraphBuilder.GridTypes type = ((MenuItem)sender).Uid switch
            {
                "Dot" => Models.GraphBuilder.GridTypes.Disconnected,
                "Rec" => Models.GraphBuilder.GridTypes.Rectangle,
                "Tri" => Models.GraphBuilder.GridTypes.Triangle,
                "Rom" => Models.GraphBuilder.GridTypes.Romb,
                _ or "_" => Models.GraphBuilder.GridTypes.Empty
            };
            graph.Reset(Properties.Settings.Default.templates_density, type);
        }
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!graph.GetPath())
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch(System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }       
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new();
            aboutWindow.ShowDialog();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new();
            if(dialog.ShowDialog() == true)
            {
                try
                {
                    graph.SaveJSON(dialog.FileName);
                }
                catch (System.Exception ex)
                {
                    System.Media.SystemSounds.Asterisk.Play();
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            if (dialog.ShowDialog() == true)
            {
                try 
                { 
                    graph.LoadJSON(dialog.FileName); 
                }
                catch(System.Exception ex) 
                {
                    System.Media.SystemSounds.Asterisk.Play();
                    MessageBox.Show(ex.Message, "Error"); 
                }
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new();
            if (settingsWindow.ShowDialog() == true)
            {
                graph.SizeMultiplier = Properties.Settings.Default.size_multiplier;
            }
        }
    }
}
