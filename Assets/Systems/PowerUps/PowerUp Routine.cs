using UnityEngine;
using System.Collections;

/// <summary>
/// Містить список можливих бафів та відповідає за візуальне відображення їх
/// </summary>
public class PlayerPowerUpManager : MonoBehaviour
{
    public enum PowerUpType { Shield, FocusTime, SuperPull, OtherShield, WebLength,WebSpeed,Coin }

    public float shieldMultiplier = 1f;

    private SpiderController player;


    // --- Загальні посилання ---
    private SpiderController spiderController;
    private SpriteRenderer spiderSpriteRenderer;
    private Color originalSpiderColor;

    // --- Загальні стани ---
    public bool hasFocusTime { get; private set; }
    public bool hasSuperPull { get; private set; }
    // (Для "Монетки" стан не потрібен, бо вона миттєва)

    // --- Налаштування візуальних ефектів ---
    [Header("Visual Effects")]
    [SerializeField] private Color focusTimeColor = Color.blue;
    [SerializeField] private Color superPullColor = Color.red;

    private void Start()
    {
        //shopBoughtItemsManager = GetComponent<ShopBoughtItemsManager>();
        if (PlayerPrefs.GetInt("ShopItem_" + "+ShieldDuration") == 1)
        {
            shieldMultiplier = 1.5f;
        }
        player = GameManager.Instance.player.GetComponent<SpiderController>();

        // Деактивуємо всі візуальні ефекти на старті
        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
        }
        spiderController = gameObject.GetComponent<SpiderController>();
    }
    public void ActivatePowerUp(PowerUpType type, float duration)
    {
        switch (type)
        {
            case PowerUpType.Shield:
                ActivateShield( duration* shieldMultiplier);
                break;
            case PowerUpType.FocusTime:
                StartCoroutine(FocusTimeRoutine(duration));
                break;
            case PowerUpType.SuperPull:
                StartCoroutine(SuperPullRoutine(duration));
                break;
            case PowerUpType.OtherShield:
                StartCoroutine(ShieldRoutine(duration * shieldMultiplier));
                break;
            case PowerUpType.WebLength:
                StartCoroutine(WebLengthRoutine(duration));

                break;
            case PowerUpType.WebSpeed:
                //StartCoroutine(WebSpeedRoutine(duration));

                break;
            case PowerUpType.Coin:
                CoinManager.Instance.AddCoins(1);

                break;
        }   
    }

    /// <summary>
    /// Оновлює колір павука залежно від активних бафів (з пріоритетом).
    /// </summary>
    private void UpdateSpiderColor()
    {
        if (spiderSpriteRenderer == null) return;

        // SuperPull має найвищий пріоритет
        if (hasSuperPull)
        {
            spiderSpriteRenderer.color = superPullColor;
        }
        // Далі йде FocusTime
        else if (hasFocusTime)
        {
            spiderSpriteRenderer.color = focusTimeColor;
        }
        // Якщо бафів немає, повертаємо оригінальний колір
        else
        {
            spiderSpriteRenderer.color = originalSpiderColor;
        }
    }


    #region Shield
    [Header("Shield")]
    public GameObject shieldObject;

    public bool hasShield { get; private set; }

    private float shieldTimer;
    private Coroutine shieldCoroutine;

    private IEnumerator ShieldRoutine(float duration)
    {
        hasShield = true;
        shieldObject.SetActive(true);
        Debug.Log("Shield Activated");

        shieldTimer = duration;
        while (shieldTimer > 0f)
        {
            shieldTimer -= Time.deltaTime;
            yield return null; // wait one frame
        }

        hasShield = false;
        shieldObject.SetActive(false);
        Debug.Log("Shield Deactivated");
    }

    public void ActivateShield(float duration)
    {
        if (shieldCoroutine == null)
        {
            shieldCoroutine = StartCoroutine(ShieldRoutine(duration));
        }
        else
        {
            // Extend current shield
            shieldTimer += duration;
            Debug.Log($"Shield extended by {duration:F1}s, new remaining time: {shieldTimer:F1}s");
        }
    }
    #endregion




    #region WebLenght


    private IEnumerator WebLengthRoutine(float duration)
    {
        player.webvLMultiplier =1.5f;
        //shieldObject.SetActive(true);
        Debug.Log("webl Activated");
        if (PlayerPrefs.GetInt("ShopItem_" + "+WebLengthDuration") == 1)
        {
            duration *= 2f;
        }
        yield return new WaitForSeconds(duration);

        player.webvLMultiplier = 1f;
        //shieldObject.SetActive(false);
        Debug.Log("webl Deactivated");
    }
    #endregion


    // -------------------------------------------------------------------
    #region Focus Time PowerUp
    // -------------------------------------------------------------------
    private IEnumerator FocusTimeRoutine(float duration)
    {
        hasFocusTime = true;
        UpdateSpiderColor(); // Оновлюємо колір
        Debug.Log("Focus Time Activated");

        yield return new WaitForSeconds(duration);

        hasFocusTime = false;
        UpdateSpiderColor(); // Перевіряємо, який колір тепер ставити
        Debug.Log("Focus Time Deactivated");
    }
    #endregion
    // -------------------------------------------------------------------


    // -------------------------------------------------------------------
    #region Super Pull PowerUp
    // -------------------------------------------------------------------
    [Header("Super Pull")]
    [SerializeField] private float superPullMultiplier = 3f;

    private IEnumerator SuperPullRoutine(float duration)
    {
        hasSuperPull = true;
        UpdateSpiderColor(); // Оновлюємо колір
        Debug.Log("Super Pull Activated");

        if (spiderController != null)
        {
            float originalSpeed = spiderController.defaultPullSpeed;
            spiderController.pullSpeed = originalSpeed * superPullMultiplier;
            if (PlayerPrefs.GetInt("ShopItem_" + "+WebSpeedPowerup") == 1)
            {
                duration *= 2f;
            }
            yield return new WaitForSeconds(duration);

            spiderController.pullSpeed = originalSpeed;
        }
        else
        {
            Debug.LogError("SpiderController не знайдено!");
            yield return new WaitForSeconds(duration);
        }

        hasSuperPull = false;
        UpdateSpiderColor();
        Debug.Log("Super Pull Deactivated");
    }
    #endregion
    // -------------------------------------------------------------------





}
