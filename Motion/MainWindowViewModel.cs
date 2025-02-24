using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using Motion.Config;
using Motion.Core;
using Motion.Model;
using Motion.View;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ControlzEx.Theming;
using Motion.ViewModel;
using Motion.MotionBoard;

namespace Motion
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private PerformanceCounter cpuCounter;
        private readonly IDialogCoordinator _dialogCoordinator;
        public List<ButtonViewModel> Modules { get; set; }

        [ObservableProperty]
        public DateTime startingTime;

        [ObservableProperty]
        public DateTime adminLoginTime;

        [ObservableProperty]
        public string runTime;

        [ObservableProperty]
        public string? _Title = "TEST";

        [ObservableProperty]
        public string? _TitleSelectName = "主页面";

        [ObservableProperty]
        public float cpuUsage;

        [ObservableProperty]
        private object _currentPage;

        private DispatcherTimer _timer;
        private int _elapsedSeconds;

        public RelayCommand ButtonPressedCommand { get; }
        public RelayCommand ButtonReleasedCommand { get; }

        public RelayCommand<Window> LoadCommand
        {
            get
            {
                return new RelayCommand<Window>((w) =>
                {
                    if (w is MainWindow window)
                    {
                        Title = "运动控制测试软件";
                        StartingTime = DateTime.Now;
                        StartTimer();
                        LogHelper.Info("软件启动成功");


                        MotionClass.motion = new Googol();
                        MotionClass.motion.Initialize(0);
                        if (MotionClass.motion.CordInitState)
                        {
                            LogHelper.Info("板卡启动成功");
                        }
                        else
                        {
                            LogHelper.Error("板卡启动失败");
                        }


                    }
                });
            }
        }

        public ICommand ShowMessageDialogCommand
        {
            get
            {
                return new SimpleCommand<string>(
                    x => !string.IsNullOrEmpty(x),
                    x => PerformDialogCoordinatorAction(() =>
                    {
                        this._dialogCoordinator.ShowMessageAsync(this, $"Tips", x!).ContinueWith(t => Console.WriteLine(t.Result));
                    }, x == "")
                );
            }
        }

        /// <summary>
        /// 启动按钮
        /// </summary>
        public RelayCommand StartCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                });
            }
        }

        /// <summary>
        /// 复位错误按钮
        /// </summary>
        public RelayCommand ResetErrorCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                });
            }
        }

        /// <summary>
        /// 长按回原按钮
        /// </summary>
        public RelayCommand ResetGoHomeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                });
            }
        }

        /// <summary>
        /// 停止按钮
        /// </summary>
        public RelayCommand StopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                });
            }
        }

        public RelayCommand<Button> OpenCommand
        {
            get
            {
                return new RelayCommand<Button>((s) =>
                {
                    if (s == null) return;
                    TitleSelectName = s.Content.ToString();

                    switch (s.Content)
                    {
                        case "用户登录":
                            HandleUserLogin();
                            return;
                        default:
                            CurrentPage = Modules.FirstOrDefault(o => o.Name == s.Content.ToString())?.Page;
                            break;
                    }

                    UpdateButtonForeground();
                });
            }
        }

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            }
            catch (Exception ex)
            {
                LogHelper.Error($"初始化性能计数器失败: {ex.Message}");
            }
            _dialogCoordinator = dialogCoordinator;

            ButtonPressedCommand = new RelayCommand(OnButtonPressed);
            ButtonReleasedCommand = new RelayCommand(OnButtonReleased);


            InitializeConfig();
            InitializeModules();
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateCurrentTime();
        }

        private void UpdateCurrentTime()
        {
            SystemState.NowTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            TimeSpan timeDifference = DateTime.Now - StartingTime;
            RunTime = $"运行时间: {timeDifference.Days} 天 {timeDifference.Hours} 小时 {timeDifference.Minutes} 分钟 {timeDifference.Seconds} 秒";

            if ((DateTime.Now - AdminLoginTime).TotalSeconds > 300 && SystemState.NowUser == "管理员")
            {
                LogHelper.Info("管理员登录超时,已切换为操作员");
                SystemState.NowUser = "操作员";
            }

            try
            {
                CpuUsage = float.Parse(cpuCounter.NextValue().ToString("0.00"));
            }
            catch (Exception ex)
            {
                LogHelper.Error($"获取CPU使用率失败: {ex.Message}");
            }
        }

        private static void PerformDialogCoordinatorAction(Action action, bool runInMainThread)
        {
            if (!runInMainThread)
            {
                Task.Factory.StartNew(action);
            }
            else
            {
                action();
            }
        }

        private void OnButtonPressed()
        {
            _elapsedSeconds = 0;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += ButtonTimer_Tick;
            _timer.Start();
        }

        private void ButtonTimer_Tick(object? sender, EventArgs e)
        {
            _elapsedSeconds++;
            if (_elapsedSeconds >= 3)
            {
                _timer?.Stop();
                _timer = null;
                // 触发长按 3 秒后的事件  回原点
                ResetGoHomeCommand.Execute(true);
            }
        }

        private void OnButtonReleased()
        {
            _timer?.Stop();
            _timer = null;
        }

        private void HandleUserLogin()
        {
            LoginDialogSettings settings = new LoginDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                FirstAuxiliaryButtonText = "Cancel",
                DialogButtonFontSize = 20D,
                MaximumBodyHeight = 200
            };

            var result = this._dialogCoordinator.ShowModalLoginExternal(this, "Tips", "Welcome to the Motion Software!", new LoginDialogSettings() { InitialUsername = "管理员", EnablePasswordPreview = true });
            if (result == null)
            {
                // User pressed cancel
                SystemState.NowUser = "操作员";
            }
            else
            {
                if (result.Username == "管理员")
                {
                    if (result.Password == "admin")
                    {
                        this._dialogCoordinator.ShowModalMessageExternal(this, "Tips!", $"{result.Username}登录成功", MessageDialogStyle.Affirmative, settings);
                        SystemState.NowUser = "管理员";
                        AdminLoginTime = DateTime.Now;
                        LogHelper.Info("管理员登录成功");
                    }
                    else
                    {
                        SystemState.NowUser = "操作员";
                    }
                }
                else
                {
                    SystemState.NowUser = "操作员";
                    this._dialogCoordinator.ShowModalMessageExternal(this, "Tips!", $"账户名错误!", MessageDialogStyle.Affirmative, settings);
                }
            }
        }

        private void InitializeConfig()
        {
            if (!Directory.Exists("Config"))
            {
                Directory.CreateDirectory("Config");
            }

            try
            {
                SystemConfig systemConfig = SerializerJson.LoadFromJson<SystemConfig>("Config//SystemConfig.json");
                GlobalParameter.SystemConfig = systemConfig == null ? new SystemConfig() : systemConfig;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"加载配置文件SystemConfig失败: {ex.Message}");
            }
            try
            {
                ObservableCollection<IOSignal> inputs = SerializerJson.LoadFromJson<ObservableCollection<IOSignal>>("Config//InputSignals.json");
                GlobalParameter.InputSignals = inputs == null ? new ObservableCollection<IOSignal>() : inputs;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"加载配置文件InputSignals失败: {ex.Message}");
            }
            try
            {
                ObservableCollection<IOSignal> outputs = SerializerJson.LoadFromJson<ObservableCollection<IOSignal>>("Config//OutputSignals.json");
                GlobalParameter.OutputSignals = outputs == null ? new ObservableCollection<IOSignal>() : outputs;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"加载配置文件OutputSignals失败: {ex.Message}");
            }
            try
            {
                ObservableCollection<AxisInfo> axisInfos = SerializerJson.LoadFromJson<ObservableCollection<AxisInfo>>("Config//AxisInfos.json");
                GlobalParameter.AxisInfos = axisInfos == null ? new ObservableCollection<AxisInfo>() : axisInfos;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"加载配置文件AxisInfos失败: {ex.Message}");
            }

            try
            {
                ThemeManager.Current.ChangeThemeBaseColor(Application.Current, GlobalParameter.SystemConfig.AppTheme);
                ThemeManager.Current.ChangeThemeColorScheme(Application.Current, GlobalParameter.SystemConfig.AccentColor);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"加载主题失败: {ex.Message}");
            }
        }

        private void InitializeModules()
        {
            Modules = new List<ButtonViewModel>
            {
                new ButtonViewModel { Name = "用户登录", Foreground = new SolidColorBrush(Colors.Black) },
                new ButtonViewModel { Name = "主页面", Foreground = new SolidColorBrush(Colors.Blue), Page = new HomeView() },
                new ButtonViewModel { Name = "轴页面", Foreground = new SolidColorBrush(Colors.Black), Page = new AxisView() },
                new ButtonViewModel { Name = "IO页面", Foreground = new SolidColorBrush(Colors.Black), Page = new IOView() },
                new ButtonViewModel { Name = "运行日志", Foreground = new SolidColorBrush(Colors.Black), Page = new LogView() },
                new ButtonViewModel { Name = "操作页面", Foreground = new SolidColorBrush(Colors.Black), Page = new OperateView() },
                new ButtonViewModel { Name = "系统设置", Foreground = new SolidColorBrush(Colors.Black), Page = new SystemSettingView() }
            };

            CurrentPage = Modules.Find(o => o.Name == "主页面").Page;
        }

        private void UpdateButtonForeground()
        {
            foreach (var item in Modules)
            {
                item.Foreground = item.Name == TitleSelectName ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Black);
            }
        }
    }

    public partial class ButtonViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private SolidColorBrush foreground;

        [ObservableProperty]
        public UserControl page;
    }
}