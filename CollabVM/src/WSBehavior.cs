using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CollabVM
{
    class WSBehavior : WebSocketBehavior
    {
        private List<User> users;
        private List<VirtualMachine> virtualMachines;
        private Timer serverTimer;

        public WSBehavior()
        {
            users = new List<User>();
            virtualMachines = new List<VirtualMachine>();

            serverTimer = new Timer(175);
            serverTimer.Elapsed += ActionTimerElapsed;
            serverTimer.AutoReset = true;
            serverTimer.Enabled = true;

        }

        private User GetUserFromID(string id)
        {
            return users.Find(x => x.id == id);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Logger.Log($"[ IP {GetUserFromID(ID).ipi.GetIP()} ] Connection closed");
            users.RemoveAll(pool => pool == GetUserFromID(ID));
            Sessions.CloseSession(ID);
        }

        private void ActionTimerElapsed(Object source, ElapsedEventArgs e)
        {
            foreach (User u in users)
            {
                Logger.Log("hih " + u.id + " " + u.ActionQueue.Count);

                while (u.ActionQueue.Count != 0)
                {
                    Logger.Log("hih shus");
                    Action act = u.ActionQueue.Dequeue();
                    u.sockhandle.Send(ProtocolCodec.Encode(act.inst));
                }
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            User u = GetUserFromID(ID);
            if (u != null)
            {
                /*
                    ProtocolInstruction inst = ProtocolCodec.Decode(e.Data);
                    switch (inst.instruction)
                    {
                        default: break;


                    }
                    */

                Action a = new Action() { inst = new ProtocolInstruction() { instruction = "chat", arguments = { "modeco80", "cbt" } } };
                Logger.Log("enquing");
                u.ActionQueue.Enqueue(a);
                Logger.Log("enqueued " + u.ActionQueue.Peek().inst.instruction);
            }

        }

        protected override void OnOpen()
        {
            // Block non-NVM2 connnections
            if (!Context.SecWebSocketProtocols.Contains("nvm2"))
            {
                Sessions.CloseSession(ID);
                return;
            }


            users.Add(new User(ID, Context.UserEndPoint.Address, Context.WebSocket));
            Context.WebSocket.Send("hi");
            Logger.Log($"[ IP {GetUserFromID(ID).ipi.GetIP()} ] Connection opened");

        }
    }
}
