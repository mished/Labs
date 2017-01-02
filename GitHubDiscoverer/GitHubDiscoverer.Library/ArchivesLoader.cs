using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace GitHubDiscoverer.Library
{
    public class ArchivesLoader
    {
        private const int maxAttempts = 3;

        public ArchivesLoader()
        {
            Directory.CreateDirectory(DefaultPathes.appDirectory + "\\archives");
        }

        public async Task<string> DownloadAsync(string date)
        {
            var address = string.Format(DefaultPathes.archiveURL, date);
            var fileName = string.Format(DefaultPathes.archivesPath, date);            
            var attemptsCount = 0;

            using (var client = new WebClient()) {
                while (true) {
                    try {
                        await client.DownloadFileTaskAsync(address, fileName);
                        return fileName;
                    } catch (WebException ex) {
                        if (++attemptsCount == maxAttempts)
                            throw new WebException("Failed to download archive: " + fileName);
                    }
                }
            }
        }
    }
}
