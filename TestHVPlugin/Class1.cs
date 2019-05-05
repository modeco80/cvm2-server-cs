using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterface;

namespace TestHVPlugin
{
   public class TestHV : IVirtualMachineContoller
    {
        string IVirtualMachineContoller.Name { get { return "TestHV";  } }
        string IVirtualMachineContoller.Description { get { return "Test Hypervisor Plugin"; } }
        string IVirtualMachineContoller.Author { get { return "Mode man co80 man"; } }

    }
}
