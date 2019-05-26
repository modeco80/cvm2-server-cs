using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterface;
using System.Reflection;

namespace CollabVM
{
    class Program
    {
        private static Dictionary<string, IVirtualMachineController> vms;

        static void Main(string[] args)
        {
            vms = new Dictionary<string, IVirtualMachineController>();
            LoadPlugins();
            Logger.Log("CollabVM Server 2.0, (C) 2019 Computernewb Development Team.", Logger.Severity.Logo);
            Logger.Log("Initalizing VM Controller plugins...");
            // TODO: load configuration
            Logger.Log("Initalizing server...");
            Server srv = new Server(vms, new ServerConfig(9090)); // TODO
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
                vms.Add(vm.Name, vm);
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
                            Logger.Log("VM Controller Plugin \"" + vm.Description + "\" loaded successfully!");
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
