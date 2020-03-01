using UnityEngine;

namespace src
{
    public class Circle : MonoBehaviour
    {
        private Mesh _mesh;

        public Vector3 speed;

        public float Radius { get; set; }

        public Color FillColor { get; set; }
        public Vector3 TexturePosition { get; set; }

        private void Awake()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            FillColor = Color.white;
        }

        public void CreateMesh(float radius, int trianglesCount = 40)
        {
            Radius = radius;

            var center = Vector2.zero;
            var stepCount = (int) (trianglesCount * Mathf.Max(1, radius));
            var step = 2 * Mathf.PI / stepCount;
            
            int verticesCount = stepCount + 1;
            var colors = new Color[verticesCount];
            var vertices = new Vector3[verticesCount];
            var ebo = new int[stepCount*3];

            //colors
            for (var i = 0; i < colors.Length; i++) colors[i] = FillColor;
            
            //verticies
            vertices[0] = center;
            for (int i = 1; i < verticesCount; i++)
            {
                vertices[i] = CalcPoint((i-1) * step, radius);
            }

            //ebo
            int eboIdx = 0;
            for (int vertIdx = 1; vertIdx < stepCount; eboIdx += 3, vertIdx++)
            {
                ebo[eboIdx] = vertIdx;
                ebo[eboIdx+1] = vertIdx+1;
                ebo[eboIdx+2] = 0;
            }

            ebo[eboIdx] = stepCount;
            ebo[eboIdx+1] = 1;
            ebo[eboIdx+2] = 0;
            
            _mesh.vertices = vertices;
            _mesh.triangles = ebo;
            _mesh.colors = colors;
        }

        private Vector2 CalcPoint(float angle, float radius)
        {
            var x = radius * Mathf.Cos(angle);
            var y = radius * Mathf.Sin(angle);
            return new Vector2(x, y);
        }
    }
}