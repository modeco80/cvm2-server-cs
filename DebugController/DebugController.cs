using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using PluginInterface;

namespace DebugPlugin
{
    public class DebugController : IVirtualMachineController
    {
        string IVirtualMachineController.Name { get { return "debug"; } }
        string IVirtualMachineController.Description { get { return "Debug VM Controller Plugin"; } }
        string IVirtualMachineController.Author { get { return "kubapolish, modeco80"; } }

        private bool started = false;
        private Bitmap map;
        private Graphics g;

        bool IVirtualMachineController.IsStarted() => started;

        void IVirtualMachineController.Start()
        {
            if (started) return;
            map = new Bitmap(800, 600);
            g = Graphics.FromImage(map);
            var a = g.VisibleClipBounds;
            g.FillRectangle(Brushes.White, a.X, a.Y, a.Width, a.Height);
            OnDisplayUpdate(new DisplayUpdateArgs() { displayData = map, x = 0, y = 0, width = 800, height = 600 });
            started = true;
        }

        void IVirtualMachineController.Stop()
        {
            if (!started) return;
            map?.Dispose();
            g?.Dispose();
            started = false;
        }

        void IVirtualMachineController.Restore()
        {
            ((IVirtualMachineController)this).Stop();
            ((IVirtualMachineController)this).Start();
        }

        Bitmap IVirtualMachineController.GetDisplayBitmap() => map;


        void IVirtualMachineController.ForceDisplayUpdate()
        {
            OnDisplayUpdate(new DisplayUpdateArgs() { displayData = map, x = 0, y = 0, width = 800, height = 600 });
        }

        // Control functions
        void IVirtualMachineController.SendKey(int keysym)
        {
            g.DrawString(keysym.ToString(), new Font(FontFamily.GenericMonospace, 20, FontStyle.Italic | FontStyle.Bold | FontStyle.Underline), Brushes.Red, 50, 50);
            OnDisplayUpdate(new DisplayUpdateArgs() { displayData = map, x = 0, y = 0, width = 800, height = 600 });
        }

        void IVirtualMachineController.SendMouse(int x, int y, int ms)
        {
            g.FillRectangle(Brushes.Black, x - 1, y - 1, 3, 3);
            try
            {
                OnDisplayUpdate(new DisplayUpdateArgs() { displayData = map.Clone(new Rectangle(x - 1, y - 1, 3, 3), map.PixelFormat), x = x - 1, y = y - 1, width = 4, height = 4 });
            }
            catch
            {
                Console.Write("\nlol d'oops!\n");
            }
        }

        protected void OnDisplayUpdate(DisplayUpdateArgs e)
        {
            TestHvDispUpd?.Invoke(this, e);
        }

        event EventHandler<DisplayUpdateArgs> TestHvDispUpd;
        private readonly object event_lock = new object();

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
