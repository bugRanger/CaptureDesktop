namespace Capture.AForge
{
    using System;
    using System.IO;
    using Capture.Core;

    public class AForgeSettings : ICaptureSettings
    {
        #region Properties

        public string OutputPath { get; set; }

        public AreaKind AreaKind { get; set; }

        public string AreaName { get; set; }

        public BitRate Rate { get; set; }

        public int Fps { get; set; }

        public VideoCodec Codec { get; set; }

        public VideoCodec[] AllowCodecs { get; set; }

        public ICaptureSettings Default => Empty;

        public static AForgeSettings Empty => new AForgeSettings
        {
            AllowCodecs = new []{ VideoCodec.MPEG4 },
            Codec = VideoCodec.MPEG4,
            AreaKind = AreaKind.All,
            Rate = BitRate._5000kbit,
            Fps = 15,

            OutputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Templates),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),
        };

        #endregion Properties
    }
}
