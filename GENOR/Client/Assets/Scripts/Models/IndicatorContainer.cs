using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class IndicatorContainer : MonoBehaviour
{
    [SerializeField]
    public Button delete;

    [SerializeField]
    public TMP_Text indicatorName;


    void Awake()
    {
        Assert.IsNotNull(delete);
        Assert.IsNotNull(indicatorName);
    }
}
