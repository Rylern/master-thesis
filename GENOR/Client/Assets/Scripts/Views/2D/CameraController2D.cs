using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using System;

public class CameraController2D : MonoBehaviour
{
    [SerializeField]
    [Range(1, 20)]
    public float _panSpeed = 1.0f;

    [SerializeField]
    AbstractMap _mapManager;

    [SerializeField]
    bool _useDegreeMethod;

    private Camera _camera;
    private Vector3 _origin;
    private Vector3 _mousePosition;
    private Vector3 _mousePositionPrevious;
    private bool _shouldDrag;
    private bool _isInitialized = false;
    private Plane _groundPlane = new Plane(Vector3.up, 0);
    private bool _dragStartedOnUI = false;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        
        _mapManager.OnInitialized += () =>
        {
            _isInitialized = true;
        };

        Assert.IsNotNull(_mapManager);
        Assert.IsNotNull(_camera);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
        {
            _dragStartedOnUI = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dragStartedOnUI = false;
        }
    }


    private void LateUpdate()
    {
        if (_isInitialized && !_dragStartedOnUI) {
            if (Input.touchSupported && Input.touchCount > 0)
            {
                HandleTouch();
            } else {
                HandleMouse();
            }
        }
    }

    void HandleMouse()
    {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            ZoomMapUsingTouchOrMouse(Input.GetAxis("Mouse ScrollWheel"));
        }

        PanMapUsingTouchOrMouse();
    }

    void HandleTouch()
    {
        float zoomFactor = 0.0f;
        //pinch to zoom.
        switch (Input.touchCount)
        {
            case 1:
                {
                    PanMapUsingTouchOrMouse();
                }
                break;
            case 2:
                {
                    // Store both touches.
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    zoomFactor = 0.01f * (touchDeltaMag - prevTouchDeltaMag);
                }
                ZoomMapUsingTouchOrMouse(zoomFactor);
                break;
            default:
                break;
        }
    }

    void ZoomMapUsingTouchOrMouse(float zoomFactor)
    {
        int deltaZoom;
        if (zoomFactor > 0) {
            deltaZoom = 1;
        } else if (zoomFactor < 0) {
            deltaZoom = -1;
        } else {
            deltaZoom = 0;
        }

        var zoom = Mathf.Max(0.0f, Mathf.Min(_mapManager.Zoom + deltaZoom, 21.0f));
        if (Math.Abs(zoom - _mapManager.Zoom) > 0.0f)
        {
            _mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, zoom);
        }
    }

    void PanMapUsingTouchOrMouse()
    {
        if (_useDegreeMethod)
        {
            UseDegreeConversion();
        }
        else
        {
            UseMeterConversion();
        }
    }

    void UseMeterConversion()
    {
        if (Input.GetMouseButtonUp(1))
        {
            var mousePosScreen = Input.mousePosition;
            //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
            //http://answers.unity3d.com/answers/599100/view.html
            mousePosScreen.z = _camera.transform.localPosition.y;
            var pos = _camera.ScreenToWorldPoint(mousePosScreen);

            var latlongDelta = _mapManager.WorldToGeoPosition(pos);
        }

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var mousePosScreen = Input.mousePosition;
            //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
            //http://answers.unity3d.com/answers/599100/view.html
            mousePosScreen.z = _camera.transform.localPosition.y;
            _mousePosition = _camera.ScreenToWorldPoint(mousePosScreen);

            if (_shouldDrag == false)
            {
                _shouldDrag = true;
                _origin = _camera.ScreenToWorldPoint(mousePosScreen);
            }
        }
        else
        {
            _shouldDrag = false;
        }

        if (_shouldDrag == true)
        {
            var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
            if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
            {
                _mousePositionPrevious = _mousePosition;
                var offset = _origin - _mousePosition;

                if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
                {
                    if (null != _mapManager)
                    {
                        float factor = _panSpeed * Conversions.GetTileScaleInMeters((float)0, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
                        var latlongDelta = Conversions.MetersToLatLon(new Vector2d(offset.x * factor, offset.z * factor));
                        var newLatLong = _mapManager.CenterLatitudeLongitude + latlongDelta;

                        _mapManager.UpdateMap(newLatLong, _mapManager.Zoom);
                    }
                }
                _origin = _mousePosition;
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                _mousePositionPrevious = _mousePosition;
                _origin = _mousePosition;
            }
        }
    }

    void UseDegreeConversion()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var mousePosScreen = Input.mousePosition;
            //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
            //http://answers.unity3d.com/answers/599100/view.html
            mousePosScreen.z = _camera.transform.localPosition.y;
            _mousePosition = _camera.ScreenToWorldPoint(mousePosScreen);

            if (_shouldDrag == false)
            {
                _shouldDrag = true;
                _origin = _camera.ScreenToWorldPoint(mousePosScreen);
            }
        }
        else
        {
            _shouldDrag = false;
        }

        if (_shouldDrag == true)
        {
            var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
            if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
            {
                _mousePositionPrevious = _mousePosition;
                var offset = _origin - _mousePosition;

                if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
                {
                    if (null != _mapManager)
                    {
                        // Get the number of degrees in a tile at the current zoom level.
                        // Divide it by the tile width in pixels ( 256 in our case)
                        // to get degrees represented by each pixel.
                        // Mouse offset is in pixels, therefore multiply the factor with the offset to move the center.
                        float factor = _panSpeed * Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;

                        var latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + offset.z * factor, _mapManager.CenterLatitudeLongitude.y + offset.x * factor);
                        _mapManager.UpdateMap(latitudeLongitude, _mapManager.Zoom);
                    }
                }
                _origin = _mousePosition;
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                _mousePositionPrevious = _mousePosition;
                _origin = _mousePosition;
            }
        }
    }
}