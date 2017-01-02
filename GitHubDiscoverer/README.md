## GitHubDiscoverer

`ArchivesAnalyzer` downloads archives from [GitHub Archive](https://www.githubarchive.org/) to parse github user names:

```C#
var archivesAnalyzer = new ArchivesAnalyzer("2011-12-02-0", "2015-10-10-0");
archivesAnalyzer.DownloadAndParseArchives();
```

The result is in the *tool_dir*/userNames/githubers.txt file.

Then you can analyze the repositories of users in *githubers.txt* and download projects in specified languages
with a certain amount of stars:

```C#
var repositoryAnalyzer = new RepositoryRegexAnalyzer();
repositoryAnalyzer.DownloadRepositories(5, "Haskell", "Scala", "Clojure", "F#");
```

Projects are downloaded in *tool_dir*/repositories/*repository_name*.zip
