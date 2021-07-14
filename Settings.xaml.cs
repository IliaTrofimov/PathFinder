using System;
using System.Windows;
using System.Windows.Controls;

namespace PathFinder
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            combo_size.SelectedIndex = Properties.Settings.Default.size_multiplier switch
            {
                0 => 4,
                > 0 and <= 0.2 => 3,
                > 0.2 and <= 0.81 => 2,
                > 0.81 and <= 1 => 1,
                > 1 => 0,
                _ => 1
            };
            combo_density.SelectedIndex = Properties.Settings.Default.templates_density switch
            {
                5 => 0,
                10 => 1,
                15 => 2,
                20 => 3,
                25 => 4,
                _ => 1
            };
        }



        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.templates_density = Convert.ToInt32(((ComboBoxItem)combo_density.SelectedItem).Content);
            Properties.Settings.Default.size_multiplier = combo_size.SelectedIndex switch
            {
                0 => 1.5,
                2 => 0.81,
                3 => 0.2,
                4 => 0,
                1 or _ => 1,
            };
            Properties.Settings.Default.Save();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


    }
}
