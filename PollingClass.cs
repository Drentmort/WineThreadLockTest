using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimerConsoleTest
{
    internal class PollingClass
    {
        private  readonly object _o = new object();
        private readonly PollerTimer _timer = new PollerTimer(500);
        public event Action<TimeSpan> TimeLockHandler;

        public void Start()
        {
            _timer.NeedPoll += () =>
            {
                var timer = new Stopwatch();
                timer.Start();
                Monitor.Enter(_o);
                Monitor.Exit(_o);
                timer.Stop();

                TimeLockHandler?.Invoke(timer.Elapsed);
            };
            _timer.Start(); 
        }
        public void Stop() => _timer.Stop();
    }

    public class PollerTimer
    {

        private const int PollingPeriodInMs = 500;
        private readonly Timer _pollerTimer;
        private readonly object _timerLocker = new object();
        private bool _isDisposed;
        private int _pollingPeriodInMs;

        public event Action NeedPoll = delegate { };

        public PollerTimer() : this(PollingPeriodInMs)
        {

        }

        public PollerTimer(int periodMs)
        {
            _pollingPeriodInMs = periodMs;
            _pollerTimer = new Timer(OnTimerTick, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            if (_isDisposed)
            {
                return;
            }
            _pollerTimer.Change(_pollingPeriodInMs, _pollingPeriodInMs);
        }

        public void Start(int periodMs)
        {
            ChangePeriod(periodMs);
            Start();
        }

        public void Stop()
        {
            if (_isDisposed)
            {
                return;
            }
            _pollerTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            _isDisposed = true;
            _pollerTimer.Dispose();
        }


        private void OnTimerTick(object state)
        {
            Stop();
            try
            {
                NeedPoll();
            }
            catch (Exception e)
            {

            }
            Start();
        }

        public void ChangePeriod(int periodMs)
        {
            _pollingPeriodInMs = periodMs;
        }
    }
}
