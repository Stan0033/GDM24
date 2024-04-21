using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Games_Database
{
    public static class ImageSources
    {
        public static List<ImageSource> Sources = new List<ImageSource>();
        public static ImageSource Get(string url)
        {
            try
            {
                // Create a BitmapImage object
                BitmapImage bitmap = new BitmapImage();

                // Set BitmapImage properties
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(url);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // Return the BitmapImage as ImageSource
                return bitmap;
            }
            catch (Exception ex)
            {
                // Handle any exceptions, such as invalid URL or file not found
                MessageBox.Show($"Error loading image from URL: {ex.Message}");
                return null;
            }
        }
    }
}
