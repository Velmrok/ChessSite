
using System.ComponentModel.DataAnnotations;
using backend.Enums;
using ErrorOr;

namespace backend.DTO.Games
{
    public record GetGamesQuery(
    [Required(ErrorMessage = "requiredPageNumber")]
    int PageNumber = 1,

    [Required(ErrorMessage = "requiredLimit"), Range(1, 100, ErrorMessage = "limitRange")]
    int Limit = 10,
    [MaxLength(50, ErrorMessage = "queryMaxLength")]
    string? Query = null,
    GameType? GameType = null,
    GamesSortBy? SortBy = null,
    bool SortDescending = false,
    GameStatus? Status = null


    );
}

