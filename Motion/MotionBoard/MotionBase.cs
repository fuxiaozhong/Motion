using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Motion.Model;

namespace Motion.MotionBoard
{
    public partial class P2PStruct : ObservableObject
    {
        /// <summary>
        /// 移动模式
        /// </summary>
        [ObservableProperty]
        public AxisMotionModel model = AxisMotionModel.绝对模式;
        /// <summary>
        /// 轴ID
        /// </summary>
        [ObservableProperty]
        public short axisId;
        /// <summary>
        /// 加速时间
        /// </summary>
        [ObservableProperty]
        public double accTime = 0.1;
        /// <summary>
        /// 减速时间
        /// </summary>
        [ObservableProperty]
        public double decTime = 0.1;
        /// <summary>
        /// 平滑时间
        /// </summary>
        [ObservableProperty]
        public short smoothTime;
        /// <summary>
        /// 开始速度
        /// </summary>
        [ObservableProperty]
        public double startSpeed;
        /// <summary>
        /// 最大速度
        /// </summary>
        [ObservableProperty]
        public double speed;
        /// <summary>
        /// 目标位置
        /// </summary>
        [ObservableProperty]
        public double targetPosition;
        /// <summary>
        /// 是否等待停止
        /// </summary>
        [ObservableProperty]
        public bool await = true;
        /// <summary>
        /// 是否等待到位
        /// </summary>
        [ObservableProperty]
        public bool axisdw = true;
        /// <summary>
        /// 到位判断精度
        /// </summary>
        [ObservableProperty]
        public double accuracy = 0.01;
        /// <summary>
        /// 超时时间
        /// </summary>
        [ObservableProperty]
        public int timeout = 3000;

    }

    public abstract partial class MotionBase : ObservableObject
    {
        [ObservableProperty]
        public short cordNo = 0;

        [ObservableProperty]
        public bool cordInitState = false;

        public abstract void JogMotion(short cord, short axis, double jogSpeed, double jogAcc);

        public abstract bool PointToPointMotion(short cord, short axis, PointInfo point, bool await = true, bool axisdw = true);

        public abstract bool CheckDone(short cord, short axis);

        public abstract bool StopAxis(short cord, short axis);


        public abstract bool EnableAxis(short cord, short axis);

        public abstract bool DisableAxis(short cord, short axis);

        public abstract void ClearAlarm(short cord, short axis);

        public abstract bool EcatGoHome(short cord, short axis, short method, double maxspeed, double minspeed, double acc = 0.1);

        public abstract void Initialize(short cord);

        public abstract double GetAxisEncPosition(short cord, short axis);
        public abstract double GetAxisPrfPosition(short cord, short axis);

        public abstract double GetAxisEncVel(short cord, short axis);

        public abstract bool GetAxisPositiveLimit(short cord, short axisIndex);

        public abstract bool GetAxisNegativeLimit(short cord, short axisIndex);

        public abstract bool GetAxisOrgLimit(short cord, short axisIndex);

        public abstract bool GetAxisDriverAlarm(short cord, short axisIndex);

        public abstract bool GetAxisEnable(short cord, short axisIndex);

        public abstract bool GetInputIoState(short ioIndex);

        public abstract bool GetOutputIoState(short ioIndex);
    }
}
