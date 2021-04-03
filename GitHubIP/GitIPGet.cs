using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GitHubIP
{
    public static class GitIPGet
    {
        public async static Task<string> GetIP()
        {
            string uri = "https://api.github.com/meta";
            HttpClient client = new HttpClient();
            ProductInfoHeaderValue ua1 = ProductInfoHeaderValue.Parse("Mozilla/5.0");
            ProductInfoHeaderValue ua2 = ProductInfoHeaderValue.Parse("(Windows NT 10.0; Win64; x64; rv:87.0)");
            ProductInfoHeaderValue ua3 = ProductInfoHeaderValue.Parse("Gecko/20100101");
            ProductInfoHeaderValue ua4 = ProductInfoHeaderValue.Parse("Firefox/87.0");
            client.DefaultRequestHeaders.UserAgent.Add(ua1);
            client.DefaultRequestHeaders.UserAgent.Add(ua2);
            client.DefaultRequestHeaders.UserAgent.Add(ua3);
            client.DefaultRequestHeaders.UserAgent.Add(ua4);
            HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
