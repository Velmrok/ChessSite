
namespace backend.DTO.Games;

public record AIMoveRequest
(
    string Fen,
    int ThinkTimeMs
);