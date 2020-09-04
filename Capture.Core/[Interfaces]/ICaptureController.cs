namespace Capture.Core
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Захват видео.
    /// </summary>
    public interface ICaptureController<out TSettings>
        where TSettings : ICaptureSettings
    {
        #region Properties

        /// <summary>
        /// Настройки захвата.
        /// </summary>
        TSettings Settings { get; }

        /// <summary>
        /// Режим информирования.
        /// </summary>
        CaptureInfObjects Mods { get; }

        /// <summary>
        /// Флаг протекания захвата.
        /// </summary>
        CaptureState State { get; }

        #endregion Properties

        #region Events

        event EventHandler<CaptureState> OnStateUpdated;

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

        void CaptureFrame(Bitmap frame);

        #endregion Methods
    }
}