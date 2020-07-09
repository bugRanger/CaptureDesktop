using System;
using System.Drawing;

namespace Capture
{
    /// <summary>
    /// Захват видео.
    /// </summary>
    public interface ICaptureController<TBitRate, TVideoCodec, TAreaKind>
        where TBitRate : struct
        where TAreaKind : struct
        where TVideoCodec : struct
    {
        #region Properties

        /// <summary>
        /// Настройки захвата.
        /// </summary>
        CaptureSettings<TBitRate, TVideoCodec, TAreaKind> Settings { get; }

        /// <summary>
        /// Флаг протекания захвата.
        /// </summary>
        bool IsRecording { get; }

        #endregion Properties

        #region Events

        event EventHandler<CaptureState> OnUpdated;

        #endregion Events

        #region Methods

        /// <summary>
        /// Запуск захвата.
        /// </summary>
        void Start();

        /// <summary>
        /// Остановка захвата.
        /// </summary>
        void Stop();

        ///// <summary>
        ///// Пауза захвата.
        ///// </summary>
        //void Pause();

        void GetFrame(object sender, Bitmap frame);

        #endregion Methods
    }
}
