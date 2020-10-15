namespace Capture.Desktop.ViewModel
{
    using System.ComponentModel;
    using Common.Utils.Converter;

    using Core;

    /// <summary>
    /// Количество бит используемых для обработки данных в единицу времени.
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum BitRateDesc : int
    {
        [Description("50 Кбит")]
        _50kbit = BitRate._50kbit,
        [Description("100 Кбит")]
        _100kbit = BitRate._100kbit,
        [Description("500 Кбит")]
        _500kbit = BitRate._500kbit,
        [Description("1000 Кбит")]
        _1000kbit = BitRate._1000kbit,
        [Description("2000 Кбит")]
        _2000kbit = BitRate._2000kbit,
        [Description("3000 Кбит")]
        _3000kbit = BitRate._3000kbit,
        [Description("4000 Кбит")]
        _4000kbit = BitRate._4000kbit,
        [Description("5000 Кбит")]
        _5000kbit = BitRate._5000kbit,
    }
}
