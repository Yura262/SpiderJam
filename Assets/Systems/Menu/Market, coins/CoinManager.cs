using System.Configuration;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    int prevCoins=0;
    public int coins = 0;
    public TextMeshProUGUI coinText;

    private const string COIN_KEY = "PlayerCoins";
    private void Update()
    {
        if (coins != prevCoins) {
            
            coins = Mathf.Max(0, coins);
            prevCoins = coins;
            SaveCoins();
            UpdateUI();
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadCoins();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        LoadCoins();
        coins += amount;
        SaveCoins();
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        LoadCoins();
        if (coins >= amount)
        {
            coins -= amount;
            SaveCoins();
            UpdateUI();
            return true;
        }
        return false;
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COIN_KEY, coins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        //coins = 10000;
        coins = PlayerPrefs.GetInt(COIN_KEY, 0);
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
}
