using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopItem:MonoBehaviour
{
    public string itemName;
    public int cost;
    public Image icon;
    public bool purchased = false;
    bool prevPurchased = false;

    [Tooltip("Time (in seconds) player must stay to load scene")]
    public float holdDuration = 2f;

    [Tooltip("UI Image with Fill method set to Radial 360")]
    public Image loadingCircle;

    private float holdTimer = 0f;
    private bool playerInside = false;
    public TextMeshProUGUI costText;
    private string OwnedString = "OWNED";
    public AudioSource boughtSound;
    void Start()
    {

        purchased = PlayerPrefs.GetInt("ShopItem_" + itemName, 0) == 1;
        //purchased = false;
        //PlayerPrefs.SetInt("ShopItem_" + itemName, Convert.ToInt32(false));
        //PlayerPrefs.Save();

        loadingCircle.fillAmount = 0;
        if (purchased)
        {
            Color currentColor = icon.color;
            currentColor.a = 0.8f;
            icon.color = currentColor;

            costText.text = OwnedString;
        }
        else
        {
            costText.text = cost.ToString();
        }

    }
    private void Update()
    {
        if (purchased != prevPurchased)
        {
            PlayerPrefs.SetInt("ShopItem_" + itemName, Convert.ToInt32(purchased));
            PlayerPrefs.Save();
            prevPurchased= purchased;
            Start();
        }
        if (purchased)
        {
            return;
        }
        if (playerInside)
        {
            holdTimer += Time.deltaTime;
            float fill = Mathf.Clamp01(holdTimer / holdDuration);

            if (loadingCircle)
            {
                loadingCircle.fillAmount = fill;
            }

            if (fill >= 1f)
            {
                TryBuyItem();
            }
        }
        else if (holdTimer > 0f)
        {
            holdTimer = Mathf.MoveTowards(holdTimer, 0f, Time.deltaTime * 2);
            if (loadingCircle)
                loadingCircle.fillAmount = holdTimer / holdDuration;
        }
    }


    public void TryBuyItem()
    {
        if (purchased)
        {
            Debug.Log(itemName + " already purchased!");
            return;
        }
        Debug.Log(CoinManager.Instance.coins);
        if (CoinManager.Instance.SpendCoins(cost))
        {

            purchased = true;
            PlayerPrefs.SetInt("ShopItem_" + itemName, 1);
            PlayerPrefs.Save();
            boughtSound.Play();
            costText.text = OwnedString;
            Debug.Log("Purchased: " + itemName);
            Debug.Log(CoinManager.Instance.coins);
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }





    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CoinManager.Instance.coins < cost)
            return;
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (loadingCircle)
            {
                loadingCircle.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (loadingCircle)
                loadingCircle.gameObject.SetActive(false);
        }
    }
}