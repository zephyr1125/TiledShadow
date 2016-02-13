using System;
using UnityEngine;
using System.Collections.Generic;

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

        private void Init(List<GameObject> wallTiles)
        {
            _listEdges.Clear();
            _listLightedEdges.Clear();

            for (int i = 0; i < wallTiles.Count; i++)
            {
                _listEdges.AddRange(Edge.GetEdgesOfTile(wallTiles[i]));
            }
        }
        
        /// <summary>
        /// 计算并存储所有被照亮的边缘
        /// </summary>
        public void CalcLightedEdges(List<GameObject> wallTiles, Vector2 lightPos)
        {
            Init(wallTiles);

            //1.基于edge中点与光源的距离与所在tile与光源的距离，筛出朝向光源的edge
            for (int i = 0; i < _listEdges.Count; i++)
            {
                if (_listEdges[i].CalcLightDistance(lightPos))
                {
                    _listLightedEdges.Add(_listEdges[i]);
                }
            }

            //2.移除两个块之间夹着的edge
            CleanInsideEdges();

            //3.按照edge之间的相连关系，设置他们的prev和next
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

            //4.按distance由近及远排序
            _listLightedEdges.Sort((x, y) =>
            {
                if (x.Distance < y.Distance) return -1;
                if (x.Distance > y.Distance) return 1;
                return 0;
            });

            //5.进行射线切割
            SliceEdges(lightPos);

            //6.清理无头无尾的edge
            CleanIsolatedEdges();

            if (action != null)
            {
                DataShadow data = new DataShadow
                {
                    LightedEdges = _listLightedEdges.ToArray()
                };
                action(data);
            }
        }

        private void CleanInsideEdges()
        {
            _listLightedEdges.RemoveAll(edge =>
            {
                Vector2 offset = Vector2.zero;
                switch (edge.Dir)
                {
                    case Edge.Direction.Top:
                        offset = Vector2.up;
                        break;
                    case Edge.Direction.Bottom:
                        offset = Vector2.down;
                        break;
                    case Edge.Direction.Left:
                        offset = Vector2.left;
                        break;
                    case Edge.Direction.Right:
                        offset = Vector2.right;
                        break;
                }
                foreach (Edge target in _listLightedEdges)
                {
                    if (Vector2.Distance(edge.PointCenter + offset, target.PointCenter) < 0.05f)
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        private void SliceEdges(Vector2 lightPos)
        {
            for (int i = 0; i < _listLightedEdges.Count; i++)
            {
                Edge edge = _listLightedEdges[i];
                if (edge.Next == null)
                {
                    Vector2 interSectPos = Vector2.zero;
                    Edge edgeInterSect = CheckInterSection(i, lightPos, edge.PointEnd, ref interSectPos);
                    if (edgeInterSect != null)
                    {
                        edgeInterSect.PointStart = interSectPos;
                        edgeInterSect.Prev = edge;
                        edge.Next = edgeInterSect;
                    }
                }
                if (edge.Prev == null)
                {
                    Vector2 interSectPos = Vector2.zero;
                    Edge edgeInterSect = CheckInterSection(i, lightPos, edge.PointStart, ref interSectPos);
                    if (edgeInterSect != null)
                    {
                        edgeInterSect.PointEnd = interSectPos;
                        edgeInterSect.Next = edge;
                        edge.Prev = edgeInterSect;
                    }
                }
            }
        }

        /// <summary>
        /// 获取从光源发出，经过指定点的射线，与光照边缘数组中的第一个交点
        /// </summary>
        /// <param name="idCurrent"></param>
        /// <param name="lightStart"></param>
        /// <param name="lightThrough"></param>
        /// <param name="interSectPoint"></param>
        /// <returns></returns>
        private Edge CheckInterSection(int idCurrent, Vector2 lightStart, Vector2 lightThrough, ref Vector2 interSectPoint)
        {
            for (int i = idCurrent+1; i < _listLightedEdges.Count; i++)
            {
                Edge edge = _listLightedEdges[i];
                //先寻找交点
                Vector2? result = LineIntersectionPoint(lightStart, lightThrough, edge.PointStart, edge.PointEnd);
                //注意这个交点是直线交点，不一定落在edge内，因此要进行范围检查
                if (result == null || !CheckInsideBound(result.Value, edge.PointStart, edge.PointEnd)) continue;
                interSectPoint = result.Value;
                return edge;
            }
            return null;
        }

        /// <summary>
        /// 注意，只能检查指定点是否在指定线段的方形范围内，并不保证在线上
        /// 但因为在此处给定的点是之前先进行相交计算后的结果，因此一定在线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        private bool CheckInsideBound(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            if (point.x < Mathf.Min(lineStart.x, lineEnd.x) || point.x > Mathf.Max(lineStart.x, lineEnd.x))
            {
                return false;
            }

            if (point.y < Mathf.Min(lineStart.y, lineEnd.y) || point.y > Mathf.Max(lineStart.y, lineEnd.y))
            {
                return false;
            }

            return true;
        }

        private Vector2? LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            float a1 = pe1.y - ps1.y;
            float b1 = ps1.x - pe1.x;
            float c1 = a1 * ps1.x + b1 * ps1.y;

            // Get A,B,C of second line - points : ps2 to pe2
            float a2 = pe2.y - ps2.y;
            float b2 = ps2.x - pe2.x;
            float c2 = a2 * ps2.x + b2 * ps2.y;

            // Get delta and check if the lines are parallel
            float delta = a1 * b2 - a2 * b1;
            if (Math.Abs(delta) < 0.001f)
                return null;

            // now return the Vector2 intersection point
            return new Vector2(
                (b2 * c1 - b1 * c2) / delta,
                (a1 * c2 - a2 * c1) / delta
            );
        }

        private void CleanIsolatedEdges()
        {
            _listLightedEdges.RemoveAll(edge =>
            {
                bool clean = edge.Prev == null || edge.Next == null;
                if (clean)
                {
                    if (edge.Prev != null)
                    {
                        edge.Prev.Next = null;
                        edge.Prev = null;
                    }

                    if (edge.Next != null)
                    {
                        edge.Next.Prev = null;
                        edge.Next = null;
                    }
                }
                return clean;
            });
        }
    }
}