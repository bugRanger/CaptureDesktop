namespace Capture.FFmpeg
{
    public interface IFFmpegSettings
    {
        string OutputPath { get; }

        string Options { get; }
    }
}
