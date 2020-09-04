namespace Capture.Core
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Linq;

    public abstract class CaptureService<TSettings> : ICaptureController<TSettings>
        where TSettings : class, ICaptureSettings
    {
        #region Fields

        protected Dictionary<CaptureInfObjects, ICaptureInfObject> _captureObjects;

        private Rectangle _captureArea;

        #endregion Fields

        #region Properties

        public ICaptureInfObject this[CaptureInfObjects key]
        {
            get => _captureObjects.ContainsKey(key) ? _captureObjects[key] : null;
            set => _captureObjects[key] = value;
        }

        public IAreaSelector Selector { get; }

        public TSettings Settings { get; }

        public CaptureInfObjects Mods { get; set; } = CaptureInfObjects.Default;

        public virtual CaptureState State { get; private set; }

        #endregion Properties

        #region Events

        public event EventHandler<CaptureState> OnStateUpdated;

        #endregion Events

        #region Constructors

        protected CaptureService(TSettings settings, IAreaSelector selector)
        {
            _captureObjects = new Dictionary<CaptureInfObjects, ICaptureInfObject>();

            Selector = selector;
            Settings = settings;
        }

        #endregion Constructors

        #region Methods
        
        /// <summary>
        /// Запуск захвата.
        /// </summary>
        public void Start()
        {
            //Проверка пути.
            if (string.IsNullOrEmpty(Settings.OutputPath) || !Directory.Exists(Path.GetFullPath(Settings.OutputPath)))
                Settings.OutputPath = Settings.Default.OutputPath;

            //Проверяем каталог.
            if (!Directory.Exists(Settings.OutputPath))
                Directory.CreateDirectory(Settings.OutputPath);

            _captureArea = Rectangle.Empty;

            switch (Settings.AreaKind)
            {
                case AreaKind.All:
                    _captureArea = Selector.GetScreenAll();
                    break;

                case AreaKind.Area:
                    _captureArea = Selector.GetScreenArea();
                    break;

                case AreaKind.Device:
                    _captureArea = Selector.GetScreenDevice(Settings.AreaName);
                    break;

                case AreaKind.Window:
                    _captureArea = Selector.GetScreenWindow();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_captureArea == null || _captureArea == Rectangle.Empty)
                throw new ArgumentException("Is empty capture area.");

            StartEx(_captureArea);
            SetState(CaptureState.Started);
        }

        /// <summary>
        /// Остановка захвата.
        /// </summary>
        public void Stop()
        {
            try
            {
                StopEx();
            }
            finally
            {
                SetState(CaptureState.Finished);
            }
        }

        /// <summary>
        /// Получить изображение.
        /// </summary>
        /// <param name="frame">Кадр</param>
        public virtual void CaptureFrame(Bitmap frame)
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
        /// Установить состояние.
        /// </summary>
        /// <param name="captureEvent">Состояние</param>
        private void SetState(CaptureState captureEvent)
        {
            State = captureEvent;
            OnStateUpdated?.Invoke(this, State);
        }

        /// <summary>
        /// Запустить.
        /// </summary>
        /// <param name="captureArea">Область захвата.</param>
        protected virtual void StartEx(Rectangle captureArea)
        {
        }

        /// <summary>
        /// Остановить.
        /// </summary>
        protected virtual void StopEx()
        {
        }

        /// <summary>
        /// Добавить объектов.
        /// </summary>
        /// <param name="graphics"></param>
        protected virtual void MakeGraphics(Graphics graphics)
        {
            if (Mods == CaptureInfObjects.None)
                return;

            foreach (CaptureInfObjects item in Enum.GetValues(typeof(CaptureInfObjects)).Cast<CaptureInfObjects>())
            {
                if ((Mods & item) == item && _captureObjects.ContainsKey(item))
                    _captureObjects[item]?.Draw(graphics, _captureArea.Left, _captureArea.Top);
            }
        }

        #endregion Methods
    }
}
