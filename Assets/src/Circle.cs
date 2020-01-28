using System;
using UnityEngine;

namespace src
{
    public class Circle : MonoBehaviour
    {
        private Mesh _mesh;
        public Color FillColor { get; set; }

        private void Awake()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            FillColor = Color.white;
        }

        public void Create(float radius, int quality = 40)
        {
            Vector2 center = Vector2.zero;
            int stepCount = (int)(quality * Mathf.Max(1, radius));
            
            Color[] colors = new Color[4*(stepCount+1)];
            Vector3[] vertices = new Vector3[4*(stepCount+1)];
            int[] eboTemplate = {0, 1, 2, 1, 3, 2};
            int[] ebo = new int[eboTemplate.Length * (stepCount+1)];
            
            
            float step = 2*Mathf.PI / stepCount;

            float angle = 0;
            int eboIdx = 0;
            for (int i = 0, vertIdx = 0; i <= stepCount; i+=2, vertIdx += 4, angle = i*step)
            {
                var vecF = CalcPoint(i*step, radius);
                var vecS = CalcPoint((i+1)*step, radius);
                var vecT = CalcPoint((i+2)*step, radius);

                vertices[vertIdx] = vecF;
                vertices[vertIdx+1] = vecS;
                vertices[vertIdx+2] = center;
                vertices[vertIdx+3] = vecT;
                
                // 0F--1S--3T
                //  \  |  /
                //   \ | /
                //    2C
                for (int k = 0; k < eboTemplate.Length; k++)
                {
                    ebo[eboIdx+ k] = vertIdx + eboTemplate[k];
                }

                eboIdx += eboTemplate.Length;
            }
            
            
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = FillColor;
            }

            _mesh.vertices = vertices;
            _mesh.triangles = ebo;
            _mesh.colors = colors;
        }

        private Vector2 CalcPoint(float angle, float radius)
        {
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            return new Vector2(x, y);
        }
    }
}