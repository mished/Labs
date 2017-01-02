using GitHubDiscoverer.Library;

namespace GitHubDiscoverer.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            // Download archives and parse github usernames:
            //
            var archivesAnalyzer = new ArchivesAnalyzer("2011-12-02-0", "2015-06-19-0");
            archivesAnalyzer.DownloadAndParseArchives();

            // Download and parse user repositories page using RepositoryRegexAnalyzer:
            //
            var repositoryAnalyzer = new RepositoryRegexAnalyzer();
            repositoryAnalyzer.DownloadRepositories(5, "Haskell", "Scala", "Clojure", "F#");
        }
    }
}
