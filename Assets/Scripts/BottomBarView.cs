using System;
using UnityEngine;

public class BottomBarView : MonoBehaviour
{
    public static BottomBarView instance;

    public event EventHandler ContentActivated;
    public event EventHandler Closed;

    [SerializeField] private GameObject ButtonsContainer;

    private BottomButton[] bottomButtons;
    private int selectedButtonIndex = -1;

    private void Awake()
    {
        instance = this;
        bottomButtons = ButtonsContainer.GetComponentsInChildren<BottomButton>();
    }

    public void ButtonClick(int index)
    {
        if (selectedButtonIndex == index)
        {
            bottomButtons[index].SelectButton(false);
            selectedButtonIndex = -1;
            Closed?.Invoke(this, EventArgs.Empty);
            return;
        }
        if (selectedButtonIndex >= 0)
        {
            bottomButtons[selectedButtonIndex].SelectButton(false);
        }
        selectedButtonIndex = index;
        bottomButtons[index].SelectButton(true);
        ContentActivated?.Invoke(this, EventArgs.Empty);
    }
}
