using UnityEngine;
using System.Collections.Generic;

namespace zephyr.twodshadow
{
    public class TestMesh : MonoBehaviour
    {

        public Vector3[] Vertices;
        //public Vector2[] UV;
        //public int[] Triangles;

        private Mesh stuff;

        void MeshSetup()
        {
            Vertices = new Vector3[] { new Vector3(-1, -1, 1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, -1, 1) };
            //UV = new Vector2[] { new Vector2(0, 256), new Vector2(256, 256), new Vector2(256, 0), new Vector2(0, 0) };
            //Triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        }


        void Start()
        {
            MeshSetup();
            stuff = gameObject.GetComponent<MeshFilter>().mesh;

        }

        void Update()
        {
            stuff.vertices = Vertices;
            //stuff.triangles = Triangles;
            //stuff.uv = UV;
        }
    }
}