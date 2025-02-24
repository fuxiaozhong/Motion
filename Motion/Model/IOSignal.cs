using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Motion.Model
{
    public class IOSignal : INotifyPropertyChanged
    {
        private string _name;
        private bool _status;
        private string _mark = "";
        private bool _shield = false;
        private bool _reversal = false;
        private double _reversalTime = 0.1;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [JsonIgnore]
        public bool Status
        {
            get { return _status; }
            set
            {
                _status = value;
                UpdatePropertyAsync(nameof(Status));
            }
        }

        public string Mark
        {
            get
            {
                return _mark;
            }

            set
            {
                _mark = value;
                UpdatePropertyAsync(nameof(Mark));
            }
        }

        public bool Shield
        {
            get
            {
                return _shield;
            }

            set
            {
                _shield = value;
                UpdatePropertyAsync(nameof(Shield));
            }
        }

        public bool Reversal
        {
            get
            {
                return _reversal;
            }

            set
            {
                _reversal = value;
                UpdatePropertyAsync(nameof(Reversal));
            }
        }

        public double ReversalTime
        {
            get
            {
                return _reversalTime;
            }

            set
            {
                _reversalTime = value;
                UpdatePropertyAsync(nameof(ReversalTime));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void UpdatePropertyAsync(string propertyName)
        {
            await Task.Run(() =>
            {
                // 模拟一些耗时操作
                System.Threading.Thread.Sleep(100);
            });

            // 在UI线程上更新属性
            Application.Current.Dispatcher.Invoke(() =>
            {
                OnPropertyChanged(propertyName);
            });
        }
    }
}
