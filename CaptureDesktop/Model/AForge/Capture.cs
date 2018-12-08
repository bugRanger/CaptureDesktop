using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using AForge.Video;
using AForge.Video.FFMPEG;

namespace CaptureDesktop.Model
{
    using static Capture;

    /// <summary>
    /// Настройки захвата.
    /// </summary>
    interface ICaptureOptions
    {
        /// <summary>
        /// Путь до папки хранения.
        /// </summary>
        string DirPath { get; }
        /// <summary>
        /// Количество бит исп. для обработки данных.
        /// </summary>
        BitRate Rate { get; }
        /// <summary>
        /// Тип используемого сжатия.
        /// </summary>
        VideoCodec Codec { get; }
        /// <summary>
        /// Количество кадров в секунду.
        /// </summary>
        int Fps { get; }
    }
    /// <summary>
    /// Настройки захвата области.
    /// </summary>
    interface IAreaCaptureOptions : ICaptureOptions
    {
        /// <summary>
        /// Тип захвата видео.
        /// </summary>
        ScreenKind AreaKind { get; }
        /// <summary>
        /// Наименование устройства.
        /// </summary>
        string DeviceName { get; }
    }

    /// <summary>
    /// Базовые значения захвата, для обработки случая запуска без внесения настроек.
    /// </summary>
    class CaptureDefault : IAreaCaptureOptions
    {
        protected CaptureDefault() { }
        public static CaptureDefault Empty
        {
            get
            {
                return new CaptureDefault();
            }
        }

        public string FolderName { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name; } }
        /// <summary>
        /// Путь до папки хранения.
        /// </summary>
        public string DirPath { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Templates), FolderName); } }
        /// <summary>
        /// Наименование файла.
        /// </summary>
        public string FileName { get; } = null;
        /// <summary>
        /// Количество бит исп. для обработки данных.
        /// </summary>
        public BitRate Rate { get; } = BitRate._5000kbit;
        /// <summary>
        /// Тип используемого сжатия.
        /// </summary>
        public VideoCodec Codec { get; } = VideoCodec.MPEG4;
        /// <summary>
        /// Количество кадров в секунду.
        /// </summary>
        public int Fps { get; } = 15;
        /// <summary>
        /// Тип захвата видео.
        /// </summary>
        public ScreenKind AreaKind { get; } = ScreenKind.skAll;
        /// <summary>
        /// Наименование устройства.
        /// </summary>
        public string DeviceName { get; } = null;
    }

    class CaptureException : Exception
    {
        public CaptureException(string message) : base(message) { }
    }
    class Capture : IAreaCaptureOptions
    {
        public Capture()
        {
            _area = Rectangle.Empty;
            _width = SystemInformation.VirtualScreen.Width;
            _height = SystemInformation.VirtualScreen.Height;

            _writer = new VideoFileWriter();
        }

        const Int32 CURSOR_SHOWING = 0x00000001;
        const Int32 CURSOR_ARROW = 0x00010003;
        const Int32 CURSOR_EDIT = 0x00010005;
        const Int32 CURSOR_REVERSE = 0x000803ae;

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct ICONINFO
        {
            public bool fIcon;
            public Int32 xHotspot;
            public Int32 yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);
        [DllImport("user32.dll")]
        public static extern IntPtr CopyIcon(IntPtr hIcon);
        [DllImport("user32.dll")]
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);
        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        /// <summary>
        /// Количество бит используемых для обработки данных в единицу времени.
        /// </summary>
        public enum BitRate : int
        {
            _50kbit = 5000,
            _100kbit = 10000,
            _500kbit = 50000,
            _1000kbit = 1000000,
            _2000kbit = 2000000,
            _3000kbit = 3000000,
            _4000kbit = 4000000,
            _5000kbit = 5000000
        }
        /// <summary>
        /// Тип захвата видео.
        /// </summary>
        public enum ScreenKind : int
        {
            /// <summary>
            /// Захват всех источников.
            /// </summary>
            skAll,
            /// <summary>
            /// Захват области.
            /// </summary>
            skArea,
            /// <summary>
            /// Захват устройства.
            /// </summary>
            skDevice
        }

        private Rectangle _area { get; set; }
        private int _width { get; set; }
        private int _height { get; set; }
        private int _left { get; set; }
        private int _top { get; set; }
        /// <summary>
        /// Флаг активности захвата.
        /// </summary>
        private bool _isRecording { get; set; } = false;
        private bool _isStoped { get; set; } = true;
        private Stopwatch _watch { get; set; }
        private VideoFileWriter _writer { get; set; }
        private ScreenCaptureStream _streamVideo { get; set; }

        /// <summary>
        /// Тип захвата видео.
        /// </summary>
        public ScreenKind AreaKind { get; private set; }
        /// <summary>
        /// Наименование устройства.
        /// </summary>
        public string DeviceName { get; private set; }

        /// <summary>
        /// Полное наименование файла.
        /// </summary>
        public string FullName { get; private set; }
        /// <summary>
        /// Путь до папки хранения.
        /// </summary>
        public string DirPath { get; set; }
        /// <summary>
        /// Наименование файла.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Расширение файла.
        /// </summary>
        public string FileExt { get; private set; } = "avi";
        /// <summary>
        /// Количество бит исп. для обработки данных.
        /// </summary>
        public BitRate Rate { get; set; }
        /// <summary>
        /// Тип используемого сжатия.
        /// </summary>
        public VideoCodec Codec { get; set; }
        /// <summary>
        /// Количество кадров в секунду.
        /// </summary>
        public int Fps { get; set; }
        /// <summary>
        /// Флаг выделения зоны.
        /// </summary>
        public bool IsSelectedArea { get; protected set; } = false;

        /// <summary>
        /// Событие настройки зоны.
        /// </summary>
        public event EventHandler SettingArea;
        /// <summary>
        /// Процесс записи.
        /// </summary>
        public event EventHandler Recording;
        /// <summary>
        /// Завершение записи.
        /// </summary>
        public event EventHandler Recorded;

        protected void OnSettingArea()
        {
            EventHandler handler = SettingArea;
            handler?.Invoke(this, EventArgs.Empty);
        }
        protected void OnRecording()
        {
            EventHandler handler = Recording;
            handler?.Invoke(this, EventArgs.Empty);
        }
        protected void OnRecorded()
        {
            EventHandler handler = Recorded;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Вызываем метод записи в поток.
        /// </summary>
        /// <returns>В случае успеха операции вернет true, в противном случае - false.</returns>
        protected bool StartAsyncEx()
        {
            try
            {
                _streamVideo = new ScreenCaptureStream(_area);
                _streamVideo.NewFrame += new NewFrameEventHandler(NewFrame);
                _streamVideo.Start();
                _watch = new Stopwatch();
                _watch.Start();
                //Информируем о запуске.
                _isRecording = true;
                OnRecording();
                //Возвращаем результат.
                return true;
            }
            catch (Exception)
            {
                _isRecording = false;
                throw;
            }
            finally
            {
                _isStoped = !_isRecording;
            }
        }
        /// <summary>
        /// Захват изображения.
        /// </summary>
        /// <param name="sender">Источник</param>
        /// <param name="eventArgs"></param>
        private void NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //HINT> Получаем снимок экрана, всех???
            try
            {
                //Проверяем состояние.
                if (_isRecording)
                {
                    //_frameCount++;
                    //Получение изображения.
                    Bitmap frame = eventArgs.Frame;
                    //В графический элемент(для формирования дорисовки)
                    Graphics graphics = Graphics.FromImage(frame);
                    //Получение информации о курсоре.
                    CURSORINFO pci;
                    pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));
                    if (GetCursorInfo(out pci))
                    {
                        //Формирование дорисовки к позиции курсора.
                        if (pci.flags == CURSOR_SHOWING)
                        {
                            //Смещение при обрезке кадра.
                            int x = pci.ptScreenPos.x - _left;
                            int y = pci.ptScreenPos.y - _top;
                            int rX = 0;
                            int rY = 0;
                            //Получаем информацию о иконке курсора для корректировки смещения.
                            ICONINFO iconInfo;
                            IntPtr hicon = CopyIcon(pci.hCursor);
                            if (hicon != IntPtr.Zero && GetIconInfo(hicon, out iconInfo))
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
                            //Прорисовка курсора.
                            DrawIcon(graphics.GetHdc(), x, y, pci.hCursor);
                            //Завершение работы с изображением.
                            graphics.ReleaseHdc();
                        }
                    }
                    //Проверка флага(Обрезка кадра, в случае использования области).
                    //Т.к. в потоке записи уже объявленна облесть в размер кадра.
                    if (IsSelectedArea)
                    {
                        //Обрезка кадра.
                        var destRect = new Rectangle(Convert.ToInt32((_width - frame.Width) / 2), Convert.ToInt32((_height - frame.Height) / 2), frame.Width, frame.Height);
                        var destImage = new Bitmap(_width, _height);
                        destImage.SetResolution(frame.HorizontalResolution, frame.VerticalResolution);
                        //Перегружаем контейнеры связанные с кадром.
                        graphics = Graphics.FromImage(destImage);
                        graphics.DrawImage(frame, destRect, 0, 0, frame.Width, frame.Height, GraphicsUnit.Pixel, null);
                        frame = destImage;
                    }
                    //Запись в поток.
                    _writer.WriteVideoFrame(frame);
                }
                else
                {
                    //Останавливаем.
                    _watch.Reset();
                    Thread.Sleep(500);
                    _streamVideo.SignalToStop();
                    Thread.Sleep(500);
                    _writer.Close();
                }
            }
            finally
            {
                if (_isRecording && !_isStoped)
                    OnRecording();
                else
                    OnRecorded();
                _isStoped = !_isRecording;
            }
        }

        /// <summary>
        /// Получить список устройств.
        /// </summary>
        /// <returns>В случае успеха вернет набор доступных устройств, в противном случае - пустой набор.</returns>
        public string[] GetAllDevices()
        {
            return Screen.AllScreens.Select(s => s.DeviceName).ToArray();
        }

        public bool HasArea()
        {
            return !Rectangle.Empty.Equals(_area);
        }
        public bool HasRecord()
        {
            return _isRecording && !_isStoped;
        }

        /// <summary>
        /// Устанавливаем зону.
        /// </summary>
        /// <param name="kind">Тип захвата</param>
        /// <param name="deviceName">Наименование устройства</param>
        public bool SetArea(ScreenKind kind = ScreenKind.skAll, string deviceName = "")
        {
            //Обнуляем флаг.
            IsSelectedArea = false;
            //Обнуляем зону.
            _area = Rectangle.Empty;
            _width = _height = _top = _left = 0;
            //Проверяем тип.
            switch (kind)
            {
                //Все.
                case ScreenKind.skAll:
                    foreach (var item in Screen.AllScreens)
                    {
                        _area = Rectangle.Union(_area, item.Bounds);
                    }
                    _width = _area.Width;
                    _height = _area.Height;
                    //Вызываем событие.
                    OnSettingArea();
                    break;
                //Зона.
                case ScreenKind.skArea:
                    using (var selected = new TopForm())
                    {
                        if (selected.ShowDialog() == DialogResult.OK
                            && selected.w != 0
                            && selected.h != 0)
                        {
                            _area = selected.AreaBounds;

                            decimal prop = (decimal)4 / 3;
                            decimal realProp = (decimal)selected.w / selected.h;
                            bool makeLonger = realProp < prop;
                            int w = Convert.ToInt32(makeLonger ? selected.h * prop : selected.w);
                            int h = Convert.ToInt32(makeLonger ? selected.h : selected.w / prop);

                            if ((w & 1) != 0)
                                w = w + 1;
                            if ((h & 1) != 0)
                                h = h + 1;

                            _width = w;
                            _height = h;
                            _left = selected.AreaBounds.Left;
                            _top = selected.AreaBounds.Top;
                            IsSelectedArea = true;
                        }
                    }
                    //Вызываем событие.
                    OnSettingArea();
                    break;
                //Устройство.
                case ScreenKind.skDevice:
                    _area = Screen.AllScreens.First(scr => scr.DeviceName.Equals(deviceName)).Bounds;
                    _width = _area.Width;
                    _height = _area.Height;
                    //Вызываем событие.
                    OnSettingArea();
                    break;
                default:
                    //Возвращаем результат.
                    return false;
            }
            //Назначаем настройки.
            AreaKind = kind;
            DeviceName = deviceName;
            //Возвращаем результат.
            return true;
        }
        /// <summary>
        /// Устанавливаем зону захвата.
        /// </summary>
        /// <param name="options">Настройки для зоны</param>
        public bool SetArea(IAreaCaptureOptions options)
        {
            return SetArea(options.AreaKind, options.DeviceName);
        }
        /// <summary>
        /// Устанавливаем настройки захвата.
        /// </summary>
        /// <param name="options">Настройки</param>
        public void SetOptions(ICaptureOptions options)
        {
            DirPath = options.DirPath;
            Codec = options.Codec;
            Rate = options.Rate;
            Fps = options.Fps;
        }

        /// <summary>
        /// Запускаем запись.
        /// </summary>
        /// <returns>В случае успеха операции вернет true, в противном случае - false.</returns>
        public bool StartAsync()
        {
            //Проверка пути.
            if (string.IsNullOrEmpty(DirPath) || !Directory.Exists(Path.GetFullPath(DirPath)))
            {
                //Получаем базовый.
                DirPath = CaptureDefault.Empty.DirPath;
            }
            //Проверяем каталог.
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }
            //Формируем имя файла.
            FileName = string.Format(@"{0}_{1}", Environment.UserName.ToUpper(), DateTime.Now.ToString("d_MMM_yyyy_HH_mm_ssff"));
            //Формируем путь.
            FullName = Path.Combine(DirPath, Path.ChangeExtension(Path.GetFileNameWithoutExtension(FileName), FileExt));
            try
            {
                //Проверяем.
                if (!HasArea())
                {
                    throw new Exception("Not selected area!");
                }
                //Открываем поток на запись.
                _writer.Open(
                    FullName,
                    _width,
                    _height,
                    //Перехват для базовых значений в случае отсутствия.
                    Fps == default(int) ? CaptureDefault.Empty.Fps : Fps,
                    Codec == VideoCodec.Default ? CaptureDefault.Empty.Codec : Codec,
                    (int)(Rate == default(int) ? CaptureDefault.Empty.Rate : Rate));
                //Запускаем запись.
                return StartAsyncEx();
            }
            catch (Exception)
            {
                _writer.Close();
                throw;
            }
        }
        /// <summary>
        /// Останавливаем запись.
        /// </summary>
        public void StopAsync()
        {
            _isRecording = false;
        }
    }
}
