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
            #region Logo stuff
#if DEBUG
            Console.Title = $"CollabVM2.Server {ThisAssembly.Git.Commit}";
#else
            Console.Title = "CollabVM2.Server";
#endif

#if DEBUG
            Logger.Log($"CollabVM2.Server {ThisAssembly.Git.Commit} (C) 2019 Computernewb Development Team.", Logger.Severity.Logo);
#else
            Logger.Log("CollabVM2.Server (C) 2019 Computernewb Development Team.", Logger.Severity.Logo);
#endif
            #endregion
            vms = new Dictionary<string, IVirtualMachineController>();
            LoadAllControllers();
            Logger.Log("Initalizing server...");
            CollabVMServer srv = new CollabVMServer(vms, new ServerConfig(9090)); // TODO
            srv.Start();
        }

        static void LoadAllControllers()
        {
            string[] controllers = Directory.GetFiles("controllers/", "*.dll");
            foreach (string controller in controllers)
            {
                Logger.Log("Attempting to load controller " + controller);
                IVirtualMachineController vm = LoadController(controller);
                if (vm == null)
                {
                    continue; // error loading that plugin; skip list add
                }
                vms.Add(vm.Id, vm);
            }
        }

        #region Plugin stuff

        // Loads a controller plugin.
        public static IVirtualMachineController LoadController(string ControllerAssemblyPath)
        {
            Assembly ControllerAssembly;
            IVirtualMachineController vm = null;

            try
            {
                ControllerAssembly = Assembly.LoadFrom(ControllerAssemblyPath);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error loading controller {ControllerAssemblyPath}", Logger.Severity.Error);
                Logger.Log($"Exception message: {ex.Message}", Logger.Severity.Error);
                return null;
            }

            foreach (Type AssemblyType in ControllerAssembly.GetTypes())
            {
                if (typeof(IVirtualMachineController).GetTypeInfo().IsAssignableFrom(AssemblyType.GetTypeInfo()))
                {
                    object ControllerInstance = Activator.CreateInstance(AssemblyType);
                    try
                    {
                        vm = (IVirtualMachineController)ControllerInstance;
                        Logger.Log("VM Controller \"" + vm.DescribingName + "\" loaded successfully!");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error loading controller {ControllerAssemblyPath}", Logger.Severity.Error);
                        Logger.Log($"Exception message: {ex.Message}", Logger.Severity.Error);
                        return null;
                    }
                    return vm;
                }
            }
            
            // Return a nullref if there is an error.
            return null;
        }
        #endregion
    }
}
