using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterface;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CollabVM
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
    class Server
    {

        private Dictionary<string, IVirtualMachineController> MachineContollers;
        private ServerConfig config;

        public Server(Dictionary<string, IVirtualMachineController> controllers, ServerConfig config)
        {
            this.MachineContollers = controllers;
            this.config = config;
            Logger.Log("Server: Initalized");
        }


        public void Start()
        {
            Logger.Log("Server: Starting the server on :" + this.config.port + "...");
            // TODO: do thing and thingy thongy
            //       translation: "make virtual machines from config file"
            this.config.vms["test"] = new VirtualMachine(this.MachineContollers["debug"], "test");
            WebSocketServer ws = new WebSocketServer(this.config.port);
            ws.Log.Level = LogLevel.Error;
            ws.AddWebSocketService("/", () => new WSBehavior(this.config.vms) { Protocol = "cvm2", IgnoreExtensions = true });
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
