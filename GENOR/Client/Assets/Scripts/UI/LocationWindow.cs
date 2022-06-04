using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;
using Mapbox.Geocoding;
using Mapbox.Utils;
using Mapbox.Unity;
using Mapbox.Unity.Map;



public class LocationWindow : MonoBehaviour
{
    [SerializeField]
    private AbstractMap _mapManager;

    [SerializeField]
    private TMP_InputField _location;

    [SerializeField]
    private Button _search;

    private ForwardGeocodeResource _resource;
    private CanvasGroup _canvasGroup;


    void Awake()
    {
        _search.onClick.AddListener(SearchPressed);

        _resource = new ForwardGeocodeResource("");
        _canvasGroup = GetComponent<CanvasGroup>();

        Assert.IsNotNull(_mapManager);
        Assert.IsNotNull(_location);
        Assert.IsNotNull(_search);
        Assert.IsNotNull(_resource);
        Assert.IsNotNull(_canvasGroup);
    }

    void HandleGeocoderResponse(ForwardGeocodeResponse res)
    {
        if (null != res && null != res.Features && res.Features.Count > 0) {
			_mapManager.UpdateMap(res.Features[0].Center, _mapManager.AbsoluteZoom);
        }
    }

    void SearchPressed()
    {
        if (!string.IsNullOrEmpty(_location.text))
        {
            _resource.Query = _location.text;
            MapboxAccess.Instance.Geocoder.Geocode(_resource, HandleGeocoderResponse);
            ChangeVisibility();
        }
    }

    public void ChangeVisibility()
    {
        if (_canvasGroup.alpha == 1f) {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        } else {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
