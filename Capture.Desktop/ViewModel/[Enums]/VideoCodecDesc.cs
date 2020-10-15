namespace Capture.Desktop.ViewModel
{
    using System.ComponentModel;
    using Common.Utils.Converter;

    using Capture.Core;

    /// <summary>
    /// Тип используемого сжатия.
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum VideoCodecDesc
    {
        [Description("По умолчанию")]
        Default = VideoCodec.Default,
        FLV1 = VideoCodec.FLV1,
        H263P = VideoCodec.H263P,
        MPEG2 = VideoCodec.MPEG2,
        MPEG4 = VideoCodec.MPEG4,
        MSMPEG4v2 = VideoCodec.MSMPEG4v2,
        MSMPEG4v3 = VideoCodec.MSMPEG4v3,
        Raw = VideoCodec.Raw,
        WMV1 = VideoCodec.WMV1,
        WMV2 = VideoCodec.WMV2,
    }
}
