public interface ITurnTaker
{
    void StartTurn();
    void EndTurn();
    bool IsTurnComplete { get; }
    string Name { get; }
}