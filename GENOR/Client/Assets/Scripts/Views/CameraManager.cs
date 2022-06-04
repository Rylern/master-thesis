using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Camera _camera2D;

    [SerializeField]
    private Camera _camera3D;

    void Awake()
    {
        Assert.IsNotNull(_camera2D);
        Assert.IsNotNull(_camera3D);
    }


    public void Set2D()
    {
        _camera2D.gameObject.SetActive(true);
        _camera3D.gameObject.SetActive(false);
    }


    public void ChangeCamera()
    {
        _camera2D.gameObject.SetActive(!_camera2D.gameObject.activeSelf);
        _camera3D.gameObject.SetActive(!_camera3D.gameObject.activeSelf);
    }
}