using System;

namespace GitHubDiscoverer.Library
{
    public class DefaultPathes
    {
        public static readonly string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string archivesPath = appDirectory + @"\archives\{0}.json.gz";
        public static readonly string userNamesPath = appDirectory + @"\userNames\githubers.txt";
        public static readonly string repositoriesPath = appDirectory + @"\repositories\{0}.zip";

        public static readonly string archiveURL = @"http://data.githubarchive.org/{0}.json.gz";
        public static readonly string repositoriesPage = @"https://github.com/{0}?tab=repositories";
        public static readonly string downloadRepositoryZip = @"https://github.com{0}/archive/master.zip";
    }
}
