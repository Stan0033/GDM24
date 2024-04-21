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

namespace Games_Database
{
    /// <summary>
    /// Interaction logic for enter_value.xaml
    /// </summary>
    public partial class enter_value : Window
    {
       public int value = 0;
        public enter_value()
        {
            InitializeComponent();
        }

        private void Set(object sender, RoutedEventArgs e)
        {
            if (Helper.IsInteger(Input.Text.Trim()) == false)
            {
                return;
            }
            value = Convert.ToInt32(Input.Text.Trim());
            DialogResult = true;
        }
    }
}
