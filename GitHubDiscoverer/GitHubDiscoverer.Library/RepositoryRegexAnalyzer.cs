using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GitHubDiscoverer.Library
{
    public class RepositoryRegexAnalyzer
    {
        private List<string> githubers = new List<string>(1000000);
        private List<Task> downloadQuery = new List<Task>();
        private int minStarsCount;
        private string[] languages;
        private const int maxAttempts = 3;

        private RepositoryRegexAnalyzer()
        {
            Directory.CreateDirectory(DefaultPathes.appDirectory + "\\repositories");
        }

        public RepositoryRegexAnalyzer(string userNamesPath = null)
            : this()
        {
            if (userNamesPath == null)
                userNamesPath = DefaultPathes.userNamesPath;
            githubers = new List<string>(File.ReadAllLines(DefaultPathes.userNamesPath));
        }

        public void DownloadRepositories(int minStarsCount)
        {
            DownloadRepositories(minStarsCount, null);
        }

        public void DownloadRepositories(int minStarsCount, params string[] languages)
        {
            this.minStarsCount = minStarsCount;
            this.languages = languages;

            foreach (var githuber in githubers) {
                try {
                    Parse(DownloadPage(githuber));
                    Console.WriteLine("Page parsed: " + githuber);
                } catch (WebException ex) {
                    Trace.WriteLine("WebError occured: " + ex.Message);
                    continue;
                }
            }

            Task.WaitAll(downloadQuery.ToArray());
        }        

        private void Parse(string userRepositoriesPage)
        {
            var languagePattern = new Regex(@"<div class=""repo-list-stats"">\s+(?<language>.+)");
            var starsPattern = new Regex(@"<span class=""octicon octicon-star""></span>\s+(?<stars>.+)");
            var linkPattern = new Regex(@"<h3 class=""repo-list-name"">\s+<a href=""(?<link>[^""]+)"">");
            int repoStarsCount;
            string repoLanguage, repoLink;

            var stars = starsPattern.Match(userRepositoriesPage);
            var link = linkPattern.Match(userRepositoriesPage);

            foreach (Match repository in languagePattern.Matches(userRepositoriesPage)) {
                repoStarsCount = int.Parse(stars.Groups["stars"].Value, NumberStyles.AllowThousands);
                repoLanguage = repository.Groups["language"].Value;
                repoLink = link.Groups["link"].Value;

                if (repoStarsCount >= minStarsCount && (languages == null || languages.Contains(repoLanguage)))
                    downloadQuery.Add(DownloadRepositoryAsync(repoLink));

                stars = stars.NextMatch();
                link = link.NextMatch();
            }
        }

        public string DownloadPage(string userName)
        {            
            var attemptsCount = 0;

            using (var client = new WebClient()) {
                while (true) {
                    try {
                        return client.DownloadString(string.Format(DefaultPathes.repositoriesPage, userName));
                    } catch (WebException ex) {
                        if (++attemptsCount == maxAttempts)
                            throw new WebException("Failed to download user's page: " + userName);                        
                    }
                }
            }
        }

        public async Task DownloadRepositoryAsync(string repositoryName)
        {
            var address = string.Format(DefaultPathes.downloadRepositoryZip, repositoryName);
            var fileName = string.Format(DefaultPathes.repositoriesPath, repositoryName.Replace('/', '-'));            
            var attemptsCount = 0;

            using (var client = new WebClient()) {
                while (true) {
                    try {
                        Console.WriteLine("Starting repository download: " + repositoryName);
                        await client.DownloadFileTaskAsync(address, fileName);
                        return;
                    } catch (WebException ex) {
                        if (++attemptsCount == maxAttempts)
                            throw new WebException("Failed to download repository: " + repositoryName);                        
                    }
                }
            }
        }
    }
}
