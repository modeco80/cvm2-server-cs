using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace CollabVM2.Plugins
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

    public enum PanelItemType
    {
       Boolean,
       TextInput
    }

    public class PanelItem
    {
        public PanelItemType type;
        public string description;

        public bool bval;
        public string sval;
    }

    public enum MouseStates
    {
        Nop,
        LeftClick = 1,
        RightClick = 2,
        MiddleClick = 4,
        WheelUp = 8,
        WheelDn = 16
    }

    public interface IVirtualMachineController
    {
        // Human identifiable data
        string Id { get; } // Ex.: QEMU
        string DescribingName { get; } // Ex.: QEMU Virtual Machine Plugin
        string Author { get; } //Ex.: modeco80
        //Dictionary<string, PanelItem> PanelItems { get; }

        void Start();
        bool IsStarted();
        void Stop();
        void Restore();

        Bitmap GetDisplayBitmap();
        void ForceDisplayUpdate();
        void SendKey(int keysym);
        void SendMouse(int x, int y, int ms);

        event EventHandler<DisplayUpdateArgs> DisplayUpdate;

        
    }
}
