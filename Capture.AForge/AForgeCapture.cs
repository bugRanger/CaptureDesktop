namespace Capture.AForge
{
    using System;
    using System.Drawing;
    using System.IO;

    using Core;

    using global::AForge.Video;
    using global::AForge.Video.FFMPEG;

    /// <summary>
    /// Захват изображения.
    /// </summary>
    public class AForgeCapture : CaptureService<AForgeSettings>
    {
        #region Fields

        private readonly VideoFileWriter _writer;

        private ScreenCaptureStream _streamVideo;

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
        /// <param name="settings">Настройки</param>
        public AForgeCapture(AForgeSettings settings) : base(settings, new AreaSelector())
        {
            _writer = new VideoFileWriter();
        }

        ~AForgeCapture()
        {
            _writer?.Dispose();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Получение кадра.
        /// </summary>
        /// <param name="frame"></param>
        public override void CaptureFrame(Bitmap frame)
        {
            base.CaptureFrame(frame);

            //Проверка флага.
            if (State == CaptureState.Started)
                _writer.WriteVideoFrame(frame);//Запись в поток.
        }

        /// <summary>
        /// Запускаем запись.
        /// </summary>
        /// <returns>В случае успеха операции вернет true, в противном случае - false.</returns>
        protected override void StartEx(Rectangle captureArea)
        {
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
                    captureArea.Width,
                    captureArea.Height,
                    Settings.Fps == default(int) ? Settings.Default.Fps : Settings.Fps,
                    Settings.VideoCodec,
                    (int)(Settings.Rate == default(int) ? Settings.Default.Rate : Settings.Rate));

                //HINT> Перехват для базовых значений в случае отсутствия.
                _streamVideo = new ScreenCaptureStream(captureArea);
                _streamVideo.NewFrame += (s, e) => CaptureFrame(e.Frame);
                _streamVideo.Start();
            }
            catch
            {
                //Останавливаем запись.
                StopEx();
                //TODO: Add write log.
                throw;
            }
        }

        /// <summary>
        /// Остановка захвата видео в поток.
        /// </summary>
        protected override void StopEx()
        {
            _streamVideo?.Stop();
            _writer?.Close();
        }

        #endregion Methods
    }
}
