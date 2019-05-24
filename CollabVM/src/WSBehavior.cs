using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CollabVM
{
    // The WebSocket server behavior
    class WSBehavior : WebSocketBehavior
    {
        private List<User> users;
        private Dictionary<string, VirtualMachine> virtualMachines;
        private System.Timers.Timer serverTimer;
        private object VMLock = new object();

        public WSBehavior(Dictionary<string, VirtualMachine> vms)
        {
            users = new List<User>();
            virtualMachines = vms;

            serverTimer = new System.Timers.Timer(150);
            serverTimer.Elapsed += (src, e) =>
            {
                // Start a new thread to work on user action queues
                new Thread(() =>
                {
                    ActionWork();
                }).Start();
            };
            serverTimer.AutoReset = true;
            serverTimer.Enabled = true;

        }

        // Get a user from a WebSocket id.
        private User GetUserFromID(string id)
        {
            return users.Find(x => x.id == id);
        }

        // Right now this function just cleans up *our* user pool.
        // Later we'll need to remove stuff from virtual machines in order
        // to stop the chance of nullrefs
        private void CleanupUser(string id)
        {
            /*
                foreach(KeyValuePair<string,VirtualMachine> v in virtualMachines){
                    VirtualMachine vm = v.Value;
                    if(vm == GetUserFromID(ID).vm) vm.DisconnectUser(GetUserFromID(ID));
                 }
            */
            users.RemoveAll(pool => pool == GetUserFromID(ID));
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Logger.Log($"[ IP {GetUserFromID(ID).ipi.GetIP()} ] Connection closed");
            CleanupUser(ID);
        }

        // This function processes the action queue for all connected users.
        private void ActionWork()
        {
            foreach (User u in users)
            {
                // Process the action queue if it's not blank.
                while (u.ActionQueue.Count != 0)
                {
                    Action act = u.ActionQueue.Dequeue();
                    if (act == null) break;
                    if(act.binaryData == null)
                    {
                        if (act.inst == null) continue;
                        u.sockhandle.Send(ProtocolCodec.Encode(act.inst));
                    } else
                    {
                        u.sockhandle.Send(act.binaryData);
                    }
                    
                }
            }
        }

        private void OnWS(User u, string message)
        {
            string[] decoded = ProtocolCodec.Decode(message);
            switch (decoded[0])
            {
                default:break;
                case "mouse":
                {
                        if (u.vm == null || u.connected == false) break;
                       
                        lock (VMLock)
                        {
                            u.vm.MouseMove(int.Parse(decoded[1]), int.Parse(decoded[2]));
                        }
                } break;
            }

        }

        // Fired on a WebSocket message.
        protected override void OnMessage(MessageEventArgs e)
        {
            User u = GetUserFromID(ID);
            if (u != null)
            {
              OnWS(u, e.Data);
            }
        }

        // Fired when a client first connects.
        protected override void OnOpen()
        {
            // Block non-CollabVM connnections
            if (!Context.SecWebSocketProtocols.Contains("cvm2"))
            {
                Sessions.CloseSession(ID);
                return;
            }


            users.Add(new User(ID, Context.UserEndPoint.Address, Context.WebSocket));
            Logger.Log($"[ IP {GetUserFromID(ID).ipi.GetIP()} ] Connection opened");
            // TODO: we might want to send a hello packet too
            this.virtualMachines["test"].ConnectUser(GetUserFromID(ID));
        }
    }
}
