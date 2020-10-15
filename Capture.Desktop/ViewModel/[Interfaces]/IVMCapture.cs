namespace Capture.Desktop.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    public interface IVMCapture
    {
        ///// <summary>
        ///// Путь до папки хранения.
        ///// </summary>
        //string OutputPath { get; set; }

        /// <summary>
        /// Тип захвата.
        /// </summary>
        [DisplayName("Область")]
        AreaKindDesc AreaKind { get; set; }

        /// <summary>
        /// Наименование устройства.
        /// </summary>
        [DisplayName("Устройство")]
        string DeviceName { get; set; }

        /// <summary>
        /// Количество бит исп. для обработки данных.
        /// </summary>
        [DisplayName("Скорость")]
        BitRateDesc Rate { get; set; }

        /// <summary>
        /// Тип используемого сжатия.
        /// </summary>
        [DisplayName("Сжатие")]
        VideoCodecDesc Codec { get; set; }

        /// <summary>
        /// Доступные к исп. типы сжатия.
        /// </summary>
        VideoCodecDesc[] AllowCodecs { get; }

        /// <summary>
        /// Количество кадров в секунду.
        /// </summary>
        [DisplayName("Кадров")]
        int Fps { get; set; }

        bool IsStopped { get; }
        
        string[] DeviceArray { get; }

        ICommand CommandGetAllDevices { get; }

        ICommand CommandRecord { get; }

        ICommand CommandStop { get; }

        ICommand CommandClose { get; }
    }
}
