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

        public string BuildHistoryDirectory
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), @"buildhistory");
            }
        }

        public BuildController(BuildControllerOptions options)
        {
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
            Regex filenameExpression = new Regex(@"[a-zA-Z0-9_\-]+\.bhis$", RegexOptions.IgnoreCase);

            string[] files = Directory.GetFiles(BuildHistoryDirectory);

            List<BuildHistoryObject> buildHistory = new List<BuildHistoryObject>();

            foreach (string filename in files)
            {
                if (!filenameExpression.Match(filename).Success)
                    continue;
                BuildHistoryObject item = parseBuildHistoryFile(filename);
                if (item != null)
                {
                    buildHistory.Add(item);
                }
            }
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
