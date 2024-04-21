using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Orientation = System.Windows.Controls.Orientation;
using Brush = System.Drawing.Brush;
using Image = System.Windows.Controls.Image;
using System.Windows.Media;

namespace Games_Database
{
    class Record
    {
        public string Title { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new List<string>();
        public string Series { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public int Disk { get; set; } = -1;
        public override string ToString()
        {
            return $"<{Title}|{Disk}|{Series}|{Developer}|{Publisher}|{ImageURL}|{string.Join("/", Tags)}>";
        }

        public Record(string name, int disk, string series, string developer, string publisher, string imageUrl, List<string> tags)
        {
            Title = name;
            Disk = disk;
            Tags = tags;
            Series = series;
            Developer = developer;
            Publisher = publisher;
            ImageURL = imageUrl;

        }
        public Record()
        {
        }

        public ListBoxItem GetAsItem()
        {
            ListBoxItem listBoxItem = new ListBoxItem();

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Children.Add(new TextBlock() { Width = 250, Text = Title });
            stackPanel.Children.Add(new TextBlock() { Width = 150, Text = Disk.ToString(), TextAlignment = System.Windows.TextAlignment.Center });
            stackPanel.Children.Add(new TextBlock() { Width = 150, Text = Series });
            stackPanel.Children.Add(new TextBlock() { Width = 150, Text = Developer });
            stackPanel.Children.Add(new TextBlock() { Width = 150, Text = Publisher });
            stackPanel.Children.Add(new TextBlock() { Width = 150, Text = string.Join(",", Tags) });
            if (ImageURL.Length > 0) {

                listBoxItem.Foreground = System.Windows.Media.Brushes.Gold;
                ImageSource img = ImageSources.Get(ImageURL);

                listBoxItem.ToolTip = new Image() { Width=230,Height=107.5, Source = img };
                ImageSources.Sources.Add(img);
            };
        
     
            listBoxItem.Content = stackPanel;
            return listBoxItem;
        }

        internal void SetSame(Record record)
        {
           Title = record.Title;
            Disk = record.Disk;
            Series = record.Series;
            Developer = record.Developer;
            Publisher = record.Publisher;
            ImageURL = record.ImageURL;
            Tags = record.Tags.ToList();
            
        }
    }
}
