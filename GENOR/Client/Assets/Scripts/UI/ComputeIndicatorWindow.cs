using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class ComputeIndicatorWindow : MonoBehaviour
{
    [SerializeField]
    private IndicatorsController _indicatorsController;

    [SerializeField]
    private GameObject _page1;

    [SerializeField]
    private TMP_InputField _tileSize;

    [SerializeField]
    private Transform _categoryList;

    [SerializeField]
    private Button _indicatorNext1;

    [SerializeField]
    private Button _indicatorBack1;

    [SerializeField]
    private GameObject _page2;

    [SerializeField]
    private Transform _indicatorList;

    [SerializeField]
    private Button _indicatorNext2;

    [SerializeField]
    private Button _indicatorBack2;

    [SerializeField]
    private GameObject _indicatorWeightPrefab;

    [SerializeField]
    private GameObject _categorySelectorPrefab;

    [SerializeField]
    private UIController _uiController;

    [SerializeField]
    private MenuBar _menuBar;

    private CategoryResponse _categoryResponse;
    private CanvasGroup _canvasGroup;


    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _indicatorNext1.onClick.AddListener(next1Pressed);
        _indicatorBack1.onClick.AddListener(back1Pressed);
        _indicatorNext2.onClick.AddListener(next2Pressed);
        _indicatorBack2.onClick.AddListener(back2Pressed);

        Assert.IsNotNull(_indicatorsController);
        Assert.IsNotNull(_page1);
        Assert.IsNotNull(_tileSize);
        Assert.IsNotNull(_categoryList);
        Assert.IsNotNull(_indicatorNext1);
        Assert.IsNotNull(_indicatorBack1);
        Assert.IsNotNull(_page2);
        Assert.IsNotNull(_indicatorList);
        Assert.IsNotNull(_indicatorNext2);
        Assert.IsNotNull(_indicatorBack2);
        Assert.IsNotNull(_indicatorWeightPrefab);
        Assert.IsNotNull(_categorySelectorPrefab);
        Assert.IsNotNull(_uiController);
        Assert.IsNotNull(_menuBar);
        Assert.IsNotNull(_canvasGroup);
    }

    void next1Pressed()
    {
        bool atLeastOneIndicatorChosen = false;
        foreach (Transform category in _categoryList) {
            atLeastOneIndicatorChosen = atLeastOneIndicatorChosen || category.gameObject.GetComponent<CategorySelector>().category.isOn;
        }

        if (atLeastOneIndicatorChosen) {
            foreach (Transform child in _indicatorList) {
                GameObject.Destroy(child.gameObject);
            }
        
            foreach (Transform category in _categoryList) {
                CategorySelector categorySelector = category.gameObject.GetComponent<CategorySelector>();

                if (categorySelector.category.isOn) {
                    foreach (Category groupIndicatorResponse in _categoryResponse.categories) {
                        if (groupIndicatorResponse.name == categorySelector.categoryName.text) {
                            foreach(string indicator in groupIndicatorResponse.indicators) {
                                GameObject container = Instantiate(_indicatorWeightPrefab, _indicatorList) as GameObject;
                                IndicatorWeight indicatorWeight = container.GetComponent<IndicatorWeight>();

                                indicatorWeight.indicatorName.text = indicator;
                            }
                        }
                    }
                }
            }

            _indicatorsController.SetTileSize(_tileSize.text);

            _page1.SetActive(false);
            _page2.SetActive(true);
        }
    }

    void back1Pressed()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
    }

    void next2Pressed()
    {
        back1Pressed();
        back2Pressed();

        foreach (Transform indicator in _indicatorList) {
            IndicatorWeight indicatorWeight = indicator.gameObject.GetComponent<IndicatorWeight>();

            float weight;
            bool success = float.TryParse(indicatorWeight.weight.text, out weight);
            if (!success) {
                weight = 0;
            }
            _indicatorsController.AddIndicator(new Indicator(indicatorWeight.indicatorName.text, weight));
        }

        _indicatorsController.LaunchAreaSelection();
        _uiController.SetStatus("Select an area");
    }

    void back2Pressed()
    {
        _page1.SetActive(true);
        _page2.SetActive(false);
    }

    public void ChangeVisibility()
    {
        if (_canvasGroup.alpha == 1f) {
            back2Pressed();
            back1Pressed();
            _menuBar.EnableIndicatorButton();
        } else {
            foreach (Transform child in _categoryList) {
                GameObject.Destroy(child.gameObject);
            }

            StartCoroutine(APIClient.Get<CategoryResponse>("category/", onCategoryResponse));
        }
    }

    private void onCategoryResponse(CategoryResponse categoryResponse) {
        _categoryResponse = categoryResponse;
        if (_categoryResponse != null) {
            foreach (Category category in _categoryResponse.categories) {
                GameObject container = Instantiate(_categorySelectorPrefab, _categoryList) as GameObject;
                CategorySelector categorySelector = container.GetComponent<CategorySelector>();

                categorySelector.categoryName.text = category.name;
            }

            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }
        _menuBar.EnableIndicatorButton();
    }
}
