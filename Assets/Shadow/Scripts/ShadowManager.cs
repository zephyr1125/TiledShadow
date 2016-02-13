using System;
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
        private List<Edge> _listEdges = new List<Edge>();
        /// <summary>
        /// 被照亮的边缘
        /// </summary>
        private List<Edge> _listLightedEdges = new List<Edge>();

        /// <summary>
        /// 阴影计算完毕后的事件
        /// </summary>
        public Action<DataShadow> action; 

        public void Init(List<GameObject> wallTiles)
        {
            _listEdges.Clear();

            for (int i = 0; i < wallTiles.Count; i++)
            {
                _listEdges.AddRange(Edge.GetEdgesOfTile(wallTiles[i]));
            }
        }
        
        /// <summary>
        /// 计算并存储所有被照亮的边缘
        /// </summary>
        public void CalcLightedEdges(Vector2 lightPos)
        {
            _listLightedEdges.Clear();

            //1.基于edge中点与光源的距离与所在tile与光源的距离，筛出朝向光源的edge
            for (int i = 0; i < _listEdges.Count; i++)
            {
                if (_listEdges[i].CalcLightDistance(lightPos))
                {
                    _listLightedEdges.Add(_listEdges[i]);
                }
            }

            //2.按照edge之间的相连关系，设置他们的prev和next
            for (int i = 0; i < _listLightedEdges.Count; i++)
            {
                Edge edge = _listLightedEdges[i];
                for (int j = 0; j < _listLightedEdges.Count; j++)
                {
                    Edge another = _listLightedEdges[j];
                    if (edge.PointEnd.Equals(another.PointStart))
                    {
                        edge.Next = another;
                        another.Prev = edge;
                        break;
                    }
                }
            }

            //3.按distance由近及远排序
            _listLightedEdges.Sort((x, y) =>
            {
                if (x.Distance < y.Distance) return -1;
                if (x.Distance > y.Distance) return 1;
                return 0;
            });

            if (action != null)
            {
                DataShadow data = new DataShadow
                {
                    LightedEdges = _listLightedEdges.ToArray()
                };
                action(data);
            }
        }
    }
}