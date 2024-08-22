using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace iMonitor
{
    public class IRacingWatcher
    {
        private const string MemoryMappedFileName = "Local\\IRSDKMemMapFileName";
        private const string DataValidEventName = "Local\\IRSDKDataValidEvent";
        private readonly CancellationTokenSource _cancellationTokenSource;
		private bool _hasStarted;

		private TimeSpan _checkInterval;

		public TimeSpan CheckInterval
		{
			get { return _checkInterval; }
			set { _checkInterval = value; }
		}

        public bool IsStarted
        {
            get { return _hasStarted; }
        }

        public event EventHandler IRacingStarted;
		public event EventHandler IRacingStopped;

        public IRacingWatcher(double interval = 5)
        {
            _checkInterval = TimeSpan.FromSeconds(interval);
            _cancellationTokenSource = new CancellationTokenSource();
            _hasStarted = false;
        }

        public void StartMonitoring()
        {
            Task.Run(() =>
            {
                MonitorMemoryMappedFile(_cancellationTokenSource.Token);
            });
        }

        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task MonitorMemoryMappedFile(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var memMapFile = MemoryMappedFile.OpenExisting(MemoryMappedFileName))
                    {
                        //var hEvent = OpenEvent(2031619, false, DataValidEventName);
                        if (!_hasStarted)
                        {
                            OnIRacingStarted(EventArgs.Empty);
                            continue;
                        }
                        // else keep checking
                    }
                }
                catch (FileNotFoundException)
                {
                    if (_hasStarted)
                    {
                        OnIRacingStopped(EventArgs.Empty);
                        continue;
                    }
                    // else keep checking
                }
                catch (Exception ex)
                {
                    // handle other exceptions
                }

                await Task.Delay(_checkInterval, cancellationToken);
            }
        }

        protected virtual void OnIRacingStarted(EventArgs e)
        {
            _hasStarted = true;
            IRacingStarted?.Invoke(this, e);
        }

        protected virtual void OnIRacingStopped(EventArgs e)
        {
            _hasStarted = false;
            IRacingStopped?.Invoke(this, e);
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr OpenEvent(UInt32 dwDesiredAccess, Boolean bInheritHandle, String lpName);
    }
}
