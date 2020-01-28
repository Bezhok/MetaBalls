using System;
using System.Collections;
using System.Collections.Generic;
using src;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class Main : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private Circle circlePrefab;
    private Camera _mainCamera;
    private Texture2D _texture;
    private Vector2Int _textureRes;
    private float _prevAspect;
    private Vector3 _defaultTextureSize;
    Color32[] colors32;
    private void InitBackground()
    {

        _texture = new Texture2D(_textureRes.x, _textureRes.y);
        var sprite = Sprite.Create(_texture, new Rect(0, 0, _textureRes.x, _textureRes.y), Vector2.zero);

        backgroundSpriteRenderer.sprite = sprite;
        backgroundSpriteRenderer.transform.position = _mainCamera.ScreenToWorldPoint(new Vector3(0,0,50));
        
        _defaultTextureSize = backgroundSpriteRenderer.bounds.size;
        UpdateTextureSize();
        
        colors32 = new Color32[_texture.width*_texture.height];
    }

    private Vector2 CameraSize()
    {
        var cameraHeight = 2 * _mainCamera.orthographicSize;
        var cameraWidth = cameraHeight * _mainCamera.aspect;
        
        return new Vector2(cameraWidth, cameraHeight);
    }
    
    private void UpdateTextureSize()
    {
        var size = CameraSize();

        float scaller;
        if (size.x < size.y)
            scaller = size.x / _defaultTextureSize.x;
        else
            scaller = size.y / _defaultTextureSize.y;

        backgroundSpriteRenderer.transform.localScale = new Vector3(scaller, scaller);
        
        backgroundSpriteRenderer.transform.position = _mainCamera.ScreenToWorldPoint(new Vector3(0,0,50));
    }
    private void Start()
    {
        _textureRes.x = 1280/4;
        _textureRes.y = 720/4;


        _mainCamera = Camera.main;
        InitBackground();

        _prevAspect = _mainCamera.aspect;
        
        _texture.Apply();
    }

    private float ComputeColor(int x, int y, Vector2 center)
    {
        var distance = Vector2.Distance(new Vector2(x, y), center);
//        Mathf.Abs(x-center.x)+Mathf.Abs(y-center.y);
        return distance;
    }

    private List<Circle> _circles = new List<Circle>();
    private void Update()
    {
        if (!FloatComparer.AreEqualRelative(_prevAspect, _mainCamera.aspect, 0.0001f))
        {
            UpdateTextureSize();
            _prevAspect = _mainCamera.aspect;
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var pz = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pz.z = 0;

            var circle = Instantiate(circlePrefab);
            circle.transform.position = pz;
            circle.Radius = 0.1f;
//            circle.Create(0.1f);
            _circles.Add(circle);
            
            var pos = WorldToTexturePoint(_mainCamera.ScreenToWorldPoint(Input.mousePosition));
            circle.TexturePosition = pos;
            
            
            for (var y = 0; y < _texture.height; y++)
            for (var x = 0; x < _texture.width; x++)
            {
                var sum = 0f;
                foreach (var c in _circles)
                {
                    var val = ComputeColor(x, y, c.TexturePosition);
                    val = c.Radius / val;
                    sum += val;
                }

                sum *= 1000*10;
                if (sum > 255) sum = 255;
                byte sumb = (byte) sum;
                colors32[x + y * _texture.width] = new Color32(sumb,sumb,sumb, 255);
            }
            
            _texture.SetPixels32(colors32);
            _texture.Apply();
        }
    }

    private void TestInput()
    {
        var pos = WorldToTexturePoint(_mainCamera.ScreenToWorldPoint(Input.mousePosition));
        _texture.SetPixel((int)pos.x, (int)pos.y, Color.red);
        _texture.Apply();
    }

    private Vector3 ScreenToTexturePoint(Vector3 pos)
    {
        throw new Exception("Doesn't work with not default aspect");
        var scaller = new Vector2(_mainCamera.pixelWidth/(float)_textureRes.x, _mainCamera.pixelHeight/(float)_textureRes.y);
        return new Vector3(pos.x / scaller.x, pos.y/scaller.y);
    }

    private Vector3 WorldToTexturePoint(Vector3 pos)
    {
        var bounds = backgroundSpriteRenderer.bounds;
        var textureX = bounds.size.x;
        var textureY = bounds.size.y;

        var camSize = CameraSize();
        
        // x          - (pos+shift)
        // resolution - texsize
        var relation = new Vector2(_textureRes.x/textureX, _textureRes.y/textureY);
        var camPosition = _mainCamera.transform.position;
        return new Vector3((pos.x + camSize.x/2 - camPosition.x)* relation.x, (pos.y+camSize.y/2- camPosition.y)*relation.y);
    }
}
