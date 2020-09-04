namespace Capture.Core.Objects
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Capture;

    using Core;

    /// <summary>
    /// Захват курсора.
    /// </summary>
    public class CursorCapture : ICaptureInfObject
    {
        /// <summary>
        /// От рисовка курсора.
        /// </summary>
        /// <param name="graphics">Изображение</param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void Draw(Graphics graphics, int left, int top)
        {
            //Получение информации о курсоре.
            WinApi.User32.CursorInfo pci = new WinApi.User32.CursorInfo();
            pci.cbSize = Marshal.SizeOf(typeof(WinApi.User32.CursorInfo));
            //Извлечение информации И сверка с флагом наличия отображения курсора.
            if (WinApi.User32.GetCursorInfo(out pci) 
                && pci.flags == WinApi.User32.CURSOR_SHOWING)
            {
                //Смещение при обрезке кадра.
                int x = pci.ptScreenPos.x - left;
                int y = pci.ptScreenPos.y - top;
                int rX = 0;
                int rY = 0;
                //Получаем информацию о иконке курсора для корректировки смещения.
                WinApi.User32.IconInfo iconInfo;
                IntPtr hicon = WinApi.User32.CopyIcon(pci.hCursor);
                if (hicon != IntPtr.Zero && WinApi.User32.GetIconInfo(hicon, out iconInfo))
                {
                    //Смещаем курсор относительно размерности иконки.
                    x -= iconInfo.xHotspot;
                    y -= iconInfo.yHotspot;
                    //Смещаем маркер.
                    rX += iconInfo.xHotspot;
                    rY += iconInfo.yHotspot;
                }
                //Задаем настройки маркера.
                Color c = Color.WhiteSmoke;
                float width = 3;
                int radius = 30;
                //При зажатия клавиш.
                if ((Control.MouseButtons & MouseButtons.Left) != 0 || (Control.MouseButtons & MouseButtons.Right) != 0)
                {
                    //Задаем настройки маркера.
                    c = ((Control.MouseButtons & MouseButtons.Right) != 0) ? Color.OrangeRed : Color.YellowGreen;
                    width = 5;
                    radius = 40;
                }
                //Прорисовка маркера.
                var background = new Pen(Color.Black, width + 1.5F);
                graphics.DrawEllipse(background, (x - radius / 2) + rX, (y - radius / 2) + rY, radius, radius);
                var marker = new Pen(c, width);
                graphics.DrawEllipse(marker, (x - radius / 2) + rX, (y - radius / 2) + rY, radius, radius);
                try
                {
                    //Прорисовка курсора.
                    WinApi.User32.DrawIcon(graphics.GetHdc(), x, y, pci.hCursor);
                }
                finally
                {
                    //Завершение работы с изображением.
                    graphics.ReleaseHdc();
                }
            }
        }
    }
}
