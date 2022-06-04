using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class IndicatorBar : MonoBehaviour
{
    private AbstractMap _mapManager;
    private Transform _cameraTransform;
    private float _height;
    private Vector2[] _uvs;
    private Mesh _mesh;
    private Renderer _renderer;
    private bool _initialized = false;
    private Vector2d _coordinate;
    private float _indicator;
    private const float _renderDistanceOffset = 50;


    public void Create(Vector2d coordinate, float indicator)
    {
        _coordinate = coordinate;
        _indicator = indicator;
        _mapManager = GameObject.Find("Map").GetComponent<AbstractMap>();
        _height = GetComponent<Transform>().localScale.y;
        _mesh = GetComponent<MeshFilter>().mesh;
        _renderer = GetComponent<Renderer>();
        _uvs = new Vector2[_mesh.vertices.Length];

        for (int i = 0; i < _uvs.Length; i++)
        {
            _uvs[i] = new Vector2(_indicator, 0);
        }
        _mesh.uv = _uvs;

        _initialized = true;

        Assert.IsNotNull(_mapManager);
    }


    void Update()
    {
        if (_initialized) {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        if (_cameraTransform == null) {
            _cameraTransform = GameObject.Find("Camera3D").GetComponent<Transform>();
        }

        Vector3 worldPosition = Conversions.GeoToWorldPosition(_coordinate.x, _coordinate.y, _mapManager.CenterMercator, _mapManager.WorldRelativeScale).ToVector3xz();
        worldPosition.y += _mapManager.QueryElevationInUnityUnitsAt(_coordinate);
        worldPosition.y += (_indicator - 0.5f) * _height;
        transform.position = worldPosition;

        float renderDistance = _mapManager.Options.extentOptions.defaultExtents.rangeAroundTransformOptions.visibleBuffer * _mapManager.Options.scalingOptions.unityTileSize;
        renderDistance += _renderDistanceOffset;
        _renderer.enabled = Mathf.Abs(worldPosition.x - _cameraTransform.position.x) < renderDistance && Mathf.Abs(worldPosition.z - _cameraTransform.position.z) < renderDistance;
    }
}
