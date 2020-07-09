using System.Drawing;

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
}
