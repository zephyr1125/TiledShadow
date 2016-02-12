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

        public bool IsFaceToLight;

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

        public static Edge[] GetEdgesOfTile(GameObject tile)
        {
            //1.构造出4个边
            Vector2 center = tile.transform.position;
            Sprite sprite = tile.GetComponent<SpriteRenderer>().sprite;
            float extendX = sprite.bounds.extents.x;
            float extendY = sprite.bounds.extents.y;

            //顺序：左下、左上、右上、右下
            Vector2[] vertices = new[]
            {
                new Vector2(center.x - extendX, center.y - extendY),
                new Vector2(center.x - extendX, center.y + extendY),
                new Vector2(center.x + extendX, center.y + extendY),
                new Vector2(center.x + extendX, center.y - extendY),
            };

            return new[]
            {
                new Edge(vertices[0], vertices[1], center),
                new Edge(vertices[1], vertices[2], center),
                new Edge(vertices[2], vertices[3], center),
                new Edge(vertices[3], vertices[0], center),
            };
        }

        /// <summary>
        /// 计算与光源的距离，并通过与所在tile的中心与光源的距离的比较，以确定是否面向光源
        /// </summary>
        /// <param name="lightPos"></param>
        /// <returns></returns>
        public bool CalcLightDistance(Vector2 lightPos)
        {
            Distance = Vector2.Distance(lightPos, PointMiddle);
            float distCenter = Vector2.Distance(lightPos, PointCenter);
            IsFaceToLight = Distance < distCenter;
            return IsFaceToLight;
        }
    }
}