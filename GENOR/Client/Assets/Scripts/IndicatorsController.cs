using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine.Assertions;


public class IndicatorsController : MonoBehaviour
{
    [SerializeField]
    private Transform _container2D;

    [SerializeField]
    private Transform _container3D;

    [SerializeField]
    private GameObject _indicatorTilePrefab;

    [SerializeField]
    private GameObject _indicatorBarPrefab;

    [SerializeField]
    private UIController _uiController;

    [SerializeField]
    private ViewManager _viewManager;

    private AreaSelection _areaSelection;
    private List<Indicator> _indicators;
    private string _tileSize;
    
    
    void Awake()
    {
        _areaSelection = GetComponent<AreaSelection>();
        _indicators = new List<Indicator>();

        Assert.IsNotNull(_container2D);
        Assert.IsNotNull(_container3D);
        Assert.IsNotNull(_indicatorTilePrefab);
        Assert.IsNotNull(_indicatorBarPrefab);
        Assert.IsNotNull(_uiController);
        Assert.IsNotNull(_viewManager);
        Assert.IsNotNull(_areaSelection);
    }

    private void DeleteIndicators()
    {
        foreach (Transform child in _container2D) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in _container3D) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void SetTileSize(string tileSize) {
        _tileSize = tileSize;
    }

    public void AddIndicator(Indicator indicator)
    {
        _indicators.Add(indicator);
    }

    public void LaunchAreaSelection()
    {
        _viewManager.Set2D();
        _areaSelection.Enable();
    }

    public void VisualizeWalkability(Vector2d latLonStartPosition, Vector2d latLonEndPosition)
    {
        string url = "indicator/compute?";

        double startingLongitude;
        double endingLongitude;
        double startingLatitude;
        double endingLatitude;
        if (latLonStartPosition[1] < latLonEndPosition[1]) {
            startingLongitude = latLonStartPosition[1];
            endingLongitude = latLonEndPosition[1];
        } else {
            startingLongitude = latLonEndPosition[1];
            endingLongitude = latLonStartPosition[1];
        }
        if (latLonStartPosition[0] < latLonEndPosition[0]) {
            startingLatitude = latLonStartPosition[0];
            endingLatitude = latLonEndPosition[0];
        } else {
            startingLatitude = latLonEndPosition[0];
            endingLatitude = latLonStartPosition[0];
        }

        url += "startingLongitude=" + startingLongitude.ToString() + "&";
        url += "startingLatitude=" + startingLatitude.ToString() + "&";
        url += "endingLongitude=" + endingLongitude.ToString() + "&";
        url += "endingLatitude=" + endingLatitude.ToString() + "&";
        foreach (Indicator indicator in _indicators) {
            url += "indicators=" + indicator.name + "&";
            url += "weights=" + indicator.weight.ToString() + "&";
        }
        url += "tileSize=" + _tileSize;

        _uiController.SetStatus("Computing indicators...");

        StartCoroutine(APIClient.Get<ComputeIndicatorResponse>(url, onComputeIndicatorResponse));
    }

    private void onComputeIndicatorResponse(ComputeIndicatorResponse computeIndicatorResponse)
    {
        if (computeIndicatorResponse != null) {
            DeleteIndicators();
            
            foreach (Feature feature in computeIndicatorResponse.features) {
                if (feature.properties.valueAvailable) {
                    float indicator = (float) feature.properties.globalIndicator;
                
                    List<Vector2d> coordinates = new List<Vector2d>();
                    Vector2d centroid = new Vector2d();

                    foreach (List<double> coordinate in feature.geometry.coordinates[0]) {
                        coordinates.Add(new Vector2d(coordinate[1], coordinate[0]));
                        centroid.x += coordinate[1];
                        centroid.y += coordinate[0];
                    }
                    centroid.x /= coordinates.Count;
                    centroid.y /= coordinates.Count;

                    GameObject indicatorTileObject = Instantiate(_indicatorTilePrefab, _container2D) as GameObject;
                    indicatorTileObject.GetComponent<IndicatorTile>().Create(coordinates, indicator);

                    GameObject indicatorBarObject = Instantiate(_indicatorBarPrefab, _container3D) as GameObject;
                    indicatorBarObject.GetComponent<IndicatorBar>().Create(centroid, indicator);
                }
            }
            _uiController.SetStatus("Indicators computed");

            _indicators = new List<Indicator>();
        } else {
            _uiController.SetStatus("Failed to compute the indicators");
        }
    }

    public void Set2D()
    {
        SwitchView(SimulationView.TwoDimension);
    }

    public void SwitchView(SimulationView newView)
    {
        _container2D.gameObject.SetActive(newView == SimulationView.TwoDimension);
        _container3D.gameObject.SetActive(newView == SimulationView.ThreeDimension);
    }
}