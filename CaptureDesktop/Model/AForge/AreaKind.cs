namespace CaptureDesktop.Model.AForge
{
    /// <summary>
    /// Тип захвата видео.
    /// </summary>
    public enum AreaKind : int
    {
        /// <summary>
        /// Захват всех источников.
        /// </summary>
        All,
        /// <summary>
        /// Захват области.
        /// </summary>
        Area,
        /// <summary>
        /// Захват окна.
        /// </summary>
        Window,
        /// <summary>
        /// Захват устройства.
        /// </summary>
        Device
    }
}
