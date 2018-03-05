using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;

namespace AuthorizationCenter.Helpers
{
    public class RestClient
    {

        internal static HttpClient httpClient;

        public string BaseUrl { get; private set; }

        private ILogger logger = LoggerManager.GetLogger("RestClient");

        static RestClient()
        {
            httpClient = new HttpClient();
        }
        public RestClient(string url)
        {
            this.BaseUrl = url;
            this.BaseUrl = this.BaseUrl.TrimEnd('/');
        }


        public async Task<TResult> Get<TResult>(string url)
        {

            string apiUrl = $"{BaseUrl}{url}";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException("验证失败");
            }
            response.EnsureSuccessStatusCode();
            string str = await response.Content.ReadAsStringAsync();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(str);
            
        }

        public async Task<TResult> Post<TResult>(string url, object body, string method = "Post")
        {

            string apiUrl = $"{BaseUrl}{url}";

            HttpMethod hm = new HttpMethod(method);

            var request = new HttpRequestMessage(hm, apiUrl);
           
          
            string json = "";
            if(body!=null)
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            }
            request.Content = new StringContent(json);
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException("验证失败");
            }
            try
            {
                response.EnsureSuccessStatusCode();
                string str = await response.Content.ReadAsStringAsync();

                if(typeof(TResult) == typeof(String))
                {
                    return (TResult)(object)str;
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(str);
            }catch(Exception e)
            {
                logger.Error("Post 失败：{0}\r\n{1}", url, e.ToString());
                string str = await response.Content.ReadAsStringAsync();
                logger.Error(str);
                throw;
            }
        }
        
    }
}
