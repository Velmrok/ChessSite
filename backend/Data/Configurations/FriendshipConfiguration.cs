using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations;
public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(f => new { f.UserId, f.FriendId });
        builder.HasOne(f => f.User)
               .WithMany()
               .HasForeignKey(f => f.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Friend)
               .WithMany()
               .HasForeignKey(f => f.FriendId)
                .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        
    }
}