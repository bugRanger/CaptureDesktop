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
        skAll,
        /// <summary>
        /// Захват области.
        /// </summary>
        skArea,
        /// <summary>
        /// Захват окна.
        /// </summary>
        skWindow,
        /// <summary>
        /// Захват устройства.
        /// </summary>
        skDevice
    }

    public class AForgeCaptureSettings : CaptureSettings<BitRate, VideoCodec, AreaKind>
    {
        public static AForgeCaptureSettings Default => new AForgeCaptureSettings()
        {
            OutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Templates),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),

            Area = GetScreenAll(),
            AreaKind = AreaKind.skAll,

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

    public class AForgeCapture : CaptureCommon<BitRate, VideoCodec, AreaKind>
    {
        public AForgeCapture()
        {
            _writer = new VideoFileWriter();
        }
        public AForgeCapture(AForgeCaptureSettings settings) : this()
        {
            Settings = settings;
        }

        private string FileExt => ".avi";

        private Stopwatch _watch { get; set; }
        private VideoFileWriter _writer { get; set; }
        private ScreenCaptureStream _streamVideo { get; set; }
        
        protected override void MakeGraphics(Graphics graphics)
        {
        }

        public override void GetFrame(object sender, Bitmap frame)
        {
            base.GetFrame(sender, frame);

            if (IsRecording)
            {
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

        /// <summary>
        /// Вызываем метод записи в поток.
        /// </summary>
        /// <returns>В случае успеха операции вернет true, в противном случае - false.</returns>
        protected bool StartAsyncEx()
        {
            try
            {
                _streamVideo = new ScreenCaptureStream(Settings.Area);
                _streamVideo.NewFrame += (s, e) => GetFrame(s, e.Frame);
                _streamVideo.Start();
                _watch = new Stopwatch();
                _watch.Start();
                //Возвращаем результат.
                return true;
            }
            catch (Exception)
            {
                base.Stop();

                throw;
            }
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
                Settings.OutputPath = AForgeCaptureSettings.Default.OutputPath;
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
                ////Проверяем.
                //if (!HasArea())
                //{
                //    throw new Exception("Not selected area!");
                //}
                //Открываем поток на запись.
                _writer.Open(
                    fullName,
                    Settings.Area.Width,
                    Settings.Area.Height,
                    //Перехват для базовых значений в случае отсутствия.
                    Settings.Fps == default(int) ? AForgeCaptureSettings.Default.Fps : Settings.Fps,
                    Settings.VideoCodec == VideoCodec.Default ? AForgeCaptureSettings.Default.VideoCodec : Settings.VideoCodec,
                    (int)(Settings.Rate == default(int) ? AForgeCaptureSettings.Default.Rate : Settings.Rate));

                //Запускаем запись.
                if (StartAsyncEx())
                    base.Start();
            }
            catch (Exception)
            {
                _writer.Close();
                base.Stop();

                throw;
            }
        }
    }
}
