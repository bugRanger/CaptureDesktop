namespace Capture.Desktop
{
    using System;
    using System.Windows;
    using System.Collections.Specialized;

    using Capture.FFmpeg;
    using Capture.AForge;

    using Capture.Core;
    using Capture.Core.Objects;
    using Capture.Desktop.ViewModel;
    using Capture.Desktop.View;
    using System.Threading;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constants

        public const string GIF_EXT = ".gif";

        #endregion Constants

        #region Fields

        private IConverterService _converterService;

        private ICaptureService _captureService;

        #endregion Fields

        #region Methods

        private void OnMessageError(object sender, Exception ex) => MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);

        private void OnStartup(object sender, StartupEventArgs e)
        {
            // TODO Loaded settings.
            // ...

            _captureService = new AForgeCapture(AForgeSettings.Empty) { [CaptureInfObjects.Cursor] = new CursorCapture() };
            _captureService.OnFinished += (s, fname) => _converterService.Enqeue(fname, GIF_EXT);
            _captureService.OnError += OnMessageError;

            _converterService = new FFmpegConverter(new FFmpegSettings { OutputPath = AForgeSettings.Empty.OutputPath, Options = "" });
            _converterService.OnFinished += (s, fname) =>
            {
                Thread thread = new Thread(() => Clipboard.SetDataObject(new DataObject(DataFormats.FileDrop, new string[] { fname }), true));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            };
            _converterService.OnError += OnMessageError;
            _converterService.Start();

            // Startup.
            new MainWindow
            {
                DataContext = new VmCapture(_captureService)
            }
            .Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _converterService.Stop();
        }

        #endregion Methods
    }
}
