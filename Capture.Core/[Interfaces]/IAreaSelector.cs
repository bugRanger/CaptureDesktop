namespace Capture.Core
{
    using System.Collections.Generic;
    using System.Drawing;

    public interface IAreaSelector
    {
        IEnumerable<string> Devices { get; }

        Rectangle GetScreenAll();

        Rectangle GetScreenArea(string deviceName);

        Rectangle GetScreenDevice(string deviceName);

        Rectangle GetScreenWindow();
    }
}