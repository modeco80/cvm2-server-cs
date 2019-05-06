using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace PluginInterface
{
    public class DisplayUpdateArgs
    {
        // The X/Y of chunk
        public int x;
        public int y;

        // Width and height of chunk.
        public int width;
        public int height;

        // Chunk's bitmap data
        public Bitmap displayData;
    }

    public interface IVirtualMachineController
    {
        // Human identifiable data
        string Name { get; } // Ex.: QEMU
        string Description { get; } // Ex.: QEMU Virtual Machine Plugin
        string Author { get; } //Ex.: modeco80


        void Start();
        bool IsStarted();
        void Stop();
        void Restore();

        void SendKey(int keysym);
        void MouseMove(int x, int y);

        event EventHandler<DisplayUpdateArgs> DisplayUpdate;
    }
}
