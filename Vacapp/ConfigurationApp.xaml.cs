using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Vacapp
{
    public  partial class ConfigurationApp : PhoneApplicationPage
    {
        static String value;
        public ConfigurationApp()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)comboBox1.SelectedItem;
            value = typeItem.Content.ToString();
            System.Diagnostics.Debug.WriteLine(value);
        }

        public static String getValue()
        {
            return value;
        }
    }
}