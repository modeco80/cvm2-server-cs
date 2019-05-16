using System;
using System.Collections.Generic;
using PluginInterface;

namespace CollabVM
{

    // A repressentation of a virtual machine.
    public class VirtualMachine
    {
        List<User> users = new List<User>(); // All users this VM has.
        public string id { get; set; }
        private IVirtualMachineController vmc;

        public VirtualMachine(IVirtualMachineController vmc)
        {
            this.vmc = vmc;
            this.vmc.DisplayUpdate += this.OnDisplayUpdate;
        }

        private User GetUserFromName(string name)
        {
            return users.Find(x => x.username == name);
        }

        private User GetUserFromUser(User u)
        {
            return users.Find(x => x == u);
        }

        // Starts this VM.
        public void Start()
        {
            Logger.Log("VirtualMachine: Starting VM id " + id);
            vmc.Start();
        }

        public void Stop()
        {
            Logger.Log("VirtualMachine: Stopping VM id " + id);
            vmc.Stop();
        }

        public void Reset()
        {
            Logger.Log("VirtualMachine: Resetting VM id " + id);
            vmc.Restore();
        }

        public void ConnectUser(User u)
        {
            u.connected = true;
            u.vm = this;
            users.Add(u);
        }

        public void DisconnectUser(User u)
        {
            u.connected = false;
            u.vm = null;
            users.RemoveAll(x => x == u);
        }

        // Fired when the IVirtualMachineController sends a new display chunk.
        public void OnDisplayUpdate(object sender, DisplayUpdateArgs e)
        {
            foreach (User u in users)
            {
                if (!u.connected) continue; // FUCK

            }
        }
    }

}