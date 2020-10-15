namespace Capture.Core
{
    /// <summary>
    /// Настройки захвата.
    /// </summary>
    public interface ICaptureSettings
    {
        /// <summary>
        /// Путь до папки хранения.
        /// </summary>
        string OutputPath { get; set; }

        /// <summary>
        /// Тип захвата.
        /// </summary>
        AreaKind AreaKind { get; set; }

        /// <summary>
        /// Наименование области захвата.
        /// </summary>
        string AreaName { get; set; }

        /// <summary>
        /// Количество бит исп. для обработки данных.
        /// </summary>
        BitRate Rate { get; set; }

        /// <summary>
        /// Тип используемого сжатия.
        /// </summary>
        VideoCodec Codec { get; set; }

        /// <summary>
        /// Допустимые к использованию кодеки.
        /// </summary>
        VideoCodec[] AllowCodecs { get; set; }

        /// <summary>
        /// Количество кадров в секунду.
        /// </summary>
        int Fps { get; set; }

        ICaptureSettings Default { get; }
    }
}