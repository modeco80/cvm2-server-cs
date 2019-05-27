using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using CollabVM2.Plugins;

namespace CollabVM2.Server
{

    // A repressentation of a virtual machine.
    public class VirtualMachine
    {
        List<User> users = new List<User>(); // All users this VM has.
        public string id { get; set; }
        public IVirtualMachineController vmc;

        public VirtualMachine(IVirtualMachineController vmc, string id)
        {
            this.id = id;
            this.vmc = vmc;
            this.vmc.DisplayUpdate += this.OnDisplayUpdate;
        }

        private User GetUserFromName(string name)
        {
            return users.Find(x => x.Username == name);
        }

        private User GetUserFromUser(User u)
        {
            return users.Find(x => x == u);
        }

        // Starts this VM.
        public void Start()
        {
            Utils.Logger.Log("VirtualMachine: Starting VM id " + id);
            vmc.Start();
        }

        public void Stop()
        {
            Utils.Logger.Log("VirtualMachine: Stopping VM id " + id);
            vmc.Stop();
        }

        public void Reset()
        {
            Utils.Logger.Log("VirtualMachine: Resetting VM id " + id);
            vmc.Restore();
        }

        public void MouseMove(int x, int y)
        {
            vmc.SendMouse(x, y, 0);
        }

        public void ConnectUser(User u)
        {
            u.connected = true;
            u.vm = this;
            users.Add(u);
            vmc.ForceDisplayUpdate();
        }

        public void DisconnectUser(User u)
        {
            u.connected = false;
            u.vm = null;
            users.RemoveAll(x => x == u);
        }

        private static MemoryStream EncodeBitmapToPNG(Bitmap bm)
        {
            MemoryStream ms = new MemoryStream();
            Image im = bm;
            im.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return ms;
        }

        // Fired when the IVirtualMachineController sends a new display chunk.
        public void OnDisplayUpdate(object sender, DisplayUpdateArgs e)
        {
            foreach (User u in users)
            {
                if (!u.connected) continue; // FUCK
                MemoryStream png = EncodeBitmapToPNG(e.displayData);

                u.ActionQueue.Enqueue(new Action
                {
                    inst = new string[] {
                    "rect", e.x.ToString(), e.y.ToString(), e.width.ToString(), e.height.ToString()
                }
                });

                u.ActionQueue.Enqueue(new Action { binaryData = png.ToArray() });

                png.Dispose();
            }
        }
    }

}