using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GitHubIP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string hosts = @"C:\Windows\System32\drivers\etc\hosts";
        private const string host = "github.com";
        private const string url = "https://github.com";
        private MainWindowViewModel viewModel = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private async void RefreshIP(object sender, RoutedEventArgs e)
        {
            viewModel.SelectMetaType = null;
            viewModel.MetaTypes = null;
            loading.Visibility = Visibility.Visible;
            string str = await GitIPGet.GetIP();
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            GitHubMeta meta = JsonSerializer.Deserialize<GitHubMeta>(str, options);
            var type = typeof(GitHubMeta);
            var properties = type.GetProperties(BindingFlags.Instance| BindingFlags.Public);
            foreach (var item in properties)
            {
                GitHubMeta.Meta[item.Name] = type.GetProperty(item.Name).GetValue(meta) as List<string>;
            }

            viewModel.MetaTypes = GitHubMeta.Meta.Keys.ToList();
            viewModel.SelectMetaType = "Web";
            loading.Visibility = Visibility.Collapsed;
            pingAll.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, pingAll));
        }

        private async void PingIP(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is IPInfo ipInfo)
            {
                Ping ping = new Ping();
                ipInfo.Ping = true;
                var replay = await ping.SendPingAsync(ipInfo.IP);
                ipInfo.Ping = false;
                ipInfo.PingResult = replay;
            }
        }

        private async void PingAll(object sender, RoutedEventArgs e)
        {
            if (viewModel.IPList != null)
            {
                IEnumerable<Task> tasks = viewModel.IPList.Select(async ipInfo =>
                {
                    Ping ping = new Ping();
                    ipInfo.Ping = true;
                    var replay = await ping.SendPingAsync(ipInfo.IP);
                    ipInfo.Ping = false;
                    ipInfo.PingResult = replay;
                });
                await Task.WhenAll(tasks);
                sort.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, sort));
            }
        }

        private void SortIP(object sender, RoutedEventArgs e)
        {
            if (viewModel.IPList != null)
            {
                viewModel.IPList.Sort((x1, x2) =>
                {
                    if (x1.PingResult != null && x1.PingResult.Status == IPStatus.Success && x2.PingResult != null && x2.PingResult.Status == IPStatus.Success)
                    {
                        return (int)(x1.PingResult.RoundtripTime - x2.PingResult.RoundtripTime);
                    }
                    if (x1.PingResult != null && x1.PingResult.Status == IPStatus.Success && x2.PingResult != null && x2.PingResult.Status != IPStatus.Success)
                    {
                        return int.MinValue;
                    }
                    if (x1.PingResult != null && x1.PingResult.Status != IPStatus.Success && x2.PingResult != null && x2.PingResult.Status == IPStatus.Success)
                    {
                        return int.MaxValue;
                    }
                    return 0;
                });
                viewModel.IPList = viewModel.IPList.ToList();
            }
        }

        private async void SetHosts(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is IPInfo ipInfo)
            {
                ips.SelectedItem = ipInfo;
                var lines = await File.ReadAllLinesAsync(hosts);
                var list = lines.ToList();
                bool find = false;
                string item = $"{ipInfo.IP} {host}";
                foreach (var line in list)
                {
                    string s = line.Trim();
                    var split = s.Split(' ');
                    if (split.Length == 2 && split[1] == host)
                    {
                        find = true;
                        list[list.IndexOf(line)] = item;
                        break;
                    }
                }
                if (!find)
                {
                    list.Add(item);
                }
                await File.WriteAllLinesAsync(hosts, list);
                Process.Start("explorer.exe", url);
            }
        }

        private async void Revert(object sender, RoutedEventArgs e)
        {
            var lines = await File.ReadAllLinesAsync(hosts);
            var list = lines.ToList();
            bool find = false;
            foreach (var line in list)
            {
                string s = line.Trim();
                var split = s.Split(' ');
                if (split.Length == 2 && split[1] == host)
                {
                    find = true;
                    list.Remove(line);
                    break;
                }
            }
            if (find)
            {
                await File.WriteAllLinesAsync(hosts, list);
            }
            Process.Start("explorer.exe", url);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            refresh.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, refresh));
        }
    }
}
