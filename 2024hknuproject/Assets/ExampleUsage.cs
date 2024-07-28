using UnityEngine;

public class ExampleUsage : MonoBehaviour
{
    private DatabaseManager dbManager;
    private Vector3 playerPosition;
    private int playerHealth;

    void Start()
    {
        dbManager = FindObjectOfType<DatabaseManager>();

        // ������ ������ ����
        dbManager.InsertItem("Potion", "Restores health.", 50);
        dbManager.InsertItem("Elixir", "Restores all stats.", 100);

        // ���� ������ ����
        dbManager.InsertWeapon("Pistol", 500.0f, 25, 15, "Bullet");
        dbManager.InsertWeapon("Rifle", 800.0f, 40, 30, "Bullet");

        // ������ ��ȸ
        dbManager.GetItems();
        dbManager.GetWeapons();

        // ���̺� ������ ����
        playerPosition = new Vector3(10.0f, 0.0f, 5.0f);
        playerHealth = 80;
        dbManager.SaveGame(playerPosition, playerHealth);

        // ���̺� ������ �ε�
        dbManager.LoadGame(out playerPosition, out playerHealth);
        Debug.Log("Loaded Player Position: " + playerPosition);
        Debug.Log("Loaded Player Health: " + playerHealth);
    }
}
