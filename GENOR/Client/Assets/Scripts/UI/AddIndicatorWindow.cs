using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;
using SimpleFileBrowser;
using System.IO;


public class AddIndicatorWindow : MonoBehaviour
{
    private static string DEFAULT_MESSAGE = "No file choosen";

    [SerializeField]
    private UIController _uiController;

    [SerializeField]
    private TMP_Text _filename;

    [SerializeField]
    private Button _browse;

    [SerializeField]
    private Button _cancel;

    [SerializeField]
    private Button _submit;

    private CanvasGroup _canvasGroup;


    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _browse.onClick.AddListener(BrowsePressed);
        _cancel.onClick.AddListener(ChangeVisibility);
        _submit.onClick.AddListener(SubmitPressed);

        Assert.IsNotNull(_uiController);
        Assert.IsNotNull(_filename);
        Assert.IsNotNull(_browse);
        Assert.IsNotNull(_cancel);
        Assert.IsNotNull(_submit);
        Assert.IsNotNull(_canvasGroup);
    }

    private void SubmitPressed()
    {
        if (File.Exists(_filename.text)) {
            StartCoroutine(APIClient.Post<StatusResponse>("indicator/", _filename.text, onAddCategoryResponse));
        }
    }

    private void onAddCategoryResponse(StatusResponse addCategoryResponse)
    {
        if (addCategoryResponse != null) {
            ChangeVisibility();
            _uiController.SetStatus("Indicator added");
        } else {
            _uiController.SetStatus("Failed to add the indicator");
        }
    }

    private void BrowsePressed()
    {
        FileBrowser.ShowLoadDialog(OnDialogSuccess, OnDialogCancel, 0 );
    }

    public void OnDialogSuccess(string[] paths)
    {
        foreach (string path in paths) {
            _filename.text = path;
        }
    }
    public void OnDialogCancel()
    {}

    public void ChangeVisibility()
    {
        if (_canvasGroup.alpha == 1f) {
            _filename.text = "";
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        } else {
            _filename.text = DEFAULT_MESSAGE;
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
