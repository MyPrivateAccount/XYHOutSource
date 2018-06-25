using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Managers
{
    public class TokenManager
    {
        class BearerToken
        {
            public string token_type { get; set; }

            public string access_token { get; set; }

            public long expires_in { get; set; }

            public DateTime LastGetTime { get; set; }

            public bool IsExpired()
            {
                if (((DateTime.UtcNow - LastGetTime).TotalSeconds - expires_in) >= -30)
                {
                    return true;
                }
                return false;
            }
        }

        private RestClient restClient = null;
        private string clientId = "";
        private string clientSecret = "";
        private string _url { get; set; }

        private BearerToken token = null;


        private object locker = new object();

        //private ILogger Logger = LoggerManager.GetLogger("TokenManager");

        public TokenManager(string url, string clientId, string clientSecret)
        {
            restClient = new RestClient();
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this._url = url;
        }

        public async Task<string> GetAccessToken()
        {
            if (token == null || token.IsExpired())
            {
                lock (locker)
                {
                    if (token != null && !token.IsExpired())
                    {
                        return token.access_token;
                    }

                    Dictionary<String, string> dic = new Dictionary<string, string>()
                    {
                        {"grant_type","client_credentials" },
                        {"client_id", clientId },
                        {"client_secret", clientSecret }
                    };
                    BearerToken bt = restClient.SubmitForm<BearerToken>(_url, dic).Result;
                    if (bt != null && !String.IsNullOrEmpty(bt.access_token))
                    {
                        bt.LastGetTime = DateTime.UtcNow;
                        //Logger.Info("获取Access Token:{0}\r\n{1}", clientId, bt.access_token);
                    }
                    token = bt;
                }
            }
            return token?.access_token;
        }

        public void ClearToken()
        {
            lock (locker)
            {
                token = null;
            }
        }



        public async Task<TResult> Execute<TResult>(Func<string, Task<TResult>> fun)
        {
            TResult result = default(TResult);
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    string accessToken = await GetAccessToken();
                    if (String.IsNullOrEmpty(accessToken))
                    {
                        continue;
                    }

                    result = await fun(accessToken);
                    break;
                }
                catch (UnauthorizedAccessException e)
                {
                    ClearToken();
                }
                catch (Exception e)
                {
                    throw;
                    //Logger.Error("Execute error {0} \r\n{1}", i, e.ToString());
                }
            }
            return result;
        }

    }
}
