using Motion.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion.Config
{
    public class GlobalParameter
    {
        public static SystemConfig SystemConfig { get; set; } = new SystemConfig();
        public static ObservableCollection<IOSignal> InputSignals { get; set; } = new ObservableCollection<IOSignal> { };
        public static ObservableCollection<IOSignal> OutputSignals { get; set; } = new ObservableCollection<IOSignal> { };
        public static ObservableCollection<AxisInfo> AxisInfos { get; set; } = new ObservableCollection<AxisInfo> { new AxisInfo() , new AxisInfo() , new AxisInfo() , new AxisInfo() };


    }
}
