namespace Capture.FFmpeg
{
    using System;

    public class FFmpegSettings : IFFmpegSettings
    {
        public string OutputPath { get; set; }

        public string Options { get; set; }
    }
}
