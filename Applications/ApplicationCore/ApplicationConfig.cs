using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class ApplicationConfig
    {
        private string ConfigFilePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Config", "NWorkflowConfig.json");
        //private Sobey.Core.Log.ILogger logger = LoggerManager.GetLogger("NWorkflowConfig");

        public bool Save()
        {
            string json = JsonHelper.ToJson(this);
            try
            {
                System.IO.File.WriteAllText(ConfigFilePath, json, Encoding.UTF8);
            }
            catch (System.Exception e)
            {
                //logger.Error("save config error:\r\n{0}", e.ToString());
                return false;
            }

            return true;
        }

        public void Get()
        {
            ApplicationConfig cfg = null;
            if (System.IO.File.Exists(ConfigFilePath))
            {
                string json = System.IO.File.ReadAllText(ConfigFilePath);
                cfg = JsonHelper.ToObject<ApplicationConfig>(json);

            }
            else
            {
                cfg = GetDefaultConfig();

            }

            if (cfg != null)
            {
                CopyFrom(cfg);
            }
        }

        public void CopyFrom(ApplicationConfig config)
        {
            //if (config != null)
            //{
            //    this.MergeFrom(config);
            //}
            //if (this.WebSocketPort <= 0)
            //{
            //    this.WebSocketPort = 4515;
            //}
            //if (this.ServicePort <= 0)
            //{
            //    this.ServicePort = 7099;
            //}
            //if (this.TaskPingSeconds <= 0)
            //{
            //    this.TaskPingSeconds = 5;
            //}

        }

        public ApplicationConfig GetDefaultConfig()
        {
            ApplicationConfig cfg = new ApplicationConfig()
            {
                //WebSocketPort = 4515,
                //LogLevel = LogLevels.Info,
                //MaxLogFileSize = "10MB",
                //ServicePort = 7099,
                //ClusterName = "Default",
                //SlowLogSeconds = 5,
                //TaskPingSeconds = 5,
                //IsDebug = false,
                //TaskStatusPushInterval = 5,
                //Version = 0
            };
            cfg.Save();

            return cfg;
        }



    }
}
