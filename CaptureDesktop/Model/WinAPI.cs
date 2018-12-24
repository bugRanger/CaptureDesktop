using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Capture
{
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
            //HINT> Unused
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left { get; set; }
                public int top { get; set; }
                public int right { get; set; }
                public int bottom { get; set; }
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
            //HINT< Unused

            [DllImport("user32.dll")]
            public static extern bool GetCursorInfo(out CursorInfo pci);
            [DllImport("user32.dll")]
            public static extern IntPtr CopyIcon(IntPtr hIcon);
            [DllImport("user32.dll")]
            public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);
            [DllImport("user32.dll")]
            public static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        }

        //HINT> Unused
        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        public static class Gdi32
        {
            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }
        //HINT< Unused
    }
}
