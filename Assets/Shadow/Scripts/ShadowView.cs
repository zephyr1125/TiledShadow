using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace zephyr.twodshadow
{
    public class ShadowView : MonoBehaviour
    {
        private DataShadow _data;
        public LineRenderer Line;

        public void OnReceiveShadowData(DataShadow data)
        {
            _data = data;
            UpdateLineParams();
        }

        private void UpdateLineParams()
        {
            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < _data.LightedEdges.Count(); i++)
            {
                positions.Add(_data.LightedEdges[i].PointStart);
                positions.Add(_data.LightedEdges[i].PointEnd);
            }
            Line.sortingLayerName = "Units";
            Line.SetVertexCount(positions.Count);
            Line.SetPositions(positions.ToArray());
        }
    }
}