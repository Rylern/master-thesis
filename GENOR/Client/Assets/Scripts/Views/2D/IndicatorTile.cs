using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class IndicatorTile : MonoBehaviour
{
    private AbstractMap _mapManager;
    private MeshFilter _meshfilter;
    private List<Vector2d> _coordinates;
    private bool _initialized = false;


    public void Create(List<Vector2d> coordinates, float indicator)
    {
        _mapManager = GameObject.Find("Map").GetComponent<AbstractMap>();
        _meshfilter = GetComponent<MeshFilter>();

        _coordinates = coordinates;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        Vector2d mapRef = _mapManager.CenterMercator;
        float worldRelScale = _mapManager.WorldRelativeScale;

        for (int i=0; i<4; i++) {
            Vector3 worldPosition = Conversions.GeoToWorldPosition(_coordinates[i].x, _coordinates[i].y, mapRef, worldRelScale).ToVector3xz();
            worldPosition.y += 1;
            vertices[i] = worldPosition;
            uv[i] = new Vector2(indicator, 0.5f);
        }

        /*
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 2;
        triangles[6] = 0;
        triangles[7] = 4;
        triangles[8] = 3;
        triangles[9] = 0;
        triangles[10] = 5;
        triangles[11] = 4;
        */
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 2;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        _meshfilter.mesh = mesh;

        _initialized = true;

        Assert.IsNotNull(_mapManager);
    }
    
    void Update()
    {
        if (_initialized) {
            Vector3[] vertices = new Vector3[4];

            Vector2d mapRef = _mapManager.CenterMercator;
            float worldRelScale = _mapManager.WorldRelativeScale;


            for (int i=0; i<4; i++) {
                Vector3 worldPosition = Conversions.GeoToWorldPosition(_coordinates[i].x, _coordinates[i].y, mapRef, worldRelScale).ToVector3xz();
                vertices[i] = worldPosition;
            }

            _meshfilter.mesh.vertices = vertices;
        }
    }
}
