using MahApps.Metro.Controls.Dialogs;
using Motion.ViewModel;
using System.Windows.Controls;

namespace Motion.View
{
    /// <summary>
    /// SystemSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSettingView : UserControl
    {
        public SystemSettingView()
        {
            InitializeComponent();
            this.DataContext = new SystemSettingViewModel(DialogCoordinator.Instance);
        }
    }
}