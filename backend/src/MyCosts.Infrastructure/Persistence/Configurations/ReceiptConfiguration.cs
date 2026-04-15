using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyCosts.Domain.Receipts;
using MyCosts.Domain.Users;

namespace MyCosts.Infrastructure.Persistence.Configurations;

internal sealed class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.ToTable("receipts");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.UserId).IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(r => r.PurchaseDate).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.Shop).IsRequired().HasMaxLength(200);
        builder.Property(r => r.CurrencyCode).IsRequired().HasMaxLength(3);

        builder.HasMany(r => r.Lines)
            .WithOne()
            .HasForeignKey(l => l.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(r => r.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
