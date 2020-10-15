namespace Capture.AForge
{
    using System;
    using System.IO;
    using System.Drawing;
    using System.Collections.Generic;

    using Core;

    using global::AForge.Video;
    using global::AForge.Video.FFMPEG;

    using CoreCodec = Core.VideoCodec;
    using AForgeCodec = global::AForge.Video.FFMPEG.VideoCodec;

    /// <summary>
    /// Захват изображения.
    /// </summary>
    public class AForgeCapture : CaptureService<AForgeSettings>
    {
        #region Fields

        private const string FILE_EXT = ".avi";

        private readonly VideoFileWriter _writer;
        private readonly Dictionary<CoreCodec, AForgeCodec> _codecMapper;
        private ScreenCaptureStream _streamVideo;

        #endregion Fields

        #region Constructors
        
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="settings">Настройки</param>
        public AForgeCapture(AForgeSettings settings) : base(settings, new AreaSelector())
        {
            _writer = new VideoFileWriter();
            _codecMapper = new Dictionary<CoreCodec, AForgeCodec>()
            {
                { CoreCodec.Default, AForgeCodec.Default },
                { CoreCodec.FLV1, AForgeCodec.FLV1 },
                { CoreCodec.H263P, AForgeCodec.H263P },
                { CoreCodec.MPEG2, AForgeCodec.MPEG2 },
                { CoreCodec.MPEG4, AForgeCodec.MPEG4 },
                { CoreCodec.MSMPEG4v2, AForgeCodec.MSMPEG4v2 },
                { CoreCodec.MSMPEG4v3, AForgeCodec.MSMPEG4v3 },
                { CoreCodec.Raw, AForgeCodec.Raw },
                { CoreCodec.WMV1, AForgeCodec.WMV1 },
                { CoreCodec.WMV2, AForgeCodec.WMV2 },
            };

            FileExt = FILE_EXT;
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
        protected override void RecordEx(Rectangle captureArea)
        {
            if (!_codecMapper.TryGetValue(Settings.Codec, out var codec))
                throw new NotSupportedException($"Not supported codec {Enum.GetName(typeof(CoreCodec), Settings.Codec)}");

            //Открываем поток на запись.
            _writer.Open(
                FileName,
                captureArea.Width,
                captureArea.Height,
                Settings.Fps == default(int) ? Settings.Default.Fps : Settings.Fps,
                codec,
                (int)(Settings.Rate == default(int) ? Settings.Default.Rate : Settings.Rate));

            //HINT> Перехват для базовых значений в случае отсутствия.
            _streamVideo = new ScreenCaptureStream(captureArea);
            _streamVideo.NewFrame += (s, e) => CaptureFrame(e.Frame);
            _streamVideo.Start();
        }

        /// <summary>
        /// Остановка захвата видео в поток.
        /// </summary>
        protected override void StopEx()
        {
            _streamVideo?.Stop();
            _writer?.Close();
            base.StopEx();
        }

        #endregion Methods
    }
}
