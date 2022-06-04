using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController3D : MonoBehaviour
{
    private float _speed;
    private float _sensitivity;
    private Vector3 _anchorPoint;
    private Vector3 _anchorRot;
    private bool qwerty;

    void Awake()
    {
        _speed = 100f;
        _sensitivity = 0.2f;
        qwerty = true;
    }
    
    void LateUpdate()
    {
        // Translation
        Vector3 move = Vector3.zero;
        if (qwerty)
        {
            if (Input.GetKey(KeyCode.W))
                move += Vector3.forward * _speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A))
                move -= Vector3.right * _speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.Q))
                move -= Vector3.up * _speed * Time.deltaTime;
        }
        else
        {
            if (Input.GetKey(KeyCode.Z))
                move += Vector3.forward * _speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.Q))
                move -= Vector3.right * _speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A))
                move -= Vector3.up * _speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
            move -= Vector3.forward * _speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            move += Vector3.right * _speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E))
            move += Vector3.up * _speed * Time.deltaTime;
        transform.Translate(move);

        // Rotation
        if (Input.GetMouseButtonDown(1))
        {
            _anchorPoint = new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            _anchorRot = transform.eulerAngles;
        }
        if (Input.GetMouseButton(1))
        {
            Vector3 rot = _anchorRot;
            Vector3 dif = _anchorPoint - new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            rot += dif * _sensitivity;
            transform.eulerAngles = rot;
        }
    }
}