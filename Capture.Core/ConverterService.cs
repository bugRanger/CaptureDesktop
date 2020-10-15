namespace Capture.Core
{
    using System;
    using System.Threading;
    using System.Collections.Concurrent;

    public abstract class ConverterService : IConverterService
    {
        #region Constants

        private const int WAIT_INTERVAL = 500;

        private const int ENABLED = 1;
        private const int DISABLED = 0;

        #endregion Constants

        #region Fields

        private readonly ConcurrentQueue<Action> _queue;
        private Thread _thread;
        private int _action;

        #endregion Fields

        #region Events

        public event EventHandler<Exception> OnError;
        public event EventHandler<string> OnFinished;

        #endregion Events

        #region Constructors

        public ConverterService()
        {
            _action = DISABLED;
            _queue = new ConcurrentQueue<Action>();
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            if (Interlocked.CompareExchange(ref _action, ENABLED, DISABLED) == ENABLED)
                return;

            _thread = new Thread(() => Work());
            _thread.Start();
        }

        public void Stop()
        {
            if (Interlocked.CompareExchange(ref _action, DISABLED, ENABLED) != DISABLED)
                return;

            _thread.Join();
        }

        public void Enqeue(string sourcePath, string extension)
        {
            _queue.Enqueue(() =>
            {
                if (Handle(sourcePath, extension, out var destPath))
                    OnFinished?.Invoke(this, destPath);
            });
        }

        protected abstract bool Handle(string sourcePath, string extension, out string destPath);

        private void Work()
        {
            try
            {
                while (_action == ENABLED)
                {
                    if (!_queue.TryDequeue(out Action action))
                    {
                        Thread.Sleep(WAIT_INTERVAL);
                        continue;
                    }

                    action();
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
            }
        }

        #endregion Methods
    }
}
