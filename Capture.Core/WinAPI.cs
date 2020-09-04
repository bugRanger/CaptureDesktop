namespace Capture.Core
{
    using System;
    using System.Runtime.InteropServices;

    public static class WinApi
    {
        public static class User32
        {
            public const Int32 CURSOR_SHOWING = 0x00000001;
            public const Int32 CURSOR_ARROW = 0x00010003;
            public const Int32 CURSOR_EDIT = 0x00010005;
            public const Int32 CURSOR_REVERSE = 0x000803ae;

            [StructLayout(LayoutKind.Sequential)]
            public struct CursorInfo
            {
                public Int32 cbSize { get; set; }
                public Int32 flags { get; set; }
                public IntPtr hCursor { get; set; }
                public PointApi ptScreenPos { get; set; }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct PointApi
            {
                public int x { get; set; }
                public int y { get; set; }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct IconInfo
            {
                public bool fIcon { get; set; }
                public Int32 xHotspot { get; set; }
                public Int32 yHotspot { get; set; }
                public IntPtr hbmMask { get; set; }
                public IntPtr hbmColor { get; set; }
            }

            [DllImport("user32.dll")]
            public static extern bool GetCursorInfo(out CursorInfo pci);
            [DllImport("user32.dll")]
            public static extern IntPtr CopyIcon(IntPtr hIcon);
            [DllImport("user32.dll")]
            public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);
            [DllImport("user32.dll")]
            public static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);
        }
    }
}
