namespace Capture.Core
{
    using System;
    using System.Drawing;

    /// <summary>
    /// ������ �����.
    /// </summary>
    public interface ICaptureController<out TSettings>
        where TSettings : ICaptureSettings
    {
        #region Properties

        /// <summary>
        /// ��������� �������.
        /// </summary>
        TSettings Settings { get; }

        /// <summary>
        /// ����� ��������������.
        /// </summary>
        CaptureInfObjects Mods { get; }

        /// <summary>
        /// ���� ���������� �������.
        /// </summary>
        CaptureState State { get; }

        #endregion Properties

        #region Events

        event EventHandler<CaptureState> OnStateUpdated;

        #endregion Events

        #region Methods

        /// <summary>
        /// ������ �������.
        /// </summary>
        void Start();

        /// <summary>
        /// ��������� �������.
        /// </summary>
        void Stop();

        ///// <summary>
        ///// ����� �������.
        ///// </summary>
        //void Pause();

        void CaptureFrame(Bitmap frame);

        #endregion Methods
    }
}