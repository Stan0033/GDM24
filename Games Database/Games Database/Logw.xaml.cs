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
    /// Interaction logic for Logw.xaml
    /// </summary>
    public partial class Logw : Window
    {
        public Logw(List<string> list)
        {
            InitializeComponent();
            foreach(string  item in list)
            {
               Listx.Items.Add(item);
            }
        }
    }
}
