using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollabVM2.Plugins;
using WebSocketSharp;
using WebSocketSharp.Server;

using Logger = CollabVM2.Utils.Logger;

namespace CollabVM2.Server
{

    // Server configuration data
    class ServerConfig
    {
        public int port;
        public Dictionary<string, VirtualMachine> vms = new Dictionary<string, VirtualMachine>();

        public ServerConfig(int port)
        {
            this.port = port;
        }
    }

    // The server class.
    class CollabVMServer
    {

        private Dictionary<string, IVirtualMachineController> MachineContollers;
        private ServerConfig config;

        public CollabVMServer(Dictionary<string, IVirtualMachineController> controllers, ServerConfig config)
        {
            this.MachineContollers = controllers;
            this.config = config;
            Logger.Log("Server: Initalized");
        }


        public void Start()
        {
            Logger.Log("Server: Starting the server on :" + this.config.port + "...");

            this.config.vms["test"] = new VirtualMachine(this.MachineContollers["debug"], "test");
           
            ServerGlobals.virtualMachines = this.config.vms;
            WebSocketServer ws = new WebSocketServer(this.config.port);

            ws.Log.Level = LogLevel.Error;
            ws.AddWebSocketService("/", () => new VMServerBehaviour() { Protocol = "cvm2", IgnoreExtensions = true });

            this.config.vms["test"].Start();

            ws.Start();

            while (ws.IsListening)
            {
                // I didn't want to use Console.ReadKey()
                // So this is Betterer I guess
                Thread.Sleep(100);
            }
            ws.Stop();
        }
    }
}
