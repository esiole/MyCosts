using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyCosts.Domain.Products;
using MyCosts.Domain.Receipts;

namespace MyCosts.Infrastructure.Persistence.Configurations;

internal sealed class ReceiptLineConfiguration : IEntityTypeConfiguration<ReceiptLine>
{
    public void Configure(EntityTypeBuilder<ReceiptLine> builder)
    {
        builder.ToTable("receipt_lines");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();

        builder.Property(l => l.ReceiptId).IsRequired();
        builder.Property(l => l.ProductId).IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ComplexProperty(l => l.LinePricing, pricing =>
        {
            pricing.Property(p => p.Kind)
                .HasColumnName("kind")
                .IsRequired();
            pricing.Property(p => p.Quantity)
                .HasColumnName("quantity")
                .HasPrecision(18, 3)
                .IsRequired();
            pricing.Property(p => p.UnitPrice)
                .HasColumnName("unit_price")
                .HasPrecision(18, 2)
                .IsRequired();
        });
    }
}
