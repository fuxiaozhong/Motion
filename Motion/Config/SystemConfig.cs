using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion.Config
{
    public partial class SystemConfig : ObservableObject
    {
        [ObservableProperty]
        private string appTheme = "Light";

        [ObservableProperty]
        private string accentColor = "Cyan";



    }
}
