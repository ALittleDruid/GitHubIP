using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace GitHubIP
{
    public class IPInfo : ModelBase
    {
        private string ip;
        public string IP
        {

            get => ip;

            set
            {
                ip = value;
                NotifyOfPropertyChange(nameof(IP));
            }
        }

        private PingReply pingResult;
        public PingReply PingResult
        {

            get => pingResult;

            set
            {
                pingResult = value;
                NotifyOfPropertyChange(nameof(PingResult));
            }
        }

        private bool ping;
        public bool Ping
        {

            get => ping;

            set
            {
                ping = value;
                NotifyOfPropertyChange(nameof(Ping));
            }
        }
    }
    public class MainWindowViewModel : ModelBase
    {
        private static Dictionary<string, List<IPInfo>> IPCache { get; } = new Dictionary<string, List<IPInfo>>();
        private List<string> metaTypes;
        public List<string> MetaTypes
        {
            get => metaTypes;

            set
            {
                metaTypes = value;
                NotifyOfPropertyChange(nameof(MetaTypes));
            }
        }

        private string selectMetaType;

        public string SelectMetaType
        {

            get => selectMetaType;

            set
            {
                selectMetaType = value;
                NotifyOfPropertyChange(nameof(SelectMetaType));
                if (!string.IsNullOrEmpty(value))
                {
                    if (IPCache.ContainsKey(value))
                    {
                        IPList = IPCache[value];
                        return;
                    }
                    if (GitHubMeta.Meta.ContainsKey(value))
                    {
                        var list = GitHubMeta.Meta[value];
                        if (list != null)
                        {
                            var current = new List<IPInfo>();
                            list.ForEach(x =>
                            {
                                if (!string.IsNullOrEmpty(x))
                                {
                                    current.Add(new IPInfo
                                    {
                                        IP = x.Split('/')[0]
                                    });
                                }

                            });
                            IPList = current;
                            IPCache[value] = IPList;
                            return;
                        }
                    }
                }
                IPList = null;
            }
        }

        private List<IPInfo> ipList;
        public List<IPInfo> IPList
        {

            get => ipList;

            set
            {
                ipList = value;
                NotifyOfPropertyChange(nameof(IPList));
            }
        }
    }
}
