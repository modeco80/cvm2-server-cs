using System;
using System.Collections.Generic;
using PluginInterface;

namespace CollabVM
{

    // A repressentation of a virtual machine.
    public class VirtualMachine
    {
        List<User> users; // All users this VM has.
        public string id { get; set; }
        private IVirtualMachineController vmc;

        public VirtualMachine(IVirtualMachineController vmc)
        {
            this.vmc = vmc;
            this.vmc.OnDisplayUpdate += this.OnDisplayUpdate;
        }

        // Starts this VM.
        public void Start()
        {
            Logger.Log("VirtualMachine: Starting VM id " + id);
            vmc.Start();
        }

        // Fired when the IVirtualMachineController sends a new display chunk.
        public void OnDisplayUpdate(object sender, DisplayUpdateArgs e)
        {
            // TODO: 
            Logger.Log("Oops where the fuck did I put the display");
        }
    }

}