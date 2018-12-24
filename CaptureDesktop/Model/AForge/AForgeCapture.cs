using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Capture;

using AForge.Video;
using AForge.Video.FFMPEG;
using System.Windows.Forms;

namespace CaptureDesktop.Model.AForge
{
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
    public enum AreaKind : int
    {
        /// <summary>
        /// Захват всех источников.
        /// </summary>
        All,
        /// <summary>
        /// Захват области.
        /// </summary>
        Area,
        /// <summary>
        /// Захват окна.
        /// </summary>
        Window,
        /// <summary>
        /// Захват устройства.
        /// </summary>
        Device
    }

    public class AForgeCaptureSettings : CaptureSettings<BitRate, VideoCodec, AreaKind>
    {
        public override ICaptureSettings<BitRate, VideoCodec, AreaKind> Default { get { return Empty; } }

        public static AForgeCaptureSettings Empty => new AForgeCaptureSettings()
        {
            OutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Templates),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),

            Area = GetScreenAll(),
            AreaKind = AreaKind.All,

            VideoCodec = VideoCodec.MPEG4,
            Rate = BitRate._5000kbit,
            Fps = 15,
        };

        public static Rectangle GetScreenAll()
        {
            var result = new Rectangle();

            Screen.AllScreens.ToList().ForEach(f => result = Rectangle.Union(result, f.Bounds));

            return result;
        }
    }

    /// <summary>
    /// Захват изображения.
    /// </summary>
    public class AForgeCapture : CaptureCommon<BitRate, VideoCodec, AreaKind>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public AForgeCapture()
        {
            _writer = new VideoFileWriter();            
        }
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="settings">Настройки</param>
        public AForgeCapture(AForgeCaptureSettings settings) : this()
        {
            Settings = settings;
        }

        /// <summary>
        /// Поддерживаемое расширение.
        /// </summary>
        private string FileExt => ".avi";

        private Stopwatch _watch { get; set; }
        private VideoFileWriter _writer { get; set; }
        private ScreenCaptureStream _streamVideo { get; set; }

        public override void GetFrame(object sender, Bitmap frame)
        {
            base.GetFrame(sender, frame);
            //Проверка флага.
            if (IsRecording)
                _writer.WriteVideoFrame(frame);//Запись в поток.
        }

        /// <summary>
        /// Запуск захвата видео в поток.
        /// </summary>
        /// <returns>В случае успеха операции вернет true, в противном случае - false.</returns>
        protected bool StartAsyncEx()
        {
            _streamVideo = new ScreenCaptureStream(
                //HINT> Перехват для базовых значений в случае отсутствия.
                Settings.Area == Rectangle.Empty ? 
                Settings.Default.Area : Settings.Area);
            _streamVideo.NewFrame += (s, e) => GetFrame(s, e.Frame);
            _streamVideo.Start();
            _watch = new Stopwatch();
            _watch.Start();
            //Возвращаем результат.
            return true;
        }
        /// <summary>
        /// Остановка захвата видео в поток.
        /// </summary>
        protected void StopAsyncEx()
        {
            //Останавливаем.
            _watch?.Reset();
            Thread.Sleep(500);
            _streamVideo?.SignalToStop();
            Thread.Sleep(500);
            _writer?.Close();
        }

        /// <summary>
        /// Запускаем запись.
        /// </summary>
        /// <returns>В случае успеха операции вернет true, в противном случае - false.</returns>
        public override void Start()
        {
            //Проверка пути.
            if (string.IsNullOrEmpty(Settings.OutputPath) || !Directory.Exists(Path.GetFullPath(Settings.OutputPath)))
            {
                //Получаем базовый.
                Settings.OutputPath = Settings.Default.OutputPath;
            }
            //Проверяем каталог.
            if (!Directory.Exists(Settings.OutputPath))
            {
                Directory.CreateDirectory(Settings.OutputPath);
            }
            //Формируем имя файла.
            var fileName = string.Format(@"{0}_{1}",
                Environment.UserName.ToUpper(), DateTime.Now.ToString("d_MMM_yyyy_HH_mm_ssff"));
            //Формируем путь.
            var fullName = Path.Combine(Settings.OutputPath,
                Path.ChangeExtension(Path.GetFileNameWithoutExtension(fileName), FileExt));
            try
            {
                //Открываем поток на запись.
                _writer.Open(
                    fullName,
                    Settings.Area.Width == default(int) ? Settings.Default.Area.Width : Settings.Area.Width,
                    Settings.Area.Height == default(int) ? Settings.Default.Area.Height : Settings.Area.Height,
                    Settings.Fps == default(int) ? Settings.Default.Fps : Settings.Fps,
                    Settings.VideoCodec == VideoCodec.Default ? Settings.Default.VideoCodec : Settings.VideoCodec,
                    (int)(Settings.Rate == default(int) ? Settings.Default.Rate : Settings.Rate));

                //Запускаем запись.
                if (StartAsyncEx())
                    base.Start();
            }
            catch (Exception)
            {
                //Останавливаем запись.
                StopAsyncEx();
                base.Stop();

                throw;
            }
        }
        /// <summary>
        /// Останавливаем запись.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            this.StopAsyncEx();
        }
    }
}
