using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
namespace PathFinder
{
    /// <summary>
    /// Логика взаимодействия для AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            txt_appname.Text = Assembly.GetExecutingAssembly().GetName().Name;
            txt_version.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            txt_author.Text = "Ilia Trofimov";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
