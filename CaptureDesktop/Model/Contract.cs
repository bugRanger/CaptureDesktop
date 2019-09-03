using System;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using CaptureDesktop.Model;
using System.Windows.Forms;

namespace Capture
{
    /// <summary>
    /// Настройки захвата.
    /// </summary>
    public interface ICaptureSettings<TBitRate, TVideoCodec, TAreaKind>
        where TBitRate : struct
        where TAreaKind : struct
        where TVideoCodec : struct
    {
        /// <summary>
        /// Путь до папки хранения.
        /// </summary>
        string OutputPath { get; set; }

        /// <summary>
        /// Выбранная область.
        /// </summary>
        Rectangle Area { get; set; }

        /// <summary>
        /// Тип захвата видео.
        /// </summary>
        TAreaKind AreaKind { get; set; }

        /// <summary>
        /// Количество бит исп. для обработки данных.
        /// </summary>
        TBitRate Rate { get; set; }

        /// <summary>
        /// Тип используемого сжатия.
        /// </summary>
        TVideoCodec VideoCodec { get; set; }

        /// <summary>
        /// Количество кадров в секунду.
        /// </summary>
        int Fps { get; set; }
    }

    public interface ICaptureInfObject
    {
        void Draw(Graphics graphics, int left, int top);
    }

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
        /// Режим информирования.
        /// </summary>
        CaptureInfObjects Mods { get; }

        /// <summary>
        /// Флаг протекания захвата.
        /// </summary>
        bool IsRecording { get; }

        #endregion Properties

        #region Events

        event EventHandler<CaptureEvent> OnUpdated;

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

    [Flags]
    public enum CaptureInfObjects : int
    {
        /// <summary>
        /// Без информации.
        /// </summary>
        None = 0,
        /// <summary>
        /// Информация о курсоре.
        /// </summary>
        Cursor = 1 << 1,
        /// <summary>
        /// Информация о клавишах.
        /// </summary>
        Keys = 1 << 2,
        /// <summary>
        /// Базовая информация.
        /// </summary>
        Default = Cursor | Keys,
        /// <summary>
        /// Полная информация.
        /// </summary>
        Fully = -1
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TBitRate"></typeparam>
    /// <typeparam name="TVideoCodec"></typeparam>
    /// <typeparam name="TAreaKind"></typeparam>
    public abstract class CaptureSettings<TBitRate, TVideoCodec, TAreaKind> :
        ICaptureSettings<TBitRate, TVideoCodec, TAreaKind>
        where TBitRate : struct
        where TAreaKind : struct
        where TVideoCodec : struct
    {
        #region Properties

        public abstract ICaptureSettings<TBitRate, TVideoCodec, TAreaKind> Default { get; }

        public string OutputPath { get; set; } = string.Empty;

        public Rectangle Area { get; set; } = Rectangle.Empty;

        public TAreaKind AreaKind { get; set; } = GetFirstValue<TAreaKind>();

        public TVideoCodec VideoCodec { get; set; } = GetFirstValue<TVideoCodec>();

        public TBitRate Rate { get; set; } = GetFirstValue<TBitRate>();

        public int Fps { get; set; }

        #endregion Properties

        #region Methods

        // TODO: Убрать в отдельный модуль.

        public static Rectangle GetScreenAll()
        {
            var result = new Rectangle();

            Screen.AllScreens.ToList().ForEach(f => result = Rectangle.Union(result, f.Bounds));

            return result;
        }

        public static Rectangle GetScreenArea()
        {
            using (var selected = new TopForm())
            {
                if (selected.ShowDialog() == DialogResult.OK
                    && selected.w != 0
                    && selected.h != 0)
                {
                    // Hint: Должны быть кратны 2ум.
                    if ((selected.w & 1) != 0)
                        selected.w += 1;
                    if ((selected.h & 1) != 0)
                        selected.h += 1;

                    return new Rectangle(selected.l, selected.t, selected.w, selected.h);
                }

                return GetScreenAll();
            }
        }

        public static Rectangle GetScreenDevice()
        {
            return Rectangle.Empty;
            //return Screen.AllScreens.First(scr => scr.DeviceName.Equals(DeviceName)).Bounds;
        }

        public static Rectangle GetScreenWindow()
        {
            return Rectangle.Empty;
        }

        protected static T GetFirstValue<T>()
        {
            return typeof(T).IsEnum ? Enum.GetValues(typeof(T)).Cast<T>().First() : default(T);
        }

        #endregion Methods
    }

    public enum CaptureEvent
    {
        Started,
        Finished
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TBitRate"></typeparam>
    /// <typeparam name="TVideoCodec"></typeparam>
    /// <typeparam name="TAreaKind"></typeparam>
    public abstract class CaptureCommon<TBitRate, TVideoCodec, TAreaKind> :
        ICaptureController<TBitRate, TVideoCodec, TAreaKind>
        where TBitRate : struct
        where TAreaKind : struct
        where TVideoCodec : struct
    {
        #region Fields

        protected Dictionary<CaptureInfObjects, ICaptureInfObject> CaptureObjects { get; } =
            new Dictionary<CaptureInfObjects, ICaptureInfObject>();

        #endregion Fields

        #region Properties

        public ICaptureInfObject this[CaptureInfObjects key]
        {
            get => CaptureObjects.ContainsKey(key) ? CaptureObjects[key] : null;
            set => CaptureObjects[key] = value;
        }

        public CaptureSettings<TBitRate, TVideoCodec, TAreaKind> Settings { get; protected set; }

        public CaptureInfObjects Mods { get; set; } = CaptureInfObjects.Default;

        public virtual bool IsRecording { get; protected set; } = false;

        #endregion Properties

        #region Events

        public event EventHandler<CaptureEvent> OnUpdated;

        #endregion Events

        #region Methods

        /// <summary>
        /// Запуск захвата.
        /// </summary>
        public virtual void Start()
        {
            IsRecording = true;
            RaiseUpdated(CaptureEvent.Started);
        }

        /// <summary>
        /// Остановка захвата.
        /// </summary>
        public virtual void Stop()
        {
            IsRecording = false;
            RaiseUpdated(CaptureEvent.Finished);
        }

        ///// <summary>
        ///// Остановка записи.
        ///// </summary>
        //public virtual void Pause() { IsRecording = false; }

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
            if (Mods == CaptureInfObjects.None)
                return;

            foreach (CaptureInfObjects item in Enum.GetValues(typeof(CaptureInfObjects)).Cast<CaptureInfObjects>())
            {
                if ((Mods & item) == item && CaptureObjects.ContainsKey(item))
                    CaptureObjects[item]?.Draw(graphics, Settings.Area.Left, Settings.Area.Top);
            }
        }

        protected void RaiseUpdated(CaptureEvent @event)
        {
            EventHandler<CaptureEvent> handler = OnUpdated;
            handler?.Invoke(this, @event);
        }

        #endregion Methods
    }
}
