namespace Capture.Core
{
    using System.Drawing;

    public interface ICaptureInfObject
    {
        void Draw(Graphics graphics, int left, int top);
    }
}