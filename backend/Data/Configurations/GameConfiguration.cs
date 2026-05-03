

using backend.Models;
using Microsoft.EntityFrameworkCore;
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
    }
}