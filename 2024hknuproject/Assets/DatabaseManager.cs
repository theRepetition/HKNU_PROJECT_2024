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

                Debug.Log("Tables created successfully.");
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
