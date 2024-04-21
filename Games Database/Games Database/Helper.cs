using System.IO;
using System.Text.RegularExpressions;

namespace Games_Database
{
    public static class Helper
    {
        public static bool IsInteger(string input)
        {
            if (int.TryParse(input, out _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public static List<string> GetFileAndFolderNames(string folderPath)
        {
            List<string> folders = Directory.GetDirectories(folderPath)
                                            .Where(dir => !IsHidden(dir))
                                            .Select(dir => Path.GetFileName(dir))
                                            .ToList();

            List<string> names = Directory.GetFiles(folderPath)
                                          .Where(file => !IsHidden(file))
                                          .Select(file => Path.GetFileNameWithoutExtension(file))
                                          .ToList();

            names.AddRange(folders);

            return names;
        }

        public static bool IsHidden(string path)
        {
            // Check if the file or folder is hidden
            FileAttributes attributes = File.GetAttributes(path);
            return (attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
        }





        public static List<string> ExtractStringsInAngleBrackets(string input)
        {
            List<string> extractedStrings = new List<string>();

            // Define the regular expression pattern to match strings inside angle brackets
            string pattern = @"\<([^>]*)\>";

            // Match the pattern against the input string
            MatchCollection matches = Regex.Matches(input, pattern);

            // Iterate through the matches and extract the strings
            foreach (Match match in matches)
            {
                // Get the captured group within the angle brackets
                Group group = match.Groups[1];
                // Add the captured string to the list
                extractedStrings.Add(group.Value);
            }

            return extractedStrings;
        }
      public static List<string> GetFoldersContentInFolders(string FolderPath)
        {
            List<string> result = new();
            List<string> folders = Directory.GetDirectories(FolderPath).ToList();
            foreach (string folder in folders) 

            {
                result.AddRange(GetFileAndFolderNames(folder));
            }
            return result;
        }
      
    }
}
