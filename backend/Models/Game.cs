using backend.DTO.Common;
using backend.Enums;


namespace backend.Models;
public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid WhitePlayerId { get; set; }
    public Guid BlackPlayerId { get; set; }
    public Guid? WinnerId { get; set; } = null;

    public User WhitePlayer { get; set; } = null!;
    public User BlackPlayer { get; set; } = null!;
    public User? Winner { get; set; } = null;


    public GameType Type { get; set; } = GameType.Rapid;
    public GameStatus Status { get; set; } = GameStatus.Active;
    public DateTime? FinishedAt { get; set; } = null;
    public string Moves { get; set; } = "";
    public int Time { get; set; } = 10;
    public int Increment { get; set; } = 0;
}