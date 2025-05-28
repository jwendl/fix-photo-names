using MetadataExtractor;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var directoryInfo = new DirectoryInfo(@"");
            var files = directoryInfo.GetFiles().OrderBy(fi => GetDateTime(fi.FullName)).ToArray();
            for (var index = 0; index < files.Length; index++)
            {
                var fileInfo = files[index];
                var regex = new Regex(@"\d+");
                var match = regex.Match(fileInfo.Name);
                var replace = Regex.Replace(fileInfo.Name, @" \(\d+\)", $"-{index + 1:000}");
                fileInfo.MoveTo(Path.Combine(fileInfo.Directory.FullName, replace));
            }
        }

        private static DateTime GetDateTime(string imagePath)
        {
            var directories = ImageMetadataReader.ReadMetadata(imagePath);
            var possibleDates = directories.SelectMany(d => d.Tags).Where(t => t.Name.ToLowerInvariant().Contains("date"));
            var tag = possibleDates.OrderByDescending(d => ParseDate(d.Description)).FirstOrDefault();
            var possibleDate = ParseDate(tag.Description);
            return possibleDate;
        }

        private static DateTime ParseDate(string date)
        {
            DateTime resultDateTime;
            if (DateTime.TryParseExact(date, "ddd MMM dd HH:mm:ss zzz yyyy", null, DateTimeStyles.None, out resultDateTime))
                return resultDateTime;

            if (DateTime.TryParseExact(date, "yyyy:MM:dd HH:mm:ss", null, DateTimeStyles.None, out resultDateTime))
                return resultDateTime;

            if (DateTime.TryParseExact(date, "yyyy:MM:dd", null, DateTimeStyles.None, out resultDateTime))
                return resultDateTime;

            Console.WriteLine($"Didn't know date format: {date}");
            return DateTime.Now;
        }
    }
}
