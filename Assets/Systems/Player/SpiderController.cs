using UnityEngine;

public class SpiderController : MonoBehaviour
{
    // --- Налаштування в інспекторі ---
    [Header("Grappling Hook")]

    // --- ЗМІНЕNO ---: Тепер це базова швидкість
    [SerializeField] public float defaultPullSpeed = 15f;

    // --- НОВЕ ---: Публічна властивість, яку буде змінювати PowerUpManager
    public float pullSpeed { get; set; }

    [SerializeField] protected float hookSpeed = 25f;
    [SerializeField] protected LayerMask grappleLayer;

    // --- НОВЕ: Налаштування для бафів ---
    [Header("PowerUps")]
    [SerializeField] private float focusTimeScale = 0.5f; // Наскільки уповільнювати час

    // --- Внутрішні змінні ---
    public Rigidbody2D rb { get; private set; }
    protected LineRenderer lineRenderer;
    protected Camera mainCamera;
    protected PlayerHealth playerHealth;

    protected Vector2 grapplePoint;
    public bool IsGrappling { get; private set; } = false;

    protected bool isShooting = false;
    protected Vector2 hookPosition;

    protected float initialGravityScale;
    protected ShopBoughtItemsManager shopBoughtItemsManager;


    public AudioSource webTooSmall;
    public float maxWebLength=50f;



    public float webvLMultiplier=1f;
    public float webvSPEPPDMultiplier=1f;



    // --- НОВЕ: Посилання на менеджер бафів ---
    private PlayerPowerUpManager powerUpManager;
    private bool isTimeSlowed = false; // Чи сповільнений час зараз

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        playerHealth = GetComponent<PlayerHealth>();
        mainCamera = Camera.main;
        initialGravityScale = rb.gravityScale;

        // --- НОВЕ: Отримуємо менеджер та ініціалізуємо швидкість ---
        powerUpManager = GetComponent<PlayerPowerUpManager>();
        pullSpeed = defaultPullSpeed; // Встановлюємо поточну швидкість = базовій

        // --- ЗМІНЕNO ---: Додаємо скидання часу на випадок смерті
        playerHealth.OnDeath += (DamageType none) =>
        {
            StopGrapple();
            ResetTimeScale(); // Переконуємось, що час повернувся до норми
        };
        if (PlayerPrefs.GetInt("ShopItem_" + "+WebPlayerLength") == 1)
        {
            maxWebLength *= 1.5f;
        }
        if (PlayerPrefs.GetInt("ShopItem_" + "+WebPlayerSpeed") == 1)
        {
            hookSpeed *= 1.8f;
        }
    }

    protected void Update()
    {
        if (!playerHealth.isAlive)
        {
            // --- НОВЕ ---: Ще одна перевірка на скидання часу
            ResetTimeScale();
            return;
        }

        // --- Логіка павутини ---
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        UpdateWeb();

        // --- НОВЕ: Обробляємо логіку уповільнення часу ---
        HandleFocusTime();
    }

    void FixedUpdate()
    {
        if (playerHealth.isAlive)
            if (IsGrappling)
            {
                Vector2 direction = (grapplePoint - (Vector2)transform.position).normalized;
                // --- ЗМІНЕНО ---: Тепер тут використовується `pullSpeed` (яка є public property)
                rb.AddForce(direction * pullSpeed);
            }
    }

    protected virtual void StartGrapple()
    {
        if (isShooting || IsGrappling || !playerHealth.isAlive) return;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, grappleLayer);

        if (hit.collider != null)
        {
            if ((hit.point - (Vector2)transform.position).magnitude > maxWebLength * webvLMultiplier)
                Debug.Log("limited web length"+ (hit.point - (Vector2)transform.position).magnitude.ToString());
            if ((hit.point - (Vector2)transform.position).magnitude > maxWebLength * webvLMultiplier)
            {
                if (webTooSmall)
                    webTooSmall.Play();
                return;
            }
            grapplePoint = hit.point;
            hookPosition = transform.position;
            isShooting = true;
            lineRenderer.positionCount = 2;
        }
    }

    private void StopGrapple()
    {
        IsGrappling = false;
        isShooting = false;
        rb.gravityScale = initialGravityScale;
        lineRenderer.positionCount = 0;

        // --- НОВЕ: Скидаємо час, коли відпускаємо кнопку ---
        ResetTimeScale();
    }

    private void UpdateWeb()
    {
        if (isShooting)
        {
            hookPosition = Vector2.MoveTowards(hookPosition, grapplePoint, hookSpeed* webvSPEPPDMultiplier * Time.deltaTime);

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hookPosition);

            if (hookPosition == grapplePoint)
            {
                isShooting = false;
                IsGrappling = true;
                rb.gravityScale = 0f;
            }
        }
        else if (IsGrappling)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    // --- НОВІ МЕТОДИ ДЛЯ БАФІВ ---

    /// <summary>
    /// Перевіряє, чи має бути активне уповільнення часу.
    /// </summary>
    private void HandleFocusTime()
    {
        // Перевіряємо, чи існує менеджер, перш ніж його використовувати
        if (powerUpManager == null) return;

        // Уповільнюємо час, ТІЛЬКИ ЯКЩО: баф активний І павутина летить
        bool shouldBeSlowed = powerUpManager.hasFocusTime && isShooting;

        if (shouldBeSlowed && !isTimeSlowed)
        {
            // Вмикаємо slow-mo
            Time.timeScale = focusTimeScale;
            Time.fixedDeltaTime = 0.02F * Time.timeScale; // Важливо для фізики!
            isTimeSlowed = true;
        }
        else if (!shouldBeSlowed && isTimeSlowed)
        {
            // Вимикаємо slow-mo
            ResetTimeScale();
        }
    }

    /// <summary>
    /// Безпечно повертає час до нормального.
    /// </summary>
    private void ResetTimeScale()
    {
        if (isTimeSlowed)
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02F;
            isTimeSlowed = false;
        }
    }
}