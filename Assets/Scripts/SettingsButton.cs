using UnityEngine;
using UnityEngine.UI;

public class TopPanelButton : MonoBehaviour
{

    private void Awake()
    {
        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(button_OnClick);
        }
    }

    private void button_OnClick()
    {
        SettingsManager.Instance.OpenSettings();
    }

    private void OnDestroy()
    {
        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}