
using UnityEngine;

public class spiderMenuController : SpiderController
{
    private new void Update()
    {
        base.Update();

        
    }
    protected override void StartGrapple()
    {
        // --- ���� ---: �� ���� �������, ���� �������� ��� ������ ��� ����������
        if (isShooting || IsGrappling) return;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;


        grapplePoint = mousePosition;
        // --- ���� ---: ������������ ��������� ������� ��������
        hookPosition = transform.position;
        isShooting = true; // ������� ���� "�������"
        lineRenderer.positionCount = 2;

        // --- �̲���� ---: �� ����� �� ���������� � �� �������� ��������� ���Ҫ��.
        // IsGrappling = true; // (��������)
        // rb.gravityScale = 0f; // (��������)
    }

}
