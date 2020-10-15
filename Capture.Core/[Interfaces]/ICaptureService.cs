namespace Capture.Core
{
    using System;

    public interface ICaptureService
    {
        #region Properties

        IAreaSelector Selector { get; }

        ICaptureSettings Settings { get; }

        CaptureInfObjects Mods { get; }

        CaptureState State { get; }

        #endregion Properties

        #region Events

        event EventHandler<CaptureState> OnStateUpdated;
        event EventHandler<string> OnFinished;
        event EventHandler<Exception> OnError;

        #endregion Events

        #region Methods

        void Record();

        void Stop();

        //void Pause();

        #endregion Methods
    }
}