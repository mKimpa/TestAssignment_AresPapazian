using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }

    public event EventHandler<OnCoinsAmountChangeArgs> OnCoinsAmountChange;
    public class OnCoinsAmountChangeArgs : EventArgs
    {
        public int Value;
    }

    private int coins = 1850;
    private int lifes = 5;
    private int stars = 165;

    private int lifesCapacity = 5;

    private void Awake()
    {
        instance = this;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinsAmountChange?.Invoke(this, new OnCoinsAmountChangeArgs
        {
            Value = coins,
        });
    }

    public int GetCoins()
    {
        return coins;
    }

    public int GetLifes()
    {
        return lifes;
    }

    public int GetStars()
    {
        return stars;
    }

    public bool IsLivesFull()
    {
        return lifes >= lifesCapacity;
    }
}
