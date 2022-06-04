using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class StatusBar : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _status;


    void Awake()
    {
        Assert.IsNotNull(_status);
    }

    public void SetStatus(string message)
    {
        _status.text = message;
    }
}
