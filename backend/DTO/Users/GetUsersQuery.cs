using System.ComponentModel.DataAnnotations;
using backend.Enums;
namespace backend.DTO.Users;
public record GetUsersQuery(
    [Range(1, int.MaxValue)]
    int Page = 1,

    [Range(1, 100)]
    int Limit = 10,

    [StringLength(100, MinimumLength = 0)]
    string Search = "",

    UsersSortBy SortBy = UsersSortBy.CreatedAt,

    bool SortDescending = true,

    [Range(0, 3000)]
    int MinRating = 0,

    [Range(0, 3000)]
    int MaxRating = 3000,

    bool JustOnline = false,

    RatingType RatingType = RatingType.Rapid


);