using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Reflection;
using Path = System.IO.Path;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using SelectionMode = System.Windows.Controls.SelectionMode;
using Clipboard = System.Windows.Clipboard;
using System.Text.RegularExpressions;


namespace Games_Database
{

    public partial class MainWindow : Window
    {

        public List<string> Genres = new List<string>();
        List<Record> records = new();
        string exeDirectory = string.Empty;
        private bool Saved = true;
        List<string> Logg = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            string exePath = Assembly.GetExecutingAssembly().Location;
              exeDirectory = System.IO.Path.GetDirectoryName(exePath);
            
            Genres = File.ReadAllLines(Path.Combine(exeDirectory,"Genres.txt")).ToList();
            foreach (string line in Genres)
            {
                GenresContainer.Items.Add( new CheckBox() { Content = line });
            }
            

            
        }
        private void ClearFields()
        {
            List_Disks.Items.Clear();
            Field_Title.Text = string.Empty;
            Field_Series.Text = string.Empty;
            Field_Disk.Text = string.Empty;
            Field_Dev.Text = string.Empty;
            Field_Pub.Text = string.Empty;
            Field_Url.Text = string.Empty;
            foreach (object item in GenresContainer.Items)
            {
                CheckBox c = (CheckBox)item;
                c.IsChecked = false;
            }
        }
        private List<string> GetCheckedTags()
        {
            List<string> ch = new List<string>();
            foreach (object item in GenresContainer.Items)
            {
                CheckBox c = (CheckBox)item;
               if (c.IsChecked == true)
                {
                    ch.Add(c.Content.ToString());
                }
            }
            return ch;
        }
        private Record CollectData()
        {
            return new Record(Field_Title.Text, int.Parse(Field_Disk.Text), Field_Series.Text, Field_Dev.Text,Field_Pub.Text,Field_Url.Text, GetCheckedTags());
        }
        private void ClearDB(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                records.Clear();
                Lister.Items.Clear();
                ClearFields();
                RefreshDiskList();
                SetSaved(false);
            }
            
        }

        private void NewRecord(object sender, RoutedEventArgs e)
        {
            if (Field_Title.Text.Trim().Length == 0) { MessageBox.Show("Empty title name is not allowed"); return; }
            if (RecordExists(Field_Title.Text.Trim())) {  return; }
            if (Helper.IsInteger(Field_Disk.Text) == false) { MessageBox.Show("Disk input is not integer"); return; }
            if (GetCheckedTags().Count == 0) { MessageBox.Show("Select at least one tag"); return; }
            Record r = CollectData();
           
            int disk = int.Parse(Field_Disk.Text);
            r.Disk = disk;
           records.Add(r);
            RefreshDiskList();
            SetSaved(false);
        }   
         private void RefreshDiskList(){
            List_Disks.Items.Clear();
            List<int> uniqueDisks = records.Select(record => record.Disk)
                                 .Distinct()
                                 .OrderBy(disk => disk)
                                 .ToList();

            foreach (int key in uniqueDisks) { 
                List_Disks.Items.Add(new ListBoxItem() { 
                Content = key
                , 
                BorderThickness = new Thickness(1), 
                BorderBrush = System.Windows.Media.Brushes.Black 
            }); 
            }
            DiskCountPreview.Text = $"Disks: {uniqueDisks.Count}";
            RecordsCounter.Text = $"Records: {records.Count}";
        }
         
       
        private bool RecordExists(string name)
        {
           
                foreach (Record r in  records)
                {
                    if (r.Title.ToLower() == name.ToLower())
                    {
                        Logg.Add($"{r.Disk}:" + r.Title);
                    MessageBox.Show("A record with this title exists.");
                    return true;

                    }
                }
 
                   
                 
            
            return false;
        }
        private void Update(object sender, RoutedEventArgs e)
        {
            if (Field_Title.Text.Trim().Length == 0) { MessageBox.Show("Empty title name is not allowed"); return; }
             
            if (Helper.IsInteger(Field_Disk.Text) == false) { MessageBox.Show("Disk input is not integer"); return; }
            if (Lister.SelectedItems.Count!=1) { MessageBox.Show("Select 1 record"); return; }
         
            Record selected = GetSelectedRecord(); 
            selected. SetSame(CollectData());
            ListBoxItem i = Lister.SelectedItem as ListBoxItem;
            i = selected.GetAsItem();
            SetSaved(false);
        }

      

        private Record GetSelectedRecord()
        {
            ListBoxItem item = Lister.SelectedItem as ListBoxItem;
            StackPanel p = item.Content as StackPanel;
            TextBlock one = p.Children[0] as TextBlock;
            string title = one.Text.ToLower();
             
                foreach (Record r in records)
                {
                    if (r.Title.ToLower() == title)
                    {
                        return r;
                    }
                }
            
            return new Record();



        }

        private void AddFolder(object sender, RoutedEventArgs e)
        {
            if (Helper.IsInteger(Field_Disk.Text) == false) { MessageBox.Show("Disk input is not integer"); return; }
            if (GetCheckedTags().Count == 0) { MessageBox.Show("Select at least one tag"); return; }
            int disk = int.Parse(Field_Disk.Text);
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display the selected folder path
               string selected = dialog.SelectedPath;
                List<string> files = Helper.GetFileAndFolderNames(selected);
                List<string> notadded = new();
                foreach (string file in files)
                {
                    if (RecordExists(file))
                    {

                        Logg.Add($"{disk}:" + file);

                    }
                    else
                    {
                        
                        records.Add(new Record(file, disk, "", "", "", "", GetCheckedTags()));
                    }
                     
                }
                if (notadded.Count > 0)
                {
                    MessageBox.Show("These titles were not added because of duplication:\n" + string.Join("\n", notadded)); ;
                }
                RefreshDiskList();
                SetSaved(false);
            }


        }

        private void AddFolders(object sender, RoutedEventArgs e)
        {
            if (Helper.IsInteger(Field_Disk.Text) == false) { MessageBox.Show("Disk input is not integer"); return; }
            if (GetCheckedTags().Count == 0) { MessageBox.Show("Select at least one tag"); return; }
            int disk = int.Parse(Field_Disk.Text);
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display the selected folder path
                string selected = dialog.SelectedPath;
                List<string> files = Helper.GetFoldersContentInFolders(selected);
                
                foreach (string file in files)
                {
                    if (RecordExists(file))
                    {
                        Logg.Add($"{disk}:" + file);

                    }
                    else
                    {
                    
                        records.Add(new Record(file, disk, "", "", "", "", GetCheckedTags()));
                    }

                }
                if (Logg.Count > 0)
                {
                    MessageBox.Show("There a re duplicating titles that were not added. Check the log."); ;
                }
                RefreshDiskList();
                SetSaved(false);
            }
        }

        private void ViewDisk(object sender, MouseButtonEventArgs e)
        {
          
            if (List_Disks.SelectedItems.Count != 1) { return; }
            Lister.Items.Clear();
            ListBoxItem item = List_Disks.SelectedItems[0] as ListBoxItem;
            int key = int.Parse(item.Content.ToString()); ;
            foreach (Record record in records)
            {
                if (record.Disk == key)
                {
                    Lister.Items.Add(record.GetAsItem());
                }
            }
            Input_Disksz.Text = key.ToString();
        }

        private void ViewAllDisks(object sender, RoutedEventArgs e)
        {
            Lister.Items.Clear();
           
                foreach (Record r in  records)
                {
                    Lister.Items.Add(r.GetAsItem());
                }
             
        }
        private void ViewAllNonGolden(object sender, RoutedEventArgs e)
        {
            Lister.Items.Clear();

            foreach (Record r in records)
            {
                if (r.ImageURL.Trim().Length == 0)
                {

                    Lister.Items.Add(r.GetAsItem());
                }
            }

        }
        private void ChangediskNumber(object sender, RoutedEventArgs e)
        {
            if (List_Disks.SelectedItems.Count != 1) { return; }
            ListBoxItem item = List_Disks.SelectedItems[0] as ListBoxItem;
            int oldKey = int.Parse(item.Content.ToString()); ;
            enter_value ev = new enter_value();
            ev.ShowDialog();
            if (ev.DialogResult == true)
            {
                int newKey = ev.value;
                ChangediskNumber( oldKey, newKey);
                item.Content = newKey;
                SetSaved(false);
            }
        }
        void ChangediskNumber(int old, int nw)
        {
            foreach (Record r in records)
            {
                if (r.Disk == old) { r.Disk = nw; }
            }
        }
        

        
        public void findDuplicates(object sender, RoutedEventArgs e)
        {
            List<string> duplicates = FindDuplicateTitles( );
            if (duplicates.Count == 0) { return; }
            else 
            { 
                Lister.Items.Clear();
            }
            foreach (string duplicate in duplicates)
            {
                Lister.Items.Add(duplicate);    
            }
        }

        private void DeleteDisk(object sender, RoutedEventArgs e)
        {
            if (List_Disks.SelectedItems.Count != 1) { return; }
            ListBoxItem item = List_Disks.SelectedItems[0] as ListBoxItem;
            int n = int.Parse(item.Content.ToString()); ;
            //unfinished
            records.RemoveAll(x=>x.Disk == n);
            RefreshDiskList();
            Lister.Items.Clear();
            SetSaved(false);
        }

        private void SelectedItem(object sender, SelectionChangedEventArgs e)
        {
            Field_Title.Text = string.Empty;
            Field_Series.Text = string.Empty;
            Field_Disk.Text = string.Empty;
            Field_Dev.Text = string.Empty;
            Field_Pub.Text = string.Empty;
            Field_Url.Text = string.Empty;
            if (Lister.SelectedItems.Count != 1) { return; }
            ButtonUpdate.IsEnabled = Lister.SelectedItems.Count == 1;
            //  if (Lister.SelectedItem !is ListBoxItem) { return; }
            Record selected = GetSelectedRecord();
            Field_Title.Text = selected.Title;
            Field_Series.Text = selected.Series;
            Field_Disk.Text = selected.Disk.ToString();
            Field_Dev.Text = selected.Developer;
            Field_Pub.Text = selected.Publisher;
            Field_Url.Text = selected.ImageURL;
            FillTags(selected.Tags);

        }
        private void FillTags(List<string> tags)
        {
            foreach (object cb in GenresContainer.Items)
            {
                CheckBox c = cb as CheckBox;   
                if (tags.Contains(c.Content.ToString())){
                    c.IsChecked = true;
                }
                else
                {
                    c.IsChecked= false;
                }
               
            }
        }
        private void DeleteRecord(string title)
        {
          records.RemoveAll(x => x.Title == title);
        }
        private void DeleteRecord(object sender, MouseButtonEventArgs e)
        {
            if (Lister.SelectedItems.Count != 1) { return; }
            if (Lister.SelectedItem! is ListBoxItem) { return; }
            ListBoxItem item = Lister.SelectedItems[0] as ListBoxItem;
            StackPanel st = item.Content as StackPanel;
            TextBlock t = st.Children[0] as TextBlock;
            DeleteRecord(t.Text);
            Lister.Items.Remove(Lister.SelectedItem);
          
        }
        private void SetSaved(bool saved)
        {
           Saved = saved;
            string sav = saved ? "saved" : "unsaved";
            Title = $"GDM24 [{sav}]";
            Button_Save.IsEnabled = !saved;
        }
        private void SearchCount(object sender, RoutedEventArgs e)
        {
            List<Record> results = Search();
          
            SearchResultPreview.Text = "Search - " + results.Count.ToString();
        }

        static bool ContainsAnySubstring(string mainString, List<string> substrings)
        {
            foreach (var substring in substrings)
            {
                if (mainString.Contains(substring))
                {
                    return true;
                }
            }
            return false;
        }
        public  List<string> FindDuplicateTitles( )
        {
            return records.GroupBy(record => record.Title.ToLower())
                      .Where(group => group.Count() > 1)
                      .Select(group => group.Key)
                      .ToList();

             
        }
          List<int> GetDisks( )
        {
            string input = Input_Disksz.Text.Trim();
            if (input.Length == 0) { return new List<int>(); }
            List<int> integers = new List<int>();
            string[] substrings = input.Split(',');

            foreach (string substring in substrings)
            {
                if (int.TryParse(substring, out int number))
                {
                    integers.Add(number);
                }
            }

            return integers;
        }
        private List<string> GetKeywords()
        {
            List<string> result = InputSearch.Text.Trim().Split(",").ToList();
            foreach (string record in result)
            {
                record.Trim();
            }
            return result;

        }
        private List<Record> Search()
        {
            List<Record> results = new();
            
            List<int> searchedDisks = GetDisks();




            List<string> keywords = GetKeywords();
            for (int i = 0; i < keywords.Count; i++) { keywords[i] = keywords[i].ToLower(); }
            
            List<string> tags = GetCheckedTags();
           
            if (Check_Tags.IsChecked == true && tags.Count == 0) { goto skip; }
            
                foreach (Record r in records)
                {
                if (searchedDisks.Count > 0)  { if (searchedDisks.Contains(r.Disk) == false) { continue; } }
                if (Check_Img.IsChecked == true) { if (r.ImageURL.Length == 0) { continue; } }
                if (keywords.Count == 0) { results.Add(r); continue; }  
                    if (Check_Tags.IsChecked == true) { if (r.Tags.Intersect(tags).Any() == false) { continue; } }
                    if (Check_Title.IsChecked == true) { if (ContainsAnySubstring(r.Title.ToLower(), keywords)) { results.Add(r); continue; } }
                    if (Check_Dev.IsChecked == true) { if (ContainsAnySubstring(r.Developer.ToLower(), keywords)) { results.Add(r); continue; } }
                    if (Check_Pub.IsChecked == true) { if (ContainsAnySubstring(r.Publisher.ToLower(), keywords)) { results.Add(r); continue; } }
                    if (Check_Series.IsChecked == true) { if (ContainsAnySubstring(r.Series.ToLower(), keywords)) { results.Add(r); continue; } }
                }
 skip:
           return results;

        
        }
        private List<Record> SearchWhite()
        {
            List<Record> results = new();
            if (InputSearch.Text.Trim().Length == 0) { MessageBox.Show("Empty search"); goto skip; }
            List<string> keywords = InputSearch.Text.Trim().Split(",").ToList();
            for (int i = 0; i < keywords.Count; i++) { keywords[i] = keywords[i].ToLower(); }

            List<string> tags = GetCheckedTags();

            if (Check_Tags.IsChecked == true && tags.Count == 0) { goto skip; }

            foreach (Record r in records)
            {
                if (r.ImageURL.Trim().Length > 0) { continue; }
                if (Check_Tags.IsChecked == true) { if (r.Tags.Intersect(tags).Any() == false) { continue; } }
                if (Check_Title.IsChecked == true) { if (ContainsAnySubstring(r.Title.ToLower(), keywords)) { results.Add(r); continue; } }
                if (Check_Dev.IsChecked == true) { if (ContainsAnySubstring(r.Developer.ToLower(), keywords)) { results.Add(r); continue; } }
                if (Check_Pub.IsChecked == true) { if (ContainsAnySubstring(r.Publisher.ToLower(), keywords)) { results.Add(r); continue; } }
                if (Check_Series.IsChecked == true) { if (ContainsAnySubstring(r.Series.ToLower(), keywords)) { results.Add(r); continue; } }
            }



        skip:
            return results;
        }
        private void Search(object sender, RoutedEventArgs e)
        {
            List<Record> results = Search();
            SearchResultPreview.Text = "Search - " + results.Count.ToString();
            Lister.Items.Clear();
            foreach (Record r in results)
            {
                Lister.Items.Add(r.GetAsItem());
            }

            
           
           
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if (!Saved)
            {
                 
                    foreach (Record r in records)
                    {
                        sb.AppendLine(r.ToString()); ;
                    }
                   
                 
                SetSaved(true);
            }
            else
            {
                MessageBox.Show("Already saved"); return;
            }
           
            File.WriteAllText(Path.Combine(exeDirectory, "Database.txt"),sb.ToString());
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(exeDirectory, "Database.txt");
            if (!File.Exists(path)) { return; }
            string l = File.ReadAllText(path);
            records.Clear();
            List<string> items = Helper.ExtractStringsInAngleBrackets(l);
           
            foreach (string item in items)
            {
                List<string> segments = item.Split("|").ToList();
                if (segments.Count != 7) {  continue; }
                List<string> tags = new List<string>();

                
                    tags = segments[6].Split("/").ToList();
                 
               records.Add(new Record(segments[0], int.Parse(segments[1]), segments[2], segments[3], segments[4], segments[5], tags));
               
                
            }
            SetSaved(true);
           RefreshDiskList();
        }

        private void SearchEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { Search(null,null); }
        }

        private void Log(object sender, RoutedEventArgs e)
        {
            Logw log = new Logw(Logg);
            log.ShowDialog();
        }

        private void DisplayRecoprdsCount(object sender, SelectionChangedEventArgs e)
        {
            if (List_Disks.SelectedItems.Count != 1) { return; }
            Lister.Items.Clear();
            ListBoxItem item = List_Disks.SelectedItems[0] as ListBoxItem;
            int key = int.Parse(item.Content.ToString()); ;
            
            RecordsCountPreview.Text = $"Records in disk: {records.Count(x=>x.Disk == key)}";
        }
        private List<string> GetSelectedRecords()
        {
            if (Lister.SelectedItems.Count == 0) { return new List<string>(); }
            List<string> records = new List<string>();

            foreach (object item in Lister.SelectedItems)
            {
                ListBoxItem i = item as ListBoxItem;
                StackPanel s = i.Content as StackPanel;
                TextBlock t = s.Children[0] as TextBlock;
                records.Add(t.Text);
            }
            return records;
        }

        private void DeleteRecords(object sender, RoutedEventArgs e)
        {
            List<string> rc = GetSelectedRecords();
            
            for (int i = Lister.SelectedItems.Count - 1; i >= 0; i--)
            {
                Lister.Items.Remove(Lister.SelectedItems[i]);
            }
            foreach (string record in rc)
            {
                foreach (var item in records.ToList())
                {
                    records.RemoveAll(x => x.Title == record); ;
                }
            }
            SetSaved(false);
            RefreshDiskList();
        }
             

        private void ChangeRecordDisk(object sender, RoutedEventArgs e)
        {
            if (Field_Disk.Text.Trim().Length == 0) { MessageBox.Show("enter disk number");return; }
            if (Helper.IsInteger(Field_Disk.Text) == false) { MessageBox.Show("Disk input is not integer"); return; }
            List<string> SelectedRecords = GetSelectedRecords();
            int disk = int.Parse(Field_Disk.Text);
           foreach (Record r in  records)
            {
                if (SelectedRecords.Contains( r.Title)) { r.Disk =  disk;  }
            }
            Search(null, null);
            RefreshDiskList();
            SetSaved(false);
        }

        private void SearchInGoogle(object sender, RoutedEventArgs e)
        {
            string search = RemoveParenthesisPrefix( Field_Title.Text.Trim());// + "+Steam";
            if (search.Length == 0) { return; }
            SearchInGoogle(search);
        }
        static void SearchInGoogle(string query)
        {
            try
            {
                // Replace spaces with plus signs and encode the query
               // string encodedQuery = Uri.EscapeDataString(query.Replace(" ", "+"));
                string encodedQuery = query.Replace(" ", "+");
                // Construct the Google search URL
                string searchUrl = $"https://store.steampowered.com/search/?term={encodedQuery}";
                // Open the URL in the default web browser
                Process.Start(new ProcessStartInfo(searchUrl) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        static string RemoveParenthesisPrefix(string input)
        {
            // Define a regular expression pattern to match anything enclosed in parenthesis at the start of the string
            Regex regex = new Regex(@"^\((.*?)\)\s*");

            // Check if the input string matches the pattern
            Match match = regex.Match(input);

            if (match.Success)
            {
                // If there's a match, remove the matched part from the input string
                return input.Substring(match.Length);
            }
            else
            {
                // If there's no match, return the original string
                return input;
            }
        }

            private void Multiselect(object sender, RoutedEventArgs e)
        {
            Lister.SelectionMode = Check_Multiselect.IsChecked == false?SelectionMode.Single :  SelectionMode.Multiple;
        }

        private void PasteURL(object sender, RoutedEventArgs e)
        {
            Field_Url.Text = Clipboard.GetText();
        }

        private void SearchWhite(object sender, MouseButtonEventArgs e)
        {
            List<Record> results = SearchWhite();
            SearchResultPreview.Text = "Search - " + results.Count.ToString();
            Lister.Items.Clear();
            foreach (Record r in results)
            {
                Lister.Items.Add(r.GetAsItem());
            }
        }

        private void SearchCountWhite(object sender, MouseButtonEventArgs e)
        {
            List<Record> results = SearchWhite();

            SearchResultPreview.Text = "Search - " + results.Count.ToString();
        }

        private void Input_Disksz_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search(null,null);
            }
        }
    }

   

}