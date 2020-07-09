using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CaptureDesktop.ViewModel
{
    public interface IVMCapture
    {
        /// <summary>
        /// Путь до папки хранения.
        /// </summary>
        string OutputPath { get; set; }
        /// <summary>
        /// Наименование файла.
        /// </summary>
        string FileName { get; set; }
        /// <summary>
        /// Тип захвата.
        /// </summary>
        [DisplayName("Область")]
        object AreaKind { get; set; }
        /// <summary>
        /// Наименование устройства.
        /// </summary>
        [DisplayName("Устройство")]
        string DeviceName { get; set; }
        /// <summary>
        /// Количество бит исп. для обработки данных.
        /// </summary>
        [DisplayName("Скорость")]
        object Rate { get; set; }
        /// <summary>
        /// Тип используемого сжатия.
        /// </summary>
        [DisplayName("Сжатие")]
        object Codec { get; set; }
        /// <summary>
        /// Количество кадров в секунду.
        /// </summary>
        [DisplayName("Кадров")]
        int Fps { get; set; }
        
        bool IsRecording { get; }
        
        string[] DeviceArray { get; }        

        [DisplayName("Выбрать")]
        ICommand ClickSelectDir { get; }
        [DisplayName("Назначить")]
        ICommand ClickSetHotKey { get; }
        [DisplayName("Обновить")]
        ICommand ClickGetDeviceList { get; }
        [DisplayName("Выделить")]
        ICommand ClickSettingArea { get; }
        [DisplayName("Запуск")]
        ICommand ClickStartAsync { get; }
        [DisplayName("Стоп")]
        ICommand ClickStopAsync { get; }        

        ICommand CommandLoaded { get; }
        ICommand CommandClose { get; }
    }
}
