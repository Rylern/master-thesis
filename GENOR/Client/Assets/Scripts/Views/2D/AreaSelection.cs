using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class AreaSelection : MonoBehaviour
{
    [SerializeField]
    private AbstractMap _mapManager;

    [SerializeField]
    private Camera _camera;

    private bool _enabled = false;
    private bool _isDraggingMouseBox = false;
    private Vector3 _dragStartPosition;
    private Texture2D _whiteTexture;
    private IndicatorsController _indicators;

    void Awake()
    {
        _whiteTexture = new Texture2D(1, 1);
        _whiteTexture.SetPixel(0, 0, Color.white);
        _whiteTexture.Apply();

        _indicators = GameObject.Find("Indicators").GetComponent<IndicatorsController>();

        Assert.IsNotNull(_mapManager);
        Assert.IsNotNull(_camera);
        Assert.IsNotNull(_whiteTexture);
        Assert.IsNotNull(_indicators);
    }


    void Update()
    {
        if (_enabled) {
            if (Input.GetMouseButtonDown(1)) {
                _isDraggingMouseBox = true;
                _dragStartPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1)) {
                _isDraggingMouseBox = false;
                _enabled = false;

                Vector2d latLonStartPosition = _mapManager.WorldToGeoPosition(_camera.ScreenToWorldPoint(_dragStartPosition));
                Vector2d latLonEndPosition = _mapManager.WorldToGeoPosition(_camera.ScreenToWorldPoint(Input.mousePosition));

                _indicators.VisualizeWalkability(latLonStartPosition, latLonEndPosition);
            }
        }
    }

    public void Enable()
    {
        _enabled = true;
    }

    void OnGUI()
    {
        if (_enabled && _isDraggingMouseBox)
        {
            var rect = GetScreenRect(_dragStartPosition, Input.mousePosition);
            DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
            DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f));
        }
    }

    private Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    private void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, _whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
}
