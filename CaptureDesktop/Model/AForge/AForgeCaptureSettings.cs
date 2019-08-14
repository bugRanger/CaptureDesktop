using System;
using System.IO;
using System.Linq;
using System.Drawing;

using Capture;
using AForge.Video.FFMPEG;
using System.Windows.Forms;

namespace CaptureDesktop.Model.AForge
{
    public class AForgeCaptureSettings : CaptureSettings<BitRate, VideoCodec, AreaKind>
    {
        #region Properties

        public override ICaptureSettings<BitRate, VideoCodec, AreaKind> Default => Empty;

        public static AForgeCaptureSettings Empty => new AForgeCaptureSettings
        {
            OutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Templates),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),
            Area = GetScreenAll(),
            AreaKind = AreaKind.All,
            VideoCodec = VideoCodec.MPEG4,
            Rate = BitRate._5000kbit,
            Fps = 15,
        };


        #endregion Properties
    }
}
