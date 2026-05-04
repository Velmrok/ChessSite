namespace backend.DTO.Games
{
    public record GameEndedResponse(
        string GameId,
        string? Winner,
        string Reason
    );
}