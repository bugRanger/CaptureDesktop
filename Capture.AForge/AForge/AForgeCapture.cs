using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Capture;

using AForge.Video;
using AForge.Video.FFMPEG;

namespace CaptureDesktop.Model.AForge
{
    /// <summary>
    /// Захват изображения.
    /// </summary>
    public class AForgeCapture : CaptureCommon<BitRate, VideoCodec, AreaKind>
    {
        #region Fields

        private VideoFileWriter _writer { get; set; }

        private ScreenCaptureStream _streamVideo { get; set; }

        #endregion Fields

        #region Properties

        public string FileName { get; protected set; }

        /// <summary>
        /// Поддерживаемое расширение.
        /// </summary>
        public string FileExt => ".avi";

        #endregion Properties

        #region Constructors

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

        ~AForgeCapture()
        {
            _writer?.Dispose();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Запускаем запись.
        /// </summary>
        /// <returns>В случае успеха операции вернет true, в противном случае - false.</returns>
        public override void Start()
        {
            switch (Settings.AreaKind)
            {
                case AreaKind.All:
                    Settings.Area = AForgeCaptureSettings.GetScreenAll();
                    break;
                case AreaKind.Area:
                    Settings.Area = AForgeCaptureSettings.GetScreenArea();
                    break;
                case AreaKind.Device:
                    Settings.Area = AForgeCaptureSettings.GetScreenDevice();
                    break;
                case AreaKind.Window:
                    Settings.Area = AForgeCaptureSettings.GetScreenWindow();
                    break;

                default:
                    break;
            }

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
            FileName = $@"{Environment.UserName.ToUpper()}_{DateTime.Now:d_MMM_yyyy_HH_mm_ssff}";
            //Формируем путь.
            string fullName = Path.Combine(Settings.OutputPath,
                Path.ChangeExtension(Path.GetFileNameWithoutExtension(FileName), FileExt));
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
                if (StartEx())
                    base.Start();
            }
            catch(Exception e)
            {
                //Останавливаем запись.
                Stop();
                //TODO: Необходим Logger.
                throw;
            }
        }

        /// <summary>
        /// Останавливаем запись.
        /// </summary>
        public override void Stop()
        {
            try
            {
                base.Stop();
            }
            finally
            {
                StopEx();
            }
        }

        /// <summary>
        /// Получение кадра.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="frame"></param>
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
        protected bool StartEx()
        {

            //HINT> Перехват для базовых значений в случае отсутствия.
            _streamVideo = new ScreenCaptureStream(Settings.Area == Rectangle.Empty ? Settings.Default.Area : Settings.Area);
            _streamVideo.NewFrame += (s, e) => GetFrame(s, e.Frame);
            _streamVideo.Start();
            //Возвращаем результат.
            return true;
        }

        /// <summary>
        /// Остановка захвата видео в поток.
        /// </summary>
        protected void StopEx()
        {
            _streamVideo?.Stop();
            _writer?.Close();
        }

        #endregion Methods
    }
}
