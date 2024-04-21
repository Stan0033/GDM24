using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using MessageBox = System.Windows.Forms.MessageBox;

namespace Games_Database
{
    /// <summary>
    /// Interaction logic for previewer.xaml
    /// </summary>
    public partial class previewer : Window
    {
        public previewer(string path)
        {
            InitializeComponent();
            LoadImageFromUrl(path);
        }
        private void LoadImageFromUrl(string url)
        {
            try
            {
                // Create a WebClient to download the image
                WebClient webClient = new WebClient();
                byte[] data = webClient.DownloadData(url);

                // Create a BitmapImage from the downloaded data
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new System.IO.MemoryStream(data);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // Set the BitmapImage as the source for the Image control
                Img.Source = bitmap;
            }
            catch (Exception ex)
            {
                // Handle any errors
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }
    }
}
