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
    void Start()
    {
        _textureRes.x = 1280;
        _textureRes.y = 720;
        
        _circle = Instantiate(circlePrefab);
        _circle.Create(0.5f);

        _mainCamera = Camera.main;

        int cameraPixelHeight = _mainCamera.pixelHeight * 2;
        int cameraPixelWidth = _mainCamera.pixelWidth * 2;

        backgroundSpriteRenderer.transform.localScale = new Vector3(cameraPixelWidth/(float)_textureRes.x, cameraPixelHeight/(float)_textureRes.y);
        _texture = new Texture2D(_textureRes.x, _textureRes.y);
        Sprite sprite = Sprite.Create(_texture, new Rect(0, 0, _textureRes.x, _textureRes.y), Vector2.zero);

        backgroundSpriteRenderer.sprite = sprite;
        backgroundSpriteRenderer.transform.position = _mainCamera.ScreenToWorldPoint(new Vector3(0,0,50));
        
        var scaller = new Vector3(_mainCamera.pixelWidth/(float)_textureRes.x, _mainCamera.pixelHeight/(float)_textureRes.y);
        var pos = _mainCamera.WorldToScreenPoint(_circle.transform.position);
        pos = new Vector3(pos.x / scaller.x, pos.y/scaller.y);
        
        for (int y = 0; y < _texture.height; y++)
        {
            for (int x = 0; x < _texture.width; x++) 
            {
                Color pixelColor;

                var val = ComputeColor(x, y, pos);
                pixelColor = new Color(val, val, val, 1);
                    _texture.SetPixel(x, y, pixelColor);
            }
        }

        _texture.Apply();
    }

    float ComputeColor(int x, int y, Vector2 center)
    {
        float distance = Vector2.Distance(new Vector2(x, y), center);
        return 1 - Mathf.Clamp01(distance / 1000);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pz.z = 0;

            var circle = Instantiate(circlePrefab);
            circle.transform.position = pz;
            circle.Create(0.1f);
            

            var scaller = new Vector3(_mainCamera.pixelWidth/(float)_textureRes.x, _mainCamera.pixelHeight/(float)_textureRes.y);
            var pos = Input.mousePosition;
            pos = new Vector3(pos.x / scaller.x, pos.y/scaller.y);

            
            for (int y = 0; y < _texture.height; y++)
            {
                for (int x = 0; x < _texture.width; x++) 
                {
                    var val = ComputeColor(x, y, pos);
                    Color pixelColor = new Color(val, val, val, 1);
                    _texture.SetPixel(x, y, pixelColor);
                }
            }

            _texture.Apply();
        }

        testInput();
    }

    void testInput()
    {
        var scaller = new Vector3(_mainCamera.pixelWidth/(float)_textureRes.x, _mainCamera.pixelHeight/(float)_textureRes.y);
        var pos = Input.mousePosition;
        pos = new Vector3(pos.x / scaller.x, pos.y / scaller.y);

        _texture.SetPixel((int)pos.x, (int) pos.y, Color.red);
        _texture.Apply();
    }

    Vector3 ScreenToTexturePoint(Vector3 pos)
    {
        var scaller = new Vector3(_mainCamera.pixelWidth/(float)_textureRes.x, _mainCamera.pixelHeight/(float)_textureRes.y);
        return new Vector3(pos.x / scaller.x, pos.y/scaller.y);
    }

}
