namespace Capture.Core
{
    public interface IConverterService
    {
        void Start();

        void Stop();

        void Enqeue(string filePath, string fileExtension);
    }
}
