using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ann.Core
{
    public class InputQueue : IDisposable
    {
        public InputQueue()
        {
            Start();
        }

        public void Push(Action action)
        {
            lock (_lockObj)
            {
                _inputData = new InputData(action);

                if (_isRunning)
                {
                    if (_isActive)
                        return;

                    _isActive = true;
                    RunJob();
                }
            }
        }

        private class InputData
        {
            private readonly Action _Action;

            public InputData(Action action)
            {
                _Action = action;
            }

            public void InvokeAction()
            {
                _Action?.Invoke();
            }
        }

        private InputData _inputData;
        private volatile bool _isActive;
        private bool _isRunning;
        private readonly object _lockObj = new object();

        private void Start()
        {
            Debug.Assert(_isRunning == false);

            _isRunning = true;
            _isActive = true;

            RunJob();
        }

        private void RunJob()
        {
            InputData current;

            lock (_lockObj)
            {
                if (_inputData == null || _disposeResetEvent != null)
                {
                    _isActive = false;
                    _disposeResetEvent?.Set();
                    return;
                }

                current = _inputData;
                _inputData = null;
            }

            Task.Run(() => current.InvokeAction())
                .ContinueWith(_ => RunJob());
        }

        private ManualResetEventSlim _disposeResetEvent;

        private void Release()
        {
            lock (_lockObj)
            {
                if (_isActive)
                    _disposeResetEvent = new ManualResetEventSlim();
            }

            _disposeResetEvent?.Wait();
            _disposeResetEvent?.Dispose();
        }

        #region IDisposable

        private bool _isDisposed;

        ~InputQueue()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                Release();

            _isDisposed = true;
        }

        #endregion
    }
}