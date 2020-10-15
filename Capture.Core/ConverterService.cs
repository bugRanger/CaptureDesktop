﻿namespace Capture.Core
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

        private Thread _thread;
        private ConcurrentQueue<Action> _queue;
        private int _action;

        #endregion Fields

        #region Events

        public event EventHandler<Exception> Error;

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

        public void Enqeue(string filePath, string fileExtension) 
        {
            _queue.Enqueue(() => Handle(filePath, fileExtension));
        }

        protected abstract void Handle(string filePath, string fileExtension);

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
                Error?.Invoke(this, ex);
            }
        }

        #endregion Methods
    }
}
