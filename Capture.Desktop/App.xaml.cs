namespace Capture.Desktop
{
    using System.Windows;

    using Capture.FFmpeg;
    using Capture.AForge;

    using Capture.Core;
    using Capture.Core.Objects;
    using Capture.Desktop.ViewModel;
    using Capture.Desktop.View;

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

        private void OnStartup(object sender, StartupEventArgs e)
        {
            // TODO Loaded settings.
            // ...

            _converterService = new FFmpegConverter(
                new FFmpegSettings
                {
                    OutputPath = AForgeSettings.Empty.OutputPath,
                    Options = ""
                });

            _captureService = new AForgeCapture(AForgeSettings.Empty)
            {
                [CaptureInfObjects.Cursor] = new CursorCapture()
            };
            _captureService.OnFinished += (s, fname) => _converterService.Enqeue(fname, GIF_EXT);
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
