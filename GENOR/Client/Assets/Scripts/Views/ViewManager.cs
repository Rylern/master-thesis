using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mapbox.Unity.Map;


public class ViewManager : MonoBehaviour
{
    [SerializeField]
    private AbstractMap _mapManager;

    [SerializeField]
    private IndicatorsController _indicators;

    private CameraManager _cameraManager;
    private SimulationView _view;
    private const float _zoom3D = 15;
    private float _zoom2D;


    private void Awake()
    {
        _cameraManager = GetComponent<CameraManager>();

        _zoom2D = _mapManager.Zoom;
        _view = SimulationView.TwoDimension;
        _cameraManager.Set2D();
        _indicators.Set2D();

        Assert.IsNotNull(_mapManager);
        Assert.IsNotNull(_indicators);
        Assert.IsNotNull(_cameraManager);
    }

    public void Set2D()
    {
        if (_view == SimulationView.ThreeDimension) {
            _cameraManager.ChangeCamera();

            _mapManager.Terrain.SetElevationType(ElevationLayerType.FlatTerrain);
            _mapManager.Terrain.DisableSideWalls();
            
            _mapManager.VectorData.GetFeatureSubLayerAtIndex(0).SetActive(false);

            _mapManager.SetExtent(MapExtentType.RangeAroundCenter);
            
            _mapManager.ImageLayer.SetLayerSource(ImagerySourceType.MapboxStreets);
            _mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, _zoom2D);

            _indicators.SwitchView(SimulationView.TwoDimension);

            _view = SimulationView.TwoDimension;
        }
    }

    private void Set3D()
    {
        _cameraManager.ChangeCamera();

        _mapManager.Terrain.SetElevationType(ElevationLayerType.TerrainWithElevation);
        _mapManager.Terrain.EnableSideWalls(10, null);

        _mapManager.VectorData.GetFeatureSubLayerAtIndex(0).SetActive(true);

        _mapManager.SetExtent(MapExtentType.RangeAroundTransform);

        _mapManager.ImageLayer.SetLayerSource(ImagerySourceType.MapboxSatellite);

        _zoom2D = _mapManager.Zoom;
        _mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, _zoom3D);

        _indicators.SwitchView(SimulationView.ThreeDimension);

        _view = SimulationView.ThreeDimension;
    }

    public void SwitchView()
    {
        if (_view == SimulationView.TwoDimension) {
            Set3D();
        } else {
            Set2D();
        }
        
    }
}