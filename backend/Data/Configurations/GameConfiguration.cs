

using System.Text.Json;
using backend.DTO.Games;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {

        builder.HasOne(g => g.WhitePlayer)
            .WithMany()
            .HasForeignKey(g => g.WhitePlayerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.BlackPlayer)
            .WithMany()
            .HasForeignKey(g => g.BlackPlayerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Winner)
            .WithMany()
            .HasForeignKey(g => g.WinnerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder
        .Property(g => g.Moves)
        .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
            v => JsonSerializer.Deserialize<List<MoveInfo>>(v, (JsonSerializerOptions)null!) ?? new List<MoveInfo>()
        )
        .Metadata.SetValueComparer(new ValueComparer<List<MoveInfo>>(
            (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions)null!) == JsonSerializer.Serialize(b, (JsonSerializerOptions)null!),
            c => c.Aggregate(0, (h, v) => HashCode.Combine(h, v.GetHashCode())),
            c => c.ToList()
        ));
    }
}