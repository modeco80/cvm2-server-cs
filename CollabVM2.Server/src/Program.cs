using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollabVM2.Plugins;
using CollabVM2.Utils;
using System.Reflection;

namespace CollabVM2.Server
{
    class Program
    {
        private static Dictionary<string, IVirtualMachineController> vms;

        static void Main(string[] args) 
        {
#if DEBUG
            Console.Title = $"CollabVM2.Server {ThisAssembly.Git.Commit}";
#else
            Console.Title = "CollabVM2.Server";
#endif
            vms = new Dictionary<string, IVirtualMachineController>();
            LoadPlugins();

#if DEBUG
            Logger.Log($"CollabVM2.Server {ThisAssembly.Git.Commit} (C) 2019 Computernewb Development Team.", Logger.Severity.Logo);
#else
            Logger.Log("CollabVM2.Server (C) 2019 Computernewb Development Team.", Logger.Severity.Logo);
#endif
            Logger.Log("Initalizing server...");
            CollabVMServer srv = new CollabVMServer(vms, new ServerConfig(9090)); // TODO
            srv.Start();
        }

        static void LoadPlugins()
        {
            string[] plugins = Directory.GetFiles("plugins/", "*.dll");
            foreach (string plugin in plugins)
            {
                Logger.Log("Attempting to load plugin " + plugin);
                IVirtualMachineController vm = LoadPlugin(plugin);
                if (vm == null)
                {
                    Logger.Log("Error loading plugin " + plugin + ", skipping", Logger.Severity.Error);
                    continue; // error loading that plugin; skip list add
                }
                vms.Add(vm.Id, vm);
            }
        }

        private static bool IsControllerInterface(Type t) => typeof(IVirtualMachineController).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo());

        // Loads a controller plugin.
        public static IVirtualMachineController LoadPlugin(string pluginPath)
        {
            Assembly pluginAssembly;
            try
            {
                pluginAssembly = Assembly.LoadFrom(pluginPath);
                foreach (Type t in pluginAssembly.GetTypes())
                {
                    if (IsControllerInterface(t))
                    {
                        object pluginInstance = Activator.CreateInstance(t);
                        try
                        {
                            IVirtualMachineController vm = (IVirtualMachineController)pluginInstance;
                            Logger.Log("VM Controller Plugin \"" + vm.DescribingName + "\" loaded successfully!");
                            return vm;
                        }
                        catch
                        {
                            Logger.Log("Error attempting to load plugin.", Logger.Severity.Error);
                        }
                    }
                }
            }
            catch { }
            // Return a nullref if there is an error.
            return null;
        }

    }
}