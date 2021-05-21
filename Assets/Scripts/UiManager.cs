using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    public static UiManager Singleton;

    public TextMeshProUGUI GoldText;
    public TextMeshProUGUI CoinText;
    public TextMeshProUGUI HpText;
    public TextMeshProUGUI ManaText;

    private void Awake()
    {
        Singleton = this;
    }

    private void OnEnable()
    {
        EventManager.Singleton.E_ValuableItemAdded += SyncValuables;
    }

    // Start is called before the first frame update
    void Start()
    {
        SyncValuables(null);
    }

    public void SyncValuables(ValueableItem item)
    {
        int gold = PlayerController.Singleton.Inventory.Gold;
        int coins = PlayerController.Singleton.Inventory.Coins;

        GoldText.text = "Gold: " + gold;
        CoinText.text = "Coins: " + coins;
    }

    public void SyncStats()
    {
        int hp = PlayerController.Singleton.Inventory.Gold;
        int mana = PlayerController.Singleton.Inventory.Coins;

        HpText.text = "Hp: " + hp;
        ManaText.text = "Mana: " + mana;
    }
}
