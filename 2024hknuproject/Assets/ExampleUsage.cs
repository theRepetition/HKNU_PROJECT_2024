using UnityEngine;

public class ExampleUsage : MonoBehaviour
{
    private DatabaseManager dbManager;
    private Vector3 playerPosition;
    private int playerHealth;

    void Start()
    {
        dbManager = FindObjectOfType<DatabaseManager>();

        // 아이템 데이터 삽입
        dbManager.InsertItem("Potion", "Restores health.", 50);
        dbManager.InsertItem("Elixir", "Restores all stats.", 100);

        // 무기 데이터 삽입
        dbManager.InsertWeapon("Pistol", 500.0f, 25, 15, "Bullet");
        dbManager.InsertWeapon("Rifle", 800.0f, 40, 30, "Bullet");

        // 데이터 조회
        dbManager.GetItems();
        dbManager.GetWeapons();

        // 세이브 데이터 저장
        playerPosition = new Vector3(10.0f, 0.0f, 5.0f);
        playerHealth = 80;
        dbManager.SaveGame(playerPosition, playerHealth);

        // 세이브 데이터 로드
        dbManager.LoadGame(out playerPosition, out playerHealth);
        Debug.Log("Loaded Player Position: " + playerPosition);
        Debug.Log("Loaded Player Health: " + playerHealth);
    }
}
