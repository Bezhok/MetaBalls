using System;
using System.Collections;
using System.Collections.Generic;
using src;
using UnityEditor;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private Circle circlePrefab;
    private Circle _circle;
    private Sprite _backgroundSprite;
    private Camera _mainCamera;
    private Texture2D _texture;
    private Vector2Int _textureRes;

    private void InitBackground()
    {
        var height = _mainCamera.pixelHeight;
        var width = _mainCamera.pixelWidth;

        backgroundSpriteRenderer.transform.localScale = new Vector3(
                width * 2/(float)_textureRes.x,
                height * 2/(float)_textureRes.y
                );
        
        _texture = new Texture2D(_textureRes.x, _textureRes.y);
        var sprite = Sprite.Create(_texture, new Rect(0, 0, _textureRes.x, _textureRes.y), Vector2.zero);

        backgroundSpriteRenderer.sprite = sprite;
        backgroundSpriteRenderer.transform.position = _mainCamera.ScreenToWorldPoint(new Vector3(0,0,50));
    }

    private void Start()
    {
        _textureRes.x = 1280;
        _textureRes.y = 720;
        
        _mainCamera = Camera.main;
        InitBackground();
        
        _circle = Instantiate(circlePrefab);
        _circle.Create(0.5f);
        
        var pos = _mainCamera.WorldToScreenPoint(_circle.transform.position);
        pos = ScreenToTexturePoint(pos);
        
        for (var y = 0; y < _texture.height; y++)
        for (var x = 0; x < _texture.width; x++) 
        {
            var val = ComputeColor(x, y, pos);
            Color pixelColor = new Color(val, val, val, 1);
            _texture.SetPixel(x, y, pixelColor);
        }

        _texture.Apply();
    }

    private float ComputeColor(int x, int y, Vector2 center)
    {
        var distance = Vector2.Distance(new Vector2(x, y), center);
        return 1 - Mathf.Clamp01(distance / 1000);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var pz = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pz.z = 0;

            var circle = Instantiate(circlePrefab);
            circle.transform.position = pz;
            circle.Create(0.1f);
            
            var pos = ScreenToTexturePoint(Input.mousePosition);

            for (var y = 0; y < _texture.height; y++)
            for (var x = 0; x < _texture.width; x++) 
            {
                var val = ComputeColor(x, y, pos);
                var pixelColor = new Color(val, val, val, 1);
                _texture.SetPixel(x, y, pixelColor);
            }

            _texture.Apply();
        }

        TestInput();
    }

    private void TestInput()
    {
        var pos = ScreenToTexturePoint(Input.mousePosition);
        
        _texture.SetPixel((int)pos.x, (int)pos.y, Color.red);
        _texture.Apply();
    }

    private Vector3 ScreenToTexturePoint(Vector3 pos)
    {
        var scaller = new Vector2(_mainCamera.pixelWidth/(float)_textureRes.x, _mainCamera.pixelHeight/(float)_textureRes.y);
        return new Vector3(pos.x / scaller.x, pos.y/scaller.y);
    }
}
