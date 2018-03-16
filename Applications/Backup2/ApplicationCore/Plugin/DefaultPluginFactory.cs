using ApplicationCore.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Plugin

{
    public class DefaultPluginFactory : IPluginFactory
    {
        //protected ILogger Logger = LoggerManager.GetLogger("Plugin");

        protected class PluginInfo : PluginItem
        {
            public Type Type { get; set; }

            public IPlugin Instance { get; set; }


            public void SetInitFail(bool fail)
            {
                this.InitFail = fail;
            }
            public void SetStartFail(bool fail)
            {
                this.StartFail = fail;
            }
            public void SetRunning(bool run)
            {
                this.IsRunning = run;
            }
            public void SetMessage(string msg)
            {
                this.Message = msg;
            }
        }

        protected List<PluginInfo> PluginList = new List<PluginInfo>();

        public List<Assembly> LoadedAssembly { get; }

        public DefaultPluginFactory()
        {
            LoadedAssembly = new List<Assembly>();
        }

        public IPlugin GetPlugin(string pluginId)
        {
            PluginInfo pi = PluginList.FirstOrDefault(x => x.ID == pluginId);
            if (pi == null)
                return null;

            return pi.Instance;

        }



        public void Load(string pluginPath)
        {
            PluginList.Clear();
            if (!Directory.Exists(pluginPath))
            {
                return;
            }

            DirectoryLoader dl = new DirectoryLoader();
            var al = dl.LoadFromDirectory(pluginPath);

            al.ForEach(a =>
            {
                LoadedAssembly.Add(a);
                var tl = dl.GetTypes(a, typeof(IPlugin), (t) =>
                {
                    if (t.GetTypeInfo().IsAbstract)
                    {
                        return false;
                    }
                    if (t.GetTypeInfo().IsPublic)
                    {
                        return true;
                    }
                    return false;
                });



                tl.ForEach(t =>
                {
                    try
                    {
                        IPlugin p = Activator.CreateInstance(t) as IPlugin;
                        PluginInfo pi = new PluginInfo()
                        {
                            Description = p.Description,
                            Order = p.Order,
                            ID = p.PluginID,
                            Type = t,
                            Instance = p,
                            Name = p.PluginName
                        };

                        if (typeof(IPluginConfig).GetTypeInfo().IsAssignableFrom(p.GetType()))
                        {
                            pi.HasConfig = true;
                        }

                        PluginList.Add(pi);
                    }
                    catch (System.Exception e)
                    {
                        //Logger.Error("can not load plugin type: {0}\r\n{1}", t.FullName, e.ToString());
                    }
                });
            });

            if (PluginList.Count > 0)
            {
                var orderList = PluginList.OrderBy(x => x.Order).ToList();
                PluginList.Clear();
                PluginList.AddRange(orderList);
            }

        }

        public PluginItem GetPluginInfo(string pluginId, bool secret = false)
        {
            PluginInfo pi = PluginList.FirstOrDefault(x => x.ID == pluginId);
            if (pi != null)
            {
                PluginItem p = new PluginItem();
                p.CopyFrom(pi, secret);
                return p;
            }

            return null;
        }

        public List<PluginItem> GetPluginList(bool secret = false)
        {
            List<PluginItem> list = PluginList.Select(p =>
            {
                var pi = new PluginItem();
                pi.CopyFrom(p, secret);
                return pi;
            }).ToList();

            return list;
        }
        public async Task<bool> Init(ApplicationContext context)
        {
            foreach (PluginInfo pi in PluginList)
            {
                try
                {
                    var r = await pi.Instance.Init(context);
                    pi.SetInitFail(r.Code != "0");
                    pi.SetMessage(r.Message);
                    //Logger.Debug("plugin {0} init {1} {2}", pi.Name, r.Code, r.Message ?? "");
                }
                catch (Exception e)
                {
                    //Logger.Error("plugin {0} init error \r\n{1}", pi.Name, e.ToString());
                    pi.SetInitFail(true);
                    pi.SetMessage(e.ToString());
                }
            }
            return true;
        }

        public async Task<bool> Start(ApplicationContext context)
        {
            foreach (PluginInfo pi in PluginList)
            {
                if (pi.InitFail)
                    continue;
                try
                {
                    var r = await pi.Instance.Start(context);
                    pi.SetStartFail(r.Code != "0");
                    pi.SetMessage(r.Message);
                    //Logger.Debug("plugin {0} start {1} {2}", pi.Name, r.Code, r.Message ?? "");
                    if (r.IsSuccess())
                    {
                        pi.SetRunning(true);
                    }
                }
                catch (Exception e)
                {
                    //Logger.Error("plugin {0} start error \r\n{1}", pi.Name, e.ToString());
                    pi.SetStartFail(true);
                    pi.SetMessage(e.ToString());
                }
            }
            return true;
        }

        public async Task<bool> Stop(ApplicationContext context)
        {
            foreach (PluginInfo pi in PluginList)
            {
                //初始化失败 不调用停止，但如果初始化成功，后面Start失败，依然会调用Stop
                if (pi.InitFail)
                    continue;
                try
                {
                    var r = await pi.Instance.Stop(context);
                    pi.SetMessage(r.Message);
                    //Logger.Debug("plugin {0} stop {1} {2}", pi.Name, r.Code, r.Message ?? "");
                    if (r.IsSuccess())
                    {
                        pi.SetRunning(false);
                    }
                }
                catch (Exception e)
                {
                    //Logger.Error("plugin {0} stop error \r\n{1}", pi.Name, e.ToString());
                    pi.SetMessage(e.ToString());
                }
            }
            return true;
        }


    }
}
