namespace Capture.FFmpeg
{
    using System;
    using System.IO;
    using System.Diagnostics;

    using Capture.Core;

    public class FFmpegConverter : ConverterService
    {
        #region Constants

        private const string FILE_NAME = "ffmpeg.exe";
        private const int CONVERTION_TIMEOUT = 50000;

        #endregion Constants

        #region Fields

        private IFFmpegSettings _settings;

        #endregion Fields

        #region Constructors

        public FFmpegConverter(IFFmpegSettings settings) 
        {
            _settings = settings;
        }

        #endregion Constructors

        #region Methods

        protected override void Handle(string filePath, string fileExtension)
        {
            var ffmpegProcess = new Process
            {
                StartInfo =
                {
                    FileName = FILE_NAME,
                    // -c:v libx264 -c:a aac -strict experimental -b:a 192K 
                    //ffmpeg -i … -c:a copy -c:v libx264 -crf 18 -preset veryslow …
                    //string options = " -c:v libx264 -c:a aac -strict experimental -b:a 192K ", ext = "flv";
                    //string options = "-s 1280x720 -ar 44100 -async 44100 -r 29.970 -ac 2 -qscale 10", ext = "swf";
                    Arguments = $"-i \"{filePath}\" {_settings.Options} \"{Path.Combine(_settings.OutputPath, Path.GetFileNameWithoutExtension(filePath))}.{fileExtension}\" -y",
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardOutput = false
                }
            };

            ffmpegProcess.Start();
            ffmpegProcess.WaitForExit(CONVERTION_TIMEOUT);
        }

        #endregion Methods
    }
}
