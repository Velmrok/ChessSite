using System.ComponentModel.DataAnnotations;

public record GetUsersQuery(
    [Range(1, int.MaxValue)]
    int Page = 1,

    [Range(1, 100)]
    int Limit = 10,

    [StringLength(100, MinimumLength = 0)]
    string Search = "",

    UsersSortBy SortBy = UsersSortBy.CreatedAt,
    
    bool SortDescending = true
);