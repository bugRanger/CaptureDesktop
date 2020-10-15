namespace Capture.Desktop.ViewModel
{
    using System.ComponentModel;
    using Common.Utils.Converter;

    using Core;

    /// <summary>
    /// Тип захвата видео.
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum AreaKindDesc : int
    {
        /// <summary>
        /// Захват всех источников.
        /// </summary>
        [Description("Все")]
        All = AreaKind.All,
        /// <summary>
        /// Захват области.
        /// </summary>
        [Description("Выделенная")]
        Area = AreaKind.Area,
        /// <summary>
        /// Захват устройства.
        /// </summary>
        [Description("Устройство")]
        Device = AreaKind.Device,
        /// <summary>
        /// Захват окна.
        /// </summary>
        [Description("Окно")]
        Window = AreaKind.Window,
    }
}
