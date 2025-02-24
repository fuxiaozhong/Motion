using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using GTN;

using log4net;

using Motion.Config;
using Motion.Core;
using Motion.Model;

using static GTN.mc;

namespace Motion.MotionBoard
{
    public class Googol : MotionBase
    {

        /// <summary>
        /// 检查指定轴的操作是否完成
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <returns>操作是否完成</returns>
        public override bool CheckDone(short cord, short axis)
        {
            int stsL1;
            uint clk;
            // 调用 GTN_GetSts 函数获取轴的状态信息
            GTN_GetSts(cord, axis, out stsL1, 1, out clk);
            // 线程休眠 1 毫秒，避免过于频繁的状态检查
            Thread.Sleep(1);
            // 通过位运算判断指定状态位是否为 1，以此判断操作是否完成
            return ((stsL1 & 0x400) != 0);
        }

        /// <summary>
        /// 清除指定轴的报警信息
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        public override void ClearAlarm(short cord, short axis)
        {
            // 发送清除报警命令
            short rtn = GTN.mc.GTN_ClrSts(cord, axis, 1); // 此处假设轴号从 1 开始编号
        }

        /// <summary>
        /// 禁用指定轴
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <returns>禁用操作是否成功</returns>
        public override bool DisableAxis(short cord, short axis)
        {
            // 调用 GTN_AxisOff 函数尝试禁用轴
            short rtn = GTN.mc.GTN_AxisOff(cord, axis); // 此处假设轴号从 1 开始编号
            // 根据返回值判断操作是否成功
            if (rtn != 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// EtherCAT 轴回零操作
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <param name="method">回零方法</param>
        /// <param name="maxspeed">最大速度</param>
        /// <param name="minspeed">最小速度</param>
        /// <param name="acc">加速度，默认为 0.1</param>
        /// <returns>回零操作是否成功</returns>
        public override bool EcatGoHome(short cord, short axis, short method, double maxspeed, double minspeed, double acc = 0.1)
        {
            short sRtn;
            ushort sHomeSts;
            int pSts;

            // 清除轴的状态
            sRtn = GTN_ClrSts(cord, axis, 1);
            if (sRtn != 0)
            {
                return false;
            }

            // 使能轴
            sRtn = GTN_AxisOn(cord, axis);
            if (sRtn != 0)
            {
                return false;
            }

            // 等待轴状态满足条件
            do
            {
                // 线程休眠 2 毫秒，避免过于频繁的状态检查
                Thread.Sleep(2);
                // 读取轴的状态
                sRtn = GTN.mc.GTN_GetSts(cord, (short)axis, out pSts, 1, out uint pCloCk);
                if (sRtn != 0)
                {
                    return false;
                }
            } while ((pSts & (1 << 10)) != 0);

            // 记录日志，轴切换到回零模式
            LogHelper.Info($"轴{axis} 切换到回零模式");

            // 必须处于伺服使能状态，切换到回零模式
            sRtn = GTN_SetHomingMode(cord, axis, 6);
            if (sRtn != 0)
            {
                return false;
            }

            // 设置回零参数
            sRtn = GTN_SetEcatHomingPrm(cord, axis, method, maxspeed, minspeed, acc, 0, 0);
            if (sRtn != 0)
            {
                return false;
            }

            // 启动回零
            sRtn = GTN_StartEcatHoming(cord, axis);
            if (sRtn != 0)
            {
                // 记录日志，轴启动回零失败
                LogHelper.Info($"轴{axis}启动回零失败");
                return false;
            }

            // 循环等待回零完成或中断条件满足
            do
            {
                sRtn = GTN_GetEcatHomingStatus(cord, axis, out sHomeSts);
                if (sRtn != 0)
                {
                    return false;
                }

                // 如果按下停止或者急停，则中断回零
                if (GetInputIoState(1) || !GetInputIoState(3))
                {
                    sRtn = GTN_StopEcatHoming(cord, axis);

                    ushort usHomingStatus;
                    ushort usDrvMode;

                    sRtn = GTN_GetEcatHomingStatus(cord, axis, out usHomingStatus);
                    sRtn = GTN_GetEcatAxisMode(cord, axis, out usDrvMode);

                    // 中断、停止回零，记录相关状态信息
                    LogHelper.Info($"轴{axis}中断、停止回零  查询EtherCAT轴的回零状态:{usHomingStatus.ToString()}读取 EtheCAT 轴的操作模式:{usDrvMode.ToString()}");

                    // 切换到位置控制模式
                    sRtn = GTN_SetHomingMode(cord, axis, 8);
                    if (sRtn != 0)
                    {
                        return false;
                    }

                    return false;
                }
            } while (sHomeSts != 3);

            // 线程休眠 100 毫秒，等待一段时间
            Thread.Sleep(100);

            // 等待搜索原点完成后，切换到位置控制模式
            sRtn = GTN_SetHomingMode(cord, axis, 8);
            if (sRtn != 0)
            {
                return false;
            }

            // 线程休眠 100 毫秒，等待一段时间
            Thread.Sleep(100);

            // 清零轴的位置
            sRtn = GTN.mc.GTN_ZeroPos(cord, (short)axis, 1);
            if (sRtn != 0)
            {
                return false;
            }

            // 记录日志，轴回零完成
            LogHelper.Info($"轴{axis}回零完成");
            return true;
        }
        /// <summary>
        /// 使能指定轴
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <returns>使能操作是否成功</returns>
        public override bool EnableAxis(short cord, short axis)
        {
            // 发送使能命令
            short rtn = GTN.mc.GTN_AxisOn(cord, axis); // 此处假设轴号从 1 开始编号
            // 根据返回值判断操作是否成功
            if (rtn != 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取指定轴的驱动器报警状态
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axisIndex">轴编号</param>
        /// <returns>是否有驱动器报警</returns>
        public override bool GetAxisDriverAlarm(short cord, short axisIndex)
        {
            int stsL1;
            uint clk;
            // 调用 GTN_GetSts 函数获取轴的状态信息
            GTN.mc.GTN_GetSts(cord, axisIndex, out stsL1, 1, out clk);
            // 通过位运算判断指定状态位是否为 1，以此判断是否有报警
            return ((stsL1 & 0x2) != 0);
        }

        /// <summary>
        /// 获取指定轴的使能状态
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axisIndex">轴编号</param>
        /// <returns>轴是否使能</returns>
        public override bool GetAxisEnable(short cord, short axisIndex)
        {
            int stsL1;
            uint clk;
            // 调用 GTN_GetSts 函数获取轴的状态信息
            GTN.mc.GTN_GetSts(cord, axisIndex, out stsL1, 1, out clk);
            // 通过位运算判断指定状态位是否为 1，以此判断轴是否使能
            return ((stsL1 & 0x200) != 0);
        }

        /// <summary>
        /// 获取指定轴的编码器位置
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <returns>编码器位置</returns>
        public override double GetAxisEncPosition(short cord, short axis)
        {
            uint pClock = 0;
            double pos = 0;
            // 调用 GTN_GetAxisEncPos 函数获取轴的编码器位置
            GTN_GetAxisEncPos(cord, axis, out pos, (short)1, out pClock);

            // 获取轴的脉冲当量
            int pulseEquivalent = GlobalParameter.AxisInfos.First(o => o.AxisNumber == axis).PulseEquivalent;
            // 根据脉冲当量计算实际位置
            pos = pos / pulseEquivalent;
            return pos;
        }

        /// <summary>
        /// 获取指定轴的编码器速度
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <returns>编码器速度</returns>
        public override double GetAxisEncVel(short cord, short axis)
        {
            uint pClock = 0;
            double speed = 0;
            // 调用 GTN_GetAxisEncVel 函数获取轴的编码器速度
            GTN_GetAxisEncVel(cord, axis, out speed, (short)1, out pClock);
            // 速度单位转换
            return speed / 1000;
        }

        /// <summary>
        /// 获取指定轴的负限位状态
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axisIndex">轴编号</param>
        /// <returns>是否达到负限位</returns>
        public override bool GetAxisNegativeLimit(short cord, short axisIndex)
        {
            int stsL1;
            uint clk;
            // 调用 GTN_GetSts 函数获取轴的状态信息
            GTN.mc.GTN_GetSts(cord, axisIndex, out stsL1, 1, out clk);
            // 通过位运算判断指定状态位是否为 1，以此判断是否达到负限位
            return ((stsL1 & 0x40) != 0);
        }

        /// <summary>
        /// 获取指定轴的原点限位状态
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axisIndex">轴编号</param>
        /// <returns>是否达到原点限位</returns>
        public override bool GetAxisOrgLimit(short cord, short axisIndex)
        {
            int stsL1;
            uint clk;
            // 调用 GTN_GetSts 函数获取轴的状态信息
            GTN.mc.GTN_GetSts(cord, axisIndex, out stsL1, 1, out clk);
            // 通过位运算判断指定状态位是否为 1，以此判断是否达到原点限位
            return (stsL1 & (1 << 5)) > 0;
        }

        /// <summary>
        /// 获取指定轴的正限位状态
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axisIndex">轴编号</param>
        /// <returns>是否达到正限位</returns>
        public override bool GetAxisPositiveLimit(short cord, short axisIndex)
        {
            int stsL1;
            uint clk;
            // 调用 GTN_GetSts 函数获取轴的状态信息
            GTN.mc.GTN_GetSts(cord, axisIndex, out stsL1, 1, out clk);
            // 通过位运算判断指定状态位是否为 1，以此判断是否达到正限位
            return ((stsL1 & 0x20) != 0);
        }

        /// <summary>
        /// 获取指定轴的规划位置
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <returns>规划位置</returns>
        public override double GetAxisPrfPosition(short cord, short axis)
        {
            uint pClock = 0;
            double pos = 0;
            // 调用 GTN_GetAxisPrfPos 函数获取轴的规划位置
            GTN_GetAxisPrfPos(cord, axis, out pos, (short)1, out pClock);

            // 获取轴的脉冲当量
            int pulseEquivalent = GlobalParameter.AxisInfos.First(o => o.AxisNumber == axis).PulseEquivalent;
            // 根据脉冲当量计算实际位置
            pos = pos / pulseEquivalent;
            return pos;
        }

        /// <summary>
        /// 获取指定输入 IO 的状态
        /// </summary>
        /// <param name="ioIndex">IO 编号</param>
        /// <returns>IO 状态</returns>
        public override bool GetInputIoState(short ioIndex)
        {
            int slaveno = ioIndex / 16;
            int bit = ioIndex % 16;
            bit = bit <= 15 ? bit : bit - 1;
            byte pValue;
            // 调用 GT_GetGLinkDiBit 函数获取指定 IO 位的状态
            short rtn = GT_GetGLinkDiBit((short)slaveno, (short)bit, out pValue);
            return pValue == 1;
        }

        /// <summary>
        /// 获取指定输出 IO 的状态
        /// </summary>
        /// <param name="ioIndex">IO 编号</param>
        /// <returns>IO 状态</returns>
        public override bool GetOutputIoState(short ioIndex)
        {
            int slaveno = ioIndex / 16;
            int bit = ioIndex % 16;
            bit = bit <= 15 ? bit : bit - 1;
            uint inputFlag = 0;
            // 调用 GT_GetGLinkDo 函数获取指定 IO 的状态
            short ret = GT_GetGLinkDo((short)slaveno, 0, ref inputFlag, 2);

            return isBitZero((int)inputFlag, bit);
        }

        /// <summary>
        /// 判断指定整数的指定位是否为 0
        /// </summary>
        /// <param name="number">整数</param>
        /// <param name="index">位索引</param>
        /// <returns>指定位是否为 0</returns>
        private static bool isBitZero(int number, int index)
        {
            return (number & 1 << (index)) > 0;
        }
        public override void Initialize(short cord)
        {
            short rtn, sEcatSts;
            string strY = DateTime.Now.ToString();

            // 打开运动控制器
            rtn = GTN_Open(5, 1);
            if (rtn != 0)
            {
                LogHelper.Error($"打开运动控制器失败:{rtn}");
                CordInitState = false;
                return;
            }
            LogHelper.Info("打开运动控制器成功");

            // 复位运动控制器
            rtn = GTN_Reset(cord);

            // 终止之前的 EtherCAT 通讯
            rtn = GTN_TerminateEcatComm(cord);

            // 初始化 EtherCAT 总线
            rtn = GTN_InitEcatComm(cord);
            if (rtn != 0)
            {
                LogHelper.Error($"初始化EtherCat总线失败:{rtn}");
                CordInitState = false;
                return;
            }
            LogHelper.Info("初始化EtherCat总线成功");
            CordNo = cord;

            // 等待 EtherCAT 总线准备就绪
            short sCnt = 0;
            do
            {
                try
                {
                    GTN_IsEcatReady(cord, out sEcatSts);
                    Thread.Sleep(500);
                    sCnt++;
                }
                catch (ThreadInterruptedException ex)
                {
                    LogHelper.Error($"等待EtherCAT总线准备就绪时被中断: {ex.Message}");
                    CordInitState = false;
                    return;
                }
            } while ((sEcatSts != 1 || rtn != 0) && (sCnt < 20));

            // 启动 EtherCAT 通讯
            rtn = GTN_StartEcatComm(cord);
            if (rtn != 0)
            {
                LogHelper.Error($"启动EtherCAT通讯失败:{rtn}");
                CordInitState = false;
                return;
            }
            LogHelper.Info("启动EtherCAT通讯成功");

            // 获取已加载的运动轴和 IO 数量
            short sMotionNum, sIONum;
            rtn = GTN_GetEcatSlaves(cord, out sMotionNum, out sIONum);
            LogHelper.Info($"已加载{sMotionNum}轴，{sIONum}IO");

            // 初始化扩展模块
            rtn = GT_GLinkInit((short)0);
            if (rtn != 0)
            {
                LogHelper.Error($"初始化扩展模块失败{rtn}");
                CordInitState = false;
                return;
            }

            // 获取已初始化的扩展模块从站数量
            byte slavenum1;
            rtn = GT_GetGLinkOnlineSlaveNum(out slavenum1);
            LogHelper.Info($"初始化扩展模块{slavenum1}成功");

            CordInitState = true;
        }

        /// <summary>
        /// 启动指定控制器和轴的 Jog 运动
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <param name="jogSpeed">Jog 运动的速度</param>
        /// <param name="jogAcc">Jog 运动的加速度</param>
        public override void JogMotion(short cord, short axis, double jogSpeed, double jogAcc)
        {
            // 步骤 1: 配置 Jog 运动参数
            // 创建一个 TJogPrm 结构体实例，用于存储 Jog 运动的参数
            GTN.mc.TJogPrm jogParam = new TJogPrm();
            // 计算加速度，将速度转换为脉冲/ms²，使用绝对值确保为正值
            jogParam.acc = Math.Abs(jogSpeed / (jogAcc * 1000));
            // 减速度与加速度相同
            jogParam.dec = Math.Abs(jogSpeed / (jogAcc * 1000));
            // 设置平滑时间为 0.3ms，用于运动的平滑过渡
            jogParam.smooth = 0.3;

            // 步骤 2: 清除轴的状态
            // 调用 GTN_ClrSts 函数清除指定轴的状态，传入控制器编号、轴编号和相关参数
            short rtn = GTN_ClrSts(cord, axis, 1);
            if (rtn != 0)
            {
                // 若清除状态失败，记录错误日志并返回，终止后续操作
                LogHelper.Error($"清除轴 {axis} 状态失败，错误码: {rtn}");
                return;
            }

            // 步骤 3: 设置 Jog 运动模式
            // 调用 GTN_PrfJog 函数将指定轴设置为 Jog 运动模式
            rtn = GTN.mc.GTN_PrfJog(cord, axis);
            if (rtn != 0)
            {
                // 若设置模式失败，记录错误日志并返回，终止后续操作
                LogHelper.Error($"设置轴 {axis} 的 Jog 运动模式失败，错误码: {rtn}");
                return;
            }

            // 步骤 4: 设置 Jog 运动参数
            // 调用 GTN_SetJogPrm 函数将之前配置好的 Jog 运动参数应用到指定轴
            rtn = GTN.mc.GTN_SetJogPrm(cord, axis, ref jogParam);
            if (rtn != 0)
            {
                // 若设置参数失败，记录错误日志并返回，终止后续操作
                LogHelper.Error($"设置轴 {axis} 的 Jog 运动参数失败，错误码: {rtn}");
                return;
            }

            // 步骤 5: 设置 Jog 运动速度
            // 调用 GTN_SetVel 函数设置指定轴的 Jog 运动速度
            rtn = GTN.mc.GTN_SetVel(cord, axis, jogSpeed);
            if (rtn != 0)
            {
                // 若设置速度失败，记录错误日志并返回，终止后续操作
                LogHelper.Error($"设置轴 {axis} 的 Jog 运动速度失败，错误码: {rtn}");
                return;
            }

            // 步骤 6: 启动 Jog 运动
            // 调用 GTN_Update 函数更新轴的运动状态，启动 Jog 运动
            rtn = GTN.mc.GTN_Update(cord, 1 << (axis - 1));
            if (rtn != 0)
            {
                // 若启动运动失败，记录错误日志
                LogHelper.Error($"启动轴 {axis} 的 Jog 运动失败，错误码: {rtn}");
            }
            else
            {
                // 若启动运动成功，记录成功日志
                LogHelper.Info($"轴 {axis} 的 Jog 运动启动成功");
            }
        }


        /// <summary>
        /// 实现轴的点位运动控制
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <param name="point">点位信息</param>
        /// <param name="waitForCompletion">是否等待运动完成，默认为 true</param>
        /// <param name="axisdw">是否检查轴的位置精度，默认为 true</param>
        /// <returns>运动是否成功</returns>
        public override bool PointToPointMotion(short cord, short axis, PointInfo point, bool waitForCompletion = true, bool axisdw = true)
        {
            try
            {
                // 步骤 1: 计算脉冲当量和目标位置
                // 从全局参数中获取指定轴的脉冲当量
                int pulseEquivalent = GlobalParameter.AxisInfos.First(o => o.AxisNumber == axis).PulseEquivalent;
                // 根据脉冲当量将目标位置转换为脉冲数
                int tempPos = (int)(point.Postation * pulseEquivalent);

                // 步骤 2: 检查当前位置
                short rtn;
                uint clk;
                double prfpos;
                // 获取轴的当前规划位置
                rtn = GTN_GetPrfPos(cord, axis, out prfpos, 1, out clk);
                if (point.Postation == prfpos)
                {
                    // 如果当前位置已经是目标位置，直接返回运动成功
                    return true;
                }

                // 步骤 3: 等待上一次运动完成
                if (!WaitForPreviousMotionToComplete(cord, axis))
                {
                    // 如果上一次运动未完成或者系统处于暂停状态，返回运动失败
                    return false;
                }

                // 步骤 4: 设置点位运动参数
                // 创建梯形加减速运动参数结构体
                GTN.mc.TTrapPrm trap = new TTrapPrm();
                // 计算加速度
                trap.acc = point.DMaxVel / (point.DTacc * 1000);
                // 计算减速度
                trap.dec = point.DMaxVel / (point.DTdec * 1000);
                // 设置平滑时间
                trap.smoothTime = point.SmoothTime;
                // 设置起始速度
                trap.velStart = point.DStartVel;
                // 清除轴的报警信息
                ClearAlarm(cord, axis);
                // 设置停止减速度
                short ret = GTN_SetStopDec(cord, axis, trap.dec, trap.dec);

                // 步骤 5: 设置运动模式和参数
                if (!SetMotionParameters(cord, axis, trap, point, tempPos))
                {
                    // 如果设置运动模式和参数失败，返回运动失败
                    return false;
                }

                // 步骤 6: 启动点位运动
                rtn = GTN.mc.GTN_Update(cord, 1 << (axis - 1));
                if (rtn != 0)
                {
                    // 如果启动运动失败，记录错误日志并返回运动失败
                    LogHelper.Error($"点位运动启动失败，错误码: {rtn}");
                    return false;
                }

                // 步骤 7: 等待运动完成
                if (waitForCompletion)
                {
                    if (!WaitForMotionToComplete(cord, axis))
                    {
                        // 如果等待运动完成过程中出现问题，返回运动失败
                        return false;
                    }

                    if (axisdw)
                    {
                        // 如果需要检查轴的位置精度，调用相应方法进行检查
                        return CheckAxisPositionAccuracy(cord, axis, point);
                    }
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                // 捕获并记录运动过程中出现的异常，返回运动失败
                LogHelper.Error($"点位运动发生异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 等待上一次运动完成
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <returns>是否成功等待上一次运动完成</returns>
        private bool WaitForPreviousMotionToComplete(short cord, short axis)
        {
            // 循环检查轴的运动是否完成
            while (CheckDone(cord, axis))
            {
                if (SystemState.PAUSE)
                {
                    // 如果系统处于暂停状态，返回等待失败
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 设置运动模式和参数
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <param name="trap">梯形加减速运动参数结构体</param>
        /// <param name="point">点位信息</param>
        /// <param name="tempPos">目标位置（脉冲数）</param>
        /// <returns>是否成功设置运动模式和参数</returns>
        private bool SetMotionParameters(short cord, short axis, GTN.mc.TTrapPrm trap, PointInfo point, int tempPos)
        {
            short rtn;
            // 设置点位运动模式为梯形加减速运动
            rtn = GTN.mc.GTN_PrfTrap(cord, axis);
            if (rtn != 0)
            {
                // 如果设置运动模式失败，记录错误日志并返回设置失败
                LogHelper.Error($"设置点位运动模式失败，错误码: {rtn}");
                return false;
            }

            // 设置梯形加减速运动参数
            rtn = GTN.mc.GTN_SetTrapPrm(cord, axis, ref trap);
            if (rtn != 0)
            {
                // 如果设置运动参数失败，记录错误日志并返回设置失败
                LogHelper.Error($"设置点位运动参数失败，错误码: {rtn}");
                return false;
            }

            // 设置目标速度
            rtn = GTN.mc.GTN_SetVel(cord, axis, point.DMaxVel);
            if (rtn != 0)
            {
                // 如果设置目标速度失败，记录错误日志并返回设置失败
                LogHelper.Error($"设置目标速度失败，错误码: {rtn}");
                return false;
            }

            // 设置目标位置
            rtn = GTN.mc.GTN_SetPos(cord, axis, tempPos);
            if (rtn != 0)
            {
                // 如果设置目标位置失败，记录错误日志并返回设置失败
                LogHelper.Error($"设置目标位置失败，错误码: {rtn}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 等待运动完成
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <returns>是否成功等待运动完成</returns>
        private bool WaitForMotionToComplete(short cord, short axis)
        {
            // 循环等待轴的运动完成
            while (CheckDone(cord, axis)) ;
            return true;
        }

        /// <summary>
        /// 检查轴的位置精度
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号</param>
        /// <param name="point">点位信息</param>
        /// <returns>轴的位置是否达到精度要求</returns>
        private bool CheckAxisPositionAccuracy(short cord, short axis, PointInfo point)
        {
            // 记录开始时间
            DateTime start = DateTime.Now;
            bool stop = false;
            do
            {
                if (SystemState.PAUSE)
                {
                    // 如果系统处于暂停状态，返回检查失败
                    return false;
                }
                // 获取轴的当前编码器位置
                double tagpos = GetAxisEncPosition(cord, axis);
                if (Math.Abs(tagpos - point.Postation) <= point.Accuracy)
                {
                    // 如果当前位置与目标位置的差值在精度范围内，返回检查成功
                    return true;
                }
                if ((DateTime.Now - start).TotalMilliseconds > point.Timeout)
                {
                    // 如果超过了超时时间，返回检查失败
                    return false;
                }
                // 线程休眠 5 毫秒，避免过于频繁的检查
                Thread.Sleep(5);
            } while (!stop);

            return false;
        }



        /// <summary>
        /// 停止指定控制器和轴的运动
        /// </summary>
        /// <param name="cord">控制器编号</param>
        /// <param name="axis">轴编号，假设轴号从 1 开始编号</param>
        /// <returns>停止操作是否成功</returns>
        public override bool StopAxis(short cord, short axis)
        {
            // 步骤 1: 调用 GTN_Stop 函数停止轴的运动
            // GTN_Stop 函数用于停止指定轴的运动，第一个参数为控制器编号
            // 第二个参数 1 << (axis - 1) 用于指定要停止的轴，通过位运算将对应轴的位置置为 1
            // 第三个参数 0 << (axis - 1) 表示停止方式，这里假设使用默认的停止方式
            short rtn = GTN.mc.GTN_Stop(cord, 1 << (axis - 1), 0 << (axis - 1));

            // 步骤 2: 检查停止操作的返回结果
            if (rtn != 0)
            {
                // 如果返回结果不为 0，说明停止操作失败
                // 记录错误日志，方便后续排查问题
                LogHelper.Error($"停止轴 {axis} 运动失败，错误码: {rtn}");
                return false;
            }

            // 步骤 3: 停止操作成功
            // 记录成功日志，表明轴的停止操作已成功完成
            LogHelper.Info($"轴 {axis} 运动停止成功");
            return true;
        }
    }
}
