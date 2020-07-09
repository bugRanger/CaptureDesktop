namespace Capture
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Collections.Concurrent;

    public abstract class CaptureCommon<TBitRate, TVideoCodec, TAreaKind> :
        ICaptureController<TBitRate, TVideoCodec, TAreaKind>
        where TBitRate : struct
        where TAreaKind : struct
        where TVideoCodec : struct
    {
        #region Fields

        protected ConcurrentBag<ICaptureObject> CaptureObjects { get; }

        #endregion Fields

        #region Properties

        public CaptureSettings<TBitRate, TVideoCodec, TAreaKind> Settings { get; protected set; }

        public virtual bool IsRecording { get; protected set; } = false;

        #endregion Properties

        #region Events

        public event EventHandler<CaptureState> OnUpdated;

        #endregion Events

        #region Constructors

        protected CaptureCommon() 
        {
            CaptureObjects = new ConcurrentBag<ICaptureObject>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Запуск захвата.
        /// </summary>
        public virtual void Start()
        {
            IsRecording = true;
            RaiseUpdated(CaptureState.Started);
        }

        /// <summary>
        /// Остановка захвата.
        /// </summary>
        public virtual void Stop()
        {
            IsRecording = false;
            RaiseUpdated(CaptureState.Finished);
        }

        /// <summary>
        /// Получить изображение.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="frame"></param>
        public virtual void GetFrame(object sender, Bitmap frame)
        {
            using (Graphics graphics = Graphics.FromImage(frame))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                //Формируем изображение.
                MakeGraphics(graphics);
            }
        }

        /// <summary>
        /// Добавить объектов.
        /// </summary>
        /// <param name="graphics"></param>
        protected virtual void MakeGraphics(Graphics graphics)
        {
            foreach (var captureItem in CaptureObjects) 
            {
                captureItem.Draw(graphics, Settings.Area.Left, Settings.Area.Top);
            }
        }

        protected void RaiseUpdated(CaptureState @event)
        {
            EventHandler<CaptureState> handler = OnUpdated;
            handler?.Invoke(this, @event);
        }

        #endregion Methods
    }
}
