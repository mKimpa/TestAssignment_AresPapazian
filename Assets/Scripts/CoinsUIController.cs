using System.ComponentModel.Design;
using System.Globalization;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class TopBarController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI CoinsAmount;
    

    private void Awake()
    {
        UpdateCoinsAmount(GameManager.instance.GetCoins());                        
    }

    private void Start()
    {
        GameManager.instance.OnCoinsAmountChange += Instance_OnCoinsAmountChange;
    }

    private void Instance_OnCoinsAmountChange(object sender, GameManager.OnCoinsAmountChangeArgs e)
    {
        UpdateCoinsAmount(GameManager.instance.GetCoins());
    }

    private void UpdateCoinsAmount(int amount)
    {
        if (CoinsAmount != null)
        {
            CoinsAmount.text = amount.ToString("N0", new CultureInfo("en-US"));
        }
    }

}
