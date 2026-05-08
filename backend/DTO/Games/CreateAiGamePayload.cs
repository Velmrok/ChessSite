
using System.ComponentModel.DataAnnotations;

namespace backend.DTO.Games;

public record CreateAiGamePayload
(
    int Time,
    int Increment,
    [Range(1, 10)]
    int Difficulty
);