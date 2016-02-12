using UnityEngine;
using System.Collections.Generic;

namespace zephyr.twodshadow
{
    /// <summary>
    /// 光照边缘
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// 起始、中点与结束点
        /// </summary>
        public Vector2 PointStart, PointMiddle, PointEnd;

        /// <summary>
        /// 所在tile的中心点，用于计算是否应该被照亮
        /// </summary>
        public Vector2 PointCenter;

        /// <summary>
        /// 前一个与后一个被照亮的边缘
        /// </summary>
        public Edge Prev, Next;

        /// <summary>
        /// 与光源的距离
        /// </summary>
        public float Distance;

        /// <summary>
        /// 基于起始点、终点和进行构造
        /// warning: 构造完毕后Distance,Prev与Next并没有赋值
        /// </summary>
        /// <param name="pointStart"></param>
        /// <param name="pointEnd"></param>
        /// <param name="pointCenter"></param>
        public Edge(Vector2 pointStart, Vector2 pointEnd, Vector2 pointCenter)
        {
            PointStart = pointStart;
            PointEnd = pointEnd;
            PointCenter = pointCenter;
            PointMiddle = new Vector2(pointStart.x + pointEnd.x, pointStart.y + pointEnd.y)/2;
        }
    }
}