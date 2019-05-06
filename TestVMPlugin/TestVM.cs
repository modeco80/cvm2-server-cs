using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using PluginInterface;

namespace TestVMPlugin
{
    public class TestVM : IVirtualMachineController
    {
        string IVirtualMachineController.Name { get { return "testvm"; } }
        string IVirtualMachineController.Description { get { return "Sample Hypervisor Plugin"; } }
        string IVirtualMachineController.Author { get { return "Modeco80 / Computernewb Development Team"; } }

        private bool started = false;

        bool IVirtualMachineController.IsStarted() => started;

        void IVirtualMachineController.Start()
        {
            started = true;
            // TODO: In your plugin, do the logic to start the VM AND hook up the OnDisplayUpdate event.
            // Remove the below sample code.

            // Fire a sample display update to test if events work.
            OnDisplayUpdate(new DisplayUpdateArgs()
            {
                x = 0,
                y = 0,
                width = 10,
                height = 10,
                displayData = new Bitmap(10, 10)
            });
        }

        void IVirtualMachineController.Stop()
        {
            // TODO: In your plugin, do the logic to stop here.
            started = false;
        }

        void IVirtualMachineController.Restore()
        {
            ((IVirtualMachineController)this).Stop();
            // TODO: In your plugin, do the logic to restore here.
            ((IVirtualMachineController)this).Start();
        }

        // Control functions
        void IVirtualMachineController.SendKey(int keysym)
        {

        }

        void IVirtualMachineController.MouseMove(int x, int y)
        {

        }

        protected void OnDisplayUpdate(DisplayUpdateArgs e)
        {
            TestHvDispUpd?.Invoke(this, e);
        }

        event EventHandler<DisplayUpdateArgs> TestHvDispUpd;
        private object event_lock = new object();

        event EventHandler<DisplayUpdateArgs> IVirtualMachineController.DisplayUpdate
        {
            add
            {
                lock (event_lock)
                {
                    TestHvDispUpd += value;
                }
            }

            remove
            {
                lock (event_lock)
                {
                    TestHvDispUpd -= value;
                }
            }
        }
    }
}
