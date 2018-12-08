using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CaptureDesktop.WinApi
{
    public static class WinApi
    {
        public static class User32
        {
            const Int32 CURSOR_SHOWING = 0x00000001;
            const Int32 CURSOR_ARROW = 0x00010003;
            const Int32 CURSOR_EDIT = 0x00010005;
            const Int32 CURSOR_REVERSE = 0x000803ae;

            [StructLayout(LayoutKind.Sequential)]
            struct CursorInfo
            {
                public Int32 cbSize;
                public Int32 flags;
                public IntPtr hCursor;
                public PointApi ptScreenPos;
            }
            [StructLayout(LayoutKind.Sequential)]
            struct PointApi
            {
                public int x;
                public int y;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct IconInfo
            {
                public bool fIcon;
                public Int32 xHotspot;
                public Int32 yHotspot;
                public IntPtr hbmMask;
                public IntPtr hbmColor;
            }

            [DllImport("user32.dll")]
            static extern bool GetCursorInfo(out CursorInfo pci);
            [DllImport("user32.dll")]
            public static extern IntPtr CopyIcon(IntPtr hIcon);
            [DllImport("user32.dll")]
            public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);
            [DllImport("user32.dll")]
            static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        }
        
    }
}
