using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Motion.Config;
using Motion.Model;

namespace Motion.MotionBoard
{
    public class MotionClass
    {
        public static MotionBase motion;

        /// <summary>
        /// 轴设置使能
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="enable"></param>
        public static void AxisSetEnable(short axis, bool enable)
        {

        }

        /// <summary>
        /// 轴启动回原
        /// </summary>
        /// <param name="id"></param>
        public static void AxisGoHome(short id)
        {

        }

        /// <summary>
        /// 停止轴
        /// </summary>
        /// <param name="id"></param>
        public static void AxisStop(short id)
        {

        }

        /// <summary>
        /// Jog移动
        /// </summary>
        /// <param name="id"></param>
        /// <param name="direction">1正  -1反</param>
        public static void AxisJogMove(short id, int direction)
        {

        }

        /// <summary>
        /// 寸动
        /// </summary>
        /// <param name="id"></param>
        public static void AxisLengthMove(short id)
        {

        }

        /// <summary>
        /// 示教点位
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="point"></param>
        public static void AxisTeachPoint(short axis, string point)
        {


        }

        /// <summary>
        /// 移动至点位
        /// </summary>
        /// <param name="axisNumber"></param>
        /// <param name="id"></param>
        internal static bool AxisMovePoint(short axisNumber, string? id)
        {
            AxisInfo? axis = null;
            foreach (var item in GlobalParameter.AxisInfos)
            {
                if (item.AxisNumber == axisNumber)
                {
                    axis = item;
                    break;
                }
            }

            if (axis == null)
            {
                return false;
            }

            PointInfo? point = null;
            foreach (var item in axis.PointInfos)
            {
                if (item.PointID == id)
                {
                    point = item;
                    break;
                }
            }
            if (point == null)
            {
                return false;
            }

            return motion.PointToPointMotion(motion.CordNo, axisNumber, point);
        }
    }
}
