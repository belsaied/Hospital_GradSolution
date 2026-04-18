using Domain.Models.BillingModule;
using Domain.Models.Enums.BillingEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations.BillingConfigs
{
    public class InsuranceClaimConfiguration : IEntityTypeConfiguration<InsuranceClaim>
    {
        public void Configure(EntityTypeBuilder<InsuranceClaim> builder)
        {
            builder.HasKey(c => c.Id);

            // One claim per invoice
            builder.HasIndex(c => c.InvoiceId).IsUnique();

            builder.Property(c => c.ClaimStatus)
                .HasConversion(
                    s => s.ToString(),
                    s => Enum.Parse<ClaimStatus>(s))
                .HasMaxLength(25);

            builder.Property(c => c.InsuranceProvider).IsRequired().HasMaxLength(200);
            builder.Property(c => c.PolicyNumber).IsRequired().HasMaxLength(100);
            builder.Property(c => c.MembershipNumber).IsRequired().HasMaxLength(100);
            builder.Property(c => c.ExternalClaimReference).HasMaxLength(200);
            builder.Property(c => c.Notes).HasMaxLength(1000);
            builder.Property(c => c.RejectionReason).HasMaxLength(1000);

            builder.Property(c => c.ClaimedAmount).HasColumnType("decimal(18,4)");
            builder.Property(c => c.ApprovedAmount).HasColumnType("decimal(18,4)");
            

            builder.ToTable("InsuranceClaims");
        }
    }
}
