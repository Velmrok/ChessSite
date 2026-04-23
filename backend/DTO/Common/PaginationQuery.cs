
using System.ComponentModel.DataAnnotations;

public record PaginationQuery(
        [Range(1, int.MaxValue)]
    int PageNumber = 1,
    [Range(1, 100)]
    int Limit = 10
);