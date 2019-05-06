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
            WebSocketServer ws = new WebSocketServer(this.config.port);
            ws.Log.Level = LogLevel.Warn;
            WSBehavior wb = new WSBehavior() { Protocol = "cvm2", IgnoreExtensions = true };
            ws.AddWebSocketService("/", () => wb);

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
