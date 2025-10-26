
using UnityEngine;

public class spiderMenuController : SpiderController
{
    private new void Update()
    {
        base.Update();

        
    }
    protected override void StartGrapple()
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

}
