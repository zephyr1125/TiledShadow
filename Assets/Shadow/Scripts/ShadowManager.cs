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
        private Dictionary<int, Edge> _dicEdges;
        /// <summary>
        /// 被照亮的边缘
        /// </summary>
        private Dictionary<int, Edge> _dicLightedEdges; 

        public void Init(BoardManager boardManager)
        {
            GameObject[] wallTiles = boardManager.wallTiles;
            _dicEdges = new Dictionary<int, Edge>();
            for (int i = 0; i < wallTiles.Length; i++)
            {
                InitAllEdges(wallTiles[i]);
            }
        }

        /// <summary>
        /// 传入tile块构造edge数据
        /// 假定：tile都为方形，且居中绘制
        /// </summary>
        /// <param name="tileObject"></param>
        private void InitAllEdges(GameObject tile)
        {
            //1.构造出4个边，各自中点
            Sprite sprite = tile.GetComponent<SpriteRenderer>().sprite;
            Debug.Log(sprite);
        }
    }
}