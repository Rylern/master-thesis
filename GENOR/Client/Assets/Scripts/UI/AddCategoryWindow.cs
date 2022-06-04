using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class AddCategoryWindow : MonoBehaviour
{
    private List<string> _indicatorsChoosen;
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private TMP_InputField _name;

    [SerializeField]
    private Transform _scroll;

    [SerializeField]
    private Dropdown _indicatorChooser;

    [SerializeField]
    private Button _addIndicator;

    [SerializeField]
    private Button _cancel;

    [SerializeField]
    private Button _submit;

    [SerializeField]
    private GameObject _indicatorContainerPrefab;

    [SerializeField]
    private UIController _uiController;

    [SerializeField]
    private MenuBar _menuBar;
    


    void Awake()
    {
        _indicatorsChoosen = new List<string>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _addIndicator.onClick.AddListener(AddIndicatorPressed);
        _cancel.onClick.AddListener(ChangeVisibility);
        _submit.onClick.AddListener(SubmitPressed);

        Assert.IsNotNull(_canvasGroup);
        Assert.IsNotNull(_name);
        Assert.IsNotNull(_scroll);
        Assert.IsNotNull(_indicatorChooser);
        Assert.IsNotNull(_addIndicator);
        Assert.IsNotNull(_cancel);
        Assert.IsNotNull(_submit);
        Assert.IsNotNull(_indicatorContainerPrefab);
        Assert.IsNotNull(_uiController);
        Assert.IsNotNull(_menuBar);
    }

    private void AddIndicatorPressed()
    {
        if (_indicatorChooser.captionText.text != "") {
            GameObject container = Instantiate(_indicatorContainerPrefab, _scroll) as GameObject;
            IndicatorContainer indicatorContainer = container.GetComponent<IndicatorContainer>();

            indicatorContainer.indicatorName.text = _indicatorChooser.captionText.text;

            indicatorContainer.delete.onClick.AddListener(delegate{IndicatorDeletedFromList(indicatorContainer);});

            _indicatorsChoosen.Add(_indicatorChooser.captionText.text);

            for(int i = 0; i < _indicatorChooser.options.Count; i++) {
                if(_indicatorChooser.options[i].text == _indicatorChooser.captionText.text) {
                    _indicatorChooser.options.RemoveAt(i);
                    break;
                }
            }
            _indicatorChooser.value = 0;
            if (_indicatorChooser.options.Count > 0) {
                _indicatorChooser.captionText.text = _indicatorChooser.options[0].text;
            } else {
                _indicatorChooser.captionText.text = "";
            }
        }
    }

    private void IndicatorDeletedFromList(IndicatorContainer indicatorContainer)
    {
        string[] indicators = { indicatorContainer.indicatorName.text };
        _indicatorChooser.AddOptions(new List<string>(indicators));
        _indicatorsChoosen.Remove(indicatorContainer.indicatorName.text);
        GameObject.Destroy(indicatorContainer.gameObject);
    }

    private void SubmitPressed()
    {
        AddCategoryRequest addCategoryRequest = new AddCategoryRequest(_name.text, _indicatorsChoosen);
        StartCoroutine(APIClient.Post<AddCategoryRequest, StatusResponse>("category/", addCategoryRequest, onAddCategoryResponse));
    }

    private void onAddCategoryResponse(StatusResponse addCategoryResponse)
    {
        if (addCategoryResponse != null) {
            ChangeVisibility();
            _uiController.SetStatus("Category added");
        } else {
            _uiController.SetStatus("Failed to add the category");
        }
    }

    public void ChangeVisibility()
    {
        if (_canvasGroup.alpha == 1f) {
            _indicatorsChoosen.Clear();

            foreach (Transform child in _scroll) {
                GameObject.Destroy(child.gameObject);
            }

            _name.text = "";
            _menuBar.EnableAddIndicatorButton();

            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        } else {
            StartCoroutine(APIClient.Get<IndicatorResponse>("indicator/", onIndicatorResponse));
        }
    }

    private void onIndicatorResponse(IndicatorResponse indicatorResponse) {
        if (indicatorResponse != null) {
            _indicatorChooser.AddOptions(indicatorResponse.indicators);
            _indicatorChooser.value = 0;
            
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }
        _menuBar.EnableAddIndicatorButton();
    }
}
