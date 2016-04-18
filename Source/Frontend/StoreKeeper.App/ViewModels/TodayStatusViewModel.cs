using System;
using System.Timers;
using CommonBase.Application;

namespace StoreKeeper.App.ViewModels
{
    public class TodayStatusViewModel : ViewModelBase
    {
        private readonly Timer _timer;

        public TodayStatusViewModel()
        {
            _timer = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds);
            _timer.Elapsed += TimerProc;
            _timer.Start();
        }

        public string TodayString
        {
            get
            {
                DateTime now = DateTime.Now;
                return String.Format("{0}.{1}.{2}", now.Day.ToString("D2"), now.Month.ToString("D2"), now.Year.ToString("D4"));
            }
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void TimerProc(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            NotifyPropertyChanged("TodayString");
        }
    }
}