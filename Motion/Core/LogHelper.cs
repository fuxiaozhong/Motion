using CommunityToolkit.Mvvm.ComponentModel;
using log4net;
using MahApps.Metro.Controls;
using Motion.Config;
using Motion.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Motion.Core
{
    public partial class LogHelper : INotifyPropertyChanged
    {
        #region

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static void OnStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private static int infoNum1;
        private static int warningNum1;
        private static int errorNum1;
        private static int allNum;

        private static bool infoClick = true;
        private static bool warnClick = true;
        private static bool errorClick = true;
        private static bool allClick = true;


        public static ObservableCollection<LogModel> Logs
        {
            get
            {
                return logs;
            }
            set
            {
                logs = value;
                OnStaticPropertyChanged();
            }
        }
        public static MainWindow mainWindow;
        private static ObservableCollection<LogModel> logs = new ObservableCollection<LogModel>();
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        public static int InfoNum
        {
            get
            {
                return infoNum1;
            }

            set
            {
                infoNum1 = value;
                OnStaticPropertyChanged();
            }
        }

        public static int WarningNum
        {
            get
            {
                return warningNum1;
            }

            set
            {
                warningNum1 = value;
                OnStaticPropertyChanged();
            }
        }

        public static int ErrorNum
        {
            get
            {
                return errorNum1;
            }

            set
            {
                errorNum1 = value;
                OnStaticPropertyChanged();
            }
        }

        public static int AllNum
        {
            get
            {
                return allNum;
            }

            set
            {
                allNum = value;
                OnStaticPropertyChanged();
            }
        }

        public static bool InfoClick
        {
            get
            {
                return infoClick;
            }

            set
            {
                infoClick = value;
            }
        }

        public static bool WarnClick
        {
            get
            {
                return warnClick;
            }

            set
            {
                warnClick = value;
            }
        }

        public static bool ErrorClick
        {
            get
            {
                return errorClick;
            }

            set
            {
                errorClick = value;
            }
        }

        public static bool AllClick
        {
            get
            {
                return allClick;
            }

            set
            {
                allClick = value;
            }
        }

        public static void Info(string message)
        {
            mainWindow.Dispatcher.Invoke(() =>
            {
                AllNum++;
                InfoNum++;
                log.Info(message);
                Logs.Insert(0, new Model.LogModel()
                {
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    Type = Model.LogType.Info,
                    Message = message,
                    IsVisible = InfoClick
                });
                if (Logs.Count > 300)
                {
                    Logs.RemoveAt(Logs.Count - 1);
                    InfoNum--;
                    AllNum--;
                }
            });
        }

        public static void Warning(string message)
        {
            mainWindow.Dispatcher.Invoke(() =>
            {
                AllNum++;
                WarningNum++;
                log.Warn(message);
                Logs.Insert(0, new Model.LogModel()
                {
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    Type = Model.LogType.Warning,
                    Message = message,
                    IsVisible = WarnClick
                });
                if (Logs.Count > 300)
                {
                    Logs.RemoveAt(Logs.Count - 1);
                    WarningNum--;
                    AllNum--;
                }
            });
        }

        public static void Error(string message)
        {
            mainWindow.Dispatcher.Invoke(() =>
            {
                AllNum++;
                ErrorNum++;
                log.Error(message);
                Logs.Insert(0, new Model.LogModel()
                {
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    Type = Model.LogType.Error,
                    Message = message
                    ,IsVisible = ErrorClick
                });
                if (Logs.Count > 300)
                {
                    Logs.RemoveAt(Logs.Count - 1);
                    ErrorNum--;
                    AllNum--;
                }
            });
        }

        public static void Error(string message, Exception exception)
        {

        }










    }
}
