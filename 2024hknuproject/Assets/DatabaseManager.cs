using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;

    void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/GameDatabase.db";
        CreateDatabase();
        CreateTables();
    }

    void CreateDatabase()
    {
        if (!System.IO.File.Exists(dbPath))
        {
            SqliteConnection.CreateFile(dbPath);
            Debug.Log("Database created at " + dbPath);
        }
    }

    void CreateTables()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                // 아이템 테이블 생성
                command.CommandText = "CREATE TABLE IF NOT EXISTS Items (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Description TEXT, Value INTEGER)";
                command.ExecuteNonQuery();

                // 무기 테이블 생성
                command.CommandText = "CREATE TABLE IF NOT EXISTS Weapons (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, ProjectileSpeed REAL, Damage INTEGER, MagazineCapacity INTEGER, ProjectileShape TEXT)";
                command.ExecuteNonQuery();

                // 세이브 데이터 테이블 생성
                command.CommandText = "CREATE TABLE IF NOT EXISTS SaveData (Id INTEGER PRIMARY KEY AUTOINCREMENT, PlayerX REAL, PlayerY REAL, PlayerZ REAL, PlayerHealth INTEGER)";
                command.ExecuteNonQuery();

                // 보상 풀 테이블 생성
                command.CommandText = "CREATE TABLE IF NOT EXISTS RewardPool (Id INTEGER PRIMARY KEY AUTOINCREMENT, ItemName TEXT, ItemDescription TEXT, ItemType TEXT, ItemEffect REAL, ItemIcon TEXT, ItemRarity INTEGER)";
                command.ExecuteNonQuery();

                // NPC 테이블 생성
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS NPCs (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Dialogue TEXT,
                        PositionX REAL,
                        PositionY REAL,
                        PositionZ REAL,
                        NPCType TEXT,
                        IsInteractable INTEGER,
                        State INTEGER)";
                command.ExecuteNonQuery();

                Debug.Log("Tables created successfully.");
            }
        }
    }

    // NPC 데이터 삽입
    public void InsertNPC(string name, string dialogue, Vector3 position, string npcType, bool isInteractable, int state)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO NPCs (Name, Dialogue, PositionX, PositionY, PositionZ, NPCType, IsInteractable, State)
                    VALUES (@name, @dialogue, @x, @y, @z, @npcType, @isInteractable, @state)";
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@dialogue", dialogue);
                command.Parameters.AddWithValue("@x", position.x);
                command.Parameters.AddWithValue("@y", position.y);
                command.Parameters.AddWithValue("@z", position.z);
                command.Parameters.AddWithValue("@npcType", npcType);
                command.Parameters.AddWithValue("@isInteractable", isInteractable ? 1 : 0);
                command.Parameters.AddWithValue("@state", state);
                command.ExecuteNonQuery();
            }
        }
    }

    // NPC 데이터 조회
    public void GetNPCs()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM NPCs";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["Name"].ToString();
                        string dialogue = reader["Dialogue"].ToString();
                        Vector3 position = new Vector3(reader.GetFloat(2), reader.GetFloat(3), reader.GetFloat(4));
                        string npcType = reader["NPCType"].ToString();
                        bool isInteractable = reader.GetInt32(6) == 1;
                        int state = reader.GetInt32(7);

                        Debug.Log($"NPC Name: {name} | Dialogue: {dialogue} | Position: {position} | Type: {npcType} | Interactable: {isInteractable} | State: {state}");
                    }
                }
            }
        }
    }

    // 보상 풀 아이템 삽입
    public void InsertRewardItem(string itemName, string itemDescription, string itemType, float itemEffect, string itemIcon, int itemRarity)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO RewardPool (ItemName, ItemDescription, ItemType, ItemEffect, ItemIcon, ItemRarity) VALUES (@itemName, @itemDescription, @itemType, @itemEffect, @itemIcon, @itemRarity)";
                command.Parameters.AddWithValue("@itemName", itemName);
                command.Parameters.AddWithValue("@itemDescription", itemDescription);
                command.Parameters.AddWithValue("@itemType", itemType);
                command.Parameters.AddWithValue("@itemEffect", itemEffect);
                command.Parameters.AddWithValue("@itemIcon", itemIcon);
                command.Parameters.AddWithValue("@itemRarity", itemRarity);
                command.ExecuteNonQuery();
            }
        }
    }

    // 보상 풀 아이템 조회
    public void GetRewardItems()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM RewardPool";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("Item Name: " + reader["ItemName"] +
                                  " | Description: " + reader["ItemDescription"] +
                                  " | Type: " + reader["ItemType"] +
                                  " | Effect: " + reader["ItemEffect"] +
                                  " | Icon: " + reader["ItemIcon"] +
                                  " | Rarity: " + reader["ItemRarity"]);
                    }
                }
            }
        }
    }


    public void InsertItem(string name, string description, int value)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Items (Name, Description, Value) VALUES (@name, @description, @value)";
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@value", value);
                command.ExecuteNonQuery();
            }
        }
    }

    public void InsertWeapon(string name, float projectileSpeed, int damage, int magazineCapacity, string projectileShape)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Weapons (Name, ProjectileSpeed, Damage, MagazineCapacity, ProjectileShape) VALUES (@name, @projectileSpeed, @damage, @magazineCapacity, @projectileShape)";
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@projectileSpeed", projectileSpeed);
                command.Parameters.AddWithValue("@damage", damage);
                command.Parameters.AddWithValue("@magazineCapacity", magazineCapacity);
                command.Parameters.AddWithValue("@projectileShape", projectileShape);
                command.ExecuteNonQuery();
            }
        }
    }

    public void SaveGame(Vector3 playerPosition, int playerHealth)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO SaveData (PlayerX, PlayerY, PlayerZ, PlayerHealth) VALUES (@x, @y, @z, @health)";
                command.Parameters.AddWithValue("@x", playerPosition.x);
                command.Parameters.AddWithValue("@y", playerPosition.y);
                command.Parameters.AddWithValue("@z", playerPosition.z);
                command.Parameters.AddWithValue("@health", playerHealth);
                command.ExecuteNonQuery();
            }
        }
    }

    public void LoadGame(out Vector3 playerPosition, out int playerHealth)
    {
        playerPosition = Vector3.zero;
        playerHealth = 100;

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT PlayerX, PlayerY, PlayerZ, PlayerHealth FROM SaveData ORDER BY Id DESC LIMIT 1";

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        playerPosition = new Vector3(reader.GetFloat(0), reader.GetFloat(1), reader.GetFloat(2));
                        playerHealth = reader.GetInt32(3);
                    }
                }
            }
        }
    }

    public void GetItems()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Items";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("Id: " + reader["Id"] + " Name: " + reader["Name"] + " Description: " + reader["Description"] + " Value: " + reader["Value"]);
                    }
                }
            }
        }
    }

    public void GetWeapons()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Weapons";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("Id: " + reader["Id"] + " Name: " + reader["Name"] + " ProjectileSpeed: " + reader["ProjectileSpeed"] + " Damage: " + reader["Damage"] + " MagazineCapacity: " + reader["MagazineCapacity"] + " ProjectileShape: " + reader["ProjectileShape"]);
                    }
                }
            }
        }
    }
}
