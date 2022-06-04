using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class UIController : MonoBehaviour
{
    [SerializeField]
    private StatusBar _statusBar;


    void Awake()
    {
        Assert.IsNotNull(_statusBar);
    }

    public void SetStatus(string message)
    {
        _statusBar.SetStatus(message);
    }
}
