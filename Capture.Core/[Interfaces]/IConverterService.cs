namespace Capture.Core
{
    using System;

    public interface IConverterService
    {
        event EventHandler<Exception> OnError;

        event EventHandler<string> OnFinished;

        void Start();

        void Stop();

        void Enqeue(string filePath, string fileExtension);
    }
}
