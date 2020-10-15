namespace Capture.Desktop.ViewModel
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.ComponentModel;


    using Common.Utils.Command;
    using Common.Utils.Keys;

    using Core;

    public class VmCapture : IVMCapture, INotifyPropertyChanged
    {
        #region Fields

        private readonly HotKey _hotKey;

        private readonly ICaptureService _capture;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Наименование устройства.
        /// </summary>
        [DisplayName("Устройство")]
        public string DeviceName
        {
            get => _capture.Settings.AreaName;
            set
            {
                _capture.Settings.AreaName = value;
                OnPropertyChanged(nameof(DeviceName));
            }
        }

        private string[] _deviceArray;
        public string[] DeviceArray
        {
            get => _deviceArray;
            set
            {
                _deviceArray = value;
                OnPropertyChanged(nameof(DeviceArray));
            }
        }

        public string OutputPath
        {
            get => _capture.Settings.OutputPath;
            set
            {
                _capture.Settings.OutputPath = value; 
                OnPropertyChanged(nameof(OutputPath));
            }
        }

        public AreaKindDesc AreaKind
        {
            get => (AreaKindDesc)_capture.Settings.AreaKind;
            set 
            { 
                _capture.Settings.AreaKind = (AreaKind)value; 
                OnPropertyChanged(nameof(AreaKind)); 
            }
        }

        public BitRateDesc Rate
        {
            get => (BitRateDesc)_capture.Settings.Rate;
            set
            { 
                _capture.Settings.Rate = (BitRate)value; 
                OnPropertyChanged(nameof(Rate)); 
            }
        }

        public VideoCodecDesc Codec
        {
            get => (VideoCodecDesc)_capture.Settings.Codec;
            set
            {
                _capture.Settings.Codec = (VideoCodec)value;
                OnPropertyChanged(nameof(VideoCodec));
            }
        }

        public VideoCodecDesc[] AllowCodecs => _capture.Settings.AllowCodecs.Select(s => (VideoCodecDesc)s).ToArray();

        public int Fps
        {
            get => _capture.Settings.Fps;
            set
            {
                _capture.Settings.Fps = value;
                OnPropertyChanged(nameof(Fps));
            }
        }

        public bool IsStopped => _capture.State != CaptureState.Started;

        #endregion Properties

        #region Constructors

        public VmCapture(ICaptureService capture)
        {
            // TODO Add hot key manager.
            _hotKey = new HotKey(Key.R, KeyModifier.Alt | KeyModifier.Shift, CommandRecord);

            _capture = capture;
            _deviceArray = _capture.Selector.Devices.ToArray();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Подписка на событие уведомления об изменениях свойств.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Methods

        /// <summary>
        /// Получить ссылку на окно.
        /// </summary>
        /// <param name="sender">Объект содержащий ссылку на окно</param>
        /// <returns>В случае успеха вернет ссылку на окно, в противном случае - null.</returns>
        public Window GetWindow(object sender)
        {
            //Получаем ссылку на родительское окно(если оно было указанно).
            return sender is Window window ? window :
                sender is DependencyObject dependencyObject ? Window.GetWindow(dependencyObject) : null;
        }

        /// <summary>
        /// Вызов уведомления об изменениях свойств. 
        /// </summary>
        /// <param name="prop">Наименование свойства.</param>
        protected void OnPropertyChanged(string prop)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion Methods

        #region Commands

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

        [DisplayName("Обновить")]
        public ICommand CommandGetAllDevices
        {
            get
            {
                return
                    new DelegateCommand((obj) =>
                    {
                        DeviceArray = Screen.AllScreens.Select(s => s.DeviceName).ToArray();
                    },
                    (obj) => !IsStopped);
            }
        }

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
        public ICommand CommandRecord
        {
            get
            {
                return
                    new DelegateCommand((obj) =>
                    {
                        //Запуск захвата.
                        _capture.Record();
                        OnPropertyChanged(nameof(IsStopped));
                    },
                    (obj) => IsStopped);
            }
        }

        [DisplayName("Стоп")]
        public ICommand CommandStop
        {
            get
            {
                return
                    new DelegateCommand((obj) =>
                    {
                        try
                        {
                            //Остановка захвата.
                            _capture.Stop();
                        }
                        finally
                        {
                            OnPropertyChanged(nameof(IsStopped));
                        }
                    },
                    (obj) => !IsStopped);
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

        public ICommand CommandClose
        {
            get
            {
                return
                    new DelegateCommand((obj) =>
                    {
                        _hotKey?.Dispose();
                        //Остановка захвата.
                        CommandStop?.Execute(obj);
                    });
            }
        }

        #endregion Commands
    }
}
