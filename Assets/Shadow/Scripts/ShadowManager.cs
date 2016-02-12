using UnityEngine;
using System.Collections.Generic;
using Completed;

namespace zephyr.twodshadow
{

    public class ShadowManager
    {
        /// <summary>
        /// 地图中的所有边缘
        /// </summary>
        private List<Edge> _listEdges;
        /// <summary>
        /// 被照亮的边缘
        /// </summary>
        private List<Edge> _listLightedEdges; 

        public void Init(GameObject[] wallTiles)
        {
            _listEdges = new List<Edge>();
            for (int i = 0; i < wallTiles.Length; i++)
            {
                _listEdges.AddRange(GetTileEdges(wallTiles[i]));
            }
        }

        /// <summary>
        /// 获取指定tile块的所有边edge
        /// 假定：tile都为矩形，且居中绘制
        /// </summary>
        /// <param name="tileObject"></param>
        private Edge[] GetTileEdges(GameObject tile)
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
        /// 计算并存储所有被照亮的边缘
        /// </summary>
        public void CalcLightedEdges()
        {
            
        }
    }
}