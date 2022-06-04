using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class IndicatorWeight : MonoBehaviour
{
    [SerializeField]
    public TMP_Text indicatorName;

    [SerializeField]
    public TMP_InputField weight;


    void Awake()
    {
        Assert.IsNotNull(indicatorName);
        Assert.IsNotNull(weight);
    }
}
