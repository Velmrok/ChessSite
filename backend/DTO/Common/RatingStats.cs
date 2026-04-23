
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.DTO.Common;

[ComplexType]
public record RatingStats(int Rapid, int Blitz, int Bullet)
{
    public RatingStats() : this(0, 0, 0) { }
};
