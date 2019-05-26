using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CollabVM
{

    public class ServerGlobals
    {
        public static List<User> users = new List<User>();
        public static Dictionary<string, VirtualMachine> virtualMachines;
        
        // Get a user from a WebSocket id.
        public static User GetUserFromID(string id)
        {
            return users.Find(x => x.id == id);
        }
    }

    // The WebSocket server behavior
    class VMServerBehaviour : WebSocketBehavior
    {
        
        private System.Timers.Timer serverTimer;
        private object VMLock = new object();

        public VMServerBehaviour()
        {
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

        private void CleanupUser(string id)
        {
           
           foreach(VirtualMachine vm in ServerGlobals.virtualMachines.Values)
           {
              if(vm.id == ServerGlobals.GetUserFromID(ID).vm.id) vm.DisconnectUser(ServerGlobals.GetUserFromID(ID));
           }

           ServerGlobals.users.RemoveAll(pool => pool == ServerGlobals.GetUserFromID(ID));
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Logger.Log(ServerGlobals.GetUserFromID(ID), "Connection closed");
            CleanupUser(ID);
        }

        // This function processes the action queue for all connected users when they have one.
        private void ActionWork()
        {
            foreach (User u in ServerGlobals.users)
            {
                // Process the action queue if it's not blank.
                while (u.ActionQueue.Count != 0)
                {
                    Action act = u.ActionQueue.Dequeue();
                    if (act == null) break;
                    if(act.binaryData == null)
                    {
                        if (act.inst == null) continue;
                        u.Socket.Send(ProtocolCodec.Encode(act.inst));
                    } else
                    {
                        u.Socket.Send(act.binaryData);
                    }
                    
                }
            }
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void OnWS(User u, string message)
        {
            string[] decoded = ProtocolCodec.Decode(message);
            if (decoded == null) return;

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

                case "connect":
                    lock (VMLock)
                    {
                        if (!u.connected)
                        {
                            // Bad but unless you have 1000 vms it should be alright.
                            foreach(string id in ServerGlobals.virtualMachines.Keys)
                            {
                                if(decoded[1] == id)
                                {
                                    Logger.Log(ServerGlobals.GetUserFromID(ID), "Connecting to VM " + id);
                                    ServerGlobals.virtualMachines[ decoded[1] ].ConnectUser(ServerGlobals.GetUserFromID(ID));
                                }
                            }
                            
                        }
                    }
                break;
#if false
                case "list":
                    if (!u.connected)
                    {
                        List<string> vmids = new List<string>();
                        foreach(string id in ServerGlobals.virtualMachines.Keys)
                        {
                            vmids.Add(id);
                            Bitmap scalene_triangle = ServerGlobals.virtualMachines[id].vmc.GetDisplayBitmap();
                            Bitmap scaled = ResizeImage(scalene_triangle, 160, 160);
                            //vmids.Add();
                            
                        }
                        u.ActionQueue.Enqueue(new Action { inst = vmids.ToArray() });
                    }
                break;
#endif
            }

        }

        // Fired on a WebSocket message.
        protected override void OnMessage(MessageEventArgs e)
        {
            User u = ServerGlobals.GetUserFromID(ID);
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

            ServerGlobals.users.Add(new User(ID, Context.UserEndPoint.Address, Context.WebSocket));
            Logger.Log(ServerGlobals.GetUserFromID(ID), "Connection opened");
        }
    }
}
