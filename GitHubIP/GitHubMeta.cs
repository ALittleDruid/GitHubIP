using System.Collections.Generic;

namespace GitHubIP
{
    public class GitHubMeta
    {
        public List<string> Actions { get; set; }
        public List<string> Api { get; set; }
        public List<string> Hooks { get; set; }
        public List<string> Web { get; set; }
        public List<string> Git { get; set; }
        public List<string> Packages { get; set; }
        public List<string> Pages { get; set; }
        public List<string> Importer { get; set; }
        public List<string> Dependabot { get; set; }

        public static Dictionary<string, List<string>> Meta { get; } = new Dictionary<string, List<string>>();
    }
}
