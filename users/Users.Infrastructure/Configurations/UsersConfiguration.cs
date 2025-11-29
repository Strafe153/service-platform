using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Aggregates.User;

namespace Users.Infrastructure.Configurations;

public class UsersConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).HasMaxLength(26);

        builder.Property(b => b.Id).HasConversion(
            u => u.ToString(),
            s => Ulid.Parse(s));

        builder.Property(b => b.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.FirstName)
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(b => b.LastName)
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(b => b.PhoneNumber)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(b => b.BirthDate).IsRequired();

        builder.Property(b => b.AuthProviderId)
            .HasMaxLength(36)
            .IsRequired();

        builder.OwnsOne(
            b => b.Address,
            o => o.ToTable("Addresses"));

        builder.ToTable("Users");
    }
}