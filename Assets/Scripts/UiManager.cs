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

    [Header("Menu")]
    private static int menuPageID;
    public GameObject MainMenu;
    public GameObject ShopMenu;
    public GameObject SettingsMenu;
    public GameObject CreditsMenu;
    public GamePlayUi GameplayUi;

    private GameObject _activeWindow;
    private void Awake()
    {
        Singleton = this;
        menuPageID = 0;
    }

    private void OnEnable()
    {
        EventManager.Singleton.E_ValuableItemAdded += SyncValuables;
    }

    // Start is called before the first frame update
    void Start()
    {
        SyncValuables(null);
        SyncStats();
    }
    void Update()
    {

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
        int hp = PlayerController.Singleton.PlayerStats.HP;
        int mana = PlayerController.Singleton.PlayerStats.Mana;

        HpText.text = "Hp: " + hp;
        ManaText.text = "Mana: " + mana;
    }
    public void OpenMenuBtn()
    {
        GameManager.Singleton.PauseGame();
        GameplayUi.gameObject.SetActive(false); 
        OpenWindow(MainMenu);
    }

    public void PlayBtnMenu()
    {
        menuPageID = 0;
        GameManager.Singleton.ResumeGame();
        GameplayUi.gameObject.SetActive(true);
        MainMenu.SetActive(false);
    }
    public void ShopBtnMenu()
    {
        menuPageID = 1;
        MainMenu.SetActive(false);
        ShopMenu.SetActive(true);
    }
    public void SettingsBtnMenu()
    {
        menuPageID = 2;
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }
    public void creditsBtnMenu()
    {
        menuPageID = 3;
        SettingsMenu.SetActive(false);
        CreditsMenu.SetActive(true);
    }
    public void PlayStoreBtn()
    {
        // playstore link
    }
    public void BackBtnMEnu()
    {
        switch (menuPageID)
        {
            case 1:
                Debug.Log("ShopMenu");
                menuPageID = 0;
                MainMenu.SetActive(true);
                ShopMenu.SetActive(false);
                break;
            case 2:
                Debug.Log("SettingsMenu");
                menuPageID = 0;
                MainMenu.SetActive(true);
                SettingsMenu.SetActive(false);
                break;
            case 3:
                Debug.Log("creditsMenu");
                menuPageID = 2;
                CreditsMenu.SetActive(false);
                SettingsMenu.SetActive(true);
                break;
            default:
                Debug.Log("PlayMenu");
                menuPageID = 0;
                break;
        }
    }
    public void OpenWindow(GameObject window)
    {
        _activeWindow = window;
        _activeWindow.SetActive(true);
    }
    public void SwitchWindow(GameObject window)
    {
        _activeWindow.SetActive(false);
        OpenWindow(window);
    }

}
