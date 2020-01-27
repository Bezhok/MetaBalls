using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    private Sprite _backgroundSprite;
    private Camera _mainCamera;
    private Texture2D _texture;
    void Start()
    {
//        _backgroundSprite = backgroundSpriteRenderer.sprite;
        _mainCamera = Camera.main;
        double multiplier = 1.005;
        int pixelHeight = (int) (_mainCamera.pixelHeight * 2 * multiplier);
        int pixelWidth = (int) (_mainCamera.pixelWidth * 2 * multiplier);
        _texture = new Texture2D(pixelWidth, pixelHeight);
        Sprite sprite = Sprite.Create(_texture, new Rect(0, 0, pixelWidth, pixelHeight), Vector2.zero);
        
        var halfHeight = Camera.main.orthographicSize;
        var halfWidth = halfHeight * Screen.width / Screen.height;
        
        backgroundSpriteRenderer.sprite = sprite;
        backgroundSpriteRenderer.transform.position = new Vector3(-halfWidth*1.002f, -halfHeight*1.002f);
//        backgroundSpriteRenderer.transform.position = _mainCamera.ScreenToWorldPoint(Vector3.zero);;
        for (int y = 0; y < _texture.height; y++)
        {
            for (int x = 0; x < _texture.width; x++) 
            {
                Color pixelColour;
                var val = ComputeColor(x, y, new Vector2(pixelWidth/2, pixelHeight/2));
                pixelColour = new Color(val, val, val, 1);
                    _texture.SetPixel(x, y, pixelColour);
            }
        }
        _texture.Apply();
    }

    float ComputeColor(int x, int y, Vector2 center)
    {
        float distance = Vector2.Distance(new Vector2(x, y), center);
        return 1 - Mathf.Clamp01(Mathf.Sqrt(distance / 1000));
    }
    
    void Update()
    {

    }
}
