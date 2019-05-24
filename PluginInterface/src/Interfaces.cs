﻿using System;
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

    public class ConfigInfo
    {
        #region vnc 
        public string VNCAddress = "";
        public string VNCPasswd = "";
        #endregion
        // TODO: handle other thingys
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
        string Name { get; } // Ex.: QEMU
        string Description { get; } // Ex.: QEMU Virtual Machine Plugin
        string Author { get; } //Ex.: modeco80


        void Start();
        bool IsStarted();
        void Stop();
        void Restore();

        void ForceDisplayUpdate();
        void SendKey(int keysym);
        void SendMouse(int x, int y, int ms);

        event EventHandler<DisplayUpdateArgs> DisplayUpdate;
    }
}
