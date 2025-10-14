using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Infrastructure.EF.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        builder.HasKey(ii => ii.Id);

        builder.HasIndex(ii => ii.InvoiceId);

        builder.Property(ii => ii.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ii => ii.Quantity)
            .IsRequired();

        builder.Property(ii => ii.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(ii => ii.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(ii => ii.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(ii => ii.TotalAmount)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(ii => ii.Invoice)
            .WithMany(i => i.InvoiceItems)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ii => ii.OrderItem)
            .WithMany()
            .HasForeignKey(ii => ii.OrderItemId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Private setters configuration
        builder.Property(ii => ii.Id)
            .ValueGeneratedNever();
    }
}
