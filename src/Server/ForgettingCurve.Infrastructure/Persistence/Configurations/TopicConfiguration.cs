using ForgettingCurve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgettingCurve.Infrastructure.Persistence.Configurations;

public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable("Topics");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
            .HasColumnName("TopicId")
            .ValueGeneratedOnAdd();
            
        builder.Property(t => t.ScopeId)
            .IsRequired();
            
        builder.Property(t => t.OwnerUserId)
            .IsRequired();
            
        builder.Property(t => t.Name)
            .HasMaxLength(400)
            .IsRequired();
            
        builder.Property(t => t.StartDate)
            .IsRequired();
            
        builder.Property(t => t.Notes)
            .HasMaxLength(3000)
            .IsRequired(false);
            
        builder.Property(t => t.IsMastered)
            .HasDefaultValue(false)
            .IsRequired();
            
        builder.Property(t => t.CreationDate)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
            
        builder.Property(t => t.LastModifiedDate)
            .IsRequired(false);
            
        builder.HasOne(t => t.Scope)
            .WithMany(s => s.Topics)
            .HasForeignKey(t => t.ScopeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(t => t.ScopeId);
        builder.HasIndex(t => t.OwnerUserId);
        builder.HasIndex(t => t.IsMastered);
    }
} 