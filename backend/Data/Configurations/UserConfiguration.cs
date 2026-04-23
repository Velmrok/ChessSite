using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.Nickname).IsUnique();
        builder.HasIndex(u => u.Login).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

    }
}