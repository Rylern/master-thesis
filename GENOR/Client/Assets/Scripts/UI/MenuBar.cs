using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class MenuBar : MonoBehaviour
{
    [SerializeField]
    private ViewManager _viewManager;

    [SerializeField]
    private AddIndicatorWindow _addIndicatorWindow;

    [SerializeField]
    private AddCategoryWindow _addCategoryWindow;

    [SerializeField]
    private DeleteCategoryWindow _deleteCategoryWindow;

    [SerializeField]
    private ComputeIndicatorWindow _computeIndicatorWindow;

    [SerializeField]
    private LocationWindow _locationWindow;

    [SerializeField]
    private Button _addIndicator;

    [SerializeField]
    private Button _addCategory;

    [SerializeField]
    private Button _deleteCategory;

    [SerializeField]
    private Button _indicatorComputation;

    [SerializeField]
    private Button _selectLocation;

    [SerializeField]
    private Button _changeView;

    [SerializeField]
    private Button _quit;
    
    void Awake()
    {
        _addIndicator.onClick.AddListener(AddIndicatorPressed);
        _addCategory.onClick.AddListener(AddCategoryPressed);
        _deleteCategory.onClick.AddListener(DeleteCategoryPressed);
        _indicatorComputation.onClick.AddListener(IndicatorComputationPressed);
        _selectLocation.onClick.AddListener(SelectLocationPressed);
        _changeView.onClick.AddListener(ChangeViewPressed);
        _quit.onClick.AddListener(QuitPressed);

        Assert.IsNotNull(_viewManager);
        Assert.IsNotNull(_addIndicatorWindow);
        Assert.IsNotNull(_addCategoryWindow);
        Assert.IsNotNull(_deleteCategoryWindow);
        Assert.IsNotNull(_computeIndicatorWindow);
        Assert.IsNotNull(_locationWindow);
        Assert.IsNotNull(_addIndicator);
        Assert.IsNotNull(_addCategory);
        Assert.IsNotNull(_deleteCategory);
        Assert.IsNotNull(_indicatorComputation);
        Assert.IsNotNull(_selectLocation);
        Assert.IsNotNull(_changeView);
        Assert.IsNotNull(_quit);
    }

    void AddIndicatorPressed()
    {
        _addIndicatorWindow.ChangeVisibility();
    }

    void AddCategoryPressed()
    {
        _addCategory.interactable = false;
        _addCategoryWindow.ChangeVisibility();
    }

    void DeleteCategoryPressed()
    {
        _deleteCategory.interactable = false;
        _deleteCategoryWindow.ChangeVisibility();
    }

    void IndicatorComputationPressed()
    {
        _indicatorComputation.interactable = false;
        _computeIndicatorWindow.ChangeVisibility();
    }

    void SelectLocationPressed()
    {
        _locationWindow.ChangeVisibility();
    }

    void ChangeViewPressed()
    {
        _viewManager.SwitchView();
    }

    void QuitPressed()
    {
        Application.Quit();
    }

    public void EnableIndicatorButton()
    {
        _indicatorComputation.interactable = true;
    }

    public void EnableAddIndicatorButton()
    {
        _addCategory.interactable = true;
    }

    public void EnableDeleteIndicatorButton()
    {
        _deleteCategory.interactable = true;
    }
}
