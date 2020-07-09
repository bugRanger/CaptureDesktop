using System;
using System.Linq;
using System.Drawing;
using CaptureDesktop.Model;
using System.Windows.Forms;

namespace Capture
{
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
}
