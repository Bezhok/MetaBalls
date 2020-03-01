using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace src
{
    public class Main : MonoBehaviour
    {
        private readonly List<Circle> _circles = new List<Circle>();
        private Color32[] _colors32;
        private Vector3 _defaultTextureSize;
        private Camera _mainCamera;
        private float _prevAspect;
        private Texture2D _texture;
        private Vector2Int _textureRes;
        [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
        [SerializeField] private Circle circlePrefab;

        private void InitBackground()
        {
            _texture = new Texture2D(_textureRes.x, _textureRes.y);
            Sprite sprite = Sprite.Create(_texture, new Rect(0, 0, _textureRes.x, _textureRes.y), Vector2.zero);

            backgroundSpriteRenderer.sprite = sprite;
            backgroundSpriteRenderer.transform.position = _mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 50));

            _defaultTextureSize = backgroundSpriteRenderer.bounds.size;
            UpdateTextureSize();

            _colors32 = new Color32[_texture.width * _texture.height];
        }

        private Vector2 CameraSize()
        {
            float cameraHeight = 2 * _mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * _mainCamera.aspect;

            return new Vector2(cameraWidth, cameraHeight);
        }

        private void UpdateTextureSize()
        {
            Vector2 size = CameraSize();

            float scale;
            if (size.x < size.y)
                scale = size.x / _defaultTextureSize.x;
            else
                scale = size.y / _defaultTextureSize.y;

            backgroundSpriteRenderer.transform.localScale = new Vector3(scale, scale);

            backgroundSpriteRenderer.transform.position = _mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 50));
        }

        private void Start()
        {
            _textureRes.x = 1280 / 8;
            _textureRes.y = 720 / 8;


            _mainCamera = Camera.main;
            InitBackground();

            _prevAspect = _mainCamera.aspect;

            _texture.Apply();

            GenerateCircles();
        }

        private void GenerateCircles()
        {
            Vector2 camSize = CameraSize();
            for (int i = 0; i < 6; i++)
            {
                _circles.Add(Instantiate(circlePrefab));
                _circles[i].Radius = Random.Range(0.08f, 0.3f);
                _circles[i].speed = new Vector2(Random.Range(0f, camSize.x / 4), Random.Range(0f, camSize.x / 4));
            }
        }

        private float ComputeColor(int x, int y, Vector2 center)
        {
            float distance = Vector2.Distance(new Vector2(x, y), center);
            return distance;
        }

        private void Update()
        {
            if (!FloatComparer.AreEqualRelative(_prevAspect, _mainCamera.aspect, 0.0001f))
            {
                UpdateTextureSize();
                _prevAspect = _mainCamera.aspect;
            }

            ProcessInput();
            UpdateCirclePositions();

            UpdateTexture();
        }

        private void UpdateTexture()
        {
            for (int y = 0, i = 0; y < _texture.height; y++)
            for (int x = 0; x < _texture.width; x++, i++)
            {
                float sum = 0f;
                foreach (Circle c in _circles)
                {
                    float val = ComputeColor(x, y, c.TexturePosition);
                    val = c.Radius / val;
                    sum += val;
                }

                sum *= 6000;
                if (sum > 255) sum = 255;
                byte sumB = (byte) sum;
                _colors32[i] = new Color32(sumB, sumB, sumB, 255);
            }

            _texture.SetPixels32(_colors32);
            _texture.Apply();
        }

        private void ProcessInput()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 pos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                CreateStaticCircle(pos);
            }
        }

        private void UpdateCirclePositions()
        {
            Vector3 bottomLeft = _mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 50));
            Vector3 upperRight =
                _mainCamera.ScreenToWorldPoint(new Vector3(_mainCamera.pixelWidth, _mainCamera.pixelHeight, 50));

            foreach (Circle circle in _circles)
            {
                Vector3 circlePos = circle.transform.position;
                if (circlePos.x > upperRight.x || circlePos.x < bottomLeft.x) circle.speed.x *= -1;

                if (circlePos.y > upperRight.y || circlePos.y < bottomLeft.y) circle.speed.y *= -1;

                circlePos += Time.deltaTime * circle.speed;
                circle.transform.position = circlePos;
                circle.TexturePosition = WorldToTexturePoint(circlePos);
            }
        }

        private void CreateStaticCircle(Vector3 globalPosition)
        {
            globalPosition.z = 0;

            Circle circle = Instantiate(circlePrefab);
            circle.transform.position = globalPosition;
            circle.Radius = 0.1f;
            _circles.Add(circle);

            Vector3 pos = WorldToTexturePoint(globalPosition);
            circle.TexturePosition = pos;
        }

        private Vector3 WorldToTexturePoint(Vector3 pos)
        {
            Bounds bounds = backgroundSpriteRenderer.bounds;
            float textureX = bounds.size.x;
            float textureY = bounds.size.y;

            Vector2 camSize = CameraSize();

            // x          - (pos+shift)
            // resolution - texture size
            var relation = new Vector2(_textureRes.x / textureX, _textureRes.y / textureY);
            Vector3 camPosition = _mainCamera.transform.position;
            return new Vector3((pos.x + camSize.x / 2 - camPosition.x) * relation.x,
                (pos.y + camSize.y / 2 - camPosition.y) * relation.y);
        }
    }
}