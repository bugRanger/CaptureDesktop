using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;

using System.Diagnostics;

using AForge.Video.FFMPEG;

namespace CaptureDesktop.ViewModel.AForge
{
    using Capture;

    using Model;
    using Model.AForge;
    using Model.CursorCapture;

    using DevExpress.Xpf.Core;

    using Sima.Common.WPF.Tools.Command;
    using Sima.Common.WPF.Tools.PropertyAttribute;

    /// <summary>
    /// Количество бит используемых для обработки данных в единицу времени.
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
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
    /// <summary>
    /// Тип захвата видео.
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
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
    /// <summary>
    /// Тип используемого сжатия.
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
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

    public class VmAForgeCapture : ICaptureSettings<BitRateDesc, VideoCodecDesc, AreaKindDesc>, INotifyPropertyChanged
    {
        public VmAForgeCapture()
        {
            _capture[CaptureInfObjects.Cursor] = new CursorCapture();
            //SetEvent();
        }

        protected void SetEvent()
        {
            //_capture.Recording += (s, e) =>
            //{
            //    OnPropertyChanged(nameof(DirPath));
            //    OnPropertyChanged(nameof(FileName));
            //    OnPropertyChanged(nameof(IsRecordBlock));
            //};
            //_capture.Recorded += (s, e) =>
            //{
            //    if (true)
            //    {
            //        // -c:v libx264 -c:a aac -strict experimental -b:a 192K 
            //        //ffmpeg -i … -c:a copy -c:v libx264 -crf 18 -preset veryslow …

            //        string options = " -c:v libx264 -c:a aac -strict experimental -b:a 192K ";//flv
            //        //string options = "-s 1280x720 -ar 44100 -async 44100 -r 29.970 -ac 2 -qscale 10";//swf
            //        string fileargs = $"-i \"{ _capture.FullName}\" " + options + $" \"{Path.Combine(_capture.DirPath, _capture.FileName)}.flv\" -y";
            //        System.Diagnostics.Process p = new System.Diagnostics.Process();
            //        p.StartInfo.FileName = "ffmpeg.exe";
            //        p.StartInfo.Arguments = fileargs;
            //        p.StartInfo.UseShellExecute = false;
            //        p.StartInfo.CreateNoWindow = false;
            //        p.StartInfo.RedirectStandardOutput = false;
            //        p.Start();
            //        //string str_output = p.StandardOutput.ReadToEnd();
            //        p.WaitForExit(50000);

            //        //Открываем расположение(с выделением файла).
            //        //System.Diagnostics.Process.Start("cmd.exe", $"call ffmpeg -i {_capture.FullName} {Path.GetFileNameWithoutExtension(_capture.FullName)}.swf -y");
            //    }
            //    if (true)
            //    {
            //        //Открываем расположение(с выделением файла).
            //        System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{_capture.FullName}\"");
            //    }
            //    OnPropertyChanged(nameof(IsRecordBlock));
            //};
        }

        private HotKey _hotKey = null;
        private AForgeCapture _capture { get; } = new AForgeCapture(new AForgeCaptureSettings());

        public string OutputPath
        {
            get { return _capture.Settings.OutputPath; }
            set { _capture.Settings.OutputPath = value; OnPropertyChanged(nameof(OutputPath)); }
        }

        public Rectangle Area
        {
            get { return _capture.Settings.Area; }
            set { _capture.Settings.Area = value; OnPropertyChanged(nameof(Area)); }
        }

        public AreaKindDesc AreaKind
        {
            get { return (AreaKindDesc)_capture.Settings.AreaKind; }
            set { _capture.Settings.AreaKind = (AreaKind)value; OnPropertyChanged(nameof(AreaKind)); }
        }

        public BitRateDesc Rate
        {
            get { return (BitRateDesc)_capture.Settings.Rate; }
            set { _capture.Settings.Rate = (BitRate)value; OnPropertyChanged(nameof(Rate)); }
        }

        public VideoCodecDesc VideoCodec
        {
            get { return (VideoCodecDesc)_capture.Settings.VideoCodec; }
            set { _capture.Settings.VideoCodec = (VideoCodec)value; OnPropertyChanged(nameof(VideoCodec)); }
        }

        public int Fps
        {
            get { return _capture.Settings.Fps; }
            set { _capture.Settings.Fps = value; OnPropertyChanged(nameof(Fps)); }
        }

        /// <summary>
        /// Получить ссылку на окно.
        /// </summary>
        /// <param name="sender">Объект содержащий ссылку на окно</param>
        /// <returns>В случае успеха вернет ссылку на окно, в противном случае - null.</returns>
        public Window GetWindow(object sender)
        {
            //Получаем ссылку на родительское окно(если оно было указанно).
            return sender is Window ? (Window)sender :
                sender is DependencyObject ? Window.GetWindow((DependencyObject)sender) : null;
        }
        private void OnHotKeyFromStartOrStop(HotKey hotKey)
        {
            //CommandRecord?.Execute(hotKey is HotKeyWnd ? (hotKey as HotKeyWnd).Owner : null);
        }

        /// <summary>
        /// Подписка на событие уведомления об изменениях свойств.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Вызов уведомления об изменениях свойств. 
        /// </summary>
        /// <param name="prop">Наименование свойства.</param>
        protected void OnPropertyChanged(string prop)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        //[DisplayName("Выбрать")]
        //public ICommand ClickSelectDir
        //{
        //    get
        //    {
        //        return new DelegateCommand((obj) =>
        //        {
        //            //Вызываем диалог ля выбора директории.
        //            var dialog = new FolderBrowserDialog();
        //            //Проверка результата.
        //            if (dialog.ShowDialog() == DialogResult.OK)
        //            {
        //                //Присвоение результата.
        //                OutputPath = dialog.SelectedPath;
        //            }
        //        });
        //    }
        //}
        //[DisplayName("Тест")]
        //public ICommand ClickHotKeys
        //{
        //    get
        //    {
        //        return new DelegateCommand((obj) =>
        //        {
        //            _hotKey?.Dispose();
        //            _hotKey = new HotKeyWnd(GetWindow(obj), Key.F9, KeyModifier.Shift | KeyModifier.Win, CommandRecord);
        //        });
        //    }
        //}

        //[DisplayName("Обновить")]
        //public ICommand ClickGetAllDevices
        //{
        //    get
        //    {
        //        return new DelegateCommand((obj) =>
        //        {
        //            DeviceArray = _capture.GetAllDevices();
        //        }, (obj) => { return _capture != null; });
        //    }
        //}
        //[DisplayName("Выделить")]
        //public ICommand ClickSettingArea
        //{
        //    get
        //    {
        //        return new DelegateCommand((obj) =>
        //        {
        //            //Настройка.
        //            _capture.SetArea(this);
        //        },
        //        (obj) =>
        //        {
        //            return
        //                !_capture.HasRecord()
        //                && AreaKind == AreaKindDesc.skArea;
        //        });
        //    }
        //}

        [DisplayName("Запуск")]
        public ICommand ClickStartAsync
        {
            get
            {
                return
                    new DelegateCommand((obj) =>
                    {
                        try
                        {
                            ////Настройка.
                            //_capture.SetOptions(this);
                            //if ((this as IAreaCaptureOptions).AreaKind != _capture.AreaKind || !_capture.HasArea())
                            //    _capture.SetArea(this);
                            //Запуск захвата.
                            _capture.Start();
                        }
                        catch (Exception ex)
                        {
                            DXMessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                    },
                    (obj) => { return !_capture.IsRecording; });
            }
        }
        [DisplayName("Стоп")]
        public ICommand ClickStopAsync
        {
            get
            {
                return
                    new DelegateCommand((obj) =>
                    {
                        //Остановка захвата.
                        _capture.Stop();
                    },
                    (obj) => { return _capture.IsRecording; });
            }
        }

        //public ICommand CommandRecord
        //{
        //    get
        //    {
        //        return new DelegateCommand((obj) =>
        //        {
        //            var hot = obj is HotKeyWnd ? (obj as HotKeyWnd).Owner : null;
        //            var wnd = GetWindow(hot);
        //            if ((bool)ClickStartAsync?.CanExecute(wnd))
        //            {
        //                if (wnd != null && true)
        //                {
        //                    wnd.WindowState = WindowState.Minimized;
        //                }
        //                ClickStartAsync?.Execute(wnd);
        //            }
        //            else
        //            {
        //                ClickStopAsync?.Execute(wnd);
        //            }
        //        });
        //    }
        //}
        //public ICommand CommandLoaded
        //{
        //    get
        //    {
        //        return
        //            new DelegateCommand((obj) =>
        //            {
        //                ClickHotKeys?.Execute(obj);
        //            });
        //    }
        //}
        //public ICommand CommandClose
        //{
        //    get
        //    {
        //        return
        //            new DelegateCommand((obj) =>
        //            {
        //                _hotKey?.Dispose();
        //                //Остановка захвата.
        //                ClickStopAsync?.Execute(obj);
        //            });
        //    }
        //}

    }
}
