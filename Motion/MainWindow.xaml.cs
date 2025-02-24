using log4net.Config;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Motion.Core;
using System.Diagnostics;
using System.Windows.Threading;

namespace Motion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow()
        {
            this.viewModel = new MainWindowViewModel(DialogCoordinator.Instance);
            this.DataContext = this.viewModel;

            LogHelper.mainWindow = this;
            XmlConfigurator.Configure();


            InitializeComponent();

            DialogManager.DialogOpened += (_, args) => Debug.WriteLine($"Dialog {args.Dialog} - '{args.Dialog.Title}' opened.");
            DialogManager.DialogClosed += (_, args) => Debug.WriteLine($"Dialog {args.Dialog} - '{args.Dialog.Title}' closed.");
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dialogSettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "是",
                NegativeButtonText = "否",
                MaximumBodyHeight = 200
            };
            // 使用 MahApps.Metro 的消息对话框
            MessageDialogResult result = this.ShowModalMessageExternal("确认关闭", "你确定要关闭窗口吗？", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
            if (result == MessageDialogResult.Negative)
            {
                // 设置 Cancel 为 true 以阻止窗口关闭
                e.Cancel = true;
            }
        }
    }
}