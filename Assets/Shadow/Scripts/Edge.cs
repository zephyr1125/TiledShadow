using UnityEngine;
using System.Collections.Generic;

namespace zephyr.twodshadow
{
    /// <summary>
    /// 光照边缘的存储类
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// 起始、中点与结束点
        /// </summary>
        public Vector2 PointStart, PointMiddle, PointEnd;

        /// <summary>
        /// 前一个与后一个边缘的ID
        /// </summary>
        public int Prev, Next;

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
        /// <param name="projecter"></param>
        public Edge(Vector2 pointStart, Vector2 pointEnd)
        {
            PointStart = pointStart;
            PointEnd = pointEnd;
            PointMiddle = new Vector2(pointStart.x + pointEnd.x, pointEnd.y + pointEnd.y)/2;
        }
    }
}