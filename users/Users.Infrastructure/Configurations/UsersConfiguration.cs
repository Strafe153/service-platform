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
            o =>
            {
                o.Property(a => a.Country).HasMaxLength(60);
                o.Property(a => a.State).HasMaxLength(100);
                o.Property(a => a.City).HasMaxLength(100);
                o.Property(a => a.ZipCode).HasMaxLength(10);
                o.Property(a => a.Street).HasMaxLength(100);

                o.ToTable("Addresses");
            });

        builder.HasIndex(b => b.AuthProviderId)
            .HasDatabaseName("IX_Users_AuthProviderId")
            .IsClustered(false);

        builder.ToTable("Users");
    }
}