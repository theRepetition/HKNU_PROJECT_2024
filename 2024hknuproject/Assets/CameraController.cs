using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 5.0f; // WASD Ű �Է¿� ���� ī�޶� �̵� �ӵ�
    public float mouseEdgeScrollSpeed = 3.0f; // ���콺 �����ڸ� �̵� �ӵ�
    public float edgeSize = 10.0f; // ȭ�� �����ڸ� ���� ũ��
    public float maxMouseEdgeDistance = 50.0f; // ���콺 �̵��� ���� �ִ� ī�޶� �̵� ����
    public Transform player; // �÷��̾� ��ġ�� �����ϱ� ���� ����

    private Vector3 offset; // �÷��̾�� ī�޶� ������ �Ÿ�
    private Vector3 initialCameraPosition; // ī�޶��� �ʱ� ��ġ

    void Start()
    {
        // �÷��̾� ������Ʈ�� ã�Ƽ� ����
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        offset = transform.position - player.position;
        initialCameraPosition = transform.position;
    }

    void Update()
    {
        // WASD Ű �Է¿� ���� ī�޶� �̵�
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0);
        transform.position += movement * speed * Time.deltaTime;

        // ���콺 ��ġ�� ���� ī�޶� �̵�
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.x > Screen.width - edgeSize && transform.position.x < initialCameraPosition.x + maxMouseEdgeDistance)
        {
            transform.position += Vector3.right * mouseEdgeScrollSpeed * Time.deltaTime;
        }
        else if (mousePosition.x < edgeSize && transform.position.x > initialCameraPosition.x - maxMouseEdgeDistance)
        {
            transform.position += Vector3.left * mouseEdgeScrollSpeed * Time.deltaTime;
        }
        if (mousePosition.y > Screen.height - edgeSize && transform.position.y < initialCameraPosition.y + maxMouseEdgeDistance)
        {
            transform.position += Vector3.up * mouseEdgeScrollSpeed * Time.deltaTime;
        }
        else if (mousePosition.y < edgeSize && transform.position.y > initialCameraPosition.y - maxMouseEdgeDistance)
        {
            transform.position += Vector3.down * mouseEdgeScrollSpeed * Time.deltaTime;
        }

        // ���콺�� ȭ�� �߾����� ���ƿ��� �÷��̾� �߽����� ī�޶� �̵�
        if (mousePosition.x >= edgeSize && mousePosition.x <= Screen.width - edgeSize &&
            mousePosition.y >= edgeSize && mousePosition.y <= Screen.height - edgeSize)
        {
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }
}
