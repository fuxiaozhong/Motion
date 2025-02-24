using Motion.ViewModel;
using System.Windows.Controls;

namespace Motion.View
{
    /// <summary>
    /// HomeView.xaml 的交互逻辑
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

            DataContext = new HomeViewModel();
        }
    }
}