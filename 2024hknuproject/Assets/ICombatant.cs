using UnityEngine;

public interface ICombatant
{
    void Attack(Vector2 direction);
    void NotifyProjectileDestroyed(); // �� ���� �߰��մϴ�.
    void ResetProjectilesFired();
    void StartTurn();
    void EndTurn();
    int MaxActionPoints { get; }
    int CurrentActionPoints { get; set; }
    int MaxProjectilesPerTurn { get; }
    int ProjectilesFiredThisTurn { get; set; }
    GameObject ProjectilePrefab { get; }
    float ProjectileSpeed { get; }
    int ProjectileDamage { get; }
    int ProjectilesOnField { get; set; }
}
