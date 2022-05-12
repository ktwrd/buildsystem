using System;
using System.IO;

namespace KBuildSystem.App.Build
{
    public class BuildSignature
    {
        private BuildController controller;
        public void SetController(BuildController _controller)
        {
            controller = _controller;
        }
        public BuildController GetController() => controller;
        public BuildSignature(BuildController _controller)
        {
        }

        public string Repository { get; set; }
        public string Organization { get; set; }
        public string Branch { get; set; }

        public string SourceLocation => Path.Combine(controller.Options.RepositoryBasePath, Organization, Repository);
        public string BuildRootLocation => Path.Combine(controller.Options.BuildOutputBasePath, Organization, Repository);
    }
}
