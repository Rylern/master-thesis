using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class CategorySelector : MonoBehaviour
{
    [SerializeField]
    public Toggle category;

    [SerializeField]
    public TMP_Text categoryName;


    void Awake()
    {
        Assert.IsNotNull(category);
        Assert.IsNotNull(categoryName);
    }
}
