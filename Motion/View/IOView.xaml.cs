using MahApps.Metro.Controls.Dialogs;
using Motion.Model;
using Motion.ViewModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Motion.View
{
    /// <summary>
    /// IOView.xaml 的交互逻辑
    /// </summary>
    public partial class IOView : UserControl
    {
        public IOView()
        {
            InitializeComponent();
            DataContext = new IOViewModel(DialogCoordinator.Instance);
        }

        private void ToggleOutput_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton && toggleButton.DataContext is IOSignal outputSignal)
            {
                // 这里可以添加实际的输出控制逻辑，例如通过串口、网口等控制硬件
                // 示例代码仅更新界面显示
                outputSignal.Status = toggleButton.IsChecked ?? false;
            }
        }
    }
}