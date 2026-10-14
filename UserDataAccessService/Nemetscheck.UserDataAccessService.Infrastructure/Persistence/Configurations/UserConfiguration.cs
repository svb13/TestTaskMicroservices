using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nemetscheck.UserDataAccessService.Domain.Users;

namespace Nemetscheck.UserDataAccessService.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property(u => u.Email).HasMaxLength(User.EmailMaxLength).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FirstName).HasMaxLength(User.NameMaxLength).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(User.NameMaxLength).IsRequired();

        builder.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(u => u.ImagePath).HasMaxLength(512);
        builder.Property(u => u.CreatedAtUtc).IsRequired();
    }
}
