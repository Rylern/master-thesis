using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DeleteCategoryWindow : MonoBehaviour
{
    private List<string> _categoriesChoosen;
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private Transform _scroll;

    [SerializeField]
    private Dropdown _categoryChooser;

    [SerializeField]
    private Button _addCategory;

    [SerializeField]
    private Button _cancel;

    [SerializeField]
    private Button _submit;

    [SerializeField]
    private GameObject _categoryContainerPrefab;

    [SerializeField]
    private UIController _uiController;

    [SerializeField]
    private MenuBar _menuBar;


    void Awake()
    {
        _categoriesChoosen = new List<string>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _addCategory.onClick.AddListener(AddCategoryPressed);
        _cancel.onClick.AddListener(ChangeVisibility);
        _submit.onClick.AddListener(SubmitPressed);

        Assert.IsNotNull(_canvasGroup);
        Assert.IsNotNull(_scroll);
        Assert.IsNotNull(_categoryChooser);
        Assert.IsNotNull(_addCategory);
        Assert.IsNotNull(_cancel);
        Assert.IsNotNull(_submit);
        Assert.IsNotNull(_categoryContainerPrefab);
        Assert.IsNotNull(_uiController);
        Assert.IsNotNull(_menuBar);
    }

    private void AddCategoryPressed()
    {
        if (_categoryChooser.captionText.text != "") {
            GameObject container = Instantiate(_categoryContainerPrefab, _scroll) as GameObject;
            IndicatorContainer categoryContainer = container.GetComponent<IndicatorContainer>();
            
            categoryContainer.indicatorName.text = _categoryChooser.captionText.text;

            categoryContainer.delete.onClick.AddListener(delegate{CategoryDeletedFromList(categoryContainer);});

            _categoriesChoosen.Add(_categoryChooser.captionText.text);

            for(int i = 0; i < _categoryChooser.options.Count; i++) {
                if(_categoryChooser.options[i].text == _categoryChooser.captionText.text) {
                    _categoryChooser.options.RemoveAt(i);
                    break;
                }
            }
            _categoryChooser.value = 0;
            if (_categoryChooser.options.Count > 0) {
                _categoryChooser.captionText.text = _categoryChooser.options[0].text;
            } else {
                _categoryChooser.captionText.text = "";
            }
        }
    }

    private void CategoryDeletedFromList(IndicatorContainer categoryContainer)
    {
        string[] categories = { categoryContainer.indicatorName.text };
        _categoryChooser.AddOptions(new List<string>(categories));
        _categoriesChoosen.Remove(_categoryChooser.captionText.text);
        GameObject.Destroy(categoryContainer.gameObject);
    }

    private void SubmitPressed()
    {
        string url = "category/?";
        foreach (string category in _categoriesChoosen) {
            url += "categories=" + category + "&";
        }
        url = url.Remove(url.Length - 1, 1);

        StartCoroutine(APIClient.Delete<StatusResponse>(url, onDeleteCategoryResponse));
    }

    private void onDeleteCategoryResponse(StatusResponse deleteCategoryResponse)
    {
        if (deleteCategoryResponse != null) {
            ChangeVisibility();
            _uiController.SetStatus("Categories deleted");
        } else {
            _uiController.SetStatus("Failed to delete some or all categories");
        }
    }

    public void ChangeVisibility()
    {
        if (_canvasGroup.alpha == 1f) {
            _categoryChooser.ClearOptions();
            _categoriesChoosen.Clear();

            foreach (Transform child in _scroll) {
                GameObject.Destroy(child.gameObject);
            }
            _menuBar.EnableDeleteIndicatorButton();

            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        } else {
            StartCoroutine(APIClient.Get<CategoryResponse>("category/", onCategoryResponse));
        }
    }

    private void onCategoryResponse(CategoryResponse categoryResponse) {
        if (categoryResponse != null) {
            List<string> categories = new List<string>();
            foreach (Category category in categoryResponse.categories) {
                categories.Add(category.name);
            }
            _categoryChooser.AddOptions(categories);
            _categoryChooser.value = 0;

            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }
        _menuBar.EnableDeleteIndicatorButton();
    }
}
