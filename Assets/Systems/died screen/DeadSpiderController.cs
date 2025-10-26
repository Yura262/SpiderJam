using UnityEngine;

public class DeadSpiderController : MonoBehaviour
{
    // --- Налаштування в інспекторі ---
    [Header("Grappling Hook")]
    // --- ЗМІНЕНО ---: Перейменовано для ясності
    [SerializeField] protected float pullSpeed = 15f; // Сила, з якою павук притягується
    // --- НОВЕ ---: Швидкість польоту самої павутини
    [SerializeField] protected float hookSpeed = 25f;
    [SerializeField] protected LayerMask grappleLayer; // Шар, за який можна чіплятися

    // --- Внутрішні змінні ---
    public Rigidbody2D rb { get; private set; } //also used for animation (velocity)
    protected LineRenderer lineRenderer;
    protected Camera mainCamera;


    protected Vector2 grapplePoint;
    public bool IsGrappling { get; private set; } = false; //also used for animation

    // --- НОВЕ ---: Нові стани для логіки польоту
    protected bool isShooting = false;
    protected Vector2 hookPosition; // Поточна позиція кінчика павутини під час польоту

    protected float initialGravityScale;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
        initialGravityScale = rb.gravityScale;
    }

    protected void Update()
    {


        // --- Логіка павутини ---
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        // --- ЗМІНЕНО ---: Тепер цей метод керує і польотом, і малюванням
        UpdateWeb();
    }

    void FixedUpdate()
    {
        // Вона застосовує силу тільки тоді, коли павук ВЖЕ причепився
        if (IsGrappling)
        {
            Vector2 direction = (grapplePoint - (Vector2)transform.position).normalized;
            // --- ЗМІНЕНО ---: Використовуємо нову назву змінної
            rb.AddForce(direction * pullSpeed);
        }
    }
    protected void StartGrapple()
    {
        // --- НОВЕ ---: Не даємо стріляти, якщо павутина вже летить або причеплена
        if (isShooting || IsGrappling) return;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;


        grapplePoint = mousePosition;
        // --- НОВЕ ---: Встановлюємо початкову позицію павутини
        hookPosition = transform.position;
        isShooting = true; // Вмикаємо стан "пострілу"
        lineRenderer.positionCount = 2;

        // --- ЗМІНЕНО ---: Ми більше НЕ чіпляємось і не вимикаємо гравітацію МИТТЄВО.
        // IsGrappling = true; // (Видалено)
        // rb.gravityScale = 0f; // (Видалено)
    }
    

    private void StopGrapple()
    {
        IsGrappling = false;
        isShooting = false; // --- НОВЕ ---: Також припиняємо політ павутини
        rb.gravityScale = initialGravityScale;
        lineRenderer.positionCount = 0;
    }

    // --- ЗМІНЕНО ---: Повністю переписаний метод (раніше був DrawWeb)
    private void UpdateWeb()
    {
        // Логіка, коли павутина летить до цілі
        if (isShooting)
        {
            // Рухаємо кінчик павутини до цілі
            hookPosition = Vector2.MoveTowards(hookPosition, grapplePoint, hookSpeed * Time.deltaTime);

            // Малюємо лінію від павука до кінчика павутини
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hookPosition);

            // Перевіряємо, чи павутина долетіла
            if (hookPosition == grapplePoint)
            {
                isShooting = false; // Вимикаємо "постріл"
                IsGrappling = true; // Вмикаємо "притягання"
                rb.gravityScale = 0f; // Вимикаємо гравітацію ТІЛЬКИ ЗАРАЗ
            }
        }
        // Логіка, коли павутина вже причеплена і притягує
        else if (IsGrappling)
        {
            // Просто оновлюємо лінію, щоб вона слідувала за павуком
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }
}