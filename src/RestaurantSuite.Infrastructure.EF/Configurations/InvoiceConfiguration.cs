using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Infrastructure.EF.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        builder.HasIndex(i => i.OrderId);

        builder.HasIndex(i => i.Status);

        builder.HasIndex(i => new { i.CreatedAt, i.Status });

        builder.Property(i => i.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(i => i.CustomerName)
            .HasMaxLength(200);

        builder.Property(i => i.CustomerEmail)
            .HasMaxLength(200);

        builder.Property(i => i.CustomerPhone)
            .HasMaxLength(50);

        builder.Property(i => i.BillingAddress)
            .HasMaxLength(500);

        builder.Property(i => i.Notes)
            .HasMaxLength(1000);

        builder.Property(i => i.DiscountReason)
            .HasMaxLength(500);

        builder.Property(i => i.SubtotalAmount)
            .HasPrecision(18, 2);

        builder.Property(i => i.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(i => i.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(i => i.TipAmount)
            .HasPrecision(18, 2);

        builder.Property(i => i.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(i => i.TaxRate)
            .HasPrecision(5, 4);

        // Relationships
        builder.HasOne(i => i.Order)
            .WithMany()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Payment)
            .WithMany()
            .HasForeignKey(i => i.PaymentId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasMany(i => i.InvoiceItems)
            .WithOne(ii => ii.Invoice)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Private setters configuration
        builder.Property(i => i.Id)
            .ValueGeneratedNever();
    }
}
