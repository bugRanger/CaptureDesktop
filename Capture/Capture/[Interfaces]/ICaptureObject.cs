namespace Capture
{
    using System.Drawing;

    public interface ICaptureObject
    {
        void Draw(Graphics graphics, int left, int top);
    }
}
