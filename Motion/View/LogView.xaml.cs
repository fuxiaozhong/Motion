using Motion.Core;
using Motion.ViewModel;
using System.Windows.Controls;

namespace Motion.View
{
    /// <summary>
    /// LogView.xaml 的交互逻辑
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
            this.DataContext = new LogViewModel();
        }

        private void Tile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LogHelper.InfoClick = true;
            LogHelper.WarnClick = false;
            LogHelper.ErrorClick = false;
            LogHelper.AllClick = false;
            foreach (var item in LogHelper.Logs)
            {
                if (item.Type == Model.LogType.Info)
                {
                    item.IsVisible = true;
                }
                else
                {
                    item.IsVisible = false;
                }
            }
        }

        private void Tile_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            LogHelper.InfoClick = false;
            LogHelper.WarnClick = true;
            LogHelper.ErrorClick = false;
            LogHelper.AllClick = false;
            foreach (var item in LogHelper.Logs)
            {
                if (item.Type == Model.LogType.Warning)
                {
                    item.IsVisible = true;
                }
                else
                {
                    item.IsVisible = false;
                }
            }
        }

        private void Tile_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {
            LogHelper.InfoClick = false;
            LogHelper.WarnClick = false;
            LogHelper.ErrorClick = true;
            LogHelper.AllClick = false;
            foreach (var item in LogHelper.Logs)
            {
                if (item.Type == Model.LogType.Error)
                {
                    item.IsVisible = true;
                }
                else
                {
                    item.IsVisible = false;
                }
            }
        }

        private void Tile_Click_3(object sender, System.Windows.RoutedEventArgs e)
        {
            LogHelper.InfoClick = true;
            LogHelper.WarnClick = true;
            LogHelper.ErrorClick = true;
            LogHelper.AllClick = true;
            foreach (var item in LogHelper.Logs)
            {
                item.IsVisible = true;
            }
        }
    }
}