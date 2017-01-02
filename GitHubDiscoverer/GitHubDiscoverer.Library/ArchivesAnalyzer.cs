using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GitHubDiscoverer.Library
{
    public class ArchivesAnalyzer
    {
        private ArchivesLoader loader;
        private Dictionary<int, string> githubers = new Dictionary<int, string>(1000000);
        private int totalArchivesParsed = 0;

        private string StartArchive { get; set; }
        private string EndArchive { get; set; }

        private ArchivesAnalyzer()
        {
            Directory.CreateDirectory(DefaultPathes.appDirectory + "\\userNames");
            loader = new ArchivesLoader();
        }

        public ArchivesAnalyzer(string startArchive, string endArchive)
            : this()
        {
            DateTime s, e;
            if (DateTime.TryParseExact(startArchive, "yyyy-MM-dd-H", CultureInfo.InvariantCulture, DateTimeStyles.None, out s) == false
            || DateTime.TryParseExact(endArchive, "yyyy-MM-dd-H", CultureInfo.InvariantCulture, DateTimeStyles.None, out e) == false
               )
                throw new FormatException("Wrong archive's date format.");

            StartArchive = startArchive;
            EndArchive = endArchive;
        }

        public void DownloadAndParseArchives()
        {
            var downloadQuery = new List<Task<string>>();
            var currentArchive = StartArchive;
            DateTime currentArchiveTime;
            DateTime endArchive;
            var archivesParsed = 0;
            Task<string> finishedTask = null;

            endArchive = DateTime.ParseExact(EndArchive, "yyyy-MM-dd-H", CultureInfo.InvariantCulture, DateTimeStyles.None);

            do {
                downloadQuery.Add(loader.DownloadAsync(currentArchive));

                currentArchiveTime = DateTime.ParseExact(currentArchive, "yyyy-MM-dd-H", CultureInfo.InvariantCulture, DateTimeStyles.None);
                currentArchiveTime = currentArchiveTime.AddHours(1);
                currentArchive = currentArchiveTime.ToString("yyyy-MM-dd-H");

            } while (currentArchiveTime <= endArchive);

            while (downloadQuery.Count > 0) {
                try {
                    finishedTask = Task.WhenAny(downloadQuery).Result;
                    downloadQuery.Remove(finishedTask);
                    Parse(finishedTask.Result);
                    Console.WriteLine("Archive parsed: {0}, Total archives parsed: {1}", finishedTask.Result, totalArchivesParsed);
                } catch (Exception ex) {
                    Trace.WriteLine(string.Format("WebError occured: " + ex.InnerException.Message + " Total archives parsed: {0}", totalArchivesParsed));
                    downloadQuery.Remove(finishedTask);
                    continue;
                }

                if (++archivesParsed == 1000)
                    using (var tempWriter = new StreamWriter(DefaultPathes.appDirectory + @"\userNames\githubersTemp.txt")) {
                        archivesParsed = 0;
                        foreach (var githuber in githubers)
                            tempWriter.WriteLine(githuber.Value);
                    }
            }

            using (var namesWriter = new StreamWriter(DefaultPathes.userNamesPath)) {
                foreach (var githuber in githubers)
                    namesWriter.WriteLine(githuber.Value);
            }
        }

        private void Parse(string archivePath)
        {
            using (var compressedStream = new FileStream(archivePath, FileMode.Open))
            using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(decompressionStream)) {
                var json = reader.ReadToEnd();
                var loginPattern = new Regex(@"""id"":(?<id>\d+),""login"":""(?<login>[a-zA-Z0-9]+_?[a-zA-Z0-9]*)""");    // "id":2600066,"login":"someLogin"

                foreach (Match m in loginPattern.Matches(json)) {
                    var id = Convert.ToInt32(m.Groups["id"].Value);
                    if (!githubers.ContainsKey(id))
                        githubers.Add(id, m.Groups["login"].Value);
                }

                totalArchivesParsed++;
            }

            File.Delete(archivePath);
        }
    }
}
