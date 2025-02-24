using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Motion.Model
{
    public enum LogType
    {
        Info, Warning, Error
    }

    public partial class LogModel : ObservableObject
    {
        [ObservableProperty]
        public string _Time;


        private LogType type = LogType.Info;

        [ObservableProperty]
        public string _Message;

        [ObservableProperty]
        public bool _IsVisible=true;

        [ObservableProperty]
        private SolidColorBrush foreground;

        public LogType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
                SetProperty(ref type, value);
                switch (type)
                {
                    case LogType.Info:
                        Foreground = new SolidColorBrush(Colors.Green);
                        break;
                    case LogType.Warning:
                        Foreground = new SolidColorBrush(Colors.Orange);
                        break;
                    case LogType.Error:
                        Foreground = new SolidColorBrush(Colors.Red);
                        break;
                }
            }
        }
    }
}
