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
    // The possible choices for a vote.
    public enum VoteChoices
    {
        NotDecided,
        No,
        Yes
    }

    // IP information for each user
    public class IPInfo
    {
        private IPAddress ip;
        public VoteChoices vote = VoteChoices.NotDecided;

        public IPInfo(IPAddress ip)
        {
            this.ip = ip;
        }

        public string GetIP() => this.ip.ToString();
    }

    // Action data
    public class Action
    {
        public string[] inst;

        public byte[] binaryData;
    }

    // User data, contains socket handle and virtual machine user is looking at.
    public class User
    {
        public IPInfo ipi;
        public WebSocket sockhandle;
        public string id;
        public string username = "";

        public bool connected = false;
        public bool freshConnection = true;

        // If connected is false this should always be expected to be null.
        // Otherwise the VM the user is on is here.
        public VirtualMachine vm = null;

        // Queue of actions this user has.
        public Queue<Action> ActionQueue = new Queue<Action>();


        public User(string id, IPAddress ip, WebSocket sock)
        {
            ipi = new IPInfo(ip);
            this.id = id;
            sockhandle = sock;

        }

    }
}
