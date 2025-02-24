using Motion.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Motion.Config
{
    public class SystemState : INotifyPropertyChanged
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

        private static string _nowTime = "0000/00/00 00:00:00";
        private static string _nowUser = "操作员";
        private static string _nowRunState = "未初始化";
        private static bool _PAUSE = false;
        private static bool _EMERGENCY_STOP = false;
        /// <summary>
        /// 当前时间
        /// </summary>
        public static string NowTime
        {
            get
            {
                return _nowTime;
            }

            set
            {
                _nowTime = value;
                OnStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 当前用户
        /// </summary>
        public static string NowUser
        {
            get
            {
                return _nowUser;
            }

            set
            {
                _nowUser = value;
                OnStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 当前系统状态
        /// </summary>
        public static string NowRunState
        {
            get
            {
                return _nowRunState;
            }

            set
            {
                _nowRunState = value;
                OnStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public static bool PAUSE
        {
            get
            {
                return _PAUSE;
            }

            set
            {
                _PAUSE = value;
                OnStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 紧急停止
        /// </summary>
        public static bool EMERGENCY_STOP
        {
            get
            {
                return _EMERGENCY_STOP;
            }

            set
            {
                _EMERGENCY_STOP = value;
                OnStaticPropertyChanged();
            }
        }
    }
}    