namespace backend.DTO.Games;
public record MakeMoveRequest(
    string GameId,
    string San
);