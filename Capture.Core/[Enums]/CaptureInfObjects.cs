namespace Capture.Core
{
    using System;

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
        Fully = -1,
    }
}