using System;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Capture
{
    /// <summary>
    /// Настройки захвата.
    /// </summary>
    public interface ICaptureSettings<out TBitRate, out TVideoCodec, out TAreaKind>
        where TBitRate : struct
        where TVideoCodec : struct
        where TAreaKind : struct
    {
        /// <summary>
        /// Путь до папки хранения.
        /// </summary>
        string OutputPath { get; }

        /// <summary>
        /// Количество бит исп. для обработки данных.
        /// </summary>
        TBitRate Rate { get; }
        /// <summary>
        /// Тип используемого сжатия.
        /// </summary>
        TVideoCodec VideoCodec { get; }
        /// <summary>
        /// Количество кадров в секунду.
        /// </summary>
        int Fps { get; }

        /// <summary>
        /// Выбраная область.
        /// </summary>
        Rectangle Area { get; }
        /// <summary>
        /// Тип захвата видео.
        /// </summary>
        TAreaKind AreaKind { get; }
    }

    public interface ICaptureInfObject
    {
        void Draw(Graphics graphics, int left, int top);
    }

    /// <summary>
    /// Захват видео.
    /// </summary>
    public interface ICaptureController<TBitRate, TVideoCodec, TAreaKind>
        where TBitRate : struct
        where TVideoCodec : struct
        where TAreaKind : struct
    {
        /// <summary>
        /// Настройки захвата.
        /// </summary>
        ICaptureSettings<TBitRate, TVideoCodec, TAreaKind> Settings { get; }

        /// <summary>
        /// Режим информирования.
        /// </summary>
        CaptureInfObjects Mods { get; }

        /// <summary>
        /// Флаг протекания захвата.
        /// </summary>
        bool IsRecording { get; }

        void GetFrame(object sender, ref Bitmap frame);

        /// <summary>
        /// Запуск захвата.
        /// </summary>
        /// <returns>В случае успеха возвращает true, в провтином случае - false.</returns>
        bool Start();
        /// <summary>
        /// Остановка захвата.
        /// </summary>
        /// <returns>В случае успеха возвращает true, в провтином случае - false.</returns>
        bool Stop();
        /// <summary>
        /// Пауза захвата.
        /// </summary>
        /// <returns>В случае успеха возвращает true, в провтином случае - false.</returns>
        bool Pause();
    }
    
    [Flags]
    public enum CaptureInfObjects : int
    {
        /// <summary>
        /// Без информации.
        /// </summary>
        None = 0,
        /// <summary>
        /// Информация о курсоре.
        /// </summary>
        Cursor = 1 << 1,
        /// <summary>
        /// Информация о клавишах.
        /// </summary>
        Keys = 1 << 2,
        /// <summary>
        /// Базовая информация.
        /// </summary>
        Default = Cursor,
        /// <summary>
        /// Полная информация.
        /// </summary>
        Fully = -1
    }

    public abstract class CaptureCommon<TBitRate, TVideoCodec, TAreaKind> : 
        ICaptureController<TBitRate, TVideoCodec, TAreaKind>
        where TBitRate : struct
        where TVideoCodec : struct
        where TAreaKind : struct
    {
        public abstract ICaptureSettings<TBitRate, TVideoCodec, TAreaKind> Settings { get; }

        public abstract CaptureInfObjects Mods { get; }

        public abstract bool IsRecording { get; }

        protected Dictionary<CaptureInfObjects, ICaptureInfObject> _captureObjs { get; } = 
                new Dictionary<CaptureInfObjects, ICaptureInfObject>();

        public virtual void GetFrame(object sender, ref Bitmap frame)
        {
            if (!IsRecording)
                return;

            using (Graphics graphics = Graphics.FromImage(frame))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;

                foreach (var item in Enum.GetValues(typeof(CaptureInfObjects)).Cast<CaptureInfObjects>())
                {
                    if ((Mods & item) == item && _captureObjs.ContainsKey(item))
                        _captureObjs[item]?.Draw(graphics, Settings.Area.Left, Settings.Area.Top);
                }
                //NewFrame?(graphics)
            }
        }

        public abstract bool Pause();
        public abstract bool Start();
        public abstract bool Stop();
    }


    //public abstract class CaptureCursor : ICaptureObject
    //{
    //    public virtual void Draw(Graphics graphics, int left, int top)
    //    {
    //        if (graphics == null)
    //        {
    //            throw new ArgumentNullException(nameof(graphics));
    //        }


    //        ////Получение информации о курсоре.
    //        //CURSORINFO pci;
    //        //pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));
    //        //if (GetCursorInfo(out pci))
    //        //{

    //        //    //Формирование дорисовки к позиции курсора.
    //        //    if (pci.flags == CURSOR_SHOWING)
    //        //    {
    //        //        //Смещение при обрезке кадра.
    //        //        int x = pci.ptScreenPos.x - left;
    //        //        int y = pci.ptScreenPos.y - top;
    //        //        int rX = 0;
    //        //        int rY = 0;

    //        //        //Получаем информацию о иконке курсора для корректировки смещения.
    //        //        ICONINFO iconInfo;
    //        //        IntPtr hicon = CopyIcon(pci.hCursor);
    //        //        if (hicon != IntPtr.Zero && GetIconInfo(hicon, out iconInfo))
    //        //        {
    //        //            //Смещаем курсор относительно размерности иконки.
    //        //            x -= iconInfo.xHotspot;
    //        //            y -= iconInfo.yHotspot;
    //        //            //Смещаем маркер.
    //        //            rX += iconInfo.xHotspot;
    //        //            rY += iconInfo.yHotspot;
    //        //        }
    //        //        //Задаем настройки маркера.
    //        //        Color c = Color.WhiteSmoke;
    //        //        float width = 3;
    //        //        int radius = 30;
    //        //        //При зажатия клавиш.
    //        //        if ((Control.MouseButtons & MouseButtons.Left) != 0 || (Control.MouseButtons & MouseButtons.Right) != 0)
    //        //        {
    //        //            //Задаем настройки маркера.
    //        //            c = ((Control.MouseButtons & MouseButtons.Right) != 0) ? Color.OrangeRed : Color.YellowGreen;
    //        //            width = 5;
    //        //            radius = 40;
    //        //        }
    //        //        //Прорисовка маркера.
    //        //        var background = new Pen(Color.Black, width + 1.5F);
    //        //        graphics.DrawEllipse(background, (x - radius / 2) + rX, (y - radius / 2) + rY, radius, radius);
    //        //        var marker = new Pen(c, width);
    //        //        graphics.DrawEllipse(marker, (x - radius / 2) + rX, (y - radius / 2) + rY, radius, radius);
    //        //        //Прорисовка курсора.
    //        //        DrawIcon(graphics.GetHdc(), x, y, pci.hCursor);
    //        //        //Завершение работы с изображением.
    //        //        graphics.ReleaseHdc();
    //        //    }
    //        //}

    //    }
    //}
}
