using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion.Model
{
    public enum AxisStopModel
    {
        减速停止, 立即停止
    }
    public enum AxisMotionModel
    {
        相对模式, 绝对模式
    }

    public partial class PointInfo : ObservableObject
    {
        [ObservableProperty]
        private string pointID = "";//点位名称
        [ObservableProperty]
        private string pointName = "";//点位名称
        [ObservableProperty]
        private double dStartVel = 10;//起始速度
        [ObservableProperty]
        private double dMaxVel = 30;//运行速度
        [ObservableProperty]
        private double dTacc = 0.1;//加速时间
        [ObservableProperty]
        private double dTdec = 0.1;//减速时间
        [ObservableProperty]
        private double dStopVel = 20;//停止速度
        [ObservableProperty]
        private double postation = 0;//位置
        [ObservableProperty]
        private short smoothTime = 0;//平滑时间
        [ObservableProperty]
        private double accuracy = 0.01;//到位精度
        [ObservableProperty]
        private double timeout = 3000;//到位超时时间

    }

    public partial class AxisInfo : ObservableObject
    {
        /// <summary>
        /// 轴编号
        /// </summary>
        [ObservableProperty]
        private short axisNumber;

        /// <summary>
        /// 轴名称
        /// </summary>
        [ObservableProperty]
        private string axisName = "TEST";


        #region Jog
        /// <summary>
        /// 减速度[需要多久时间 加速到正常速度]  
        /// 加速度 = 正常速度 / 加速时间
        /// </summary>
        [ObservableProperty]
        private double jogDecTime;


        /// <summary>
        /// 加速度 [需要多久时间减速停止]
        /// </summary>
        [ObservableProperty]
        private double jogAccTime;


        /// <summary>
        /// 速度 mm/s
        /// </summary>
        [ObservableProperty]
        private double jogSpeed;

        /// <summary>
        /// 平滑时间
        /// </summary>
        [ObservableProperty]
        private double jogStime;

        /// <summary>
        /// 停止模式
        /// </summary>
        [ObservableProperty]
        private AxisStopModel jogStopModel = AxisStopModel.减速停止;
        #endregion

        #region 寸动

        /// <summary>
        /// 运动模式
        /// </summary>
        [ObservableProperty]
        public AxisMotionModel inchModel = AxisMotionModel.相对模式;

        /// <summary>
        /// 寸动距离
        /// </summary>
        [ObservableProperty]
        private double inchLength;

        #endregion

        #region

        /// <summary>
        /// 回零模式
        /// </summary>
        [ObservableProperty]
        private int goHomeModel;

        /// <summary>
        /// 回零停止模式
        /// </summary>
        [ObservableProperty]
        private AxisStopModel goHomeStopModel = AxisStopModel.减速停止;

        /// <summary>
        /// 回零低速
        /// </summary>
        [ObservableProperty]
        private double goHomeLowSpeed;

        /// <summary>
        /// 回零高速
        /// </summary>
        [ObservableProperty]
        private double goHomeHighSpeed;

        /// <summary>
        /// 回零偏移
        /// </summary>
        [ObservableProperty]
        private bool goHomeOffset;

        /// <summary>
        /// 回零偏移位置
        /// </summary>
        [ObservableProperty]
        private double goHomeOffsetPosition;

        #endregion

        /// <summary>
        /// 负限位
        /// </summary>
        [ObservableProperty]
        private bool nLimit;

        /// <summary>
        /// 正限位
        /// </summary>
        [ObservableProperty]
        private bool pLimit;

        /// <summary>
        /// 原点
        /// </summary>
        [ObservableProperty]
        private bool oLimit;

        /// <summary>
        /// 移动中
        /// </summary>
        [ObservableProperty]
        private bool moving;

        /// <summary>
        /// 使能
        /// </summary>
        [ObservableProperty]
        private bool enable;



        /// <summary>
        /// 当前脉冲位置
        /// </summary>
        [ObservableProperty]
        private double nowPulse;


        /// <summary>
        /// 当前编码器位置
        /// </summary>
        [ObservableProperty]
        private double nowEncoder;

        /// <summary>
        /// 当前编码器位置
        /// </summary>
        [ObservableProperty]
        private double nowSpeed;


        /// <summary>
        /// 脉冲当量
        /// </summary>
        [ObservableProperty]
        private int pulseEquivalent;

        public ObservableCollection<PointInfo> PointInfos { get; set; } = new ObservableCollection<PointInfo>() { };



        public bool PointToPointMotion(string pointId)
        {
            







            return false;
        }
    }
}
