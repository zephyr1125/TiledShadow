using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace zephyr.twodshadow
{
    public class ShadowView : MonoBehaviour
    {
        private DataShadow _data;

        public Material LineMaterial;

        public void OnReceiveShadowData(DataShadow data)
        {
            _data = data;
            UpdateLineParams();
        }

        private void UpdateLineParams()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < _data.LightedEdges.Count(); i++)
            {
                Edge edge = _data.LightedEdges[i];
                CreateLine(edge.PointStart, edge.PointEnd, Color.white);
                if (edge.Next != null)
                {
                    Vector2 pStart = edge.PointEnd;
                    Vector2 pEnd = edge.Next.PointStart;
                    float widthStart = 0;
                    float widthEnd = 0.1f;
                    if (Vector2.Distance(pStart, pEnd) < 0.05f)
                    {
                        pStart -= new Vector2(0.07f, 0.07f);
                        pEnd += new Vector2(0.07f, 0.07f);
                        widthStart = 0.2f;
                        widthEnd = 0.2f;
                    }
                    CreateLine(pStart, pEnd, Color.red, widthStart, widthEnd);
                }
            }
        }

        private void CreateLine(Vector2 start, Vector2 end, Color color, float widthStart = 0f, float widthEnd = 0.1f)
        {
            GameObject go = new GameObject("line");
            go.transform.parent = transform;
            LineRenderer line = go.AddComponent<LineRenderer>();
            line.sortingLayerName = "Units";
            line.material = LineMaterial;
            line.SetVertexCount(2);
            line.SetWidth(widthStart, widthEnd);
            line.SetColors(color, color);
            line.SetPosition(0, start);
            line.SetPosition(1, end);
        }
    }
}