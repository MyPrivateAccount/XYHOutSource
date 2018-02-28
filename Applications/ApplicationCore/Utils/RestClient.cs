using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore
{
    public class RestClient
    {
        //ILogger Logger = LoggerManager.GetLogger("ApiClient");

        private static HttpClient _httpClient = null;

        public RestClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Connection.Clear();
            _httpClient.DefaultRequestHeaders.ConnectionClose = false;
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<TResponse> Post<TResponse>(string url, object body, string method = "POST", NameValueCollection queryString = null, int timeout = 60)
            where TResponse : class, new()
        {
            TResponse response = null;
            try
            {
                string json = JsonHelper.ToJson(body);
                HttpClient client = _httpClient;
                if (queryString == null)
                {
                    queryString = new NameValueCollection();
                }
                if (String.IsNullOrEmpty(method))
                {
                    method = "POST";
                }
                url = CreateUrl(url, queryString);
                //Logger.Debug("请求：{0} {1}", method, url);
                byte[] strData = Encoding.UTF8.GetBytes(json);
                MemoryStream ms = new MemoryStream(strData);
                StreamContent sc = new StreamContent(ms);
                sc.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                var res = await client.PostAsync(url, sc);
                byte[] rData = await res.Content.ReadAsByteArrayAsync();
                string rJson = Encoding.UTF8.GetString(rData);
                //Logger.Debug("应答：\r\n{0}", rJson);
                response = JsonHelper.ToObject<TResponse>(rJson);
                return response;
            }
            catch (System.Exception e)
            {
                TResponse r = new TResponse();
                //Logger.Error("请求异常：\r\n{0}", e.ToString());
                throw;
            }
        }

        public async Task<string> Post(string url, string body, string method = "POST", NameValueCollection queryString = null, int timeout = 60)
        {
            string response = null;
            try
            {
                string json = body;
                HttpClient client = _httpClient;
                if (queryString == null)
                {
                    queryString = new NameValueCollection();
                }
                if (String.IsNullOrEmpty(method))
                {
                    method = "POST";
                }
                url = CreateUrl(url, queryString);
                //Logger.Debug("请求：{0} {1}", method, url);
                byte[] strData = Encoding.UTF8.GetBytes(json);
                MemoryStream ms = new MemoryStream(strData);
                StreamContent sc = new StreamContent(ms);
                sc.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                var res = await client.PostAsync(url, sc);
                if (res.Content == null || res.Content.Headers.ContentLength == 0)
                {
                    response = "";
                }
                else
                {
                    byte[] rData = await res.Content.ReadAsByteArrayAsync();
                    string rJson = Encoding.UTF8.GetString(rData);
                    //Logger.Debug("应答：\r\n{0}", rJson);
                    response = rJson;
                }
            }
            catch (System.Exception e)
            {
                response = "ERROR";
                //Logger.Error("请求异常：\r\n{0}", e.ToString());
            }
            return response;
        }

        public async Task<TResponse> Get<TResponse>(string url, NameValueCollection queryString)
                    where TResponse : class, new()
        {
            TResponse response = null;
            try
            {
                HttpClient client = _httpClient;
                if (queryString == null)
                {
                    queryString = new NameValueCollection();
                }
                url = CreateUrl(url, queryString);
                //Logger.Debug("请求：{0} {1}", "GET", url);
                byte[] rData = await client.GetByteArrayAsync(url);
                string rJson = Encoding.UTF8.GetString(rData);
                //Logger.Debug("应答：\r\n{0}", rJson);
                response = JsonHelper.ToObject<TResponse>(rJson);
            }
            catch (System.Exception e)
            {
                TResponse r = new TResponse();
                //Logger.Error("请求异常：\r\n{0}", e.ToString());
                return r;
            }
            return response;
        }


        public async Task<string> Post(string url, object body, string method, NameValueCollection queryString)
        {
            string response = null;
            try
            {
                string json = JsonHelper.ToJson(body);
                HttpClient client = _httpClient;
                if (queryString == null)
                {
                    queryString = new NameValueCollection();
                }

                url = CreateUrl(url, queryString);
                if (String.IsNullOrEmpty(method))
                {
                    method = "POST";
                }
                //Logger.Debug("请求：{0} {1}", method, url);
                byte[] strData = Encoding.UTF8.GetBytes(json);
                MemoryStream ms = new MemoryStream(strData);
                StreamContent sc = new StreamContent(ms);
                sc.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                var res = await client.PostAsync(url, sc);
                byte[] rData = await res.Content.ReadAsByteArrayAsync();
                string rJson = Encoding.UTF8.GetString(rData);
                //Logger.Debug("应答：\r\n{0}", rJson);
                response = rJson;
                return response;
            }
            catch (System.Exception e)
            {
                //Logger.Error("请求异常：\r\n{0}", e.ToString());
                throw;
            }
        }


        public static string CreateUrl(string url, NameValueCollection qs)
        {
            if (qs != null && qs.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                List<string> kl = qs.AllKeys.ToList();
                foreach (string k in kl)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append("&");
                    }
                    sb.Append(k).Append("=");
                    if (!String.IsNullOrEmpty(qs[k]))
                    {

                        sb.Append(System.Net.WebUtility.UrlEncode(qs[k]));
                    }
                }


                if (url.Contains("?"))
                {
                    url = url + "&" + sb.ToString();
                }
                else
                {
                    url = url + "?" + sb.ToString();
                }
            }

            return url;

        }


    }
}
