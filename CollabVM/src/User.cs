using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace CollabVM
{
    enum VoteChoices
    {
        kNotDecided,
        kNo,
        kYes
    }

    // IP information for each user
    class IPInfo
    {
        private IPAddress ip;
        public VoteChoices vote = VoteChoices.kNotDecided;

        public IPInfo(IPAddress ip)
        {
            this.ip = ip;
        }

        public string GetIP() => this.ip.ToString();
    }

    class Action
    {
        public ProtocolInstruction inst;
    }

    // User object.
    class User
    {
        public IPInfo ipi;
        public WebSocket sockhandle;
        public string id;
        private string username;
        public bool controlling = false;
        public VirtualMachine Vm { get; set; }

        public Queue<Action> ActionQueue = new Queue<Action>();


        public User(string id, IPAddress ip, WebSocket sock)
        {
            ipi = new IPInfo(ip);
            this.id = id;
            sockhandle = sock;

        }

        // I know properties are a thing but fuck you Harold
        // I can do what I damn well please
        public void SetUsername(string username) => this.username = username;
        public string GetUsername => username;
    }
}
