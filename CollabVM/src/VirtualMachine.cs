using System;
using System.Collections.Generic;
using PluginInterface;

namespace CollabVM
{



    public class VirtualMachine
    {
        public string id { get; set; }
        private IVirtualMachineController vmc;

        public VirtualMachine(IVirtualMachineController vmc)
        {
            this.vmc = vmc;
            this.vmc.OnDisplayUpdate += this.OnDisplayUpdate;
        }

        public void Start()
        {
            Logger.Log("VirtualMachine: Starting VM id " + id);
            vmc.Start();
        }

        public void OnDisplayUpdate(object sender, DisplayUpdateArgs e)
        {
            // TODO: 
            Logger.Log("Oops where the fuck did I put the display");
        }
    }

}