using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MahApps.Metro.Controls.Dialogs;

using Motion.Config;
using Motion.Core;
using Motion.Model;
using Motion.MotionBoard;
using System.IO;

namespace Motion.ViewModel
{
    public partial class AxisViewModel : ObservableObject
    {
        MetroDialogSettings dialogSettings = new MetroDialogSettings
        {
            AffirmativeButtonText = "是",
            NegativeButtonText = "否",
            MaximumBodyHeight = 200
        };


        [ObservableProperty]
        public AxisInfo axisInfo;
        [ObservableProperty]
        public PointInfo pointInfo;
        public List<AxisStopModel> AxisStopModels { get; set; }
        public List<AxisMotionModel> AxisMotionModels { get; set; }

        public AxisViewModel()
        {
            AxisStopModels = new List<AxisStopModel>();
            foreach (AxisStopModel axisStopModel in Enum.GetValues(typeof(AxisStopModel)))
            {
                AxisStopModels.Add(axisStopModel);
            }

            AxisMotionModels = new List<AxisMotionModel>();
            foreach (AxisMotionModel axisMotion in Enum.GetValues(typeof(AxisMotionModel)))
            {
                AxisMotionModels.Add(axisMotion);
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    LoginDialogSettings settings = new LoginDialogSettings()
                    {
                        AffirmativeButtonText = "Yes",
                        FirstAuxiliaryButtonText = "Cancel",
                        DialogButtonFontSize = 20D,
                        MaximumBodyHeight = 200
                    };


                    if (!Directory.Exists("Config"))
                    {
                        Directory.CreateDirectory("Config");
                    }

                    if (SerializerJson.SaveToJson(GlobalParameter.AxisInfos, "Config//AxisInfos.json"))
                    {
                        DialogCoordinator.Instance.ShowModalMessageExternal(this, "Tips!", $"保存成功", MessageDialogStyle.Affirmative, settings);
                    }
                });
            }
        }
        public RelayCommand AddAxisCommand
        {
            get
            {
                return new RelayCommand(() =>
                {


                    short ID = 0;
                LABEL0:
                    bool ishave = false;
                    foreach (var item in GlobalParameter.AxisInfos)
                    {
                        if (item.AxisNumber == ID)
                        {
                            ishave = true;
                            break;
                        }
                    }
                    if (ishave)
                    {
                        ID++;
                        goto LABEL0;

                    }

                    GlobalParameter.AxisInfos.Insert(ID, new AxisInfo() { AxisNumber = ID, AxisName = $"轴{ID}" });
                });
            }
        }
        public RelayCommand AddPointCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (AxisInfo != null)
                    {
                        short axisid = AxisInfo.AxisNumber;

                        short point_count = 0;

                        string ID = axisid + "_0";
                    LABEL0:
                        bool ishave = false;
                        foreach (var item in AxisInfo.PointInfos)
                        {
                            if (item.PointID == axisid + "_" + point_count)
                            {
                                ishave = true;
                                break;
                            }
                        }
                        if (ishave)
                        {
                            point_count++;
                            goto LABEL0;
                        }
                        AxisInfo.PointInfos.Add(new PointInfo() { PointID = axisid + "_" + point_count, PointName = $"点位" + axisid + "_" + point_count });
                    }
                });
            }
        }
        public RelayCommand DeletePointCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (AxisInfo != null && PointInfo != null)
                    {

                        // 使用 MahApps.Metro 的消息对话框
                        MessageDialogResult result = DialogCoordinator.Instance.ShowModalMessageExternal(this, "Tips", $"确认要删除点位:{PointInfo.PointID}？", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            AxisInfo.PointInfos.Remove(PointInfo);
                        }
                    }
                });
            }
        }
        public RelayCommand DeleteAxisCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (AxisInfo != null)
                    {

                        // 使用 MahApps.Metro 的消息对话框
                        MessageDialogResult result = DialogCoordinator.Instance.ShowModalMessageExternal(this, "Tips", $"确认要删除轴:{AxisInfo.AxisNumber}？", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            GlobalParameter.AxisInfos.Remove(AxisInfo);
                        }
                    }
                });
            }
        }


        public RelayCommand<short> AxisEnableCommand
        {
            get
            {
                return new RelayCommand<short>((id) =>
                {

                    if (GlobalParameter.AxisInfos.First(o => o.AxisNumber == id).Enable)
                    {
                        MessageDialogResult result = DialogCoordinator.Instance.ShowModalMessageExternal(this, "Tips", $"确认要关闭轴:{id}使能？", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            MotionClass.AxisSetEnable(id, false);
                        }
                    }
                    else
                    {
                        MessageDialogResult result = DialogCoordinator.Instance.ShowModalMessageExternal(this, "Tips", $"确认要打开轴:{id}使能？", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
                        if (result == MessageDialogResult.Affirmative)
                        {
                            MotionClass.AxisSetEnable(id, true);
                        }
                    }


                });
            }
        }
        public RelayCommand<short> AxisGoHomeCommand
        {
            get
            {
                return new RelayCommand<short>((id) =>
                {
                    MessageDialogResult result = DialogCoordinator.Instance.ShowModalMessageExternal(this, "Tips", $"确认要启动轴:{id}回原？", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
                    if (result == MessageDialogResult.Affirmative)
                    {
                        MotionClass.AxisGoHome(id);
                    }
                });
            }
        }

        /// <summary>
        /// 正向移动 鼠标抬起
        /// </summary>
        public RelayCommand<short> ForwardMovePutUp
        {
            get
            {
                return new RelayCommand<short>((id) =>
                {
                    MotionClass.AxisJogMove(id, 1);
                });
            }
        }
        /// <summary>
        /// 正向移动 鼠标松开
        /// </summary>
        public RelayCommand<short> ForwardMovePress
        {
            get
            {
                return new RelayCommand<short>((id) =>
                {
                    MotionClass.AxisStop(id);
                });
            }
        }

        /// <summary>
        /// 反向移动 鼠标抬起
        /// </summary>
        public RelayCommand<short> ReverseMovePutUp
        {
            get
            {
                return new RelayCommand<short>((id) =>
                {
                    MotionClass.AxisJogMove(id, -1);
                });
            }
        }

        /// <summary>
        /// 反向移动 鼠标松开
        /// </summary>
        public RelayCommand<short> ReverseMovePress
        {
            get
            {
                return new RelayCommand<short>((id) =>
                {
                    MotionClass.AxisStop(id);
                });
            }
        }

        /// <summary>
        /// 寸动
        /// </summary>
        public RelayCommand<short> AxisMoveLengthCommand
        {
            get
            {
                return new RelayCommand<short>((id) =>
                {
                    MotionClass.AxisLengthMove(id);
                });
            }
        }

        /// <summary>
        /// 示教点位
        /// </summary>
        public RelayCommand<string> TeachPointCommand
        {
            get
            {
                return new RelayCommand<string>((id) =>
                {
                    MessageDialogResult result = DialogCoordinator.Instance.ShowModalMessageExternal(this, "Tips", $"确认要示教点位:{id}？", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
                    if (result == MessageDialogResult.Affirmative)
                    {
                        MotionClass.AxisTeachPoint(AxisInfo.AxisNumber, id);
                    }
                });
            }
        }

        /// <summary>
        /// 移动点位
        /// </summary>
        public RelayCommand<string> MovePointCommand
        {
            get
            {
                return new RelayCommand<string>((id) =>
                {
                    MessageDialogResult result = DialogCoordinator.Instance.ShowModalMessageExternal(this, "Tips", $"确认要移动至点位:{id}？", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
                    if (result == MessageDialogResult.Affirmative)
                    {
                        MotionClass.AxisMovePoint(AxisInfo.AxisNumber, id);
                    }
                });
            }
        }
    }
}