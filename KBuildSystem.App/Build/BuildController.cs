using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace KBuildSystem.App.Build
{
    public struct BuildControllerOptions
    {
        public string BasePath { get; set; }
        public string RepositoryBasePath
        {
            get
            {
                return Path.Combine(BasePath, @"repos");
            }
        }
        public string BuildOutputBasePath
        {
            get
            {
                return Path.Combine(BasePath, @"builds");
            }
        }
    }
    public class BuildController
    {
        public BuildControllerOptions Options { get; private set; }
        public Server Server;
        public string BuildHistoryDirectory
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), @"buildhistory");
            }
        }

        public BuildController(Server server, BuildControllerOptions options)
        {
            Server = server;
            Options = options;
            if (!Directory.Exists(Options.BasePath))
                Directory.CreateDirectory(Options.BasePath);
            if (!Directory.Exists(Options.RepositoryBasePath))
                Directory.CreateDirectory(Options.RepositoryBasePath);
            if (!Directory.Exists(Options.BuildOutputBasePath))
                Directory.CreateDirectory(Options.BuildOutputBasePath);
            if (!Directory.Exists(BuildHistoryDirectory))
                Directory.CreateDirectory(BuildHistoryDirectory);
            updateBuildHistory();
        }

        public List<BuildHistoryObject> BuildHistory = new List<BuildHistoryObject>();

        public BuildHistoryObject GetByID(string id)
        {
            foreach (BuildHistoryObject build in BuildHistory)
            {
                if (build.ID == id)
                {
                    return build;
                }
            }
            return null;
        }

        private void updateBuildHistory()
        {
            Regex filenameExpression = new Regex(@"(\/|\\\\)[a-zA-Z0-9_\-]{1,}\.bhis$", RegexOptions.IgnoreCase);

            string[] files = Directory.GetFiles(BuildHistoryDirectory);

            List<BuildHistoryObject> buildHistory = new List<BuildHistoryObject>();

            foreach (string filename in files)
            {
                Match regexMatch = filenameExpression.Match(filename);
                if (!regexMatch.Success)
                    continue;
                BuildHistoryObject item = parseBuildHistoryFile(filename);
                if (item != null)
                {
                    buildHistory.Add(item);
                }
            }

            BuildHistory = buildHistory;
        }

        private BuildHistoryObject parseBuildHistoryFile(string filename)
        {
            BuildHistoryObject obj = new BuildHistoryObject(filename);
            if (obj.Valid)
            {
                return obj;
            }
            else
            {
                return null;
            }
        }
    }
}
