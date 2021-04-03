using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows.Data;

namespace GitHubIP.Converter
{

    public class PingReplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2)
            {
                if (values[0] is true)
                {
                    return "正在Ping...";
                }
                else
                {
                    if (values[1] is PingReply pingReply)
                    {
                        if (pingReply.Status == IPStatus.Success)
                        {
                            return $"{pingReply.RoundtripTime} ms";
                        }
                        return pingReply.Status.ToString();
                    }
                    return "待Ping";
                }
            }
            return "Error";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
